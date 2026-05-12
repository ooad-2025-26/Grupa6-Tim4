using HealthZone.Models;

namespace HealthZone.Services
{
    public interface IListaCekanjaService
    {
        Task<ListaCekanja?> GetByIdAsync(int id);
        Task<IEnumerable<ListaCekanja>> GetAllAsync();
        Task AddAsync(ListaCekanja listaCekanja);
        void Update(ListaCekanja listaCekanja);
        void Delete(ListaCekanja listaCekanja);
        Task<int> SaveChangesAsync();

        Task<IEnumerable<Korisnik>> GetDoktoriAsync();
    }
}
