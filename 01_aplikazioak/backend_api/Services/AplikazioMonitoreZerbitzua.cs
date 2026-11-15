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
    public string IpHelbidea { get; set; } = "-";
    public DateTime AzkenKonexioa { get; set; } = DateTime.MinValue;
    public string AzkenEskaera { get; set; } = "-";
    public int EskaerakGuztira { get; set; } = 0;

    /// <summary>
    /// Azken 35 segundoetan eskaeraren bat egin badu, aplikazioa AKTIBO dagoela kontsideratzen da.
    /// </summary>
    public bool AktiboDago => AzkenKonexioa != DateTime.MinValue && (DateTime.UtcNow - AzkenKonexioa).TotalSeconds <= 35;

    public string Egoera
    {
        get
        {
            if (AzkenKonexioa == DateTime.MinValue) return "DESKONEKTATUTA";
            return AktiboDago ? "AKTIBO" : "ISILIK";
        }
    }

    public string AzkenOrduaFormatua
    {
        get
        {
            if (AzkenKonexioa == DateTime.MinValue) return "Ez da konektatu";
            return AzkenKonexioa.ToLocalTime().ToString("HH:mm:ss");
        }
    }
}

/// <summary>
/// REST APIari eskaerak egiten dizkioten bezeroak (C# TPV, JavaFX KDS, Web PHP, Mugikorrak, Admin...)
/// denbora errealean identifikatu, zenbatu eta monitorizatzen dituen zerbitzu zentrala.
/// </summary>
public static class AplikazioMonitoreZerbitzua
{
    private static readonly ConcurrentDictionary<string, KonektatutakoAppInfo> Apps = new();

    static AplikazioMonitoreZerbitzua()
    {
        // Proiektuko aplikazio ezagunak hasieratu aurrez, deskonektatuta badaude ere ager daitezen
        HasieratuAppEzagunak();
    }

    private static void HasieratuAppEzagunak()
    {
        ErregistratuAplikazioEredua("csharp_tpv", "C# TPV Areto & Kutxazaina", "Desktop C# (Windows Forms)");
        ErregistratuAplikazioEredua("javafx_kds", "JavaFX Sukaldea KDS Terminala", "Desktop Java (JavaFX 21)");
        ErregistratuAplikazioEredua("web_php", "Web PHP Frontend Publikoa", "Web Apache (PHP 8.2)");
        ErregistratuAplikazioEredua("mobile_zerbitzaria", "Flutter Zerbitzaria (Komandera)", "Mugikorra (Flutter / Dart)");
        ErregistratuAplikazioEredua("mobile_bezeroa", "Flutter Bezero Aplikazioa", "Mugikorra (Flutter / Dart)");
        ErregistratuAplikazioEredua("android_tv", "Android TV Sukaldea KDS", "Telebista (Compose TV)");
        ErregistratuAplikazioEredua("admin_it", "Admin IT Zentrala (Portu 5000)", "Web Panela (ASP.NET Core)");
    }

    private static void ErregistratuAplikazioEredua(string id, string izena, string mota)
    {
        Apps.TryAdd(id, new KonektatutakoAppInfo
        {
            Id = id,
            Izena = izena,
            Mota = mota,
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
            if (id.Contains("csharp") || id.Contains("tpv")) return "csharp_tpv";
            if (id.Contains("javafx") || id.Contains("kds")) return "javafx_kds";
            if (id.Contains("web") || id.Contains("php")) return "web_php";
            if (id.Contains("zerbitzari")) return "mobile_zerbitzaria";
            if (id.Contains("bezero")) return "mobile_bezeroa";
            if (id.Contains("android_tv") || id.Contains("tv")) return "android_tv";
            if (id.Contains("admin")) return "admin_it";
            return id;
        }

        // 2. X-App-Name goiburua
        if (headers.TryGetValue("X-App-Name", out var appNameVal) && !string.IsNullOrWhiteSpace(appNameVal))
        {
            string name = appNameVal.ToString().ToLowerInvariant();
            if (name.Contains("c#") || name.Contains("tpv") || name.Contains("kutxazain")) return "csharp_tpv";
            if (name.Contains("javafx") || name.Contains("kds")) return "javafx_kds";
            if (name.Contains("php") || name.Contains("web")) return "web_php";
            if (name.Contains("zerbitzari") || name.Contains("komandera")) return "mobile_zerbitzaria";
            if (name.Contains("bezero")) return "mobile_bezeroa";
            if (name.Contains("tv")) return "android_tv";
            if (name.Contains("admin")) return "admin_it";
        }

        // 3. User-Agent bidezko detekzio automatikoa
        string userAgent = headers.UserAgent.ToString().ToLowerInvariant();

        if (userAgent.Contains("jatetxea.desktop") || userAgent.Contains("csharp") || userAgent.Contains("windowsforms"))
            return "csharp_tpv";

        if (userAgent.Contains("javafx") || userAgent.Contains("java-http-client") || userAgent.Contains("java/"))
            return "javafx_kds";

        if (userAgent.Contains("php") || userAgent.Contains("guzzle") || userAgent.Contains("jatetxea-web-php"))
            return "web_php";

        if (userAgent.Contains("dart") || userAgent.Contains("flutter"))
            return "mobile_zerbitzaria";

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
    public static void ErregistratuEskaera(string appId, string ipHelbidea, string eskaeraBidea)
    {
        if (string.IsNullOrWhiteSpace(appId)) return;

        // Zerrendan ez badago, modu dinamikoan gehitu
        var app = Apps.GetOrAdd(appId, id => new KonektatutakoAppInfo
        {
            Id = id,
            Izena = FormatAppIzena(id),
            Mota = "Kanpoko Bezeroa",
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
        }

        // Kontsolara mezua inprimatu
        string ordua = DateTime.Now.ToString("HH:mm:ss");
        Console.WriteLine($"[{ordua}] [APP KONEKTATUTA: {app.Izena}] -> {eskaeraBidea} (IP: {ipHelbidea})");
    }

    private static string FormatAppIzena(string id) => id switch
    {
        "csharp_tpv" => "C# TPV Areto & Kutxazaina",
        "javafx_kds" => "JavaFX Sukaldea KDS Terminala",
        "web_php" => "Web PHP Frontend Publikoa",
        "mobile_zerbitzaria" => "Flutter Zerbitzaria (Komandera)",
        "mobile_bezeroa" => "Flutter Bezero Aplikazioa",
        "android_tv" => "Android TV Sukaldea KDS",
        "admin_it" => "Admin IT Zentrala (Portu 5000)",
        "browser_client" => "Web Nabigatzailea / Swagger UI",
        _ => $"Bezeroa ({id})"
    };

    public static object LortuEgoeraLaburpena()
    {
        var zerrenda = Apps.Values
            .OrderByDescending(a => a.AktiboDago)
            .ThenByDescending(a => a.AzkenKonexioa)
            .Select(a => new
            {
                id = a.Id,
                izena = a.Izena,
                mota = a.Mota,
                ip = a.IpHelbidea,
                aktiboDago = a.AktiboDago,
                egoera = a.Egoera,
                azkenEskaera = a.AzkenEskaera,
                azkenOrdua = a.AzkenOrduaFormatua,
                eskaerakGuztira = a.EskaerakGuztira
            })
            .ToList();

        int aktiboak = zerrenda.Count(a => a.aktiboDago);

        return new
        {
            aktiboakGuztira = aktiboak,
            aplikazioakGuztira = zerrenda.Count,
            data = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            aplikazioak = zerrenda
        };
    }
}
