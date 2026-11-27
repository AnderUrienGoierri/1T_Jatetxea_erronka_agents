using Jatetxea.Desktop.Models;

namespace Jatetxea.Desktop.Services;

/// <summary>
/// TPV bezeroaren REST API kontratu-interfazea (OOP abstrakzioa eta mendekotasunen inbertsioa).
/// </summary>
public interface IApiService
{
    void EzarriUnekoLangilea(string? izena, string? erabiltzailea, string? rola);
    Task<(bool ApiMartxan, bool DatuBaseaMartxan, string Mezua)> CheckHealthAsync();
    Task<bool> IsApiReachableAsync();
    Task<List<LangileaDto>> GetLangileakAsync();
    Task<(bool Arrakasta, string Mezua, LangileaDto? Langilea)> LoginAsync(string erabiltzailea, string pasahitza);
    Task<List<MahaiaDto>> GetMahaiakAsync();
    Task<bool> EguneratuMahaiarenEgoeraAsync(int mahaiaId, string egoera);
    Task<(bool Arrakasta, string Mezua)> AldatuMahaiarenEgoeraAsync(int mahaiaId, string egoera);
    Task<(bool Arrakasta, string Mezua)> EzabatuEskaeraLerroaAsync(int lerroId);
    Task<(bool Arrakasta, string Mezua)> EzabatuEskaeraAsync(int eskaeraId);
    Task<List<PlateraDto>> GetPlaterakAsync(string? kategoria = null);
    Task<Dictionary<int, int>> GetPlaterenStockAhalmenaAsync();
    Task<List<EskaeraDto>> GetMahaiarenEskaeraGuztiakAsync(int mahaiaId);
    Task<EskaeraDto?> GetMahaiarenEskaeraAktiboaAsync(int mahaiaId);
    Task<(bool Arrakasta, string Mezua)> BidaliSukalderaAsync(int eskaeraId);
    Task<bool> EguneratuLerroEgoeraAsync(int lerroId, string egoera);
    Task<EskaeraDto?> GetEskaeraXehetasunakAsync(int eskaeraId);
    Task<(bool Arrakasta, string Mezua, EskaeraDto? Eskaera)> SortuEskaeraAsync(int mahaiaId, int langileaId, int pertsonaKopurua);
    Task<(bool Arrakasta, string Mezua)> GehituPlateraEskaeraraAsync(int eskaeraId, int plateraId, int kantitatea, string? oharrak);
    Task<bool> EguneratuEskaerarenEgoeraAsync(int eskaeraId, string egoera);
    Task<(bool Arrakasta, string Mezua, OrdainketaErantzunaDto? Ordainketa)> ProzesatuOrdainketaAsync(OrdainketaEskaeraDto dto);
    Task<List<ErreserbaDto>> GetErreserbakAsync(string? data = null, string? txanda = null);
    Task<(bool Arrakasta, string Mezua, ErreserbaDto? Erreserba)> SortuErreserbaAsync(ErreserbaSortuEskaeraDto dto);
    Task<(bool Arrakasta, string Mezua)> EzeztatuErreserbaAsync(int id);
    Task<(bool Arrakasta, string Mezua, TxatEgoeraDto? Egoera)> GetTxatEgoeraAsync(int langileaId);
    Task<(bool Arrakasta, string Mezua, List<TxatMezuaDto> Mezuak)> GetTxatMezuakAsync(string gela, int langileaId);
    Task<(bool Arrakasta, string Mezua)> BidaliTxatMezuaAsync(int langileaId, string gela, string testua);
}
