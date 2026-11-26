using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.CompilerServices;

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

    public static Font DisplayFont(Control? c, float ptSize, FontStyle style = FontStyle.Bold)
    {
        float px = PtToPx(c, ptSize);
        if (CinzelFamily != null)
            return new Font(CinzelFamily, px, style, GraphicsUnit.Pixel);
        return new Font("Georgia", px, style, GraphicsUnit.Pixel);
    }

    public static Font BodyFont(float size, bool bold = false)
    {
        FontStyle style = bold ? FontStyle.Bold : FontStyle.Regular;
        // Segoe UI Windows ClearType-ekin perfektuki errendatzen da (espaziatu garbia, lausotasunik gabe)
        return new Font("Segoe UI", size, style);
    }

    public static Font BodyFont(Control? c, float ptSize, bool bold = false, FontStyle? explicitStyle = null)
    {
        FontStyle style = explicitStyle ?? (bold ? FontStyle.Bold : FontStyle.Regular);
        return new Font("Segoe UI", PtToPx(c, ptSize), style, GraphicsUnit.Pixel);
    }

    public static Font MonoFont(float size, bool bold = true)
    {
        FontStyle style = bold ? FontStyle.Bold : FontStyle.Regular;
        if (MonoFamily != null)
            return new Font(MonoFamily, size, style);
        return new Font("Consolas", size, style);
    }

    public static Font MonoFont(Control? c, float ptSize, bool bold = true)
    {
        FontStyle style = bold ? FontStyle.Bold : FontStyle.Regular;
        float px = PtToPx(c, ptSize);
        if (MonoFamily != null)
            return new Font(MonoFamily, px, style, GraphicsUnit.Pixel);
        return new Font("Consolas", px, style, GraphicsUnit.Pixel);
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
    public static Screen? ActiveScreen { get; set; }

    private sealed record BaseFontSpec(FontFamily Family, float BasePt, FontStyle Style);
    private static readonly ConditionalWeakTable<Control, BaseFontSpec> BaseFonts = new();
    private static readonly ConditionalWeakTable<Control, object> DpiHookedContainers = new();

    /// <summary>
    /// Leiho bat erabiltzaileak unean darabilen pantaila berean (ActiveScreen / sourceForm) kokatzen du,
    /// pantailaz aldatzean edo login egitean pantaila nagusira itzultzea saihesteko.
    /// </summary>
    public static void PlaceOnActiveScreen(Form targetForm, Form? sourceForm = null)
    {
        if (targetForm.IsDisposed) return;

        if (sourceForm != null && !sourceForm.IsDisposed)
        {
            ActiveScreen = Screen.FromControl(sourceForm);
        }
        else if (targetForm.Owner != null && !targetForm.Owner.IsDisposed)
        {
            ActiveScreen = Screen.FromControl(targetForm.Owner);
        }

        var screen = ActiveScreen ?? Screen.FromControl(targetForm);
        var wa = screen.WorkingArea;
        bool wasMaximized = targetForm.WindowState == FormWindowState.Maximized;

        targetForm.StartPosition = FormStartPosition.Manual;
        if (wasMaximized)
        {
            targetForm.WindowState = FormWindowState.Normal;
            targetForm.Bounds = wa;
            targetForm.WindowState = FormWindowState.Maximized;
        }
        else
        {
            int x = wa.Left + Math.Max(0, (wa.Width - targetForm.Width) / 2);
            int y = wa.Top + Math.Max(0, (wa.Height - targetForm.Height) / 2);
            targetForm.Location = new Point(x, y);
        }
    }

    private static void HookContainerForDpi(Control container)
    {
        if (container.IsDisposed) return;
        if (DpiHookedContainers.TryGetValue(container, out _)) return;
        DpiHookedContainers.Add(container, new object());

        container.ControlAdded += (s, e) =>
        {
            if (e.Control == null || e.Control.IsDisposed) return;
            var frm = e.Control.FindForm() ?? container.FindForm();
            int dpi = GetControlDpi(frm ?? container);
            RefreshFontsRecursive(e.Control, dpi);
            HookContainerForDpi(e.Control);
        };
    }

    public static int GetControlDpi(Control? c)
    {
        if (c == null || c.IsDisposed) return 96;
        int dpi = c.DeviceDpi;
        if (!c.IsHandleCreated)
        {
            var frm = c.FindForm();
            if (frm != null && !frm.IsDisposed && frm.IsHandleCreated && frm.DeviceDpi > 0)
            {
                dpi = frm.DeviceDpi;
            }
        }
        return dpi > 0 ? dpi : 96;
    }

    public static float PtToPx(Control? c, float ptSize)
    {
        return Math.Max(6f, ptSize * (GetControlDpi(c) / 72f));
    }

    public static float DpiScale(Control? c) => GetControlDpi(c) / 96f;

    public static int S(Control? c, int px) => (int)Math.Round(px * DpiScale(c));

    /// <summary>
    /// AutoScaleMode.None + PerMonitorV2 konbinazioan, GraphicsUnit.Point letra-tipoek
    /// Windows-eko pantaila nagusiko LOGPIXELSY (adib. 192 DPI) hartzen dute beti.
    /// Metodo honek kontrol-zuhaitz osoko letra-tipoak GraphicsUnit.Pixel bihurtzen ditu
    /// uneko pantailaren DeviceDpi zehatzera (pt * DeviceDpi / 72), pantaila guztietan
    /// proportzio berdin-berdina lortzeko (200% portatila <-> 100% HDMI).
    /// </summary>
    public static void RefreshFontsForDpi(Control root)
    {
        if (root == null || root.IsDisposed) return;

        int dpi = GetControlDpi(root);

        try
        {
            root.SuspendLayout();
            ApplyPixelFontToControl(root, dpi);

            foreach (Control child in root.Controls)
            {
                RefreshFontsRecursive(child, dpi);
            }
        }
        finally
        {
            root.ResumeLayout(true);
        }
    }

    private static void RefreshFontsRecursive(Control ctrl, int dpi)
    {
        if (ctrl.IsDisposed) return;

        ApplyPixelFontToControl(ctrl, dpi);
        HookContainerForDpi(ctrl);

        if (ctrl.HasChildren)
        {
            foreach (Control child in ctrl.Controls)
            {
                RefreshFontsRecursive(child, dpi);
            }
        }
    }

    private static void ApplyPixelFontToControl(Control ctrl, int dpi)
    {
        if (ctrl.Font == null) return;

        var f = ctrl.Font;
        BaseFontSpec spec;

        if (f.Unit == GraphicsUnit.Point)
        {
            // Puntutan esleitutako letra-tipo berria edo hasierakoa: gorde bere oinarrizko pt tamaina
            spec = new BaseFontSpec(f.FontFamily, f.Size, f.Style);
            BaseFonts.AddOrUpdate(ctrl, spec);
        }
        else if (!BaseFonts.TryGetValue(ctrl, out spec!))
        {
            // Pixel-etan sortutakoa (adib. BodyFont(this, pt)): atzera-kalkulatu pt tamaina
            float basePt = f.Size * 72f / Math.Max(96, dpi);
            spec = new BaseFontSpec(f.FontFamily, basePt, f.Style);
            BaseFonts.AddOrUpdate(ctrl, spec);
        }

        float targetPx = Math.Max(6f, spec.BasePt * (dpi / 72f));
        if (f.Unit == GraphicsUnit.Pixel && Math.Abs(f.Size - targetPx) < 0.15f && f.Style == spec.Style)
        {
            return;
        }

        ctrl.Font = new Font(spec.Family, targetPx, spec.Style, GraphicsUnit.Pixel);
    }

    /// <summary>
    /// Formulario nagusi bati uneko pantailaren DPI eta WorkingArea-ra egokitutako MinimumSize ezartzen dio.
    /// </summary>
    public static void ApplyMainFormDpiBounds(Form form, int minLogicalW = 800, int minLogicalH = 500)
    {
        if (form.IsDisposed) return;
        var screen = Screen.FromControl(form.Owner ?? form).WorkingArea;
        int minW = Math.Min(S(form, minLogicalW), (int)(screen.Width * 0.92));
        int minH = Math.Min(S(form, minLogicalH), (int)(screen.Height * 0.92));
        form.MinimumSize = new Size(Math.Max(320, minW), Math.Max(240, minH));
    }

    /// <summary>
    /// Formulario nagusi batean (Maximized) pantaila anitzeko PerMonitorV2 egokitzapen automatikoa aktibatzen du:
    /// - Guraso-leihoaren (Owner) pantaila berean irekitzen da.
    /// - DPI aldaketan (200% <-> 100%) letra-tipoak, MinimumSize eta diseinu dinamikoa automatikoki berritzen ditu.
    /// </summary>
    public static void AttachMainFormDpiAdaptation(Form form, Action onDpiRefresh, int minLogicalW = 800, int minLogicalH = 500)
    {
        bool dpiRefreshQueued = false;

        void RememberScreen()
        {
            if (!form.IsDisposed && form.Visible && form.WindowState != FormWindowState.Minimized)
            {
                ActiveScreen = Screen.FromControl(form);
            }
        }

        form.Activated += (s, e) => RememberScreen();
        form.LocationChanged += (s, e) => RememberScreen();

        form.Load += (s, e) =>
        {
            if (form.IsDisposed) return;

            // Guraso-leihoaren edo azken pantaila aktiboaren pantaila berean kokatu
            if (form.Owner != null || ActiveScreen != null)
            {
                PlaceOnActiveScreen(form, form.Owner);
            }

            ApplyMainFormDpiBounds(form, minLogicalW, minLogicalH);
            RefreshFontsForDpi(form);
        };

        form.DpiChanged += (s, e) =>
        {
            if (form.IsDisposed || dpiRefreshQueued) return;
            dpiRefreshQueued = true;

            form.BeginInvoke(new Action(() =>
            {
                dpiRefreshQueued = false;
                if (form.IsDisposed) return;

                ApplyMainFormDpiBounds(form, minLogicalW, minLogicalH);
                RefreshFontsForDpi(form);
                onDpiRefresh();
                form.PerformLayout();
                form.Invalidate(true);
            }));
        };
    }

    /// <summary>
    /// Modal batean pantaila anitzeko PerMonitorV2 egokitzapen automatikoa aktibatzen du
    /// (Load eta DpiChanged gertaeretan tamaina, zentratzea, letra-tipoak eta layout-a berrituz).
    /// </summary>
    public static void AttachModalDpiAdaptation(Form modal, int logicalW, int logicalH, int minLogicalW, int minLogicalH, Action onDpiRefresh)
    {
        bool dpiRefreshQueued = false;

        modal.Load += (s, e) =>
        {
            if (modal.IsDisposed) return;
            RefreshFontsForDpi(modal);
            ApplyModalDpiSize(modal, logicalW, logicalH, minLogicalW, minLogicalH);
            onDpiRefresh();
            modal.PerformLayout();
            modal.Invalidate(true);
        };

        modal.DpiChanged += (s, e) =>
        {
            if (modal.IsDisposed || dpiRefreshQueued) return;
            dpiRefreshQueued = true;

            modal.BeginInvoke(new Action(() =>
            {
                dpiRefreshQueued = false;
                if (modal.IsDisposed) return;

                RefreshFontsForDpi(modal);
                ApplyModalDpiSize(modal, logicalW, logicalH, minLogicalW, minLogicalH);
                onDpiRefresh();
                modal.PerformLayout();
                modal.Invalidate(true);
            }));
        };
    }

    /// <summary>
    /// Modal bati DPI-arekin eskalatutako tamaina erosoa ezartzen dio (pantailaren %90 gainditu gabe)
    /// eta guraso-leihoaren erdian kokatzen du.
    /// </summary>
    public static void ApplyModalDpiSize(Form modal, int logicalW, int logicalH, int minLogicalW, int minLogicalH)
    {
        var targetScreen = modal.Owner != null
            ? Screen.FromControl(modal.Owner)
            : (ActiveScreen ?? Screen.FromControl(modal));
        var screen = targetScreen.WorkingArea;
        int targetW = Math.Min(S(modal, logicalW), (int)(screen.Width * 0.90));
        int targetH = Math.Min(S(modal, logicalH), (int)(screen.Height * 0.90));
        int minW = Math.Min(S(modal, minLogicalW), targetW);
        int minH = Math.Min(S(modal, minLogicalH), targetH);

        modal.StartPosition = FormStartPosition.Manual;
        modal.MinimumSize = new Size(minW, minH);
        modal.ClientSize = new Size(targetW, targetH);

        var refRect = modal.Owner != null ? modal.Owner.Bounds : screen;
        int x = Math.Clamp(refRect.Left + (refRect.Width - modal.Width) / 2, screen.Left, Math.Max(screen.Left, screen.Right - modal.Width));
        int y = Math.Clamp(refRect.Top + (refRect.Height - modal.Height) / 2, screen.Top, Math.Max(screen.Top, screen.Bottom - modal.Height));
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
        var f = BodyFont(btn, fontSize, bold);
        btn.Font = f;
        BaseFonts.AddOrUpdate(btn, new BaseFontSpec(f.FontFamily, fontSize, f.Style));
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
