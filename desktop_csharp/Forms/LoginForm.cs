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

        // Zentratze dinamikoa tamaina aldatzean
        Resize += (s, e) => ZentratuTxartela();
        Load += async (s, e) =>
        {
            ZentratuTxartela();
            EraikiTeklatua();
            await KargatuLangileakAsync();
        };

        // Goiko barraren beheko ertza margotu
        pnlTopBar.Paint += (s, e) =>
        {
            using var pen = new Pen(Color.FromArgb(226, 232, 240), 1f);
            e.Graphics.DrawLine(pen, 0, pnlTopBar.Height - 1, pnlTopBar.Width, pnlTopBar.Height - 1);
        };

        // Txartelaren ertz dotorea margotu
        pnlCard.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var pen = new Pen(Color.FromArgb(203, 213, 225), 1.5f);
            e.Graphics.DrawRectangle(pen, 0, 0, pnlCard.Width - 1, pnlCard.Height - 1);
        };

        // 4 PIN borobilak margotu (1-saioa_hasi_interfazea.jpg zirriborroaren arabera)
        pnlPinDots.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            int count = 4;
            int diameter = 18;
            int spacing = 16;
            int totalWidth = (count * diameter) + ((count - 1) * spacing);
            int startX = (pnlPinDots.Width - totalWidth) / 2;
            int y = (pnlPinDots.Height - diameter) / 2;

            for (int i = 0; i < count; i++)
            {
                int x = startX + i * (diameter + spacing);
                bool betea = i < _pin.Length;

                if (betea)
                {
                    using var brush = new SolidBrush(Color.FromArgb(51, 65, 85)); // Borobil betea
                    e.Graphics.FillEllipse(brush, x, y, diameter, diameter);
                }
                else
                {
                    using var pen = new Pen(Color.FromArgb(148, 163, 184), 2f); // Borobil hutsaren ertza
                    e.Graphics.DrawEllipse(pen, x, y, diameter, diameter);
                }
            }
        };

        btnLogin.Click += async (s, e) => await LoginExekutatuAsync();
    }

    private void ZentratuTxartela()
    {
        if (pnlCard == null || pnlTopBar == null) return;
        int x = Math.Max(10, (ClientSize.Width - pnlCard.Width) / 2);
        int erabilgarriAltua = ClientSize.Height - pnlTopBar.Height;
        int y = pnlTopBar.Height + Math.Max(10, (erabilgarriAltua - pnlCard.Height) / 2);
        pnlCard.Location = new Point(x, y);
    }

    private void EraikiTeklatua()
    {
        pnlKeypad.Controls.Clear();

        string[] botoiak = {
            "1", "2", "3",
            "4", "5", "6",
            "7", "8", "9",
            "X Clear", "0", "<-"
        };

        foreach (var txt in botoiak)
        {
            var btn = new Button
            {
                Text = txt,
                Dock = DockStyle.Fill,
                Font = txt.Length > 2 ? new Font("Segoe UI", 11F, FontStyle.Bold) : new Font("Segoe UI", 16F, FontStyle.Bold),
                Margin = new Padding(5),
                BackColor = txt switch
                {
                    "X Clear" => Color.FromArgb(254, 242, 242),
                    "<-" => Color.FromArgb(248, 250, 252),
                    _ => Color.White
                },
                ForeColor = txt switch
                {
                    "X Clear" => Color.FromArgb(220, 38, 38),
                    _ => Color.FromArgb(30, 41, 59)
                },
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btn.FlatAppearance.BorderSize = 1;
            btn.Click += (s, e) => KeypadBotoiaKlikatua(txt);
            pnlKeypad.Controls.Add(btn);
        }
    }

    private async Task KargatuLangileakAsync()
    {
        pnlAvatars.Controls.Clear();

        _langileak = await _api.GetLangileakAsync();

        if (_langileak.Count == 0)
        {
            // Zirriborroko 5 zerbitzariak lehenetsita
            _langileak = new List<LangileaDto>
            {
                new() { Id = 1, Erabiltzailea = "aitor", Izena = "Aitor B.", Rola = "Zerbitzaria", Aktibo = true },
                new() { Id = 2, Erabiltzailea = "maite", Izena = "Maite L.", Rola = "Zerbitzaria", Aktibo = true },
                new() { Id = 3, Erabiltzailea = "mikel", Izena = "Mikel A.", Rola = "Zerbitzaria", Aktibo = true },
                new() { Id = 4, Erabiltzailea = "ane", Izena = "Ane G.", Rola = "Zerbitzaria", Aktibo = true },
                new() { Id = 5, Erabiltzailea = "iker", Izena = "Iker M.", Rola = "Zerbitzaria", Aktibo = true }
            };
        }

        // Gehienez lehen 5ak erakutsi zirriborroaren arabera
        var bistaratzekoak = _langileak.Take(5).ToList();

        foreach (var l in bistaratzekoak)
        {
            var avatarCard = SortuAvatarTxartela(l);
            pnlAvatars.Controls.Add(avatarCard);
        }

        // Lehenengoa (Aitor) lehenetsi zirriborroan bezala
        if (bistaratzekoak.Count > 0)
        {
            HautatuLangilea(bistaratzekoak[0]);
        }
    }

    private Control SortuAvatarTxartela(LangileaDto langilea)
    {
        var item = new Panel
        {
            Width = 80,
            Height = 82,
            Margin = new Padding(2, 2, 2, 2),
            BackColor = Color.Transparent,
            Cursor = Cursors.Hand,
            Tag = langilea
        };

        // Zirriborroko izen laburtua prestatu (adib. "Aitor B.")
        string izenLaburtua = langilea.Izena;
        var zatiak = langilea.Izena.Split(' ');
        if (zatiak.Length >= 2 && !langilea.Izena.Contains('.'))
        {
            izenLaburtua = $"{zatiak[0]} {zatiak[1][0]}.";
        }

        // Inizialak (adib. "AB")
        string initials = string.Join("", zatiak.Select(z => z.Length > 0 ? z[0].ToString() : "")).ToUpper();
        if (initials.Length > 2) initials = initials.Substring(0, 2);

        var pnlCircle = new Panel
        {
            Location = new Point(16, 4),
            Size = new Size(48, 48),
            BackColor = Color.Transparent
        };

        var lblInitial = new Label
        {
            Dock = DockStyle.Fill,
            Text = initials,
            Font = new Font("Segoe UI", 11F, FontStyle.Bold),
            ForeColor = Color.FromArgb(71, 85, 105),
            TextAlign = ContentAlignment.MiddleCenter
        };
        pnlCircle.Controls.Add(lblInitial);
        item.Controls.Add(pnlCircle);

        var lblName = new Label
        {
            Text = izenLaburtua,
            Font = new Font("Segoe UI", 8.5F, FontStyle.Regular),
            ForeColor = Color.FromArgb(51, 65, 85),
            Location = new Point(0, 56),
            Size = new Size(80, 20),
            TextAlign = ContentAlignment.TopCenter,
            AutoEllipsis = true
        };
        item.Controls.Add(lblName);

        // Zirriborroko borobila margotu (hautatuta badago borobil urdin/bordelegina inguruan)
        pnlCircle.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            bool isSelected = _aukeratutakoLangilea?.Id == langilea.Id;

            // Barruko hondo leuna
            using var bgBrush = new SolidBrush(isSelected ? Color.FromArgb(239, 246, 255) : Color.FromArgb(241, 245, 249));
            e.Graphics.FillEllipse(bgBrush, 2, 2, 43, 43);

            // Kanpoko ertza (hautatua denean zirriborroko borobil urdina)
            var ertzAkolorea = isSelected ? Color.FromArgb(37, 99, 235) : Color.FromArgb(203, 213, 225);
            float ertzAStroked = isSelected ? 2.5f : 1.2f;
            using var pen = new Pen(ertzAkolorea, ertzAStroked);
            e.Graphics.DrawEllipse(pen, 2, 2, 43, 43);
        };

        void OnClick(object? s, EventArgs e) => HautatuLangilea(langilea);
        item.Click += OnClick;
        pnlCircle.Click += OnClick;
        lblInitial.Click += OnClick;
        lblName.Click += OnClick;

        return item;
    }

    private void HautatuLangilea(LangileaDto langilea)
    {
        _aukeratutakoLangilea = langilea;
        _pin = string.Empty;
        pnlPinDots.Invalidate();
        lblStatus.Text = "";

        // Avatar guztien borobilak birbalidatu
        foreach (Control c in pnlAvatars.Controls)
        {
            foreach (Control sub in c.Controls)
            {
                sub.Invalidate();
            }
        }
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

        pnlPinDots.Invalidate();
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
            pnlPinDots.Invalidate();

            var mahaiakForm = new MahaiakForm(langilea, _api);
            mahaiakForm.FormClosed += (s, e) =>
            {
                Show();
                _pin = string.Empty;
                pnlPinDots.Invalidate();
            };

            Hide();
            mahaiakForm.Show();
        }
        else
        {
            lblStatus.Text = mezua;
            _pin = string.Empty;
            pnlPinDots.Invalidate();
        }
    }
}
