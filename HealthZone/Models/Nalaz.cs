using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace HealthZone.Models
{
	public class Nalaz
    {
        Nalaz() { }
        [Key]
        public int NalazId { get; set; }
        public string Opis { get; set; }
        public string Dijagnoza { get; set; }
        public string Terapija { get; set; }

        [ForeignKey("Termin")]
        public int TerminID { get; set; }
        public Termin Termin { get; set; }

        [ForeignKey("Korisnik")]
        public string PacijentID { get; set; }
        public Korisnik Pacijent { get; set; }
    }
}