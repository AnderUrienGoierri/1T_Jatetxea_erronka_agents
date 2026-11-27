using System.Drawing.Drawing2D;
using Jatetxea.Desktop.Models;
using Jatetxea.Desktop.Services;
using Jatetxea.Desktop.UI;

namespace Jatetxea.Desktop.Forms;

public partial class ErreserbakForm : BaseTpvForm
{
    private readonly LangileaDto _langilea;
    private readonly ApiService _api;
    private readonly List<MahaiaDto> _mahaiak;

    private readonly DateTime _momentukoHilabetea;
    private DateTime _bistaratzekoHilabetea;
    private DateTime _hautatutakoData;

    private List<ErreserbaDto> _erreserbaGuztiak = new();
    private Dictionary<string, List<ErreserbaDto>> _erreserbakDataMap = new();
    private readonly System.Windows.Forms.Timer _autoRefreshTimer = new() { Interval = 5000 };
    private bool _kargatzen = false;

    private readonly (string Kodea, string Izena, string Ordutegia)[] _txandak = new[]
    {
        ("Bazkaria_1", "Bazkaria 1", "13:00 - 14:30"),
        ("Bazkaria_2", "Bazkaria 2", "14:30 - 16:00"),
        ("Afaria_1", "Afaria 1", "20:30 - 22:00"),
        ("Afaria_2", "Afaria 2", "22:00 - 23:30")
    };

    /// <summary>
    /// Parameterless constructor for Visual Studio Form Designer.
    /// </summary>
    public ErreserbakForm() : this(new LangileaDto(), new ApiService(), new List<MahaiaDto>())
    {
    }

    public ErreserbakForm(LangileaDto langilea, ApiService api, List<MahaiaDto> mahaiak)
    {
        _langilea = langilea;
        _api = api;
        _mahaiak = mahaiak;

        _momentukoHilabetea = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        _bistaratzekoHilabetea = _momentukoHilabetea;
        _hautatutakoData = DateTime.Today;

        InitializeComponent();

        // T1 Warm Hospitality v2.0 paleta + parpadeorik gabeko DoubleBuffering
        Theme.ApplyWarmHospitalityTheme(this);
        BackColor = Theme.WarmLinen;
        pnlLeftCalendar.BackColor = Theme.MarbleWhite;
        _pnlRightDetails.BackColor = Theme.WarmLinen;
        pnlSelectedDayHeader.BackColor = Theme.MarbleWhite;
        Theme.StyleButton(btnFreskatu, Theme.StoneMist, Theme.EspressoInk, Theme.WarmTaupe, radius: 10, fontSize: 9.5f);
        Theme.StyleButton(btnBerriaTop, Theme.ChiantiPrimary, Theme.MarbleWhite, null, radius: 10, fontSize: 9.5f);
        Theme.StyleButton(btnEgunekoErreserba, Theme.ChiantiPrimary, Theme.MarbleWhite, null, radius: 10, fontSize: 9.5f);

        if (!string.IsNullOrWhiteSpace(_langilea.Izena))
        {
            lblSub.Text = $"Kudeatzailea: {_langilea.Izena} ({_langilea.Rola}) · Telefonoz eta Pertsonan jasotako erreserbak";
        }

        btnFreskatu.Click += async (s, e) => await KargatuErreserbakGuztiakAsync(behartu: true);
        btnBerriaTop.Click += (s, e) => IrekiErreserbaBerria(_hautatutakoData);

        pnlLeftCalendar.Paint += (s, e) =>
        {
            using var pen = new Pen(Theme.WarmTaupe, 1.2f);
            e.Graphics.DrawLine(pen, pnlLeftCalendar.Width - 1, 0, pnlLeftCalendar.Width - 1, pnlLeftCalendar.Height);
        };

        btnPrevMonth.Click += (s, e) =>
        {
            if (_bistaratzekoHilabetea > _momentukoHilabetea)
            {
                _bistaratzekoHilabetea = _bistaratzekoHilabetea.AddMonths(-1);
                EguneratuHilabeteIkuspegia();
            }
        };

        btnNextMonth.Click += (s, e) =>
        {
            if (_bistaratzekoHilabetea < _momentukoHilabetea.AddMonths(4))
            {
                _bistaratzekoHilabetea = _bistaratzekoHilabetea.AddMonths(1);
                EguneratuHilabeteIkuspegia();
            }
        };

        string[] egunLaburrak = { "Al", "Ar", "Az", "Og", "Or", "Lr", "Ig" };
        for (int i = 0; i < 7; i++)
        {
            bool asteburua = i >= 5;
            var lbl = new Label
            {
                Text = egunLaburrak[i],
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = asteburua ? Theme.AccentWine : Color.FromArgb(71, 85, 105)
            };
            pnlDayHeaders.Controls.Add(lbl, i, 0);
        }

        pnlSelectedDayHeader.Paint += (s, e) =>
        {
            using var pen = new Pen(Color.FromArgb(226, 232, 240), 1.5f);
            e.Graphics.DrawRectangle(pen, 0, 0, pnlSelectedDayHeader.Width - 1, pnlSelectedDayHeader.Height - 1);
        };

        btnEgunekoErreserba.Click += (s, e) => IrekiErreserbaBerria(_hautatutakoData);

        _autoRefreshTimer.Tick += async (s, e) => await KargatuErreserbakGuztiakAsync(behartu: false);

        FormClosing += (s, e) =>
        {
            _autoRefreshTimer.Stop();
            _autoRefreshTimer.Dispose();
        };

        Load += async (s, e) =>
        {
            EgokituResponsiveDiseinua();
            await KargatuErreserbakGuztiakAsync(behartu: true);
            _autoRefreshTimer.Start();
        };

        Resize += (s, e) => EgokituResponsiveDiseinua();

        Theme.AttachMainFormDpiAdaptation(this, () =>
        {
            EgokituResponsiveDiseinua();
            EguneratuHilabeteIkuspegia();
        });
    }

    private void EgokituResponsiveDiseinua()
    {
        int S(int px) => Theme.S(this, px);
        bool estua = ClientSize.Width < S(1040);

        // 1. Goiko Chianti barra (DPI-rekin eskalatuta, titulu/azpititulu gainjarpenik gabe)
        pnlHeader.Height = S(86);
        lblTit.Font = Theme.DisplayFont(this, 13.5f);
        lblTit.ForeColor = Theme.MarbleWhite;
        lblTit.Location = new Point(S(24), S(14));
        lblSub.Font = Theme.BodyFont(this, 9.2f, bold: false);
        lblSub.ForeColor = Theme.ChiantiTint;
        lblSub.Location = new Point(S(25), S(48));
        lblSub.Visible = ClientSize.Width >= S(900);

        int btnH = S(40);
        int btnY = (pnlHeader.Height - btnH) / 2;
        int wBerria = S(186);
        int wFres = S(112);
        btnBerriaTop.SetBounds(pnlHeader.Width - wBerria - S(24), btnY, wBerria, btnH);
        btnFreskatu.SetBounds(btnBerriaTop.Left - wFres - S(12), btnY, wFres, btnH);

        // 2. Ezkerreko egutegia
        pnlLeftCalendar.Width = estua ? S(380) : S(490);
        pnlLeftCalendar.Padding = new Padding(S(16), S(14), S(16), S(14));
        pnlMonthNav.Height = S(44);
        int navBtnW = S(38);
        int navBtnH = S(36);
        int navBtnY = (pnlMonthNav.Height - navBtnH) / 2;
        btnPrevMonth.SetBounds(0, navBtnY, navBtnW, navBtnH);
        btnNextMonth.SetBounds(pnlMonthNav.ClientSize.Width - navBtnW, navBtnY, navBtnW, navBtnH);
        _lblMonthTitle.SetBounds(btnPrevMonth.Right + S(6), 0, Math.Max(S(120), btnNextMonth.Left - btnPrevMonth.Right - S(12)), pnlMonthNav.Height);
        _pnlMonthPills.AutoScroll = true;
        _pnlMonthPills.Height = S(46);
        pnlDayHeaders.Height = S(30);
        _pnlDayGrid.Height = ClientSize.Height < S(600) ? S(250) : S(340);

        // 3. Eskuineko egun-hautatu goiburua (titulu eta estatistika bananduta!)
        _pnlRightDetails.Padding = estua ? new Padding(S(14), S(12), S(14), S(12)) : new Padding(S(24), S(16), S(24), S(20));
        pnlSelectedDayHeader.Height = S(84);
        _lblSelectedDayTitle.Font = Theme.BodyFont(this, 13f, bold: true);
        _lblSelectedDayTitle.ForeColor = Theme.EspressoInk;
        _lblSelectedDayTitle.Location = new Point(S(20), S(12));
        _lblSelectedDayStats.Font = Theme.BodyFont(this, 9.5f, bold: true);
        _lblSelectedDayStats.ForeColor = Theme.ChiantiPrimary;
        _lblSelectedDayStats.Location = new Point(S(20), S(48));

        btnEgunekoErreserba.Text = estua ? "+ Erreserbatu" : "+ Egun honetan erreserbatu";
        var btnSz = TextRenderer.MeasureText(btnEgunekoErreserba.Text, btnEgunekoErreserba.Font);
        int btnDayW = Math.Max(S(140), btnSz.Width + S(30));
        int btnDayH = S(40);
        btnEgunekoErreserba.SetBounds(pnlSelectedDayHeader.Width - btnDayW - S(18), (pnlSelectedDayHeader.Height - btnDayH) / 2, btnDayW, btnDayH);

        // 4. Txanden txartelak zabalera guztira egokitu
        int targetW = Math.Max(S(340), _pnlTurnosContainer.ClientSize.Width - S(28));
        _pnlTurnosContainer.SuspendLayout();
        foreach (Control box in _pnlTurnosContainer.Controls)
        {
            if (box is Panel p)
            {
                p.Width = targetW;
                foreach (Control child in p.Controls)
                {
                    if (child is Panel card)
                    {
                        card.Width = Math.Max(S(280), targetW - S(32));
                    }
                }
                p.Invalidate();
            }
        }
        _pnlTurnosContainer.ResumeLayout(true);
    }

    /// <summary>
    /// Zerbitzariko erreserba guztiak deskargatzen ditu eta soilik aldaketak badaude eguneratzen du UI-a (AJAX diff, 0 parpadeo).
    /// </summary>
    private async Task KargatuErreserbakGuztiakAsync(bool behartu = true)
    {
        if (_kargatzen) return;
        _kargatzen = true;
        try
        {
            var berriak = await _api.GetErreserbakAsync();
            if (IsDisposed) return;

            if (!behartu && !ErreserbakAldatuDira(_erreserbaGuztiak, berriak))
            {
                return;
            }

            _erreserbaGuztiak = berriak;
            _erreserbakDataMap = _erreserbaGuztiak
                .Where(x => !string.IsNullOrWhiteSpace(x.Data))
                .GroupBy(x => x.Data)
                .ToDictionary(g => g.Key, g => g.ToList());

            UiPerf.RunFlickerFree(this, () => EguneratuHilabeteIkuspegia(), preserveScroll: true);
        }
        catch
        {
            // Auto-freskatze isilean erroreak ez blokeatu UI-a
        }
        finally
        {
            _kargatzen = false;
        }
    }

    private static bool ErreserbakAldatuDira(List<ErreserbaDto> a, List<ErreserbaDto> b)
    {
        if (a.Count != b.Count) return true;
        for (int i = 0; i < a.Count; i++)
        {
            if (a[i].Id != b[i].Id ||
                a[i].MahaiaId != b[i].MahaiaId ||
                a[i].PertsonaKopurua != b[i].PertsonaKopurua ||
                !string.Equals(a[i].Data, b[i].Data, StringComparison.Ordinal) ||
                !string.Equals(a[i].Txanda, b[i].Txanda, StringComparison.Ordinal) ||
                !string.Equals(a[i].Egoera, b[i].Egoera, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Hilabetea aldatzen denean (edo datuak freskatzean), goiburua, tab azkarrak,
    /// kalendarioko sareta eta hautatutako egunaren xehetasunak berritzen ditu.
    /// </summary>
    private void EguneratuHilabeteIkuspegia()
    {
        string hilabeteIzena = LortuHilabeteIzena(_bistaratzekoHilabetea.Month);
        _lblMonthTitle.Text = $"{hilabeteIzena} {_bistaratzekoHilabetea.Year}";

        BerrituHilabetePillak();
        BerrituEgutegia();
        BerrituEgunekoTxandak();
    }

    /// <summary>
    /// Momentuko hilabetea eta hurrengo 4 hilabeteak (guztira 5 hilabete) bistaratzen dituen botoi-barra.
    /// </summary>
    private void BerrituHilabetePillak()
    {
        _pnlMonthPills.SuspendLayout();
        _pnlMonthPills.Controls.Clear();

        for (int i = 0; i < 5; i++)
        {
            var hil = _momentukoHilabetea.AddMonths(i);
            bool isCurrentSelected = hil.Year == _bistaratzekoHilabetea.Year && hil.Month == _bistaratzekoHilabetea.Month;

            // Zenbatu hilabete honetako erreserbak
            int hErreserbak = _erreserbaGuztiak.Count(x =>
            {
                if (DateTime.TryParse(x.Data, out var d))
                    return d.Year == hil.Year && d.Month == hil.Month;
                return false;
            });

            string hIzena = LortuHilabeteIzena(hil.Month);
            string laburra = hIzena.Length > 4 ? hIzena.Substring(0, 4) : hIzena;
            string testua = i == 0 ? $"{laburra}* ({hErreserbak})" : $"{laburra} ({hErreserbak})";

            var pillFont = Theme.BodyFont(8.5f, bold: isCurrentSelected || hErreserbak > 0);
            var sz = TextRenderer.MeasureText(testua, pillFont);
            int pillW = Math.Max(Theme.S(this, 82), sz.Width + Theme.S(this, 18));

            var btnPill = new Button
            {
                Text = testua,
                Size = new Size(pillW, Theme.S(this, 34)),
                Margin = new Padding(Theme.S(this, 2), Theme.S(this, 4), Theme.S(this, 4), Theme.S(this, 4))
            };
            Theme.StyleButton(
                btnPill,
                isCurrentSelected ? Theme.ChiantiPrimary : (hErreserbak > 0 ? Theme.ChiantiTint : Theme.MarbleWhite),
                isCurrentSelected ? Theme.MarbleWhite : (hErreserbak > 0 ? Theme.ChiantiPrimary : Theme.EspressoInk),
                isCurrentSelected ? null : (hErreserbak > 0 ? Theme.ChiantiPrimary : Theme.WarmTaupe),
                radius: 8,
                fontSize: 8.5f,
                bold: isCurrentSelected || hErreserbak > 0);

            var captureHil = hil;
            btnPill.Click += (s, e) =>
            {
                _bistaratzekoHilabetea = captureHil;
                // Egun hautatua hilabete berriaren barruan ez badago, hilabetearen lehen egunera aldatu
                if (_hautatutakoData.Year != captureHil.Year || _hautatutakoData.Month != captureHil.Month)
                {
                    _hautatutakoData = (captureHil.Year == DateTime.Today.Year && captureHil.Month == DateTime.Today.Month)
                        ? DateTime.Today
                        : captureHil;
                }
                EguneratuHilabeteIkuspegia();
            };

            _pnlMonthPills.Controls.Add(btnPill);
        }

        _pnlMonthPills.ResumeLayout(true);
    }

    /// <summary>
    /// Kalendarioko 7x6 sareta margotzen du egunekin, erreserben markekin eta hautatutako egunarekin.
    /// </summary>
    private void BerrituEgutegia()
    {
        _pnlDayGrid.SuspendLayout();
        _pnlDayGrid.Controls.Clear();

        DateTime lehena = new DateTime(_bistaratzekoHilabetea.Year, _bistaratzekoHilabetea.Month, 1);
        int egunKop = DateTime.DaysInMonth(_bistaratzekoHilabetea.Year, _bistaratzekoHilabetea.Month);

        // Astelehena = 0, Igandea = 6
        int hasieraZutabea = ((int)lehena.DayOfWeek - 1 + 7) % 7;

        int row = 0;
        int col = hasieraZutabea;

        int hilabetekoErreserbaKop = 0;
        int hilabetekoPaxKop = 0;

        for (int eguna = 1; eguna <= egunKop; eguna++)
        {
            var egunData = new DateTime(_bistaratzekoHilabetea.Year, _bistaratzekoHilabetea.Month, eguna);
            string dataKey = egunData.ToString("yyyy-MM-dd");

            bool badaudeErreserbak = _erreserbakDataMap.TryGetValue(dataKey, out var egunekoErreserbak) && egunekoErreserbak.Count > 0;
            int errKop = badaudeErreserbak ? egunekoErreserbak!.Count : 0;
            int paxKop = badaudeErreserbak ? egunekoErreserbak!.Sum(x => x.PertsonaKopurua) : 0;

            if (badaudeErreserbak)
            {
                hilabetekoErreserbaKop += errKop;
                hilabetekoPaxKop += paxKop;
            }

            bool isToday = egunData == DateTime.Today;
            bool isSelected = egunData.Date == _hautatutakoData.Date;

            int captureEgunaZenbakia = eguna;
            var cell = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(2),
                Cursor = Cursors.Hand,
                BackColor = isSelected 
                    ? Theme.AccentWine 
                    : (badaudeErreserbak ? Color.FromArgb(253, 242, 248) : Color.White)
            };

            cell.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                // Ertza
                Color borderColor = isSelected 
                    ? Color.FromArgb(136, 19, 55) 
                    : (isToday ? Color.FromArgb(37, 99, 235) : (badaudeErreserbak ? Color.FromArgb(244, 114, 182) : Color.FromArgb(226, 232, 240)));
                float stroke = (isSelected || isToday || badaudeErreserbak) ? 1.8f : 1f;

                using var pen = new Pen(borderColor, stroke);
                e.Graphics.DrawRectangle(pen, 1, 1, cell.Width - 2, cell.Height - 2);

                // Egunaren zenbakia
                Color dayColor = isSelected 
                    ? Color.White 
                    : (isToday ? Color.FromArgb(37, 99, 235) : Color.FromArgb(30, 41, 59));
                using var fontDay = Theme.BodyFont(this, 10f, bold: isToday || isSelected);
                using var brushDay = new SolidBrush(dayColor);
                e.Graphics.DrawString(captureEgunaZenbakia.ToString(), fontDay, brushDay, new PointF(Theme.S(this, 4), Theme.S(this, 3)));

                // Gaur bada eta hautatua ez bada, "Gaur" testua
                if (isToday && !isSelected)
                {
                    using var fontGaur = Theme.BodyFont(this, 6.5f, bold: true);
                    using var gaurBrush = new SolidBrush(Color.FromArgb(37, 99, 235));
                    var gSz = e.Graphics.MeasureString("GAUR", fontGaur);
                    e.Graphics.DrawString("GAUR", fontGaur, gaurBrush, new PointF(cell.Width - gSz.Width - Theme.S(this, 3), Theme.S(this, 4)));
                }

                // Erreserba badaude, intsignia / badge margotu
                if (badaudeErreserbak)
                {
                    Color badgeBg = isSelected ? Color.White : Color.FromArgb(190, 24, 93);
                    Color badgeFg = isSelected ? Theme.AccentWine : Color.White;

                    string badgeText = $"{errKop} err";
                    using var fontBadge = Theme.BodyFont(this, 7f, bold: true);
                    var txtSize = e.Graphics.MeasureString(badgeText, fontBadge);

                    int badgeW = (int)txtSize.Width + Theme.S(this, 8);
                    int badgeH = Theme.S(this, 16);
                    int badgeX = (cell.Width - badgeW) / 2;
                    int badgeY = cell.Height - badgeH - Theme.S(this, 3);

                    using var brush = new SolidBrush(badgeBg);
                    e.Graphics.FillRectangle(brush, badgeX, badgeY, badgeW, badgeH);

                    using var fgBrush = new SolidBrush(badgeFg);
                    e.Graphics.DrawString(badgeText, fontBadge, fgBrush, new PointF(badgeX + Theme.S(this, 4), badgeY + Theme.S(this, 1)));
                }
            };

            var captureEguna = egunData;
            cell.Click += (s, e) =>
            {
                _hautatutakoData = captureEguna;
                BerrituEgutegia();
                BerrituEgunekoTxandak();
            };

            _pnlDayGrid.Controls.Add(cell, col, row);

            col++;
            if (col > 6)
            {
                col = 0;
                row++;
                if (row >= 6) break;
            }
        }

        _pnlDayGrid.ResumeLayout(true);

        // Oineko estatistikak eta legenda
        _lblCalendarFooter.Text = $"Hilabete honetan: {hilabetekoErreserbaKop} erreserba ({hilabetekoPaxKop} lagun)\n" +
                                  $"● Arrosa: Erreserbak daude  |  Urdina: Gaur  |  Granatea: Hautatua";
    }

    /// <summary>
    /// Eskuineko panelean hautatutako egunaren xehetasunak margotzen ditu, 4 txandatan banatuta.
    /// </summary>
    private void BerrituEgunekoTxandak()
    {
        _pnlTurnosContainer.SuspendLayout();
        _pnlTurnosContainer.Controls.Clear();

        string dataStr = _hautatutakoData.ToString("yyyy-MM-dd");
        string egunIzenburua = FormatuaEgunaEuskaraz(_hautatutakoData);
        _lblSelectedDayTitle.Text = $"📅 {egunIzenburua}";

        bool badaude = _erreserbakDataMap.TryGetValue(dataStr, out var egunekoak) && egunekoak.Count > 0;
        int guztiraErreserbak = badaude ? egunekoak!.Count : 0;
        int guztiraPax = badaude ? egunekoak!.Sum(x => x.PertsonaKopurua) : 0;

        _lblSelectedDayStats.Text = guztiraErreserbak > 0
            ? $"Guztira: {guztiraErreserbak} erreserba egun honetan ({guztiraPax} pertsona)"
            : "Ez dago erreserbarik egun honetan (Mahaia guztiak libre)";

        int containerWidth = Math.Max(320, _pnlTurnosContainer.ClientSize.Width - 26);

        foreach (var (kodea, izena, ordutegia) in _txandak)
        {
            var txandakoErreserbak = badaude
                ? egunekoak!.Where(x => x.Txanda.Equals(kodea, StringComparison.OrdinalIgnoreCase)).ToList()
                : new List<ErreserbaDto>();

            var pnlTxandaCard = SortuTxandaBlokea(kodea, izena, ordutegia, txandakoErreserbak, containerWidth);
            _pnlTurnosContainer.Controls.Add(pnlTxandaCard);
        }

        _pnlTurnosContainer.ResumeLayout(true);
    }

    /// <summary>
    /// Txanda bakoitzeko txartela sortzen du (Goiburua, estatistika eta erreserba txartelak edo hutsik mezua).
    /// </summary>
    private Control SortuTxandaBlokea(string kodea, string izena, string ordutegia, List<ErreserbaDto> txandakoak, int width)
    {
        int S(int px) => Theme.S(this, px);
        var box = new Panel
        {
            Width = width,
            AutoSize = false,
            BackColor = Theme.MarbleWhite,
            Margin = new Padding(0, 0, 0, S(14)),
            Padding = new Padding(S(16))
        };
        box.Paint += (s, e) =>
        {
            using var pen = new Pen(Theme.WarmTaupe, 1.2f);
            e.Graphics.DrawRectangle(pen, 0, 0, box.Width - 1, box.Height - 1);

            Color ribbonColor = kodea.StartsWith("Bazkaria") ? Theme.Warning : Theme.Purple;
            using var brush = new SolidBrush(ribbonColor);
            e.Graphics.FillRectangle(brush, 0, 0, S(5), box.Height);
        };

        int curY = S(14);

        // Txanda Goiburua
        var lblTxandaTit = new Label
        {
            Text = $"{izena} ({ordutegia})",
            Font = Theme.BodyFont(11f, bold: true),
            ForeColor = Theme.EspressoInk,
            BackColor = Theme.MarbleWhite,
            Location = new Point(S(16), curY),
            AutoSize = true
        };

        var titSz = TextRenderer.MeasureText(lblTxandaTit.Text, lblTxandaTit.Font);
        int paxGuztira = txandakoak.Sum(x => x.PertsonaKopurua);
        var lblTxandaInfo = new Label
        {
            Text = txandakoak.Count > 0 ? $"{txandakoak.Count} erreserba · {paxGuztira} lagun" : "0 erreserba",
            Font = Theme.BodyFont(9f, bold: true),
            ForeColor = txandakoak.Count > 0 ? Theme.ChiantiPrimary : Theme.StoneMuted,
            BackColor = Theme.MarbleWhite,
            Location = new Point(S(16) + titSz.Width + S(16), curY + S(3)),
            AutoSize = true
        };

        int btnW = S(130);
        int btnH = S(34);
        var btnGehituTxandan = new Button
        {
            Text = "+ Erreserbatu",
            Size = new Size(btnW, btnH),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Location = new Point(box.Width - btnW - S(16), curY - S(2))
        };
        Theme.StyleButton(btnGehituTxandan, Theme.SuccessBg, Theme.Success, Theme.SuccessBorder, radius: 8, fontSize: 9f);
        btnGehituTxandan.Click += (s, e) => IrekiErreserbaBerria(_hautatutakoData, kodea);

        box.Resize += (s, e) =>
        {
            btnGehituTxandan.Location = new Point(box.Width - btnGehituTxandan.Width - S(16), S(12));
        };

        box.Controls.Add(lblTxandaTit);
        box.Controls.Add(lblTxandaInfo);
        box.Controls.Add(btnGehituTxandan);

        curY += S(44);

        if (txandakoak.Count == 0)
        {
            var lblHutsa = new Label
            {
                Text = "Ez dago erreserbarik txanda honetan (Mahai guztiak libre daude).",
                Font = new Font("Segoe UI", 9f, FontStyle.Italic),
                ForeColor = Theme.StoneMuted,
                BackColor = Theme.MarbleWhite,
                Location = new Point(S(16), curY),
                Size = new Size(Math.Max(S(240), width - S(36)), S(26))
            };
            box.Controls.Add(lblHutsa);
            curY += S(32);
        }
        else
        {
            foreach (var err in txandakoak)
            {
                var card = SortuErreserbaTxartelTxikia(err, width - S(36));
                card.Location = new Point(S(16), curY);
                box.Controls.Add(card);
                curY += card.Height + S(10);
            }
        }

        box.Height = curY + S(10);
        return box;
    }

    /// <summary>
    /// Txanda baten barruko erreserba bakoitzaren txartela.
    /// </summary>
    private Control SortuErreserbaTxartelTxikia(ErreserbaDto err, int width)
    {
        var item = new Panel
        {
            Width = width,
            Height = 88,
            BackColor = Color.FromArgb(248, 250, 252),
            Padding = new Padding(12)
        };
        item.Paint += (s, e) =>
        {
            using var pen = new Pen(Color.FromArgb(226, 232, 240), 1f);
            e.Graphics.DrawRectangle(pen, 0, 0, item.Width - 1, item.Height - 1);
        };

        // Mahai badgea
        var lblMahaia = new Label
        {
            Text = $"MAHAIA {err.MahaiZenbakia}",
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = Theme.AccentWine,
            TextAlign = ContentAlignment.MiddleCenter,
            Location = new Point(10, 10),
            Size = new Size(84, 24)
        };

        // Kokapena eta Pax
        var lblKokapena = new Label
        {
            Text = $"{err.Kokapena}  ·  {err.PertsonaKopurua} lagun",
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            ForeColor = Color.FromArgb(71, 85, 105),
            Location = new Point(102, 13),
            AutoSize = true
        };

        // Bezeroaren Izena
        var lblBezeroa = new Label
        {
            Text = $"{err.BezeroIzena} {err.BezeroAbizena}",
            Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42),
            Location = new Point(10, 38),
            AutoSize = true
        };

        // Kontaktua
        var lblKontaktua = new Label
        {
            Text = $"Tel: {err.BezeroTelefonoa}  |  {err.BezeroEmaila}",
            Font = new Font("Segoe UI", 8.5F, FontStyle.Regular),
            ForeColor = Color.FromArgb(100, 116, 139),
            Location = new Point(10, 62),
            AutoSize = true
        };

        // Oharrak (eskuinean)
        if (!string.IsNullOrWhiteSpace(err.Oharrak))
        {
            var lblOharra = new Label
            {
                Text = $"Oharra: {err.Oharrak}",
                Font = new Font("Segoe UI", 8F, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(280, 40),
                Size = new Size(Math.Max(120, width - 400), 38)
            };
            item.Controls.Add(lblOharra);
        }

        // Ezeztatu Botoia
        var btnEzeztatu = new Button
        {
            Text = "Ezeztatu",
            Size = new Size(80, 26),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Location = new Point(item.Width - 92, 10),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(254, 242, 242),
            ForeColor = Color.FromArgb(220, 38, 38),
            Font = new Font("Segoe UI", 8F, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnEzeztatu.FlatAppearance.BorderColor = Color.FromArgb(254, 202, 202);
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
                    await KargatuErreserbakGuztiakAsync();
                }
                else
                {
                    MessageBox.Show(msg, "Errorea", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        };

        item.Resize += (s, e) =>
        {
            btnEzeztatu.Location = new Point(item.Width - 92, 10);
        };

        item.Controls.Add(lblMahaia);
        item.Controls.Add(lblKokapena);
        item.Controls.Add(lblBezeroa);
        item.Controls.Add(lblKontaktua);
        item.Controls.Add(btnEzeztatu);

        return item;
    }

    private void IrekiErreserbaBerria(DateTime? hasieraData = null, string? hasieraTxanda = null)
    {
        using var modal = new ErreserbaBerriaModal(_langilea, _api, _mahaiak, null, hasieraData ?? _hautatutakoData, hasieraTxanda);
        if (modal.ShowDialog(this) == DialogResult.OK)
        {
            _ = KargatuErreserbakGuztiakAsync();
        }
    }

    private static string LortuHilabeteIzena(int hilabetea) => hilabetea switch
    {
        1 => "Urtarrila",
        2 => "Otsaila",
        3 => "Martxoa",
        4 => "Apirila",
        5 => "Maiatza",
        6 => "Ekaina",
        7 => "Uztaila",
        8 => "Abuztua",
        9 => "Iraila",
        10 => "Urria",
        11 => "Azaroa",
        12 => "Abendua",
        _ => ""
    };

    private static string LortuEgunIzena(DayOfWeek dow) => dow switch
    {
        DayOfWeek.Monday => "Astelehena",
        DayOfWeek.Tuesday => "Asteartea",
        DayOfWeek.Wednesday => "Asteazkena",
        DayOfWeek.Thursday => "Osteguna",
        DayOfWeek.Friday => "Ostirala",
        DayOfWeek.Saturday => "Larunbata",
        DayOfWeek.Sunday => "Igandea",
        _ => ""
    };

    private static string FormatuaEgunaEuskaraz(DateTime d)
    {
        string hilabetea = LortuHilabeteIzena(d.Month);
        string astekoEguna = LortuEgunIzena(d.DayOfWeek);
        return $"{d.Year}ko {hilabetea}ren {d.Day}a, {astekoEguna}";
    }
}
