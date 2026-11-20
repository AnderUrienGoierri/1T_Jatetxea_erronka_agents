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
