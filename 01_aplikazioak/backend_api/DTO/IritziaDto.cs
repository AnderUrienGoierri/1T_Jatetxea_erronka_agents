using System.ComponentModel.DataAnnotations;

namespace Jatetxea.Api.DTO;

public class IritziaDTO
{
    public int Id { get; set; }
    public int BezeroaId { get; set; }
    public string BezeroIzena { get; set; } = string.Empty;
    public string BezeroEmaila { get; set; } = string.Empty;
    public int Balorazioa { get; set; }
    public string? Testua { get; set; }
    public string SortzeData { get; set; } = string.Empty;
}

public class IritziaSortuDto
{
    public int? BezeroaId { get; set; }
    public string? BezeroIzena { get; set; }
    public string? BezeroAbizena { get; set; }
    public string? BezeroTelefonoa { get; set; }
    public string? BezeroEmaila { get; set; }

    [Required]
    [Range(0, 5)]
    public int Balorazioa { get; set; }

    public string? Testua { get; set; }
}

public class BalorazioEstatistikakDTO
{
    public int GuztiraIritziak { get; set; }
    public decimal BatazBestekoBalorazioa { get; set; }
    public int BostIzarrak { get; set; }
    public int LauIzarrak { get; set; }
    public int HiruIzarrak { get; set; }
    public int BiIzarrak { get; set; }
    public int IzarBat { get; set; }
    public int ZeroIzar { get; set; }
}
