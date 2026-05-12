using HealthZone.Models;
using HealthZone.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HealthZone.Controllers
{
    public class KorisnikNaListiController : Controller
    {
        private readonly IKorisnikNaListiService _korisnikNaListiService;
        private readonly IKorisnikService _korisnikService;
        private readonly IListaCekanjaService _listaCekanjaService;

        public KorisnikNaListiController(
            IKorisnikNaListiService korisnikNaListiService,
            IKorisnikService korisnikService,
            IListaCekanjaService listaCekanjaService)
        {
            _korisnikNaListiService = korisnikNaListiService;
            _korisnikService = korisnikService;
            _listaCekanjaService = listaCekanjaService;
        }

        // GET: KorisnikNaListi
        public async Task<IActionResult> Index()
        {
            var stavke = await _korisnikNaListiService.GetAllAsync();
            return View(stavke);
        }

        // GET: KorisnikNaListi/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var korisnikNaListi = await _korisnikNaListiService.GetByIdAsync(id.Value);
            if (korisnikNaListi == null)
            {
                return NotFound();
            }

            return View(korisnikNaListi);
        }

        // GET: KorisnikNaListi/Create
        public async Task<IActionResult> Create()
        {
            await PopuniDropDownListe();
            return View();
        }

        // POST: KorisnikNaListi/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("KorisnikNaListiID,Datum,ListaID,KorisnikID")] KorisnikNaListi korisnikNaListi)
        {
            if (ModelState.IsValid)
            {
                if (korisnikNaListi.Datum == default)
                {
                    korisnikNaListi.Datum = DateTime.Now;
                }

                await _korisnikNaListiService.AddAsync(korisnikNaListi);
                await _korisnikNaListiService.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await PopuniDropDownListe(korisnikNaListi.ListaID, korisnikNaListi.KorisnikID);
            return View(korisnikNaListi);
        }

        // GET: KorisnikNaListi/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var korisnikNaListi = await _korisnikNaListiService.GetByIdAsync(id.Value);
            if (korisnikNaListi == null)
            {
                return NotFound();
            }

            await PopuniDropDownListe(korisnikNaListi.ListaID, korisnikNaListi.KorisnikID);
            return View(korisnikNaListi);
        }

        // POST: KorisnikNaListi/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("KorisnikNaListiID,Datum,ListaID,KorisnikID")] KorisnikNaListi korisnikNaListi)
        {
            if (id != korisnikNaListi.KorisnikNaListiID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _korisnikNaListiService.Update(korisnikNaListi);
                    await _korisnikNaListiService.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await KorisnikNaListiExists(korisnikNaListi.KorisnikNaListiID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            await PopuniDropDownListe(korisnikNaListi.ListaID, korisnikNaListi.KorisnikID);
            return View(korisnikNaListi);
        }

        // GET: KorisnikNaListi/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var korisnikNaListi = await _korisnikNaListiService.GetByIdAsync(id.Value);
            if (korisnikNaListi == null)
            {
                return NotFound();
            }

            return View(korisnikNaListi);
        }

        // POST: KorisnikNaListi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var korisnikNaListi = await _korisnikNaListiService.GetByIdAsync(id);
            if (korisnikNaListi != null)
            {
                _korisnikNaListiService.Delete(korisnikNaListi);
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
            var liste = await _listaCekanjaService.GetAllAsync();
            var korisnici = await _korisnikService.GetAllAsync();

            ViewData["ListaID"] = new SelectList(liste, "ListaId", "ListaId", selectedListaId);
            ViewData["KorisnikID"] = new SelectList(korisnici, "Id", "Ime", selectedKorisnikId);
        }
    }
}