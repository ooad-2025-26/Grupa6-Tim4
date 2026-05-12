using HealthZone.Data;
using HealthZone.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthZone.Repositories
{
    public class MedicinskaUslugaRepository : IMedicinskaUslugaRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<MedicinskaUsluga> _medicinskaUsluga;

        public MedicinskaUslugaRepository(ApplicationDbContext context)
        {
            _context = context;
            _medicinskaUsluga = context.MedicinskaUsluga;
        }

        public async Task<MedicinskaUsluga?> GetByIdAsync(int id)
        {
            return await _medicinskaUsluga.FindAsync(id);
        }

        public async Task<IEnumerable<MedicinskaUsluga>> GetAllAsync()
        {
            return await _medicinskaUsluga.ToListAsync();
        }

        public async Task AddAsync(MedicinskaUsluga medicinskaUsluga)
        {
            await _medicinskaUsluga.AddAsync(medicinskaUsluga);
        }

        public void Update(MedicinskaUsluga medicinskaUsluga)
        {
            _medicinskaUsluga.Update(medicinskaUsluga);
        }

        public void Delete(MedicinskaUsluga medicinskaUsluga)
        {
            _medicinskaUsluga.Remove(medicinskaUsluga);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
