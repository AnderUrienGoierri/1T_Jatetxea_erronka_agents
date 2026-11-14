using System.Drawing.Drawing2D;
using Jatetxea.Desktop.Models;
using Jatetxea.Desktop.Services;
using Jatetxea.Desktop.UI;

namespace Jatetxea.Desktop.Forms;

public class ErreserbaBerriaModal : Form
{
    private readonly LangileaDto _langilea;
    private readonly ApiService _api;
    private readonly List<MahaiaDto> _mahaiak;

    private readonly TextBox _txtIzena;
    private readonly TextBox _txtAbizena;
    private readonly TextBox _txtTelefonoa;
    private readonly TextBox _txtEmaila;
    private readonly DateTimePicker _dtpData;
    private readonly ComboBox _cmbTxanda;
    private readonly NumericUpDown _nudPax;
    private readonly ComboBox _cmbMahaia;
    private readonly ComboBox _cmbJatorria;
    private readonly TextBox _txtOharrak;
    private readonly Label _lblErrorea;
    private readonly Button _btnGorde;
    private readonly Button _btnUtzi;

    public ErreserbaDto? SortutakoErreserba { get; private set; }

    public ErreserbaBerriaModal(LangileaDto langilea, ApiService api, List<MahaiaDto> mahaiak, int? hautatutakoMahaiZenbakia = null)
    {
        _langilea = langilea;
        _api = api;
        _mahaiak = mahaiak;

        Text = "T1 Jatetxea - Erreserba Berria Sartu";
        Size = new Size(540, 680);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Theme.Background;
        Font = Theme.FontBody;

        // 1. Goiburua
        var pnlHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 70,
            BackColor = Theme.AccentWine,
            Padding = new Padding(20, 12, 20, 12)
        };

        var lblTit = new Label
        {
            Text = "Erreserba Berria Sartu",
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 13.5F, FontStyle.Bold),
            Dock = DockStyle.Top,
            Height = 24
        };

        var lblSub = new Label
        {
            Text = $"Kudeatzailea: {_langilea.Izena} ({_langilea.Rola}) · Telefonoz edo Pertsonan jasota",
            ForeColor = Color.FromArgb(254, 205, 211),
            Font = new Font("Segoe UI", 9F, FontStyle.Regular),
            Dock = DockStyle.Top,
            Height = 20
        };

        pnlHeader.Controls.Add(lblSub);
        pnlHeader.Controls.Add(lblTit);

        // 2. Beheko botoien panela
        var pnlFooter = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 70,
            BackColor = Color.White,
            Padding = new Padding(20, 14, 20, 14)
        };
        pnlFooter.Paint += (s, e) =>
        {
            using var pen = new Pen(Theme.Border, 1);
            e.Graphics.DrawLine(pen, 0, 0, pnlFooter.Width, 0);
        };

        _btnUtzi = new Button
        {
            Text = "Utzi",
            Size = new Size(110, 42),
            Location = new Point(270, 14),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(241, 245, 249),
            ForeColor = Color.FromArgb(51, 65, 85),
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        _btnUtzi.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        _btnUtzi.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

        _btnGorde = new Button
        {
            Text = "Gorde Erreserba",
            Size = new Size(130, 42),
            Location = new Point(390, 14),
            FlatStyle = FlatStyle.Flat,
            BackColor = Theme.AccentWine,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        _btnGorde.FlatAppearance.BorderSize = 0;
        _btnGorde.Click += async (s, e) => await GordeErreserbaAsync();

        pnlFooter.Controls.Add(_btnUtzi);
        pnlFooter.Controls.Add(_btnGorde);

        // 3. Gorputz nagusia (Formularioko eremuak)
        var pnlBody = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(24, 16, 24, 16),
            BackColor = Color.White
        };

        int y = 14;
        const int labelH = 20;
        const int inputH = 34;
        const int gap = 14;

        // Bezeroaren Izena eta Abizena
        var lblIzena = SortuEtiketa("Bezeroaren Izena (*):", 0, y);
        var lblAbizena = SortuEtiketa("Abizena:", 250, y);
        pnlBody.Controls.Add(lblIzena);
        pnlBody.Controls.Add(lblAbizena);
        y += labelH + 2;

        _txtIzena = new TextBox { Location = new Point(0, y), Size = new Size(236, inputH), Font = new Font("Segoe UI", 10.5F) };
        _txtAbizena = new TextBox { Location = new Point(250, y), Size = new Size(236, inputH), Font = new Font("Segoe UI", 10.5F) };
        pnlBody.Controls.Add(_txtIzena);
        pnlBody.Controls.Add(_txtAbizena);
        y += inputH + gap;

        // Telefonoa eta Emaila
        var lblTel = SortuEtiketa("Telefonoa (* kontaktu zuzenerako):", 0, y);
        var lblMail = SortuEtiketa("Emaila (aukerakoa):", 250, y);
        pnlBody.Controls.Add(lblTel);
        pnlBody.Controls.Add(lblMail);
        y += labelH + 2;

        _txtTelefonoa = new TextBox { Location = new Point(0, y), Size = new Size(236, inputH), Font = new Font("Segoe UI", 10.5F) };
        _txtEmaila = new TextBox { Location = new Point(250, y), Size = new Size(236, inputH), Font = new Font("Segoe UI", 10.5F) };
        pnlBody.Controls.Add(_txtTelefonoa);
        pnlBody.Controls.Add(_txtEmaila);
        y += inputH + gap;

        // Data eta Txanda
        var lblData = SortuEtiketa("Erreserba Data (*):", 0, y);
        var lblTxanda = SortuEtiketa("Txanda (*):", 250, y);
        pnlBody.Controls.Add(lblData);
        pnlBody.Controls.Add(lblTxanda);
        y += labelH + 2;

        _dtpData = new DateTimePicker
        {
            Location = new Point(0, y),
            Size = new Size(236, inputH),
            Format = DateTimePickerFormat.Short,
            Font = new Font("Segoe UI", 10.5F),
            Value = DateTime.Today
        };
        _cmbTxanda = new ComboBox
        {
            Location = new Point(250, y),
            Size = new Size(236, inputH),
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 10.5F)
        };
        _cmbTxanda.Items.Add("Bazkaria_1 (13:00 - 14:30)");
        _cmbTxanda.Items.Add("Bazkaria_2 (14:30 - 16:00)");
        _cmbTxanda.Items.Add("Afaria_1 (20:30 - 22:00)");
        _cmbTxanda.Items.Add("Afaria_2 (22:00 - 23:30)");
        _cmbTxanda.SelectedIndex = 0;

        pnlBody.Controls.Add(_dtpData);
        pnlBody.Controls.Add(_cmbTxanda);
        y += inputH + gap;

        // Pax (Pertsona kopurua) eta Mahaia
        var lblPax = SortuEtiketa("Pertsona Kopurua (Pax):", 0, y);
        var lblMahaia = SortuEtiketa("Mahaia Hautatu:", 250, y);
        pnlBody.Controls.Add(lblPax);
        pnlBody.Controls.Add(lblMahaia);
        y += labelH + 2;

        _nudPax = new NumericUpDown
        {
            Location = new Point(0, y),
            Size = new Size(236, inputH),
            Minimum = 1,
            Maximum = 30,
            Value = 2,
            Font = new Font("Segoe UI", 10.5F)
        };

        _cmbMahaia = new ComboBox
        {
            Location = new Point(250, y),
            Size = new Size(236, inputH),
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 10F)
        };
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

        pnlBody.Controls.Add(_nudPax);
        pnlBody.Controls.Add(_cmbMahaia);
        y += inputH + gap;

        // Erreserbaren jatorria (Telefonoz / Pertsonan)
        var lblJatorria = SortuEtiketa("Erreserbaren Jatorria:", 0, y);
        pnlBody.Controls.Add(lblJatorria);
        y += labelH + 2;

        _cmbJatorria = new ComboBox
        {
            Location = new Point(0, y),
            Size = new Size(486, inputH),
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 10F)
        };
        _cmbJatorria.Items.Add("Telefonoz jasotako erreserba (Dei zuzena)");
        _cmbJatorria.Items.Add("Pertsonan jasotako erreserba (Barran edo Sarreran)");
        _cmbJatorria.SelectedIndex = 0;
        pnlBody.Controls.Add(_cmbJatorria);
        y += inputH + gap;

        // Oharrak
        var lblOharrak = SortuEtiketa("Oharrak (Alergiak, lehentasunak, etab.):", 0, y);
        pnlBody.Controls.Add(lblOharrak);
        y += labelH + 2;

        _txtOharrak = new TextBox
        {
            Location = new Point(0, y),
            Size = new Size(486, 60),
            Multiline = true,
            Font = new Font("Segoe UI", 9.5F),
            ScrollBars = ScrollBars.Vertical
        };
        pnlBody.Controls.Add(_txtOharrak);
        y += 64;

        // Errore mezua
        _lblErrorea = new Label
        {
            Location = new Point(0, y),
            Size = new Size(486, 26),
            ForeColor = Color.FromArgb(220, 38, 38),
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            Text = ""
        };
        pnlBody.Controls.Add(_lblErrorea);

        Controls.Add(pnlBody);
        Controls.Add(pnlFooter);
        Controls.Add(pnlHeader);
    }

    private static Label SortuEtiketa(string text, int x, int y)
    {
        return new Label
        {
            Text = text,
            Location = new Point(x, y),
            AutoSize = true,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            ForeColor = Color.FromArgb(51, 65, 85)
        };
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
