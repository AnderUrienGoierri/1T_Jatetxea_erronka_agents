using System.Drawing.Drawing2D;
using Jatetxea.Desktop.Models;
using Jatetxea.Desktop.Services;
using Jatetxea.Desktop.UI;

namespace Jatetxea.Desktop.Forms;

public partial class EskaeraForm : BaseTpvForm
{
    private readonly MahaiaDto _mahaia;
    private readonly LangileaDto _langilea;
    private readonly ApiService _api;
    private readonly int _pertsonaKopurua;
    private readonly int? _dagoenekoEskaeraId;

    private List<PlateraDto> _platerak = new();
    private Dictionary<int, int> _stockMap = new();
    private readonly List<CartItem> _karritoa = new();
    private string _aukeratutakoKategoria = "Edariak";
    private string _aukeratutakoEdariAzpikat = "Guztiak";
    private FlowLayoutPanel _pnlEdariAzpikategoriak = null!;

    private static readonly string[] EdariAzpikategoriakLehenetsiak =
    {
        "Urak", "Freskagarriak", "Garagardoak", "Sagardoak",
        "Ardoak", "Kafeak", "Infusioak", "Likoreak",
        "Konbinatuak", "Koktailak"
    };

    /// <summary>
    /// Visual Studio WinForms Designer-erako eraikitzailea
    /// </summary>
    public EskaeraForm()
    {
        InitializeComponent();
        _mahaia = new MahaiaDto { Id = 4, Zenbakia = 4, Edukiera = 4, Egoera = "Okupatuta" };
        _langilea = new LangileaDto { Id = 1, Izena = "Ane G.", Rola = "Zerbitzaria" };
        _api = new ApiService();
        _pertsonaKopurua = 4;
        KonfiguratuInterfazea();
    }

    public EskaeraForm(MahaiaDto mahaia, LangileaDto langilea, ApiService api, int pertsonaKopurua = 2, int? dagoenekoEskaeraId = null)
    {
        InitializeComponent();
        _mahaia = mahaia;
        _langilea = langilea;
        _api = api;
        _pertsonaKopurua = pertsonaKopurua;
        _dagoenekoEskaeraId = dagoenekoEskaeraId;
        KonfiguratuInterfazea();
    }

    private void KonfiguratuInterfazea()
    {
        Text = _dagoenekoEskaeraId.HasValue
            ? $"T1 Jatetxea - Platerak Gehitu - Mahaia {_mahaia.Zenbakia}"
            : $"T1 Jatetxea - Komanda Berria - Mahaia {_mahaia.Zenbakia}";
        lblMahaiaTit.Text = $"T1 Jatetxea - Mahaia {_mahaia.Zenbakia}";
        lblZerbitzaria.Text = $"Zerbitzaria: {_langilea.Izena} ({_pertsonaKopurua} pax)";
        lblKomandaTit.Text = $"ESKAERA - Mahaia {_mahaia.Zenbakia}";

        if (_dagoenekoEskaeraId.HasValue)
        {
            btnBidali.Text = "PLATERAK GEHITU";
        }

        btnAtzera.Click += (s, e) => Close();
        btnBidali.Click += async (s, e) => await EskaeraBidaliAsync();

        // Barrako langileen rol mugaketa: soilik edariak, kafeak eta barrako produktuak
        bool isBarrakoa = _langilea.Rola.Contains("Barra", StringComparison.OrdinalIgnoreCase);
        if (isBarrakoa)
        {
            btnKatHastekoak.Visible = false;
            btnKatHaragiak.Visible = false;
            btnKatGuztiak.Visible = false;
            btnKatEdariak.Text = "Edariak";
            btnKatPostreak.Text = "Postreak & Kafeak";
            lblZerbitzaria.Text = $"Barrakoa: {_langilea.Izena} (Edariak & Kafeak soilik)";
            lblMahaiaTit.Text = $"T1 Jatetxea (Barra TPV) - Mahaia {_mahaia.Zenbakia}";
        }

        // Goiko barra eta diseinua modu dinamikoan egokitu (800x500 -> 4K)
        pnlTopBar.Resize += (s, e) => EgokituResponsiveDiseinua();
        Resize += (s, e) => EgokituResponsiveDiseinua();
        splitMain.SplitterMoved += (s, e) => EgokituKarritoEtaPlaterak();
        pnlKarritoa.Resize += (s, e) => EgokituKarritoLerroenZabalera();
        pnlPlaterak.Resize += (s, e) => EgokituPlaterTxartelenZabalera();

        // Kategoriak botoien ekitaldiak
        btnKatEdariak.Click += (s, e) => AldatuKategoria("Edariak");
        btnKatHastekoak.Click += (s, e) => AldatuKategoria("Hastekoak");
        btnKatHaragiak.Click += (s, e) => AldatuKategoria("Haragiak");
        btnKatPostreak.Click += (s, e) => AldatuKategoria("Postreak");
        btnKatGuztiak.Click += (s, e) => AldatuKategoria("Guztiak");

        // Edarien azpikategorien barra (Urak, Freskagarriak, Garagardoak, Ardoak, Kafeak...)
        // WrapContents = true + AutoSize = true -> scrollbarrik gabe eta 1-2 lerro garbitan guztiak ikusgai!
        _pnlEdariAzpikategoriak = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            AutoScroll = false,
            WrapContents = true,
            BackColor = Theme.WarmLinen,
            Padding = new Padding(4, 8, 4, 4),
            Visible = true
        };
        splitMain.Panel1.Controls.Add(_pnlEdariAzpikategoriak);
        // Z-ordena DockStyle.Top-erako: pnlKategoriak goian -> _pnlEdariAzpikategoriak -> pnlKategoriaIzenburua -> pnlPlaterak (Fill)
        splitMain.Panel1.Controls.SetChildIndex(pnlPlaterak, 0);
        splitMain.Panel1.Controls.SetChildIndex(pnlKategoriaIzenburua, 1);
        splitMain.Panel1.Controls.SetChildIndex(_pnlEdariAzpikategoriak, 2);
        splitMain.Panel1.Controls.SetChildIndex(pnlKategoriak, 3);

        // T1 Warm Hospitality v2.0 paleta + parpadeorik gabeko DoubleBuffering aplikatu
        Theme.ApplyWarmHospitalityTheme(this);
        BackColor = Theme.WarmLinen;
        pnlTopBar.BackColor = Theme.MarbleWhite;
        pnlKategoriak.BackColor = Theme.MarbleWhite;
        _pnlEdariAzpikategoriak.BackColor = Theme.WarmLinen;
        pnlKategoriaIzenburua.BackColor = Theme.WarmLinen;
        pnlPlaterak.BackColor = Theme.WarmLinen;
        pnlKomandaGoiburua.BackColor = Theme.MarbleWhite;
        pnlKarritoa.BackColor = Theme.MarbleWhite;
        pnlLaburpena.BackColor = Theme.MarbleWhite;
        lblMahaiaTit.Font = Theme.DisplayFont(15f);
        lblMahaiaTit.ForeColor = Theme.ChiantiPrimary;
        lblKategoriaAktiboa.Font = Theme.BodyFont(14.5f, bold: true);
        lblKategoriaAktiboa.ForeColor = Theme.EspressoInk;
        lblKategoriaAktiboa.AutoSize = true;
        lblGuztira.Font = Theme.MonoFont(13.5f, bold: true);
        lblGuztira.ForeColor = Theme.ChiantiPrimary;
        lblAzpitotala.Font = Theme.MonoFont(10f, bold: false);
        lblBez.Font = Theme.MonoFont(10f, bold: false);
        Theme.StyleButton(btnAtzera, Theme.StoneMist, Theme.EspressoInk, Theme.WarmTaupe, radius: 10, fontSize: 10f);
        Theme.StyleButton(btnBidali, Theme.ChiantiPrimary, Theme.MarbleWhite, null, radius: 12, fontSize: 11f);

        // Goiko barraren marra banatzailea
        pnlTopBar.Paint += (s, e) =>
        {
            using var pen = new Pen(Theme.WarmTaupe, 1.2f);
            e.Graphics.DrawLine(pen, 0, pnlTopBar.Height - 1, pnlTopBar.Width, pnlTopBar.Height - 1);
        };

        // Komandaren goiburuko marra banatzailea
        pnlKomandaGoiburua.Paint += (s, e) =>
        {
            using var pen = new Pen(Theme.WarmTaupe, 1.2f);
            e.Graphics.DrawLine(pen, 0, pnlKomandaGoiburua.Height - 1, pnlKomandaGoiburua.Width, pnlKomandaGoiburua.Height - 1);
        };

        // Laburpenaren goiko marra banatzailea
        pnlLaburpena.Paint += (s, e) =>
        {
            using var pen = new Pen(Theme.GoldBorder, 1.4f);
            e.Graphics.DrawLine(pen, 0, 0, pnlLaburpena.Width, 0);
        };

        pnlKarritoa.BringToFront();

        Load += async (s, e) =>
        {
            EgokituResponsiveDiseinua();
            await KargatuDatuakAsync();
        };

        Theme.AttachMainFormDpiAdaptation(this, () =>
        {
            EgokituResponsiveDiseinua();
            AldatuKategoria(_aukeratutakoKategoria);
            EguneratuKarritoBistaratzea();
        });
    }

    private void EgokituResponsiveDiseinua()
    {
        int S(int px) => Theme.S(this, px);
        bool estua = ClientSize.Width < S(1040);

        pnlTopBar.Height = ClientSize.Height < S(580) ? S(64) : S(80);
        int yBtn = (pnlTopBar.Height - S(42)) / 2;
        btnAtzera.SetBounds(S(16), yBtn, estua ? S(130) : S(156), S(42));

        lblMahaiaTit.Font = Theme.DisplayFont(this, estua ? 12.5f : 15f);
        lblMahaiaTit.SetBounds(btnAtzera.Right + S(14), yBtn, estua ? S(280) : S(480), S(42));

        lblZerbitzaria.Visible = ClientSize.Width >= S(860);
        if (lblZerbitzaria.Visible)
        {
            int zLeft = lblMahaiaTit.Right + S(10);
            int zWidth = Math.Max(S(180), pnlTopBar.Width - zLeft - S(20));
            lblZerbitzaria.SetBounds(zLeft, yBtn, zWidth, S(42));
        }

        // Goiko kategoria eta azpikategoria barrak DPI-rekin eskalatuta
        pnlKategoriak.Height = S(54);
        pnlKategoriak.Padding = new Padding(S(8), S(6), S(8), S(6));
        _pnlEdariAzpikategoriak.Padding = new Padding(S(4), S(10), S(4), S(4));
        _pnlEdariAzpikategoriak.MaximumSize = new Size(Math.Max(S(300), splitMain.Panel1.ClientSize.Width - S(24)), 0);
        pnlKategoriaIzenburua.Height = S(46);
        pnlKomandaGoiburua.Height = S(54);

        // Kategoria botoiak testuaren zabalera zehatzarekin (ez mozteko!)
        Button[] katBtns = { btnKatEdariak, btnKatHastekoak, btnKatHaragiak, btnKatPostreak, btnKatGuztiak };
        foreach (var b in katBtns)
        {
            var sz = TextRenderer.MeasureText(b.Text, b.Font);
            b.AutoSize = false;
            b.Size = new Size(sz.Width + S(32), S(38));
            b.Margin = new Padding(0, 0, S(8), 0);
        }

        // SplitContainer proportzionala (ezkerra 66% / eskuina 34%)
        splitMain.Panel1MinSize = S(360);
        splitMain.Panel2MinSize = S(290);
        if (splitMain.Width > splitMain.Panel1MinSize + splitMain.Panel2MinSize + S(20))
        {
            int rightW = Math.Clamp((int)(splitMain.Width * 0.32), S(320), S(460));
            int targetDist = Math.Clamp(splitMain.Width - rightW, splitMain.Panel1MinSize, splitMain.Width - splitMain.Panel2MinSize);
            if (Math.Abs(splitMain.SplitterDistance - targetDist) > 12)
            {
                try { splitMain.SplitterDistance = targetDist; } catch { }
            }
        }

        // Laburpena DPI-rekin eskalatuta
        bool baxua = ClientSize.Height < S(600);
        pnlLaburpena.Height = baxua ? S(170) : S(198);
        int labW = Math.Max(S(240), pnlLaburpena.ClientSize.Width - S(8));
        lblAzpitotala.SetBounds(S(4), S(8), labW, S(28));
        lblBez.SetBounds(S(4), S(38), labW, S(28));
        lblGuztira.SetBounds(S(4), S(70), labW, S(38));
        int btnH = baxua ? S(46) : S(54);
        btnBidali.SetBounds(S(4), pnlLaburpena.Height - btnH - S(10), labW, btnH);

        EgokituKarritoEtaPlaterak();
    }

    private void EgokituKarritoEtaPlaterak()
    {
        EgokituKarritoLerroenZabalera();
        EgokituPlaterTxartelenZabalera();
    }

    private void EgokituPlaterTxartelenZabalera()
    {
        int S(int px) => Theme.S(this, px);
        int availW = pnlPlaterak.ClientSize.Width - S(28);
        if (availW < S(220)) return;
        int cols = Math.Max(2, availW / S(215));
        if (availW < S(480)) cols = 2;
        int margin = S(7);
        int cardW = Math.Clamp((availW - cols * margin * 2) / cols, S(180), S(265));
        int cardH = S(188);

        pnlPlaterak.SuspendLayout();
        foreach (Control c in pnlPlaterak.Controls)
        {
            if (c is Panel card)
            {
                card.Size = new Size(cardW, cardH);
                card.Margin = new Padding(margin);
                card.Invalidate();
            }
        }
        pnlPlaterak.ResumeLayout(true);
    }

    private void EgokituKarritoLerroenZabalera()
    {
        int S(int px) => Theme.S(this, px);
        int w = Math.Max(S(260), pnlKarritoa.ClientSize.Width - S(24));
        pnlKarritoa.SuspendLayout();
        foreach (Control c in pnlKarritoa.Controls)
        {
            if (c is Panel row)
            {
                row.Width = w;
                foreach (Control child in row.Controls)
                {
                    if (child.Tag is not string tag) continue;
                    switch (tag)
                    {
                        case "del":
                            child.Location = new Point(w - child.Width - S(8), S(8));
                            break;
                        case "izena":
                            child.Width = Math.Max(S(110), w - child.Left - S(42));
                            break;
                        case "totala":
                            child.Location = new Point(w - child.Width - S(12), child.Top);
                            break;
                        case "oharra-btn":
                            child.Width = Math.Max(S(180), w - S(20));
                            break;
                        case "oharra-pnl":
                            child.Width = Math.Max(S(180), w - S(20));
                            foreach (Control pc in child.Controls)
                            {
                                if (pc is Button bk && bk.Text == "X") bk.Location = new Point(child.Width - bk.Width - S(6), (child.Height - bk.Height) / 2);
                                else if (pc is Button ba && ba.Text == "Aldatu") ba.Location = new Point(child.Width - ba.Width - S(38), (child.Height - ba.Height) / 2);
                                else if (pc is Label lo) lo.Width = Math.Max(S(80), child.Width - S(112));
                            }
                            break;
                    }
                }
                row.Invalidate();
            }
        }
        pnlKarritoa.ResumeLayout(true);
    }

    private void AldatuKategoria(string kategoria)
    {
        _aukeratutakoKategoria = kategoria;
        bool isEdariak = kategoria.Equals("Edariak", StringComparison.OrdinalIgnoreCase);

        if (isEdariak)
        {
            _pnlEdariAzpikategoriak.Visible = true;
            BerrituEdariAzpikategoriak();
            lblKategoriaAktiboa.Text = _aukeratutakoEdariAzpikat.Equals("Guztiak", StringComparison.OrdinalIgnoreCase)
                ? "EDARIAK"
                : $"EDARIAK · {_aukeratutakoEdariAzpikat.ToUpperInvariant()}";
        }
        else
        {
            _pnlEdariAzpikategoriak.Visible = false;
            lblKategoriaAktiboa.Text = kategoria.ToUpperInvariant();
        }

        EguneratuKategoriaBotoiak();
        IragaziPlaterak(_aukeratutakoKategoria);
    }

    private void BerrituEdariAzpikategoriak()
    {
        // API-tik datozen edari kategoriak bildu (ordenatuta + lehenetsitakoak)
        var aurkituak = _platerak
            .Select(p => p.Kategoria?.Trim() ?? "")
            .Where(k => !string.IsNullOrWhiteSpace(k) && DaEdariKategoria(k))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var ordenatuta = new List<string> { "Guztiak" };
        foreach (var kat in EdariAzpikategoriakLehenetsiak)
        {
            if (aurkituak.Any(a => a.Equals(kat, StringComparison.OrdinalIgnoreCase)) || aurkituak.Count == 0)
            {
                ordenatuta.Add(kat);
            }
        }
        foreach (var kat in aurkituak)
        {
            if (!ordenatuta.Any(o => o.Equals(kat, StringComparison.OrdinalIgnoreCase)))
            {
                ordenatuta.Add(kat);
            }
        }

        if (!ordenatuta.Any(o => o.Equals(_aukeratutakoEdariAzpikat, StringComparison.OrdinalIgnoreCase)))
        {
            _aukeratutakoEdariAzpikat = "Guztiak";
        }

        UiPerf.RunFlickerFree(_pnlEdariAzpikategoriak, () =>
        {
            _pnlEdariAzpikategoriak.Controls.Clear();
            foreach (var azpikat in ordenatuta)
            {
                bool aktiboa = azpikat.Equals(_aukeratutakoEdariAzpikat, StringComparison.OrdinalIgnoreCase);
                int kop = azpikat == "Guztiak"
                    ? _platerak.Count(p => DaEdariKategoria(p.Kategoria))
                    : _platerak.Count(p => string.Equals(p.Kategoria?.Trim(), azpikat, StringComparison.OrdinalIgnoreCase));

                string etiketa = kop > 0 ? $"{azpikat} ({kop})" : azpikat;
                using var pillFont = Theme.BodyFont(this, 9f, bold: aktiboa);
                var sz = TextRenderer.MeasureText(etiketa, pillFont);
                int btnW = Math.Max(Theme.S(this, 86), sz.Width + Theme.S(this, 28));

                var btn = new Button
                {
                    Text = etiketa,
                    Size = new Size(btnW, Theme.S(this, 36)),
                    Margin = new Padding(0, 0, Theme.S(this, 8), Theme.S(this, 8))
                };
                Theme.StyleButton(
                    btn,
                    aktiboa ? Theme.ChiantiPrimary : Theme.MarbleWhite,
                    aktiboa ? Theme.MarbleWhite : Theme.EspressoInk,
                    aktiboa ? null : Theme.WarmTaupe,
                    radius: 16,
                    fontSize: 9f,
                    bold: aktiboa);

                var captured = azpikat;
                btn.Click += (s, e) =>
                {
                    _aukeratutakoEdariAzpikat = captured;
                    AldatuKategoria("Edariak");
                };

                _pnlEdariAzpikategoriak.Controls.Add(btn);
            }
        }, preserveScroll: true);
    }

    private static bool DaEdariKategoria(string? kategoria)
    {
        if (string.IsNullOrWhiteSpace(kategoria)) return false;
        string k = kategoria.Trim().ToLowerInvariant();
        if (k.StartsWith("edari")) return true;
        return EdariAzpikategoriakLehenetsiak.Any(e => e.Equals(k, StringComparison.OrdinalIgnoreCase));
    }

    private void EguneratuKategoriaBotoiak()
    {
        var btns = new[]
        {
            (btnKatEdariak, "Edariak"),
            (btnKatHastekoak, "Hastekoak"),
            (btnKatHaragiak, "Haragiak"),
            (btnKatPostreak, "Postreak"),
            (btnKatGuztiak, "Guztiak")
        };

        foreach (var (btn, kat) in btns)
        {
            bool isAktiboa = kat.Equals(_aukeratutakoKategoria, StringComparison.OrdinalIgnoreCase);
            btn.Font = Theme.BodyFont(this, 10.5f, bold: isAktiboa);
            btn.ForeColor = isAktiboa ? Theme.ChiantiPrimary : Theme.WarmSlate;
            btn.BackColor = isAktiboa ? Theme.ChiantiTint : Theme.MarbleWhite;
        }
    }

    private async Task KargatuDatuakAsync()
    {
        try
        {
            _platerak = await _api.GetPlaterakAsync();
            _stockMap = await _api.GetPlaterenStockAhalmenaAsync();
        }
        catch
        {
            _platerak = new List<PlateraDto>();
        }

        foreach (var p in _platerak)
        {
            if (_stockMap.TryGetValue(p.Id, out int ahalmena))
            {
                p.StockAhalmena = ahalmena;
            }
        }

        AldatuKategoria("Edariak");
        EguneratuKarritoBistaratzea();
    }

    private void IragaziPlaterak(string kategoria)
    {
        bool isBarrakoa = _langilea.Rola.Contains("Barra", StringComparison.OrdinalIgnoreCase);
        var zerrenda = _platerak.Where(p => KategoriaBatDator(p.Kategoria, kategoria)).ToList();

        if (isBarrakoa)
        {
            // Barrako langileek ezin dituzte sukaldeko janariak ikusi edo eskatu, soilik edariak, kafeak eta postreak
            zerrenda = zerrenda.Where(p =>
                DaEdariKategoria(p.Kategoria) ||
                p.Kategoria.Contains("Postre", StringComparison.OrdinalIgnoreCase) ||
                p.Izena.Contains("Kafe", StringComparison.OrdinalIgnoreCase) ||
                p.Izena.Contains("Ura", StringComparison.OrdinalIgnoreCase) ||
                p.Izena.Contains("Ardo", StringComparison.OrdinalIgnoreCase) ||
                p.Izena.Contains("Garagardo", StringComparison.OrdinalIgnoreCase) ||
                p.Izena.Contains("Kopa", StringComparison.OrdinalIgnoreCase) ||
                p.Izena.Contains("Infusio", StringComparison.OrdinalIgnoreCase) ||
                p.Izena.Contains("Trago", StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }

        // Edariak atalean azpikategoria hautatuta badago, horren arabera iragazi
        if (kategoria.Equals("Edariak", StringComparison.OrdinalIgnoreCase) &&
            !_aukeratutakoEdariAzpikat.Equals("Guztiak", StringComparison.OrdinalIgnoreCase))
        {
            zerrenda = zerrenda
                .Where(p => string.Equals(p.Kategoria?.Trim(), _aukeratutakoEdariAzpikat, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        UiPerf.RunFlickerFree(pnlPlaterak, () =>
        {
            pnlPlaterak.Controls.Clear();
            foreach (var p in zerrenda)
            {
                var card = SortuPlaterTxartela(p);
                pnlPlaterak.Controls.Add(card);
            }
        }, preserveScroll: false);

        EgokituPlaterTxartelenZabalera();
    }

    private static bool KategoriaBatDator(string platerKategoria, string aukeratutakoKat)
    {
        if (string.IsNullOrWhiteSpace(aukeratutakoKat) || aukeratutakoKat.Equals("Guztiak", StringComparison.OrdinalIgnoreCase))
            return true;

        string pKat = platerKategoria.Trim().ToLowerInvariant();
        string aKat = aukeratutakoKat.Trim().ToLowerInvariant();

        if (aKat.StartsWith("edari"))
            return DaEdariKategoria(platerKategoria);

        if (aKat.StartsWith("hastek") || aKat.StartsWith("sarrera") || aKat.StartsWith("lehen"))
            return pKat.Contains("sarrera") || pKat.Contains("lehen") || pKat.Contains("hastek");

        if (aKat.StartsWith("haragi") || aKat.StartsWith("arrain") || aKat.StartsWith("bigarren"))
            return pKat.Contains("bigarren") || pKat.Contains("haragi") || pKat.Contains("arrain");

        if (aKat.StartsWith("postre"))
            return pKat.StartsWith("postre");

        return pKat.Contains(aKat) || aKat.Contains(pKat);
    }

    /// <summary>
    /// TPV profesional estiloko argazkidun produktu-txartela (irudia goian + kolore-banda izena eta prezioarekin behean).
    /// </summary>
    private Control SortuPlaterTxartela(PlateraDto platera)
    {
        int S(int px) => Theme.S(this, px);
        var img = ProductImageCache.GetImage(platera);
        var (badgeText, bandBg) = LortuPlaterTxartelKolorea(platera);
        bool isHovered = false;

        var card = new Panel
        {
            Width = S(215),
            Height = S(188),
            Margin = new Padding(S(7)),
            BackColor = Theme.WarmLinen,
            Cursor = Cursors.Hand
        };

        card.MouseEnter += (_, _) => { isHovered = true; card.Invalidate(); };
        card.MouseLeave += (_, _) => { isHovered = false; card.Invalidate(); };
        card.Click += (_, _) => GehituKarritora(platera);

        card.Paint += (_, e) =>
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int w = card.Width;
            int h = card.Height;
            int radius = S(12);
            int imgH = (int)(h * 0.60f);
            var fullRect = new RectangleF(1f, 1f, w - 2.5f, h - 2.5f);

            using var clipPath = Theme.CreateRoundedRectPath(fullRect, radius);
            var prevClip = g.Clip;
            g.SetClip(clipPath);

            // 1. Goiko argazkia (object-fit: cover)
            var imgRect = new RectangleF(1f, 1f, w - 2f, imgH);
            if (img != null)
            {
                ProductImageCache.DrawImageCover(g, img, imgRect);
            }
            else
            {
                using var phBrush = new SolidBrush(Theme.StoneMist);
                g.FillRectangle(phBrush, imgRect);
                using var phFont = Theme.DisplayFont(this, 18f);
                TextRenderer.DrawText(g, badgeText, phFont, Rectangle.Round(imgRect), Theme.WarmSlate,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }

            // 2. Beheko kolore-banda (izena + prezioa kontraste altuarekin)
            var bandRect = new RectangleF(1f, imgH, w - 2f, h - imgH);
            using (var bandBrush = new SolidBrush(isHovered ? ControlPaint.Light(bandBg, 0.08f) : bandBg))
            {
                g.FillRectangle(bandBrush, bandRect);
            }

            // 3. Goiko eskuineko "+" pilula azkarra (bektore-lerroekin matematikoki zentratuta)
            int plusSz = S(28);
            var plusRect = new Rectangle(w - plusSz - S(8), S(8), plusSz, plusSz);
            using (var plusBg = new SolidBrush(Color.FromArgb(isHovered ? 245 : 220, Theme.MarbleWhite)))
            {
                g.FillEllipse(plusBg, plusRect);
            }
            float cx = plusRect.X + plusRect.Width / 2f;
            float cy = plusRect.Y + plusRect.Height / 2f;
            float arm = plusSz * 0.22f;
            using (var plusPen = new Pen(Theme.ChiantiPrimary, Math.Max(2f, S(2))))
            {
                plusPen.StartCap = LineCap.Round;
                plusPen.EndCap = LineCap.Round;
                g.DrawLine(plusPen, cx - arm, cy, cx + arm, cy);
                g.DrawLine(plusPen, cx, cy - arm, cx, cy + arm);
            }

            // 4. Produktuaren izena eta prezioa beheko bandan
            int padX = S(10);
            var nameRect = new Rectangle(padX, imgH + S(6), w - padX * 2, S(38));
            using (var nameFont = Theme.BodyFont(this, 9.2f, bold: true))
            {
                TextRenderer.DrawText(g, platera.Izena, nameFont, nameRect, Theme.MarbleWhite,
                    TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis);
            }

            var priceRect = new Rectangle(padX, h - S(28), w - padX * 2, S(24));
            using (var priceFont = Theme.MonoFont(this, 10.5f, bold: true))
            {
                TextRenderer.DrawText(g, $"{platera.Prezioa:F2} €", priceFont, priceRect, Theme.GoldTint,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
            }

            g.Clip = prevClip;

            // 5. Kanpoko ertza (hover-ean urrezkoa)
            using var borderPen = new Pen(isHovered ? Theme.GoldBorder : Theme.WarmTaupe, isHovered ? 2f : 1.2f);
            g.DrawPath(borderPen, clipPath);
        };

        return card;
    }

    private static (string Code, Color BandBg) LortuPlaterTxartelKolorea(PlateraDto p)
    {
        string kat = p.Kategoria.ToLowerInvariant();
        string izena = p.Izena.ToLowerInvariant();

        if (kat.Contains("ardo") || izena.Contains("ardo") || izena.Contains("txakolin"))
            return ("AR", Theme.ChiantiPrimary);
        if (kat.Contains("garagardo") || kat.Contains("sagardo"))
            return ("GA", Theme.AntiqueGold);
        if (kat.Contains("urak") || kat.Contains("infusio"))
            return ("UR", Theme.Info);
        if (kat.Contains("freskagarri") || kat.Contains("koktail"))
            return ("FR", Theme.Warning);
        if (kat.Contains("kafe"))
            return ("KA", Theme.EspressoInk);
        if (kat.Contains("konbinatu") || kat.Contains("likore"))
            return ("KO", Theme.Purple);
        if (kat.Contains("sarrera") || kat.Contains("lehen") || kat.Contains("hastek"))
            return ("HA", Theme.Success);
        if (kat.Contains("bigarren") || kat.Contains("haragi") || kat.Contains("arrain"))
            return ("BI", Theme.BurgundyDark);
        if (kat.Contains("postre"))
            return ("PO", Color.FromArgb(146, 64, 14));

        return ("PL", Theme.EspressoInk);
    }

    private void GehituKarritora(PlateraDto platera)
    {
        // Barrako langileen segurtasun egiaztapena
        bool isBarrakoa = _langilea.Rola.Contains("Barra", StringComparison.OrdinalIgnoreCase);
        if (isBarrakoa &&
            !platera.Kategoria.Contains("Edari", StringComparison.OrdinalIgnoreCase) &&
            !platera.Kategoria.Contains("Postre", StringComparison.OrdinalIgnoreCase) &&
            !platera.Izena.Contains("Kafe", StringComparison.OrdinalIgnoreCase) &&
            !platera.Izena.Contains("Ura", StringComparison.OrdinalIgnoreCase) &&
            !platera.Izena.Contains("Ardo", StringComparison.OrdinalIgnoreCase))
        {
            MessageBox.Show(
                "Barrako langileek ezin dituzte sukaldeko janariak eskatu.\nSoilik edariak, kafeak eta barrako produktuak baimenduta daude.",
                "Baimen Mugatua - Barra TPV",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        // Oharrik gabeko lerro berdina badago, kopurua gehitu.
        // Oharra duen lerroa badago, lerro berri bat sortu (oharra desberdina izan daitekeelako).
        var dagoena = _karritoa.FirstOrDefault(c => c.Platera.Id == platera.Id && string.IsNullOrWhiteSpace(c.Oharrak));
        if (dagoena != null)
        {
            dagoena.Kopurua++;
        }
        else
        {
            _karritoa.Add(new CartItem(platera, 1));
        }

        EguneratuKarritoBistaratzea();
    }

    private void EguneratuKarritoBistaratzea()
    {
        decimal azpitotala = 0;

        UiPerf.RunFlickerFree(pnlKarritoa, () =>
        {
            pnlKarritoa.Controls.Clear();

            foreach (var item in _karritoa)
            {
                azpitotala += item.Azpitotala;
                var row = SortuKarritoLerroa(item);
                pnlKarritoa.Controls.Add(row);
            }
        }, preserveScroll: true);

        EgokituKarritoLerroenZabalera();

        // Laburpena kalkulatu (%10 BEZ barne)
        decimal bez = Math.Round(azpitotala * 0.10m, 2);
        decimal guztira = azpitotala + bez;

        lblAzpitotala.Text = $"Azpitotala:  {azpitotala:F2} €";
        lblBez.Text = $"BEZ (%10):  {bez:F2} €";
        lblGuztira.Text = $"Guztira:  {guztira:F2} €";

        btnBidali.Enabled = _karritoa.Count > 0;
    }

    /// <summary>
    /// Zirriborroko (5-eskaera_produktuak_interfaazea.jpg) karrito txartela eraikitzen du oharrekin
    /// </summary>
    private Control SortuKarritoLerroa(CartItem item)
    {
        int S(int px) => Theme.S(this, px);
        bool hasOharra = !string.IsNullOrWhiteSpace(item.Oharrak);
        int cardHeight = hasOharra ? S(132) : S(116);
        int rowW = Math.Max(S(280), pnlKarritoa.ClientSize.Width - S(24));
        var thumbImg = ProductImageCache.GetImage(item.Platera);

        var row = new Panel
        {
            Width = rowW,
            Height = cardHeight,
            Margin = new Padding(0, 0, 0, S(10)),
            BackColor = Theme.MarbleWhite
        };

        row.Paint += (s, e) =>
        {
            Theme.DrawRoundedCard(e.Graphics, new Rectangle(0, 0, row.Width, row.Height), 10,
                Theme.WarmLinen, hasOharra ? Theme.GoldBorder : Theme.WarmTaupe, 1.2f);

            // Miniatura biribildua ezkerrean (TPV profesional estilora)
            if (thumbImg != null)
            {
                var thumbRect = new RectangleF(S(10), S(12), S(48), S(48));
                using var path = Theme.CreateRoundedRectPath(thumbRect, S(8));
                var prevClip = e.Graphics.Clip;
                e.Graphics.SetClip(path);
                ProductImageCache.DrawImageCover(e.Graphics, thumbImg, thumbRect);
                e.Graphics.Clip = prevClip;
            }
        };

        int textLeft = thumbImg != null ? S(66) : S(12);

        // 1. Goiko lerroa: Izena soilik eta Kendu [✕]
        var lblIzena = new Label
        {
            Text = item.Platera.Izena,
            Font = Theme.BodyFont(9.5f, bold: true),
            ForeColor = Theme.EspressoInk,
            BackColor = Theme.WarmLinen,
            Location = new Point(textLeft, S(8)),
            Size = new Size(Math.Max(S(120), rowW - textLeft - S(42)), S(24)),
            AutoEllipsis = true,
            TextAlign = ContentAlignment.MiddleLeft,
            Tag = "izena",
            UseCompatibleTextRendering = false
        };
        row.Controls.Add(lblIzena);

        var btnDel = new Button
        {
            Text = "✕",
            Font = Theme.BodyFont(9.5f, bold: true),
            ForeColor = Theme.StoneMuted,
            BackColor = Theme.WarmLinen,
            Location = new Point(rowW - S(36), S(6)),
            Size = new Size(S(28), S(26)),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Tag = "del",
            UseCompatibleTextRendering = false
        };
        btnDel.FlatAppearance.BorderSize = 0;
        btnDel.Click += (s, e) =>
        {
            _karritoa.Remove(item);
            EguneratuKarritoBistaratzea();
        };
        row.Controls.Add(btnDel);

        // 2. Bigarren lerroa: Stepper [-] [Kop] [+] eta eskuinean Azpitotala (miniaturaren eskuinean!)
        int stepY = S(36);
        int stepH = S(30);
        int stepBtnW = S(32);

        var btnMinus = new Button
        {
            Text = "-",
            Location = new Point(textLeft, stepY),
            Size = new Size(stepBtnW, stepH)
        };
        Theme.StyleButton(btnMinus, Theme.MarbleWhite, Theme.EspressoInk, Theme.WarmTaupe, radius: 6, fontSize: 10.5f);
        btnMinus.Click += (s, e) =>
        {
            if (item.Kopurua > 1) item.Kopurua--;
            else _karritoa.Remove(item);
            EguneratuKarritoBistaratzea();
        };
        row.Controls.Add(btnMinus);

        var lblKop = new Label
        {
            Text = item.Kopurua.ToString(),
            Font = Theme.MonoFont(10.5f, bold: true),
            ForeColor = Theme.EspressoInk,
            BackColor = Theme.WarmLinen,
            Location = new Point(btnMinus.Right + S(2), stepY),
            Size = new Size(S(32), stepH),
            TextAlign = ContentAlignment.MiddleCenter,
            UseCompatibleTextRendering = false
        };
        row.Controls.Add(lblKop);

        var btnPlus = new Button
        {
            Text = "+",
            Location = new Point(lblKop.Right + S(2), stepY),
            Size = new Size(stepBtnW, stepH)
        };
        Theme.StyleButton(btnPlus, Theme.MarbleWhite, Theme.EspressoInk, Theme.WarmTaupe, radius: 6, fontSize: 10.5f);
        btnPlus.Click += (s, e) =>
        {
            item.Kopurua++;
            EguneratuKarritoBistaratzea();
        };
        row.Controls.Add(btnPlus);

        int totW = S(96);
        var lblLerroTotala = new Label
        {
            Text = $"{item.Azpitotala:F2} €",
            Font = Theme.MonoFont(10.5f, bold: true),
            ForeColor = Theme.ChiantiPrimary,
            BackColor = Theme.WarmLinen,
            Location = new Point(rowW - totW - S(12), stepY),
            Size = new Size(totW, stepH),
            TextAlign = ContentAlignment.MiddleRight,
            Tag = "totala",
            UseCompatibleTextRendering = false
        };
        row.Controls.Add(lblLerroTotala);

        // 3. Beheko lerroa: Oharra gehitu / bistaratu (gainjarpenik gabe!)
        int noteY = S(74);
        int noteW = Math.Max(S(180), rowW - S(20));
        if (!hasOharra)
        {
            var btnGehituOharra = new Button
            {
                Text = "+ Oharra gehitu (gazta gutxi, puntua...)",
                Location = new Point(S(10), noteY),
                Size = new Size(noteW, S(32)),
                Tag = "oharra-btn"
            };
            Theme.StyleButton(btnGehituOharra, Theme.InfoBg, Theme.Info, Theme.InfoBorder, radius: 8, fontSize: 8.8f, bold: false);
            btnGehituOharra.Click += (s, e) => IrekiOharraModala(item);
            row.Controls.Add(btnGehituOharra);
        }
        else
        {
            var pnlOharra = new Panel
            {
                Location = new Point(S(10), noteY),
                Size = new Size(noteW, S(46)),
                BackColor = Theme.GoldTint,
                Tag = "oharra-pnl"
            };
            pnlOharra.Paint += (s, e) =>
            {
                using var p = new Pen(Theme.GoldBorder, 1.1f);
                e.Graphics.DrawRectangle(p, 0, 0, pnlOharra.Width - 1, pnlOharra.Height - 1);
            };

            var lblOharra = new Label
            {
                Text = $"Oharra: {item.Oharrak}",
                Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                ForeColor = Color.FromArgb(146, 64, 14),
                Location = new Point(6, 2),
                Size = new Size(270, 44),
                TextAlign = ContentAlignment.MiddleLeft,
                AutoEllipsis = true,
                Cursor = Cursors.Hand,
                UseCompatibleTextRendering = false
            };
            lblOharra.Click += (s, e) => IrekiOharraModala(item);
            pnlOharra.Controls.Add(lblOharra);

            var btnAldatu = new Button
            {
                Text = "Aldatu",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(2, 132, 199),
                BackColor = Color.White,
                Location = new Point(280, 8),
                Size = new Size(54, 30),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                UseCompatibleTextRendering = false
            };
            btnAldatu.FlatAppearance.BorderColor = Color.FromArgb(252, 211, 77);
            btnAldatu.Click += (s, e) => IrekiOharraModala(item);
            pnlOharra.Controls.Add(btnAldatu);

            var btnKenduOharra = new Button
            {
                Text = "X",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 38, 38),
                BackColor = Color.White,
                Location = new Point(338, 8),
                Size = new Size(26, 30),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                UseCompatibleTextRendering = false
            };
            btnKenduOharra.FlatAppearance.BorderColor = Color.FromArgb(252, 211, 77);
            btnKenduOharra.Click += (s, e) =>
            {
                item.Oharrak = null;
                EguneratuKarritoBistaratzea();
            };
            pnlOharra.Controls.Add(btnKenduOharra);

            row.Controls.Add(pnlOharra);
        }

        return row;
    }

    private void IrekiOharraModala(CartItem item)
    {
        using var modal = new OharraModal(item.Platera.Izena, item.Oharrak);
        if (modal.ShowDialog(this) == DialogResult.OK)
        {
            item.Oharrak = string.IsNullOrWhiteSpace(modal.Oharra) ? null : modal.Oharra.Trim();
            EguneratuKarritoBistaratzea();
        }
    }

    private async Task EskaeraBidaliAsync()
    {
        if (_karritoa.Count == 0) return;

        btnBidali.Enabled = false;
        btnBidali.Text = "Bidaltzen...";

        int eskaeraId;

        if (!_dagoenekoEskaeraId.HasValue)
        {
            var (arrakastaSortu, mezuaSortu, eskaeraSortua) = await _api.SortuEskaeraAsync(_mahaia.Id, _langilea.Id, _pertsonaKopurua);
            if (!arrakastaSortu || eskaeraSortua == null)
            {
                MessageBox.Show($"Ezin izan da komanda sortu: {mezuaSortu}\n\nEgiaztatu API zerbitzaria (http://localhost:5050) martxan dagoela.", "Errorea", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnBidali.Enabled = true;
                btnBidali.Text = "ESKAERA BIDALI";
                return;
            }
            eskaeraId = eskaeraSortua.Id;
        }
        else
        {
            eskaeraId = _dagoenekoEskaeraId.Value;
        }

        bool erroreak = false;
        foreach (var item in _karritoa)
        {
            var (arrakastaLerroa, mezuaLerroa) = await _api.GehituPlateraEskaeraraAsync(eskaeraId, item.Platera.Id, item.Kopurua, item.Oharrak);
            if (!arrakastaLerroa)
            {
                erroreak = true;
                MessageBox.Show($"Ezin izan da '{item.Platera.Izena}' gehitu: {mezuaLerroa}", "Stock edo Datu Errorea", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        btnBidali.Enabled = true;
        btnBidali.Text = "ESKAERA SORTU";

        if (!erroreak)
        {
            MessageBox.Show($"Komanda arrakastaz bidali da Mahaia {_mahaia.Zenbakia}-rentzat!", "Eskaera Bidalita", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}

public class CartItem
{
    public PlateraDto Platera { get; set; }
    public int Kopurua { get; set; }
    public string? Oharrak { get; set; }

    public decimal Azpitotala => Platera.Prezioa * Kopurua;

    public CartItem(PlateraDto platera, int kopurua = 1, string? oharrak = null)
    {
        Platera = platera;
        Kopurua = kopurua;
        Oharrak = oharrak;
    }
}
