using HealthZone.Models;
using HealthZone.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HealthZone.Controllers
{
    public class HomeController : Controller
    {
        private readonly IKorisnikService _korisnikService;
        private readonly IRecenzijaService _recenzijaService;

        public HomeController(IKorisnikService korisnikService, IRecenzijaService recenzijaService)
        {
            _korisnikService = korisnikService;
            _recenzijaService = recenzijaService;
        }

        public IActionResult Index()
        {
            return View();
        }

        // GET: /Home/MedicinskoOsoblje
        public async Task<IActionResult> MedicinskoOsoblje()
        {
            var doktori = await _korisnikService.GetDoktoriAsync();
            var sestre = await _korisnikService.GetMedicinskeSestreAsync();

            // Dohvati prosjeène ocjene za svakog doktora
            var ocjene = new Dictionary<string, double>();
            foreach (var doktor in doktori)
            {
                var (_, _, _, ukupno) = await _recenzijaService.GetProsjecneOcjeneAsync(doktor.Id);
                ocjene[doktor.Id] = ukupno;
            }

            ViewBag.Doktori = doktori;
            ViewBag.Sestre = sestre;
            ViewBag.Ocjene = ocjene;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}