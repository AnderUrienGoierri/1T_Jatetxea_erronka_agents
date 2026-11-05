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
        pnlTabStrip = new FlowLayoutPanel();
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
        // pnlTopBar (Goiko barra zuria - POS tablet estiloko goiburu zabala)
        // 
        pnlTopBar.BackColor = Color.White;
        pnlTopBar.Controls.Add(pnlProfile);
        pnlTopBar.Controls.Add(lblBrand);
        pnlTopBar.Controls.Add(lblLogo);
        pnlTopBar.Dock = DockStyle.Top;
        pnlTopBar.Location = new Point(0, 0);
        pnlTopBar.Name = "pnlTopBar";
        pnlTopBar.Size = new Size(1280, 84);
        pnlTopBar.TabIndex = 0;

        // 
        // lblLogo ("T1" txapa)
        // 
        lblLogo.BackColor = Color.FromArgb(136, 19, 55);
        lblLogo.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        lblLogo.ForeColor = Color.White;
        lblLogo.Location = new Point(20, 16);
        lblLogo.Name = "lblLogo";
        lblLogo.Size = new Size(52, 52);
        lblLogo.TabIndex = 0;
        lblLogo.Text = "T1";
        lblLogo.TextAlign = ContentAlignment.MiddleCenter;

        // 
        // lblBrand ("T1 Jatetxea - Mahaiak")
        // 
        lblBrand.Font = new Font("Segoe UI", 17F, FontStyle.Bold);
        lblBrand.ForeColor = Color.FromArgb(136, 19, 55);
        lblBrand.Location = new Point(84, 16);
        lblBrand.Name = "lblBrand";
        lblBrand.Size = new Size(400, 52);
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
        pnlProfile.Location = new Point(750, 0);
        pnlProfile.Name = "pnlProfile";
        pnlProfile.Size = new Size(530, 84);
        pnlProfile.TabIndex = 2;

        // 
        // btnLogout ("Saioa Itxi")
        // 
        btnLogout.BackColor = Color.FromArgb(136, 19, 55);
        btnLogout.Cursor = Cursors.Hand;
        btnLogout.FlatAppearance.BorderSize = 0;
        btnLogout.FlatStyle = FlatStyle.Flat;
        btnLogout.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
        btnLogout.ForeColor = Color.White;
        btnLogout.Location = new Point(390, 18);
        btnLogout.Name = "btnLogout";
        btnLogout.Size = new Size(125, 48);
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
        btnFreskatu.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
        btnFreskatu.ForeColor = Color.FromArgb(51, 65, 85);
        btnFreskatu.Location = new Point(258, 18);
        btnFreskatu.Name = "btnFreskatu";
        btnFreskatu.Size = new Size(120, 48);
        btnFreskatu.TabIndex = 3;
        btnFreskatu.Text = "Freskatu";
        btnFreskatu.UseVisualStyleBackColor = false;

        // 
        // lblServerStatus ("● Online")
        // 
        lblServerStatus.Font = new Font("Segoe UI", 9.5F);
        lblServerStatus.ForeColor = Color.FromArgb(22, 163, 74);
        lblServerStatus.Location = new Point(72, 46);
        lblServerStatus.Name = "lblServerStatus";
        lblServerStatus.Size = new Size(170, 26);
        lblServerStatus.TabIndex = 2;
        lblServerStatus.Text = "● Online";
        lblServerStatus.TextAlign = ContentAlignment.MiddleLeft;

        // 
        // lblServerName (Zerbitzariaren izena)
        // 
        lblServerName.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        lblServerName.ForeColor = Color.FromArgb(30, 41, 59);
        lblServerName.Location = new Point(72, 16);
        lblServerName.Name = "lblServerName";
        lblServerName.Size = new Size(180, 30);
        lblServerName.TabIndex = 1;
        lblServerName.Text = "Ane G.";
        lblServerName.TextAlign = ContentAlignment.MiddleLeft;

        // 
        // lblAvatarCircle (Profil borobila)
        // 
        lblAvatarCircle.BackColor = Color.FromArgb(239, 246, 255);
        lblAvatarCircle.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
        lblAvatarCircle.ForeColor = Color.FromArgb(37, 99, 235);
        lblAvatarCircle.Location = new Point(10, 16);
        lblAvatarCircle.Name = "lblAvatarCircle";
        lblAvatarCircle.Size = new Size(52, 52);
        lblAvatarCircle.TabIndex = 0;
        lblAvatarCircle.Text = "AG";
        lblAvatarCircle.TextAlign = ContentAlignment.MiddleCenter;

        // 
        // pnlTabStrip (Guneen fitxak: Jangela Nagusia, Terraza, Barra, VIP, Guztiak)
        // 
        pnlTabStrip.AutoScroll = true;
        pnlTabStrip.BackColor = Color.White;
        pnlTabStrip.Controls.Add(btnTabJangela);
        pnlTabStrip.Controls.Add(btnTabTerraza);
        pnlTabStrip.Controls.Add(btnTabBarra);
        pnlTabStrip.Controls.Add(btnTabVIP);
        pnlTabStrip.Controls.Add(btnTabGuztiak);
        pnlTabStrip.Dock = DockStyle.Top;
        pnlTabStrip.FlowDirection = FlowDirection.LeftToRight;
        pnlTabStrip.Location = new Point(0, 84);
        pnlTabStrip.Name = "pnlTabStrip";
        pnlTabStrip.Padding = new Padding(24, 6, 24, 0);
        pnlTabStrip.Size = new Size(1280, 58);
        pnlTabStrip.TabIndex = 1;
        pnlTabStrip.WrapContents = false;

        // 
        // btnTabJangela
        // 
        btnTabJangela.AutoSize = true;
        btnTabJangela.BackColor = Color.White;
        btnTabJangela.Cursor = Cursors.Hand;
        btnTabJangela.FlatAppearance.BorderSize = 0;
        btnTabJangela.FlatStyle = FlatStyle.Flat;
        btnTabJangela.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        btnTabJangela.ForeColor = Color.FromArgb(136, 19, 55);
        btnTabJangela.Location = new Point(24, 6);
        btnTabJangela.Margin = new Padding(0, 0, 14, 0);
        btnTabJangela.Name = "btnTabJangela";
        btnTabJangela.Padding = new Padding(20, 0, 20, 0);
        btnTabJangela.Size = new Size(170, 48);
        btnTabJangela.TabIndex = 0;
        btnTabJangela.Text = "Jangela Nagusia";
        btnTabJangela.UseVisualStyleBackColor = false;

        // 
        // btnTabTerraza
        // 
        btnTabTerraza.AutoSize = true;
        btnTabTerraza.BackColor = Color.White;
        btnTabTerraza.Cursor = Cursors.Hand;
        btnTabTerraza.FlatAppearance.BorderSize = 0;
        btnTabTerraza.FlatStyle = FlatStyle.Flat;
        btnTabTerraza.Font = new Font("Segoe UI", 11F);
        btnTabTerraza.ForeColor = Color.FromArgb(100, 116, 139);
        btnTabTerraza.Location = new Point(208, 6);
        btnTabTerraza.Margin = new Padding(0, 0, 14, 0);
        btnTabTerraza.Name = "btnTabTerraza";
        btnTabTerraza.Padding = new Padding(20, 0, 20, 0);
        btnTabTerraza.Size = new Size(130, 48);
        btnTabTerraza.TabIndex = 1;
        btnTabTerraza.Text = "Terraza";
        btnTabTerraza.UseVisualStyleBackColor = false;

        // 
        // btnTabBarra
        // 
        btnTabBarra.AutoSize = true;
        btnTabBarra.BackColor = Color.White;
        btnTabBarra.Cursor = Cursors.Hand;
        btnTabBarra.FlatAppearance.BorderSize = 0;
        btnTabBarra.FlatStyle = FlatStyle.Flat;
        btnTabBarra.Font = new Font("Segoe UI", 11F);
        btnTabBarra.ForeColor = Color.FromArgb(100, 116, 139);
        btnTabBarra.Location = new Point(352, 6);
        btnTabBarra.Margin = new Padding(0, 0, 14, 0);
        btnTabBarra.Name = "btnTabBarra";
        btnTabBarra.Padding = new Padding(20, 0, 20, 0);
        btnTabBarra.Size = new Size(120, 48);
        btnTabBarra.TabIndex = 2;
        btnTabBarra.Text = "Barra";
        btnTabBarra.UseVisualStyleBackColor = false;

        // 
        // btnTabVIP
        // 
        btnTabVIP.AutoSize = true;
        btnTabVIP.BackColor = Color.White;
        btnTabVIP.Cursor = Cursors.Hand;
        btnTabVIP.FlatAppearance.BorderSize = 0;
        btnTabVIP.FlatStyle = FlatStyle.Flat;
        btnTabVIP.Font = new Font("Segoe UI", 11F);
        btnTabVIP.ForeColor = Color.FromArgb(100, 116, 139);
        btnTabVIP.Location = new Point(486, 6);
        btnTabVIP.Margin = new Padding(0, 0, 14, 0);
        btnTabVIP.Name = "btnTabVIP";
        btnTabVIP.Padding = new Padding(20, 0, 20, 0);
        btnTabVIP.Size = new Size(110, 48);
        btnTabVIP.TabIndex = 3;
        btnTabVIP.Text = "VIP";
        btnTabVIP.UseVisualStyleBackColor = false;

        // 
        // btnTabGuztiak
        // 
        btnTabGuztiak.AutoSize = true;
        btnTabGuztiak.BackColor = Color.White;
        btnTabGuztiak.Cursor = Cursors.Hand;
        btnTabGuztiak.FlatAppearance.BorderSize = 0;
        btnTabGuztiak.FlatStyle = FlatStyle.Flat;
        btnTabGuztiak.Font = new Font("Segoe UI", 11F);
        btnTabGuztiak.ForeColor = Color.FromArgb(100, 116, 139);
        btnTabGuztiak.Location = new Point(610, 6);
        btnTabGuztiak.Margin = new Padding(0, 0, 14, 0);
        btnTabGuztiak.Name = "btnTabGuztiak";
        btnTabGuztiak.Padding = new Padding(20, 0, 20, 0);
        btnTabGuztiak.Size = new Size(125, 48);
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
        pnlContent.Location = new Point(0, 142);
        pnlContent.Name = "pnlContent";
        pnlContent.Padding = new Padding(24, 16, 24, 20);
        pnlContent.Size = new Size(1280, 658);
        pnlContent.TabIndex = 2;

        // 
        // pnlSectionHeader (Sekzioaren goiburua eta laburpena)
        // 
        pnlSectionHeader.Controls.Add(lblSummary);
        pnlSectionHeader.Controls.Add(lblSectionTitle);
        pnlSectionHeader.Dock = DockStyle.Top;
        pnlSectionHeader.Location = new Point(24, 16);
        pnlSectionHeader.Name = "pnlSectionHeader";
        pnlSectionHeader.Size = new Size(1232, 52);
        pnlSectionHeader.TabIndex = 0;

        // 
        // lblSectionTitle ("JANGELA NAGUSIA")
        // 
        lblSectionTitle.AutoSize = true;
        lblSectionTitle.Dock = DockStyle.Left;
        lblSectionTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        lblSectionTitle.ForeColor = Color.FromArgb(30, 41, 59);
        lblSectionTitle.Location = new Point(0, 0);
        lblSectionTitle.Name = "lblSectionTitle";
        lblSectionTitle.Size = new Size(260, 52);
        lblSectionTitle.TabIndex = 0;
        lblSectionTitle.Text = "JANGELA NAGUSIA";
        lblSectionTitle.TextAlign = ContentAlignment.MiddleLeft;

        // 
        // lblSummary (Mahaien egoera laburpena)
        // 
        lblSummary.Dock = DockStyle.Fill;
        lblSummary.Font = new Font("Segoe UI", 11F);
        lblSummary.ForeColor = Color.FromArgb(100, 116, 139);
        lblSummary.Location = new Point(260, 0);
        lblSummary.Name = "lblSummary";
        lblSummary.Size = new Size(972, 52);
        lblSummary.TabIndex = 1;
        lblSummary.Text = "Guztira: 6  |  Libre: 5  |  Okupatuta: 0  |  Bidean: 0  |  Garbitzen: 1";
        lblSummary.TextAlign = ContentAlignment.MiddleRight;

        // 
        // pnlMahaiak (Mahai txartelen sareta dinamikoa)
        // 
        pnlMahaiak.AutoScroll = true;
        pnlMahaiak.Dock = DockStyle.Fill;
        pnlMahaiak.Location = new Point(24, 68);
        pnlMahaiak.Name = "pnlMahaiak";
        pnlMahaiak.Padding = new Padding(0, 10, 0, 10);
        pnlMahaiak.Size = new Size(1232, 570);
        pnlMahaiak.TabIndex = 1;

        // 
        // MahaiakForm
        // 
        AutoScaleMode = AutoScaleMode.None;
        BackColor = Color.FromArgb(241, 245, 249);
        ClientSize = new Size(1280, 800);
        Controls.Add(pnlContent);
        Controls.Add(pnlTabStrip);
        Controls.Add(pnlTopBar);
        MinimumSize = new Size(1024, 700);
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
    private FlowLayoutPanel pnlTabStrip;
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
