using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HealthZone.Services
{
    public class EmailSettings
    {
        public string Host { get; set; } = null!;
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string FromEmail { get; set; } = null!;
        public string FromName { get; set; } = "HealthZone Poliklinika";
    }

    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        // ─── POTVRDA TERMINA ──────────────────────────────────────────────────

        public async Task PošaljiPotvrduTerminaAsync(string email, string imePacijenta,
            string imeDoktora, DateTime datum, TimeOnly vrijeme, string usluga, int terminId)
        {
            var subject = "✅ Potvrda zakazanog termina — HealthZone";
            var linkPotvrdi = $"http://ajsaooad1-001-site1.jtempurl.com/Termin/PotvrdiIzMaila/{terminId}";
            var linkOtkazi = $"http://ajsaooad1-001-site1.jtempurl.com/Termin/OtkaziIzMaila/{terminId}";
            var body = HtmlTemplate(
                naslov: "Termin uspješno zakazan",
                ikona: "✅",
                boja: "#0BA5A4",
                sadrzaj: $@"
        <p>Poštovani/a <strong>{imePacijenta}</strong>,</p>
        <p>Vaš termin je uspješno zakazan. U nastavku su detalji:</p>
        {InfoBox(new (string, string)[] {
    ("📅 Datum",   datum.ToString("dd.MM.yyyy")),
    ("🕐 Vrijeme", vrijeme.ToString("HH:mm")),
    ("👨 Doktor",  imeDoktora),
    ("🏥 Usluga",  usluga)
})}
        <p>Molimo vas da potvrdite ili otkažete termin klikom na jedno od dugmadi ispod.</p>

        <div style='text-align:center;margin-top:25px;display:flex;gap:16px;justify-content:center;'>
            <a href='{linkPotvrdi}'
               style='background:#0BA5A4;color:white;padding:12px 28px;
                      border-radius:8px;text-decoration:none;font-weight:bold;
                      display:inline-block;'>
                ✅ Potvrdi termin
            </a>
            <a href='{linkOtkazi}'
               style='background:#e74c3c;color:white;padding:12px 28px;
                      border-radius:8px;text-decoration:none;font-weight:bold;
                      display:inline-block;'>
                ❌ Otkaži termin
            </a>
        </div>"
            );
            await PošaljiAsync(email, subject, body);
        }

        // ─── OTKAZIVANJE TERMINA ──────────────────────────────────────────────

        public async Task PošaljiOtkazTerminaAsync(string email, string imePacijenta,
            string imeDoktora, DateTime datum, TimeOnly vrijeme)
        {
            var subject = "❌ Otkazivanje termina — HealthZone";
            var body = HtmlTemplate(
                naslov: "Termin je otkazan",
                ikona: "❌",
                boja: "#e74c3c",
                sadrzaj: $@"
                    <p>Poštovani/a <strong>{imePacijenta}</strong>,</p>
                    <p>Obavještavamo vas da je vaš termin otkazan:</p>
                    {InfoBox(new[] {
                        ("📅 Datum",   datum.ToString("dd.MM.yyyy")),
                        ("🕐 Vrijeme", vrijeme.ToString("HH:mm")),
                        ("👨‍⚕️ Doktor",  imeDoktora)
                    })}
                    <p>Zakažite novi termin putem HealthZone sistema u bilo koje vrijeme.</p>"
            );
            await PošaljiAsync(email, subject, body);
        }

        // ─── PODSJETNIK 24H ───────────────────────────────────────────────────

        public async Task PošaljiPodsjetnikAsync(string email, string imePacijenta,
            string imeDoktora, DateTime datum, TimeOnly vrijeme, string usluga)
        {
            var subject = "🔔 Podsjetnik: termin sutra — HealthZone";
            var body = HtmlTemplate(
                naslov: "Podsjetnik za termin",
                ikona: "🔔",
                boja: "#f39c12",
                sadrzaj: $@"
                    <p>Poštovani/a <strong>{imePacijenta}</strong>,</p>
                    <p>Podsjećamo vas da imate zakazan termin <strong>sutra</strong>:</p>
                    {InfoBox(new[] {
                        ("📅 Datum",   datum.ToString("dd.MM.yyyy")),
                        ("🕐 Vrijeme", vrijeme.ToString("HH:mm")),
                        ("👨‍⚕️ Doktor",  imeDoktora),
                        ("🏥 Usluga",  usluga)
                    })}
                    <p>Molimo vas da potvrdite ili otkažete termin klikom na dugme ispod.</p>"
            );
            await PošaljiAsync(email, subject, body);
        }

        // ─── OBAVJEŠTENJE S LISTE ČEKANJA ─────────────────────────────────────

        public async Task PošaljiObavještenjeListe(string email, string imePacijenta,
            string imeDoktora, DateTime datum, TimeOnly vrijeme)
        {
            var subject = "🕐 Termin dostupan — HealthZone lista čekanja";
            var body = HtmlTemplate(
                naslov: "Termin se oslobodio!",
                ikona: "🕐",
                boja: "#0BA5A4",
                sadrzaj: $@"
                    <p>Poštovani/a <strong>{imePacijenta}</strong>,</p>
                    <p>Obavještavamo vas da se <strong>oslobodio termin</strong> za kojeg ste bili na listi čekanja:</p>
                    {InfoBox(new[] {
                        ("📅 Datum",   datum.ToString("dd.MM.yyyy")),
                        ("🕐 Vrijeme", vrijeme.ToString("HH:mm")),
                        ("👨‍⚕️ Doktor",  imeDoktora)
                    })}
                    <p>Prijavite se u HealthZone sistem što prije kako biste potvrdili termin.</p>
                    <p style='color:#888;font-size:13px;'>Napomena: termin se dodjeljuje prema prioritetu i redoslijedu prijave na listu čekanja.</p>"
            );
            await PošaljiAsync(email, subject, body);
        }

        // ─── PRIVATNE HELPER METODE ───────────────────────────────────────────

        private async Task PošaljiAsync(string to, string subject, string htmlBody)
        {
            using var client = new SmtpClient(_settings.Host, _settings.Port)
            {
                EnableSsl = _settings.EnableSsl,
                Credentials = new NetworkCredential(_settings.Username, _settings.Password)
            };

            using var message = new MailMessage
            {
                From = new MailAddress(_settings.FromEmail, _settings.FromName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };
            message.To.Add(to);

            await client.SendMailAsync(message);
        }

        // ─── HTML TEMPLATE ────────────────────────────────────────────────────

        private static string InfoBox(IEnumerable<(string Label, string Vrijednost)> redovi)
        {
            var rows = string.Join("", redovi.Select(r => $@"
                <tr>
                    <td style='padding:8px 12px;color:#666;font-size:14px;width:130px'>{r.Label}</td>
                    <td style='padding:8px 12px;font-weight:600;color:#1a2236;font-size:14px'>{r.Vrijednost}</td>
                </tr>"));

            return $@"
                <table style='background:#f7f9fc;border-radius:10px;border-collapse:collapse;
                              width:100%;margin:18px 0;'>
                    {rows}
                </table>";
        }

        private static string HtmlTemplate(string naslov, string ikona, string boja, string sadrzaj)
        {
            return $@"
<!DOCTYPE html>
<html lang='bs'>
<head><meta charset='UTF-8'/></head>
<body style='margin:0;padding:0;background:#f0f4f8;font-family:Inter,Segoe UI,sans-serif;'>
  <table width='100%' cellpadding='0' cellspacing='0' style='padding:40px 20px;'>
    <tr><td align='center'>
      <table width='560' cellpadding='0' cellspacing='0'
             style='background:#fff;border-radius:18px;overflow:hidden;
                    box-shadow:0 4px 24px rgba(0,0,0,.08);'>

        <!-- Header -->
        <tr>
          <td style='background:{boja};padding:28px 36px;text-align:center;'>
            <div style='font-size:36px;margin-bottom:8px;'>{ikona}</div>
            <h1 style='margin:0;color:#fff;font-size:22px;font-weight:700;
                       letter-spacing:-.3px;'>{naslov}</h1>
          </td>
        </tr>

        <!-- Body -->
        <tr>
          <td style='padding:32px 36px;color:#333;font-size:15px;line-height:1.6;'>
            {sadrzaj}
          </td>
        </tr>

        <!-- Footer -->
        <tr>
          <td style='background:#f7f9fc;padding:20px 36px;border-top:1px solid #eee;
                     text-align:center;'>
            <p style='margin:0;font-size:12px;color:#aaa;'>
              HealthZone Poliklinika &bull; Automatska poruka, ne odgovarajte na ovaj email.
            </p>
          </td>
        </tr>

      </table>
    </td></tr>
  </table>
</body>
</html>";
        }
    }
}