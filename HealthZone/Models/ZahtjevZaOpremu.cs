using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace HealthZone.Models
{
	public class ZahtjevZaOpremu
	{
		ZahtjevZaOpremu() { }
		[Key]
		public int ZahtjevId { get; set; }
		public string Naziv { get; set; }
		public string Opis { get; set; }
        public Status Status { get; set; }

        [ForeignKey("Korisnik")]
        public string DoktorID { get; set; }
        public Korisnik Doktor { get; set; }

        public VrstaZahtjeva VrstaZahtjeva { get; set; }
		public KategorijaOpreme Kategorija { get; set; }
		public Hitnost Hitnost { get; set; }
		
	
		
	}
}