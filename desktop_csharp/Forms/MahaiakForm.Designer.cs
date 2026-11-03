namespace Jatetxea.Desktop.Forms;

partial class MahaiakForm
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
        lblLogo = new Label();
        lblBrand = new Label();
        pnlProfile = new Panel();
        btnLogout = new Button();
        btnFreskatu = new Button();
        lblServerStatus = new Label();
        lblServerName = new Label();
        lblAvatarCircle = new Label();
        pnlTabStrip = new Panel();
        btnTabJangela = new Button();
        btnTabTerraza = new Button();
        btnTabBarra = new Button();
        btnTabVIP = new Button();
        btnTabGuztiak = new Button();
        pnlContent = new Panel();
        pnlMahaiak = new FlowLayoutPanel();
        pnlSectionHeader = new Panel();
        lblSummary = new Label();
        lblSectionTitle = new Label();
        pnlTopBar.SuspendLayout();
        pnlProfile.SuspendLayout();
        pnlTabStrip.SuspendLayout();
        pnlContent.SuspendLayout();
        pnlSectionHeader.SuspendLayout();
        SuspendLayout();

        // 
        // pnlTopBar (Goiko barra zuria)
        // 
        pnlTopBar.BackColor = Color.White;
        pnlTopBar.Controls.Add(pnlProfile);
        pnlTopBar.Controls.Add(lblBrand);
        pnlTopBar.Controls.Add(lblLogo);
        pnlTopBar.Dock = DockStyle.Top;
        pnlTopBar.Location = new Point(0, 0);
        pnlTopBar.Name = "pnlTopBar";
        pnlTopBar.Size = new Size(1184, 65);
        pnlTopBar.TabIndex = 0;

        // 
        // lblLogo ("T1" txapa)
        // 
        lblLogo.BackColor = Color.FromArgb(136, 19, 55);
        lblLogo.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        lblLogo.ForeColor = Color.White;
        lblLogo.Location = new Point(20, 12);
        lblLogo.Name = "lblLogo";
        lblLogo.Size = new Size(42, 42);
        lblLogo.TabIndex = 0;
        lblLogo.Text = "T1";
        lblLogo.TextAlign = ContentAlignment.MiddleCenter;

        // 
        // lblBrand ("T1 Jatetxea - Mahaiak")
        // 
        lblBrand.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
        lblBrand.ForeColor = Color.FromArgb(136, 19, 55);
        lblBrand.Location = new Point(72, 16);
        lblBrand.Name = "lblBrand";
        lblBrand.Size = new Size(340, 34);
        lblBrand.TabIndex = 1;
        lblBrand.Text = "T1 Jatetxea - Mahaiak";
        lblBrand.TextAlign = ContentAlignment.MiddleLeft;

        // 
        // pnlProfile (Eskuineko erabiltzaile profila eta ekintzak)
        // 
        pnlProfile.Controls.Add(btnLogout);
        pnlProfile.Controls.Add(btnFreskatu);
        pnlProfile.Controls.Add(lblServerStatus);
        pnlProfile.Controls.Add(lblServerName);
        pnlProfile.Controls.Add(lblAvatarCircle);
        pnlProfile.Dock = DockStyle.Right;
        pnlProfile.Location = new Point(544, 0);
        pnlProfile.Name = "pnlProfile";
        pnlProfile.Size = new Size(640, 65);
        pnlProfile.TabIndex = 2;

        // 
        // btnLogout ("Saioa Itxi")
        // 
        btnLogout.BackColor = Color.FromArgb(136, 19, 55);
        btnLogout.Cursor = Cursors.Hand;
        btnLogout.FlatAppearance.BorderSize = 0;
        btnLogout.FlatStyle = FlatStyle.Flat;
        btnLogout.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnLogout.ForeColor = Color.White;
        btnLogout.Location = new Point(505, 12);
        btnLogout.Name = "btnLogout";
        btnLogout.Size = new Size(115, 40);
        btnLogout.TabIndex = 4;
        btnLogout.Text = "Saioa Itxi";
        btnLogout.UseVisualStyleBackColor = false;

        // 
        // btnFreskatu ("Freskatu")
        // 
        btnFreskatu.BackColor = Color.FromArgb(241, 245, 249);
        btnFreskatu.Cursor = Cursors.Hand;
        btnFreskatu.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnFreskatu.FlatStyle = FlatStyle.Flat;
        btnFreskatu.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnFreskatu.ForeColor = Color.FromArgb(51, 65, 85);
        btnFreskatu.Location = new Point(375, 12);
        btnFreskatu.Name = "btnFreskatu";
        btnFreskatu.Size = new Size(115, 40);
        btnFreskatu.TabIndex = 3;
        btnFreskatu.Text = "Freskatu";
        btnFreskatu.UseVisualStyleBackColor = false;

        // 
        // lblServerStatus ("● Online")
        // 
        lblServerStatus.Font = new Font("Segoe UI", 9F);
        lblServerStatus.ForeColor = Color.FromArgb(22, 163, 74);
        lblServerStatus.Location = new Point(205, 34);
        lblServerStatus.Name = "lblServerStatus";
        lblServerStatus.Size = new Size(100, 20);
        lblServerStatus.TabIndex = 2;
        lblServerStatus.Text = "● Online";
        lblServerStatus.TextAlign = ContentAlignment.MiddleLeft;

        // 
        // lblServerName (Zerbitzariaren izena)
        // 
        lblServerName.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblServerName.ForeColor = Color.FromArgb(30, 41, 59);
        lblServerName.Location = new Point(205, 12);
        lblServerName.Name = "lblServerName";
        lblServerName.Size = new Size(150, 22);
        lblServerName.TabIndex = 1;
        lblServerName.Text = "Ane G.";
        lblServerName.TextAlign = ContentAlignment.MiddleLeft;

        // 
        // lblAvatarCircle (Profil borobila)
        // 
        lblAvatarCircle.BackColor = Color.FromArgb(239, 246, 255);
        lblAvatarCircle.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
        lblAvatarCircle.ForeColor = Color.FromArgb(37, 99, 235);
        lblAvatarCircle.Location = new Point(150, 10);
        lblAvatarCircle.Name = "lblAvatarCircle";
        lblAvatarCircle.Size = new Size(44, 44);
        lblAvatarCircle.TabIndex = 0;
        lblAvatarCircle.Text = "AG";
        lblAvatarCircle.TextAlign = ContentAlignment.MiddleCenter;

        // 
        // pnlTabStrip (Guneen fitxak: Jangela Nagusia, Terraza, Barra, VIP, Guztiak)
        // 
        pnlTabStrip.BackColor = Color.White;
        pnlTabStrip.Controls.Add(btnTabGuztiak);
        pnlTabStrip.Controls.Add(btnTabVIP);
        pnlTabStrip.Controls.Add(btnTabBarra);
        pnlTabStrip.Controls.Add(btnTabTerraza);
        pnlTabStrip.Controls.Add(btnTabJangela);
        pnlTabStrip.Dock = DockStyle.Top;
        pnlTabStrip.Location = new Point(0, 65);
        pnlTabStrip.Name = "pnlTabStrip";
        pnlTabStrip.Padding = new Padding(20, 4, 20, 0);
        pnlTabStrip.Size = new Size(1184, 52);
        pnlTabStrip.TabIndex = 1;

        // 
        // btnTabJangela
        // 
        btnTabJangela.BackColor = Color.White;
        btnTabJangela.Cursor = Cursors.Hand;
        btnTabJangela.FlatAppearance.BorderSize = 0;
        btnTabJangela.FlatStyle = FlatStyle.Flat;
        btnTabJangela.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnTabJangela.ForeColor = Color.FromArgb(136, 19, 55);
        btnTabJangela.Location = new Point(20, 4);
        btnTabJangela.Name = "btnTabJangela";
        btnTabJangela.Size = new Size(210, 44);
        btnTabJangela.TabIndex = 0;
        btnTabJangela.Text = "Jangela Nagusia";
        btnTabJangela.UseVisualStyleBackColor = false;

        // 
        // btnTabTerraza
        // 
        btnTabTerraza.BackColor = Color.White;
        btnTabTerraza.Cursor = Cursors.Hand;
        btnTabTerraza.FlatAppearance.BorderSize = 0;
        btnTabTerraza.FlatStyle = FlatStyle.Flat;
        btnTabTerraza.Font = new Font("Segoe UI", 10F);
        btnTabTerraza.ForeColor = Color.FromArgb(100, 116, 139);
        btnTabTerraza.Location = new Point(240, 4);
        btnTabTerraza.Name = "btnTabTerraza";
        btnTabTerraza.Size = new Size(120, 44);
        btnTabTerraza.TabIndex = 1;
        btnTabTerraza.Text = "Terraza";
        btnTabTerraza.UseVisualStyleBackColor = false;

        // 
        // btnTabBarra
        // 
        btnTabBarra.BackColor = Color.White;
        btnTabBarra.Cursor = Cursors.Hand;
        btnTabBarra.FlatAppearance.BorderSize = 0;
        btnTabBarra.FlatStyle = FlatStyle.Flat;
        btnTabBarra.Font = new Font("Segoe UI", 10F);
        btnTabBarra.ForeColor = Color.FromArgb(100, 116, 139);
        btnTabBarra.Location = new Point(370, 4);
        btnTabBarra.Name = "btnTabBarra";
        btnTabBarra.Size = new Size(110, 44);
        btnTabBarra.TabIndex = 2;
        btnTabBarra.Text = "Barra";
        btnTabBarra.UseVisualStyleBackColor = false;

        // 
        // btnTabVIP
        // 
        btnTabVIP.BackColor = Color.White;
        btnTabVIP.Cursor = Cursors.Hand;
        btnTabVIP.FlatAppearance.BorderSize = 0;
        btnTabVIP.FlatStyle = FlatStyle.Flat;
        btnTabVIP.Font = new Font("Segoe UI", 10F);
        btnTabVIP.ForeColor = Color.FromArgb(100, 116, 139);
        btnTabVIP.Location = new Point(490, 4);
        btnTabVIP.Name = "btnTabVIP";
        btnTabVIP.Size = new Size(100, 44);
        btnTabVIP.TabIndex = 3;
        btnTabVIP.Text = "VIP";
        btnTabVIP.UseVisualStyleBackColor = false;

        // 
        // btnTabGuztiak
        // 
        btnTabGuztiak.BackColor = Color.White;
        btnTabGuztiak.Cursor = Cursors.Hand;
        btnTabGuztiak.FlatAppearance.BorderSize = 0;
        btnTabGuztiak.FlatStyle = FlatStyle.Flat;
        btnTabGuztiak.Font = new Font("Segoe UI", 10F);
        btnTabGuztiak.ForeColor = Color.FromArgb(100, 116, 139);
        btnTabGuztiak.Location = new Point(600, 4);
        btnTabGuztiak.Name = "btnTabGuztiak";
        btnTabGuztiak.Size = new Size(110, 44);
        btnTabGuztiak.TabIndex = 4;
        btnTabGuztiak.Text = "Guztiak";
        btnTabGuztiak.UseVisualStyleBackColor = false;

        // 
        // pnlContent (Erdiko edukia)
        // 
        pnlContent.BackColor = Color.FromArgb(241, 245, 249);
        pnlContent.Controls.Add(pnlMahaiak);
        pnlContent.Controls.Add(pnlSectionHeader);
        pnlContent.Dock = DockStyle.Fill;
        pnlContent.Location = new Point(0, 117);
        pnlContent.Name = "pnlContent";
        pnlContent.Padding = new Padding(24, 16, 24, 20);
        pnlContent.Size = new Size(1184, 603);
        pnlContent.TabIndex = 2;

        // 
        // pnlSectionHeader (Sekzioaren goiburua eta laburpena)
        // 
        pnlSectionHeader.Controls.Add(lblSummary);
        pnlSectionHeader.Controls.Add(lblSectionTitle);
        pnlSectionHeader.Dock = DockStyle.Top;
        pnlSectionHeader.Location = new Point(24, 16);
        pnlSectionHeader.Name = "pnlSectionHeader";
        pnlSectionHeader.Size = new Size(1136, 48);
        pnlSectionHeader.TabIndex = 0;

        // 
        // lblSectionTitle ("JANGELA NAGUSIA")
        // 
        lblSectionTitle.Dock = DockStyle.Left;
        lblSectionTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        lblSectionTitle.ForeColor = Color.FromArgb(30, 41, 59);
        lblSectionTitle.Location = new Point(0, 0);
        lblSectionTitle.Name = "lblSectionTitle";
        lblSectionTitle.Size = new Size(350, 48);
        lblSectionTitle.TabIndex = 0;
        lblSectionTitle.Text = "JANGELA NAGUSIA";
        lblSectionTitle.TextAlign = ContentAlignment.MiddleLeft;

        // 
        // lblSummary (Mahaien egoera laburpena)
        // 
        lblSummary.Dock = DockStyle.Right;
        lblSummary.Font = new Font("Segoe UI", 9.5F);
        lblSummary.ForeColor = Color.FromArgb(100, 116, 139);
        lblSummary.Location = new Point(536, 0);
        lblSummary.Name = "lblSummary";
        lblSummary.Size = new Size(600, 48);
        lblSummary.TabIndex = 1;
        lblSummary.Text = "Guztira: 6  |  Libre: 5  |  Okupatuta: 0  |  Garbitzen: 1";
        lblSummary.TextAlign = ContentAlignment.MiddleRight;

        // 
        // pnlMahaiak (Mahai txartelen sareta dinamikoa)
        // 
        pnlMahaiak.AutoScroll = true;
        pnlMahaiak.Dock = DockStyle.Fill;
        pnlMahaiak.Location = new Point(24, 64);
        pnlMahaiak.Name = "pnlMahaiak";
        pnlMahaiak.Padding = new Padding(0, 10, 0, 10);
        pnlMahaiak.Size = new Size(1136, 519);
        pnlMahaiak.TabIndex = 1;

        // 
        // MahaiakForm
        // 
        AutoScaleMode = AutoScaleMode.None;
        BackColor = Color.FromArgb(241, 245, 249);
        ClientSize = new Size(1184, 720);
        Controls.Add(pnlContent);
        Controls.Add(pnlTabStrip);
        Controls.Add(pnlTopBar);
        MinimumSize = new Size(950, 650);
        Name = "MahaiakForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "T1 Jatetxea - Mahaiak";
        pnlTopBar.ResumeLayout(false);
        pnlProfile.ResumeLayout(false);
        pnlTabStrip.ResumeLayout(false);
        pnlContent.ResumeLayout(false);
        pnlSectionHeader.ResumeLayout(false);
        ResumeLayout(false);
    }

    private Panel pnlTopBar;
    private Label lblLogo;
    private Label lblBrand;
    private Panel pnlProfile;
    private Label lblAvatarCircle;
    private Label lblServerName;
    private Label lblServerStatus;
    private Button btnFreskatu;
    private Button btnLogout;
    private Panel pnlTabStrip;
    private Button btnTabJangela;
    private Button btnTabTerraza;
    private Button btnTabBarra;
    private Button btnTabVIP;
    private Button btnTabGuztiak;
    private Panel pnlContent;
    private Panel pnlSectionHeader;
    private Label lblSectionTitle;
    private Label lblSummary;
    private FlowLayoutPanel pnlMahaiak;
}
