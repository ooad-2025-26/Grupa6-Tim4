using HealthZone.Models;
using HealthZone.Repositories;

namespace HealthZone.Services
{
    public class ListaCekanjaService : IListaCekanjaService
    {
        private readonly IListaCekanjaRepository _listaCekanjaRepository;
        private readonly IKorisnikRepository _korisnikRepository;

        public ListaCekanjaService(
            IListaCekanjaRepository listaCekanjaRepository,
            IKorisnikRepository korisnikRepository)
        {
            _listaCekanjaRepository = listaCekanjaRepository;
            _korisnikRepository = korisnikRepository;
        }

        public async Task<ListaCekanja?> GetByIdAsync(int id)
        {
            return await _listaCekanjaRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<ListaCekanja>> GetAllAsync()
        {
            return await _listaCekanjaRepository.GetAllAsync();
        }

        public async Task AddAsync(ListaCekanja listaCekanja)
        {
            await _listaCekanjaRepository.AddAsync(listaCekanja);
        }

        public void Update(ListaCekanja listaCekanja)
        {
            _listaCekanjaRepository.Update(listaCekanja);
        }

        public void Delete(ListaCekanja listaCekanja)
        {
            _listaCekanjaRepository.Delete(listaCekanja);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _listaCekanjaRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<Korisnik>> GetDoktoriAsync()
        {
            var sviKorisnici = await _korisnikRepository.GetAllAsync();
            return sviKorisnici.Where(k => k.Specijalizacija != null);
        }
    }
}
