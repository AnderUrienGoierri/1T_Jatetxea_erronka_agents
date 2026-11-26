using System.Drawing.Drawing2D;
using Jatetxea.Desktop.Models;
using Jatetxea.Desktop.Services;
using Jatetxea.Desktop.UI;

namespace Jatetxea.Desktop.Forms;

public partial class ErreserbaBerriaModal : Form
{
    private readonly LangileaDto _langilea;
    private readonly ApiService _api;
    private readonly List<MahaiaDto> _mahaiak;

    public ErreserbaDto? SortutakoErreserba { get; private set; }

    /// <summary>
    /// Parameterless constructor for Visual Studio Form Designer.
    /// </summary>
    public ErreserbaBerriaModal() : this(new LangileaDto(), new ApiService(), new List<MahaiaDto>())
    {
    }

    public ErreserbaBerriaModal(LangileaDto langilea, ApiService api, List<MahaiaDto> mahaiak, int? hautatutakoMahaiZenbakia = null, DateTime? hasierakoData = null, string? hasierakoTxanda = null)
    {
        _langilea = langilea;
        _api = api;
        _mahaiak = mahaiak;

        InitializeComponent();

        // T1 Warm Hospitality v2.0 paleta + DoubleBuffering
        Theme.ApplyWarmHospitalityTheme(this);
        BackColor = Theme.WarmLinen;
        pnlBody.BackColor = Theme.WarmLinen;
        pnlFooter.BackColor = Theme.MarbleWhite;
        foreach (Control c in pnlBody.Controls)
        {
            if (c is Label lbl)
            {
                lbl.BackColor = Theme.WarmLinen;
            }
        }
        Theme.StyleButton(_btnUtzi, Theme.StoneMist, Theme.EspressoInk, Theme.WarmTaupe, radius: 10, fontSize: 10f);
        Theme.StyleButton(_btnGorde, Theme.ChiantiPrimary, Theme.MarbleWhite, null, radius: 10, fontSize: 10f);

        if (!string.IsNullOrWhiteSpace(_langilea.Izena))
        {
            lblSub.Text = $"Kudeatzailea: {_langilea.Izena} ({_langilea.Rola}) · Telefonoz edo Pertsonan jasota";
        }

        pnlFooter.Paint += (s, e) =>
        {
            using var pen = new Pen(Theme.Border, 1);
            e.Graphics.DrawLine(pen, 0, 0, pnlFooter.Width, 0);
        };

        _btnUtzi.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
        _btnGorde.Click += async (s, e) => await GordeErreserbaAsync();

        _dtpData.Value = hasierakoData ?? DateTime.Today;

        _cmbTxanda.Items.Clear();
        _cmbTxanda.Items.Add("Bazkaria_1 (13:00 - 14:30)");
        _cmbTxanda.Items.Add("Bazkaria_2 (14:30 - 16:00)");
        _cmbTxanda.Items.Add("Afaria_1 (20:30 - 22:00)");
        _cmbTxanda.Items.Add("Afaria_2 (22:00 - 23:30)");
        _cmbTxanda.SelectedIndex = 0;

        if (!string.IsNullOrWhiteSpace(hasierakoTxanda))
        {
            for (int idx = 0; idx < _cmbTxanda.Items.Count; idx++)
            {
                if (_cmbTxanda.Items[idx]?.ToString()?.StartsWith(hasierakoTxanda, StringComparison.OrdinalIgnoreCase) == true)
                {
                    _cmbTxanda.SelectedIndex = idx;
                    break;
                }
            }
        }

        _cmbMahaia.Items.Clear();
        _cmbMahaia.Items.Add("0 - Automatikoki esleitu mahai librea");
        int aukeratutakoIndizea = 0;
        int i = 1;
        foreach (var m in _mahaiak.OrderBy(m => m.Zenbakia))
        {
            _cmbMahaia.Items.Add($"Mahaia {m.Zenbakia} ({m.Edukiera} pax) · {m.Kokapena}");
            if (hautatutakoMahaiZenbakia.HasValue && m.Zenbakia == hautatutakoMahaiZenbakia.Value)
            {
                aukeratutakoIndizea = i;
            }
            i++;
        }
        _cmbMahaia.SelectedIndex = aukeratutakoIndizea;

        _cmbJatorria.Items.Clear();
        _cmbJatorria.Items.Add("Telefonoz jasotako erreserba (Dei zuzena)");
        _cmbJatorria.Items.Add("Pertsonan jasotako erreserba (Barran edo Sarreran)");
        _cmbJatorria.SelectedIndex = 0;

        // Responsive 2-zutabe GridPane estiloko egokitzea (DPI eskalatua)
        pnlBody.Resize += (s, e) => EgokituFormularioResponsive();
        Load += (s, e) =>
        {
            Theme.ApplyModalDpiSize(this, 820, 660, 660, 540);
            EgokituFormularioResponsive();
        };
    }

    private void EgokituFormularioResponsive()
    {
        int S(int px) => Theme.S(this, px);

        // 1. Goiko barra
        pnlHeader.Height = S(84);
        lblTit.Font = Theme.DisplayFont(13.5f);
        lblTit.ForeColor = Theme.MarbleWhite;
        lblTit.SetBounds(S(24), S(12), Math.Max(S(340), pnlHeader.ClientSize.Width - S(48)), S(32));
        lblSub.Font = Theme.BodyFont(9.2f, bold: false);
        lblSub.ForeColor = Theme.ChiantiTint;
        lblSub.SetBounds(S(24), S(48), Math.Max(S(340), pnlHeader.ClientSize.Width - S(48)), S(24));

        // 2. Formularioaren 2 zutabeak bertikalki eta horizontalki DPI-eskalatuta
        int availW = pnlBody.ClientSize.Width;
        int contentW = Math.Clamp(availW - S(48), S(500), S(860));
        int leftX = Math.Max(S(24), (availW - contentW) / 2);
        int gap = S(20);
        int colW = (contentW - gap) / 2;
        int rightX = leftX + colW + gap;

        int lblH = S(22);
        int inpH = S(32);
        int rowStep = S(72);
        int y = S(16);

        lblIzena.SetBounds(leftX, y, colW, lblH);
        _txtIzena.SetBounds(leftX, y + lblH + S(4), colW, inpH);
        lblAbizena.SetBounds(rightX, y, colW, lblH);
        _txtAbizena.SetBounds(rightX, y + lblH + S(4), colW, inpH);
        y += rowStep;

        lblTel.SetBounds(leftX, y, colW, lblH);
        _txtTelefonoa.SetBounds(leftX, y + lblH + S(4), colW, inpH);
        lblMail.SetBounds(rightX, y, colW, lblH);
        _txtEmaila.SetBounds(rightX, y + lblH + S(4), colW, inpH);
        y += rowStep;

        lblData.SetBounds(leftX, y, colW, lblH);
        _dtpData.SetBounds(leftX, y + lblH + S(4), colW, inpH);
        lblTxanda.SetBounds(rightX, y, colW, lblH);
        _cmbTxanda.SetBounds(rightX, y + lblH + S(4), colW, inpH);
        y += rowStep;

        lblPax.SetBounds(leftX, y, colW, lblH);
        _nudPax.SetBounds(leftX, y + lblH + S(4), colW, inpH);
        lblMahaia.SetBounds(rightX, y, colW, lblH);
        _cmbMahaia.SetBounds(rightX, y + lblH + S(4), colW, inpH);
        y += rowStep;

        lblJatorria.SetBounds(leftX, y, contentW, lblH);
        _cmbJatorria.SetBounds(leftX, y + lblH + S(4), contentW, inpH);
        y += rowStep;

        lblOharrak.SetBounds(leftX, y, contentW, lblH);
        _txtOharrak.SetBounds(leftX, y + lblH + S(4), contentW, S(68));
        y += lblH + S(78);

        _lblErrorea.SetBounds(leftX, y, contentW, S(28));

        // 3. Beheko barra eta botoiak
        pnlFooter.Height = S(72);
        int bH = S(42);
        int bY = (pnlFooter.Height - bH) / 2;
        int wGorde = S(186);
        int wUtzi = S(112);
        _btnGorde.SetBounds(pnlFooter.ClientSize.Width - wGorde - S(24), bY, wGorde, bH);
        _btnUtzi.SetBounds(_btnGorde.Left - wUtzi - S(12), bY, wUtzi, bH);
    }

    private async Task GordeErreserbaAsync()
    {
        _lblErrorea.Text = "";

        string izena = _txtIzena.Text.Trim();
        string abizena = _txtAbizena.Text.Trim();
        string telefonoa = _txtTelefonoa.Text.Trim();
        string emaila = _txtEmaila.Text.Trim();

        if (string.IsNullOrWhiteSpace(izena))
        {
            _lblErrorea.Text = "Bezeroaren izena derrigorrezkoa da.";
            _txtIzena.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(telefonoa))
        {
            _lblErrorea.Text = "Telefonoa derrigorrezkoa da telefonoz/pertsonan kontaktatzeko.";
            _txtTelefonoa.Focus();
            return;
        }

        DateTime data = _dtpData.Value.Date;
        if (data.DayOfWeek == DayOfWeek.Monday)
        {
            _lblErrorea.Text = "Astelehenetan jatetxea itxita dago (atseden eguna).";
            return;
        }

        string rawTxanda = _cmbTxanda.SelectedItem?.ToString() ?? "Bazkaria_1";
        string txanda = rawTxanda.Split(' ')[0]; // Bazkaria_1, Bazkaria_2, Afaria_1, Afaria_2

        int pertsonaKopurua = (int)_nudPax.Value;

        // Mahai aukeraketa
        int? mahaiZenbakia = null;
        if (_cmbMahaia.SelectedIndex > 0)
        {
            string aukera = _cmbMahaia.SelectedItem?.ToString() ?? "";
            // Format: "Mahaia 4 (4 pax)..."
            var zatiak = aukera.Split(' ');
            if (zatiak.Length >= 2 && int.TryParse(zatiak[1], out int mz))
            {
                mahaiZenbakia = mz;
            }
        }

        string jatorria = _cmbJatorria.SelectedItem?.ToString() ?? "Telefonoz";
        string oharrak = _txtOharrak.Text.Trim();
        if (string.IsNullOrWhiteSpace(oharrak))
        {
            oharrak = $"[{jatorria} - Hartzailea: {_langilea.Izena}]";
        }
        else
        {
            oharrak = $"[{jatorria} - Hartzailea: {_langilea.Izena}] " + oharrak;
        }

        _btnGorde.Enabled = false;
        _btnGorde.Text = "GORDETZEN...";

        var dto = new ErreserbaSortuEskaeraDto
        {
            BezeroIzena = izena,
            BezeroAbizena = abizena,
            BezeroTelefonoa = telefonoa,
            BezeroEmaila = string.IsNullOrWhiteSpace(emaila) ? $"{izena.ToLower()}.{telefonoa}@bezeroa.jatetxea.eus" : emaila,
            Data = data.ToString("yyyy-MM-dd"),
            Txanda = txanda,
            PertsonaKopurua = pertsonaKopurua,
            MahaiZenbakia = mahaiZenbakia,
            Oharrak = oharrak
        };

        var (arrakasta, mezua, sortutakoa) = await _api.SortuErreserbaAsync(dto);

        _btnGorde.Enabled = true;
        _btnGorde.Text = "Gorde Erreserba";

        if (arrakasta && sortutakoa != null)
        {
            SortutakoErreserba = sortutakoa;
            MessageBox.Show(
                $"Erreserba arrakastaz gorde da!\n\n" +
                $"Bezeroa: {sortutakoa.BezeroIzena} {sortutakoa.BezeroAbizena}\n" +
                $"Data: {sortutakoa.Data} ({sortutakoa.Txanda})\n" +
                $"Mahaia: Mahaia {sortutakoa.MahaiZenbakia} ({sortutakoa.Kokapena})\n" +
                $"Pertsonak: {sortutakoa.PertsonaKopurua} pax",
                "Erreserba Konfirmatua",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            DialogResult = DialogResult.OK;
            Close();
        }
        else
        {
            _lblErrorea.Text = mezua;
        }
    }
}
