using HealthZone.Models;

namespace HealthZone.Repositories
{
    public interface IZahtjevZaOpremuRepository
    {
        Task<ZahtjevZaOpremu?> GetByIdAsync(int id);
        Task<IEnumerable<ZahtjevZaOpremu>> GetAllAsync();
        Task AddAsync(ZahtjevZaOpremu zahtjevZaOpremu);
        void Update(ZahtjevZaOpremu zahtjevZaOpremu);
        void Delete(ZahtjevZaOpremu zahtjevZaOpremu);
        Task<int> SaveChangesAsync();
    }
}
