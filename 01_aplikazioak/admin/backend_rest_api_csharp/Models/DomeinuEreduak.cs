namespace Jatetxea.Api.Models;

/// <summary>
/// jatetxea_db datu-baseko domeinu-entitateak (POCO).
/// NHibernate-k proxy-ak eta lazy loading-a sortu ahal izateko propietate guztiak 'public virtual' dira.
/// </summary>

public class Hornitzailea
{
    public virtual int Id { get; set; }
    public virtual string Izena { get; set; } = string.Empty;
    public virtual string Cif { get; set; } = string.Empty;
    public virtual string ProduktuMota { get; set; } = string.Empty;
    public virtual decimal BezEhunekoa { get; set; } = 10.00m;
    public virtual string? Telefonoa { get; set; }
    public virtual string? Emaila { get; set; }
    public virtual string? Helbidea { get; set; }
    public virtual bool Aktibo { get; set; } = true;
    public virtual bool Ezabatua { get; set; }
    public virtual DateTime? EzabatzeData { get; set; }
    public virtual DateTime SortzeData { get; set; } = DateTime.Now;

    public virtual IList<Lehengaia> Lehengaiak { get; set; } = new List<Lehengaia>();
    public virtual IList<Faktura> Fakturak { get; set; } = new List<Faktura>();
}

public class Lehengaia
{
    public virtual int Id { get; set; }
    public virtual string Izena { get; set; } = string.Empty;
    public virtual string Unitatea { get; set; } = "kg";
    public virtual decimal UnekoStocka { get; set; }
    public virtual decimal GutxienekoStocka { get; set; }
    public virtual decimal KostuUnitarioa { get; set; }
    public virtual Hornitzailea? HornitzaileaEntitatea { get; set; }
    public virtual string? Hornitzailea { get; set; }
    public virtual bool Ezabatua { get; set; }
    public virtual DateTime? EzabatzeData { get; set; }
    public virtual DateTime SortzeData { get; set; } = DateTime.Now;

    public virtual IList<PlaterOsagaia> PlaterOsagaiak { get; set; } = new List<PlaterOsagaia>();
    public virtual IList<StockMugimendua> StockMugimenduak { get; set; } = new List<StockMugimendua>();
}

public class Platera
{
    public virtual int Id { get; set; }
    public virtual string Izena { get; set; } = string.Empty;
    public virtual string? Deskribapena { get; set; }
    public virtual string Kategoria { get; set; } = "Bigarrena";
    public virtual decimal Prezioa { get; set; }
    public virtual string? Alergenoak { get; set; }
    public virtual string? IrudiErreferentzia { get; set; }
    public virtual bool Aktibo { get; set; } = true;
    public virtual bool Ezabatua { get; set; }
    public virtual DateTime? EzabatzeData { get; set; }
    public virtual DateTime SortzeData { get; set; } = DateTime.Now;

    public virtual IList<PlaterOsagaia> Osagaiak { get; set; } = new List<PlaterOsagaia>();
    public virtual IList<EskaeraLerroa> EskaeraLerroak { get; set; } = new List<EskaeraLerroa>();
}

public class PlaterOsagaia
{
    public virtual int PlateraId { get; set; }
    public virtual int LehengaiaId { get; set; }
    public virtual decimal Kantitatea { get; set; }

    public virtual Platera? Platera { get; set; }
    public virtual Lehengaia? Lehengaia { get; set; }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        if (obj is not PlaterOsagaia other) return false;
        return PlateraId == other.PlateraId && LehengaiaId == other.LehengaiaId;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(PlateraId, LehengaiaId);
    }
}

public class Mahaia
{
    public virtual int Id { get; set; }
    public virtual int Zenbakia { get; set; }
    public virtual int Edukiera { get; set; } = 4;
    public virtual string Kokapena { get; set; } = "Jantoki Nagusia";
    public virtual string Egoera { get; set; } = "Libre";
    public virtual bool Ezabatua { get; set; }
    public virtual DateTime? EzabatzeData { get; set; }
    public virtual DateTime SortzeData { get; set; } = DateTime.Now;

    public virtual IList<Erreserba> Erreserbak { get; set; } = new List<Erreserba>();
    public virtual IList<Eskaera> Eskaerak { get; set; } = new List<Eskaera>();
}

public class Bezeroa
{
    public virtual int Id { get; set; }
    public virtual string Izena { get; set; } = string.Empty;
    public virtual string Abizena { get; set; } = string.Empty;
    public virtual string Telefonoa { get; set; } = string.Empty;
    public virtual string Emaila { get; set; } = string.Empty;
    public virtual bool Ezabatua { get; set; }
    public virtual DateTime? EzabatzeData { get; set; }
    public virtual DateTime SortzeData { get; set; } = DateTime.Now;

    public virtual IList<Erreserba> Erreserbak { get; set; } = new List<Erreserba>();
    public virtual IList<Iritzia> Iritziak { get; set; } = new List<Iritzia>();
}

public class Langilea
{
    public virtual int Id { get; set; }
    public virtual string Erabiltzailea { get; set; } = string.Empty;
    public virtual string PasahitzaHash { get; set; } = string.Empty;
    public virtual string Izena { get; set; } = string.Empty;
    public virtual string Abizena { get; set; } = string.Empty;
    public virtual string NanDni { get; set; } = string.Empty;
    public virtual string Emaila { get; set; } = string.Empty;
    public virtual string? Telefonoa { get; set; }
    public virtual string Rola { get; set; } = "Zerbitzaria";
    public virtual string Saila { get; set; } = "Jantokia";
    public virtual string Kargua { get; set; } = "Langilea";
    public virtual int? OdooEmployeeId { get; set; }
    public virtual bool Aktibo { get; set; } = true;
    public virtual bool TxataAktibo { get; set; } = true;
    public virtual bool Ezabatua { get; set; }
    public virtual DateTime? EzabatzeData { get; set; }
    public virtual DateTime SortzeData { get; set; } = DateTime.Now;

    public virtual IList<Eskaera> Eskaerak { get; set; } = new List<Eskaera>();
}

public class TxatMezua
{
    public virtual int Id { get; set; }
    public virtual Langilea Langilea { get; set; } = null!;
    public virtual string Gela { get; set; } = "Orokorra";
    public virtual string Testua { get; set; } = string.Empty;
    public virtual DateTime SortzeData { get; set; } = DateTime.Now;
}

public class Erreserba
{
    public virtual int Id { get; set; }
    public virtual Bezeroa Bezeroa { get; set; } = null!;
    public virtual Mahaia Mahaia { get; set; } = null!;
    public virtual DateTime Data { get; set; }
    public virtual string Txanda { get; set; } = "Bazkaria_1";
    public virtual int PertsonaKopurua { get; set; }
    public virtual string Egoera { get; set; } = "Baieztatua";
    public virtual string? Oharrak { get; set; }
    public virtual bool Ezabatua { get; set; }
    public virtual DateTime? EzabatzeData { get; set; }
    public virtual DateTime SortzeData { get; set; } = DateTime.Now;
}

public class Eskaera
{
    public virtual int Id { get; set; }
    public virtual Mahaia Mahaia { get; set; } = null!;
    public virtual Langilea Langilea { get; set; } = null!;
    public virtual string Egoera { get; set; } = "Irekia";
    public virtual decimal Guztira { get; set; }
    public virtual bool Ezabatua { get; set; }
    public virtual DateTime? EzabatzeData { get; set; }
    public virtual DateTime SortzeData { get; set; } = DateTime.Now;

    public virtual IList<EskaeraLerroa> Lerroak { get; set; } = new List<EskaeraLerroa>();
    public virtual IList<Ordainketa> Ordainketak { get; set; } = new List<Ordainketa>();
}

public class EskaeraLerroa
{
    public virtual int Id { get; set; }
    public virtual Eskaera Eskaera { get; set; } = null!;
    public virtual Platera Platera { get; set; } = null!;
    public virtual int Kantitatea { get; set; } = 1;
    public virtual decimal UnitatePrezioa { get; set; }
    public virtual string Egoera { get; set; } = "Eskatua";
    public virtual string? Oharrak { get; set; }
}

public class StockMugimendua
{
    public virtual int Id { get; set; }
    public virtual Lehengaia Lehengaia { get; set; } = null!;
    public virtual string Mota { get; set; } = "Kontsumoa_Platera";
    public virtual decimal Kantitatea { get; set; }
    public virtual Eskaera? Eskaera { get; set; }
    public virtual string? AlbaranZenbakia { get; set; }
    public virtual Faktura? Faktura { get; set; }
    public virtual string? Oharrak { get; set; }
    public virtual DateTime Data { get; set; } = DateTime.Now;
}

public class Iritzia
{
    public virtual int Id { get; set; }
    public virtual Bezeroa Bezeroa { get; set; } = null!;
    public virtual int Balorazioa { get; set; }
    public virtual string? Testua { get; set; }
    public virtual bool Aktibo { get; set; } = true;
    public virtual bool Ezabatua { get; set; }
    public virtual DateTime? EzabatzeData { get; set; }
    public virtual DateTime SortzeData { get; set; } = DateTime.Now;
}

public class Ordainketa
{
    public virtual int Id { get; set; }
    public virtual Eskaera Eskaera { get; set; } = null!;
    public virtual string TiketZenbakia { get; set; } = string.Empty;
    public virtual string Mota { get; set; } = "Txartela";
    public virtual decimal Zenbatekoa { get; set; }
    public virtual decimal? Jasotakoa { get; set; }
    public virtual decimal? Itzulitakoa { get; set; }
    public virtual string? TxartelMaskaratua { get; set; }
    public virtual string? TxartelMarka { get; set; }
    public virtual string OperazioKodea { get; set; } = string.Empty;
    public virtual string? TokenZifratua { get; set; }
    public virtual string Egoera { get; set; } = "Arrakastatsua";
    public virtual int? LangileaId { get; set; }
    public virtual string? TiketTestua { get; set; }
    public virtual DateTime Data { get; set; } = DateTime.Now;
}

public class Faktura
{
    public virtual int Id { get; set; }
    public virtual Eskaera? Eskaera { get; set; }
    public virtual Hornitzailea? HornitzaileaEntitatea { get; set; }
    public virtual string FakturaZenbakia { get; set; } = string.Empty;
    public virtual string Mota { get; set; } = "Salmenta";
    public virtual string Azpimota { get; set; } = "Sinplifikatua";
    public virtual string Kontzeptua { get; set; } = string.Empty;
    public virtual string? Hornitzailea { get; set; }
    public virtual string? HornitzaileCif { get; set; }
    public virtual string? BezeroNif { get; set; }
    public virtual string? BezeroIzena { get; set; }
    public virtual string? BezeroHelbidea { get; set; }
    public virtual decimal OinarriZergagarria { get; set; }
    public virtual decimal BezEhunekoa { get; set; } = 10.00m;
    public virtual decimal BezZenbatekoa { get; set; }
    public virtual decimal Guztira { get; set; }
    public virtual string OrdainketaMetodoa { get; set; } = "Txartela";
    public virtual string Egoera { get; set; } = "Ordaindua";
    public virtual DateTime Data { get; set; } = DateTime.Now;
    public virtual DateTime? EpemugaData { get; set; }
    public virtual string? Oharrak { get; set; }
    public virtual bool Ezabatua { get; set; }
    public virtual DateTime? EzabatzeData { get; set; }
    public virtual DateTime SortzeData { get; set; } = DateTime.Now;
}

// =========================================================================
// IRAKURKETA SOILIK DIREN DATU-BASE IKUSPEGIAK (MySQL VIEW-ak)
// =========================================================================

public class ErosiBeharrekoLehengaia
{
    public virtual int LehengaiaId { get; set; }
    public virtual string Izena { get; set; } = string.Empty;
    public virtual string Unitatea { get; set; } = string.Empty;
    public virtual decimal UnekoStocka { get; set; }
    public virtual decimal GutxienekoStocka { get; set; }
    public virtual decimal ProposatutakoErosketa { get; set; }
    public virtual string? Hornitzailea { get; set; }
}

public class PlaterStockAhalmena
{
    public virtual int PlateraId { get; set; }
    public virtual string PlaterIzena { get; set; } = string.Empty;
    public virtual int EginDaitezkeenPlaterak { get; set; }
}
