using System.ComponentModel.DataAnnotations;

namespace Jatetxea.Api.DTO;

public class LangileaDTO
{
    public int Id { get; set; }
    public string Erabiltzailea { get; set; } = string.Empty;
    public string Izena { get; set; } = string.Empty;
    public string Abizena { get; set; } = string.Empty;
    public string NanDni { get; set; } = string.Empty;
    public string Emaila { get; set; } = string.Empty;
    public string Rola { get; set; } = string.Empty;
    public bool Aktibo { get; set; }
    public bool TxataAktibo { get; set; } = true;
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

    [StringLength(100)]
    public string Abizena { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string NanDni { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(120)]
    public string Emaila { get; set; } = string.Empty;

    [Required]
    public string Rola { get; set; } = "Zerbitzaria"; // Zerbitzaria, Sukaldaria, Kudeatzailea, Admin

    public bool TxataAktibo { get; set; } = true;
}

public class LangileaEguneratuDto
{
    [Required]
    [StringLength(50)]
    public string Erabiltzailea { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string Izena { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Abizena { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string NanDni { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(120)]
    public string Emaila { get; set; } = string.Empty;

    [Required]
    public string Rola { get; set; } = string.Empty;

    [StringLength(255, MinimumLength = 4)]
    public string? Pasahitza { get; set; }

    public bool? TxataAktibo { get; set; }
    public bool? Aktibo { get; set; }
}

public class LangileTxataDto
{
    public bool TxataAktibo { get; set; }
}

public class LangileAktiboDto
{
    public bool Aktibo { get; set; }
}

public class TxatMezuaDto
{
    public int Id { get; set; }
    public int LangileaId { get; set; }
    public string LangileIzena { get; set; } = string.Empty;
    public string Erabiltzailea { get; set; } = string.Empty;
    public string Rola { get; set; } = string.Empty;
    public string Gela { get; set; } = "Orokorra";
    public string Testua { get; set; } = string.Empty;
    public string SortzeData { get; set; } = string.Empty;
}

public class TxatMezuaSortuDto
{
    [Required]
    public int LangileaId { get; set; }

    [StringLength(40)]
    public string Gela { get; set; } = "Orokorra";

    [Required]
    [StringLength(1000, MinimumLength = 1)]
    public string Testua { get; set; } = string.Empty;
}

public class LangileLoginDto
{
    [Required]
    public string Erabiltzailea { get; set; } = string.Empty; // edo PIN kodea

    public string Pasahitza { get; set; } = string.Empty;

    public string? Pin { get; set; }
}

public class LoginEmaitzaDto
{
    public bool Arrakasta { get; set; }
    public string Mezua { get; set; } = string.Empty;
    public LangileaDTO? Langilea { get; set; }
    public string? Token { get; set; }
    public string? TokenMota { get; set; } = "Bearer";
    public string? IraungitzeData { get; set; }
}
