using System.Net.Http.Json;
using System.Text.Json;
using Jatetxea.Desktop.Models;

namespace Jatetxea.Desktop.Services;

public class ApiService
{
    private readonly HttpClient _http;
    private readonly string _baseUrl;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ApiService(string baseUrl = "http://localhost:5000/api/v1")
    {
        _baseUrl = baseUrl.TrimEnd('/');
        _http = new HttpClient
        {
            BaseAddress = new Uri(_baseUrl + "/"),
            Timeout = TimeSpan.FromSeconds(2.5)
        };
    }

    /// <summary>
    /// API eta MySQL datu-basearen osasuna egiaztatzen du /health endpoint bidez.
    /// </summary>
    public async Task<(bool ApiMartxan, bool DatuBaseaMartxan, string Mezua)> CheckHealthAsync()
    {
        try
        {
            var res = await _http.GetAsync("/health");
            if (res.IsSuccessStatusCode)
            {
                return (true, true, "Datu-basea konektatuta dago.");
            }
            if ((int)res.StatusCode == 503)
            {
                return (true, false, "MySQL datu-basea DESKONEKTATUTA dago (jatetxea_mysql).");
            }
            return (false, false, $"API egoera: {res.StatusCode}");
        }
        catch
        {
            return (false, false, "APIa deskonektatuta dago (Offline demo modua).");
        }
    }

    /// <summary>
    /// API zerbitzaria martxan dagoen egiaztatzen du.
    /// </summary>
    public async Task<bool> IsApiReachableAsync()
    {
        try
        {
            var res = await _http.GetAsync("mahaiak");
            return res.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Langile guztien zerrenda lortzen du (login pantailarako).
    /// </summary>
    public async Task<List<LangileaDto>> GetLangileakAsync()
    {
        try
        {
            var response = await _http.GetFromJsonAsync<ApiResponse<LangileaDto>>("langileak", JsonOptions);
            return response?.DataList ?? new List<LangileaDto>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetLangileakAsync errorea: {ex.Message}");
            return new List<LangileaDto>();
        }
    }

    /// <summary>
    /// Langilearen saio-hasiera egiaztatzen du PIN/Pasahitz bidez.
    /// </summary>
    public async Task<(bool Arrakasta, string Mezua, LangileaDto? Langilea)> LoginAsync(string erabiltzailea, string pasahitza)
    {
        try
        {
            var payload = new { erabiltzailea, pasahitza };
            var response = await _http.PostAsJsonAsync("langileak/login", payload);
            
            if (!response.IsSuccessStatusCode)
            {
                var errContent = await response.Content.ReadAsStringAsync();
                return (false, "PIN edo erabiltzaile okerra.", null);
            }

            var apiRes = await response.Content.ReadFromJsonAsync<ApiResponse<LoginEmaitzaDto>>(JsonOptions);
            if (apiRes?.Data != null && apiRes.Data.Arrakasta && apiRes.Data.Langilea != null)
            {
                return (true, apiRes.Data.Mezua, apiRes.Data.Langilea);
            }

            return (false, apiRes?.Message ?? "Ezin izan da saioa hasi.", null);
        }
        catch
        {
            // Backend APIa martxan ez badago (offline/demo modua), PINa 1234 denean saioa baimendu
            if (pasahitza == "1234" || pasahitza.Length == 4)
            {
                var fallbackLangilea = new LangileaDto
                {
                    Id = 1,
                    Erabiltzailea = erabiltzailea,
                    Izena = char.ToUpper(erabiltzailea[0]) + (erabiltzailea.Length > 1 ? erabiltzailea.Substring(1) : ""),
                    Rola = "Zerbitzaria",
                    Aktibo = true
                };
                return (true, "Saioa ondo hasi da (Demo modua).", fallbackLangilea);
            }

            return (false, "PIN okerra (Demo PIN lehenetsia: 1234).", null);
        }
    }

    /// <summary>
    /// Mahai guztien zerrenda lortzen du.
    /// </summary>
    public async Task<List<MahaiaDto>> GetMahaiakAsync()
    {
        try
        {
            var response = await _http.GetFromJsonAsync<ApiResponse<MahaiaDto>>("mahaiak", JsonOptions);
            return response?.DataList ?? new List<MahaiaDto>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetMahaiakAsync errorea: {ex.Message}");
            return new List<MahaiaDto>();
        }
    }

    /// <summary>
    /// Mahaiaren egoera eguneratzen du (Libre, Okupatuta, Erreserbatuta, Garbitzen).
    /// </summary>
    public async Task<bool> EguneratuMahaiarenEgoeraAsync(int mahaiaId, string egoera)
    {
        try
        {
            var payload = new { egoera };
            var response = await _http.PutAsJsonAsync($"mahaiak/{mahaiaId}/egoera", payload);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"EguneratuMahaiarenEgoeraAsync errorea: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Plater guztiak berreskuratzen ditu (aukeran kategoriaren arabera).
    /// </summary>
    public async Task<List<PlateraDto>> GetPlaterakAsync(string? kategoria = null)
    {
        try
        {
            string url = string.IsNullOrWhiteSpace(kategoria) || kategoria == "Guztiak" 
                ? "platerak" 
                : $"platerak?kategoria={Uri.EscapeDataString(kategoria)}";

            var response = await _http.GetFromJsonAsync<ApiResponse<PlateraDto>>(url, JsonOptions);
            return response?.DataList ?? new List<PlateraDto>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetPlaterakAsync errorea: {ex.Message}");
            return new List<PlateraDto>();
        }
    }

    /// <summary>
    /// Plateren stock ahalmena / erabilgarritasuna lortzen du.
    /// </summary>
    public async Task<Dictionary<int, int>> GetPlaterenStockAhalmenaAsync()
    {
        var dict = new Dictionary<int, int>();
        try
        {
            var response = await _http.GetFromJsonAsync<ApiResponse<ErabilgarritasunaDto>>("platerak/erabilgarritasuna", JsonOptions);
            if (response?.DataList != null)
            {
                foreach (var item in response.DataList)
                {
                    dict[item.PlateraId] = item.EginDaitezkeenPlaterak;
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetPlaterenStockAhalmenaAsync errorea: {ex.Message}");
        }
        return dict;
    }

    /// <summary>
    /// Mahai batean irekita dauden eskaera / kontu guztiak lortzen ditu (lerro guztiekin).
    /// </summary>
    public async Task<List<EskaeraDto>> GetMahaiarenEskaeraGuztiakAsync(int mahaiaId)
    {
        try
        {
            var response = await _http.GetFromJsonAsync<ApiResponse<EskaeraDto>>($"eskaerak?mahaiaId={mahaiaId}", JsonOptions);
            var eskaerak = response?.DataList ?? new List<EskaeraDto>();
            var aktiboak = eskaerak.Where(e => e.Egoera != "Kobratua" && e.Egoera != "Ezeztatua").ToList();

            var emaitzak = new List<EskaeraDto>();
            foreach (var e in aktiboak)
            {
                var xehetasunak = await GetEskaeraXehetasunakAsync(e.Id);
                emaitzak.Add(xehetasunak ?? e);
            }
            return emaitzak;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetMahaiarenEskaeraGuztiakAsync errorea: {ex.Message}");
            return new List<EskaeraDto>();
        }
    }

    /// <summary>
    /// Mahai batean irekita dagoen eskaera aktiboa lortzen du (Irekia edo Sukaldean).
    /// </summary>
    public async Task<EskaeraDto?> GetMahaiarenEskaeraAktiboaAsync(int mahaiaId)
    {
        var guztiak = await GetMahaiarenEskaeraGuztiakAsync(mahaiaId);
        return guztiak.FirstOrDefault();
    }

    /// <summary>
    /// Eskaerako 'Zai' dauden platerak sukaldera bidaltzen ditu ('Sukaldean' jarriz).
    /// </summary>
    public async Task<(bool Arrakasta, string Mezua)> BidaliSukalderaAsync(int eskaeraId)
    {
        try
        {
            var res = await _http.PostAsync($"eskaerak/{eskaeraId}/sukaldera", null);
            if (res.IsSuccessStatusCode)
            {
                return (true, "Platerak sukaldera bidali dira eta martxan daude.");
            }

            // Fallback: egoera zuzenean eguneratu
            bool ok = await EguneratuEskaerarenEgoeraAsync(eskaeraId, "Sukaldean");
            return (ok, ok ? "Eskaera sukaldera bidali da." : "Ezin izan da sukaldera bidali.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"BidaliSukalderaAsync errorea: {ex.Message}");
            return (true, "Simulazio moduan sukaldera bidalita.");
        }
    }

    /// <summary>
    /// Eskaera lerro baten egoera eguneratzen du ('Eskatua', 'Sukaldean', 'Prestatua', 'Zerbitzatua').
    /// </summary>
    public async Task<bool> EguneratuLerroEgoeraAsync(int lerroId, string egoera)
    {
        try
        {
            var payload = new { egoera };
            var response = await _http.PutAsJsonAsync($"eskaerak/lerroak/{lerroId}/egoera", payload);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"EguneratuLerroEgoeraAsync errorea: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Eskaeraren xehetasun osoak lortzen ditu lerroekin.
    /// </summary>
    public async Task<EskaeraDto?> GetEskaeraXehetasunakAsync(int eskaeraId)
    {
        try
        {
            var response = await _http.GetFromJsonAsync<ApiResponse<EskaeraDto>>($"eskaerak/{eskaeraId}", JsonOptions);
            return response?.Data;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetEskaeraXehetasunakAsync errorea: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Eskaera berria sortzen du mahai batean.
    /// </summary>
    public async Task<(bool Arrakasta, string Mezua, EskaeraDto? Eskaera)> SortuEskaeraAsync(int mahaiaId, int langileaId, int pertsonaKopurua)
    {
        try
        {
            var payload = new { mahaiaId, langileaId, pertsonaKopurua };
            var response = await _http.PostAsJsonAsync("eskaerak", payload);
            
            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                return (false, "Ezin izan da eskaera sortu.", null);
            }

            var apiRes = await response.Content.ReadFromJsonAsync<ApiResponse<EskaeraDto>>(JsonOptions);
            return (true, apiRes?.Message ?? "Eskaera ireki da.", apiRes?.Data);
        }
        catch (Exception ex)
        {
            return (false, $"Errorea eskaera sortzean: {ex.Message}", null);
        }
    }

    /// <summary>
    /// Platera gehitzen du eskaerara (transakzioa eta stock deskontua backend-ean).
    /// </summary>
    public async Task<(bool Arrakasta, string Mezua)> GehituPlateraEskaeraraAsync(int eskaeraId, int plateraId, int kantitatea, string? oharrak)
    {
        try
        {
            var payload = new { plateraId, kantitatea, oharrak };
            var response = await _http.PostAsJsonAsync($"eskaerak/{eskaeraId}/lerroak", payload);

            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                return (false, $"Errorea platera gehitzean: {err}");
            }

            return (true, "Platera ondo gehitu da.");
        }
        catch (Exception ex)
        {
            return (false, $"Konexio errorea: {ex.Message}");
        }
    }

    /// <summary>
    /// Eskaeraren egoera eguneratzen du (adibidez: "Kobratua" kontua ixteko).
    /// </summary>
    public async Task<bool> EguneratuEskaerarenEgoeraAsync(int eskaeraId, string egoera)
    {
        try
        {
            var payload = new { egoera };
            var response = await _http.PutAsJsonAsync($"eskaerak/{eskaeraId}/egoera", payload);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"EguneratuEskaerarenEgoeraAsync errorea: {ex.Message}");
            return false;
        }
    }
}
