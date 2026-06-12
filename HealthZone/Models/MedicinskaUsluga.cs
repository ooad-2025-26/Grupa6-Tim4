using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace HealthZone.Models
{
	public class MedicinskaUsluga
	{
		public MedicinskaUsluga() { }
        [Key]
        public int UslugaId { get; set; }
        [Required(ErrorMessage = "Polje Naziv je obavezno.")]
        public string Naziv { get; set; }
        [Required(ErrorMessage = "Polje Vrsta je obavezno.")]
        public Specijalizacija Vrsta { get; set; }
        [Required(ErrorMessage = "Polje Opis je obavezno.")]
        public string Opis { get; set; }
        [Required(ErrorMessage = "Polje Cijena je obavezno.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cijena mora biti veća od 0.")]
        public decimal Cijena { get; set; }
	
		
	}
}