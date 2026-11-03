using Jatetxea.Desktop.Models;
using Jatetxea.Desktop.UI;

namespace Jatetxea.Desktop.Forms;

public class MahaikideakModal : Form
{
    public int PertsonaKopurua { get; private set; } = 2;
    private readonly Label _lblKopurua;
    private readonly MahaiaDto _mahaia;

    public MahaikideakModal(MahaiaDto mahaia)
    {
        _mahaia = mahaia;
        PertsonaKopurua = Math.Min(2, mahaia.Edukiera > 0 ? mahaia.Edukiera : 2);

        Text = $"Mahaia {_mahaia.Zenbakia} - Pertsona Kopurua";
        Size = new Size(460, 420);
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

        // Titulua
        var lblTitulua = new Label
        {
            Text = $"Mahaia {_mahaia.Zenbakia} ({_mahaia.Kokapena})",
            Font = Theme.FontTitle,
            ForeColor = Theme.AccentWine,
            Dock = DockStyle.Top,
            Height = 35,
            TextAlign = ContentAlignment.MiddleCenter
        };
        pnlRoot.Controls.Add(lblTitulua);

        var lblAzpi = new Label
        {
            Text = $"Hautatu mahaikide kopurua (Gehieneko edukiera: {_mahaia.Edukiera})",
            Font = Theme.FontBody,
            ForeColor = Theme.TextMuted,
            Dock = DockStyle.Top,
            Height = 25,
            TextAlign = ContentAlignment.MiddleCenter
        };
        pnlRoot.Controls.Add(lblAzpi);

        // Zenbatzaile handia ([-] 4 [+])
        var pnlStepper = new Panel
        {
            Dock = DockStyle.Top,
            Height = 85,
            Margin = new Padding(0, 15, 0, 15)
        };
        pnlRoot.Controls.Add(pnlStepper);

        var btnMinus = new Button
        {
            Text = "-",
            Font = Theme.FontTitle,
            Size = new Size(65, 65),
            Location = new Point(50, 10),
            BackColor = Color.FromArgb(241, 245, 249),
            ForeColor = Theme.TextMain,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnMinus.FlatAppearance.BorderColor = Theme.Border;
        btnMinus.Click += (s, e) => AldatuKopurua(-1);
        pnlStepper.Controls.Add(btnMinus);

        _lblKopurua = new Label
        {
            Text = PertsonaKopurua.ToString(),
            Font = new Font("Segoe UI", 32, FontStyle.Bold),
            ForeColor = Theme.TextMain,
            Size = new Size(130, 65),
            Location = new Point(135, 10),
            TextAlign = ContentAlignment.MiddleCenter
        };
        pnlStepper.Controls.Add(_lblKopurua);

        var btnPlus = new Button
        {
            Text = "+",
            Font = Theme.FontTitle,
            Size = new Size(65, 65),
            Location = new Point(285, 10),
            BackColor = Color.FromArgb(241, 245, 249),
            ForeColor = Theme.TextMain,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnPlus.FlatAppearance.BorderColor = Theme.Border;
        btnPlus.Click += (s, e) => AldatuKopurua(1);
        pnlStepper.Controls.Add(btnPlus);

        // Botoi azkarrak (1..8)
        var pnlQuick = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 90,
            Padding = new Padding(10, 5, 10, 5)
        };
        pnlRoot.Controls.Add(pnlQuick);

        for (int i = 1; i <= 8; i++)
        {
            int balioa = i;
            var btnQuick = new Button
            {
                Text = balioa.ToString(),
                Size = new Size(42, 42),
                Margin = new Padding(4),
                Font = Theme.FontBodyBold,
                BackColor = Color.FromArgb(248, 250, 252),
                ForeColor = Theme.TextMain,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnQuick.FlatAppearance.BorderColor = Theme.Border;
            btnQuick.Click += (s, e) =>
            {
                PertsonaKopurua = balioa;
                _lblKopurua.Text = PertsonaKopurua.ToString();
            };
            pnlQuick.Controls.Add(btnQuick);
        }

        // Beheko ekintza botoiak
        var pnlEkintzak = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 55
        };
        pnlRoot.Controls.Add(pnlEkintzak);

        var btnUtzi = new Button
        {
            Text = "Utzi",
            DialogResult = DialogResult.Cancel,
            Size = new Size(130, 48),
            Location = new Point(20, 4),
            Font = Theme.FontBodyBold,
            BackColor = Color.FromArgb(241, 245, 249),
            ForeColor = Theme.TextMuted,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnUtzi.FlatAppearance.BorderColor = Theme.Border;
        pnlEkintzak.Controls.Add(btnUtzi);

        var btnJarraitu = new Button
        {
            Text = "JARRAITU",
            DialogResult = DialogResult.OK,
            Size = new Size(220, 48),
            Location = new Point(170, 4),
            Font = Theme.FontBodyBold,
            BackColor = Theme.AccentWine,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnJarraitu.FlatAppearance.BorderSize = 0;
        pnlEkintzak.Controls.Add(btnJarraitu);

        AcceptButton = btnJarraitu;
        CancelButton = btnUtzi;
    }

    private void AldatuKopurua(int aldaketa)
    {
        PertsonaKopurua = Math.Max(1, Math.Min(20, PertsonaKopurua + aldaketa));
        _lblKopurua.Text = PertsonaKopurua.ToString();
    }
}
