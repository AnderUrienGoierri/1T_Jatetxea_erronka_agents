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

    public ApiService(string? baseUrl = null)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            baseUrl = LoadBaseUrlFromConfig();
        }
        _baseUrl = baseUrl.TrimEnd('/');
        _http = new HttpClient
        {
            BaseAddress = new Uri(_baseUrl + "/"),
            Timeout = TimeSpan.FromSeconds(2.5)
        };
    }

    private static string LoadBaseUrlFromConfig()
    {
        try
        {
            var env = Environment.GetEnvironmentVariable("API_BASE_URL");
            if (!string.IsNullOrWhiteSpace(env)) return env;

            if (File.Exists("appsettings.json"))
            {
                var json = File.ReadAllText("appsettings.json");
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("ApiBaseUrl", out var prop))
                {
                    var val = prop.GetString();
                    if (!string.IsNullOrWhiteSpace(val)) return val;
                }
            }
        }
        catch { }
        return "http://localhost:5000/api/v1";
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
            return (false, false, "API zerbitzaria deskonektatuta dago (http://localhost:5000).");
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
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LoginAsync konexio errorea: {ex.Message}");
            return (false, "Ezin izan da API zerbitzariarekin konektatu (http://localhost:5000). Ziurtatu martxan dagoela.", null);
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

            return (false, "Ezin izan da sukaldera bidali.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"BidaliSukalderaAsync errorea: {ex.Message}");
            return (false, "Ezin izan da sukaldera bidali: API zerbitzaria ez dago eskuragarri.");
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
                try
                {
                    var apiErr = await response.Content.ReadFromJsonAsync<ApiResponse<object>>(JsonOptions);
                    if (!string.IsNullOrWhiteSpace(apiErr?.Message))
                    {
                        return (false, apiErr.Message, null);
                    }
                }
                catch { }

                var err = await response.Content.ReadAsStringAsync();
                return (false, !string.IsNullOrWhiteSpace(err) ? err : "Ezin izan da eskaera sortu.", null);
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

    /// <summary>
    /// Eskaeraren ordainketa segurua prozesatzen du (Eskudirua, Txartela, Bizum) eta 80mm-ko tiketa sortzen du.
    /// </summary>
    public async Task<(bool Arrakasta, string Mezua, OrdainketaErantzunaDto? Ordainketa)> ProzesatuOrdainketaAsync(OrdainketaEskaeraDto dto)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("ordainketak", dto);
            if (!response.IsSuccessStatusCode)
            {
                try
                {
                    var apiErr = await response.Content.ReadFromJsonAsync<ApiResponse<object>>(JsonOptions);
                    if (!string.IsNullOrWhiteSpace(apiErr?.Message))
                    {
                        return (false, apiErr.Message, null);
                    }
                }
                catch { }

                var raw = await response.Content.ReadAsStringAsync();
                return (false, !string.IsNullOrWhiteSpace(raw) ? raw : "Ezin izan da ordainketa burutu.", null);
            }

            var apiRes = await response.Content.ReadFromJsonAsync<ApiResponse<OrdainketaErantzunaDto>>(JsonOptions);
            return (true, apiRes?.Message ?? "Ordainketa arrakastaz burutu da.", apiRes?.Data);
        }
        catch (Exception ex)
        {
            return (false, $"Konexio errorea ordaintzean: {ex.Message}", null);
        }
    }

    /// <summary>
    /// Erreserben zerrenda lortzen du (data eta txandaren arabera iragazgarria).
    /// </summary>
    public async Task<List<ErreserbaDto>> GetErreserbakAsync(string? data = null, string? txanda = null)
    {
        try
        {
            string url = "erreserbak?";
            if (!string.IsNullOrWhiteSpace(data)) url += $"data={Uri.EscapeDataString(data)}&";
            if (!string.IsNullOrWhiteSpace(txanda) && txanda != "Guztiak") url += $"txanda={Uri.EscapeDataString(txanda)}&";

            var apiRes = await _http.GetFromJsonAsync<ApiResponse<ErreserbaDto>>(url.TrimEnd('&', '?'), JsonOptions);
            return apiRes?.DataList ?? new List<ErreserbaDto>();
        }
        catch
        {
            return new List<ErreserbaDto>();
        }
    }

    /// <summary>
    /// Erreserba berria sortzen du backend APIan (telefonoz edo pertsonan jasota).
    /// </summary>
    public async Task<(bool Arrakasta, string Mezua, ErreserbaDto? Erreserba)> SortuErreserbaAsync(ErreserbaSortuEskaeraDto dto)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("erreserbak", dto);
            if (!response.IsSuccessStatusCode)
            {
                try
                {
                    var apiErr = await response.Content.ReadFromJsonAsync<ApiResponse<object>>(JsonOptions);
                    if (!string.IsNullOrWhiteSpace(apiErr?.Message))
                    {
                        return (false, apiErr.Message, null);
                    }
                }
                catch { }

                var raw = await response.Content.ReadAsStringAsync();
                return (false, !string.IsNullOrWhiteSpace(raw) ? raw : "Ezin izan da erreserba gorde.", null);
            }

            var apiRes = await response.Content.ReadFromJsonAsync<ApiResponse<ErreserbaDto>>(JsonOptions);
            return (true, apiRes?.Message ?? "Erreserba arrakastaz gorde da.", apiRes?.Data);
        }
        catch (Exception ex)
        {
            return (false, $"Konexio errorea erreserba sortzean: {ex.Message}", null);
        }
    }

    /// <summary>
    /// Erreserba bat ezeztatzen edo ezabatzen du.
    /// </summary>
    public async Task<(bool Arrakasta, string Mezua)> EzeztatuErreserbaAsync(int id)
    {
        try
        {
            var response = await _http.DeleteAsync($"erreserbak/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var raw = await response.Content.ReadAsStringAsync();
                return (false, !string.IsNullOrWhiteSpace(raw) ? raw : "Ezin izan da erreserba ezeztatu.");
            }
            return (true, "Erreserba ezeztatu da.");
        }
        catch (Exception ex)
        {
            return (false, $"Errorea erreserba ezeztatzean: {ex.Message}");
        }
    }
}
