using HealthZone.Models;
using HealthZone.Repositories;

namespace HealthZone.Services
{
    public class KorisnikService : IKorisnikService
    {
        private readonly IKorisnikRepository _korisnikRepository;

        public KorisnikService(IKorisnikRepository korisnikRepository)
        {
            _korisnikRepository = korisnikRepository;
        }

        public async Task<Korisnik?> GetByIdAsync(string id)
        {
            return await _korisnikRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Korisnik>> GetAllAsync()
        {
            return await _korisnikRepository.GetAllAsync();
        }

        public async Task AddAsync(Korisnik korisnik)
        {
            await _korisnikRepository.AddAsync(korisnik);
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
            var sviKorisnici = await _korisnikRepository.GetAllAsync();
            return sviKorisnici.Where(k => k.Specijalizacija != null);
        }

        public async Task<IEnumerable<Korisnik>> GetPacijentiAsync()
        {
            var sviKorisnici = await _korisnikRepository.GetAllAsync();
            return sviKorisnici.Where(k => k.Prioritet != null);
        }
    }
}
