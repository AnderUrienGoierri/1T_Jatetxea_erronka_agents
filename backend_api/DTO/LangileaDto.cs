using System.ComponentModel.DataAnnotations;

namespace Jatetxea.Api.DTO;

public class LangileaDTO
{
    public int Id { get; set; }
    public string Erabiltzailea { get; set; } = string.Empty;
    public string Izena { get; set; } = string.Empty;
    public string Rola { get; set; } = string.Empty;
    public bool Aktibo { get; set; }
    public bool Ezabatua { get; set; }
}

public class LangileaSortuDto
{
    [Required]
    [StringLength(50)]
    public string Erabiltzailea { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string Pasahitza { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string Izena { get; set; } = string.Empty;

    [Required]
    public string Rola { get; set; } = "Zerbitzaria"; // Zerbitzaria, Sukaldaria, Kudeatzailea, Admin
}

public class LangileLoginDto
{
    [Required]
    public string Erabiltzailea { get; set; } = string.Empty; // edo PIN kodea

    [Required]
    public string Pasahitza { get; set; } = string.Empty;
}

public class LoginEmaitzaDto
{
    public bool Arrakasta { get; set; }
    public string Mezua { get; set; } = string.Empty;
    public LangileaDTO? Langilea { get; set; }
}
