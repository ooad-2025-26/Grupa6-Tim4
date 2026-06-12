using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace HealthZone.Models
{
	public class Termin
	{
        public Termin() { }
        [Key]
        public int TerminId { get; set; }
   
        [Required(ErrorMessage = "Polje Datum je obavezno.")]
        public DateTime Datum { get; set; }
        
        [Required(ErrorMessage = "Polje Vrijeme je obavezno.")]
        public TimeOnly Vrijeme { get; set; }
		public Status Status { get; set; }

        [ForeignKey("Korisnik")]
        [Display(Name = "Doktor")]
        [Required(ErrorMessage = "Polje Doktor je obavezno.")]
        public string DoktorID { get; set; }
		public Korisnik? Doktor { get; set; }

        [ForeignKey("Korisnik")]
        [Display(Name = "Pacijent")]
        [Required(ErrorMessage = "Polje Pacijent je obavezno.")]
        public string PacijentID { get; set; }
        public Korisnik? Pacijent { get; set; }

        [ForeignKey("MedicinskaUsluga")]
        [Display(Name = "MedicinskaUsluga")]
        [Required(ErrorMessage = "Polje Usluga je obavezno.")]
        public int UslugaID { get; set; }
        public MedicinskaUsluga? Usluga { get; set; }
    }
}