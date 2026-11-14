using System.ComponentModel.DataAnnotations;

namespace Jatetxea.Api.DTO;

public class PlateraDTO
{
    public int Id { get; set; }
    public string Izena { get; set; } = string.Empty;
    public string? Deskribapena { get; set; }
    public string Kategoria { get; set; } = string.Empty;
    public decimal Prezioa { get; set; }
    public string? Alergenoak { get; set; }
    public bool Aktibo { get; set; }
    public bool Ezabatua { get; set; }
}

public class PlaterOsagaiaDto
{
    [Required]
    public int LehengaiaId { get; set; }
    public string? LehengaiaIzena { get; set; }

    [Required]
    [Range(0.001, 999999.0)]
    public decimal Kantitatea { get; set; }

    public string? Unitatea { get; set; }
}

public class PlateraSortuDto
{
    [Required]
    [StringLength(100)]
    public string Izena { get; set; } = string.Empty;

    public string? Deskribapena { get; set; }

    [Required]
    public string Kategoria { get; set; } = "Bigarrena"; // Sarrerakoa, Lehena, Bigarrena, Postrea, Edaria

    [Required]
    [Range(0, 999999)]
    public decimal Prezioa { get; set; }

    public string? Alergenoak { get; set; }

    public List<PlaterOsagaiaDto>? Osagaiak { get; set; }
}

public class PlaterErabilgarritasunaDTO
{
    public int PlateraId { get; set; }
    public string PlaterIzena { get; set; } = string.Empty;
    public int EginDaitezkeenPlaterak { get; set; }
}

public class PlaterEskandalloaDTO
{
    public int PlateraId { get; set; }
    public string PlaterIzena { get; set; } = string.Empty;
    public decimal Prezioa { get; set; }
    public List<PlaterOsagaiaDto> Osagaiak { get; set; } = new();
}
