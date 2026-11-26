using System.Drawing.Drawing2D;
using Jatetxea.Desktop.Models;
using Jatetxea.Desktop.UI;

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

        // T1 Warm Hospitality v2.0 paleta + DoubleBuffering
        Theme.ApplyWarmHospitalityTheme(this);
        BackColor = Theme.WarmLinen;
        pnlKutxa.BackColor = Theme.WarmLinen;
        pnlBehea.BackColor = Theme.MarbleWhite;
        _txtTiket.Font = Theme.MonoFont(10f, bold: false);
        Theme.StyleButton(btnGorde, Theme.StoneMist, Theme.EspressoInk, Theme.WarmTaupe, radius: 10, fontSize: 10f);
        Theme.StyleButton(btnInprimatu, Theme.ChiantiPrimary, Theme.MarbleWhite, null, radius: 10, fontSize: 10f);
        Theme.StyleButton(btnItxi, Theme.EspressoInk, Theme.MarbleWhite, null, radius: 10, fontSize: 10f);

        if (!string.IsNullOrWhiteSpace(_ordainketa.TiketZenbakia))
        {
            Text = $"T1 Jatetxea - Tiket Fiskala ({_ordainketa.TiketZenbakia})";
            lblSub.Text = $"Tiket Zenbakia: {_ordainketa.TiketZenbakia} · {_ordainketa.Zenbatekoa:F2} € ({_ordainketa.Mota})";
        }

        // Tiketaren lerro-jauziak Windows CRLF formatura normalizatu (ez daitezen lerroak elkartu!)
        _txtTiket.WordWrap = false;
        _txtTiket.ScrollBars = ScrollBars.Both;
        _txtTiket.Text = (_ordainketa.TiketTestua ?? string.Empty)
            .Replace("\r\n", "\n")
            .Replace("\r", "\n")
            .Replace("\n", Environment.NewLine);

        pnlBehea.Paint += (s, e) =>
        {
            using var pen = new Pen(Color.FromArgb(226, 232, 240), 1.5f);
            e.Graphics.DrawLine(pen, 0, 0, pnlBehea.Width, 0);
        };

        btnGorde.Click += GordeTiketaFitxategian;
        btnInprimatu.Click += InprimatuSimulazioa;
        btnItxi.Click += (s, e) => { DialogResult = DialogResult.OK; Close(); };

        // Responsive: tiket termikoa zentratuta eta DPI-rekin eskalatuta
        pnlKutxa.Resize += (s, e) => EgokituTiketZabalera();
        Load += (s, e) =>
        {
            Theme.ApplyModalDpiSize(this, 680, 700, 540, 520);
            EgokituTiketZabalera();
        };
    }

    private void EgokituTiketZabalera()
    {
        int S(int px) => Theme.S(this, px);

        // 1. Goiko barra (titulu eta azpititulu gainjarpenik gabe)
        pnlGoiburua.Height = S(86);
        lblTit.Font = Theme.DisplayFont(13.5f);
        lblTit.ForeColor = Theme.MarbleWhite;
        lblTit.SetBounds(S(24), S(14), Math.Max(S(360), pnlGoiburua.ClientSize.Width - S(48)), S(32));
        lblSub.Font = Theme.BodyFont(9.5f, bold: false);
        lblSub.ForeColor = Theme.SuccessBg;
        lblSub.SetBounds(S(24), S(48), Math.Max(S(360), pnlGoiburua.ClientSize.Width - S(48)), S(26));

        // 2. Tiket termikoa zentratuta eta zabalera osoarekin (80mm monospaced)
        int targetW = S(580);
        int horizPad = Math.Max(S(20), (pnlKutxa.ClientSize.Width - targetW) / 2);
        pnlKutxa.Padding = new Padding(horizPad, S(16), horizPad, S(16));

        // 3. Beheko barra eta botoiak (testu-mozketarik gabe!)
        pnlBehea.Height = S(72);
        int bH = S(42);
        int bY = (pnlBehea.Height - bH) / 2;
        int wGorde = Math.Max(S(156), TextRenderer.MeasureText(btnGorde.Text, btnGorde.Font).Width + S(32));
        int wInpr = Math.Max(S(156), TextRenderer.MeasureText(btnInprimatu.Text, btnInprimatu.Font).Width + S(32));
        int wItxi = Math.Max(S(100), TextRenderer.MeasureText(btnItxi.Text, btnItxi.Font).Width + S(32));

        btnGorde.SetBounds(S(20), bY, wGorde, bH);
        btnInprimatu.SetBounds(btnGorde.Right + S(12), bY, wInpr, bH);
        btnItxi.SetBounds(pnlBehea.ClientSize.Width - wItxi - S(20), bY, wItxi, bH);
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
