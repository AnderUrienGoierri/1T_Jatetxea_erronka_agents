namespace Jatetxea.Desktop;

static class Program
{
    [STAThread]
    static void Main()
    {
        try
        {
            Application.SetHighDpiMode(HighDpiMode.DpiUnaware);
        }
        catch { }

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new Forms.LoginForm());
    }
}
