namespace Jatetxea.Desktop.Forms;

partial class TiketIkusteaForm
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
        pnlGoiburua = new Panel();
        lblSub = new Label();
        lblTit = new Label();
        pnlBehea = new Panel();
        btnItxi = new Button();
        btnInprimatu = new Button();
        btnGorde = new Button();
        pnlKutxa = new Panel();
        _txtTiket = new TextBox();
        pnlGoiburua.SuspendLayout();
        pnlBehea.SuspendLayout();
        pnlKutxa.SuspendLayout();
        SuspendLayout();
        // 
        // pnlGoiburua
        // 
        pnlGoiburua.BackColor = Color.FromArgb(22, 101, 52);
        pnlGoiburua.Controls.Add(lblSub);
        pnlGoiburua.Controls.Add(lblTit);
        pnlGoiburua.Dock = DockStyle.Top;
        pnlGoiburua.Location = new Point(0, 0);
        pnlGoiburua.Name = "pnlGoiburua";
        pnlGoiburua.Padding = new Padding(20, 12, 20, 12);
        pnlGoiburua.Size = new Size(524, 70);
        pnlGoiburua.TabIndex = 0;
        // 
        // lblSub
        // 
        lblSub.Dock = DockStyle.Top;
        lblSub.Font = new Font("Segoe UI", 10F);
        lblSub.ForeColor = Color.FromArgb(220, 252, 231);
        lblSub.Location = new Point(20, 36);
        lblSub.Name = "lblSub";
        lblSub.Size = new Size(484, 20);
        lblSub.TabIndex = 1;
        lblSub.Text = "Tiket Zenbakia: ...";
        // 
        // lblTit
        // 
        lblTit.Dock = DockStyle.Top;
        lblTit.Font = new Font("Segoe UI", 12.5F, FontStyle.Bold);
        lblTit.ForeColor = Color.White;
        lblTit.Location = new Point(20, 12);
        lblTit.Name = "lblTit";
        lblTit.Size = new Size(484, 24);
        lblTit.TabIndex = 0;
        lblTit.Text = "ORDAINKETA ONARTUA - TIKET FISKALA";
        // 
        // pnlBehea
        // 
        pnlBehea.BackColor = Color.White;
        pnlBehea.Controls.Add(btnItxi);
        pnlBehea.Controls.Add(btnInprimatu);
        pnlBehea.Controls.Add(btnGorde);
        pnlBehea.Dock = DockStyle.Bottom;
        pnlBehea.Location = new Point(0, 616);
        pnlBehea.Name = "pnlBehea";
        pnlBehea.Padding = new Padding(16, 12, 16, 12);
        pnlBehea.Size = new Size(524, 65);
        pnlBehea.TabIndex = 1;
        // 
        // btnItxi
        // 
        btnItxi.BackColor = Color.FromArgb(71, 85, 105);
        btnItxi.Cursor = Cursors.Hand;
        btnItxi.FlatAppearance.BorderSize = 0;
        btnItxi.FlatStyle = FlatStyle.Flat;
        btnItxi.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnItxi.ForeColor = Color.White;
        btnItxi.Location = new Point(410, 12);
        btnItxi.Name = "btnItxi";
        btnItxi.Size = new Size(95, 40);
        btnItxi.TabIndex = 2;
        btnItxi.Text = "Itxi";
        btnItxi.UseVisualStyleBackColor = false;
        // 
        // btnInprimatu
        // 
        btnInprimatu.BackColor = Color.FromArgb(136, 19, 55);
        btnInprimatu.Cursor = Cursors.Hand;
        btnInprimatu.FlatAppearance.BorderSize = 0;
        btnInprimatu.FlatStyle = FlatStyle.Flat;
        btnInprimatu.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnInprimatu.ForeColor = Color.White;
        btnInprimatu.Location = new Point(176, 12);
        btnInprimatu.Name = "btnInprimatu";
        btnInprimatu.Size = new Size(160, 40);
        btnInprimatu.TabIndex = 1;
        btnInprimatu.Text = "Inprimatu (ESC/POS)";
        btnInprimatu.UseVisualStyleBackColor = false;
        // 
        // btnGorde
        // 
        btnGorde.BackColor = Color.FromArgb(241, 245, 249);
        btnGorde.Cursor = Cursors.Hand;
        btnGorde.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnGorde.FlatStyle = FlatStyle.Flat;
        btnGorde.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnGorde.ForeColor = Color.FromArgb(30, 41, 59);
        btnGorde.Location = new Point(16, 12);
        btnGorde.Name = "btnGorde";
        btnGorde.Size = new Size(150, 40);
        btnGorde.TabIndex = 0;
        btnGorde.Text = "Gorde Tiketa (.txt)";
        btnGorde.UseVisualStyleBackColor = false;
        // 
        // pnlKutxa
        // 
        pnlKutxa.BackColor = Color.FromArgb(241, 245, 249);
        pnlKutxa.Controls.Add(_txtTiket);
        pnlKutxa.Dock = DockStyle.Fill;
        pnlKutxa.Location = new Point(0, 70);
        pnlKutxa.Name = "pnlKutxa";
        pnlKutxa.Padding = new Padding(24, 16, 24, 16);
        pnlKutxa.Size = new Size(524, 546);
        pnlKutxa.TabIndex = 2;
        // 
        // _txtTiket
        // 
        _txtTiket.BackColor = Color.White;
        _txtTiket.Dock = DockStyle.Fill;
        _txtTiket.Font = new Font("Consolas", 10F);
        _txtTiket.ForeColor = Color.FromArgb(15, 23, 42);
        _txtTiket.Location = new Point(24, 16);
        _txtTiket.Multiline = true;
        _txtTiket.Name = "_txtTiket";
        _txtTiket.ReadOnly = true;
        _txtTiket.ScrollBars = ScrollBars.Vertical;
        _txtTiket.Size = new Size(476, 514);
        _txtTiket.TabIndex = 0;
        // 
        // TiketIkusteaForm
        // 
        AutoScaleMode = AutoScaleMode.None;
        BackColor = Color.FromArgb(248, 250, 252);
        ClientSize = new Size(524, 681);
        Controls.Add(pnlKutxa);
        Controls.Add(pnlBehea);
        Controls.Add(pnlGoiburua);
        Font = new Font("Segoe UI", 9.5F);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "TiketIkusteaForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "T1 Jatetxea - Tiket Fiskala";
        pnlGoiburua.ResumeLayout(false);
        pnlBehea.ResumeLayout(false);
        pnlKutxa.ResumeLayout(false);
        pnlKutxa.PerformLayout();
        ResumeLayout(false);
    }

    private Panel pnlGoiburua;
    private Label lblTit;
    private Label lblSub;
    private Panel pnlBehea;
    private Button btnGorde;
    private Button btnInprimatu;
    private Button btnItxi;
    private Panel pnlKutxa;
    private TextBox _txtTiket;
}
