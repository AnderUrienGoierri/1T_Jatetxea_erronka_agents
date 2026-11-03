namespace Jatetxea.Desktop;

static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();
        if (args.Length > 0 && args[0] == "--screenshot-mahaiak")
        {
            var demoLangilea = new Models.LangileaDto { Id = 2, Izena = "Ane G.", Rola = "Zerbitzaria" };
            var api = new Services.ApiService();
            using var form = new Forms.MahaiakForm(demoLangilea, api);
            form.Show();
            for (int i = 0; i < 20; i++)
            {
                Application.DoEvents();
                System.Threading.Thread.Sleep(50);
            }
            using var bmp = new Bitmap(form.Width, form.Height);
            form.DrawToBitmap(bmp, new Rectangle(0, 0, form.Width, form.Height));
            string outPath = args.Length > 1 ? args[1] : "screenshot_mahaiak.png";
            bmp.Save(outPath, System.Drawing.Imaging.ImageFormat.Png);
            return;
        }

        if (args.Length > 0 && args[0] == "--screenshot")
        {
            using var form = new Forms.LoginForm();
            form.Show();
            for (int i = 0; i < 15; i++)
            {
                Application.DoEvents();
                System.Threading.Thread.Sleep(50);
            }
            using var bmp = new Bitmap(form.Width, form.Height);
            form.DrawToBitmap(bmp, new Rectangle(0, 0, form.Width, form.Height));
            string outPath = args.Length > 1 ? args[1] : "screenshot_login.png";
            bmp.Save(outPath, System.Drawing.Imaging.ImageFormat.Png);
            return;
        }

        Application.Run(new Forms.LoginForm());
    }    
}
