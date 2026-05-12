using HealthZone.Data;
using HealthZone.Models;
using HealthZone.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<IActionResult> Index()
        {
            var zahtjevi = await _zahtjevService.GetAllAsync();
            return View(zahtjevi);

        }

        // GET: ZahtjevZaOpremu/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zahtjevZaOpremu = await _zahtjevService.GetByIdAsync(id.Value);
            if (zahtjevZaOpremu == null)
            {
                return NotFound();
            }

            return View(zahtjevZaOpremu);
        }

        // GET: ZahtjevZaOpremu/Create
        public async Task<IActionResult> Create()
        {
            await PopuniDropDownListe();
            return View();
        }

        // POST: ZahtjevZaOpremu/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ZahtjevId,Naziv,Opis,Status,DoktorID,VrstaZahtjeva,Kategorija,Hitnost")] ZahtjevZaOpremu zahtjevZaOpremu)
        {
            if (ModelState.IsValid)
            {
                await _zahtjevService.AddAsync(zahtjevZaOpremu);
                await _zahtjevService.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            await PopuniDropDownListe(zahtjevZaOpremu.DoktorID);
            return View(zahtjevZaOpremu);
        }

        // GET: ZahtjevZaOpremu/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zahtjevZaOpremu = await _zahtjevService.GetByIdAsync(id.Value);
            if (zahtjevZaOpremu == null)
            {
                return NotFound();
            }
            await PopuniDropDownListe(zahtjevZaOpremu.DoktorID);
            return View(zahtjevZaOpremu);
        }

        // POST: ZahtjevZaOpremu/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ZahtjevId,Naziv,Opis,Status,DoktorID,VrstaZahtjeva,Kategorija,Hitnost")] ZahtjevZaOpremu zahtjevZaOpremu)
        {
            if (id != zahtjevZaOpremu.ZahtjevId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _zahtjevService.Update(zahtjevZaOpremu);
                    await _zahtjevService.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (! await ZahtjevZaOpremuExists(zahtjevZaOpremu.ZahtjevId))
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
            await PopuniDropDownListe(zahtjevZaOpremu.DoktorID);
            return View(zahtjevZaOpremu);
        }

        // GET: ZahtjevZaOpremu/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zahtjevZaOpremu = await _zahtjevService.GetByIdAsync(id.Value);
            if (zahtjevZaOpremu == null)
            {
                return NotFound();
            }

            return View(zahtjevZaOpremu);
        }

        // POST: ZahtjevZaOpremu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var zahtjevZaOpremu = await _zahtjevService.GetByIdAsync(id);
            if (zahtjevZaOpremu != null)
            {
                _zahtjevService.Delete(zahtjevZaOpremu);
                await _zahtjevService.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ZahtjevZaOpremuExists(int id)
        {
            var zahtjev = await _zahtjevService.GetByIdAsync(id);
            return zahtjev != null;
        }


        private async Task PopuniDropDownListe(string? selectedDoktorId = null)
        {
            // Dohvati doktore preko servisa
            var doktori = await _korisnikService.GetDoktoriAsync();

            // Popuni dropdown listu - prikazujemo Ime doktora
            ViewData["DoktorID"] = new SelectList(doktori, "Id", "Ime", selectedDoktorId);
        }
    }
}
