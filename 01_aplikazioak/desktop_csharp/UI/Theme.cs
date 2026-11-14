using System.Drawing.Drawing2D;

namespace Jatetxea.Desktop.UI;

public static class Theme
{
    // Oinarrizko Koloreak
    public static readonly Color Primary = Color.FromArgb(15, 23, 42); // #0f172a
    public static readonly Color PrimaryLight = Color.FromArgb(30, 41, 59); // #1e293b
    public static readonly Color AccentWine = Color.FromArgb(136, 19, 55); // #881337 (T1 Bordele)
    public static readonly Color AccentGold = Color.FromArgb(245, 158, 11); // #f59e0b
    public static readonly Color Background = Color.FromArgb(248, 250, 252); // #f8fafc
    public static readonly Color Surface = Color.White;
    public static readonly Color Border = Color.FromArgb(226, 232, 240); // #e2e8f0

    // Testu koloreak
    public static readonly Color TextMain = Color.FromArgb(15, 23, 42);
    public static readonly Color TextMuted = Color.FromArgb(100, 116, 139);
    public static readonly Color TextLight = Color.FromArgb(241, 245, 249);

    // Egoeren koloreak
    public static readonly Color Success = Color.FromArgb(22, 163, 74); // Libre
    public static readonly Color SuccessBg = Color.FromArgb(240, 253, 244);
    public static readonly Color SuccessBorder = Color.FromArgb(134, 239, 172);

    public static readonly Color Danger = Color.FromArgb(220, 38, 38); // Okupatua
    public static readonly Color DangerBg = Color.FromArgb(254, 242, 242);
    public static readonly Color DangerBorder = Color.FromArgb(252, 165, 165);

    public static readonly Color Warning = Color.FromArgb(217, 119, 6); // Garbitzen
    public static readonly Color WarningBg = Color.FromArgb(255, 251, 235);
    public static readonly Color WarningBorder = Color.FromArgb(253, 230, 138);

    public static readonly Color Info = Color.FromArgb(37, 99, 235); // Eskaera bidean / Sukaldean
    public static readonly Color InfoBg = Color.FromArgb(239, 246, 255);
    public static readonly Color InfoBorder = Color.FromArgb(147, 197, 253);

    public static readonly Color Purple = Color.FromArgb(124, 58, 237); // Erreserbatuta
    public static readonly Color PurpleBg = Color.FromArgb(245, 243, 255);
    public static readonly Color PurpleBorder = Color.FromArgb(196, 181, 253);

    // Letra-tipoak
    public static readonly Font FontTitle = new("Segoe UI", 16, FontStyle.Bold);
    public static readonly Font FontSubtitle = new("Segoe UI", 12, FontStyle.Bold);
    public static readonly Font FontBody = new("Segoe UI", 10, FontStyle.Regular);
    public static readonly Font FontBodyBold = new("Segoe UI", 10, FontStyle.Bold);
    public static readonly Font FontSmall = new("Segoe UI", 8.5f, FontStyle.Regular);
    public static readonly Font FontBadge = new("Segoe UI", 8, FontStyle.Bold);
    public static readonly Font FontPinDisplay = new("Segoe UI", 24, FontStyle.Bold);
    public static readonly Font FontKeypad = new("Segoe UI", 18, FontStyle.Bold);

    /// <summary>
    /// Tablet-erako botoi ukigarri modernoa estiloztatua.
    /// </summary>
    public static void ApplyTabletButton(Button btn, Color bg, Color fg, int radius = 8)
    {
        btn.FlatStyle = FlatStyle.Flat;
        btn.FlatAppearance.BorderSize = 0;
        btn.BackColor = bg;
        btn.ForeColor = fg;
        btn.Font = FontBodyBold;
        btn.Cursor = Cursors.Hand;
    }

    /// <summary>
    /// Mahai txarteletako egoeraren arabera koloreak itzultzen ditu.
    /// </summary>
    public static (Color Bg, Color Border, Color Fg) GetEgoeraKoloreak(string egoera)
    {
        return egoera.ToLowerInvariant() switch
        {
            "libre" => (SuccessBg, SuccessBorder, Success),
            "okupatuta" => (DangerBg, DangerBorder, Danger),
            "garbitzen" => (WarningBg, WarningBorder, Warning),
            "eskaera bidean" or "sukaldean" or "zerbitzatzen" => (InfoBg, InfoBorder, Info),
            "erreserbatuta" => (PurpleBg, PurpleBorder, Purple),
            _ => (Surface, Border, TextMain)
        };
    }
}
