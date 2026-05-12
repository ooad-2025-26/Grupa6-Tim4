using HealthZone.Data;
using HealthZone.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthZone.Repositories
{
    public class ListaCekanjaRepository : IListaCekanjaRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<ListaCekanja> _listaCekanja;

        public ListaCekanjaRepository(ApplicationDbContext context)
        {
            _context = context;
            _listaCekanja = context.ListaCekanja;
        }

        public async Task<ListaCekanja?> GetByIdAsync(int id)
        {
            return await _listaCekanja.FindAsync(id);
        }

        public async Task<IEnumerable<ListaCekanja>> GetAllAsync()
        {
            return await _listaCekanja.ToListAsync();
        }

        public async Task AddAsync(ListaCekanja listaCekanja)
        {
            await _listaCekanja.AddAsync(listaCekanja);
        }

        public void Update(ListaCekanja listaCekanja)
        {
            _listaCekanja.Update(listaCekanja);
        }

        public void Delete(ListaCekanja listaCekanja)
        {
            _listaCekanja.Remove(listaCekanja);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
