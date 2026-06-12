using HealthZone.Models;
using HealthZone.Repositories;

namespace HealthZone.Services
{
    public class MedicinskaUslugaService : IMedicinskaUslugaService
    {
        private readonly IMedicinskaUslugaRepository _medicinskaUslugaRepository;

        public MedicinskaUslugaService(IMedicinskaUslugaRepository medicinskaUslugaRepository)
        {
            _medicinskaUslugaRepository = medicinskaUslugaRepository;
        }

        public async Task<MedicinskaUsluga?> GetByIdAsync(int id)
            => await _medicinskaUslugaRepository.GetByIdAsync(id);

        public async Task<IEnumerable<MedicinskaUsluga>> GetAllAsync()
            => await _medicinskaUslugaRepository.GetAllAsync();

        public void Update(MedicinskaUsluga u) => _medicinskaUslugaRepository.Update(u);
        public void Delete(MedicinskaUsluga u) => _medicinskaUslugaRepository.Delete(u);

        public async Task<int> SaveChangesAsync()
            => await _medicinskaUslugaRepository.SaveChangesAsync();

        // ─── DODAJ USLUGU ─────────────────────────────────────────────────────

        public async Task AddAsync(MedicinskaUsluga usluga)
        {
            if (string.IsNullOrWhiteSpace(usluga.Naziv))
                throw new Exception("Naziv usluge je obavezan.");

            if (usluga.Cijena <= 0)
                throw new Exception("Cijena mora biti veća od 0.");

            // Pravilo: ne može postojati usluga s istim nazivom
            var sve = await _medicinskaUslugaRepository.GetAllAsync();
            if (sve.Any(u => u.Naziv.ToLower() == usluga.Naziv.ToLower()))
                throw new Exception("Usluga s ovim nazivom već postoji.");

            await _medicinskaUslugaRepository.AddAsync(usluga);
            await _medicinskaUslugaRepository.SaveChangesAsync();
        }

        // ─── USLUGE PO SPECIJALIZACIJI ────────────────────────────────────────

        public async Task<IEnumerable<MedicinskaUsluga>> GetUslugePoVrstiAsync(Specijalizacija vrsta)
        {
            var sve = await _medicinskaUslugaRepository.GetAllAsync();
            return sve.Where(u => u.Vrsta == vrsta);
        }

        // ─── PRETRAGA PO NAZIVU ───────────────────────────────────────────────

        public async Task<IEnumerable<MedicinskaUsluga>> PretraziAsync(string naziv)
        {
            var sve = await _medicinskaUslugaRepository.GetAllAsync();
            return sve.Where(u => u.Naziv.ToLower().Contains(naziv.ToLower()));
        }
    }
}