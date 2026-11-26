using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace Jatetxea.Desktop.UI;

/// <summary>
/// T1 Jatetxea v2.0 — "Warm Hospitality" diseinu sistema zentralizatua.
/// zerbitzaria_komandera_android eta Figma diseinu-plantillekin 100% lerrokatuta.
/// </summary>
public static class Theme
{
    // ── Marka & Azentu Koloreak (Warm Hospitality) ────────────────────────────
    public static readonly Color ChiantiPrimary = Color.FromArgb(136, 19, 55);   // #881337
    public static readonly Color PomodoroHover = Color.FromArgb(159, 18, 57);    // #9F1239
    public static readonly Color BurgundyDark = Color.FromArgb(76, 5, 25);       // #4C0519
    public static readonly Color ChiantiTint = Color.FromArgb(252, 231, 240);    // #FCE7F0
    public static readonly Color AntiqueGold = Color.FromArgb(180, 83, 9);       // #B45309
    public static readonly Color GoldBorder = Color.FromArgb(212, 163, 89);      // #D4A359
    public static readonly Color GoldTint = Color.FromArgb(254, 243, 199);       // #FEF3C7

    // ── Azalerak & Testuak ────────────────────────────────────────────────────
    public static readonly Color WarmLinen = Color.FromArgb(250, 247, 242);      // #FAF7F2
    public static readonly Color MarbleWhite = Color.FromArgb(255, 255, 255);    // #FFFFFF
    public static readonly Color StoneMist = Color.FromArgb(243, 239, 232);      // #F3EFE8
    public static readonly Color WarmTaupe = Color.FromArgb(230, 223, 213);      // #E6DFD5
    public static readonly Color EspressoInk = Color.FromArgb(28, 25, 23);       // #1C1917
    public static readonly Color WarmSlate = Color.FromArgb(87, 83, 78);         // #57534E
    public static readonly Color StoneMuted = Color.FromArgb(120, 113, 108);     // #78716C

    // ── Bateragarritasun aliasak (lehendik dauden deietarako) ─────────────────
    public static readonly Color Primary = EspressoInk;
    public static readonly Color PrimaryLight = Color.FromArgb(41, 37, 36);      // #292524
    public static readonly Color AccentWine = ChiantiPrimary;
    public static readonly Color AccentGold = AntiqueGold;
    public static readonly Color Background = WarmLinen;
    public static readonly Color Surface = MarbleWhite;
    public static readonly Color Border = WarmTaupe;

    public static readonly Color TextMain = EspressoInk;
    public static readonly Color TextMuted = WarmSlate;
    public static readonly Color TextLight = WarmLinen;

    // ── Egoeren Koloreak (WCAG 2.2 AA kontrastea) ─────────────────────────────
    public static readonly Color Success = Color.FromArgb(21, 128, 61);          // #15803D (Albahaka - Libre)
    public static readonly Color SuccessBg = Color.FromArgb(220, 252, 231);      // #DCFCE7
    public static readonly Color SuccessBorder = Color.FromArgb(134, 239, 172);  // #86EFAC

    public static readonly Color Danger = Color.FromArgb(185, 28, 28);           // #B91C1C (Gorria - Okupatuta / Ezabatu)
    public static readonly Color DangerBg = Color.FromArgb(254, 226, 226);       // #FEE2E2
    public static readonly Color DangerBorder = Color.FromArgb(252, 165, 165);   // #FCA5A5

    public static readonly Color Warning = Color.FromArgb(194, 65, 12);          // #C2410C (Terrakota - Zai / Sukaldean)
    public static readonly Color WarningBg = Color.FromArgb(255, 237, 213);      // #FFEDD5
    public static readonly Color WarningBorder = Color.FromArgb(253, 186, 116);  // #FDBA74

    public static readonly Color Info = Color.FromArgb(3, 105, 161);             // #0369A1 (Urdina - Garbitzen / Martxan)
    public static readonly Color InfoBg = Color.FromArgb(224, 242, 254);         // #E0F2FE
    public static readonly Color InfoBorder = Color.FromArgb(125, 211, 252);     // #7DD3FC

    public static readonly Color Purple = Color.FromArgb(109, 40, 217);          // #6D28D9 (Erreserbak)
    public static readonly Color PurpleBg = Color.FromArgb(245, 243, 255);       // #F5F3FF
    public static readonly Color PurpleBorder = Color.FromArgb(196, 181, 253);   // #C4B5FD

    // ── Tipografia Sistema (Cinzel + Plus Jakarta Sans + JetBrains Mono) ──────
    private static readonly PrivateFontCollection CustomFonts = new();
    private static readonly FontFamily? CinzelFamily;
    private static readonly FontFamily? JakartaFamily;
    private static readonly FontFamily? MonoFamily;

    static Theme()
    {
        try
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                return;
            }

            string fontsDir = Path.Combine(AppContext.BaseDirectory, "Assets", "Fonts");
            if (!Directory.Exists(fontsDir))
            {
                return;
            }

            string[] files =
            {
                "cinzel_bold.ttf",
                "plus_jakarta_sans_regular.ttf",
                "plus_jakarta_sans_medium.ttf",
                "plus_jakarta_sans_semibold.ttf",
                "plus_jakarta_sans_bold.ttf",
                "jetbrains_mono_medium.ttf",
                "jetbrains_mono_bold.ttf"
            };

            foreach (string file in files)
            {
                string fullPath = Path.Combine(fontsDir, file);
                if (File.Exists(fullPath))
                {
                    CustomFonts.AddFontFile(fullPath);
                }
            }

            foreach (var fam in CustomFonts.Families)
            {
                if (fam.Name.Contains("Cinzel", StringComparison.OrdinalIgnoreCase))
                    CinzelFamily ??= fam;
                else if (fam.Name.Contains("Jakarta", StringComparison.OrdinalIgnoreCase))
                    JakartaFamily ??= fam;
                else if (fam.Name.Contains("JetBrains", StringComparison.OrdinalIgnoreCase))
                    MonoFamily ??= fam;
            }
        }
        catch
        {
            // Fallback segurua sistemako letra-tipoetara
        }
    }

    public static Font DisplayFont(float size, FontStyle style = FontStyle.Bold)
    {
        if (CinzelFamily != null)
            return new Font(CinzelFamily, size, style);
        return new Font("Georgia", size, style);
    }

    public static Font BodyFont(float size, bool bold = false)
    {
        FontStyle style = bold ? FontStyle.Bold : FontStyle.Regular;
        // Segoe UI Windows ClearType-ekin perfektuki errendatzen da (espaziatu garbia, lausotasunik gabe)
        return new Font("Segoe UI", size, style);
    }

    public static Font MonoFont(float size, bool bold = true)
    {
        FontStyle style = bold ? FontStyle.Bold : FontStyle.Regular;
        if (MonoFamily != null)
            return new Font(MonoFamily, size, style);
        return new Font("Consolas", size, style);
    }

    // Letra-tipo estandarrak
    public static readonly Font FontTitle = DisplayFont(16f);
    public static readonly Font FontSubtitle = BodyFont(12f, bold: true);
    public static readonly Font FontBody = BodyFont(10f, bold: false);
    public static readonly Font FontBodyBold = BodyFont(10f, bold: true);
    public static readonly Font FontSmall = BodyFont(8.5f, bold: false);
    public static readonly Font FontBadge = BodyFont(8.5f, bold: true);
    public static readonly Font FontPinDisplay = MonoFont(22f, bold: true);
    public static readonly Font FontKeypad = MonoFont(17f, bold: true);

    // ── DPI Eskalatze Laguntzaileak (PerMonitorV2 100% / 125% / 150% / 200%) ──
    public static float DpiScale(Control? c)
    {
        int dpi = c != null && !c.IsDisposed && c.DeviceDpi > 0 ? c.DeviceDpi : 96;
        return dpi / 96f;
    }

    public static int S(Control? c, int px) => (int)Math.Round(px * DpiScale(c));

    /// <summary>
    /// Modal bati DPI-arekin eskalatutako tamaina erosoa ezartzen dio (pantailaren %90 gainditu gabe)
    /// eta guraso-leihoaren erdian kokatzen du.
    /// </summary>
    public static void ApplyModalDpiSize(Form modal, int logicalW, int logicalH, int minLogicalW, int minLogicalH)
    {
        var screen = Screen.FromControl(modal.Owner ?? modal).WorkingArea;
        int targetW = Math.Min(S(modal, logicalW), (int)(screen.Width * 0.90));
        int targetH = Math.Min(S(modal, logicalH), (int)(screen.Height * 0.90));
        int minW = Math.Min(S(modal, minLogicalW), targetW);
        int minH = Math.Min(S(modal, minLogicalH), targetH);

        modal.MinimumSize = new Size(minW, minH);
        modal.ClientSize = new Size(targetW, targetH);

        var refRect = modal.Owner != null ? modal.Owner.Bounds : screen;
        int x = Math.Max(screen.Left, refRect.Left + (refRect.Width - modal.Width) / 2);
        int y = Math.Max(screen.Top, refRect.Top + (refRect.Height - modal.Height) / 2);
        modal.Location = new Point(x, y);
    }

    // ── GDI+ Marrazketa Laguntzaileak (Txartelak, Pilulak eta T1 Monograma) ───

    public static GraphicsPath CreateRoundedRectPath(RectangleF rect, float radius)
    {
        var path = new GraphicsPath();
        float d = Math.Max(1f, Math.Min(radius * 2f, Math.Min(rect.Width, rect.Height)));
        path.AddArc(rect.X, rect.Y, d, d, 180, 90);
        path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
        path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
        path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }

    /// <summary>
    /// Ertz biribildu leunak dituen txartela edo pilula margotzen du (AntiAlias).
    /// </summary>
    public static void DrawRoundedCard(Graphics g, Rectangle bounds, int radius, Color fill, Color border, float borderWidth = 1.2f)
    {
        if (bounds.Width <= 2 || bounds.Height <= 2) return;

        g.SmoothingMode = SmoothingMode.AntiAlias;
        var rect = new RectangleF(bounds.X + 0.5f, bounds.Y + 0.5f, bounds.Width - 1.5f, bounds.Height - 1.5f);
        using var path = CreateRoundedRectPath(rect, radius);
        using var fillBrush = new SolidBrush(fill);
        g.FillPath(fillBrush, path);

        if (borderWidth > 0f && border.A > 0)
        {
            using var pen = new Pen(border, borderWidth);
            g.DrawPath(pen, path);
        }
    }

    /// <summary>
    /// Kontrol bati ertz biribilduko Region aplikatzen dio (botoi edo txartel modernoetarako).
    /// </summary>
    public static void ApplyRoundedRegion(Control ctrl, int radius)
    {
        void UpdateRegion()
        {
            if (ctrl.IsDisposed || ctrl.Width < 4 || ctrl.Height < 4) return;
            using var path = CreateRoundedRectPath(new RectangleF(0, 0, ctrl.Width, ctrl.Height), radius);
            var oldRegion = ctrl.Region;
            ctrl.Region = new Region(path);
            oldRegion?.Dispose();
        }

        UpdateRegion();
        ctrl.Resize += (_, _) => UpdateRegion();
    }

    /// <summary>
    /// Android `ic_t1_logo.xml` eta Figma-ko T1 monograma arkitektonikoa margotzen du.
    /// </summary>
    public static void DrawT1Logo(Graphics g, Rectangle bounds)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

        // Kanpoko urrezko kutxa biribildua (Warm Linen + Gold Border)
        var outerRect = new RectangleF(bounds.X + 1, bounds.Y + 1, bounds.Width - 2.5f, bounds.Height - 2.5f);
        using (var outerPath = CreateRoundedRectPath(outerRect, 12f))
        using (var bgBrush = new SolidBrush(WarmLinen))
        using (var goldPen = new Pen(GoldBorder, 1.8f))
        {
            g.FillPath(bgBrush, outerPath);
            g.DrawPath(goldPen, outerPath);
        }

        // Barruko koadro fina
        var innerRect = new RectangleF(bounds.X + 4.5f, bounds.Y + 4.5f, bounds.Width - 9.5f, bounds.Height - 9.5f);
        using (var innerPath = CreateRoundedRectPath(innerRect, 8f))
        using (var innerPen = new Pen(Color.FromArgb(140, GoldBorder), 1.0f))
        {
            g.DrawPath(innerPen, innerPath);
        }

        // "T" (Chianti Primary) + "1" (Espresso Ink) Cinzel letra-tipoan (GraphicsUnit.Pixel -> DPI edozein dela ondo sartzen da!)
        float pxSize = Math.Max(14f, bounds.Height * 0.42f);
        using var font = CinzelFamily != null
            ? new Font(CinzelFamily, pxSize, FontStyle.Bold, GraphicsUnit.Pixel)
            : new Font("Georgia", pxSize, FontStyle.Bold, GraphicsUnit.Pixel);
        using var brushT = new SolidBrush(ChiantiPrimary);
        using var brush1 = new SolidBrush(EspressoInk);
        using var sf = StringFormat.GenericTypographic;

        var sizeT = g.MeasureString("T", font, PointF.Empty, sf);
        var size1 = g.MeasureString("1", font, PointF.Empty, sf);
        float kerning = Math.Max(1f, bounds.Width * 0.02f);
        float totalW = sizeT.Width + size1.Width + kerning;
        float startX = bounds.X + (bounds.Width - totalW) / 2f;
        float startY = bounds.Y + (bounds.Height - sizeT.Height) / 2f - bounds.Height * 0.05f;

        g.DrawString("T", font, brushT, startX, startY, sf);
        g.DrawString("1", font, brush1, startX + sizeT.Width + kerning, startY, sf);

        // Beheko urrezko lerro arkitektonikoa + puntu gorria
        float lineY = bounds.Bottom - Math.Max(7f, bounds.Height * 0.19f);
        float padX = bounds.Width * 0.24f;
        using var accentPen = new Pen(GoldBorder, 1.2f);
        g.DrawLine(accentPen, bounds.X + padX, lineY, bounds.Right - padX, lineY);
        float dotR = Math.Max(2f, bounds.Width * 0.035f);
        using var dotBrush = new SolidBrush(ChiantiPrimary);
        g.FillEllipse(dotBrush, bounds.X + bounds.Width / 2f - dotR, lineY - dotR, dotR * 2f, dotR * 2f);
    }

    /// <summary>
    /// Botoi bat T1 Warm Hospitality estiloan (ertz biribildu jarraitu eta garbiekin, Region mozketarik gabe) prestatzen du.
    /// </summary>
    public static void StyleButton(Button btn, Color bg, Color fg, Color? borderColor = null, int radius = 10, float fontSize = 10f, bool bold = true)
    {
        btn.FlatStyle = FlatStyle.Flat;
        btn.FlatAppearance.BorderSize = 0;
        btn.BackColor = bg;
        btn.ForeColor = fg;
        btn.Font = BodyFont(fontSize, bold);
        btn.Cursor = Cursors.Hand;
        btn.UseVisualStyleBackColor = false;
        btn.Region = null;

        Color hoverBg = bg == ChiantiPrimary
            ? PomodoroHover
            : (bg == StoneMist || bg == MarbleWhite ? WarmTaupe : ControlPaint.Dark(bg, 0.06f));
        Color downBg = bg == ChiantiPrimary
            ? BurgundyDark
            : ControlPaint.Dark(bg, 0.12f);

        bool isHovered = false;
        bool isPressed = false;

        btn.MouseEnter += (_, _) => { isHovered = true; btn.Invalidate(); };
        btn.MouseLeave += (_, _) => { isHovered = false; isPressed = false; btn.Invalidate(); };
        btn.MouseDown += (_, _) => { isPressed = true; btn.Invalidate(); };
        btn.MouseUp += (_, _) => { isPressed = false; btn.Invalidate(); };

        btn.Paint += (_, e) =>
        {
            Color parentBg = btn.Parent?.BackColor ?? WarmLinen;
            if (parentBg == Color.Transparent || parentBg.A == 0) parentBg = WarmLinen;
            e.Graphics.Clear(parentBg);

            Color fillCol = !btn.Enabled
                ? StoneMist
                : (isPressed ? downBg : (isHovered ? hoverBg : btn.BackColor));
            Color textCol = !btn.Enabled ? StoneMuted : btn.ForeColor;
            Color bCol = borderColor ?? Color.Transparent;

            DrawRoundedCard(
                e.Graphics,
                new Rectangle(1, 1, btn.Width - 2, btn.Height - 2),
                radius,
                fillCol,
                bCol,
                borderColor.HasValue ? 1.3f : 0f);

            TextRenderer.DrawText(
                e.Graphics,
                btn.Text,
                btn.Font,
                new Rectangle(4, 0, btn.Width - 8, btn.Height),
                textCol,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        };
    }

    /// <summary>
    /// Tablet-erako botoi ukigarri modernoa estiloztatua (bateragarritasunerako).
    /// </summary>
    public static void ApplyTabletButton(Button btn, Color bg, Color fg, int radius = 10)
    {
        StyleButton(btn, bg, fg, null, radius);
    }

    /// <summary>
    /// Formulario osoari Warm Hospitality v2.0 paleta, Plus Jakarta Sans tipografia
    /// eta parpadeorik gabeko DoubleBuffering aplikatzen dizkio modu zentralizatuan.
    /// </summary>
    public static void ApplyWarmHospitalityTheme(Form form)
    {
        if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
        {
            return;
        }

        // 1. Parpadeorik gabeko DoubleBuffering kontrol-zuhaitz osoan
        UiPerf.EnableDoubleBuffering(form);

        // 2. Formularioaren oinarrizko hondoa eta koloreak
        form.BackColor = MapLegacyColor(form.BackColor, isBackground: true);
        form.ForeColor = EspressoInk;

        // 3. Azpikontrol guztiak errekurtsiboki berritu
        StyleControlTree(form);
    }

    private static void StyleControlTree(Control parent)
    {
        foreach (Control ctrl in parent.Controls)
        {
            // Hondo eta testu kolore hotzak (Tailwind Slate) -> Warm Hospitality tonuetara mapatu
            ctrl.BackColor = MapLegacyColor(ctrl.BackColor, isBackground: true);
            ctrl.ForeColor = MapLegacyColor(ctrl.ForeColor, isBackground: false);

            // Letra-tipoa berritu (Segoe UI -> Plus Jakarta Sans, Consolas -> JetBrains Mono)
            if (ctrl.Font != null)
            {
                string famName = ctrl.Font.FontFamily.Name;
                if (famName.Equals("Segoe UI", StringComparison.OrdinalIgnoreCase))
                {
                    ctrl.Font = BodyFont(ctrl.Font.Size, ctrl.Font.Bold);
                }
                else if (famName.Equals("Consolas", StringComparison.OrdinalIgnoreCase) ||
                         famName.Equals("Courier New", StringComparison.OrdinalIgnoreCase))
                {
                    ctrl.Font = MonoFont(ctrl.Font.Size, ctrl.Font.Bold);
                }
            }

            if (ctrl is Button btn)
            {
                if (btn.FlatStyle == FlatStyle.Flat && btn.FlatAppearance.BorderSize > 0)
                {
                    btn.FlatAppearance.BorderColor = MapLegacyColor(btn.FlatAppearance.BorderColor, isBackground: false);
                }

                if (btn.BackColor == ChiantiPrimary)
                {
                    btn.FlatAppearance.MouseOverBackColor = PomodoroHover;
                    btn.FlatAppearance.MouseDownBackColor = BurgundyDark;
                }
            }

            if (ctrl.HasChildren)
            {
                StyleControlTree(ctrl);
            }
        }
    }

    /// <summary>
    /// Aurreko Tailwind Slate kolore hotzak T1 Warm Hospitality kolore beroetara bihurtzen ditu.
    /// </summary>
    private static Color MapLegacyColor(Color c, bool isBackground)
    {
        if (c == Color.Transparent || c.A == 0) return c;

        // Atzeko plano hotzak -> Warm Linen / Stone Mist
        if (c == Color.FromArgb(248, 250, 252) || c == Color.FromArgb(241, 245, 249))
            return isBackground ? WarmLinen : StoneMist;

        // Ertz eta marra hotzak -> Warm Taupe
        if (c == Color.FromArgb(226, 232, 240) || c == Color.FromArgb(203, 213, 225))
            return WarmTaupe;

        // Testu ilun hotzak -> Espresso Ink
        if (c == Color.FromArgb(15, 23, 42) || c == Color.FromArgb(30, 41, 59))
            return EspressoInk;

        // Testu gris hotzak -> Warm Slate / Stone Muted
        if (c == Color.FromArgb(51, 65, 85) || c == Color.FromArgb(71, 85, 105))
            return WarmSlate;
        if (c == Color.FromArgb(100, 116, 139) || c == Color.FromArgb(148, 163, 184))
            return StoneMuted;

        // Berde zaharrak -> Basil Green
        if (c == Color.FromArgb(22, 163, 74) || c == Color.FromArgb(22, 101, 52))
            return Success;

        // Gorri zaharrak -> Crimson
        if (c == Color.FromArgb(220, 38, 38))
            return Danger;

        return c;
    }

    /// <summary>
    /// Mahai txarteletako egoeraren arabera Warm Hospitality koloreak itzultzen ditu.
    /// </summary>
    public static (Color Bg, Color Border, Color Fg) GetEgoeraKoloreak(string egoera)
    {
        return egoera.ToLowerInvariant() switch
        {
            "libre" or "librea" => (SuccessBg, SuccessBorder, Success),
            "okupatuta" or "okupatua" => (EspressoInk, EspressoInk, MarbleWhite),
            "garbitzen" or "garbitzeko" => (InfoBg, InfoBorder, Info),
            "eskaera bidean" or "sukaldean" or "zerbitzatzen" => (WarningBg, WarningBorder, Warning),
            "erreserbatuta" => (PurpleBg, PurpleBorder, Purple),
            _ => (StoneMist, WarmTaupe, EspressoInk)
        };
    }
}
