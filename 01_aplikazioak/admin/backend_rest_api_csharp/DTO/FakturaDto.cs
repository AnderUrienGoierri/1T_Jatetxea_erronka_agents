using System.ComponentModel.DataAnnotations;

namespace Jatetxea.Api.DTO;

public class FakturaDTO
{
    public int Id { get; set; }
    public int? EskaeraId { get; set; }
    public string FakturaZenbakia { get; set; } = string.Empty;
    public string Mota { get; set; } = "Salmenta"; // "Salmenta", "Erosketa"
    public string Azpimota { get; set; } = "Sinplifikatua"; // "Sinplifikatua", "Osoa"
    public string Kontzeptua { get; set; } = string.Empty;
    public string? Hornitzailea { get; set; }
    public string? HornitzaileCif { get; set; }
    public string? BezeroNif { get; set; }
    public string? BezeroIzena { get; set; }
    public string? BezeroHelbidea { get; set; }
    public decimal OinarriZergagarria { get; set; }
    public decimal BezEhunekoa { get; set; }
    public decimal BezZenbatekoa { get; set; }
    public decimal Guztira { get; set; }
    public string OrdainketaMetodoa { get; set; } = "Txartela";
    public string Egoera { get; set; } = "Ordaindua"; // "Ordaindua", "Zain", "Ezeztatua"
    public DateTime Data { get; set; }
    public string? Oharrak { get; set; }
    public bool Ezabatua { get; set; }
}

public class FakturaSortuDto
{
    public int? EskaeraId { get; set; }
    public string? FakturaZenbakia { get; set; }

    [Required]
    public string Mota { get; set; } = "Salmenta"; // "Salmenta" edo "Erosketa"

    public string Azpimota { get; set; } = "Sinplifikatua"; // "Sinplifikatua" edo "Osoa"

    [Required]
    [StringLength(255)]
    public string Kontzeptua { get; set; } = string.Empty;

    public string? Hornitzailea { get; set; }
    public string? HornitzaileCif { get; set; }
    public string? BezeroNif { get; set; }
    public string? BezeroIzena { get; set; }
    public string? BezeroHelbidea { get; set; }

    [Range(0, 1000000)]
    public decimal OinarriZergagarria { get; set; }

    [Range(0, 100)]
    public decimal BezEhunekoa { get; set; } = 10.00m;

    public decimal? Guztira { get; set; }

    public string OrdainketaMetodoa { get; set; } = "Txartela";
    public string Egoera { get; set; } = "Ordaindua";
    public DateTime? Data { get; set; }
    public string? Oharrak { get; set; }
}

public class FakturaEgoeraEguneratuDto
{
    [Required]
    public string Egoera { get; set; } = "Ordaindua"; // "Ordaindua", "Zain", "Ezeztatua"
}

public class FinantzaLaburpenaDto
{
    public decimal SalmentakGuztira { get; set; } // Diru-sarrerak (Ingresos)
    public decimal GastuakGuztira { get; set; }   // Gastuak (Gastos)
    public decimal MozkinGarbia => SalmentakGuztira - GastuakGuztira; // Irabazia (Beneficio Neto)
    public int SalmentaKopurua { get; set; }
    public int GastuKopurua { get; set; }
    public decimal JasotakoBez { get; set; }     // Salmentetako BEZa (IVA Repercutido)
    public decimal OrdaindutakoBez { get; set; } // Erosketetako BEZa (IVA Soportado)
    public decimal BezLikidazioa => JasotakoBez - OrdaindutakoBez; // Ogasunari ordaindu beharrekoa
}
