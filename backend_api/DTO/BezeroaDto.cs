using System.ComponentModel.DataAnnotations;

namespace Jatetxea.Api.DTO;

public class BezeroaDTO
{
    public int Id { get; set; }
    public string Izena { get; set; } = string.Empty;
    public string Abizena { get; set; } = string.Empty;
    public string Telefonoa { get; set; } = string.Empty;
    public string Emaila { get; set; } = string.Empty;
    public bool Ezabatua { get; set; }
}

public class BezeroaSortuDto
{
    [Required]
    [StringLength(80)]
    public string Izena { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Abizena { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Telefonoa { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(120)]
    public string Emaila { get; set; } = string.Empty;
}
