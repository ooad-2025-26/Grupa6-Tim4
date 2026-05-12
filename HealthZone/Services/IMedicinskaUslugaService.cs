using HealthZone.Models;

namespace HealthZone.Services
{
    public interface IMedicinskaUslugaService
    {
        Task<MedicinskaUsluga?> GetByIdAsync(int id);
        Task<IEnumerable<MedicinskaUsluga>> GetAllAsync();
        Task AddAsync(MedicinskaUsluga medicinskaUsluga);
        void Update(MedicinskaUsluga medicinskaUsluga);
        void Delete(MedicinskaUsluga medicinskaUsluga);
        Task<int> SaveChangesAsync();
    }
}
