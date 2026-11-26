namespace Jatetxea.Desktop.Forms;

partial class MahaiaXehetasunakForm
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
        pnlEgoeraChanger = new Panel();
        lblEgoeraEtiketa = new Label();
        btnEgoeraLibre = new Button();
        btnEgoeraOkupatuta = new Button();
        btnEgoeraGarbitzen = new Button();
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
        pnlEgoeraChanger.SuspendLayout();
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
        pnlTopBar.Controls.Add(pnlEgoeraChanger);
        pnlTopBar.Controls.Add(lblZerbitzaria);
        pnlTopBar.Controls.Add(lblMahaiaTit);
        pnlTopBar.Controls.Add(btnAtzera);
        pnlTopBar.Dock = DockStyle.Top;
        pnlTopBar.Location = new Point(0, 0);
        pnlTopBar.Name = "pnlTopBar";
        pnlTopBar.Size = new Size(1366, 88);
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
        btnAtzera.Size = new Size(145, 48);
        btnAtzera.TabIndex = 0;
        btnAtzera.Text = "< Mahaiak";
        btnAtzera.TextAlign = ContentAlignment.MiddleCenter;
        btnAtzera.UseVisualStyleBackColor = false;
        // 
        // lblMahaiaTit ("T1 Jatetxea - Mahaia 4")
        // 
        lblMahaiaTit.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        lblMahaiaTit.ForeColor = Color.FromArgb(136, 19, 55);
        lblMahaiaTit.Location = new Point(175, 20);
        lblMahaiaTit.Name = "lblMahaiaTit";
        lblMahaiaTit.Size = new Size(345, 48);
        lblMahaiaTit.TabIndex = 1;
        lblMahaiaTit.Text = "T1 Jatetxea - Mahaia 4";
        lblMahaiaTit.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // lblZerbitzaria
        // 
        lblZerbitzaria.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        lblZerbitzaria.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblZerbitzaria.ForeColor = Color.FromArgb(71, 85, 105);
        lblZerbitzaria.Location = new Point(530, 20);
        lblZerbitzaria.Name = "lblZerbitzaria";
        lblZerbitzaria.Size = new Size(340, 48);
        lblZerbitzaria.TabIndex = 2;
        lblZerbitzaria.Text = "Zerbitzaria: Ane G. (4 pax)";
        lblZerbitzaria.TextAlign = ContentAlignment.MiddleRight;
        // 
        // pnlEgoeraChanger
        // 
        pnlEgoeraChanger.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        pnlEgoeraChanger.BackColor = Color.White;
        pnlEgoeraChanger.Controls.Add(btnEgoeraGarbitzen);
        pnlEgoeraChanger.Controls.Add(btnEgoeraOkupatuta);
        pnlEgoeraChanger.Controls.Add(btnEgoeraLibre);
        pnlEgoeraChanger.Controls.Add(lblEgoeraEtiketa);
        pnlEgoeraChanger.Location = new Point(880, 18);
        pnlEgoeraChanger.Name = "pnlEgoeraChanger";
        pnlEgoeraChanger.Size = new Size(465, 52);
        pnlEgoeraChanger.TabIndex = 3;
        // 
        // lblEgoeraEtiketa
        // 
        lblEgoeraEtiketa.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblEgoeraEtiketa.ForeColor = Color.FromArgb(71, 85, 105);
        lblEgoeraEtiketa.Location = new Point(0, 6);
        lblEgoeraEtiketa.Name = "lblEgoeraEtiketa";
        lblEgoeraEtiketa.Size = new Size(62, 40);
        lblEgoeraEtiketa.TabIndex = 0;
        lblEgoeraEtiketa.Text = "Egoera:";
        lblEgoeraEtiketa.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // btnEgoeraLibre
        // 
        btnEgoeraLibre.Cursor = Cursors.Hand;
        btnEgoeraLibre.FlatStyle = FlatStyle.Flat;
        btnEgoeraLibre.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnEgoeraLibre.Location = new Point(65, 4);
        btnEgoeraLibre.Name = "btnEgoeraLibre";
        btnEgoeraLibre.Size = new Size(115, 44);
        btnEgoeraLibre.TabIndex = 1;
        btnEgoeraLibre.Text = "Libre";
        btnEgoeraLibre.UseVisualStyleBackColor = false;
        // 
        // btnEgoeraOkupatuta
        // 
        btnEgoeraOkupatuta.Cursor = Cursors.Hand;
        btnEgoeraOkupatuta.FlatStyle = FlatStyle.Flat;
        btnEgoeraOkupatuta.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnEgoeraOkupatuta.Location = new Point(188, 4);
        btnEgoeraOkupatuta.Name = "btnEgoeraOkupatuta";
        btnEgoeraOkupatuta.Size = new Size(125, 44);
        btnEgoeraOkupatuta.TabIndex = 2;
        btnEgoeraOkupatuta.Text = "Okupatuta";
        btnEgoeraOkupatuta.UseVisualStyleBackColor = false;
        // 
        // btnEgoeraGarbitzen
        // 
        btnEgoeraGarbitzen.Cursor = Cursors.Hand;
        btnEgoeraGarbitzen.FlatStyle = FlatStyle.Flat;
        btnEgoeraGarbitzen.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnEgoeraGarbitzen.Location = new Point(321, 4);
        btnEgoeraGarbitzen.Name = "btnEgoeraGarbitzen";
        btnEgoeraGarbitzen.Size = new Size(130, 44);
        btnEgoeraGarbitzen.TabIndex = 3;
        btnEgoeraGarbitzen.Text = "Garbitzeko";
        btnEgoeraGarbitzen.UseVisualStyleBackColor = false;
        // 
        // pnlEskuina (Ekintza botoiak eta laburpena - Responsive)
        // 
        pnlEskuina.AutoScroll = true;
        pnlEskuina.BackColor = Color.FromArgb(248, 250, 252);
        pnlEskuina.Controls.Add(pnlActions);
        pnlEskuina.Controls.Add(pnlSummaryCard);
        pnlEskuina.Dock = DockStyle.Right;
        pnlEskuina.Location = new Point(830, 88);
        pnlEskuina.Name = "pnlEskuina";
        pnlEskuina.Padding = new Padding(14, 14, 18, 14);
        pnlEskuina.Size = new Size(420, 712);
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
        pnlSummaryCard.Location = new Point(14, 14);
        pnlSummaryCard.Name = "pnlSummaryCard";
        pnlSummaryCard.Padding = new Padding(16);
        pnlSummaryCard.Size = new Size(388, 245);
        pnlSummaryCard.TabIndex = 0;
        // 
        // lblEgoeraOrokorra
        // 
        lblEgoeraOrokorra.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        lblEgoeraOrokorra.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblEgoeraOrokorra.ForeColor = Color.FromArgb(180, 83, 9);
        lblEgoeraOrokorra.Location = new Point(16, 182);
        lblEgoeraOrokorra.Name = "lblEgoeraOrokorra";
        lblEgoeraOrokorra.Size = new Size(356, 48);
        lblEgoeraOrokorra.TabIndex = 5;
        lblEgoeraOrokorra.Text = "Platerak sukaldera bidaltzeke daude";
        lblEgoeraOrokorra.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // lblGuztira
        // 
        lblGuztira.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        lblGuztira.Font = new Font("Segoe UI", 16.5F, FontStyle.Bold);
        lblGuztira.ForeColor = Color.FromArgb(30, 41, 59);
        lblGuztira.Location = new Point(16, 128);
        lblGuztira.Name = "lblGuztira";
        lblGuztira.Size = new Size(356, 46);
        lblGuztira.TabIndex = 4;
        lblGuztira.Text = "GUZTIRA:  60,00 €";
        lblGuztira.TextAlign = ContentAlignment.MiddleRight;
        // 
        // lblBez
        // 
        lblBez.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        lblBez.Font = new Font("Segoe UI", 10F);
        lblBez.ForeColor = Color.FromArgb(100, 116, 139);
        lblBez.Location = new Point(16, 92);
        lblBez.Name = "lblBez";
        lblBez.Size = new Size(356, 30);
        lblBez.TabIndex = 3;
        lblBez.Text = "BEZ (%10 barne):  5,45 €";
        lblBez.TextAlign = ContentAlignment.MiddleRight;
        // 
        // lblAzpitotala
        // 
        lblAzpitotala.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        lblAzpitotala.Font = new Font("Segoe UI", 10.5F);
        lblAzpitotala.ForeColor = Color.FromArgb(71, 85, 105);
        lblAzpitotala.Location = new Point(16, 58);
        lblAzpitotala.Name = "lblAzpitotala";
        lblAzpitotala.Size = new Size(356, 30);
        lblAzpitotala.TabIndex = 2;
        lblAzpitotala.Text = "Azpitotala:  54,55 €";
        lblAzpitotala.TextAlign = ContentAlignment.MiddleRight;
        // 
        // pnlDividerSummary
        // 
        pnlDividerSummary.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        pnlDividerSummary.BackColor = Color.FromArgb(226, 232, 240);
        pnlDividerSummary.Location = new Point(16, 48);
        pnlDividerSummary.Name = "pnlDividerSummary";
        pnlDividerSummary.Size = new Size(356, 1);
        pnlDividerSummary.TabIndex = 1;
        // 
        // lblSummaryTitle
        // 
        lblSummaryTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        lblSummaryTitle.Font = new Font("Segoe UI", 11.5F, FontStyle.Bold);
        lblSummaryTitle.ForeColor = Color.FromArgb(30, 41, 59);
        lblSummaryTitle.Location = new Point(16, 12);
        lblSummaryTitle.Name = "lblSummaryTitle";
        lblSummaryTitle.Size = new Size(356, 32);
        lblSummaryTitle.TabIndex = 0;
        lblSummaryTitle.Text = "KONTUAREN LABURPENA";
        lblSummaryTitle.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // pnlActions
        // 
        pnlActions.AutoScroll = true;
        pnlActions.Controls.Add(btnSukalderaBidali);
        pnlActions.Controls.Add(btnGehituEskaera);
        pnlActions.Controls.Add(btnGehituKontua);
        pnlActions.Controls.Add(btnKontuaEskatu);
        pnlActions.Controls.Add(btnAldatuMahaiaz);
        pnlActions.Dock = DockStyle.Fill;
        pnlActions.Location = new Point(14, 259);
        pnlActions.Name = "pnlActions";
        pnlActions.Padding = new Padding(0, 10, 0, 0);
        pnlActions.Size = new Size(388, 439);
        pnlActions.TabIndex = 1;
        // 
        // btnSukalderaBidali ("SUKALDERA BIDALI")
        // 
        btnSukalderaBidali.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        btnSukalderaBidali.BackColor = Color.FromArgb(22, 101, 52);
        btnSukalderaBidali.Cursor = Cursors.Hand;
        btnSukalderaBidali.FlatAppearance.BorderSize = 0;
        btnSukalderaBidali.FlatStyle = FlatStyle.Flat;
        btnSukalderaBidali.Font = new Font("Segoe UI", 11.5F, FontStyle.Bold);
        btnSukalderaBidali.ForeColor = Color.White;
        btnSukalderaBidali.Location = new Point(0, 10);
        btnSukalderaBidali.Name = "btnSukalderaBidali";
        btnSukalderaBidali.Size = new Size(388, 58);
        btnSukalderaBidali.TabIndex = 0;
        btnSukalderaBidali.Text = "SUKALDERA BIDALI";
        btnSukalderaBidali.UseVisualStyleBackColor = false;
        // 
        // btnGehituEskaera ("+ PRODUKTUAK GEHITU")
        // 
        btnGehituEskaera.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        btnGehituEskaera.BackColor = Color.FromArgb(136, 19, 55);
        btnGehituEskaera.Cursor = Cursors.Hand;
        btnGehituEskaera.FlatAppearance.BorderSize = 0;
        btnGehituEskaera.FlatStyle = FlatStyle.Flat;
        btnGehituEskaera.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        btnGehituEskaera.ForeColor = Color.White;
        btnGehituEskaera.Location = new Point(0, 76);
        btnGehituEskaera.Name = "btnGehituEskaera";
        btnGehituEskaera.Size = new Size(388, 50);
        btnGehituEskaera.TabIndex = 1;
        btnGehituEskaera.Text = "+ PRODUKTUAK GEHITU";
        btnGehituEskaera.UseVisualStyleBackColor = false;
        // 
        // btnGehituKontua ("+ KONTU BERRIA (Banatua)")
        // 
        btnGehituKontua.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        btnGehituKontua.BackColor = Color.White;
        btnGehituKontua.Cursor = Cursors.Hand;
        btnGehituKontua.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnGehituKontua.FlatStyle = FlatStyle.Flat;
        btnGehituKontua.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
        btnGehituKontua.ForeColor = Color.FromArgb(15, 118, 110);
        btnGehituKontua.Location = new Point(0, 134);
        btnGehituKontua.Name = "btnGehituKontua";
        btnGehituKontua.Size = new Size(388, 46);
        btnGehituKontua.TabIndex = 2;
        btnGehituKontua.Text = "+ KONTU BERRIA (Mahaikide Banatua)";
        btnGehituKontua.UseVisualStyleBackColor = false;
        // 
        // btnKontuaEskatu ("KONTUA ESKATU / KOBRATU")
        // 
        btnKontuaEskatu.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        btnKontuaEskatu.BackColor = Color.FromArgb(241, 245, 249);
        btnKontuaEskatu.Cursor = Cursors.Hand;
        btnKontuaEskatu.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnKontuaEskatu.FlatStyle = FlatStyle.Flat;
        btnKontuaEskatu.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
        btnKontuaEskatu.ForeColor = Color.FromArgb(30, 41, 59);
        btnKontuaEskatu.Location = new Point(0, 188);
        btnKontuaEskatu.Name = "btnKontuaEskatu";
        btnKontuaEskatu.Size = new Size(388, 46);
        btnKontuaEskatu.TabIndex = 3;
        btnKontuaEskatu.Text = "KONTUA ESKATU / KOBRATU";
        btnKontuaEskatu.UseVisualStyleBackColor = false;
        // 
        // btnAldatuMahaiaz ("Aldatu Mahaiaz")
        // 
        btnAldatuMahaiaz.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        btnAldatuMahaiaz.BackColor = Color.FromArgb(248, 250, 252);
        btnAldatuMahaiaz.Cursor = Cursors.Hand;
        btnAldatuMahaiaz.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnAldatuMahaiaz.FlatStyle = FlatStyle.Flat;
        btnAldatuMahaiaz.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnAldatuMahaiaz.ForeColor = Color.FromArgb(100, 116, 139);
        btnAldatuMahaiaz.Location = new Point(0, 242);
        btnAldatuMahaiaz.Name = "btnAldatuMahaiaz";
        btnAldatuMahaiaz.Size = new Size(388, 44);
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
        pnlEskaeraInfoBar.Location = new Point(24, 76);
        pnlEskaeraInfoBar.Name = "pnlEskaeraInfoBar";
        pnlEskaeraInfoBar.Size = new Size(790, 52);
        pnlEskaeraInfoBar.TabIndex = 1;
        // 
        // lblLerroLaburpena
        // 
        lblLerroLaburpena.Dock = DockStyle.Right;
        lblLerroLaburpena.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblLerroLaburpena.ForeColor = Color.FromArgb(100, 116, 139);
        lblLerroLaburpena.Location = new Point(430, 0);
        lblLerroLaburpena.Name = "lblLerroLaburpena";
        lblLerroLaburpena.Size = new Size(360, 52);
        lblLerroLaburpena.TabIndex = 1;
        lblLerroLaburpena.Text = "3 plater (1 Zai · 2 Martxan · 0 Eginda)";
        lblLerroLaburpena.TextAlign = ContentAlignment.MiddleRight;
        lblLerroLaburpena.UseCompatibleTextRendering = false;
        // 
        // lblEskaeraInfo
        // 
        lblEskaeraInfo.Dock = DockStyle.Fill;
        lblEskaeraInfo.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
        lblEskaeraInfo.ForeColor = Color.FromArgb(30, 41, 59);
        lblEskaeraInfo.Location = new Point(0, 0);
        lblEskaeraInfo.Name = "lblEskaeraInfo";
        lblEskaeraInfo.Size = new Size(430, 52);
        lblEskaeraInfo.TabIndex = 0;
        lblEskaeraInfo.Text = "1. Kontua: Eskaera #1 · Egoera: Sukaldean";
        lblEskaeraInfo.TextAlign = ContentAlignment.MiddleLeft;
        lblEskaeraInfo.UseCompatibleTextRendering = false;
        // 
        // pnlKontuakGoiburua (Kontu fitxak - FlowLayoutPanel)
        // 
        pnlKontuakGoiburua.BackColor = Color.White;
        pnlKontuakGoiburua.Controls.Add(flowKontuFitxak);
        pnlKontuakGoiburua.Dock = DockStyle.Top;
        pnlKontuakGoiburua.Location = new Point(24, 16);
        pnlKontuakGoiburua.Name = "pnlKontuakGoiburua";
        pnlKontuakGoiburua.Padding = new Padding(6, 4, 6, 4);
        pnlKontuakGoiburua.Size = new Size(790, 60);
        pnlKontuakGoiburua.TabIndex = 0;
        // 
        // flowKontuFitxak
        // 
        flowKontuFitxak.AutoScroll = true;
        flowKontuFitxak.Dock = DockStyle.Fill;
        flowKontuFitxak.Location = new Point(6, 4);
        flowKontuFitxak.Name = "flowKontuFitxak";
        flowKontuFitxak.Size = new Size(778, 52);
        flowKontuFitxak.TabIndex = 0;
        flowKontuFitxak.WrapContents = false;
        // 
        // MahaiaXehetasunakForm (Responsive, Maximized, MinimumSize 800x500)
        // 
        AutoScaleMode = AutoScaleMode.None;
        BackColor = Color.FromArgb(241, 245, 249);
        ClientSize = new Size(1280, 800);
        Controls.Add(pnlEzkerra);
        Controls.Add(pnlEskuina);
        Controls.Add(pnlTopBar);
        FormBorderStyle = FormBorderStyle.Sizable;
        MaximizeBox = true;
        MinimizeBox = true;
        MinimumSize = new Size(800, 500);
        Name = "MahaiaXehetasunakForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "T1 Jatetxea - Mahaiaren Xehetasunak";
        WindowState = FormWindowState.Maximized;
        pnlTopBar.ResumeLayout(false);
        pnlEgoeraChanger.ResumeLayout(false);
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
    private Panel pnlEgoeraChanger;
    private Label lblEgoeraEtiketa;
    private Button btnEgoeraLibre;
    private Button btnEgoeraOkupatuta;
    private Button btnEgoeraGarbitzen;
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
