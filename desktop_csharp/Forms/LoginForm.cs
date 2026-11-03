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

        // Goiko barraren beheko ertza margotu
        pnlTopBar.Paint += (s, e) =>
        {
            using var pen = new Pen(Color.FromArgb(203, 213, 225), 1.5f);
            e.Graphics.DrawLine(pen, 0, pnlTopBar.Height - 1, pnlTopBar.Width, pnlTopBar.Height - 1);
        };

        // Txartelaren kanpoko ertz dotorea margotu
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
            await KargatuLangileakAsync();
        };
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
                    e.Graphics.FillEllipse(brush, 1, 1, dot.Width - 3, dot.Height - 3);
                }
                else
                {
                    using var pen = new Pen(Color.FromArgb(148, 163, 184), 2f); // Borobil hutsaren ertza
                    e.Graphics.DrawEllipse(pen, 1, 1, dot.Width - 3, dot.Height - 3);
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
        btnKeyClear.Click += (s, e) => KeypadBotoiaKlikatua("X Clear");
        btnKey0.Click += (s, e) => KeypadBotoiaKlikatua("0");
        btnKeyBack.Click += (s, e) => KeypadBotoiaKlikatua("<-");
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

                var ertzAkolorea = isSelected ? Color.FromArgb(37, 99, 235) : Color.FromArgb(203, 213, 225);
                float stroke = isSelected ? 2.8f : 1.2f;
                using var pen = new Pen(ertzAkolorea, stroke);
                e.Graphics.DrawEllipse(pen, 2, 2, item.Circle.Width - 5, item.Circle.Height - 5);
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

        for (int i = 0; i < items.Length; i++)
        {
            if (i < _langileak.Count)
            {
                var l = _langileak[i];
                var zatiak = l.Izena.Split(' ');
                string shortName = l.Izena;
                if (zatiak.Length >= 2 && !l.Izena.Contains('.'))
                {
                    shortName = $"{zatiak[0]} {zatiak[1][0]}.";
                }
                string initials = string.Join("", zatiak.Select(z => z.Length > 0 ? z[0].ToString() : "")).ToUpper();
                if (initials.Length > 2) initials = initials.Substring(0, 2);

                items[i].Name.Text = shortName;
                items[i].Circle.Text = initials;
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

        lblCircle1.Invalidate();
        lblCircle2.Invalidate();
        lblCircle3.Invalidate();
        lblCircle4.Invalidate();
        lblCircle5.Invalidate();
    }

    private void KeypadBotoiaKlikatua(string txt)
    {
        lblStatus.Text = "";

        if (txt == "X Clear")
        {
            _pin = string.Empty;
        }
        else if (txt == "<-")
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
}
