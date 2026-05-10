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
    public class TerminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TerminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Termin
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Termin.Include(t => t.Doktor).Include(t => t.Pacijent).Include(t => t.Usluga);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Termin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var termin = await _context.Termin
                .Include(t => t.Doktor)
                .Include(t => t.Pacijent)
                .Include(t => t.Usluga)
                .FirstOrDefaultAsync(m => m.TerminId == id);
            if (termin == null)
            {
                return NotFound();
            }

            return View(termin);
        }

        // GET: Termin/Create
        public IActionResult Create()
        {
            ViewData["DoktorID"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["PacijentID"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["UslugaID"] = new SelectList(_context.MedicinskaUsluga, "UslugaId", "UslugaId");
            return View();
        }

        // POST: Termin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TerminId,Datum,Vrijeme,Status,DoktorID,PacijentID,UslugaID")] Termin termin)
        {
            if (ModelState.IsValid)
            {
                _context.Add(termin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DoktorID"] = new SelectList(_context.Users, "Id", "Id", termin.DoktorID);
            ViewData["PacijentID"] = new SelectList(_context.Users, "Id", "Id", termin.PacijentID);
            ViewData["UslugaID"] = new SelectList(_context.MedicinskaUsluga, "UslugaId", "UslugaId", termin.UslugaID);
            return View(termin);
        }

        // GET: Termin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var termin = await _context.Termin.FindAsync(id);
            if (termin == null)
            {
                return NotFound();
            }
            ViewData["DoktorID"] = new SelectList(_context.Users, "Id", "Id", termin.DoktorID);
            ViewData["PacijentID"] = new SelectList(_context.Users, "Id", "Id", termin.PacijentID);
            ViewData["UslugaID"] = new SelectList(_context.MedicinskaUsluga, "UslugaId", "UslugaId", termin.UslugaID);
            return View(termin);
        }

        // POST: Termin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
                    _context.Update(termin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TerminExists(termin.TerminId))
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
            ViewData["DoktorID"] = new SelectList(_context.Users, "Id", "Id", termin.DoktorID);
            ViewData["PacijentID"] = new SelectList(_context.Users, "Id", "Id", termin.PacijentID);
            ViewData["UslugaID"] = new SelectList(_context.MedicinskaUsluga, "UslugaId", "UslugaId", termin.UslugaID);
            return View(termin);
        }

        // GET: Termin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var termin = await _context.Termin
                .Include(t => t.Doktor)
                .Include(t => t.Pacijent)
                .Include(t => t.Usluga)
                .FirstOrDefaultAsync(m => m.TerminId == id);
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
            var termin = await _context.Termin.FindAsync(id);
            if (termin != null)
            {
                _context.Termin.Remove(termin);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TerminExists(int id)
        {
            return _context.Termin.Any(e => e.TerminId == id);
        }
    }
}
