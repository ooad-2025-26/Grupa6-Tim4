using HealthZone.Models;

namespace HealthZone.Services
{
    public interface ITerminService
    {
        Task<Termin?> GetByIdAsync(int id);
        Task<IEnumerable<Termin>> GetAllAsync();
        Task AddAsync(Termin termin);
        void Update(Termin termin);
        void Delete(Termin termin);
        Task<int> SaveChangesAsync();

    }
}
