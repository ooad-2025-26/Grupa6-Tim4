using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HealthZone.Models;
namespace HealthZone.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<Korisnik>(options)
    {
        public DbSet<Termin> Termin { get; set; }
        public DbSet<Nalaz> Nalaz { get; set; }
        public DbSet<Recenzija> Recenzija { get; set; }
        public DbSet<ZahtjevZaOpremu> ZahtjevZaOpremu { get; set; }
        public DbSet<ListaCekanja> ListaCekanja { get; set; }
        public DbSet<KorisnikNaListi> KorisnikNaListi { get; set; }
        public DbSet<Notifikacija> Notifikacija { get; set; }
        public DbSet<MedicinskaUsluga> MedicinskaUsluga { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Korisnik>().ToTable("Korisnici");
            modelBuilder.Entity<Termin>().ToTable("Termini");
            modelBuilder.Entity<Nalaz>().ToTable("Nalazi");
            modelBuilder.Entity<Recenzija>().ToTable("Recenzije");
            modelBuilder.Entity<ZahtjevZaOpremu>().ToTable("Zahtjevi");
            modelBuilder.Entity<ListaCekanja>().ToTable("ListeCekanja");
            modelBuilder.Entity<KorisnikNaListi>().ToTable("KorisniciNaListi");
            modelBuilder.Entity<Notifikacija>().ToTable("Notifikacije");
            modelBuilder.Entity<MedicinskaUsluga>().ToTable("MedicinskeUsluge");
            modelBuilder.Entity<Recenzija>()
    .HasOne(r => r.Doktor)
    .WithMany()
    .HasForeignKey(r => r.DoktorID)
    .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Recenzija>()
                .HasOne(r => r.Pacijent)
                .WithMany()
                .HasForeignKey(r => r.PacijentID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Termin>()
                .HasOne(t => t.Doktor)
                .WithMany()
                .HasForeignKey(t => t.DoktorID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Termin>()
                .HasOne(t => t.Pacijent)
                .WithMany()
                .HasForeignKey(t => t.PacijentID)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<KorisnikNaListi>()
    .HasOne(k => k.Lista)
    .WithMany()
    .HasForeignKey(k => k.ListaID)
    .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<KorisnikNaListi>()
                .HasOne(k => k.Korisnik)
                .WithMany()
                .HasForeignKey(k => k.KorisnikID)
                .OnDelete(DeleteBehavior.NoAction);
            base.OnModelCreating(modelBuilder);
        }
    }
}
