using System.Collections.Concurrent;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Text;
using Jatetxea.Desktop.Models;

namespace Jatetxea.Desktop.UI;

/// <summary>
/// TPV produktu-irudien memoria-cache profesionala (lokalean, offline %100):
/// - 3 mailako bilaketa: Platerak/platera_{id} -> Platerak/{slug} -> Kategoriak/{kategoria}
/// - Fitxategia blokeatu gabe kargatzen du (MemoryStream bidez)
/// - GDI+ Cover-Crop (object-fit: cover) marrazketa deformaziorik gabe
/// </summary>
public static class ProductImageCache
{
    private static readonly ConcurrentDictionary<string, Bitmap?> Cache = new(StringComparer.OrdinalIgnoreCase);
    private static readonly string BaseImagesDir = Path.Combine(AppContext.BaseDirectory, "Assets", "Images");
    private static readonly string[] Extensions = { ".jpg", ".jpeg", ".png", ".webp" };

    /// <summary>
    /// Plater edo edari baten irudia lortzen du cachetik (3 mailako fallback hierarkikoarekin).
    /// </summary>
    public static Bitmap? GetImage(PlateraDto platera)
    {
        string cacheKey = $"p:{platera.Id}:{platera.Kategoria}:{platera.Izena}";
        return Cache.GetOrAdd(cacheKey, _ => ResolveImage(platera));
    }

    private static Bitmap? ResolveImage(PlateraDto p)
    {
        if (!Directory.Exists(BaseImagesDir)) return null;

        string platerakDir = Path.Combine(BaseImagesDir, "Platerak");
        string kategoriakDir = Path.Combine(BaseImagesDir, "Kategoriak");

        // 1. Maila: ID zehatza (platera_{id}.jpg/png)
        var byId = TryLoadFromFolder(platerakDir, $"platera_{p.Id}");
        if (byId != null) return byId;

        // 2. Maila: Izenaren slug-a edo gako-hitz espezifikoa
        string slug = ToSlug(p.Izena);
        var bySlug = TryLoadFromFolder(platerakDir, slug);
        if (bySlug != null) return bySlug;

        string? keywordFile = MapKeywordImage(p.Izena, p.Kategoria);
        if (!string.IsNullOrEmpty(keywordFile))
        {
            var byKeyword = TryLoadFromFolder(platerakDir, keywordFile);
            if (byKeyword != null) return byKeyword;
        }

        // 3. Maila: Kategoriaren irudia (Kategoriak/{kategoria}.jpg/png)
        string katSlug = ToSlug(p.Kategoria);
        var byKat = TryLoadFromFolder(kategoriakDir, katSlug);
        if (byKat != null) return byKat;

        // Kategoriaren alias fallback-a
        string? katAlias = MapCategoryAlias(p.Kategoria);
        if (!string.IsNullOrEmpty(katAlias))
        {
            return TryLoadFromFolder(kategoriakDir, katAlias);
        }

        return null;
    }

    private static Bitmap? TryLoadFromFolder(string dir, string baseName)
    {
        if (string.IsNullOrWhiteSpace(baseName) || !Directory.Exists(dir)) return null;

        foreach (string ext in Extensions)
        {
            string fullPath = Path.Combine(dir, baseName + ext);
            if (!File.Exists(fullPath)) continue;

            return Cache.GetOrAdd("file:" + fullPath, _ => LoadBitmapUnlocked(fullPath));
        }
        return null;
    }

    private static Bitmap? LoadBitmapUnlocked(string path)
    {
        try
        {
            byte[] bytes = File.ReadAllBytes(path);
            using var ms = new MemoryStream(bytes);
            using var temp = Image.FromStream(ms);
            return new Bitmap(temp);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Irudia "object-fit: cover" moduan marrazten du helburuko laukizuzenean (deformaziorik gabe).
    /// </summary>
    public static void DrawImageCover(Graphics g, Image img, RectangleF target)
    {
        if (target.Width <= 1 || target.Height <= 1 || img.Width <= 0 || img.Height <= 0) return;

        float targetRatio = target.Width / target.Height;
        float imgRatio = (float)img.Width / img.Height;

        RectangleF srcRect;
        if (imgRatio > targetRatio)
        {
            float srcW = img.Height * targetRatio;
            srcRect = new RectangleF((img.Width - srcW) / 2f, 0f, srcW, img.Height);
        }
        else
        {
            float srcH = img.Width / targetRatio;
            srcRect = new RectangleF(0f, (img.Height - srcH) / 2f, img.Width, srcH);
        }

        var prevInterp = g.InterpolationMode;
        var prevPixel = g.PixelOffsetMode;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        g.DrawImage(img, target, srcRect, GraphicsUnit.Pixel);
        g.InterpolationMode = prevInterp;
        g.PixelOffsetMode = prevPixel;
    }

    private static string? MapKeywordImage(string izena, string kategoria)
    {
        string s = ToSlug(izena);
        if (s.Contains("txuleta") || s.Contains("entrekot") || s.Contains("solomillo") || s.Contains("arkume") || s.Contains("haragi"))
            return "txuleta";
        if (s.Contains("bakailao") || s.Contains("legatz") || s.Contains("txipiroi") || s.Contains("arrain") || s.Contains("izokin") || s.Contains("pulpo") || s.Contains("olagarro"))
            return "bakailaoa";
        if (s.Contains("entsalada") || s.Contains("burrata") || s.Contains("barazki"))
            return "entsalada";
        if (s.Contains("kroket") || s.Contains("urdaiazpik") || s.Contains("txistor") || s.Contains("pintxo") || s.Contains("rabas"))
            return "sarrerakoa";
        if (s.Contains("pasta") || s.Contains("risotto") || s.Contains("zopa") || s.Contains("babarrun") || s.Contains("porrusalda"))
            return "lehena";
        if (s.Contains("gazta") || s.Contains("tarta") || s.Contains("tiramisu") || s.Contains("txokolate") || s.Contains("izozki") || s.Contains("flan") || s.Contains("mamia") || s.Contains("pantxineta"))
            return "postrea";
        if (s.Contains("txakolin") || s.Contains("zuri") || s.Contains("prosecco"))
            return "ardo_zuria";
        if (s.Contains("ardo") || s.Contains("chianti") || s.Contains("errioxa"))
            return "ardoak";
        if (s.Contains("sagardo"))
            return "sagardoak";
        if (s.Contains("garagardo") || s.Contains("peroni") || s.Contains("basqueland") || s.Contains("zurito") || s.Contains("kana"))
            return "garagardoak";
        if (s.Contains("cappuccino") || s.Contains("espresso") || s.Contains("kafe") || s.Contains("carajillo"))
            return "kafeak";
        if (s.Contains("te_") || s.StartsWith("te_") || s.Contains("kamamila") || s.Contains("poleo") || s.Contains("infusio"))
            return "infusioak";
        if (s.Contains("gin") || s.Contains("ron") || s.Contains("vodka") || s.Contains("whisky"))
            return "konbinatuak";
        if (s.Contains("mojito") || s.Contains("aperol") || s.Contains("negroni") || s.Contains("martini") || s.Contains("koktail"))
            return "koktailak";
        if (s.Contains("patxaran") || s.Contains("orujo") || s.Contains("limoncello") || s.Contains("amaretto") || s.Contains("likore"))
            return "likoreak";
        if (s.Contains("limonada") || s.Contains("zuku") || s.Contains("mosto") || s.Contains("tonika") || s.Contains("freskagarri") || s.Contains("kola"))
            return "freskagarriak";
        if (s.Contains("ur_") || s.Contains("ura") || s.Contains("alzola") || s.Contains("pellegrino"))
            return "urak";

        return null;
    }

    private static string? MapCategoryAlias(string kategoria)
    {
        string k = ToSlug(kategoria);
        if (k.Contains("sarrera") || k.Contains("hastek")) return "sarrerakoa";
        if (k.Contains("lehen")) return "lehena";
        if (k.Contains("bigarren") || k.Contains("haragi") || k.Contains("arrain")) return "bigarrena";
        if (k.Contains("postre")) return "postrea";
        return k;
    }

    public static string ToSlug(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;
        string normalized = text.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        bool prevUnderscore = false;

        foreach (char c in normalized)
        {
            var cat = CharUnicodeInfo.GetUnicodeCategory(c);
            if (cat == UnicodeCategory.NonSpacingMark) continue;

            char lower = char.ToLowerInvariant(c);
            if ((lower >= 'a' && lower <= 'z') || (lower >= '0' && lower <= '9'))
            {
                sb.Append(lower);
                prevUnderscore = false;
            }
            else if (!prevUnderscore && sb.Length > 0)
            {
                sb.Append('_');
                prevUnderscore = true;
            }
        }

        return sb.ToString().Trim('_');
    }
}
