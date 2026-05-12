using HealthZone.Models;
using HealthZone.Repositories;

namespace HealthZone.Services
{
    public class TerminService : ITerminService
    {
        private readonly ITerminRepository _terminRepository;

        public TerminService(ITerminRepository terminRepository)
        {
            _terminRepository = terminRepository;
        }

        public async Task<Termin?> GetByIdAsync(int id)
        {
            return await _terminRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Termin>> GetAllAsync()
        {
            return await _terminRepository.GetAllAsync();
        }

        public async Task AddAsync(Termin termin)
        {
            await _terminRepository.AddAsync(termin);
        }

        public void Update(Termin termin)
        {
            _terminRepository.Update(termin);
        }

        public void Delete(Termin termin)
        {
            _terminRepository.Delete(termin);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _terminRepository.SaveChangesAsync();
        }


       

        

    }
}
