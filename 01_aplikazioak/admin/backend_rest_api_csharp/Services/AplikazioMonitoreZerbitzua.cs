using System.Collections.Concurrent;

namespace Jatetxea.Api.Services;

/// <summary>
/// APIra konektatzen diren bezero/aplikazio bakoitzaren egoera erregistratzen duen eredua.
/// </summary>
public class KonektatutakoAppInfo
{
    public string Id { get; set; } = string.Empty;
    public string Izena { get; set; } = string.Empty;
    public string Mota { get; set; } = string.Empty;
    public string Rola { get; set; } = string.Empty; // "Admin", "Jangela", "Barra", "Sukaldea", "Biltokia", "Kanpokoa / Web Publikoa"
    public string IpHelbidea { get; set; } = "-";
    public DateTime AzkenKonexioa { get; set; } = DateTime.MinValue;
    public string AzkenEskaera { get; set; } = "-";
    public int EskaerakGuztira { get; set; } = 0;

    /// <summary>
    /// Aplikazio honetan saioa hasi edo eskaerak egin dituzten erabiltzaileen zerrenda eta azken ordua.
    /// </summary>
    public ConcurrentDictionary<string, DateTime> Erabiltzaileak { get; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Azken 35 segundoetan eskaeraren bat egin badu, aplikazioa AKTIBO dagoela kontsideratzen da.
    /// </summary>
    public bool AktiboDago => AzkenKonexioa != DateTime.MinValue && (DateTime.UtcNow - AzkenKonexioa).TotalSeconds <= 35;

    public string Egoera
    {
        get
        {
            if (AzkenKonexioa == DateTime.MinValue) return "DESKONEKTATUTA";
            double seconds = (DateTime.UtcNow - AzkenKonexioa).TotalSeconds;
            if (seconds <= 35) return "AKTIBO";
            if (seconds <= 90) return "ISILIK";
            return "DESKONEKTATUTA";
        }
    }

    public string AzkenOrduaFormatua
    {
        get
        {
            if (AzkenKonexioa == DateTime.MinValue) return "Ez da konektatu";
            return Jatetxea.Api.Helpers.OrduHelper.ToEuskalHerriaOrdua(AzkenKonexioa);
        }
    }

    /// <summary>
    /// Azken 'segundoMuga' segundoetan jarduera izan duten erabiltzaile aktiboak itzultzen ditu (lehenetsia: 300 s / 5 min).
    /// </summary>
    public List<string> LortuErabiltzaileAktiboak(int segundoMuga = 300)
    {
        var muga = DateTime.UtcNow.AddSeconds(-segundoMuga);
        return Erabiltzaileak
            .Where(kv => kv.Value >= muga)
            .OrderByDescending(kv => kv.Value)
            .Select(kv => kv.Key)
            .ToList();
    }

    public string FormatErabiltzaileakTestua(int segundoMuga = 300)
    {
        var aktiboak = LortuErabiltzaileAktiboak(segundoMuga);
        if (aktiboak.Count == 0) return string.Empty;
        if (aktiboak.Count == 1) return $"Erabiltzailea: {aktiboak[0]}";
        return $"Erabiltzaileak: {string.Join(", ", aktiboak)} ({aktiboak.Count} langile)";
    }
}

/// <summary>
/// REST APIari eskaerak egiten dizkioten bezeroak (Admin, Jangela, Barra, Sukaldea, Biltokia)
/// denbora errealean identifikatu, rol bidez sailkatu, konektatutako erabiltzaileak zenbatu eta monitorizatzen dituen zerbitzu zentrala.
/// </summary>
public static class AplikazioMonitoreZerbitzua
{
    private static readonly ConcurrentDictionary<string, KonektatutakoAppInfo> Apps = new();

    private static readonly string[] RolakOrdena = 
    { 
        "Admin", 
        "Jangela", 
        "Barra", 
        "Sukaldea", 
        "Biltokia", 
        "Kanpokoa / Web Publikoa" 
    };

    static AplikazioMonitoreZerbitzua()
    {
        HasieratuAppEzagunak();
    }

    private static void HasieratuAppEzagunak()
    {
        // 1. Admin
        ErregistratuAplikazioEredua("admin_it", "Admin IT Zentrala (Portu 5000)", "Web Panela (ASP.NET Core)", "Admin");
        ErregistratuAplikazioEredua("desktop_admin_kudeaketa", "Admin JavaFX Kudeaketa Desktop", "Desktop Java (JavaFX 21)", "Admin");

        // 2. Jangela
        ErregistratuAplikazioEredua("csharp_tpv", "C# TPV Areto & Kutxazaina", "Desktop C# (Windows Forms)", "Jangela");
        ErregistratuAplikazioEredua("android_komandera", "Android Zerbitzaria (Komandera)", "Mugikorra (Jetpack Compose)", "Jangela");
        ErregistratuAplikazioEredua("android_bezeroak", "Android Bezero Aplikazioa", "Mugikorra (Jetpack Compose)", "Jangela");

        // 3. Barra
        ErregistratuAplikazioEredua("desktop_barra_bds", "JavaFX Barra BDS", "Desktop Java (JavaFX 21)", "Barra");

        // 4. Sukaldea
        ErregistratuAplikazioEredua("desktop_sukaldea_kds", "JavaFX Sukaldea KDS", "Desktop Java (JavaFX 21)", "Sukaldea");
        ErregistratuAplikazioEredua("android_tv", "Android TV Sukaldea KDS", "Telebista (Compose TV)", "Sukaldea");

        // 5. Biltokia
        ErregistratuAplikazioEredua("desktop_biltegia_stock", "JavaFX Biltegia Stock", "Desktop Java (JavaFX 21)", "Biltokia");

        // 6. Kanpokoak / Web Publikoa
        ErregistratuAplikazioEredua("web_php", "Web PHP Frontend Publikoa", "Web Apache (PHP 8.4)", "Kanpokoa / Web Publikoa");
        ErregistratuAplikazioEredua("browser_client", "Web Nabigatzailea / Swagger UI", "Nabigatzailea", "Kanpokoa / Web Publikoa");
    }

    private static void ErregistratuAplikazioEredua(string id, string izena, string mota, string rola)
    {
        Apps.TryAdd(id, new KonektatutakoAppInfo
        {
            Id = id,
            Izena = izena,
            Mota = mota,
            Rola = rola,
            IpHelbidea = "-",
            AzkenKonexioa = DateTime.MinValue,
            AzkenEskaera = "-",
            EskaerakGuztira = 0
        });
    }

    /// <summary>
    /// Sarrerako HTTP eskaeratik aplikazioa identifikatzen du goiburu eta User-Agent bidez.
    /// </summary>
    public static string IdentifikatuAplikazioa(HttpContext context)
    {
        var headers = context.Request.Headers;

        // 1. Esplizituki X-App-Id goiburua bidali bada
        if (headers.TryGetValue("X-App-Id", out var appIdVal) && !string.IsNullOrWhiteSpace(appIdVal))
        {
            string id = appIdVal.ToString().ToLowerInvariant().Trim();
            if (id.Contains("biltegi") || id.Contains("stock")) return "desktop_biltegia_stock";
            if (id.Contains("barra") || id.Contains("bds")) return "desktop_barra_bds";
            if (id.Contains("sukalde") || id.Contains("kds")) return "desktop_sukaldea_kds";
            if (id.Contains("kudeaketa") || id.Contains("admin_javafx") || id.Contains("admin_desktop")) return "desktop_admin_kudeaketa";
            if (id.Contains("csharp") || id.Contains("tpv") || id.Contains("kutxazain")) return "csharp_tpv";
            if (id.Contains("android_komandera") || id.Contains("komandera_android") || id.Contains("android_zerbitzaria")) return "android_komandera";
            if (id.Contains("android_bezero") || id.Contains("bezeroak_android")) return "android_bezeroak";
            if (id.Contains("android_tv") || id.Contains("tv")) return "android_tv";
            if (id.Contains("web") || id.Contains("php")) return "web_php";
            if (id.Contains("admin")) return "admin_it";
            return id;
        }

        // 2. X-App-Name goiburua
        if (headers.TryGetValue("X-App-Name", out var appNameVal) && !string.IsNullOrWhiteSpace(appNameVal))
        {
            string name = appNameVal.ToString().ToLowerInvariant();
            if (name.Contains("biltegi") || name.Contains("stock")) return "desktop_biltegia_stock";
            if (name.Contains("barra") || name.Contains("bds")) return "desktop_barra_bds";
            if (name.Contains("sukalde") || name.Contains("kds")) return "desktop_sukaldea_kds";
            if (name.Contains("kudeaketa")) return "desktop_admin_kudeaketa";
            if (name.Contains("c#") || name.Contains("tpv") || name.Contains("kutxazain")) return "csharp_tpv";
            if (name.Contains("komandera") || name.Contains("zerbitzari")) return "android_komandera";
            if (name.Contains("bezero")) return "android_bezeroak";
            if (name.Contains("tv")) return "android_tv";
            if (name.Contains("admin")) return "admin_it";
            if (name.Contains("php") || name.Contains("web")) return "web_php";
        }

        // 3. User-Agent bidezko detekzio automatikoa
        string userAgent = headers.UserAgent.ToString().ToLowerInvariant();
        string path = context.Request.Path.Value?.ToLowerInvariant() ?? "";

        if (userAgent.Contains("android-komandera") || userAgent.Contains("jatetxea.komandera") || userAgent.Contains("android_komandera"))
            return "android_komandera";

        if (userAgent.Contains("android-bezero") || userAgent.Contains("jatetxea.bezeroak"))
            return "android_bezeroak";

        if (userAgent.Contains("android-tv") || userAgent.Contains("sukaldea-tv"))
            return "android_tv";

        if (userAgent.Contains("jatetxea.desktop") || userAgent.Contains("csharp") || userAgent.Contains("windowsforms"))
            return "csharp_tpv";

        if (userAgent.Contains("kudeaketa"))
            return "desktop_admin_kudeaketa";

        if (userAgent.Contains("javafx") || userAgent.Contains("java-http-client") || userAgent.Contains("java/"))
        {
            if (path.Contains("lehengai") || path.Contains("stock")) return "desktop_biltegia_stock";
            if (path.Contains("barra")) return "desktop_barra_bds";
            if (path.Contains("langileak/sortu") || path.Contains("langileak/eguneratu")) return "desktop_admin_kudeaketa";
            return "desktop_sukaldea_kds";
        }

        if (userAgent.Contains("php") || userAgent.Contains("guzzle") || userAgent.Contains("jatetxea-web-php"))
            return "web_php";

        if (userAgent.Contains("adminzentrala") || headers.ContainsKey("X-Admin-User"))
            return "admin_it";

        // 4. Referer bidezko detekzioa
        string referer = headers.Referer.ToString().ToLowerInvariant();
        if (referer.Contains(":8085")) return "web_php";
        if (referer.Contains(":8086")) return "android_tv";
        if (referer.Contains(":5000")) return "admin_it";

        // 5. Nabigatzaile arrunta / Swagger
        if (userAgent.Contains("mozilla") || userAgent.Contains("chrome") || userAgent.Contains("safari"))
            return "browser_client";

        return "bezero_ezezaguna";
    }

    /// <summary>
    /// Eskaera berria erregistratzen du eta kontsolan denbora errealeko mezua inprimatzen du.
    /// </summary>
    public static void ErregistratuEskaera(string appId, string ipHelbidea, string eskaeraBidea, string? erabiltzailea = null)
    {
        if (string.IsNullOrWhiteSpace(appId)) return;

        var app = Apps.GetOrAdd(appId, id => new KonektatutakoAppInfo
        {
            Id = id,
            Izena = FormatAppIzena(id),
            Mota = "Kanpoko Bezeroa",
            Rola = FormatAppRola(id),
            IpHelbidea = ipHelbidea,
            AzkenKonexioa = DateTime.UtcNow,
            AzkenEskaera = eskaeraBidea,
            EskaerakGuztira = 1
        });

        lock (app)
        {
            app.IpHelbidea = ipHelbidea;
            app.AzkenKonexioa = DateTime.UtcNow;
            app.AzkenEskaera = eskaeraBidea;
            app.EskaerakGuztira++;

            if (!string.IsNullOrWhiteSpace(erabiltzailea))
            {
                app.Erabiltzaileak[erabiltzailea.Trim()] = DateTime.UtcNow;
            }
        }

        string ordua = Jatetxea.Api.Helpers.OrduHelper.NowFormatted;
        string erabLog = !string.IsNullOrWhiteSpace(erabiltzailea) ? $" [Erabiltzailea: {erabiltzailea.Trim()}]" : "";
        Console.WriteLine($"[{ordua}] [APP: {app.Izena}]{erabLog} -> {eskaeraBidea} (IP: {ipHelbidea})");
    }

    /// <summary>
    /// Erabiltzaile baten saio-hasiera zuzena aplikazio zehatz baten azpian erregistratzen du.
    /// </summary>
    public static void ErregistratuErabiltzailea(string appId, string erabiltzaileIzena)
    {
        if (string.IsNullOrWhiteSpace(appId) || string.IsNullOrWhiteSpace(erabiltzaileIzena)) return;

        if (Apps.TryGetValue(appId, out var app))
        {
            app.Erabiltzaileak[erabiltzaileIzena.Trim()] = DateTime.UtcNow;
        }
    }

    private static string FormatAppIzena(string id) => id switch
    {
        "admin_it" => "Admin IT Zentrala (Portu 5000)",
        "desktop_admin_kudeaketa" => "Admin JavaFX Kudeaketa Desktop",
        "csharp_tpv" => "C# TPV Areto & Kutxazaina",
        "android_komandera" => "Android Zerbitzaria (Komandera)",
        "android_bezeroak" => "Android Bezero Aplikazioa",
        "desktop_barra_bds" => "JavaFX Barra BDS",
        "desktop_sukaldea_kds" => "JavaFX Sukaldea KDS",
        "android_tv" => "Android TV Sukaldea KDS",
        "desktop_biltegia_stock" => "JavaFX Biltegia Stock",
        "web_php" => "Web PHP Frontend Publikoa",
        "browser_client" => "Web Nabigatzailea / Swagger UI",
        _ => $"Bezeroa ({id})"
    };

    private static string FormatAppRola(string id) => id switch
    {
        "admin_it" or "desktop_admin_kudeaketa" => "Admin",
        "csharp_tpv" or "android_komandera" or "android_bezeroak" => "Jangela",
        "desktop_barra_bds" => "Barra",
        "desktop_sukaldea_kds" or "android_tv" => "Sukaldea",
        "desktop_biltegia_stock" => "Biltokia",
        _ => "Kanpokoa / Web Publikoa"
    };

    public static object LortuEgoeraLaburpena()
    {
        int RolIndizea(string rola)
        {
            int idx = Array.IndexOf(RolakOrdena, rola);
            return idx >= 0 ? idx : 99;
        }

        var zerrenda = Apps.Values
            .OrderBy(a => RolIndizea(a.Rola))
            .ThenByDescending(a => a.AktiboDago)
            .ThenBy(a => a.Izena)
            .Select(a => new
            {
                id = a.Id,
                izena = a.Izena,
                mota = a.Mota,
                rola = a.Rola,
                ip = a.IpHelbidea,
                aktiboDago = a.AktiboDago,
                egoera = a.Egoera,
                azkenEskaera = a.AzkenEskaera,
                azkenOrdua = a.AzkenOrduaFormatua,
                eskaerakGuztira = a.EskaerakGuztira,
                erabiltzaileak = a.LortuErabiltzaileAktiboak(),
                erabiltzaileKopurua = a.LortuErabiltzaileAktiboak().Count,
                erabiltzaileakTestua = a.FormatErabiltzaileakTestua()
            })
            .ToList();

        int aktiboak = zerrenda.Count(a => a.aktiboDago);

        return new
        {
            aktiboakGuztira = aktiboak,
            aplikazioakGuztira = zerrenda.Count,
            data = Jatetxea.Api.Helpers.OrduHelper.EuskalHerriaNow.ToString("yyyy-MM-dd HH:mm:ss"),
            rolak = RolakOrdena,
            aplikazioak = zerrenda
        };
    }
}
