using HealthZone.Data;
using HealthZone.Models;
using HealthZone.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthZone.Controllers
{
    [Authorize] // neprijavljeni nemaju pristup ničemu u ovom controlleru
    public class TerminController : Controller
    {
        private readonly ITerminService _terminService;
        private readonly IKorisnikService _korisnikService;
        private readonly IMedicinskaUslugaService _uslugaService;
        private readonly UserManager<Korisnik> _userManager;
        private readonly IKorisnikNaListiService _korisnikNaListiService;
        private readonly IListaCekanjaService _listaCekanjaService;

        public TerminController(
            ITerminService terminService,
            IKorisnikService korisnikService,
            IMedicinskaUslugaService uslugaService,
            UserManager<Korisnik> userManager,
            IKorisnikNaListiService korisnikNaListiService,
            IListaCekanjaService listaCekanjaService)
        {
            _terminService = terminService;
            _korisnikService = korisnikService;
            _uslugaService = uslugaService;
            _userManager = userManager;
            _korisnikNaListiService = korisnikNaListiService;
            _listaCekanjaService = listaCekanjaService;
        }

        // GET: Termin
        public async Task<IActionResult> Index()
        {
            var termini = await _terminService.GetAllAsync();

            var korisnik = await _userManager.GetUserAsync(User);

            if (User.IsInRole("Doktor"))
            {
                termini = termini
                    .Where(t => t.DoktorID == korisnik!.Id)
                    .ToList();
            }
            else if (User.IsInRole("Pacijent"))
            {
                termini = termini
                    .Where(t => t.PacijentID == korisnik!.Id)
                    .ToList();
            }

            return View(termini);
        }

        // GET: Termin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var termin = await _terminService.GetByIdAsync(id.Value);

            if (termin == null)
                return NotFound();

            var korisnik = await _userManager.GetUserAsync(User);

            if (User.IsInRole("Doktor") && termin.DoktorID != korisnik!.Id)
                return Forbid();

            if (User.IsInRole("Pacijent") && termin.PacijentID != korisnik!.Id)
                return Forbid();

            // Dohvati dodatne podatke za prikaz umjesto ID-a
            var doktor = await _korisnikService.GetByIdAsync(termin.DoktorID);
            var pacijent = await _korisnikService.GetByIdAsync(termin.PacijentID);
            var usluga = await _uslugaService.GetByIdAsync(termin.UslugaID);

            ViewBag.DoktorIme = doktor != null ? $"{doktor.Ime} {doktor.Prezime}" : "Nepoznat";
            ViewBag.PacijentIme = pacijent != null ? $"{pacijent.Ime} {pacijent.Prezime}" : "Nepoznat";
            ViewBag.UslugaNaziv = usluga != null ? usluga.Naziv : "Nepoznat";

            return View(termin);
        }

        // GET: Termin/Create
        [Authorize(Roles = "Pacijent,Doktor,Administrator")]
        public async Task<IActionResult> Create()
        {
            await PopuniDropDownListePoUlozi();
            if (User.IsInRole("Doktor"))
            {
                var doktor = await _userManager.GetUserAsync(User);
                ViewBag.UlogovanDoktorId = doktor?.Id ?? "";
            }

            return View();
        }

        // POST: Termin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Pacijent,Doktor,Administrator")]
        public async Task<IActionResult> Create(
            [Bind("TerminId,Datum,Vrijeme,DoktorID,PacijentID,UslugaID")] Termin termin,
            bool dodajNaListu = false)
        {
            var trenutniKorisnik = await _userManager.GetUserAsync(User);

            ModelState.Remove("PacijentID");
            ModelState.Remove("DoktorID");
            ModelState.Remove("Status");
            ModelState.Remove("Pacijent");
            ModelState.Remove("Doktor");

            if (User.IsInRole("Pacijent"))
            {
                termin.PacijentID = trenutniKorisnik!.Id;
                termin.Status = Status.NaCekanju;
            }
            else if (User.IsInRole("Doktor"))
            {
                termin.DoktorID = trenutniKorisnik!.Id;
                termin.Status = Status.NaCekanju;
            }
            else if (User.IsInRole("Administrator"))
            {
                if (termin.Status == default)
                    termin.Status = Status.NaCekanju;
            }

            // Ako pacijent želi na listu čekanja umjesto termina
            if (dodajNaListu && User.IsInRole("Pacijent") && !string.IsNullOrEmpty(termin.DoktorID))
            {
                try
                {
                    var lista = await _listaCekanjaService.GetListaZaDoktoraAsync(termin.DoktorID);
                    if (lista != null)
                    {
                        var stavka = new KorisnikNaListi
                        {
                            ListaID = lista.ListaId,
                            KorisnikID = trenutniKorisnik!.Id,
                            Datum = DateTime.Now
                        };
                        await _korisnikNaListiService.AddAsync(stavka);
                        TempData["Success"] = "Uspješno ste dodani na listu čekanja. Bićete kontaktirani kada se oslobodi termin.";
                    }
                    else
                    {
                        TempData["Info"] = "Ovaj doktor trenutno nema aktivnu listu čekanja.";
                    }
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Info"] = ex.Message;
                    return RedirectToAction(nameof(Index));
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _terminService.ZakaziTerminAsync(termin);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Vrijeme", ex.Message);
                }
            }

            await PopuniDropDownListePoUlozi(termin.DoktorID, termin.UslugaID);
            return View(termin);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> OtkaziIzMaila(int id)
        {
            try
            {
                await _terminService.OtkaziTerminAsync(id);
                return View("TerminOtkazan");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Otkazi(int id)
        {
            var termin = await _terminService.GetByIdAsync(id);

            if (termin == null)
                return NotFound();

            var korisnik = await _userManager.GetUserAsync(User);

            if (User.IsInRole("Doktor") && termin.DoktorID != korisnik!.Id)
                return Forbid();

            if (User.IsInRole("Pacijent") && termin.PacijentID != korisnik!.Id)
                return Forbid();

            try
            {
                await _terminService.OtkaziTerminAsync(id);
            }
            catch (Exception ex)
            {
                TempData["Greška"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Doktor,Administrator,Pacijent")]
        public async Task<IActionResult> OznaciAktivan(int id)
        {
            await _terminService.OznaciKaoPotvrdjenAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: Termin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var termin = await _terminService.GetByIdAsync(id.Value);

            if (termin == null)
                return NotFound();

            var korisnik = await _userManager.GetUserAsync(User);

            if (User.IsInRole("Doktor") && termin.DoktorID != korisnik!.Id)
                return Forbid();

            if (User.IsInRole("Pacijent") && termin.PacijentID != korisnik!.Id)
                return Forbid();

            await PopuniDropDownListe(termin.DoktorID, termin.UslugaID);

            return View(termin);
        }

        // POST: Termin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("TerminId,Datum,Vrijeme,Status,DoktorID,PacijentID,UslugaID")] Termin termin)
        {
            if (id != termin.TerminId)
                return NotFound();

            var postojeciTermin = await _terminService.GetByIdAsync(id);

            if (postojeciTermin == null)
                return NotFound();

            var korisnik = await _userManager.GetUserAsync(User);

            if (User.IsInRole("Doktor") &&
                postojeciTermin.DoktorID != korisnik!.Id)
                return Forbid();

            if (User.IsInRole("Pacijent") &&
                postojeciTermin.PacijentID != korisnik!.Id)
                return Forbid();

            if (ModelState.IsValid)
            {
                try
                {
                    // Status se ne mijenja kroz Edit — čuva se postojeći iz baze
                    postojeciTermin.Datum = termin.Datum;
                    postojeciTermin.Vrijeme = termin.Vrijeme;
                    postojeciTermin.DoktorID = termin.DoktorID;
                    postojeciTermin.PacijentID = termin.PacijentID;
                    postojeciTermin.UslugaID = termin.UslugaID;

                    _terminService.Update(postojeciTermin);
                    await _terminService.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await TerminExists(termin.TerminId))
                        return NotFound();

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            await PopuniDropDownListe(termin.DoktorID, termin.UslugaID);

            return View(termin);
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> PotvrdiIzMaila(int id)
        {
            try
            {
                await _terminService.OznaciKaoPotvrdjenAsync(id);
                return View("TerminPotvrdjen");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }


        // GET: Termin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var termin = await _terminService.GetByIdAsync(id.Value);

            if (termin == null)
                return NotFound();

            var korisnik = await _userManager.GetUserAsync(User);

            if (User.IsInRole("Doktor") && termin.DoktorID != korisnik!.Id)
                return Forbid();

            if (User.IsInRole("Pacijent") && termin.PacijentID != korisnik!.Id)
                return Forbid();

            return View(termin);
        }

        // POST: Termin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var termin = await _terminService.GetByIdAsync(id);

            if (termin == null)
                return NotFound();

            var korisnik = await _userManager.GetUserAsync(User);

            if (User.IsInRole("Doktor") && termin.DoktorID != korisnik!.Id)
                return Forbid();

            if (User.IsInRole("Pacijent") && termin.PacijentID != korisnik!.Id)
                return Forbid();

            _terminService.Delete(termin);
            await _terminService.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /Termin/GetZauzetiSlotovi?datum=2026-06-17&doktorId=abc123
        [AllowAnonymous]
        public async Task<IActionResult> GetZauzetiSlotovi(string datum, string doktorId)
        {
            if (!DateTime.TryParse(datum, out var parsedDatum) || string.IsNullOrEmpty(doktorId))
                return Json(new { zauzetiSlotovi = new List<string>() });

            var svi = await _terminService.GetAllAsync();
            var zauzetiSlotovi = svi
                .Where(t =>
                    t.DoktorID == doktorId &&
                    t.Datum.Date == parsedDatum.Date &&
                    t.Status != Status.Otkazan)
                .Select(t => t.Vrijeme.ToString("HH:mm"))
                .ToList();

            return Json(new { zauzetiSlotovi });
        }

        // GET: /Termin/GetZauzetiDani?doktorId=abc123
        [AllowAnonymous]
        public async Task<IActionResult> GetZauzetiDani(string doktorId)
        {
            if (string.IsNullOrEmpty(doktorId))
                return Json(new { zauzetiDani = new List<string>() });

            int ukupnoSlotova = 7; // 08:00 - 14:00

            var svi = await _terminService.GetAllAsync();
            var zauzetiDani = svi
                .Where(t =>
                    t.DoktorID == doktorId &&
                    t.Datum.Date >= DateTime.Today &&
                    t.Status != Status.Otkazan)
                .GroupBy(t => t.Datum.Date)
                .Where(g => g.Count() >= ukupnoSlotova)
                .Select(g => g.Key.ToString("yyyy-MM-dd"))
                .ToList();

            return Json(new { zauzetiDani });
        }

        // GET: /Termin/GetMjesecZauzet?doktorId=abc123&godina=2026&mjesec=6
        [AllowAnonymous]
        public async Task<IActionResult> GetMjesecZauzet(string doktorId, int godina, int mjesec)
        {
            if (string.IsNullOrEmpty(doktorId))
                return Json(new { zauzet = false });

            int ukupnoSlotova = 7; // mora biti isto kao sviSlotovi.length u JS-u

            // Broj radnih dana u tom mjesecu (pon–pet)
            int radniDaniUMjesecu = 0;
            var pocetak = new DateTime(godina, mjesec, 1);
            var kraj = pocetak.AddMonths(1).AddDays(-1);

            for (var d = pocetak; d <= kraj; d = d.AddDays(1))
                if (d.DayOfWeek != DayOfWeek.Saturday && d.DayOfWeek != DayOfWeek.Sunday)
                    radniDaniUMjesecu++;

            var svi = await _terminService.GetAllAsync();
            var zauzetiDaniCount = svi
                .Where(t =>
                    t.DoktorID == doktorId &&
                    t.Datum.Year == godina &&
                    t.Datum.Month == mjesec &&
                    t.Status != Status.Otkazan)
                .GroupBy(t => t.Datum.Date)
                .Count(g => g.Count() >= ukupnoSlotova);

            // Smatra se da je mjesec zauzet ako su svi radni dani popunjeni
            bool zauzet = zauzetiDaniCount >= radniDaniUMjesecu;
            return Json(new { zauzet });
        }

        private async Task<bool> TerminExists(int id)
        {
            var termin = await _terminService.GetByIdAsync(id);
            return termin != null;
        }

        // Stara metoda za Edit
        private async Task PopuniDropDownListe(string? selectedDoktorId = null, int? selectedUslugaId = null)
        {
            var doktori = await _korisnikService.GetDoktoriAsync();
            var pacijenti = await _korisnikService.GetPacijentiAsync();
            var usluge = await _uslugaService.GetAllAsync();

            ViewData["DoktorID"] = new SelectList(
                doktori.Select(d => new { d.Id, PunoIme = $"{d.Ime} {d.Prezime}" }),
                "Id", "PunoIme", selectedDoktorId);

            ViewData["PacijentID"] = new SelectList(
                pacijenti.Select(p => new { p.Id, PunoIme = $"{p.Ime} {p.Prezime}" }),
                "Id", "PunoIme");

            ViewData["UslugaID"] = new SelectList(usluge, "UslugaId", "Naziv", selectedUslugaId);
        }

        // Nova metoda po ulozi
        private async Task PopuniDropDownListePoUlozi(string? selectedDoktorId = null, int? selectedUslugaId = null)
        {
            var usluge = await _uslugaService.GetAllAsync();
            ViewData["UslugaID"] = new SelectList(usluge, "UslugaId", "Naziv", selectedUslugaId);

            if (User.IsInRole("Pacijent"))
            {
                var doktori = await _korisnikService.GetDoktoriAsync();
                ViewData["DoktorID"] = new SelectList(
                    doktori.Select(d => new { d.Id, PunoIme = $"{d.Ime} {d.Prezime}" }),
                    "Id", "PunoIme", selectedDoktorId);
            }
            else if (User.IsInRole("Doktor"))
            {
                var pacijenti = await _korisnikService.GetPacijentiAsync();
                ViewData["PacijentID"] = new SelectList(
                    pacijenti.Select(p => new { p.Id, PunoIme = $"{p.Ime} {p.Prezime}" }),
                    "Id", "PunoIme");
            }
            else if (User.IsInRole("Administrator"))
            {
                var doktori = await _korisnikService.GetDoktoriAsync();
                var pacijenti = await _korisnikService.GetPacijentiAsync();
                ViewData["DoktorID"] = new SelectList(
                    doktori.Select(d => new { d.Id, PunoIme = $"{d.Ime} {d.Prezime}" }),
                    "Id", "PunoIme", selectedDoktorId);
                ViewData["PacijentID"] = new SelectList(
                    pacijenti.Select(p => new { p.Id, PunoIme = $"{p.Ime} {p.Prezime}" }),
                    "Id", "PunoIme");
            }
        }

        // GET: Termin/PromijeniPrioritet/{pacijentId}
        [Authorize(Roles = "Doktor")]
        public async Task<IActionResult> PromijeniPrioritet(string pacijentId)
        {
            if (string.IsNullOrEmpty(pacijentId))
                return NotFound();

            var doktor = await _userManager.GetUserAsync(User);
            if (doktor == null)
                return Unauthorized();

            var pacijent = await _korisnikService.GetByIdAsync(pacijentId);
            if (pacijent == null)
                return NotFound();

            // Provjeri da li je ovaj pacijent doktorov (ima termin kod njega)
            var termini = await _terminService.GetAllAsync();
            bool jeMojPacijent = termini.Any(t => t.DoktorID == doktor.Id &&
                                                  t.PacijentID == pacijentId &&
                                                  t.Status != Status.Otkazan);

            if (!jeMojPacijent && !User.IsInRole("Administrator"))
            {
                TempData["Greska"] = "Možete mijenjati prioritet samo svojim pacijentima!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.PacijentIme = $"{pacijent.Ime} {pacijent.Prezime}";
            ViewBag.TrenutniPrioritet = pacijent.Prioritet?.ToString() ?? "Nije postavljen";

            var prioriteti = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "-- Bez prioriteta --" },
                new SelectListItem { Value = "Nizak", Text = "Nizak" },
                new SelectListItem { Value = "Srednji", Text = "Srednji" },
                new SelectListItem { Value = "Visok", Text = "Visok" }
            };
            ViewBag.Prioriteti = prioriteti;

            return View(pacijent);
        }

        // POST: Termin/PromijeniPrioritet
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Doktor")]
        public async Task<IActionResult> PromijeniPrioritet(string pacijentId, string prioritet)
        {
            var doktor = await _userManager.GetUserAsync(User);
            if (doktor == null)
                return Unauthorized();

            var pacijent = await _korisnikService.GetByIdAsync(pacijentId);
            if (pacijent == null)
                return NotFound();

            // Provjeri da li je ovaj pacijent doktorov (ima termin kod njega)
            var termini = await _terminService.GetAllAsync();
            bool jeMojPacijent = termini.Any(t => t.DoktorID == doktor.Id &&
                                                  t.PacijentID == pacijentId &&
                                                  t.Status != Status.Otkazan);

            if (!jeMojPacijent && !User.IsInRole("Administrator"))
            {
                TempData["Greska"] = "Možete mijenjati prioritet samo svojim pacijentima!";
                return RedirectToAction(nameof(Index));
            }

            // Postavi novi prioritet
            if (string.IsNullOrEmpty(prioritet))
            {
                pacijent.Prioritet = null;
            }
            else
            {
                if (Enum.TryParse<Prioritet>(prioritet, out var noviPrioritet))
                {
                    pacijent.Prioritet = noviPrioritet;
                }
                else
                {
                    TempData["Greska"] = "Neispravna vrijednost prioriteta.";
                    return RedirectToAction(nameof(Index));
                }
            }

            await _korisnikService.AzurirajAsync(pacijent);
            TempData["Success"] = $"Prioritet za pacijenta {pacijent.Ime} {pacijent.Prezime} je promijenjen na {(prioritet == "" ? "nije postavljen" : prioritet)}.";

            return RedirectToAction(nameof(MojiPacijenti));
        }

        // GET: Termin/MojiPacijenti
        [Authorize(Roles = "Doktor")]
        public async Task<IActionResult> MojiPacijenti()
        {
            var doktor = await _userManager.GetUserAsync(User);
            if (doktor == null)
                return Unauthorized();

            var sviTermini = await _terminService.GetAllAsync();

            // Dohvati sve pacijente koji imaju termin kod ovog doktora
            var mojiPacijentiIds = sviTermini
                .Where(t => t.DoktorID == doktor.Id && t.Status != Status.Otkazan)
                .Select(t => t.PacijentID)
                .Distinct()
                .ToList();

            var mojiPacijenti = new List<Korisnik>();
            foreach (var id in mojiPacijentiIds)
            {
                var pacijent = await _korisnikService.GetByIdAsync(id);
                if (pacijent != null)
                {
                    // Dodaj broj termina za ovog pacijenta
                    var brojTermina = sviTermini.Count(t => t.PacijentID == id && t.Status != Status.Otkazan);
                    ViewData[$"BrojTermina_{id}"] = brojTermina;
                    mojiPacijenti.Add(pacijent);
                }
            }

            return View(mojiPacijenti);
        }
    }
}