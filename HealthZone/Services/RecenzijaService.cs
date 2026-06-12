using HealthZone.Models;
using HealthZone.Repositories;

namespace HealthZone.Services
{
    public class RecenzijaService : IRecenzijaService
    {
        private readonly IRecenzijaRepository _recenzijaRepository;
        private readonly IKorisnikRepository _korisnikRepository;
        private readonly ITerminRepository _terminRepository;

        public RecenzijaService(
            IRecenzijaRepository recenzijaRepository,
            IKorisnikRepository korisnikRepository,
            ITerminRepository terminRepository)
        {
            _recenzijaRepository = recenzijaRepository;
            _korisnikRepository = korisnikRepository;
            _terminRepository = terminRepository;
        }

        public async Task<Recenzija?> GetByIdAsync(int id)
            => await _recenzijaRepository.GetByIdAsync(id);

        public async Task<IEnumerable<Recenzija>> GetAllAsync()
            => await _recenzijaRepository.GetAllAsync();

        public void Update(Recenzija r) => _recenzijaRepository.Update(r);
        public void Delete(Recenzija r) => _recenzijaRepository.Delete(r);

        public async Task<int> SaveChangesAsync()
            => await _recenzijaRepository.SaveChangesAsync();

        // ─── DODAJ RECENZIJU (s pravilima) ───────────────────────────────────

        public async Task AddAsync(Recenzija recenzija)
        {
            // Pravilo: sve tri ocjene moraju biti 1-5
            if (!OcjenaValjana(recenzija.OcjenaLjubaznosti))
                throw new Exception("Ocjena ljubaznosti mora biti između 1 i 5.");
            if (!OcjenaValjana(recenzija.OcjenaProfesionalnosti))
                throw new Exception("Ocjena profesionalnosti mora biti između 1 i 5.");
            if (!OcjenaValjana(recenzija.OcjenaUsluge))
                throw new Exception("Ocjena usluge mora biti između 1 i 5.");

            // Pravilo: pacijent mora imati aktivan termin s tim doktorom
            var termini = await _terminRepository.GetAllAsync();
            bool imaTermin = termini.Any(t =>
                t.PacijentID == recenzija.PacijentID &&
                t.DoktorID == recenzija.DoktorID &&
                t.Status == Status.Aktivan);

            if (!imaTermin)
                throw new Exception("Možete recenzirati samo doktore kod kojih ste imali obavljeni pregled.");

            // Pravilo: jedna recenzija po pacijentu po doktoru
            var sve = await _recenzijaRepository.GetAllAsync();
            if (sve.Any(r => r.PacijentID == recenzija.PacijentID && r.DoktorID == recenzija.DoktorID))
                throw new Exception("Već ste ocijenili ovog doktora. Obrišite postojeću recenziju za unos nove.");

            await _recenzijaRepository.AddAsync(recenzija);
            await _recenzijaRepository.SaveChangesAsync();
        }

        // ─── RECENZIJE DOKTORA ────────────────────────────────────────────────

        public async Task<IEnumerable<Recenzija>> GetRecenzijeDoktoraAsync(string doktorId)
        {
            var sve = await _recenzijaRepository.GetAllAsync();
            return sve.Where(r => r.DoktorID == doktorId);
        }

        // ─── PROSJEČNE OCJENE DOKTORA ─────────────────────────────────────────

        public async Task<(double Ljubaznost, double Profesionalnost, double Usluga, double Ukupno)>
            GetProsjecneOcjeneAsync(string doktorId)
        {
            var recenzije = (await GetRecenzijeDoktoraAsync(doktorId)).ToList();
            if (!recenzije.Any()) return (0, 0, 0, 0);

            double ljubaznost = Math.Round(recenzije.Average(r => r.OcjenaLjubaznosti), 1);
            double profesionalnost = Math.Round(recenzije.Average(r => r.OcjenaProfesionalnosti), 1);
            double usluga = Math.Round(recenzije.Average(r => r.OcjenaUsluge), 1);
            double ukupno = Math.Round((ljubaznost + profesionalnost + usluga) / 3, 1);

            return (ljubaznost, profesionalnost, usluga, ukupno);
        }

        // ─── TOP DOKTORI ──────────────────────────────────────────────────────

        public async Task<IEnumerable<Korisnik>> GetTopDoktoriAsync(int top = 5)
        {
            var doktori = await GetDoktoriAsync();
            var ocjene = new List<(Korisnik Doktor, double Ukupno)>();

            foreach (var doktor in doktori)
            {
                var (_, _, _, ukupno) = await GetProsjecneOcjeneAsync(doktor.Id);
                ocjene.Add((doktor, ukupno));
            }

            return ocjene
                .OrderByDescending(o => o.Ukupno)
                .Take(top)
                .Select(o => o.Doktor);
        }

        // ─── HELPER ───────────────────────────────────────────────────────────

        private static bool OcjenaValjana(int ocjena) => ocjena >= 1 && ocjena <= 5;

        public async Task<IEnumerable<Korisnik>> GetDoktoriAsync()
        {
            var svi = await _korisnikRepository.GetAllAsync();
            return svi.Where(k => k.Specijalizacija != null);
        }

        public async Task<IEnumerable<Korisnik>> GetPacijentiAsync()
        {
            var svi = await _korisnikRepository.GetAllAsync();
            return svi.Where(k => k.Prioritet != null);
        }
    }
}