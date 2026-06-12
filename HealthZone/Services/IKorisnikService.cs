using HealthZone.Models;
using System.Threading.Tasks;

namespace HealthZone.Services
{
    public interface IKorisnikService
    {
        Task<Korisnik?> GetByIdAsync(string id);
        Task<IEnumerable<Korisnik>> GetAllAsync();
        Task AddAsync(Korisnik korisnik, string lozinka, string uloga);

        void Update(Korisnik korisnik);
        void Delete(Korisnik korisnik);
        Task<int> SaveChangesAsync();
        Task AzurirajAsync(Korisnik korisnik);
        Task PromijeniUloguAsync(string korisnikId, string novaUloga);
        Task<string> GetUlogaAsync(Korisnik korisnik);
        Task<Korisnik?> GetDoktorByIdAsync(string id);
        Task<IEnumerable<Korisnik>> GetDoktoriAsync();
        Task<IEnumerable<Korisnik>> GetPacijentiAsync();
        Task<IEnumerable<Korisnik>> GetMedicinskeSestreAsync();
    }
}
