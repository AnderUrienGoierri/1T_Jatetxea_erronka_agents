using System.Drawing.Drawing2D;
using Jatetxea.Desktop.Models;
using Jatetxea.Desktop.Services;
using Jatetxea.Desktop.UI;

namespace Jatetxea.Desktop.Forms;

public partial class MahaiakForm : Form
{
    private sealed class MahaiTxartelUi
    {
        public MahaiaDto Mahaia { get; set; } = null!;
        public Panel Card { get; init; } = null!;
        public Label LblZenbakia { get; init; } = null!;
        public Label LblSeats { get; init; } = null!;
        public Panel PnlBadge { get; init; } = null!;
        public Label LblBadgeText { get; init; } = null!;
        public Label LblBeheEzkerra { get; init; } = null!;
        public Label LblBeheEskuina { get; init; } = null!;
        public Button BtnLibreJarri { get; init; } = null!;
        public Color BadgeBg { get; set; }
        public Color BadgeBorder { get; set; }
        public bool IsHovered { get; set; }
    }

    private readonly LangileaDto _langilea;
    private readonly ApiService _api;
    private List<MahaiaDto> _mahaiak = new();
    private readonly Dictionary<int, MahaiTxartelUi> _txartelakById = new();
    private string _aukeratutakoEremua = "Jangela Nagusia";
    private readonly System.Windows.Forms.Timer _tenporizadorea;
    private bool _kargatzen = false;

    /// <summary>
    /// Visual Studio WinForms Designer-erako eraikitzailea
    /// </summary>
    public MahaiakForm() : this(new LangileaDto { Id = 1, Izena = "Ane G.", Rola = "Zerbitzaria" }, new ApiService())
    {
    }

    public MahaiakForm(LangileaDto langilea, ApiService api)
    {
        InitializeComponent();
        _langilea = langilea;
        _api = api;

        Text = $"T1 Jatetxea - Mahaiak ({_langilea.Izena})";

        // T1 Warm Hospitality v2.0 diseinua + parpadeorik gabeko DoubleBuffering aplikatu
        AplikatuEstiloBerria();

        // Goiko barraren marra banatzailea (Warm Taupe)
        pnlTopBar.Paint += (s, e) =>
        {
            using var pen = new Pen(Theme.WarmTaupe, 1.2f);
            e.Graphics.DrawLine(pen, 0, pnlTopBar.Height - 1, pnlTopBar.Width, pnlTopBar.Height - 1);
        };

        // Guneen fitxen azpiko marra banatzailea
        pnlTabStrip.Paint += (s, e) =>
        {
            using var pen = new Pen(Theme.WarmTaupe, 1.2f);
            e.Graphics.DrawLine(pen, 0, pnlTabStrip.Height - 1, pnlTabStrip.Width, pnlTabStrip.Height - 1);
        };

        // T1 monograma arkitektonikoa margotu (Android ic_t1_logo.xml & Figma)
        lblLogo.Text = string.Empty;
        lblLogo.BackColor = Theme.MarbleWhite;
        lblLogo.Paint += (s, e) => Theme.DrawT1Logo(e.Graphics, new Rectangle(0, 0, lblLogo.Width, lblLogo.Height));

        // Zerbitzariaren avatar borobila margotu
        SetupProfileAvatar();

        // Fitxen klikak lotu
        LotuFitxak();

        // Ekintza nagusiak
        btnErreserbak.Click += (s, e) =>
        {
            using var erreserbakForm = new ErreserbakForm(_langilea, _api, _mahaiak);
            erreserbakForm.ShowDialog(this);
            _ = KargatuMahaiakAsync(isSilent: true, behartuEguneratzea: true);
        };
        btnFreskatu.Click += async (s, e) => await KargatuMahaiakAsync(isSilent: false, behartuEguneratzea: true);
        btnLogout.Click += (s, e) => Close();

        // Tenporizadorea 3 segundoro AJAX-moduko diff freskatze isila egiteko (0 parpadeo)
        _tenporizadorea = new System.Windows.Forms.Timer { Interval = 3000 };
        _tenporizadorea.Tick += async (s, e) => await KargatuMahaiakAsync(isSilent: true);

        Load += async (s, e) =>
        {
            EguneratuLangileProfila();
            EgokituResponsiveDiseinua();
            await KargatuMahaiakAsync(isSilent: false, behartuEguneratzea: true);
            _tenporizadorea.Start();
        };

        Resize += (s, e) => EgokituResponsiveDiseinua();

        Theme.AttachMainFormDpiAdaptation(this, () =>
        {
            EgokituResponsiveDiseinua();
            EgokituMahaiTxartelak();
        });

        FormClosed += (s, e) =>
        {
            _tenporizadorea.Stop();
            _tenporizadorea.Dispose();
        };
    }

    private void AplikatuEstiloBerria()
    {
        Theme.ApplyWarmHospitalityTheme(this);

        BackColor = Theme.WarmLinen;
        pnlTopBar.BackColor = Theme.MarbleWhite;
        pnlProfile.BackColor = Theme.MarbleWhite;
        pnlTabStrip.BackColor = Theme.MarbleWhite;
        pnlContent.BackColor = Theme.WarmLinen;
        pnlSectionHeader.BackColor = Theme.WarmLinen;
        pnlMahaiak.BackColor = Theme.WarmLinen;

        lblBrand.ForeColor = Theme.ChiantiPrimary;
        lblBrand.Font = Theme.DisplayFont(15.5f);

        lblServerName.ForeColor = Theme.EspressoInk;
        lblServerName.Font = Theme.BodyFont(10.5f, bold: true);
        lblServerStatus.ForeColor = Theme.Success;
        lblServerStatus.Font = Theme.BodyFont(9f, bold: true);

        lblSectionTitle.ForeColor = Theme.EspressoInk;
        lblSectionTitle.Font = Theme.BodyFont(14.5f, bold: true);
        lblSummary.ForeColor = Theme.WarmSlate;
        lblSummary.Font = Theme.MonoFont(9.5f, bold: false);

        Theme.StyleButton(btnErreserbak, Theme.PurpleBg, Theme.Purple, Theme.PurpleBorder, radius: 10, fontSize: 9.5f);
        Theme.StyleButton(btnFreskatu, Theme.StoneMist, Theme.EspressoInk, Theme.WarmTaupe, radius: 10, fontSize: 9.5f);
        Theme.StyleButton(btnLogout, Theme.ChiantiPrimary, Theme.MarbleWhite, null, radius: 10, fontSize: 9.5f);
    }

    private void EgokituResponsiveDiseinua()
    {
        int S(int px) => Theme.S(this, px);
        bool estua = ClientSize.Width < S(1040);

        // 1. Goiko barra DPI-rekin eskalatuta, gainjarpenik eta testu-mozketarik gabe
        pnlTopBar.Height = ClientSize.Height < S(580) ? S(68) : S(84);
        int yBtn = (pnlTopBar.Height - S(42)) / 2;

        int logoSz = S(48);
        lblLogo.Size = new Size(logoSz, logoSz);
        lblLogo.Location = new Point(S(18), (pnlTopBar.Height - logoSz) / 2);

        lblBrand.Location = new Point(lblLogo.Right + S(12), (pnlTopBar.Height - logoSz) / 2);
        lblBrand.Text = estua ? "T1 MAHAIAK" : "T1 JATETXEA · MAHAIAK";
        lblBrand.Font = Theme.DisplayFont(this, estua ? 12.5f : 15f);
        lblBrand.Size = new Size(estua ? S(200) : S(390), logoSz);

        int btnH = S(42);
        int gap = S(10);
        int wErr = S(132);
        int wFres = S(104);
        int wOut = S(116);

        if (estua)
        {
            lblAvatarCircle.Visible = false;
            int wName = S(115);
            pnlProfile.Width = wName + wErr + wFres + wOut + gap * 4 + S(14);
            lblServerName.Location = new Point(S(4), yBtn);
            lblServerName.Size = new Size(wName, S(22));
            lblServerStatus.Location = new Point(S(4), yBtn + S(21));
            lblServerStatus.Size = new Size(wName, S(20));

            int x = lblServerName.Right + gap;
            btnErreserbak.SetBounds(x, yBtn, wErr, btnH);
            btnFreskatu.SetBounds(btnErreserbak.Right + gap, yBtn, wFres, btnH);
            btnLogout.SetBounds(btnFreskatu.Right + gap, yBtn, wOut, btnH);
        }
        else
        {
            lblAvatarCircle.Visible = true;
            int avSz = S(46);
            int wName = S(130);
            pnlProfile.Width = avSz + wName + wErr + wFres + wOut + gap * 5 + S(20);
            lblAvatarCircle.SetBounds(S(6), (pnlTopBar.Height - avSz) / 2, avSz, avSz);
            lblServerName.SetBounds(lblAvatarCircle.Right + S(10), yBtn, wName, S(22));
            lblServerStatus.SetBounds(lblAvatarCircle.Right + S(10), yBtn + S(21), wName, S(20));

            int x = lblServerName.Right + gap;
            btnErreserbak.SetBounds(x, yBtn, wErr, btnH);
            btnFreskatu.SetBounds(btnErreserbak.Right + gap, yBtn, wFres, btnH);
            btnLogout.SetBounds(btnFreskatu.Right + gap, yBtn, wOut, btnH);
        }

        // Guneen fitxa-barra DPI-rekin eskalatuta
        pnlTabStrip.Height = S(58);
        pnlTabStrip.Padding = new Padding(S(24), S(9), S(24), S(9));
        pnlSectionHeader.Height = S(50);
        pnlContent.Padding = new Padding(S(24), S(14), S(24), S(18));

        Button[] fitxak = { btnTabJangela, btnTabTerraza, btnTabBarra, btnTabVIP, btnTabGuztiak };
        foreach (var btn in fitxak)
        {
            btn.Height = S(38);
            btn.Padding = new Padding(S(18), 0, S(18), 0);
            btn.Margin = new Padding(0, 0, S(10), 0);
            var sz = TextRenderer.MeasureText(btn.Text.Replace('\u00A0', ' '), btn.Font);
            btn.AutoSize = false;
            btn.Width = sz.Width + S(36);
        }

        // 2. Mahai txartelen zabalera dinamikoa (pantaila osoa aprobetxatuz)
        EgokituMahaiTxartelak();
    }

    private void EgokituMahaiTxartelak()
    {
        int S(int px) => Theme.S(this, px);
        int availW = pnlMahaiak.ClientSize.Width - pnlMahaiak.Padding.Horizontal - S(26);
        if (availW < S(280)) return;

        int minCardW = S(300);
        int cols = Math.Max(2, availW / S(360));
        if (availW < S(700)) cols = 2;
        if (availW < S(520)) cols = 1;

        int marginX = S(10);
        int cardW = Math.Max(minCardW, (availW - (cols * marginX * 2)) / cols);
        int cardH = S(196);

        pnlMahaiak.SuspendLayout();
        foreach (Control ctrl in pnlMahaiak.Controls)
        {
            if (ctrl is Panel card && card.Tag is MahaiTxartelUi ui)
            {
                card.Size = new Size(cardW, cardH);
                card.Margin = new Padding(marginX);
                EguneratuTxartelGeometria(ui);
                card.Invalidate();
            }
        }
        pnlMahaiak.ResumeLayout(true);
    }

    private void EguneratuTxartelGeometria(MahaiTxartelUi ui)
    {
        int S(int px) => Theme.S(this, px);
        int pad = S(20);
        int cardW = ui.Card.Width;
        int cardH = ui.Card.Height;

        var badgeSz = TextRenderer.MeasureText(ui.LblBadgeText.Text, ui.LblBadgeText.Font);
        int badgeW = Math.Max(S(112), badgeSz.Width + S(28));
        int badgeH = S(32);

        ui.PnlBadge.SetBounds(cardW - badgeW - pad, S(18), badgeW, badgeH);
        ui.LblZenbakia.SetBounds(pad, S(14), Math.Max(S(120), ui.PnlBadge.Left - pad - S(10)), S(34));
        ui.LblSeats.SetBounds(pad, S(54), cardW - pad * 2, S(28));

        int footerY = cardH - S(52);
        ui.LblBeheEzkerra.SetBounds(pad, footerY, (cardW - pad * 2) / 2 - S(6), S(34));
        ui.LblBeheEskuina.SetBounds(cardW / 2, footerY, (cardW - pad * 2) / 2, S(34));

        int btnW = S(138);
        int btnH = S(38);
        ui.BtnLibreJarri.SetBounds(cardW - btnW - pad, cardH - btnH - S(14), btnW, btnH);
    }

    private void SetupProfileAvatar()
    {
        lblAvatarCircle.BackColor = Theme.MarbleWhite;
        lblAvatarCircle.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using var bgBrush = new SolidBrush(Theme.StoneMist);
            e.Graphics.FillEllipse(bgBrush, 2, 2, lblAvatarCircle.Width - 5, lblAvatarCircle.Height - 5);

            using var pen = new Pen(Theme.GoldBorder, 1.6f);
            e.Graphics.DrawEllipse(pen, 2, 2, lblAvatarCircle.Width - 5, lblAvatarCircle.Height - 5);

            using var avatarFont = Theme.BodyFont(this, 12f, bold: true);
            TextRenderer.DrawText(e.Graphics, lblAvatarCircle.Text, avatarFont,
                new Rectangle(0, 0, lblAvatarCircle.Width, lblAvatarCircle.Height),
                Theme.ChiantiPrimary,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        };
    }

    private void EguneratuLangileProfila()
    {
        var zatiak = _langilea.Izena.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        string initials = string.Join("", zatiak.Select(z => z[0].ToString())).ToUpper();
        if (initials.Length > 2) initials = initials.Substring(0, 2);

        lblServerName.Text = _langilea.Izena;
        lblAvatarCircle.Text = initials;
        lblServerStatus.Text = "● Online";
        lblAvatarCircle.Invalidate();
    }

    private void LotuFitxak()
    {
        btnTabJangela.Text = "Jangela\u00A0Nagusia";
        btnTabTerraza.Text = "Terraza";
        btnTabBarra.Text = "Barra";
        btnTabVIP.Text = "VIP";
        btnTabGuztiak.Text = "Guztiak";

        btnTabJangela.Click += (s, e) => AukeratuGunea("Jangela Nagusia");
        btnTabTerraza.Click += (s, e) => AukeratuGunea("Terraza");
        btnTabBarra.Click += (s, e) => AukeratuGunea("Barra");
        btnTabVIP.Click += (s, e) => AukeratuGunea("VIP");
        btnTabGuztiak.Click += (s, e) => AukeratuGunea("Guztiak");

        pnlTabStrip.Padding = new Padding(24, 9, 24, 9);
        pnlTabStrip.Height = 60;

        Button[] fitxak = { btnTabJangela, btnTabTerraza, btnTabBarra, btnTabVIP, btnTabGuztiak };
        foreach (var btn in fitxak)
        {
            btn.Height = 40;
            btn.Margin = new Padding(0, 0, 10, 0);
            btn.Padding = new Padding(16, 0, 16, 0);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Theme.StoneMist;
            btn.Paint += (s, e) =>
            {
                string garbia = btn.Text.Replace('\u00A0', ' ');
                bool isAktiboa = garbia.Equals(_aukeratutakoEremua, StringComparison.OrdinalIgnoreCase);

                e.Graphics.Clear(Theme.MarbleWhite);
                if (isAktiboa)
                {
                    Theme.DrawRoundedCard(e.Graphics, new Rectangle(1, 1, btn.Width - 2, btn.Height - 2), 19, Theme.ChiantiTint, Theme.ChiantiPrimary, 1.4f);
                }

                TextRenderer.DrawText(
                    e.Graphics,
                    garbia,
                    btn.Font,
                    new Rectangle(0, 0, btn.Width, btn.Height),
                    isAktiboa ? Theme.ChiantiPrimary : Theme.WarmSlate,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            };
        }

        FreskatuFitxenItxura();
    }

    private void AukeratuGunea(string gunea)
    {
        if (_aukeratutakoEremua.Equals(gunea, StringComparison.OrdinalIgnoreCase)) return;
        _aukeratutakoEremua = gunea;
        FreskatuFitxenItxura();
        lblSectionTitle.Text = gunea.ToUpperInvariant();
        BistaratuMahaiak(behartuBerreraiki: true);
    }

    private void FreskatuFitxenItxura()
    {
        Button[] fitxak = { btnTabJangela, btnTabTerraza, btnTabBarra, btnTabVIP, btnTabGuztiak };
        foreach (var btn in fitxak)
        {
            string garbia = btn.Text.Replace('\u00A0', ' ');
            bool isAktiboa = garbia.Equals(_aukeratutakoEremua, StringComparison.OrdinalIgnoreCase);
            btn.Font = Theme.BodyFont(10f, bold: isAktiboa);
            btn.ForeColor = isAktiboa ? Theme.ChiantiPrimary : Theme.WarmSlate;
            btn.BackColor = Theme.MarbleWhite;
            btn.Invalidate();
        }
    }

    /// <summary>
    /// AJAX-moduko diff freskatzea: soilik datuak aldatu badira eguneratzen du UI-a,
    /// eta ahal den guztietan txartelak lekuan bertan (in-place) patched egiten ditu 0 parpadeorekin.
    /// </summary>
    private async Task KargatuMahaiakAsync(bool isSilent = false, bool behartuEguneratzea = false)
    {
        if (_kargatzen) return;
        _kargatzen = true;

        try
        {
            if (!isSilent && _mahaiak.Count == 0)
            {
                lblSummary.Text = "Mahaiak kargatzen...";
            }

            var datuak = await _api.GetMahaiakAsync();
            if (IsDisposed) return;

            bool aldatuDa = behartuEguneratzea || MahaiakAldatuDira(_mahaiak, datuak);
            if (!aldatuDa)
            {
                return; // Ezer ez da aldatu -> 0 UI ukitze, 0 parpadeo!
            }

            _mahaiak = datuak;
            EguneratuLaburpena();
            BistaratuMahaiak(behartuBerreraiki: false);
        }
        catch
        {
            if (!isSilent && !IsDisposed)
            {
                lblSummary.Text = "API konexio errorea — berriro saiatzen...";
            }
        }
        finally
        {
            _kargatzen = false;
        }
    }

    private static bool MahaiakAldatuDira(List<MahaiaDto> zaharra, List<MahaiaDto> berria)
    {
        if (zaharra.Count != berria.Count) return true;

        for (int i = 0; i < zaharra.Count; i++)
        {
            var a = zaharra[i];
            var b = berria[i];
            if (a.Id != b.Id ||
                a.Zenbakia != b.Zenbakia ||
                a.Edukiera != b.Edukiera ||
                !string.Equals(a.Kokapena, b.Kokapena, StringComparison.Ordinal) ||
                !string.Equals(a.Egoera, b.Egoera, StringComparison.OrdinalIgnoreCase) ||
                a.Mahaikideak != b.Mahaikideak ||
                !string.Equals(a.Iraupena, b.Iraupena, StringComparison.Ordinal) ||
                a.AktiboEskaeraId != b.AktiboEskaeraId)
            {
                return true;
            }
        }

        return false;
    }

    private void EguneratuLaburpena()
    {
        int guztira = _mahaiak.Count;
        int libre = _mahaiak.Count(m => m.Egoera.Equals("Libre", StringComparison.OrdinalIgnoreCase));
        int okupatuta = _mahaiak.Count(m => m.Egoera.Equals("Okupatuta", StringComparison.OrdinalIgnoreCase));
        int sukaldean = _mahaiak.Count(m => m.Egoera.Equals("Eskaera bidean", StringComparison.OrdinalIgnoreCase) || m.Egoera.Equals("Sukaldean", StringComparison.OrdinalIgnoreCase));
        int garbitzen = _mahaiak.Count(m => m.Egoera.Equals("Garbitzen", StringComparison.OrdinalIgnoreCase));

        string laburpena = sukaldean > 0
            ? $"Guztira: {guztira} · Libre: {libre} · Okupatuta: {okupatuta} · Sukaldean: {sukaldean} · Garbitzen: {garbitzen}"
            : $"Guztira: {guztira} · Libre: {libre} · Okupatuta: {okupatuta} · Garbitzen: {garbitzen}";

        if (lblSummary.Text != laburpena)
        {
            lblSummary.Text = laburpena;
        }
    }

    private bool KokapenaBatDator(string mahaiaKokapena, string tabIzena)
    {
        if (string.IsNullOrWhiteSpace(tabIzena) || tabIzena.Equals("Guztiak", StringComparison.OrdinalIgnoreCase))
            return true;

        if (tabIzena.Contains("Jangela") || tabIzena.Contains("Jantoki"))
            return mahaiaKokapena.Contains("Jangela") || mahaiaKokapena.Contains("Jantoki");

        return mahaiaKokapena.Equals(tabIzena, StringComparison.OrdinalIgnoreCase);
    }

    private void BistaratuMahaiak(bool behartuBerreraiki = false)
    {
        if (_mahaiak.Count == 0)
        {
            UiPerf.RunFlickerFree(pnlMahaiak, () =>
            {
                _txartelakById.Clear();
                pnlMahaiak.Controls.Clear();
                var lblError = new Label
                {
                    Text = "Ez da mahairik aurkitu datu-basean edo API konexioa falta da.\nMesedez, egiaztatu backend zerbitzaria (http://localhost:5050) martxan dagoela.",
                    Font = Theme.BodyFont(11.5f, bold: true),
                    ForeColor = Theme.Danger,
                    AutoSize = false,
                    Size = new Size(Math.Max(420, pnlMahaiak.ClientSize.Width - 60), 80),
                    Location = new Point(20, 40),
                    TextAlign = ContentAlignment.MiddleCenter
                };
                pnlMahaiak.Controls.Add(lblError);
            }, preserveScroll: false);
            return;
        }

        var iragaziak = _mahaiak.Where(m => KokapenaBatDator(m.Kokapena, _aukeratutakoEremua)).ToList();

        // In-place patching: ID multzoa berdina bada, EZ ezabatu kontrolik (0 parpadeo eta 0 scroll-jauzi)
        bool idMultzoBerdina = !behartuBerreraiki
            && _txartelakById.Count == iragaziak.Count
            && iragaziak.All(m => _txartelakById.ContainsKey(m.Id));

        if (idMultzoBerdina)
        {
            foreach (var mahaia in iragaziak)
            {
                var ui = _txartelakById[mahaia.Id];
                EguneratuTxartelaInPlace(ui, mahaia);
            }
            return;
        }

        // Gunea aldatu da edo mahai berriak daude -> WM_SETREDRAW blokeoarekin parpadeorik gabe berreraiki
        UiPerf.RunFlickerFree(pnlMahaiak, () =>
        {
            _txartelakById.Clear();
            pnlMahaiak.Controls.Clear();

            foreach (var mahaia in iragaziak)
            {
                var ui = SortuMahaiTxartela(mahaia);
                _txartelakById[mahaia.Id] = ui;
                pnlMahaiak.Controls.Add(ui.Card);
            }
        }, preserveScroll: !behartuBerreraiki);

        EgokituMahaiTxartelak();
    }

    /// <summary>
    /// T1 Warm Hospitality estiloko mahai-txartela sortzen du (Figma 02 · MahaiakForm bezala).
    /// </summary>
    private MahaiTxartelUi SortuMahaiTxartela(MahaiaDto mahaia)
    {
        var card = new Panel
        {
            Width = 370,
            Height = 204,
            Margin = new Padding(12),
            BackColor = Theme.WarmLinen,
            Cursor = Cursors.Hand
        };

        var lblZenbakia = new Label
        {
            Font = Theme.BodyFont(15.5f, bold: true),
            ForeColor = Theme.EspressoInk,
            BackColor = Theme.MarbleWhite,
            Location = new Point(22, 18),
            Size = new Size(195, 34),
            TextAlign = ContentAlignment.MiddleLeft,
            AutoSize = false
        };

        var lblSeats = new Label
        {
            Font = Theme.BodyFont(10.5f, bold: false),
            ForeColor = Theme.WarmSlate,
            BackColor = Theme.MarbleWhite,
            Location = new Point(22, 54),
            Size = new Size(210, 26),
            TextAlign = ContentAlignment.MiddleLeft,
            AutoSize = false
        };

        var pnlBadge = new Panel
        {
            Location = new Point(card.Width - 156, 22),
            Size = new Size(136, 34),
            BackColor = Theme.MarbleWhite,
            Tag = "right-aligned"
        };

        var lblBadgeText = new Label
        {
            Dock = DockStyle.Fill,
            Font = Theme.BodyFont(9.5f, bold: true),
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleCenter
        };
        pnlBadge.Controls.Add(lblBadgeText);

        var lblBeheEzkerra = new Label
        {
            Font = Theme.MonoFont(13f, bold: true),
            ForeColor = Theme.ChiantiPrimary,
            BackColor = Theme.MarbleWhite,
            Location = new Point(22, 146),
            Size = new Size(165, 34),
            TextAlign = ContentAlignment.MiddleLeft,
            AutoSize = false
        };

        var lblBeheEskuina = new Label
        {
            Font = Theme.BodyFont(10f, bold: true),
            ForeColor = Theme.WarmSlate,
            BackColor = Theme.MarbleWhite,
            Location = new Point(card.Width - 170, 146),
            Size = new Size(150, 34),
            TextAlign = ContentAlignment.MiddleRight,
            Tag = "right-aligned"
        };

        var btnLibreJarri = new Button
        {
            Text = "✓ Libre jarri",
            Location = new Point(card.Width - 160, 140),
            Size = new Size(140, 40),
            Visible = false,
            Tag = "right-aligned"
        };
        Theme.StyleButton(btnLibreJarri, Theme.Success, Theme.MarbleWhite, null, radius: 10, fontSize: 10f);

        card.Controls.Add(lblZenbakia);
        card.Controls.Add(lblSeats);
        card.Controls.Add(pnlBadge);
        card.Controls.Add(lblBeheEzkerra);
        card.Controls.Add(lblBeheEskuina);
        card.Controls.Add(btnLibreJarri);

        var ui = new MahaiTxartelUi
        {
            Mahaia = mahaia,
            Card = card,
            LblZenbakia = lblZenbakia,
            LblSeats = lblSeats,
            PnlBadge = pnlBadge,
            LblBadgeText = lblBadgeText,
            LblBeheEzkerra = lblBeheEzkerra,
            LblBeheEskuina = lblBeheEskuina,
            BtnLibreJarri = btnLibreJarri
        };
        card.Tag = ui;

        // Txartelaren gorputz biribildua (Warm Hospitality MarbleWhite + WarmTaupe/GoldBorder hover)
        card.Paint += (s, e) =>
        {
            Color borderCol = ui.IsHovered ? Theme.GoldBorder : Theme.WarmTaupe;
            float borderW = ui.IsHovered ? 1.8f : 1.3f;
            Theme.DrawRoundedCard(e.Graphics, new Rectangle(0, 0, card.Width, card.Height), 16, Theme.MarbleWhite, borderCol, borderW);

            // Azpiko banatzaile fina (txartel barruan, DPI-rekin eskalatuta)
            int divY = card.Height - Theme.S(this, 64);
            using var divPen = new Pen(Theme.StoneMist, 1.2f);
            e.Graphics.DrawLine(divPen, Theme.S(this, 20), divY, card.Width - Theme.S(this, 20), divY);
        };

        // Egoera pilula biribildua
        pnlBadge.Paint += (s, e) =>
        {
            Theme.DrawRoundedCard(e.Graphics, new Rectangle(0, 0, pnlBadge.Width, pnlBadge.Height), 16, ui.BadgeBg, ui.BadgeBorder, 1.2f);
        };

        btnLibreJarri.Click += async (s, e) =>
        {
            var m = ui.Mahaia;
            btnLibreJarri.Enabled = false;
            btnLibreJarri.Text = "Eguneratzen...";
            bool ok = await _api.EguneratuMahaiarenEgoeraAsync(m.Id, "Libre");
            if (ok)
            {
                m.Egoera = "Libre";
                await KargatuMahaiakAsync(isSilent: true, behartuEguneratzea: true);
            }
            else
            {
                MessageBox.Show("Ezin izan da mahaiaren egoera eguneratu APIan.", "Errorea", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnLibreJarri.Enabled = true;
                btnLibreJarri.Text = "✓ Libre jarri";
            }
        };

        void OnClick(object? s, EventArgs e) => MahaiTxartelaKlikatua(ui.Mahaia);
        void OnEnter(object? s, EventArgs e) { if (!ui.IsHovered) { ui.IsHovered = true; card.Invalidate(); } }
        void OnLeave(object? s, EventArgs e)
        {
            if (ui.IsHovered && !card.ClientRectangle.Contains(card.PointToClient(Cursor.Position)))
            {
                ui.IsHovered = false;
                card.Invalidate();
            }
        }

        Control[] clickable = { card, lblZenbakia, lblSeats, pnlBadge, lblBadgeText, lblBeheEzkerra, lblBeheEskuina };
        foreach (var ctrl in clickable)
        {
            ctrl.Click += OnClick;
            ctrl.MouseEnter += OnEnter;
            ctrl.MouseLeave += OnLeave;
        }

        EguneratuTxartelaInPlace(ui, mahaia);
        return ui;
    }

    /// <summary>
    /// Txartel baten datuak lekuan bertan eguneratzen ditu kontrolik suntsitu gabe (0 parpadeo).
    /// </summary>
    private void EguneratuTxartelaInPlace(MahaiTxartelUi ui, MahaiaDto mahaia)
    {
        ui.Mahaia = mahaia;

        string tit = $"Mahaia {mahaia.Zenbakia}";
        if (ui.LblZenbakia.Text != tit) ui.LblZenbakia.Text = tit;

        string seats = string.IsNullOrWhiteSpace(mahaia.Kokapena)
            ? $"{mahaia.Edukiera} leku"
            : $"{mahaia.Edukiera} leku · {mahaia.Kokapena}";
        if (ui.LblSeats.Text != seats) ui.LblSeats.Text = seats;

        string egoeraText = mahaia.Egoera switch
        {
            "Libre" => "● Librea",
            "Okupatuta" => "● Okupatuta",
            "Eskaera bidean" => "● Sukaldean",
            "Garbitzen" => "● Garbitzen",
            _ => $"● {mahaia.Egoera}"
        };

        var (badgeBg, badgeBorder, badgeFg) = Theme.GetEgoeraKoloreak(mahaia.Egoera);
        bool badgeChanged = ui.BadgeBg != badgeBg || ui.BadgeBorder != badgeBorder || ui.LblBadgeText.Text != egoeraText;

        ui.BadgeBg = badgeBg;
        ui.BadgeBorder = badgeBorder;
        ui.LblBadgeText.Text = egoeraText;
        ui.LblBadgeText.ForeColor = badgeFg;

        if (badgeChanged)
        {
            ui.PnlBadge.Invalidate();
        }

        bool isLibre = mahaia.Egoera.Equals("Libre", StringComparison.OrdinalIgnoreCase);
        bool isGarbitzen = mahaia.Egoera.Equals("Garbitzen", StringComparison.OrdinalIgnoreCase);
        bool isOkupatua = mahaia.Egoera.Equals("Okupatuta", StringComparison.OrdinalIgnoreCase) ||
                          mahaia.Egoera.Equals("Eskaera bidean", StringComparison.OrdinalIgnoreCase);

        if (isGarbitzen)
        {
            ui.LblBeheEzkerra.Text = "Garbitzeko zain";
            ui.LblBeheEzkerra.Font = Theme.BodyFont(10f, bold: false);
            ui.LblBeheEzkerra.ForeColor = Theme.Info;
            ui.LblBeheEskuina.Visible = false;
            ui.BtnLibreJarri.Visible = true;
            ui.BtnLibreJarri.Enabled = true;
            ui.BtnLibreJarri.Text = "✓ Libre jarri";
        }
        else if (isOkupatua)
        {
            ui.LblBeheEzkerra.Text = !string.IsNullOrWhiteSpace(mahaia.Iraupena) ? mahaia.Iraupena : "Zerbitzuan";
            ui.LblBeheEzkerra.Font = Theme.MonoFont(11.5f, bold: true);
            ui.LblBeheEzkerra.ForeColor = Theme.ChiantiPrimary;

            int pax = mahaia.Mahaikideak ?? mahaia.Edukiera;
            ui.LblBeheEskuina.Text = $"{pax} PAX";
            ui.LblBeheEskuina.ForeColor = Theme.EspressoInk;
            ui.LblBeheEskuina.Visible = true;
            ui.BtnLibreJarri.Visible = false;
        }
        else if (isLibre)
        {
            ui.LblBeheEzkerra.Text = "Prest";
            ui.LblBeheEzkerra.Font = Theme.BodyFont(10f, bold: false);
            ui.LblBeheEzkerra.ForeColor = Theme.StoneMuted;

            ui.LblBeheEskuina.Text = string.Empty;
            ui.LblBeheEskuina.Visible = false;
            ui.BtnLibreJarri.Visible = false;
        }
        else
        {
            ui.LblBeheEzkerra.Text = mahaia.Egoera;
            ui.LblBeheEzkerra.Font = Theme.BodyFont(10f, bold: true);
            ui.LblBeheEzkerra.ForeColor = Theme.Purple;

            ui.LblBeheEskuina.Text = string.Empty;
            ui.LblBeheEskuina.Visible = false;
            ui.BtnLibreJarri.Visible = false;
        }

        if (ui.Card.Parent != null)
        {
            EguneratuTxartelGeometria(ui);
        }
    }

    private async void MahaiTxartelaKlikatua(MahaiaDto mahaia)
    {
        if (mahaia.Egoera.Equals("Libre", StringComparison.OrdinalIgnoreCase))
        {
            using var modalKopurua = new MahaikideakModal(mahaia);
            if (modalKopurua.ShowDialog(this) == DialogResult.OK)
            {
                int pertsonaKopurua = modalKopurua.PertsonaKopurua;

                var formEskaera = new EskaeraForm(mahaia, _langilea, _api, pertsonaKopurua);
                Theme.PlaceOnActiveScreen(formEskaera, this);
                formEskaera.FormClosed += async (s, e) =>
                {
                    Theme.PlaceOnActiveScreen(this);
                    Show();
                    await KargatuMahaiakAsync(isSilent: true, behartuEguneratzea: true);
                };
                Hide();
                formEskaera.Show();
            }
        }
        else if (mahaia.Egoera.Equals("Garbitzen", StringComparison.OrdinalIgnoreCase))
        {
            var erantzuna = MessageBox.Show(
                $"Mahaia {mahaia.Zenbakia} 'Garbitzen' egoeran dago.\n\nMahaia garbituta dago eta 'Librea' egoerara aldatu nahi duzu?",
                $"Mahaia {mahaia.Zenbakia} - Garbiketa",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);

            if (erantzuna == DialogResult.Yes)
            {
                bool ok = await _api.EguneratuMahaiarenEgoeraAsync(mahaia.Id, "Libre");
                if (ok)
                {
                    mahaia.Egoera = "Libre";
                    await KargatuMahaiakAsync(isSilent: true, behartuEguneratzea: true);
                }
                else
                {
                    MessageBox.Show("Ezin izan da mahaiaren egoera eguneratu APIan.", "Errorea", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }
            else if (erantzuna == DialogResult.Cancel)
            {
                return;
            }

            var formXehetasunak = new MahaiaXehetasunakForm(mahaia, _langilea, _api);
            Theme.PlaceOnActiveScreen(formXehetasunak, this);
            formXehetasunak.FormClosed += async (s, e) =>
            {
                Theme.PlaceOnActiveScreen(this);
                Show();
                await KargatuMahaiakAsync(isSilent: true, behartuEguneratzea: true);
            };
            Hide();
            formXehetasunak.Show();
        }
        else
        {
            var formXehetasunak = new MahaiaXehetasunakForm(mahaia, _langilea, _api);
            Theme.PlaceOnActiveScreen(formXehetasunak, this);
            formXehetasunak.FormClosed += async (s, e) =>
            {
                Theme.PlaceOnActiveScreen(this);
                Show();
                await KargatuMahaiakAsync(isSilent: true, behartuEguneratzea: true);
            };
            Hide();
            formXehetasunak.Show();
        }
    }
}
