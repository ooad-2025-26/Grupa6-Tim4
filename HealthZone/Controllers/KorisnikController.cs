using HealthZone.Models;
using HealthZone.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace HealthZone.Controllers
{
    public class KorisnikController : Controller
    {
        private readonly IKorisnikService _korisnikService;

        public KorisnikController(IKorisnikService korisnikService)
        {
            _korisnikService = korisnikService;
        }

        // GET: Korisnik
        public async Task<IActionResult> Index()
        {
            var korisnici = await _korisnikService.GetAllAsync();
            return View(korisnici);
        }

        // GET: Korisnik/Details/id
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var korisnik = await _korisnikService.GetByIdAsync(id);
            if (korisnik == null)
            {
                return NotFound();
            }

            return View(korisnik);
        }

        // GET: Korisnik/Create
        public IActionResult Create()
        {
            PopuniViewBag();
            return View();
        }

        // POST: Korisnik/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Korisnik korisnik, string lozinka, string uloga)
        {
            if (ModelState.IsValid)
            {
                await _korisnikService.AddAsync(korisnik, lozinka, uloga);
                await _korisnikService.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopuniViewBag();
            return View(korisnik);
        }

        // GET: Korisnik/Edit/id
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var korisnik = await _korisnikService.GetByIdAsync(id);
            if (korisnik == null)
            {
                return NotFound();
            }

            PopuniViewBag();
            return View(korisnik);
        }

        // POST: Korisnik/Edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Korisnik korisnik)
        {
            if (id != korisnik.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _korisnikService.Update(korisnik);
                    await _korisnikService.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await KorisnikExists(korisnik.Id))
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

            PopuniViewBag();
            return View(korisnik);
        }

        // GET: Korisnik/Delete/id
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var korisnik = await _korisnikService.GetByIdAsync(id);
            if (korisnik == null)
            {
                return NotFound();
            }

            return View(korisnik);
        }

        // POST: Korisnik/Delete/id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var korisnik = await _korisnikService.GetByIdAsync(id);
            if (korisnik != null)
            {
                _korisnikService.Delete(korisnik);
                await _korisnikService.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        private async Task<bool> KorisnikExists(string id)
        {
            var korisnik = await _korisnikService.GetByIdAsync(id);
            return korisnik != null;
        }

        private void PopuniViewBag()
        {
            ViewBag.Prioriteti = new SelectList(Enum.GetValues(typeof(Prioritet)));
            ViewBag.Specijalizacije = new SelectList(Enum.GetValues(typeof(Specijalizacija)));
        }
    }
}