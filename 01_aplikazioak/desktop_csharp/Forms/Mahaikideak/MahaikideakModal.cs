using Jatetxea.Desktop.Models;

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

        EguneratuUI();
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
