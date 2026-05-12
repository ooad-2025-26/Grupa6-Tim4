using HealthZone.Models;

namespace HealthZone.Repositories
{
    public interface IRecenzijaRepository
    {
        Task<Recenzija?> GetByIdAsync(int id);

        Task<IEnumerable<Recenzija>> GetAllAsync();

        Task AddAsync(Recenzija recenzija);


        void Update(Recenzija recenzija);

        void Delete(Recenzija recenzija);

        Task<int> SaveChangesAsync();
    }
}
