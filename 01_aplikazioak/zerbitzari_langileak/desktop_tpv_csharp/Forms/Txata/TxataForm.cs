using Jatetxea.Desktop.Models;
using Jatetxea.Desktop.Services;
using Jatetxea.Desktop.UI;

namespace Jatetxea.Desktop.Forms;

/// <summary>
/// TPV Barne-Txat leihoa (BaseTpvForm oinarri-klasea heredatzen du).
/// Gerente aplikaziotik aktibatutako erabiltzaileek soilik bidali/irakur ditzakete mezuak.
/// </summary>
public sealed class TxataForm : BaseTpvForm
{
    private readonly LangileaDto _langilea;
    private readonly IApiService _api;
    private readonly System.Windows.Forms.Timer _refreshTimer;

    private readonly Panel _pnlTop;
    private readonly Label _lblTitle;
    private readonly Label _lblStatusBadge;
    private readonly ComboBox _cmbGela;
    private readonly Button _btnItxi;

    private readonly RichTextBox _rtbMessages;
    private readonly Panel _pnlBottom;
    private readonly TextBox _txtMessage;
    private readonly Button _btnBidali;
    private readonly Label _lblInfo;

    private bool _kargatzen;
    private bool _txataBaimenduta = true;
    private int _azkenMezuId = -1;

    public TxataForm(LangileaDto langilea, IApiService api)
    {
        _langilea = langilea;
        _api = api;

        Text = $"T1 Jatetxea · Barne Txata ({_langilea.Izena})";
        FormBorderStyle = FormBorderStyle.Sizable;
        MinimumSize = new Size(560, 440);
        ClientSize = new Size(760, 580);
        StartPosition = FormStartPosition.CenterParent;
        BackColor = Theme.WarmLinen;

        // Goiko barra
        _pnlTop = new Panel
        {
            Dock = DockStyle.Top,
            Height = 68,
            BackColor = Theme.MarbleWhite,
            Padding = new Padding(16, 12, 16, 12)
        };
        _pnlTop.Paint += (_, e) =>
        {
            using var pen = new Pen(Theme.WarmTaupe, 1.2f);
            e.Graphics.DrawLine(pen, 0, _pnlTop.Height - 1, _pnlTop.Width, _pnlTop.Height - 1);
        };

        _lblTitle = new Label
        {
            Text = "BARNE TXATA",
            Font = Theme.DisplayFont(13.5f),
            ForeColor = Theme.ChiantiPrimary,
            AutoSize = true,
            Location = new Point(16, 18)
        };

        _lblStatusBadge = new Label
        {
            Text = "Egiaztatzen...",
            Font = Theme.BodyFont(9f, bold: true),
            ForeColor = Theme.Warning,
            BackColor = Theme.WarningBg,
            TextAlign = ContentAlignment.MiddleCenter,
            Size = new Size(175, 32),
            Location = new Point(210, 18)
        };

        _cmbGela = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = Theme.BodyFont(10f, bold: true),
            Size = new Size(140, 34),
            Anchor = AnchorStyles.Top | AnchorStyles.Right
        };
        _cmbGela.Items.AddRange(new object[] { "Orokorra", "Jangela", "Sukaldea", "Barra" });
        _cmbGela.SelectedIndex = 0;
        _cmbGela.SelectedIndexChanged += async (_, _) =>
        {
            _azkenMezuId = -1;
            await KargatuMezuakAsync(isSilent: false);
        };

        _btnItxi = new Button
        {
            Text = "Itxi",
            Size = new Size(88, 36),
            Anchor = AnchorStyles.Top | AnchorStyles.Right
        };
        Theme.StyleButton(_btnItxi, Theme.StoneMist, Theme.EspressoInk, Theme.WarmTaupe, radius: 10, fontSize: 9.5f);
        _btnItxi.Click += (_, _) => Close();

        _pnlTop.Controls.Add(_lblTitle);
        _pnlTop.Controls.Add(_lblStatusBadge);
        _pnlTop.Controls.Add(_cmbGela);
        _pnlTop.Controls.Add(_btnItxi);
        _pnlTop.Resize += (_, _) => EgokituGoikoBarra();

        // Mezuen gunea
        var pnlCenter = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(16, 12, 16, 10),
            BackColor = Theme.WarmLinen
        };

        _rtbMessages = new RichTextBox
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Theme.MarbleWhite,
            ForeColor = Theme.EspressoInk,
            Font = Theme.BodyFont(10.5f, bold: false),
            DetectUrls = false
        };
        pnlCenter.Controls.Add(_rtbMessages);

        // Beheko idazketa-barra
        _pnlBottom = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 96,
            BackColor = Theme.MarbleWhite,
            Padding = new Padding(16, 10, 16, 10)
        };
        _pnlBottom.Paint += (_, e) =>
        {
            using var pen = new Pen(Theme.WarmTaupe, 1.2f);
            e.Graphics.DrawLine(pen, 0, 0, _pnlBottom.Width, 0);
        };

        _txtMessage = new TextBox
        {
            Font = Theme.BodyFont(10.5f, bold: false),
            PlaceholderText = "Idatzi mezua eta sakatu Enter edo Bidali...",
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            Location = new Point(16, 12),
            Height = 36
        };
        _txtMessage.KeyDown += async (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                await BidaliMezuaAsync();
            }
        };

        _btnBidali = new Button
        {
            Text = "Bidali",
            Size = new Size(118, 38),
            Anchor = AnchorStyles.Top | AnchorStyles.Right
        };
        Theme.StyleButton(_btnBidali, Theme.ChiantiPrimary, Theme.MarbleWhite, null, radius: 10, fontSize: 10f);
        _btnBidali.Click += async (_, _) => await BidaliMezuaAsync();

        _lblInfo = new Label
        {
            Text = "Mezuak kargatzen...",
            Font = Theme.BodyFont(9f, bold: false),
            ForeColor = Theme.WarmSlate,
            AutoSize = false,
            Location = new Point(16, 58),
            Size = new Size(680, 24),
            Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
        };

        _pnlBottom.Controls.Add(_txtMessage);
        _pnlBottom.Controls.Add(_btnBidali);
        _pnlBottom.Controls.Add(_lblInfo);
        _pnlBottom.Resize += (_, _) => EgokituBehekoBarra();

        Controls.Add(pnlCenter);
        Controls.Add(_pnlBottom);
        Controls.Add(_pnlTop);

        AplikatuGaiKomuna();

        _refreshTimer = new System.Windows.Forms.Timer { Interval = 2000 };
        _refreshTimer.Tick += async (_, _) => await KargatuMezuakAsync(isSilent: true);

        Load += async (_, _) =>
        {
            EgokituGoikoBarra();
            EgokituBehekoBarra();
            await KargatuMezuakAsync(isSilent: false);
            _refreshTimer.Start();
        };

        FormClosed += (_, _) =>
        {
            _refreshTimer.Stop();
            _refreshTimer.Dispose();
        };
    }

    public override void EgokituDpiAldaketan()
    {
        EgokituGoikoBarra();
        EgokituBehekoBarra();
    }

    private void EgokituGoikoBarra()
    {
        _btnItxi.Location = new Point(_pnlTop.ClientSize.Width - _btnItxi.Width - 16, 16);
        _cmbGela.Location = new Point(_btnItxi.Left - _cmbGela.Width - 12, 18);
    }

    private void EgokituBehekoBarra()
    {
        _btnBidali.Location = new Point(_pnlBottom.ClientSize.Width - _btnBidali.Width - 16, 11);
        _txtMessage.Width = Math.Max(180, _btnBidali.Left - 28);
        _lblInfo.Width = Math.Max(200, _pnlBottom.ClientSize.Width - 32);
    }

    private async Task KargatuMezuakAsync(bool isSilent)
    {
        if (_kargatzen || IsDisposed) return;
        _kargatzen = true;

        try
        {
            string gela = _cmbGela.SelectedItem?.ToString() ?? "Orokorra";
            var (okEgoera, msgEgoera, egoera) = await _api.GetTxatEgoeraAsync(_langilea.Id);
            if (IsDisposed) return;

            if (!okEgoera || egoera == null || !egoera.Baimenduta)
            {
                _txataBaimenduta = false;
                EguneratuTxatEgoeraUi(false, egoera != null && !egoera.TxataAktibo
                    ? "Txata DESAKTIBATUTA daukazu. Kudeatzaileak aktibatu behar dizu."
                    : (string.IsNullOrWhiteSpace(msgEgoera) ? "Txata ez dago baimenduta." : msgEgoera));
                return;
            }

            _txataBaimenduta = true;
            var (ok, mezua, mezuak) = await _api.GetTxatMezuakAsync(gela, _langilea.Id);
            if (IsDisposed) return;

            if (!ok)
            {
                _txataBaimenduta = false;
                EguneratuTxatEgoeraUi(false, mezua);
                return;
            }

            EguneratuTxatEgoeraUi(true, $"{mezuak.Count} mezu '{gela}' gelan · Sinkronizatuta ({DateTime.Now:HH:mm:ss})");

            int azkenId = mezuak.Count > 0 ? mezuak[^1].Id : 0;
            if (!isSilent || azkenId != _azkenMezuId)
            {
                _azkenMezuId = azkenId;
                MarraztuMezuak(mezuak);
            }
        }
        finally
        {
            _kargatzen = false;
        }
    }

    private void EguneratuTxatEgoeraUi(bool baimenduta, string info)
    {
        _txtMessage.Enabled = baimenduta;
        _btnBidali.Enabled = baimenduta;

        if (baimenduta)
        {
            _lblStatusBadge.Text = "● Txata AKTIBATUTA";
            _lblStatusBadge.ForeColor = Theme.Success;
            _lblStatusBadge.BackColor = Theme.SuccessBg;
            _lblInfo.ForeColor = Theme.WarmSlate;
        }
        else
        {
            _lblStatusBadge.Text = "✕ Txata DESAKTIBATUTA";
            _lblStatusBadge.ForeColor = Theme.Danger;
            _lblStatusBadge.BackColor = Theme.DangerBg;
            _lblInfo.ForeColor = Theme.Danger;
            _rtbMessages.Clear();
            _rtbMessages.SelectionColor = Theme.Danger;
            _rtbMessages.SelectionFont = Theme.BodyFont(11f, bold: true);
            _rtbMessages.AppendText("\n  " + info + "\n");
        }

        _lblInfo.Text = info;
    }

    private void MarraztuMezuak(List<TxatMezuaDto> mezuak)
    {
        _rtbMessages.Clear();
        if (mezuak.Count == 0)
        {
            _rtbMessages.SelectionColor = Theme.StoneMuted;
            _rtbMessages.SelectionFont = Theme.BodyFont(10f, bold: false);
            _rtbMessages.AppendText("Oraindik ez dago mezurik gela honetan. Idatzi lehen mezua!\n");
            return;
        }

        foreach (var m in mezuak)
        {
            string ordua = m.SortzeData.Length >= 16 ? m.SortzeData.Substring(11, 5) : m.SortzeData;
            bool nirea = m.LangileaId == _langilea.Id;

            _rtbMessages.SelectionColor = Theme.StoneMuted;
            _rtbMessages.SelectionFont = Theme.MonoFont(9f, bold: false);
            _rtbMessages.AppendText($"[{ordua}] ");

            _rtbMessages.SelectionColor = nirea ? Theme.ChiantiPrimary : Theme.Info;
            _rtbMessages.SelectionFont = Theme.BodyFont(10f, bold: true);
            _rtbMessages.AppendText($"{m.LangileIzena} ({m.Rola}): ");

            _rtbMessages.SelectionColor = Theme.EspressoInk;
            _rtbMessages.SelectionFont = Theme.BodyFont(10f, bold: false);
            _rtbMessages.AppendText(m.Testua + "\n");
        }

        _rtbMessages.SelectionStart = _rtbMessages.TextLength;
        _rtbMessages.ScrollToCaret();
    }

    private async Task BidaliMezuaAsync()
    {
        if (!_txataBaimenduta) return;
        string testua = _txtMessage.Text.Trim();
        if (string.IsNullOrWhiteSpace(testua)) return;

        string gela = _cmbGela.SelectedItem?.ToString() ?? "Orokorra";
        _btnBidali.Enabled = false;
        _lblInfo.Text = "Mezua bidaltzen...";

        var (ok, mezua) = await _api.BidaliTxatMezuaAsync(_langilea.Id, gela, testua);
        if (IsDisposed) return;

        _btnBidali.Enabled = _txataBaimenduta;
        if (ok)
        {
            _txtMessage.Clear();
            _azkenMezuId = -1;
            await KargatuMezuakAsync(isSilent: false);
            _txtMessage.Focus();
        }
        else
        {
            _lblInfo.ForeColor = Theme.Danger;
            _lblInfo.Text = mezua;
            await KargatuMezuakAsync(isSilent: false);
        }
    }
}
