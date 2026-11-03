namespace Jatetxea.Desktop.Forms;

partial class MahaikideakModal
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
        lblTitulua = new Label();
        lblAzpititulua = new Label();
        pnlSareta = new TableLayoutPanel();
        btnNum1 = new Button();
        btnNum2 = new Button();
        btnNum3 = new Button();
        btnNum4 = new Button();
        btnNum5 = new Button();
        btnNum6 = new Button();
        btnNum7 = new Button();
        btnNum8Plus = new Button();
        pnlStepper = new Panel();
        btnMinus = new Button();
        lblKopuruaTestua = new Label();
        btnPlus = new Button();
        pnlEkintzak = new Panel();
        btnUtzi = new Button();
        btnJarraitu = new Button();
        pnlSareta.SuspendLayout();
        pnlStepper.SuspendLayout();
        pnlEkintzak.SuspendLayout();
        SuspendLayout();
        // 
        // lblTitulua
        // 
        lblTitulua.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        lblTitulua.ForeColor = Color.FromArgb(30, 41, 59);
        lblTitulua.Location = new Point(20, 20);
        lblTitulua.Name = "lblTitulua";
        lblTitulua.Size = new Size(464, 38);
        lblTitulua.TabIndex = 0;
        lblTitulua.Text = "Mahaia 4 - Mahaikide Kopurua";
        lblTitulua.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // lblAzpititulua
        // 
        lblAzpititulua.Font = new Font("Segoe UI", 11F);
        lblAzpititulua.ForeColor = Color.FromArgb(100, 116, 139);
        lblAzpititulua.Location = new Point(20, 58);
        lblAzpititulua.Name = "lblAzpititulua";
        lblAzpititulua.Size = new Size(464, 28);
        lblAzpititulua.TabIndex = 1;
        lblAzpititulua.Text = "Aukeratu pertsona kopurua";
        lblAzpititulua.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // pnlSareta (4x2 botoi handien sareta)
        // 
        pnlSareta.ColumnCount = 4;
        pnlSareta.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        pnlSareta.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        pnlSareta.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        pnlSareta.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        pnlSareta.Controls.Add(btnNum1, 0, 0);
        pnlSareta.Controls.Add(btnNum2, 1, 0);
        pnlSareta.Controls.Add(btnNum3, 2, 0);
        pnlSareta.Controls.Add(btnNum4, 3, 0);
        pnlSareta.Controls.Add(btnNum5, 0, 1);
        pnlSareta.Controls.Add(btnNum6, 1, 1);
        pnlSareta.Controls.Add(btnNum7, 2, 1);
        pnlSareta.Controls.Add(btnNum8Plus, 3, 1);
        pnlSareta.Location = new Point(34, 102);
        pnlSareta.Name = "pnlSareta";
        pnlSareta.RowCount = 2;
        pnlSareta.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        pnlSareta.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        pnlSareta.Size = new Size(436, 160);
        pnlSareta.TabIndex = 2;
        // 
        // btnNum1
        // 
        btnNum1.BackColor = Color.FromArgb(248, 250, 252);
        btnNum1.Cursor = Cursors.Hand;
        btnNum1.Dock = DockStyle.Fill;
        btnNum1.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnNum1.FlatStyle = FlatStyle.Flat;
        btnNum1.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        btnNum1.ForeColor = Color.FromArgb(30, 41, 59);
        btnNum1.Location = new Point(4, 4);
        btnNum1.Margin = new Padding(4);
        btnNum1.Name = "btnNum1";
        btnNum1.Size = new Size(101, 72);
        btnNum1.TabIndex = 0;
        btnNum1.Text = "1";
        btnNum1.UseVisualStyleBackColor = false;
        // 
        // btnNum2
        // 
        btnNum2.BackColor = Color.FromArgb(248, 250, 252);
        btnNum2.Cursor = Cursors.Hand;
        btnNum2.Dock = DockStyle.Fill;
        btnNum2.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnNum2.FlatStyle = FlatStyle.Flat;
        btnNum2.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        btnNum2.ForeColor = Color.FromArgb(30, 41, 59);
        btnNum2.Location = new Point(113, 4);
        btnNum2.Margin = new Padding(4);
        btnNum2.Name = "btnNum2";
        btnNum2.Size = new Size(101, 72);
        btnNum2.TabIndex = 1;
        btnNum2.Text = "2";
        btnNum2.UseVisualStyleBackColor = false;
        // 
        // btnNum3
        // 
        btnNum3.BackColor = Color.FromArgb(248, 250, 252);
        btnNum3.Cursor = Cursors.Hand;
        btnNum3.Dock = DockStyle.Fill;
        btnNum3.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnNum3.FlatStyle = FlatStyle.Flat;
        btnNum3.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        btnNum3.ForeColor = Color.FromArgb(30, 41, 59);
        btnNum3.Location = new Point(222, 4);
        btnNum3.Margin = new Padding(4);
        btnNum3.Name = "btnNum3";
        btnNum3.Size = new Size(101, 72);
        btnNum3.TabIndex = 2;
        btnNum3.Text = "3";
        btnNum3.UseVisualStyleBackColor = false;
        // 
        // btnNum4
        // 
        btnNum4.BackColor = Color.FromArgb(136, 19, 55);
        btnNum4.Cursor = Cursors.Hand;
        btnNum4.Dock = DockStyle.Fill;
        btnNum4.FlatAppearance.BorderSize = 0;
        btnNum4.FlatStyle = FlatStyle.Flat;
        btnNum4.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        btnNum4.ForeColor = Color.White;
        btnNum4.Location = new Point(331, 4);
        btnNum4.Margin = new Padding(4);
        btnNum4.Name = "btnNum4";
        btnNum4.Size = new Size(101, 72);
        btnNum4.TabIndex = 3;
        btnNum4.Text = "4";
        btnNum4.UseVisualStyleBackColor = false;
        // 
        // btnNum5
        // 
        btnNum5.BackColor = Color.FromArgb(248, 250, 252);
        btnNum5.Cursor = Cursors.Hand;
        btnNum5.Dock = DockStyle.Fill;
        btnNum5.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnNum5.FlatStyle = FlatStyle.Flat;
        btnNum5.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        btnNum5.ForeColor = Color.FromArgb(30, 41, 59);
        btnNum5.Location = new Point(4, 84);
        btnNum5.Margin = new Padding(4);
        btnNum5.Name = "btnNum5";
        btnNum5.Size = new Size(101, 72);
        btnNum5.TabIndex = 4;
        btnNum5.Text = "5";
        btnNum5.UseVisualStyleBackColor = false;
        // 
        // btnNum6
        // 
        btnNum6.BackColor = Color.FromArgb(248, 250, 252);
        btnNum6.Cursor = Cursors.Hand;
        btnNum6.Dock = DockStyle.Fill;
        btnNum6.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnNum6.FlatStyle = FlatStyle.Flat;
        btnNum6.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        btnNum6.ForeColor = Color.FromArgb(30, 41, 59);
        btnNum6.Location = new Point(113, 84);
        btnNum6.Margin = new Padding(4);
        btnNum6.Name = "btnNum6";
        btnNum6.Size = new Size(101, 72);
        btnNum6.TabIndex = 5;
        btnNum6.Text = "6";
        btnNum6.UseVisualStyleBackColor = false;
        // 
        // btnNum7
        // 
        btnNum7.BackColor = Color.FromArgb(248, 250, 252);
        btnNum7.Cursor = Cursors.Hand;
        btnNum7.Dock = DockStyle.Fill;
        btnNum7.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnNum7.FlatStyle = FlatStyle.Flat;
        btnNum7.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        btnNum7.ForeColor = Color.FromArgb(30, 41, 59);
        btnNum7.Location = new Point(222, 84);
        btnNum7.Margin = new Padding(4);
        btnNum7.Name = "btnNum7";
        btnNum7.Size = new Size(101, 72);
        btnNum7.TabIndex = 6;
        btnNum7.Text = "7";
        btnNum7.UseVisualStyleBackColor = false;
        // 
        // btnNum8Plus
        // 
        btnNum8Plus.BackColor = Color.FromArgb(248, 250, 252);
        btnNum8Plus.Cursor = Cursors.Hand;
        btnNum8Plus.Dock = DockStyle.Fill;
        btnNum8Plus.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnNum8Plus.FlatStyle = FlatStyle.Flat;
        btnNum8Plus.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        btnNum8Plus.ForeColor = Color.FromArgb(30, 41, 59);
        btnNum8Plus.Location = new Point(331, 84);
        btnNum8Plus.Margin = new Padding(4);
        btnNum8Plus.Name = "btnNum8Plus";
        btnNum8Plus.Size = new Size(101, 72);
        btnNum8Plus.TabIndex = 7;
        btnNum8Plus.Text = "8+";
        btnNum8Plus.UseVisualStyleBackColor = false;
        // 
        // pnlStepper ([-] 4 pertsona [+])
        // 
        pnlStepper.Controls.Add(btnPlus);
        pnlStepper.Controls.Add(lblKopuruaTestua);
        pnlStepper.Controls.Add(btnMinus);
        pnlStepper.Location = new Point(34, 276);
        pnlStepper.Name = "pnlStepper";
        pnlStepper.Size = new Size(436, 60);
        pnlStepper.TabIndex = 3;
        // 
        // btnMinus
        // 
        btnMinus.BackColor = Color.FromArgb(241, 245, 249);
        btnMinus.Cursor = Cursors.Hand;
        btnMinus.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnMinus.FlatStyle = FlatStyle.Flat;
        btnMinus.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        btnMinus.ForeColor = Color.FromArgb(51, 65, 85);
        btnMinus.Location = new Point(4, 4);
        btnMinus.Name = "btnMinus";
        btnMinus.Size = new Size(58, 52);
        btnMinus.TabIndex = 0;
        btnMinus.Text = "-";
        btnMinus.UseVisualStyleBackColor = false;
        // 
        // lblKopuruaTestua
        // 
        lblKopuruaTestua.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
        lblKopuruaTestua.ForeColor = Color.FromArgb(30, 41, 59);
        lblKopuruaTestua.Location = new Point(68, 4);
        lblKopuruaTestua.Name = "lblKopuruaTestua";
        lblKopuruaTestua.Size = new Size(300, 52);
        lblKopuruaTestua.TabIndex = 1;
        lblKopuruaTestua.Text = "4 pertsona";
        lblKopuruaTestua.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // btnPlus
        // 
        btnPlus.BackColor = Color.FromArgb(241, 245, 249);
        btnPlus.Cursor = Cursors.Hand;
        btnPlus.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnPlus.FlatStyle = FlatStyle.Flat;
        btnPlus.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        btnPlus.ForeColor = Color.FromArgb(51, 65, 85);
        btnPlus.Location = new Point(374, 4);
        btnPlus.Name = "btnPlus";
        btnPlus.Size = new Size(58, 52);
        btnPlus.TabIndex = 2;
        btnPlus.Text = "+";
        btnPlus.UseVisualStyleBackColor = false;
        // 
        // pnlEkintzak (Utzi / Jarraitu)
        // 
        pnlEkintzak.Controls.Add(btnJarraitu);
        pnlEkintzak.Controls.Add(btnUtzi);
        pnlEkintzak.Location = new Point(34, 348);
        pnlEkintzak.Name = "pnlEkintzak";
        pnlEkintzak.Size = new Size(436, 62);
        pnlEkintzak.TabIndex = 4;
        // 
        // btnUtzi
        // 
        btnUtzi.BackColor = Color.FromArgb(241, 245, 249);
        btnUtzi.Cursor = Cursors.Hand;
        btnUtzi.DialogResult = DialogResult.Cancel;
        btnUtzi.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnUtzi.FlatStyle = FlatStyle.Flat;
        btnUtzi.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        btnUtzi.ForeColor = Color.FromArgb(71, 85, 105);
        btnUtzi.Location = new Point(4, 6);
        btnUtzi.Name = "btnUtzi";
        btnUtzi.Size = new Size(185, 50);
        btnUtzi.TabIndex = 0;
        btnUtzi.Text = "Utzi";
        btnUtzi.UseVisualStyleBackColor = false;
        // 
        // btnJarraitu
        // 
        btnJarraitu.BackColor = Color.FromArgb(136, 19, 55);
        btnJarraitu.Cursor = Cursors.Hand;
        btnJarraitu.DialogResult = DialogResult.OK;
        btnJarraitu.FlatAppearance.BorderSize = 0;
        btnJarraitu.FlatStyle = FlatStyle.Flat;
        btnJarraitu.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        btnJarraitu.ForeColor = Color.White;
        btnJarraitu.Location = new Point(203, 6);
        btnJarraitu.Name = "btnJarraitu";
        btnJarraitu.Size = new Size(229, 50);
        btnJarraitu.TabIndex = 1;
        btnJarraitu.Text = "Jarraitu";
        btnJarraitu.UseVisualStyleBackColor = false;
        // 
        // MahaikideakModal
        // 
        AcceptButton = btnJarraitu;
        AutoScaleMode = AutoScaleMode.None;
        BackColor = Color.White;
        CancelButton = btnUtzi;
        ClientSize = new Size(504, 430);
        Controls.Add(pnlEkintzak);
        Controls.Add(pnlStepper);
        Controls.Add(pnlSareta);
        Controls.Add(lblAzpititulua);
        Controls.Add(lblTitulua);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "MahaikideakModal";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Mahaikide Kopurua";
        pnlSareta.ResumeLayout(false);
        pnlStepper.ResumeLayout(false);
        pnlEkintzak.ResumeLayout(false);
        ResumeLayout(false);
    }

    private Label lblTitulua;
    private Label lblAzpititulua;
    private TableLayoutPanel pnlSareta;
    private Button btnNum1;
    private Button btnNum2;
    private Button btnNum3;
    private Button btnNum4;
    private Button btnNum5;
    private Button btnNum6;
    private Button btnNum7;
    private Button btnNum8Plus;
    private Panel pnlStepper;
    private Button btnMinus;
    private Label lblKopuruaTestua;
    private Button btnPlus;
    private Panel pnlEkintzak;
    private Button btnUtzi;
    private Button btnJarraitu;
}
