using HealthZone.Models;

namespace HealthZone.Repositories
{
    public interface IMedicinskaUslugaRepository
    {
        Task<MedicinskaUsluga?> GetByIdAsync(int id);
        Task<IEnumerable<MedicinskaUsluga>> GetAllAsync();
        Task AddAsync(MedicinskaUsluga medicinskaUsluga);
        void Update(MedicinskaUsluga medicinskaUsluga);
        void Delete(MedicinskaUsluga medicinskaUsluga);
        Task<int> SaveChangesAsync();
    }
}
