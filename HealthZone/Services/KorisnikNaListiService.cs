using HealthZone.Models;
using HealthZone.Repositories;

namespace HealthZone.Services
{
    public class KorisnikNaListiService : IKorisnikNaListiService
    {
        private readonly IKorisnikNaListiRepository _korisnikNaListiRepository;
        private readonly IListaCekanjaRepository _listaCekanjaRepository;
        private readonly IKorisnikRepository _korisnikRepository;
        private readonly ITerminService _terminService;

        public KorisnikNaListiService(
            IKorisnikNaListiRepository korisnikNaListiRepository,
            IListaCekanjaRepository listaCekanjaRepository,
            IKorisnikRepository korisnikRepository,
            ITerminService terminService)
        {
            _korisnikNaListiRepository = korisnikNaListiRepository;
            _listaCekanjaRepository = listaCekanjaRepository;
            _korisnikRepository = korisnikRepository;
            _terminService = terminService;
        }

        public async Task<KorisnikNaListi?> GetByIdAsync(int id)
            => await _korisnikNaListiRepository.GetByIdAsync(id);

        public async Task<IEnumerable<KorisnikNaListi>> GetAllAsync()
            => await _korisnikNaListiRepository.GetAllAsync();

        public void Update(KorisnikNaListi k) => _korisnikNaListiRepository.Update(k);
        public void Delete(KorisnikNaListi k) => _korisnikNaListiRepository.Delete(k);

        public async Task<int> SaveChangesAsync()
            => await _korisnikNaListiRepository.SaveChangesAsync();

        // ─── DODAJ NA LISTU ───────────────────────────────────────────────────

        public async Task AddAsync(KorisnikNaListi korisnikNaListi)
        {
            var sve = await _korisnikNaListiRepository.GetAllAsync();

            // Pravilo: pacijent ne može biti dvaput na istoj listi
            bool vecNaListi = sve.Any(k =>
                k.KorisnikID == korisnikNaListi.KorisnikID &&
                k.ListaID == korisnikNaListi.ListaID);

            if (vecNaListi)
                throw new Exception("Pacijent je već na ovoj listi čekanja.");

            korisnikNaListi.Datum = DateTime.Now;
            await _korisnikNaListiRepository.AddAsync(korisnikNaListi);
            await _korisnikNaListiRepository.SaveChangesAsync();
        }
        public async Task<IEnumerable<Korisnik>> GetPacijentiSaTerminomAsync(string doktorId)
        {
            var termini = await _terminService.GetAllAsync(); // ili inject ITerminService
            return termini
                .Where(t => t.DoktorID == doktorId && t.Pacijent != null)
                .Select(t => t.Pacijent!)
                .DistinctBy(p => p.Id)
                .ToList();
        }
        // ─── UKLONI S LISTE ───────────────────────────────────────────────────

        public async Task UkloniSListeAsync(int id)
        {
            var stavka = await _korisnikNaListiRepository.GetByIdAsync(id)
                ?? throw new Exception("Stavka nije pronađena na listi čekanja.");

            _korisnikNaListiRepository.Delete(stavka);
            await _korisnikNaListiRepository.SaveChangesAsync();
        }

        // ─── ALGORITAM: Sortiraj po prioritetu pa datumu prijave ─────────────
        // Korisnik.Prioritet: Visok > Srednji > Nizak
        // Za isti prioritet — ko se ranije prijavio ide prvi

        public async Task<IEnumerable<KorisnikNaListi>> GetSortiranaListaAsync(int listaId)
        {
            var sve = await _korisnikNaListiRepository.GetAllAsync();
            return sve
                .Where(k => k.ListaID == listaId)
                .OrderByDescending(k => (int)(k.Korisnik?.Prioritet ?? Prioritet.Nizak))
                .ThenBy(k => k.Datum);
        }

        // ─── SLJEDEĆI PACIJENT NA REDU ────────────────────────────────────────

        public async Task<KorisnikNaListi?> GetSljedeciPacijentAsync(int listaId)
        {
            var sortirana = await GetSortiranaListaAsync(listaId);
            return sortirana.FirstOrDefault();
        }

        // ─── LISTA PO KORISNIKU ───────────────────────────────────────────────

        public async Task<IEnumerable<KorisnikNaListi>> GetListeKorisnikaAsync(string korisnikId)
        {
            var sve = await _korisnikNaListiRepository.GetAllAsync();
            return sve.Where(k => k.KorisnikID == korisnikId);
        }

        // ─── BROJ PACIJENATA NA LISTI ─────────────────────────────────────────

        public async Task<int> GetBrojNaListiAsync(int listaId)
        {
            var sve = await _korisnikNaListiRepository.GetAllAsync();
            return sve.Count(k => k.ListaID == listaId);
        }

        // ─── POZICIJA PACIJENTA NA LISTI ─────────────────────────────────────

        public async Task<int> GetPozicijaAsync(int listaId, string korisnikId)
        {
            var sortirana = (await GetSortiranaListaAsync(listaId)).ToList();
            var index = sortirana.FindIndex(k => k.KorisnikID == korisnikId);
            return index == -1 ? -1 : index + 1; // vrati 1-based poziciju
        }

        // ─── HELPER METODE ────────────────────────────────────────────────────

        public async Task<IEnumerable<ListaCekanja>> GetListeCekanjaAsync()
            => await _listaCekanjaRepository.GetAllAsync();

        public async Task<IEnumerable<Korisnik>> GetKorisniciAsync()
            => await _korisnikRepository.GetAllAsync();
    }
}