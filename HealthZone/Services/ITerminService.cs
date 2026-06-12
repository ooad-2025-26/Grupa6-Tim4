using HealthZone.Models;
using HealthZone.Repositories;
using System.Threading.Tasks;

namespace HealthZone.Services
{
    public interface ITerminService
    {
        Task<Termin?> GetByIdAsync(int id);
        Task<IEnumerable<Termin>> GetAllAsync();
        Task AddAsync(Termin termin);
        void Update(Termin termin);
        void Delete(Termin termin);
        Task<int> SaveChangesAsync();


        Task ZakaziTerminAsync(Termin termin);

        Task OtkaziTerminAsync(int terminId);

        Task PromijeniTerminAsync(int terminId, DateTime noviDatum, TimeOnly novoVrijeme);

        Task OznaciKaoAktivanAsync(int terminId);
        Task OznaciKaoPotvrdjenAsync(int terminId);


        Task<bool> IsTerminDostupanAsync(string doktorId, DateTime datum, TimeOnly vrijeme);
        Task<IEnumerable<TimeOnly>> GetSlobodniTerminiAsync(string doktorId, DateTime datum);

        Task<IEnumerable<Termin>> GetTerminiDoktoraAsync(string doktorId, DateTime? datum = null);

        Task<IEnumerable<Termin>> GetTerminiPacijentaAsync(string pacijentId);
        Task KreirajNotifikacijuAsync(int terminId, string poruka, Status status);
        Task PošaljiPodsjetnikeSutraAsync();

        }
    }

