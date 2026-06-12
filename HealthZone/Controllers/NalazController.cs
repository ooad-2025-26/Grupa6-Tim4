using HealthZone.Models;
using HealthZone.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HealthZone.Controllers
{
    [Authorize(Roles = "Administrator,Doktor,Pacijent,MedicinskaSestra")]
    public class NalazController : Controller
    {
        private readonly INalazService _nalazService;
        private readonly IKorisnikService _korisnikService;
        private readonly ITerminService _terminService;

        public NalazController(INalazService nalazService, IKorisnikService korisnikService, ITerminService terminService)
        {
            _nalazService = nalazService;
            _korisnikService = korisnikService;
            _terminService = terminService;
        }

        // GET: Nalaz
        public async Task<IActionResult> Index()
        {
            var svi = await _nalazService.GetAllAsync();

            IEnumerable<Nalaz> nalazi;

            if (User.IsInRole("Administrator") || User.IsInRole("Doktor") || User.IsInRole("MedicinskaSestra"))
            {
                nalazi = svi;
            }
            else
            {
                // Pacijent vidi samo svoje nalaze
                var trenutniKorisnikId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                nalazi = svi.Where(n => n.PacijentID == trenutniKorisnikId);
            }

            // Sortiraj od najnovijeg ka najstarijem (in-memory, sigurno za DateOnly/TimeOnly)
            nalazi = nalazi.ToList().OrderByDescending(n => n.Termin != null
                ? new DateTime(n.Termin.Datum.Year, n.Termin.Datum.Month, n.Termin.Datum.Day,
                               n.Termin.Vrijeme.Hour, n.Termin.Vrijeme.Minute, 0)
                : DateTime.MinValue);

            return View(nalazi);
        }

        // GET: Nalaz/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var nalaz = await _nalazService.GetByIdAsync(id.Value);
            if (nalaz == null) return NotFound();

            // Pacijent mo�e vidjeti samo svoj nalaz
            if (User.IsInRole("Pacijent"))
            {
                var trenutniKorisnikId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (nalaz.PacijentID != trenutniKorisnikId)
                    return Forbid();
            }

            return View(nalaz);
        }

        // GET: Nalaz/Create
        [Authorize(Roles = "Doktor,Administrator")]
        public async Task<IActionResult> Create()
        {
            await PopuniDropDownListe();
            return View();
        }

        // POST: Nalaz/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Doktor,Administrator")]
        public async Task<IActionResult> Create([Bind("NalazId,Opis,Dijagnoza,Terapija,TerminID,PacijentID")] Nalaz nalaz)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _nalazService.AddAsync(nalaz);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("TerminID", ex.Message);
                }
            }
            await PopuniDropDownListe(nalaz.TerminID, nalaz.PacijentID);
            return View(nalaz);
        }

        // GET: Nalaz/Edit/5
        [Authorize(Roles = "Doktor,Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var nalaz = await _nalazService.GetByIdAsync(id.Value);
            if (nalaz == null) return NotFound();

            await PopuniDropDownListe(nalaz.TerminID, nalaz.PacijentID);
            return View(nalaz);
        }

        // POST: Nalaz/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Doktor,Administrator")]
        public async Task<IActionResult> Edit(int id, [Bind("NalazId,Opis,Dijagnoza,Terapija,TerminID,PacijentID")] Nalaz nalaz)
        {
            if (id != nalaz.NalazId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _nalazService.Update(nalaz);
                    await _nalazService.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await NalazExists(nalaz.NalazId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(nalaz);
        }

        // GET: Nalaz/Delete/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var nalaz = await _nalazService.GetByIdAsync(id.Value);
            if (nalaz == null) return NotFound();

            return View(nalaz);
        }

        // POST: Nalaz/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Doktor")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nalaz = await _nalazService.GetByIdAsync(id);
            if (nalaz != null)
            {
                // Doktor mo�e brisati samo svoje nalaze
                if (User.IsInRole("Doktor"))
                {
                    var trenutniId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (nalaz.Termin?.DoktorID != trenutniId)
                        return Forbid();
                }

                _nalazService.Delete(nalaz);
                await _nalazService.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Nalaz/Print/5
        [Authorize(Roles = "MedicinskaSestra,Doktor,Administrator")]
        public async Task<IActionResult> Print(int id)
        {
            var nalaz = await _nalazService.GetByIdAsync(id);
            if (nalaz == null) return NotFound();
            return View(nalaz);
        }

        // GET: Nalaz/MedicinskiKarton/5  (5 = pacijentId string)
        [Authorize(Roles = "Doktor,Administrator")]
        public async Task<IActionResult> MedicinskiKarton(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var pacijent = await _korisnikService.GetByIdAsync(id);
            if (pacijent == null) return NotFound();

            var nalazi = await _nalazService.GetNalaziPacijentaAsync(id);

            ViewData["Pacijent"] = pacijent;
            return View(nalazi);
        }

        // GET: Nalaz/GetPacijentZaTermin/5  (AJAX)
        [Authorize(Roles = "Doktor,Administrator")]
        public async Task<IActionResult> GetPacijentZaTermin(int id)
        {
            var termin = await _terminService.GetByIdAsync(id);
            if (termin == null) return NotFound();
            return Json(new { pacijentId = termin.PacijentID });
        }

        // ========== PRIVATNE METODE ==========

        private async Task PopuniDropDownListe(int? selectedTerminId = null, string? selectedPacijentId = null)
        {
            var sviTermini = await _terminService.GetAllAsync();

            // Doktor vidi samo svoje aktivne termine; Administrator vidi sve aktivne
            IEnumerable<Termin> filtrirani = sviTermini
                .Where(t => t.Status == Status.Aktivan);

            if (User.IsInRole("Doktor"))
            {
                var trenutniDoktorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                filtrirani = filtrirani.Where(t => t.DoktorID == trenutniDoktorId);
            }

            var terminItems = filtrirani
                .Select(t => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = t.TerminId.ToString(),
                    Text = $"{t.Datum:dd.MM.yyyy} {t.Vrijeme:HH:mm} — {t.Pacijent?.Ime} {t.Pacijent?.Prezime} ({t.Usluga?.Naziv ?? "—"})",
                    Selected = t.TerminId == selectedTerminId
                });
            ViewData["TerminID"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(terminItems, "Value", "Text", selectedTerminId);

            // Pacijenti: doktor vidi samo pacijente sa svojih aktivnih termina
            IEnumerable<Korisnik> pacijentiKorisnici;

            if (User.IsInRole("Doktor"))
            {
                var trenutniDoktorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var pacijentIdsDoktora = sviTermini
                    .Where(t => t.DoktorID == trenutniDoktorId && t.Status == Status.Aktivan)
                    .Select(t => t.PacijentID)
                    .Distinct()
                    .ToHashSet();

                var sviKorisnici = await _korisnikService.GetAllAsync();
                pacijentiKorisnici = sviKorisnici
                    .Where(k => k.Specijalizacija == null && pacijentIdsDoktora.Contains(k.Id));
            }
            else
            {
                var sviKorisnici = await _korisnikService.GetAllAsync();
                pacijentiKorisnici = sviKorisnici.Where(k => k.Specijalizacija == null);
            }

            var pacijenti = pacijentiKorisnici
                .Select(k => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = k.Id,
                    Text = $"{k.Ime} {k.Prezime}",
                    Selected = k.Id == selectedPacijentId
                });
            ViewData["PacijentID"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(pacijenti, "Value", "Text", selectedPacijentId);
        }


        private async Task<bool> NalazExists(int id)
        {
            var nalaz = await _nalazService.GetByIdAsync(id);
            return nalaz != null;
        }
    }
}