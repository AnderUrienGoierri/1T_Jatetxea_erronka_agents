using System.Drawing.Drawing2D;
using Jatetxea.Desktop.Models;
using Jatetxea.Desktop.Services;

namespace Jatetxea.Desktop.Forms;

public partial class EskaeraForm : Form
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
        Text = $"T1 Jatetxea - Komanda Berria - Mahaia {_mahaia.Zenbakia}";
        lblMahaiaTit.Text = $"T1 Jatetxea - Mahaia {_mahaia.Zenbakia}";
        lblZerbitzaria.Text = $"Zerbitzaria: {_langilea.Izena} ({_pertsonaKopurua} pax)";
        lblKomandaTit.Text = $"ESKAERA - Mahaia {_mahaia.Zenbakia}";

        btnAtzera.Click += (s, e) => Close();
        btnBidali.Click += async (s, e) => await EskaeraBidaliAsync();

        // Zerbitzariaren etiketa eskuinean finkatu beti
        pnlTopBar.Resize += (s, e) =>
        {
            lblZerbitzaria.Location = new Point(pnlTopBar.Width - lblZerbitzaria.Width - 24, (pnlTopBar.Height - lblZerbitzaria.Height) / 2);
        };
        lblZerbitzaria.Location = new Point(pnlTopBar.Width - lblZerbitzaria.Width - 24, (pnlTopBar.Height - lblZerbitzaria.Height) / 2);

        // Kategoriak botoien ekitaldiak
        btnKatEdariak.Click += (s, e) => AldatuKategoria("Edariak");
        btnKatHastekoak.Click += (s, e) => AldatuKategoria("Hastekoak");
        btnKatHaragiak.Click += (s, e) => AldatuKategoria("Haragiak");
        btnKatPostreak.Click += (s, e) => AldatuKategoria("Postreak");
        btnKatGuztiak.Click += (s, e) => AldatuKategoria("Guztiak");

        // Goiko barraren marra banatzailea
        pnlTopBar.Paint += (s, e) =>
        {
            using var pen = new Pen(Color.FromArgb(226, 232, 240), 1.5f);
            e.Graphics.DrawLine(pen, 0, pnlTopBar.Height - 1, pnlTopBar.Width, pnlTopBar.Height - 1);
        };

        // Komandaren goiburuko marra banatzailea
        pnlKomandaGoiburua.Paint += (s, e) =>
        {
            using var pen = new Pen(Color.FromArgb(226, 232, 240), 1.5f);
            e.Graphics.DrawLine(pen, 0, pnlKomandaGoiburua.Height - 1, pnlKomandaGoiburua.Width, pnlKomandaGoiburua.Height - 1);
        };

        // Laburpenaren goiko marra banatzailea
        pnlLaburpena.Paint += (s, e) =>
        {
            using var pen = new Pen(Color.FromArgb(226, 232, 240), 1.5f);
            e.Graphics.DrawLine(pen, 0, 0, pnlLaburpena.Width, 0);
        };

        pnlKarritoa.BringToFront();

        Load += async (s, e) => await KargatuDatuakAsync();
    }

    private void AldatuKategoria(string kategoria)
    {
        _aukeratutakoKategoria = kategoria;
        lblKategoriaAktiboa.Text = kategoria.ToUpper();

        EguneratuKategoriaBotoiak();
        IragaziPlaterak(_aukeratutakoKategoria);
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
            btn.Font = new Font("Segoe UI", 10.5F, isAktiboa ? FontStyle.Bold : FontStyle.Regular);
            btn.ForeColor = isAktiboa ? Color.FromArgb(136, 19, 55) : Color.FromArgb(100, 116, 139);
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
        pnlPlaterak.SuspendLayout();
        pnlPlaterak.Controls.Clear();

        var zerrenda = _platerak.Where(p => KategoriaBatDator(p.Kategoria, kategoria)).ToList();

        foreach (var p in zerrenda)
        {
            var card = SortuPlaterTxartela(p);
            pnlPlaterak.Controls.Add(card);
        }

        pnlPlaterak.ResumeLayout(true);
    }

    private static bool KategoriaBatDator(string platerKategoria, string aukeratutakoKat)
    {
        if (string.IsNullOrWhiteSpace(aukeratutakoKat) || aukeratutakoKat.Equals("Guztiak", StringComparison.OrdinalIgnoreCase))
            return true;

        string pKat = platerKategoria.Trim().ToLowerInvariant();
        string aKat = aukeratutakoKat.Trim().ToLowerInvariant();

        if (aKat.StartsWith("edari"))
            return pKat.StartsWith("edari");

        if (aKat.StartsWith("hastek") || aKat.StartsWith("sarrera") || aKat.StartsWith("lehen"))
            return pKat.Contains("sarrera") || pKat.Contains("lehen") || pKat.Contains("hastek");

        if (aKat.StartsWith("haragi") || aKat.StartsWith("arrain") || aKat.StartsWith("bigarren"))
            return pKat.Contains("bigarren") || pKat.Contains("haragi") || pKat.Contains("arrain");

        if (aKat.StartsWith("postre"))
            return pKat.StartsWith("postre");

        return pKat.Contains(aKat) || aKat.Contains(pKat);
    }

    /// <summary>
    /// Zirriborroko produktuen txartela sortzen du (Ikonoa, Izena, Prezioa eta [+] botoia)
    /// <summary>
    /// Zirriborroko (4-eskaera_produktuak_kategoriak_interfazea.jpg) produktu-txartela sortzen du
    /// </summary>
    private Control SortuPlaterTxartela(PlateraDto platera)
    {
        var card = new Panel
        {
            Width = 275,
            Height = 135,
            Margin = new Padding(8),
            BackColor = Color.White,
            Cursor = Cursors.Hand
        };

        // Txartelaren ertza
        card.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var pen = new Pen(Color.FromArgb(203, 213, 225), 1.2f);
            e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
        };

        // Kategoria txapa / ikur laburra
        var (badgeText, badgeBg, badgeFg) = LortuPlaterKategoriaBadge(platera);
        var lblBadge = new Label
        {
            Location = new Point(12, 16),
            Size = new Size(42, 42),
            Text = badgeText,
            Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter,
            BackColor = badgeBg,
            ForeColor = badgeFg,
            UseCompatibleTextRendering = false
        };
        card.Controls.Add(lblBadge);

        // Izena (2-3 lerro har ditzake hitz luzeetan)
        var lblIzena = new Label
        {
            Text = platera.Izena,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 41, 59),
            Location = new Point(62, 10),
            Size = new Size(160, 68),
            AutoEllipsis = true,
            TextAlign = ContentAlignment.TopLeft,
            UseCompatibleTextRendering = false
        };
        card.Controls.Add(lblIzena);

        // Prezioa
        var lblPrezioa = new Label
        {
            Text = $"{platera.Prezioa:F2} €",
            Font = new Font("Segoe UI", 12.5F, FontStyle.Bold),
            ForeColor = Color.FromArgb(136, 19, 55),
            Location = new Point(62, 84),
            Size = new Size(160, 40),
            TextAlign = ContentAlignment.MiddleLeft,
            UseCompatibleTextRendering = false
        };
        card.Controls.Add(lblPrezioa);

        // [+] Botoia
        var btnAdd = new Button
        {
            Text = "+",
            Font = new Font("Segoe UI", 15F, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 41, 59),
            BackColor = Color.FromArgb(248, 250, 252),
            Location = new Point(card.Width - 50, 44),
            Size = new Size(42, 44),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            UseCompatibleTextRendering = false
        };
        btnAdd.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnAdd.Click += (s, e) => GehituKarritora(platera);
        card.Controls.Add(btnAdd);

        // Klik egitean (txartelean bertan)
        void OnCardClick(object? s, EventArgs e) => GehituKarritora(platera);
        card.Click += OnCardClick;
        lblBadge.Click += OnCardClick;
        lblIzena.Click += OnCardClick;
        lblPrezioa.Click += OnCardClick;

        return card;
    }

    private static (string Text, Color Bg, Color Fg) LortuPlaterKategoriaBadge(PlateraDto p)
    {
        string kat = p.Kategoria.ToLowerInvariant();
        string izena = p.Izena.ToLowerInvariant();

        if (kat.Contains("edari") || izena.Contains("ardo") || izena.Contains("sagardo") || izena.Contains("ur") || izena.Contains("kafe"))
            return ("ED", Color.FromArgb(239, 246, 255), Color.FromArgb(37, 99, 235));
        if (kat.Contains("hastek") || kat.Contains("sarrera") || kat.Contains("lehen"))
            return ("HA", Color.FromArgb(240, 253, 244), Color.FromArgb(22, 163, 74));
        if (kat.Contains("haragi") || izena.Contains("txulet") || izena.Contains("arkume"))
            return ("HA", Color.FromArgb(254, 242, 242), Color.FromArgb(220, 38, 38));
        if (kat.Contains("arrain") || izena.Contains("bakailao") || izena.Contains("legatz"))
            return ("AR", Color.FromArgb(240, 249, 255), Color.FromArgb(14, 116, 144));
        if (kat.Contains("postre"))
            return ("PO", Color.FromArgb(254, 243, 199), Color.FromArgb(217, 119, 6));

        return ("PL", Color.FromArgb(241, 245, 249), Color.FromArgb(71, 85, 105));
    }

    private void GehituKarritora(PlateraDto platera)
    {
        var dagoena = _karritoa.FirstOrDefault(c => c.Platera.Id == platera.Id);
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
        pnlKarritoa.SuspendLayout();
        pnlKarritoa.Controls.Clear();

        decimal azpitotala = 0;

        foreach (var item in _karritoa)
        {
            azpitotala += item.Azpitotala;
            var row = SortuKarritoLerroa(item);
            pnlKarritoa.Controls.Add(row);
        }

        pnlKarritoa.ResumeLayout(true);

        // Laburpena kalkulatu (%10 BEZ barne)
        decimal bez = Math.Round(azpitotala * 0.10m, 2);
        decimal guztira = azpitotala + bez;

        lblAzpitotala.Text = $"Azpitotala:                    {azpitotala:F2} €";
        lblBez.Text = $"BEZ (%10):                    {bez:F2} €";
        lblGuztira.Text = $"Guztira:                        {guztira:F2} €";

        btnBidali.Enabled = _karritoa.Count > 0;
    }

    /// <summary>
    /// Zirriborroko (5-eskaera_produktuak_interfaazea.jpg) karrito txartela eraikitzen du
    /// </summary>
    private Control SortuKarritoLerroa(CartItem item)
    {
        bool hasOharra = !string.IsNullOrWhiteSpace(item.Oharrak);
        int cardHeight = hasOharra ? 122 : 92;

        var row = new Panel
        {
            Width = 400,
            Height = cardHeight,
            Margin = new Padding(0, 0, 0, 10),
            BackColor = Color.FromArgb(248, 250, 252)
        };

        row.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var pen = new Pen(Color.FromArgb(226, 232, 240), 1.2f);
            e.Graphics.DrawRectangle(pen, 0, 0, row.Width - 1, row.Height - 1);
        };

        // Goiko lerroa: Izena eta Unitate Prezioa
        var lblIzena = new Label
        {
            Text = $"{item.Platera.Izena} ({item.Platera.Prezioa:F2} €)",
            Font = new Font("Segoe UI", 11F, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 41, 59),
            Location = new Point(10, 8),
            Size = new Size(346, 32),
            AutoEllipsis = true,
            TextAlign = ContentAlignment.MiddleLeft,
            UseCompatibleTextRendering = false
        };
        row.Controls.Add(lblIzena);

        // Stepper: [-]  Kop  [+]  Zenbatekoa
        var btnMinus = new Button
        {
            Text = "-",
            Font = new Font("Segoe UI", 11F, FontStyle.Bold),
            ForeColor = Color.FromArgb(51, 65, 85),
            BackColor = Color.White,
            Location = new Point(10, 44),
            Size = new Size(38, 36),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            UseCompatibleTextRendering = false
        };
        btnMinus.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
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
            Font = new Font("Segoe UI", 12F, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 41, 59),
            Location = new Point(52, 44),
            Size = new Size(38, 36),
            TextAlign = ContentAlignment.MiddleCenter,
            UseCompatibleTextRendering = false
        };
        row.Controls.Add(lblKop);

        var btnPlus = new Button
        {
            Text = "+",
            Font = new Font("Segoe UI", 11F, FontStyle.Bold),
            ForeColor = Color.FromArgb(51, 65, 85),
            BackColor = Color.White,
            Location = new Point(94, 44),
            Size = new Size(38, 36),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            UseCompatibleTextRendering = false
        };
        btnPlus.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnPlus.Click += (s, e) =>
        {
            item.Kopurua++;
            EguneratuKarritoBistaratzea();
        };
        row.Controls.Add(btnPlus);

        var lblLerroTotala = new Label
        {
            Text = $"{item.Azpitotala:F2} €",
            Font = new Font("Segoe UI", 12F, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 41, 59),
            Location = new Point(144, 44),
            Size = new Size(210, 36),
            TextAlign = ContentAlignment.MiddleLeft,
            UseCompatibleTextRendering = false
        };
        row.Controls.Add(lblLerroTotala);

        // Kendu botoia [X]
        var btnDel = new Button
        {
            Text = "X",
            Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
            ForeColor = Color.FromArgb(148, 163, 184),
            BackColor = Color.Transparent,
            Location = new Point(362, 8),
            Size = new Size(30, 30),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            UseCompatibleTextRendering = false
        };
        btnDel.FlatAppearance.BorderSize = 0;
        btnDel.Click += (s, e) =>
        {
            _karritoa.Remove(item);
            EguneratuKarritoBistaratzea();
        };
        row.Controls.Add(btnDel);

        // Oharraren panela (zirriborroko "Tipularik gabe ✏️")
        if (hasOharra)
        {
            var pnlOharra = new Panel
            {
                Location = new Point(10, 84),
                Size = new Size(380, 30),
                BackColor = Color.FromArgb(241, 245, 249)
            };
            pnlOharra.Paint += (s, e) =>
            {
                using var p = new Pen(Color.FromArgb(203, 213, 225), 1f);
                e.Graphics.DrawRectangle(p, 0, 0, pnlOharra.Width - 1, pnlOharra.Height - 1);
            };

            var lblOharra = new Label
            {
                Text = $"{item.Oharrak}",
                Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                ForeColor = Color.FromArgb(71, 85, 105),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(6, 0, 0, 0)
            };
            pnlOharra.Controls.Add(lblOharra);
            row.Controls.Add(pnlOharra);
        }

        return row;
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
                MessageBox.Show($"Ezin izan da komanda sortu: {mezuaSortu}\n\nEgiaztatu API zerbitzaria (http://localhost:5000) martxan dagoela.", "Errorea", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
