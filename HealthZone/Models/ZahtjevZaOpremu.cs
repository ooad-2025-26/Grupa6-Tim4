using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace HealthZone.Models
{
	public class ZahtjevZaOpremu
	{
		public ZahtjevZaOpremu() { }
		[Key]
		public int ZahtjevId { get; set; }
        [Required(ErrorMessage = "Polje Naziv je obavezno.")]
        public string Naziv { get; set; }
        [Required(ErrorMessage = "Polje Opis je obavezno.")]
        public string Opis { get; set; }
        public Status Status { get; set; }

        [ForeignKey("Korisnik")]
        [Display(Name = "Doktor")]
        [Required(ErrorMessage = "Polje Doktor je obavezno.")]
        public string DoktorID { get; set; }
        public Korisnik Doktor { get; set; }
        [Required(ErrorMessage = "Polje Vrsta zahtjeva je obavezno.")]
        public VrstaZahtjeva VrstaZahtjeva { get; set; }
        [Required(ErrorMessage = "Polje Kategorija je obavezno.")]
        public KategorijaOpreme Kategorija { get; set; }
        [Required(ErrorMessage = "Polje Hitnost je obavezno.")]
        public Hitnost Hitnost { get; set; }
		
	
		
	}
}