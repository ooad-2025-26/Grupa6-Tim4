using HealthZone.Models;

namespace HealthZone.Services
{
    public class PodsjetnikHostedService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<PodsjetnikHostedService> _logger;

        public PodsjetnikHostedService(
            IServiceScopeFactory scopeFactory,
            ILogger<PodsjetnikHostedService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.WhenAll(
                PodsjetnikLoopAsync(stoppingToken),
                StatusLoopAsync(stoppingToken)
            );
        }

        // Svaki dan u 08:00 šalje podsjetnik email
        private async Task PodsjetnikLoopAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var sada = DateTime.Now;
                var sljedeće = DateTime.Today.AddDays(1).AddHours(8);
                var čekanje = sljedeće - sada;

                _logger.LogInformation("Podsjetnik: sljedeće slanje u {Sljedece}", sljedeće);
                await Task.Delay(čekanje, stoppingToken);

                if (stoppingToken.IsCancellationRequested) break;

                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var terminService = scope.ServiceProvider.GetRequiredService<ITerminService>();
                    await terminService.PošaljiPodsjetnikeSutraAsync();
                    _logger.LogInformation("Podsjetnici poslani za {Datum}", DateTime.Today.AddDays(1).ToString("dd.MM.yyyy"));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Greška pri slanju podsjetnika.");
                }
            }
        }

        // Svake minute provjerava i mijenja statuse termina
        private async Task StatusLoopAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var terminService = scope.ServiceProvider.GetRequiredService<ITerminService>();

                    var sada = DateTime.Now;

                    var sviTermini = await terminService.GetAllAsync();

                    foreach (var termin in sviTermini)
                    {
                        // Precizno poređenje — datum + vrijeme zajedno
                        var vrijemeTermina = termin.Datum.Date.Add(termin.Vrijeme.ToTimeSpan());

                        // Koliko vremena ostaje do termina
                        var preostalo = vrijemeTermina - sada;

                        if (termin.Status == Status.NaCekanju)
                        {
                            // Ako termin za manje od 24h → treba potvrda 12h ranije
                            // Ako termin za 24h+ → treba potvrda 24h ranije
                            bool trebaPotvrdu = preostalo.TotalHours <= 12;

                            if (trebaPotvrdu)
                            {
                                await terminService.OtkaziTerminAsync(termin.TerminId);
                                _logger.LogInformation(
                                    "Termin {Id} automatski otkazan (nije potvrđen na vrijeme, preostalo {H:F1}h).",
                                    termin.TerminId, preostalo.TotalHours);
                            }
                        }
                        else if (termin.Status == Status.Potvrdjen)
                        {
                            // Termin se desio → postavi na Aktivan
                            if (vrijemeTermina <= sada)
                            {
                                await terminService.OznaciKaoAktivanAsync(termin.TerminId);
                                _logger.LogInformation("Termin {Id} postavljen na Aktivan.", termin.TerminId);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Greška pri provjeri statusa termina.");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}