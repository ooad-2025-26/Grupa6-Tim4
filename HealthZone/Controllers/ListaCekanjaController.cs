
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthZone.Models;
using HealthZone.Data;

public class ListaCekanjaController : Controller
{
    private readonly ApplicationDbContext _context;

    public ListaCekanjaController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: LISTACEKANJAS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.ListaCekanja.ToListAsync());
    }

    // GET: LISTACEKANJAS/Details/5
    public async Task<IActionResult> Details(int? listaid)
    {
        if (listaid == null)
        {
            return NotFound();
        }

        var listacekanja = await _context.ListaCekanja
            .FirstOrDefaultAsync(m => m.ListaId == listaid);
        if (listacekanja == null)
        {
            return NotFound();
        }

        return View(listacekanja);
    }

    // GET: LISTACEKANJAS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: LISTACEKANJAS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ListaId,DoktorID,Doktor")] ListaCekanja listacekanja)
    {
        if (ModelState.IsValid)
        {
            _context.Add(listacekanja);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(listacekanja);
    }

    // GET: LISTACEKANJAS/Edit/5
    public async Task<IActionResult> Edit(int? listaid)
    {
        if (listaid == null)
        {
            return NotFound();
        }

        var listacekanja = await _context.ListaCekanja.FindAsync(listaid);
        if (listacekanja == null)
        {
            return NotFound();
        }
        return View(listacekanja);
    }

    // POST: LISTACEKANJAS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? listaid, [Bind("ListaId,DoktorID,Doktor")] ListaCekanja listacekanja)
    {
        if (listaid != listacekanja.ListaId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(listacekanja);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ListaCekanjaExists(listacekanja.ListaId))
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
        return View(listacekanja);
    }

    // GET: LISTACEKANJAS/Delete/5
    public async Task<IActionResult> Delete(int? listaid)
    {
        if (listaid == null)
        {
            return NotFound();
        }

        var listacekanja = await _context.ListaCekanja
            .FirstOrDefaultAsync(m => m.ListaId == listaid);
        if (listacekanja == null)
        {
            return NotFound();
        }

        return View(listacekanja);
    }

    // POST: LISTACEKANJAS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? listaid)
    {
        var listacekanja = await _context.ListaCekanja.FindAsync(listaid);
        if (listacekanja != null)
        {
            _context.ListaCekanja.Remove(listacekanja);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ListaCekanjaExists(int? listaid)
    {
        return _context.ListaCekanja.Any(e => e.ListaId == listaid);
    }
}
