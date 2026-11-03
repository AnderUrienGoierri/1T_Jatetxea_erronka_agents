namespace Jatetxea.Desktop;

static class Program
{
    /// <summary>
    /// Aplikazioaren sarrera puntu nagusia.
    /// </summary>
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new Forms.LoginForm());
    }    
}
