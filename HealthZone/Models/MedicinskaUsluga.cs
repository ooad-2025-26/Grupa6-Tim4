using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace HealthZone.Models
{
	public class MedicinskaUsluga
	{
		MedicinskaUsluga() { }
        [Key]
        public int UslugaId { get; set; }
		public string Naziv { get; set; }
        public Specijalizacija Vrsta { get; set; }
        public string Opis { get; set; }
		public decimal Cijena { get; set; }
	
		
	}
}