using HealthZone.Data;
using HealthZone.Models;
using HealthZone.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HealthZone.Controllers
{
    public class KorisnikController : Controller
    {
        private readonly IKorisnikService _korisnikService;
        private readonly ITerminService _terminService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<Korisnik> _userManager;
        private readonly ApplicationDbContext _context;

        public KorisnikController(
            IKorisnikService korisnikService,
            ITerminService terminService,
            RoleManager<IdentityRole> roleManager,
            UserManager<Korisnik> userManager,
            ApplicationDbContext context)
        {
            _korisnikService = korisnikService;
            _terminService = terminService;
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
        }

        // GET: Korisnik
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Index()
        {
            var korisnici = await _korisnikService.GetAllAsync();

            var ulogeMap = new Dictionary<string, string>();
            foreach (var k in korisnici)
            {
                var uloga = await _korisnikService.GetUlogaAsync(k);
                ulogeMap[k.Id] = uloga ?? "—";
            }
            ViewBag.UlogeMap = ulogeMap;

            ViewBag.SveUloge = await _roleManager.Roles
                .Select(r => r.Name)
                .OrderBy(n => n)
                .ToListAsync();

            return View(korisnici);
        }

        // GET: Korisnik/Details/id
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null) return NotFound();

            var korisnik = await _korisnikService.GetByIdAsync(id);
            if (korisnik == null) return NotFound();
            ViewBag.Uloga = await _korisnikService.GetUlogaAsync(korisnik);
            return View(korisnik);
        }

        // GET: Korisnik/Create
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create()
        {
            await PopuniViewBag();
            return View();
        }

        // POST: Korisnik/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Korisnik korisnik, string lozinka, string uloga = "Pacijent")
        {
            if (ModelState.IsValid)
            {
                try
                {
                    korisnik.Prioritet = Prioritet.Nizak;
                    await _korisnikService.AddAsync(korisnik, lozinka, uloga);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            await PopuniViewBag();
            return View(korisnik);
        }

        // GET: Korisnik/Edit/id
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
                return NotFound();

            var korisnik = await _korisnikService.GetByIdAsync(id);
            if (korisnik == null)
                return NotFound();

            var trenutni = await _userManager.GetUserAsync(User);
            if (!User.IsInRole("Administrator") && trenutni!.Id != korisnik.Id)
                return Forbid();

            ViewBag.TrenutnaUloga = await _korisnikService.GetUlogaAsync(korisnik);
            await PopuniViewBag();

            return View(korisnik);
        }

        // POST: Korisnik/Edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Korisnik korisnik, string uloga, string? novaLozinka, string? potvrdalozinke)
        {
            if (id != korisnik.Id)
                return NotFound();

            var trenutni = await _userManager.GetUserAsync(User);
            if (!User.IsInRole("Administrator") && trenutni!.Id != korisnik.Id)
                return Forbid();

            try
            {
                var postojeci = await _userManager.FindByIdAsync(id);
                if (postojeci == null) return NotFound();

                // Regeneriši SecurityStamp ako nedostaje (stari korisnici)
                if (string.IsNullOrEmpty(postojeci.SecurityStamp))
                {
                    await _userManager.UpdateSecurityStampAsync(postojeci);
                    postojeci = await _userManager.FindByIdAsync(id);
                }

                if (User.IsInRole("Administrator"))
                {
                    postojeci!.Ime = korisnik.Ime;
                    postojeci.Prezime = korisnik.Prezime;
                    postojeci.Email = korisnik.Email;
                    postojeci.UserName = korisnik.Email;
                    postojeci.PhoneNumber = korisnik.PhoneNumber;
                    postojeci.Specijalizacija = korisnik.Specijalizacija;
                    postojeci.Prioritet = korisnik.Prioritet;
                    postojeci.BrojKartona = korisnik.BrojKartona;

                    if (uloga == "Pacijent" && string.IsNullOrEmpty(postojeci.BrojKartona))
                        postojeci.BrojKartona = $"HZ-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..5].ToUpper()}";

                    var updateRez = await _userManager.UpdateAsync(postojeci);
                    if (!updateRez.Succeeded)
                        throw new Exception(string.Join("; ", updateRez.Errors.Select(e => e.Description)));

                    if (!string.IsNullOrEmpty(uloga))
                        await _korisnikService.PromijeniUloguAsync(id, uloga);

                    // Učitaj SVJEŽI objekat nakon UpdateAsync
                    if (!string.IsNullOrWhiteSpace(novaLozinka))
                    {
                        var svjezi = await _userManager.FindByIdAsync(id);
                        var token = await _userManager.GeneratePasswordResetTokenAsync(svjezi!);
                        var lozinkaRez = await _userManager.ResetPasswordAsync(svjezi!, token, novaLozinka);
                        if (!lozinkaRez.Succeeded)
                            throw new Exception(string.Join("; ", lozinkaRez.Errors.Select(e => e.Description)));
                    }
                }
                else
                {
                    postojeci!.Email = korisnik.Email;
                    postojeci.UserName = korisnik.Email;
                    postojeci.PhoneNumber = korisnik.PhoneNumber;

                    var updateRez = await _userManager.UpdateAsync(postojeci);
                    if (!updateRez.Succeeded)
                        throw new Exception(string.Join("; ", updateRez.Errors.Select(e => e.Description)));

                    // DODAJ OVO - promjena lozinke za običnog korisnika
                    if (!string.IsNullOrWhiteSpace(novaLozinka))
                    {
                        if (novaLozinka != potvrdalozinke)
                            throw new Exception("Lozinke se ne podudaraju.");

                        var svjezi = await _userManager.FindByIdAsync(id);
                        var token = await _userManager.GeneratePasswordResetTokenAsync(svjezi!);
                        var lozinkaRez = await _userManager.ResetPasswordAsync(svjezi!, token, novaLozinka);
                        if (!lozinkaRez.Succeeded)
                            throw new Exception(string.Join("; ", lozinkaRez.Errors.Select(e => e.Description)));
                    }
                }

                return RedirectToAction(nameof(Details), new { id = id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            var svjeziZaViewBag = await _userManager.FindByIdAsync(id);
            ViewBag.TrenutnaUloga = svjeziZaViewBag != null
                ? await _korisnikService.GetUlogaAsync(svjeziZaViewBag)
                : uloga;
            await PopuniViewBag();
            return View(korisnik);
        }

        // GET: Korisnik/Delete/id
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null) return NotFound();

            var korisnik = await _korisnikService.GetByIdAsync(id);
            if (korisnik == null) return NotFound();
            ViewBag.Uloga = await _korisnikService.GetUlogaAsync(korisnik);
            return View(korisnik);
        }

        // POST: Korisnik/Delete/id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var korisnik = await _userManager.FindByIdAsync(id);
            if (korisnik == null) return NotFound();

            var uloga = await _korisnikService.GetUlogaAsync(korisnik);

            try
            {
                // 1. Obriši KorisnikNaListi zapise (vezani za ListaCekanja i Korisnika)
                var naListi = await _context.KorisnikNaListi
                    .Where(k => k.KorisnikID == id)
                    .ToListAsync();
                _context.KorisnikNaListi.RemoveRange(naListi);

                // 2. Obriši ListeCekanja gdje je ovaj korisnik doktor
                var listeCekanja = await _context.ListaCekanja
                    .Where(l => l.DoktorID == id)
                    .ToListAsync();

                // Za svaku listu čekanja obriši i njene KorisnikNaListi zapise
                var listeIds = listeCekanja.Select(l => l.ListaId).ToList();
                var naListiZaListe = await _context.KorisnikNaListi
                    .Where(k => listeIds.Contains(k.ListaID))
                    .ToListAsync();
                _context.KorisnikNaListi.RemoveRange(naListiZaListe);
                _context.ListaCekanja.RemoveRange(listeCekanja);

                // 3. Dohvati termine korisnika
                var termini = await _context.Termin
                    .Where(t => t.DoktorID == id || t.PacijentID == id)
                    .ToListAsync();
                var terminIds = termini.Select(t => t.TerminId).ToList();

                // 4. Obriši notifikacije vezane za termine
                var notifikacije = await _context.Notifikacija
                    .Where(n => terminIds.Contains(n.TerminID))
                    .ToListAsync();
                _context.Notifikacija.RemoveRange(notifikacije);

                // 5. Obriši nalaze vezane za termine
                var nalazi = await _context.Nalaz
                    .Where(n => terminIds.Contains(n.TerminID))
                    .ToListAsync();
                _context.Nalaz.RemoveRange(nalazi);

                // 6. Obriši termine
                _context.Termin.RemoveRange(termini);

                // 7. Obriši recenzije
                var recenzije = await _context.Recenzija
                    .Where(r => r.DoktorID == id || r.PacijentID == id)
                    .ToListAsync();
                _context.Recenzija.RemoveRange(recenzije);

                // 8. Sačuvaj sve odjednom
                await _context.SaveChangesAsync();

                // 9. Obriši korisnika
                var rezultat = await _userManager.DeleteAsync(korisnik);
                if (!rezultat.Succeeded)
                    throw new Exception(string.Join("; ", rezultat.Errors.Select(e => e.Description)));

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                var poruka = ex.InnerException?.Message ?? ex.Message;
                ViewBag.GreskaBrisanja = poruka;
                ViewBag.Uloga = uloga;
                return View(korisnik);
            }
        }

        // Popravi SecurityStamp za sve korisnike koji ga nemaju (jednokratno)
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> PopraviStampove()
        {
            var svi = await _korisnikService.GetAllAsync();
            int broj = 0;
            foreach (var k in svi)
            {
                if (string.IsNullOrEmpty(k.SecurityStamp))
                {
                    await _userManager.UpdateSecurityStampAsync(k);
                    broj++;
                }
            }
            return Content($"Gotovo — popavljeno {broj} korisnika.");
        }

        private async Task<bool> KorisnikExists(string id)
        {
            var korisnik = await _korisnikService.GetByIdAsync(id);
            return korisnik != null;
        }

        private async Task PopuniViewBag()
        {
            ViewBag.Prioriteti = new SelectList(Enum.GetValues(typeof(Prioritet)));
            ViewBag.Specijalizacije = new SelectList(Enum.GetValues(typeof(Specijalizacija)));
            var uloge = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            ViewBag.Uloge = new SelectList(uloge);
        }
    }
}