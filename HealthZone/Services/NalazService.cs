using HealthZone.Models;
using HealthZone.Repositories;
using Microsoft.Extensions.Configuration;

namespace HealthZone.Services
{
    public class NalazService : INalazService
    {
        private readonly INalazRepository _nalazRepository;
        private readonly ITerminRepository _terminRepository;
        private readonly IConfiguration _config;

        public NalazService(
            INalazRepository nalazRepository,
            ITerminRepository terminRepository,
            IConfiguration configuration)
        {
            _nalazRepository = nalazRepository;
            _terminRepository = terminRepository;
            _config = configuration;
        }

        public async Task<Nalaz?> GetByIdAsync(int id)
            => await _nalazRepository.GetByIdAsync(id);

        public async Task<IEnumerable<Nalaz>> GetAllAsync()
            => await _nalazRepository.GetAllAsync();

        public void Update(Nalaz n) => _nalazRepository.Update(n);
        public void Delete(Nalaz n) => _nalazRepository.Delete(n);

        public async Task<int> SaveChangesAsync()
            => await _nalazRepository.SaveChangesAsync();

        // ─── DODAJ NALAZ ──────────────────────────────────────────────────────

        public async Task AddAsync(Nalaz nalaz)
        {
            if (string.IsNullOrWhiteSpace(nalaz.Dijagnoza))
                throw new Exception("Dijagnoza je obavezna.");

            if (string.IsNullOrWhiteSpace(nalaz.Opis))
                throw new Exception("Opis nalaza je obavezan.");

            var termin = await _terminRepository.GetByIdAsync(nalaz.TerminID)
                ?? throw new Exception("Termin nije pronađen.");

            if (termin.Status != Status.Aktivan)
                throw new Exception("Nalaz se može dodati samo za aktivan (obavljen) termin.");

            var svi = await _nalazRepository.GetAllAsync();
            if (svi.Any(n => n.TerminID == nalaz.TerminID))
                throw new Exception("Za ovaj termin već postoji nalaz.");

            nalaz.PacijentID = termin.PacijentID;

            await _nalazRepository.AddAsync(nalaz);
            await _nalazRepository.SaveChangesAsync();
        }

        // ─── NALAZI PO PACIJENTU ──────────────────────────────────────────────

        public async Task<IEnumerable<Nalaz>> GetNalaziPacijentaAsync(string pacijentId)
        {
            var svi = await _nalazRepository.GetAllAsync();
            return svi
                .Where(n => n.PacijentID == pacijentId)
                .OrderByDescending(n => n.Termin?.Datum);
        }

        // ─── NALAZ ZA TERMIN ──────────────────────────────────────────────────

        public async Task<Nalaz?> GetNalazZaTerminAsync(int terminId)
        {
            var svi = await _nalazRepository.GetAllAsync();
            return svi.FirstOrDefault(n => n.TerminID == terminId);
        }

        // ─── GENERIRAJ HTML ZA PRINTANJE ──────────────────────────────────────

        public async Task<string> GenerirajHtmlZaPrintAsync(int nalazId)
        {
            var nalaz = await _nalazRepository.GetByIdAsync(nalazId)
                ?? throw new Exception("Nalaz nije pronađen.");

            return $@"
<!DOCTYPE html>
<html lang='bs'>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 40px; color: #1a1a1a; }}
        .header {{ text-align: center; border-bottom: 2px solid #0BA5A4; padding-bottom: 12px; margin-bottom: 24px; }}
        .logo {{ color: #0BA5A4; font-size: 22px; font-weight: bold; }}
        .field {{ margin: 8px 0; font-size: 14px; }}
        .label {{ font-weight: bold; color: #555; }}
        .box {{ border: 1px solid #ddd; padding: 14px; margin-top: 8px; border-radius: 6px; font-size: 14px; }}
        .footer {{ margin-top: 50px; font-size: 11px; color: #aaa; text-align: center; }}
        @media print {{ .no-print {{ display: none; }} }}
    </style>
</head>
<body>
    <div class='header'>
        <div class='logo'>🏥 HealthZone Poliklinika</div>
        <p style='margin:4px 0; color:#555;'>Medicinski nalaz</p>
    </div>

    <div class='field'><span class='label'>Pacijent:</span> {nalaz.Pacijent?.Ime} {nalaz.Pacijent?.Prezime}</div>
    <div class='field'><span class='label'>Broj kartona:</span> {nalaz.Pacijent?.BrojKartona}</div>
    <div class='field'><span class='label'>Datum pregleda:</span> {nalaz.Termin?.Datum:dd.MM.yyyy} u {nalaz.Termin?.Vrijeme:HH:mm}</div>
    <div class='field'><span class='label'>Dijagnoza:</span> {nalaz.Dijagnoza}</div>

    <br/>
    <div class='label'>Opis nalaza:</div>
    <div class='box'>{nalaz.Opis}</div>

    {(string.IsNullOrEmpty(nalaz.Terapija) ? "" : $@"
    <br/>
    <div class='label'>Terapija:</div>
    <div class='box'>{nalaz.Terapija}</div>")}

    <div class='footer'>
        HealthZone Poliklinika — Generisano {DateTime.Now:dd.MM.yyyy HH:mm}
    </div>

    <br/>
    <button class='no-print' onclick='window.print()'
        style='background:#0BA5A4;color:#fff;border:none;padding:10px 24px;
               border-radius:8px;cursor:pointer;font-size:15px;'>
        🖨️ Štampaj nalaz
    </button>
</body>
</html>";
        }

        // ─── POŠALJI NALAZ NA EMAIL ───────────────────────────────────────────
        //
        // Zahtijeva konfiguraciju u appsettings.json:
        //
        //   "EmailSettings": {
        //     "SmtpHost": "smtp.gmail.com",
        //     "SmtpPort": "587",
        //     "SmtpUser": "vasadresa@gmail.com",
        //     "SmtpPass": "vas-app-password"
        //   }
        //
        // Za Gmail: uključiti dvofaktorsku autentifikaciju i
        // generisati "App password" na: myaccount.google.com/apppasswords
        // Za testiranje bez pravog servera koristiti: https://mailtrap.io
        // ─────────────────────────────────────────────────────────────────────

        public async Task PosaljiNaEmailAsync(int nalazId)
        {
            var nalaz = await _nalazRepository.GetByIdAsync(nalazId)
                ?? throw new Exception("Nalaz nije pronađen.");

            var emailAdresa = nalaz.Pacijent?.Email;
            if (string.IsNullOrWhiteSpace(emailAdresa))
                throw new Exception("Pacijent nema registrovanu email adresu.");

            var htmlTijelo = await GenerirajHtmlZaPrintAsync(nalazId);
            var imePacijenta = $"{nalaz.Pacijent?.Ime} {nalaz.Pacijent?.Prezime}".Trim();

            // Čitanje SMTP postavki iz appsettings.json
            var smtpHost = _config["EmailSettings:SmtpHost"] ?? "smtp.gmail.com";
            var smtpPort = int.Parse(_config["EmailSettings:SmtpPort"] ?? "587");
            var smtpUser = _config["EmailSettings:SmtpUser"]
                ?? throw new Exception("SMTP korisnik nije konfigurisan (EmailSettings:SmtpUser).");
            var smtpPass = _config["EmailSettings:SmtpPass"]
                ?? throw new Exception("SMTP lozinka nije konfigurirana (EmailSettings:SmtpPass).");

            using var poruka = new System.Net.Mail.MailMessage
            {
                From = new System.Net.Mail.MailAddress(smtpUser, "HealthZone Poliklinika"),
                Subject = "Vaš medicinski nalaz – HealthZone",
                Body = htmlTijelo,
                IsBodyHtml = true
            };
            poruka.To.Add(new System.Net.Mail.MailAddress(emailAdresa, imePacijenta));

            using var klijent = new System.Net.Mail.SmtpClient(smtpHost, smtpPort)
            {
                EnableSsl = true,
                Credentials = new System.Net.NetworkCredential(smtpUser, smtpPass)
            };

            await klijent.SendMailAsync(poruka);
        }
    }
}