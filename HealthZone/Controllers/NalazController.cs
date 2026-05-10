
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthZone.Models;
using HealthZone.Data;

public class NalazController : Controller
{
    private readonly ApplicationDbContext _context;

    public NalazController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: NALAZS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Nalaz.ToListAsync());
    }

    // GET: NALAZS/Details/5
    public async Task<IActionResult> Details(int? nalazid)
    {
        if (nalazid == null)
        {
            return NotFound();
        }

        var nalaz = await _context.Nalaz
            .FirstOrDefaultAsync(m => m.NalazId == nalazid);
        if (nalaz == null)
        {
            return NotFound();
        }

        return View(nalaz);
    }

    // GET: NALAZS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: NALAZS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("NalazId,Opis,Dijagnoza,Terapija,TerminID,Termin,PacijentID,Pacijent")] Nalaz nalaz)
    {
        if (ModelState.IsValid)
        {
            _context.Add(nalaz);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(nalaz);
    }

    // GET: NALAZS/Edit/5
    public async Task<IActionResult> Edit(int? nalazid)
    {
        if (nalazid == null)
        {
            return NotFound();
        }

        var nalaz = await _context.Nalaz.FindAsync(nalazid);
        if (nalaz == null)
        {
            return NotFound();
        }
        return View(nalaz);
    }

    // POST: NALAZS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? nalazid, [Bind("NalazId,Opis,Dijagnoza,Terapija,TerminID,Termin,PacijentID,Pacijent")] Nalaz nalaz)
    {
        if (nalazid != nalaz.NalazId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(nalaz);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NalazExists(nalaz.NalazId))
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

    // GET: NALAZS/Delete/5
    public async Task<IActionResult> Delete(int? nalazid)
    {
        if (nalazid == null)
        {
            return NotFound();
        }

        var nalaz = await _context.Nalaz
            .FirstOrDefaultAsync(m => m.NalazId == nalazid);
        if (nalaz == null)
        {
            return NotFound();
        }

        return View(nalaz);
    }

    // POST: NALAZS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? nalazid)
    {
        var nalaz = await _context.Nalaz.FindAsync(nalazid);
        if (nalaz != null)
        {
            _context.Nalaz.Remove(nalaz);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool NalazExists(int? nalazid)
    {
        return _context.Nalaz.Any(e => e.NalazId == nalazid);
    }
}
