using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace HealthZone.Models
{ 

public class Korisnik : IdentityUser
{
    public Korisnik() { }

    [Required(ErrorMessage = "Polje Ime je obavezno.")]
    public string Ime { get; set; }
    [Required(ErrorMessage = "Polje Prezime je obavezno.")]
    public string Prezime { get; set; }  
    public string? BrojKartona { get; set; }
    public Prioritet? Prioritet { get; set; }
    public Specijalizacija? Specijalizacija { get; set; }    

    }
}
