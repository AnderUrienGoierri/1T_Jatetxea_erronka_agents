using Jatetxea.Desktop.UI;

namespace Jatetxea.Desktop.Forms;

public partial class OharraModal : Form
{
    public string? Oharra { get; private set; }

    private readonly List<Button> _presetButtons = new();
    private readonly string[] _aurrezarpenak = new[]
    {
        "Gazta gutxi",
        "Gaztarik gabe",
        "Tipularik gabe",
        "Gatzik gabe",
        "Puntuan",
        "Gutxi eginda",
        "Ondo eginda",
        "Saltsa aparte",
        "Glutenik gabe",
        "Laktosarik gabe",
        "Izotzik gabe",
        "Izotzarekin",
        "Limoiarekin",
        "Epela",
        "Ogi gehiago"
    };

    public OharraModal(string platerIzena, string? unekoOharra = null)
    {
        InitializeComponent();
        Oharra = unekoOharra;

        // T1 Warm Hospitality v2.0 paleta + DoubleBuffering
        Theme.ApplyWarmHospitalityTheme(this);
        BackColor = Theme.WarmLinen;
        pnlGoiburua.BackColor = Theme.MarbleWhite;
        pnlBehekoa.BackColor = Theme.MarbleWhite;
        pnlPresets.BackColor = Theme.WarmLinen;
        lblTitulua.Font = Theme.DisplayFont(13.5f);
        lblTitulua.ForeColor = Theme.ChiantiPrimary;
        Theme.StyleButton(btnGorde, Theme.ChiantiPrimary, Theme.MarbleWhite, null, radius: 10, fontSize: 10.5f);
        Theme.StyleButton(btnKendu, Theme.DangerBg, Theme.Danger, Theme.DangerBorder, radius: 10, fontSize: 10f);
        Theme.StyleButton(btnUtzi, Theme.StoneMist, Theme.EspressoInk, Theme.WarmTaupe, radius: 10, fontSize: 10f);

        lblPlaterIzena.Text = platerIzena;
        txtOharra.Text = unekoOharra ?? string.Empty;

        SortuPresetBotoiak();
        LotuEkitaldiak();
        EguneratuUI();

        Resize += (s, e) => EgokituResponsiveDiseinua();
        Load += (s, e) =>
        {
            Theme.ApplyModalDpiSize(this, 760, 560, 620, 460);
            EgokituResponsiveDiseinua();
        };
    }

    /// <summary>
    /// Visual Studio WinForms Designer-erako eraikitzailea
    /// </summary>
    public OharraModal() : this("Plateraren Izena", null)
    {
    }

    private void EgokituResponsiveDiseinua()
    {
        int S(int px) => Theme.S(this, px);
        pnlGoiburua.Height = S(76);
        pnlBehekoa.Height = S(78);

        int contentW = Math.Clamp(ClientSize.Width - S(48), S(500), S(860));
        int leftX = (ClientSize.Width - contentW) / 2;

        lblTitulua.SetBounds(leftX, S(10), contentW, S(28));
        lblPlaterIzena.SetBounds(leftX, S(40), contentW, S(26));

        int topY = pnlGoiburua.Bottom + S(12);
        int bottomLimit = pnlBehekoa.Top - S(14);
        int availH = Math.Max(S(200), bottomLimit - topY);

        lblPresetsTit.SetBounds(leftX, topY, contentW, S(24));
        int presetsH = Math.Clamp((int)(availH * 0.45), S(105), S(240));
        pnlPresets.SetBounds(leftX, topY + S(26), contentW, presetsH);

        int idatziY = pnlPresets.Bottom + S(10);
        int karW = S(180);
        lblIdatziTit.SetBounds(leftX, idatziY, contentW - karW, S(24));
        lblKaraktereak.SetBounds(leftX + contentW - karW, idatziY, karW, S(24));

        int txtY = idatziY + S(26);
        int txtH = Math.Max(S(64), bottomLimit - txtY);
        txtOharra.SetBounds(leftX, txtY, contentW, txtH);

        // Beheko 3 botoiak proportzionalki banatu
        int gap = S(12);
        int btnH = S(46);
        int btnY = (pnlBehekoa.Height - btnH) / 2;
        int btnGordeW = (int)(contentW * 0.42);
        int btnKenduW = (int)(contentW * 0.34);
        int btnUtziW = contentW - btnGordeW - btnKenduW - gap * 2;
        btnGorde.SetBounds(leftX, btnY, btnGordeW, btnH);
        btnKendu.SetBounds(btnGorde.Right + gap, btnY, btnKenduW, btnH);
        btnUtzi.SetBounds(btnKendu.Right + gap, btnY, btnUtziW, btnH);
    }

    private void SortuPresetBotoiak()
    {
        pnlPresets.SuspendLayout();
        pnlPresets.Controls.Clear();
        _presetButtons.Clear();

        foreach (var preset in _aurrezarpenak)
        {
            var btn = new Button
            {
                Text = preset,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
                Height = 34,
                AutoSize = true,
                Margin = new Padding(0, 0, 8, 8),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                UseCompatibleTextRendering = false
            };

            btn.Click += (s, e) => TxipaTxandakatu(preset);
            _presetButtons.Add(btn);
            pnlPresets.Controls.Add(btn);
        }

        pnlPresets.ResumeLayout(true);
    }

    private void LotuEkitaldiak()
    {
        txtOharra.TextChanged += (s, e) => EguneratuUI();

        btnGorde.Click += (s, e) =>
        {
            Oharra = string.IsNullOrWhiteSpace(txtOharra.Text) ? null : txtOharra.Text.Trim();
            DialogResult = DialogResult.OK;
            Close();
        };

        btnKendu.Click += (s, e) =>
        {
            Oharra = null;
            DialogResult = DialogResult.OK;
            Close();
        };

        btnUtzi.Click += (s, e) =>
        {
            DialogResult = DialogResult.Cancel;
            Close();
        };

        // Goiko eta beheko barren marrak
        pnlGoiburua.Paint += (s, e) =>
        {
            using var pen = new Pen(Color.FromArgb(226, 232, 240), 1.5f);
            e.Graphics.DrawLine(pen, 0, pnlGoiburua.Height - 1, pnlGoiburua.Width, pnlGoiburua.Height - 1);
        };

        pnlBehekoa.Paint += (s, e) =>
        {
            using var pen = new Pen(Color.FromArgb(226, 232, 240), 1.5f);
            e.Graphics.DrawLine(pen, 0, 0, pnlBehekoa.Width, 0);
        };
    }

    private void TxipaTxandakatu(string preset)
    {
        var testua = txtOharra.Text.Trim();
        var zatiak = testua
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();

        int index = zatiak.FindIndex(z => string.Equals(z, preset, StringComparison.OrdinalIgnoreCase));
        if (index >= 0)
        {
            zatiak.RemoveAt(index);
        }
        else
        {
            zatiak.Add(preset);
        }

        txtOharra.Text = string.Join(", ", zatiak);
        txtOharra.SelectionStart = txtOharra.Text.Length;
    }

    private void EguneratuUI()
    {
        var testua = txtOharra.Text.Trim();
        var zatiak = testua
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();

        // Preset botoien itxura eguneratu (hautatua vs ez hautatua)
        foreach (var btn in _presetButtons)
        {
            bool hautatua = zatiak.Any(z => string.Equals(z, btn.Text, StringComparison.OrdinalIgnoreCase));
            if (hautatua)
            {
                btn.BackColor = Color.FromArgb(136, 19, 55);
                btn.ForeColor = Color.White;
                btn.FlatAppearance.BorderColor = Color.FromArgb(136, 19, 55);
                btn.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            }
            else
            {
                btn.BackColor = Color.FromArgb(241, 245, 249);
                btn.ForeColor = Color.FromArgb(30, 41, 59);
                btn.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
                btn.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            }
        }

        // Karaktere kontagailua
        int len = txtOharra.Text.Length;
        lblKaraktereak.Text = $"{len} / 150 karaktere";
        lblKaraktereak.ForeColor = len > 140 ? Color.FromArgb(220, 38, 38) : Color.FromArgb(148, 163, 184);

        // Kendu botoia gaitu/ezgaitu
        btnKendu.Enabled = !string.IsNullOrWhiteSpace(testua);
    }
}

