namespace Jatetxea.Desktop.Forms;

partial class MahaiaXehetasunakModal
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
        btnAtzera = new Button();
        lblMahaiaTit = new Label();
        lblZerbitzaria = new Label();
        pnlEskuina = new Panel();
        pnlSummaryCard = new Panel();
        lblEgoeraOrokorra = new Label();
        lblGuztira = new Label();
        lblBez = new Label();
        lblAzpitotala = new Label();
        pnlDividerSummary = new Panel();
        lblSummaryTitle = new Label();
        pnlActions = new Panel();
        btnSukalderaBidali = new Button();
        btnGehituEskaera = new Button();
        btnGehituKontua = new Button();
        btnKontuaEskatu = new Button();
        btnAldatuMahaiaz = new Button();
        pnlEzkerra = new Panel();
        pnlTicketCard = new Panel();
        pnlTicketLines = new Panel();
        pnlEskaeraInfoBar = new Panel();
        lblLerroLaburpena = new Label();
        lblEskaeraInfo = new Label();
        pnlKontuakGoiburua = new Panel();
        flowKontuFitxak = new FlowLayoutPanel();
        pnlTopBar.SuspendLayout();
        pnlEskuina.SuspendLayout();
        pnlSummaryCard.SuspendLayout();
        pnlActions.SuspendLayout();
        pnlEzkerra.SuspendLayout();
        pnlTicketCard.SuspendLayout();
        pnlEskaeraInfoBar.SuspendLayout();
        pnlKontuakGoiburua.SuspendLayout();
        SuspendLayout();
        // 
        // pnlTopBar (POS Tablet estiloko goiburu zabala - 88px)
        // 
        pnlTopBar.BackColor = Color.White;
        pnlTopBar.Controls.Add(lblZerbitzaria);
        pnlTopBar.Controls.Add(lblMahaiaTit);
        pnlTopBar.Controls.Add(btnAtzera);
        pnlTopBar.Dock = DockStyle.Top;
        pnlTopBar.Location = new Point(0, 0);
        pnlTopBar.Name = "pnlTopBar";
        pnlTopBar.Size = new Size(1280, 88);
        pnlTopBar.TabIndex = 0;
        // 
        // btnAtzera ("< Mahaiak")
        // 
        btnAtzera.BackColor = Color.White;
        btnAtzera.Cursor = Cursors.Hand;
        btnAtzera.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnAtzera.FlatStyle = FlatStyle.Flat;
        btnAtzera.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
        btnAtzera.ForeColor = Color.FromArgb(51, 65, 85);
        btnAtzera.Location = new Point(20, 20);
        btnAtzera.Name = "btnAtzera";
        btnAtzera.Size = new Size(175, 48);
        btnAtzera.TabIndex = 0;
        btnAtzera.Text = "< Mahaiak";
        btnAtzera.TextAlign = ContentAlignment.MiddleCenter;
        btnAtzera.UseCompatibleTextRendering = true;
        btnAtzera.UseVisualStyleBackColor = false;
        // 
        // lblMahaiaTit ("T1 Jatetxea - Mahaia 4")
        // 
        lblMahaiaTit.Font = new Font("Segoe UI", 17F, FontStyle.Bold);
        lblMahaiaTit.ForeColor = Color.FromArgb(136, 19, 55);
        lblMahaiaTit.Location = new Point(210, 20);
        lblMahaiaTit.Name = "lblMahaiaTit";
        lblMahaiaTit.Size = new Size(520, 48);
        lblMahaiaTit.TabIndex = 1;
        lblMahaiaTit.Text = "T1 Jatetxea - Mahaia 4";
        lblMahaiaTit.TextAlign = ContentAlignment.MiddleLeft;
        lblMahaiaTit.UseCompatibleTextRendering = true;
        // 
        // lblZerbitzaria
        // 
        lblZerbitzaria.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        lblZerbitzaria.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblZerbitzaria.ForeColor = Color.FromArgb(71, 85, 105);
        lblZerbitzaria.Location = new Point(660, 24);
        lblZerbitzaria.Name = "lblZerbitzaria";
        lblZerbitzaria.Size = new Size(590, 40);
        lblZerbitzaria.TabIndex = 2;
        lblZerbitzaria.Text = "Zerbitzaria: Ane G. (4 pax) · Okupatuta";
        lblZerbitzaria.TextAlign = ContentAlignment.MiddleRight;
        lblZerbitzaria.UseCompatibleTextRendering = true;
        // 
        // pnlEskuina (Ekintza botoiak eta laburpena - 440px)
        // 
        pnlEskuina.BackColor = Color.FromArgb(248, 250, 252);
        pnlEskuina.Controls.Add(pnlActions);
        pnlEskuina.Controls.Add(pnlSummaryCard);
        pnlEskuina.Dock = DockStyle.Right;
        pnlEskuina.Location = new Point(830, 88);
        pnlEskuina.Name = "pnlEskuina";
        pnlEskuina.Padding = new Padding(16, 16, 24, 16);
        pnlEskuina.Size = new Size(450, 712);
        pnlEskuina.TabIndex = 2;
        // 
        // pnlSummaryCard
        // 
        pnlSummaryCard.BackColor = Color.White;
        pnlSummaryCard.Controls.Add(lblEgoeraOrokorra);
        pnlSummaryCard.Controls.Add(lblGuztira);
        pnlSummaryCard.Controls.Add(lblBez);
        pnlSummaryCard.Controls.Add(lblAzpitotala);
        pnlSummaryCard.Controls.Add(pnlDividerSummary);
        pnlSummaryCard.Controls.Add(lblSummaryTitle);
        pnlSummaryCard.Dock = DockStyle.Top;
        pnlSummaryCard.Location = new Point(16, 16);
        pnlSummaryCard.Name = "pnlSummaryCard";
        pnlSummaryCard.Padding = new Padding(18);
        pnlSummaryCard.Size = new Size(410, 230);
        pnlSummaryCard.TabIndex = 0;
        // 
        // lblEgoeraOrokorra
        // 
        lblEgoeraOrokorra.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblEgoeraOrokorra.ForeColor = Color.FromArgb(180, 83, 9);
        lblEgoeraOrokorra.Location = new Point(18, 182);
        lblEgoeraOrokorra.Name = "lblEgoeraOrokorra";
        lblEgoeraOrokorra.Size = new Size(374, 32);
        lblEgoeraOrokorra.TabIndex = 5;
        lblEgoeraOrokorra.Text = "Platerak sukaldera bidaltzeke daude";
        lblEgoeraOrokorra.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // lblGuztira
        // 
        lblGuztira.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
        lblGuztira.ForeColor = Color.FromArgb(30, 41, 59);
        lblGuztira.Location = new Point(18, 130);
        lblGuztira.Name = "lblGuztira";
        lblGuztira.Size = new Size(374, 46);
        lblGuztira.TabIndex = 4;
        lblGuztira.Text = "GUZTIRA:  60,00 €";
        lblGuztira.TextAlign = ContentAlignment.MiddleRight;
        // 
        // lblBez
        // 
        lblBez.Font = new Font("Segoe UI", 10.5F);
        lblBez.ForeColor = Color.FromArgb(100, 116, 139);
        lblBez.Location = new Point(18, 94);
        lblBez.Name = "lblBez";
        lblBez.Size = new Size(374, 28);
        lblBez.TabIndex = 3;
        lblBez.Text = "BEZ (%10 barne):  5,45 €";
        lblBez.TextAlign = ContentAlignment.MiddleRight;
        // 
        // lblAzpitotala
        // 
        lblAzpitotala.Font = new Font("Segoe UI", 11F);
        lblAzpitotala.ForeColor = Color.FromArgb(71, 85, 105);
        lblAzpitotala.Location = new Point(18, 62);
        lblAzpitotala.Name = "lblAzpitotala";
        lblAzpitotala.Size = new Size(374, 28);
        lblAzpitotala.TabIndex = 2;
        lblAzpitotala.Text = "Azpitotala:  54,55 €";
        lblAzpitotala.TextAlign = ContentAlignment.MiddleRight;
        // 
        // pnlDividerSummary
        // 
        pnlDividerSummary.BackColor = Color.FromArgb(226, 232, 240);
        pnlDividerSummary.Location = new Point(18, 50);
        pnlDividerSummary.Name = "pnlDividerSummary";
        pnlDividerSummary.Size = new Size(374, 1);
        pnlDividerSummary.TabIndex = 1;
        // 
        // lblSummaryTitle
        // 
        lblSummaryTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        lblSummaryTitle.ForeColor = Color.FromArgb(30, 41, 59);
        lblSummaryTitle.Location = new Point(18, 14);
        lblSummaryTitle.Name = "lblSummaryTitle";
        lblSummaryTitle.Size = new Size(374, 30);
        lblSummaryTitle.TabIndex = 0;
        lblSummaryTitle.Text = "KONTUAREN LABURPENA";
        lblSummaryTitle.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // pnlActions
        // 
        pnlActions.Controls.Add(btnSukalderaBidali);
        pnlActions.Controls.Add(btnGehituEskaera);
        pnlActions.Controls.Add(btnGehituKontua);
        pnlActions.Controls.Add(btnKontuaEskatu);
        pnlActions.Controls.Add(btnAldatuMahaiaz);
        pnlActions.Dock = DockStyle.Fill;
        pnlActions.Location = new Point(16, 246);
        pnlActions.Name = "pnlActions";
        pnlActions.Padding = new Padding(0, 16, 0, 0);
        pnlActions.Size = new Size(410, 450);
        pnlActions.TabIndex = 1;
        // 
        // btnSukalderaBidali ("SUKALDERA BIDALI")
        // 
        btnSukalderaBidali.BackColor = Color.FromArgb(22, 101, 52);
        btnSukalderaBidali.Cursor = Cursors.Hand;
        btnSukalderaBidali.FlatAppearance.BorderSize = 0;
        btnSukalderaBidali.FlatStyle = FlatStyle.Flat;
        btnSukalderaBidali.Font = new Font("Segoe UI", 12.5F, FontStyle.Bold);
        btnSukalderaBidali.ForeColor = Color.White;
        btnSukalderaBidali.Location = new Point(0, 16);
        btnSukalderaBidali.Name = "btnSukalderaBidali";
        btnSukalderaBidali.Size = new Size(410, 68);
        btnSukalderaBidali.TabIndex = 0;
        btnSukalderaBidali.Text = "SUKALDERA BIDALI";
        btnSukalderaBidali.UseVisualStyleBackColor = false;
        // 
        // btnGehituEskaera ("+ PRODUKTUAK GEHITU")
        // 
        btnGehituEskaera.BackColor = Color.FromArgb(136, 19, 55);
        btnGehituEskaera.Cursor = Cursors.Hand;
        btnGehituEskaera.FlatAppearance.BorderSize = 0;
        btnGehituEskaera.FlatStyle = FlatStyle.Flat;
        btnGehituEskaera.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        btnGehituEskaera.ForeColor = Color.White;
        btnGehituEskaera.Location = new Point(0, 96);
        btnGehituEskaera.Name = "btnGehituEskaera";
        btnGehituEskaera.Size = new Size(410, 58);
        btnGehituEskaera.TabIndex = 1;
        btnGehituEskaera.Text = "+ PRODUKTUAK GEHITU";
        btnGehituEskaera.UseVisualStyleBackColor = false;
        // 
        // btnGehituKontua ("+ KONTU BERRIA (Banatua)")
        // 
        btnGehituKontua.BackColor = Color.White;
        btnGehituKontua.Cursor = Cursors.Hand;
        btnGehituKontua.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnGehituKontua.FlatStyle = FlatStyle.Flat;
        btnGehituKontua.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        btnGehituKontua.ForeColor = Color.FromArgb(15, 118, 110);
        btnGehituKontua.Location = new Point(0, 166);
        btnGehituKontua.Name = "btnGehituKontua";
        btnGehituKontua.Size = new Size(410, 52);
        btnGehituKontua.TabIndex = 2;
        btnGehituKontua.Text = "+ KONTU BERRIA (Mahaikide Banatua)";
        btnGehituKontua.UseVisualStyleBackColor = false;
        // 
        // btnKontuaEskatu ("KONTUA ESKATU / KOBRATU")
        // 
        btnKontuaEskatu.BackColor = Color.FromArgb(241, 245, 249);
        btnKontuaEskatu.Cursor = Cursors.Hand;
        btnKontuaEskatu.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnKontuaEskatu.FlatStyle = FlatStyle.Flat;
        btnKontuaEskatu.Font = new Font("Segoe UI", 11.5F, FontStyle.Bold);
        btnKontuaEskatu.ForeColor = Color.FromArgb(30, 41, 59);
        btnKontuaEskatu.Location = new Point(0, 230);
        btnKontuaEskatu.Name = "btnKontuaEskatu";
        btnKontuaEskatu.Size = new Size(410, 52);
        btnKontuaEskatu.TabIndex = 3;
        btnKontuaEskatu.Text = "KONTUA ESKATU / KOBRATU";
        btnKontuaEskatu.UseVisualStyleBackColor = false;
        // 
        // btnAldatuMahaiaz ("Aldatu Mahaiaz")
        // 
        btnAldatuMahaiaz.BackColor = Color.FromArgb(248, 250, 252);
        btnAldatuMahaiaz.Cursor = Cursors.Hand;
        btnAldatuMahaiaz.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnAldatuMahaiaz.FlatStyle = FlatStyle.Flat;
        btnAldatuMahaiaz.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
        btnAldatuMahaiaz.ForeColor = Color.FromArgb(100, 116, 139);
        btnAldatuMahaiaz.Location = new Point(0, 294);
        btnAldatuMahaiaz.Name = "btnAldatuMahaiaz";
        btnAldatuMahaiaz.Size = new Size(410, 48);
        btnAldatuMahaiaz.TabIndex = 4;
        btnAldatuMahaiaz.Text = "Aldatu Mahaiaz";
        btnAldatuMahaiaz.UseVisualStyleBackColor = false;
        // 
        // pnlEzkerra (Kontuak eta Eskaera lerroak - Dock Fill)
        // 
        pnlEzkerra.Controls.Add(pnlTicketCard);
        pnlEzkerra.Controls.Add(pnlEskaeraInfoBar);
        pnlEzkerra.Controls.Add(pnlKontuakGoiburua);
        pnlEzkerra.Dock = DockStyle.Fill;
        pnlEzkerra.Location = new Point(0, 88);
        pnlEzkerra.Name = "pnlEzkerra";
        pnlEzkerra.Padding = new Padding(24, 16, 16, 20);
        pnlEzkerra.Size = new Size(830, 712);
        pnlEzkerra.TabIndex = 1;
        // 
        // pnlTicketCard (Plateren zerrendaren kaxa)
        // 
        pnlTicketCard.BackColor = Color.White;
        pnlTicketCard.Controls.Add(pnlTicketLines);
        pnlTicketCard.Dock = DockStyle.Fill;
        pnlTicketCard.Location = new Point(24, 116);
        pnlTicketCard.Name = "pnlTicketCard";
        pnlTicketCard.Padding = new Padding(12);
        pnlTicketCard.Size = new Size(790, 576);
        pnlTicketCard.TabIndex = 2;
        // 
        // pnlTicketLines (Scrollable lerroen zerrenda)
        // 
        pnlTicketLines.AutoScroll = true;
        pnlTicketLines.Dock = DockStyle.Fill;
        pnlTicketLines.Location = new Point(12, 12);
        pnlTicketLines.Name = "pnlTicketLines";
        pnlTicketLines.Padding = new Padding(4);
        pnlTicketLines.Size = new Size(766, 552);
        pnlTicketLines.TabIndex = 0;
        // 
        // pnlEskaeraInfoBar (Egoera laburpen barra)
        // 
        pnlEskaeraInfoBar.BackColor = Color.FromArgb(241, 245, 249);
        pnlEskaeraInfoBar.Controls.Add(lblLerroLaburpena);
        pnlEskaeraInfoBar.Controls.Add(lblEskaeraInfo);
        pnlEskaeraInfoBar.Dock = DockStyle.Top;
        pnlEskaeraInfoBar.Location = new Point(24, 72);
        pnlEskaeraInfoBar.Name = "pnlEskaeraInfoBar";
        pnlEskaeraInfoBar.Size = new Size(790, 44);
        pnlEskaeraInfoBar.TabIndex = 1;
        // 
        // lblLerroLaburpena
        // 
        lblLerroLaburpena.Dock = DockStyle.Right;
        lblLerroLaburpena.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
        lblLerroLaburpena.ForeColor = Color.FromArgb(100, 116, 139);
        lblLerroLaburpena.Location = new Point(410, 0);
        lblLerroLaburpena.Name = "lblLerroLaburpena";
        lblLerroLaburpena.Size = new Size(380, 44);
        lblLerroLaburpena.TabIndex = 1;
        lblLerroLaburpena.Text = "3 plater (1 Zai · 2 Martxan · 0 Eginda)";
        lblLerroLaburpena.TextAlign = ContentAlignment.MiddleRight;
        // 
        // lblEskaeraInfo
        // 
        lblEskaeraInfo.Dock = DockStyle.Fill;
        lblEskaeraInfo.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblEskaeraInfo.ForeColor = Color.FromArgb(30, 41, 59);
        lblEskaeraInfo.Location = new Point(0, 0);
        lblEskaeraInfo.Name = "lblEskaeraInfo";
        lblEskaeraInfo.Size = new Size(410, 44);
        lblEskaeraInfo.TabIndex = 0;
        lblEskaeraInfo.Text = "1. Kontua: Eskaera #1 · Egoera: Sukaldean";
        lblEskaeraInfo.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // pnlKontuakGoiburua (Kontu fitxak - FlowLayoutPanel)
        // 
        pnlKontuakGoiburua.BackColor = Color.White;
        pnlKontuakGoiburua.Controls.Add(flowKontuFitxak);
        pnlKontuakGoiburua.Dock = DockStyle.Top;
        pnlKontuakGoiburua.Location = new Point(24, 16);
        pnlKontuakGoiburua.Name = "pnlKontuakGoiburua";
        pnlKontuakGoiburua.Padding = new Padding(6, 4, 6, 4);
        pnlKontuakGoiburua.Size = new Size(790, 56);
        pnlKontuakGoiburua.TabIndex = 0;
        // 
        // flowKontuFitxak
        // 
        flowKontuFitxak.AutoScroll = true;
        flowKontuFitxak.Dock = DockStyle.Fill;
        flowKontuFitxak.Location = new Point(6, 4);
        flowKontuFitxak.Name = "flowKontuFitxak";
        flowKontuFitxak.Size = new Size(778, 48);
        flowKontuFitxak.TabIndex = 0;
        flowKontuFitxak.WrapContents = false;
        // 
        // MahaiaXehetasunakModal (Orain pantaila normala da, 1280x800 Tablet POS diseinuan)
        // 
        AutoScaleMode = AutoScaleMode.None;
        BackColor = Color.FromArgb(241, 245, 249);
        ClientSize = new Size(1280, 800);
        Controls.Add(pnlEzkerra);
        Controls.Add(pnlEskuina);
        Controls.Add(pnlTopBar);
        MinimumSize = new Size(1024, 720);
        Name = "MahaiaXehetasunakModal";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "T1 Jatetxea - Mahaiaren Xehetasunak";
        pnlTopBar.ResumeLayout(false);
        pnlEskuina.ResumeLayout(false);
        pnlSummaryCard.ResumeLayout(false);
        pnlActions.ResumeLayout(false);
        pnlEzkerra.ResumeLayout(false);
        pnlTicketCard.ResumeLayout(false);
        pnlEskaeraInfoBar.ResumeLayout(false);
        pnlKontuakGoiburua.ResumeLayout(false);
        ResumeLayout(false);
    }

    private Panel pnlTopBar;
    private Button btnAtzera;
    private Label lblMahaiaTit;
    private Label lblZerbitzaria;
    private Panel pnlEskuina;
    private Panel pnlSummaryCard;
    private Label lblSummaryTitle;
    private Panel pnlDividerSummary;
    private Label lblAzpitotala;
    private Label lblBez;
    private Label lblGuztira;
    private Label lblEgoeraOrokorra;
    private Panel pnlActions;
    private Button btnSukalderaBidali;
    private Button btnGehituEskaera;
    private Button btnGehituKontua;
    private Button btnKontuaEskatu;
    private Button btnAldatuMahaiaz;
    private Panel pnlEzkerra;
    private Panel pnlKontuakGoiburua;
    private FlowLayoutPanel flowKontuFitxak;
    private Panel pnlEskaeraInfoBar;
    private Label lblEskaeraInfo;
    private Label lblLerroLaburpena;
    private Panel pnlTicketCard;
    private Panel pnlTicketLines;
}
