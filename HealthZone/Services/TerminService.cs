using HealthZone.Models;
using HealthZone.Repositories;

namespace HealthZone.Services
{
    public class TerminService : ITerminService
    {
        private readonly ITerminRepository _terminRepository;
        private readonly INotifikacijaRepository _notifikacijaRepository;
        private readonly IEmailService _emailService;
        private readonly IKorisnikNaListiRepository _korisnikNaListiRepository;

        public TerminService(
            ITerminRepository terminRepository,
            INotifikacijaRepository notifikacijaRepository,
            IEmailService emailService,
            IKorisnikNaListiRepository korisnikNaListiRepository)
        {
            _terminRepository = terminRepository;
            _notifikacijaRepository = notifikacijaRepository;
            _emailService = emailService;
            _korisnikNaListiRepository = korisnikNaListiRepository;
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

        public async Task ZakaziTerminAsync(Termin termin)
        {
            var vrijemeTermina = termin.Datum.Date.Add(termin.Vrijeme.ToTimeSpan());

            if (vrijemeTermina < DateTime.Now)
                throw new Exception("Ne možete zakazati termin u prošlosti.");

            if (vrijemeTermina < DateTime.Now.AddHours(12))
                throw new Exception("Termin mora biti zakazan najmanje 12 sati unaprijed.");

            if (!await IsTerminDostupanAsync(termin.DoktorID, termin.Datum, termin.Vrijeme))
                throw new Exception("Odabrani termin nije dostupan.");

            termin.Status = Status.NaCekanju;
            await _terminRepository.AddAsync(termin);
            await _terminRepository.SaveChangesAsync();

            await KreirajNotifikacijuAsync(termin.TerminId,
                $"Termin zakazan za {termin.Datum:dd.MM.yyyy} u {termin.Vrijeme:HH:mm}. Čeka vašu potvrdu.",
                Status.NaCekanju);

            if (!string.IsNullOrEmpty(termin.Pacijent?.Email))
            {
                try
                {
                    await _emailService.PošaljiPotvrduTerminaAsync(
                        email: termin.Pacijent.Email,
                        imePacijenta: $"{termin.Pacijent.Ime} {termin.Pacijent.Prezime}",
                        imeDoktora: termin.Doktor != null ? $"{termin.Doktor.Ime} {termin.Doktor.Prezime}" : "—",
                        datum: termin.Datum,
                        vrijeme: termin.Vrijeme,
                        usluga: termin.Usluga?.Naziv ?? "—",
                        terminId: termin.TerminId
                    );
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[EMAIL GREŠKA - ZakaziTermin]: {ex.Message}");
                }
            }
        }

        public async Task OznaciKaoPotvrdjenAsync(int terminId)
        {
            var termin = await _terminRepository.GetByIdAsync(terminId)
                ?? throw new Exception("Termin nije pronađen.");

            if (termin.Status == Status.Otkazan)
                throw new Exception("Otkazani termin se ne može potvrditi.");

            termin.Status = Status.Potvrdjen;
            _terminRepository.Update(termin);
            await _terminRepository.SaveChangesAsync();

            await KreirajNotifikacijuAsync(termin.TerminId,
                $"Termin zakazan za {termin.Datum:dd.MM.yyyy} u {termin.Vrijeme:HH:mm} je potvrđen.",
                Status.Potvrdjen);
        }

        public async Task OznaciKaoAktivanAsync(int terminId)
        {
            var termin = await _terminRepository.GetByIdAsync(terminId)
                ?? throw new Exception("Termin nije pronađen.");

            termin.Status = Status.Aktivan;
            _terminRepository.Update(termin);
            await _terminRepository.SaveChangesAsync();

            await KreirajNotifikacijuAsync(termin.TerminId,
                $"Termin od {termin.Datum:dd.MM.yyyy} u {termin.Vrijeme:HH:mm} je aktivan.",
                Status.Aktivan);
        }

        public async Task OtkaziTerminAsync(int terminId)
        {
            var termin = await _terminRepository.GetByIdAsync(terminId)
                ?? throw new Exception("Termin nije pronađen.");

            if (termin.Status == Status.Otkazan)
                throw new Exception("Termin je već otkazan.");

            termin.Status = Status.Otkazan;
            _terminRepository.Update(termin);
            await _terminRepository.SaveChangesAsync();

            await KreirajNotifikacijuAsync(termin.TerminId,
                $"Termin zakazan za {termin.Datum:dd.MM.yyyy} u {termin.Vrijeme:HH:mm} je otkazan.",
                Status.Otkazan);

            if (!string.IsNullOrEmpty(termin.Pacijent?.Email))
            {
                try
                {
                    await _emailService.PošaljiOtkazTerminaAsync(
                        email: termin.Pacijent.Email,
                        imePacijenta: $"{termin.Pacijent.Ime} {termin.Pacijent.Prezime}",
                        imeDoktora: termin.Doktor != null ? $"{termin.Doktor.Ime} {termin.Doktor.Prezime}" : "—",
                        datum: termin.Datum,
                        vrijeme: termin.Vrijeme
                    );
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[EMAIL GREŠKA - OtkaziTermin]: {ex.Message}");
                }
            }

            try
            {
                await ObavijestisListeČekanjaAsync(termin);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EMAIL GREŠKA - ListaČekanja]: {ex.Message}");
            }
        }

        public async Task PromijeniTerminAsync(int terminId, DateTime noviDatum, TimeOnly novoVrijeme)
        {
            var termin = await _terminRepository.GetByIdAsync(terminId)
                ?? throw new Exception("Termin nije pronađen.");

            if (termin.Status == Status.Otkazan)
                throw new Exception("Ne možete mijenjati otkazani termin.");

            var novoVrijemeTermina = noviDatum.Date.Add(novoVrijeme.ToTimeSpan());

            if (novoVrijemeTermina < DateTime.Now)
                throw new Exception("Ne možete zakazati termin u prošlosti.");

            if (novoVrijemeTermina < DateTime.Now.AddHours(12))
                throw new Exception("Termin mora biti zakazan najmanje 12 sati unaprijed.");

            if (!await IsTerminDostupanAsync(termin.DoktorID, noviDatum, novoVrijeme))
                throw new Exception("Novi termin nije dostupan.");

            termin.Datum = noviDatum;
            termin.Vrijeme = novoVrijeme;
            termin.Status = Status.Potvrdjen;

            _terminRepository.Update(termin);
            await _terminRepository.SaveChangesAsync();

            await KreirajNotifikacijuAsync(termin.TerminId,
                $"Termin je promijenjen na {noviDatum:dd.MM.yyyy} u {novoVrijeme:HH:mm}.",
                Status.Potvrdjen);
        }

        public async Task<bool> IsTerminDostupanAsync(string doktorId, DateTime datum, TimeOnly vrijeme)
        {
            var termini = await _terminRepository.GetAllAsync();
            return !termini.Any(t =>
                t.DoktorID == doktorId &&
                t.Datum.Date == datum.Date &&
                t.Vrijeme == vrijeme &&
                t.Status != Status.Otkazan);
        }

        public async Task<IEnumerable<TimeOnly>> GetSlobodniTerminiAsync(string doktorId, DateTime datum)
        {
            var sviSlotovi = Enumerable.Range(8, 8)
                .Select(h => new TimeOnly(h, 0))
                .ToList();

            var termini = await _terminRepository.GetAllAsync();
            var zauzeti = termini
                .Where(t =>
                    t.DoktorID == doktorId &&
                    t.Datum.Date == datum.Date &&
                    t.Status != Status.Otkazan)
                .Select(t => t.Vrijeme)
                .ToHashSet();

            return sviSlotovi.Where(s => !zauzeti.Contains(s));
        }

        public async Task<IEnumerable<Termin>> GetTerminiDoktoraAsync(string doktorId, DateTime? datum = null)
        {
            var termini = await _terminRepository.GetAllAsync();
            var filtered = termini.Where(t => t.DoktorID == doktorId);

            if (datum.HasValue)
                filtered = filtered.Where(t => t.Datum.Date == datum.Value.Date);

            return filtered
                .OrderBy(t => t.Datum)
                .ThenBy(t => t.Vrijeme);
        }

        public async Task<IEnumerable<Termin>> GetTerminiPacijentaAsync(string pacijentId)
        {
            var termini = await _terminRepository.GetAllAsync();
            return termini
                .Where(t => t.PacijentID == pacijentId)
                .OrderByDescending(t => t.Datum)
                .ThenByDescending(t => t.Vrijeme);
        }

        private async Task ObavijestisListeČekanjaAsync(Termin otkazanTermin)
        {
            var sviNaListi = await _korisnikNaListiRepository.GetAllAsync();

            var prviNaListi = sviNaListi
                .Where(k => k.Lista?.DoktorID == otkazanTermin.DoktorID)
                .OrderByDescending(k => (int)(k.Korisnik?.Prioritet ?? Prioritet.Nizak))
                .ThenBy(k => k.Datum)
                .FirstOrDefault();

            if (prviNaListi?.Korisnik?.Email == null) return;

            var doktorIme = otkazanTermin.Doktor != null
                ? $"{otkazanTermin.Doktor.Ime} {otkazanTermin.Doktor.Prezime}"
                : "—";

            await _emailService.PošaljiObavještenjeListe(
                email: prviNaListi.Korisnik.Email,
                imePacijenta: $"{prviNaListi.Korisnik.Ime} {prviNaListi.Korisnik.Prezime}",
                imeDoktora: doktorIme,
                datum: otkazanTermin.Datum,
                vrijeme: otkazanTermin.Vrijeme
            );
        }

        public async Task PošaljiPodsjetnikeSutraAsync()
        {
            var sutra = DateTime.Today.AddDays(1);
            var termini = await _terminRepository.GetAllAsync();

            var sutrašnji = termini.Where(t =>
                t.Datum.Date == sutra.Date &&
                t.Status == Status.Potvrdjen &&
                t.Pacijent?.Email != null);

            foreach (var t in sutrašnji)
            {
                try
                {
                    await _emailService.PošaljiPodsjetnikAsync(
                        email: t.Pacijent!.Email!,
                        imePacijenta: $"{t.Pacijent.Ime} {t.Pacijent.Prezime}",
                        imeDoktora: t.Doktor != null ? $"{t.Doktor.Ime} {t.Doktor.Prezime}" : "—",
                        datum: t.Datum,
                        vrijeme: t.Vrijeme,
                        usluga: t.Usluga?.Naziv ?? "—"
                    );
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[EMAIL GREŠKA - Podsjetnik terminId={t.TerminId}]: {ex.Message}");
                }
            }
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
    }
}