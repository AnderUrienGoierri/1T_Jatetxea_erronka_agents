using System.Drawing.Drawing2D;
using Jatetxea.Desktop.Models;
using Jatetxea.Desktop.Services;
using Jatetxea.Desktop.UI;

namespace Jatetxea.Desktop.Forms;

public partial class LoginForm : Form
{
    private readonly ApiService _api;
    private List<LangileaDto> _langileak = new();
    private LangileaDto? _aukeratutakoLangilea;
    private string _pin = string.Empty;
    private bool _apiKonektatuta = false;
    private bool _kargatzenAriDa = false;

    // UI osagai dinamikoak konexio egoerarako
    private readonly Panel _pnlOfflineNotice;
    private readonly Label _lblOfflineTitle;
    private readonly Label _lblOfflineSubtitle;
    private readonly Button _btnOfflineRetry;

    // Goiko barrako konexio egoera eta freskatze kontrolak
    private readonly Panel _pnlTopStatus;
    private readonly Label _lblApiStatusDot;
    private readonly Label _lblApiStatusText;
    private readonly Button _btnTopFreskatu;

    // Automatikoki konexioa egiaztatzeko tenporizadorea
    private readonly System.Windows.Forms.Timer _autoKonexioTimer;

    public LoginForm()
    {
        InitializeComponent();
        _api = new ApiService();

        // 1. Lineaz kanpoko panela (Avatarren gunean bistaratzeko)
        _pnlOfflineNotice = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Visible = true
        };

        _lblOfflineTitle = new Label
        {
            Dock = DockStyle.Top,
            Height = 32,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 11F, FontStyle.Bold),
            ForeColor = Color.FromArgb(220, 38, 38),
            Text = "API zerbitzaria deskonektatuta dago!\n(http://localhost:5000)"
        };

        _lblOfflineSubtitle = new Label
        {
            Dock = DockStyle.Top,
            Height = 24,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 9F, FontStyle.Regular),
            ForeColor = Color.FromArgb(100, 116, 139),
            Text = "Automatikoki berriro konektatzen saiatuko da..."
        };

        _btnOfflineRetry = new Button
        {
            Size = new Size(180, 34),
            Text = "Berriz konektatu",
            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(254, 242, 242),
            ForeColor = Color.FromArgb(220, 38, 38),
            Cursor = Cursors.Hand
        };
        _btnOfflineRetry.FlatAppearance.BorderColor = Color.FromArgb(252, 165, 165);
        _btnOfflineRetry.Click += async (s, e) => await SaiatuKonektatzenAsync(eskuz: true);

        _pnlOfflineNotice.Controls.Add(_btnOfflineRetry);
        _pnlOfflineNotice.Controls.Add(_lblOfflineSubtitle);
        _pnlOfflineNotice.Controls.Add(_lblOfflineTitle);

        pnlAvatars.Controls.Add(_pnlOfflineNotice);

        _pnlOfflineNotice.Resize += (s, e) =>
        {
            _btnOfflineRetry.Location = new Point((_pnlOfflineNotice.Width - _btnOfflineRetry.Width) / 2, 60);
        };

        // 2. Goiko barrako konexio egoera-panela
        _pnlTopStatus = new Panel
        {
            Size = new Size(320, 44),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            BackColor = Color.Transparent
        };

        _lblApiStatusDot = new Label
        {
            Size = new Size(18, 18),
            Location = new Point(8, 13),
            BackColor = Color.Transparent
        };
        _lblApiStatusDot.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Color dotColor = _apiKonektatuta 
                ? Color.FromArgb(22, 163, 74) 
                : (_kargatzenAriDa ? Color.FromArgb(217, 119, 6) : Color.FromArgb(220, 38, 38));
            using var brush = new SolidBrush(dotColor);
            e.Graphics.FillEllipse(brush, 2, 2, 13, 13);
        };

        _lblApiStatusText = new Label
        {
            Size = new Size(160, 42),
            Location = new Point(30, 1),
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            ForeColor = Color.FromArgb(100, 116, 139),
            Text = "API: Egiaztatzen..."
        };

        _btnTopFreskatu = new Button
        {
            Size = new Size(115, 36),
            Location = new Point(195, 4),
            Text = "Freskatu",
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(241, 245, 249),
            ForeColor = Color.FromArgb(51, 65, 85),
            Cursor = Cursors.Hand
        };
        _btnTopFreskatu.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        _btnTopFreskatu.Click += async (s, e) => await SaiatuKonektatzenAsync(eskuz: true);

        _pnlTopStatus.Controls.Add(_lblApiStatusDot);
        _pnlTopStatus.Controls.Add(_lblApiStatusText);
        _pnlTopStatus.Controls.Add(_btnTopFreskatu);
        pnlTopBar.Controls.Add(_pnlTopStatus);

        // 3. Automatikoki berriz konektatzeko tenporizadorea (3 segundoro)
        _autoKonexioTimer = new System.Windows.Forms.Timer
        {
            Interval = 3000
        };
        _autoKonexioTimer.Tick += async (s, e) =>
        {
            if (_apiKonektatuta || _kargatzenAriDa) return;

            _autoKonexioTimer.Stop();
            try
            {
                await SaiatuKonektatzenAsync(eskuz: false);
            }
            finally
            {
                if (!_apiKonektatuta && !IsDisposed)
                {
                    _autoKonexioTimer.Start();
                }
            }
        };

        // Goiko barra zentratu eta egokitu
        pnlTopBar.Resize += (s, e) =>
        {
            lblZerbitzariak.Location = new Point((pnlTopBar.Width - lblZerbitzariak.Width) / 2, (pnlTopBar.Height - lblZerbitzariak.Height) / 2);
            _pnlTopStatus.Location = new Point(pnlTopBar.Width - _pnlTopStatus.Width - 24, (pnlTopBar.Height - _pnlTopStatus.Height) / 2);
        };

        // Zentratu txartel nagusia pantailan automatikoki
        Resize += (s, e) =>
        {
            ZentratuTxartela();
        };

        // Goiko barraren beheko marra margotu
        pnlTopBar.Paint += (s, e) =>
        {
            using var pen = new Pen(Color.FromArgb(203, 213, 225), 1.5f);
            e.Graphics.DrawLine(pen, 0, pnlTopBar.Height - 1, pnlTopBar.Width, pnlTopBar.Height - 1);
        };

        // Txartelaren kanpoko ertz leuna margotu
        pnlCard.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var pen = new Pen(Color.FromArgb(203, 213, 225), 1.5f);
            e.Graphics.DrawRectangle(pen, 0, 0, pnlCard.Width - 1, pnlCard.Height - 1);
        };

        // PIN borobilak margotu
        SetupPinDotsPaint();

        // Teklatuko botoiak lotu
        LotuTeklatuBotoiak();

        // Avatarren klikak eta margoak prestatu
        PrestatuAvatarrak();

        // Botoi nagusia: konektatuta badago Login egin, deskonektatuta badago berriz konektatu
        btnLogin.Click += async (s, e) =>
        {
            if (!_apiKonektatuta)
            {
                await SaiatuKonektatzenAsync(eskuz: true);
            }
            else
            {
                await LoginExekutatuAsync();
            }
        };

        lblStatus.Cursor = Cursors.Hand;
        lblStatus.Click += async (s, e) =>
        {
            if (!_apiKonektatuta)
            {
                await SaiatuKonektatzenAsync(eskuz: true);
            }
        };

        // Teklatu fisikoaren bidezko laguntza (F5 freskatu, Enter, Digituak, Escape)
        KeyPreview = true;
        KeyDown += async (s, e) =>
        {
            if (e.KeyCode == Keys.F5)
            {
                e.Handled = true;
                await SaiatuKonektatzenAsync(eskuz: true);
            }
            else if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                if (!_apiKonektatuta)
                {
                    await SaiatuKonektatzenAsync(eskuz: true);
                }
                else
                {
                    await LoginExekutatuAsync();
                }
            }
            else if (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)
            {
                KeypadBotoiaKlikatua((e.KeyCode - Keys.D0).ToString());
            }
            else if (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9)
            {
                KeypadBotoiaKlikatua((e.KeyCode - Keys.NumPad0).ToString());
            }
            else if (e.KeyCode == Keys.Back)
            {
                KeypadBotoiaKlikatua("←");
            }
            else if (e.KeyCode == Keys.Escape)
            {
                KeypadBotoiaKlikatua("Clear");
            }
        };

        Load += async (s, e) =>
        {
            lblZerbitzariak.Location = new Point((pnlTopBar.Width - lblZerbitzariak.Width) / 2, (pnlTopBar.Height - lblZerbitzariak.Height) / 2);
            _pnlTopStatus.Location = new Point(pnlTopBar.Width - _pnlTopStatus.Width - 24, (pnlTopBar.Height - _pnlTopStatus.Height) / 2);
            _btnOfflineRetry.Location = new Point((_pnlOfflineNotice.Width - _btnOfflineRetry.Width) / 2, 60);
            ZentratuTxartela();

            await SaiatuKonektatzenAsync(eskuz: false);

            if (!_apiKonektatuta)
            {
                _autoKonexioTimer.Start();
            }
        };

        FormClosing += (s, e) =>
        {
            _autoKonexioTimer.Stop();
            _autoKonexioTimer.Dispose();
        };
    }

    private void ZentratuTxartela()
    {
        int x = Math.Max(20, (ClientSize.Width - pnlCard.Width) / 2);
        int y = Math.Max(pnlTopBar.Height + 10, (ClientSize.Height - pnlTopBar.Height - pnlCard.Height) / 2 + pnlTopBar.Height);
        pnlCard.Location = new Point(x, y);
    }

    private void SetupPinDotsPaint()
    {
        Label[] dots = { lblDot1, lblDot2, lblDot3, lblDot4 };
        for (int i = 0; i < dots.Length; i++)
        {
            int index = i;
            var dot = dots[index];
            dot.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                bool betea = index < _pin.Length;

                if (betea)
                {
                    using var brush = new SolidBrush(Color.FromArgb(51, 65, 85));
                    e.Graphics.FillEllipse(brush, 2, 2, dot.Width - 5, dot.Height - 5);
                }
                else
                {
                    using var pen = new Pen(Color.FromArgb(148, 163, 184), 2f);
                    e.Graphics.DrawEllipse(pen, 2, 2, dot.Width - 5, dot.Height - 5);
                }
            };
        }
    }

    private void LotuTeklatuBotoiak()
    {
        btnKey1.Click += (s, e) => KeypadBotoiaKlikatua("1");
        btnKey2.Click += (s, e) => KeypadBotoiaKlikatua("2");
        btnKey3.Click += (s, e) => KeypadBotoiaKlikatua("3");
        btnKey4.Click += (s, e) => KeypadBotoiaKlikatua("4");
        btnKey5.Click += (s, e) => KeypadBotoiaKlikatua("5");
        btnKey6.Click += (s, e) => KeypadBotoiaKlikatua("6");
        btnKey7.Click += (s, e) => KeypadBotoiaKlikatua("7");
        btnKey8.Click += (s, e) => KeypadBotoiaKlikatua("8");
        btnKey9.Click += (s, e) => KeypadBotoiaKlikatua("9");
        btnKey0.Click += (s, e) => KeypadBotoiaKlikatua("0");
        btnKeyClear.Click += (s, e) => KeypadBotoiaKlikatua("Clear");
        btnKeyBack.Click += (s, e) => KeypadBotoiaKlikatua("←");
    }

    private void PrestatuAvatarrak()
    {
        (Panel Panel, Label Circle, Label Name)[] items = {
            (pnlAvatar1, lblCircle1, lblName1),
            (pnlAvatar2, lblCircle2, lblName2),
            (pnlAvatar3, lblCircle3, lblName3),
            (pnlAvatar4, lblCircle4, lblName4),
            (pnlAvatar5, lblCircle5, lblName5)
        };

        for (int i = 0; i < items.Length; i++)
        {
            int index = i;
            var item = items[index];

            item.Circle.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                bool isSelected = false;
                if (index < _langileak.Count && _aukeratutakoLangilea != null)
                {
                    isSelected = _aukeratutakoLangilea.Id == _langileak[index].Id;
                }

                using var bgBrush = new SolidBrush(isSelected ? Color.FromArgb(239, 246, 255) : Color.FromArgb(241, 245, 249));
                e.Graphics.FillEllipse(bgBrush, 2, 2, item.Circle.Width - 5, item.Circle.Height - 5);

                var ertzAkolorea = isSelected ? Color.FromArgb(37, 99, 235) : Color.FromArgb(203, 213, 225);
                float stroke = isSelected ? 2.8f : 1.2f;
                using var pen = new Pen(ertzAkolorea, stroke);
                e.Graphics.DrawEllipse(pen, 2, 2, item.Circle.Width - 5, item.Circle.Height - 5);

                var textColor = isSelected ? Color.FromArgb(37, 99, 235) : Color.FromArgb(71, 85, 105);
                using var fontCircle = new Font("Segoe UI", 12F, FontStyle.Bold);
                TextRenderer.DrawText(e.Graphics, item.Circle.Text, fontCircle,
                    new Rectangle(0, 0, item.Circle.Width, item.Circle.Height),
                    textColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            };

            void OnClick(object? s, EventArgs e)
            {
                if (index < _langileak.Count)
                {
                    HautatuLangilea(_langileak[index]);
                }
            }

            item.Panel.Click += OnClick;
            item.Circle.Click += OnClick;
            item.Name.Click += OnClick;
        }
    }

    /// <summary>
    /// API eta MySQL zerbitzariekin konexioa egiaztatzen du eta langileak kargatzen ditu.
    /// Konexioa berreskuratzean automatikoki berritzen du interfazea.
    /// </summary>
    private async Task SaiatuKonektatzenAsync(bool eskuz = false)
    {
        if (_kargatzenAriDa) return;
        _kargatzenAriDa = true;

        (Panel Panel, Label Circle, Label Name)[] items = {
            (pnlAvatar1, lblCircle1, lblName1),
            (pnlAvatar2, lblCircle2, lblName2),
            (pnlAvatar3, lblCircle3, lblName3),
            (pnlAvatar4, lblCircle4, lblName4),
            (pnlAvatar5, lblCircle5, lblName5)
        };

        if (eskuz)
        {
            btnLogin.Enabled = false;
            btnLogin.Text = "KONEXIOA EGIAZTATZEN...";
            btnLogin.BackColor = Color.FromArgb(148, 163, 184);
            _btnOfflineRetry.Enabled = false;
            _btnOfflineRetry.Text = "Egiaztatzen...";
            _btnTopFreskatu.Enabled = false;
            lblStatus.Text = "API zerbitzariarekin konektatzen saiatzen...";
            lblStatus.ForeColor = Color.FromArgb(100, 116, 139);
        }

        _lblApiStatusText.Text = "API: Egiaztatzen...";
        _lblApiStatusText.ForeColor = Color.FromArgb(217, 119, 6);
        _lblApiStatusDot.Invalidate();

        try
        {
            var (apiOk, dbOk, healthMsg) = await _api.CheckHealthAsync();

            if (!apiOk)
            {
                _apiKonektatuta = false;
                _langileak = new List<LangileaDto>();
                _aukeratutakoLangilea = null;

                foreach (var item in items)
                {
                    item.Panel.Visible = false;
                }

                _lblOfflineTitle.Text = "API zerbitzaria deskonektatuta dago!\n(http://localhost:5000)";
                _lblOfflineTitle.ForeColor = Color.FromArgb(220, 38, 38);
                _lblOfflineSubtitle.Text = "Automatikoki berriro saiatuko da... edo sakatu 'BERRIZ SAIATU'";
                _pnlOfflineNotice.Visible = true;

                lblStatus.Text = $"Ezin izan da APIarekin konektatu ({DateTime.Now:HH:mm:ss}).\nEgiaztatu zerbitzaria martxan dagoela (http://localhost:5000).";
                lblStatus.ForeColor = Color.FromArgb(220, 38, 38);

                lblPinTitle.Text = "API KONEXIOA BEHARREZKOA DA";
                lblPinTitle.ForeColor = Color.FromArgb(220, 38, 38);

                btnLogin.Enabled = true;
                btnLogin.Text = "BERRIZ SAIATU (Konektatu)";
                btnLogin.BackColor = Color.FromArgb(37, 99, 235);

                _lblApiStatusText.Text = "API: Deskonektatuta";
                _lblApiStatusText.ForeColor = Color.FromArgb(220, 38, 38);
                _lblApiStatusDot.Invalidate();
                return;
            }

            if (!dbOk)
            {
                _apiKonektatuta = false;
                _langileak = new List<LangileaDto>();
                _aukeratutakoLangilea = null;

                foreach (var item in items)
                {
                    item.Panel.Visible = false;
                }

                _lblOfflineTitle.Text = "MySQL datu-basea deskonektatuta dago!\n(3308 portua)";
                _lblOfflineTitle.ForeColor = Color.FromArgb(220, 38, 38);
                _lblOfflineSubtitle.Text = "APIa martxan dago, baina MySQL edukiontzia falta da.";
                _pnlOfflineNotice.Visible = true;

                lblStatus.Text = $"MySQL datu-basea deskonektatuta dago ({DateTime.Now:HH:mm:ss}).\nEgiaztatu Docker edukiontzia martxan dagoela (3308 portua).";
                lblStatus.ForeColor = Color.FromArgb(220, 38, 38);

                lblPinTitle.Text = "DATU-BASEA FALTA DA";
                lblPinTitle.ForeColor = Color.FromArgb(220, 38, 38);

                btnLogin.Enabled = true;
                btnLogin.Text = "BERRIZ SAIATU (Konektatu)";
                btnLogin.BackColor = Color.FromArgb(37, 99, 235);

                _lblApiStatusText.Text = "DB: Deskonektatuta";
                _lblApiStatusText.ForeColor = Color.FromArgb(220, 38, 38);
                _lblApiStatusDot.Invalidate();
                return;
            }

            // APIa eta DBa martxan: Datu errealak kargatu API bidez
            var langileak = await _api.GetLangileakAsync();

            if (langileak.Count == 0)
            {
                _apiKonektatuta = false;
                _langileak = new List<LangileaDto>();
                _aukeratutakoLangilea = null;

                foreach (var item in items)
                {
                    item.Panel.Visible = false;
                }

                _lblOfflineTitle.Text = "Ez da zerbitzaririk aurkitu datu-basean";
                _lblOfflineTitle.ForeColor = Color.FromArgb(194, 65, 12);
                _lblOfflineSubtitle.Text = "Egiaztatu langileen taula beteta dagoela.";
                _pnlOfflineNotice.Visible = true;

                lblStatus.Text = "Ez da zerbitzaririk aurkitu datu-basean.";
                lblStatus.ForeColor = Color.FromArgb(194, 65, 12);

                lblPinTitle.Text = "EZ DAGO ZERBITZARIRIK";
                lblPinTitle.ForeColor = Color.FromArgb(194, 65, 12);

                btnLogin.Enabled = true;
                btnLogin.Text = "BERRIZ SAIATU (Freskatu)";
                btnLogin.BackColor = Color.FromArgb(37, 99, 235);

                _lblApiStatusText.Text = "Daturik gabe";
                _lblApiStatusText.ForeColor = Color.FromArgb(194, 65, 12);
                _lblApiStatusDot.Invalidate();
                return;
            }

            // Konexio arrakastatsua: datuak bistaratu
            _apiKonektatuta = true;
            _langileak = langileak;
            _autoKonexioTimer.Stop();

            _pnlOfflineNotice.Visible = false;
            lblStatus.Text = "";
            lblPinTitle.Text = "SARTU PIN ZENBAKIA";
            lblPinTitle.ForeColor = Color.FromArgb(30, 41, 59);

            btnLogin.Enabled = true;
            btnLogin.Text = "HASI SAIOA";
            btnLogin.BackColor = Color.FromArgb(136, 19, 55);

            _lblApiStatusText.Text = "API: Konektatuta";
            _lblApiStatusText.ForeColor = Color.FromArgb(22, 163, 74);
            _lblApiStatusDot.Invalidate();

            int count = Math.Min(_langileak.Count, 5);
            int avatarWidth = count <= 3 ? 120 : (count == 4 ? 104 : 90);
            int gap = count > 1 ? Math.Max(8, (pnlAvatars.Width - (count * avatarWidth)) / (count - 1)) : 0;
            int totalWidth = count * avatarWidth + (count - 1) * gap;
            int startX = Math.Max(0, (pnlAvatars.Width - totalWidth) / 2);

            var lehenIzenak = _langileak.Select(x => x.Izena.Trim().Split(' ')[0]).ToList();

            for (int i = 0; i < items.Length; i++)
            {
                if (i < count)
                {
                    var l = _langileak[i];
                    var zatiak = l.Izena.Trim().Split(' ');
                    string lehenIzena = zatiak[0];
                    bool bikoiztua = lehenIzenak.Count(x => x.Equals(lehenIzena, StringComparison.OrdinalIgnoreCase)) > 1;
                    string shortName = (bikoiztua && zatiak.Length >= 2) ? $"{lehenIzena} {zatiak[1][0]}." : lehenIzena;
                    string initials = string.Join("", zatiak.Select(z => z.Length > 0 ? z[0].ToString() : "")).ToUpper();
                    if (initials.Length > 2) initials = initials.Substring(0, 2);

                    items[i].Name.Text = shortName;
                    items[i].Circle.Text = initials;

                    items[i].Panel.Size = new Size(avatarWidth, 100);
                    items[i].Circle.Location = new Point((avatarWidth - 52) / 2, 2);
                    items[i].Circle.Size = new Size(52, 52);
                    items[i].Name.Location = new Point(0, 58);
                    items[i].Name.Size = new Size(avatarWidth, 38);
                    items[i].Name.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

                    items[i].Panel.Location = new Point(startX + i * (avatarWidth + gap), 2);
                    items[i].Panel.Visible = true;
                }
                else
                {
                    items[i].Panel.Visible = false;
                }
            }

            HautatuLangilea(_langileak[0]);
        }
        catch (Exception ex)
        {
            _apiKonektatuta = false;
            lblStatus.Text = $"Errorea konexioan: {ex.Message}";
            lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
            _lblApiStatusText.Text = "Errorea";
            _lblApiStatusText.ForeColor = Color.FromArgb(220, 38, 38);
            _lblApiStatusDot.Invalidate();
        }
        finally
        {
            _kargatzenAriDa = false;
            _btnOfflineRetry.Enabled = true;
            _btnOfflineRetry.Text = "Berriz konektatu";
            _btnTopFreskatu.Enabled = true;
        }
    }

    private async Task KargatuLangileakAsync()
    {
        await SaiatuKonektatzenAsync(eskuz: true);
    }

    private void HautatuLangilea(LangileaDto langilea)
    {
        _aukeratutakoLangilea = langilea;
        _pin = string.Empty;
        FreskatuPinDots();
        lblStatus.Text = "";

        (Label Circle, Label Name)[] items = {
            (lblCircle1, lblName1),
            (lblCircle2, lblName2),
            (lblCircle3, lblName3),
            (lblCircle4, lblName4),
            (lblCircle5, lblName5)
        };

        for (int i = 0; i < items.Length; i++)
        {
            bool isSelected = i < _langileak.Count && _langileak[i].Id == langilea.Id;
            items[i].Name.Font = new Font("Segoe UI", 10F, isSelected ? FontStyle.Bold : FontStyle.Regular);
            items[i].Name.ForeColor = isSelected ? Color.FromArgb(30, 41, 59) : Color.FromArgb(100, 116, 139);
            items[i].Circle.Invalidate();
        }
    }

    private void KeypadBotoiaKlikatua(string txt)
    {
        if (!_apiKonektatuta || _langileak.Count == 0 || _aukeratutakoLangilea == null)
        {
            lblStatus.Text = "APIa deskonektatuta dago. Sakatu 'BERRIZ SAIATU' edo itxaron automatikoki konektatzeko.";
            lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
            return;
        }

        lblStatus.Text = "";

        if (txt.Contains("Clear") || txt == "X" || txt == "C")
        {
            _pin = string.Empty;
        }
        else if (txt == "←" || txt == "<-" || txt == "Back")
        {
            if (_pin.Length > 0)
                _pin = _pin.Substring(0, _pin.Length - 1);
        }
        else
        {
            if (_pin.Length < 4)
                _pin += txt;
        }

        FreskatuPinDots();
    }

    private void FreskatuPinDots()
    {
        lblDot1.Invalidate();
        lblDot2.Invalidate();
        lblDot3.Invalidate();
        lblDot4.Invalidate();
    }

    private async Task LoginExekutatuAsync()
    {
        if (!_apiKonektatuta)
        {
            await SaiatuKonektatzenAsync(eskuz: true);
            return;
        }

        if (_aukeratutakoLangilea == null)
        {
            lblStatus.Text = "Hautatu zerbitzari bat lehenik.";
            lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
            return;
        }

        if (string.IsNullOrWhiteSpace(_pin))
        {
            lblStatus.Text = "Sartu 4 digituko PIN zenbakia.";
            lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
            return;
        }

        btnLogin.Enabled = false;
        btnLogin.Text = "EGIAZTATZEN...";

        var (arrakasta, mezua, langilea) = await _api.LoginAsync(_aukeratutakoLangilea.Erabiltzailea, _pin);

        btnLogin.Enabled = true;
        btnLogin.Text = "HASI SAIOA";

        if (arrakasta && langilea != null)
        {
            var sesioLangilea = _aukeratutakoLangilea ?? langilea;
            _pin = string.Empty;
            FreskatuPinDots();

            _autoKonexioTimer.Stop();

            var mahaiakForm = new MahaiakForm(sesioLangilea, _api);
            mahaiakForm.FormClosed += async (s, e) =>
            {
                Show();
                _pin = string.Empty;
                FreskatuPinDots();
                await SaiatuKonektatzenAsync(eskuz: false);
                if (!_apiKonektatuta)
                {
                    _autoKonexioTimer.Start();
                }
            };

            Hide();
            mahaiakForm.Show();
        }
        else
        {
            lblStatus.Text = mezua;
            lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
            _pin = string.Empty;
            FreskatuPinDots();
        }
    }

    private void btnKeyClear_Click(object? sender, EventArgs e)
    {
        KeypadBotoiaKlikatua("X Clear");
    }

    private void LoginForm_Load(object? sender, EventArgs e)
    {
    }
}
