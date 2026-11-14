using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace AdminLauncher;

public static class Program
{
    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern int MessageBoxW(IntPtr hWnd, string text, string caption, uint type);

    private const uint MB_OK = 0x00000000;
    private const uint MB_ICONERROR = 0x00000010;
    private const uint MB_ICONINFORMATION = 0x00000040;

    private const string TargetUrl = "http://localhost:5000";

    [STAThread]
    public static async Task Main(string[] args)
    {
        try
        {
            string rootDir = FindProjectRoot();
            EnsureDesktopShortcut(rootDir);

            // Zuzeneko irekiera modua (soilik argumentu esplizitua pasatzen bada, adibidez --direct edo --silent):
            bool directLaunch = args.Length > 0 && (args[0] == "--direct" || args[0] == "--silent");

            if (!directLaunch)
            {
                // Erabiltzaileak exekutagarria klikatzean, BETI morroi grafikoa irekitzen da:
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new SetupWizardForm(rootDir));
                return;
            }

            // Zuzenean ireki da (--direct): egiaztatu zerbitzaria martxan dagoen
            bool running = await IsServerRunningAsync();
            if (!running)
            {
                StartServer(rootDir);

                // Polling: itxaron zerbitzaria erantzuteko prest egon arte (gehienez 15 segundu)
                bool ready = false;
                for (int i = 0; i < 30; i++)
                {
                    await Task.Delay(300);
                    if (await IsServerRunningAsync())
                    {
                        ready = true;
                        break;
                    }
                }

                if (!ready)
                {
                    MessageBoxW(
                        IntPtr.Zero,
                        "Ezin izan da IT Kontrol Zentrala (5000) abiarazi 15 segundotan.\nEgiaztatu .NET SDK eta portuaren egoera.",
                        "1T Jatetxea - Errorea",
                        MB_OK | MB_ICONERROR
                    );
                    return;
                }
            }

            // Ireki nabigatzailea zuzenean IT Kontrol Zentralera (http://localhost:5000/)
            Process.Start(new ProcessStartInfo
            {
                FileName = TargetUrl + "/",
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            MessageBoxW(
                IntPtr.Zero,
                "Errorea gertatu da abiaraztean:\n" + ex.Message,
                "1T Jatetxea - Admin IT Zentrala",
                MB_OK | MB_ICONERROR
            );
        }
    }

    private static string FindProjectRoot()
    {
        // 0. Egiaztatu LocalApplicationData-n aurretik gordetako bidea (exekutagarria kanpora mugitua bada ere)
        try
        {
            string appDataConfig = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "1T_Jatetxea", "konfigurazioa.json");
            if (File.Exists(appDataConfig))
            {
                using var doc = System.Text.Json.JsonDocument.Parse(File.ReadAllText(appDataConfig));
                if (doc.RootElement.TryGetProperty("ProiektuBidea", out var pathElem))
                {
                    string savedPath = pathElem.GetString() ?? "";
                    if (!string.IsNullOrEmpty(savedPath) &&
                        File.Exists(Path.Combine(savedPath, "docker-compose.yml")) &&
                        Directory.Exists(Path.Combine(savedPath, "01_aplikazioak")))
                    {
                        return savedPath;
                    }
                }
            }
        }
        catch { }

        // 1. Saiatu exekutagarriaren karpeta erabiltzen
        string? candidate = AppDomain.CurrentDomain.BaseDirectory;
        while (!string.IsNullOrEmpty(candidate))
        {
            if (File.Exists(Path.Combine(candidate, "docker-compose.yml")) ||
                Directory.Exists(Path.Combine(candidate, "01_aplikazioak")))
            {
                return candidate;
            }
            var parent = Directory.GetParent(candidate);
            if (parent == null) break;
            candidate = parent.FullName;
        }

        // 2. Saiatu uneko karpetatik
        candidate = Directory.GetCurrentDirectory();
        while (!string.IsNullOrEmpty(candidate))
        {
            if (File.Exists(Path.Combine(candidate, "docker-compose.yml")) ||
                Directory.Exists(Path.Combine(candidate, "01_aplikazioak")))
            {
                return candidate;
            }
            var parent = Directory.GetParent(candidate);
            if (parent == null) break;
            candidate = parent.FullName;
        }

        return AppDomain.CurrentDomain.BaseDirectory;
    }

    private static bool IsProjectAlreadyInstalled(string rootDir)
    {
        string markerPath = Path.Combine(rootDir, "konfigurazioa_amaituta.json");
        if (File.Exists(markerPath)) return true;

        string serverDir = Path.Combine(rootDir, "01_aplikazioak", "admin_IT_zentrala", "server", "Jatetxea.AdminZentrala");
        string apiDir = Path.Combine(rootDir, "01_aplikazioak", "backend_api");

        bool hasAdminDll =
            File.Exists(Path.Combine(serverDir, "bin", "Release", "net11.0", "Jatetxea.AdminZentrala.dll")) ||
            File.Exists(Path.Combine(serverDir, "bin", "Debug", "net11.0", "Jatetxea.AdminZentrala.dll"));

        bool hasApiDll =
            File.Exists(Path.Combine(apiDir, "bin", "Release", "net10.0", "Jatetxea.Api.dll")) ||
            File.Exists(Path.Combine(apiDir, "bin", "Debug", "net10.0", "Jatetxea.Api.dll")) ||
            File.Exists(Path.Combine(apiDir, "obj", "Debug", "net10.0", "Jatetxea.Api.dll"));

        if (hasAdminDll && hasApiDll)
        {
            // Dagoeneko instalatuta dagoela baieztatuta: erregistratu automatikoki hurrengo aldietarako
            try
            {
                var info = new
                {
                    Proiektua = "1T Jatetxea Erronka",
                    InstalazioData = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Detekzioa = "AUTODETEKZIOA_BITARRAK_PREST",
                    Egoera = "KONFIGURATUA_ETA_PREST"
                };
                File.WriteAllText(markerPath, System.Text.Json.JsonSerializer.Serialize(info, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));
            }
            catch { }

            return true;
        }

        return false;
    }

    private static async Task<bool> IsServerRunningAsync()
    {
        try
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromMilliseconds(500) };
            var res = await client.GetAsync(TargetUrl);
            return res.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private static void StartServer(string rootDir)
    {
        string serverDir = Path.Combine(rootDir, "01_aplikazioak", "admin_IT_zentrala", "server", "Jatetxea.AdminZentrala");
        string projPath = Path.Combine(serverDir, "Jatetxea.AdminZentrala.csproj");
        string dllRelease = Path.Combine(serverDir, "bin", "Release", "net11.0", "Jatetxea.AdminZentrala.dll");
        string dllDebug = Path.Combine(serverDir, "bin", "Debug", "net11.0", "Jatetxea.AdminZentrala.dll");

        string args;
        if (File.Exists(dllRelease))
        {
            args = $"\"{dllRelease}\" --urls \"{TargetUrl}\"";
        }
        else if (File.Exists(dllDebug))
        {
            args = $"\"{dllDebug}\" --urls \"{TargetUrl}\"";
        }
        else
        {
            args = $"run --project \"{projPath}\" --urls \"{TargetUrl}\"";
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
    }

    private static void EnsureDesktopShortcut(string rootDir)
    {
        try
        {
            string desktopDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string shortcutPath = Path.Combine(desktopDir, "1T Jatetxea - Admin IT Zentrala.lnk");
            string targetExe = Path.Combine(rootDir, "Admin_IT_Zentrala.exe");
            string iconPath = Path.Combine(rootDir, "admin_it_zentrala.ico");

            if (!File.Exists(shortcutPath) && File.Exists(targetExe))
            {
                CreateShortcut(shortcutPath, targetExe, rootDir, iconPath, "1T Jatetxea - IT Kontrol Zentrala");
            }
        }
        catch
        {
            // Lasterbidea sortzeak huts egiten badu, ez oztopatu abiaraztea
        }
    }

    private static void CreateShortcut(string shortcutPath, string targetPath, string workingDir, string iconPath, string description)
    {
        Type? shellType = Type.GetTypeFromProgID("WScript.Shell");
        if (shellType == null) return;

        dynamic? shell = Activator.CreateInstance(shellType);
        if (shell == null) return;

        dynamic shortcut = shell.CreateShortcut(shortcutPath);
        shortcut.TargetPath = targetPath;
        shortcut.WorkingDirectory = workingDir;
        shortcut.Description = description;
        if (File.Exists(iconPath))
        {
            shortcut.IconLocation = iconPath + ",0";
        }
        shortcut.Save();
    }
}
