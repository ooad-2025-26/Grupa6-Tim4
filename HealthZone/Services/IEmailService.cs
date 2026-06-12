namespace HealthZone.Services
{
    public interface IEmailService
    {
        // Potvrda nakon zakazivanja termina
        Task PošaljiPotvrduTerminaAsync(string email, string imePacijenta,
            string imeDoktora, DateTime datum, TimeOnly vrijeme, string usluga, int terminId);

        // Otkazivanje termina
        Task PošaljiOtkazTerminaAsync(string email, string imePacijenta,
            string imeDoktora, DateTime datum, TimeOnly vrijeme);

        // Podsjetnik 24h prije termina
        Task PošaljiPodsjetnikAsync(string email, string imePacijenta,
            string imeDoktora, DateTime datum, TimeOnly vrijeme, string usluga);

        // Obavještenje da se oslobodio termin (lista čekanja)
        Task PošaljiObavještenjeListe(string email, string imePacijenta,
            string imeDoktora, DateTime datum, TimeOnly vrijeme);
    }
}