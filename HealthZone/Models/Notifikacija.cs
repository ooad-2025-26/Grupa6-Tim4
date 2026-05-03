using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace HealthZone.Models
{
	public class Notifikacija
    {
		Notifikacija() { }
        [Key]
        public int NotifikacijaId { get; set; }
		public string Poruka { get; set; }
		public DateTime DatumSlanja { get; set; }
		public Status status { get; set; }

        [ForeignKey("Termin")]
        public int TerminID { get; set; }
		public Termin Termin { get; set; }
    }
}