using HealthZone.Models;
using HealthZone.Repositories;

namespace HealthZone.Services
{
    public class NalazService : INalazService
    {
        private readonly INalazRepository _nalazRepository;

        public NalazService(INalazRepository nalazRepository)
        {
            _nalazRepository = nalazRepository;
        }

        public async Task<Nalaz?> GetByIdAsync(int id)
        {
            return await _nalazRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Nalaz>> GetAllAsync()
        {
            return await _nalazRepository.GetAllAsync();
        }

        public async Task AddAsync(Nalaz nalaz)
        {
            await _nalazRepository.AddAsync(nalaz);
        }

        public void Update(Nalaz nalaz)
        {
            _nalazRepository.Update(nalaz);
        }

        public void Delete(Nalaz nalaz)
        {
            _nalazRepository.Delete(nalaz);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _nalazRepository.SaveChangesAsync();
        }

    }
}
