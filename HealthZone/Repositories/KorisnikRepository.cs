using HealthZone.Data;
using HealthZone.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthZone.Repositories
{
    public class KorisnikRepository : IKorisnikRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<Korisnik> _korisnici;

        public KorisnikRepository(ApplicationDbContext context)
        {
            _context = context;
            _korisnici = context.Users;
        }

        public async Task<Korisnik?> GetByIdAsync(string id)
        {
            return await _korisnici.FindAsync(id);
        }

        public async Task<IEnumerable<Korisnik>> GetAllAsync()
        {
            return await _korisnici.ToListAsync();
        }

        public async Task AddAsync(Korisnik korisnik)
        {
            await _korisnici.AddAsync(korisnik);
        }

        public void Update(Korisnik korisnik)
        {
            _korisnici.Update(korisnik);
        }

        public void Delete(Korisnik korisnik)
        {
            _korisnici.Remove(korisnik);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
