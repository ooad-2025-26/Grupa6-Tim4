using HealthZone.Models;
using HealthZone.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HealthZone.Controllers
{
    [Authorize(Roles = "Administrator,Doktor")]
    public class KorisnikNaListiController : Controller
    {
        private readonly IKorisnikNaListiService _korisnikNaListiService;
        private readonly IListaCekanjaService _listaCekanjaService;
        private readonly UserManager<Korisnik> _userManager;

        public KorisnikNaListiController(
            IKorisnikNaListiService korisnikNaListiService,
            IListaCekanjaService listaCekanjaService,
            UserManager<Korisnik> userManager)
        {
            _korisnikNaListiService = korisnikNaListiService;
            _listaCekanjaService = listaCekanjaService;
            _userManager = userManager;
        }

        // GET: KorisnikNaListi
        public async Task<IActionResult> Index()
        {
            var sve = await _korisnikNaListiService.GetAllAsync();

            if (User.IsInRole("Administrator"))
                return View(sve);

            // Doktor vidi samo stavke sa svojih lista
            var trenutniKorisnikId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var stavke = sve.Where(k => k.Lista?.DoktorID == trenutniKorisnikId);
            return View(stavke);
        }

        // GET: KorisnikNaListi/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var stavka = await _korisnikNaListiService.GetByIdAsync(id.Value);
            if (stavka == null) return NotFound();

            if (!User.IsInRole("Administrator"))
            {
                var trenutniKorisnikId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (stavka.Lista?.DoktorID != trenutniKorisnikId)
                    return Forbid();
            }

            return View(stavka);
        }

        // GET: KorisnikNaListi/Create
        // GET: KorisnikNaListi/Create
        public async Task<IActionResult> Create()
        {
            var korisnik = await _userManager.GetUserAsync(User);
            if (korisnik == null) return Unauthorized();

            if (User.IsInRole("Doktor"))
            {
                var lista = await _listaCekanjaService.GetListaZaDoktoraAsync(korisnik.Id);
                if (lista == null)
                {
                    TempData["Error"] = "Nemate kreiranu listu èekanja.";
                    return RedirectToAction("Index", "ListaCekanja");
                }

                // Lista je fiksna — prosljeðujemo ID i naziv u View
                ViewData["ListaID"] = lista.ListaId;
                ViewData["ListaFiksna"] = true;
                await PopuniPacijentiDropDown(korisnik.Id);
            }
            else
            {
                // Admin bira listu i sve pacijente
                ViewData["ListaFiksna"] = false;
                await PopuniDropDownListe();
            }

            return View();
        }

        // POST: KorisnikNaListi/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("KorisnikNaListiID,ListaID,KorisnikID")] KorisnikNaListi korisnikNaListi)
        {
            var korisnik = await _userManager.GetUserAsync(User);
            if (korisnik == null) return Unauthorized();

            if (User.IsInRole("Doktor"))
            {
                var lista = await _listaCekanjaService.GetListaZaDoktoraAsync(korisnik.Id);
                if (lista == null) return RedirectToAction("Index", "ListaCekanja");

                // Override ListaID bez obzira šta doðe iz forme
                korisnikNaListi.ListaID = lista.ListaId;
                ModelState.Remove("ListaID");
                ModelState.Remove("Lista");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _korisnikNaListiService.AddAsync(korisnikNaListi);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("KorisnikID", ex.Message);
                }
            }

            // Vrati dropdown u sluèaju greške
            if (User.IsInRole("Doktor"))
            {
                var lista = await _listaCekanjaService.GetListaZaDoktoraAsync(korisnik.Id);
                ViewData["ListaID"] = lista?.ListaId;
                ViewData["ListaFiksna"] = true;
                await PopuniPacijentiDropDown(korisnik.Id);
            }
            else
            {
                ViewData["ListaFiksna"] = false;
                await PopuniDropDownListe(korisnikNaListi.ListaID, korisnikNaListi.KorisnikID);
            }

            return View(korisnikNaListi);
        }

        // GET: KorisnikNaListi/Edit/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var stavka = await _korisnikNaListiService.GetByIdAsync(id.Value);
            if (stavka == null) return NotFound();
            await PopuniDropDownListe(stavka.ListaID, stavka.KorisnikID);
            return View(stavka);
        }

        // POST: KorisnikNaListi/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id, [Bind("KorisnikNaListiID,Datum,ListaID,KorisnikID")] KorisnikNaListi korisnikNaListi)
        {
            if (id != korisnikNaListi.KorisnikNaListiID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _korisnikNaListiService.Update(korisnikNaListi);
                    await _korisnikNaListiService.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await KorisnikNaListiExists(korisnikNaListi.KorisnikNaListiID))
                        return NotFound();
                    else
                        throw;
                }
            }

            await PopuniDropDownListe(korisnikNaListi.ListaID, korisnikNaListi.KorisnikID);
            return View(korisnikNaListi);
        }

        // GET: KorisnikNaListi/Delete/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var stavka = await _korisnikNaListiService.GetByIdAsync(id.Value);
            if (stavka == null) return NotFound();
            return View(stavka);
        }

        // POST: KorisnikNaListi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var stavka = await _korisnikNaListiService.GetByIdAsync(id);
            if (stavka != null)
            {
                _korisnikNaListiService.Delete(stavka);
                await _korisnikNaListiService.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // ========== PRIVATNE METODE ==========

        private async Task<bool> KorisnikNaListiExists(int id)
        {
            var stavka = await _korisnikNaListiService.GetByIdAsync(id);
            return stavka != null;
        }

        private async Task PopuniDropDownListe(int? selectedListaId = null, string? selectedKorisnikId = null)
        {
            // Liste èekanja – prikaži doktora umjesto ID-a
            var liste = await _korisnikNaListiService.GetListeCekanjaAsync();
            var listaItems = liste.Select(l => new SelectListItem
            {
                Value = l.ListaId.ToString(),
                Text = l.Doktor != null
                        ? $"Lista #{l.ListaId} – Dr. {l.Doktor.Ime} {l.Doktor.Prezime}"
                        : $"Lista #{l.ListaId}"
            });
            ViewData["ListaID"] = new SelectList(listaItems, "Value", "Text", selectedListaId);

            // Pacijenti – samo korisnici bez specijalizacije (nisu doktori)
            var sviKorisnici = await _korisnikNaListiService.GetKorisniciAsync();
            var pacijenti = sviKorisnici
                .Where(k => k.Specijalizacija == null)
                .Select(k => new SelectListItem
                {
                    Value = k.Id,
                    Text = $"{k.Ime} {k.Prezime}"
                });
            ViewData["KorisnikID"] = new SelectList(pacijenti, "Value", "Text", selectedKorisnikId);
        }
        private async Task PopuniPacijentiDropDown(string doktorId)
        {
            // Samo pacijenti koji imaju termin kod ovog doktora
            var pacijenti = await _korisnikNaListiService.GetPacijentiSaTerminomAsync(doktorId);
            var items = pacijenti.Select(k => new SelectListItem
            {
                Value = k.Id,
                Text = $"{k.Ime} {k.Prezime}"
            });
            ViewData["KorisnikID"] = new SelectList(items, "Value", "Text");
        }
    }
}