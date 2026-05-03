using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace HealthZone.Models
{
	public class Recenzija
	{
		Recenzija() { }
        [Key]
        public int RecenzijaId { get; set; }
		public string Komentar { get; set; }
		public int OcjenaLjubaznosti { get; set; }
		public int OcjenaProfesionalnosti { get; set; }
		public int OcjenaUsluge { get; set; }

        [ForeignKey("Korisnik")]
        public string PacijentID { get; set; }
		public Korisnik Pacijent { get; set; }

        [ForeignKey("Korisnik")]
        public string DoktorID { get; set; }
		public Korisnik Doktor { get; set; }
	
		
	}
}