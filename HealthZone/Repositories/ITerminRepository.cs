using HealthZone.Models;

namespace HealthZone.Repositories
{
    public interface ITerminRepository
    {
        Task<Termin?> GetByIdAsync(int id);
        Task<IEnumerable<Termin>> GetAllAsync();
        Task AddAsync(Termin termin);
        void Update(Termin termin);
        void Delete(Termin termin);
        Task<int> SaveChangesAsync();

        
    }
}
