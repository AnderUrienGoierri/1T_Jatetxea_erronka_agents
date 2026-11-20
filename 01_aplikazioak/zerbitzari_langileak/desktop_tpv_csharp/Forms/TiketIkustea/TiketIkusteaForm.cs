using System.Drawing.Drawing2D;
using Jatetxea.Desktop.Models;

namespace Jatetxea.Desktop.Forms;

public partial class TiketIkusteaForm : Form
{
    private readonly OrdainketaErantzunaDto _ordainketa;

    /// <summary>
    /// Parameterless constructor for Visual Studio Form Designer.
    /// </summary>
    public TiketIkusteaForm() : this(new OrdainketaErantzunaDto())
    {
    }

    public TiketIkusteaForm(OrdainketaErantzunaDto ordainketa)
    {
        _ordainketa = ordainketa;

        InitializeComponent();

        if (!string.IsNullOrWhiteSpace(_ordainketa.TiketZenbakia))
        {
            Text = $"T1 Jatetxea - Tiket Fiskala ({_ordainketa.TiketZenbakia})";
            lblSub.Text = $"Tiket Zenbakia: {_ordainketa.TiketZenbakia} · {_ordainketa.Zenbatekoa:F2} € ({_ordainketa.Mota})";
        }

        _txtTiket.Text = _ordainketa.TiketTestua;

        pnlBehea.Paint += (s, e) =>
        {
            using var pen = new Pen(Color.FromArgb(226, 232, 240), 1.5f);
            e.Graphics.DrawLine(pen, 0, 0, pnlBehea.Width, 0);
        };

        btnGorde.Click += GordeTiketaFitxategian;
        btnInprimatu.Click += InprimatuSimulazioa;
        btnItxi.Click += (s, e) => { DialogResult = DialogResult.OK; Close(); };
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
