using HealthZone.Data;
using HealthZone.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthZone.Repositories
{
    public class RecenzijaRepository : IRecenzijaRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<Recenzija> _recenzije;

        public RecenzijaRepository(ApplicationDbContext context)
        {
            _context = context;
            _recenzije = context.Recenzija;
        }

        public async Task<Recenzija?> GetByIdAsync(int id)
        {
            return await _recenzije
                .Include(r => r.Pacijent)
                .Include(r => r.Doktor)
                .FirstOrDefaultAsync(r => r.RecenzijaId == id);
        }

        public async Task<IEnumerable<Recenzija>> GetAllAsync()
        {
            return await _recenzije
                .Include(r => r.Pacijent)
                .Include(r => r.Doktor)
                .ToListAsync();
        }

        public async Task AddAsync(Recenzija recenzija)
        {
            await _recenzije.AddAsync(recenzija);
        }

        public void Update(Recenzija recenzija)
        {
            _recenzije.Update(recenzija);
        }

        public void Delete(Recenzija recenzija)
        {
            _recenzije.Remove(recenzija);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}