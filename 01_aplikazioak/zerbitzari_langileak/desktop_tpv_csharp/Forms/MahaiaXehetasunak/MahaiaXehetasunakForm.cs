using System.Drawing.Drawing2D;
using Jatetxea.Desktop.Models;
using Jatetxea.Desktop.Services;
using Jatetxea.Desktop.UI;

namespace Jatetxea.Desktop.Forms;

public partial class MahaiaXehetasunakForm : BaseTpvForm
{
    private readonly MahaiaDto _mahaia;
    private readonly LangileaDto _langilea;
    private readonly ApiService _api;
    private List<EskaeraDto> _eskaerak = new();
    private int _hautatutakoEskaeraIndex = 0;

    // Sukaldeko egoerak unean-unean hautemateko tenporizadoreak (Auto-Refresh eta Dir-Dir)
    private readonly System.Windows.Forms.Timer _autoRefreshTimer = new() { Interval = 1500 };
    private readonly System.Windows.Forms.Timer _pulseTimer = new() { Interval = 450 };
    private bool _pulseState = false;
    private bool _kargatzen = false;
    private bool _eragiketanAri = false; // Eragiketa bat (bidali, kobratu, etab.) martxan dagoenean true
    private readonly List<(Panel Card, Button BtnZerbitzatu, EskaeraLerroaDto Lerro)> _egindaControls = new();

    /// <summary>
    /// Visual Studio WinForms Designer-erako eraikitzailea
    /// </summary>
    public MahaiaXehetasunakForm()
    {
        InitializeComponent();
        _mahaia = new MahaiaDto { Id = 4, Zenbakia = 4, Edukiera = 4, Egoera = "Okupatuta" };
        _langilea = new LangileaDto { Id = 1, Izena = "Ane G.", Rola = "Zerbitzaria" };
        _api = new ApiService();
        KonfiguratuInterfazea();
    }

    public MahaiaXehetasunakForm(MahaiaDto mahaia, LangileaDto langilea, ApiService api)
    {
        InitializeComponent();
        _mahaia = mahaia;
        _langilea = langilea;
        _api = api;
        KonfiguratuInterfazea();
    }

    private void KonfiguratuInterfazea()
    {
        Text = $"T1 Jatetxea - Mahaia {_mahaia.Zenbakia} Xehetasunak";
        lblMahaiaTit.Text = $"T1 Jatetxea - Mahaia {_mahaia.Zenbakia}";
        lblZerbitzaria.Text = $"Zerbitzaria: {_langilea.Izena} ({_mahaia.Edukiera} pax)";

        // T1 Warm Hospitality v2.0 paleta + DoubleBuffering aplikatu
        Theme.ApplyWarmHospitalityTheme(this);
        BackColor = Theme.WarmLinen;
        pnlTopBar.BackColor = Theme.MarbleWhite;
        pnlEzkerra.BackColor = Theme.WarmLinen;
        pnlEskuina.BackColor = Theme.WarmLinen;
        pnlActions.BackColor = Theme.WarmLinen;
        pnlSummaryCard.BackColor = Theme.WarmLinen;
        pnlTicketCard.BackColor = Theme.WarmLinen;
        pnlTicketLines.BackColor = Theme.MarbleWhite;
        flowKontuFitxak.BackColor = Theme.MarbleWhite;

        lblMahaiaTit.Font = Theme.DisplayFont(15f);
        lblMahaiaTit.ForeColor = Theme.ChiantiPrimary;
        lblZerbitzaria.Font = Theme.BodyFont(10f, bold: false);
        lblZerbitzaria.ForeColor = Theme.WarmSlate;
        lblGuztira.Font = Theme.MonoFont(16.5f, bold: true);
        lblGuztira.ForeColor = Theme.ChiantiPrimary;
        lblAzpitotala.Font = Theme.MonoFont(10f, bold: false);
        lblBez.Font = Theme.MonoFont(10f, bold: false);

        Theme.StyleButton(btnAtzera, Theme.StoneMist, Theme.EspressoInk, Theme.WarmTaupe, radius: 10, fontSize: 10f);
        Theme.StyleButton(btnGehituEskaera, Theme.ChiantiPrimary, Theme.MarbleWhite, null, radius: 10, fontSize: 10.5f);
        Theme.StyleButton(btnGehituKontua, Theme.MarbleWhite, Theme.Success, Theme.WarmTaupe, radius: 10, fontSize: 10f);
        Theme.StyleButton(btnAldatuMahaiaz, Theme.StoneMist, Theme.WarmSlate, Theme.WarmTaupe, radius: 10, fontSize: 10f);
        Theme.ApplyRoundedRegion(btnSukalderaBidali, 10);
        Theme.ApplyRoundedRegion(btnKontuaEskatu, 10);
        Theme.ApplyRoundedRegion(btnEgoeraLibre, 10);
        Theme.ApplyRoundedRegion(btnEgoeraOkupatuta, 10);
        Theme.ApplyRoundedRegion(btnEgoeraGarbitzen, 10);

        // Barraren beheko marra
        pnlTopBar.Paint += (s, e) =>
        {
            using var pen = new Pen(Theme.WarmTaupe, 1.2f);
            e.Graphics.DrawLine(pen, 0, pnlTopBar.Height - 1, pnlTopBar.Width, pnlTopBar.Height - 1);
        };

        // Laburpen-txartela: Warm Hospitality urrezko ertzarekin (Figma 03 · MahaiaXehetasunakForm)
        pnlSummaryCard.Paint += (s, e) =>
        {
            Theme.DrawRoundedCard(e.Graphics, new Rectangle(0, 0, pnlSummaryCard.Width, pnlSummaryCard.Height), 14, Theme.MarbleWhite, Theme.GoldBorder, 1.5f);
        };

        pnlTicketCard.Paint += (s, e) =>
        {
            Theme.DrawRoundedCard(e.Graphics, new Rectangle(0, 0, pnlTicketCard.Width, pnlTicketCard.Height), 14, Theme.MarbleWhite, Theme.WarmTaupe, 1.2f);
        };

        // Botoien ekintzak lotu
        btnAtzera.Click += (s, e) => Close();
        btnSukalderaBidali.Click += async (s, e) => await SukalderaBidaliKlikatuaAsync();
        btnGehituEskaera.Click += (s, e) => GehituProduktuakKlikatua();
        btnGehituKontua.Click += async (s, e) => await GehituKontuBerriaKlikatuaAsync();
        btnKontuaEskatu.Click += async (s, e) => await KobratuEtaItxiAsync();
        btnAldatuMahaiaz.Click += (s, e) => AldatuMahaiazKlikatua();

        // Mahaiaren egoera aldatzeko botoiak (Libre, Okupatuta, Garbitzeko)
        btnEgoeraLibre.Click += async (s, e) => await AldatuEgoeraLibreAsync();
        btnEgoeraOkupatuta.Click += async (s, e) => await AldatuEgoeraOkupatutaAsync();
        btnEgoeraGarbitzen.Click += async (s, e) => await AldatuEgoeraGarbitzenAsync();

        EguneratuEgoeraBotoiak();

        // Auto-freskatzea eta Dir-Dir animazioa konfiguratu
        _autoRefreshTimer.Tick += async (s, e) => await AutoFreskatuAsync();
        _pulseTimer.Tick += (s, e) => AnimatuDirDir();

        FormClosing += (s, e) =>
        {
            _autoRefreshTimer.Stop();
            _autoRefreshTimer.Dispose();
            _pulseTimer.Stop();
            _pulseTimer.Dispose();
        };

        Load += async (s, e) =>
        {
            EgokituResponsiveDiseinua();
            await KargatuDatuakAsync();
            _autoRefreshTimer.Start();
            _pulseTimer.Start();
        };

        Resize += (s, e) => EgokituResponsiveDiseinua();
        pnlTicketLines.Resize += (s, e) => EgokituLerroTxartelak();

        Theme.AttachMainFormDpiAdaptation(this, () =>
        {
            EgokituResponsiveDiseinua();
            EgokituLerroTxartelak();
        });
    }

    private void EgokituResponsiveDiseinua()
    {
        int S(int px) => Theme.S(this, px);
        bool estua = ClientSize.Width < S(1080);
        bool osoEstua = ClientSize.Width < S(920);

        // 1. Goiko barra: DPI-rekin eskalatuta, gainjarpenik gabe
        pnlTopBar.Height = ClientSize.Height < S(580) ? S(68) : S(84);
        int yCenter = (pnlTopBar.Height - S(42)) / 2;

        btnAtzera.SetBounds(S(16), yCenter, osoEstua ? S(112) : S(142), S(42));

        lblMahaiaTit.Text = osoEstua ? $"MAHAIA {_mahaia.Zenbakia}" : $"T1 JATETXEA · MAHAIA {_mahaia.Zenbakia}";
        lblMahaiaTit.Font = Theme.DisplayFont(this, osoEstua ? 12.5f : 14.5f);
        lblMahaiaTit.SetBounds(btnAtzera.Right + S(12), yCenter, osoEstua ? S(165) : S(360), S(42));

        int btnH = S(40);
        int gap = S(8);
        if (estua)
        {
            lblEgoeraEtiketa.Visible = !osoEstua;
            int lblW = osoEstua ? 0 : S(62);
            int wLib = S(92);
            int wOku = S(110);
            int wGar = S(114);
            int totalW = lblW + wLib + wOku + wGar + gap * 3;
            pnlEgoeraChanger.Size = new Size(totalW, S(48));
            if (lblEgoeraEtiketa.Visible) lblEgoeraEtiketa.SetBounds(0, S(4), lblW, btnH);
            btnEgoeraLibre.SetBounds(lblW, S(4), wLib, btnH);
            btnEgoeraOkupatuta.SetBounds(btnEgoeraLibre.Right + gap, S(4), wOku, btnH);
            btnEgoeraGarbitzen.SetBounds(btnEgoeraOkupatuta.Right + gap, S(4), wGar, btnH);
            pnlEgoeraChanger.Location = new Point(pnlTopBar.Width - pnlEgoeraChanger.Width - S(16), (pnlTopBar.Height - pnlEgoeraChanger.Height) / 2);

            lblZerbitzaria.Visible = !osoEstua && (pnlEgoeraChanger.Left - lblMahaiaTit.Right > S(160));
            if (lblZerbitzaria.Visible)
            {
                lblZerbitzaria.SetBounds(lblMahaiaTit.Right + S(8), yCenter, pnlEgoeraChanger.Left - lblMahaiaTit.Right - S(16), S(42));
            }
        }
        else
        {
            lblEgoeraEtiketa.Visible = true;
            int lblW = S(68);
            int wLib = S(104);
            int wOku = S(122);
            int wGar = S(126);
            int totalW = lblW + wLib + wOku + wGar + gap * 3;
            pnlEgoeraChanger.Size = new Size(totalW, S(50));
            lblEgoeraEtiketa.SetBounds(0, S(5), lblW, btnH);
            btnEgoeraLibre.SetBounds(lblW, S(5), wLib, btnH);
            btnEgoeraOkupatuta.SetBounds(btnEgoeraLibre.Right + gap, S(5), wOku, btnH);
            btnEgoeraGarbitzen.SetBounds(btnEgoeraOkupatuta.Right + gap, S(5), wGar, btnH);
            pnlEgoeraChanger.Location = new Point(pnlTopBar.Width - pnlEgoeraChanger.Width - S(18), (pnlTopBar.Height - pnlEgoeraChanger.Height) / 2);

            lblZerbitzaria.Visible = true;
            int zLeft = lblMahaiaTit.Right + S(10);
            lblZerbitzaria.SetBounds(zLeft, yCenter, Math.Max(S(140), pnlEgoeraChanger.Left - zLeft - S(16)), S(42));
        }

        // 2. Eskuineko panelaren zabalera eta barruko txartel/botoien DPI geometria
        pnlEskuina.Width = osoEstua ? S(310) : (estua ? S(360) : S(430));
        pnlEskuina.Padding = new Padding(S(14), S(16), S(20), S(16));
        pnlEzkerra.Padding = osoEstua ? new Padding(S(12), S(10), S(8), S(12)) : new Padding(S(24), S(16), S(16), S(20));
        lblLerroLaburpena.Visible = ClientSize.Width >= S(900);

        int sumW = Math.Max(S(260), pnlEskuina.ClientSize.Width - pnlEskuina.Padding.Horizontal);
        pnlSummaryCard.Height = S(226);
        int padS = S(18);
        lblSummaryTitle.SetBounds(padS, S(14), sumW - padS * 2, S(28));
        pnlDividerSummary.SetBounds(padS, S(44), sumW - padS * 2, 1);
        lblAzpitotala.SetBounds(padS, S(54), sumW - padS * 2, S(26));
        lblBez.SetBounds(padS, S(84), sumW - padS * 2, S(26));
        lblGuztira.SetBounds(padS, S(118), sumW - padS * 2, S(42));
        lblEgoeraOrokorra.SetBounds(padS, S(170), sumW - padS * 2, S(38));

        int actY = S(12);
        int actH = S(50);
        int actGap = S(10);
        Button[] actBtns = { btnSukalderaBidali, btnGehituEskaera, btnGehituKontua, btnKontuaEskatu, btnAldatuMahaiaz };
        foreach (var b in actBtns)
        {
            b.SetBounds(0, actY, sumW, actH);
            actY += actH + actGap;
        }

        // 3. Ezkerreko lerro-txartelen zabalera eguneratu
        EgokituLerroTxartelak();
    }

    private void EgokituLerroTxartelak()
    {
        int w = Math.Max(340, pnlTicketLines.ClientSize.Width - 18);
        foreach (Control c in pnlTicketLines.Controls)
        {
            if (c is Panel card && card.Dock == DockStyle.None)
            {
                card.Width = w;
            }
        }
    }

    private async Task AutoFreskatuAsync()
    {
        // Eragiketa bat (bidali, kobratu) martxan dagoenean, ez freskatu
        if (_kargatzen || _eragiketanAri) return;
        _kargatzen = true;
        try
        {
            var berria = await _api.GetMahaiarenEskaeraGuztiakAsync(_mahaia.Id);
            // Bikoiztu eragiketa-babesa await ondoren (baliteke _eragiketanAri aldatu izana)
            if (_eragiketanAri) return;
            if (AldaketakDitu(_eskaerak, berria))
            {
                _eskaerak = berria;
                if (_eskaerak.Count == 0)
                {
                    _hautatutakoEskaeraIndex = 0;
                }
                else if (_hautatutakoEskaeraIndex >= _eskaerak.Count)
                {
                    _hautatutakoEskaeraIndex = _eskaerak.Count - 1;
                }
                EguneratuKontuFitxak();
                BistaratuHautatutakoEskaera();
            }
        }
        catch
        {
            // API konexio akatsak isilean kudeatu auto-freskatzean
        }
        finally
        {
            _kargatzen = false;
        }
    }

    private static bool AldaketakDitu(List<EskaeraDto> zaharra, List<EskaeraDto> berria)
    {
        if (zaharra == null || berria == null) return true;
        if (zaharra.Count != berria.Count) return true;
        for (int i = 0; i < zaharra.Count; i++)
        {
            var ez = zaharra[i];
            var eb = berria[i];
            if (ez.Id != eb.Id || ez.Egoera != eb.Egoera || ez.Guztira != eb.Guztira) return true;
            if (ez.Lerroak == null || eb.Lerroak == null) return true;
            if (ez.Lerroak.Count != eb.Lerroak.Count) return true;
            for (int j = 0; j < ez.Lerroak.Count; j++)
            {
                if (ez.Lerroak[j].Id != eb.Lerroak[j].Id ||
                    ez.Lerroak[j].Egoera != eb.Lerroak[j].Egoera ||
                    ez.Lerroak[j].Kantitatea != eb.Lerroak[j].Kantitatea ||
                    ez.Lerroak[j].UnitatePrezioa != eb.Lerroak[j].UnitatePrezioa ||
                    !string.Equals(ez.Lerroak[j].Oharrak, eb.Lerroak[j].Oharrak, StringComparison.Ordinal))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void AnimatuDirDir()
    {
        if (_egindaControls.Count == 0) return;

        _pulseState = !_pulseState;

        foreach (var item in _egindaControls)
        {
            if (item.Card.IsDisposed) continue;

            if (_pulseState)
            {
                // A fasea: distira urre-horia bizia (dir-dir atentzio deigarria)
                item.Card.BackColor = Color.FromArgb(254, 240, 138); // Urre-hori argitsua
                item.BtnZerbitzatu.BackColor = Color.FromArgb(180, 83, 9); // Amber iluna
                item.BtnZerbitzatu.Text = "PREST DAGO!";
            }
            else
            {
                // B fasea: berde distiratsua
                item.Card.BackColor = Color.FromArgb(220, 252, 231); // Berde argia
                item.BtnZerbitzatu.BackColor = Color.FromArgb(22, 101, 52); // Berde iluna
                item.BtnZerbitzatu.Text = "ZERBITZATU!";
            }
            item.Card.Invalidate();
        }

        // Botoi nagusia ere distiratsu jarri zerbitzatzeko prest badago
        if (btnSukalderaBidali.Enabled && btnSukalderaBidali.Text.Contains("EGINDA"))
        {
            btnSukalderaBidali.BackColor = _pulseState
                ? Color.FromArgb(180, 83, 9)
                : Color.FromArgb(22, 101, 52);
        }
    }

    private async Task KargatuDatuakAsync()
    {
        try
        {
            _eskaerak = await _api.GetMahaiarenEskaeraGuztiakAsync(_mahaia.Id);
        }
        catch
        {
            _eskaerak = new List<EskaeraDto>();
        }

        if (_eskaerak.Count == 0)
        {
            _hautatutakoEskaeraIndex = 0;
        }
        else if (_hautatutakoEskaeraIndex >= _eskaerak.Count)
        {
            _hautatutakoEskaeraIndex = _eskaerak.Count - 1;
        }

        EguneratuKontuFitxak();
        BistaratuHautatutakoEskaera();
    }

    private void EguneratuKontuFitxak()
    {
        UiPerf.RunFlickerFree(flowKontuFitxak, () =>
        {
            flowKontuFitxak.Controls.Clear();

            for (int i = 0; i < _eskaerak.Count; i++)
            {
                int index = i;
                var eskaera = _eskaerak[index];
                bool isAukeratua = index == _hautatutakoEskaeraIndex;
                bool platerikGabe = (eskaera.Lerroak == null || eskaera.Lerroak.Count == 0 || eskaera.Lerroak.Sum(l => l.Kantitatea) == 0) && eskaera.Guztira == 0m;

                if (platerikGabe)
                {
                    var pnlTab = new Panel
                    {
                        Size = new Size(255, 44),
                        Margin = new Padding(0, 0, 10, 0),
                        BackColor = isAukeratua ? Theme.ChiantiPrimary : Theme.DangerBg
                    };
                    Theme.ApplyRoundedRegion(pnlTab, 10);
                    pnlTab.Paint += (s, e) =>
                    {
                        Theme.DrawRoundedCard(e.Graphics, new Rectangle(0, 0, pnlTab.Width, pnlTab.Height), 10,
                            isAukeratua ? Theme.ChiantiPrimary : Theme.DangerBg,
                            isAukeratua ? Theme.ChiantiPrimary : Theme.DangerBorder, 1.3f);
                    };

                    var btnTab = new Button
                    {
                        Text = $"{index + 1}. Kontua (#{eskaera.Id}) — 0,00 €",
                        Font = Theme.BodyFont(9.2f, bold: isAukeratua),
                        BackColor = isAukeratua ? Theme.ChiantiPrimary : Theme.DangerBg,
                        ForeColor = isAukeratua ? Theme.MarbleWhite : Theme.Danger,
                        Location = new Point(2, 2),
                        Size = new Size(212, 40),
                        FlatStyle = FlatStyle.Flat,
                        Cursor = Cursors.Hand,
                        TextAlign = ContentAlignment.MiddleCenter
                    };
                    btnTab.FlatAppearance.BorderSize = 0;
                    btnTab.Click += (s, e) =>
                    {
                        _hautatutakoEskaeraIndex = index;
                        EguneratuKontuFitxak();
                        BistaratuHautatutakoEskaera();
                    };

                    var btnX = new Button
                    {
                        Text = "✕",
                        Font = Theme.BodyFont(11f, bold: true),
                        BackColor = isAukeratua ? Theme.PomodoroHover : Theme.DangerBg,
                        ForeColor = isAukeratua ? Theme.ChiantiTint : Theme.Danger,
                        Location = new Point(215, 2),
                        Size = new Size(38, 40),
                        FlatStyle = FlatStyle.Flat,
                        Cursor = Cursors.Hand
                    };
                    btnX.FlatAppearance.BorderSize = 0;

                    var tip = new ToolTip();
                    tip.SetToolTip(btnX, "Ezabatu kontu hutsa (platerik gabe)");

                    btnX.MouseEnter += (s, e) =>
                    {
                        btnX.BackColor = Theme.Danger;
                        btnX.ForeColor = Theme.MarbleWhite;
                    };
                    btnX.MouseLeave += (s, e) =>
                    {
                        btnX.BackColor = isAukeratua ? Theme.PomodoroHover : Theme.DangerBg;
                        btnX.ForeColor = isAukeratua ? Theme.ChiantiTint : Theme.Danger;
                    };

                    btnX.Click += async (s, e) => await EzabatuKontuHutsaAsync(eskaera, index);

                    pnlTab.Controls.Add(btnTab);
                    pnlTab.Controls.Add(btnX);
                    flowKontuFitxak.Controls.Add(pnlTab);
                }
                else
                {
                    var btn = new Button
                    {
                        Text = $"{index + 1}. Kontua (#{eskaera.Id}) — {eskaera.Guztira:F2} €",
                        Size = new Size(245, 44),
                        Margin = new Padding(0, 0, 10, 0)
                    };
                    Theme.StyleButton(btn,
                        isAukeratua ? Theme.ChiantiPrimary : Theme.MarbleWhite,
                        isAukeratua ? Theme.MarbleWhite : Theme.EspressoInk,
                        isAukeratua ? null : Theme.WarmTaupe,
                        radius: 10,
                        fontSize: 9.5f,
                        bold: isAukeratua);

                    btn.Click += (s, e) =>
                    {
                        _hautatutakoEskaeraIndex = index;
                        EguneratuKontuFitxak();
                        BistaratuHautatutakoEskaera();
                    };

                    flowKontuFitxak.Controls.Add(btn);
                }
            }
        }, preserveScroll: true);
    }

    private void BistaratuHautatutakoEskaera()
    {
        EguneratuEgoeraBotoiak();

        if (_eskaerak.Count == 0 || _hautatutakoEskaeraIndex >= _eskaerak.Count)
        {
            pnlTicketLines.Controls.Clear();
            bool isGarbitzen = _mahaia.Egoera.Equals("Garbitzen", StringComparison.OrdinalIgnoreCase);
            bool isLibre = _mahaia.Egoera.Equals("Libre", StringComparison.OrdinalIgnoreCase);
            string hutsikText = isGarbitzen
                ? $"Mahaia {_mahaia.Zenbakia} 'Garbitzen' egoeran dago.\nKontu guztiak kobratuta daude.\n\nSakatu beheko botoia edo goiko 'Libre' botoia mahaia libratzeko."
                : (isLibre
                    ? $"Mahaia {_mahaia.Zenbakia} 'Libre' dago.\n\nEskari berria hasteko, sakatu beheko '+ PRODUKTUAK GEHITU' botoia."
                    : $"Mahaia {_mahaia.Zenbakia} 'Okupatuta' dago baina ez du eskari aktiborik.\n\nSakatu beheko '+ PRODUKTUAK GEHITU' botoia.");

            var lblHutsik = new Label
            {
                Text = hutsikText,
                Font = new Font("Segoe UI", 12F, FontStyle.Regular),
                ForeColor = Color.FromArgb(100, 116, 139),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlTicketLines.Controls.Add(lblHutsik);
            lblEskaeraInfo.Text = isGarbitzen ? "Mahaia Garbitzen" : (isLibre ? "Mahaia Librea" : "Mahaia Okupatuta");
            lblLerroLaburpena.Text = "0 plater";
            lblGuztira.Text = "GUZTIRA: 0,00 €";
            lblAzpitotala.Text = "Azpitotala: 0,00 €";
            lblBez.Text = "BEZ (%10 barne): 0,00 €";
            lblEgoeraOrokorra.Text = isGarbitzen 
                ? "Mahaia garbitu eta prest utzi bezero berrientzat" 
                : (isLibre ? "Mahaia libre dago bezero berrientzat" : "Eskaerarik gabe");
            lblEgoeraOrokorra.ForeColor = isGarbitzen || isLibre ? Color.FromArgb(22, 101, 52) : Color.FromArgb(100, 116, 139);

            if (isGarbitzen)
            {
                btnSukalderaBidali.Text = "✓ MAHAIA LIBRE JARRI";
                btnSukalderaBidali.BackColor = Color.FromArgb(22, 101, 52);
                btnSukalderaBidali.Enabled = true;
            }
            else
            {
                btnSukalderaBidali.Text = isLibre ? "MAHAIA LIBRE DAGO" : "EZ DAGO ESKAERARIK";
                btnSukalderaBidali.BackColor = Color.FromArgb(71, 85, 105);
                btnSukalderaBidali.Enabled = false;
            }
            return;
        }

        var eskaera = _eskaerak[_hautatutakoEskaeraIndex];
        int targetWidth = pnlTicketLines.ClientSize.Width > 100 ? pnlTicketLines.ClientSize.Width - 16 : 740;

        // 1. Egoera eta Laburpena
        int guztiraKop = eskaera.Lerroak.Sum(l => l.Kantitatea);
        int zaiKop = eskaera.Lerroak.Where(l => NormalizatuEgoera(l.Egoera) == "Zai").Sum(l => l.Kantitatea);
        int martxanKop = eskaera.Lerroak.Where(l => NormalizatuEgoera(l.Egoera) == "Martxan").Sum(l => l.Kantitatea);
        int egindaKop = eskaera.Lerroak.Where(l => NormalizatuEgoera(l.Egoera) == "Eginda").Sum(l => l.Kantitatea);
        int zerbitzatuaKop = eskaera.Lerroak.Where(l => NormalizatuEgoera(l.Egoera) == "Zerbitzatua").Sum(l => l.Kantitatea);

        lblEskaeraInfo.Text = $"{_hautatutakoEskaeraIndex + 1}. Kontua: Eskaera #{eskaera.Id} · Egoera: {eskaera.Egoera}";
        lblLerroLaburpena.Text = $"{guztiraKop} plater ({zaiKop} Zai · {martxanKop} Martxan · {egindaKop} Eginda · {zerbitzatuaKop} Zerbitzatuta)";

        // 2. Kaxa eta Botoiaren Egoera
        decimal subtotal = Math.Round(eskaera.Guztira / 1.10m, 2);
        decimal bez = eskaera.Guztira - subtotal;
        lblAzpitotala.Text = $"Azpitotala:  {subtotal:F2} €";
        lblBez.Text = $"BEZ (%10 barne):  {bez:F2} €";
        lblGuztira.Text = $"GUZTIRA:  {eskaera.Guztira:F2} €";

        if (guztiraKop == 0)
        {
            lblEgoeraOrokorra.Text = "Ez dago platerik kontu honetan (0,00 €)";
            lblEgoeraOrokorra.ForeColor = Color.FromArgb(100, 116, 139);
            btnSukalderaBidali.Text = "EZ DAGO PLATERIK";
            btnSukalderaBidali.BackColor = Color.FromArgb(71, 85, 105); // Grisa
            btnSukalderaBidali.Enabled = false;

            btnKontuaEskatu.Text = "ITXI / EZABATU KONTUA";
            btnKontuaEskatu.BackColor = Color.FromArgb(220, 38, 38); // Gorria
            btnKontuaEskatu.ForeColor = Color.White;

            _egindaControls.Clear();
            pnlTicketLines.SuspendLayout();
            pnlTicketLines.Controls.Clear();

            var pnlHutsa = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                ColumnCount = 1,
                RowCount = 6
            };
            pnlHutsa.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            pnlHutsa.RowStyles.Add(new RowStyle(SizeType.Percent, 32f));
            pnlHutsa.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            pnlHutsa.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            pnlHutsa.RowStyles.Add(new RowStyle(SizeType.Absolute, 20f));
            pnlHutsa.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            pnlHutsa.RowStyles.Add(new RowStyle(SizeType.Percent, 68f));

            var lblTit = new Label
            {
                Text = "Kontu honek ez du platerik (0,00 €)",
                Font = new Font("Segoe UI", 13.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                AutoSize = true,
                Margin = new Padding(12, 0, 12, 6)
            };

            var lblSub = new Label
            {
                Text = "Sakatu beheko '+ PRODUKTUAK GEHITU' platerak eskatzeko,\nedo fitxako '✕' / azpiko botoia kontu hau itxi eta ezabatzeko.",
                Font = new Font("Segoe UI", 10.5F, FontStyle.Regular),
                ForeColor = Color.FromArgb(148, 163, 184),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                AutoSize = true,
                Margin = new Padding(12, 0, 12, 8)
            };

            var btnEzabatuZentroa = new Button
            {
                Text = $"🗑 Itxi / Ezabatu {_hautatutakoEskaeraIndex + 1}. Kontua (#{eskaera.Id})",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                BackColor = Color.FromArgb(254, 242, 242),
                ForeColor = Color.FromArgb(220, 38, 38),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(340, 48),
                Anchor = AnchorStyles.None,
                Cursor = Cursors.Hand
            };
            btnEzabatuZentroa.FlatAppearance.BorderColor = Color.FromArgb(252, 165, 165);
            btnEzabatuZentroa.Click += async (s, e) => await EzabatuKontuHutsaAsync(eskaera, _hautatutakoEskaeraIndex);

            pnlHutsa.Controls.Add(lblTit, 0, 1);
            pnlHutsa.Controls.Add(lblSub, 0, 2);
            pnlHutsa.Controls.Add(btnEzabatuZentroa, 0, 4);
            pnlTicketLines.Controls.Add(pnlHutsa);
            pnlTicketLines.ResumeLayout(true);
            return;
        }
        else
        {
            btnKontuaEskatu.Text = "KONTUA ESKATU / KOBRATU";
            btnKontuaEskatu.BackColor = Color.FromArgb(241, 245, 249);
            btnKontuaEskatu.ForeColor = Color.FromArgb(30, 41, 59);
        }

        if (zaiKop > 0)
        {
            lblEgoeraOrokorra.Text = $"{zaiKop} plater sukaldera bidaltzeke (Zai)";
            lblEgoeraOrokorra.ForeColor = Color.FromArgb(180, 83, 9);
            btnSukalderaBidali.Text = $"SUKALDERA BIDALI ({zaiKop} ZAI)";
            btnSukalderaBidali.BackColor = Color.FromArgb(22, 101, 52); // Berdea
            btnSukalderaBidali.Enabled = true;
        }
        else if (egindaKop > 0)
        {
            lblEgoeraOrokorra.Text = $"{egindaKop} plater zerbitzatzeko prest daude!";
            lblEgoeraOrokorra.ForeColor = Color.FromArgb(21, 128, 61);
            btnSukalderaBidali.Text = $"PLATERAK ZERBITZATU ({egindaKop} EGINDA)";
            btnSukalderaBidali.BackColor = Color.FromArgb(136, 19, 55); // Granatea
            btnSukalderaBidali.Enabled = true;
        }
        else if (martxanKop > 0)
        {
            lblEgoeraOrokorra.Text = $"{martxanKop} plater sukaldean prestatzen ari dira";
            lblEgoeraOrokorra.ForeColor = Color.FromArgb(29, 78, 216);
            btnSukalderaBidali.Text = "SUKALDEAN MARTXAN...";
            btnSukalderaBidali.BackColor = Color.FromArgb(71, 85, 105); // Grisa
            btnSukalderaBidali.Enabled = false;
        }
        else
        {
            lblEgoeraOrokorra.Text = "Plater guztiak mahaira zerbitzatuta daude";
            lblEgoeraOrokorra.ForeColor = Color.FromArgb(22, 101, 52);
            btnSukalderaBidali.Text = "ZERBITZATUTA";
            btnSukalderaBidali.BackColor = Color.FromArgb(71, 85, 105); // Grisa
            btnSukalderaBidali.Enabled = false;
        }

        // 3. Plateren zerrenda eraiki (parpadeorik gabe + scroll posizioa mantenduz)
        UiPerf.RunFlickerFree(pnlTicketLines, () =>
        {
            _egindaControls.Clear();
            pnlTicketLines.Controls.Clear();

            int y = 8;

            // Platerak lehentasunaren arabera ordenatu:
            // 1. EGINDA (zerbitzatzeko prest daudenak lehenengo, GORAN!)
            // 2. MARTXAN (sukaldean prestatzen ari direnak)
            // 3. ZAI (oraindik sukaldera bidali gabekoak)
            // 4. ZERBITZATUA (dagoeneko mahaira eramandakoak, AZKENA/BEHERAN)
            int LortuLehentasuna(string egoera) => NormalizatuEgoera(egoera) switch
            {
                "Eginda" => 0,      // Zerbitzatzeko prest: LEHENENGO (GORAN)
                "Martxan" => 1,     // Sukaldean prestatzen
                "Zai" => 2,         // Sukaldera bidaltzeke
                "Zerbitzatua" => 3, // Zerbitzatuta: AZKENA (BEHERAN)
                _ => 4
            };

            var lerroakOrdenatuta = eskaera.Lerroak
                .OrderBy(l => LortuLehentasuna(l.Egoera))
                .ThenBy(l => l.Id)
                .ToList();

            foreach (var lerro in lerroakOrdenatuta)
            {
                var rowCard = SortuLerroTxartela(lerro, targetWidth);
                rowCard.Location = new Point(4, y);
                pnlTicketLines.Controls.Add(rowCard);
                y += rowCard.Height + 10;
            }
        }, preserveScroll: true);
    }

    private Control SortuLerroTxartela(EskaeraLerroaDto lerro, int cardWidth)
    {
        string lerroEgoera = NormalizatuEgoera(lerro.Egoera);
        var (badgeText, badgeBg, badgeFg, badgeBorder) = GetBadgeStyle(lerroEgoera);

        bool hasOharra = !string.IsNullOrWhiteSpace(lerro.Oharrak);
        int cardHeight = hasOharra ? 96 : 72;

        var card = new Panel
        {
            Width = cardWidth,
            Height = cardHeight,
            BackColor = lerroEgoera == "Eginda" ? Color.FromArgb(220, 252, 231) : Color.FromArgb(248, 250, 252),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        card.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Color borderCol;
            float borderWidth;
            if (lerroEgoera == "Eginda")
            {
                borderCol = _pulseState ? Color.FromArgb(234, 179, 8) : Color.FromArgb(34, 197, 94);
                borderWidth = 2.5f;
            }
            else if (lerroEgoera == "Zai")
            {
                borderCol = Color.FromArgb(253, 230, 138);
                borderWidth = 1.2f;
            }
            else
            {
                borderCol = Color.FromArgb(226, 232, 240);
                borderWidth = 1.2f;
            }
            using var pen = new Pen(borderCol, borderWidth);
            e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
        };

        // 1. Kantitatea (2x)
        var lblKop = new Label
        {
            Text = $"{lerro.Kantitatea}x",
            Font = new Font("Segoe UI", 13F, FontStyle.Bold),
            ForeColor = Color.FromArgb(136, 19, 55),
            Location = new Point(14, 16),
            Size = new Size(50, 40),
            TextAlign = ContentAlignment.MiddleLeft,
            UseCompatibleTextRendering = false
        };
        card.Controls.Add(lblKop);

        // 2. Plateraren Izena
        int nameWidth = lerroEgoera == "Zai" ? Math.Max(160, cardWidth - 460) : Math.Max(200, cardWidth - 395);
        var lblIzena = new Label
        {
            Text = lerro.PlaterIzena,
            Font = new Font("Segoe UI", 12F, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 41, 59),
            Location = new Point(68, 16),
            Size = new Size(nameWidth, 40),
            TextAlign = ContentAlignment.MiddleLeft,
            UseCompatibleTextRendering = false
        };
        card.Controls.Add(lblIzena);

        // 3. Oharrak (azpian)
        if (hasOharra)
        {
            var lblOharra = new Label
            {
                Text = $"Oharra: {lerro.Oharrak}",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(68, 58),
                Size = new Size(nameWidth, 28),
                TextAlign = ContentAlignment.MiddleLeft,
                UseCompatibleTextRendering = false
            };
            card.Controls.Add(lblOharra);
        }

        // 4. Prezioa eta Ekintzak ([ZAI: Sukaldera banaka & Ezabatu] / [EGINDA: Zerbitzatu] / [MARTXAN/ZERBITZATUA: Txapa])
        if (lerroEgoera == "Zai")
        {
            // Prezioa
            var lblPrezioa = new Label
            {
                Text = $"{lerro.Azpitotala:F2} €",
                Font = new Font("Segoe UI", 12.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(cardWidth - 375, 16),
                Size = new Size(110, 40),
                TextAlign = ContentAlignment.MiddleRight,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                UseCompatibleTextRendering = false
            };
            card.Controls.Add(lblPrezioa);

            // Botoia: Sukaldera banaka bidali (▶ Sukaldera)
            var btnBidaliBanaka = new Button
            {
                Location = new Point(cardWidth - 250, 16),
                Size = new Size(130, 40),
                BackColor = Color.FromArgb(22, 101, 52), // Berde iluna
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Text = "▶ Sukaldera",
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                UseCompatibleTextRendering = false
            };
            btnBidaliBanaka.FlatAppearance.BorderSize = 0;
            btnBidaliBanaka.Click += async (s, e) =>
            {
                if (_eragiketanAri) return;
                _eragiketanAri = true;
                btnBidaliBanaka.Enabled = false;
                btnBidaliBanaka.Text = "...";
                try
                {
                    bool ok = await _api.EguneratuLerroEgoeraAsync(lerro.Id, "Sukaldean");
                    if (ok)
                    {
                        lerro.Egoera = "Sukaldean";
                        _mahaia.Egoera = "Okupatuta";
                        await KargatuDatuakAsync();
                    }
                    else
                    {
                        MessageBox.Show("Ezin izan da platera sukaldera bidali.", "Errorea", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        btnBidaliBanaka.Enabled = true;
                        btnBidaliBanaka.Text = "▶ Sukaldera";
                    }
                }
                finally
                {
                    _eragiketanAri = false;
                }
            };
            card.Controls.Add(btnBidaliBanaka);

            // Botoia: Sukaldera bidali gabeko platera ezabatu (🗑 Ezabatu)
            var btnEzabatu = new Button
            {
                Location = new Point(cardWidth - 110, 16),
                Size = new Size(100, 40),
                BackColor = Color.FromArgb(254, 242, 242), // Gorri leuna
                ForeColor = Color.FromArgb(185, 28, 28), // Gorri iluna
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Text = "🗑 Ezabatu",
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                UseCompatibleTextRendering = false
            };
            btnEzabatu.FlatAppearance.BorderColor = Color.FromArgb(252, 165, 165);
            btnEzabatu.FlatAppearance.BorderSize = 1;
            btnEzabatu.Click += async (s, e) =>
            {
                if (_eragiketanAri) return;
                var confirm = MessageBox.Show(
                    $"Ziur zaude '{lerro.PlaterIzena}' (x{lerro.Kantitatea}) eskaritik ezabatu nahi duzula?\n\n(Platera oraindik ez da sukaldera bidali, stocka berreskuratuko da)",
                    "Platera Ezabatu",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirm != DialogResult.Yes) return;

                _eragiketanAri = true;
                btnEzabatu.Enabled = false;
                btnEzabatu.Text = "...";
                try
                {
                    var (ok, mezua) = await _api.EzabatuEskaeraLerroaAsync(lerro.Id);
                    if (ok)
                    {
                        await KargatuDatuakAsync();
                    }
                    else
                    {
                        MessageBox.Show(mezua, "Errorea platera ezabatzean", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        btnEzabatu.Enabled = true;
                        btnEzabatu.Text = "🗑 Ezabatu";
                    }
                }
                finally
                {
                    _eragiketanAri = false;
                }
            };
            card.Controls.Add(btnEzabatu);
        }
        else if (lerroEgoera == "Eginda")
        {
            var lblPrezioa = new Label
            {
                Text = $"{lerro.Azpitotala:F2} €",
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(cardWidth - 305, 16),
                Size = new Size(125, 40),
                TextAlign = ContentAlignment.MiddleRight,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                UseCompatibleTextRendering = false
            };
            card.Controls.Add(lblPrezioa);

            var btnZerbitzatu = new Button
            {
                Location = new Point(cardWidth - 170, 16),
                Size = new Size(155, 40),
                BackColor = Color.FromArgb(22, 101, 52), // Berde dotorea
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Text = "ZERBITZATU!",
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                UseCompatibleTextRendering = false
            };
            btnZerbitzatu.FlatAppearance.BorderSize = 0;
            btnZerbitzatu.Click += async (s, e) =>
            {
                btnZerbitzatu.Enabled = false;
                btnZerbitzatu.Text = "...";
                bool ok = await _api.EguneratuLerroEgoeraAsync(lerro.Id, "Zerbitzatua");
                if (ok)
                {
                    lerro.Egoera = "Zerbitzatua";
                    await KargatuDatuakAsync();
                }
                else
                {
                    MessageBox.Show("Ezin izan da platera zerbitzatuta markatu.", "Errorea", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnZerbitzatu.Enabled = true;
                }
            };
            card.Controls.Add(btnZerbitzatu);

            _egindaControls.Add((card, btnZerbitzatu, lerro));
        }
        else
        {
            var lblPrezioa = new Label
            {
                Text = $"{lerro.Azpitotala:F2} €",
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(cardWidth - 305, 16),
                Size = new Size(125, 40),
                TextAlign = ContentAlignment.MiddleRight,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                UseCompatibleTextRendering = false
            };
            card.Controls.Add(lblPrezioa);

            var pnlBadge = new Panel
            {
                Location = new Point(cardWidth - 170, 16),
                Size = new Size(155, 40),
                BackColor = badgeBg,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            pnlBadge.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using var p = new Pen(badgeBorder, 1.2f);
                e.Graphics.DrawRectangle(p, 0, 0, pnlBadge.Width - 1, pnlBadge.Height - 1);
            };

            var lblBadge = new Label
            {
                Dock = DockStyle.Fill,
                Text = badgeText,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = badgeFg,
                TextAlign = ContentAlignment.MiddleCenter,
                UseCompatibleTextRendering = false
            };
            pnlBadge.Controls.Add(lblBadge);
            card.Controls.Add(pnlBadge);
        }

        return card;
    }

    private static string NormalizatuEgoera(string egoera)
    {
        string e = (egoera ?? "").Trim().ToLowerInvariant();
        if (e.Contains("zerbitzat")) return "Zerbitzatua";
        if (e.Contains("egin") || e.Contains("prestat")) return "Eginda";
        if (e.Contains("martxan") || e.Contains("sukalde")) return "Martxan";
        return "Zai";
    }

    private static (string BadgeText, Color BgColor, Color FgColor, Color BorderColor) GetBadgeStyle(string egoera)
    {
        return egoera switch
        {
            "Martxan" => ("MARTXAN", Color.FromArgb(219, 234, 254), Color.FromArgb(29, 78, 216), Color.FromArgb(147, 197, 253)),
            "Eginda" => ("EGINDA", Color.FromArgb(220, 252, 231), Color.FromArgb(21, 128, 61), Color.FromArgb(134, 239, 172)),
            "Zerbitzatua" => ("ZERBITZATUA", Color.FromArgb(241, 245, 249), Color.FromArgb(71, 85, 105), Color.FromArgb(203, 213, 225)),
            _ => ("ZAI", Color.FromArgb(254, 243, 199), Color.FromArgb(180, 83, 9), Color.FromArgb(253, 230, 138))
        };
    }

    /// <summary>
    /// Zai dauden platerak sukaldera bidaltzen ditu, edo Eginda daudenak zerbitzatuta markatzen ditu
    /// </summary>
    private async Task SukalderaBidaliKlikatuaAsync()
    {
        if (_eragiketanAri) return;

        // Mahaia Garbitzen badago eta eskaerarik ez badago, zuzenean 'Libre' jarri
        if (_mahaia.Egoera.Equals("Garbitzen", StringComparison.OrdinalIgnoreCase) && _eskaerak.Count == 0)
        {
            _eragiketanAri = true;
            try
            {
                btnSukalderaBidali.Enabled = false;
                btnSukalderaBidali.Text = "LIBRE UZTEN...";
                bool ok = await _api.EguneratuMahaiarenEgoeraAsync(_mahaia.Id, "Libre");
                if (ok)
                {
                    _mahaia.Egoera = "Libre";
                    MessageBox.Show($"Mahaia {_mahaia.Zenbakia} 'Librea' egoerara aldatu da.", "Mahaia Garbituta", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Close();
                }
                else
                {
                    MessageBox.Show("Ezin izan da mahaiaren egoera eguneratu APIan.", "Errorea", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnSukalderaBidali.Enabled = true;
                    btnSukalderaBidali.Text = "✓ MAHAIA LIBRE JARRI";
                }
            }
            finally
            {
                _eragiketanAri = false;
            }
            return;
        }

        if (_eskaerak.Count == 0 || _hautatutakoEskaeraIndex >= _eskaerak.Count) return;
        _eragiketanAri = true;
        try
        {
        var eskaera = _eskaerak[_hautatutakoEskaeraIndex];
        var zaiLerroak = eskaera.Lerroak.Where(l => NormalizatuEgoera(l.Egoera) == "Zai").ToList();
        var egindaLerroak = eskaera.Lerroak.Where(l => NormalizatuEgoera(l.Egoera) == "Eginda").ToList();

        if (zaiLerroak.Count > 0)
        {
            btnSukalderaBidali.Enabled = false;
            btnSukalderaBidali.Text = "SUKALDERA BIDALTZEN...";

            // 1. API bidez egoerak eguneratu
            var (ok, mezua) = await _api.BidaliSukalderaAsync(eskaera.Id);

            // 2. Tokiko memoriako lerroak 'Martxan' jarri feedback azkarra emateko (sukaldeko platerak soilik)
            foreach (var l in zaiLerroak)
            {
                bool isEdaria = l.Kategoria.Equals("Edariak", StringComparison.OrdinalIgnoreCase) ||
                                l.Kategoria.Equals("Kafeak", StringComparison.OrdinalIgnoreCase) ||
                                l.Kategoria.Equals("Barra", StringComparison.OrdinalIgnoreCase);
                if (!isEdaria)
                {
                    l.Egoera = "Sukaldean";
                }
            }
            eskaera.Egoera = "Sukaldean";
            _mahaia.Egoera = "Okupatuta";

            // Mahaiaren egoera eguneratu datu-basean
            _ = _api.EguneratuMahaiarenEgoeraAsync(_mahaia.Id, "Okupatuta");

            EguneratuEgoeraBotoiak();
            await KargatuDatuakAsync();

            MessageBox.Show(
                $"{zaiLerroak.Count} plater arrakastaz bidali dira sukaldera!\n\nSukaldean martxan jarrita daude.",
                "Sukaldera Bidalita",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        else if (egindaLerroak.Count > 0)
        {
            btnSukalderaBidali.Enabled = false;
            btnSukalderaBidali.Text = "ZERBITZATZEN...";

            bool guztiakOndo = true;
            foreach (var l in egindaLerroak)
            {
                bool ok = await _api.EguneratuLerroEgoeraAsync(l.Id, "Zerbitzatua");
                if (ok) l.Egoera = "Zerbitzatua";
                else guztiakOndo = false;
            }

            await KargatuDatuakAsync();

            if (guztiakOndo)
            {
                MessageBox.Show(
                    $"{egindaLerroak.Count} plater arrakastaz zerbitzatu dira mahaira!",
                    "Zerbitzatuta",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Plater batzuk ezin izan dira zerbitzatuta markatu.", "Abisua", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        else
        {
            MessageBox.Show("Ez dago zerbitzatzeko prest dagoen platerik une honetan.", "Informazioa", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        }
        finally
        {
            _eragiketanAri = false;
        }
    }

    /// <summary>
    /// Aukeratutako eskaerari plater / lerro gehiago gehitzen dizkio EskaeraForm bidez
    /// </summary>
    private void GehituProduktuakKlikatua()
    {
        int? dagoenekoEskaeraId = _eskaerak.Count > _hautatutakoEskaeraIndex ? _eskaerak[_hautatutakoEskaeraIndex].Id : null;

        using var eskaeraForm = new EskaeraForm(_mahaia, _langilea, _api, _mahaia.Edukiera, dagoenekoEskaeraId);
        Theme.PlaceOnActiveScreen(eskaeraForm, this);
        Hide();
        eskaeraForm.ShowDialog();
        Theme.PlaceOnActiveScreen(this);
        Show();
        _ = KargatuDatuakAsync();
    }

    /// <summary>
    /// Mahai berean beste kontu / eskaera bat irekitzen du (mahaikideek aparte ordaintzeko)
    /// </summary>
    private async Task GehituKontuBerriaKlikatuaAsync()
    {
        var konfirmatu = MessageBox.Show(
            $"Mahaia {_mahaia.Zenbakia}-rentzat beste kontu / eskaera berri bat sortu nahi duzu?\n\n(Mahaikideen kontuak bereizteko baliagarria da)",
            "Kontu Berria Gehitu",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (konfirmatu != DialogResult.Yes) return;

        int pertsonaKop = _mahaia.Edukiera > 0 ? _mahaia.Edukiera : 1;
        var (arrakasta, mezua, berria) = await _api.SortuEskaeraAsync(_mahaia.Id, _langilea.Id, pertsonaKop);
        if (!arrakasta || berria == null)
        {
            MessageBox.Show(
                $"Ezin izan da kontu/eskaera berririk sortu APIan:\n{mezua}",
                "Errorea",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return;
        }

        _eskaerak.Add(berria);
        _hautatutakoEskaeraIndex = _eskaerak.Count - 1;

        EguneratuKontuFitxak();
        BistaratuHautatutakoEskaera();

        // Zuzenean produktuen hautatzailea ireki
        GehituProduktuakKlikatua();
    }

    /// <summary>
    /// Platerik gabeko kontu / eskaera huts bat ezabatzen du APIan eta interfazetik kentzen du.
    /// </summary>
    private async Task EzabatuKontuHutsaAsync(EskaeraDto eskaera, int index, bool galdetu = true)
    {
        if (_eragiketanAri) return;

        if (galdetu)
        {
            var confirm = MessageBox.Show(
                $"Ziur zaude {index + 1}. Kontua (#{eskaera.Id}) itxi eta ezabatu nahi duzula?\n\nEz du platerik eta kontua behin betiko ezabatuko da.",
                "Kontua Itxi / Ezabatu",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;
        }

        _eragiketanAri = true;
        try
        {
            var (ok, mezua) = await _api.EzabatuEskaeraAsync(eskaera.Id);
            if (ok)
            {
                _eskaerak.RemoveAt(index);
                if (_eskaerak.Count == 0)
                {
                    _hautatutakoEskaeraIndex = 0;
                    // Kontu gehiago ez badaude, mahaia automatikoki Libre geratuko da!
                    await _api.EguneratuMahaiarenEgoeraAsync(_mahaia.Id, "Libre");
                    _mahaia.Egoera = "Libre";
                    EguneratuEgoeraBotoiak();
                    MessageBox.Show(
                        $"Kontua arrakastaz itxi da.\nMahai honetan ez da kontu gehiagorik geratzen eta mahaia 'Libre' geratu da.",
                        "Kontua Itxita",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    _hautatutakoEskaeraIndex = Math.Max(0, Math.Min(_hautatutakoEskaeraIndex, _eskaerak.Count - 1));
                }

                await KargatuDatuakAsync();
            }
            else
            {
                MessageBox.Show(mezua, "Errorea kontua ezabatzean", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        finally
        {
            _eragiketanAri = false;
        }
    }

    /// <summary>
    /// Ordainketa pantaila profesionala irekitzen du (Eskudirua, Txartela, Bizum) eta tiketa sortzen du
    /// </summary>
    private async Task KobratuEtaItxiAsync()
    {
        if (_eragiketanAri) return;
        if (_eskaerak.Count == 0 || _hautatutakoEskaeraIndex >= _eskaerak.Count)
        {
            MessageBox.Show("Ez dago kobratzeko eskaerarik.", "Informazioa", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var eskaera = _eskaerak[_hautatutakoEskaeraIndex];

        // Kontua hutsik badago (platerik gabe eta 0,00 €), zuzenean ixteko proposatu
        bool platerikGabe = (eskaera.Lerroak == null || eskaera.Lerroak.Count == 0 || eskaera.Lerroak.Sum(l => l.Kantitatea) == 0) && eskaera.Guztira == 0m;
        if (platerikGabe)
        {
            var delConfirm = MessageBox.Show(
                $"{_hautatutakoEskaeraIndex + 1}. Kontuak ez du platerik (0,00 €).\n\nNahi duzu kontu huts hau itxi eta ezabatu?",
                "Kontu Hutsa Itxi",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (delConfirm == DialogResult.Yes)
            {
                await EzabatuKontuHutsaAsync(eskaera, _hautatutakoEskaeraIndex, galdetu: false);
            }
            return;
        }

        _eragiketanAri = true;
        try
        {
        using var ordainketaForm = new OrdainketaForm(_mahaia, eskaera, _langilea, _api);
        var emaitza = ordainketaForm.ShowDialog(this);

        if (emaitza == DialogResult.OK)
        {
            _eskaerak.RemoveAt(_hautatutakoEskaeraIndex);

            if (_eskaerak.Count == 0)
            {
                await _api.EguneratuMahaiarenEgoeraAsync(_mahaia.Id, "Garbitzen");
                MessageBox.Show(
                    $"Mahaia {_mahaia.Zenbakia}-ren kontu guztiak kobratuta eta tiketa sortuta daude.\nMahaia 'Garbitzen' egoeran geratu da.",
                    "Kontua Kobratuta",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                Close();
            }
            else
            {
                _hautatutakoEskaeraIndex = Math.Max(0, _hautatutakoEskaeraIndex - 1);
                EguneratuKontuFitxak();
                BistaratuHautatutakoEskaera();
                MessageBox.Show(
                    "Kontua arrakastaz kobratu da eta tiketa sortu da.\nMahai honetan beste kontu batzuk irekita daude oraindik.",
                    "Kontua Kobratuta",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }
        }
        finally
        {
            _eragiketanAri = false;
        }
    }

    private void EguneratuEgoeraBotoiak()
    {
        string egoera = _mahaia.Egoera;
        bool isLibre = egoera.Equals("Libre", StringComparison.OrdinalIgnoreCase);
        bool isOkupatuta = egoera.Equals("Okupatuta", StringComparison.OrdinalIgnoreCase);
        bool isGarbitzen = egoera.Equals("Garbitzen", StringComparison.OrdinalIgnoreCase) ||
                           egoera.Equals("Garbitzeko", StringComparison.OrdinalIgnoreCase);

        // 1. Libre botoia
        btnEgoeraLibre.Text = "Libre";
        btnEgoeraLibre.BackColor = isLibre ? Color.FromArgb(220, 252, 231) : Color.White;
        btnEgoeraLibre.ForeColor = isLibre ? Color.FromArgb(22, 101, 52) : Color.FromArgb(100, 116, 139);
        btnEgoeraLibre.FlatAppearance.BorderColor = isLibre ? Color.FromArgb(34, 197, 94) : Color.FromArgb(203, 213, 225);
        btnEgoeraLibre.FlatAppearance.BorderSize = isLibre ? 2 : 1;

        // 2. Okupatuta botoia
        btnEgoeraOkupatuta.Text = "Okupatuta";
        btnEgoeraOkupatuta.BackColor = isOkupatuta ? Color.FromArgb(51, 65, 85) : Color.White;
        btnEgoeraOkupatuta.ForeColor = isOkupatuta ? Color.White : Color.FromArgb(100, 116, 139);
        btnEgoeraOkupatuta.FlatAppearance.BorderColor = isOkupatuta ? Color.FromArgb(30, 41, 59) : Color.FromArgb(203, 213, 225);
        btnEgoeraOkupatuta.FlatAppearance.BorderSize = isOkupatuta ? 2 : 1;

        // 3. Garbitzeko botoia
        btnEgoeraGarbitzen.Text = "Garbitzeko";
        btnEgoeraGarbitzen.BackColor = isGarbitzen ? Color.FromArgb(254, 243, 199) : Color.White;
        btnEgoeraGarbitzen.ForeColor = isGarbitzen ? Color.FromArgb(180, 83, 9) : Color.FromArgb(100, 116, 139);
        btnEgoeraGarbitzen.FlatAppearance.BorderColor = isGarbitzen ? Color.FromArgb(245, 158, 11) : Color.FromArgb(203, 213, 225);
        btnEgoeraGarbitzen.FlatAppearance.BorderSize = isGarbitzen ? 2 : 1;

        lblZerbitzaria.Text = $"Zerbitzaria: {_langilea.Izena} ({_mahaia.Edukiera} pax)";
    }

    /// <summary>
    /// Mahaia 'Libre' egoeran jartzen du, eskaeretan platerik EZ badago.
    /// Araua: Platerak badaude mahai bateko eskarian ezin du mahai batek libre egon.
    /// </summary>
    private async Task AldatuEgoeraLibreAsync()
    {
        if (_eragiketanAri) return;

        // Platerak badaude mahai bateko eskarian, ezin da mahaia libratu!
        bool platerakDitu = _eskaerak.Any(e => e.Lerroak != null && e.Lerroak.Count > 0);
        if (platerakDitu)
        {
            int guztiraPlaterak = _eskaerak.Sum(e => e.Lerroak?.Sum(l => l.Kantitatea) ?? 0);
            MessageBox.Show(
                $"Mahaia {_mahaia.Zenbakia} ezin da 'Libre' egoeran jarri eskari aktiboetan {guztiraPlaterak} plater daudelako.\n\n" +
                "⚠️ Platerak badaude mahai bateko eskarian, ezin du mahai batek libre egon.\n\n" +
                "Sukaldera bidali gabeko platerak ezabatu (🗑 Ezabatu botoiarekin) edo kontua kobratu behar da lehenik.",
                "Ezin da Mahaia Libratu",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        var confirm = MessageBox.Show(
            $"Mahaia {_mahaia.Zenbakia} 'Libre' egoerara aldatu nahi duzu?",
            "Mahaia Libre Jarri",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirm != DialogResult.Yes) return;

        _eragiketanAri = true;
        try
        {
            // Mahaiak hutsik zeuden kontuak baditu (platerik gabe), guztiak ezabatu/itxi
            foreach (var e in _eskaerak.Where(e => (e.Lerroak == null || e.Lerroak.Count == 0 || e.Lerroak.Sum(l => l.Kantitatea) == 0) && e.Guztira == 0m).ToList())
            {
                await _api.EzabatuEskaeraAsync(e.Id);
            }

            var (ok, mezua) = await _api.AldatuMahaiarenEgoeraAsync(_mahaia.Id, "Libre");
            if (ok)
            {
                _mahaia.Egoera = "Libre";
                EguneratuEgoeraBotoiak();
                await KargatuDatuakAsync();
                MessageBox.Show($"Mahaia {_mahaia.Zenbakia} 'Libre' egoerara aldatu da arrakastaz.", "Egoera Aldatuta", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(mezua, "Errorea egoera aldatzean", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        finally
        {
            _eragiketanAri = false;
        }
    }

    /// <summary>
    /// Mahaia 'Okupatuta' egoeran jartzen du.
    /// </summary>
    private async Task AldatuEgoeraOkupatutaAsync()
    {
        if (_eragiketanAri) return;
        if (_mahaia.Egoera.Equals("Okupatuta", StringComparison.OrdinalIgnoreCase)) return;

        _eragiketanAri = true;
        try
        {
            var (ok, mezua) = await _api.AldatuMahaiarenEgoeraAsync(_mahaia.Id, "Okupatuta");
            if (ok)
            {
                _mahaia.Egoera = "Okupatuta";
                EguneratuEgoeraBotoiak();
                await KargatuDatuakAsync();
            }
            else
            {
                MessageBox.Show(mezua, "Errorea egoera aldatzean", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        finally
        {
            _eragiketanAri = false;
        }
    }

    /// <summary>
    /// Mahaia 'Garbitzeko' ('Garbitzen') egoeran jartzen du.
    /// </summary>
    private async Task AldatuEgoeraGarbitzenAsync()
    {
        if (_eragiketanAri) return;
        if (_mahaia.Egoera.Equals("Garbitzen", StringComparison.OrdinalIgnoreCase) ||
            _mahaia.Egoera.Equals("Garbitzeko", StringComparison.OrdinalIgnoreCase)) return;

        _eragiketanAri = true;
        try
        {
            var (ok, mezua) = await _api.AldatuMahaiarenEgoeraAsync(_mahaia.Id, "Garbitzen");
            if (ok)
            {
                _mahaia.Egoera = "Garbitzen";
                EguneratuEgoeraBotoiak();
                await KargatuDatuakAsync();
            }
            else
            {
                MessageBox.Show(mezua, "Errorea egoera aldatzean", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        finally
        {
            _eragiketanAri = false;
        }
    }

    private void AldatuMahaiazKlikatua()
    {
        MessageBox.Show(
            $"Mahaia {_mahaia.Zenbakia} beste mahai batera aldatzeko eskaera kudeaketara bidali da.",
            "Aldatu Mahaiaz",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }
}
