using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace HealthZone.Models { 
public class ListaCekanja
    {
        public ListaCekanja() { }
        [Key]
        public int ListaId { get; set; }
       
        [ForeignKey("Korisnik")]
        [Display(Name = "Doktor")]
        [Required(ErrorMessage = "Polje Doktor je obavezno.")]
        public string DoktorID { get; set; }
        public Korisnik? Doktor { get; set; }

    }
}