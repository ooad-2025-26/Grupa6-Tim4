using HealthZone.Models;
using HealthZone.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HealthZone.Controllers
{
    public class ZahtjevZaOpremuController : Controller
    {
        private readonly IZahtjevZaOpremuService _zahtjevService;
        private readonly IKorisnikService _korisnikService;

        public ZahtjevZaOpremuController(
            IZahtjevZaOpremuService zahtjevService,
            IKorisnikService korisnikService)
        {
            _zahtjevService = zahtjevService;
            _korisnikService = korisnikService;
        }

        // GET: ZahtjevZaOpremu
        [Authorize(Roles = "Administrator,Doktor")]
        public async Task<IActionResult> Index()
        {
            var zahtjevi = await _zahtjevService.GetAllAsync();
            return View(zahtjevi);
        }

        // GET: ZahtjevZaOpremu/Details/5
        [Authorize(Roles = "Administrator,Doktor")]

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var zahtjev = await _zahtjevService.GetByIdAsync(id.Value);
            if (zahtjev == null) return NotFound();
            return View(zahtjev);
        }

        // GET: ZahtjevZaOpremu/Create
        [Authorize(Roles = "Doktor")]
        public async Task<IActionResult> Create()
        {
            await PopuniDropDownListe();
            return View();
        }

        // POST: ZahtjevZaOpremu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Doktor")]
        public async Task<IActionResult> Create(
            [Bind("Naziv,Opis,VrstaZahtjeva,Kategorija,Hitnost")]
            ZahtjevZaOpremu zahtjevZaOpremu)
        {
            // ✅ FIX: navigation property nije u formi → .NET 6+ ga tretira kao Required
            //         bez ovoga ModelState.IsValid je uvijek false i forma se ne snima
            ModelState.Remove(nameof(ZahtjevZaOpremu.Doktor));
            ModelState.Remove(nameof(ZahtjevZaOpremu.Status));
            ModelState.Remove(nameof(ZahtjevZaOpremu.DoktorID));

            if (ModelState.IsValid)
            {
                try
                {
                    zahtjevZaOpremu.DoktorID = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

                    await _zahtjevService.AddAsync(zahtjevZaOpremu);

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Greška iz servisa (prazno Naziv/Opis) prikazuje se u formi
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            await PopuniDropDownListe(zahtjevZaOpremu.DoktorID);
            return View(zahtjevZaOpremu);
        }

        // GET: ZahtjevZaOpremu/Edit/5
        [Authorize(Roles = "Doktor")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var zahtjev = await _zahtjevService.GetByIdAsync(id.Value);
            if (zahtjev == null) return NotFound();
            if (zahtjev.Status != Status.NaCekanju) return RedirectToAction(nameof(Index));

            // Provjeri da li je doktor vlasnik zahtjeva
            var trenutniKorisnikId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (zahtjev.DoktorID != trenutniKorisnikId)
                return RedirectToAction(nameof(Index));

            await PopuniDropDownListe(zahtjev.DoktorID);
            return View(zahtjev);
        }

        // POST: ZahtjevZaOpremu/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Doktor")]
        public async Task<IActionResult> Edit(int id,
            [Bind("ZahtjevId,Naziv,Opis,Status,DoktorID,VrstaZahtjeva,Kategorija,Hitnost")]
            ZahtjevZaOpremu zahtjevZaOpremu)
        {
            if (id != zahtjevZaOpremu.ZahtjevId) return NotFound();

            // ✅ Isti FIX kao u Create
            ModelState.Remove(nameof(ZahtjevZaOpremu.Doktor));

            if (ModelState.IsValid)
            {
                var trenutniKorisnikId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (zahtjevZaOpremu.DoktorID != trenutniKorisnikId)
                    return RedirectToAction(nameof(Index));

                try
                {
                    _zahtjevService.Update(zahtjevZaOpremu);
                    await _zahtjevService.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ZahtjevPostoji(zahtjevZaOpremu.ZahtjevId))
                        return NotFound();
                    throw;
                }
            }

            await PopuniDropDownListe(zahtjevZaOpremu.DoktorID);
            return View(zahtjevZaOpremu);
        }

        // GET: ZahtjevZaOpremu/Delete/5
        [Authorize(Roles = "Doktor")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var zahtjev = await _zahtjevService.GetByIdAsync(id.Value);
            if (zahtjev == null) return NotFound();

            // Provjeri da li je doktor vlasnik zahtjeva
            var trenutniKorisnikId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (zahtjev.DoktorID != trenutniKorisnikId)
                return RedirectToAction(nameof(Index));

            return View(zahtjev);
        }

        // POST: ZahtjevZaOpremu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Doktor")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var zahtjev = await _zahtjevService.GetByIdAsync(id);
            if (zahtjev != null)
            {
                // Provjeri da li je doktor vlasnik
                var trenutniKorisnikId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (zahtjev.DoktorID != trenutniKorisnikId)
                    return RedirectToAction(nameof(Index));

                _zahtjevService.Delete(zahtjev);
                await _zahtjevService.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: ZahtjevZaOpremu/Odobri
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Odobri(int id)
        {
            try { await _zahtjevService.OdobriZahtjevAsync(id); }
            catch (Exception ex) { TempData["Greška"] = ex.Message; }
            return RedirectToAction(nameof(Index));
        }

        // POST: ZahtjevZaOpremu/Odbij
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Odbij(int id)
        {
            try { await _zahtjevService.OdbijZahtjevAsync(id); }
            catch (Exception ex) { TempData["Greška"] = ex.Message; }
            return RedirectToAction(nameof(Index));
        }

        // ─── HELPERS ──────────────────────────────────────────────────────────
        private async Task<bool> ZahtjevPostoji(int id)
            => await _zahtjevService.GetByIdAsync(id) != null;

        private async Task PopuniDropDownListe(string? selectedDoktorId = null)
        {
            var doktori = await _korisnikService.GetDoktoriAsync();
            ViewData["DoktorID"] = new SelectList(doktori, "Id", "Ime", selectedDoktorId);
        }

        public async Task<IActionResult> Dokumentacija(int id)
        {
            var zahtjev = await _zahtjevService.GetByIdAsync(id);
            if (zahtjev == null) return NotFound();
            return View(zahtjev);
        }
    }
}