using HealthZone.Models;
using HealthZone.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthZone.Controllers
{
    public class MedicinskaUslugaController : Controller
    {
        private readonly IMedicinskaUslugaService _medicinskaUslugaService;

        public MedicinskaUslugaController(IMedicinskaUslugaService medicinskaUslugaService)
        {
            _medicinskaUslugaService = medicinskaUslugaService;
        }

        // GET: MedicinskaUsluga
        public async Task<IActionResult> Index()
        {
            var usluge = await _medicinskaUslugaService.GetAllAsync();
            return View(usluge);
        }

        // GET: MedicinskaUsluga/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicinskaUsluga = await _medicinskaUslugaService.GetByIdAsync(id.Value);
            if (medicinskaUsluga == null)
            {
                return NotFound();
            }

            return View(medicinskaUsluga);
        }

        // GET: MedicinskaUsluga/Create
        [Authorize(Roles = "Administrator,Doktor")]

        public IActionResult Create()
        {
            return View();
        }

        // POST: MedicinskaUsluga/Create
        [Authorize(Roles = "Administrator,Doktor")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UslugaId,Naziv,Vrsta,Opis,Cijena")] MedicinskaUsluga medicinskaUsluga)
        {
            if (ModelState.IsValid)
            {
                await _medicinskaUslugaService.AddAsync(medicinskaUsluga);
                return RedirectToAction(nameof(Index));
            }
            return View(medicinskaUsluga);
        }

        // GET: MedicinskaUsluga/Edit/5
        [Authorize(Roles = "Administrator,Doktor")]

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicinskaUsluga = await _medicinskaUslugaService.GetByIdAsync(id.Value);
            if (medicinskaUsluga == null)
            {
                return NotFound();
            }
            return View(medicinskaUsluga);
        }
        [Authorize(Roles = "Administrator,Doktor")]

        // POST: MedicinskaUsluga/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UslugaId,Naziv,Vrsta,Opis,Cijena")] MedicinskaUsluga medicinskaUsluga)
        {
            if (id != medicinskaUsluga.UslugaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _medicinskaUslugaService.Update(medicinskaUsluga);
                    await _medicinskaUslugaService.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await MedicinskaUslugaExists(medicinskaUsluga.UslugaId))
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
            return View(medicinskaUsluga);
        }

        // GET: MedicinskaUsluga/Delete/5
        [Authorize(Roles = "Administrator,Doktor")]

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicinskaUsluga = await _medicinskaUslugaService.GetByIdAsync(id.Value);
            if (medicinskaUsluga == null)
            {
                return NotFound();
            }

            return View(medicinskaUsluga);
        }
        [Authorize(Roles = "Administrator,Doktor")]

        // POST: MedicinskaUsluga/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medicinskaUsluga = await _medicinskaUslugaService.GetByIdAsync(id);
            if (medicinskaUsluga != null)
            {
                _medicinskaUslugaService.Delete(medicinskaUsluga);
                await _medicinskaUslugaService.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // ========== PRIVATNE METODE ==========

        private async Task<bool> MedicinskaUslugaExists(int id)
        {
            var usluga = await _medicinskaUslugaService.GetByIdAsync(id);
            return usluga != null;
        }
    }
}