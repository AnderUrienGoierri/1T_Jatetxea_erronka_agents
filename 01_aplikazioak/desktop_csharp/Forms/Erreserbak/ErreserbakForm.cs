using System.Drawing.Drawing2D;
using Jatetxea.Desktop.Models;
using Jatetxea.Desktop.Services;
using Jatetxea.Desktop.UI;

namespace Jatetxea.Desktop.Forms;

public class ErreserbakForm : Form
{
    private readonly LangileaDto _langilea;
    private readonly ApiService _api;
    private readonly List<MahaiaDto> _mahaiak;

    private readonly DateTimePicker _dtpIragazkia;
    private readonly ComboBox _cmbTxandaIragazkia;
    private readonly FlowLayoutPanel _pnlCards;
    private readonly Label _lblEstatistika;
    private readonly Label _lblKargatzen;

    public ErreserbakForm(LangileaDto langilea, ApiService api, List<MahaiaDto> mahaiak)
    {
        _langilea = langilea;
        _api = api;
        _mahaiak = mahaiak;

        Text = "T1 Jatetxea - Erreserben Kudeaketa Zentrala (TPV)";
        Size = new Size(1000, 700);
        MinimumSize = new Size(850, 550);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Theme.Background;
        Font = Theme.FontBody;

        // 1. Goiko goiburua
        var pnlHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 75,
            BackColor = Theme.AccentWine,
            Padding = new Padding(24, 14, 24, 14)
        };

        var lblTit = new Label
        {
            Text = "Erreserben Kudeaketa Zentrala (TPV)",
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
            Location = new Point(24, 12),
            AutoSize = true
        };

        var lblSub = new Label
        {
            Text = $"Kudeatzailea: {_langilea.Izena} ({_langilea.Rola}) · Telefonoz eta Pertsonan jasotako erreserbak",
            ForeColor = Color.FromArgb(254, 205, 211),
            Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
            Location = new Point(25, 40),
            AutoSize = true
        };

        var btnBerriaTop = new Button
        {
            Text = "+ Erreserba Berria",
            Size = new Size(180, 44),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Location = new Point(pnlHeader.Width - 210, 15),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(22, 163, 74),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnBerriaTop.FlatAppearance.BorderSize = 0;
        btnBerriaTop.Click += (s, e) => IrekiErreserbaBerria();

        pnlHeader.Resize += (s, e) =>
        {
            btnBerriaTop.Location = new Point(pnlHeader.Width - 210, 15);
        };

        pnlHeader.Controls.Add(lblTit);
        pnlHeader.Controls.Add(lblSub);
        pnlHeader.Controls.Add(btnBerriaTop);

        // 2. Iragazki barra
        var pnlFilters = new Panel
        {
            Dock = DockStyle.Top,
            Height = 55,
            BackColor = Color.White,
            Padding = new Padding(24, 10, 24, 10)
        };
        pnlFilters.Paint += (s, e) =>
        {
            using var pen = new Pen(Theme.Border, 1);
            e.Graphics.DrawLine(pen, 0, pnlFilters.Height - 1, pnlFilters.Width, pnlFilters.Height - 1);
        };

        var lblData = new Label
        {
            Text = "Data:",
            Location = new Point(24, 16),
            AutoSize = true,
            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
            ForeColor = Color.FromArgb(71, 85, 105)
        };

        _dtpIragazkia = new DateTimePicker
        {
            Location = new Point(68, 12),
            Size = new Size(140, 30),
            Format = DateTimePickerFormat.Short,
            Font = new Font("Segoe UI", 9.5F),
            Value = DateTime.Today
        };
        _dtpIragazkia.ValueChanged += async (s, e) => await KargatuErreserbakAsync();

        var lblTxanda = new Label
        {
            Text = "Txanda:",
            Location = new Point(230, 16),
            AutoSize = true,
            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
            ForeColor = Color.FromArgb(71, 85, 105)
        };

        _cmbTxandaIragazkia = new ComboBox
        {
            Location = new Point(290, 12),
            Size = new Size(180, 30),
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 9.5F)
        };
        _cmbTxandaIragazkia.Items.Add("Guztiak");
        _cmbTxandaIragazkia.Items.Add("Bazkaria_1");
        _cmbTxandaIragazkia.Items.Add("Bazkaria_2");
        _cmbTxandaIragazkia.Items.Add("Afaria_1");
        _cmbTxandaIragazkia.Items.Add("Afaria_2");
        _cmbTxandaIragazkia.SelectedIndex = 0;
        _cmbTxandaIragazkia.SelectedIndexChanged += async (s, e) => await KargatuErreserbakAsync();

        var btnFreskatu = new Button
        {
            Text = "Freskatu",
            Location = new Point(490, 10),
            Size = new Size(95, 34),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(241, 245, 249),
            ForeColor = Color.FromArgb(51, 65, 85),
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnFreskatu.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnFreskatu.Click += async (s, e) => await KargatuErreserbakAsync();

        _lblEstatistika = new Label
        {
            Location = new Point(610, 16),
            AutoSize = true,
            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
            ForeColor = Theme.AccentWine,
            Text = ""
        };

        pnlFilters.Controls.Add(lblData);
        pnlFilters.Controls.Add(_dtpIragazkia);
        pnlFilters.Controls.Add(lblTxanda);
        pnlFilters.Controls.Add(_cmbTxandaIragazkia);
        pnlFilters.Controls.Add(btnFreskatu);
        pnlFilters.Controls.Add(_lblEstatistika);

        // 3. Erdiko panela (Txartelak)
        _pnlCards = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(24),
            BackColor = Theme.Background
        };

        _lblKargatzen = new Label
        {
            Text = "Erreserbak kargatzen...",
            AutoSize = true,
            Font = new Font("Segoe UI", 11F, FontStyle.Regular),
            ForeColor = Color.FromArgb(100, 116, 139),
            Margin = new Padding(10)
        };
        _pnlCards.Controls.Add(_lblKargatzen);

        Controls.Add(_pnlCards);
        Controls.Add(pnlFilters);
        Controls.Add(pnlHeader);

        Load += async (s, e) => await KargatuErreserbakAsync();
    }

    private async Task KargatuErreserbakAsync()
    {
        _pnlCards.SuspendLayout();
        _pnlCards.Controls.Clear();
        _pnlCards.Controls.Add(_lblKargatzen);
        _lblKargatzen.Text = "Erreserbak zerbitzaritik deskargatzen...";
        _pnlCards.ResumeLayout(true);

        string dataStr = _dtpIragazkia.Value.ToString("yyyy-MM-dd");
        string txandaStr = _cmbTxandaIragazkia.SelectedItem?.ToString() ?? "Guztiak";

        var zerrenda = await _api.GetErreserbakAsync(dataStr, txandaStr);

        _pnlCards.SuspendLayout();
        _pnlCards.Controls.Clear();

        _lblEstatistika.Text = $"Guztira: {zerrenda.Count} erreserba ({dataStr})";

        if (zerrenda.Count == 0)
        {
            var pnlHutsa = new Panel
            {
                Size = new Size(700, 140),
                BackColor = Color.White,
                Margin = new Padding(0, 20, 0, 0),
                Padding = new Padding(24)
            };
            pnlHutsa.Paint += (s, e) =>
            {
                using var pen = new Pen(Theme.Border, 1);
                e.Graphics.DrawRectangle(pen, 0, 0, pnlHutsa.Width - 1, pnlHutsa.Height - 1);
            };

            var lblHutsTit = new Label
            {
                Text = "Ez dago erreserbarik hautatutako datan eta txandan.",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(24, 20),
                AutoSize = true
            };

            var lblHutsSub = new Label
            {
                Text = "Telefonoz edo pertsonan bezero batek mahaia eskatzen badu, sakatu '+ Erreserba Berria' botoia.",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(24, 52),
                AutoSize = true
            };

            var btnSortuHuts = new Button
            {
                Text = "+ Erreserba Sartu Orain",
                Location = new Point(24, 85),
                Size = new Size(180, 36),
                FlatStyle = FlatStyle.Flat,
                BackColor = Theme.AccentWine,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSortuHuts.FlatAppearance.BorderSize = 0;
            btnSortuHuts.Click += (s, e) => IrekiErreserbaBerria();

            pnlHutsa.Controls.Add(lblHutsTit);
            pnlHutsa.Controls.Add(lblHutsSub);
            pnlHutsa.Controls.Add(btnSortuHuts);

            _pnlCards.Controls.Add(pnlHutsa);
        }
        else
        {
            foreach (var err in zerrenda)
            {
                _pnlCards.Controls.Add(SortuErreserbaTxartela(err));
            }
        }

        _pnlCards.ResumeLayout(true);
    }

    private Control SortuErreserbaTxartela(ErreserbaDto err)
    {
        var card = new Panel
        {
            Size = new Size(450, 175),
            BackColor = Color.White,
            Margin = new Padding(0, 0, 16, 16),
            Padding = new Padding(16)
        };
        card.Paint += (s, e) =>
        {
            using var pen = new Pen(Theme.Border, 1);
            e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);

            // Ezkerreko zinta koloreztatua (Erreserba egoera)
            using var brush = new SolidBrush(Theme.Purple);
            e.Graphics.FillRectangle(brush, 0, 0, 5, card.Height);
        };

        // Mahai badgea
        var lblMahaiBadge = new Label
        {
            Text = $"MAHAIA {err.MahaiZenbakia}",
            Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = Theme.AccentWine,
            TextAlign = ContentAlignment.MiddleCenter,
            Location = new Point(14, 14),
            Size = new Size(100, 28)
        };

        // Pax eta Kokapena
        var lblKokapena = new Label
        {
            Text = $"{err.Kokapena} · {err.PertsonaKopurua} pax",
            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
            ForeColor = Color.FromArgb(71, 85, 105),
            Location = new Point(122, 18),
            AutoSize = true
        };

        // Txanda etiketa
        var lblTxanda = new Label
        {
            Text = $"{err.Txanda} ({err.Data})",
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            ForeColor = Theme.Purple,
            Location = new Point(280, 18),
            AutoSize = true
        };

        // Bezeroaren izena
        var lblBezeroa = new Label
        {
            Text = $"{err.BezeroIzena} {err.BezeroAbizena}",
            Font = new Font("Segoe UI", 12F, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42),
            Location = new Point(14, 52),
            AutoSize = true
        };

        // Telefonoa eta Emaila
        var lblKontaktua = new Label
        {
            Text = $"Tel: {err.BezeroTelefonoa}   Email: {err.BezeroEmaila}",
            Font = new Font("Segoe UI", 9F, FontStyle.Regular),
            ForeColor = Color.FromArgb(100, 116, 139),
            Location = new Point(14, 80),
            AutoSize = true
        };

        // Oharrak
        var lblOharrak = new Label
        {
            Text = string.IsNullOrWhiteSpace(err.Oharrak) ? "Oharrik gabe" : err.Oharrak,
            Font = new Font("Segoe UI", 8.5F, FontStyle.Italic),
            ForeColor = Color.FromArgb(100, 116, 139),
            Location = new Point(14, 106),
            Size = new Size(420, 24)
        };

        // Ezeztatu botoia
        var btnEzeztatu = new Button
        {
            Text = "Ezeztatu",
            Size = new Size(90, 30),
            Location = new Point(345, 135),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(254, 242, 242),
            ForeColor = Color.FromArgb(220, 38, 38),
            Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnEzeztatu.FlatAppearance.BorderColor = Color.FromArgb(252, 165, 165);
        btnEzeztatu.Click += async (s, e) =>
        {
            var res = MessageBox.Show(
                $"Ziur zaude {err.BezeroIzena} bezeroaren erreserba (Mahaia {err.MahaiZenbakia}) ezeztatu nahi duzula?",
                "Erreserba Ezeztatu",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (res == DialogResult.Yes)
            {
                var (ok, msg) = await _api.EzeztatuErreserbaAsync(err.Id);
                if (ok)
                {
                    await KargatuErreserbakAsync();
                }
                else
                {
                    MessageBox.Show(msg, "Errorea", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        };

        var lblEgoeraBadge = new Label
        {
            Text = $"EGOERA: {err.Egoera.ToUpper()}",
            Font = new Font("Segoe UI", 8F, FontStyle.Bold),
            ForeColor = Color.FromArgb(22, 163, 74),
            Location = new Point(14, 140),
            AutoSize = true
        };

        card.Controls.Add(lblMahaiBadge);
        card.Controls.Add(lblKokapena);
        card.Controls.Add(lblTxanda);
        card.Controls.Add(lblBezeroa);
        card.Controls.Add(lblKontaktua);
        card.Controls.Add(lblOharrak);
        card.Controls.Add(lblEgoeraBadge);
        card.Controls.Add(btnEzeztatu);

        return card;
    }

    private void IrekiErreserbaBerria()
    {
        using var modal = new ErreserbaBerriaModal(_langilea, _api, _mahaiak);
        if (modal.ShowDialog(this) == DialogResult.OK)
        {
            _dtpIragazkia.Value = DateTime.Today;
            _ = KargatuErreserbakAsync();
        }
    }
}
