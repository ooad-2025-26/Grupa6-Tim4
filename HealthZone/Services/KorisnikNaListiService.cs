using HealthZone.Models;
using HealthZone.Repositories;

namespace HealthZone.Services
{
    public class KorisnikNaListiService : IKorisnikNaListiService
    {
        private readonly IKorisnikNaListiRepository _korisnikNaListiRepository;
        private readonly IListaCekanjaRepository _listaCekanjaRepository;
        private readonly IKorisnikRepository _korisnikRepository;

        public KorisnikNaListiService(
            IKorisnikNaListiRepository korisnikNaListiRepository,
            IListaCekanjaRepository listaCekanjaRepository,
            IKorisnikRepository korisnikRepository)
        {
            _korisnikNaListiRepository = korisnikNaListiRepository;
            _listaCekanjaRepository = listaCekanjaRepository;
            _korisnikRepository = korisnikRepository;
        }

        public async Task<KorisnikNaListi?> GetByIdAsync(int id)
        {
            return await _korisnikNaListiRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<KorisnikNaListi>> GetAllAsync()
        {
            return await _korisnikNaListiRepository.GetAllAsync();
        }

        public async Task AddAsync(KorisnikNaListi korisnikNaListi)
        {
            await _korisnikNaListiRepository.AddAsync(korisnikNaListi);
        }

        public void Update(KorisnikNaListi korisnikNaListi)
        {
            _korisnikNaListiRepository.Update(korisnikNaListi);
        }

        public void Delete(KorisnikNaListi korisnikNaListi)
        {
            _korisnikNaListiRepository.Delete(korisnikNaListi);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _korisnikNaListiRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<ListaCekanja>> GetListeCekanjaAsync()
        {
            return await _listaCekanjaRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Korisnik>> GetKorisniciAsync()
        {
            return await _korisnikRepository.GetAllAsync();
        }
    }
}
