using System.Drawing.Drawing2D;
using Jatetxea.Desktop.Models;
using Jatetxea.Desktop.Services;
using Jatetxea.Desktop.UI;

namespace Jatetxea.Desktop.Forms;

public partial class LoginForm : Form
{
    private readonly ApiService _api;
    private List<LangileaDto> _langileak = new();
    private LangileaDto? _aukeratutakoLangilea;
    private string _pin = string.Empty;

    public LoginForm()
    {
        InitializeComponent();
        _api = new ApiService();

        // Zentratu goiburuko "Zerbitzariak" testua automatikoki
        pnlTopBar.Resize += (s, e) =>
        {
            lblZerbitzariak.Location = new Point((pnlTopBar.Width - lblZerbitzariak.Width) / 2, (pnlTopBar.Height - lblZerbitzariak.Height) / 2);
        };

        // Zentratu txartel nagusia pantailan automatikoki
        Resize += (s, e) =>
        {
            ZentratuTxartela();
        };

        // Goiko barraren beheko marra margotu (zirriborroaren arabera)
        pnlTopBar.Paint += (s, e) =>
        {
            using var pen = new Pen(Color.FromArgb(203, 213, 225), 1.5f);
            e.Graphics.DrawLine(pen, 0, pnlTopBar.Height - 1, pnlTopBar.Width, pnlTopBar.Height - 1);
        };

        // Txartelaren kanpoko ertz leuna margotu
        pnlCard.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var pen = new Pen(Color.FromArgb(203, 213, 225), 1.5f);
            e.Graphics.DrawRectangle(pen, 0, 0, pnlCard.Width - 1, pnlCard.Height - 1);
        };

        // PIN borobilak margotu
        SetupPinDotsPaint();

        // Teklatuko botoiak lotu
        LotuTeklatuBotoiak();

        // Avatarren klikak eta margoak prestatu
        PrestatuAvatarrak();

        btnLogin.Click += async (s, e) => await LoginExekutatuAsync();

        Load += async (s, e) =>
        {
            lblZerbitzariak.Location = new Point((pnlTopBar.Width - lblZerbitzariak.Width) / 2, (pnlTopBar.Height - lblZerbitzariak.Height) / 2);
            ZentratuTxartela();
            await KargatuLangileakAsync();
        };
    }

    private void ZentratuTxartela()
    {
        int x = Math.Max(20, (ClientSize.Width - pnlCard.Width) / 2);
        int y = Math.Max(75, (ClientSize.Height - 65 - pnlCard.Height) / 2 + 65);
        pnlCard.Location = new Point(x, y);
    }

    private void SetupPinDotsPaint()
    {
        Label[] dots = { lblDot1, lblDot2, lblDot3, lblDot4 };
        for (int i = 0; i < dots.Length; i++)
        {
            int index = i;
            var dot = dots[index];
            dot.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                bool betea = index < _pin.Length;

                if (betea)
                {
                    using var brush = new SolidBrush(Color.FromArgb(51, 65, 85)); // Borobil betea
                    e.Graphics.FillEllipse(brush, 2, 2, dot.Width - 5, dot.Height - 5);
                }
                else
                {
                    using var pen = new Pen(Color.FromArgb(148, 163, 184), 2f); // Borobil hutsaren ertza
                    e.Graphics.DrawEllipse(pen, 2, 2, dot.Width - 5, dot.Height - 5);
                }
            };
        }
    }

    private void LotuTeklatuBotoiak()
    {
        btnKey1.Click += (s, e) => KeypadBotoiaKlikatua("1");
        btnKey2.Click += (s, e) => KeypadBotoiaKlikatua("2");
        btnKey3.Click += (s, e) => KeypadBotoiaKlikatua("3");
        btnKey4.Click += (s, e) => KeypadBotoiaKlikatua("4");
        btnKey5.Click += (s, e) => KeypadBotoiaKlikatua("5");
        btnKey6.Click += (s, e) => KeypadBotoiaKlikatua("6");
        btnKey7.Click += (s, e) => KeypadBotoiaKlikatua("7");
        btnKey8.Click += (s, e) => KeypadBotoiaKlikatua("8");
        btnKey9.Click += (s, e) => KeypadBotoiaKlikatua("9");
        btnKey0.Click += (s, e) => KeypadBotoiaKlikatua("0");
        btnKeyBack.Click += (s, e) => KeypadBotoiaKlikatua("←");
    }

    private void PrestatuAvatarrak()
    {
        (Panel Panel, Label Circle, Label Name)[] items = {
            (pnlAvatar1, lblCircle1, lblName1),
            (pnlAvatar2, lblCircle2, lblName2),
            (pnlAvatar3, lblCircle3, lblName3),
            (pnlAvatar4, lblCircle4, lblName4),
            (pnlAvatar5, lblCircle5, lblName5)
        };

        for (int i = 0; i < items.Length; i++)
        {
            int index = i;
            var item = items[index];

            item.Circle.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                bool isSelected = false;
                if (index < _langileak.Count && _aukeratutakoLangilea != null)
                {
                    isSelected = _aukeratutakoLangilea.Id == _langileak[index].Id;
                }

                // 1. Barruko hondo borobila
                using var bgBrush = new SolidBrush(isSelected ? Color.FromArgb(239, 246, 255) : Color.FromArgb(241, 245, 249));
                e.Graphics.FillEllipse(bgBrush, 2, 2, item.Circle.Width - 5, item.Circle.Height - 5);

                // 2. Kanpoko borobila
                var ertzAkolorea = isSelected ? Color.FromArgb(37, 99, 235) : Color.FromArgb(203, 213, 225);
                float stroke = isSelected ? 2.8f : 1.2f;
                using var pen = new Pen(ertzAkolorea, stroke);
                e.Graphics.DrawEllipse(pen, 2, 2, item.Circle.Width - 5, item.Circle.Height - 5);

                // 3. Inizialen testua borobilaren barruan marraztu
                var textColor = isSelected ? Color.FromArgb(37, 99, 235) : Color.FromArgb(71, 85, 105);
                TextRenderer.DrawText(e.Graphics, item.Circle.Text, item.Circle.Font,
                    new Rectangle(0, 0, item.Circle.Width, item.Circle.Height),
                    textColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            };

            void OnClick(object? s, EventArgs e)
            {
                if (index < _langileak.Count)
                {
                    HautatuLangilea(_langileak[index]);
                }
            }

            item.Panel.Click += OnClick;
            item.Circle.Click += OnClick;
            item.Name.Click += OnClick;
        }
    }

    private async Task KargatuLangileakAsync()
    {
        _langileak = await _api.GetLangileakAsync();

        if (_langileak.Count == 0)
        {
            _langileak = new List<LangileaDto>
            {
                new() { Id = 1, Erabiltzailea = "aitor", Izena = "Aitor B.", Rola = "Zerbitzaria", Aktibo = true },
                new() { Id = 2, Erabiltzailea = "maite", Izena = "Maite L.", Rola = "Zerbitzaria", Aktibo = true },
                new() { Id = 3, Erabiltzailea = "mikel", Izena = "Mikel A.", Rola = "Zerbitzaria", Aktibo = true },
                new() { Id = 4, Erabiltzailea = "ane", Izena = "Ane G.", Rola = "Zerbitzaria", Aktibo = true },
                new() { Id = 5, Erabiltzailea = "iker", Izena = "Iker M.", Rola = "Zerbitzaria", Aktibo = true }
            };
        }

        (Panel Panel, Label Circle, Label Name)[] items = {
            (pnlAvatar1, lblCircle1, lblName1),
            (pnlAvatar2, lblCircle2, lblName2),
            (pnlAvatar3, lblCircle3, lblName3),
            (pnlAvatar4, lblCircle4, lblName4),
            (pnlAvatar5, lblCircle5, lblName5)
        };

        int count = Math.Min(_langileak.Count, 5);
        int avatarWidth = count <= 3 ? 105 : (count == 4 ? 92 : 78);
        int gap = count > 1 ? Math.Max(4, (pnlAvatars.Width - (count * avatarWidth)) / (count - 1)) : 0;
        int totalWidth = count * avatarWidth + (count - 1) * gap;
        int startX = Math.Max(0, (pnlAvatars.Width - totalWidth) / 2);

        var lehenIzenak = _langileak.Select(x => x.Izena.Trim().Split(' ')[0]).ToList();

        for (int i = 0; i < items.Length; i++)
        {
            if (i < count)
            {
                var l = _langileak[i];
                var zatiak = l.Izena.Trim().Split(' ');
                string lehenIzena = zatiak[0];
                bool bikoiztua = lehenIzenak.Count(x => x.Equals(lehenIzena, StringComparison.OrdinalIgnoreCase)) > 1;
                string shortName = (bikoiztua && zatiak.Length >= 2) ? $"{lehenIzena} {zatiak[1][0]}." : lehenIzena;
                string initials = string.Join("", zatiak.Select(z => z.Length > 0 ? z[0].ToString() : "")).ToUpper();
                if (initials.Length > 2) initials = initials.Substring(0, 2);

                items[i].Name.Text = shortName;
                items[i].Circle.Text = initials;

                // Neurriak eta posizioak egokitu izena moztu ez dadin
                items[i].Panel.Size = new Size(avatarWidth, 88);
                items[i].Circle.Location = new Point((avatarWidth - 46) / 2, 2);
                items[i].Name.Location = new Point(0, 52);
                items[i].Name.Size = new Size(avatarWidth, 34);

                items[i].Panel.Location = new Point(startX + i * (avatarWidth + gap), 2);
                items[i].Panel.Visible = true;
            }
            else
            {
                items[i].Panel.Visible = false;
            }
        }

        if (_langileak.Count > 0)
        {
            HautatuLangilea(_langileak[0]);
        }
    }

    private void HautatuLangilea(LangileaDto langilea)
    {
        _aukeratutakoLangilea = langilea;
        _pin = string.Empty;
        FreskatuPinDots();
        lblStatus.Text = "";

        (Label Circle, Label Name)[] items = {
            (lblCircle1, lblName1),
            (lblCircle2, lblName2),
            (lblCircle3, lblName3),
            (lblCircle4, lblName4),
            (lblCircle5, lblName5)
        };

        for (int i = 0; i < items.Length; i++)
        {
            bool isSelected = i < _langileak.Count && _langileak[i].Id == langilea.Id;
            items[i].Name.Font = new Font("Segoe UI", 8F, isSelected ? FontStyle.Bold : FontStyle.Regular);
            items[i].Name.ForeColor = isSelected ? Color.FromArgb(30, 41, 59) : Color.FromArgb(100, 116, 139);
            items[i].Circle.Invalidate();
        }
    }

    private void KeypadBotoiaKlikatua(string txt)
    {
        lblStatus.Text = "";

        if (txt.Contains("Clear"))
        {
            _pin = string.Empty;
        }
        else if (txt == "←" || txt == "<-")
        {
            if (_pin.Length > 0)
                _pin = _pin.Substring(0, _pin.Length - 1);
        }
        else
        {
            if (_pin.Length < 4)
                _pin += txt;
        }

        FreskatuPinDots();
    }

    private void FreskatuPinDots()
    {
        lblDot1.Invalidate();
        lblDot2.Invalidate();
        lblDot3.Invalidate();
        lblDot4.Invalidate();
    }

    private async Task LoginExekutatuAsync()
    {
        if (_aukeratutakoLangilea == null)
        {
            lblStatus.Text = "Hautatu zerbitzari bat lehenik.";
            return;
        }

        if (string.IsNullOrWhiteSpace(_pin))
        {
            lblStatus.Text = "Sartu 4 digituko PIN zenbakia.";
            return;
        }

        btnLogin.Enabled = false;
        btnLogin.Text = "EGIAZTATZEN...";

        var (arrakasta, mezua, langilea) = await _api.LoginAsync(_aukeratutakoLangilea.Erabiltzailea, _pin);

        btnLogin.Enabled = true;
        btnLogin.Text = "HASI SAIOA";

        if (arrakasta && langilea != null)
        {
            _pin = string.Empty;
            FreskatuPinDots();

            var mahaiakForm = new MahaiakForm(langilea, _api);
            mahaiakForm.FormClosed += (s, e) =>
            {
                Show();
                _pin = string.Empty;
                FreskatuPinDots();
            };

            Hide();
            mahaiakForm.Show();
        }
        else
        {
            lblStatus.Text = mezua;
            _pin = string.Empty;
            FreskatuPinDots();
        }
    }

    private void btnKeyClear_Click(object? sender, EventArgs e)
    {
        KeypadBotoiaKlikatua("X Clear");
    }

    private void LoginForm_Load(object? sender, EventArgs e)
    {
    }
}
