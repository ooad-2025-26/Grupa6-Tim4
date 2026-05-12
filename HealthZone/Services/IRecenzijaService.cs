using HealthZone.Models;
using System.Threading.Tasks;

namespace HealthZone.Services
{
    public interface IRecenzijaService
    {
        Task<Recenzija?> GetByIdAsync(int id);
        Task<IEnumerable<Recenzija>> GetAllAsync();
        Task AddAsync(Recenzija recenzija);
        void Update(Recenzija recenzija);
        void Delete(Recenzija recenzija);
        Task<int> SaveChangesAsync();

        Task<IEnumerable<Korisnik>> GetDoktoriAsync();
        Task<IEnumerable<Korisnik>> GetPacijentiAsync();
    }
}
