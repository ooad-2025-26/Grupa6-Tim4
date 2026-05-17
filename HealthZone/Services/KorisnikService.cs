using HealthZone.Models;
using HealthZone.Repositories;
using Microsoft.AspNetCore.Identity;

namespace HealthZone.Services
{
    public class KorisnikService : IKorisnikService
    {
        private readonly IKorisnikRepository _korisnikRepository;
        private readonly UserManager<Korisnik> _userManager;
        public KorisnikService(IKorisnikRepository korisnikRepository, UserManager<Korisnik> userManager)
        {
            _korisnikRepository = korisnikRepository;
            _userManager = userManager;
        }

        public async Task<Korisnik?> GetByIdAsync(string id)
        {
            return await _korisnikRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Korisnik>> GetAllAsync()
        {
            return await _korisnikRepository.GetAllAsync();
        }


        public async Task AddAsync(Korisnik korisnik, string lozinka, string uloga  = "Pacijent")
        {
            korisnik.UserName = korisnik.Email;

            if (uloga == "Pacijent" && string.IsNullOrEmpty(korisnik.BrojKartona))
                korisnik.BrojKartona = $"HZ-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..5].ToUpper()}";

            await _userManager.CreateAsync(korisnik, lozinka);
            await _userManager.AddToRoleAsync(korisnik, uloga);
        }
        public void Update(Korisnik korisnik)
        {
            _korisnikRepository.Update(korisnik);
        }

        public void Delete(Korisnik korisnik)
        {
            _korisnikRepository.Delete(korisnik);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _korisnikRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<Korisnik>> GetDoktoriAsync()
        {
            return await _userManager.GetUsersInRoleAsync("Doktor");
        }

        public async Task<IEnumerable<Korisnik>> GetPacijentiAsync()
        {
            return await _userManager.GetUsersInRoleAsync("Pacijent");
        }

        public async Task<IEnumerable<Korisnik>> GetMedicinskeSestreAsync()
        {
            return await _userManager.GetUsersInRoleAsync("MedicinskaSestra");
        }
    }
}
