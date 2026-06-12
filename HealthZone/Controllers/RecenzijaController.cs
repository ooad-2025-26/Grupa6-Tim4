using HealthZone.Models;
using HealthZone.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthZone.Controllers
{
    public class RecenzijaController : Controller
    {
        private readonly IRecenzijaService _recenzijaService;
        private readonly IKorisnikService _korisnikService;
        private readonly UserManager<Korisnik> _userManager;

        public RecenzijaController(
            IRecenzijaService recenzijaService,
            IKorisnikService korisnikService,
            UserManager<Korisnik> userManager)
        {
            _recenzijaService = recenzijaService;
            _korisnikService = korisnikService;
            _userManager = userManager;
        }

        // GET: Recenzija � javno dostupno svima
        public async Task<IActionResult> Index()
        {
            var recenzije = await _recenzijaService.GetAllAsync();
            return View(recenzije);
        }

        // GET: Recenzija/RecenzijeDoktora/{id} – javno dostupno, recenzije određenog doktora
        public async Task<IActionResult> RecenzijeDoktora(string id)
        {
            var doktor = await _korisnikService.GetDoktorByIdAsync(id);
            if (doktor == null) return NotFound();

            var recenzije = await _recenzijaService.GetRecenzijeDoktoraAsync(id);
            var ocjene = await _recenzijaService.GetProsjecneOcjeneAsync(id);

            ViewBag.Doktor = doktor;
            ViewBag.Ocjene = new
            {
                ocjene.Ljubaznost,
                ocjene.Profesionalnost,
                ocjene.Usluga,
                ocjene.Ukupno
            };

            ViewBag.MozeOstaviti = User.IsInRole("Pacijent");

            return View(recenzije);
        }

        // GET: Recenzija/MojeRecenzije � doktor vidi svoje ocjene
        [Authorize(Roles = "Doktor")]
        public async Task<IActionResult> MojeRecenzije()
        {
            var doktor = await _userManager.GetUserAsync(User);
            var recenzije = await _recenzijaService.GetRecenzijeDoktoraAsync(doktor!.Id);
            var ocjene = await _recenzijaService.GetProsjecneOcjeneAsync(doktor.Id);

            ViewBag.Ocjene = new
            {
                ocjene.Ljubaznost,
                ocjene.Profesionalnost,
                ocjene.Usluga,
                ocjene.Ukupno
            };

            return View(recenzije);
        }

        // GET: Recenzija/Create
        [Authorize(Roles = "Pacijent")]
        public async Task<IActionResult> Create(string? doktorId = null)
        {
            var doktori = await _korisnikService.GetDoktoriAsync();

            ViewData["DoktorID"] = new SelectList(
                doktori.Select(d => new
                {
                    d.Id,
                    PunoIme = d.Ime + " " + d.Prezime
                }),
                "Id",
                "PunoIme",
                doktorId
            );
            return View();
        }

        // POST: Recenzija/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Pacijent")]
        public async Task<IActionResult> Create([Bind("Komentar,OcjenaLjubaznosti,OcjenaProfesionalnosti,OcjenaUsluge,DoktorID")] Recenzija recenzija)
        {
            var pacijent = await _userManager.GetUserAsync(User);
            recenzija.PacijentID = pacijent!.Id;

            ModelState.Remove("PacijentID");

            if (ModelState.IsValid)
            {
                try
                {
                    await _recenzijaService.AddAsync(recenzija);
                    TempData["Uspjeh"] = "Recenzija je uspje�no dodana.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            var doktori = await _korisnikService.GetDoktoriAsync();
            ViewData["DoktorID"] = new SelectList(
                doktori.Select(d => new
                {
                    d.Id,
                    PunoIme = d.Ime + " " + d.Prezime
                }),
                "Id",
                "PunoIme",
                recenzija.DoktorID
                );
            return View(recenzija);
        }

        // POST: Recenzija/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Pacijent")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recenzija = await _recenzijaService.GetByIdAsync(id);
            if (recenzija != null)
            {
                var korisnik = await _userManager.GetUserAsync(User);
                if (!User.IsInRole("Administrator") && recenzija.PacijentID != korisnik!.Id)
                {
                    return Forbid();
                }
                _recenzijaService.Delete(recenzija);
                await _recenzijaService.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}