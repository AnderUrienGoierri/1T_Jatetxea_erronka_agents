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

        // Goiko barra zentratu eta egokitu (responsive breakpoint < 960px)
        pnlTopBar.Resize += (s, e) => EgokituGoikoBarra();

        // Zentratu eta eskalatu txartel nagusia pantailan automatikoki
        Resize += (s, e) =>
        {
            EgokituGoikoBarra();
            ZentratuTxartela();
        };

        // Goiko barraren beheko marra margotu
        pnlTopBar.Paint += (s, e) =>
        {
            using var pen = new Pen(Theme.WarmTaupe, 1.2f);
            e.Graphics.DrawLine(pen, 0, pnlTopBar.Height - 1, pnlTopBar.Width, pnlTopBar.Height - 1);
        };

        // T1 monograma arkitektonikoa margotu
        lblLogo.Text = string.Empty;
        lblLogo.BackColor = Theme.MarbleWhite;
        lblLogo.Paint += (s, e) => Theme.DrawT1Logo(e.Graphics, new Rectangle(0, 0, lblLogo.Width, lblLogo.Height));

        // Txartelaren kanpoko ertz biribildu urreztatua margotu (Warm Hospitality)
        pnlCard.BackColor = Theme.WarmLinen;
        pnlCard.Paint += (s, e) =>
        {
            Theme.DrawRoundedCard(e.Graphics, new Rectangle(0, 0, pnlCard.Width, pnlCard.Height), 18, Theme.MarbleWhite, Theme.GoldBorder, 1.5f);
        };

        // Warm Hospitality v2.0 paleta + parpadeorik gabeko DoubleBuffering aplikatu
        Theme.ApplyWarmHospitalityTheme(this);
        BackColor = Theme.WarmLinen;
        pnlTopBar.BackColor = Theme.MarbleWhite;
        pnlAvatars.BackColor = Theme.MarbleWhite;
        _pnlOfflineNotice.BackColor = Theme.MarbleWhite;
        pnlDivider.BackColor = Theme.WarmTaupe;
        lblBrand.Font = Theme.DisplayFont(16f);
        lblBrand.ForeColor = Theme.ChiantiPrimary;
        lblBrand.Text = "T1 JATETXEA";
        lblZerbitzariak.Font = Theme.BodyFont(10.5f, bold: true);
        lblZerbitzariak.ForeColor = Theme.WarmSlate;
        lblPinTitle.Font = Theme.BodyFont(11f, bold: true);
        lblPinTitle.ForeColor = Theme.EspressoInk;
        lblPinTitle.BackColor = Theme.MarbleWhite;
        pnlPinDots.BackColor = Theme.MarbleWhite;
        lblDot1.BackColor = Theme.MarbleWhite;
        lblDot2.BackColor = Theme.MarbleWhite;
        lblDot3.BackColor = Theme.MarbleWhite;
        lblDot4.BackColor = Theme.MarbleWhite;
        pnlKeypad.BackColor = Theme.MarbleWhite;
        lblStatus.BackColor = Theme.MarbleWhite;
        Theme.StyleButton(_btnTopFreskatu, Theme.StoneMist, Theme.EspressoInk, Theme.WarmTaupe, radius: 10, fontSize: 9f);
        Theme.StyleButton(_btnOfflineRetry, Theme.DangerBg, Theme.Danger, Theme.DangerBorder, radius: 10, fontSize: 9.5f);
        Theme.StyleButton(btnLogin, Theme.ChiantiPrimary, Theme.MarbleWhite, null, radius: 12, fontSize: 11.5f);

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
            EgokituGoikoBarra();
            _btnOfflineRetry.Location = new Point((_pnlOfflineNotice.Width - _btnOfflineRetry.Width) / 2, Theme.S(this, 60));
            ZentratuTxartela();

            await SaiatuKonektatzenAsync(eskuz: false);

            if (!_apiKonektatuta)
            {
                _autoKonexioTimer.Start();
            }
        };

        Theme.AttachMainFormDpiAdaptation(this, () =>
        {
            EgokituGoikoBarra();
            ZentratuTxartela();
            if (_baimendutakoLangileak.Count > 0)
            {
                var unekoak = new List<LangileaDto>(_baimendutakoLangileak);
                _avatarCards.Clear();
                BistaratuLangileak(unekoak);
            }
        });

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

    private void EgokituGoikoBarra()
    {
        int S(int px) => Theme.S(this, px);
        bool estua = pnlTopBar.Width < S(960);
        pnlTopBar.Height = ClientSize.Height < S(580) ? S(64) : S(82);
        int logoSize = pnlTopBar.Height < S(70) ? S(42) : S(50);
        int yLogo = (pnlTopBar.Height - logoSize) / 2;

        lblLogo.SetBounds(S(20), yLogo, logoSize, logoSize);
        lblBrand.SetBounds(lblLogo.Right + S(12), yLogo, estua ? S(210) : S(300), logoSize);

        // Eskuineko API egoera + Freskatu botoia DPI-rekin eskalatuta
        int btnFresW = S(112);
        int btnFresH = S(36);
        int statusTextW = S(175);
        int statusPanelW = S(28) + statusTextW + S(12) + btnFresW + S(8);
        int statusPanelH = S(44);
        _pnlTopStatus.Size = new Size(statusPanelW, statusPanelH);
        _lblApiStatusDot.SetBounds(S(6), (statusPanelH - S(16)) / 2, S(16), S(16));
        _lblApiStatusText.SetBounds(_lblApiStatusDot.Right + S(6), 0, statusTextW, statusPanelH);
        _btnTopFreskatu.SetBounds(_lblApiStatusText.Right + S(8), (statusPanelH - btnFresH) / 2, btnFresW, btnFresH);

        _pnlTopStatus.Location = new Point(Math.Max(lblBrand.Right + S(10), pnlTopBar.Width - _pnlTopStatus.Width - S(20)), (pnlTopBar.Height - statusPanelH) / 2);

        // Breakpoint (< 960px): erdiko titulua ezkutatu gainjarpenak saihesteko
        lblZerbitzariak.Visible = !estua;
        if (!estua)
        {
            lblZerbitzariak.Size = new Size(S(220), S(36));
            lblZerbitzariak.Location = new Point((pnlTopBar.Width - lblZerbitzariak.Width) / 2, (pnlTopBar.Height - lblZerbitzariak.Height) / 2);
        }
    }

    private void ZentratuTxartela()
    {
        int S(int px) => Theme.S(this, px);
        int erabilgarriH = Math.Max(S(400), ClientSize.Height - pnlTopBar.Height - S(24));
        bool trinkoa = erabilgarriH < S(640);

        int cardW = Math.Clamp(ClientSize.Width - S(48), S(500), S(640));
        int cardH = trinkoa ? Math.Clamp(erabilgarriH - S(12), S(410), S(600)) : Math.Min(erabilgarriH - S(16), S(690));

        pnlCard.SuspendLayout();
        pnlCard.Size = new Size(cardW, cardH);

        int padX = S(24);
        int innerW = cardW - padX * 2;
        int avatarH = trinkoa ? S(104) : S(124);
        int y = trinkoa ? S(10) : S(14);

        pnlAvatars.SetBounds(padX, y, innerW, avatarH);
        _pnlOfflineNotice.SetBounds(padX, y, innerW, avatarH);
        _btnOfflineRetry.Size = new Size(S(180), S(36));
        _btnOfflineRetry.Location = new Point((_pnlOfflineNotice.Width - _btnOfflineRetry.Width) / 2, Math.Max(S(56), avatarH - S(42)));
        y += avatarH + (trinkoa ? S(6) : S(10));

        pnlDivider.SetBounds(padX, y, innerW, 1);
        y += trinkoa ? S(6) : S(10);

        int pinTitleH = trinkoa ? S(26) : S(34);
        lblPinTitle.SetBounds(padX, y, innerW, pinTitleH);
        y += pinTitleH + S(4);

        int dotSz = S(18);
        int dotGap = S(18);
        int dotsW = dotSz * 4 + dotGap * 3 + S(16);
        int dotsH = S(30);
        pnlPinDots.SetBounds((cardW - dotsW) / 2, y, dotsW, dotsH);
        Label[] dots = { lblDot1, lblDot2, lblDot3, lblDot4 };
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].SetBounds(S(8) + i * (dotSz + dotGap), (dotsH - dotSz) / 2, dotSz, dotSz);
        }
        y += dotsH + (trinkoa ? S(6) : S(10));

        int btnLoginH = trinkoa ? S(48) : S(58);
        int statusH = trinkoa ? S(34) : S(44);
        int bottomReserve = statusH + btnLoginH + (trinkoa ? S(18) : S(28));
        int keypadH = Math.Clamp(cardH - y - bottomReserve, S(180), S(290));
        int keypadW = Math.Min(innerW - S(8), S(520));

        pnlKeypad.SetBounds((cardW - keypadW) / 2, y, keypadW, keypadH);
        y += keypadH + (trinkoa ? S(6) : S(10));

        lblStatus.SetBounds(padX, y, innerW, statusH);
        y += statusH + S(4);

        btnLogin.SetBounds((cardW - keypadW) / 2, Math.Min(y, cardH - btnLoginH - S(12)), keypadW, btnLoginH);

        pnlCard.ResumeLayout(true);

        int x = Math.Max(S(16), (ClientSize.Width - pnlCard.Width) / 2);
        int cy = Math.Max(pnlTopBar.Height + S(12), pnlTopBar.Height + (ClientSize.Height - pnlTopBar.Height - pnlCard.Height) / 2);
        pnlCard.Location = new Point(x, cy);
        pnlCard.Invalidate();
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
        // Iragazi eta ordenatu: Zerbitzariak lehenik (Mikel Maître buru), gero Barrakoak, ondoren AdminIT eta Kudeatzaileak.
        var berriak = langileak
            .Where(DaBaimendua)
            .OrderBy(l => LortuRolaLehentasuna(l.Rola))
            .ThenBy(l => l.Erabiltzailea == "mikel_zerb" ? 0 :
                         l.Erabiltzailea == "gorka_barra" ? 0 :
                         l.Erabiltzailea == "anderu" ? 0 : 1)
            .ThenBy(l => l.Izena)
            .ToList();

        // AJAX-diff: langile zerrenda ez bada aldatu, ez berreraiki avatarrak (0 parpadeo)
        if (_avatarCards.Count == berriak.Count &&
            _avatarCards.Count > 0 &&
            _avatarCards.Zip(berriak, (c, l) => c.Langilea.Id == l.Id && c.Langilea.Izena == l.Izena && c.Langilea.Rola == l.Rola).All(x => x))
        {
            return;
        }

        _baimendutakoLangileak = berriak;

        UiPerf.RunFlickerFree(pnlAvatars, () =>
        {
            pnlAvatars.AutoScrollPosition = new Point(0, 0);
            pnlAvatars.Controls.Clear();
            _avatarCards.Clear();

            if (_baimendutakoLangileak.Count == 0)
            {
                return;
            }

            int S(int px) => Theme.S(this, px);
            int cardWidth = S(92);
            int cardHeight = S(96);
            int gap = S(8);
            int circleSz = S(48);
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
                    Location = new Point(startX + i * (cardWidth + gap), S(2)),
                    Cursor = Cursors.Hand,
                    BackColor = Color.Transparent
                };

                var lblCircle = new Label
                {
                    Size = new Size(circleSz, circleSz),
                    Location = new Point((cardWidth - circleSz) / 2, S(2)),
                    Text = initials,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Cursor = Cursors.Hand,
                    BackColor = Color.Transparent
                };

                var lblName = new Label
                {
                    Size = new Size(cardWidth, S(20)),
                    Location = new Point(0, circleSz + S(4)),
                    Text = shortName,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = Theme.BodyFont(this, 8.8f, bold: false),
                    ForeColor = Theme.WarmSlate,
                    Cursor = Cursors.Hand,
                    BackColor = Color.Transparent
                };

                var lblRole = new Label
                {
                    Size = new Size(cardWidth, S(18)),
                    Location = new Point(0, circleSz + S(24)),
                    Text = roleText,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = Theme.BodyFont(this, 7.5f, bold: true),
                    ForeColor = Theme.StoneMuted,
                    Cursor = Cursors.Hand,
                    BackColor = Color.Transparent
                };

                var info = new AvatarCardInfo
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
                    bool isSelected = _aukeratutakoLangilea?.Id == l.Id;
                    Color bgCol = isSelected ? Theme.ChiantiPrimary : Theme.StoneMist;
                    Color borderCol = isSelected ? Theme.GoldBorder : Theme.WarmTaupe;
                    Color fgCol = isSelected ? Theme.MarbleWhite : Theme.EspressoInk;

                    using var bgBrush = new SolidBrush(bgCol);
                    e.Graphics.FillEllipse(bgBrush, 2, 2, lblCircle.Width - 5, lblCircle.Height - 5);
                    using var pen = new Pen(borderCol, isSelected ? 2f : 1.2f);
                    e.Graphics.DrawEllipse(pen, 2, 2, lblCircle.Width - 5, lblCircle.Height - 5);

                    using var circleFont = Theme.BodyFont(this, 12f, bold: true);
                    TextRenderer.DrawText(e.Graphics, lblCircle.Text, circleFont,
                        new Rectangle(0, 0, lblCircle.Width, lblCircle.Height),
                        fgCol,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                };

                void SelectWorker(object? s, EventArgs e)
                {
                    HautatuLangilea(l);
                }

                pnlCardItem.Click += SelectWorker;
                lblCircle.Click += SelectWorker;
                lblName.Click += SelectWorker;
                lblRole.Click += SelectWorker;
                LotuHorizontalaMouseWheel(pnlCardItem);

                pnlCardItem.Controls.Add(lblCircle);
                pnlCardItem.Controls.Add(lblName);
                pnlCardItem.Controls.Add(lblRole);

                _avatarCards.Add(info);
                pnlAvatars.Controls.Add(pnlCardItem);
            }
        }, preserveScroll: false);

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
            item.Name.Font = Theme.BodyFont(this, 9.2f, bold: isSelected);
            item.Name.ForeColor = isSelected ? Color.FromArgb(30, 41, 59) : Color.FromArgb(100, 116, 139);
            item.Role.Font = Theme.BodyFont(this, 7.5f, bold: isSelected);
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
            _api.EzarriUnekoLangilea(sesioLangilea.Izena, sesioLangilea.Erabiltzailea, sesioLangilea.Rola);
            _pin = string.Empty;
            FreskatuPinDots();

            _autoKonexioTimer.Stop();

            var mahaiakForm = new MahaiakForm(sesioLangilea, _api);
            Theme.PlaceOnActiveScreen(mahaiakForm, this);
            mahaiakForm.FormClosed += async (s, e) =>
            {
                _api.EzarriUnekoLangilea(null, null, null);
                Theme.PlaceOnActiveScreen(this);
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
