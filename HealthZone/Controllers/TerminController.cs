using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthZone.Data;
using HealthZone.Models;
using HealthZone.Services;

namespace HealthZone.Controllers
{
    public class TerminController : Controller
    {
        private readonly ITerminService _terminService;
        private readonly IKorisnikService _korisnikService;
        private readonly IMedicinskaUslugaService _uslugaService;

        public TerminController(
            ITerminService terminService,
            IKorisnikService korisnikService,
            IMedicinskaUslugaService uslugaService)
        {
            _terminService = terminService;
            _korisnikService = korisnikService;
            _uslugaService = uslugaService;
        }

        // GET: Termin
        public async Task<IActionResult> Index()
        {
            var termini = await _terminService.GetAllAsync();
            return View(termini);
        }

        // GET: Termin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var termin = await _terminService.GetByIdAsync(id.Value);
            if (termin == null)
            {
                return NotFound();
            }

            return View(termin);
        }

        // GET: Termin/Create
        public async Task<IActionResult> Create()
        {
            await PopuniDropDownListe();
            return View();
        }


        // POST: Termin/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TerminId,Datum,Vrijeme,Status,DoktorID,PacijentID,UslugaID")] Termin termin)
        {
            if (ModelState.IsValid)
            {
                await _terminService.AddAsync(termin);
                await _terminService.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            await PopuniDropDownListe(termin.DoktorID, termin.UslugaID);
            return View(termin);
        }

        // GET: Termin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var termin = await _terminService.GetByIdAsync(id.Value);
            if (termin == null)
            {
                return NotFound();
            }
            await PopuniDropDownListe(termin.DoktorID, termin.UslugaID);
            return View(termin);
        }

        // POST: Termin/Edit/5
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TerminId,Datum,Vrijeme,Status,DoktorID,PacijentID,UslugaID")] Termin termin)
        {
            if (id != termin.TerminId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _terminService.Update(termin);
                    await _terminService.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await TerminExists(termin.TerminId))
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
            await PopuniDropDownListe(termin.DoktorID, termin.UslugaID);
            return View(termin);
        }

        // GET: Termin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var termin = await _terminService.GetByIdAsync(id.Value);
            if (termin == null)
            {
                return NotFound();
            }

            return View(termin);
        }

        // POST: Termin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var termin = await _terminService.GetByIdAsync(id);
            if (termin != null)
            {
                _terminService.Delete(termin);
                await _terminService.SaveChangesAsync();
            }

           
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> TerminExists(int id)
        {
            var termin = await _terminService.GetByIdAsync(id);
            return termin != null;
        }
        private async Task PopuniDropDownListe(string? selectedDoktorId = null, int? selectedUslugaId = null)
        {
            // Dohvati podatke preko servisa (NE _context!)
            var doktori = await _korisnikService.GetDoktoriAsync();
            var pacijenti = await _korisnikService.GetPacijentiAsync();
            var usluge = await _uslugaService.GetAllAsync();

            // Popuni ViewData za dropdown liste
            // Prikazujemo Ime i Prezime za doktore/pacijente, a za usluge Naziv
            ViewData["DoktorID"] = new SelectList(doktori, "Id", "Ime", selectedDoktorId);
            ViewData["PacijentID"] = new SelectList(pacijenti, "Id", "Ime");
            ViewData["UslugaID"] = new SelectList(usluge, "UslugaId", "Naziv", selectedUslugaId);
        }
    }
}
