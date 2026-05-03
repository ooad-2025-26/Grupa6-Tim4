using System;
using Microsoft.AspNetCore.Identity;

namespace HealthZone.Models
{ 

public class Korisnik : IdentityUser
{
    Korisnik() { }
	public string Ime { get; set; } 
    public string Prezime { get; set; } 
    public string BrojKartona { get; set; }
    public Prioritet Prioritet { get; set; }
    public Specijalizacija Specijalizacija { get; set; }    

    }
}
