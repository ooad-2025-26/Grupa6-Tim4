using HealthZone.Data;
using HealthZone.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthZone.Repositories
{
    public class NalazRepository : INalazRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<Nalaz> _nalazi;

        public NalazRepository(ApplicationDbContext context)
        {
            _context = context;
            _nalazi = context.Nalaz;
        }

        public async Task<Nalaz?> GetByIdAsync(int id)
        {
            return await _nalazi.FindAsync(id);
        }

        public async Task<IEnumerable<Nalaz>> GetAllAsync()
        {
            return await _nalazi.ToListAsync();
        }

        public async Task AddAsync(Nalaz nalaz)
        {
            await _nalazi.AddAsync(nalaz);
        }

        public void Update(Nalaz nalaz)
        {
            _nalazi.Update(nalaz);
        }

        public void Delete(Nalaz nalaz)
        {
            _nalazi.Remove(nalaz);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
