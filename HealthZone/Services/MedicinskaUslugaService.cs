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
        {
            return await _medicinskaUslugaRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<MedicinskaUsluga>> GetAllAsync()
        {
            return await _medicinskaUslugaRepository.GetAllAsync();
        }

        public async Task AddAsync(MedicinskaUsluga medicinskaUsluga)
        {
            await _medicinskaUslugaRepository.AddAsync(medicinskaUsluga);
        }

        public void Update(MedicinskaUsluga medicinskaUsluga)
        {
            _medicinskaUslugaRepository.Update(medicinskaUsluga);
        }

        public void Delete(MedicinskaUsluga medicinskaUsluga)
        {
            _medicinskaUslugaRepository.Delete(medicinskaUsluga);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _medicinskaUslugaRepository.SaveChangesAsync();
        }
    }
}
