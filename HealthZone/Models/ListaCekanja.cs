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
        public string DoktorID { get; set; }
        public Korisnik Doktor { get; set; }

    }
}