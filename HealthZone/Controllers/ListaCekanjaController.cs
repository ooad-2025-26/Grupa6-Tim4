using HealthZone.Models;
using HealthZone.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HealthZone.Controllers
{
    public class ListaCekanjaController : Controller
    {
        private readonly IListaCekanjaService _listaCekanjaService;
        private readonly IKorisnikService _korisnikService;

        public ListaCekanjaController(
            IListaCekanjaService listaCekanjaService,
            IKorisnikService korisnikService)
        {
            _listaCekanjaService = listaCekanjaService;
            _korisnikService = korisnikService;
        }

        // GET: ListaCekanja
        public async Task<IActionResult> Index()
        {
            var liste = await _listaCekanjaService.GetAllAsync();
            return View(liste);
        }

        // GET: ListaCekanja/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var listaCekanja = await _listaCekanjaService.GetByIdAsync(id.Value);
            if (listaCekanja == null)
            {
                return NotFound();
            }

            return View(listaCekanja);
        }

        // GET: ListaCekanja/Create
        public async Task<IActionResult> Create()
        {
            await PopuniDropDownListe();
            return View();
        }

        // POST: ListaCekanja/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ListaId,DoktorID")] ListaCekanja listaCekanja)
        {
            if (ModelState.IsValid)
            {
                await _listaCekanjaService.AddAsync(listaCekanja);
                await _listaCekanjaService.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await PopuniDropDownListe(listaCekanja.DoktorID);
            return View(listaCekanja);
        }

        // GET: ListaCekanja/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var listaCekanja = await _listaCekanjaService.GetByIdAsync(id.Value);
            if (listaCekanja == null)
            {
                return NotFound();
            }

            await PopuniDropDownListe(listaCekanja.DoktorID);
            return View(listaCekanja);
        }

        // POST: ListaCekanja/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ListaId,DoktorID")] ListaCekanja listaCekanja)
        {
            if (id != listaCekanja.ListaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _listaCekanjaService.Update(listaCekanja);
                    await _listaCekanjaService.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ListaCekanjaExists(listaCekanja.ListaId))
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

            await PopuniDropDownListe(listaCekanja.DoktorID);
            return View(listaCekanja);
        }

        // GET: ListaCekanja/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var listaCekanja = await _listaCekanjaService.GetByIdAsync(id.Value);
            if (listaCekanja == null)
            {
                return NotFound();
            }

            return View(listaCekanja);
        }

        // POST: ListaCekanja/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var listaCekanja = await _listaCekanjaService.GetByIdAsync(id);
            if (listaCekanja != null)
            {
                _listaCekanjaService.Delete(listaCekanja);
                await _listaCekanjaService.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // ========== PRIVATNE METODE ==========

        private async Task<bool> ListaCekanjaExists(int id)
        {
            var lista = await _listaCekanjaService.GetByIdAsync(id);
            return lista != null;
        }

        private async Task PopuniDropDownListe(string? selectedDoktorId = null)
        {
            var doktori = await _korisnikService.GetDoktoriAsync();
            ViewData["DoktorID"] = new SelectList(doktori, "Id", "Ime", selectedDoktorId);
        }
    }
}