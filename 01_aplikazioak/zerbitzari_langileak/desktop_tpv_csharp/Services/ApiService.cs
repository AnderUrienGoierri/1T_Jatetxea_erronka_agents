using System.Net.Http.Json;
using System.Text.Json;
using Jatetxea.Desktop.Models;

namespace Jatetxea.Desktop.Services;

public class ApiService : IApiService
{
    private HttpClient _http;
    private string _baseUrl;

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
        _http = SortuHttpClient(_baseUrl);
    }

    private static HttpClient SortuHttpClient(string baseUrl)
    {
        var http = new HttpClient
        {
            BaseAddress = new Uri(baseUrl + "/"),
            Timeout = TimeSpan.FromSeconds(2.5)
        };
        http.DefaultRequestHeaders.Add("X-App-Id", "csharp_tpv");
        http.DefaultRequestHeaders.Add("X-App-Name", "C# TPV Areto & Kutxazaina");
        http.DefaultRequestHeaders.UserAgent.ParseAdd("Jatetxea.Desktop/1.0 (WindowsForms .NET10)");
        return http;
    }

    public void EzarriUnekoLangilea(string? izena, string? erabiltzailea, string? rola)
    {
        _http.DefaultRequestHeaders.Remove("X-Langilea");
        _http.DefaultRequestHeaders.Remove("X-Erabiltzailea");
        _http.DefaultRequestHeaders.Remove("X-Rola");
        if (!string.IsNullOrWhiteSpace(izena))
            _http.DefaultRequestHeaders.Add("X-Langilea", izena.Trim());
        if (!string.IsNullOrWhiteSpace(erabiltzailea))
            _http.DefaultRequestHeaders.Add("X-Erabiltzailea", erabiltzailea.Trim());
        if (!string.IsNullOrWhiteSpace(rola))
            _http.DefaultRequestHeaders.Add("X-Rola", rola.Trim());
    }

    private void EguneratuBaseUrl(string baseUrl)
    {
        if (string.Equals(_baseUrl, baseUrl, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var aurrekoHttp = _http;
        _http = SortuHttpClient(baseUrl);
        _baseUrl = baseUrl;
        aurrekoHttp.Dispose();
    }

    private static string LoadBaseUrlFromConfig()
    {
        try
        {
            var env = Environment.GetEnvironmentVariable("API_BASE_URL");
            if (!string.IsNullOrWhiteSpace(env)) return env;

            string configPath = "appsettings.json";
            if (!File.Exists(configPath))
            {
                configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            }

            if (File.Exists(configPath))
            {
                var json = File.ReadAllText(configPath);
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("ApiBaseUrl", out var prop))
                {
                    var val = prop.GetString();
                    if (!string.IsNullOrWhiteSpace(val)) return val;
                }
            }
        }
        catch { }
        return "http://127.0.0.1:5050/api/v1";
    }

    /// <summary>
    /// API eta MySQL datu-basearen osasuna egiaztatzen du /health endpoint bidez.
    /// Konexioak huts egiten badu, automatikoki localhost / 127.0.0.1 bidez ere egiaztatzen du fallback gisa.
    /// </summary>
    public async Task<(bool ApiMartxan, bool DatuBaseaMartxan, string Mezua)> CheckHealthAsync()
    {
        var candidates = new List<string>
        {
            "http://localhost:5050/api/v1",
            "http://127.0.0.1:5050/api/v1"
        };

        if (!string.IsNullOrWhiteSpace(_baseUrl) &&
            !candidates.Contains(_baseUrl, StringComparer.OrdinalIgnoreCase))
        {
            candidates.Add(_baseUrl);
        }

        var failures = new List<string>();
        foreach (var candidate in candidates)
        {
            var attempts = candidate.StartsWith("http://localhost:", StringComparison.OrdinalIgnoreCase) ? 2 : 1;
            for (var attempt = 0; attempt < attempts; attempt++)
            {
                try
                {
                    var candidateUri = new Uri(candidate);
                    using var client = new HttpClient
                    {
                        BaseAddress = new Uri(candidateUri.GetLeftPart(UriPartial.Authority) + "/"),
                        Timeout = TimeSpan.FromSeconds(2.5)
                    };

                    using var res = await client.GetAsync("health");
                    if (res.IsSuccessStatusCode)
                    {
                        EguneratuBaseUrl(candidate);
                        return (true, true, "Datu-basea konektatuta dago.");
                    }
                    if ((int)res.StatusCode == 503)
                    {
                        EguneratuBaseUrl(candidate);
                        return (true, false, "MySQL datu-basea DESKONEKTATUTA dago (jatetxea_mysql).");
                    }

                    failures.Add($"{candidate}: HTTP {(int)res.StatusCode} ({res.StatusCode})");
                }
                catch (Exception ex)
                {
                    var detail = ex.GetBaseException().Message;
                    failures.Add($"{candidate}: {detail}");
                    System.Diagnostics.Debug.WriteLine($"Health errorea ({candidate}): {detail}");
                }

                if (attempt + 1 < attempts)
                {
                    await Task.Delay(250);
                }
            }
        }

        var failureDetails = failures.Count == 0 ? "" : $" Azken erantzunak: {string.Join(" | ", failures)}";
        return (false, false, $"API zerbitzaria deskonektatuta dago ({_baseUrl}).{failureDetails}");
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
                try
                {
                    var apiErr = await response.Content.ReadFromJsonAsync<ApiResponse<object>>(JsonOptions);
                    if (!string.IsNullOrWhiteSpace(apiErr?.Message))
                    {
                        return (false, apiErr.Message, null);
                    }
                }
                catch { }

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
            return (false, $"Ezin izan da API zerbitzariarekin konektatu ({_baseUrl}). Ziurtatu martxan dagoela.", null);
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
        var (arrakasta, _) = await AldatuMahaiarenEgoeraAsync(mahaiaId, egoera);
        return arrakasta;
    }

    /// <summary>
    /// Mahaiaren egoera aldatzen du eta APIren erantzun-mezua (edo errore-mezua) itzultzen du.
    /// </summary>
    public async Task<(bool Arrakasta, string Mezua)> AldatuMahaiarenEgoeraAsync(int mahaiaId, string egoera)
    {
        try
        {
            var payload = new { egoera };
            var response = await _http.PutAsJsonAsync($"mahaiak/{mahaiaId}/egoera", payload);
            if (!response.IsSuccessStatusCode)
            {
                try
                {
                    var apiErr = await response.Content.ReadFromJsonAsync<ApiResponse<object>>(JsonOptions);
                    if (!string.IsNullOrWhiteSpace(apiErr?.Message))
                    {
                        return (false, apiErr.Message);
                    }
                }
                catch { }

                var raw = await response.Content.ReadAsStringAsync();
                return (false, !string.IsNullOrWhiteSpace(raw) ? raw : "Ezin izan da mahaiaren egoera eguneratu.");
            }

            var apiRes = await response.Content.ReadFromJsonAsync<ApiResponse<object>>(JsonOptions);
            return (true, apiRes?.Message ?? "Mahaiaren egoera eguneratu da.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"AldatuMahaiarenEgoeraAsync errorea: {ex.Message}");
            return (false, $"Konexio errorea: {ex.Message}");
        }
    }

    /// <summary>
    /// Sukaldera bidali gabeko plater-lerroa ezabatzen du eta stocka leheneratzen du.
    /// </summary>
    public async Task<(bool Arrakasta, string Mezua)> EzabatuEskaeraLerroaAsync(int lerroId)
    {
        try
        {
            var response = await _http.DeleteAsync($"eskaerak/lerroak/{lerroId}");
            if (!response.IsSuccessStatusCode)
            {
                try
                {
                    var apiErr = await response.Content.ReadFromJsonAsync<ApiResponse<object>>(JsonOptions);
                    if (!string.IsNullOrWhiteSpace(apiErr?.Message))
                    {
                        return (false, apiErr.Message);
                    }
                }
                catch { }

                var raw = await response.Content.ReadAsStringAsync();
                return (false, !string.IsNullOrWhiteSpace(raw) ? raw : "Ezin izan da platera ezabatu.");
            }

            var apiRes = await response.Content.ReadFromJsonAsync<ApiResponse<object>>(JsonOptions);
            return (true, apiRes?.Message ?? "Platera eskaritik ezabatu da.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"EzabatuEskaeraLerroaAsync errorea: {ex.Message}");
            return (false, $"Konexio errorea: {ex.Message}");
        }
    }

    /// <summary>
    /// Platerik gabeko edo aktibo dagoen eskaera/kontu bat ezabatzen du (soft delete).
    /// </summary>
    public async Task<(bool Arrakasta, string Mezua)> EzabatuEskaeraAsync(int eskaeraId)
    {
        try
        {
            var response = await _http.DeleteAsync($"eskaerak/{eskaeraId}");
            if (!response.IsSuccessStatusCode)
            {
                try
                {
                    var apiErr = await response.Content.ReadFromJsonAsync<ApiResponse<object>>(JsonOptions);
                    if (!string.IsNullOrWhiteSpace(apiErr?.Message))
                    {
                        return (false, apiErr.Message);
                    }
                }
                catch { }

                var raw = await response.Content.ReadAsStringAsync();
                return (false, !string.IsNullOrWhiteSpace(raw) ? raw : "Ezin izan da eskaera ezabatu.");
            }

            var apiRes = await response.Content.ReadFromJsonAsync<ApiResponse<object>>(JsonOptions);
            return (true, apiRes?.Message ?? "Eskaera arrakastaz ezabatu da.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"EzabatuEskaeraAsync errorea: {ex.Message}");
            return (false, $"Konexio errorea: {ex.Message}");
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

    /// <summary>
    /// Langileak barne-txata aktibatuta daukan egiaztatzen du.
    /// </summary>
    public async Task<(bool Arrakasta, string Mezua, TxatEgoeraDto? Egoera)> GetTxatEgoeraAsync(int langileaId)
    {
        try
        {
            var response = await _http.GetAsync($"txata/egoera/{langileaId}");
            if (!response.IsSuccessStatusCode)
            {
                var apiErr = await response.Content.ReadFromJsonAsync<ApiResponse<object>>(JsonOptions);
                return (false, apiErr?.Message ?? "Ezin izan da txat-egoera egiaztatu.", null);
            }
            var apiRes = await response.Content.ReadFromJsonAsync<ApiResponse<TxatEgoeraDto>>(JsonOptions);
            return (true, apiRes?.Message ?? "", apiRes?.Data);
        }
        catch (Exception ex)
        {
            return (false, $"Konexio errorea: {ex.Message}", null);
        }
    }

    /// <summary>
    /// Gela bateko azken txat mezuak eskuratzen ditu.
    /// </summary>
    public async Task<(bool Arrakasta, string Mezua, List<TxatMezuaDto> Mezuak)> GetTxatMezuakAsync(string gela, int langileaId)
    {
        try
        {
            string url = $"txata/mezuak?gela={Uri.EscapeDataString(gela)}&limit=80&langileaId={langileaId}";
            var response = await _http.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                try
                {
                    var apiErr = await response.Content.ReadFromJsonAsync<ApiResponse<object>>(JsonOptions);
                    if (!string.IsNullOrWhiteSpace(apiErr?.Message))
                        return (false, apiErr.Message, new List<TxatMezuaDto>());
                }
                catch { }
                return (false, "Txata ez dago erabilgarri.", new List<TxatMezuaDto>());
            }

            var apiRes = await response.Content.ReadFromJsonAsync<ApiResponse<TxatMezuaDto>>(JsonOptions);
            return (true, apiRes?.Message ?? "", apiRes?.DataList ?? new List<TxatMezuaDto>());
        }
        catch (Exception ex)
        {
            return (false, $"Konexio errorea: {ex.Message}", new List<TxatMezuaDto>());
        }
    }

    /// <summary>
    /// Txat mezu berria bidaltzen du REST APIra.
    /// </summary>
    public async Task<(bool Arrakasta, string Mezua)> BidaliTxatMezuaAsync(int langileaId, string gela, string testua)
    {
        try
        {
            var payload = new { langileaId, gela, testua };
            var response = await _http.PostAsJsonAsync("txata/mezuak", payload);
            if (!response.IsSuccessStatusCode)
            {
                try
                {
                    var apiErr = await response.Content.ReadFromJsonAsync<ApiResponse<object>>(JsonOptions);
                    if (!string.IsNullOrWhiteSpace(apiErr?.Message))
                        return (false, apiErr.Message);
                }
                catch { }
                return (false, "Ezin izan da mezua bidali.");
            }
            return (true, "Mezua bidali da.");
        }
        catch (Exception ex)
        {
            return (false, $"Konexio errorea: {ex.Message}");
        }
    }
}
