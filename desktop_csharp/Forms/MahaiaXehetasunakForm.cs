using System.Drawing.Drawing2D;
using Jatetxea.Desktop.Models;
using Jatetxea.Desktop.Services;

namespace Jatetxea.Desktop.Forms;

public partial class MahaiaXehetasunakForm : Form
{
    private readonly MahaiaDto _mahaia;
    private readonly LangileaDto _langilea;
    private readonly ApiService _api;
    private List<EskaeraDto> _eskaerak = new();
    private int _hautatutakoEskaeraIndex = 0;

    /// <summary>
    /// Visual Studio WinForms Designer-erako eraikitzailea
    /// </summary>
    public MahaiaXehetasunakForm()
    {
        InitializeComponent();
        _mahaia = new MahaiaDto { Id = 4, Zenbakia = 4, Edukiera = 4, Egoera = "Okupatuta" };
        _langilea = new LangileaDto { Id = 1, Izena = "Ane G.", Rola = "Zerbitzaria" };
        _api = new ApiService();
        KonfiguratuInterfazea();
    }

    public MahaiaXehetasunakForm(MahaiaDto mahaia, LangileaDto langilea, ApiService api)
    {
        InitializeComponent();
        _mahaia = mahaia;
        _langilea = langilea;
        _api = api;
        KonfiguratuInterfazea();
    }

    private void KonfiguratuInterfazea()
    {
        Text = $"T1 Jatetxea - Mahaia {_mahaia.Zenbakia} Xehetasunak";
        lblMahaiaTit.Text = $"T1 Jatetxea - Mahaia {_mahaia.Zenbakia}";
        lblZerbitzaria.Text = $"Zerbitzaria: {_langilea.Izena} ({_mahaia.Edukiera} pax) · {_mahaia.Egoera}";

        // Barraren beheko marra
        pnlTopBar.Paint += (s, e) =>
        {
            using var pen = new Pen(Color.FromArgb(203, 213, 225), 1.5f);
            e.Graphics.DrawLine(pen, 0, pnlTopBar.Height - 1, pnlTopBar.Width, pnlTopBar.Height - 1);
        };

        // Txartelen ertz leunak
        pnlSummaryCard.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var pen = new Pen(Color.FromArgb(203, 213, 225), 1.5f);
            e.Graphics.DrawRectangle(pen, 0, 0, pnlSummaryCard.Width - 1, pnlSummaryCard.Height - 1);
        };

        pnlTicketCard.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var pen = new Pen(Color.FromArgb(203, 213, 225), 1.5f);
            e.Graphics.DrawRectangle(pen, 0, 0, pnlTicketCard.Width - 1, pnlTicketCard.Height - 1);
        };

        // Botoien ekintzak lotu
        btnAtzera.Click += (s, e) => Close();
        btnSukalderaBidali.Click += async (s, e) => await SukalderaBidaliKlikatuaAsync();
        btnGehituEskaera.Click += (s, e) => GehituProduktuakKlikatua();
        btnGehituKontua.Click += async (s, e) => await GehituKontuBerriaKlikatuaAsync();
        btnKontuaEskatu.Click += async (s, e) => await KobratuEtaItxiAsync();
        btnAldatuMahaiaz.Click += (s, e) => AldatuMahaiazKlikatua();

        Load += async (s, e) => await KargatuDatuakAsync();
    }

    private async Task KargatuDatuakAsync()
    {
        try
        {
            _eskaerak = await _api.GetMahaiarenEskaeraGuztiakAsync(_mahaia.Id);
        }
        catch
        {
            _eskaerak = new List<EskaeraDto>();
        }

        if (_hautatutakoEskaeraIndex >= _eskaerak.Count)
        {
            _hautatutakoEskaeraIndex = 0;
        }

        EguneratuKontuFitxak();
        BistaratuHautatutakoEskaera();
    }

    private void EguneratuKontuFitxak()
    {
        flowKontuFitxak.SuspendLayout();
        flowKontuFitxak.Controls.Clear();

        for (int i = 0; i < _eskaerak.Count; i++)
        {
            int index = i;
            var eskaera = _eskaerak[index];
            bool isAukeratua = index == _hautatutakoEskaeraIndex;

            var btn = new Button
            {
                Text = $"{index + 1}. Kontua (#{eskaera.Id}) - {eskaera.Guztira:F2} €",
                Font = new Font("Segoe UI", 10.5F, isAukeratua ? FontStyle.Bold : FontStyle.Regular),
                BackColor = isAukeratua ? Color.FromArgb(136, 19, 55) : Color.White,
                ForeColor = isAukeratua ? Color.White : Color.FromArgb(51, 65, 85),
                Size = new Size(245, 46),
                Margin = new Padding(0, 0, 10, 0),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderColor = isAukeratua ? Color.FromArgb(136, 19, 55) : Color.FromArgb(203, 213, 225);
            btn.FlatAppearance.BorderSize = 1;

            btn.Click += (s, e) =>
            {
                _hautatutakoEskaeraIndex = index;
                EguneratuKontuFitxak();
                BistaratuHautatutakoEskaera();
            };

            flowKontuFitxak.Controls.Add(btn);
        }

        flowKontuFitxak.ResumeLayout(true);
    }

    private void BistaratuHautatutakoEskaera()
    {
        if (_eskaerak.Count == 0 || _hautatutakoEskaeraIndex >= _eskaerak.Count)
        {
            pnlTicketLines.Controls.Clear();
            var lblHutsik = new Label
            {
                Text = "Ez dago eskaera aktiborik mahai honetan.\nSakatu beheko '+ ESKAERA GEHITU' botoia.",
                Font = new Font("Segoe UI", 12F, FontStyle.Regular),
                ForeColor = Color.FromArgb(100, 116, 139),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlTicketLines.Controls.Add(lblHutsik);
            lblEskaeraInfo.Text = "Ez dago eskaera aktiborik";
            lblLerroLaburpena.Text = "0 plater";
            lblGuztira.Text = "GUZTIRA: 0,00 €";
            lblAzpitotala.Text = "Azpitotala: 0,00 €";
            lblBez.Text = "BEZ (%10 barne): 0,00 €";
            lblEgoeraOrokorra.Text = "Eskaerarik gabe";
            lblEgoeraOrokorra.ForeColor = Color.FromArgb(100, 116, 139);
            btnSukalderaBidali.Text = "EZ DAGO ESKAERARIK";
            btnSukalderaBidali.BackColor = Color.FromArgb(71, 85, 105);
            btnSukalderaBidali.Enabled = false;
            return;
        }

        var eskaera = _eskaerak[_hautatutakoEskaeraIndex];

        // 1. Egoera eta Laburpena
        int guztiraKop = eskaera.Lerroak.Sum(l => l.Kantitatea);
        int zaiKop = eskaera.Lerroak.Where(l => NormalizatuEgoera(l.Egoera) == "Zai").Sum(l => l.Kantitatea);
        int martxanKop = eskaera.Lerroak.Where(l => NormalizatuEgoera(l.Egoera) == "Martxan").Sum(l => l.Kantitatea);
        int egindaKop = eskaera.Lerroak.Where(l => NormalizatuEgoera(l.Egoera) == "Eginda").Sum(l => l.Kantitatea);

        lblEskaeraInfo.Text = $"{_hautatutakoEskaeraIndex + 1}. Kontua: Eskaera #{eskaera.Id} · Egoera: {eskaera.Egoera}";
        lblLerroLaburpena.Text = $"{guztiraKop} plater ({zaiKop} Zai · {martxanKop} Martxan · {egindaKop} Eginda)";

        // 2. Kaxa eta Botoiaren Egoera
        decimal subtotal = Math.Round(eskaera.Guztira / 1.10m, 2);
        decimal bez = eskaera.Guztira - subtotal;
        lblAzpitotala.Text = $"Azpitotala:  {subtotal:F2} €";
        lblBez.Text = $"BEZ (%10 barne):  {bez:F2} €";
        lblGuztira.Text = $"GUZTIRA:  {eskaera.Guztira:F2} €";

        if (zaiKop > 0)
        {
            lblEgoeraOrokorra.Text = $"{zaiKop} plater sukaldera bidaltzeke (Zai)";
            lblEgoeraOrokorra.ForeColor = Color.FromArgb(180, 83, 9);
            btnSukalderaBidali.Text = $"SUKALDERA BIDALI ({zaiKop} ZAI)";
            btnSukalderaBidali.BackColor = Color.FromArgb(22, 101, 52); // Berdea
            btnSukalderaBidali.Enabled = true;
        }
        else
        {
            lblEgoeraOrokorra.Text = "Plater guztiak sukaldera bidalita daude";
            lblEgoeraOrokorra.ForeColor = Color.FromArgb(22, 101, 52);
            btnSukalderaBidali.Text = "SUKALDERA BIDALITA";
            btnSukalderaBidali.BackColor = Color.FromArgb(71, 85, 105); // Grisa
            btnSukalderaBidali.Enabled = false;
        }

        // 3. Plateren zerrenda eraiki
        pnlTicketLines.SuspendLayout();
        pnlTicketLines.Controls.Clear();

        int targetWidth = pnlTicketLines.ClientSize.Width > 100 ? pnlTicketLines.ClientSize.Width - 16 : 740;
        int y = 8;
        foreach (var lerro in eskaera.Lerroak)
        {
            var rowCard = SortuLerroTxartela(lerro, targetWidth);
            rowCard.Location = new Point(4, y);
            pnlTicketLines.Controls.Add(rowCard);
            y += rowCard.Height + 10;
        }

        pnlTicketLines.ResumeLayout(true);
    }

    private Control SortuLerroTxartela(EskaeraLerroaDto lerro, int cardWidth)
    {
        string lerroEgoera = NormalizatuEgoera(lerro.Egoera);
        var (badgeText, badgeBg, badgeFg, badgeBorder) = GetBadgeStyle(lerroEgoera);

        bool hasOharra = !string.IsNullOrWhiteSpace(lerro.Oharrak);
        int cardHeight = hasOharra ? 96 : 72;

        var card = new Panel
        {
            Width = cardWidth,
            Height = cardHeight,
            BackColor = Color.FromArgb(248, 250, 252),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        card.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            var borderCol = lerroEgoera == "Zai" ? Color.FromArgb(253, 230, 138) : Color.FromArgb(226, 232, 240);
            using var pen = new Pen(borderCol, 1.2f);
            e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
        };

        // 1. Kantitatea (2x)
        var lblKop = new Label
        {
            Text = $"{lerro.Kantitatea}x",
            Font = new Font("Segoe UI", 13F, FontStyle.Bold),
            ForeColor = Color.FromArgb(136, 19, 55),
            Location = new Point(14, 16),
            Size = new Size(50, 40),
            TextAlign = ContentAlignment.MiddleLeft,
            UseCompatibleTextRendering = false
        };
        card.Controls.Add(lblKop);

        // 2. Plateraren Izena
        int nameWidth = Math.Max(220, cardWidth - 365);
        var lblIzena = new Label
        {
            Text = lerro.PlaterIzena,
            Font = new Font("Segoe UI", 12F, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 41, 59),
            Location = new Point(68, 16),
            Size = new Size(nameWidth, 40),
            TextAlign = ContentAlignment.MiddleLeft,
            UseCompatibleTextRendering = false
        };
        card.Controls.Add(lblIzena);

        // 3. Oharrak (azpian)
        if (hasOharra)
        {
            var lblOharra = new Label
            {
                Text = $"Oharra: {lerro.Oharrak}",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(68, 58),
                Size = new Size(nameWidth, 28),
                TextAlign = ContentAlignment.MiddleLeft,
                UseCompatibleTextRendering = false
            };
            card.Controls.Add(lblOharra);
        }

        // 4. Prezioa (Eskuinean)
        var lblPrezioa = new Label
        {
            Text = $"{lerro.Azpitotala:F2} €",
            Font = new Font("Segoe UI", 13F, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 41, 59),
            Location = new Point(cardWidth - 275, 16),
            Size = new Size(125, 40),
            TextAlign = ContentAlignment.MiddleRight,
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            UseCompatibleTextRendering = false
        };
        card.Controls.Add(lblPrezioa);

        // 5. Egoera Txapa ([ZAI] / [MARTXAN] / [EGINDA])
        var pnlBadge = new Panel
        {
            Location = new Point(cardWidth - 142, 16),
            Size = new Size(130, 40),
            BackColor = badgeBg,
            Anchor = AnchorStyles.Top | AnchorStyles.Right
        };
        pnlBadge.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var p = new Pen(badgeBorder, 1.2f);
            e.Graphics.DrawRectangle(p, 0, 0, pnlBadge.Width - 1, pnlBadge.Height - 1);
        };

        var lblBadge = new Label
        {
            Dock = DockStyle.Fill,
            Text = badgeText,
            Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
            ForeColor = badgeFg,
            TextAlign = ContentAlignment.MiddleCenter,
            UseCompatibleTextRendering = false
        };
        pnlBadge.Controls.Add(lblBadge);
        card.Controls.Add(pnlBadge);

        return card;
    }

    private static string NormalizatuEgoera(string egoera)
    {
        string e = (egoera ?? "").Trim().ToLowerInvariant();
        if (e.Contains("martxan") || e.Contains("sukalde")) return "Martxan";
        if (e.Contains("egin") || e.Contains("prestat") || e.Contains("zerbitzat")) return "Eginda";
        return "Zai";
    }

    private static (string BadgeText, Color BgColor, Color FgColor, Color BorderColor) GetBadgeStyle(string egoera)
    {
        return egoera switch
        {
            "Martxan" => ("MARTXAN", Color.FromArgb(219, 234, 254), Color.FromArgb(29, 78, 216), Color.FromArgb(147, 197, 253)),
            "Eginda" => ("EGINDA", Color.FromArgb(220, 252, 231), Color.FromArgb(21, 128, 61), Color.FromArgb(134, 239, 172)),
            _ => ("ZAI", Color.FromArgb(254, 243, 199), Color.FromArgb(180, 83, 9), Color.FromArgb(253, 230, 138))
        };
    }

    /// <summary>
    /// Zai dauden platerak sukaldera bidaltzen ditu eta 'Martxan' jartzen ditu
    /// </summary>
    private async Task SukalderaBidaliKlikatuaAsync()
    {
        if (_eskaerak.Count == 0 || _hautatutakoEskaeraIndex >= _eskaerak.Count) return;

        var eskaera = _eskaerak[_hautatutakoEskaeraIndex];
        var zaiLerroak = eskaera.Lerroak.Where(l => NormalizatuEgoera(l.Egoera) == "Zai").ToList();

        if (zaiLerroak.Count == 0)
        {
            MessageBox.Show("Dagoeneko plater guztiak sukaldera bidalita daude.", "Informazioa", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        btnSukalderaBidali.Enabled = false;
        btnSukalderaBidali.Text = "SUKALDERA BIDALTZEN...";

        // 1. API bidez egoerak eguneratu
        var (ok, mezua) = await _api.BidaliSukalderaAsync(eskaera.Id);

        // 2. Tokiko memoriako lerroak 'Martxan' jarri feedback azkarra emateko
        foreach (var l in zaiLerroak)
        {
            l.Egoera = "Martxan";
        }
        eskaera.Egoera = "Sukaldean";
        _mahaia.Egoera = "Eskaera bidean";

        // Mahaiaren egoera eguneratu datu-basean
        _ = _api.EguneratuMahaiarenEgoeraAsync(_mahaia.Id, "Eskaera bidean");

        lblZerbitzaria.Text = $"Zerbitzaria: {_langilea.Izena} ({_mahaia.Edukiera} pax) · Eskaera bidean";
        BistaratuHautatutakoEskaera();

        MessageBox.Show(
            $"{zaiLerroak.Count} plater arrakastaz bidali dira sukaldera!\n\nSukaldean martxan jarrita daude.",
            "Sukaldera Bidalita",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    /// <summary>
    /// Aukeratutako eskaerari plater / lerro gehiago gehitzen dizkio EskaeraForm bidez
    /// </summary>
    private void GehituProduktuakKlikatua()
    {
        int? dagoenekoEskaeraId = _eskaerak.Count > _hautatutakoEskaeraIndex ? _eskaerak[_hautatutakoEskaeraIndex].Id : null;

        using var eskaeraForm = new EskaeraForm(_mahaia, _langilea, _api, _mahaia.Edukiera, dagoenekoEskaeraId);
        Hide();
        eskaeraForm.ShowDialog();
        Show();
        _ = KargatuDatuakAsync();
    }

    /// <summary>
    /// Mahai berean beste kontu / eskaera bat irekitzen du (mahaikideek aparte ordaintzeko)
    /// </summary>
    private async Task GehituKontuBerriaKlikatuaAsync()
    {
        var konfirmatu = MessageBox.Show(
            $"Mahaia {_mahaia.Zenbakia}-rentzat beste kontu / eskaera berri bat sortu nahi duzu?\n\n(Mahaikideen kontuak bereizteko baliagarria da)",
            "Kontu Berria Gehitu",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (konfirmatu != DialogResult.Yes) return;

        var (arrakasta, mezua, berria) = await _api.SortuEskaeraAsync(_mahaia.Id, _langilea.Id, 2);
        if (!arrakasta || berria == null)
        {
            MessageBox.Show(
                $"Ezin izan da kontu/eskaera berririk sortu APIan:\n{mezua}",
                "Errorea",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return;
        }

        _eskaerak.Add(berria);
        _hautatutakoEskaeraIndex = _eskaerak.Count - 1;

        EguneratuKontuFitxak();
        BistaratuHautatutakoEskaera();

        // Zuzenean produktuen hautatzailea ireki
        GehituProduktuakKlikatua();
    }

    /// <summary>
    /// Kontua kobratu eta mahaia Garbitzen egoeran uzten du
    /// </summary>
    private async Task KobratuEtaItxiAsync()
    {
        if (_eskaerak.Count == 0 || _hautatutakoEskaeraIndex >= _eskaerak.Count)
        {
            MessageBox.Show("Ez dago kobratzeko eskaerarik.", "Informazioa", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var eskaera = _eskaerak[_hautatutakoEskaeraIndex];
        var konfirmatu = MessageBox.Show(
            $"Mahaia {_mahaia.Zenbakia} - {_hautatutakoEskaeraIndex + 1}. Kontua (#{eskaera.Id}) kobratu nahi duzu?\n\nGUZTIRA: {eskaera.Guztira:F2} €",
            "Kontua Kobratu",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (konfirmatu == DialogResult.Yes)
        {
            _ = _api.EguneratuEskaerarenEgoeraAsync(eskaera.Id, "Kobratua");
            _eskaerak.RemoveAt(_hautatutakoEskaeraIndex);

            if (_eskaerak.Count == 0)
            {
                await _api.EguneratuMahaiarenEgoeraAsync(_mahaia.Id, "Garbitzen");
                MessageBox.Show($"Mahaia {_mahaia.Zenbakia}-ren kontu guztiak kobratuta daude. Mahaia 'Garbitzen' egoeran geratu da.", "Kobratuta", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            else
            {
                _hautatutakoEskaeraIndex = Math.Max(0, _hautatutakoEskaeraIndex - 1);
                EguneratuKontuFitxak();
                BistaratuHautatutakoEskaera();
                MessageBox.Show($"Kontua ondo kobratu da. Mahai honetan beste kontu batzuk irekita daude oraindik.", "Kontua Kobratuta", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }

    private void AldatuMahaiazKlikatua()
    {
        MessageBox.Show(
            $"Mahaia {_mahaia.Zenbakia} beste mahai batera aldatzeko eskaera kudeaketara bidali da.",
            "Aldatu Mahaiaz",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }
}
