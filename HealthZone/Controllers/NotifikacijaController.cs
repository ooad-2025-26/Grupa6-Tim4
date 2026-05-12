
using HealthZone.Data;
using HealthZone.Models;
using HealthZone.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class NotifikacijaController : Controller
{
    private readonly INotifikacijaService _notifikacijaService;

    public NotifikacijaController(INotifikacijaService notifikacijaService)
    {
        _notifikacijaService = notifikacijaService;
    }

    // GET: NOTIFIKACIJAS
    public async Task<IActionResult> Index()
    {
        var notifikacije = await _notifikacijaService.GetAllAsync();
        return View(notifikacije);
    }

    // GET: NOTIFIKACIJAS/Details/5
    public async Task<IActionResult> Details(int? notifikacijaid)
    {
        if (notifikacijaid == null)
        {
            return NotFound();
        }

        var notifikacija = await _notifikacijaService.GetByIdAsync(notifikacijaid.Value);
        if (notifikacija == null)
        {
            return NotFound();
        }

        return View(notifikacija);
    }

    // GET: NOTIFIKACIJAS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: NOTIFIKACIJAS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("NotifikacijaId,Poruka,DatumSlanja,status,TerminID,Termin")] Notifikacija notifikacija)
    {
        if (ModelState.IsValid)
        {
            await _notifikacijaService.AddAsync(notifikacija);
            await _notifikacijaService.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(notifikacija);
    }

    // GET: NOTIFIKACIJAS/Edit/5
    public async Task<IActionResult> Edit(int? notifikacijaid)
    {
        if (notifikacijaid == null)
        {
            return NotFound();
        }

        var notifikacija = await _notifikacijaService.GetByIdAsync(notifikacijaid.Value);
        if (notifikacija == null)
        {
            return NotFound();
        }
        return View(notifikacija);
    }

    // POST: NOTIFIKACIJAS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? notifikacijaid, [Bind("NotifikacijaId,Poruka,DatumSlanja,status,TerminID,Termin")] Notifikacija notifikacija)
    {
        if (notifikacijaid != notifikacija.NotifikacijaId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _notifikacijaService.Update(notifikacija);
                await _notifikacijaService.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (! await NotifikacijaExists(notifikacija.NotifikacijaId))
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
        return View(notifikacija);
    }

    // GET: NOTIFIKACIJAS/Delete/5
    public async Task<IActionResult> Delete(int? notifikacijaid)
    {
        if (notifikacijaid == null)
        {
            return NotFound();
        }

        var notifikacija = await _notifikacijaService.GetByIdAsync(notifikacijaid.Value);
        if (notifikacija == null)
        {
            return NotFound();
        }

        return View(notifikacija);
    }

    // POST: NOTIFIKACIJAS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? notifikacijaid)
    {
        var notifikacija = await _notifikacijaService.GetByIdAsync(notifikacijaid.Value);
        if (notifikacija != null)
        {
            _notifikacijaService.Delete(notifikacija);
            await _notifikacijaService.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task<bool> NotifikacijaExists(int? notifikacijaid)
    {
        var notifikacija = await _notifikacijaService.GetByIdAsync(notifikacijaid.Value);
        return notifikacija != null;
    }
}
