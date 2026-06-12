using HealthZone.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HealthZone.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<Korisnik> _signInManager;
        private readonly UserManager<Korisnik> _userManager;

        public AccountController(SignInManager<Korisnik> signInManager, UserManager<Korisnik> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // GET: Account/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string lozinka, bool zapamtiMe = false, string? returnUrl = null)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(lozinka))
            {
                ViewData["Greška"] = "Email i lozinka su obavezni.";
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(email, lozinka, zapamtiMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }

            ViewData["Greška"] = "Pogrešan email ili lozinka.";
            return View();
        }

        // POST: Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        // GET: Account/AccessDenied
        public IActionResult AccessDenied()
        {
            return View();
        }

        // GET: Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string ime, string prezime, string email, string telefon, string lozinka, string potvrdiLozinku)
        {
            if (string.IsNullOrEmpty(ime) || string.IsNullOrEmpty(prezime) ||
                string.IsNullOrEmpty(email) || string.IsNullOrEmpty(lozinka))
            {
                ViewData["Greška"] = "Sva polja osim telefona su obavezna.";
                return View();
            }

            if (lozinka != potvrdiLozinku)
            {
                ViewData["Greška"] = "Lozinke se ne podudaraju.";
                return View();
            }

            var korisnik = new Korisnik
            {
                Ime = ime,
                Prezime = prezime,
                Email = email,
                UserName = email,
                PhoneNumber = string.IsNullOrEmpty(telefon) ? null : telefon, // Dodaj telefon
                BrojKartona = $"HZ-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..5].ToUpper()}",
                Prioritet = null // Postavi prioritet na null
            };

            var result = await _userManager.CreateAsync(korisnik, lozinka);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(korisnik, "Pacijent");
                await _signInManager.SignInAsync(korisnik, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            var prevedeneGreske = result.Errors.Select(e => e.Code switch
            {
                "PasswordTooShort" => "Lozinka mora imati najmanje 6 znakova.",
                "PasswordRequiresNonAlphanumeric" => "Lozinka mora sadržavati najmanje jedan specijalni znak (npr. !, @, #).",
                "PasswordRequiresLower" => "Lozinka mora sadržavati najmanje jedno malo slovo.",
                "PasswordRequiresUpper" => "Lozinka mora sadržavati najmanje jedno veliko slovo.",
                "PasswordRequiresDigit" => "Lozinka mora sadržavati najmanje jedan broj.",
                "DuplicateEmail" => "Ovaj email je već registrovan.",
                "DuplicateUserName" => "Ovaj email je već u upotrebi.",
                "InvalidEmail" => "Email adresa nije ispravnog formata.",
                _ => e.Description
            });
            ViewData["Greška"] = string.Join(" ", prevedeneGreske); return View();
        }
    }
}