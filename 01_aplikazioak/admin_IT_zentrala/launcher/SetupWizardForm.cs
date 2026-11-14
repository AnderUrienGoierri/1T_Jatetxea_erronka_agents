using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdminLauncher;

public class SetupWizardForm : Form
{
    private string _rootDir;
    private readonly string _targetUrl = "http://localhost:5000";

    // Panels
    private Panel _pnlWelcome = null!;
    private Panel _pnlProgress = null!;

    // Path selection controls
    private TextBox _txtPath = null!;
    private Button _btnBrowse = null!;
    private Label _lblValidation = null!;

    // Welcome buttons
    private Button _btnAlreadyInstalled = null!;
    private Button _btnInstall = null!;
    private Button _btnCancel = null!;

    // Progress controls
    private Label _lblStepTitle = null!;
    private Label _lblStepDetail = null!;
    private ProgressBar _progressBar = null!;
    private Label _lblPercent = null!;
    private RichTextBox _rtbLog = null!;
    private Button _btnLaunch = null!;
    private Button _btnClose = null!;

    public SetupWizardForm(string rootDir)
    {
        _rootDir = rootDir;
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        Text = "1T Jatetxea - IT Kontrol Zentrala | Hasierako Konfigurazio Morroia";
        Size = new Size(860, 690);
        MinimumSize = new Size(820, 650);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        BackColor = Color.FromArgb(15, 23, 42); // slate-900
        ForeColor = Color.White;
        Font = new Font("Segoe UI", 9F, FontStyle.Regular);

        // Icon
        string iconPath = Path.Combine(_rootDir, "admin_it_zentrala.ico");
        if (File.Exists(iconPath))
        {
            try { Icon = new Icon(iconPath); } catch { }
        }

        // =====================================================================
        // 1. WELCOME PANEL
        // =====================================================================
        _pnlWelcome = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(15, 23, 42),
            Padding = new Padding(30)
        };

        var lblHeader = new Label
        {
            Text = "1T JATETXEA - IT KONTROL ZENTRALA",
            Font = new Font("Segoe UI", 16F, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = true,
            Location = new Point(30, 20)
        };

        var lblSubheader = new Label
        {
            Text = "Hasierako Instalazio eta Konfigurazio Automatikoaren Morroia",
            Font = new Font("Segoe UI", 10.5F, FontStyle.Regular),
            ForeColor = Color.FromArgb(56, 189, 248), // sky-400
            AutoSize = true,
            Location = new Point(30, 52)
        };

        // ---------------------------------------------------------------------
        // KARPETA HAUTATZAILEA ETA EGIAZTAPENA
        // ---------------------------------------------------------------------
        var pnlPath = new Panel
        {
            Location = new Point(30, 82),
            Size = new Size(785, 78),
            BackColor = Color.FromArgb(30, 41, 59), // slate-800
            Padding = new Padding(12)
        };

        var lblPathTitle = new Label
        {
            Text = "PROIEKTUAREN KOKAPENA (ERROKO KARPETA):",
            Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
            ForeColor = Color.FromArgb(148, 163, 184), // slate-400
            Location = new Point(12, 8),
            AutoSize = true
        };

        _txtPath = new TextBox
        {
            Text = _rootDir,
            Font = new Font("Segoe UI", 9F),
            BackColor = Color.FromArgb(15, 23, 42),
            ForeColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Location = new Point(12, 28),
            Size = new Size(625, 25)
        };
        _txtPath.TextChanged += (s, e) => ValidateProjectDirectory(_txtPath.Text.Trim());

        _btnBrowse = new Button
        {
            Text = "Arakatu...",
            Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = Color.FromArgb(51, 65, 85), // slate-700
            FlatStyle = FlatStyle.Flat,
            Location = new Point(645, 26),
            Size = new Size(125, 27),
            Cursor = Cursors.Hand
        };
        _btnBrowse.FlatAppearance.BorderSize = 0;
        _btnBrowse.Click += (s, e) =>
        {
            using var fbd = new FolderBrowserDialog
            {
                Description = "Hautatu 1T Jatetxea proiektuaren erroko karpeta (docker-compose.yml dagoen lekua):",
                UseDescriptionForTitle = true,
                SelectedPath = Directory.Exists(_rootDir) ? _rootDir : AppDomain.CurrentDomain.BaseDirectory
            };
            if (fbd.ShowDialog(this) == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                _txtPath.Text = fbd.SelectedPath;
            }
        };

        _lblValidation = new Label
        {
            Text = "Egiaztatzen...",
            Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
            ForeColor = Color.FromArgb(52, 211, 153),
            Location = new Point(12, 55),
            AutoSize = true
        };

        pnlPath.Controls.Add(lblPathTitle);
        pnlPath.Controls.Add(_txtPath);
        pnlPath.Controls.Add(_btnBrowse);
        pnlPath.Controls.Add(_lblValidation);

        // ---------------------------------------------------------------------
        // INFORMAZIO KARTA
        // ---------------------------------------------------------------------
        var cardInfo = new Panel
        {
            Location = new Point(30, 168),
            Size = new Size(785, 385),
            BackColor = Color.FromArgb(30, 41, 59), // slate-800
            Padding = new Padding(18)
        };

        var lblCardTitle = new Label
        {
            Text = "Ongi etorri! Lehen aldiz exekutatzen ari zara proiektua.",
            Font = new Font("Segoe UI", 11.5F, FontStyle.Bold),
            ForeColor = Color.FromArgb(248, 250, 252),
            Location = new Point(16, 12),
            AutoSize = true
        };

        var lblCardDesc = new Label
        {
            Text = "Morroi honek zure ordenagailuan 1T Jatetxea sistema osoa instalatu, egiaztatu eta konfiguratuko du automatikoki.\n" +
                   "Zure taldekideek edo zuk ez duzue komando konplexurik exekutatu behar: prozesu honek dena prest utziko du.",
            Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
            ForeColor = Color.FromArgb(203, 213, 225),
            Location = new Point(16, 40),
            Size = new Size(750, 42)
        };

        var lblStepsTitle = new Label
        {
            Text = "Morroiak burutuko dituen konfigurazio eta instalazioak:",
            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
            ForeColor = Color.FromArgb(125, 211, 252), // sky-300
            Location = new Point(16, 88),
            AutoSize = true
        };

        var rtbSteps = new RichTextBox
        {
            Location = new Point(16, 112),
            Size = new Size(750, 255),
            BackColor = Color.FromArgb(15, 23, 42),
            ForeColor = Color.FromArgb(226, 232, 240),
            Font = new Font("Segoe UI", 9F),
            ReadOnly = true,
            BorderStyle = BorderStyle.None,
            Text =
                "1. SISTEMAREN BALDINTZAK:\n" +
                "   - Docker Desktop eta .NET SDK ingurunean martxan dauden egiaztatzen da.\n\n" +
                "2. DOCKER SAREAK ETA BOLUMENAK:\n" +
                "   - 'jatetxea_net' sarea eta datu-baseen biltegi iraunkorrak (MySQL, PostgreSQL, Odoo) sortzen dira.\n\n" +
                "3. DOCKER IRUDIAK ETA EDUKIONTZIAK:\n" +
                "   - MySQL 8.0, PostgreSQL, phpMyAdmin, Adminer, Web PHP, Txat Node.js, Sukaldea TV eta Odoo 17 prestatu.\n\n" +
                "4. .NET REST API NAGUSIA:\n" +
                "   - C# backend_api proiektuaren mendekotasunak leheneratu eta eraikitzen dira.\n\n" +
                "5. IT KONTROL ZENTRALA (ADMIN):\n" +
                "   - Kudeaketa zentroa eta zerbitzuen kontrolatzailea konpilatu eta prest uzten dira.\n\n" +
                "6. MAHAIGAINERAKO LASTERBIDEA:\n" +
                "   - Windows mahaigainean '1T Jatetxea - Admin IT Zentrala' ikono ofiziala sortzen da."
        };

        cardInfo.Controls.Add(lblCardTitle);
        cardInfo.Controls.Add(lblCardDesc);
        cardInfo.Controls.Add(lblStepsTitle);
        cardInfo.Controls.Add(rtbSteps);

        // ---------------------------------------------------------------------
        // BOTOIAK (BEHEAN)
        // ---------------------------------------------------------------------
        _btnCancel = new Button
        {
            Text = "Irten",
            Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
            ForeColor = Color.FromArgb(203, 213, 225),
            BackColor = Color.FromArgb(51, 65, 85), // slate-700
            FlatStyle = FlatStyle.Flat,
            Size = new Size(110, 48),
            Location = new Point(30, 565),
            Cursor = Cursors.Hand
        };
        _btnCancel.FlatAppearance.BorderSize = 0;
        _btnCancel.Click += (s, e) => Close();

        _btnAlreadyInstalled = new Button
        {
            Text = "AURRERA JARRAITU (IREKI 5000)",
            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = Color.FromArgb(5, 150, 105), // emerald-600
            FlatStyle = FlatStyle.Flat,
            Size = new Size(285, 48),
            Location = new Point(150, 565),
            Cursor = Cursors.Hand,
            Enabled = true
        };
        _btnAlreadyInstalled.FlatAppearance.BorderSize = 0;
        _btnAlreadyInstalled.Click += async (s, e) =>
        {
            _btnAlreadyInstalled.Enabled = false;
            _btnAlreadyInstalled.Text = "Abiarazten...";
            SaveSetupMarkerFile();
            CreateDesktopShortcut();
            await EnsureAdminZentralaRunningAsync();
            OpenBrowser();
            Close();
        };

        _btnInstall = new Button
        {
            Text = "INSTALATU ETA PRESTATU PROIEKTU OSOA",
            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = Color.FromArgb(2, 132, 199), // sky-600
            FlatStyle = FlatStyle.Flat,
            Size = new Size(370, 48),
            Location = new Point(445, 565),
            Cursor = Cursors.Hand
        };
        _btnInstall.FlatAppearance.BorderSize = 0;
        _btnInstall.Click += async (s, e) => await StartInstallationProcessAsync();

        _pnlWelcome.Controls.Add(lblHeader);
        _pnlWelcome.Controls.Add(lblSubheader);
        _pnlWelcome.Controls.Add(pnlPath);
        _pnlWelcome.Controls.Add(cardInfo);
        _pnlWelcome.Controls.Add(_btnCancel);
        _pnlWelcome.Controls.Add(_btnAlreadyInstalled);
        _pnlWelcome.Controls.Add(_btnInstall);

        // Hasierako balioztapena exekutatu
        ValidateProjectDirectory(_rootDir);

        // =====================================================================
        // 2. PROGRESS PANEL
        // =====================================================================
        _pnlProgress = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(15, 23, 42),
            Padding = new Padding(30),
            Visible = false
        };

        var lblProgHeader = new Label
        {
            Text = "1T JATETXEA - PROIEKTUA PRESTATZEN ETA INSTALATZEN",
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = true,
            Location = new Point(30, 20)
        };

        _lblStepTitle = new Label
        {
            Text = "Hasten...",
            Font = new Font("Segoe UI", 11F, FontStyle.Bold),
            ForeColor = Color.FromArgb(56, 189, 248), // sky-400
            AutoSize = true,
            Location = new Point(30, 55)
        };

        _lblStepDetail = new Label
        {
            Text = "Prestaketa lanak hasten...",
            Font = new Font("Segoe UI", 9F, FontStyle.Regular),
            ForeColor = Color.FromArgb(148, 163, 184), // slate-400
            AutoSize = true,
            Location = new Point(30, 80)
        };

        _progressBar = new ProgressBar
        {
            Location = new Point(30, 105),
            Size = new Size(680, 24),
            Style = ProgressBarStyle.Continuous,
            Value = 0,
            Maximum = 100
        };

        _lblPercent = new Label
        {
            Text = "0%",
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            ForeColor = Color.FromArgb(52, 211, 153), // emerald-400
            Location = new Point(720, 107),
            AutoSize = true
        };

        var lblLogTitle = new Label
        {
            Text = "Prozesuen garapena eta irteera monitorizatua (Denbora errealean):",
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            ForeColor = Color.FromArgb(203, 213, 225),
            Location = new Point(30, 140),
            AutoSize = true
        };

        _rtbLog = new RichTextBox
        {
            Location = new Point(30, 165),
            Size = new Size(745, 315),
            BackColor = Color.FromArgb(2, 6, 23), // slate-950
            ForeColor = Color.FromArgb(52, 211, 153), // emerald-400
            Font = new Font("Consolas", 8.5F),
            ReadOnly = true,
            BorderStyle = BorderStyle.None
        };

        _btnLaunch = new Button
        {
            Text = "IREKI IT KONTROL ZENTRALA (5000)",
            Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = Color.FromArgb(16, 185, 129), // emerald-500
            FlatStyle = FlatStyle.Flat,
            Size = new Size(360, 46),
            Location = new Point(415, 500),
            Visible = false,
            Cursor = Cursors.Hand
        };
        _btnLaunch.FlatAppearance.BorderSize = 0;
        _btnLaunch.Click += (s, e) =>
        {
            OpenBrowser();
            Close();
        };

        _btnClose = new Button
        {
            Text = "Itxi",
            Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
            ForeColor = Color.FromArgb(203, 213, 225),
            BackColor = Color.FromArgb(51, 65, 85),
            FlatStyle = FlatStyle.Flat,
            Size = new Size(130, 46),
            Location = new Point(270, 500),
            Visible = false,
            Cursor = Cursors.Hand
        };
        _btnClose.FlatAppearance.BorderSize = 0;
        _btnClose.Click += (s, e) => Close();

        _pnlProgress.Controls.Add(lblProgHeader);
        _pnlProgress.Controls.Add(_lblStepTitle);
        _pnlProgress.Controls.Add(_lblStepDetail);
        _pnlProgress.Controls.Add(_progressBar);
        _pnlProgress.Controls.Add(_lblPercent);
        _pnlProgress.Controls.Add(lblLogTitle);
        _pnlProgress.Controls.Add(_rtbLog);
        _pnlProgress.Controls.Add(_btnLaunch);
        _pnlProgress.Controls.Add(_btnClose);

        Controls.Add(_pnlProgress);
        Controls.Add(_pnlWelcome);
    }

    private void AppendLog(string message)
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action(() => AppendLog(message)));
            return;
        }

        _rtbLog.AppendText(message + Environment.NewLine);
        _rtbLog.SelectionStart = _rtbLog.Text.Length;
        _rtbLog.ScrollToCaret();
    }

    private void UpdateProgress(int percent, string title, string detail)
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action(() => UpdateProgress(percent, title, detail)));
            return;
        }

        _progressBar.Value = Math.Clamp(percent, 0, 100);
        _lblPercent.Text = $"{percent}%";
        _lblStepTitle.Text = title;
        _lblStepDetail.Text = detail;
    }

    private async Task StartInstallationProcessAsync()
    {
        _pnlWelcome.Visible = false;
        _pnlProgress.Visible = true;

        AppendLog("================================================================================");
        AppendLog("[HASIERA]: 1T Jatetxea Erronka - Instalazio eta Konfigurazio Automatikoa");
        AppendLog("[DATA]: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        AppendLog("[BIDEA]: " + _rootDir);
        AppendLog("================================================================================");

        await Task.Run(async () =>
        {
            try
            {
                // -------------------------------------------------------------
                // 1. URRATSA: Baldintzen Egiaztapena
                // -------------------------------------------------------------
                UpdateProgress(10, "1/6 Urratsa: Ingurunea eta baldintzak egiaztatzen...", "Docker Desktop eta .NET SDK egiaztatzen");
                AppendLog("\n>>> 1/6 URRATSA: Baldintzen egiaztapena...");

                bool dotnetOk = await RunCmdAsync("dotnet", "--version", _rootDir);
                if (!dotnetOk)
                {
                    AppendLog("[KONTUZ]: .NET SDK ez da aurkitu PATH aldagaian. Ziurtatu instalatuta dagoela.");
                }

                bool dockerOk = await RunCmdAsync("docker", "--version", _rootDir);
                if (!dockerOk)
                {
                    AppendLog("[ERROREA]: Docker ez dago eskuragarri. Mesedez, abiarazi Docker Desktop!");
                }
                else
                {
                    await RunCmdAsync("docker", "info", _rootDir);
                }

                // -------------------------------------------------------------
                // 2. URRATSA: Docker Sareak eta Bolumenak
                // -------------------------------------------------------------
                UpdateProgress(25, "2/6 Urratsa: Docker sarea eta bolumenak sortzen...", "jatetxea_net eta bolumen iraunkorrak");
                AppendLog("\n>>> 2/6 URRATSA: Docker azpiegitura prestatzen...");

                await RunCmdAsync("docker", "network create jatetxea_net", _rootDir);
                await RunCmdAsync("docker", "volume create jatetxea_db_data", _rootDir);
                await RunCmdAsync("docker", "volume create odoo_postgres_data", _rootDir);
                await RunCmdAsync("docker", "volume create odoo_web_data", _rootDir);

                // -------------------------------------------------------------
                // 3. URRATSA: Docker Irudiak deskargatu eta eraiki
                // -------------------------------------------------------------
                UpdateProgress(50, "3/6 Urratsa: Docker irudiak deskargatzen eta eraikitzen...", "docker compose build & pull");
                AppendLog("\n>>> 3/6 URRATSA: Docker irudiak deskargatzen eta zerbitzuak eraikitzen...");

                await RunCmdAsync("docker", "compose build jatetxea-db jatetxea-api jatetxea-web jatetxea-chat jatetxea-kds-tv", _rootDir);
                await RunCmdAsync("docker", "compose pull db-odoo phpmyadmin adminer jatetxea-odoo", _rootDir);

                // -------------------------------------------------------------
                // 4. URRATSA: C# REST API Nagusia konpilatu
                // -------------------------------------------------------------
                UpdateProgress(70, "4/6 Urratsa: C# REST API Nagusia (.NET) eraikitzen...", "backend_api proiektuaren konpilazioa");
                AppendLog("\n>>> 4/6 URRATSA: C# REST API Nagusia leheneratzen eta konpilatzen...");

                string apiProj = Path.Combine(_rootDir, "01_aplikazioak", "backend_api", "Jatetxea.BackendApi.csproj");
                if (File.Exists(apiProj))
                {
                    await RunCmdAsync("dotnet", $"build \"{apiProj}\" -c Release", Path.GetDirectoryName(apiProj)!);
                }
                else
                {
                    AppendLog("[OHARRA]: backend_api proiektua ez da aurkitu bide horretan: " + apiProj);
                }

                // -------------------------------------------------------------
                // 5. URRATSA: IT Kontrol Zentrala Zerbitzaria konpilatu
                // -------------------------------------------------------------
                UpdateProgress(85, "5/6 Urratsa: IT Kontrol Zentrala (5000) eraikitzen...", "Admin Zentralaren konpilazioa");
                AppendLog("\n>>> 5/6 URRATSA: IT Kontrol Zentralaren zerbitzaria eraikitzen...");

                string serverProj = Path.Combine(_rootDir, "01_aplikazioak", "admin_IT_zentrala", "server", "Jatetxea.AdminZentrala", "Jatetxea.AdminZentrala.csproj");
                if (File.Exists(serverProj))
                {
                    await RunCmdAsync("dotnet", $"build \"{serverProj}\" -c Release", Path.GetDirectoryName(serverProj)!);
                }

                // -------------------------------------------------------------
                // 6. URRATSA: Lasterbidea eta Konfigurazio Fitxategia Gorde
                // -------------------------------------------------------------
                UpdateProgress(95, "6/6 Urratsa: Mahaigaineko lasterbidea eta erregistroa sortzen...", "konfigurazioa_amaituta.json sortzen");
                AppendLog("\n>>> 6/6 URRATSA: Amaierako doikuntzak eta lasterbidea...");

                CreateDesktopShortcut();
                SaveSetupMarkerFile();

                // -------------------------------------------------------------
                // BUKAERA: Arrakasta
                // -------------------------------------------------------------
                UpdateProgress(100, "Instalazioa eta konfigurazioa arrakastaz burutu da!", "Guztia prest dago erabiltzeko.");
                AppendLog("\n================================================================================");
                AppendLog("[AMAITUTA]: Proiektua %100 prest dago!");
                AppendLog("Orain IT Kontrol Zentrala paneletik edozein zerbitzu piztu dezakezu klik bakar batean.");
                AppendLog("================================================================================");

                // Start AdminZentrala in background
                StartAdminZentrala();

                Invoke(new Action(() =>
                {
                    _btnLaunch.Visible = true;
                    _btnClose.Visible = true;
                }));
            }
            catch (Exception ex)
            {
                AppendLog("\n[ERRORE LARRI BAT GERTATU DA]: " + ex.Message);
                UpdateProgress(_progressBar.Value, "Errorea gertatu da instalazioan", ex.Message);
                Invoke(new Action(() => { _btnClose.Visible = true; }));
            }
        });
    }

    private async Task<bool> RunCmdAsync(string exe, string args, string workingDir)
    {
        AppendLog($"$ {exe} {args}");
        var psi = new ProcessStartInfo
        {
            FileName = exe,
            Arguments = args,
            WorkingDirectory = workingDir,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var proc = new Process { StartInfo = psi, EnableRaisingEvents = true };
        proc.OutputDataReceived += (s, e) => { if (!string.IsNullOrEmpty(e.Data)) AppendLog("  " + e.Data); };
        proc.ErrorDataReceived += (s, e) => { if (!string.IsNullOrEmpty(e.Data)) AppendLog("  [ERROREA] " + e.Data); };

        try
        {
            proc.Start();
            proc.BeginOutputReadLine();
            proc.BeginErrorReadLine();
            await proc.WaitForExitAsync();
            return proc.ExitCode == 0;
        }
        catch (Exception ex)
        {
            AppendLog($"  [KOMANDO HUTS]: {ex.Message}");
            return false;
        }
    }

    private bool ValidateProjectDirectory(string path)
    {
        // _btnAlreadyInstalled beti gaituta mantendu erabiltzaileak nahi duenean aurrera jarraitu ahal izateko ("egiaztapena eginda edo egin gabe ere")
        if (_btnAlreadyInstalled != null) _btnAlreadyInstalled.Enabled = true;

        if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
        {
            _lblValidation.Text = "Kargu: Karpeta hau ez da existitzen sisteman.";
            _lblValidation.ForeColor = Color.FromArgb(248, 113, 113); // red-400
            if (_btnInstall != null) _btnInstall.Enabled = false;
            return false;
        }

        bool hasCompose = File.Exists(Path.Combine(path, "docker-compose.yml"));
        bool hasApps = Directory.Exists(Path.Combine(path, "01_aplikazioak"));

        if (hasCompose && hasApps)
        {
            _lblValidation.Text = "Baliozkoa: 1T Jatetxeko fitxategi guztiak (docker-compose eta 01_aplikazioak) ondo aurkitu dira!";
            _lblValidation.ForeColor = Color.FromArgb(52, 211, 153); // emerald-400
            if (_btnInstall != null) _btnInstall.Enabled = true;
            _rootDir = path;
            return true;
        }
        else
        {
            _lblValidation.Text = "Kargu: Karpetan ez dira aurkitu docker-compose.yml edo 01_aplikazioak!";
            _lblValidation.ForeColor = Color.FromArgb(251, 146, 60); // amber-400
            if (_btnInstall != null) _btnInstall.Enabled = false;
            _rootDir = path;
            return false;
        }
    }

    private void CreateDesktopShortcut()
    {
        try
        {
            string desktopDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string shortcutPath = Path.Combine(desktopDir, "1T Jatetxea - Admin IT Zentrala.lnk");
            string targetExe = Path.Combine(_rootDir, "Admin_IT_Zentrala.exe");
            string iconPath = Path.Combine(_rootDir, "admin_it_zentrala.ico");

            Type? shellType = Type.GetTypeFromProgID("WScript.Shell");
            if (shellType == null) return;

            dynamic? shell = Activator.CreateInstance(shellType);
            if (shell == null) return;

            dynamic shortcut = shell.CreateShortcut(shortcutPath);
            shortcut.TargetPath = targetExe;
            shortcut.WorkingDirectory = _rootDir;
            shortcut.Description = "1T Jatetxea - IT Kontrol Zentrala Kudeaketa Panela (5000)";
            if (File.Exists(iconPath))
            {
                shortcut.IconLocation = iconPath + ",0";
            }
            shortcut.Save();
            AppendLog("[LASTERBIDEA SORTUTA]: " + shortcutPath);
        }
        catch (Exception ex)
        {
            AppendLog("[LASTERBIDE ERROREA]: " + ex.Message);
        }
    }

    private void SaveSetupMarkerFile()
    {
        try
        {
            var info = new
            {
                Proiektua = "1T Jatetxea Erronka",
                ProiektuBidea = _rootDir,
                Bertsioa = "1.0.0",
                InstalazioData = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Erabiltzailea = Environment.UserName,
                Makina = Environment.MachineName,
                Osagaiak = new[]
                {
                    "Docker Sareak (jatetxea_net)",
                    "Datu-Baseen Bolumenak (MySQL, Postgres, Odoo)",
                    "Docker Edukiontziak (MySQL, Postgres, phpMyAdmin, Adminer, Web, Chat, KDS TV, Odoo)",
                    "ASP.NET Core REST API (.NET 10/11)",
                    "IT Kontrol Zentrala (Portua 5000)",
                    "Mahaigaineko Lasterbidea"
                },
                Egoera = "KONFIGURATUA_ETA_PREST"
            };

            string jsonStr = JsonSerializer.Serialize(info, new JsonSerializerOptions { WriteIndented = true });

            // 1. Gorde erroko karpetan
            string markerPath = Path.Combine(_rootDir, "konfigurazioa_amaituta.json");
            File.WriteAllText(markerPath, jsonStr);
            AppendLog("[KONFIGURAZIO FITXATEGIA GORDETA]: " + markerPath);

            // 2. Gorde LocalApplicationData-n (exekutagarria kanpora mugituko balitz ere gogoratzeko)
            try
            {
                string localAppDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "1T_Jatetxea");
                Directory.CreateDirectory(localAppDir);
                File.WriteAllText(Path.Combine(localAppDir, "konfigurazioa.json"), jsonStr);
            }
            catch { }
        }
        catch (Exception ex)
        {
            AppendLog("[ERROREA FITXATEGIA GORDETZEAN]: " + ex.Message);
        }
    }

    private async Task EnsureAdminZentralaRunningAsync()
    {
        if (await IsServerRunningAsync()) return;

        StartAdminZentrala();

        for (int i = 0; i < 25; i++)
        {
            await Task.Delay(300);
            if (await IsServerRunningAsync()) break;
        }
    }

    private async Task<bool> IsServerRunningAsync()
    {
        try
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromMilliseconds(500) };
            var res = await client.GetAsync(_targetUrl);
            return res.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private void StartAdminZentrala()
    {
        try
        {
            string serverDir = Path.Combine(_rootDir, "01_aplikazioak", "admin_IT_zentrala", "server", "Jatetxea.AdminZentrala");
            string projPath = Path.Combine(serverDir, "Jatetxea.AdminZentrala.csproj");
            string dllRelease = Path.Combine(serverDir, "bin", "Release", "net11.0", "Jatetxea.AdminZentrala.dll");
            string dllDebug = Path.Combine(serverDir, "bin", "Debug", "net11.0", "Jatetxea.AdminZentrala.dll");

            string args;
            if (File.Exists(dllRelease))
            {
                args = $"\"{dllRelease}\" --urls \"{_targetUrl}\"";
            }
            else if (File.Exists(dllDebug))
            {
                args = $"\"{dllDebug}\" --urls \"{_targetUrl}\"";
            }
            else
            {
                args = $"run --project \"{projPath}\" --urls \"{_targetUrl}\"";
            }

            string dotnetPath = File.Exists(@"C:\Program Files\dotnet\dotnet.exe") ? @"C:\Program Files\dotnet\dotnet.exe" : "dotnet";
            var psi = new ProcessStartInfo
            {
                FileName = dotnetPath,
                Arguments = args,
                WorkingDirectory = serverDir,
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            Process.Start(psi);
            AppendLog("[ZERBITZARIA ABIARAZITA]: " + _targetUrl);
        }
        catch (Exception ex)
        {
            AppendLog("[ZERBITZARI ERROREA]: " + ex.Message);
        }
    }

    private void OpenBrowser()
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = _targetUrl,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show("Ezin izan da nabigatzailea automatikoki ireki:\n" + ex.Message, "1T Jatetxea", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
