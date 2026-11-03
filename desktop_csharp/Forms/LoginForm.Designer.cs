namespace Jatetxea.Desktop.Forms;

partial class LoginForm
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
        lblZerbitzariak = new Label();
        pnlCard = new Panel();
        pnlAvatars = new Panel();
        pnlAvatar1 = new Panel();
        lblCircle1 = new Label();
        lblName1 = new Label();
        pnlAvatar2 = new Panel();
        lblCircle2 = new Label();
        lblName2 = new Label();
        pnlAvatar3 = new Panel();
        lblCircle3 = new Label();
        lblName3 = new Label();
        pnlAvatar4 = new Panel();
        lblCircle4 = new Label();
        lblName4 = new Label();
        pnlAvatar5 = new Panel();
        lblCircle5 = new Label();
        lblName5 = new Label();
        pnlDivider = new Panel();
        lblPinTitle = new Label();
        pnlPinDots = new Panel();
        lblDot1 = new Label();
        lblDot2 = new Label();
        lblDot3 = new Label();
        lblDot4 = new Label();
        pnlKeypad = new TableLayoutPanel();
        btnKey1 = new Button();
        btnKey2 = new Button();
        btnKey3 = new Button();
        btnKey4 = new Button();
        btnKey5 = new Button();
        btnKey6 = new Button();
        btnKey7 = new Button();
        btnKey8 = new Button();
        btnKey9 = new Button();
        btnKeyClear = new Button();
        btnKey0 = new Button();
        btnKeyBack = new Button();
        lblStatus = new Label();
        btnLogin = new Button();
        pnlTopBar.SuspendLayout();
        pnlCard.SuspendLayout();
        pnlAvatars.SuspendLayout();
        pnlAvatar1.SuspendLayout();
        pnlAvatar2.SuspendLayout();
        pnlAvatar3.SuspendLayout();
        pnlAvatar4.SuspendLayout();
        pnlAvatar5.SuspendLayout();
        pnlPinDots.SuspendLayout();
        pnlKeypad.SuspendLayout();
        SuspendLayout();

        // 
        // pnlTopBar (Goiburua: Logoa, Jatetxe izena eta Zerbitzariak)
        // 
        pnlTopBar.BackColor = Color.White;
        pnlTopBar.Controls.Add(lblLogo);
        pnlTopBar.Controls.Add(lblBrand);
        pnlTopBar.Controls.Add(lblZerbitzariak);
        pnlTopBar.Dock = DockStyle.Top;
        pnlTopBar.Height = 65;
        pnlTopBar.Location = new Point(0, 0);
        pnlTopBar.Name = "pnlTopBar";
        pnlTopBar.Size = new Size(1024, 65);
        pnlTopBar.TabIndex = 0;

        // 
        // lblLogo (T1 logo txapa bordelea)
        // 
        lblLogo.BackColor = Color.FromArgb(136, 19, 55); // #881337
        lblLogo.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        lblLogo.ForeColor = Color.White;
        lblLogo.Location = new Point(25, 12);
        lblLogo.Name = "lblLogo";
        lblLogo.Size = new Size(42, 42);
        lblLogo.Text = "T1";
        lblLogo.TextAlign = ContentAlignment.MiddleCenter;

        // 
        // lblBrand ("T1 Jatetxea")
        // 
        lblBrand.AutoSize = true;
        lblBrand.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
        lblBrand.ForeColor = Color.FromArgb(136, 19, 55);
        lblBrand.Location = new Point(76, 18);
        lblBrand.Name = "lblBrand";
        lblBrand.Size = new Size(122, 28);
        lblBrand.Text = "T1 Jatetxea";

        // 
        // lblZerbitzariak ("Zerbitzariak")
        // 
        lblZerbitzariak.AutoSize = true;
        lblZerbitzariak.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
        lblZerbitzariak.ForeColor = Color.FromArgb(51, 65, 85);
        lblZerbitzariak.Location = new Point(450, 18);
        lblZerbitzariak.Name = "lblZerbitzariak";
        lblZerbitzariak.Size = new Size(125, 28);
        lblZerbitzariak.Text = "Zerbitzariak";
        lblZerbitzariak.TextAlign = ContentAlignment.MiddleCenter;

        // 
        // pnlCard (Txartel nagusia, zentratuta automatikoki)
        // 
        pnlCard.Anchor = AnchorStyles.None;
        pnlCard.BackColor = Color.White;
        pnlCard.Controls.Add(pnlAvatars);
        pnlCard.Controls.Add(pnlDivider);
        pnlCard.Controls.Add(lblPinTitle);
        pnlCard.Controls.Add(pnlPinDots);
        pnlCard.Controls.Add(pnlKeypad);
        pnlCard.Controls.Add(lblStatus);
        pnlCard.Controls.Add(btnLogin);
        pnlCard.Location = new Point(292, 100);
        pnlCard.Name = "pnlCard";
        pnlCard.Size = new Size(440, 560);
        pnlCard.TabIndex = 1;

        // 
        // pnlAvatars (5 Zerbitzarien zerrenda horizontala)
        // 
        pnlAvatars.Controls.Add(pnlAvatar1);
        pnlAvatars.Controls.Add(pnlAvatar2);
        pnlAvatars.Controls.Add(pnlAvatar3);
        pnlAvatars.Controls.Add(pnlAvatar4);
        pnlAvatars.Controls.Add(pnlAvatar5);
        pnlAvatars.Location = new Point(10, 15);
        pnlAvatars.Name = "pnlAvatars";
        pnlAvatars.Size = new Size(420, 75);
        pnlAvatars.TabIndex = 0;

        // Avatar 1: Aitor B.
        pnlAvatar1.Controls.Add(lblCircle1);
        pnlAvatar1.Controls.Add(lblName1);
        pnlAvatar1.Location = new Point(4, 2);
        pnlAvatar1.Name = "pnlAvatar1";
        pnlAvatar1.Size = new Size(78, 70);
        pnlAvatar1.Cursor = Cursors.Hand;
        lblCircle1.BackColor = Color.FromArgb(239, 246, 255);
        lblCircle1.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblCircle1.ForeColor = Color.FromArgb(37, 99, 235);
        lblCircle1.Location = new Point(17, 2);
        lblCircle1.Name = "lblCircle1";
        lblCircle1.Size = new Size(44, 44);
        lblCircle1.Text = "AB";
        lblCircle1.TextAlign = ContentAlignment.MiddleCenter;
        lblName1.Font = new Font("Segoe UI", 8.5F);
        lblName1.ForeColor = Color.FromArgb(51, 65, 85);
        lblName1.Location = new Point(0, 48);
        lblName1.Name = "lblName1";
        lblName1.Size = new Size(78, 18);
        lblName1.Text = "Aitor B.";
        lblName1.TextAlign = ContentAlignment.TopCenter;

        // Avatar 2: Maite L.
        pnlAvatar2.Controls.Add(lblCircle2);
        pnlAvatar2.Controls.Add(lblName2);
        pnlAvatar2.Location = new Point(87, 2);
        pnlAvatar2.Name = "pnlAvatar2";
        pnlAvatar2.Size = new Size(78, 70);
        pnlAvatar2.Cursor = Cursors.Hand;
        lblCircle2.BackColor = Color.FromArgb(241, 245, 249);
        lblCircle2.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblCircle2.ForeColor = Color.FromArgb(71, 85, 105);
        lblCircle2.Location = new Point(17, 2);
        lblCircle2.Name = "lblCircle2";
        lblCircle2.Size = new Size(44, 44);
        lblCircle2.Text = "ML";
        lblCircle2.TextAlign = ContentAlignment.MiddleCenter;
        lblName2.Font = new Font("Segoe UI", 8.5F);
        lblName2.ForeColor = Color.FromArgb(51, 65, 85);
        lblName2.Location = new Point(0, 48);
        lblName2.Name = "lblName2";
        lblName2.Size = new Size(78, 18);
        lblName2.Text = "Maite L.";
        lblName2.TextAlign = ContentAlignment.TopCenter;

        // Avatar 3: Mikel A.
        pnlAvatar3.Controls.Add(lblCircle3);
        pnlAvatar3.Controls.Add(lblName3);
        pnlAvatar3.Location = new Point(170, 2);
        pnlAvatar3.Name = "pnlAvatar3";
        pnlAvatar3.Size = new Size(78, 70);
        pnlAvatar3.Cursor = Cursors.Hand;
        lblCircle3.BackColor = Color.FromArgb(241, 245, 249);
        lblCircle3.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblCircle3.ForeColor = Color.FromArgb(71, 85, 105);
        lblCircle3.Location = new Point(17, 2);
        lblCircle3.Name = "lblCircle3";
        lblCircle3.Size = new Size(44, 44);
        lblCircle3.Text = "MA";
        lblCircle3.TextAlign = ContentAlignment.MiddleCenter;
        lblName3.Font = new Font("Segoe UI", 8.5F);
        lblName3.ForeColor = Color.FromArgb(51, 65, 85);
        lblName3.Location = new Point(0, 48);
        lblName3.Name = "lblName3";
        lblName3.Size = new Size(78, 18);
        lblName3.Text = "Mikel A.";
        lblName3.TextAlign = ContentAlignment.TopCenter;

        // Avatar 4: Ane G.
        pnlAvatar4.Controls.Add(lblCircle4);
        pnlAvatar4.Controls.Add(lblName4);
        pnlAvatar4.Location = new Point(253, 2);
        pnlAvatar4.Name = "pnlAvatar4";
        pnlAvatar4.Size = new Size(78, 70);
        pnlAvatar4.Cursor = Cursors.Hand;
        lblCircle4.BackColor = Color.FromArgb(241, 245, 249);
        lblCircle4.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblCircle4.ForeColor = Color.FromArgb(71, 85, 105);
        lblCircle4.Location = new Point(17, 2);
        lblCircle4.Name = "lblCircle4";
        lblCircle4.Size = new Size(44, 44);
        lblCircle4.Text = "AG";
        lblCircle4.TextAlign = ContentAlignment.MiddleCenter;
        lblName4.Font = new Font("Segoe UI", 8.5F);
        lblName4.ForeColor = Color.FromArgb(51, 65, 85);
        lblName4.Location = new Point(0, 48);
        lblName4.Name = "lblName4";
        lblName4.Size = new Size(78, 18);
        lblName4.Text = "Ane G.";
        lblName4.TextAlign = ContentAlignment.TopCenter;

        // Avatar 5: Iker M.
        pnlAvatar5.Controls.Add(lblCircle5);
        pnlAvatar5.Controls.Add(lblName5);
        pnlAvatar5.Location = new Point(336, 2);
        pnlAvatar5.Name = "pnlAvatar5";
        pnlAvatar5.Size = new Size(78, 70);
        pnlAvatar5.Cursor = Cursors.Hand;
        lblCircle5.BackColor = Color.FromArgb(241, 245, 249);
        lblCircle5.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblCircle5.ForeColor = Color.FromArgb(71, 85, 105);
        lblCircle5.Location = new Point(17, 2);
        lblCircle5.Name = "lblCircle5";
        lblCircle5.Size = new Size(44, 44);
        lblCircle5.Text = "IM";
        lblCircle5.TextAlign = ContentAlignment.MiddleCenter;
        lblName5.Font = new Font("Segoe UI", 8.5F);
        lblName5.ForeColor = Color.FromArgb(51, 65, 85);
        lblName5.Location = new Point(0, 48);
        lblName5.Name = "lblName5";
        lblName5.Size = new Size(78, 18);
        lblName5.Text = "Iker M.";
        lblName5.TextAlign = ContentAlignment.TopCenter;

        // 
        // pnlDivider (Marra banatzailea)
        // 
        pnlDivider.BackColor = Color.FromArgb(226, 232, 240);
        pnlDivider.Location = new Point(20, 95);
        pnlDivider.Name = "pnlDivider";
        pnlDivider.Size = new Size(400, 1);
        pnlDivider.TabIndex = 1;

        // 
        // lblPinTitle ("SARTU PIN ZENBAKIA")
        // 
        lblPinTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblPinTitle.ForeColor = Color.FromArgb(30, 41, 59);
        lblPinTitle.Location = new Point(20, 106);
        lblPinTitle.Name = "lblPinTitle";
        lblPinTitle.Size = new Size(400, 24);
        lblPinTitle.Text = "SARTU PIN ZENBAKIA";
        lblPinTitle.TextAlign = ContentAlignment.MiddleCenter;
        lblPinTitle.TabIndex = 2;

        // 
        // pnlPinDots (4 PIN borobilak)
        // 
        pnlPinDots.Controls.Add(lblDot1);
        pnlPinDots.Controls.Add(lblDot2);
        pnlPinDots.Controls.Add(lblDot3);
        pnlPinDots.Controls.Add(lblDot4);
        pnlPinDots.Location = new Point(130, 134);
        pnlPinDots.Name = "pnlPinDots";
        pnlPinDots.Size = new Size(180, 26);
        pnlPinDots.TabIndex = 3;

        lblDot1.Location = new Point(25, 4);
        lblDot1.Name = "lblDot1";
        lblDot1.Size = new Size(18, 18);
        lblDot2.Location = new Point(65, 4);
        lblDot2.Name = "lblDot2";
        lblDot2.Size = new Size(18, 18);
        lblDot3.Location = new Point(105, 4);
        lblDot3.Name = "lblDot3";
        lblDot3.Size = new Size(18, 18);
        lblDot4.Location = new Point(145, 4);
        lblDot4.Name = "lblDot4";
        lblDot4.Size = new Size(18, 18);

        // 
        // pnlKeypad (3x4 Matrizea)
        // 
        pnlKeypad.ColumnCount = 3;
        pnlKeypad.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
        pnlKeypad.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
        pnlKeypad.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
        pnlKeypad.RowCount = 4;
        pnlKeypad.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
        pnlKeypad.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
        pnlKeypad.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
        pnlKeypad.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
        pnlKeypad.Controls.Add(btnKey1, 0, 0);
        pnlKeypad.Controls.Add(btnKey2, 1, 0);
        pnlKeypad.Controls.Add(btnKey3, 2, 0);
        pnlKeypad.Controls.Add(btnKey4, 0, 1);
        pnlKeypad.Controls.Add(btnKey5, 1, 1);
        pnlKeypad.Controls.Add(btnKey6, 2, 1);
        pnlKeypad.Controls.Add(btnKey7, 0, 2);
        pnlKeypad.Controls.Add(btnKey8, 1, 2);
        pnlKeypad.Controls.Add(btnKey9, 2, 2);
        pnlKeypad.Controls.Add(btnKeyClear, 0, 3);
        pnlKeypad.Controls.Add(btnKey0, 1, 3);
        pnlKeypad.Controls.Add(btnKeyBack, 2, 3);
        pnlKeypad.Location = new Point(45, 166);
        pnlKeypad.Name = "pnlKeypad";
        pnlKeypad.Size = new Size(350, 255);
        pnlKeypad.TabIndex = 4;

        // Botoien estiloak
        ConfigureKeypadButton(btnKey1, "1");
        ConfigureKeypadButton(btnKey2, "2");
        ConfigureKeypadButton(btnKey3, "3");
        ConfigureKeypadButton(btnKey4, "4");
        ConfigureKeypadButton(btnKey5, "5");
        ConfigureKeypadButton(btnKey6, "6");
        ConfigureKeypadButton(btnKey7, "7");
        ConfigureKeypadButton(btnKey8, "8");
        ConfigureKeypadButton(btnKey9, "9");
        ConfigureKeypadButton(btnKeyClear, "X Clear");
        ConfigureKeypadButton(btnKey0, "0");
        ConfigureKeypadButton(btnKeyBack, "<-");

        // 
        // lblStatus
        // 
        lblStatus.Font = new Font("Segoe UI", 9F);
        lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
        lblStatus.Location = new Point(20, 430);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(400, 22);
        lblStatus.TextAlign = ContentAlignment.MiddleCenter;
        lblStatus.TabIndex = 5;

        // 
        // btnLogin ("HASI SAIOA")
        // 
        btnLogin.BackColor = Color.FromArgb(136, 19, 55);
        btnLogin.FlatAppearance.BorderSize = 0;
        btnLogin.FlatStyle = FlatStyle.Flat;
        btnLogin.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        btnLogin.ForeColor = Color.White;
        btnLogin.Location = new Point(45, 462);
        btnLogin.Name = "btnLogin";
        btnLogin.Size = new Size(350, 52);
        btnLogin.Text = "HASI SAIOA";
        btnLogin.UseVisualStyleBackColor = false;
        btnLogin.Cursor = Cursors.Hand;
        btnLogin.TabIndex = 6;

        // 
        // LoginForm
        // 
        AutoScaleMode = AutoScaleMode.None;
        BackColor = Color.FromArgb(241, 245, 249);
        ClientSize = new Size(1024, 720);
        Controls.Add(pnlCard);
        Controls.Add(pnlTopBar);
        MinimumSize = new Size(700, 680);
        Name = "LoginForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "T1 Jatetxea - Saioa Hasi (TPV Tablet)";
        pnlTopBar.ResumeLayout(false);
        pnlTopBar.PerformLayout();
        pnlCard.ResumeLayout(false);
        pnlAvatars.ResumeLayout(false);
        pnlAvatar1.ResumeLayout(false);
        pnlAvatar2.ResumeLayout(false);
        pnlAvatar3.ResumeLayout(false);
        pnlAvatar4.ResumeLayout(false);
        pnlAvatar5.ResumeLayout(false);
        pnlPinDots.ResumeLayout(false);
        pnlKeypad.ResumeLayout(false);
        ResumeLayout(false);
    }

    private void ConfigureKeypadButton(Button btn, string txt)
    {
        btn.Text = txt;
        btn.Dock = DockStyle.Fill;
        btn.Font = txt.Length > 2 ? new Font("Segoe UI", 10.5F, FontStyle.Bold) : new Font("Segoe UI", 16F, FontStyle.Bold);
        btn.Margin = new Padding(4);
        btn.BackColor = txt switch
        {
            "X Clear" => Color.FromArgb(254, 242, 242),
            "<-" => Color.FromArgb(248, 250, 252),
            _ => Color.White
        };
        btn.ForeColor = txt switch
        {
            "X Clear" => Color.FromArgb(220, 38, 38),
            _ => Color.FromArgb(30, 41, 59)
        };
        btn.FlatStyle = FlatStyle.Flat;
        btn.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btn.FlatAppearance.BorderSize = 1;
        btn.Cursor = Cursors.Hand;
    }

    private Panel pnlTopBar;
    private Label lblLogo;
    private Label lblBrand;
    private Label lblZerbitzariak;
    private Panel pnlCard;
    private Panel pnlAvatars;
    private Panel pnlAvatar1;
    private Label lblCircle1;
    private Label lblName1;
    private Panel pnlAvatar2;
    private Label lblCircle2;
    private Label lblName2;
    private Panel pnlAvatar3;
    private Label lblCircle3;
    private Label lblName3;
    private Panel pnlAvatar4;
    private Label lblCircle4;
    private Label lblName4;
    private Panel pnlAvatar5;
    private Label lblCircle5;
    private Label lblName5;
    private Panel pnlDivider;
    private Label lblPinTitle;
    private Panel pnlPinDots;
    private Label lblDot1;
    private Label lblDot2;
    private Label lblDot3;
    private Label lblDot4;
    private TableLayoutPanel pnlKeypad;
    private Button btnKey1;
    private Button btnKey2;
    private Button btnKey3;
    private Button btnKey4;
    private Button btnKey5;
    private Button btnKey6;
    private Button btnKey7;
    private Button btnKey8;
    private Button btnKey9;
    private Button btnKeyClear;
    private Button btnKey0;
    private Button btnKeyBack;
    private Label lblStatus;
    private Button btnLogin;
}
