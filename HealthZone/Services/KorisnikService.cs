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
            => await _korisnikRepository.GetByIdAsync(id);

        public async Task<IEnumerable<Korisnik>> GetAllAsync()
            => await _korisnikRepository.GetAllAsync();

        public void Update(Korisnik korisnik)
            => _korisnikRepository.Update(korisnik);

        // Koristiti ovu metodu umjesto Update() za edit forme
        public async Task AzurirajAsync(Korisnik korisnik)
        {
            var postojeci = await _userManager.FindByIdAsync(korisnik.Id)
                ?? throw new Exception("Korisnik nije pronađen.");

            postojeci.Ime = korisnik.Ime;
            postojeci.Prezime = korisnik.Prezime;
            postojeci.Email = korisnik.Email;
            postojeci.UserName = korisnik.Email;
            postojeci.PhoneNumber = korisnik.PhoneNumber;
            postojeci.BrojKartona = korisnik.BrojKartona;
            postojeci.Prioritet = korisnik.Prioritet;
            postojeci.Specijalizacija = korisnik.Specijalizacija;

            var result = await _userManager.UpdateAsync(postojeci);
            if (!result.Succeeded)
                throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        public async Task PromijeniUloguAsync(string korisnikId, string novaUloga)
        {
            var korisnik = await _userManager.FindByIdAsync(korisnikId)
                ?? throw new Exception("Korisnik nije pronađen.");

            var trenutneUloge = await _userManager.GetRolesAsync(korisnik);
            if (trenutneUloge.Any())
                await _userManager.RemoveFromRolesAsync(korisnik, trenutneUloge);

            var result = await _userManager.AddToRoleAsync(korisnik, novaUloga);
            if (!result.Succeeded)
                throw new Exception($"Uloga '{novaUloga}' ne postoji.");
        }

        public async Task<int> SaveChangesAsync()
            => await _korisnikRepository.SaveChangesAsync();

        public async Task AddAsync(Korisnik korisnik, string lozinka, string uloga = "Pacijent")
        {
            if (string.IsNullOrWhiteSpace(lozinka))
                throw new Exception("Lozinka je obavezna.");

            korisnik.UserName = korisnik.Email;

            if (uloga == "Pacijent" && string.IsNullOrEmpty(korisnik.BrojKartona))
                korisnik.BrojKartona = $"HZ-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..5].ToUpper()}";

            if (uloga == "Doktor" && korisnik.Specijalizacija == null)
                throw new Exception("Doktor mora imati odabranu specijalizaciju.");

            var result = await _userManager.CreateAsync(korisnik, lozinka);
            if (!result.Succeeded)
                throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));

            var roleResult = await _userManager.AddToRoleAsync(korisnik, uloga);
            if (!roleResult.Succeeded)
                throw new Exception($"Uloga '{uloga}' ne postoji. Provjerite seed uloga u Program.cs.");
        }

        public async Task DeleteAsync(Korisnik korisnik)
        {
            var result = await _userManager.DeleteAsync(korisnik);
            if (!result.Succeeded)
                throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        public void Delete(Korisnik korisnik)
            => _userManager.DeleteAsync(korisnik).Wait();

        public async Task PromijeniLozinkuAsync(string korisnikId, string staraLozinka, string novaLozinka)
        {
            var korisnik = await _userManager.FindByIdAsync(korisnikId)
                ?? throw new Exception("Korisnik nije pronađen.");

            var result = await _userManager.ChangePasswordAsync(korisnik, staraLozinka, novaLozinka);
            if (!result.Succeeded)
                throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        public async Task<IEnumerable<Korisnik>> GetDoktoriAsync()
            => await _userManager.GetUsersInRoleAsync("Doktor");

        public async Task<IEnumerable<Korisnik>> GetPacijentiAsync()
            => await _userManager.GetUsersInRoleAsync("Pacijent");

        public async Task<IEnumerable<Korisnik>> GetMedicinskeSestreAsync()
            => await _userManager.GetUsersInRoleAsync("MedicinskaSestra");

        public async Task<IEnumerable<Korisnik>> GetDoktoriPoSpecijalizacijiAsync(Specijalizacija specijalizacija)
        {
            var doktori = await GetDoktoriAsync();
            return doktori.Where(d => d.Specijalizacija == specijalizacija);
        }

        public async Task<string> GetUlogaAsync(Korisnik korisnik)
        {
            var uloge = await _userManager.GetRolesAsync(korisnik);
            return uloge.FirstOrDefault() ?? "Nepoznato";
        }
        public async Task<Korisnik?> GetDoktorByIdAsync(string id)
        {
            var korisnik = await _userManager.FindByIdAsync(id);

            if (korisnik == null)
                return null;

            if (!await _userManager.IsInRoleAsync(korisnik, "Doktor"))
                return null;

            return korisnik;
        }
    }
}
