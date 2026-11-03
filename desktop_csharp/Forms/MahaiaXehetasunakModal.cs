using Jatetxea.Desktop.Models;
using Jatetxea.Desktop.Services;
using Jatetxea.Desktop.UI;

namespace Jatetxea.Desktop.Forms;

public class MahaiaXehetasunakModal : Form
{
    private readonly MahaiaDto _mahaia;
    private readonly LangileaDto _langilea;
    private readonly ApiService _api;
    private EskaeraDto? _eskaera;

    private readonly Panel _pnlLerroak;
    private readonly Label _lblAzpitotala;
    private readonly Label _lblBez;
    private readonly Label _lblGuztira;
    private readonly Label _lblEgoeraInfo;
    private readonly Button _btnGehituEskaera;
    private readonly Button _btnKobratu;
    private readonly Button _btnGarbitu;

    public MahaiaXehetasunakModal(MahaiaDto mahaia, LangileaDto langilea, ApiService api)
    {
        _mahaia = mahaia;
        _langilea = langilea;
        _api = api;

        Text = $"Mahaia {_mahaia.Zenbakia} - Xehetasunak eta Kontua";
        Size = new Size(620, 680);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Theme.Surface;

        var pnlRoot = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(25)
        };
        Controls.Add(pnlRoot);

        // Titulua eta Mahaiaren egoera
        var lblTitulua = new Label
        {
            Text = $"MAHAIA {_mahaia.Zenbakia} ({_mahaia.Kokapena})",
            Font = Theme.FontTitle,
            ForeColor = Theme.AccentWine,
            Dock = DockStyle.Top,
            Height = 35,
            TextAlign = ContentAlignment.MiddleLeft
        };
        pnlRoot.Controls.Add(lblTitulua);

        _lblEgoeraInfo = new Label
        {
            Text = $"Egoera: {_mahaia.Egoera} | Edukiera: {_mahaia.Edukiera} lagun",
            Font = Theme.FontBody,
            ForeColor = Theme.TextMuted,
            Dock = DockStyle.Top,
            Height = 25,
            TextAlign = ContentAlignment.MiddleLeft
        };
        pnlRoot.Controls.Add(_lblEgoeraInfo);

        // Eskaera lerroen panela
        var lblKomandaIzenburua = new Label
        {
            Text = "Uneko Komandaren Lerroak:",
            Font = Theme.FontBodyBold,
            ForeColor = Theme.TextMain,
            Dock = DockStyle.Top,
            Height = 30,
            TextAlign = ContentAlignment.BottomLeft
        };
        pnlRoot.Controls.Add(lblKomandaIzenburua);

        _pnlLerroak = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            BackColor = Color.FromArgb(248, 250, 252),
            Margin = new Padding(0, 10, 0, 10),
            Padding = new Padding(10)
        };
        pnlRoot.Controls.Add(_pnlLerroak);

        // Laburpena eta Botoiak
        var pnlBehea = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 210,
            Padding = new Padding(0, 10, 0, 0)
        };
        pnlRoot.Controls.Add(pnlBehea);

        var pnlZenbatekoak = new Panel
        {
            Dock = DockStyle.Top,
            Height = 90,
            BackColor = Color.White
        };
        pnlBehea.Controls.Add(pnlZenbatekoak);

        _lblAzpitotala = new Label
        {
            Text = "Azpitotala: 0.00 €",
            Font = Theme.FontBody,
            ForeColor = Theme.TextMuted,
            Location = new Point(10, 8),
            AutoSize = true
        };
        pnlZenbatekoak.Controls.Add(_lblAzpitotala);

        _lblBez = new Label
        {
            Text = "BEZ (%10): 0.00 €",
            Font = Theme.FontBody,
            ForeColor = Theme.TextMuted,
            Location = new Point(10, 32),
            AutoSize = true
        };
        pnlZenbatekoak.Controls.Add(_lblBez);

        _lblGuztira = new Label
        {
            Text = "GUZTIRA: 0.00 €",
            Font = Theme.FontTitle,
            ForeColor = Theme.AccentWine,
            Location = new Point(10, 56),
            AutoSize = true
        };
        pnlZenbatekoak.Controls.Add(_lblGuztira);

        // Ekintza botoiak
        var pnlBotoiak = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 110
        };
        pnlBehea.Controls.Add(pnlBotoiak);

        _btnGehituEskaera = new Button
        {
            Text = "+ ESKAERA GEHITU",
            Font = Theme.FontBodyBold,
            Size = new Size(180, 48),
            Location = new Point(10, 10),
            BackColor = Theme.Info,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        _btnGehituEskaera.FlatAppearance.BorderSize = 0;
        _btnGehituEskaera.Click += (s, e) => GehituEskaeraKlikatua();
        pnlBotoiak.Controls.Add(_btnGehituEskaera);

        _btnKobratu = new Button
        {
            Text = "KOBRATU ETA ITXI",
            Font = Theme.FontBodyBold,
            Size = new Size(180, 48),
            Location = new Point(200, 10),
            BackColor = Theme.Success,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        _btnKobratu.FlatAppearance.BorderSize = 0;
        _btnKobratu.Click += async (s, e) => await KobratuEtaItxiAsync();
        pnlBotoiak.Controls.Add(_btnKobratu);

        _btnGarbitu = new Button
        {
            Text = "LIBRE MARKATU",
            Font = Theme.FontBodyBold,
            Size = new Size(160, 48),
            Location = new Point(390, 10),
            BackColor = Theme.Warning,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Visible = _mahaia.Egoera.Equals("Garbitzen", StringComparison.OrdinalIgnoreCase)
        };
        _btnGarbitu.FlatAppearance.BorderSize = 0;
        _btnGarbitu.Click += async (s, e) => await GarbitutaMarkatuAsync();
        pnlBotoiak.Controls.Add(_btnGarbitu);

        var btnItxi = new Button
        {
            Text = "Itxi Lehioa",
            Font = Theme.FontBody,
            Size = new Size(130, 38),
            Location = new Point(10, 68),
            BackColor = Color.FromArgb(241, 245, 249),
            ForeColor = Theme.TextMuted,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnItxi.FlatAppearance.BorderColor = Theme.Border;
        btnItxi.Click += (s, e) => Close();
        pnlBotoiak.Controls.Add(btnItxi);

        Load += async (s, e) => await KargatuEskaeraDatuakAsync();
    }

    private async Task KargatuEskaeraDatuakAsync()
    {
        _eskaera = await _api.GetMahaiarenEskaeraAktiboaAsync(_mahaia.Id);

        _pnlLerroak.Controls.Clear();

        if (_eskaera == null || _eskaera.Lerroak.Count == 0)
        {
            var lblHutsik = new Label
            {
                Text = "Ez dago eskaera-lerro aktiborik mahai honetan.",
                Font = Theme.FontBody,
                ForeColor = Theme.TextMuted,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            _pnlLerroak.Controls.Add(lblHutsik);

            _lblAzpitotala.Text = "Azpitotala: 0.00 €";
            _lblBez.Text = "BEZ (%10): 0.00 €";
            _lblGuztira.Text = "GUZTIRA: 0.00 €";
            _btnKobratu.Enabled = false;
            return;
        }

        _lblEgoeraInfo.Text = $"Egoera: {_mahaia.Egoera} | Zerbitzaria: {_eskaera.LangileIzena} | Data: {_eskaera.SortzeData}";
        _btnKobratu.Enabled = true;

        int y = 5;
        foreach (var l in _eskaera.Lerroak)
        {
            var pnlLerroa = new Panel
            {
                Location = new Point(5, y),
                Size = new Size(_pnlLerroak.Width - 30, 50),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.White,
                Padding = new Padding(8)
            };

            var lblIzena = new Label
            {
                Text = $"{l.PlaterIzena} x{l.Kantitatea}",
                Font = Theme.FontBodyBold,
                ForeColor = Theme.TextMain,
                Location = new Point(8, 8),
                Size = new Size(260, 20),
                AutoEllipsis = true
            };
            pnlLerroa.Controls.Add(lblIzena);

            var lblEgoeraLerroa = new Label
            {
                Text = l.Egoera,
                Font = Theme.FontBadge,
                BackColor = Theme.InfoBg,
                ForeColor = Theme.Info,
                Location = new Point(8, 28),
                AutoSize = true,
                Padding = new Padding(3, 1, 3, 1)
            };
            pnlLerroa.Controls.Add(lblEgoeraLerroa);

            var lblPrezioa = new Label
            {
                Text = $"{l.UnitatePrezioa * l.Kantitatea:F2} €",
                Font = Theme.FontBodyBold,
                ForeColor = Theme.AccentWine,
                Location = new Point(pnlLerroa.Width - 110, 14),
                Size = new Size(100, 22),
                TextAlign = ContentAlignment.MiddleRight,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            pnlLerroa.Controls.Add(lblPrezioa);

            _pnlLerroak.Controls.Add(pnlLerroa);
            y += 56;
        }

        decimal guztira = _eskaera.Guztira;
        decimal azpitotala = guztira / 1.10m;
        decimal bez = guztira - azpitotala;

        _lblAzpitotala.Text = $"Azpitotala: {azpitotala:F2} €";
        _lblBez.Text = $"BEZ (%10): {bez:F2} €";
        _lblGuztira.Text = $"GUZTIRA: {guztira:F2} €";
    }

    private void GehituEskaeraKlikatua()
    {
        int pertsonak = _eskaera?.PertsonaKopurua ?? 2;
        int? eskaeraId = _eskaera?.Id;

        using var f = new EskaeraForm(_mahaia, _langilea, _api, pertsonak, eskaeraId);
        if (f.ShowDialog() == DialogResult.OK)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }

    private async Task KobratuEtaItxiAsync()
    {
        if (_eskaera == null) return;

        var confirm = MessageBox.Show(
            $"Mahaia {_mahaia.Zenbakia}-ko kontua kobratu nahi duzu?\n\nGuztira: {_eskaera.Guztira:F2} €",
            "Kontua Kobratu",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirm != DialogResult.Yes) return;

        _btnKobratu.Enabled = false;
        _btnKobratu.Text = "Kobratzen...";

        bool ok = await _api.EguneratuEskaerarenEgoeraAsync(_eskaera.Id, "Kobratua");
        if (ok)
        {
            MessageBox.Show(
                $"Mahaia {_mahaia.Zenbakia}-ko kontua arrakastaz kobratu da.\nMahaia 'Garbitzen' egoerara igaro da automatikoki.",
                "Kobratuta",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            DialogResult = DialogResult.OK;
            Close();
        }
        else
        {
            MessageBox.Show("Errorea kontua kobratzean.", "Errorea", MessageBoxButtons.OK, MessageBoxIcon.Error);
            _btnKobratu.Enabled = true;
            _btnKobratu.Text = "KOBRATU ETA ITXI";
        }
    }

    private async Task GarbitutaMarkatuAsync()
    {
        bool ok = await _api.EguneratuMahaiarenEgoeraAsync(_mahaia.Id, "Libre");
        if (ok)
        {
            MessageBox.Show($"Mahaia {_mahaia.Zenbakia} 'Libre' egoeran dago orain.", "Mahaia Garbituta", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
