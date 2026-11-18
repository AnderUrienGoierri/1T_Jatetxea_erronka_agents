using System.Drawing.Drawing2D;
using Jatetxea.Desktop.Models;
using Jatetxea.Desktop.Services;
using Jatetxea.Desktop.UI;

namespace Jatetxea.Desktop.Forms;

public partial class LoginForm : Form
{
    private sealed class AvatarCardInfo
    {
        public LangileaDto Langilea { get; set; } = null!;
        public Panel Panel { get; set; } = null!;
        public Label Circle { get; set; } = null!;
        public Label Name { get; set; } = null!;
        public Label Role { get; set; } = null!;
    }

    private readonly ApiService _api;
    private List<LangileaDto> _langileak = new();
    private List<LangileaDto> _baimendutakoLangileak = new();
    private readonly List<AvatarCardInfo> _avatarCards = new();
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
            Location = new Point(15, 10),
            Size = new Size(490, 114),
            BackColor = Color.White,
            Visible = true
        };

        _lblOfflineTitle = new Label
        {
            Dock = DockStyle.Top,
            Height = 36,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 11F, FontStyle.Bold),
            ForeColor = Color.FromArgb(220, 38, 38),
            Text = "API zerbitzaria deskonektatuta dago!\n(http://localhost:5050)"
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

        pnlCard.Controls.Add(_pnlOfflineNotice);
        _pnlOfflineNotice.BringToFront();

        _pnlOfflineNotice.Resize += (s, e) =>
        {
            _btnOfflineRetry.Location = new Point((_pnlOfflineNotice.Width - _btnOfflineRetry.Width) / 2, 65);
        };

        pnlAvatars.MouseWheel += OnAvatarsMouseWheel;

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

        Shown += (s, e) =>
        {
            try
            {
                TopMost = true;
                TopMost = false;
                Activate();
                BringToFront();
                Focus();
            }
            catch { }
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

    private void OnAvatarsMouseWheel(object? sender, MouseEventArgs e)
    {
        if (pnlAvatars.HorizontalScroll.Visible)
        {
            int change = (e.Delta > 0 ? -1 : 1) * 94;
            int newVal = Math.Clamp(pnlAvatars.HorizontalScroll.Value + change,
                                    pnlAvatars.HorizontalScroll.Minimum,
                                    pnlAvatars.HorizontalScroll.Maximum);
            pnlAvatars.AutoScrollPosition = new Point(newVal, 0);
        }
    }

    private void LotuHorizontalaMouseWheel(Control ctrl)
    {
        ctrl.MouseWheel += OnAvatarsMouseWheel;
        foreach (Control child in ctrl.Controls)
        {
            LotuHorizontalaMouseWheel(child);
        }
    }

    private static bool DaBaimendua(LangileaDto l)
    {
        var rola = (l.Rola ?? "").Trim();
        if (rola.Equals("Zerbitzaria", StringComparison.OrdinalIgnoreCase)) return true;
        if (rola.Equals("Barrakoa", StringComparison.OrdinalIgnoreCase)) return true;
        if (rola.Equals("AdminIT", StringComparison.OrdinalIgnoreCase)) return true;
        if (rola.Equals("Admin", StringComparison.OrdinalIgnoreCase)) return true;
        if (rola.Equals("Kudeatzailea", StringComparison.OrdinalIgnoreCase)) return true;
        return false;
    }

    private static int LortuRolaLehentasuna(string rola)
    {
        var r = (rola ?? "").Trim();
        if (r.Equals("Zerbitzaria", StringComparison.OrdinalIgnoreCase)) return 1;
        if (r.Equals("Barrakoa", StringComparison.OrdinalIgnoreCase)) return 2;
        if (r.Equals("AdminIT", StringComparison.OrdinalIgnoreCase) || r.Equals("Admin", StringComparison.OrdinalIgnoreCase)) return 3;
        if (r.Equals("Kudeatzailea", StringComparison.OrdinalIgnoreCase)) return 4;
        return 99;
    }

    private void BistaratuLangileak(List<LangileaDto> langileak)
    {
        pnlAvatars.SuspendLayout();
        pnlAvatars.AutoScrollPosition = new Point(0, 0);
        pnlAvatars.Controls.Clear();
        _avatarCards.Clear();

        // Iragazi eta ordenatu: Zerbitzariak lehenik (Mikel Maître buru), gero Barrakoak, ondoren AdminIT eta Kudeatzaileak.
        _baimendutakoLangileak = langileak
            .Where(DaBaimendua)
            .OrderBy(l => LortuRolaLehentasuna(l.Rola))
            .ThenBy(l => l.Erabiltzailea == "mikel_zerb" ? 0 :
                         l.Erabiltzailea == "gorka_barra" ? 0 :
                         l.Erabiltzailea == "anderu" ? 0 : 1)
            .ThenBy(l => l.Izena)
            .ToList();

        if (_baimendutakoLangileak.Count == 0)
        {
            pnlAvatars.ResumeLayout(true);
            return;
        }

        const int cardWidth = 86;
        const int cardHeight = 90;
        const int gap = 8;
        int count = _baimendutakoLangileak.Count;
        int totalWidth = count * cardWidth + (count - 1) * gap;
        int startX = totalWidth < pnlAvatars.Width ? Math.Max(0, (pnlAvatars.Width - totalWidth) / 2) : 0;

        var lehenIzenak = _baimendutakoLangileak.Select(x => x.Izena.Trim().Split(' ')[0]).ToList();

        for (int i = 0; i < count; i++)
        {
            var l = _baimendutakoLangileak[i];
            var zatiak = l.Izena.Trim().Split(' ');
            string lehenIzena = zatiak[0];
            bool bikoiztua = lehenIzenak.Count(x => x.Equals(lehenIzena, StringComparison.OrdinalIgnoreCase)) > 1;

            string shortName = l.Erabiltzailea switch
            {
                "anderu" => "Ander U.",
                "ander_m_zuz" => "Ander M.",
                _ => (bikoiztua && zatiak.Length >= 2 && !string.IsNullOrEmpty(zatiak[1]))
                        ? $"{lehenIzena} {zatiak[1][0]}."
                        : lehenIzena
            };

            string initials = l.Erabiltzailea switch
            {
                "mikel_zerb" => "MA",
                "maite_zerb" => "MZ",
                "unai_zerb" => "UL",
                "gorka_barra" => "GM",
                "uxue_barra" => "UE",
                "anderu" => "AU",
                "julen" => "JA",
                "iker" => "IA",
                "aratz" => "AH",
                "ander_m_zuz" => "AM",
                "joseba_r_zuz" => "JR",
                "estebe_o_zuz" => "EO",
                _ => (zatiak.Length >= 2 && !string.IsNullOrEmpty(zatiak[1]))
                        ? $"{zatiak[0][0]}{zatiak[1][0]}".ToUpper()
                        : (zatiak[0].Length >= 2 ? zatiak[0].Substring(0, 2).ToUpper() : zatiak[0].ToUpper())
            };

            string roleText = l.Rola switch
            {
                "Zerbitzaria" => (l.Erabiltzailea == "mikel_zerb" ? "Maitre" : "Jangela"),
                "Barrakoa" => "Barra",
                "AdminIT" => "Admin IT",
                "Admin" => "Admin",
                "Kudeatzailea" => "Zuzendari",
                _ => l.Rola
            };

            var pnlCardItem = new Panel
            {
                Size = new Size(cardWidth, cardHeight),
                Location = new Point(startX + i * (cardWidth + gap), 2),
                Cursor = Cursors.Hand,
                BackColor = Color.Transparent
            };

            var lblCircle = new Label
            {
                Size = new Size(48, 48),
                Location = new Point((cardWidth - 48) / 2, 2),
                Text = initials,
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand,
                BackColor = Color.Transparent
            };

            var lblName = new Label
            {
                Size = new Size(cardWidth, 18),
                Location = new Point(0, 52),
                Text = shortName,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                ForeColor = Color.FromArgb(100, 116, 139),
                Cursor = Cursors.Hand,
                AutoEllipsis = true
            };

            var lblRole = new Label
            {
                Size = new Size(cardWidth, 16),
                Location = new Point(0, 71),
                Text = roleText,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 7.5F, FontStyle.Regular),
                ForeColor = Color.FromArgb(148, 163, 184),
                Cursor = Cursors.Hand,
                AutoEllipsis = true
            };

            var cardInfo = new AvatarCardInfo
            {
                Langilea = l,
                Panel = pnlCardItem,
                Circle = lblCircle,
                Name = lblName,
                Role = lblRole
            };

            lblCircle.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                bool isSelected = _aukeratutakoLangilea != null && _aukeratutakoLangilea.Id == cardInfo.Langilea.Id;

                using var bgBrush = new SolidBrush(isSelected ? Color.FromArgb(239, 246, 255) : Color.FromArgb(241, 245, 249));
                e.Graphics.FillEllipse(bgBrush, 2, 2, lblCircle.Width - 5, lblCircle.Height - 5);

                var ertzAkolorea = isSelected ? Color.FromArgb(37, 99, 235) : Color.FromArgb(203, 213, 225);
                float stroke = isSelected ? 3.0f : 1.2f;
                using var pen = new Pen(ertzAkolorea, stroke);
                e.Graphics.DrawEllipse(pen, 2, 2, lblCircle.Width - 5, lblCircle.Height - 5);

                var textColor = isSelected ? Color.FromArgb(37, 99, 235) : Color.FromArgb(71, 85, 105);
                using var fontCircle = new Font("Segoe UI", 11F, FontStyle.Bold);
                TextRenderer.DrawText(e.Graphics, lblCircle.Text, fontCircle,
                    new Rectangle(0, 0, lblCircle.Width, lblCircle.Height),
                    textColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            };

            void OnItemClick(object? s, EventArgs e)
            {
                HautatuLangilea(cardInfo.Langilea);
            }

            pnlCardItem.Click += OnItemClick;
            lblCircle.Click += OnItemClick;
            lblName.Click += OnItemClick;
            lblRole.Click += OnItemClick;

            LotuHorizontalaMouseWheel(pnlCardItem);

            pnlCardItem.Controls.Add(lblCircle);
            pnlCardItem.Controls.Add(lblName);
            pnlCardItem.Controls.Add(lblRole);

            pnlAvatars.Controls.Add(pnlCardItem);
            _avatarCards.Add(cardInfo);
        }

        pnlAvatars.ResumeLayout(true);

        // Hautatu lehenetsitako langilea (Mikel Maître edo lehena)
        var lehenetsia = _baimendutakoLangileak.FirstOrDefault(x => x.Erabiltzailea == "mikel_zerb")
                      ?? _baimendutakoLangileak.FirstOrDefault();
        if (lehenetsia != null)
        {
            HautatuLangilea(lehenetsia);
        }
    }

    private void HautatuLangilea(LangileaDto langilea)
    {
        _aukeratutakoLangilea = langilea;
        _pin = string.Empty;
        FreskatuPinDots();
        lblStatus.Text = "";

        foreach (var item in _avatarCards)
        {
            bool isSelected = item.Langilea.Id == langilea.Id;
            item.Name.Font = new Font("Segoe UI", 9.5F, isSelected ? FontStyle.Bold : FontStyle.Regular);
            item.Name.ForeColor = isSelected ? Color.FromArgb(30, 41, 59) : Color.FromArgb(100, 116, 139);
            item.Role.Font = new Font("Segoe UI", 7.5F, isSelected ? FontStyle.Bold : FontStyle.Regular);
            item.Role.ForeColor = isSelected ? Color.FromArgb(37, 99, 235) : Color.FromArgb(148, 163, 184);
            item.Circle.Invalidate();

            if (isSelected)
            {
                pnlAvatars.ScrollControlIntoView(item.Panel);
            }
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
                _baimendutakoLangileak.Clear();
                _aukeratutakoLangilea = null;

                pnlAvatars.Visible = false;

                _lblOfflineTitle.Text = "API zerbitzaria deskonektatuta dago!\n(http://localhost:5050)";
                _lblOfflineTitle.ForeColor = Color.FromArgb(220, 38, 38);
                _lblOfflineSubtitle.Text = "Automatikoki berriro saiatuko da... edo sakatu 'BERRIZ SAIATU'";
                _pnlOfflineNotice.Visible = true;

                lblStatus.Text = $"{healthMsg} ({DateTime.Now:HH:mm:ss}).";
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
                _baimendutakoLangileak.Clear();
                _aukeratutakoLangilea = null;

                pnlAvatars.Visible = false;

                _lblOfflineTitle.Text = "API-ra konektatuta | Datu-basea DESKONEKTATUTA!\n(3308 portua)";
                _lblOfflineTitle.ForeColor = Color.FromArgb(234, 88, 12);
                _lblOfflineSubtitle.Text = "API zerbitzariarekin konexioa ondo lortu da, baina MySQL datu-basearekin ezin izan da konexioa lortu.";
                _pnlOfflineNotice.Visible = true;

                lblStatus.Text = $"APIra konektatuta dago, baina MySQL datu-basearekin ezin izan da konexioa lortu ({DateTime.Now:HH:mm:ss}).\nEgiaztatu Docker edukiontzia (3308 portua).";
                lblStatus.ForeColor = Color.FromArgb(234, 88, 12);

                lblPinTitle.Text = "DATU-BASEAREKIN EZIN IZAN DA KONEXIOA LORTU";
                lblPinTitle.ForeColor = Color.FromArgb(234, 88, 12);

                btnLogin.Enabled = true;
                btnLogin.Text = "BERRIZ SAIATU (Konektatu)";
                btnLogin.BackColor = Color.FromArgb(37, 99, 235);

                _lblApiStatusText.Text = "API Konektatuta | DB Deskonektatuta";
                _lblApiStatusText.ForeColor = Color.FromArgb(234, 88, 12);
                _lblApiStatusDot.Invalidate();
                return;
            }

            // APIa eta DBa martxan: Datu errealak kargatu API bidez
            var langileak = await _api.GetLangileakAsync();

            if (langileak.Count == 0)
            {
                _apiKonektatuta = false;
                _langileak = new List<LangileaDto>();
                _baimendutakoLangileak.Clear();
                _aukeratutakoLangilea = null;

                pnlAvatars.Visible = false;

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
            pnlAvatars.Visible = true;
            lblStatus.Text = "";
            lblPinTitle.Text = "SARTU PIN ZENBAKIA";
            lblPinTitle.ForeColor = Color.FromArgb(30, 41, 59);

            btnLogin.Enabled = true;
            btnLogin.Text = "HASI SAIOA";
            btnLogin.BackColor = Color.FromArgb(136, 19, 55);

            _lblApiStatusText.Text = "API: Konektatuta";
            _lblApiStatusText.ForeColor = Color.FromArgb(22, 163, 74);
            _lblApiStatusDot.Invalidate();

            BistaratuLangileak(_langileak);
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

    private void KeypadBotoiaKlikatua(string txt)
    {
        if (!_apiKonektatuta || _baimendutakoLangileak.Count == 0 || _aukeratutakoLangilea == null)
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
