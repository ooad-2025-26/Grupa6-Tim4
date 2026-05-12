using HealthZone.Models;

namespace HealthZone.Services
{
    public interface IKorisnikNaListiService
    {
        Task<KorisnikNaListi?> GetByIdAsync(int id);
        Task<IEnumerable<KorisnikNaListi>> GetAllAsync();
        Task AddAsync(KorisnikNaListi korisnikNaListi);
        void Update(KorisnikNaListi korisnikNaListi);
        void Delete(KorisnikNaListi korisnikNaListi);
        Task<int> SaveChangesAsync();

        Task<IEnumerable<ListaCekanja>> GetListeCekanjaAsync();
        Task<IEnumerable<Korisnik>> GetKorisniciAsync();
    }
}
