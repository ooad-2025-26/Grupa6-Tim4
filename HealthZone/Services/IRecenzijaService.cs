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

        Task<IEnumerable<Recenzija>> GetRecenzijeDoktoraAsync(string doktorId);
        Task<(double Ljubaznost, double Profesionalnost, double Usluga, double Ukupno)>
            GetProsjecneOcjeneAsync(string doktorId);
        Task<int> SaveChangesAsync();

        Task<IEnumerable<Korisnik>> GetDoktoriAsync();
        Task<IEnumerable<Korisnik>> GetPacijentiAsync();
    }
}
