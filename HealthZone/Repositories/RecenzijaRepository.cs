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
            return await _recenzije.FindAsync(id);
        }

        public async Task<IEnumerable<Recenzija>> GetAllAsync()
        {
            return await _recenzije.ToListAsync();
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
