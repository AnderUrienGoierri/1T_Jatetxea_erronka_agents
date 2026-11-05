namespace Jatetxea.Desktop.Forms;

partial class EskaeraForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        pnlTopBar = new Panel();
        lblZerbitzaria = new Label();
        lblMahaiaTit = new Label();
        btnAtzera = new Button();
        splitMain = new SplitContainer();
        pnlPlaterak = new FlowLayoutPanel();
        pnlKategoriaIzenburua = new Panel();
        lblKategoriaAktiboa = new Label();
        pnlKategoriak = new FlowLayoutPanel();
        btnKatEdariak = new Button();
        btnKatHastekoak = new Button();
        btnKatHaragiak = new Button();
        btnKatPostreak = new Button();
        btnKatGuztiak = new Button();
        pnlLaburpena = new Panel();
        btnBidali = new Button();
        lblGuztira = new Label();
        lblBez = new Label();
        lblAzpitotala = new Label();
        pnlKarritoa = new FlowLayoutPanel();
        pnlKomandaGoiburua = new Panel();
        lblKomandaTit = new Label();
        pnlTopBar.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitMain).BeginInit();
        splitMain.Panel1.SuspendLayout();
        splitMain.Panel2.SuspendLayout();
        splitMain.SuspendLayout();
        pnlKategoriaIzenburua.SuspendLayout();
        pnlKategoriak.SuspendLayout();
        pnlLaburpena.SuspendLayout();
        pnlKomandaGoiburua.SuspendLayout();
        SuspendLayout();
        // 
        // pnlTopBar (Goiko barra ardo-ilun/zuria - POS tablet estiloko goiburu zabala)
        // 
        pnlTopBar.BackColor = Color.White;
        pnlTopBar.Controls.Add(lblZerbitzaria);
        pnlTopBar.Controls.Add(lblMahaiaTit);
        pnlTopBar.Controls.Add(btnAtzera);
        pnlTopBar.Dock = DockStyle.Top;
        pnlTopBar.Location = new Point(0, 0);
        pnlTopBar.Name = "pnlTopBar";
        pnlTopBar.Size = new Size(1280, 84);
        pnlTopBar.TabIndex = 0;
        // 
        // lblZerbitzaria
        // 
        lblZerbitzaria.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        lblZerbitzaria.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblZerbitzaria.ForeColor = Color.FromArgb(71, 85, 105);
        lblZerbitzaria.Location = new Point(810, 20);
        lblZerbitzaria.Name = "lblZerbitzaria";
        lblZerbitzaria.Size = new Size(530, 44);
        lblZerbitzaria.TabIndex = 2;
        lblZerbitzaria.Text = "Zerbitzaria: Ane G. (4 pax)";
        lblZerbitzaria.TextAlign = ContentAlignment.MiddleRight;
        lblZerbitzaria.UseCompatibleTextRendering = false;
        // 
        // lblMahaiaTit
        // 
        lblMahaiaTit.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        lblMahaiaTit.ForeColor = Color.FromArgb(136, 19, 55);
        lblMahaiaTit.Location = new Point(195, 18);
        lblMahaiaTit.Name = "lblMahaiaTit";
        lblMahaiaTit.Size = new Size(600, 48);
        lblMahaiaTit.TabIndex = 1;
        lblMahaiaTit.Text = "T1 Jatetxea - Mahaia 4";
        lblMahaiaTit.TextAlign = ContentAlignment.MiddleLeft;
        lblMahaiaTit.UseCompatibleTextRendering = false;
        // 
        // btnAtzera ("< Mahaiak")
        // 
        btnAtzera.BackColor = Color.FromArgb(241, 245, 249);
        btnAtzera.Cursor = Cursors.Hand;
        btnAtzera.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnAtzera.FlatStyle = FlatStyle.Flat;
        btnAtzera.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
        btnAtzera.ForeColor = Color.FromArgb(51, 65, 85);
        btnAtzera.Location = new Point(20, 18);
        btnAtzera.Name = "btnAtzera";
        btnAtzera.Size = new Size(160, 48);
        btnAtzera.TabIndex = 0;
        btnAtzera.Text = "< Mahaiak";
        btnAtzera.TextAlign = ContentAlignment.MiddleCenter;
        btnAtzera.UseCompatibleTextRendering = false;
        btnAtzera.UseVisualStyleBackColor = false;
        // 
        // splitMain (Ezkerra Platerak, Eskuina Komanda)
        // 
        splitMain.Dock = DockStyle.Fill;
        splitMain.FixedPanel = FixedPanel.Panel2;
        splitMain.IsSplitterFixed = true;
        splitMain.Location = new Point(0, 84);
        splitMain.Name = "splitMain";
        // 
        // splitMain.Panel1 (Ezkerra: Kategoriak eta Platerak)
        // 
        splitMain.Panel1.BackColor = Color.FromArgb(248, 250, 252);
        splitMain.Panel1.Controls.Add(pnlPlaterak);
        splitMain.Panel1.Controls.Add(pnlKategoriaIzenburua);
        splitMain.Panel1.Controls.Add(pnlKategoriak);
        splitMain.Panel1.Padding = new Padding(16, 12, 16, 12);
        // 
        // splitMain.Panel2 (Eskuina: Komandaren laburpena eta Sortu)
        // 
        splitMain.Panel2.BackColor = Color.White;
        splitMain.Panel2.Controls.Add(pnlKarritoa);
        splitMain.Panel2.Controls.Add(pnlLaburpena);
        splitMain.Panel2.Controls.Add(pnlKomandaGoiburua);
        splitMain.Panel2.Padding = new Padding(16);
        splitMain.Size = new Size(1366, 766);
        splitMain.SplitterDistance = 910;
        splitMain.TabIndex = 1;
        // 
        // pnlPlaterak (Produktuen txartelen flow panela)
        // 
        pnlPlaterak.AutoScroll = true;
        pnlPlaterak.Dock = DockStyle.Fill;
        pnlPlaterak.Location = new Point(20, 118);
        pnlPlaterak.Name = "pnlPlaterak";
        pnlPlaterak.Size = new Size(820, 582);
        pnlPlaterak.TabIndex = 2;
        // 
        // pnlKategoriaIzenburua ("EDARIAK")
        // 
        pnlKategoriaIzenburua.Controls.Add(lblKategoriaAktiboa);
        pnlKategoriaIzenburua.Dock = DockStyle.Top;
        pnlKategoriaIzenburua.Location = new Point(20, 68);
        pnlKategoriaIzenburua.Name = "pnlKategoriaIzenburua";
        pnlKategoriaIzenburua.Size = new Size(820, 50);
        pnlKategoriaIzenburua.TabIndex = 1;
        // 
        // lblKategoriaAktiboa ("EDARIAK")
        // 
        lblKategoriaAktiboa.Dock = DockStyle.Left;
        lblKategoriaAktiboa.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        lblKategoriaAktiboa.ForeColor = Color.FromArgb(30, 41, 59);
        lblKategoriaAktiboa.Location = new Point(0, 0);
        lblKategoriaAktiboa.Name = "lblKategoriaAktiboa";
        lblKategoriaAktiboa.Size = new Size(350, 50);
        lblKategoriaAktiboa.TabIndex = 0;
        lblKategoriaAktiboa.Text = "EDARIAK";
        lblKategoriaAktiboa.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // pnlKategoriak (EDARIAK, HASTEKOAK, HARAGIAK, POSTREAK...)
        // 
        pnlKategoriak.AutoScroll = false;
        pnlKategoriak.Controls.Add(btnKatEdariak);
        pnlKategoriak.Controls.Add(btnKatHastekoak);
        pnlKategoriak.Controls.Add(btnKatHaragiak);
        pnlKategoriak.Controls.Add(btnKatPostreak);
        pnlKategoriak.Controls.Add(btnKatGuztiak);
        pnlKategoriak.Dock = DockStyle.Top;
        pnlKategoriak.Location = new Point(20, 14);
        pnlKategoriak.Name = "pnlKategoriak";
        pnlKategoriak.Padding = new Padding(0, 4, 0, 4);
        pnlKategoriak.Size = new Size(820, 58);
        pnlKategoriak.TabIndex = 0;
        pnlKategoriak.WrapContents = false;
        // 
        // btnKatEdariak
        // 
        btnKatEdariak.BackColor = Color.White;
        btnKatEdariak.Cursor = Cursors.Hand;
        btnKatEdariak.FlatAppearance.BorderSize = 0;
        btnKatEdariak.FlatStyle = FlatStyle.Flat;
        btnKatEdariak.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        btnKatEdariak.ForeColor = Color.FromArgb(136, 19, 55);
        btnKatEdariak.Location = new Point(0, 0);
        btnKatEdariak.Margin = new Padding(0, 0, 10, 0);
        btnKatEdariak.Name = "btnKatEdariak";
        btnKatEdariak.Padding = new Padding(12, 0, 12, 0);
        btnKatEdariak.Size = new Size(140, 48);
        btnKatEdariak.TabIndex = 0;
        btnKatEdariak.Text = "Edariak";
        btnKatEdariak.UseVisualStyleBackColor = false;
        // 
        // btnKatHastekoak
        // 
        btnKatHastekoak.BackColor = Color.White;
        btnKatHastekoak.Cursor = Cursors.Hand;
        btnKatHastekoak.FlatAppearance.BorderSize = 0;
        btnKatHastekoak.FlatStyle = FlatStyle.Flat;
        btnKatHastekoak.Font = new Font("Segoe UI", 11F);
        btnKatHastekoak.ForeColor = Color.FromArgb(100, 116, 139);
        btnKatHastekoak.Location = new Point(150, 0);
        btnKatHastekoak.Margin = new Padding(0, 0, 10, 0);
        btnKatHastekoak.Name = "btnKatHastekoak";
        btnKatHastekoak.Padding = new Padding(12, 0, 12, 0);
        btnKatHastekoak.Size = new Size(150, 48);
        btnKatHastekoak.TabIndex = 1;
        btnKatHastekoak.Text = "Hastekoak";
        btnKatHastekoak.UseVisualStyleBackColor = false;
        // 
        // btnKatHaragiak
        // 
        btnKatHaragiak.BackColor = Color.White;
        btnKatHaragiak.Cursor = Cursors.Hand;
        btnKatHaragiak.FlatAppearance.BorderSize = 0;
        btnKatHaragiak.FlatStyle = FlatStyle.Flat;
        btnKatHaragiak.Font = new Font("Segoe UI", 11F);
        btnKatHaragiak.ForeColor = Color.FromArgb(100, 116, 139);
        btnKatHaragiak.Location = new Point(310, 0);
        btnKatHaragiak.Margin = new Padding(0, 0, 10, 0);
        btnKatHaragiak.Name = "btnKatHaragiak";
        btnKatHaragiak.Padding = new Padding(12, 0, 12, 0);
        btnKatHaragiak.Size = new Size(140, 48);
        btnKatHaragiak.TabIndex = 2;
        btnKatHaragiak.Text = "Haragiak";
        btnKatHaragiak.UseVisualStyleBackColor = false;
        // 
        // btnKatPostreak
        // 
        btnKatPostreak.BackColor = Color.White;
        btnKatPostreak.Cursor = Cursors.Hand;
        btnKatPostreak.FlatAppearance.BorderSize = 0;
        btnKatPostreak.FlatStyle = FlatStyle.Flat;
        btnKatPostreak.Font = new Font("Segoe UI", 11F);
        btnKatPostreak.ForeColor = Color.FromArgb(100, 116, 139);
        btnKatPostreak.Location = new Point(460, 0);
        btnKatPostreak.Margin = new Padding(0, 0, 10, 0);
        btnKatPostreak.Name = "btnKatPostreak";
        btnKatPostreak.Padding = new Padding(12, 0, 12, 0);
        btnKatPostreak.Size = new Size(140, 48);
        btnKatPostreak.TabIndex = 3;
        btnKatPostreak.Text = "Postreak";
        btnKatPostreak.UseVisualStyleBackColor = false;
        // 
        // btnKatGuztiak
        // 
        btnKatGuztiak.BackColor = Color.White;
        btnKatGuztiak.Cursor = Cursors.Hand;
        btnKatGuztiak.FlatAppearance.BorderSize = 0;
        btnKatGuztiak.FlatStyle = FlatStyle.Flat;
        btnKatGuztiak.Font = new Font("Segoe UI", 11F);
        btnKatGuztiak.ForeColor = Color.FromArgb(100, 116, 139);
        btnKatGuztiak.Location = new Point(610, 0);
        btnKatGuztiak.Margin = new Padding(0, 0, 10, 0);
        btnKatGuztiak.Name = "btnKatGuztiak";
        btnKatGuztiak.Padding = new Padding(12, 0, 12, 0);
        btnKatGuztiak.Size = new Size(130, 48);
        btnKatGuztiak.TabIndex = 4;
        btnKatGuztiak.Text = "Guztiak";
        btnKatGuztiak.UseVisualStyleBackColor = false;
        // 
        // pnlLaburpena (Beheko totalak eta ESKAERA SORTU botoia)
        // 
        pnlLaburpena.Controls.Add(btnBidali);
        pnlLaburpena.Controls.Add(lblGuztira);
        pnlLaburpena.Controls.Add(lblBez);
        pnlLaburpena.Controls.Add(lblAzpitotala);
        pnlLaburpena.Dock = DockStyle.Bottom;
        pnlLaburpena.Location = new Point(16, 490);
        pnlLaburpena.Name = "pnlLaburpena";
        pnlLaburpena.Size = new Size(424, 240);
        pnlLaburpena.TabIndex = 2;
        // 
        // btnBidali ("ESKAERA SORTU")
        // 
        btnBidali.BackColor = Color.FromArgb(136, 19, 55);
        btnBidali.Cursor = Cursors.Hand;
        btnBidali.FlatAppearance.BorderSize = 0;
        btnBidali.FlatStyle = FlatStyle.Flat;
        btnBidali.Font = new Font("Segoe UI", 13.5F, FontStyle.Bold);
        btnBidali.ForeColor = Color.White;
        btnBidali.Location = new Point(6, 150);
        btnBidali.Name = "btnBidali";
        btnBidali.Size = new Size(412, 70);
        btnBidali.TabIndex = 3;
        btnBidali.Text = "ESKAERA SORTU";
        btnBidali.UseVisualStyleBackColor = false;
        // 
        // lblGuztira ("Guztira: 43.45 €")
        // 
        lblGuztira.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        lblGuztira.ForeColor = Color.FromArgb(30, 41, 59);
        lblGuztira.Location = new Point(6, 96);
        lblGuztira.Name = "lblGuztira";
        lblGuztira.Size = new Size(412, 46);
        lblGuztira.TabIndex = 2;
        lblGuztira.Text = "Guztira:                        0.00 €";
        lblGuztira.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // lblBez ("BEZ (%10): 3.95 €")
        // 
        lblBez.Font = new Font("Segoe UI", 11.5F);
        lblBez.ForeColor = Color.FromArgb(100, 116, 139);
        lblBez.Location = new Point(6, 54);
        lblBez.Name = "lblBez";
        lblBez.Size = new Size(412, 34);
        lblBez.TabIndex = 1;
        lblBez.Text = "BEZ (%10):                    0.00 €";
        lblBez.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // lblAzpitotala ("Azpitotala: 39.50 €")
        // 
        lblAzpitotala.Font = new Font("Segoe UI", 11.5F);
        lblAzpitotala.ForeColor = Color.FromArgb(100, 116, 139);
        lblAzpitotala.Location = new Point(6, 14);
        lblAzpitotala.Name = "lblAzpitotala";
        lblAzpitotala.Size = new Size(412, 34);
        lblAzpitotala.TabIndex = 0;
        lblAzpitotala.Text = "Azpitotala:                   0.00 €";
        lblAzpitotala.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // pnlKarritoa (Aukeratutako produktuen zerrenda oharrekin)
        // 
        pnlKarritoa.AutoScroll = true;
        pnlKarritoa.Dock = DockStyle.Fill;
        pnlKarritoa.FlowDirection = FlowDirection.TopDown;
        pnlKarritoa.Location = new Point(16, 58);
        pnlKarritoa.Name = "pnlKarritoa";
        pnlKarritoa.Size = new Size(424, 432);
        pnlKarritoa.TabIndex = 1;
        pnlKarritoa.WrapContents = false;
        // 
        // pnlKomandaGoiburua ("ESKAERA - Mahaia 4")
        // 
        pnlKomandaGoiburua.Controls.Add(lblKomandaTit);
        pnlKomandaGoiburua.Dock = DockStyle.Top;
        pnlKomandaGoiburua.Location = new Point(16, 16);
        pnlKomandaGoiburua.Name = "pnlKomandaGoiburua";
        pnlKomandaGoiburua.Size = new Size(424, 44);
        pnlKomandaGoiburua.TabIndex = 0;
        // 
        // lblKomandaTit
        // 
        lblKomandaTit.Dock = DockStyle.Fill;
        lblKomandaTit.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
        lblKomandaTit.ForeColor = Color.FromArgb(30, 41, 59);
        lblKomandaTit.Location = new Point(0, 0);
        lblKomandaTit.Name = "lblKomandaTit";
        lblKomandaTit.Size = new Size(424, 44);
        lblKomandaTit.TabIndex = 0;
        lblKomandaTit.Text = "ESKAERA - Mahaia 4";
        lblKomandaTit.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // EskaeraForm
        // 
        AutoScaleMode = AutoScaleMode.None;
        BackColor = Color.FromArgb(248, 250, 252);
        ClientSize = new Size(1366, 850);
        Controls.Add(splitMain);
        Controls.Add(pnlTopBar);
        MinimumSize = new Size(1180, 750);
        Name = "EskaeraForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "T1 Jatetxea - Eskaera Berria";
        pnlTopBar.ResumeLayout(false);
        splitMain.Panel1.ResumeLayout(false);
        splitMain.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitMain).EndInit();
        splitMain.ResumeLayout(false);
        pnlKategoriaIzenburua.ResumeLayout(false);
        pnlKategoriak.ResumeLayout(false);
        pnlLaburpena.ResumeLayout(false);
        pnlKomandaGoiburua.ResumeLayout(false);
        ResumeLayout(false);
    }

    private Panel pnlTopBar;
    private Button btnAtzera;
    private Label lblMahaiaTit;
    private Label lblZerbitzaria;
    private SplitContainer splitMain;
    private FlowLayoutPanel pnlKategoriak;
    private Button btnKatEdariak;
    private Button btnKatHastekoak;
    private Button btnKatHaragiak;
    private Button btnKatPostreak;
    private Button btnKatGuztiak;
    private Panel pnlKategoriaIzenburua;
    private Label lblKategoriaAktiboa;
    private FlowLayoutPanel pnlPlaterak;
    private Panel pnlKomandaGoiburua;
    private Label lblKomandaTit;
    private FlowLayoutPanel pnlKarritoa;
    private Panel pnlLaburpena;
    private Label lblAzpitotala;
    private Label lblBez;
    private Label lblGuztira;
    private Button btnBidali;
}

