using MySqlConnector;

namespace Jatetxea.Api.Services;

/// <summary>
/// MySQL datu-basearen konexioa atzeko planoan aldizka (denbora errealean)
/// monitorizatzen duen zerbitzua. Egoera aldatzean (itzali edo berriz piztu),
/// kontsolan abisu argia inprimatzen du berehala.
/// </summary>
public class DatuBaseaMonitorZerbitzua : BackgroundService
{
    private readonly IConfiguration _config;
    private readonly ILogger<DatuBaseaMonitorZerbitzua> _logger;
    private string _connStr = string.Empty;

    /// <summary>
    /// Uneko egoera orokorra (beste kontroladore edo endpoint-ek kontsultatu ahal izateko)
    /// </summary>
    public static bool IsKonektatuta { get; private set; } = false;
    public static string AzkenEgoeraMezua { get; private set; } = "Konexioa hasieratzen...";
    public static DateTime AzkenEgiaztapena { get; private set; } = DateTime.MinValue;

    public DatuBaseaMonitorZerbitzua(IConfiguration config, ILogger<DatuBaseaMonitorZerbitzua> logger)
    {
        _config = config;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        string? envConn = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
        _connStr = !string.IsNullOrWhiteSpace(envConn)
            ? envConn
            : (_config.GetConnectionString("Jatetxea") ?? _config.GetConnectionString("JatetxeaRoot") ?? "");

        bool? aurrekoEgoera = null;

        // Atzeko planoan aldizka (3 segundoro) egiaztatzen du
        while (!stoppingToken.IsCancellationRequested)
        {
            var (konektatuta, errorea) = await EgiaztatuKonexioaAsync(stoppingToken);

            AzkenEgiaztapena = DateTime.Now;
            IsKonektatuta = konektatuta;

            // Egoera aldaketa gertatu denean bakarrik inprimatu kontsolan mezu deigarria
            if (aurrekoEgoera == null || aurrekoEgoera != konektatuta)
            {
                if (konektatuta)
                {
                    AzkenEgoeraMezua = "MySQL datu-basea KONEKTATUTA dago.";
                    InprimatuKonektatuta(aurrekoEgoera != null);
                }
                else
                {
                    AzkenEgoeraMezua = $"MySQL datu-basea DESKONEKTATUTA dago: {errorea}";
                    InprimatuDeskonektatuta(errorea);
                }

                aurrekoEgoera = konektatuta;
            }

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    private async Task<(bool Konektatuta, string? Errorea)> EgiaztatuKonexioaAsync(CancellationToken ct)
    {
        try
        {
            await using var conn = new MySqlConnection(_connStr);
            await conn.OpenAsync(ct);
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    private const string AnsiRed = "\u001b[91m";
    private const string AnsiGreen = "\u001b[92m";
    private const string AnsiReset = "\u001b[0m";

    private static void InprimatuKonektatuta(bool berreskuratua)
    {
        try { Console.ForegroundColor = ConsoleColor.Green; } catch { }
        Console.WriteLine();
        Console.WriteLine($"{AnsiGreen}=================================================================={AnsiReset}");
        if (berreskuratua)
        {
            Console.WriteLine($"{AnsiGreen}[{DateTime.Now:HH:mm:ss}] [DATU-BASEA BERRESKURATUA - KONEKTATUTA]{AnsiReset}");
            Console.WriteLine($"{AnsiGreen}   -> MySQL konexioa berriz ondo ezarri da (localhost:3308).{AnsiReset}");
            Console.WriteLine($"{AnsiGreen}   -> APIak eskaerak modu normalean prozesatuko ditu.{AnsiReset}");
        }
        else
        {
            Console.WriteLine($"{AnsiGreen}[{DateTime.Now:HH:mm:ss}] [DATU-BASEA OK] MySQL konexioa ondo ezarri da!{AnsiReset}");
            Console.WriteLine($"{AnsiGreen}   -> Zerbitzaria: localhost:3308 | Datu-basea: jatetxea_db{AnsiReset}");
        }
        Console.WriteLine($"{AnsiGreen}=================================================================={AnsiReset}");
        Console.WriteLine();
        try { Console.ResetColor(); } catch { }
    }

    private static void InprimatuDeskonektatuta(string? errorea)
    {
        try { Console.ForegroundColor = ConsoleColor.Red; } catch { }
        Console.WriteLine();
        Console.WriteLine($"{AnsiRed}=================================================================={AnsiReset}");
        Console.WriteLine($"{AnsiRed}[{DateTime.Now:HH:mm:ss}] [ADI! DATU-BASEA ITZALI DA / DESKONEKTATUTA]{AnsiReset}");
        Console.WriteLine($"{AnsiRed}   -> Kausa: Ezin izan da MySQL-ra konektatu (localhost:3308).{AnsiReset}");
        if (!string.IsNullOrWhiteSpace(errorea))
        {
            Console.WriteLine($"{AnsiRed}   -> Errore mezua: {errorea}{AnsiReset}");
        }
        Console.WriteLine($"{AnsiRed}   -> Konponbidea: 'docker compose up -d' exekutatu datu-basea berriz pizteko.{AnsiReset}");
        Console.WriteLine($"{AnsiRed}=================================================================={AnsiReset}");
        Console.WriteLine();
        try { Console.ResetColor(); } catch { }
    }
}

