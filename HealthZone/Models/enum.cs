using System;
namespace HealthZone.Models
{
	public enum Prioritet
	{
		Nizak,
		Srednji,
		Visok
	}
	public enum Specijalizacija
	{
		OpcaMedicina,
		Kardiologija,
		Pedijatrija,
		Dermatologija
	}
	public enum Status{
		Aktivan,
		Otkazan,
		NaCekanju,
		Potvrdjen
	}
	public enum VrstaZahtjeva
	{
		Nabavka,
		Popravka,
        Zamjena,
		Servisiranje

    }
	public enum KategorijaOpreme
	{
		Dijagnosticka,
		Hirurska,
		Laboratorijska,
		Rehabilitacijska

    }
	public enum Hitnost
	{
		Nizak,
		Visok,
        Srednji,
		Hitno
    }


}