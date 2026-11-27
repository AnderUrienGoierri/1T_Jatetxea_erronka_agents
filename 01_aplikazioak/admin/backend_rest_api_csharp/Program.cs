using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
using MySqlConnector;
using NHibernate;
using NHibernate.Exceptions;
using Jatetxea.Api.Data;
using Jatetxea.Api.Helpers;
using Jatetxea.Api.Responses;
using Jatetxea.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Entzun sare-interfaze guztietan (0.0.0.0), mugikorrak Wi-Fi bidez konektatu ahal izateko
builder.WebHost.UseUrls(Environment.GetEnvironmentVariable("ASPNETCORE_URLS") ?? "http://0.0.0.0:5000");

// Zerbitzuak gehitu kontainerrean
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// JWT Bearer autentifikazioa + Rol bidezko baimenak (RBAC)
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = JwtHelper.LortuBalidazioParametroak(builder.Configuration);
        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(ApiError.Create(
                    401,
                    "UNAUTHORIZED",
                    "Autentifikazioa beharrezkoa da (Authorization: Bearer <token>).",
                    "Hasi saioa POST /api/v1/langileak/login endpoint-ean JWT tokena eskuratzeko."));
            },
            OnForbidden = async context =>
            {
                context.Response.StatusCode = 403;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(ApiError.Create(
                    403,
                    "FORBIDDEN",
                    "Ez duzu beharrezko rolik baliabide hau kontsultatzeko.",
                    "Admin, AdminIT edo Kudeatzailea rola behar da."));
            }
        };
    });
builder.Services.AddAuthorization();

// NHibernate + FluentNHibernate ISessionFactory Singleton gisa erregistratu (behin bakarrik eraikitzen da)
builder.Services.AddSingleton<ISessionFactory>(_ => NHibernateHelper.SortuSessionFactory(builder.Configuration));

// Datu-basearen konexioa denbora errealean (atzeko planoan) monitorizatzen duen zerbitzua
builder.Services.AddHostedService<DatuBaseaMonitorZerbitzua>();

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

// HTTP eskaeren hoditeria konfiguratu — OpenAPI + Swagger UI ingurune guztietan (Docker barne)
app.MapOpenApi();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "1T Jatetxea REST API v1 (NHibernate)");
    options.RoutePrefix = "swagger";
    options.DocumentTitle = "1T Jatetxea — REST API Dokumentazio Interaktiboa";
});

app.UseCors("AllowAll");

// Konektatutako bezeroak eta aplikazioak denbora errealean erregistratzeko middleware-a
app.Use(async (context, next) =>
{
    string path = context.Request.Path.Value ?? "";

    // Monitorearen polling bera ez erregistratu aplikazio arrunt gisa
    if (!path.Equals("/api/v1/aplikazioak-linean", StringComparison.OrdinalIgnoreCase) &&
        !path.Equals("/favicon.ico", StringComparison.OrdinalIgnoreCase))
    {
        string appId = AplikazioMonitoreZerbitzua.IdentifikatuAplikazioa(context);
        string ip = context.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";

        string? erabiltzailea = null;
        var headers = context.Request.Headers;
        if (headers.TryGetValue("X-Langilea", out var lVal) && !string.IsNullOrWhiteSpace(lVal))
            erabiltzailea = lVal.ToString();
        else if (headers.TryGetValue("X-Erabiltzailea", out var eVal) && !string.IsNullOrWhiteSpace(eVal))
            erabiltzailea = eVal.ToString();
        else if (headers.TryGetValue("X-User-Name", out var unVal) && !string.IsNullOrWhiteSpace(unVal))
            erabiltzailea = unVal.ToString();
        else if (headers.TryGetValue("X-User", out var uVal) && !string.IsNullOrWhiteSpace(uVal))
            erabiltzailea = uVal.ToString();
        else if (headers.TryGetValue("X-Admin-User", out var auVal) && !string.IsNullOrWhiteSpace(auVal))
            erabiltzailea = auVal.ToString();

        AplikazioMonitoreZerbitzua.ErregistratuEskaera(appId, ip, $"{context.Request.Method} {path}", erabiltzailea);
    }

    await next();
});

// Datu-baseko salbuespenen kudeaketa globala (503 Service Unavailable JSON garbian)
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex) when (ex is MySqlException || ex is GenericADOException || ex.InnerException is MySqlException)
    {
        var mysqlEx = ex as MySqlException ?? ex.InnerException as MySqlException;
        context.Response.StatusCode = 503;
        context.Response.ContentType = "application/json";
        var errorea = new
        {
            status = "error",
            code = 503,
            message = "Datu-basea ez dago erabilgarri une honetan.",
            xehetasuna = mysqlEx?.Message ?? ex.Message,
            soluzioa = "Egiaztatu Docker edukiontzia ('jatetxea_mysql') 3308 portuan martxan dagoela ('docker compose up -d')."
        };
        await context.Response.WriteAsJsonAsync(errorea);
    }
});

// Web aplikazioaren fitxategiak zerbitzatu /app bidean
var webAppPath = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", "bezeroak", "web_php_bezeroak"));
if (!Directory.Exists(webAppPath))
{
    webAppPath = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", "web_php"));
}
if (Directory.Exists(webAppPath))
{
    app.UseFileServer(new FileServerOptions
    {
        FileProvider = new PhysicalFileProvider(webAppPath),
        RequestPath = "/app",
        EnableDefaultFiles = true
    });
}

app.UseAuthentication();
app.UseAuthorization();

// Erro bidean APIaren deskribapen, datu-basearen egoera eta endpoint zerrenda osoa eskaini
app.MapGet("/", () =>
{
    bool dbKonektatuta = DatuBaseaMonitorZerbitzua.IsKonektatuta;
    string dbOharra = DatuBaseaMonitorZerbitzua.ZerbitzariaOharra;

    return Results.Ok(new
    {
        mezua = "Jatetxea Kudeaketa Sistema - REST API martxan dago!",
        datuBaseaKonektatuta = dbKonektatuta,
        datuBaseaEgoera = dbKonektatuta ? "KONEKTATUTA (Linean)" : "DESKONEKTATUTA / ITZALITA",
        datuBaseaAbisua = dbKonektatuta
            ? "MySQL konexioa aktibo eta egonkorra dago. Eragiketa guztiak eskuragarri daude."
            : "ADI! Datu-basea deskonektatuta dago. APIa martxan dago baina datu-baseko kontsultak ezin dira burutu.",
        datuBasea = dbKonektatuta
            ? $"MySQL 8.0 InnoDB ({dbOharra}) - KONEKTATUTA"
            : $"MySQL 8.0 InnoDB ({dbOharra}) - DESKONEKTATUTA / ITZALITA",
        datuBaseaXehetasunak = DatuBaseaMonitorZerbitzua.AzkenEgoeraMezua,
        azkenEgiaztapena = DatuBaseaMonitorZerbitzua.AzkenEgiaztapena != DateTime.MinValue
            ? DatuBaseaMonitorZerbitzua.AzkenEgiaztapena.ToString("yyyy-MM-dd HH:mm:ss")
            : "Hasieratzen...",
        soluzioa = dbKonektatuta
            ? null
            : "Datu-basea pizteko IT Kontrol Zentroa (http://localhost:5000) erabili edo kontsolan 'docker compose up -d jatetxea-db' exekutatu.",
        bertsioa = "v1",
        orm = "NHibernate 5.7 + FluentNHibernate 3.5 (MySQL8InnoDBDialect)",
        segurtasuna = "BCrypt PIN/Pasahitzak + JWT Bearer (HS256) + RBAC",
        swagger = "/swagger",
        openapi = "/openapi/v1.json",
        arkitektura = "3-mailako arkitektura (Web PHP, Desktop C#, Desktop JavaFX, Android/TV Kotlin, Flutter)",
        transakzioak = "NHibernate ITransaction (IsolationLevel.ReadCommitted) + LockMode.Upgrade (SELECT ... FOR UPDATE) blokeo pesimistak",
        endpointsGuztira = 38,
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
            "PUT    /api/v1/eskaerak/{id}/egoera - Eskaeraren egoera eguneratu (Irekia, Sukaldean, Prestatua, Zerbitzatua, Kobratua)",
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
            "DELETE /api/v1/iritziak/{id} (?hard=false) - Iritzia ezabatu",

            // 9. Fakturak eta Finantzak (/api/v1/fakturak)
            "GET    /api/v1/fakturak (?mota=Salmenta|Erosketa&egoera=...) - Salmenta eta gastu fakturen zerrenda",
            "GET    /api/v1/fakturak/laburpena - Finantza laburpena: diru-sarrerak, gastuak, mozkin garbia eta BEZa",
            "GET    /api/v1/fakturak/{id} - Faktura bakarra ID bidez",
            "POST   /api/v1/fakturak - Faktura berria erregistratu (salmenta edo hornitzaile gastua)",
            "PUT    /api/v1/fakturak/{id}/egoera - Fakturaren egoera aldatu (Ordaindua, Zain, Ezeztatua)",
            "DELETE /api/v1/fakturak/{id} - Faktura ezabatu",

            // 10. Analitika, Paginazioa, Batch Transakzioak eta JWT Segurtasuna
            "GET    /swagger - OpenAPI / Swagger UI dokumentazio interaktiboa",
            "GET    /api/v1/langileak/nire-profila [JWT] - Autentifikatutako langilearen profila",
            "GET    /api/v1/txostenak/laburpena (?hasiera=...&amaiera=...&top=5) - Analitika agregatua (Top platerak, ordainketak, langileak, biltegia)",
            "GET    /api/v1/txostenak/platerak-bilatu (?search=...&kategoria=...&minPrezioa=...&maxPrezioa=...&sortBy=...&page=1&pageSize=10) - Bilaketa eta paginazio dinamikoa",
            "GET    /api/v1/txostenak/auditoria-segurua [JWT: Admin/AdminIT/Kudeatzailea] - Rol bidez babestutako auditoria",
            "POST   /api/v1/eskaerak/komanda-osoa - Batch transakzio atomikoa (komanda + hainbat plater + stock deskontu metatua)",
            "POST   /api/v1/ordainketak/zatikatu - Ordainketa zatitua hainbat metodotan + faktura automatikoa transakzio bakarrean"
        }
    });
});

// Health Check endpoint-ak: APIa eta Datu-basea martxan dauden egiaztatzeko
var osasunEgiaztapena = () =>
{
    bool konektatuta = DatuBaseaMonitorZerbitzua.IsKonektatuta;
    string dbOharra = DatuBaseaMonitorZerbitzua.ZerbitzariaOharra;

    if (konektatuta)
    {
        return Results.Ok(new
        {
            status = "Healthy",
            egoera = "Konektatuta",
            database = "Connected",
            engine = "MySQL 8.0 InnoDB",
            server = dbOharra,
            azkenEgiaztapena = DatuBaseaMonitorZerbitzua.AzkenEgiaztapena != DateTime.MinValue
                ? DatuBaseaMonitorZerbitzua.AzkenEgiaztapena.ToString("yyyy-MM-dd HH:mm:ss")
                : null,
            timestamp = DateTime.UtcNow
        });
    }

    return Results.Json(new
    {
        status = "Unhealthy",
        egoera = "Deskonektatuta",
        database = "Disconnected",
        exceptionType = DatuBaseaMonitorZerbitzua.AzkenSalbuespenMota,
        server = dbOharra,
        errorea = DatuBaseaMonitorZerbitzua.AzkenEgoeraMezua,
        soluzioa = "Egiaztatu Docker-eko 'jatetxea_mysql' edukiontzia martxan dagoela ('docker compose up -d jatetxea-db') edo IT Kontrol Zentroan (http://localhost:5000) piztu.",
        azkenEgiaztapena = DatuBaseaMonitorZerbitzua.AzkenEgiaztapena != DateTime.MinValue
            ? DatuBaseaMonitorZerbitzua.AzkenEgiaztapena.ToString("yyyy-MM-dd HH:mm:ss")
            : null,
        timestamp = DateTime.UtcNow
    }, statusCode: 503);
};

app.MapGet("/health", osasunEgiaztapena);
app.MapGet("/osasuna", osasunEgiaztapena);
app.MapGet("/api/v1/health", osasunEgiaztapena);
app.MapGet("/api/v1/osasuna", osasunEgiaztapena);

// /api/v1 erroa informaziora bideratu
app.MapGet("/api/v1", () => Results.Redirect("/"));

// Denbora errealean konektatutako aplikazioen zerrenda
app.MapGet("/api/v1/aplikazioak-linean", () => Results.Ok(AplikazioMonitoreZerbitzua.LortuEgoeraLaburpena()));

app.MapControllers();

app.Run();
