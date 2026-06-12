using HealthZone.Models;
using HealthZone.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HealthZone.Controllers
{
    [Authorize(Roles = "Doktor,Administrator")]
    public class ListaCekanjaController : Controller
    {
        private readonly IListaCekanjaService _listaCekanjaService;
        private readonly IKorisnikNaListiService _korisnikNaListiService;
        private readonly UserManager<Korisnik> _userManager;

        public ListaCekanjaController(
            IListaCekanjaService listaCekanjaService,
            IKorisnikNaListiService korisnikNaListiService,
            UserManager<Korisnik> userManager)
        {
            _listaCekanjaService = listaCekanjaService;
            _korisnikNaListiService = korisnikNaListiService;
            _userManager = userManager;
        }

        // GET: ListaCekanja
        public async Task<IActionResult> Index()
        {
            var korisnik = await _userManager.GetUserAsync(User);
            bool jeDoktor = User.IsInRole("Doktor");

            ListaCekanja? mojaLista = null;
            if (jeDoktor && korisnik != null)
                mojaLista = await _listaCekanjaService.GetListaZaDoktoraAsync(korisnik.Id);

            ViewData["ImaListu"] = mojaLista != null;
            ViewData["MojaListaId"] = mojaLista?.ListaId;
            ViewData["JeDoktor"] = jeDoktor;

            // ✅ Doktor vidi samo svoju, admin vidi sve
            IEnumerable<ListaCekanja> liste;
            if (jeDoktor)
                liste = mojaLista != null ? new[] { mojaLista } : Enumerable.Empty<ListaCekanja>();
            else
                liste = await _listaCekanjaService.GetAllAsync();

            return View(liste);
        }

        // GET: ListaCekanja/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var listaCekanja = await _listaCekanjaService.GetByIdAsync(id.Value);
            if (listaCekanja == null) return NotFound();
            return View(listaCekanja);
        }

        // GET: ListaCekanja/Create
        [Authorize(Roles = "Doktor")]
        public async Task<IActionResult> Create()
        {
            var korisnik = await _userManager.GetUserAsync(User);
            if (korisnik == null) return Unauthorized();

            // Ako već ima listu — redirect na Index
            var postojeca = await _listaCekanjaService.GetListaZaDoktoraAsync(korisnik.Id);
            if (postojeca != null)
                return RedirectToAction(nameof(Index));

            return View();
        }

        // POST: ListaCekanja/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Doktor,Administrator")]
        public async Task<IActionResult> Create([Bind("ListaId")] ListaCekanja listaCekanja)
        {
            var korisnik = await _userManager.GetUserAsync(User);
            if (korisnik == null) return Unauthorized();

            // Dvostruka provjera — ne smije imati već listu
            var postojeca = await _listaCekanjaService.GetListaZaDoktoraAsync(korisnik.Id);
            if (postojeca != null)
                return RedirectToAction(nameof(Index));

            // Automatski postavi prijavljenog doktora
            listaCekanja.DoktorID = korisnik.Id;

            // ModelState može imati grešku za DoktorID jer smo ga ručno setali
            ModelState.Remove("DoktorID");
            ModelState.Remove("Doktor");

            if (ModelState.IsValid)
            {
                try
                {
                    await _listaCekanjaService.AddAsync(listaCekanja);
                    TempData["Success"] = "Lista čekanja je uspješno kreirana.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            return View(listaCekanja);
        }

        // GET: ListaCekanja/Edit/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var listaCekanja = await _listaCekanjaService.GetByIdAsync(id.Value);
            if (listaCekanja == null) return NotFound();
            await PopuniDropDownListe(listaCekanja.DoktorID);
            return View(listaCekanja);
        }

        // POST: ListaCekanja/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id, [Bind("ListaId,DoktorID")] ListaCekanja listaCekanja)
        {
            if (id != listaCekanja.ListaId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _listaCekanjaService.Update(listaCekanja);
                    await _listaCekanjaService.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ListaCekanjaExists(listaCekanja.ListaId))
                        return NotFound();
                    else
                        throw;
                }
            }

            await PopuniDropDownListe(listaCekanja.DoktorID);
            return View(listaCekanja);
        }

        // GET: ListaCekanja/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var listaCekanja = await _listaCekanjaService.GetByIdAsync(id.Value);
            if (listaCekanja == null) return NotFound();
            return View(listaCekanja);
        }

        // POST: ListaCekanja/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var listaCekanja = await _listaCekanjaService.GetByIdAsync(id);
            if (listaCekanja != null)
            {
                _listaCekanjaService.Delete(listaCekanja);
                await _listaCekanjaService.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: ListaCekanja/Pregled/5
        [Authorize(Roles ="Administrator,Doktor")]
        public async Task<IActionResult> Pregled(int? id)
        {
            if (id == null) return NotFound();

            var listaCekanja = await _listaCekanjaService.GetByIdAsync(id.Value);
            if (listaCekanja == null) return NotFound();

            var svi = await _korisnikNaListiService.GetAllAsync();
            var pacijenti = svi
                .Where(k => k.ListaID == id.Value)
                .OrderByDescending(k => (int)(k.Korisnik?.Prioritet ?? Prioritet.Nizak))
                .ThenBy(k => k.Datum);

            ViewData["ListaId"] = listaCekanja.ListaId;
            ViewData["DoktorIme"] = listaCekanja.Doktor != null
                ? $"{listaCekanja.Doktor.Ime} {listaCekanja.Doktor.Prezime}"
                : "—";

            return View(pacijenti);
        }

        private async Task<bool> ListaCekanjaExists(int id)
        {
            var lista = await _listaCekanjaService.GetByIdAsync(id);
            return lista != null;
        }

        private async Task PopuniDropDownListe(string? selectedDoktorId = null)
        {
            var doktori = await _listaCekanjaService.GetDoktoriAsync();
            var items = doktori.Select(d => new SelectListItem
            {
                Value = d.Id,
                Text = $"{d.Ime} {d.Prezime}"
            });
            ViewData["DoktorID"] = new SelectList(items, "Value", "Text", selectedDoktorId);
        }
    }
}