namespace Jatetxea.Api.Models;

public class Lehengaia
{
    public int Id { get; set; }
    public string Izena { get; set; } = string.Empty;
    public string Unitatea { get; set; } = "kg";
    public decimal UnekoStocka { get; set; }
    public decimal GutxienekoStocka { get; set; }
    public decimal KostuUnitarioa { get; set; }
    public string? Hornitzailea { get; set; }
    public bool Ezabatua { get; set; }
    public DateTime? EzabatzeData { get; set; }
    public DateTime SortzeData { get; set; }
}

public class Platera
{
    public int Id { get; set; }
    public string Izena { get; set; } = string.Empty;
    public string? Deskribapena { get; set; }
    public string Kategoria { get; set; } = "Bigarrena";
    public decimal Prezioa { get; set; }
    public string? Alergenoak { get; set; }
    public bool Aktibo { get; set; } = true;
    public bool Ezabatua { get; set; }
    public DateTime? EzabatzeData { get; set; }
    public DateTime SortzeData { get; set; }
}

public class PlaterOsagaia
{
    public int PlateraId { get; set; }
    public int LehengaiaId { get; set; }
    public decimal Kantitatea { get; set; }
}

public class Mahaia
{
    public int Id { get; set; }
    public int Zenbakia { get; set; }
    public int Edukiera { get; set; }
    public string Kokapena { get; set; } = "Jantoki Nagusia";
    public string Egoera { get; set; } = "Libre";
    public bool Ezabatua { get; set; }
    public DateTime? EzabatzeData { get; set; }
    public DateTime SortzeData { get; set; }
}

public class Bezeroa
{
    public int Id { get; set; }
    public string Izena { get; set; } = string.Empty;
    public string Abizena { get; set; } = string.Empty;
    public string Telefonoa { get; set; } = string.Empty;
    public string Emaila { get; set; } = string.Empty;
    public bool Ezabatua { get; set; }
    public DateTime? EzabatzeData { get; set; }
    public DateTime SortzeData { get; set; }
}

public class Langilea
{
    public int Id { get; set; }
    public string Erabiltzailea { get; set; } = string.Empty;
    public string PasahitzaHash { get; set; } = string.Empty;
    public string Izena { get; set; } = string.Empty;
    public string Rola { get; set; } = "Zerbitzaria";
    public bool Aktibo { get; set; } = true;
    public bool Ezabatua { get; set; }
    public DateTime? EzabatzeData { get; set; }
    public DateTime SortzeData { get; set; }
}

public class Erreserba
{
    public int Id { get; set; }
    public int BezeroaId { get; set; }
    public int MahaiaId { get; set; }
    public DateTime Data { get; set; }
    public string Txanda { get; set; } = "Bazkaria_1";
    public int PertsonaKopurua { get; set; }
    public string Egoera { get; set; } = "Baieztatua";
    public string? Oharrak { get; set; }
    public bool Ezabatua { get; set; }
    public DateTime? EzabatzeData { get; set; }
    public DateTime SortzeData { get; set; }
}

public class Eskaera
{
    public int Id { get; set; }
    public int MahaiaId { get; set; }
    public int LangileaId { get; set; }
    public string Egoera { get; set; } = "Irekia";
    public decimal Guztira { get; set; }
    public bool Ezabatua { get; set; }
    public DateTime? EzabatzeData { get; set; }
    public DateTime SortzeData { get; set; }
}

public class EskaeraLerroa
{
    public int Id { get; set; }
    public int EskaeraId { get; set; }
    public int PlateraId { get; set; }
    public int Kantitatea { get; set; } = 1;
    public decimal UnitatePrezioa { get; set; }
    public string Egoera { get; set; } = "Eskatua";
    public string? Oharrak { get; set; }
}

public class StockMugimendua
{
    public int Id { get; set; }
    public int LehengaiaId { get; set; }
    public string Mota { get; set; } = "Kontsumoa_Platera";
    public decimal Kantitatea { get; set; }
    public int? EskaeraId { get; set; }
    public string? Oharrak { get; set; }
    public DateTime Data { get; set; }
}

public class Iritzia
{
    public int Id { get; set; }
    public int BezeroaId { get; set; }
    public int Balorazioa { get; set; }
    public string? Testua { get; set; }
    public bool Aktibo { get; set; } = true;
    public bool Ezabatua { get; set; }
    public DateTime? EzabatzeData { get; set; }
    public DateTime SortzeData { get; set; }
}
