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
        // pnlTopBar (Goiko barra horizontala)
        // 
        pnlTopBar.BackColor = Color.White;
        pnlTopBar.Controls.Add(lblBrand);
        pnlTopBar.Controls.Add(lblZerbitzariak);
        pnlTopBar.Dock = DockStyle.Top;
        pnlTopBar.Height = 55;
        pnlTopBar.Location = new Point(0, 0);
        pnlTopBar.Name = "pnlTopBar";
        pnlTopBar.Size = new Size(1024, 55);
        pnlTopBar.TabIndex = 0;

        // 
        // lblBrand ("T1 Jatetxea")
        // 
        lblBrand.AutoSize = true;
        lblBrand.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
        lblBrand.ForeColor = Color.FromArgb(136, 19, 55); // #881337
        lblBrand.Location = new Point(25, 12);
        lblBrand.Name = "lblBrand";
        lblBrand.Size = new Size(140, 28);
        lblBrand.Text = "T1 Jatetxea";

        // 
        // lblZerbitzariak ("Zerbitzariak")
        // 
        lblZerbitzariak.Dock = DockStyle.Fill;
        lblZerbitzariak.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
        lblZerbitzariak.ForeColor = Color.FromArgb(51, 65, 85);
        lblZerbitzariak.Location = new Point(0, 0);
        lblZerbitzariak.Name = "lblZerbitzariak";
        lblZerbitzariak.Size = new Size(1024, 55);
        lblZerbitzariak.Text = "Zerbitzariak";
        lblZerbitzariak.TextAlign = ContentAlignment.MiddleCenter;

        // 
        // pnlCard (Txartel nagusia zentratuta)
        // 
        pnlCard.BackColor = Color.White;
        pnlCard.Controls.Add(pnlAvatars);
        pnlCard.Controls.Add(pnlDivider);
        pnlCard.Controls.Add(lblPinTitle);
        pnlCard.Controls.Add(pnlPinDots);
        pnlCard.Controls.Add(pnlKeypad);
        pnlCard.Controls.Add(lblStatus);
        pnlCard.Controls.Add(btnLogin);
        pnlCard.Location = new Point(282, 75);
        pnlCard.Name = "pnlCard";
        pnlCard.Size = new Size(460, 620);
        pnlCard.TabIndex = 1;

        // 
        // pnlAvatars (Zerbitzarien zerrenda horizontala)
        // 
        pnlAvatars.Location = new Point(15, 15);
        pnlAvatars.Name = "pnlAvatars";
        pnlAvatars.Size = new Size(430, 85);
        pnlAvatars.WrapContents = false;
        pnlAvatars.TabIndex = 0;

        // 
        // pnlDivider (Lerro banatzailea)
        // 
        pnlDivider.BackColor = Color.FromArgb(226, 232, 240);
        pnlDivider.Location = new Point(25, 110);
        pnlDivider.Name = "pnlDivider";
        pnlDivider.Size = new Size(410, 1);
        pnlDivider.TabIndex = 1;

        // 
        // lblPinTitle ("SARTU PIN ZENBAKIA")
        // 
        lblPinTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblPinTitle.ForeColor = Color.FromArgb(30, 41, 59);
        lblPinTitle.Location = new Point(25, 122);
        lblPinTitle.Name = "lblPinTitle";
        lblPinTitle.Size = new Size(410, 24);
        lblPinTitle.Text = "SARTU PIN ZENBAKIA";
        lblPinTitle.TextAlign = ContentAlignment.MiddleCenter;
        lblPinTitle.TabIndex = 2;

        // 
        // pnlPinDots (4 PIN borobilak)
        // 
        pnlPinDots.Location = new Point(155, 152);
        pnlPinDots.Name = "pnlPinDots";
        pnlPinDots.Size = new Size(150, 36);
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
        pnlKeypad.Location = new Point(65, 200);
        pnlKeypad.Name = "pnlKeypad";
        pnlKeypad.Size = new Size(330, 290);
        pnlKeypad.TabIndex = 4;

        // 
        // lblStatus (Mezuak / Erroreak)
        // 
        lblStatus.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
        lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
        lblStatus.Location = new Point(25, 502);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(410, 24);
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
        btnLogin.Location = new Point(65, 536);
        btnLogin.Name = "btnLogin";
        btnLogin.Size = new Size(330, 52);
        btnLogin.Text = "HASI SAIOA";
        btnLogin.UseVisualStyleBackColor = false;
        btnLogin.Cursor = Cursors.Hand;
        btnLogin.TabIndex = 6;

        // 
        // LoginForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Dpi;
        BackColor = Color.FromArgb(241, 245, 249);
        ClientSize = new Size(1024, 730);
        Controls.Add(pnlCard);
        Controls.Add(pnlTopBar);
        MinimumSize = new Size(800, 680);
        Name = "LoginForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "T1 Jatetxea - Saioa Hasi (TPV Tablet)";
        pnlTopBar.ResumeLayout(false);
        pnlTopBar.PerformLayout();
        pnlCard.ResumeLayout(false);
        ResumeLayout(false);
    }

    private Panel pnlTopBar;
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
