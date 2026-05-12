using HealthZone.Models;

namespace HealthZone.Repositories
{
    public interface IKorisnikNaListiRepository
    {
        Task<KorisnikNaListi?> GetByIdAsync(int id);
        Task<IEnumerable<KorisnikNaListi>> GetAllAsync();
        Task AddAsync(KorisnikNaListi korisnikNaListi);
        void Update(KorisnikNaListi korisnikNaListi);
        void Delete(KorisnikNaListi korisnikNaListi);
        Task<int> SaveChangesAsync();
    }
}
