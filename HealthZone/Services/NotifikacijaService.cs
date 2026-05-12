using HealthZone.Models;
using HealthZone.Repositories;

namespace HealthZone.Services
{
    public class NotifikacijaService : INotifikacijaService
    {
        private readonly INotifikacijaRepository _notifikacijaRepository;

        public NotifikacijaService(INotifikacijaRepository notifikacijaRepository)
        {
            _notifikacijaRepository = notifikacijaRepository;
        }

        public async Task<Notifikacija?> GetByIdAsync(int id)
        {
            return await _notifikacijaRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Notifikacija>> GetAllAsync()
        {
            return await _notifikacijaRepository.GetAllAsync();
        }

        public async Task AddAsync(Notifikacija notifikacija)
        {
            await _notifikacijaRepository.AddAsync(notifikacija);
        }

        public void Update(Notifikacija notifikacija)
        {
            _notifikacijaRepository.Update(notifikacija);
        }

        public void Delete(Notifikacija notifikacija)
        {
            _notifikacijaRepository.Delete(notifikacija);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _notifikacijaRepository.SaveChangesAsync();
        }
    }
}
