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
        // pnlTopBar
        // 
        pnlTopBar.BackColor = Color.White;
        pnlTopBar.Controls.Add(lblLogo);
        pnlTopBar.Controls.Add(lblBrand);
        pnlTopBar.Controls.Add(lblZerbitzariak);
        pnlTopBar.Dock = DockStyle.Top;
        pnlTopBar.Location = new Point(0, 0);
        pnlTopBar.Name = "pnlTopBar";
        pnlTopBar.Size = new Size(1024, 89);
        pnlTopBar.TabIndex = 0;
        // 
        // lblLogo
        // 
        lblLogo.BackColor = Color.FromArgb(136, 19, 55);
        lblLogo.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        lblLogo.ForeColor = Color.White;
        lblLogo.Location = new Point(25, 12);
        lblLogo.Name = "lblLogo";
        lblLogo.Size = new Size(42, 42);
        lblLogo.TabIndex = 0;
        lblLogo.Text = "T1";
        lblLogo.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // lblBrand
        // 
        lblBrand.AutoSize = true;
        lblBrand.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
        lblBrand.ForeColor = Color.FromArgb(136, 19, 55);
        lblBrand.Location = new Point(76, 18);
        lblBrand.Name = "lblBrand";
        lblBrand.Size = new Size(240, 54);
        lblBrand.TabIndex = 1;
        lblBrand.Text = "T1 Jatetxea";
        // 
        // lblZerbitzariak
        // 
        lblZerbitzariak.AutoSize = true;
        lblZerbitzariak.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
        lblZerbitzariak.ForeColor = Color.FromArgb(51, 65, 85);
        lblZerbitzariak.Location = new Point(386, 18);
        lblZerbitzariak.Name = "lblZerbitzariak";
        lblZerbitzariak.Size = new Size(249, 54);
        lblZerbitzariak.TabIndex = 2;
        lblZerbitzariak.Text = "Zerbitzariak";
        lblZerbitzariak.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // pnlCard
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
        pnlCard.Location = new Point(291, 95);
        pnlCard.Name = "pnlCard";
        pnlCard.Size = new Size(440, 580);
        pnlCard.TabIndex = 1;
        // 
        // pnlAvatars
        // 
        pnlAvatars.Controls.Add(pnlAvatar1);
        pnlAvatars.Controls.Add(pnlAvatar2);
        pnlAvatars.Controls.Add(pnlAvatar3);
        pnlAvatars.Controls.Add(pnlAvatar4);
        pnlAvatars.Controls.Add(pnlAvatar5);
        pnlAvatars.Location = new Point(15, 14);
        pnlAvatars.Name = "pnlAvatars";
        pnlAvatars.Size = new Size(410, 92);
        pnlAvatars.TabIndex = 0;
        // 
        // pnlAvatar1
        // 
        pnlAvatar1.Controls.Add(lblCircle1);
        pnlAvatar1.Controls.Add(lblName1);
        pnlAvatar1.Cursor = Cursors.Hand;
        pnlAvatar1.Location = new Point(4, 2);
        pnlAvatar1.Name = "pnlAvatar1";
        pnlAvatar1.Size = new Size(78, 88);
        pnlAvatar1.TabIndex = 0;
        // 
        // lblCircle1
        // 
        lblCircle1.BackColor = Color.FromArgb(239, 246, 255);
        lblCircle1.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
        lblCircle1.ForeColor = Color.FromArgb(37, 99, 235);
        lblCircle1.Location = new Point(16, 2);
        lblCircle1.Name = "lblCircle1";
        lblCircle1.Size = new Size(46, 46);
        lblCircle1.TabIndex = 0;
        lblCircle1.Text = "AB";
        lblCircle1.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // lblName1
        // 
        lblName1.Font = new Font("Segoe UI", 8.5F);
        lblName1.ForeColor = Color.FromArgb(51, 65, 85);
        lblName1.Location = new Point(0, 52);
        lblName1.Name = "lblName1";
        lblName1.Size = new Size(78, 34);
        lblName1.TabIndex = 1;
        lblName1.Text = "Aitor B.";
        lblName1.TextAlign = ContentAlignment.TopCenter;
        // 
        // pnlAvatar2
        // 
        pnlAvatar2.Controls.Add(lblCircle2);
        pnlAvatar2.Controls.Add(lblName2);
        pnlAvatar2.Cursor = Cursors.Hand;
        pnlAvatar2.Location = new Point(86, 2);
        pnlAvatar2.Name = "pnlAvatar2";
        pnlAvatar2.Size = new Size(78, 88);
        pnlAvatar2.TabIndex = 1;
        // 
        // lblCircle2
        // 
        lblCircle2.BackColor = Color.FromArgb(241, 245, 249);
        lblCircle2.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
        lblCircle2.ForeColor = Color.FromArgb(71, 85, 105);
        lblCircle2.Location = new Point(16, 2);
        lblCircle2.Name = "lblCircle2";
        lblCircle2.Size = new Size(46, 46);
        lblCircle2.TabIndex = 0;
        lblCircle2.Text = "ML";
        lblCircle2.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // lblName2
        // 
        lblName2.Font = new Font("Segoe UI", 8.5F);
        lblName2.ForeColor = Color.FromArgb(51, 65, 85);
        lblName2.Location = new Point(0, 52);
        lblName2.Name = "lblName2";
        lblName2.Size = new Size(78, 34);
        lblName2.TabIndex = 1;
        lblName2.Text = "Maite L.";
        lblName2.TextAlign = ContentAlignment.TopCenter;
        // 
        // pnlAvatar3
        // 
        pnlAvatar3.Controls.Add(lblCircle3);
        pnlAvatar3.Controls.Add(lblName3);
        pnlAvatar3.Cursor = Cursors.Hand;
        pnlAvatar3.Location = new Point(168, 2);
        pnlAvatar3.Name = "pnlAvatar3";
        pnlAvatar3.Size = new Size(78, 88);
        pnlAvatar3.TabIndex = 2;
        // 
        // lblCircle3
        // 
        lblCircle3.BackColor = Color.FromArgb(241, 245, 249);
        lblCircle3.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
        lblCircle3.ForeColor = Color.FromArgb(71, 85, 105);
        lblCircle3.Location = new Point(16, 2);
        lblCircle3.Name = "lblCircle3";
        lblCircle3.Size = new Size(46, 46);
        lblCircle3.TabIndex = 0;
        lblCircle3.Text = "MA";
        lblCircle3.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // lblName3
        // 
        lblName3.Font = new Font("Segoe UI", 8.5F);
        lblName3.ForeColor = Color.FromArgb(51, 65, 85);
        lblName3.Location = new Point(0, 52);
        lblName3.Name = "lblName3";
        lblName3.Size = new Size(78, 34);
        lblName3.TabIndex = 1;
        lblName3.Text = "Mikel A.";
        lblName3.TextAlign = ContentAlignment.TopCenter;
        // 
        // pnlAvatar4
        // 
        pnlAvatar4.Controls.Add(lblCircle4);
        pnlAvatar4.Controls.Add(lblName4);
        pnlAvatar4.Cursor = Cursors.Hand;
        pnlAvatar4.Location = new Point(250, 2);
        pnlAvatar4.Name = "pnlAvatar4";
        pnlAvatar4.Size = new Size(78, 88);
        pnlAvatar4.TabIndex = 3;
        // 
        // lblCircle4
        // 
        lblCircle4.BackColor = Color.FromArgb(241, 245, 249);
        lblCircle4.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
        lblCircle4.ForeColor = Color.FromArgb(71, 85, 105);
        lblCircle4.Location = new Point(16, 2);
        lblCircle4.Name = "lblCircle4";
        lblCircle4.Size = new Size(46, 46);
        lblCircle4.TabIndex = 0;
        lblCircle4.Text = "AG";
        lblCircle4.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // lblName4
        // 
        lblName4.Font = new Font("Segoe UI", 8.5F);
        lblName4.ForeColor = Color.FromArgb(51, 65, 85);
        lblName4.Location = new Point(0, 52);
        lblName4.Name = "lblName4";
        lblName4.Size = new Size(78, 34);
        lblName4.TabIndex = 1;
        lblName4.Text = "Ane G.";
        lblName4.TextAlign = ContentAlignment.TopCenter;
        // 
        // pnlAvatar5
        // 
        pnlAvatar5.Controls.Add(lblCircle5);
        pnlAvatar5.Controls.Add(lblName5);
        pnlAvatar5.Cursor = Cursors.Hand;
        pnlAvatar5.Location = new Point(332, 2);
        pnlAvatar5.Name = "pnlAvatar5";
        pnlAvatar5.Size = new Size(78, 88);
        pnlAvatar5.TabIndex = 4;
        // 
        // lblCircle5
        // 
        lblCircle5.BackColor = Color.FromArgb(241, 245, 249);
        lblCircle5.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
        lblCircle5.ForeColor = Color.FromArgb(71, 85, 105);
        lblCircle5.Location = new Point(16, 2);
        lblCircle5.Name = "lblCircle5";
        lblCircle5.Size = new Size(46, 46);
        lblCircle5.TabIndex = 0;
        lblCircle5.Text = "IM";
        lblCircle5.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // lblName5
        // 
        lblName5.Font = new Font("Segoe UI", 8.5F);
        lblName5.ForeColor = Color.FromArgb(51, 65, 85);
        lblName5.Location = new Point(0, 52);
        lblName5.Name = "lblName5";
        lblName5.Size = new Size(78, 34);
        lblName5.TabIndex = 1;
        lblName5.Text = "Iker M.";
        lblName5.TextAlign = ContentAlignment.TopCenter;
        // 
        // pnlDivider
        // 
        pnlDivider.BackColor = Color.FromArgb(226, 232, 240);
        pnlDivider.Location = new Point(20, 112);
        pnlDivider.Name = "pnlDivider";
        pnlDivider.Size = new Size(400, 1);
        pnlDivider.TabIndex = 1;
        // 
        // lblPinTitle
        // 
        lblPinTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        lblPinTitle.ForeColor = Color.FromArgb(30, 41, 59);
        lblPinTitle.Location = new Point(20, 122);
        lblPinTitle.Name = "lblPinTitle";
        lblPinTitle.Size = new Size(400, 36);
        lblPinTitle.TabIndex = 2;
        lblPinTitle.Text = "SARTU PIN ZENBAKIA";
        lblPinTitle.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // pnlPinDots
        // 
        pnlPinDots.Controls.Add(lblDot1);
        pnlPinDots.Controls.Add(lblDot2);
        pnlPinDots.Controls.Add(lblDot3);
        pnlPinDots.Controls.Add(lblDot4);
        pnlPinDots.Location = new Point(135, 164);
        pnlPinDots.Name = "pnlPinDots";
        pnlPinDots.Size = new Size(170, 26);
        pnlPinDots.TabIndex = 3;
        // 
        // lblDot1
        // 
        lblDot1.Location = new Point(15, 3);
        lblDot1.Name = "lblDot1";
        lblDot1.Size = new Size(20, 20);
        lblDot1.TabIndex = 0;
        // 
        // lblDot2
        // 
        lblDot2.Location = new Point(55, 3);
        lblDot2.Name = "lblDot2";
        lblDot2.Size = new Size(20, 20);
        lblDot2.TabIndex = 1;
        // 
        // lblDot3
        // 
        lblDot3.Location = new Point(95, 3);
        lblDot3.Name = "lblDot3";
        lblDot3.Size = new Size(20, 20);
        lblDot3.TabIndex = 2;
        // 
        // lblDot4
        // 
        lblDot4.Location = new Point(135, 3);
        lblDot4.Name = "lblDot4";
        lblDot4.Size = new Size(20, 20);
        lblDot4.TabIndex = 3;
        // 
        // pnlKeypad
        // 
        pnlKeypad.ColumnCount = 3;
        pnlKeypad.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
        pnlKeypad.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
        pnlKeypad.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34F));
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
        pnlKeypad.Location = new Point(35, 198);
        pnlKeypad.Name = "pnlKeypad";
        pnlKeypad.RowCount = 4;
        pnlKeypad.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
        pnlKeypad.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
        pnlKeypad.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
        pnlKeypad.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
        pnlKeypad.Size = new Size(370, 256);
        pnlKeypad.TabIndex = 4;
        // 
        // btnKey1
        // 
        btnKey1.BackColor = Color.White;
        btnKey1.Cursor = Cursors.Hand;
        btnKey1.Dock = DockStyle.Fill;
        btnKey1.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnKey1.FlatStyle = FlatStyle.Flat;
        btnKey1.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        btnKey1.ForeColor = Color.FromArgb(30, 41, 59);
        btnKey1.Location = new Point(4, 4);
        btnKey1.Margin = new Padding(4);
        btnKey1.Name = "btnKey1";
        btnKey1.Size = new Size(115, 56);
        btnKey1.TabIndex = 0;
        btnKey1.Text = "1";
        btnKey1.UseVisualStyleBackColor = false;
        // 
        // btnKey2
        // 
        btnKey2.BackColor = Color.White;
        btnKey2.Cursor = Cursors.Hand;
        btnKey2.Dock = DockStyle.Fill;
        btnKey2.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnKey2.FlatStyle = FlatStyle.Flat;
        btnKey2.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        btnKey2.ForeColor = Color.FromArgb(30, 41, 59);
        btnKey2.Location = new Point(127, 4);
        btnKey2.Margin = new Padding(4);
        btnKey2.Name = "btnKey2";
        btnKey2.Size = new Size(115, 56);
        btnKey2.TabIndex = 1;
        btnKey2.Text = "2";
        btnKey2.UseVisualStyleBackColor = false;
        // 
        // btnKey3
        // 
        btnKey3.BackColor = Color.White;
        btnKey3.Cursor = Cursors.Hand;
        btnKey3.Dock = DockStyle.Fill;
        btnKey3.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnKey3.FlatStyle = FlatStyle.Flat;
        btnKey3.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        btnKey3.ForeColor = Color.FromArgb(30, 41, 59);
        btnKey3.Location = new Point(250, 4);
        btnKey3.Margin = new Padding(4);
        btnKey3.Name = "btnKey3";
        btnKey3.Size = new Size(116, 56);
        btnKey3.TabIndex = 2;
        btnKey3.Text = "3";
        btnKey3.UseVisualStyleBackColor = false;
        // 
        // btnKey4
        // 
        btnKey4.BackColor = Color.White;
        btnKey4.Cursor = Cursors.Hand;
        btnKey4.Dock = DockStyle.Fill;
        btnKey4.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnKey4.FlatStyle = FlatStyle.Flat;
        btnKey4.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        btnKey4.ForeColor = Color.FromArgb(30, 41, 59);
        btnKey4.Location = new Point(4, 68);
        btnKey4.Margin = new Padding(4);
        btnKey4.Name = "btnKey4";
        btnKey4.Size = new Size(115, 56);
        btnKey4.TabIndex = 3;
        btnKey4.Text = "4";
        btnKey4.UseVisualStyleBackColor = false;
        // 
        // btnKey5
        // 
        btnKey5.BackColor = Color.White;
        btnKey5.Cursor = Cursors.Hand;
        btnKey5.Dock = DockStyle.Fill;
        btnKey5.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnKey5.FlatStyle = FlatStyle.Flat;
        btnKey5.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        btnKey5.ForeColor = Color.FromArgb(30, 41, 59);
        btnKey5.Location = new Point(127, 68);
        btnKey5.Margin = new Padding(4);
        btnKey5.Name = "btnKey5";
        btnKey5.Size = new Size(115, 56);
        btnKey5.TabIndex = 4;
        btnKey5.Text = "5";
        btnKey5.UseVisualStyleBackColor = false;
        // 
        // btnKey6
        // 
        btnKey6.BackColor = Color.White;
        btnKey6.Cursor = Cursors.Hand;
        btnKey6.Dock = DockStyle.Fill;
        btnKey6.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnKey6.FlatStyle = FlatStyle.Flat;
        btnKey6.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        btnKey6.ForeColor = Color.FromArgb(30, 41, 59);
        btnKey6.Location = new Point(250, 68);
        btnKey6.Margin = new Padding(4);
        btnKey6.Name = "btnKey6";
        btnKey6.Size = new Size(116, 56);
        btnKey6.TabIndex = 5;
        btnKey6.Text = "6";
        btnKey6.UseVisualStyleBackColor = false;
        // 
        // btnKey7
        // 
        btnKey7.BackColor = Color.White;
        btnKey7.Cursor = Cursors.Hand;
        btnKey7.Dock = DockStyle.Fill;
        btnKey7.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnKey7.FlatStyle = FlatStyle.Flat;
        btnKey7.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        btnKey7.ForeColor = Color.FromArgb(30, 41, 59);
        btnKey7.Location = new Point(4, 132);
        btnKey7.Margin = new Padding(4);
        btnKey7.Name = "btnKey7";
        btnKey7.Size = new Size(115, 56);
        btnKey7.TabIndex = 6;
        btnKey7.Text = "7";
        btnKey7.UseVisualStyleBackColor = false;
        // 
        // btnKey8
        // 
        btnKey8.BackColor = Color.White;
        btnKey8.Cursor = Cursors.Hand;
        btnKey8.Dock = DockStyle.Fill;
        btnKey8.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnKey8.FlatStyle = FlatStyle.Flat;
        btnKey8.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        btnKey8.ForeColor = Color.FromArgb(30, 41, 59);
        btnKey8.Location = new Point(127, 132);
        btnKey8.Margin = new Padding(4);
        btnKey8.Name = "btnKey8";
        btnKey8.Size = new Size(115, 56);
        btnKey8.TabIndex = 7;
        btnKey8.Text = "8";
        btnKey8.UseVisualStyleBackColor = false;
        // 
        // btnKey9
        // 
        btnKey9.BackColor = Color.White;
        btnKey9.Cursor = Cursors.Hand;
        btnKey9.Dock = DockStyle.Fill;
        btnKey9.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnKey9.FlatStyle = FlatStyle.Flat;
        btnKey9.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        btnKey9.ForeColor = Color.FromArgb(30, 41, 59);
        btnKey9.Location = new Point(250, 132);
        btnKey9.Margin = new Padding(4);
        btnKey9.Name = "btnKey9";
        btnKey9.Size = new Size(116, 56);
        btnKey9.TabIndex = 8;
        btnKey9.Text = "9";
        btnKey9.UseVisualStyleBackColor = false;
        // 
        // btnKeyClear
        // 
        btnKeyClear.BackColor = Color.FromArgb(254, 242, 242);
        btnKeyClear.Cursor = Cursors.Hand;
        btnKeyClear.Dock = DockStyle.Fill;
        btnKeyClear.FlatAppearance.BorderColor = Color.FromArgb(254, 202, 202);
        btnKeyClear.FlatStyle = FlatStyle.Flat;
        btnKeyClear.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnKeyClear.ForeColor = Color.FromArgb(220, 38, 38);
        btnKeyClear.Location = new Point(4, 196);
        btnKeyClear.Margin = new Padding(4);
        btnKeyClear.Name = "btnKeyClear";
        btnKeyClear.Size = new Size(115, 56);
        btnKeyClear.TabIndex = 9;
        btnKeyClear.Text = "X Clear";
        btnKeyClear.UseVisualStyleBackColor = false;
        btnKeyClear.Click += btnKeyClear_Click;
        // 
        // btnKey0
        // 
        btnKey0.BackColor = Color.White;
        btnKey0.Cursor = Cursors.Hand;
        btnKey0.Dock = DockStyle.Fill;
        btnKey0.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnKey0.FlatStyle = FlatStyle.Flat;
        btnKey0.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        btnKey0.ForeColor = Color.FromArgb(30, 41, 59);
        btnKey0.Location = new Point(127, 196);
        btnKey0.Margin = new Padding(4);
        btnKey0.Name = "btnKey0";
        btnKey0.Size = new Size(115, 56);
        btnKey0.TabIndex = 10;
        btnKey0.Text = "0";
        btnKey0.UseVisualStyleBackColor = false;
        // 
        // btnKeyBack
        // 
        btnKeyBack.BackColor = Color.FromArgb(248, 250, 252);
        btnKeyBack.Cursor = Cursors.Hand;
        btnKeyBack.Dock = DockStyle.Fill;
        btnKeyBack.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnKeyBack.FlatStyle = FlatStyle.Flat;
        btnKeyBack.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        btnKeyBack.ForeColor = Color.FromArgb(30, 41, 59);
        btnKeyBack.Location = new Point(250, 196);
        btnKeyBack.Margin = new Padding(4);
        btnKeyBack.Name = "btnKeyBack";
        btnKeyBack.Size = new Size(116, 56);
        btnKeyBack.TabIndex = 11;
        btnKeyBack.Text = "←";
        btnKeyBack.UseVisualStyleBackColor = false;
        // 
        // lblStatus
        // 
        lblStatus.Font = new Font("Segoe UI", 9.5F);
        lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
        lblStatus.Location = new Point(20, 462);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(400, 26);
        lblStatus.TabIndex = 5;
        lblStatus.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // btnLogin
        // 
        btnLogin.BackColor = Color.FromArgb(136, 19, 55);
        btnLogin.Cursor = Cursors.Hand;
        btnLogin.FlatAppearance.BorderSize = 0;
        btnLogin.FlatStyle = FlatStyle.Flat;
        btnLogin.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
        btnLogin.ForeColor = Color.White;
        btnLogin.Location = new Point(35, 498);
        btnLogin.Name = "btnLogin";
        btnLogin.Size = new Size(370, 56);
        btnLogin.TabIndex = 6;
        btnLogin.Text = "HASI SAIOA";
        btnLogin.UseVisualStyleBackColor = false;
        // 
        // LoginForm
        // 
        AutoScaleMode = AutoScaleMode.None;
        BackColor = Color.FromArgb(241, 245, 249);
        ClientSize = new Size(1024, 720);
        Controls.Add(pnlCard);
        Controls.Add(pnlTopBar);
        MinimumSize = new Size(720, 680);
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
