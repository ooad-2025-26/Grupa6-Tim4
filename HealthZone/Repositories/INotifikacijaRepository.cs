using HealthZone.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthZone.Repositories
{
    public interface INotifikacijaRepository
    {
        Task<Notifikacija?> GetByIdAsync(int id);

        Task<IEnumerable<Notifikacija>> GetAllAsync();

        Task AddAsync(Notifikacija notifikacija);


        void Update(Notifikacija notifikacija);

        void Delete(Notifikacija notifikacija);

        Task<int> SaveChangesAsync();
    }
}
