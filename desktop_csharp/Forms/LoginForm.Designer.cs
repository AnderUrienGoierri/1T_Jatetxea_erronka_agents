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
        lblLogoBadge = new Label();
        lblBrand = new Label();
        lblZerbitzariak = new Label();
        pnlCard = new Panel();
        pnlAvatars = new FlowLayoutPanel();
        pnlDivider = new Panel();
        lblPinTitle = new Label();
        pnlPinDots = new Panel();
        pnlKeypad = new TableLayoutPanel();
        lblStatus = new Label();
        btnLogin = new Button();
        pnlTopBar.SuspendLayout();
        pnlCard.SuspendLayout();
        SuspendLayout();

        // 
        // pnlTopBar (Goiko goiburua: Logoa, Jatetxe izena eta Zerbitzariak)
        // 
        pnlTopBar.BackColor = Color.White;
        pnlTopBar.Controls.Add(lblLogoBadge);
        pnlTopBar.Controls.Add(lblBrand);
        pnlTopBar.Controls.Add(lblZerbitzariak);
        pnlTopBar.Dock = DockStyle.Top;
        pnlTopBar.Height = 60;
        pnlTopBar.Location = new Point(0, 0);
        pnlTopBar.Name = "pnlTopBar";
        pnlTopBar.Size = new Size(1024, 60);
        pnlTopBar.TabIndex = 0;

        // 
        // lblLogoBadge (T1 borobila/txapa)
        // 
        lblLogoBadge.BackColor = Color.FromArgb(136, 19, 55);
        lblLogoBadge.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
        lblLogoBadge.ForeColor = Color.White;
        lblLogoBadge.Location = new Point(25, 11);
        lblLogoBadge.Name = "lblLogoBadge";
        lblLogoBadge.Size = new Size(38, 38);
        lblLogoBadge.Text = "T1";
        lblLogoBadge.TextAlign = ContentAlignment.MiddleCenter;

        // 
        // lblBrand ("T1 Jatetxea")
        // 
        lblBrand.AutoSize = true;
        lblBrand.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        lblBrand.ForeColor = Color.FromArgb(136, 19, 55);
        lblBrand.Location = new Point(72, 17);
        lblBrand.Name = "lblBrand";
        lblBrand.Size = new Size(115, 25);
        lblBrand.Text = "T1 Jatetxea";

        // 
        // lblZerbitzariak ("Zerbitzariak" goiburuaren erdian)
        // 
        lblZerbitzariak.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        lblZerbitzariak.ForeColor = Color.FromArgb(51, 65, 85);
        lblZerbitzariak.Location = new Point(412, 15);
        lblZerbitzariak.Name = "lblZerbitzariak";
        lblZerbitzariak.Size = new Size(200, 30);
        lblZerbitzariak.Text = "Zerbitzariak";
        lblZerbitzariak.TextAlign = ContentAlignment.MiddleCenter;

        // 
        // pnlCard (Txartel nagusia zentratua)
        // 
        pnlCard.BackColor = Color.White;
        pnlCard.Controls.Add(pnlAvatars);
        pnlCard.Controls.Add(pnlDivider);
        pnlCard.Controls.Add(lblPinTitle);
        pnlCard.Controls.Add(pnlPinDots);
        pnlCard.Controls.Add(pnlKeypad);
        pnlCard.Controls.Add(lblStatus);
        pnlCard.Controls.Add(btnLogin);
        pnlCard.Location = new Point(312, 85);
        pnlCard.Name = "pnlCard";
        pnlCard.Size = new Size(400, 520);
        pnlCard.TabIndex = 1;

        // 
        // pnlAvatars (5 Zerbitzarien borobilak horizontalean)
        // 
        pnlAvatars.BackColor = Color.Transparent;
        pnlAvatars.Location = new Point(15, 12);
        pnlAvatars.Name = "pnlAvatars";
        pnlAvatars.Size = new Size(370, 78);
        pnlAvatars.WrapContents = false;
        pnlAvatars.TabIndex = 0;

        // 
        // pnlDivider (Lerro banatzailea)
        // 
        pnlDivider.BackColor = Color.FromArgb(226, 232, 240);
        pnlDivider.Location = new Point(20, 96);
        pnlDivider.Name = "pnlDivider";
        pnlDivider.Size = new Size(360, 1);
        pnlDivider.TabIndex = 1;

        // 
        // lblPinTitle ("SARTU PIN ZENBAKIA")
        // 
        lblPinTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblPinTitle.ForeColor = Color.FromArgb(51, 65, 85);
        lblPinTitle.Location = new Point(20, 106);
        lblPinTitle.Name = "lblPinTitle";
        lblPinTitle.Size = new Size(360, 20);
        lblPinTitle.Text = "SARTU PIN ZENBAKIA";
        lblPinTitle.TextAlign = ContentAlignment.MiddleCenter;
        lblPinTitle.TabIndex = 2;

        // 
        // pnlPinDots (4 PIN borobilak)
        // 
        pnlPinDots.Location = new Point(125, 130);
        pnlPinDots.Name = "pnlPinDots";
        pnlPinDots.Size = new Size(150, 24);
        pnlPinDots.TabIndex = 3;

        // 
        // pnlKeypad (Zenbakizko teklatua 3x4)
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
        pnlKeypad.Location = new Point(50, 162);
        pnlKeypad.Name = "pnlKeypad";
        pnlKeypad.Size = new Size(300, 225);
        pnlKeypad.TabIndex = 4;

        // 
        // lblStatus (Errore mezuak)
        // 
        lblStatus.Font = new Font("Segoe UI", 8.5F, FontStyle.Regular);
        lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
        lblStatus.Location = new Point(20, 395);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(360, 22);
        lblStatus.TextAlign = ContentAlignment.MiddleCenter;
        lblStatus.TabIndex = 5;

        // 
        // btnLogin ("HASI SAIOA")
        // 
        btnLogin.BackColor = Color.FromArgb(136, 19, 55);
        btnLogin.FlatAppearance.BorderSize = 0;
        btnLogin.FlatStyle = FlatStyle.Flat;
        btnLogin.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        btnLogin.ForeColor = Color.White;
        btnLogin.Location = new Point(50, 425);
        btnLogin.Name = "btnLogin";
        btnLogin.Size = new Size(300, 46);
        btnLogin.Text = "HASI SAIOA";
        btnLogin.UseVisualStyleBackColor = false;
        btnLogin.Cursor = Cursors.Hand;
        btnLogin.TabIndex = 6;

        // 
        // LoginForm
        // 
        AutoScaleMode = AutoScaleMode.None;
        BackColor = Color.FromArgb(241, 245, 249);
        ClientSize = new Size(1024, 700);
        Controls.Add(pnlCard);
        Controls.Add(pnlTopBar);
        MinimumSize = new Size(600, 620);
        Name = "LoginForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "T1 Jatetxea - Saioa Hasi (TPV Tablet)";
        pnlTopBar.ResumeLayout(false);
        pnlTopBar.PerformLayout();
        pnlCard.ResumeLayout(false);
        ResumeLayout(false);
    }

    private Panel pnlTopBar;
    private Label lblLogoBadge;
    private Label lblBrand;
    private Label lblZerbitzariak;
    private Panel pnlCard;
    private FlowLayoutPanel pnlAvatars;
    private Panel pnlDivider;
    private Label lblPinTitle;
    private Panel pnlPinDots;
    private TableLayoutPanel pnlKeypad;
    private Label lblStatus;
    private Button btnLogin;
}
