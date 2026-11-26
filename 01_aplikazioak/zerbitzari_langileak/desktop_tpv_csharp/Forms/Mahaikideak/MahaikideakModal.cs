using Jatetxea.Desktop.Models;
using Jatetxea.Desktop.UI;

namespace Jatetxea.Desktop.Forms;

public partial class MahaikideakModal : Form
{
    public int PertsonaKopurua { get; private set; } = 4;
    private readonly MahaiaDto? _mahaia;
    private readonly List<Button> _quickButtons = new();

    /// <summary>
    /// Visual Studio WinForms Designer-erako eraikitzailea
    /// </summary>
    public MahaikideakModal()
    {
        InitializeComponent();

        // T1 Warm Hospitality v2.0 paleta + DoubleBuffering
        Theme.ApplyWarmHospitalityTheme(this);
        BackColor = Theme.WarmLinen;
        lblTitulua.Font = Theme.DisplayFont(16f);
        lblTitulua.ForeColor = Theme.ChiantiPrimary;
        lblKopuruaTestua.Font = Theme.MonoFont(15f, bold: true);
        lblKopuruaTestua.ForeColor = Theme.EspressoInk;
        Theme.StyleButton(btnMinus, Theme.StoneMist, Theme.EspressoInk, Theme.WarmTaupe, radius: 10, fontSize: 14f);
        Theme.StyleButton(btnPlus, Theme.StoneMist, Theme.EspressoInk, Theme.WarmTaupe, radius: 10, fontSize: 14f);
        Theme.StyleButton(btnUtzi, Theme.StoneMist, Theme.EspressoInk, Theme.WarmTaupe, radius: 10, fontSize: 10.5f);
        Theme.StyleButton(btnJarraitu, Theme.ChiantiPrimary, Theme.MarbleWhite, null, radius: 10, fontSize: 10.5f);

        KonfiguratuBotoiak();
    }

    public MahaikideakModal(MahaiaDto mahaia) : this()
    {
        _mahaia = mahaia;
        PertsonaKopurua = Math.Clamp(mahaia.Edukiera > 0 ? Math.Min(4, mahaia.Edukiera) : 4, 1, 20);

        lblTitulua.Text = $"Mahaia {_mahaia.Zenbakia} - Mahaikide Kopurua";
        Text = $"Mahaia {_mahaia.Zenbakia} - Mahaikide Kopurua";
        if (_mahaia.Edukiera > 0)
        {
            lblAzpititulua.Text = $"Aukeratu pertsona kopurua (Gehieneko edukiera: {_mahaia.Edukiera})";
        }

        EguneratuUI();
    }

    private void KonfiguratuBotoiak()
    {
        _quickButtons.Clear();
        _quickButtons.Add(btnNum1);
        _quickButtons.Add(btnNum2);
        _quickButtons.Add(btnNum3);
        _quickButtons.Add(btnNum4);
        _quickButtons.Add(btnNum5);
        _quickButtons.Add(btnNum6);
        _quickButtons.Add(btnNum7);
        _quickButtons.Add(btnNum8Plus);

        for (int i = 0; i < _quickButtons.Count; i++)
        {
            int balioa = i + 1;
            var btn = _quickButtons[i];
            btn.Click += (s, e) =>
            {
                PertsonaKopurua = balioa;
                EguneratuUI();
            };
        }

        btnMinus.Click += (s, e) => AldatuKopurua(-1);
        btnPlus.Click += (s, e) => AldatuKopurua(1);

        Resize += (s, e) => EgokituResponsiveDiseinua();
        Load += (s, e) =>
        {
            Theme.ApplyModalDpiSize(this, 720, 540, 620, 470);
            EgokituResponsiveDiseinua();
        };

        EguneratuUI();
    }

    private void EgokituResponsiveDiseinua()
    {
        int S(int px) => Theme.S(this, px);
        int contentW = Math.Clamp(ClientSize.Width - S(48), S(460), S(740));
        int leftX = (ClientSize.Width - contentW) / 2;
        int availH = ClientSize.Height;

        int titH = S(42);
        int subH = S(28);
        int stepH = S(56);
        int actH = S(58);
        int gridH = Math.Clamp(availH - titH - subH - stepH - actH - S(72), S(140), S(240));
        int totalH = titH + subH + S(12) + gridH + S(14) + stepH + S(16) + actH;
        int y = Math.Max(S(14), (availH - totalH) / 2);

        lblTitulua.SetBounds(leftX, y, contentW, titH);
        y += titH;
        lblAzpititulua.SetBounds(leftX, y, contentW, subH);
        y += subH + S(12);
        pnlSareta.SetBounds(leftX, y, contentW, gridH);
        y += gridH + S(14);

        pnlStepper.SetBounds(leftX, y, contentW, stepH);
        int btnStepW = S(64);
        btnMinus.SetBounds(S(4), S(4), btnStepW, stepH - S(8));
        btnPlus.SetBounds(contentW - btnStepW - S(4), S(4), btnStepW, stepH - S(8));
        lblKopuruaTestua.SetBounds(btnStepW + S(10), S(4), contentW - (btnStepW + S(10)) * 2, stepH - S(8));
        y += stepH + S(16);

        pnlEkintzak.SetBounds(leftX, y, contentW, actH);
        int gap = S(16);
        int halfW = (contentW - gap - S(8)) / 2;
        btnUtzi.SetBounds(S(4), S(4), halfW, actH - S(8));
        btnJarraitu.SetBounds(btnUtzi.Right + gap, S(4), halfW, actH - S(8));
    }

    private void AldatuKopurua(int aldaketa)
    {
        int berria = PertsonaKopurua + aldaketa;
        if (berria < 1) berria = 1;

        if (_mahaia != null && _mahaia.Edukiera > 0 && berria > _mahaia.Edukiera + 4)
        {
            // Gehiegi gainditzea saihestu
            return;
        }

        PertsonaKopurua = berria;
        EguneratuUI();
    }

    private void EguneratuUI()
    {
        lblKopuruaTestua.Text = $"{PertsonaKopurua} {(PertsonaKopurua == 1 ? "pertsona" : "pertsona")}";

        for (int i = 0; i < _quickButtons.Count; i++)
        {
            var btn = _quickButtons[i];
            int balioa = i + 1;
            bool isHautatua = (i < 7 && PertsonaKopurua == balioa) || (i == 7 && PertsonaKopurua >= 8);

            if (isHautatua)
            {
                btn.BackColor = Color.FromArgb(136, 19, 55);
                btn.ForeColor = Color.White;
                btn.FlatAppearance.BorderSize = 0;
            }
            else
            {
                btn.BackColor = Color.FromArgb(248, 250, 252);
                btn.ForeColor = Color.FromArgb(30, 41, 59);
                btn.FlatAppearance.BorderSize = 1;
                btn.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            }
        }
    }
}
