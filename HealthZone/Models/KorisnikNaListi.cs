using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace HealthZone.Models
{
	public class KorisnikNaListi
    {
		public KorisnikNaListi(){ }
        [Key]
        public int KorisnikNaListiID { get; set; }
        public DateTime Datum { get; set; }

        [ForeignKey("ListaCekanja")]
        public int ListaID { get; set; }
        public ListaCekanja Lista { get; set; }

        [ForeignKey("Korisnik")]
        public string KorisnikID { get; set; }
        public Korisnik Korisnik { get; set; }

    }
}