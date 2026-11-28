using System.ComponentModel.DataAnnotations;

namespace Jatetxea.Api.DTO;

public class HornitzaileaDTO
{
    public int Id { get; set; }
    public string Izena { get; set; } = string.Empty;
    public string Cif { get; set; } = string.Empty;
    public string ProduktuMota { get; set; } = string.Empty;
    public decimal BezEhunekoa { get; set; } = 10.00m;
    public string? Telefonoa { get; set; }
    public string? Emaila { get; set; }
    public string? Helbidea { get; set; }
    public bool Aktibo { get; set; } = true;
    public int LehengaiKopurua { get; set; }
}

public class HornitzaileaSortuDto
{
    [Required]
    [StringLength(120)]
    public string Izena { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Cif { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string ProduktuMota { get; set; } = string.Empty;

    [Range(0, 100)]
    public decimal BezEhunekoa { get; set; } = 10.00m;

    [StringLength(20)]
    public string? Telefonoa { get; set; }

    [StringLength(120)]
    public string? Emaila { get; set; }

    [StringLength(200)]
    public string? Helbidea { get; set; }
}
