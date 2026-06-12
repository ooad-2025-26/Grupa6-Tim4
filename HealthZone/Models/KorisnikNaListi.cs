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

        [Required(ErrorMessage = "Polje Datum je obavezno.")]
        public DateTime Datum { get; set; }

        [ForeignKey("ListaCekanja")]
        [Required(ErrorMessage = "Polje Lista čekanja je obavezno.")]
        public int ListaID { get; set; }
        public ListaCekanja? Lista { get; set; }

        [ForeignKey("Korisnik")]
        [Required(ErrorMessage = "Polje Korisnik je obavezno.")]
        public string KorisnikID { get; set; }
        public Korisnik? Korisnik { get; set; }

    }
}