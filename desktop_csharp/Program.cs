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
            var form = new Forms.MahaiakForm(demoLangilea, api);
            form.Shown += async (s, e) =>
            {
                await Task.Delay(400);
                using var bmp = new Bitmap(form.ClientSize.Width, form.ClientSize.Height);
                form.DrawToBitmap(bmp, new Rectangle(0, 0, form.ClientSize.Width, form.ClientSize.Height));
                string outPath = args.Length > 1 ? args[1] : "screenshot_mahaiak.png";
                bmp.Save(outPath, System.Drawing.Imaging.ImageFormat.Png);
                Application.Exit();
            };
            Application.Run(form);
            return;
        }

        if (args.Length > 0 && args[0] == "--screenshot-eskaera")
        {
            var form = new Forms.EskaeraForm();
            form.Shown += async (s, e) =>
            {
                await Task.Delay(1400);
                using var bmp = new Bitmap(form.ClientSize.Width, form.ClientSize.Height);
                form.DrawToBitmap(bmp, new Rectangle(0, 0, form.ClientSize.Width, form.ClientSize.Height));
                string outPath = args.Length > 1 ? args[1] : "screenshot_eskaera.png";
                bmp.Save(outPath, System.Drawing.Imaging.ImageFormat.Png);
                Application.Exit();
            };
            Application.Run(form);
            return;
        }

        if (args.Length > 0 && args[0] == "--screenshot-mahaikideak")
        {
            var demoMahaia = new Models.MahaiaDto { Id = 4, Zenbakia = 4, Edukiera = 4 };
            var form = new Forms.MahaikideakModal(demoMahaia);
            form.Shown += async (s, e) =>
            {
                await Task.Delay(200);
                using var bmp = new Bitmap(form.ClientSize.Width, form.ClientSize.Height);
                form.DrawToBitmap(bmp, new Rectangle(0, 0, form.ClientSize.Width, form.ClientSize.Height));
                string outPath = args.Length > 1 ? args[1] : "screenshot_mahaikideak.png";
                bmp.Save(outPath, System.Drawing.Imaging.ImageFormat.Png);
                Application.Exit();
            };
            Application.Run(form);
            return;
        }

        if (args.Length > 0 && args[0] == "--screenshot-xehetasunak")
        {
            var demoMahaia = new Models.MahaiaDto
            {
                Id = 1,
                Zenbakia = 1,
                Edukiera = 2,
                Kokapena = "Jantoki Nagusia",
                Egoera = "Okupatuta"
            };
            var demoLangilea = new Models.LangileaDto { Id = 2, Izena = "Ane G.", Rola = "Zerbitzaria" };
            var api = new Services.ApiService();
            var form = new Forms.MahaiaXehetasunakForm(demoMahaia, demoLangilea, api);
            form.Shown += async (s, e) =>
            {
                await Task.Delay(1500);
                using var bmp = new Bitmap(form.ClientSize.Width, form.ClientSize.Height);
                form.DrawToBitmap(bmp, new Rectangle(0, 0, form.ClientSize.Width, form.ClientSize.Height));
                string outPath = args.Length > 1 ? args[1] : "screenshot_xehetasunak.png";
                bmp.Save(outPath, System.Drawing.Imaging.ImageFormat.Png);
                Application.Exit();
            };
            Application.Run(form);
            return;
        }

        if (args.Length > 0 && args[0] == "--screenshot")
        {
            var form = new Forms.LoginForm();
            form.Shown += async (s, e) =>
            {
                await Task.Delay(1500);
                using var bmp = new Bitmap(form.ClientSize.Width, form.ClientSize.Height);
                form.DrawToBitmap(bmp, new Rectangle(0, 0, form.ClientSize.Width, form.ClientSize.Height));
                string outPath = args.Length > 1 ? args[1] : "screenshot_login.png";
                bmp.Save(outPath, System.Drawing.Imaging.ImageFormat.Png);
                Application.Exit();
            };
            Application.Run(form);
            return;
        }

        Application.Run(new Forms.LoginForm());
    }    
}
