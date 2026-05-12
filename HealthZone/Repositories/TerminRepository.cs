using HealthZone.Data;
using HealthZone.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthZone.Repositories
{
    public class TerminRepository : ITerminRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<Termin> _termini;

        public TerminRepository(ApplicationDbContext context)
        {
            _context = context;
            _termini = context.Termin;
        }

        public async Task<Termin?> GetByIdAsync(int id)
        {
            return await _termini.FindAsync(id);
        }

        public async Task<IEnumerable<Termin>> GetAllAsync()
        {
            return await _termini.ToListAsync();
        }

        public async Task AddAsync(Termin termin)
        {
            await _termini.AddAsync(termin);
        }

        public void Update(Termin termin)
        {
            _termini.Update(termin);
        }

        public void Delete(Termin termin)
        {
            _termini.Remove(termin);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }



    }
}
