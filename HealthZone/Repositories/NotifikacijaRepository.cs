using HealthZone.Data;
using HealthZone.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthZone.Repositories
{
    public class NotifikacijaRepository : INotifikacijaRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<Notifikacija> _notifikacije;

        public NotifikacijaRepository(ApplicationDbContext context)
        {
            _context = context;
            _notifikacije = context.Notifikacija;
        }

        public async Task<Notifikacija?> GetByIdAsync(int id)
        {
            return await _notifikacije.FindAsync(id);
        }

        public async Task<IEnumerable<Notifikacija>> GetAllAsync()
        {
            return await _notifikacije.ToListAsync();
        }

        public async Task AddAsync(Notifikacija notifikacija)
        {
            await _notifikacije.AddAsync(notifikacija);
        }

        public void Update(Notifikacija notifikacija)
        {
            _notifikacije.Update(notifikacija);
        }

        public void Delete(Notifikacija notifikacija)
        {
            _notifikacije.Remove(notifikacija);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
