using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthZone.Models;
using HealthZone.Services;

namespace HealthZone.Controllers
{
    public class NalazController : Controller
    {
        private readonly INalazService _nalazService;

        public NalazController(INalazService nalazService)
        {
            _nalazService = nalazService;
        }

        // GET: Nalaz
        public async Task<IActionResult> Index()
        {
            var nalazi = await _nalazService.GetAllAsync();
            return View(nalazi);
        }

        // GET: Nalaz/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nalaz = await _nalazService.GetByIdAsync(id.Value);
            if (nalaz == null)
            {
                return NotFound();
            }

            return View(nalaz);
        }

        // GET: Nalaz/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Nalaz/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NalazId,Opis,Dijagnoza,Terapija,TerminID,PacijentID")] Nalaz nalaz)
        {
            if (ModelState.IsValid)
            {
                await _nalazService.AddAsync(nalaz);
                await _nalazService.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(nalaz);
        }

        // GET: Nalaz/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nalaz = await _nalazService.GetByIdAsync(id.Value);
            if (nalaz == null)
            {
                return NotFound();
            }
            return View(nalaz);
        }

        // POST: Nalaz/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NalazId,Opis,Dijagnoza,Terapija,TerminID,PacijentID")] Nalaz nalaz)
        {
            if (id != nalaz.NalazId)
            {
                return NotFound();
            }

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
            return View(nalaz);
        }

        // GET: Nalaz/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nalaz = await _nalazService.GetByIdAsync(id.Value);
            if (nalaz == null)
            {
                return NotFound();
            }

            return View(nalaz);
        }

        // POST: Nalaz/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nalaz = await _nalazService.GetByIdAsync(id);
            if (nalaz != null)
            {
                _nalazService.Delete(nalaz);
                await _nalazService.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // ========== PRIVATNE METODE ==========

        private async Task<bool> NalazExists(int id)
        {
            var nalaz = await _nalazService.GetByIdAsync(id);
            return nalaz != null;
        }
    }
}