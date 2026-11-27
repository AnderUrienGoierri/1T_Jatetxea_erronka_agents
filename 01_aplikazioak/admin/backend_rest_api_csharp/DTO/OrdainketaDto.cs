using System.ComponentModel.DataAnnotations;

namespace Jatetxea.Api.DTO;

public class OrdainketaEskaeraDto
{
    [Required]
    public int EskaeraId { get; set; }

    [Required]
    public string Mota { get; set; } = "Txartela"; // 'Eskudirua', 'Txartela', 'Bizum', 'GooglePay', 'ApplePay'

    [Required]
    [Range(0.01, 99999.99)]
    public decimal Zenbatekoa { get; set; }

    public decimal? Jasotakoa { get; set; } // Eskudirutan emandako zenbatekoa

    public string? TxartelZenbakia { get; set; }

    public string? Titularra { get; set; }

    public string? Iraungitzea { get; set; }

    public string? Cvv { get; set; }

    public string? BizumTelefonoa { get; set; }

    public int? LangileaId { get; set; }
}

public class OrdainketaErantzunaDto
{
    public int Id { get; set; }
    public int EskaeraId { get; set; }
    public string TiketZenbakia { get; set; } = string.Empty;
    public string Mota { get; set; } = string.Empty;
    public decimal Zenbatekoa { get; set; }
    public decimal? Jasotakoa { get; set; }
    public decimal? Itzulitakoa { get; set; }
    public string? TxartelMaskaratua { get; set; }
    public string? TxartelMarka { get; set; }
    public string OperazioKodea { get; set; } = string.Empty;
    public string? TokenZifratua { get; set; }
    public string Egoera { get; set; } = "Arrakastatsua";
    public string Data { get; set; } = string.Empty;
    public string TiketTestua { get; set; } = string.Empty;
    public decimal Azpitotala { get; set; }
    public decimal Bez { get; set; }
}

/// <summary>
/// Ordainketa zatituaren zati bakoitza (adib. 20 EUR Eskudirua + 35 EUR Txartela).
/// </summary>
public class OrdainketaZatiaDto
{
    [Required]
    public string Mota { get; set; } = "Txartela";

    [Required]
    [Range(0.01, 99999.99)]
    public decimal Zenbatekoa { get; set; }

    public decimal? Jasotakoa { get; set; }
    public string? TxartelZenbakia { get; set; }
    public string? BizumTelefonoa { get; set; }
}

/// <summary>
/// Transakzio konposatua: eskaera bakarra hainbat ordainketa-motatan zatitu eta salmenta-faktura sortu.
/// </summary>
public class OrdainketaZatikatuDto
{
    [Required]
    public int EskaeraId { get; set; }

    public int? LangileaId { get; set; }

    public bool SortuFaktura { get; set; } = true;
    public string? BezeroNif { get; set; }
    public string? BezeroIzena { get; set; }
    public string? BezeroHelbidea { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "Gutxienez ordainketa-zati bat zehaztu behar da.")]
    public List<OrdainketaZatiaDto> Zatiak { get; set; } = new();
}

public class OrdainketaZatikatuErantzunaDto
{
    public int EskaeraId { get; set; }
    public decimal EskaeraGuztira { get; set; }
    public decimal OrdaindutakoaGuztira { get; set; }
    public decimal Azpitotala { get; set; }
    public decimal Bez { get; set; }
    public List<OrdainketaErantzunaDto> Ordainketak { get; set; } = new();
    public FakturaDTO? Faktura { get; set; }
}
