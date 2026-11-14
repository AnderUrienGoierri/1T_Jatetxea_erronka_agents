using System.Text.Json.Serialization;

namespace Jatetxea.Desktop.Models;

public class ApiResponse<T>
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public T? Data { get; set; }

    [JsonPropertyName("dataList")]
    public List<T>? DataList { get; set; }
}

public class LangileaDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("erabiltzailea")]
    public string Erabiltzailea { get; set; } = string.Empty;

    [JsonPropertyName("izena")]
    public string Izena { get; set; } = string.Empty;

    [JsonPropertyName("rola")]
    public string Rola { get; set; } = string.Empty;

    [JsonPropertyName("aktibo")]
    public bool Aktibo { get; set; }
}

public class LoginEmaitzaDto
{
    [JsonPropertyName("arrakasta")]
    public bool Arrakasta { get; set; }

    [JsonPropertyName("mezua")]
    public string Mezua { get; set; } = string.Empty;

    [JsonPropertyName("langilea")]
    public LangileaDto? Langilea { get; set; }
}

public class MahaiaDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("zenbakia")]
    public int Zenbakia { get; set; }

    [JsonPropertyName("edukiera")]
    public int Edukiera { get; set; }

    [JsonPropertyName("kokapena")]
    public string Kokapena { get; set; } = string.Empty;

    [JsonPropertyName("egoera")]
    public string Egoera { get; set; } = "Libre"; // Libre, Okupatuta, Erreserbatuta, Garbitzen

    // Bezeroan kalkulatutako eremu osagarriak
    public int? Mahaikideak { get; set; }
    public string? Iraupena { get; set; }
    public int? AktiboEskaeraId { get; set; }
}

public class PlateraDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("izena")]
    public string Izena { get; set; } = string.Empty;

    [JsonPropertyName("deskribapena")]
    public string? Deskribapena { get; set; }

    [JsonPropertyName("kategoria")]
    public string Kategoria { get; set; } = string.Empty;

    [JsonPropertyName("prezioa")]
    public decimal Prezioa { get; set; }

    [JsonPropertyName("alergenoak")]
    public string? Alergenoak { get; set; }

    [JsonPropertyName("prestatzeDenboraMin")]
    public int PrestatzeDenboraMin { get; set; } = 15;

    [JsonPropertyName("aktibo")]
    public bool Aktibo { get; set; }

    public int StockAhalmena { get; set; } = 999;
}

public class ErabilgarritasunaDto
{
    [JsonPropertyName("plateraId")]
    public int PlateraId { get; set; }

    [JsonPropertyName("plateraIzena")]
    public string PlateraIzena { get; set; } = string.Empty;

    [JsonPropertyName("eginDaitezkeenPlaterak")]
    public int EginDaitezkeenPlaterak { get; set; }
}

public class EskaeraDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("mahaiaId")]
    public int MahaiaId { get; set; }

    [JsonPropertyName("mahaiZenbakia")]
    public int MahaiZenbakia { get; set; }

    [JsonPropertyName("langileaId")]
    public int LangileaId { get; set; }

    [JsonPropertyName("langileIzena")]
    public string LangileIzena { get; set; } = string.Empty;

    [JsonPropertyName("pertsonaKopurua")]
    public int PertsonaKopurua { get; set; } = 2;

    [JsonPropertyName("egoera")]
    public string Egoera { get; set; } = "Irekia"; // Irekia, Sukaldean, Zerbitzatua, Kobratua

    [JsonPropertyName("guztira")]
    public decimal Guztira { get; set; }

    [JsonPropertyName("sortzeData")]
    public string SortzeData { get; set; } = string.Empty;

    [JsonPropertyName("lerroak")]
    public List<EskaeraLerroaDto> Lerroak { get; set; } = new();
}

public class EskaeraLerroaDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("plateraId")]
    public int PlateraId { get; set; }

    [JsonPropertyName("platerIzena")]
    public string PlaterIzena { get; set; } = string.Empty;

    [JsonPropertyName("kategoria")]
    public string Kategoria { get; set; } = string.Empty;

    [JsonPropertyName("kantitatea")]
    public int Kantitatea { get; set; }

    [JsonPropertyName("kopurua")]
    public int Kopurua
    {
        get => Kantitatea;
        set => Kantitatea = value;
    }

    [JsonPropertyName("unitatePrezioa")]
    public decimal UnitatePrezioa { get; set; }

    [JsonPropertyName("azpitotala")]
    public decimal Azpitotala => UnitatePrezioa * Kantitatea;

    [JsonPropertyName("egoera")]
    public string Egoera { get; set; } = "Irekia";

    [JsonPropertyName("oharrak")]
    public string? Oharrak { get; set; }
}

public class CartItem
{
    public PlateraDto Platera { get; set; } = null!;
    public int Kopurua { get; set; } = 1;
    public string Oharrak { get; set; } = string.Empty;
    public decimal Azpitotala => Platera.Prezioa * Kopurua;
}

public class OrdainketaEskaeraDto
{
    [JsonPropertyName("eskaeraId")]
    public int EskaeraId { get; set; }

    [JsonPropertyName("mota")]
    public string Mota { get; set; } = "Txartela";

    [JsonPropertyName("zenbatekoa")]
    public decimal Zenbatekoa { get; set; }

    [JsonPropertyName("jasotakoa")]
    public decimal? Jasotakoa { get; set; }

    [JsonPropertyName("txartelZenbakia")]
    public string? TxartelZenbakia { get; set; }

    [JsonPropertyName("titularra")]
    public string? Titularra { get; set; }

    [JsonPropertyName("iraungitzea")]
    public string? Iraungitzea { get; set; }

    [JsonPropertyName("cvv")]
    public string? Cvv { get; set; }

    [JsonPropertyName("bizumTelefonoa")]
    public string? BizumTelefonoa { get; set; }

    [JsonPropertyName("langileaId")]
    public int? LangileaId { get; set; }
}

public class OrdainketaErantzunaDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("eskaeraId")]
    public int EskaeraId { get; set; }

    [JsonPropertyName("tiketZenbakia")]
    public string TiketZenbakia { get; set; } = string.Empty;

    [JsonPropertyName("mota")]
    public string Mota { get; set; } = string.Empty;

    [JsonPropertyName("zenbatekoa")]
    public decimal Zenbatekoa { get; set; }

    [JsonPropertyName("jasotakoa")]
    public decimal? Jasotakoa { get; set; }

    [JsonPropertyName("itzulitakoa")]
    public decimal? Itzulitakoa { get; set; }

    [JsonPropertyName("txartelMaskaratua")]
    public string? TxartelMaskaratua { get; set; }

    [JsonPropertyName("txartelMarka")]
    public string? TxartelMarka { get; set; }

    [JsonPropertyName("operazioKodea")]
    public string OperazioKodea { get; set; } = string.Empty;

    [JsonPropertyName("tokenZifratua")]
    public string? TokenZifratua { get; set; }

    [JsonPropertyName("egoera")]
    public string Egoera { get; set; } = "Arrakastatsua";

    [JsonPropertyName("data")]
    public string Data { get; set; } = string.Empty;

    [JsonPropertyName("tiketTestua")]
    public string TiketTestua { get; set; } = string.Empty;

    [JsonPropertyName("azpitotala")]
    public decimal Azpitotala { get; set; }

    [JsonPropertyName("bez")]
    public decimal Bez { get; set; }
}

public class ErreserbaDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("bezeroaId")]
    public int BezeroaId { get; set; }

    [JsonPropertyName("bezeroIzena")]
    public string BezeroIzena { get; set; } = string.Empty;

    [JsonPropertyName("bezeroAbizena")]
    public string BezeroAbizena { get; set; } = string.Empty;

    [JsonPropertyName("bezeroEmaila")]
    public string BezeroEmaila { get; set; } = string.Empty;

    [JsonPropertyName("bezeroTelefonoa")]
    public string BezeroTelefonoa { get; set; } = string.Empty;

    [JsonPropertyName("mahaiaId")]
    public int MahaiaId { get; set; }

    [JsonPropertyName("mahaiZenbakia")]
    public int MahaiZenbakia { get; set; }

    [JsonPropertyName("kokapena")]
    public string Kokapena { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public string Data { get; set; } = string.Empty;

    [JsonPropertyName("txanda")]
    public string Txanda { get; set; } = string.Empty;

    [JsonPropertyName("pertsonaKopurua")]
    public int PertsonaKopurua { get; set; }

    [JsonPropertyName("egoera")]
    public string Egoera { get; set; } = "Konfirmatuta";

    [JsonPropertyName("oharrak")]
    public string? Oharrak { get; set; }

    [JsonPropertyName("ezabatua")]
    public bool Ezabatua { get; set; }
}

public class ErreserbaSortuEskaeraDto
{
    [JsonPropertyName("mahaiaId")]
    public int? MahaiaId { get; set; }

    [JsonPropertyName("mahaiZenbakia")]
    public int? MahaiZenbakia { get; set; }

    [JsonPropertyName("data")]
    public string Data { get; set; } = string.Empty;

    [JsonPropertyName("txanda")]
    public string Txanda { get; set; } = "Bazkaria_1";

    [JsonPropertyName("pertsonaKopurua")]
    public int PertsonaKopurua { get; set; } = 2;

    [JsonPropertyName("oharrak")]
    public string? Oharrak { get; set; }

    [JsonPropertyName("bezeroIzena")]
    public string? BezeroIzena { get; set; }

    [JsonPropertyName("bezeroAbizena")]
    public string? BezeroAbizena { get; set; }

    [JsonPropertyName("bezeroEmaila")]
    public string? BezeroEmaila { get; set; }

    [JsonPropertyName("bezeroTelefonoa")]
    public string? BezeroTelefonoa { get; set; }
}
