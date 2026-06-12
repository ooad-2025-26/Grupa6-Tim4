using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthZone.Models
{
    public class Recenzija
    {
        public Recenzija() { }

        [Key]
        public int RecenzijaId { get; set; }

        public string? Komentar { get; set; }
        [Range(1, 5, ErrorMessage = "Ocjena ljubaznosti mora biti između 1 i 5.")]
        public int OcjenaLjubaznosti { get; set; }
        [Range(1, 5, ErrorMessage = "Ocjena profesionalnosti mora biti između 1 i 5.")]
        public int OcjenaProfesionalnosti { get; set; }
        [Range(1, 5, ErrorMessage = "Ocjena usluge mora biti između 1 i 5.")]
        public int OcjenaUsluge { get; set; }

        [ForeignKey("Pacijent")]
        [Required(ErrorMessage = "Polje Pacijent je obavezno.")]
        public string PacijentID { get; set; }
        public Korisnik? Pacijent { get; set; }

        [ForeignKey("Doktor")]
        [Display(Name = "Doktor")]
        [Required(ErrorMessage = "Polje Doktor je obavezno.")]
        public string DoktorID { get; set; }
        public Korisnik? Doktor { get; set; }
    }
}