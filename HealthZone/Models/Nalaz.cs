using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace HealthZone.Models
{
	public class Nalaz
    {
        public Nalaz() { }
        [Key]
        public int NalazId { get; set; }
        [Required(ErrorMessage = "Polje Opis je obavezno.")]
        public string Opis { get; set; }
        [Required(ErrorMessage = "Polje Dijagnoza je obavezno.")]
        public string Dijagnoza { get; set; }
        [Required(ErrorMessage = "Polje Terapija je obavezno.")]
        public string Terapija { get; set; }

        [ForeignKey("Termin")]
        [Display(Name = "Termin")]
        [Required(ErrorMessage = "Polje Termin je obavezno.")]
        public int TerminID { get; set; }
        public Termin? Termin { get; set; }

        [ForeignKey("Korisnik")]
        [Display(Name = "Pacijent")]
        [Required(ErrorMessage = "Polje Pacijent je obavezno.")]
        public string PacijentID { get; set; }
        public Korisnik? Pacijent { get; set; }
    }
}