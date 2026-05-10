
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthZone.Models;
using HealthZone.Data;

public class MedicinskaUslugaController : Controller
{
    private readonly ApplicationDbContext _context;

    public MedicinskaUslugaController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: MEDICINSKAUSLUGAS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.MedicinskaUsluga.ToListAsync());
    }

    // GET: MEDICINSKAUSLUGAS/Details/5
    public async Task<IActionResult> Details(int? uslugaid)
    {
        if (uslugaid == null)
        {
            return NotFound();
        }

        var medicinskausluga = await _context.MedicinskaUsluga
            .FirstOrDefaultAsync(m => m.UslugaId == uslugaid);
        if (medicinskausluga == null)
        {
            return NotFound();
        }

        return View(medicinskausluga);
    }

    // GET: MEDICINSKAUSLUGAS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: MEDICINSKAUSLUGAS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("UslugaId,Naziv,Vrsta,Opis,Cijena")] MedicinskaUsluga medicinskausluga)
    {
        if (ModelState.IsValid)
        {
            _context.Add(medicinskausluga);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(medicinskausluga);
    }

    // GET: MEDICINSKAUSLUGAS/Edit/5
    public async Task<IActionResult> Edit(int? uslugaid)
    {
        if (uslugaid == null)
        {
            return NotFound();
        }

        var medicinskausluga = await _context.MedicinskaUsluga.FindAsync(uslugaid);
        if (medicinskausluga == null)
        {
            return NotFound();
        }
        return View(medicinskausluga);
    }

    // POST: MEDICINSKAUSLUGAS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? uslugaid, [Bind("UslugaId,Naziv,Vrsta,Opis,Cijena")] MedicinskaUsluga medicinskausluga)
    {
        if (uslugaid != medicinskausluga.UslugaId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(medicinskausluga);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MedicinskaUslugaExists(medicinskausluga.UslugaId))
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
        return View(medicinskausluga);
    }

    // GET: MEDICINSKAUSLUGAS/Delete/5
    public async Task<IActionResult> Delete(int? uslugaid)
    {
        if (uslugaid == null)
        {
            return NotFound();
        }

        var medicinskausluga = await _context.MedicinskaUsluga
            .FirstOrDefaultAsync(m => m.UslugaId == uslugaid);
        if (medicinskausluga == null)
        {
            return NotFound();
        }

        return View(medicinskausluga);
    }

    // POST: MEDICINSKAUSLUGAS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? uslugaid)
    {
        var medicinskausluga = await _context.MedicinskaUsluga.FindAsync(uslugaid);
        if (medicinskausluga != null)
        {
            _context.MedicinskaUsluga.Remove(medicinskausluga);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool MedicinskaUslugaExists(int? uslugaid)
    {
        return _context.MedicinskaUsluga.Any(e => e.UslugaId == uslugaid);
    }
}
