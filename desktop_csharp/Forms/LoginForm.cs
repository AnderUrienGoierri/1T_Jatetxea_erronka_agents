using System.Drawing.Drawing2D;
using Jatetxea.Desktop.Models;
using Jatetxea.Desktop.Services;
using Jatetxea.Desktop.UI;

namespace Jatetxea.Desktop.Forms;

public class LoginForm : Form
{
    private readonly ApiService _api;
    private readonly FlowLayoutPanel _panelLangileak;
    private readonly Label _lblPinDisplay;
    private readonly Label _lblStatus;
    private readonly Label _lblAukeratutakoLangilea;
    private readonly Button _btnLogin;

    private List<LangileaDto> _langileak = new();
    private LangileaDto? _aukeratutakoLangilea;
    private string _pin = string.Empty;

    public LoginForm()
    {
        _api = new ApiService();

        Text = "T1 Jatetxea - Saioa Hasi (TPV Tablet)";
        Size = new Size(1024, 700);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Theme.Background;
        MinimumSize = new Size(900, 600);

        // Edukiontzi Nagusia (Split: Ezkerra Langileak, Eskuina PIN Teklatua)
        var splitContainer = new SplitContainer
        {
            Dock = DockStyle.Fill,
            Orientation = Orientation.Vertical,
            SplitterDistance = 560,
            IsSplitterFixed = true,
            BackColor = Theme.Border
        };
        splitContainer.Panel1.BackColor = Theme.Background;
        splitContainer.Panel2.BackColor = Theme.Surface;
        Controls.Add(splitContainer);

        // === EZKERREKO PANELA: Langileen Aukeratzailea ===
        var ezkerPanela = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(30),
            BackColor = Theme.Background
        };
        splitContainer.Panel1.Controls.Add(ezkerPanela);

        // Titulua eta Azpititulua
        var lblTitulua = new Label
        {
            Text = "T1 JATETXEA",
            Font = new Font("Segoe UI", 22, FontStyle.Bold),
            ForeColor = Theme.AccentWine,
            AutoSize = true,
            Location = new Point(30, 25)
        };
        ezkerPanela.Controls.Add(lblTitulua);

        var lblAzpititulua = new Label
        {
            Text = "Hautatu zure erabiltzailea saioa hasteko:",
            Font = Theme.FontSubtitle,
            ForeColor = Theme.TextMuted,
            AutoSize = true,
            Location = new Point(30, 70)
        };
        ezkerPanela.Controls.Add(lblAzpititulua);

        _panelLangileak = new FlowLayoutPanel
        {
            Location = new Point(30, 110),
            Size = new Size(500, 480),
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            AutoScroll = true,
            BackColor = Color.Transparent
        };
        ezkerPanela.Controls.Add(_panelLangileak);

        // === ESKUINEKO PANELA: PIN Teklatua eta Display-a ===
        var eskuinPanela = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(35),
            BackColor = Theme.Surface
        };
        splitContainer.Panel2.Controls.Add(eskuinPanela);

        var lblPinIzenburua = new Label
        {
            Text = "Sartu zure PIN kodea",
            Font = Theme.FontTitle,
            ForeColor = Theme.TextMain,
            Dock = DockStyle.Top,
            Height = 40,
            TextAlign = ContentAlignment.MiddleCenter
        };
        eskuinPanela.Controls.Add(lblPinIzenburua);

        _lblAukeratutakoLangilea = new Label
        {
            Text = "Ez dago inor hautatuta",
            Font = Theme.FontSubtitle,
            ForeColor = Theme.AccentWine,
            Dock = DockStyle.Top,
            Height = 35,
            TextAlign = ContentAlignment.MiddleCenter
        };
        eskuinPanela.Controls.Add(_lblAukeratutakoLangilea);

        // PIN display koadroa
        var pnlPinDisplay = new Panel
        {
            Dock = DockStyle.Top,
            Height = 60,
            BackColor = Theme.Background,
            Margin = new Padding(0, 10, 0, 10)
        };
        _lblPinDisplay = new Label
        {
            Dock = DockStyle.Fill,
            Text = "----",
            Font = Theme.FontPinDisplay,
            ForeColor = Theme.TextMain,
            TextAlign = ContentAlignment.MiddleCenter
        };
        pnlPinDisplay.Controls.Add(_lblPinDisplay);
        eskuinPanela.Controls.Add(pnlPinDisplay);

        _lblStatus = new Label
        {
            Dock = DockStyle.Top,
            Height = 30,
            Text = "",
            Font = Theme.FontSmall,
            ForeColor = Theme.Danger,
            TextAlign = ContentAlignment.MiddleCenter
        };
        eskuinPanela.Controls.Add(_lblStatus);

        // PIN Teklatua (TableLayoutPanel 4x3)
        var keypad = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 280,
            RowCount = 4,
            ColumnCount = 3,
            Padding = new Padding(10)
        };
        for (int i = 0; i < 3; i++) keypad.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
        for (int i = 0; i < 4; i++) keypad.RowStyles.Add(new RowStyle(SizeType.Percent, 25f));

        string[] botoiak = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "C", "0", "<-" };
        foreach (var txt in botoiak)
        {
            var btn = new Button
            {
                Text = txt,
                Dock = DockStyle.Fill,
                Font = Theme.FontKeypad,
                Margin = new Padding(4),
                BackColor = txt switch
                {
                    "C" => Color.FromArgb(254, 226, 226),
                    "<-" => Color.FromArgb(241, 245, 249),
                    _ => Color.White
                },
                ForeColor = txt switch
                {
                    "C" => Theme.Danger,
                    _ => Theme.TextMain
                },
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderColor = Theme.Border;
            btn.FlatAppearance.BorderSize = 1;
            btn.Click += (s, e) => KeypadBotoiaKlikatua(txt);
            keypad.Controls.Add(btn);
        }
        eskuinPanela.Controls.Add(keypad);

        // HASI SAIOA Botoi Nagusia
        _btnLogin = new Button
        {
            Text = "HASI SAIOA",
            Dock = DockStyle.Bottom,
            Height = 55,
            Font = Theme.FontTitle,
            BackColor = Theme.AccentWine,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        _btnLogin.FlatAppearance.BorderSize = 0;
        _btnLogin.Click += async (s, e) => await LoginExekutatuAsync();
        eskuinPanela.Controls.Add(_btnLogin);

        // Karga asinkronoa
        Load += async (s, e) => await KargatuLangileakAsync();
    }

    private async Task KargatuLangileakAsync()
    {
        _panelLangileak.Controls.Clear();
        var lblKargatzen = new Label
        {
            Text = "Langileak kargatzen...",
            Font = Theme.FontBody,
            ForeColor = Theme.TextMuted,
            AutoSize = true
        };
        _panelLangileak.Controls.Add(lblKargatzen);

        _langileak = await _api.GetLangileakAsync();

        if (_langileak.Count == 0)
        {
            // Demo langileak fallback gisa API martxan ez badago
            _langileak = new List<LangileaDto>
            {
                new() { Id = 1, Erabiltzailea = "aitor", Izena = "Aitor Mendia", Rola = "Zerbitzaria", Aktibo = true },
                new() { Id = 2, Erabiltzailea = "maite", Izena = "Maite Alberdi", Rola = "Zerbitzaria", Aktibo = true },
                new() { Id = 3, Erabiltzailea = "mikel", Izena = "Mikel Agirre", Rola = "Zerbitzaria", Aktibo = true },
                new() { Id = 4, Erabiltzailea = "ane", Izena = "Ane Garmendia", Rola = "Zerbitzaria", Aktibo = true },
                new() { Id = 5, Erabiltzailea = "iker", Izena = "Iker Zabaleta", Rola = "Zerbitzaria", Aktibo = true }
            };
        }

        _panelLangileak.Controls.Clear();

        foreach (var l in _langileak)
        {
            var btnCard = SortuLangileTxartela(l);
            _panelLangileak.Controls.Add(btnCard);
        }

        // Lehenengoa hautatu lehenetsita
        if (_langileak.Count > 0)
        {
            HautatuLangilea(_langileak[0]);
        }
    }

    private Control SortuLangileTxartela(LangileaDto langilea)
    {
        var card = new Panel
        {
            Width = 230,
            Height = 85,
            Margin = new Padding(6),
            BackColor = Color.White,
            Cursor = Cursors.Hand,
            Tag = langilea
        };

        // Zirkulua inizialekin
        var initials = string.Join("", langilea.Izena.Split(' ').Select(n => n.Length > 0 ? n[0].ToString() : "")).ToUpper();
        if (initials.Length > 2) initials = initials.Substring(0, 2);

        var pnlAvatar = new Panel
        {
            Location = new Point(12, 16),
            Size = new Size(52, 52),
            BackColor = Theme.AccentWine
        };
        var lblAvatar = new Label
        {
            Dock = DockStyle.Fill,
            Text = initials,
            ForeColor = Color.White,
            Font = Theme.FontSubtitle,
            TextAlign = ContentAlignment.MiddleCenter
        };
        pnlAvatar.Controls.Add(lblAvatar);
        card.Controls.Add(pnlAvatar);

        var lblIzena = new Label
        {
            Text = langilea.Izena,
            Font = Theme.FontBodyBold,
            ForeColor = Theme.TextMain,
            Location = new Point(72, 18),
            Size = new Size(150, 24),
            AutoEllipsis = true
        };
        card.Controls.Add(lblIzena);

        var lblRola = new Label
        {
            Text = langilea.Rola,
            Font = Theme.FontSmall,
            ForeColor = Theme.TextMuted,
            Location = new Point(72, 42),
            Size = new Size(150, 20)
        };
        card.Controls.Add(lblRola);

        // Klik gertaera kontrol guztietan
        void OnClick(object? sender, EventArgs e) => HautatuLangilea(langilea);
        card.Click += OnClick;
        pnlAvatar.Click += OnClick;
        lblAvatar.Click += OnClick;
        lblIzena.Click += OnClick;
        lblRola.Click += OnClick;

        card.Paint += (s, e) =>
        {
            var isSelected = _aukeratutakoLangilea?.Id == langilea.Id;
            var borderColor = isSelected ? Theme.AccentWine : Theme.Border;
            var borderWidth = isSelected ? 3 : 1;
            using var pen = new Pen(borderColor, borderWidth);
            e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
        };

        return card;
    }

    private void HautatuLangilea(LangileaDto langilea)
    {
        _aukeratutakoLangilea = langilea;
        _lblAukeratutakoLangilea.Text = $"{langilea.Izena} ({langilea.Rola})";
        _pin = string.Empty;
        EguneratuPinDisplay();
        _lblStatus.Text = "";

        // Txartelen ertzak birbalidatu
        foreach (Control c in _panelLangileak.Controls)
        {
            c.Invalidate();
        }
    }

    private void KeypadBotoiaKlikatua(string txt)
    {
        _lblStatus.Text = "";

        if (txt == "C")
        {
            _pin = string.Empty;
        }
        else if (txt == "<-")
        {
            if (_pin.Length > 0)
                _pin = _pin.Substring(0, _pin.Length - 1);
        }
        else
        {
            if (_pin.Length < 6)
                _pin += txt;
        }

        EguneratuPinDisplay();
    }

    private void EguneratuPinDisplay()
    {
        if (_pin.Length == 0)
        {
            _lblPinDisplay.Text = "----";
            _lblPinDisplay.ForeColor = Theme.TextMuted;
        }
        else
        {
            _lblPinDisplay.Text = new string('*', _pin.Length);
            _lblPinDisplay.ForeColor = Theme.TextMain;
        }
    }

    private async Task LoginExekutatuAsync()
    {
        if (_aukeratutakoLangilea == null)
        {
            _lblStatus.Text = "Aukeratu zerbitzari bat lehenik.";
            return;
        }

        if (string.IsNullOrWhiteSpace(_pin))
        {
            _lblStatus.Text = "Sartu zure PIN kodea.";
            return;
        }

        _btnLogin.Enabled = false;
        _btnLogin.Text = "Egiaztatzen...";

        var (arrakasta, mezua, langilea) = await _api.LoginAsync(_aukeratutakoLangilea.Erabiltzailea, _pin);

        _btnLogin.Enabled = true;
        _btnLogin.Text = "HASI SAIOA";

        if (arrakasta && langilea != null)
        {
            _pin = string.Empty;
            EguneratuPinDisplay();

            // Ireki MahaiakForm
            var mahaiakForm = new MahaiakForm(langilea, _api);
            mahaiakForm.FormClosed += (s, e) =>
            {
                Show();
                _pin = string.Empty;
                EguneratuPinDisplay();
            };

            Hide();
            mahaiakForm.Show();
        }
        else
        {
            _lblStatus.Text = mezua;
            _pin = string.Empty;
            EguneratuPinDisplay();
        }
    }
}
