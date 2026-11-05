using System.Drawing.Drawing2D;
using Jatetxea.Desktop.Models;
using Jatetxea.Desktop.Services;
using Jatetxea.Desktop.UI;

namespace Jatetxea.Desktop.Forms;

public partial class MahaiakForm : Form
{
    private readonly LangileaDto _langilea;
    private readonly ApiService _api;
    private List<MahaiaDto> _mahaiak = new();
    private string _aukeratutakoEremua = "Jangela Nagusia";
    private readonly System.Windows.Forms.Timer _tenporizadorea;



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

        // Goiko barraren marra banatzailea
        pnlTopBar.Paint += (s, e) =>
        {
            using var pen = new Pen(Color.FromArgb(226, 232, 240), 1.5f);
            e.Graphics.DrawLine(pen, 0, pnlTopBar.Height - 1, pnlTopBar.Width, pnlTopBar.Height - 1);
        };

        // Guneen fitxen azpiko marra banatzailea
        pnlTabStrip.Paint += (s, e) =>
        {
            using var pen = new Pen(Color.FromArgb(226, 232, 240), 1.5f);
            e.Graphics.DrawLine(pen, 0, pnlTabStrip.Height - 1, pnlTabStrip.Width, pnlTabStrip.Height - 1);
        };

        // Zerbitzariaren avatar borobila margotu
        SetupProfileAvatar();

        // Fitxen klikak lotu
        LotuFitxak();

        // Ekintza nagusiak
        btnFreskatu.Click += async (s, e) => await KargatuMahaiakAsync();
        btnLogout.Click += (s, e) => Close();

        // Tenporizadorea 15 segundoro freskatzeko
        _tenporizadorea = new System.Windows.Forms.Timer { Interval = 15000 };
        _tenporizadorea.Tick += async (s, e) => await KargatuMahaiakAsync(isSilent: true);

        Load += async (s, e) =>
        {
            EguneratuLangileProfila();
            await KargatuMahaiakAsync();
            _tenporizadorea.Start();
        };

        FormClosed += (s, e) => _tenporizadorea.Stop();
    }

    private void SetupProfileAvatar()
    {
        lblAvatarCircle.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Barruko hondo borobila
            using var bgBrush = new SolidBrush(Color.FromArgb(241, 245, 249));
            e.Graphics.FillEllipse(bgBrush, 2, 2, lblAvatarCircle.Width - 5, lblAvatarCircle.Height - 5);

            // Kanpoko ertz leuna
            using var pen = new Pen(Color.FromArgb(203, 213, 225), 1.5f);
            e.Graphics.DrawEllipse(pen, 2, 2, lblAvatarCircle.Width - 5, lblAvatarCircle.Height - 5);

            // Inizialak erdiratuta
            TextRenderer.DrawText(e.Graphics, lblAvatarCircle.Text, lblAvatarCircle.Font,
                new Rectangle(0, 0, lblAvatarCircle.Width, lblAvatarCircle.Height),
                Color.FromArgb(71, 85, 105),
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        };
    }

    private void EguneratuLangileProfila()
    {
        var zatiak = _langilea.Izena.Trim().Split(' ');
        string initials = string.Join("", zatiak.Select(z => z.Length > 0 ? z[0].ToString() : "")).ToUpper();
        if (initials.Length > 2) initials = initials.Substring(0, 2);

        lblServerName.Text = _langilea.Izena;
        lblAvatarCircle.Text = initials;
        lblServerStatus.Text = "● Online";
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

        // Fitxa aktiboaren azpiko marra nabarmendua margotu
        Button[] fitxak = { btnTabJangela, btnTabTerraza, btnTabBarra, btnTabVIP, btnTabGuztiak };
        foreach (var btn in fitxak)
        {
            btn.Paint += (s, e) =>
            {
                string garbia = btn.Text.Replace('\u00A0', ' ');
                bool isAktiboa = garbia.Equals(_aukeratutakoEremua, StringComparison.OrdinalIgnoreCase);
                if (isAktiboa)
                {
                    using var activePen = new Pen(Color.FromArgb(136, 19, 55), 3.5f);
                    e.Graphics.DrawLine(activePen, 4, btn.Height - 3, btn.Width - 4, btn.Height - 3);
                }
            };
        }

        FreskatuFitxenItxura();
    }

    private void AukeratuGunea(string gunea)
    {
        _aukeratutakoEremua = gunea;
        FreskatuFitxenItxura();
        lblSectionTitle.Text = gunea.ToUpper();
        BistaratuMahaiak();
    }

    private void FreskatuFitxenItxura()
    {
        Button[] fitxak = { btnTabJangela, btnTabTerraza, btnTabBarra, btnTabVIP, btnTabGuztiak };
        foreach (var btn in fitxak)
        {
            string garbia = btn.Text.Replace('\u00A0', ' ');
            bool isAktiboa = garbia.Equals(_aukeratutakoEremua, StringComparison.OrdinalIgnoreCase);
            btn.Font = new Font("Segoe UI", 10.5F, isAktiboa ? FontStyle.Bold : FontStyle.Regular);
            btn.ForeColor = isAktiboa ? Color.FromArgb(136, 19, 55) : Color.FromArgb(100, 116, 139);
            btn.Invalidate();
        }
    }

    private async Task KargatuMahaiakAsync(bool isSilent = false)
    {
        if (!isSilent)
        {
            lblSummary.Text = "Mahaiak kargatzen...";
        }

        var datuak = await _api.GetMahaiakAsync();
        _mahaiak = datuak;

        EguneratuLaburpena();
        BistaratuMahaiak();
    }

    private void EguneratuLaburpena()
    {
        int guztira = _mahaiak.Count;
        int libre = _mahaiak.Count(m => m.Egoera.Equals("Libre", StringComparison.OrdinalIgnoreCase));
        int okupatuta = _mahaiak.Count(m => m.Egoera.Equals("Okupatuta", StringComparison.OrdinalIgnoreCase));
        int eskaeraBidean = _mahaiak.Count(m => m.Egoera.Equals("Eskaera bidean", StringComparison.OrdinalIgnoreCase));
        int garbitzen = _mahaiak.Count(m => m.Egoera.Equals("Garbitzen", StringComparison.OrdinalIgnoreCase));

        lblSummary.Text = $"Guztira: {guztira} · Libre: {libre} · Okupatuta: {okupatuta} · Bidean: {eskaeraBidean} · Garbitzen: {garbitzen}";
    }

    private bool KokapenaBatDator(string mahaiaKokapena, string tabIzena)
    {
        if (string.IsNullOrWhiteSpace(tabIzena) || tabIzena.Equals("Guztiak", StringComparison.OrdinalIgnoreCase))
            return true;

        if (tabIzena.Contains("Jangela") || tabIzena.Contains("Jantoki"))
            return mahaiaKokapena.Contains("Jangela") || mahaiaKokapena.Contains("Jantoki");

        return mahaiaKokapena.Equals(tabIzena, StringComparison.OrdinalIgnoreCase);
    }

    private void BistaratuMahaiak()
    {
        pnlMahaiak.SuspendLayout();
        pnlMahaiak.Controls.Clear();

        if (_mahaiak.Count == 0)
        {
            var lblError = new Label
            {
                Text = "Ez da mahairik aurkitu datu-basean edo API konexioa falta da.\nMesedez, egiaztatu backend zerbitzaria (http://localhost:5000) martxan dagoela.",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 38, 38),
                AutoSize = false,
                Size = new Size(pnlMahaiak.Width - 40, 80),
                Location = new Point(20, 40),
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlMahaiak.Controls.Add(lblError);
            pnlMahaiak.ResumeLayout(true);
            return;
        }

        var iragaziak = _mahaiak.Where(m => KokapenaBatDator(m.Kokapena, _aukeratutakoEremua)).ToList();

        foreach (var mahaia in iragaziak)
        {
            var card = SortuMahaiTxartela(mahaia);
            pnlMahaiak.Controls.Add(card);
        }

        pnlMahaiak.ResumeLayout(true);
    }

    /// <summary>
    /// Zirriborroko (2-Mahaiak_interfazea.jpg) mahai-txartel garbia sortzen du
    /// </summary>
    private Control SortuMahaiTxartela(MahaiaDto mahaia)
    {
        var card = new Panel
        {
            Width = 390,
            Height = 220,
            Margin = new Padding(16),
            BackColor = Color.White,
            Cursor = Cursors.Hand
        };

        // 1. Izenburua ("Mahaia X")
        var lblZenbakia = new Label
        {
            Text = $"Mahaia {mahaia.Zenbakia}",
            Font = new Font("Segoe UI", 16F, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 41, 59),
            Location = new Point(22, 16),
            Size = new Size(240, 40),
            TextAlign = ContentAlignment.MiddleLeft,
            AutoSize = false
        };
        card.Controls.Add(lblZenbakia);

        // 2. Edukiera ("X seats" / "X leku")
        var lblSeats = new Label
        {
            Text = $"{mahaia.Edukiera} leku",
            Font = new Font("Segoe UI", 11.5F, FontStyle.Regular),
            ForeColor = Color.FromArgb(100, 116, 139),
            Location = new Point(22, 60),
            Size = new Size(180, 28),
            TextAlign = ContentAlignment.MiddleLeft,
            AutoSize = false
        };
        card.Controls.Add(lblSeats);

        // 3. Behe-ezkerra: Okupatutako denbora (APIak bidaltzen badu)
        bool isOkupatua = mahaia.Egoera.Equals("Okupatuta", StringComparison.OrdinalIgnoreCase);
        bool isEskaeraBidean = mahaia.Egoera.Equals("Eskaera bidean", StringComparison.OrdinalIgnoreCase);

        if ((isOkupatua || isEskaeraBidean) && !string.IsNullOrWhiteSpace(mahaia.Iraupena))
        {
            var lblTime = new Label
            {
                Text = mahaia.Iraupena,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                Location = new Point(22, 146),
                Size = new Size(160, 40),
                TextAlign = ContentAlignment.MiddleLeft
            };
            card.Controls.Add(lblTime);
        }

        // 4. Behe-eskuina: Egoera Txapa ("Librea", "Okupatua", "Eskaera bidean")
        string egoeraText = mahaia.Egoera switch
        {
            "Libre" => "Librea",
            "Okupatuta" => "Okupatua",
            "Eskaera bidean" => "Eskaera bidean",
            _ => mahaia.Egoera
        };

        var (badgeBg, badgeFg, badgeBorder) = mahaia.Egoera switch
        {
            "Libre" => (Color.FromArgb(220, 252, 231), Color.FromArgb(22, 101, 52), Color.FromArgb(134, 239, 172)),
            "Okupatuta" => (Color.FromArgb(51, 65, 85), Color.White, Color.FromArgb(30, 41, 59)),
            "Eskaera bidean" => (Color.FromArgb(254, 243, 199), Color.FromArgb(146, 64, 14), Color.FromArgb(253, 230, 138)),
            "Garbitzen" => (Color.FromArgb(241, 245, 249), Color.FromArgb(71, 85, 105), Color.FromArgb(203, 213, 225)),
            _ => (Color.FromArgb(219, 234, 254), Color.FromArgb(30, 64, 175), Color.FromArgb(147, 197, 253))
        };

        int badgeWidth = Math.Max(155, egoeraText.Length * 11 + 38);
        var pnlBadge = new Panel
        {
            Location = new Point(card.Width - badgeWidth - 22, 98),
            Size = new Size(badgeWidth, 42),
            BackColor = badgeBg
        };
        pnlBadge.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var pen = new Pen(badgeBorder, 1.2f);
            e.Graphics.DrawRectangle(pen, 0, 0, pnlBadge.Width - 1, pnlBadge.Height - 1);
        };

        var lblBadgeText = new Label
        {
            Dock = DockStyle.Fill,
            Text = egoeraText,
            Font = new Font("Segoe UI", 11.5F, FontStyle.Bold),
            ForeColor = badgeFg,
            TextAlign = ContentAlignment.MiddleCenter
        };
        pnlBadge.Controls.Add(lblBadgeText);
        card.Controls.Add(pnlBadge);

        // Okupatuta badago: PAX kopurua txaparen azpian ("4 PAX")
        if (isOkupatua || isEskaeraBidean)
        {
            var lblPax = new Label
            {
                Text = $"{mahaia.Edukiera} PAX",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105),
                Location = new Point(card.Width - badgeWidth - 22, 148),
                Size = new Size(badgeWidth, 34),
                TextAlign = ContentAlignment.MiddleRight
            };
            card.Controls.Add(lblPax);
        }

        // Txartelaren kanpoko marra
        card.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var pen = new Pen(Color.FromArgb(203, 213, 225), 1.5f);
            e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
        };

        // Klik egitean dagokion ekintza
        void OnClick(object? s, EventArgs e) => MahaiTxartelaKlikatua(mahaia);
        card.Click += OnClick;
        lblZenbakia.Click += OnClick;
        lblSeats.Click += OnClick;
        pnlBadge.Click += OnClick;
        lblBadgeText.Click += OnClick;

        return card;
    }

    private async void MahaiTxartelaKlikatua(MahaiaDto mahaia)
    {
        if (mahaia.Egoera.Equals("Libre", StringComparison.OrdinalIgnoreCase))
        {
            // 1. Mahaikide kopurua hautatzeko modala ireki
            using var modalKopurua = new MahaikideakModal(mahaia);
            if (modalKopurua.ShowDialog() == DialogResult.OK)
            {
                int pertsonaKopurua = modalKopurua.PertsonaKopurua;

                // 2. Eskaera / Komanda pantaila ireki (pantaila osoan)
                Hide();
                var formEskaera = new EskaeraForm(mahaia, _langilea, _api, pertsonaKopurua);
                formEskaera.FormClosed += async (s, e) =>
                {
                    Show();
                    await KargatuMahaiakAsync();
                };
                formEskaera.Show();
            }
        }
        else
        {
            // Okupatuta, Garbitzen edo Erreserbatuta: Mahaiaren xehetasunak (pantaila osoan)
            Hide();
            var formXehetasunak = new MahaiaXehetasunakForm(mahaia, _langilea, _api);
            formXehetasunak.FormClosed += async (s, e) =>
            {
                Show();
                await KargatuMahaiakAsync();
            };
            formXehetasunak.Show();
        }
    }
}
