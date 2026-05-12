using HealthZone.Models;

namespace HealthZone.Services
{
    public interface INotifikacijaService
    {
        Task<Notifikacija?> GetByIdAsync(int id);
        Task<IEnumerable<Notifikacija>> GetAllAsync();
        Task AddAsync(Notifikacija notifikacija);
        void Update(Notifikacija notifikacija);
        void Delete(Notifikacija notifikacija);
        Task<int> SaveChangesAsync();
    }
}
