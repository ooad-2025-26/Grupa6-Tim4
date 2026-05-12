using HealthZone.Models;

namespace HealthZone.Repositories
{
    public interface IListaCekanjaRepository
    {
        Task<ListaCekanja?> GetByIdAsync(int id);
        Task<IEnumerable<ListaCekanja>> GetAllAsync();
        Task AddAsync(ListaCekanja listaCekanja);
        void Update(ListaCekanja listaCekanja);
        void Delete(ListaCekanja listaCekanja);
        Task<int> SaveChangesAsync();
    }
}
