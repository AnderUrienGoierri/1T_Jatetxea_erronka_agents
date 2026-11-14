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
