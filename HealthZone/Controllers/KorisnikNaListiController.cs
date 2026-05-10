
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthZone.Models;
using HealthZone.Data;

public class KorisnikNaListiController : Controller
{
    private readonly ApplicationDbContext _context;

    public KorisnikNaListiController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: KORISNIKNALISTIS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.KorisnikNaListi.ToListAsync());
    }

    // GET: KORISNIKNALISTIS/Details/5
    public async Task<IActionResult> Details(int? korisniknalistiid)
    {
        if (korisniknalistiid == null)
        {
            return NotFound();
        }

        var korisniknalisti = await _context.KorisnikNaListi
            .FirstOrDefaultAsync(m => m.KorisnikNaListiID == korisniknalistiid);
        if (korisniknalisti == null)
        {
            return NotFound();
        }

        return View(korisniknalisti);
    }

    // GET: KORISNIKNALISTIS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: KORISNIKNALISTIS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("KorisnikNaListiID,Datum,ListaID,Lista,KorisnikID,Korisnik")] KorisnikNaListi korisniknalisti)
    {
        if (ModelState.IsValid)
        {
            _context.Add(korisniknalisti);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(korisniknalisti);
    }

    // GET: KORISNIKNALISTIS/Edit/5
    public async Task<IActionResult> Edit(int? korisniknalistiid)
    {
        if (korisniknalistiid == null)
        {
            return NotFound();
        }

        var korisniknalisti = await _context.KorisnikNaListi.FindAsync(korisniknalistiid);
        if (korisniknalisti == null)
        {
            return NotFound();
        }
        return View(korisniknalisti);
    }

    // POST: KORISNIKNALISTIS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? korisniknalistiid, [Bind("KorisnikNaListiID,Datum,ListaID,Lista,KorisnikID,Korisnik")] KorisnikNaListi korisniknalisti)
    {
        if (korisniknalistiid != korisniknalisti.KorisnikNaListiID)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(korisniknalisti);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KorisnikNaListiExists(korisniknalisti.KorisnikNaListiID))
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
        return View(korisniknalisti);
    }

    // GET: KORISNIKNALISTIS/Delete/5
    public async Task<IActionResult> Delete(int? korisniknalistiid)
    {
        if (korisniknalistiid == null)
        {
            return NotFound();
        }

        var korisniknalisti = await _context.KorisnikNaListi
            .FirstOrDefaultAsync(m => m.KorisnikNaListiID == korisniknalistiid);
        if (korisniknalisti == null)
        {
            return NotFound();
        }

        return View(korisniknalisti);
    }

    // POST: KORISNIKNALISTIS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? korisniknalistiid)
    {
        var korisniknalisti = await _context.KorisnikNaListi.FindAsync(korisniknalistiid);
        if (korisniknalisti != null)
        {
            _context.KorisnikNaListi.Remove(korisniknalisti);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool KorisnikNaListiExists(int? korisniknalistiid)
    {
        return _context.KorisnikNaListi.Any(e => e.KorisnikNaListiID == korisniknalistiid);
    }
}
