using HealthZone.Models;
using HealthZone.Repositories;

namespace HealthZone.Services
{
    public class ZahtjevZaOpremuService : IZahtjevZaOpremuService
    {
        private readonly IZahtjevZaOpremuRepository _zahtjevZaOpremuRepository;
        private readonly IKorisnikRepository _korisnikRepository;  // ← DODAJTE OVO

        public ZahtjevZaOpremuService(
            IZahtjevZaOpremuRepository zahtjevRepository,
            IKorisnikRepository korisnikRepository)  // ← DODAJTE OVO
        {
            _zahtjevZaOpremuRepository = zahtjevRepository;
            _korisnikRepository = korisnikRepository;
        }
        public async Task<ZahtjevZaOpremu?> GetByIdAsync(int id)
        {
            return await _zahtjevZaOpremuRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<ZahtjevZaOpremu>> GetAllAsync()
        {
            return await _zahtjevZaOpremuRepository.GetAllAsync();
        }

        public async Task AddAsync(ZahtjevZaOpremu zahtjevZaOpremu)
        {
            await _zahtjevZaOpremuRepository.AddAsync(zahtjevZaOpremu);
        }

        public void Update(ZahtjevZaOpremu zahtjevZaOpremu)
        {
            _zahtjevZaOpremuRepository.Update(zahtjevZaOpremu);
        }

        public void Delete(ZahtjevZaOpremu zahtjevZaOpremu)
        {
            _zahtjevZaOpremuRepository.Delete(zahtjevZaOpremu);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _zahtjevZaOpremuRepository.SaveChangesAsync();
        }


        public async Task<IEnumerable<Korisnik>> GetDoktoriAsync()
        {
            var sviKorisnici = await _korisnikRepository.GetAllAsync();
            return sviKorisnici.Where(k => k.Specijalizacija != null);
        }
    }
}
