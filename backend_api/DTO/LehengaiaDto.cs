using System.ComponentModel.DataAnnotations;

namespace Jatetxea.Api.DTO;

public class LehengaiaDTO
{
    public int Id { get; set; }
    public string Izena { get; set; } = string.Empty;
    public string Unitatea { get; set; } = string.Empty;
    public decimal UnekoStocka { get; set; }
    public decimal GutxienekoStocka { get; set; }
    public decimal KostuUnitarioa { get; set; }
    public string? Hornitzailea { get; set; }
    public bool Ezabatua { get; set; }
}

public class LehengaiaSortuDto
{
    [Required]
    [StringLength(100)]
    public string Izena { get; set; } = string.Empty;

    [Required]
    public string Unitatea { get; set; } = "kg"; // kg, g, l, ml, unitatea

    [Range(0, 999999)]
    public decimal UnekoStocka { get; set; } = 0;

    [Range(0, 999999)]
    public decimal GutxienekoStocka { get; set; } = 5;

    [Range(0, 999999)]
    public decimal KostuUnitarioa { get; set; } = 0;

    public string? Hornitzailea { get; set; }
}

public class LehengaiaAlertaDTO
{
    public int LehengaiaId { get; set; }
    public string Izena { get; set; } = string.Empty;
    public string Unitatea { get; set; } = string.Empty;
    public decimal UnekoStocka { get; set; }
    public decimal GutxienekoStocka { get; set; }
    public decimal ProposatutakoErosketa { get; set; }
    public string? Hornitzailea { get; set; }
}

public class AlbaranaSarreraDto
{
    [Required]
    public int LehengaiaId { get; set; }

    [Required]
    [Range(0.001, 999999.0)]
    public decimal Kantitatea { get; set; }

    public string? Hornitzailea { get; set; }
    public string? Oharrak { get; set; }
}
