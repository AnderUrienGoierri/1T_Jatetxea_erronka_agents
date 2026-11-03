using Jatetxea.Desktop.Models;
using Jatetxea.Desktop.Services;
using Jatetxea.Desktop.UI;

namespace Jatetxea.Desktop.Forms;

public class MahaiakForm : Form
{
    private readonly LangileaDto _langilea;
    private readonly ApiService _api;
    private List<MahaiaDto> _mahaiak = new();
    private string _aukeratutakoEremua = "Guztiak";

    private readonly FlowLayoutPanel _pnlEremuak;
    private readonly FlowLayoutPanel _pnlMahaiak;
    private readonly Label _lblLaburpena;
    private readonly System.Windows.Forms.Timer _tenporizadorea;

    public MahaiakForm(LangileaDto langilea, ApiService api)
    {
        _langilea = langilea;
        _api = api;

        Text = $"T1 Jatetxea - Mahaien Kudeaketa ({_langilea.Izena})";
        Size = new Size(1200, 780);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Theme.Background;
        MinimumSize = new Size(1000, 680);

        // GOIBURUA (Top header bar)
        var pnlHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 70,
            BackColor = Theme.Primary,
            Padding = new Padding(25, 12, 25, 12)
        };
        Controls.Add(pnlHeader);

        // Logo eta Titulua
        var lblLogo = new Label
        {
            Text = "T1",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = Theme.AccentWine,
            Size = new Size(46, 46),
            Location = new Point(25, 12),
            TextAlign = ContentAlignment.MiddleCenter
        };
        pnlHeader.Controls.Add(lblLogo);

        var lblIzenburua = new Label
        {
            Text = "T1 JATETXEA - MAHAIEN KUDEAKETA",
            Font = Theme.FontTitle,
            ForeColor = Color.White,
            AutoSize = true,
            Location = new Point(85, 20)
        };
        pnlHeader.Controls.Add(lblIzenburua);

        // Eskuineko Zerbitzari Panela eta Botoiak
        var pnlEskuina = new FlowLayoutPanel
        {
            Dock = DockStyle.Right,
            Width = 480,
            FlowDirection = FlowDirection.RightToLeft,
            BackColor = Color.Transparent
        };
        pnlHeader.Controls.Add(pnlEskuina);

        var btnLogout = new Button
        {
            Text = "SAIOA ITXI",
            Font = Theme.FontBodyBold,
            Size = new Size(110, 42),
            Margin = new Padding(8, 2, 0, 0),
            BackColor = Color.FromArgb(220, 38, 38),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnLogout.FlatAppearance.BorderSize = 0;
        btnLogout.Click += (s, e) => Close();
        pnlEskuina.Controls.Add(btnLogout);

        var btnFreskatu = new Button
        {
            Text = "FRESKATU",
            Font = Theme.FontBodyBold,
            Size = new Size(110, 42),
            Margin = new Padding(8, 2, 0, 0),
            BackColor = Color.FromArgb(51, 65, 85),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnFreskatu.FlatAppearance.BorderSize = 0;
        btnFreskatu.Click += async (s, e) => await KargatuMahaiakAsync();
        pnlEskuina.Controls.Add(btnFreskatu);

        var lblLangilea = new Label
        {
            Text = $"{_langilea.Izena} [Online]",
            Font = Theme.FontBodyBold,
            ForeColor = Theme.AccentGold,
            AutoSize = true,
            Margin = new Padding(0, 12, 10, 0)
        };
        pnlEskuina.Controls.Add(lblLangilea);

        // AZPI-GOIBURUA: Eremuak eta Egoera Laburpena
        var pnlAzpiGoiburua = new Panel
        {
            Dock = DockStyle.Top,
            Height = 65,
            BackColor = Theme.Surface,
            Padding = new Padding(25, 10, 25, 10)
        };
        Controls.Add(pnlAzpiGoiburua);

        _pnlEremuak = new FlowLayoutPanel
        {
            Dock = DockStyle.Left,
            Width = 600,
            AutoScroll = false,
            WrapContents = false,
            BackColor = Color.Transparent
        };
        pnlAzpiGoiburua.Controls.Add(_pnlEremuak);

        _lblLaburpena = new Label
        {
            Dock = DockStyle.Right,
            Width = 520,
            Font = Theme.FontSmall,
            ForeColor = Theme.TextMuted,
            TextAlign = ContentAlignment.MiddleRight
        };
        pnlAzpiGoiburua.Controls.Add(_lblLaburpena);

        // MAHAIEN SARETA PANELA
        _pnlMahaiak = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(25),
            BackColor = Theme.Background
        };
        Controls.Add(_pnlMahaiak);

        // Tenporizadorea automatikoki freskatzeko (15 segundoro)
        _tenporizadorea = new System.Windows.Forms.Timer
        {
            Interval = 15000
        };
        _tenporizadorea.Tick += async (s, e) => await KargatuMahaiakAsync(isSilent: true);

        Load += async (s, e) =>
        {
            await KargatuMahaiakAsync();
            _tenporizadorea.Start();
        };

        FormClosed += (s, e) => _tenporizadorea.Stop();
    }

    private async Task KargatuMahaiakAsync(bool isSilent = false)
    {
        if (!isSilent)
        {
            _lblLaburpena.Text = "Mahaiak kargatzen...";
        }

        var datuak = await _api.GetMahaiakAsync();
        if (datuak.Count > 0)
        {
            _mahaiak = datuak;
        }
        else if (_mahaiak.Count == 0)
        {
            // Demo mahaiak fallback gisa
            _mahaiak = new List<MahaiaDto>
            {
                new() { Id = 1, Zenbakia = 1, Edukiera = 4, Kokapena = "Jangela Nagusia", Egoera = "Libre" },
                new() { Id = 2, Zenbakia = 2, Edukiera = 4, Kokapena = "Jangela Nagusia", Egoera = "Okupatuta" },
                new() { Id = 3, Zenbakia = 3, Edukiera = 2, Kokapena = "Jangela Nagusia", Egoera = "Libre" },
                new() { Id = 4, Zenbakia = 4, Edukiera = 6, Kokapena = "Jangela Nagusia", Egoera = "Okupatuta" },
                new() { Id = 5, Zenbakia = 5, Edukiera = 4, Kokapena = "Terraza", Egoera = "Garbitzen" },
                new() { Id = 6, Zenbakia = 6, Edukiera = 2, Kokapena = "Terraza", Egoera = "Libre" },
                new() { Id = 7, Zenbakia = 7, Edukiera = 8, Kokapena = "Terraza", Egoera = "Erreserbatuta" },
                new() { Id = 8, Zenbakia = 8, Edukiera = 2, Kokapena = "Barra", Egoera = "Libre" },
                new() { Id = 9, Zenbakia = 9, Edukiera = 2, Kokapena = "Barra", Egoera = "Okupatuta" }
            };
        }

        EraikiEremuBotoiak();
        EguneratuLaburpena();
        BistaratuMahaiak();
    }

    private void EraikiEremuBotoiak()
    {
        _pnlEremuak.Controls.Clear();
        var eremuak = new List<string> { "Guztiak" };
        var eremuZerrenda = _mahaiak.Select(m => m.Kokapena).Distinct().OrderBy(k => k);
        eremuak.AddRange(eremuZerrenda);

        foreach (var eremua in eremuak)
        {
            var btn = new Button
            {
                Text = eremua,
                Font = Theme.FontBodyBold,
                Height = 42,
                AutoSize = true,
                Margin = new Padding(0, 0, 10, 0),
                BackColor = eremua == _aukeratutakoEremua ? Theme.AccentWine : Color.FromArgb(241, 245, 249),
                ForeColor = eremua == _aukeratutakoEremua ? Color.White : Theme.TextMain,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderColor = eremua == _aukeratutakoEremua ? Theme.AccentWine : Theme.Border;
            btn.Click += (s, e) =>
            {
                _aukeratutakoEremua = eremua;
                EraikiEremuBotoiak();
                BistaratuMahaiak();
            };
            _pnlEremuak.Controls.Add(btn);
        }
    }

    private void EguneratuLaburpena()
    {
        int guztira = _mahaiak.Count;
        int libre = _mahaiak.Count(m => m.Egoera.Equals("Libre", StringComparison.OrdinalIgnoreCase));
        int okupatuta = _mahaiak.Count(m => m.Egoera.Equals("Okupatuta", StringComparison.OrdinalIgnoreCase));
        int garbitzen = _mahaiak.Count(m => m.Egoera.Equals("Garbitzen", StringComparison.OrdinalIgnoreCase));
        int erreserbatuta = _mahaiak.Count(m => m.Egoera.Equals("Erreserbatuta", StringComparison.OrdinalIgnoreCase));

        _lblLaburpena.Text = $"Guztira: {guztira}  |  Libre: {libre}  |  Okupatuta: {okupatuta}  |  Garbitzen: {garbitzen}  |  Erreserbatuta: {erreserbatuta}";
    }

    private void BistaratuMahaiak()
    {
        _pnlMahaiak.Controls.Clear();

        var iragaziak = _aukeratutakoEremua == "Guztiak"
            ? _mahaiak
            : _mahaiak.Where(m => m.Kokapena == _aukeratutakoEremua).ToList();

        foreach (var mahaia in iragaziak)
        {
            var card = SortuMahaiTxartela(mahaia);
            _pnlMahaiak.Controls.Add(card);
        }
    }

    private Control SortuMahaiTxartela(MahaiaDto mahaia)
    {
        var (bgCol, borderCol, fgCol) = Theme.GetEgoeraKoloreak(mahaia.Egoera);

        var card = new Panel
        {
            Width = 240,
            Height = 170,
            Margin = new Padding(10),
            BackColor = Color.White,
            Cursor = Cursors.Hand,
            Padding = new Padding(14)
        };

        // Goiko barra: Mahai Zenbakia eta Edukiera
        var lblZenbakia = new Label
        {
            Text = $"Mahaia {mahaia.Zenbakia}",
            Font = Theme.FontTitle,
            ForeColor = Theme.TextMain,
            Location = new Point(14, 14),
            AutoSize = true
        };
        card.Controls.Add(lblZenbakia);

        var lblEdukiera = new Label
        {
            Text = $"{mahaia.Edukiera} lagun",
            Font = Theme.FontSmall,
            ForeColor = Theme.TextMuted,
            Location = new Point(card.Width - 85, 18),
            Size = new Size(70, 20),
            TextAlign = ContentAlignment.TopRight,
            Anchor = AnchorStyles.Top | AnchorStyles.Right
        };
        card.Controls.Add(lblEdukiera);

        // Kokapena
        var lblKokapena = new Label
        {
            Text = mahaia.Kokapena,
            Font = Theme.FontSmall,
            ForeColor = Theme.TextMuted,
            Location = new Point(14, 46),
            AutoSize = true
        };
        card.Controls.Add(lblKokapena);

        // Egoera Txapa
        var pnlEgoera = new Panel
        {
            Location = new Point(14, 76),
            Size = new Size(130, 30),
            BackColor = bgCol
        };
        var lblEgoera = new Label
        {
            Dock = DockStyle.Fill,
            Text = mahaia.Egoera.ToUpper(),
            Font = Theme.FontBadge,
            ForeColor = fgCol,
            TextAlign = ContentAlignment.MiddleCenter
        };
        pnlEgoera.Controls.Add(lblEgoera);
        card.Controls.Add(pnlEgoera);

        // Ekintza gonbidapena / oharra
        string ekintzaMezua = mahaia.Egoera.ToLowerInvariant() switch
        {
            "libre" => "Klikatu eskaera irekitzeko",
            "okupatuta" => "Klikatu kontua / lerroak ikusteko",
            "garbitzen" => "Klikatu garbituta markatzeko",
            _ => "Xehetasunak ikusi"
        };
        var lblEkintza = new Label
        {
            Text = ekintzaMezua,
            Font = Theme.FontSmall,
            ForeColor = fgCol,
            Location = new Point(14, 130),
            Size = new Size(210, 20),
            AutoEllipsis = true
        };
        card.Controls.Add(lblEkintza);

        // Klik ekitaldiak kontrol guztietan
        void OnClick(object? s, EventArgs e) => MahaiTxartelaKlikatua(mahaia);
        card.Click += OnClick;
        lblZenbakia.Click += OnClick;
        lblEdukiera.Click += OnClick;
        lblKokapena.Click += OnClick;
        pnlEgoera.Click += OnClick;
        lblEgoera.Click += OnClick;
        lblEkintza.Click += OnClick;

        card.Paint += (s, e) =>
        {
            using var pen = new Pen(borderCol, 2);
            e.Graphics.DrawRectangle(pen, 1, 1, card.Width - 3, card.Height - 3);
        };

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

                // 2. Eskaera / Komanda pantaila ireki
                using var modalEskaera = new EskaeraForm(mahaia, _langilea, _api, pertsonaKopurua);
                if (modalEskaera.ShowDialog() == DialogResult.OK)
                {
                    await KargatuMahaiakAsync();
                }
            }
        }
        else
        {
            // Okupatuta, Garbitzen edo Erreserbatuta: Xehetasunak ireki
            using var modalXehetasunak = new MahaiaXehetasunakModal(mahaia, _langilea, _api);
            if (modalXehetasunak.ShowDialog() == DialogResult.OK)
            {
                await KargatuMahaiakAsync();
            }
        }
    }
}
