using HealthZone.Models;

namespace HealthZone.Services
{
    public interface INalazService
    {
        Task<Nalaz?> GetByIdAsync(int id);
        Task<IEnumerable<Nalaz>> GetAllAsync();
        Task AddAsync(Nalaz nalaz);
        void Update(Nalaz nalaz);
        void Delete(Nalaz nalaz);
        Task<int> SaveChangesAsync();
        Task<IEnumerable<Nalaz>> GetNalaziPacijentaAsync(string pacijentId);

        Task<string> GenerirajHtmlZaPrintAsync(int nalazId);
    }
}
