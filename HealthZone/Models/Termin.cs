using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace HealthZone.Models
{
	public class Termin
	{
        Termin() { }
        [Key]
        public int TerminId { get; set; }	
		public DateTime Datum { get; set; }
		public TimeOnly Vrijeme { get; set; }
		public Status Status { get; set; }

        [ForeignKey("Korisnik")]
        public string DoktorID { get; set; }
		public Korisnik Doktor { get; set; }

        [ForeignKey("Korisnik")]
        public string PacijentID { get; set; }
        public Korisnik Pacijent { get; set; }

        [ForeignKey("MedicinskaUsluga")]
        public int UslugaID { get; set; }
        public MedicinskaUsluga Usluga { get; set; }
    }
}