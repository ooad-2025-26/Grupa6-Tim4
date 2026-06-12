using HealthZone.Models;
using HealthZone.Repositories;

namespace HealthZone.Services
{
    public class ZahtjevZaOpremuService : IZahtjevZaOpremuService
    {
        private readonly IZahtjevZaOpremuRepository _zahtjevZaOpremuRepository;
        private readonly IKorisnikRepository _korisnikRepository;

        public ZahtjevZaOpremuService(
            IZahtjevZaOpremuRepository zahtjevRepository,
            IKorisnikRepository korisnikRepository)
        {
            _zahtjevZaOpremuRepository = zahtjevRepository;
            _korisnikRepository = korisnikRepository;
        }

        public async Task<ZahtjevZaOpremu?> GetByIdAsync(int id)
            => await _zahtjevZaOpremuRepository.GetByIdAsync(id);

        public async Task<IEnumerable<ZahtjevZaOpremu>> GetAllAsync()
            => await _zahtjevZaOpremuRepository.GetAllAsync();

        public void Update(ZahtjevZaOpremu z) => _zahtjevZaOpremuRepository.Update(z);
        public void Delete(ZahtjevZaOpremu z) => _zahtjevZaOpremuRepository.Delete(z);

        public async Task<int> SaveChangesAsync()
            => await _zahtjevZaOpremuRepository.SaveChangesAsync();

        // ─── PODNESI ZAHTJEV ──────────────────────────────────────────────────

        public async Task AddAsync(ZahtjevZaOpremu zahtjev)
        {
            if (string.IsNullOrWhiteSpace(zahtjev.Naziv))
                throw new Exception("Naziv opreme je obavezan.");

            if (string.IsNullOrWhiteSpace(zahtjev.Opis))
                throw new Exception("Opis razloga zahtjeva je obavezan.");

            zahtjev.Status = Status.NaCekanju;
            await _zahtjevZaOpremuRepository.AddAsync(zahtjev);
            await _zahtjevZaOpremuRepository.SaveChangesAsync();
        }

        // ─── ODOBRI ZAHTJEV ───────────────────────────────────────────────────

        public async Task OdobriZahtjevAsync(int zahtjevId)
        {
            var zahtjev = await _zahtjevZaOpremuRepository.GetByIdAsync(zahtjevId)
                ?? throw new Exception("Zahtjev nije pronađen.");

            if (zahtjev.Status != Status.NaCekanju)
                throw new Exception("Zahtjev je već obrađen.");

            zahtjev.Status = Status.Potvrdjen;
            _zahtjevZaOpremuRepository.Update(zahtjev);
            await _zahtjevZaOpremuRepository.SaveChangesAsync();
        }

        // ─── ODBIJ ZAHTJEV ────────────────────────────────────────────────────

        public async Task OdbijZahtjevAsync(int zahtjevId)
        {
            var zahtjev = await _zahtjevZaOpremuRepository.GetByIdAsync(zahtjevId)
                ?? throw new Exception("Zahtjev nije pronađen.");

            if (zahtjev.Status != Status.NaCekanju)
                throw new Exception("Zahtjev je već obrađen.");

            zahtjev.Status = Status.Otkazan;
            _zahtjevZaOpremuRepository.Update(zahtjev);
            await _zahtjevZaOpremuRepository.SaveChangesAsync();
        }

        // ─── ZAHTJEVI PO STATUSU ──────────────────────────────────────────────

        public async Task<IEnumerable<ZahtjevZaOpremu>> GetZahtjeviPoStatusuAsync(Status status)
        {
            var svi = await _zahtjevZaOpremuRepository.GetAllAsync();
            return svi.Where(z => z.Status == status);
        }

        // ─── ZAHTJEVI PO HITNOSTI ─────────────────────────────────────────────

        public async Task<IEnumerable<ZahtjevZaOpremu>> GetZahtjeviPoHitnostiAsync()
        {
            var svi = await _zahtjevZaOpremuRepository.GetAllAsync();
            return svi
                .Where(z => z.Status == Status.NaCekanju)
                .OrderByDescending(z => (int)z.Hitnost);
        }

        // ─── ZAHTJEVI DOKTORA ─────────────────────────────────────────────────

        public async Task<IEnumerable<ZahtjevZaOpremu>> GetZahtjeviDoktoraAsync(string doktorId)
        {
            var svi = await _zahtjevZaOpremuRepository.GetAllAsync();
            return svi.Where(z => z.DoktorID == doktorId);
        }

        // ─── ZAHTJEVI PO KATEGORIJI ───────────────────────────────────────────

        public async Task<IEnumerable<ZahtjevZaOpremu>> GetZahtjeviPoKategorijiAsync(KategorijaOpreme kategorija)
        {
            var svi = await _zahtjevZaOpremuRepository.GetAllAsync();
            return svi.Where(z => z.Kategorija == kategorija);
        }

        public async Task<IEnumerable<Korisnik>> GetDoktoriAsync()
        {
            var svi = await _korisnikRepository.GetAllAsync();
            return svi.Where(k => k.Specijalizacija != null);
        }
    }
}