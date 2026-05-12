using HealthZone.Data;
using HealthZone.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthZone.Repositories
{
    public class KorisnikNaListiRepository : IKorisnikNaListiRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<KorisnikNaListi> _korisniciNaListi;

        public KorisnikNaListiRepository(ApplicationDbContext context)
        {
            _context = context;
            _korisniciNaListi = context.KorisnikNaListi;
        }

        public async Task<KorisnikNaListi?> GetByIdAsync(int id)
        {
            return await _korisniciNaListi.FindAsync(id);
        }

        public async Task<IEnumerable<KorisnikNaListi>> GetAllAsync()
        {
            return await _korisniciNaListi.ToListAsync();
        }

        public async Task AddAsync(KorisnikNaListi korisnikNaListi)
        {
            await _korisniciNaListi.AddAsync(korisnikNaListi);
        }

        public void Update(KorisnikNaListi korisnikNaListi)
        {
            _korisniciNaListi.Update(korisnikNaListi);
        }

        public void Delete(KorisnikNaListi korisnikNaListi)
        {
            _korisniciNaListi.Remove(korisnikNaListi);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
