using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Portua konfiguratu (0.0.0.0:5000 - IT Kontrol Zentrala)
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000);
});

// CORS baimendu
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// Kalkulatu erroko karpeta dinamikoki eta sendo
static string LocateProjectRoot(string baseDir)
{
    string? cur = baseDir;
    while (!string.IsNullOrEmpty(cur))
    {
        if (File.Exists(Path.Combine(cur, "docker-compose.yml")) ||
            Directory.Exists(Path.Combine(cur, "01_aplikazioak")))
        {
            return cur;
        }
        var parent = Directory.GetParent(cur);
        if (parent == null) break;
        cur = parent.FullName;
    }
    return AppContext.BaseDirectory;
}

string projectRoot = LocateProjectRoot(builder.Environment.ContentRootPath);
if (!File.Exists(Path.Combine(projectRoot, "docker-compose.yml")))
{
    projectRoot = LocateProjectRoot(AppContext.BaseDirectory);
}

string webRootCandidate = Path.Combine(projectRoot, "01_aplikazioak", "admin_IT_zentrala", "server", "Jatetxea.AdminZentrala", "wwwroot");
if (Directory.Exists(webRootCandidate))
{
    builder.Environment.WebRootPath = webRootCandidate;
}

var app = builder.Build();

app.UseCors("AllowAll");
app.UseDefaultFiles();
app.UseStaticFiles();

// Osasun egiaztapena
app.MapGet("/health", () => Results.Ok(new { status = "OK", app = "Admin IT Zentrala", port = 5000 }));

// Ziurtatu erroan (/) IT Kontrol Zentralaren web interfazea (index.html) irekitzen dela beti
app.MapGet("/", async (HttpContext context) =>
{
    string indexPath = Path.Combine(app.Environment.WebRootPath ?? "", "index.html");
    if (!File.Exists(indexPath))
    {
        indexPath = Path.Combine(projectRoot, "01_aplikazioak", "admin_IT_zentrala", "server", "Jatetxea.AdminZentrala", "wwwroot", "index.html");
    }
    if (!File.Exists(indexPath))
    {
        indexPath = Path.Combine(AppContext.BaseDirectory, "wwwroot", "index.html");
    }
    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.SendFileAsync(indexPath);
});

// Oinarrizko bideak kalkulatu
string dockerDir = Path.Combine(projectRoot, "1T_Jatetxea_Entorno_Birtuala");
string vmDir = Path.Combine(projectRoot, "erronka_egitura");
string labDir = Path.Combine(projectRoot, "laborategi_fisikoa");
string vboxPath = @"C:\Program Files\Oracle\VirtualBox\VBoxManage.exe";

// Auditoretza log-a
var auditLogs = new List<AuditLogItem>();

void LogAction(string user, string action, string details)
{
    auditLogs.Insert(0, new AuditLogItem(DateTime.Now.ToString("HH:mm:ss"), user, action, details));
    if (auditLogs.Count > 100) auditLogs.RemoveAt(auditLogs.Count - 1);
}

// ---------------------------------------------------------------------------
// 1. ADMIN TALDEAREN APIA
// ---------------------------------------------------------------------------
app.MapGet("/api/admins", () =>
{
    return Results.Ok(new[]
    {
        new { Id = "AnderU", Izena = "Ander Urien", Rola = "IT Lead / FullStack Arkitektoa" },
        new { Id = "Julen",  Izena = "Julen",       Rola = "Sare eta Suebaki Administratzailea" },
        new { Id = "Iker",   Izena = "Iker",        Rola = "Sistemen eta Birtualizazio Ingeniaria" },
        new { Id = "Aratz",  Izena = "Aratz",       Rola = "Zibersegurtasun eta Ebaluazio Teknikaria" }
    });
});

app.MapGet("/api/audit", () => Results.Ok(auditLogs));

// ---------------------------------------------------------------------------
// 2. DOCKER KUDEAKETA APIA (BANAKA, GUZTIAK ETA FLUXU KOORDINATUA)
// ---------------------------------------------------------------------------
app.MapGet("/api/docker/status", async () =>
{
    try
    {
        var output = await RunCommandAsync("docker", "compose ps --format json", dockerDir);
        var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        var containers = new List<object>();

        foreach (var l in lines)
        {
            try
            {
                using var doc = JsonDocument.Parse(l);
                var root = doc.RootElement;
                containers.Add(new
                {
                    Name = root.TryGetProperty("Name", out var n) ? n.GetString() : "",
                    Service = root.TryGetProperty("Service", out var s) ? s.GetString() : "",
                    State = root.TryGetProperty("State", out var st) ? st.GetString() : "",
                    Status = root.TryGetProperty("Status", out var su) ? su.GetString() : "",
                    Publishers = root.TryGetProperty("Publishers", out var pb) ? pb.ToString() : ""
                });
            }
            catch { }
        }

        if (containers.Count == 0)
        {
            var psOut = await RunCommandAsync("docker", "ps -a --filter \"name=jatetxea_\" --format \"{{.Names}}|{{.Status}}|{{.State}}|{{.Ports}}\"", dockerDir);
            foreach (var pl in psOut.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var parts = pl.Split('|');
                if (parts.Length >= 3)
                {
                    containers.Add(new
                    {
                        Name = parts[0],
                        Service = parts[0].Replace("jatetxea_", ""),
                        State = parts[2],
                        Status = parts[1],
                        Publishers = parts.Length > 3 ? parts[3] : ""
                    });
                }
            }
        }

        bool dockerDaemonOk = !output.Contains("failed to connect to the docker API") && !output.Contains("error during connect");

        return Results.Ok(new
        {
            Success = true,
            DockerRunning = dockerDaemonOk,
            Containers = containers
        });
    }
    catch (Exception ex)
    {
        return Results.Ok(new { Success = false, DockerRunning = false, Error = ex.Message, Containers = Array.Empty<object>() });
    }
});

// Banakako ekintzak (start, stop, pause, unpause, restart)
app.MapPost("/api/docker/action/{service}/{action}", async (string service, string action, HttpContext ctx) =>
{
    string user = ctx.Request.Headers["X-Admin-User"].FirstOrDefault() ?? "Admin";
    action = action.ToLowerInvariant();

    var validActions = new[] { "start", "stop", "restart", "pause", "unpause" };
    if (!validActions.Contains(action))
    {
        return Results.BadRequest(new { Success = false, Message = "Ekintza baliogabea. Baimenduak: start, stop, restart, pause, unpause" });
    }

    LogAction(user, $"DOCKER_{action.ToUpper()}", $"Zerbitzua: {service}");

    // Edukiontzi izena zerbitzu izenera mapatu
    string composeService = service switch
    {
        "jatetxea_mysql" => "jatetxea-db",
        "jatetxea_api" => "jatetxea-api",
        "jatetxea_web_php" => "jatetxea-web",
        "jatetxea_chat" => "jatetxea-chat",
        "jatetxea_kds_tv" => "jatetxea-kds-tv",
        "jatetxea_odoo_erp" => "jatetxea-odoo",
        "jatetxea_odoo_postgres" => "db-odoo",
        _ => service
    };

    var outText = await RunCommandAsync("docker", $"compose {action} {composeService}", dockerDir);
    if (outText.Contains("no such service") || outText.Contains("No such container"))
    {
        outText = await RunCommandAsync("docker", $"{action} {service}", dockerDir);
    }

    // Datu-baseak abiaraztean kudeaketa web interfazeak ere ziurtatu
    if (action == "start")
    {
        if (composeService == "jatetxea-db")
        {
            _ = RunCommandAsync("docker", "compose up -d phpmyadmin", dockerDir);
        }
        else if (composeService == "db-odoo")
        {
            _ = RunCommandAsync("docker", "compose up -d adminer", dockerDir);
        }
    }

    return Results.Ok(new { Success = true, Action = action, Service = service, Output = outText });
});

// Taldeko ekintzak (guztiak batera: up, down, pause, unpause, restart)
app.MapPost("/api/docker/all/{action}", async (string action, HttpContext ctx) =>
{
    string user = ctx.Request.Headers["X-Admin-User"].FirstOrDefault() ?? "Admin";
    action = action.ToLowerInvariant();

    string cmdArgs = action switch
    {
        "up" => "compose up -d",
        "down" => "compose down",
        "pause" => "compose pause",
        "unpause" => "compose unpause",
        "restart" => "compose restart",
        "stop" => "compose stop",
        "start" => "compose start",
        _ => "compose ps"
    };

    LogAction(user, $"DOCKER_ALL_{action.ToUpper()}", "Edukiontzi guztiak aldi berean kudeatzen");
    var outText = await RunCommandAsync("docker", cmdArgs, dockerDir);
    return Results.Ok(new { Success = true, Action = action, Output = outText });
});

// FLUXU KOORDINATUA: URRATSEZ URRATSEKO ABIARAZTE INTEGRALA
app.MapPost("/api/docker/flow/start", async (HttpContext ctx) =>
{
    string user = ctx.Request.Headers["X-Admin-User"].FirstOrDefault() ?? "Admin";
    LogAction(user, "FLUXU_KOORDINATUA_START", "Abiarazte koordinatua martxan (DB -> Backend -> Frontend)");

    var steps = new List<object>();

    // 1. URRATSA: Datu-Baseak (MySQL + PostgreSQL + phpMyAdmin + Adminer)
    var step1 = await RunCommandAsync("docker", "compose up -d jatetxea-db db-odoo phpmyadmin adminer", dockerDir);
    steps.Add(new { Phase = 1, Name = "Datu-Baseak (MySQL 3308, PostgreSQL 5432, phpMyAdmin 8081, Adminer 8082)", Status = "Martxan jarrita", Output = step1 });
    OpenSystemBrowser("http://localhost:8081");
    OpenSystemBrowser("http://localhost:8082");
    await Task.Delay(2500); // DB initializazio tartea

    // 2. URRATSA: Backend eta API Zentrala (.NET API + Chat)
    var step2 = await RunCommandAsync("docker", "compose up -d jatetxea-api jatetxea-chat", dockerDir);
    steps.Add(new { Phase = 2, Name = "Backend eta Negozio Logika (API 5050 & Chat 3001)", Status = "Martxan jarrita", Output = step2 });
    await Task.Delay(2000);

    // 3. URRATSA: Bezeroak, Web Publikoa eta KDS Pantailak
    var step3 = await RunCommandAsync("docker", "compose up -d jatetxea-web jatetxea-kds-tv jatetxea-odoo", dockerDir);
    steps.Add(new { Phase = 3, Name = "Frontend eta Kudeaketa (Web PHP 8085, KDS TV 8086, Odoo 8070)", Status = "Martxan jarrita", Output = step3 });

    return Results.Ok(new { Success = true, Message = "Fluxu koordinatua arrakastaz burutu da.", Steps = steps });
});

// FLUXU KOORDINATUA: URRATSEZ URRATSEKO GELDITZE SEGURUA
app.MapPost("/api/docker/flow/stop", async (HttpContext ctx) =>
{
    string user = ctx.Request.Headers["X-Admin-User"].FirstOrDefault() ?? "Admin";
    LogAction(user, "FLUXU_KOORDINATUA_STOP", "Gelditze koordinatua martxan (Frontend -> Backend -> DB)");

    var steps = new List<object>();

    // 1. URRATSA: Frontendak itzali lehenik (eskaera berririk ez sartzeko)
    var step1 = await RunCommandAsync("docker", "compose stop jatetxea-web jatetxea-kds-tv jatetxea-odoo", dockerDir);
    steps.Add(new { Phase = 1, Name = "Frontendak Gelditu (Web, KDS TV, Odoo)", Output = step1 });

    // 2. URRATSA: Backend API eta Txata itzali
    var step2 = await RunCommandAsync("docker", "compose stop jatetxea-api jatetxea-chat", dockerDir);
    steps.Add(new { Phase = 2, Name = "Backend Zerbitzuak Gelditu (API, Chat)", Output = step2 });

    // 3. URRATSA: Datu-baseak segurtasunez ixtea (datu ustelkeria ekiditeko)
    var step3 = await RunCommandAsync("docker", "compose stop jatetxea-db db-odoo", dockerDir);
    steps.Add(new { Phase = 3, Name = "Datu-Baseak Seguru Itzali (MySQL, Postgres)", Output = step3 });

    return Results.Ok(new { Success = true, Message = "Gelditze koordinatua amaituta.", Steps = steps });
});

app.MapGet("/api/docker/logs/{service}", async (string service) =>
{
    var outText = await RunCommandAsync("docker", $"compose logs --tail=60 {service}", dockerDir);
    return Results.Ok(new { Service = service, Logs = outText });
});

// ---------------------------------------------------------------------------
// 3. VIRTUALBOX 3 MAKINA BIRTUALEEN APIA
// ---------------------------------------------------------------------------
app.MapGet("/api/vbox/status", async () =>
{
    if (!File.Exists(vboxPath))
    {
        return Results.Ok(new { Success = false, Installed = false, Message = "VBoxManage.exe ez da aurkitu sisteman." });
    }

    try
    {
        var runningOut = await RunCommandAsync(vboxPath, "list runningvms", vmDir);
        var expectedVms = new[] { "Erronka-OPNsense", "Erronka-WinServer2025", "Erronka-Ubuntu" };

        var vms = expectedVms.Select(name => new
        {
            Name = name,
            Running = runningOut.Contains($"\"{name}\""),
            Status = runningOut.Contains($"\"{name}\"") ? "Running" : "Stopped"
        }).ToList();

        return Results.Ok(new { Success = true, Installed = true, Vms = vms });
    }
    catch (Exception ex)
    {
        return Results.Ok(new { Success = false, Error = ex.Message });
    }
});

app.MapPost("/api/vbox/start-all", async (HttpContext ctx) =>
{
    string user = ctx.Request.Headers["X-Admin-User"].FirstOrDefault() ?? "Admin";
    LogAction(user, "VBOX_START_ALL", "VirtualBox 3 makinak headless moduan abiarazten");

    string scriptPath = Path.Combine(vmDir, "hasi_makinak_headless.ps1");
    if (File.Exists(scriptPath))
    {
        _ = Task.Run(() => RunCommandAsync("powershell", $"-ExecutionPolicy Bypass -File \"{scriptPath}\"", vmDir));
        return Results.Ok(new { Success = true, Message = "Makinak atzeko planoan abiarazten ari dira." });
    }

    string[] vms = { "Erronka-Ubuntu", "Erronka-OPNsense", "Erronka-WinServer2025" };
    foreach (var vm in vms)
    {
        _ = RunCommandAsync(vboxPath, $"startvm \"{vm}\" --type headless", vmDir);
    }
    return Results.Ok(new { Success = true, Message = "VBoxManage bidez abiarazten (headless)." });
});

// VirtualBox makinak pantailarekin (GUI / Normal) abiarazteko endpoint berria
app.MapPost("/api/vbox/start-all-gui", async (HttpContext ctx) =>
{
    string user = ctx.Request.Headers["X-Admin-User"].FirstOrDefault() ?? "Admin";
    LogAction(user, "VBOX_START_ALL_GUI", "VirtualBox 3 makinak pantailarekin (GUI) abiarazten");

    string[] vms = { "Erronka-OPNsense", "Erronka-WinServer2025", "Erronka-Ubuntu" };
    foreach (var vm in vms)
    {
        _ = RunCommandAsync(vboxPath, $"startvm \"{vm}\" --type gui", vmDir);
    }
    return Results.Ok(new { Success = true, Message = "VirtualBox makinak pantaila irekiaz abiarazten ari dira." });
});

// Banakako makina birtualen kontrola (GUI ireki, Headless, ACPI Gelditu, Reset, Pause, Resume)
app.MapPost("/api/vbox/action/{vm}/{action}", async (string vm, string action, HttpContext ctx) =>
{
    string user = ctx.Request.Headers["X-Admin-User"].FirstOrDefault() ?? "Admin";
    action = action.ToLowerInvariant();
    LogAction(user, $"VBOX_{action.ToUpper()}", $"Makina: {vm}");

    string result = action switch
    {
        "start-gui" => await RunCommandAsync(vboxPath, $"startvm \"{vm}\" --type gui", vmDir),
        "start-headless" => await RunCommandAsync(vboxPath, $"startvm \"{vm}\" --type headless", vmDir),
        "stop" => await RunCommandAsync(vboxPath, $"controlvm \"{vm}\" acpipowerbutton", vmDir),
        "reset" => await RunCommandAsync(vboxPath, $"controlvm \"{vm}\" reset", vmDir),
        "pause" => await RunCommandAsync(vboxPath, $"controlvm \"{vm}\" pause", vmDir),
        "resume" => await RunCommandAsync(vboxPath, $"controlvm \"{vm}\" resume", vmDir),
        "poweroff" => await RunCommandAsync(vboxPath, $"controlvm \"{vm}\" poweroff", vmDir),
        _ => "Ekintza ezezaguna"
    };

    return Results.Ok(new { Success = true, Vm = vm, Action = action, Output = result });
});

app.MapPost("/api/vbox/stop-all", async (HttpContext ctx) =>
{
    string user = ctx.Request.Headers["X-Admin-User"].FirstOrDefault() ?? "Admin";
    LogAction(user, "VBOX_STOP_ALL", "VirtualBox makinak segurtasunez gelditzen (ACPI)");

    string[] vms = { "Erronka-WinServer2025", "Erronka-Ubuntu", "Erronka-OPNsense" };
    foreach (var vm in vms)
    {
        _ = RunCommandAsync(vboxPath, $"controlvm \"{vm}\" acpipowerbutton", vmDir);
    }
    return Results.Ok(new { Success = true, Message = "Itzaltze seinalea (ACPI) bidalita makina guztiei." });
});

// ---------------------------------------------------------------------------
// 3.1 APLIKAZIOAK EXEKUTATZEKO / IREKITZEKO ENDPOINT-A (WEB & MAHAIGAINA)
// ---------------------------------------------------------------------------
app.MapPost("/api/apps/launch/{appId}", async (string appId, HttpContext ctx) =>
{
    string user = ctx.Request.Headers["X-Admin-User"].FirstOrDefault() ?? "Admin";
    LogAction(user, "APP_LAUNCH", $"Aplikazioa exekutatzen: {appId}");

    string agentsDir = Path.Combine(projectRoot, "1T_Jatetxea_erronka_agents");

    switch (appId.ToLowerInvariant())
    {
        case "web":
        case "jatetxea-web":
        case "web_php":
            OpenSystemBrowser("http://localhost:8085");
            return Results.Ok(new { Success = true, IsUrl = true, Url = "http://localhost:8085", Message = "Web PHP bezeroa irekita (8085)." });

        case "odoo":
        case "jatetxea-odoo":
        case "odoo_erp":
            OpenSystemBrowser("http://localhost:8070");
            return Results.Ok(new { Success = true, IsUrl = true, Url = "http://localhost:8070", Message = "1T Jatetxea Odoo 17 ERP irekita (8070)." });

        case "kds_tv":
        case "jatetxea-kds-tv":
            OpenSystemBrowser("http://localhost:8086");
            return Results.Ok(new { Success = true, IsUrl = true, Url = "http://localhost:8086", Message = "Sukaldeko KDS TV pantaila irekita (8086)." });

        case "android_tv":
        case "kds_android":
            string androidPath = Path.Combine(projectRoot, "1T_Jatetxea_erronka_agents", "sukaldea_kds_android_tv");
            Process.Start(new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = androidPath,
                UseShellExecute = true
            });
            return Results.Ok(new { Success = true, Message = "Sukaldea Android TV (Jetpack Compose) proiektua irekita." });

        case "api":
        case "jatetxea-api":
        case "swagger":
            OpenSystemBrowser("http://localhost:5050/");
            return Results.Ok(new { Success = true, IsUrl = true, Url = "http://localhost:5050/", Message = "REST API nagusia irekita (5050)." });

        case "chat":
        case "jatetxea-chat":
            OpenSystemBrowser("http://localhost:3001");
            return Results.Ok(new { Success = true, IsUrl = true, Url = "http://localhost:3001", Message = "Barne Txat zerbitzaria irekita (3001)." });

        case "phpmyadmin":
        case "mysql_web":
        case "mysql":
            _ = Task.Run(async () =>
            {
                await RunCommandAsync("docker", "compose up -d jatetxea-db phpmyadmin", dockerDir);
                await Task.Delay(500);
                OpenSystemBrowser("http://localhost:8081");
            });
            return Results.Ok(new { Success = true, IsUrl = true, Url = "http://localhost:8081", Message = "phpMyAdmin MySQL kudeatzailea abiarazten eta irekitzen (8081)..." });

        case "adminer":
        case "postgres_web":
        case "postgres":
            _ = Task.Run(async () =>
            {
                await RunCommandAsync("docker", "compose up -d db-odoo adminer", dockerDir);
                await Task.Delay(500);
                OpenSystemBrowser("http://localhost:8082");
            });
            return Results.Ok(new { Success = true, IsUrl = true, Url = "http://localhost:8082", Message = "Adminer PostgreSQL kudeatzailea abiarazten eta irekitzen (8082)..." });

        case "opnsense":
        case "opnsense_web":
            OpenSystemBrowser("https://localhost:8443");
            return Results.Ok(new { Success = true, IsUrl = true, Url = "https://localhost:8443", Message = "OPNsense WebGUI irekita (8443)." });

        case "csharp":
        case "desktop_csharp":
        case "tpv":
            string csharpProject = Path.Combine(agentsDir, "desktop_csharp", "Jatetxea.Desktop.csproj");
            if (File.Exists(csharpProject))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"run --project \"{csharpProject}\"",
                    WorkingDirectory = Path.GetDirectoryName(csharpProject),
                    UseShellExecute = true
                });
                return Results.Ok(new { Success = true, IsUrl = false, Message = "C# TPV Mahaigaineko aplikazioa abiarazten..." });
            }
            return Results.Ok(new { Success = false, Message = "Jatetxea.Desktop.csproj ez da aurkitu." });

        case "javafx":
        case "desktop_javafx":
        case "kds_javafx":
            string javafxDir = Path.Combine(agentsDir, "desktop_javafx");
            string mvnwCmd = Path.Combine(javafxDir, "mvnw.cmd");
            if (File.Exists(mvnwCmd))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c mvnw.cmd javafx:run",
                    WorkingDirectory = javafxDir,
                    UseShellExecute = true
                });
                return Results.Ok(new { Success = true, IsUrl = false, Message = "JavaFX KDS Sukaldea aplikazioa abiarazten..." });
            }
            return Results.Ok(new { Success = false, Message = "JavaFX proiektua ez da aurkitu." });

        default:
            return Results.BadRequest(new { Success = false, Message = $"Aplikazio ezezaguna: {appId}" });
    }
});

// ---------------------------------------------------------------------------
// 4. ZERBITZUEN KATALOGOA, UBIKAZIOA, FLUXUAK ETA SEKURITATEA
// ---------------------------------------------------------------------------
var zerbitzuenKatalogoa = new[]
{
    new ServiceDefinition(
        Id: "mysql",
        Izena: "MySQL Datu-Basea",
        Mota: "Datu-Base Nagusia",
        Host: "127.0.0.1",
        Port: 3308,
        Makina: "Erronka-WinServer2025 / Docker Portatila",
        Gunea: "LAN Barnea",
        Vlan: "VLAN 10",
        DockerService: "jatetxea-db",
        Fluxua: "ASP.NET Core APIak (portu 5000) eta Odoo sistemak kontsultatzen dute TCP/IP socket bidez (portu 3306 barnean, 3308 kanpoan). Bezeroek eta Web publikoak EZIN dute zuzenean DB kontsultatu.",
        Segurtasuna: "Erabiltzaile eta pasahitz zifratua. Zero-Trust printzipioa: DMZtik konexio zuzenak blokeatuta OPNsense suebakian. Soilik LANeko APIak du sarbidea.",
        Protokoloa: "MySQL Protocol (TCP / SSL gaitua)",
        HealthCheck: "SELECT 1 / TCP Port 3308",
        AppUrl: "http://localhost:8081",
        AppLauncherId: "phpmyadmin"
    ),
    new ServiceDefinition(
        Id: "api",
        Izena: "ASP.NET Core REST API",
        Mota: "Backend Zentrala",
        Host: "127.0.0.1",
        Port: 5050,
        Makina: "Erronka-WinServer2025 / Docker Portatila",
        Gunea: "LAN Barnea",
        Vlan: "VLAN 10",
        DockerService: "jatetxea-api",
        Fluxua: "Bezero guztiek (Web PHP, C# TPV, JavaFX KDS, Flutter mugikorrak) hemen zentralizatzen dute negozio logika. HTTP/JSON REST APIa. Sukaldeko egoerak WebSocket/Polling bidez hedatzen ditu.",
        Segurtasuna: "JWT Bearer Token autentifikazioa. HTTPS / TLS 1.3 euskarria. Swagger dokumentazioa babestua. CORS politika baimendutako jatorriekin soilik.",
        Protokoloa: "HTTP/1.1 eta HTTP/2 (REST JSON)",
        HealthCheck: "GET http://localhost:5050/",
        AppUrl: "http://localhost:5050/",
        AppLauncherId: "api"
    ),
    new ServiceDefinition(
        Id: "web",
        Izena: "Web PHP Frontend",
        Mota: "Frontend Publikoa",
        Host: "127.0.0.1",
        Port: 8085,
        Makina: "Erronka-Ubuntu / Docker Portatila",
        Gunea: "DMZ Publikoa",
        Vlan: "VLAN 20",
        DockerService: "jatetxea-web",
        Fluxua: "Kanpoko bezeroek Internetetik ikusten dute (Apache 2.4 + PHP 8.2). Karta, ordutegiak eta erreserbak kontsultatzen ditu. Barneko APIarekin komunikatzen da (DMZ -> LAN baimenduta portu 5000ra).",
        Segurtasuna: "DMZ gune isolatuan kokatua. Apache segurtasun moduluak (mod_security, headers). Ezin du LANeko beste zerbitzaririk ukitu, soilik APIaren 5000 ataka.",
        Protokoloa: "HTTP / HTTPS (Apache 2.4 MPM Event)",
        HealthCheck: "GET http://localhost:8085",
        AppUrl: "http://localhost:8085",
        AppLauncherId: "web"
    ),
    new ServiceDefinition(
        Id: "chat",
        Izena: "Barne Txat Zerbitzaria",
        Mota: "Denbora Errealeko Komunikazioa",
        Host: "127.0.0.1",
        Port: 3001,
        Makina: "Erronka-WinServer2025 / Docker Portatila",
        Gunea: "LAN Barnea",
        Vlan: "VLAN 10",
        DockerService: "jatetxea-chat",
        Fluxua: "Zerbitzarien, sukaldearen eta kudeatzaileen arteko mezu azkarrak trukatzeko (WebSocket bidez). Eskaeren aldaketak berehala jakinarazten ditu.",
        Segurtasuna: "WebSocket bidezko autentifikazioa (Handshake tokena). Barne LAN sarera soilik mugatua.",
        Protokoloa: "WebSocket / Socket.IO (TCP 3001)",
        HealthCheck: "GET http://localhost:3001/socket.io/",
        AppUrl: "http://localhost:3001",
        AppLauncherId: "chat"
    ),
    new ServiceDefinition(
        Id: "kds_tv",
        Izena: "Sukaldea KDS TV",
        Mota: "Pantaila / Bistaratzailea",
        Host: "127.0.0.1",
        Port: 8086,
        Makina: "Sukaldeko Android TV / Nginx Portatila",
        Gunea: "LAN Barnea",
        Vlan: "VLAN 10",
        DockerService: "jatetxea-kds-tv",
        Fluxua: "Sukaldeko hormako telebistan plateren prestaketa egoerak erakusten ditu. Nginx zerbitzariak HTML5/JS interfaze arina zerbitzatzen dio Android TV Box-ari.",
        Segurtasuna: "Irakurketa soileko interfazea (Read-Only KDS). LAN barruko IPei soilik baimendua.",
        Protokoloa: "HTTP (Nginx Alpine)",
        HealthCheck: "GET http://localhost:8086",
        AppUrl: "http://localhost:8086",
        AppLauncherId: "kds_tv"
    ),
    new ServiceDefinition(
        Id: "odoo",
        Izena: "1T Jatetxea Odoo 17 ERP",
        Mota: "Enpresa Kudeaketa / HR ERP",
        Host: "127.0.0.1",
        Port: 8070,
        Makina: "Erronka-WinServer2025 / Docker Portatila",
        Gunea: "LAN Barnea",
        Vlan: "VLAN 10",
        DockerService: "jatetxea-odoo",
        Fluxua: "1T Jatetxearen ERP instantzia ofiziala Docker bidez hedatua (Portu 8070). Langileen kudeaketa osoa, fitxaketak, bajak eta kontratuak zentralizatzen ditu PostgreSQL laguntzailearekin.",
        Segurtasuna: "Odoo Erabiltzaile eta Rol hierarkia (RBAC). Sesio kudeaketa sendoa (Cookie HttpOnly + Secure).",
        Protokoloa: "HTTP / Werkzeug Python 3.11",
        HealthCheck: "GET http://localhost:8070/web/login",
        AppUrl: "http://localhost:8070",
        AppLauncherId: "odoo"
    ),
    new ServiceDefinition(
        Id: "postgres",
        Izena: "PostgreSQL Odoo DB",
        Mota: "Datu-Base Erlazionalea",
        Host: "127.0.0.1",
        Port: 5432,
        Makina: "Erronka-WinServer2025 / Docker Portatila",
        Gunea: "LAN Barnea",
        Vlan: "VLAN 10",
        DockerService: "db-odoo",
        Fluxua: "Odoo 17 ERP-aren transakzio eta taula guztiak gordetzen ditu (erabiltzaileak, HR, asistentziak).",
        Segurtasuna: "pg_hba.conf bidez Docker barneko sareari soilik baimendua. Kanpoko sarbide zuzena mugatua.",
        Protokoloa: "PostgreSQL Wire Protocol (TCP 5432)",
        HealthCheck: "TCP Port 5432",
        AppUrl: "http://localhost:8082",
        AppLauncherId: "adminer"
    ),
    new ServiceDefinition(
        Id: "opnsense_web",
        Izena: "OPNsense WebGUI",
        Mota: "Suebakiaren Kudeaketa",
        Host: "127.0.0.1",
        Port: 8443,
        Makina: "Erronka-OPNsense (Appliance)",
        Gunea: "Kudeaketa / Gateway",
        Vlan: "VLAN 99 / LAN",
        DockerService: null,
        Fluxua: "Suebakiaren arauak, VLAN trunking-a, NAT bideraketak eta Zero-Trust trafikoa kudeatzeko interfaze administratiboa.",
        Segurtasuna: "HTTPS enkriptatua (TLS 1.3). Bi faktoreko autentifikazioa (2FA) eta brute-force blokeoa.",
        Protokoloa: "HTTPS (Lighttpd / PHP)",
        HealthCheck: "GET https://127.0.0.1:8443",
        AppUrl: "https://localhost:8443",
        AppLauncherId: "opnsense_web"
    ),
    new ServiceDefinition(
        Id: "winserver_rdp",
        Izena: "WinServer2025 RDP",
        Mota: "Urruneko Mahaigaina",
        Host: "127.0.0.1",
        Port: 3391,
        Makina: "Erronka-WinServer2025",
        Gunea: "LAN Barnea",
        Vlan: "VLAN 10",
        DockerService: null,
        Fluxua: "Windows Server 2025 zerbitzariaren mahaigainera sartzeko administrazio konexioa (RDP).",
        Segurtasuna: "Network Level Authentication (NLA) aktibatuta. Portu ez-estandarra (3391) eskaneatze automatikoak ekiditeko.",
        Protokoloa: "RDP (Remote Desktop Protocol / TLS)",
        HealthCheck: "TCP Port 3391"
    ),
    new ServiceDefinition(
        Id: "ubuntu_ssh",
        Izena: "Ubuntu DMZ SSH",
        Mota: "Linux Urruneko Kontsola",
        Host: "127.0.0.1",
        Port: 2230,
        Makina: "Erronka-Ubuntu (DMZ)",
        Gunea: "DMZ Publikoa",
        Vlan: "VLAN 20",
        DockerService: null,
        Fluxua: "Apache eta DMZ zerbitzariaren terminalera sartzeko urruneko konexio segurua.",
        Segurtasuna: "SSH v2 protokoloa. Gako publiko/pribatu bidezko autentifikazio gomendatua. Root sarbide zuzena desgaitua.",
        Protokoloa: "SSH-2.0 (OpenSSH 9.x)",
        HealthCheck: "TCP Port 2230"
    ),
    new ServiceDefinition(
        Id: "opnsense_ssh",
        Izena: "OPNsense SSH",
        Mota: "Suebaki Urruneko Kontsola",
        Host: "127.0.0.1",
        Port: 2231,
        Makina: "Erronka-OPNsense",
        Gunea: "Kudeaketa / Gateway",
        Vlan: "VLAN 99",
        DockerService: null,
        Fluxua: "OPNsense BSD terminalera sarbide zuzena sare konfigurazio aurreratuak egiteko.",
        Segurtasuna: "Kudeaketa VLAN 99tik soilik baimendua ekoizpenean.",
        Protokoloa: "SSH-2.0 (OpenSSH FreeBSD)",
        HealthCheck: "TCP Port 2231"
    ),
    new ServiceDefinition(
        Id: "winserver_ssh",
        Izena: "WinServer SSH",
        Mota: "Windows Urruneko PowerShell",
        Host: "127.0.0.1",
        Port: 2232,
        Makina: "Erronka-WinServer2025",
        Gunea: "LAN Barnea",
        Vlan: "VLAN 10",
        DockerService: null,
        Fluxua: "PowerShell remoting eta OpenSSH zerbitzaria Windows Serverren.",
        Segurtasuna: "Active Directory kredentzialekin autentifikatua. Saio guztiak Windows Event Viewer-en erregistratuak.",
        Protokoloa: "SSH-2.0 (OpenSSH Windows)",
        HealthCheck: "TCP Port 2232"
    )
};

app.MapGet("/api/health/ports", async () =>
{
    var tasks = zerbitzuenKatalogoa.Select(async s =>
    {
        var (online, latency) = await TestPortWithLatencyAsync(s.Host, s.Port, 1000);
        return new
        {
            s.Id,
            s.Izena,
            s.Mota,
            Endpoint = $"{s.Host}:{s.Port}",
            s.Port,
            s.Makina,
            s.Gunea,
            s.Vlan,
            s.DockerService,
            s.AppUrl,
            s.AppLauncherId,
            s.Fluxua,
            s.Segurtasuna,
            s.Protokoloa,
            s.HealthCheck,
            Online = online,
            LatencyMs = latency,
            Egoera = online ? "Martxan (ONLINE)" : "Itzalita / Ez dago erantzunik"
        };
    });

    var results = await Task.WhenAll(tasks);
    return Results.Ok(results);
});

// Zerbitzu zehatz baten xehetasun osoak eskuratzeko
app.MapGet("/api/services/{id}", async (string id) =>
{
    var s = zerbitzuenKatalogoa.FirstOrDefault(x => x.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
    if (s == null) return Results.NotFound(new { Message = "Zerbitzua ez da aurkitu." });

    var (online, latency) = await TestPortWithLatencyAsync(s.Host, s.Port, 1000);

    return Results.Ok(new
    {
        s.Id,
        s.Izena,
        s.Mota,
        Endpoint = $"{s.Host}:{s.Port}",
        s.Port,
        s.Makina,
        s.Gunea,
        s.Vlan,
        s.DockerService,
        s.AppUrl,
        s.AppLauncherId,
        s.Fluxua,
        s.Segurtasuna,
        s.Protokoloa,
        s.HealthCheck,
        Online = online,
        LatencyMs = latency,
        Egoera = online ? "ONLINE" : "OFFLINE"
    });
});

// ---------------------------------------------------------------------------
// 5. SARE PROFILAK (WIFI ZENTRALITA - SWITCH-ENV ETA AUTO-DETEKZIOA)
// ---------------------------------------------------------------------------
string envJsonPath = Path.Combine(dockerDir, "config", "environments.json");

app.MapGet("/api/env/current", async () =>
{
    var (ssid, ip) = await DetectCurrentNetworkAsync();
    string activeProfile = "tolosako_etxea";
    var profilesDict = new Dictionary<string, object>();

    if (File.Exists(envJsonPath))
    {
        try
        {
            var json = await File.ReadAllTextAsync(envJsonPath);
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("active_profile", out var ap))
            {
                activeProfile = ap.GetString() ?? "tolosako_etxea";
            }
            if (doc.RootElement.TryGetProperty("profiles", out var prfs))
            {
                foreach (var prop in prfs.EnumerateObject())
                {
                    profilesDict[prop.Name] = JsonSerializer.Deserialize<object>(prop.Value.GetRawText())!;
                }
            }
        }
        catch { }
    }

    // Auto-detekzioa: bilatu profil bat SSID edo IParen arabera
    string? matchedProfile = null;
    if (!string.IsNullOrEmpty(ssid))
    {
        if (ssid.Contains("MIWIFI", StringComparison.OrdinalIgnoreCase)) matchedProfile = "tolosako_etxea";
        else if (ssid.Contains("Goierri", StringComparison.OrdinalIgnoreCase)) matchedProfile = "eskola";
        else if (ssid.Contains("Etxeko", StringComparison.OrdinalIgnoreCase)) matchedProfile = "etxean";
    }

    if (matchedProfile == null && !string.IsNullOrEmpty(ip))
    {
        if (ip.StartsWith("192.168.1.")) matchedProfile = "tolosako_etxea";
        else if (ip.StartsWith("10.0.") || ip.StartsWith("172.")) matchedProfile = "eskola";
        else if (ip.StartsWith("192.168.10.")) matchedProfile = "vbox_erronka";
    }

    return Results.Ok(new
    {
        ActiveProfile = activeProfile,
        DetectedSsid = string.IsNullOrEmpty(ssid) ? "Kablez / Ezezaguna" : ssid,
        DetectedIp = string.IsNullOrEmpty(ip) ? "127.0.0.1" : ip,
        MatchedProfile = matchedProfile ?? activeProfile,
        AutoDetected = matchedProfile != null,
        Profiles = profilesDict
    });
});

app.MapGet("/api/env/list", async () =>
{
    if (File.Exists(envJsonPath))
    {
        try
        {
            var json = await File.ReadAllTextAsync(envJsonPath);
            using var doc = JsonDocument.Parse(json);
            var list = new List<object>();
            if (doc.RootElement.TryGetProperty("profiles", out var prfs))
            {
                foreach (var prop in prfs.EnumerateObject())
                {
                    var val = prop.Value;
                    list.Add(new
                    {
                        Id = prop.Name,
                        Deskribapena = val.TryGetProperty("deskribapena", out var d) ? d.GetString() : prop.Name,
                        WifiSsid = val.TryGetProperty("wifi_ssid", out var w) ? w.GetString() : "",
                        ApiHost = val.TryGetProperty("api_host", out var h) ? h.GetString() : "127.0.0.1",
                        ApiUrl = val.TryGetProperty("api_base_url", out var u) ? u.GetString() : $"http://{h}:5000/api/v1"
                    });
                }
            }
            return Results.Ok(list);
        }
        catch { }
    }

    return Results.Ok(new[]
    {
        new { Id = "localhost",       Deskribapena = "Tokiko Garapena (127.0.0.1)",     WifiSsid = "",              ApiHost = "127.0.0.1",     ApiUrl = "http://localhost:5000/api/v1" },
        new { Id = "tolosako_etxea",  Deskribapena = "Tolosako Etxeko WiFi Sarea",     WifiSsid = "MIWIFI_dc7U",   ApiHost = "192.168.1.166", ApiUrl = "http://192.168.1.166:5000/api/v1" },
        new { Id = "eskola",          Deskribapena = "Goierri Eskolako Laborategia",   WifiSsid = "Goierri_Ikasleak", ApiHost = "10.0.0.50",   ApiUrl = "http://10.0.0.50:5000/api/v1" },
        new { Id = "etxean",          Deskribapena = "Etxeko WiFi Lokala",              WifiSsid = "Etxeko_WIFI",   ApiHost = "192.168.1.100", ApiUrl = "http://192.168.1.100:5000/api/v1" },
        new { Id = "vbox_erronka",    Deskribapena = "VirtualBox DMZ/LAN Barnea",       WifiSsid = "",              ApiHost = "192.168.10.10", ApiUrl = "http://192.168.10.10:5000/api/v1" }
    });
});

app.MapPost("/api/env/switch/{envId}", async (string envId, HttpContext ctx) =>
{
    string user = ctx.Request.Headers["X-Admin-User"].FirstOrDefault() ?? "Admin";
    LogAction(user, "SWITCH_ENV", $"Sare ingurunea aldatzen: {envId}");

    // environments.json eguneratu
    if (File.Exists(envJsonPath))
    {
        try
        {
            var json = await File.ReadAllTextAsync(envJsonPath);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            var dict = new Dictionary<string, object>();
            dict["active_profile"] = envId;

            if (root.TryGetProperty("profiles", out var prfs))
            {
                dict["profiles"] = JsonSerializer.Deserialize<object>(prfs.GetRawText())!;
            }
            await File.WriteAllTextAsync(envJsonPath, JsonSerializer.Serialize(dict, new JsonSerializerOptions { WriteIndented = true }));
        }
        catch { }
    }

    string scriptPath = Path.Combine(dockerDir, "scripts", "switch-env.ps1");
    if (File.Exists(scriptPath))
    {
        var outText = await RunCommandAsync("powershell", $"-ExecutionPolicy Bypass -File \"{scriptPath}\" -Environment {envId}", dockerDir);
        return Results.Ok(new { Success = true, Output = outText, Environment = envId });
    }

    return Results.Ok(new { Success = true, Output = $"Profila {envId} hautatu da.", Environment = envId });
});

// WIFI / SARE PROFIL BERRIA GEHITU
app.MapPost("/api/env/add", async (HttpContext ctx) =>
{
    string user = ctx.Request.Headers["X-Admin-User"].FirstOrDefault() ?? "Admin";
    using var reader = new StreamReader(ctx.Request.Body);
    var body = await reader.ReadToEndAsync();

    try
    {
        using var doc = JsonDocument.Parse(body);
        var root = doc.RootElement;
        string id = root.GetProperty("id").GetString()?.Trim().ToLowerInvariant() ?? "";
        string deskribapena = root.GetProperty("deskribapena").GetString()?.Trim() ?? id;
        string wifiSsid = root.TryGetProperty("wifi_ssid", out var ws) ? ws.GetString()?.Trim() ?? "" : "";
        string apiHost = root.GetProperty("api_host").GetString()?.Trim() ?? "127.0.0.1";
        int apiPort = root.TryGetProperty("api_port", out var ap) ? ap.GetInt32() : 5000;

        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(apiHost))
        {
            return Results.BadRequest(new { Success = false, Message = "Id eta API Host derrigorrezkoak dira." });
        }

        LogAction(user, "ADD_ENV_PROFILE", $"Profil berria gehitu da: {id} ({deskribapena}) - Host: {apiHost}");

        // environments.json irakurri eta eguneratu
        var dataDict = new Dictionary<string, object>();
        var profilesMap = new Dictionary<string, object>();

        if (File.Exists(envJsonPath))
        {
            var oldJson = await File.ReadAllTextAsync(envJsonPath);
            using var oldDoc = JsonDocument.Parse(oldJson);
            if (oldDoc.RootElement.TryGetProperty("active_profile", out var act))
            {
                dataDict["active_profile"] = act.GetString() ?? id;
            }
            if (oldDoc.RootElement.TryGetProperty("profiles", out var prfs))
            {
                foreach (var p in prfs.EnumerateObject())
                {
                    profilesMap[p.Name] = JsonSerializer.Deserialize<object>(p.Value.GetRawText())!;
                }
            }
        }

        profilesMap[id] = new
        {
            deskribapena,
            wifi_ssid = wifiSsid,
            api_host = apiHost,
            api_port = apiPort,
            api_base_url = $"http://{apiHost}:{apiPort}/api/v1",
            web_url = $"http://{apiHost}:8085",
            chat_url = $"http://{apiHost}:3001",
            odoo_url = $"http://{apiHost}:8070",
            tv_kds_url = $"http://{apiHost}:8086"
        };

        dataDict["profiles"] = profilesMap;
        await File.WriteAllTextAsync(envJsonPath, JsonSerializer.Serialize(dataDict, new JsonSerializerOptions { WriteIndented = true }));

        return Results.Ok(new { Success = true, Message = $"Profil berria '{id}' arrakastaz gorde da.", ProfileId = id });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { Success = false, Message = ex.Message });
    }
});

// ---------------------------------------------------------------------------
// 6. ADARKADURA ETA TOPOLOGIA ZUHAITZA (TREE VIEW API)
// ---------------------------------------------------------------------------
app.MapGet("/api/services/tree", async () =>
{
    var tasks = zerbitzuenKatalogoa.Select(async s =>
    {
        var (online, latency) = await TestPortWithLatencyAsync(s.Host, s.Port, 1000);
        return new
        {
            s.Id,
            s.Izena,
            s.Mota,
            Endpoint = $"{s.Host}:{s.Port}",
            s.Port,
            s.Makina,
            s.Gunea,
            s.Vlan,
            s.DockerService,
            Online = online,
            LatencyMs = latency
        };
    });

    var liveServices = (await Task.WhenAll(tasks)).ToDictionary(x => x.Id);

    // 5 Mailako Arkitektura Hierarkikoa (Datu-baseak -> API -> Makinak -> OPNsense -> Bezeroak)
    var tree = new
    {
        Tiers = new[]
        {
            new
            {
                TierNumber = 1,
                TierName = "1. MAILA: DATU-BASEAK (DATA STORAGE)",
                Description = "Datuak iraunkor mantentzen dituzten motorrak (LAN barnean isolatuta)",
                Color = "sky",
                Nodes = new object[]
                {
                    new { Id = "mysql", Name = "MySQL 8.0 (jatetxea_db)", Host = "WinServer2025 (LAN)", Port = 3308, Live = liveServices.GetValueOrDefault("mysql") },
                    new { Id = "postgres", Name = "PostgreSQL 15 (odoo_db)", Host = "WinServer2025 (LAN)", Port = 5432, Live = liveServices.GetValueOrDefault("postgres") }
                }
            },
            new
            {
                TierNumber = 2,
                TierName = "2. MAILA: BACKEND & NEGOZIO LOGIKA",
                Description = "Transakzioak, JWT segurtasuna eta mezu azkarrak bideratzen dituzten zerbitzuak",
                Color = "indigo",
                Nodes = new object[]
                {
                    new { Id = "api", Name = "ASP.NET Core REST API (.NET 10 - Portu 5050)", Host = "WinServer2025 (LAN)", Port = 5050, Live = liveServices.GetValueOrDefault("api"), ConnectsTo = new[] { "mysql" } },
                    new { Id = "chat", Name = "Barne Txat Zerbitzaria (Socket.IO)", Host = "WinServer2025 (LAN)", Port = 3001, Live = liveServices.GetValueOrDefault("chat"), ConnectsTo = new[] { "mysql" } }
                }
            },
            new
            {
                TierNumber = 3,
                TierName = "3. MAILA: ZERBITZARIAK & MAKINA BIRTUALEAK",
                Description = "Sistema eragileak eta ingurune segmentatuak (Active Directory, Web, ERP)",
                Color = "purple",
                Nodes = new object[]
                {
                    new { Id = "winserver", Name = "Erronka-WinServer2025 (LAN · 192.168.10.10)", Host = "LAN Gunea", Port = 3391, Live = (object?)liveServices.GetValueOrDefault("winserver_rdp"), SubServices = "AD, DNS, DHCP, MySQL, .NET API", ConnectsTo = Array.Empty<string>() },
                    new { Id = "ubuntu", Name = "Erronka-Ubuntu (DMZ · 192.168.20.10)", Host = "DMZ Gunea", Port = 2230, Live = (object?)liveServices.GetValueOrDefault("ubuntu_ssh"), SubServices = "Apache 2.4, PHP 8.2", ConnectsTo = Array.Empty<string>() },
                    new { Id = "odoo", Name = "1T Jatetxea Odoo 17 ERP (Portu 8070)", Host = "WinServer2025 (LAN)", Port = 8070, Live = (object?)liveServices.GetValueOrDefault("odoo"), SubServices = "HR, Fitxaketak, Bajak, Kontratuak", ConnectsTo = new[] { "postgres" } },
                    new { Id = "kds_tv", Name = "Sukaldeko KDS TV (Android TV Box)", Host = "Sukaldea (LAN)", Port = 8086, Live = (object?)liveServices.GetValueOrDefault("kds_tv"), SubServices = "Nginx HTML5 KDS", ConnectsTo = new[] { "api" } }
                }
            },
            new
            {
                TierNumber = 4,
                TierName = "4. MAILA: SUEBAKIA, ROUTERRA & SEGURTASUNA",
                Description = "OPNsense Router-on-a-Stick eta Cisco Catalyst 2960 Trunk 802.1Q (Zero-Trust)",
                Color = "rose",
                Nodes = new object[]
                {
                    new { Id = "opnsense", Name = "Erronka-OPNsense Firewall (Router-on-a-Stick)", Host = "Gateway 192.168.10.1", Port = 8443, Live = (object?)liveServices.GetValueOrDefault("opnsense_web"), Rules = "WAN -> DMZ (80/443 Pass), DMZ -> LAN (5000 Pass), DMZ -> DB (Block)" },
                    new { Id = "cisco_switch", Name = "Cisco Catalyst 2960 Kommutadorea", Host = "SVI 192.168.99.2", Port = 22, Live = (object?)new { Online = true, LatencyMs = 1 }, Rules = "VLAN 10 (LAN), VLAN 20 (DMZ), VLAN 30 (WiFi), VLAN 99 (Mgmt)" }
                }
            },
            new
            {
                TierNumber = 5,
                TierName = "5. MAILA: BEZERO APLIKAZIOAK & KANPOKO PUNTUAK",
                Description = "Langile eta bezeroen gailuak (Mahaiak, Kutxak, Sukaldea eta Karta Digitala)",
                Color = "emerald",
                Nodes = new object[]
                {
                    new { Id = "web_bezeroa", Name = "Web Publikoa / Karta (PHP 8.2)", Host = "DMZ (Port 8085)", Port = 8085, Live = (object?)liveServices.GetValueOrDefault("web"), ConnectsTo = new[] { "opnsense", "api" } },
                    new { Id = "tpv_csharp", Name = "Desktop TPV Kutxa (C# WinForms)", Host = "Barra (VLAN 10)", Port = 5000, Live = (object?)new { Online = true, LatencyMs = 1 }, ConnectsTo = new[] { "api" } },
                    new { Id = "javafx_kds", Name = "Sukaldea / Barra / Biltegia (JavaFX)", Host = "Sukaldea (VLAN 10)", Port = 5000, Live = (object?)new { Online = true, LatencyMs = 1 }, ConnectsTo = new[] { "api" } },
                    new { Id = "mobile_zerbitzaria", Name = "Mobile Zerbitzaria (Flutter PDA)", Host = "WiFi Langileak (VLAN 10)", Port = 5000, Live = (object?)new { Online = true, LatencyMs = 3 }, ConnectsTo = new[] { "api" } },
                    new { Id = "mobile_bezeroa", Name = "Mobile Bezeroa (Flutter App)", Host = "WiFi Bezeroak (VLAN 30)", Port = 5000, Live = (object?)new { Online = true, LatencyMs = 3 }, ConnectsTo = new[] { "opnsense", "api" } }
                }
            }
        }
    };

    return Results.Ok(tree);
});

// ---------------------------------------------------------------------------
// 6. INGURUNE FISIKOA ETA INBENTARIOA
// ---------------------------------------------------------------------------
app.MapGet("/api/lab/overview", () =>
{
    return Results.Ok(new
    {
        Topologia = "Router-on-a-Stick (OPNsense + Cisco Catalyst 2960 Trunk 802.1Q)",
        Vlanak = new[]
        {
            new { Vlan = 10, Izena = "LAN_JATETXEA", Subnet = "192.168.10.0/24", Ekipoak = "WinServer2025 (10.10), TPV Kutxa (10.30), KDS Sukaldea (10.40), Inprimagailua (10.50)" },
            new { Vlan = 20, Izena = "DMZ_WEB",      Subnet = "192.168.20.0/24", Ekipoak = "Ubuntu Apache 2.4 / PHP 8.2 (20.10)" },
            new { Vlan = 30, Izena = "WIFI_BEZEROAK",Subnet = "192.168.30.0/24", Ekipoak = "Bezeroen Mugikorrak (Erreserbak, Karta digitala, Ordainketak)" },
            new { Vlan = 99, Izena = "KUDEAKETA",    Subnet = "192.168.99.0/24", Ekipoak = "Switch SVI (99.2), OPNsense Gateway (99.1), Admin PC (99.10)" }
        },
        Hardwareak = new[]
        {
            new { Gailua = "Cisco Catalyst 2960-X", Rola = "Kommutadore Kudeatua (Trunk 802.1Q, Port-Security, DHCP Snooping)" },
            new { Gailua = "OPNsense Mini PC",      Rola = "Suebaki / Router nagusia (Zero-Trust segmentazioa)" },
            new { Gailua = "WiFi Access Point",     Rola = "Multi-SSID (Langileak VLAN 10, Bezeroak VLAN 30)" },
            new { Gailua = "TPV Kutxa + Printer",   Rola = "Windows 11 + 80mm ESC/POS Ticket Inprimagailu Termikoa" },
            new { Gailua = "Samsung PDA / Tableta", Rola = "Zerbitzarien eta Bezeroen Flutter aplikazioak (ADB)" }
        }
    });
});

app.MapPost("/api/lab/ping", async (HttpContext ctx) =>
{
    using var reader = new StreamReader(ctx.Request.Body);
    var body = await reader.ReadToEndAsync();
    using var doc = JsonDocument.Parse(string.IsNullOrWhiteSpace(body) ? "{}" : body);
    string host = doc.RootElement.TryGetProperty("host", out var h) ? h.GetString() ?? "127.0.0.1" : "127.0.0.1";

    try
    {
        using var ping = new Ping();
        var reply = await ping.SendPingAsync(host, 1000);
        return Results.Ok(new
        {
            Host = host,
            Status = reply.Status.ToString(),
            RoundtripTime = reply.RoundtripTime,
            Success = reply.Status == IPStatus.Success
        });
    }
    catch (Exception ex)
    {
        return Results.Ok(new { Host = host, Success = false, Error = ex.Message });
    }
});

app.Run();

// Laguntza-funtzioak
static void OpenSystemBrowser(string url)
{
    try
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }
    catch { }
}

static async Task<string> RunCommandAsync(string fileName, string args, string workingDir)
{
    var psi = new ProcessStartInfo
    {
        FileName = fileName,
        Arguments = args,
        WorkingDirectory = Directory.Exists(workingDir) ? workingDir : Environment.CurrentDirectory,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true
    };

    try
    {
        using var process = new Process { StartInfo = psi };
        process.Start();

        var outTask = process.StandardOutput.ReadToEndAsync();
        var errTask = process.StandardError.ReadToEndAsync();

        await Task.WhenAny(process.WaitForExitAsync(), Task.Delay(20000));

        string output = await outTask;
        string error = await errTask;

        return string.IsNullOrWhiteSpace(error) ? output : $"{output}\n[ERROREA]: {error}";
    }
    catch (Exception ex)
    {
        return $"[EXZEPZIOA]: {ex.Message}";
    }
}

static async Task<(bool Online, long LatencyMs)> TestPortWithLatencyAsync(string host, int port, int timeoutMs)
{
    var sw = Stopwatch.StartNew();
    try
    {
        using var client = new TcpClient();
        var connectTask = client.ConnectAsync(host, port);
        var timeoutTask = Task.Delay(timeoutMs);

        var completed = await Task.WhenAny(connectTask, timeoutTask);
        sw.Stop();

        if (completed == connectTask && client.Connected)
        {
            return (true, sw.ElapsedMilliseconds);
        }
        return (false, 0);
    }
    catch
    {
        sw.Stop();
        return (false, 0);
    }
}

static async Task<(string Ssid, string Ip)> DetectCurrentNetworkAsync()
{
    string ssid = "";
    string ip = "";
    try
    {
        var netshOut = await RunCommandAsync("netsh", "wlan show interfaces", ".");
        foreach (var line in netshOut.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
        {
            var trimmed = line.Trim();
            if (trimmed.StartsWith("SSID") && !trimmed.StartsWith("SSID de BSSID"))
            {
                var parts = trimmed.Split(new[] { ':' }, 2);
                if (parts.Length > 1) ssid = parts[1].Trim();
            }
        }
    }
    catch { }

    try
    {
        var nics = NetworkInterface.GetAllNetworkInterfaces()
            .Where(n => n.OperationalStatus == OperationalStatus.Up &&
                        n.NetworkInterfaceType != NetworkInterfaceType.Loopback)
            .OrderByDescending(n => n.NetworkInterfaceType == NetworkInterfaceType.Wireless80211);

        foreach (var nic in nics)
        {
            var ipProps = nic.GetIPProperties();
            var unicast = ipProps.UnicastAddresses
                .FirstOrDefault(u => u.Address.AddressFamily == AddressFamily.InterNetwork &&
                                     !u.Address.ToString().StartsWith("169.254") &&
                                     !u.Address.ToString().StartsWith("127."));
            if (unicast != null)
            {
                ip = unicast.Address.ToString();
                break;
            }
        }
    }
    catch { }

    return (ssid, ip);
}

record AuditLogItem(string Time, string User, string Action, string Details);

record ServiceDefinition(
    string Id,
    string Izena,
    string Mota,
    string Host,
    int Port,
    string Makina,
    string Gunea,
    string Vlan,
    string? DockerService,
    string Fluxua,
    string Segurtasuna,
    string Protokoloa,
    string HealthCheck,
    string? AppUrl = null,
    string? AppLauncherId = null
);
