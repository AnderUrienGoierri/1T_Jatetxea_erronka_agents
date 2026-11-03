using Jatetxea.Desktop.Models;
using Jatetxea.Desktop.Services;
using Jatetxea.Desktop.UI;

namespace Jatetxea.Desktop.Forms;

public class EskaeraForm : Form
{
    private readonly MahaiaDto _mahaia;
    private readonly LangileaDto _langilea;
    private readonly ApiService _api;
    private readonly int _pertsonaKopurua;
    private readonly int? _dagoenekoEskaeraId;

    private List<PlateraDto> _platerak = new();
    private Dictionary<int, int> _stockMap = new();
    private readonly List<CartItem> _karritoa = new();
    private string _aukeratutakoKategoria = "Guztiak";

    private readonly FlowLayoutPanel _pnlKategoriak;
    private readonly FlowLayoutPanel _pnlPlaterak;
    private readonly FlowLayoutPanel _pnlKarritoa;
    private readonly Label _lblAzpitotala;
    private readonly Label _lblBez;
    private readonly Label _lblGuztira;
    private readonly Button _btnBidali;

    public EskaeraForm(MahaiaDto mahaia, LangileaDto langilea, ApiService api, int pertsonaKopurua = 2, int? dagoenekoEskaeraId = null)
    {
        _mahaia = mahaia;
        _langilea = langilea;
        _api = api;
        _pertsonaKopurua = pertsonaKopurua;
        _dagoenekoEskaeraId = dagoenekoEskaeraId;

        Text = $"T1 Jatetxea - Komanda Berria - Mahaia {_mahaia.Zenbakia}";
        Size = new Size(1180, 750);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Theme.Background;
        MinimumSize = new Size(1000, 650);

        // GOIBURUA (Header bar)
        var pnlHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 65,
            BackColor = Theme.Primary,
            Padding = new Padding(20, 10, 20, 10)
        };
        Controls.Add(pnlHeader);

        var btnAtzera = new Button
        {
            Text = "< ATZERA",
            Font = Theme.FontBodyBold,
            Size = new Size(110, 42),
            Location = new Point(20, 12),
            BackColor = Color.FromArgb(51, 65, 85),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnAtzera.FlatAppearance.BorderSize = 0;
        btnAtzera.Click += (s, e) => Close();
        pnlHeader.Controls.Add(btnAtzera);

        var lblMahaiaInfo = new Label
        {
            Text = $"MAHAIA {_mahaia.Zenbakia}  |  {_pertsonaKopurua} Pertsona  |  {_mahaia.Kokapena}",
            Font = Theme.FontTitle,
            ForeColor = Color.White,
            AutoSize = true,
            Location = new Point(150, 18)
        };
        pnlHeader.Controls.Add(lblMahaiaInfo);

        var lblLangileInfo = new Label
        {
            Text = $"Zerbitzaria: {_langilea.Izena}",
            Font = Theme.FontBodyBold,
            ForeColor = Theme.AccentGold,
            AutoSize = true,
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Location = new Point(pnlHeader.Width - 250, 20)
        };
        pnlHeader.Resize += (s, e) =>
        {
            lblLangileInfo.Location = new Point(pnlHeader.Width - 250, 20);
        };
        pnlHeader.Controls.Add(lblLangileInfo);

        // EDUKI NAGUSIA (Ezkerra Platerak, Eskuina Karritoa)
        var split = new SplitContainer
        {
            Dock = DockStyle.Fill,
            Orientation = Orientation.Vertical,
            SplitterDistance = 750,
            IsSplitterFixed = false,
            BackColor = Theme.Border
        };
        split.Panel1.BackColor = Theme.Background;
        split.Panel2.BackColor = Theme.Surface;
        Controls.Add(split);

        // === EZKERRA: Kategoriak eta Platerak ===
        var ezkerEduki = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(15),
            BackColor = Theme.Background
        };
        split.Panel1.Controls.Add(ezkerEduki);

        _pnlKategoriak = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 52,
            AutoScroll = true,
            WrapContents = false,
            BackColor = Color.Transparent
        };
        ezkerEduki.Controls.Add(_pnlKategoriak);

        _pnlPlaterak = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(0, 10, 0, 0),
            BackColor = Color.Transparent
        };
        ezkerEduki.Controls.Add(_pnlPlaterak);

        // === ESKUINA: Karritoa / Tiketa ===
        var eskuinEduki = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(15),
            BackColor = Theme.Surface
        };
        split.Panel2.Controls.Add(eskuinEduki);

        var lblTiketTitulua = new Label
        {
            Text = _dagoenekoEskaeraId.HasValue ? "Komanda Eguneratua" : "Komanda Berria",
            Font = Theme.FontTitle,
            ForeColor = Theme.TextMain,
            Dock = DockStyle.Top,
            Height = 35
        };
        eskuinEduki.Controls.Add(lblTiketTitulua);

        // Karritoko produktuen panela
        _pnlKarritoa = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            WrapContents = false,
            FlowDirection = FlowDirection.TopDown,
            BackColor = Color.FromArgb(248, 250, 252),
            Padding = new Padding(6)
        };
        eskuinEduki.Controls.Add(_pnlKarritoa);

        // Beheko laburpena eta botoia
        var pnlLaburpena = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 165,
            BackColor = Theme.Surface,
            Padding = new Padding(5, 10, 5, 0)
        };
        eskuinEduki.Controls.Add(pnlLaburpena);

        _lblAzpitotala = new Label
        {
            Text = "Azpitotala: 0.00 €",
            Font = Theme.FontBody,
            ForeColor = Theme.TextMuted,
            Location = new Point(10, 10),
            AutoSize = true
        };
        pnlLaburpena.Controls.Add(_lblAzpitotala);

        _lblBez = new Label
        {
            Text = "BEZ (%10 barne): 0.00 €",
            Font = Theme.FontBody,
            ForeColor = Theme.TextMuted,
            Location = new Point(10, 32),
            AutoSize = true
        };
        pnlLaburpena.Controls.Add(_lblBez);

        _lblGuztira = new Label
        {
            Text = "GUZTIRA: 0.00 €",
            Font = Theme.FontTitle,
            ForeColor = Theme.AccentWine,
            Location = new Point(10, 56),
            AutoSize = true
        };
        pnlLaburpena.Controls.Add(_lblGuztira);

        var btnHustu = new Button
        {
            Text = "Hustu",
            Font = Theme.FontSmall,
            Size = new Size(80, 48),
            Location = new Point(10, 105),
            BackColor = Color.FromArgb(241, 245, 249),
            ForeColor = Theme.TextMuted,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnHustu.FlatAppearance.BorderColor = Theme.Border;
        btnHustu.Click += (s, e) =>
        {
            _karritoa.Clear();
            EguneratuKarritoBistaratzea();
        };
        pnlLaburpena.Controls.Add(btnHustu);

        _btnBidali = new Button
        {
            Text = "ESKAERA BIDALI",
            Font = Theme.FontSubtitle,
            Size = new Size(250, 48),
            Location = new Point(100, 105),
            BackColor = Theme.Success,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        _btnBidali.FlatAppearance.BorderSize = 0;
        _btnBidali.Click += async (s, e) => await EskaeraBidaliAsync();
        pnlLaburpena.Controls.Add(_btnBidali);

        // Karga asinkronoa
        Load += async (s, e) => await KargatuDatuakAsync();
    }

    private async Task KargatuDatuakAsync()
    {
        _platerak = await _api.GetPlaterakAsync();
        _stockMap = await _api.GetPlaterenStockAhalmenaAsync();

        foreach (var p in _platerak)
        {
            if (_stockMap.TryGetValue(p.Id, out int ahalmena))
            {
                p.StockAhalmena = ahalmena;
            }
        }

        EraikiKategoriak();
        IragaziPlaterak(_aukeratutakoKategoria);
        EguneratuKarritoBistaratzea();
    }

    private void EraikiKategoriak()
    {
        _pnlKategoriak.Controls.Clear();
        var kategoriak = new List<string> { "Guztiak" };
        var katZerrenda = _platerak.Select(p => p.Kategoria).Distinct().OrderBy(k => k);
        kategoriak.AddRange(katZerrenda);

        foreach (var kat in kategoriak)
        {
            var btnKat = new Button
            {
                Text = kat,
                Font = Theme.FontBodyBold,
                Height = 40,
                AutoSize = true,
                Margin = new Padding(0, 0, 8, 0),
                BackColor = kat == _aukeratutakoKategoria ? Theme.AccentWine : Color.White,
                ForeColor = kat == _aukeratutakoKategoria ? Color.White : Theme.TextMain,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnKat.FlatAppearance.BorderColor = kat == _aukeratutakoKategoria ? Theme.AccentWine : Theme.Border;
            btnKat.Click += (s, e) =>
            {
                _aukeratutakoKategoria = kat;
                EraikiKategoriak();
                IragaziPlaterak(_aukeratutakoKategoria);
            };
            _pnlKategoriak.Controls.Add(btnKat);
        }
    }

    private void IragaziPlaterak(string kategoria)
    {
        _pnlPlaterak.Controls.Clear();
        var iragaziak = kategoria == "Guztiak" 
            ? _platerak 
            : _platerak.Where(p => p.Kategoria == kategoria).ToList();

        foreach (var platera in iragaziak)
        {
            var card = SortuPlaterTxartela(platera);
            _pnlPlaterak.Controls.Add(card);
        }
    }

    private Control SortuPlaterTxartela(PlateraDto platera)
    {
        var card = new Panel
        {
            Width = 220,
            Height = 150,
            Margin = new Padding(6),
            BackColor = Color.White,
            Padding = new Padding(12)
        };

        var lblIzena = new Label
        {
            Text = platera.Izena,
            Font = Theme.FontBodyBold,
            ForeColor = Theme.TextMain,
            Location = new Point(10, 10),
            Size = new Size(200, 40),
            AutoEllipsis = true
        };
        card.Controls.Add(lblIzena);

        var lblPrezioa = new Label
        {
            Text = $"{platera.Prezioa:F2} €",
            Font = Theme.FontSubtitle,
            ForeColor = Theme.AccentWine,
            Location = new Point(10, 52),
            AutoSize = true
        };
        card.Controls.Add(lblPrezioa);

        // Stock txapa
        int stock = platera.StockAhalmena;
        bool stockDago = stock > 0;
        var lblStock = new Label
        {
            Text = stockDago ? $"Stock: {stock}" : "Agortua",
            Font = Theme.FontBadge,
            BackColor = stockDago ? Theme.SuccessBg : Theme.DangerBg,
            ForeColor = stockDago ? Theme.Success : Theme.Danger,
            Location = new Point(10, 80),
            AutoSize = true,
            Padding = new Padding(4, 2, 4, 2)
        };
        card.Controls.Add(lblStock);

        if (!string.IsNullOrWhiteSpace(platera.Alergenoak))
        {
            var lblAlerg = new Label
            {
                Text = $"Alerg: {platera.Alergenoak}",
                Font = Theme.FontSmall,
                ForeColor = Theme.TextMuted,
                Location = new Point(10, 110),
                Size = new Size(130, 25),
                AutoEllipsis = true
            };
            card.Controls.Add(lblAlerg);
        }

        // Gehitzeko [+] Botoi Handia
        var btnAdd = new Button
        {
            Text = "+",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            Size = new Size(48, 48),
            Location = new Point(160, 92),
            BackColor = stockDago ? Theme.AccentWine : Color.FromArgb(226, 232, 240),
            ForeColor = stockDago ? Color.White : Theme.TextMuted,
            Enabled = stockDago,
            FlatStyle = FlatStyle.Flat,
            Cursor = stockDago ? Cursors.Hand : Cursors.Default
        };
        btnAdd.FlatAppearance.BorderSize = 0;
        btnAdd.Click += (s, e) => KarritoraGehitu(platera);
        card.Controls.Add(btnAdd);

        card.Paint += (s, e) =>
        {
            using var pen = new Pen(Theme.Border, 1);
            e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
        };

        return card;
    }

    private void KarritoraGehitu(PlateraDto platera)
    {
        var existitzenDa = _karritoa.FirstOrDefault(c => c.Platera.Id == platera.Id);
        if (existitzenDa != null)
        {
            if (existitzenDa.Kopurua < platera.StockAhalmena)
            {
                existitzenDa.Kopurua++;
            }
            else
            {
                MessageBox.Show($"Ezin da gehiago eskatu. Stock ahalmena: {platera.StockAhalmena}", 
                    "Stock Muga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }
        else
        {
            _karritoa.Add(new CartItem { Platera = platera, Kopurua = 1 });
        }

        EguneratuKarritoBistaratzea();
    }

    private void EguneratuKarritoBistaratzea()
    {
        _pnlKarritoa.Controls.Clear();

        decimal guztira = 0;

        foreach (var item in _karritoa)
        {
            guztira += item.Azpitotala;
            var r = SortuKarritoLerroa(item);
            _pnlKarritoa.Controls.Add(r);
        }

        decimal azpitotala = guztira / 1.10m;
        decimal bez = guztira - azpitotala;

        _lblAzpitotala.Text = $"Azpitotala: {azpitotala:F2} €";
        _lblBez.Text = $"BEZ (%10 barne): {bez:F2} €";
        _lblGuztira.Text = $"GUZTIRA: {guztira:F2} €";

        _btnBidali.Enabled = _karritoa.Count > 0;
    }

    private Control SortuKarritoLerroa(CartItem item)
    {
        var row = new Panel
        {
            Width = 350,
            Height = 72,
            BackColor = Color.White,
            Margin = new Padding(0, 0, 0, 6),
            Padding = new Padding(8)
        };

        var lblIzena = new Label
        {
            Text = item.Platera.Izena,
            Font = Theme.FontBodyBold,
            ForeColor = Theme.TextMain,
            Location = new Point(8, 8),
            Size = new Size(200, 20),
            AutoEllipsis = true
        };
        row.Controls.Add(lblIzena);

        var lblPrezioa = new Label
        {
            Text = $"{item.Azpitotala:F2} € ({item.Platera.Prezioa:F2} €/u)",
            Font = Theme.FontSmall,
            ForeColor = Theme.TextMuted,
            Location = new Point(8, 30),
            AutoSize = true
        };
        row.Controls.Add(lblPrezioa);

        // [-] [Kop] [+]
        var btnMinus = new Button
        {
            Text = "-",
            Font = Theme.FontBodyBold,
            Size = new Size(28, 28),
            Location = new Point(215, 12),
            BackColor = Color.FromArgb(241, 245, 249),
            ForeColor = Theme.TextMain,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnMinus.FlatAppearance.BorderColor = Theme.Border;
        btnMinus.Click += (s, e) =>
        {
            if (item.Kopurua > 1)
            {
                item.Kopurua--;
            }
            else
            {
                _karritoa.Remove(item);
            }
            EguneratuKarritoBistaratzea();
        };
        row.Controls.Add(btnMinus);

        var lblKop = new Label
        {
            Text = item.Kopurua.ToString(),
            Font = Theme.FontBodyBold,
            ForeColor = Theme.TextMain,
            Size = new Size(28, 28),
            Location = new Point(245, 16),
            TextAlign = ContentAlignment.MiddleCenter
        };
        row.Controls.Add(lblKop);

        var btnPlus = new Button
        {
            Text = "+",
            Font = Theme.FontBodyBold,
            Size = new Size(28, 28),
            Location = new Point(275, 12),
            BackColor = Color.FromArgb(241, 245, 249),
            ForeColor = Theme.TextMain,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnPlus.FlatAppearance.BorderColor = Theme.Border;
        btnPlus.Click += (s, e) =>
        {
            if (item.Kopurua < item.Platera.StockAhalmena)
            {
                item.Kopurua++;
                EguneratuKarritoBistaratzea();
            }
            else
            {
                MessageBox.Show($"Ezin da gehiago eskatu. Stock ahalmena: {item.Platera.StockAhalmena}", 
                    "Stock Muga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        };
        row.Controls.Add(btnPlus);

        // [X] Ezabatu
        var btnDel = new Button
        {
            Text = "X",
            Font = Theme.FontSmall,
            Size = new Size(26, 28),
            Location = new Point(310, 12),
            BackColor = Color.FromArgb(254, 226, 226),
            ForeColor = Theme.Danger,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnDel.FlatAppearance.BorderSize = 0;
        btnDel.Click += (s, e) =>
        {
            _karritoa.Remove(item);
            EguneratuKarritoBistaratzea();
        };
        row.Controls.Add(btnDel);

        row.Paint += (s, e) =>
        {
            using var pen = new Pen(Theme.Border, 1);
            e.Graphics.DrawRectangle(pen, 0, 0, row.Width - 1, row.Height - 1);
        };

        return row;
    }

    private async Task EskaeraBidaliAsync()
    {
        if (_karritoa.Count == 0) return;

        _btnBidali.Enabled = false;
        _btnBidali.Text = "Bidaltzen...";

        int eskaeraId;

        // 1. Eskaera berria sortu behar bada
        if (!_dagoenekoEskaeraId.HasValue)
        {
            var (arrakastaSortu, mezuaSortu, eskaeraSortua) = await _api.SortuEskaeraAsync(_mahaia.Id, _langilea.Id, _pertsonaKopurua);
            if (!arrakastaSortu || eskaeraSortua == null)
            {
                MessageBox.Show($"Ezin izan da eskaera sortu: {mezuaSortu}", "Errorea", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _btnBidali.Enabled = true;
                _btnBidali.Text = "ESKAERA BIDALI";
                return;
            }
            eskaeraId = eskaeraSortua.Id;
        }
        else
        {
            eskaeraId = _dagoenekoEskaeraId.Value;
        }

        // 2. Plater guztiak banan-banan gehitu (backend-ak lehengaien stocka deskontatzen du atomikoki)
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

        _btnBidali.Enabled = true;
        _btnBidali.Text = "ESKAERA BIDALI";

        if (!erroreak)
        {
            MessageBox.Show($"Komanda arrakastaz bidali da Mahaia {_mahaia.Zenbakia}-rentzat!", "Eskaera Bidalita", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
