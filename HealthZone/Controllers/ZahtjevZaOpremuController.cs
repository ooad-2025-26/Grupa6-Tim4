using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthZone.Data;
using HealthZone.Models;

namespace HealthZone.Controllers
{
    public class ZahtjevZaOpremuController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ZahtjevZaOpremuController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ZahtjevZaOpremu
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ZahtjevZaOpremu.Include(z => z.Doktor);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ZahtjevZaOpremu/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zahtjevZaOpremu = await _context.ZahtjevZaOpremu
                .Include(z => z.Doktor)
                .FirstOrDefaultAsync(m => m.ZahtjevId == id);
            if (zahtjevZaOpremu == null)
            {
                return NotFound();
            }

            return View(zahtjevZaOpremu);
        }

        // GET: ZahtjevZaOpremu/Create
        public IActionResult Create()
        {
            ViewData["DoktorID"] = new SelectList(_context.Users, "Id", "Id");
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
                _context.Add(zahtjevZaOpremu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DoktorID"] = new SelectList(_context.Users, "Id", "Id", zahtjevZaOpremu.DoktorID);
            return View(zahtjevZaOpremu);
        }

        // GET: ZahtjevZaOpremu/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zahtjevZaOpremu = await _context.ZahtjevZaOpremu.FindAsync(id);
            if (zahtjevZaOpremu == null)
            {
                return NotFound();
            }
            ViewData["DoktorID"] = new SelectList(_context.Users, "Id", "Id", zahtjevZaOpremu.DoktorID);
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
                    _context.Update(zahtjevZaOpremu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ZahtjevZaOpremuExists(zahtjevZaOpremu.ZahtjevId))
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
            ViewData["DoktorID"] = new SelectList(_context.Users, "Id", "Id", zahtjevZaOpremu.DoktorID);
            return View(zahtjevZaOpremu);
        }

        // GET: ZahtjevZaOpremu/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var zahtjevZaOpremu = await _context.ZahtjevZaOpremu
                .Include(z => z.Doktor)
                .FirstOrDefaultAsync(m => m.ZahtjevId == id);
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
            var zahtjevZaOpremu = await _context.ZahtjevZaOpremu.FindAsync(id);
            if (zahtjevZaOpremu != null)
            {
                _context.ZahtjevZaOpremu.Remove(zahtjevZaOpremu);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ZahtjevZaOpremuExists(int id)
        {
            return _context.ZahtjevZaOpremu.Any(e => e.ZahtjevId == id);
        }
    }
}
