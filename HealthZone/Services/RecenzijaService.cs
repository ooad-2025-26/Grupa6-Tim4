using HealthZone.Models;
using HealthZone.Repositories;

namespace HealthZone.Services
{
    public class RecenzijaService : IRecenzijaService
    {
        private readonly IRecenzijaRepository _recenzijaRepository;
        private readonly IKorisnikRepository _korisnikRepository;  // ← DODAJTE

        public RecenzijaService(
            IRecenzijaRepository recenzijaRepository,
            IKorisnikRepository korisnikRepository)  // ← DODAJTE
        {
            _recenzijaRepository = recenzijaRepository;
            _korisnikRepository = korisnikRepository;
        }

        public async Task<Recenzija?> GetByIdAsync(int id)
        {
            return await _recenzijaRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Recenzija>> GetAllAsync()
        {
            return await _recenzijaRepository.GetAllAsync();
        }

        public async Task AddAsync(Recenzija recenzija)
        {
            await _recenzijaRepository.AddAsync(recenzija);
        }

        public void Update(Recenzija recenzija)
        {
            _recenzijaRepository.Update(recenzija);
        }

        public void Delete(Recenzija recenzija)
        {
            _recenzijaRepository.Delete(recenzija);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _recenzijaRepository.SaveChangesAsync();
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
