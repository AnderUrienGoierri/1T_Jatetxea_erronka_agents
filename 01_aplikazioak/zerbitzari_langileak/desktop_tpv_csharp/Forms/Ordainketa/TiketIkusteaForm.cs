using System.Drawing.Drawing2D;
using Jatetxea.Desktop.Models;

namespace Jatetxea.Desktop.Forms;

public class TiketIkusteaForm : Form
{
    private readonly OrdainketaErantzunaDto _ordainketa;
    private readonly TextBox _txtTiket;

    public TiketIkusteaForm(OrdainketaErantzunaDto ordainketa)
    {
        _ordainketa = ordainketa;

        Text = $"T1 Jatetxea - Tiket Fiskala ({_ordainketa.TiketZenbakia})";
        Size = new Size(540, 720);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Color.FromArgb(248, 250, 252);

        // Goi panela (Izenburua eta Arrakasta Mezu Garbia)
        var pnlGoiburua = new Panel
        {
            Dock = DockStyle.Top,
            Height = 70,
            BackColor = Color.FromArgb(22, 101, 52), // Berde dotorea
            Padding = new Padding(20, 12, 20, 12)
        };

        var lblTit = new Label
        {
            Text = "ORDAINKETA ONARTUA - TIKET FISKALA",
            Font = new Font("Segoe UI", 12.5F, FontStyle.Bold),
            ForeColor = Color.White,
            Dock = DockStyle.Top,
            Height = 24
        };

        var lblSub = new Label
        {
            Text = $"Tiket Zenbakia: {_ordainketa.TiketZenbakia} · {_ordainketa.Zenbatekoa:F2} € ({_ordainketa.Mota})",
            Font = new Font("Segoe UI", 10F, FontStyle.Regular),
            ForeColor = Color.FromArgb(220, 252, 231),
            Dock = DockStyle.Top,
            Height = 20
        };

        pnlGoiburua.Controls.Add(lblSub);
        pnlGoiburua.Controls.Add(lblTit);

        // Beheko ekintza barra
        var pnlBehea = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 65,
            BackColor = Color.White,
            Padding = new Padding(16, 12, 16, 12)
        };
        pnlBehea.Paint += (s, e) =>
        {
            using var pen = new Pen(Color.FromArgb(226, 232, 240), 1.5f);
            e.Graphics.DrawLine(pen, 0, 0, pnlBehea.Width, 0);
        };

        var btnGorde = new Button
        {
            Text = "Gorde Tiketa (.txt)",
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            BackColor = Color.FromArgb(241, 245, 249),
            ForeColor = Color.FromArgb(30, 41, 59),
            FlatStyle = FlatStyle.Flat,
            Size = new Size(150, 40),
            Location = new Point(16, 12),
            Cursor = Cursors.Hand
        };
        btnGorde.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnGorde.Click += GordeTiketaFitxategian;

        var btnInprimatu = new Button
        {
            Text = "Inprimatu (ESC/POS)",
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            BackColor = Color.FromArgb(136, 19, 55),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Size = new Size(160, 40),
            Location = new Point(176, 12),
            Cursor = Cursors.Hand
        };
        btnInprimatu.FlatAppearance.BorderSize = 0;
        btnInprimatu.Click += InprimatuSimulazioa;

        var btnItxi = new Button
        {
            Text = "Itxi",
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            BackColor = Color.FromArgb(71, 85, 105),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Size = new Size(95, 40),
            Location = new Point(410, 12),
            Cursor = Cursors.Hand
        };
        btnItxi.FlatAppearance.BorderSize = 0;
        btnItxi.Click += (s, e) => { DialogResult = DialogResult.OK; Close(); };

        pnlBehea.Controls.Add(btnGorde);
        pnlBehea.Controls.Add(btnInprimatu);
        pnlBehea.Controls.Add(btnItxi);

        // Erdialdeko Tiket Testu Kutxa (80mm termiko itxurarekin)
        var pnlKutxa = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(24, 16, 24, 16),
            BackColor = Color.FromArgb(241, 245, 249)
        };

        _txtTiket = new TextBox
        {
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            Font = new Font("Consolas", 10F, FontStyle.Regular),
            BackColor = Color.White,
            ForeColor = Color.FromArgb(15, 23, 42),
            Dock = DockStyle.Fill,
            Text = _ordainketa.TiketTestua
        };

        pnlKutxa.Controls.Add(_txtTiket);

        Controls.Add(pnlKutxa);
        Controls.Add(pnlBehea);
        Controls.Add(pnlGoiburua);
    }

    private void GordeTiketaFitxategian(object? sender, EventArgs e)
    {
        try
        {
            using var sfd = new SaveFileDialog
            {
                Title = "Gorde Jatetxeko Tiket Fiskala",
                Filter = "Testu Fitxategia (*.txt)|*.txt|Dokumentu Guztiak (*.*)|*.*",
                FileName = $"Tiket_{_ordainketa.TiketZenbakia}.txt"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(sfd.FileName, _ordainketa.TiketTestua);
                MessageBox.Show(
                    $"Tiket fiskala arrakastaz gorde da:\n{sfd.FileName}",
                    "Tiketa Gordeta",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Errorea tiketa gordetzean: {ex.Message}", "Errorea", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void InprimatuSimulazioa(object? sender, EventArgs e)
    {
        // ESC/POS termiko inprimagailura bidalitako agindua simulatu
        MessageBox.Show(
            $"Tiketa arrakastaz bidali da 80mm-ko ESC/POS inprimagailura (Portua: USB-PRN01):\n\n" +
            $"Tiket zenbakia: {_ordainketa.TiketZenbakia}\n" +
            $"Operazio kodea: {_ordainketa.OperazioKodea}\n" +
            $"Totala: {_ordainketa.Zenbatekoa:F2} €\n\n" +
            "Inprimaketa amaituta. Moztu papera.",
            "Inprimagailu Termikoa",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }
}
