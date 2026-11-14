using System.ComponentModel.DataAnnotations;

namespace Jatetxea.Api.DTO;

public class MahaiaDTO
{
    public int Id { get; set; }
    public int Zenbakia { get; set; }
    public int Edukiera { get; set; }
    public string Kokapena { get; set; } = string.Empty;
    public string Egoera { get; set; } = string.Empty;
    public bool Ezabatua { get; set; }
}

public class MahaiaSortuDto
{
    [Required]
    [Range(1, 1000)]
    public int Zenbakia { get; set; }

    [Required]
    [Range(1, 100)]
    public int Edukiera { get; set; } = 4;

    public string Kokapena { get; set; } = "Jantoki Nagusia";
}

public class MahaiaEgoeraEguneratuDto
{
    [Required]
    public string Egoera { get; set; } = string.Empty; // 'Libre', 'Okupatuta', 'Erreserbatuta', 'Garbitzen'
}
