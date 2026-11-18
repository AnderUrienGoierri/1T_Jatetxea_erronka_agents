using System.ComponentModel.DataAnnotations;

namespace Jatetxea.Api.DTO;

public class ErreserbaDTO
{
    public int Id { get; set; }
    public int BezeroaId { get; set; }
    public string BezeroIzena { get; set; } = string.Empty;
    public string BezeroAbizena { get; set; } = string.Empty;
    public string BezeroEmaila { get; set; } = string.Empty;
    public string BezeroTelefonoa { get; set; } = string.Empty;
    public int MahaiaId { get; set; }
    public int MahaiZenbakia { get; set; }
    public string Kokapena { get; set; } = string.Empty;
    public string Data { get; set; } = string.Empty;
    public string Txanda { get; set; } = string.Empty;
    public int PertsonaKopurua { get; set; }
    public string Egoera { get; set; } = string.Empty;
    public string? Oharrak { get; set; }
    public bool Ezabatua { get; set; }
}

public class ErreserbaSortuDto
{
    public int? MahaiaId { get; set; }
    public int? MahaiZenbakia { get; set; }

    [Required]
    public string Data { get; set; } = string.Empty; // YYYY-MM-DD

    [Required]
    public string Txanda { get; set; } = "Bazkaria_1"; // Bazkaria_1, Bazkaria_2, Afaria_1, Afaria_2

    [Required]
    [Range(1, 100)]
    public int PertsonaKopurua { get; set; } = 2;

    public string? Oharrak { get; set; }

    // Bezeroa zehazteko (Id bidez edo datu pertsonalen bidez):
    public int? BezeroaId { get; set; }
    public string? BezeroIzena { get; set; }
    public string? BezeroAbizena { get; set; }
    public string? BezeroEmaila { get; set; }
    public string? BezeroTelefonoa { get; set; }

    // Probetarako edo konkurrentzia simulatzeko itxaron denbora (milisegundoak)
    public int Itxaron { get; set; } = 0;
}
