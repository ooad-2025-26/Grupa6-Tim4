using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace HealthZone.Models
{
	public class Notifikacija
    {
		public Notifikacija() { }
        [Key]
        public int NotifikacijaId { get; set; }
        [Required(ErrorMessage = "Polje Poruka je obavezno.")]
        public string Poruka { get; set; }
		public DateTime DatumSlanja { get; set; }
		public Status status { get; set; }

        [ForeignKey("Termin")]
        [Required(ErrorMessage = "Polje Termin je obavezno.")]
        public int TerminID { get; set; }
		public Termin Termin { get; set; }
    }
}