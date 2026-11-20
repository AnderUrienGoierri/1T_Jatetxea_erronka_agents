using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using Jatetxea.Desktop.Models;
using Jatetxea.Desktop.Services;

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

        AldatuMota("Eskudirua");
        EguneratuKanbioa();
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
