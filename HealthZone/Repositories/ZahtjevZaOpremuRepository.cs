using HealthZone.Data;
using HealthZone.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthZone.Repositories
{
    public class ZahtjevZaOpremuRepository : IZahtjevZaOpremuRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<ZahtjevZaOpremu> _zahtjeviZaOpremu;

        public ZahtjevZaOpremuRepository(ApplicationDbContext context)
        {
            _context = context;
            _zahtjeviZaOpremu = context.ZahtjevZaOpremu;
        }

        public async Task<ZahtjevZaOpremu?> GetByIdAsync(int id)
        {
            return await _zahtjeviZaOpremu
                .Include(z => z.Doktor)
                .FirstOrDefaultAsync(z => z.ZahtjevId == id);
        }

        public async Task<IEnumerable<ZahtjevZaOpremu>> GetAllAsync()
        {
            return await _zahtjeviZaOpremu
                .Include(z => z.Doktor)
                .OrderByDescending(z => z.ZahtjevId)
                .ToListAsync();
        }

        public async Task AddAsync(ZahtjevZaOpremu zahtjevZaOpremu)
        {
            await _zahtjeviZaOpremu.AddAsync(zahtjevZaOpremu);
        }

        public void Update(ZahtjevZaOpremu zahtjevZaOpremu)
        {
            _zahtjeviZaOpremu.Update(zahtjevZaOpremu);
        }

        public void Delete(ZahtjevZaOpremu zahtjevZaOpremu)
        {
            _zahtjeviZaOpremu.Remove(zahtjevZaOpremu);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}