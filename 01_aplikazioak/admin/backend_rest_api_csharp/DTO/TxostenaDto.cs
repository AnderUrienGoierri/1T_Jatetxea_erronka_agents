namespace Jatetxea.Api.DTO;

public class TxostenLaburpenaDto
{
    public string HasieraData { get; set; } = string.Empty;
    public string AmaieraData { get; set; } = string.Empty;
    public int EskaeraKopurua { get; set; }
    public decimal FakturazioGuztira { get; set; }
    public decimal BatezBestekoTiketa { get; set; }
    public decimal BiltegiBalioGuztira { get; set; }
    public int StockAlertaKopurua { get; set; }
    public List<TopPlateraDto> TopPlaterak { get; set; } = new();
    public List<OrdainketaMotaLaburpenaDto> OrdainketakMotaka { get; set; } = new();
    public List<LangileSalmentaDto> LangileSalmentak { get; set; } = new();
    public List<EgunekoFakturazioaDto> EgunekoFakturazioa { get; set; } = new();
}

public class TopPlateraDto
{
    public int PlateraId { get; set; }
    public string Izena { get; set; } = string.Empty;
    public string Kategoria { get; set; } = string.Empty;
    public int SaldutakoUnitateak { get; set; }
    public decimal DiruSarreraGuztira { get; set; }
}

public class OrdainketaMotaLaburpenaDto
{
    public string Mota { get; set; } = string.Empty;
    public int EragiketaKopurua { get; set; }
    public decimal ZenbatekoaGuztira { get; set; }
}

public class LangileSalmentaDto
{
    public int LangileaId { get; set; }
    public string Izena { get; set; } = string.Empty;
    public string Rola { get; set; } = string.Empty;
    public int EskaeraKopurua { get; set; }
    public decimal SalmentaGuztira { get; set; }
}

public class EgunekoFakturazioaDto
{
    public string Eguna { get; set; } = string.Empty;
    public int EskaeraKopurua { get; set; }
    public decimal Guztira { get; set; }
}

public class PlaterBilaketaItemDto
{
    public int Id { get; set; }
    public string Izena { get; set; } = string.Empty;
    public string? Deskribapena { get; set; }
    public string Kategoria { get; set; } = string.Empty;
    public decimal Prezioa { get; set; }
    public string? Alergenoak { get; set; }
    public bool Aktibo { get; set; }
    public int? EginDaitezkeenAnoak { get; set; }
}

public class PaginatutakoPlaterakDto
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public string SortBy { get; set; } = "izena";
    public string SortDir { get; set; } = "asc";
    public List<PlaterBilaketaItemDto> Items { get; set; } = new();
}

public class AuditoriaSeguruaDto
{
    public string Erabiltzailea { get; set; } = string.Empty;
    public string Izena { get; set; } = string.Empty;
    public string Rola { get; set; } = string.Empty;
    public string KontsultaOrdua { get; set; } = string.Empty;
    public int StockMugimenduakGuztira { get; set; }
    public int OrdainketakGuztira { get; set; }
    public List<object> AzkenStockMugimenduak { get; set; } = new();
}
