using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using Jatetxea.Desktop.Models;
using Jatetxea.Desktop.Services;

namespace Jatetxea.Desktop.Forms;

public class OrdainketaForm : Form
{
    private readonly MahaiaDto _mahaia;
    private readonly EskaeraDto _eskaera;
    private readonly LangileaDto _langilea;
    private readonly ApiService _api;

    private string _hautatutakoMota = "Eskudirua"; // "Eskudirua", "Txartela", "Bizum"
    public OrdainketaErantzunaDto? Emaitza { get; private set; }

    // Kontrolak
    private readonly Button _btnEskudiruaTab;
    private readonly Button _btnTxartelaTab;
    private readonly Button _btnBizumTab;

    private readonly Panel _pnlEskudirua;
    private readonly Panel _pnlTxartela;
    private readonly Panel _pnlBizum;

    // Eskudirua kontrolak
    private readonly TextBox _txtJasotakoa;
    private readonly Label _lblKanbioa;

    // Txartela kontrolak
    private readonly TextBox _txtTxartelZenbakia;
    private readonly TextBox _txtIraungitzea;
    private readonly TextBox _txtCvv;
    private readonly TextBox _txtTitularra;
    private readonly Label _lblTxartelMarka;

    // Bizum kontrolak
    private readonly TextBox _txtBizumTelefonoa;
    private readonly Label _lblBizumValid;

    // Beheko botoia
    private readonly Button _btnKobratu;

    public OrdainketaForm(MahaiaDto mahaia, EskaeraDto eskaera, LangileaDto langilea, ApiService api)
    {
        _mahaia = mahaia;
        _eskaera = eskaera;
        _langilea = langilea;
        _api = api;

        Text = $"T1 Jatetxea - Mahaia {_mahaia.Zenbakia} Kontua Kobratu (POS)";
        Size = new Size(650, 720);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Color.FromArgb(248, 250, 252);

        decimal subtotal = Math.Round(_eskaera.Guztira / 1.10m, 2);
        decimal bez = _eskaera.Guztira - subtotal;

        // 1. Goiburua: Mahaia, Eskaera eta Ordaindu beharreko zenbateko handia
        var pnlHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 110,
            BackColor = Color.FromArgb(136, 19, 55), // Jatetxeko granate nagusia
            Padding = new Padding(24, 16, 24, 16)
        };

        var lblTit = new Label
        {
            Text = $"KONTUA KOBRATU · MAHAIA {_mahaia.Zenbakia}",
            Font = new Font("Segoe UI", 12F, FontStyle.Bold),
            ForeColor = Color.FromArgb(254, 205, 211),
            Location = new Point(24, 14),
            AutoSize = true
        };

        var lblSub = new Label
        {
            Text = $"Eskaera #{_eskaera.Id} · {_eskaera.Lerroak.Sum(l => l.Kantitatea)} plater · Zerbitzaria: {_langilea.Izena}",
            Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
            ForeColor = Color.FromArgb(254, 226, 226),
            Location = new Point(24, 40),
            AutoSize = true
        };

        var lblTotala = new Label
        {
            Text = $"{_eskaera.Guztira:F2} €",
            Font = new Font("Segoe UI", 26F, FontStyle.Bold),
            ForeColor = Color.White,
            TextAlign = ContentAlignment.MiddleRight,
            Location = new Point(410, 16),
            Size = new Size(210, 48)
        };

        var lblBezInfo = new Label
        {
            Text = $"Azpitotala: {subtotal:F2} € · BEZ (%10): {bez:F2} €",
            Font = new Font("Segoe UI", 9F, FontStyle.Regular),
            ForeColor = Color.FromArgb(254, 226, 226),
            TextAlign = ContentAlignment.MiddleRight,
            Location = new Point(360, 68),
            Size = new Size(260, 20)
        };

        pnlHeader.Controls.Add(lblTit);
        pnlHeader.Controls.Add(lblSub);
        pnlHeader.Controls.Add(lblTotala);
        pnlHeader.Controls.Add(lblBezInfo);

        // 2. Ordainketa Mota Hautatzailea (Fitxak)
        var pnlTabs = new Panel
        {
            Dock = DockStyle.Top,
            Height = 54,
            BackColor = Color.White,
            Padding = new Padding(24, 6, 24, 6)
        };
        pnlTabs.Paint += (s, e) =>
        {
            using var pen = new Pen(Color.FromArgb(226, 232, 240), 1.5f);
            e.Graphics.DrawLine(pen, 0, pnlTabs.Height - 1, pnlTabs.Width, pnlTabs.Height - 1);
        };

        _btnEskudiruaTab = SortuTabBotoia("ESKUDIRUA (CASH)", 24);
        _btnTxartelaTab = SortuTabBotoia("TXARTELA (EMV)", 224);
        _btnBizumTab = SortuTabBotoia("BIZUM", 424);

        _btnEskudiruaTab.Click += (s, e) => AldatuMota("Eskudirua");
        _btnTxartelaTab.Click += (s, e) => AldatuMota("Txartela");
        _btnBizumTab.Click += (s, e) => AldatuMota("Bizum");

        pnlTabs.Controls.Add(_btnEskudiruaTab);
        pnlTabs.Controls.Add(_btnTxartelaTab);
        pnlTabs.Controls.Add(_btnBizumTab);

        // 3. Gorputz Panela (3 azpipanel motaren arabera)
        var pnlBody = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(24, 18, 24, 18),
            BackColor = Color.FromArgb(248, 250, 252)
        };

        // --- A) ESKUDIRUA PANELA ---
        _pnlEskudirua = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };

        var lblJasotakoaTit = new Label
        {
            Text = "Bezeroak emandako dirua (Jasotakoa):",
            Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 41, 59),
            Location = new Point(10, 8),
            AutoSize = true
        };

        _txtJasotakoa = new TextBox
        {
            Font = new Font("Segoe UI", 18F, FontStyle.Bold),
            Location = new Point(10, 36),
            Size = new Size(240, 44),
            TextAlign = HorizontalAlignment.Right,
            Text = $"{_eskaera.Guztira:F2}"
        };
        _txtJasotakoa.TextChanged += (s, e) => EguneratuKanbioa();

        var lblEuro = new Label
        {
            Text = "€",
            Font = new Font("Segoe UI", 18F, FontStyle.Bold),
            Location = new Point(256, 40),
            AutoSize = true,
            ForeColor = Color.FromArgb(100, 116, 139)
        };

        // Botoi azkarrak (Billete arruntak)
        var pnlAzkarrak = new FlowLayoutPanel
        {
            Location = new Point(10, 96),
            Size = new Size(580, 50),
            BackColor = Color.Transparent
        };

        SortuBotoiAzkarra(pnlAzkarrak, "Zehatza", _eskaera.Guztira);
        SortuBotoiAzkarra(pnlAzkarrak, "+ 5 €", Math.Ceiling(_eskaera.Guztira / 5m) * 5m);
        SortuBotoiAzkarra(pnlAzkarrak, "+ 10 €", Math.Ceiling(_eskaera.Guztira / 10m) * 10m);
        SortuBotoiAzkarra(pnlAzkarrak, "20 €", 20m);
        SortuBotoiAzkarra(pnlAzkarrak, "50 €", 50m);
        SortuBotoiAzkarra(pnlAzkarrak, "100 €", 100m);

        // Kanbioaren erakusleihoa
        var pnlKanbioaKutxa = new Panel
        {
            Location = new Point(10, 160),
            Size = new Size(570, 90),
            BackColor = Color.White,
            Padding = new Padding(16)
        };
        pnlKanbioaKutxa.Paint += (s, e) =>
        {
            using var pen = new Pen(Color.FromArgb(203, 213, 225), 1.2f);
            e.Graphics.DrawRectangle(pen, 0, 0, pnlKanbioaKutxa.Width - 1, pnlKanbioaKutxa.Height - 1);
        };

        var lblKanbioTit = new Label
        {
            Text = "BEZEROARI ITZULI BEHARREKO KANBIOA / BUELTA:",
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            ForeColor = Color.FromArgb(71, 85, 105),
            Location = new Point(16, 12),
            AutoSize = true
        };

        _lblKanbioa = new Label
        {
            Text = "0,00 €",
            Font = new Font("Segoe UI", 24F, FontStyle.Bold),
            ForeColor = Color.FromArgb(22, 101, 52),
            Location = new Point(16, 36),
            AutoSize = true
        };

        pnlKanbioaKutxa.Controls.Add(lblKanbioTit);
        pnlKanbioaKutxa.Controls.Add(_lblKanbioa);

        _pnlEskudirua.Controls.Add(lblJasotakoaTit);
        _pnlEskudirua.Controls.Add(_txtJasotakoa);
        _pnlEskudirua.Controls.Add(lblEuro);
        _pnlEskudirua.Controls.Add(pnlAzkarrak);
        _pnlEskudirua.Controls.Add(pnlKanbioaKutxa);

        // --- B) TXARTELA PANELA ---
        _pnlTxartela = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent, Visible = false };

        var lblTxartelTit = new Label
        {
            Text = "Kreditu / Zordunketa Txartelaren Datuak (EMV / Contactless):",
            Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 41, 59),
            Location = new Point(10, 8),
            AutoSize = true
        };

        _txtTxartelZenbakia = new TextBox
        {
            Font = new Font("Segoe UI", 13F, FontStyle.Bold),
            Location = new Point(10, 36),
            Size = new Size(340, 36),
            Text = "4532 8921 7842 4821"
        };
        _txtTxartelZenbakia.TextChanged += (s, e) => EguneratuTxartelMarka();

        _lblTxartelMarka = new Label
        {
            Text = "VISA (Baimendua)",
            Font = new Font("Segoe UI", 11F, FontStyle.Bold),
            ForeColor = Color.FromArgb(29, 78, 216),
            Location = new Point(360, 42),
            AutoSize = true
        };

        var lblIraungTit = new Label { Text = "Iraungitzea (MM/UU):", Location = new Point(10, 84), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
        _txtIraungitzea = new TextBox { Text = "12/28", Location = new Point(10, 106), Size = new Size(100, 30), Font = new Font("Segoe UI", 11F) };

        var lblCvvTit = new Label { Text = "CVV (3 digitu):", Location = new Point(130, 84), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
        _txtCvv = new TextBox { Text = "842", Location = new Point(130, 106), Size = new Size(80, 30), Font = new Font("Segoe UI", 11F), UseSystemPasswordChar = true };

        var lblTitularTit = new Label { Text = "Titularra:", Location = new Point(230, 84), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
        _txtTitularra = new TextBox { Text = "Aitor Martinez Goikoetxea", Location = new Point(230, 106), Size = new Size(340, 30), Font = new Font("Segoe UI", 11F) };

        // Txartel botoi azkarrak
        var btnPresetVisa = new Button
        {
            Text = "VISA Baimendua",
            Location = new Point(10, 150),
            Size = new Size(160, 34),
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            BackColor = Color.FromArgb(238, 242, 255),
            ForeColor = Color.FromArgb(49, 46, 129),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnPresetVisa.FlatAppearance.BorderColor = Color.FromArgb(199, 210, 254);
        btnPresetVisa.Click += (s, e) =>
        {
            _txtTxartelZenbakia.Text = "4532 8921 7842 4821";
            _txtTitularra.Text = "Aitor Martinez Goikoetxea";
            EguneratuTxartelMarka();
        };

        var btnPresetMastercard = new Button
        {
            Text = "Mastercard Baimendua",
            Location = new Point(180, 150),
            Size = new Size(180, 34),
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            BackColor = Color.FromArgb(254, 242, 242),
            ForeColor = Color.FromArgb(153, 27, 27),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnPresetMastercard.FlatAppearance.BorderColor = Color.FromArgb(254, 202, 202);
        btnPresetMastercard.Click += (s, e) =>
        {
            _txtTxartelZenbakia.Text = "5412 7534 8921 9912";
            _txtTitularra.Text = "Maite Iriarte Arana";
            EguneratuTxartelMarka();
        };

        // Segurtasun oharra
        var lblSegurtasuna = new Label
        {
            Text = "Zibersegurtasuna: EMV txip bidezko tokenizazioa. Txartelaren datu gordinak ez dira datu-basean gordetzen (PCI-DSS araudia).",
            Font = new Font("Segoe UI", 8.5F, FontStyle.Italic),
            ForeColor = Color.FromArgb(100, 116, 139),
            Location = new Point(10, 200),
            Size = new Size(560, 40)
        };

        _pnlTxartela.Controls.Add(lblTxartelTit);
        _pnlTxartela.Controls.Add(_txtTxartelZenbakia);
        _pnlTxartela.Controls.Add(_lblTxartelMarka);
        _pnlTxartela.Controls.Add(lblIraungTit);
        _pnlTxartela.Controls.Add(_txtIraungitzea);
        _pnlTxartela.Controls.Add(lblCvvTit);
        _pnlTxartela.Controls.Add(_txtCvv);
        _pnlTxartela.Controls.Add(lblTitularTit);
        _pnlTxartela.Controls.Add(_txtTitularra);
        _pnlTxartela.Controls.Add(btnPresetVisa);
        _pnlTxartela.Controls.Add(btnPresetMastercard);
        _pnlTxartela.Controls.Add(lblSegurtasuna);

        // --- C) BIZUM PANELA ---
        _pnlBizum = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent, Visible = false };

        var lblBizumTit = new Label
        {
            Text = "Bezeroaren Bizum Telefono Zenbakia:",
            Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 41, 59),
            Location = new Point(10, 8),
            AutoSize = true
        };

        _txtBizumTelefonoa = new TextBox
        {
            Font = new Font("Segoe UI", 16F, FontStyle.Bold),
            Location = new Point(10, 36),
            Size = new Size(260, 40),
            Text = "644 12 34 56"
        };
        _txtBizumTelefonoa.TextChanged += (s, e) => EguneratuBizumBalidazioa();

        _lblBizumValid = new Label
        {
            Text = "Telefono baliozkoa (Berehalako Bizum push-a bidaliko da)",
            Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
            ForeColor = Color.FromArgb(22, 101, 52),
            Location = new Point(10, 85),
            AutoSize = true
        };

        var btnPresetBizum = new Button
        {
            Text = "Probako Bezero Telefonoa",
            Location = new Point(10, 120),
            Size = new Size(220, 36),
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            BackColor = Color.FromArgb(236, 253, 245),
            ForeColor = Color.FromArgb(4, 120, 87),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnPresetBizum.FlatAppearance.BorderColor = Color.FromArgb(167, 243, 208);
        btnPresetBizum.Click += (s, e) => _txtBizumTelefonoa.Text = "688 98 76 54";

        var lblBizumInfo = new Label
        {
            Text = "Bezeroak bere mugikorrean jakinarazpen bat jasoko du ordainketa onartzeko. Baieztapena berehalakoa da.",
            Font = new Font("Segoe UI", 8.5F, FontStyle.Italic),
            ForeColor = Color.FromArgb(100, 116, 139),
            Location = new Point(10, 175),
            Size = new Size(560, 40)
        };

        _pnlBizum.Controls.Add(lblBizumTit);
        _pnlBizum.Controls.Add(_txtBizumTelefonoa);
        _pnlBizum.Controls.Add(_lblBizumValid);
        _pnlBizum.Controls.Add(btnPresetBizum);
        _pnlBizum.Controls.Add(lblBizumInfo);

        pnlBody.Controls.Add(_pnlEskudirua);
        pnlBody.Controls.Add(_pnlTxartela);
        pnlBody.Controls.Add(_pnlBizum);

        // 4. Beheko Botoiak: Utzi eta KOBRATU ETA TIKETA SORTU
        var pnlBottom = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 75,
            BackColor = Color.White,
            Padding = new Padding(24, 14, 24, 14)
        };
        pnlBottom.Paint += (s, e) =>
        {
            using var pen = new Pen(Color.FromArgb(226, 232, 240), 1.5f);
            e.Graphics.DrawLine(pen, 0, 0, pnlBottom.Width, 0);
        };

        var btnUtzi = new Button
        {
            Text = "Utzi",
            Font = new Font("Segoe UI", 11F, FontStyle.Bold),
            BackColor = Color.FromArgb(241, 245, 249),
            ForeColor = Color.FromArgb(71, 85, 105),
            FlatStyle = FlatStyle.Flat,
            Size = new Size(110, 46),
            Location = new Point(24, 14),
            Cursor = Cursors.Hand
        };
        btnUtzi.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnUtzi.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

        _btnKobratu = new Button
        {
            Text = $"KOBRATU ({_eskaera.Guztira:F2} €) ETA TIKETA SORTU",
            Font = new Font("Segoe UI", 11.5F, FontStyle.Bold),
            BackColor = Color.FromArgb(136, 19, 55),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Size = new Size(460, 46),
            Location = new Point(148, 14),
            Cursor = Cursors.Hand
        };
        _btnKobratu.FlatAppearance.BorderSize = 0;
        _btnKobratu.Click += async (s, e) => await ExekutatuOrdainketaAsync();

        pnlBottom.Controls.Add(btnUtzi);
        pnlBottom.Controls.Add(_btnKobratu);

        Controls.Add(pnlBody);
        Controls.Add(pnlBottom);
        Controls.Add(pnlTabs);
        Controls.Add(pnlHeader);

        AldatuMota("Eskudirua");
        EguneratuKanbioa();
    }

    private Button SortuTabBotoia(string testua, int x)
    {
        var btn = new Button
        {
            Text = testua,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            Size = new Size(190, 40),
            Location = new Point(x, 7),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btn.FlatAppearance.BorderSize = 1;
        return btn;
    }

    private void SortuBotoiAzkarra(FlowLayoutPanel pnl, string testua, decimal zenbatekoa)
    {
        var btn = new Button
        {
            Text = testua,
            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
            Size = new Size(88, 38),
            BackColor = Color.FromArgb(241, 245, 249),
            ForeColor = Color.FromArgb(30, 41, 59),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Margin = new Padding(0, 0, 8, 0)
        };
        btn.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btn.Click += (s, e) =>
        {
            _txtJasotakoa.Text = $"{zenbatekoa:F2}";
            EguneratuKanbioa();
        };
        pnl.Controls.Add(btn);
    }

    private void AldatuMota(string mota)
    {
        _hautatutakoMota = mota;

        _pnlEskudirua.Visible = mota == "Eskudirua";
        _pnlTxartela.Visible = mota == "Txartela";
        _pnlBizum.Visible = mota == "Bizum";

        EguneratuTabBotoia(_btnEskudiruaTab, mota == "Eskudirua");
        EguneratuTabBotoia(_btnTxartelaTab, mota == "Txartela");
        EguneratuTabBotoia(_btnBizumTab, mota == "Bizum");

        if (mota == "Eskudirua") EguneratuKanbioa();
        else if (mota == "Txartela") _btnKobratu.Enabled = true;
        else if (mota == "Bizum") EguneratuBizumBalidazioa();
    }

    private static void EguneratuTabBotoia(Button btn, bool hautatuta)
    {
        btn.BackColor = hautatuta ? Color.FromArgb(136, 19, 55) : Color.White;
        btn.ForeColor = hautatuta ? Color.White : Color.FromArgb(71, 85, 105);
        btn.FlatAppearance.BorderColor = hautatuta ? Color.FromArgb(136, 19, 55) : Color.FromArgb(203, 213, 225);
    }

    private void EguneratuKanbioa()
    {
        string text = _txtJasotakoa.Text.Replace("€", "").Trim();
        if (decimal.TryParse(text, out decimal jasotakoa))
        {
            decimal diff = jasotakoa - _eskaera.Guztira;
            if (diff >= 0)
            {
                _lblKanbioa.Text = $"{diff:F2} €";
                _lblKanbioa.ForeColor = Color.FromArgb(22, 101, 52); // Berdea
                _btnKobratu.Enabled = true;
            }
            else
            {
                _lblKanbioa.Text = $"Falta dira: {Math.Abs(diff):F2} €";
                _lblKanbioa.ForeColor = Color.FromArgb(220, 38, 38); // Gorria
                _btnKobratu.Enabled = false;
            }
        }
        else
        {
            _lblKanbioa.Text = "Idatzi zenbateko baliozko bat";
            _lblKanbioa.ForeColor = Color.FromArgb(220, 38, 38);
            _btnKobratu.Enabled = false;
        }
    }

    private void EguneratuTxartelMarka()
    {
        string zenb = _txtTxartelZenbakia.Text.Replace(" ", "").Trim();
        if (zenb.StartsWith("4"))
        {
            _lblTxartelMarka.Text = "VISA (Baimendua)";
            _lblTxartelMarka.ForeColor = Color.FromArgb(29, 78, 216);
        }
        else if (zenb.StartsWith("51") || zenb.StartsWith("52") || zenb.StartsWith("53") || zenb.StartsWith("54") || zenb.StartsWith("55"))
        {
            _lblTxartelMarka.Text = "Mastercard (Baimendua)";
            _lblTxartelMarka.ForeColor = Color.FromArgb(185, 28, 28);
        }
        else
        {
            _lblTxartelMarka.Text = "Txartel Arrunt (EMV)";
            _lblTxartelMarka.ForeColor = Color.FromArgb(71, 85, 105);
        }
    }

    private void EguneratuBizumBalidazioa()
    {
        string tlf = _txtBizumTelefonoa.Text.Replace(" ", "").Trim();
        bool ok = Regex.IsMatch(tlf, @"^[67]\d{8}$");
        if (ok)
        {
            _lblBizumValid.Text = "Telefono baliozkoa (Berehalako Bizum push-a bidaliko da)";
            _lblBizumValid.ForeColor = Color.FromArgb(22, 101, 52);
            _btnKobratu.Enabled = true;
        }
        else
        {
            _lblBizumValid.Text = "Telefonoak 9 digitu izan behar ditu eta 6 edo 7z hasi";
            _lblBizumValid.ForeColor = Color.FromArgb(220, 38, 38);
            _btnKobratu.Enabled = false;
        }
    }

    private async Task ExekutatuOrdainketaAsync()
    {
        _btnKobratu.Enabled = false;
        _btnKobratu.Text = "ORDAINKETA SEGURUA PROZESATZEN...";

        decimal? jasotakoa = null;
        if (_hautatutakoMota == "Eskudirua")
        {
            string raw = _txtJasotakoa.Text.Replace("€", "").Trim();
            if (decimal.TryParse(raw, out decimal j)) jasotakoa = j;
            else jasotakoa = _eskaera.Guztira;
        }

        var dto = new OrdainketaEskaeraDto
        {
            EskaeraId = _eskaera.Id,
            Mota = _hautatutakoMota,
            Zenbatekoa = _eskaera.Guztira,
            Jasotakoa = jasotakoa,
            TxartelZenbakia = _hautatutakoMota == "Txartela" ? _txtTxartelZenbakia.Text : null,
            Titularra = _hautatutakoMota == "Txartela" ? _txtTitularra.Text : null,
            Iraungitzea = _hautatutakoMota == "Txartela" ? _txtIraungitzea.Text : null,
            Cvv = _hautatutakoMota == "Txartela" ? _txtCvv.Text : null,
            BizumTelefonoa = _hautatutakoMota == "Bizum" ? _txtBizumTelefonoa.Text : null,
            LangileaId = _langilea.Id
        };

        var (arrakasta, mezua, ordainketa) = await _api.ProzesatuOrdainketaAsync(dto);

        if (!arrakasta || ordainketa == null)
        {
            MessageBox.Show(
                $"Errorea ordainketa burutzean:\n{mezua}",
                "Ordainketa Errorea",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            _btnKobratu.Enabled = true;
            _btnKobratu.Text = $"KOBRATU ({_eskaera.Guztira:F2} €) ETA TIKETA SORTU";
            return;
        }

        Emaitza = ordainketa;

        // Tiket fiskalaren leihoa ireki (aurrebista, gorde eta inprimatzeko aukerarekin)
        using var tiketForm = new TiketIkusteaForm(ordainketa);
        tiketForm.ShowDialog();

        DialogResult = DialogResult.OK;
        Close();
    }
}
