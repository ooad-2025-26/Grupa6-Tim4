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
    public class RecenzijaController : Controller
    {
        private readonly IRecenzijaService _recenzijaService;
        private readonly IKorisnikService _korisnikService;

        public RecenzijaController(
            IRecenzijaService recenzijaService,
            IKorisnikService korisnikService)
        {
            _recenzijaService = recenzijaService;
            _korisnikService = korisnikService;
        }

        // GET: Recenzija
        public async Task<IActionResult> Index()
        {
            var recenzije = await _recenzijaService.GetAllAsync();
            return View(recenzije);
        }

        // GET: Recenzija/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recenzija = await _recenzijaService.GetByIdAsync(id.Value);
            if (recenzija == null)
            {
                return NotFound();
            }

            return View(recenzija);
        }

        // GET: Recenzija/Create
        public async Task<IActionResult> Create()
        {
            await PopuniDropDownListe();
            return View();
        }

        // POST: Recenzija/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RecenzijaId,Komentar,OcjenaLjubaznosti,OcjenaProfesionalnosti,OcjenaUsluge,PacijentID,DoktorID")] Recenzija recenzija)
        {
            if (ModelState.IsValid)
            {
               await _recenzijaService.AddAsync(recenzija);
                await _recenzijaService.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            await PopuniDropDownListe(recenzija.DoktorID, recenzija.PacijentID);
            return View(recenzija);
        }

        // GET: Recenzija/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recenzija = await _recenzijaService.GetByIdAsync(id.Value);
            if (recenzija == null)
            {
                return NotFound();
            }
            await PopuniDropDownListe(recenzija.DoktorID, recenzija.PacijentID);
            return View(recenzija);
        }

        // POST: Recenzija/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RecenzijaId,Komentar,OcjenaLjubaznosti,OcjenaProfesionalnosti,OcjenaUsluge,PacijentID,DoktorID")] Recenzija recenzija)
        {
            if (id != recenzija.RecenzijaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _recenzijaService.Update(recenzija);
                    await _recenzijaService.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (! await RecenzijaExists(recenzija.RecenzijaId))
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
            await PopuniDropDownListe(recenzija.DoktorID, recenzija.PacijentID);
            return View(recenzija);
        }

        // GET: Recenzija/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recenzija = await _recenzijaService.GetByIdAsync(id.Value);

            if (recenzija == null)
            {
                return NotFound();
            }

            return View(recenzija);
        }

        // POST: Recenzija/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recenzija = await _recenzijaService.GetByIdAsync(id);
            if (recenzija != null)
            {
                _recenzijaService.Delete(recenzija);
                await _recenzijaService.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> RecenzijaExists(int id)
        {
            var recenzija = await _recenzijaService.GetByIdAsync(id);
            return recenzija != null;
        }

        private async Task PopuniDropDownListe(string? selectedDoktorId = null, string? selectedPacijentId = null)
        {
            // Dohvati doktore i pacijente preko servisa
            var doktori = await _korisnikService.GetDoktoriAsync();
            var pacijenti = await _korisnikService.GetPacijentiAsync();

            // Popuni dropdown liste - prikazujemo Ime (ili Ime + Prezime)
            ViewData["DoktorID"] = new SelectList(doktori, "Id", "Ime", selectedDoktorId);
            ViewData["PacijentID"] = new SelectList(pacijenti, "Id", "Ime", selectedPacijentId);
        }
    }
}
