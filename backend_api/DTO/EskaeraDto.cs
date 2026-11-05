using System.ComponentModel.DataAnnotations;

namespace Jatetxea.Api.DTO;

public class EskaeraDTO
{
    public int Id { get; set; }
    public int MahaiaId { get; set; }
    public int MahaiZenbakia { get; set; }
    public int LangileaId { get; set; }
    public string LangileIzena { get; set; } = string.Empty;
    public string Egoera { get; set; } = string.Empty;
    public decimal Guztira { get; set; }
    public bool Ezabatua { get; set; }
    public string SortzeData { get; set; } = string.Empty;
    public List<EskaeraLerroaDTO> Lerroak { get; set; } = new();
}

public class EskaeraLerroaDTO
{
    public int Id { get; set; }
    public int EskaeraId { get; set; }
    public int PlateraId { get; set; }
    public string PlaterIzena { get; set; } = string.Empty;
    public string Kategoria { get; set; } = string.Empty;
    public int Kantitatea { get; set; }
    public decimal UnitatePrezioa { get; set; }
    public decimal Azpitotala => Kantitatea * UnitatePrezioa;
    public string Egoera { get; set; } = string.Empty;
    public string? Oharrak { get; set; }
}

public class EskaeraSortuDto
{
    [Required]
    public int MahaiaId { get; set; }

    [Required]
    public int LangileaId { get; set; }

    public int PertsonaKopurua { get; set; } = 2;
}

public class EskaeraLerroaGehituDto
{
    [Required]
    public int PlateraId { get; set; }

    [Required]
    [Range(1, 100)]
    public int Kantitatea { get; set; } = 1;

    public string? Oharrak { get; set; }
}

public class EskaeraEgoeraEguneratuDto
{
    [Required]
    public string Egoera { get; set; } = string.Empty; // 'Irekia', 'Sukaldean', 'Zerbitzatua', 'Kobratua', 'Ezeztatua'
}

public class EskaeraLerroEgoeraEguneratuDto
{
    [Required]
    public string Egoera { get; set; } = string.Empty; // 'Eskatua', 'Sukaldean', 'Prestatua', 'Zerbitzatua'
}
