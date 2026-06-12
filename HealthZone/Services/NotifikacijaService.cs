using HealthZone.Models;
using HealthZone.Repositories;

namespace HealthZone.Services
{
    public class NotifikacijaService : INotifikacijaService
    {
        private readonly INotifikacijaRepository _notifikacijaRepository;
        private readonly ITerminRepository _terminRepository;

        public NotifikacijaService(
            INotifikacijaRepository notifikacijaRepository,
            ITerminRepository terminRepository)
        {
            _notifikacijaRepository = notifikacijaRepository;
            _terminRepository = terminRepository;
        }

        public async Task<Notifikacija?> GetByIdAsync(int id)
            => await _notifikacijaRepository.GetByIdAsync(id);

        public async Task<IEnumerable<Notifikacija>> GetAllAsync()
            => await _notifikacijaRepository.GetAllAsync();

        public void Update(Notifikacija n) => _notifikacijaRepository.Update(n);
        public void Delete(Notifikacija n) => _notifikacijaRepository.Delete(n);

        public async Task<int> SaveChangesAsync()
            => await _notifikacijaRepository.SaveChangesAsync();

        // ─── KREIRAJ NOTIFIKACIJU ─────────────────────────────────────────────

        public async Task AddAsync(Notifikacija notifikacija)
        {
            var termin = await _terminRepository.GetByIdAsync(notifikacija.TerminID)
                ?? throw new Exception("Termin nije pronađen.");

            notifikacija.DatumSlanja = DateTime.Now;
            await _notifikacijaRepository.AddAsync(notifikacija);
            await _notifikacijaRepository.SaveChangesAsync();
        }

        public async Task KreirajNotifikacijuAsync(int terminId, string poruka, Status status)
        {
            var notif = new Notifikacija
            {
                TerminID = terminId,
                Poruka = poruka,
                status = status,
                DatumSlanja = DateTime.Now
            };
            await _notifikacijaRepository.AddAsync(notif);
            await _notifikacijaRepository.SaveChangesAsync();
        }

        // ─── NOTIFIKACIJE PO TERMINU ──────────────────────────────────────────

        public async Task<IEnumerable<Notifikacija>> GetNotifikacijeZaTerminAsync(int terminId)
        {
            var sve = await _notifikacijaRepository.GetAllAsync();
            return sve
                .Where(n => n.TerminID == terminId)
                .OrderByDescending(n => n.DatumSlanja);
        }

        // ─── NOTIFIKACIJE PO PACIJENTU ────────────────────────────────────────
        // Ide kroz Termin da dođe do pacijenta

        public async Task<IEnumerable<Notifikacija>> GetNotifikacijePacijentaAsync(string pacijentId)
        {
            var sve = await _notifikacijaRepository.GetAllAsync();
            return sve
                .Where(n => n.Termin?.PacijentID == pacijentId)
                .OrderByDescending(n => n.DatumSlanja);
        }

        // ─── NOTIFIKACIJE PO STATUSU ──────────────────────────────────────────

        public async Task<IEnumerable<Notifikacija>> GetNotifikacijePoStatusuAsync(Status status)
        {
            var sve = await _notifikacijaRepository.GetAllAsync();
            return sve.Where(n => n.status == status);
        }
    }
}