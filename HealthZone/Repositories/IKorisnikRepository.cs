using HealthZone.Models;

namespace HealthZone.Repositories
{
    public interface IKorisnikRepository
    {
        Task<Korisnik?> GetByIdAsync(string id);
        Task<IEnumerable<Korisnik>> GetAllAsync();
        Task AddAsync(Korisnik korisnik);
        void Update(Korisnik korisnik);
        void Delete(Korisnik korisnik);
        Task<int> SaveChangesAsync();

        
       
    }
}
