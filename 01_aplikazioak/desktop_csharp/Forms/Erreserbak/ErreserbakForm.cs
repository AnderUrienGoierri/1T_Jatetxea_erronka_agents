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

    private readonly DateTime _momentukoHilabetea;
    private DateTime _bistaratzekoHilabetea;
    private DateTime _hautatutakoData;

    private List<ErreserbaDto> _erreserbaGuztiak = new();
    private Dictionary<string, List<ErreserbaDto>> _erreserbakDataMap = new();

    // UI Osagaiak
    private readonly Label _lblMonthTitle;
    private readonly FlowLayoutPanel _pnlMonthPills;
    private readonly TableLayoutPanel _pnlDayGrid;
    private readonly Label _lblCalendarFooter;

    private readonly Panel _pnlRightDetails;
    private readonly Label _lblSelectedDayTitle;
    private readonly Label _lblSelectedDayStats;
    private readonly FlowLayoutPanel _pnlTurnosContainer;

    private readonly (string Kodea, string Izena, string Ordutegia)[] _txandak = new[]
    {
        ("Bazkaria_1", "Bazkaria 1", "13:00 - 14:30"),
        ("Bazkaria_2", "Bazkaria 2", "14:30 - 16:00"),
        ("Afaria_1", "Afaria 1", "20:30 - 22:00"),
        ("Afaria_2", "Afaria 2", "22:00 - 23:30")
    };

    public ErreserbakForm(LangileaDto langilea, ApiService api, List<MahaiaDto> mahaiak)
    {
        _langilea = langilea;
        _api = api;
        _mahaiak = mahaiak;

        _momentukoHilabetea = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        _bistaratzekoHilabetea = _momentukoHilabetea;
        _hautatutakoData = DateTime.Today;

        Text = "T1 Jatetxea - Erreserben Kudeaketa Zentrala (TPV)";
        Size = new Size(1250, 800);
        MinimumSize = new Size(1050, 680);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Theme.Background;
        Font = Theme.FontBody;

        // 1. Goiko goiburua
        var pnlHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 72,
            BackColor = Theme.AccentWine,
            Padding = new Padding(24, 12, 24, 12)
        };

        var lblTit = new Label
        {
            Text = "Erreserben Kudeaketa Zentrala (TPV)",
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
            Location = new Point(24, 10),
            AutoSize = true
        };

        var lblSub = new Label
        {
            Text = $"Kudeatzailea: {_langilea.Izena} ({_langilea.Rola}) · Telefonoz eta Pertsonan jasotako erreserbak",
            ForeColor = Color.FromArgb(254, 205, 211),
            Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
            Location = new Point(25, 38),
            AutoSize = true
        };

        var btnFreskatu = new Button
        {
            Text = "Freskatu",
            Size = new Size(100, 42),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Location = new Point(pnlHeader.Width - 325, 14),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(159, 18, 57),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnFreskatu.FlatAppearance.BorderColor = Color.FromArgb(225, 29, 72);
        btnFreskatu.Click += async (s, e) => await KargatuErreserbakGuztiakAsync();

        var btnBerriaTop = new Button
        {
            Text = "+ Erreserba Berria",
            Size = new Size(190, 42),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Location = new Point(pnlHeader.Width - 215, 14),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(22, 163, 74),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnBerriaTop.FlatAppearance.BorderSize = 0;
        btnBerriaTop.Click += (s, e) => IrekiErreserbaBerria(_hautatutakoData);

        pnlHeader.Resize += (s, e) =>
        {
            btnFreskatu.Location = new Point(pnlHeader.Width - 325, 14);
            btnBerriaTop.Location = new Point(pnlHeader.Width - 215, 14);
        };

        pnlHeader.Controls.Add(lblTit);
        pnlHeader.Controls.Add(lblSub);
        pnlHeader.Controls.Add(btnFreskatu);
        pnlHeader.Controls.Add(btnBerriaTop);

        // 2. Edukiontzi Nagusia (Ezkerra: Kalendarioa | Eskuina: Eguneko Txandak)
        var pnlMain = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Theme.Background
        };

        // 2.1 Ezkerreko Panela: Kalendario Interaktiboa (480px zabal)
        var pnlLeftCalendar = new Panel
        {
            Dock = DockStyle.Left,
            Width = 490,
            BackColor = Color.White,
            Padding = new Padding(16, 12, 16, 12)
        };
        pnlLeftCalendar.Paint += (s, e) =>
        {
            using var pen = new Pen(Color.FromArgb(226, 232, 240), 1.5f);
            e.Graphics.DrawLine(pen, pnlLeftCalendar.Width - 1, 0, pnlLeftCalendar.Width - 1, pnlLeftCalendar.Height);
        };

        // 2.1.1 Hilabete hautatzailearen goiburua (Aurrekoa, Hilabete Izena, Hurrengoa)
        var pnlMonthNav = new Panel
        {
            Dock = DockStyle.Top,
            Height = 44,
            BackColor = Color.White
        };

        var btnPrevMonth = new Button
        {
            Text = "◀",
            Size = new Size(38, 36),
            Location = new Point(2, 4),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(241, 245, 249),
            ForeColor = Color.FromArgb(71, 85, 105),
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnPrevMonth.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnPrevMonth.Click += (s, e) =>
        {
            if (_bistaratzekoHilabetea > _momentukoHilabetea)
            {
                _bistaratzekoHilabetea = _bistaratzekoHilabetea.AddMonths(-1);
                EguneratuHilabeteIkuspegia();
            }
        };

        var btnNextMonth = new Button
        {
            Text = "▶",
            Size = new Size(38, 36),
            Location = new Point(416, 4),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(241, 245, 249),
            ForeColor = Color.FromArgb(71, 85, 105),
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnNextMonth.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnNextMonth.Click += (s, e) =>
        {
            if (_bistaratzekoHilabetea < _momentukoHilabetea.AddMonths(4))
            {
                _bistaratzekoHilabetea = _bistaratzekoHilabetea.AddMonths(1);
                EguneratuHilabeteIkuspegia();
            }
        };

        _lblMonthTitle = new Label
        {
            Text = "",
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 12.5F, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42),
            Location = new Point(46, 4),
            Size = new Size(364, 36)
        };

        pnlMonthNav.Controls.Add(btnPrevMonth);
        pnlMonthNav.Controls.Add(_lblMonthTitle);
        pnlMonthNav.Controls.Add(btnNextMonth);

        // 2.1.2 Hurrengo 4 hilabeteen botoi/tab azkarrak (5 hilabeteak guztira)
        _pnlMonthPills = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 44,
            BackColor = Color.FromArgb(248, 250, 252),
            Padding = new Padding(2, 4, 2, 4),
            WrapContents = false,
            AutoScroll = false
        };

        // 2.1.3 Asteko Egunen Goiburua (Al, Ar, Az, Og, Or, Lr, Ig)
        var pnlDayHeaders = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 30,
            ColumnCount = 7,
            RowCount = 1,
            BackColor = Color.FromArgb(241, 245, 249),
            Margin = new Padding(0, 8, 0, 0)
        };
        for (int i = 0; i < 7; i++)
        {
            pnlDayHeaders.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.2857f));
        }

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

        // 2.1.4 Egunen Sareta (7 zutabe x 6 errenkada)
        _pnlDayGrid = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 310,
            ColumnCount = 7,
            RowCount = 6,
            BackColor = Color.White,
            Margin = new Padding(0)
        };
        for (int i = 0; i < 7; i++)
        {
            _pnlDayGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.2857f));
        }
        for (int i = 0; i < 6; i++)
        {
            _pnlDayGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 16.6666f));
        }

        // 2.1.5 Kalendarioaren Oina (Legenda eta estatistika)
        _lblCalendarFooter = new Label
        {
            Dock = DockStyle.Bottom,
            Height = 58,
            Font = new Font("Segoe UI", 8.5F, FontStyle.Regular),
            ForeColor = Color.FromArgb(100, 116, 139),
            TextAlign = ContentAlignment.MiddleCenter
        };

        pnlLeftCalendar.Controls.Add(_lblCalendarFooter);
        pnlLeftCalendar.Controls.Add(_pnlDayGrid);
        pnlLeftCalendar.Controls.Add(pnlDayHeaders);
        pnlLeftCalendar.Controls.Add(_pnlMonthPills);
        pnlLeftCalendar.Controls.Add(pnlMonthNav);

        // 2.2 Eskuineko Panela: Hautatutako Eguneko Txandak eta Xehetasunak
        _pnlRightDetails = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Theme.Background,
            Padding = new Padding(24, 16, 24, 20)
        };

        // Eguneko goiburua
        var pnlSelectedDayHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 70,
            BackColor = Color.White,
            Padding = new Padding(20, 10, 20, 10)
        };
        pnlSelectedDayHeader.Paint += (s, e) =>
        {
            using var pen = new Pen(Color.FromArgb(226, 232, 240), 1.5f);
            e.Graphics.DrawRectangle(pen, 0, 0, pnlSelectedDayHeader.Width - 1, pnlSelectedDayHeader.Height - 1);
        };

        _lblSelectedDayTitle = new Label
        {
            Text = "",
            Font = new Font("Segoe UI", 13.5F, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42),
            Location = new Point(18, 10),
            AutoSize = true
        };

        _lblSelectedDayStats = new Label
        {
            Text = "",
            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
            ForeColor = Theme.AccentWine,
            Location = new Point(19, 38),
            AutoSize = true
        };

        var btnEgunekoErreserba = new Button
        {
            Text = "+ Egun honetan erreserbatu",
            Size = new Size(205, 38),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Location = new Point(pnlSelectedDayHeader.Width - 225, 16),
            FlatStyle = FlatStyle.Flat,
            BackColor = Theme.AccentWine,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnEgunekoErreserba.FlatAppearance.BorderSize = 0;
        btnEgunekoErreserba.Click += (s, e) => IrekiErreserbaBerria(_hautatutakoData);

        pnlSelectedDayHeader.Resize += (s, e) =>
        {
            btnEgunekoErreserba.Location = new Point(pnlSelectedDayHeader.Width - 225, 16);
        };

        pnlSelectedDayHeader.Controls.Add(_lblSelectedDayTitle);
        pnlSelectedDayHeader.Controls.Add(_lblSelectedDayStats);
        pnlSelectedDayHeader.Controls.Add(btnEgunekoErreserba);

        // Txanden edukiontzia (4 txandak)
        _pnlTurnosContainer = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            BackColor = Theme.Background,
            Padding = new Padding(0, 14, 0, 0),
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false
        };

        _pnlRightDetails.Controls.Add(_pnlTurnosContainer);
        _pnlRightDetails.Controls.Add(pnlSelectedDayHeader);

        pnlMain.Controls.Add(_pnlRightDetails);
        pnlMain.Controls.Add(pnlLeftCalendar);

        Controls.Add(pnlMain);
        Controls.Add(pnlHeader);

        Load += async (s, e) => await KargatuErreserbakGuztiakAsync();
    }

    /// <summary>
    /// Zerbitzariko erreserba guztiak deskargatzen ditu eta dataren arabera antolatzen ditu.
    /// </summary>
    private async Task KargatuErreserbakGuztiakAsync()
    {
        _erreserbaGuztiak = await _api.GetErreserbakAsync();
        _erreserbakDataMap = _erreserbaGuztiak
            .Where(x => !string.IsNullOrWhiteSpace(x.Data))
            .GroupBy(x => x.Data)
            .ToDictionary(g => g.Key, g => g.ToList());

        EguneratuHilabeteIkuspegia();
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

            var btnPill = new Button
            {
                Text = testua,
                Size = new Size(90, 32),
                Margin = new Padding(2, 2, 2, 2),
                FlatStyle = FlatStyle.Flat,
                BackColor = isCurrentSelected ? Theme.AccentWine : (hErreserbak > 0 ? Color.FromArgb(253, 242, 248) : Color.White),
                ForeColor = isCurrentSelected ? Color.White : (hErreserbak > 0 ? Theme.AccentWine : Color.FromArgb(51, 65, 85)),
                Font = new Font("Segoe UI", 8.2F, isCurrentSelected || hErreserbak > 0 ? FontStyle.Bold : FontStyle.Regular),
                Cursor = Cursors.Hand
            };
            btnPill.FlatAppearance.BorderColor = isCurrentSelected ? Theme.AccentWine : (hErreserbak > 0 ? Color.FromArgb(244, 114, 182) : Color.FromArgb(203, 213, 225));
            btnPill.FlatAppearance.BorderSize = isCurrentSelected ? 0 : 1;

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
                using var fontDay = new Font("Segoe UI", 10F, isToday || isSelected ? FontStyle.Bold : FontStyle.Regular);
                e.Graphics.DrawString(eguna.ToString(), fontDay, new SolidBrush(dayColor), new PointF(4, 3));

                // Gaur bada eta hautatua ez bada, "Gaur" testua
                if (isToday && !isSelected)
                {
                    using var fontGaur = new Font("Segoe UI", 6.5F, FontStyle.Bold);
                    e.Graphics.DrawString("GAUR", fontGaur, new SolidBrush(Color.FromArgb(37, 99, 235)), new PointF(cell.Width - 32, 4));
                }

                // Erreserba badaude, intsignia / badge margotu
                if (badaudeErreserbak)
                {
                    Color badgeBg = isSelected ? Color.White : Color.FromArgb(190, 24, 93);
                    Color badgeFg = isSelected ? Theme.AccentWine : Color.White;

                    string badgeText = $"{errKop} err";
                    using var fontBadge = new Font("Segoe UI", 7F, FontStyle.Bold);
                    var txtSize = e.Graphics.MeasureString(badgeText, fontBadge);

                    int badgeW = (int)txtSize.Width + 8;
                    int badgeH = 16;
                    int badgeX = (cell.Width - badgeW) / 2;
                    int badgeY = cell.Height - badgeH - 3;

                    using var brush = new SolidBrush(badgeBg);
                    e.Graphics.FillRectangle(brush, badgeX, badgeY, badgeW, badgeH);

                    using var fgBrush = new SolidBrush(badgeFg);
                    e.Graphics.DrawString(badgeText, fontBadge, fgBrush, new PointF(badgeX + 4, badgeY + 1));
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

        int containerWidth = Math.Max(580, _pnlRightDetails.Width - 55);

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
        var box = new Panel
        {
            Width = width,
            AutoSize = true,
            BackColor = Color.White,
            Margin = new Padding(0, 0, 0, 16),
            Padding = new Padding(16)
        };
        box.Paint += (s, e) =>
        {
            using var pen = new Pen(Color.FromArgb(226, 232, 240), 1.2f);
            e.Graphics.DrawRectangle(pen, 0, 0, box.Width - 1, box.Height - 1);

            // Ezkerreko kolore zinta
            Color ribbonColor = kodea.StartsWith("Bazkaria") ? Color.FromArgb(234, 88, 12) : Theme.Purple;
            using var brush = new SolidBrush(ribbonColor);
            e.Graphics.FillRectangle(brush, 0, 0, 5, box.Height);
        };

        int curY = 12;

        // Txanda Goiburua
        var lblTxandaTit = new Label
        {
            Text = $"{izena} ({ordutegia})",
            Font = new Font("Segoe UI", 11.5F, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42),
            Location = new Point(14, curY),
            AutoSize = true
        };

        int paxGuztira = txandakoak.Sum(x => x.PertsonaKopurua);
        var lblTxandaInfo = new Label
        {
            Text = txandakoak.Count > 0 ? $"{txandakoak.Count} erreserba · {paxGuztira} lagun" : "0 erreserba",
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            ForeColor = txandakoak.Count > 0 ? Theme.AccentWine : Color.FromArgb(100, 116, 139),
            Location = new Point(230, curY + 2),
            AutoSize = true
        };

        var btnGehituTxandan = new Button
        {
            Text = "+ Erreserbatu",
            Size = new Size(115, 28),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Location = new Point(box.Width - 135, curY - 2),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(240, 253, 244),
            ForeColor = Color.FromArgb(22, 163, 74),
            Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnGehituTxandan.FlatAppearance.BorderColor = Color.FromArgb(187, 247, 208);
        btnGehituTxandan.Click += (s, e) => IrekiErreserbaBerria(_hautatutakoData, kodea);

        box.Resize += (s, e) =>
        {
            btnGehituTxandan.Location = new Point(box.Width - 135, 10);
        };

        box.Controls.Add(lblTxandaTit);
        box.Controls.Add(lblTxandaInfo);
        box.Controls.Add(btnGehituTxandan);

        curY += 36;

        if (txandakoak.Count == 0)
        {
            var lblHutsa = new Label
            {
                Text = "Ez dago erreserbarik txanda honetan (Mahaia guztiak libre daude).",
                Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(14, curY),
                Size = new Size(width - 30, 24)
            };
            box.Controls.Add(lblHutsa);
            curY += 28;
        }
        else
        {
            foreach (var err in txandakoak)
            {
                var card = SortuErreserbaTxartelTxikia(err, width - 36);
                card.Location = new Point(14, curY);
                box.Controls.Add(card);
                curY += card.Height + 10;
            }
        }

        box.Height = curY + 8;
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
