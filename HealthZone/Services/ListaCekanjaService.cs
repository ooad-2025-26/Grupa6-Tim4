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
            => await _listaCekanjaRepository.GetByIdAsync(id);

        public async Task<IEnumerable<ListaCekanja>> GetAllAsync()
            => await _listaCekanjaRepository.GetAllAsync();

        public void Update(ListaCekanja l) => _listaCekanjaRepository.Update(l);
        public void Delete(ListaCekanja l) => _listaCekanjaRepository.Delete(l);

        public async Task<int> SaveChangesAsync()
            => await _listaCekanjaRepository.SaveChangesAsync();

        // ─── DODAJ LISTU ──────────────────────────────────────────────────────

        public async Task AddAsync(ListaCekanja listaCekanja)
        {
            // Pravilo: svaki doktor može imati samo jednu listu čekanja
            var postojeca = await GetListaZaDoktoraAsync(listaCekanja.DoktorID);
            if (postojeca != null)
                throw new Exception("Ovaj doktor već ima listu čekanja.");

            await _listaCekanjaRepository.AddAsync(listaCekanja);
            await _listaCekanjaRepository.SaveChangesAsync();
        }

        // ─── DOHVATI LISTU ZA DOKTORA ─────────────────────────────────────────

        public async Task<ListaCekanja?> GetListaZaDoktoraAsync(string doktorId)
        {
            var sve = await _listaCekanjaRepository.GetAllAsync();
            return sve.FirstOrDefault(l => l.DoktorID == doktorId);
        }

        // ─── DOHVATI ILI KREIRAJ LISTU ───────────────────────────────────────
        // Korisno u TerminService kada se termin otkaže —
        // ako lista ne postoji, automatski se kreira

        public async Task<ListaCekanja> GetIliKreirajListuAsync(string doktorId)
        {
            var postojeca = await GetListaZaDoktoraAsync(doktorId);
            if (postojeca != null) return postojeca;

            var nova = new ListaCekanja { DoktorID = doktorId };
            await _listaCekanjaRepository.AddAsync(nova);
            await _listaCekanjaRepository.SaveChangesAsync();
            return nova;
        }

        // ─── OBRIŠI LISTU (i sve pacijente s nje) ────────────────────────────

        public async Task ObrisiListuAsync(int listaId)
        {
            var lista = await _listaCekanjaRepository.GetByIdAsync(listaId)
                ?? throw new Exception("Lista čekanja nije pronađena.");

            _listaCekanjaRepository.Delete(lista);
            await _listaCekanjaRepository.SaveChangesAsync();
        }

        // ─── HELPER: Doktori ──────────────────────────────────────────────────

        public async Task<IEnumerable<Korisnik>> GetDoktoriAsync()
        {
            var svi = await _korisnikRepository.GetAllAsync();
            return svi.Where(k => k.Specijalizacija != null);
        }
    }
}