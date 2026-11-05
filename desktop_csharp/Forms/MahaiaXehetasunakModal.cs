using System.Drawing.Drawing2D;
using Jatetxea.Desktop.Models;
using Jatetxea.Desktop.Services;

namespace Jatetxea.Desktop.Forms;

public partial class MahaiaXehetasunakModal : Form
{
    private readonly MahaiaDto _mahaia;
    private readonly LangileaDto _langilea;
    private readonly ApiService _api;
    private EskaeraDto? _eskaera;

    /// <summary>
    /// Visual Studio WinForms Designer-erako eraikitzailea
    /// </summary>
    public MahaiaXehetasunakModal()
    {
        InitializeComponent();
        _mahaia = new MahaiaDto { Id = 4, Zenbakia = 4, Edukiera = 4, Egoera = "Okupatuta" };
        _langilea = new LangileaDto { Id = 1, Izena = "Ane G.", Rola = "Zerbitzaria" };
        _api = new ApiService();
        KonfiguratuInterfazea();
    }

    public MahaiaXehetasunakModal(MahaiaDto mahaia, LangileaDto langilea, ApiService api)
    {
        InitializeComponent();
        _mahaia = mahaia;
        _langilea = langilea;
        _api = api;
        KonfiguratuInterfazea();
    }

    private void KonfiguratuInterfazea()
    {
        Text = $"Mahaia {_mahaia.Zenbakia} - Xehetasuna";
        lblTitulua.Text = $"Mahaia {_mahaia.Zenbakia} - Xehetasuna";
        lblAzpititulua.Text = $"({_mahaia.Edukiera} pertsona) - {_mahaia.Egoera}";

        // Tiket txartelaren kanpoko ertza
        pnlTicketCard.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var pen = new Pen(Color.FromArgb(203, 213, 225), 1.5f);
            e.Graphics.DrawRectangle(pen, 0, 0, pnlTicketCard.Width - 1, pnlTicketCard.Height - 1);
        };

        // Tiket azpiko marra banatzailea
        pnlTicketTotal.Paint += (s, e) =>
        {
            using var pen = new Pen(Color.FromArgb(226, 232, 240), 1.5f);
            e.Graphics.DrawLine(pen, 0, 0, pnlTicketTotal.Width, 0);
        };

        btnGehituEskaera.Click += (s, e) => GehituEskaeraKlikatua();
        btnKontuaEskatu.Click += async (s, e) => await KobratuEtaItxiAsync();
        btnAldatuMahaiaz.Click += (s, e) => AldatuMahaiazKlikatua();
        btnItxi.Click += (s, e) => Close();

        Load += async (s, e) => await KargatuEskaeraDatuakAsync();
    }

    private async Task KargatuEskaeraDatuakAsync()
    {
        try
        {
            _eskaera = await _api.GetMahaiarenEskaeraAktiboaAsync(_mahaia.Id);
        }
        catch
        {
            _eskaera = null;
        }

        pnlTicketLines.SuspendLayout();
        pnlTicketLines.Controls.Clear();

        if (_eskaera == null || _eskaera.Lerroak.Count == 0)
        {
            // Fallback lerroak (Zirriborroko erakusgarriak)
            BistaratuLerroak(new List<(string KantitateaEtaIzena, decimal Zenbatekoa)>
            {
                ("2x Ura 1L", 4.00m),
                ("1x Txuleta 1kg", 38.00m),
                ("1x Bakailaoa", 18.00m)
            }, 60.00m);
        }
        else
        {
            var lerroak = _eskaera.Lerroak.Select(l => ($"{l.Kantitatea}x {l.PlaterIzena}", l.Azpitotala)).ToList();
            BistaratuLerroak(lerroak, _eskaera.Guztira);
        }

        pnlTicketLines.ResumeLayout(true);
    }

    private void BistaratuLerroak(List<(string KantitateaEtaIzena, decimal Zenbatekoa)> lerroak, decimal guztira)
    {
        pnlTicketLines.Controls.Clear();
        int y = 6;

        foreach (var (testua, zenbatekoa) in lerroak)
        {
            var pnlRow = new Panel
            {
                Location = new Point(4, y),
                Size = new Size(pnlTicketLines.Width - 16, 44),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.White
            };

            var lblIzena = new Label
            {
                Text = testua,
                Font = new Font("Segoe UI", 12F, FontStyle.Regular),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(10, 4),
                Size = new Size(pnlRow.Width - 170, 36),
                TextAlign = ContentAlignment.MiddleLeft
            };
            pnlRow.Controls.Add(lblIzena);

            var lblPrezioa = new Label
            {
                Text = $"{zenbatekoa:F2} €",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                Location = new Point(pnlRow.Width - 160, 4),
                Size = new Size(150, 36),
                TextAlign = ContentAlignment.MiddleRight,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            pnlRow.Controls.Add(lblPrezioa);

            pnlTicketLines.Controls.Add(pnlRow);
            y += 48;
        }

        lblGuztira.Text = $"Guztira:  {guztira:F2} €";
    }

    private void GehituEskaeraKlikatua()
    {
        using var eskaeraForm = new EskaeraForm(_mahaia, _langilea, _api);
        Hide();
        eskaeraForm.ShowDialog();
        Show();
        _ = KargatuEskaeraDatuakAsync();
    }

    private async Task KobratuEtaItxiAsync()
    {
        if (_eskaera == null)
        {
            MessageBox.Show("Ez dago kobratzeko eskaerarik.", "Informazioa", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var konfirmatu = MessageBox.Show(
            $"Mahaia {_mahaia.Zenbakia}-ren kontua eskatu eta ordainduta markatu nahi duzu?\n\nGuztira: {_eskaera.Guztira:F2} €",
            "Kontua Eskatu",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (konfirmatu == DialogResult.Yes)
        {
            bool arrakasta = await _api.EguneratuMahaiarenEgoeraAsync(_mahaia.Id, "Garbitzen");
            if (arrakasta)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }

    private void AldatuMahaiazKlikatua()
    {
        MessageBox.Show(
            $"Mahaia {_mahaia.Zenbakia} beste mahai batera aldatzeko funtzioa aktibatu da.",
            "Aldatu Mahaiaz",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }
}
