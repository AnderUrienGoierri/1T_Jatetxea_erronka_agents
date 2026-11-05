using Microsoft.Extensions.FileProviders;
using MySqlConnector;

var builder = WebApplication.CreateBuilder(args);

// Zerbitzuak gehitu kontainerrean
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// CORS baimentzea edozein jatorritatik (Web, WinForms eta JavaFX bezeroentzat)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// HTTP eskaeren hoditeria konfiguratu
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowAll");

// Datu-baseko salbuespenen kudeaketa globala (503 Service Unavailable JSON garbian)
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (MySqlException ex)
    {
        context.Response.StatusCode = 503;
        context.Response.ContentType = "application/json";
        var errorea = new
        {
            status = "error",
            code = 503,
            message = "Datu-basea ez dago erabilgarri une honetan.",
            xehetasuna = ex.Message,
            soluzioa = "Egiaztatu Docker edukiontzia ('jatetxea_mysql') 3308 portuan martxan dagoela ('docker compose up -d')."
        };
        await context.Response.WriteAsJsonAsync(errorea);
    }
});

// Web aplikazioaren fitxategiak zerbitzatu /app bidean
var webAppPath = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", "web_php"));
if (Directory.Exists(webAppPath))
{
    app.UseFileServer(new FileServerOptions
    {
        FileProvider = new PhysicalFileProvider(webAppPath),
        RequestPath = "/app",
        EnableDefaultFiles = true
    });
}

app.UseAuthorization();

// Erro bidean APIaren deskribapen eta endpoint zerrenda osoa eskaini
app.MapGet("/", () => Results.Ok(new
{
    mezua = "Jatetxea Kudeaketa Sistema - REST API martxan dago!",
    bertsioa = "v1",
    arkitektura = "3-mailako arkitektura (Web PHP, Desktop C#, Desktop JavaFX)",
    datuBasea = "MySQL 8.0 InnoDB (Docker jatetxea_mysql:3308)",
    transakzioak = "SELECT ... FOR UPDATE errenkada-mailako blokeo pesimistak lehengaietan eta erreserbetan",
    endpointsGuztira = 32,
    endpoints = new[]
    {
        // 1. Mahaiak (/api/v1/mahaiak)
        "GET    /api/v1/mahaiak (?barneEzabatuak=false) - Mahai guztien egoera",
        "GET    /api/v1/mahaiak/{id} - Mahai bakarra ID bidez",
        "POST   /api/v1/mahaiak - Mahai berria sortu",
        "PUT    /api/v1/mahaiak/{id}/egoera - Mahaiaren egoera eguneratu (Libre, Okupatuta, Erreserbatuta, Garbitzen)",
        "DELETE /api/v1/mahaiak/{id} (?hard=false) - Mahaia ezabatu (Soft/Hard)",
        "PUT    /api/v1/mahaiak/{id}/restore - Desgaitutako mahaia berreskuratu",

        // 2. Lehengaiak (/api/v1/lehengaiak)
        "GET    /api/v1/lehengaiak (?barneEzabatuak=false) - Lehengai guztien zerrenda eta uneko stocka",
        "GET    /api/v1/lehengaiak/{id} - Lehengai bakarra ID bidez",
        "GET    /api/v1/lehengaiak/alertak - Erosketa-zerrenda: gutxieneko stocketik behera dauden lehengaiak",
        "POST   /api/v1/lehengaiak - Lehengai berria sortu",
        "POST   /api/v1/lehengaiak/sarrera - Hornitzailearen albarana erregistratu eta stocka gehitu transakzionalean",
        "DELETE /api/v1/lehengaiak/{id} (?hard=false) - Lehengaia ezabatu (Soft/Hard)",
        "PUT    /api/v1/lehengaiak/{id}/restore - Desgaitutako lehengaia berreskuratu",

        // 3. Platerak (/api/v1/platerak)
        "GET    /api/v1/platerak (?kategoria=...&barneEzabatuak=false) - Platerak zerrendatu",
        "GET    /api/v1/platerak/{id} - Plater bakarra ID bidez",
        "GET    /api/v1/platerak/erabilgarritasuna - Plater bakoitzetik zenbat anoa prestatu daitezkeen kalkulatuta",
        "GET    /api/v1/platerak/{id}/eskandalloa - Plater baten errezeta eta osagaien proportzioak",
        "POST   /api/v1/platerak - Plater berria errezeta eta osagaiekin sortu",
        "DELETE /api/v1/platerak/{id} (?hard=false) - Platera ezabatu (Soft/Hard)",
        "PUT    /api/v1/platerak/{id}/restore - Desgaitutako platera berreskuratu",

        // 4. Erreserbak (/api/v1/erreserbak)
        "GET    /api/v1/erreserbak (?data=YYYY-MM-DD&txanda=...&barneEzabatuak=false) - Erreserba zerrenda",
        "GET    /api/v1/erreserbak/{id} - Erreserba bakarra ID bidez",
        "POST   /api/v1/erreserbak (?itxaron=ms) - Erreserba transakzional pesimista FOR UPDATE bidez (1062 -> 409 Conflict)",
        "DELETE /api/v1/erreserbak/{id} (?hard=false) - Erreserba ezeztatu edo ezabatu",
        "PUT    /api/v1/erreserbak/{id}/restore - Ezeztatutako erreserba berreskuratu",

        // 5. Eskaerak / Komandak (/api/v1/eskaerak)
        "GET    /api/v1/eskaerak (?mahaiaId=...&egoera=...&barneEzabatuak=false) - Eskaerak zerrendatu",
        "GET    /api/v1/eskaerak/{id} - Eskaera osoa lerroekin eta plateren xehetasunekin",
        "POST   /api/v1/eskaerak - Komanda berria ireki mahaian",
        "POST   /api/v1/eskaerak/{id}/lerroak - Platera gehitu komandara eta lehengaiak blokeatu/deskontatu FOR UPDATE transakzioan",
        "PUT    /api/v1/eskaerak/{id}/egoera - Eskaeraren egoera eguneratu (Irekia, Sukaldean, Zerbitzatua, Kobratua)",
        "PUT    /api/v1/eskaerak/lerroak/{lerroId}/egoera - KDS sukaldeko pantailarako lerroaren egoera eguneratu",
        "DELETE /api/v1/eskaerak/{id} - Eskaera ezabatu",

        // 6. Bezeroak (/api/v1/bezeroak)
        "GET    /api/v1/bezeroak (?barneEzabatuak=false) - Bezero guztien zerrenda",
        "GET    /api/v1/bezeroak/{id} - Bezero bakarra ID bidez",
        "POST   /api/v1/bezeroak - Bezero berria sortu",
        "PUT    /api/v1/bezeroak/{id} - Bezeroaren datuak eguneratu",
        "DELETE /api/v1/bezeroak/{id} (?hard=false) - Bezeroa ezabatu",

        // 7. Langileak (/api/v1/langileak)
        "GET    /api/v1/langileak (?barneEzabatuak=false) - Langileen zerrenda",
        "GET    /api/v1/langileak/{id} - Langile bakarra ID bidez",
        "POST   /api/v1/langileak - Langile berria sortu",
        "POST   /api/v1/langileak/login - Langilearen saio-hasiera (PIN / erabiltzailea)",

        // 8. Iritziak (/api/v1/iritziak)
        "GET    /api/v1/iritziak (?barneEzabatuak=false) - Iritzi guztien zerrenda",
        "GET    /api/v1/iritziak/estatistikak - Iritzien estatistika orokorrak eta batez besteko balorazioa",
        "POST   /api/v1/iritziak - Bezeroaren iritzi berria (0-5 izar)",
        "DELETE /api/v1/iritziak/{id} (?hard=false) - Iritzia ezabatu"
    }
}));

// Health Check endpoint-ak: APIa eta Datu-basea martxan dauden egiaztatzeko
async Task<IResult> HealthCheckHandler(IConfiguration config)
{
    string connStr = config.GetConnectionString("Jatetxea") ?? "";
    try
    {
        await using var conn = new MySqlConnection(connStr);
        await conn.OpenAsync();
        return Results.Ok(new
        {
            status = "Healthy",
            database = "Connected",
            engine = "MySQL 8.0 InnoDB",
            port = 3308,
            timestamp = DateTime.UtcNow
        });
    }
    catch (Exception ex)
    {
        return Results.Json(new
        {
            status = "Unhealthy",
            database = "Disconnected",
            error = ex.Message,
            solution = "Egiaztatu Docker Desktop-eko 'jatetxea_mysql' edukiontzia martxan dagoela ('docker compose up -d').",
            timestamp = DateTime.UtcNow
        }, statusCode: 503);
    }
}

app.MapGet("/health", (IConfiguration config) => HealthCheckHandler(config));
app.MapGet("/api/v1/health", (IConfiguration config) => HealthCheckHandler(config));

app.MapControllers();

// Abiaraztean Datu-basearen konexioa egiaztatu eta kontsolan argi inprimatu
EgiaztatuDatuBasea(app.Configuration);

app.Run();

static void EgiaztatuDatuBasea(IConfiguration config)
{
    string connStr = config.GetConnectionString("Jatetxea") ?? "";
    try
    {
        using var conn = new MySqlConnection(connStr);
        conn.Open();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine();
        Console.WriteLine("==================================================================");
        Console.WriteLine("[DATU-BASEA OK] MySQL konexioa ondo ezarri da!");
        Console.WriteLine("   -> Zerbitzaria: localhost:3308 | Datu-basea: jatetxea_db");
        Console.WriteLine("==================================================================");
        Console.WriteLine();
        Console.ResetColor();
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine();
        Console.WriteLine("==================================================================");
        Console.WriteLine("[ABISUA: DATU-BASEA EZ DAGO ERABILGARRI]");
        Console.WriteLine($"   -> Errorea: {ex.Message}");
        Console.WriteLine("   -> Kausa: 'jatetxea_mysql' edukiontzia (3308) itzalita egon daiteke.");
        Console.WriteLine("   -> Konponbidea: Exekutatu 'docker compose up -d' terminalean.");
        Console.WriteLine("==================================================================");
        Console.WriteLine();
        Console.ResetColor();
    }
}
