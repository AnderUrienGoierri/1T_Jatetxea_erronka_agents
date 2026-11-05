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

        lblPlaterIzena.Text = platerIzena;
        txtOharra.Text = unekoOharra ?? string.Empty;

        SortuPresetBotoiak();
        LotuEkitaldiak();
        EguneratuUI();
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

