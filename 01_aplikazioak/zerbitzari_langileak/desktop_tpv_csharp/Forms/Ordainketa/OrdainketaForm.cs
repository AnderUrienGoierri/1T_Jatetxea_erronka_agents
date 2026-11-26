using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using Jatetxea.Desktop.Models;
using Jatetxea.Desktop.Services;
using Jatetxea.Desktop.UI;

namespace Jatetxea.Desktop.Forms;

public partial class OrdainketaForm : Form
{
    private readonly MahaiaDto _mahaia;
    private readonly EskaeraDto _eskaera;
    private readonly LangileaDto _langilea;
    private readonly ApiService _api;

    private string _hautatutakoMota = "Eskudirua"; // "Eskudirua", "Txartela", "Bizum"
    public OrdainketaErantzunaDto? Emaitza { get; private set; }

    /// <summary>
    /// Parameterless constructor for Visual Studio Form Designer.
    /// </summary>
    public OrdainketaForm() : this(new MahaiaDto(), new EskaeraDto(), new LangileaDto(), new ApiService())
    {
    }

    public OrdainketaForm(MahaiaDto mahaia, EskaeraDto eskaera, LangileaDto langilea, ApiService api)
    {
        _mahaia = mahaia;
        _eskaera = eskaera;
        _langilea = langilea;
        _api = api;

        InitializeComponent();

        // T1 Warm Hospitality v2.0 paleta + DoubleBuffering
        Theme.ApplyWarmHospitalityTheme(this);
        BackColor = Theme.WarmLinen;
        pnlBody.BackColor = Theme.WarmLinen;
        pnlTabs.BackColor = Theme.MarbleWhite;
        pnlBottom.BackColor = Theme.MarbleWhite;
        lblTit.Font = Theme.DisplayFont(15f);
        lblTotala.Font = Theme.MonoFont(24f, bold: true);
        _lblKanbioa.Font = Theme.MonoFont(22f, bold: true);
        _txtJasotakoa.Font = Theme.MonoFont(18f, bold: true);
        Theme.StyleButton(btnUtzi, Theme.StoneMist, Theme.EspressoInk, Theme.WarmTaupe, radius: 10, fontSize: 10f);
        Theme.StyleButton(_btnKobratu, Theme.ChiantiPrimary, Theme.MarbleWhite, null, radius: 12, fontSize: 11f);
        Theme.ApplyRoundedRegion(_btnEskudiruaTab, 10);
        Theme.ApplyRoundedRegion(_btnTxartelaTab, 10);
        Theme.ApplyRoundedRegion(_btnBizumTab, 10);

        Text = $"T1 Jatetxea - Mahaia {_mahaia.Zenbakia} Kontua Kobratu (POS)";

        decimal subtotal = Math.Round(_eskaera.Guztira / 1.10m, 2);
        decimal bez = _eskaera.Guztira - subtotal;

        lblTit.Text = $"KONTUA KOBRATU · MAHAIA {_mahaia.Zenbakia}";
        lblSub.Text = $"Eskaera #{_eskaera.Id} · {_eskaera.Lerroak.Sum(l => l.Kantitatea)} plater · Zerbitzaria: {_langilea.Izena}";
        lblTotala.Text = $"{_eskaera.Guztira:F2} €";
        lblBezInfo.Text = $"Azpitotala: {subtotal:F2} € · BEZ (%10): {bez:F2} €";
        _txtJasotakoa.Text = $"{_eskaera.Guztira:F2}";
        _btnKobratu.Text = $"KOBRATU ({_eskaera.Guztira:F2} €) ETA TIKETA SORTU";

        pnlTabs.Paint += (s, e) =>
        {
            using var pen = new Pen(Color.FromArgb(226, 232, 240), 1.5f);
            e.Graphics.DrawLine(pen, 0, pnlTabs.Height - 1, pnlTabs.Width, pnlTabs.Height - 1);
        };

        _btnEskudiruaTab.Click += (s, e) => AldatuMota("Eskudirua");
        _btnTxartelaTab.Click += (s, e) => AldatuMota("Txartela");
        _btnBizumTab.Click += (s, e) => AldatuMota("Bizum");

        _txtJasotakoa.TextChanged += (s, e) => EguneratuKanbioa();

        SortuBotoiAzkarra(pnlAzkarrak, "Zehatza", _eskaera.Guztira);
        SortuBotoiAzkarra(pnlAzkarrak, "+ 5 €", Math.Ceiling(_eskaera.Guztira / 5m) * 5m);
        SortuBotoiAzkarra(pnlAzkarrak, "+ 10 €", Math.Ceiling(_eskaera.Guztira / 10m) * 10m);
        SortuBotoiAzkarra(pnlAzkarrak, "20 €", 20m);
        SortuBotoiAzkarra(pnlAzkarrak, "50 €", 50m);
        SortuBotoiAzkarra(pnlAzkarrak, "100 €", 100m);

        pnlKanbioaKutxa.Paint += (s, e) =>
        {
            using var pen = new Pen(Color.FromArgb(203, 213, 225), 1.2f);
            e.Graphics.DrawRectangle(pen, 0, 0, pnlKanbioaKutxa.Width - 1, pnlKanbioaKutxa.Height - 1);
        };

        _txtTxartelZenbakia.TextChanged += (s, e) => EguneratuTxartelMarka();

        btnPresetVisa.Click += (s, e) =>
        {
            _txtTxartelZenbakia.Text = "4532 8921 7842 4821";
            _txtTitularra.Text = "Aitor Martinez Goikoetxea";
            EguneratuTxartelMarka();
        };

        btnPresetMastercard.Click += (s, e) =>
        {
            _txtTxartelZenbakia.Text = "5412 7534 8921 9912";
            _txtTitularra.Text = "Maite Iriarte Arana";
            EguneratuTxartelMarka();
        };

        _txtBizumTelefonoa.TextChanged += (s, e) => EguneratuBizumBalidazioa();
        btnPresetBizum.Click += (s, e) => _txtBizumTelefonoa.Text = "688 98 76 54";

        pnlBottom.Paint += (s, e) =>
        {
            using var pen = new Pen(Color.FromArgb(226, 232, 240), 1.5f);
            e.Graphics.DrawLine(pen, 0, 0, pnlBottom.Width, 0);
        };

        btnUtzi.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
        _btnKobratu.Click += async (s, e) => await ExekutatuOrdainketaAsync();

        // Responsive: 3 ordainketa-fitxak eta barruko eremuak zabalera guztian banatu (DPI eskalatua)
        pnlTabs.Resize += (s, e) => EgokituOrdainketaResponsive();
        Resize += (s, e) => EgokituOrdainketaResponsive();
        Load += (s, e) =>
        {
            Theme.ApplyModalDpiSize(this, 880, 680, 740, 560);
            EgokituOrdainketaResponsive();
        };

        AldatuMota("Eskudirua");
        EguneratuKanbioa();
    }

    private void EgokituOrdainketaResponsive()
    {
        int S(int px) => Theme.S(this, px);

        // 1. Goiko Chianti barra (titulu/azpititulu eta totala/BEZ gainjarpenik gabe)
        pnlHeader.Height = S(104);
        int pad = S(24);
        int hdrW = pnlHeader.ClientSize.Width;
        int rightW = S(280);
        lblTit.AutoSize = false;
        lblTit.SetBounds(pad, S(14), Math.Max(S(260), hdrW - rightW - pad * 2), S(38));
        lblSub.AutoSize = false;
        lblSub.SetBounds(pad, S(58), Math.Max(S(260), hdrW - rightW - pad * 2), S(28));

        lblTotala.AutoSize = false;
        lblTotala.TextAlign = ContentAlignment.MiddleRight;
        lblTotala.SetBounds(hdrW - rightW - pad, S(10), rightW, S(48));
        lblBezInfo.AutoSize = false;
        lblBezInfo.TextAlign = ContentAlignment.MiddleRight;
        lblBezInfo.SetBounds(hdrW - rightW - pad, S(62), rightW, S(26));

        // 2. 3 ordainketa-fitxa proportzionalak
        pnlTabs.Height = S(62);
        int gap = S(12);
        int availW = Math.Max(S(500), pnlTabs.ClientSize.Width - pad * 2);
        int tabW = (availW - gap * 2) / 3;
        int tabH = S(42);
        int tabY = (pnlTabs.Height - tabH) / 2;
        _btnEskudiruaTab.SetBounds(pad, tabY, tabW, tabH);
        _btnTxartelaTab.SetBounds(pad + tabW + gap, tabY, tabW, tabH);
        _btnBizumTab.SetBounds(pad + (tabW + gap) * 2, tabY, tabW, tabH);

        // 3. Gorputza: Eskudirua, Txartela, Bizum guztiz DPI-eskalatuta
        pnlBody.Padding = new Padding(S(24), S(18), S(24), S(18));
        int innerW = Math.Max(S(460), pnlBody.ClientSize.Width - S(64));

        // 3a) Eskudirua
        lblJasotakoaTit.SetBounds(S(6), S(6), innerW, S(26));
        _txtJasotakoa.SetBounds(S(6), S(38), S(240), S(46));
        lblEuro.SetBounds(_txtJasotakoa.Right + S(10), S(42), S(34), S(38));

        pnlAzkarrak.SetBounds(S(6), S(98), innerW, S(50));
        foreach (Control c in pnlAzkarrak.Controls)
        {
            if (c is Button b)
            {
                var sz = TextRenderer.MeasureText(b.Text, b.Font);
                b.Size = new Size(Math.Max(S(82), sz.Width + S(26)), S(40));
                b.Margin = new Padding(0, 0, S(8), S(6));
            }
        }

        pnlKanbioaKutxa.SetBounds(S(6), S(162), innerW, S(108));
        lblKanbioTit.SetBounds(S(18), S(14), innerW - S(36), S(24));
        _lblKanbioa.SetBounds(S(18), S(44), innerW - S(36), S(50));

        // 3b) Txartela
        lblTxartelTit.SetBounds(S(6), S(6), innerW, S(24));
        _txtTxartelZenbakia.SetBounds(S(6), S(34), S(340), S(36));
        _lblTxartelMarka.SetBounds(_txtTxartelZenbakia.Right + S(12), S(36), S(180), S(32));
        lblIraungTit.SetBounds(S(6), S(82), S(160), S(24));
        _txtIraungitzea.SetBounds(S(6), S(110), S(140), S(34));
        lblCvvTit.SetBounds(S(168), S(82), S(100), S(24));
        _txtCvv.SetBounds(S(168), S(110), S(100), S(34));
        lblTitularTit.SetBounds(S(290), S(82), Math.Max(S(200), innerW - S(296)), S(24));
        _txtTitularra.SetBounds(S(290), S(110), Math.Max(S(200), innerW - S(296)), S(34));
        btnPresetVisa.SetBounds(S(6), S(160), S(190), S(36));
        btnPresetMastercard.SetBounds(btnPresetVisa.Right + S(12), S(160), S(220), S(36));
        lblSegurtasuna.SetBounds(S(6), S(210), innerW, S(46));

        // 3c) Bizum
        lblBizumTit.SetBounds(S(6), S(6), innerW, S(24));
        _txtBizumTelefonoa.SetBounds(S(6), S(36), S(260), S(40));
        _lblBizumValid.SetBounds(_txtBizumTelefonoa.Right + S(14), S(40), S(240), S(32));
        btnPresetBizum.SetBounds(S(6), S(90), S(230), S(38));
        lblBizumInfo.SetBounds(S(6), S(144), innerW, S(54));

        // 4. Beheko barra (Utzi + Kobratu botoiak)
        pnlBottom.Height = S(78);
        int bH = S(46);
        int bY = (pnlBottom.Height - bH) / 2;
        int utziW = S(110);
        btnUtzi.SetBounds(pad, bY, utziW, bH);
        _btnKobratu.SetBounds(btnUtzi.Right + S(14), bY, Math.Max(S(260), pnlBottom.ClientSize.Width - btnUtzi.Right - S(14) - pad), bH);
    }

    private void SortuBotoiAzkarra(FlowLayoutPanel pnl, string testua, decimal zenbatekoa)
    {
        var btn = new Button
        {
            Text = testua,
            Size = new Size(Theme.S(this, 88), Theme.S(this, 40)),
            Margin = new Padding(0, 0, Theme.S(this, 8), 0)
        };
        Theme.StyleButton(btn, Theme.StoneMist, Theme.EspressoInk, Theme.WarmTaupe, radius: 8, fontSize: 9.5f);
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
        if (_eskaera.Guztira <= 0)
        {
            var (ok, msg) = await _api.EzabatuEskaeraAsync(_eskaera.Id);
            if (ok)
            {
                MessageBox.Show("Kontu honek ez du platerik (0,00 €). Kontua itxi eta ezabatu da.", "Kontua Itxita", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
                return;
            }
        }

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
