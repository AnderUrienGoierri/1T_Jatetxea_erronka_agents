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
        lblTitulua = new Label();
        lblAzpititulua = new Label();
        pnlTicketCard = new Panel();
        pnlTicketTotal = new Panel();
        lblGuztira = new Label();
        pnlTicketLines = new Panel();
        btnGehituEskaera = new Button();
        btnKontuaEskatu = new Button();
        btnAldatuMahaiaz = new Button();
        btnItxi = new Button();
        pnlTicketCard.SuspendLayout();
        pnlTicketTotal.SuspendLayout();
        SuspendLayout();
        // 
        // lblTitulua ("Mahaia 4 - Xehetasuna")
        // 
        lblTitulua.Font = new Font("Segoe UI", 17F, FontStyle.Bold);
        lblTitulua.ForeColor = Color.FromArgb(30, 41, 59);
        lblTitulua.Location = new Point(20, 16);
        lblTitulua.Name = "lblTitulua";
        lblTitulua.Size = new Size(640, 48);
        lblTitulua.TabIndex = 0;
        lblTitulua.Text = "Mahaia 4 - Xehetasuna";
        lblTitulua.TextAlign = ContentAlignment.MiddleCenter;
        lblTitulua.UseCompatibleTextRendering = true;
        // 
        // lblAzpititulua ("(4 pertsona) - Okupatua")
        // 
        lblAzpititulua.Font = new Font("Segoe UI", 11.5F);
        lblAzpititulua.ForeColor = Color.FromArgb(100, 116, 139);
        lblAzpititulua.Location = new Point(20, 64);
        lblAzpititulua.Name = "lblAzpititulua";
        lblAzpititulua.Size = new Size(640, 32);
        lblAzpititulua.TabIndex = 1;
        lblAzpititulua.Text = "(4 pertsona) - Okupatua";
        lblAzpititulua.TextAlign = ContentAlignment.MiddleCenter;
        lblAzpititulua.UseCompatibleTextRendering = true;
        // 
        // pnlTicketCard (Tiket kaxa zuria zirriborroaren arabera)
        // 
        pnlTicketCard.BackColor = Color.White;
        pnlTicketCard.Controls.Add(pnlTicketLines);
        pnlTicketCard.Controls.Add(pnlTicketTotal);
        pnlTicketCard.Location = new Point(35, 104);
        pnlTicketCard.Name = "pnlTicketCard";
        pnlTicketCard.Padding = new Padding(14);
        pnlTicketCard.Size = new Size(610, 300);
        pnlTicketCard.TabIndex = 2;
        // 
        // pnlTicketTotal (Banatzailea eta Guztira)
        // 
        pnlTicketTotal.Controls.Add(lblGuztira);
        pnlTicketTotal.Dock = DockStyle.Bottom;
        pnlTicketTotal.Location = new Point(14, 232);
        pnlTicketTotal.Name = "pnlTicketTotal";
        pnlTicketTotal.Size = new Size(582, 54);
        pnlTicketTotal.TabIndex = 1;
        // 
        // lblGuztira ("Guztira: 60.00 €")
        // 
        lblGuztira.Dock = DockStyle.Fill;
        lblGuztira.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        lblGuztira.ForeColor = Color.FromArgb(30, 41, 59);
        lblGuztira.Location = new Point(0, 0);
        lblGuztira.Name = "lblGuztira";
        lblGuztira.Size = new Size(582, 54);
        lblGuztira.TabIndex = 0;
        lblGuztira.Text = "Guztira:  60.00 €";
        lblGuztira.TextAlign = ContentAlignment.MiddleRight;
        // 
        // pnlTicketLines (Eskaera lerroen zerrenda biribildua)
        // 
        pnlTicketLines.AutoScroll = true;
        pnlTicketLines.Dock = DockStyle.Fill;
        pnlTicketLines.Location = new Point(14, 14);
        pnlTicketLines.Name = "pnlTicketLines";
        pnlTicketLines.Size = new Size(582, 218);
        pnlTicketLines.TabIndex = 0;
        // 
        // btnGehituEskaera ("+ ESKAERA GEHITU")
        // 
        btnGehituEskaera.BackColor = Color.FromArgb(136, 19, 55);
        btnGehituEskaera.Cursor = Cursors.Hand;
        btnGehituEskaera.FlatAppearance.BorderSize = 0;
        btnGehituEskaera.FlatStyle = FlatStyle.Flat;
        btnGehituEskaera.Font = new Font("Segoe UI", 12.5F, FontStyle.Bold);
        btnGehituEskaera.ForeColor = Color.White;
        btnGehituEskaera.Location = new Point(35, 420);
        btnGehituEskaera.Name = "btnGehituEskaera";
        btnGehituEskaera.Size = new Size(610, 54);
        btnGehituEskaera.TabIndex = 3;
        btnGehituEskaera.Text = "+ ESKAERA GEHITU";
        btnGehituEskaera.UseVisualStyleBackColor = false;
        // 
        // btnKontuaEskatu ("Kontua Eskatu / Kobratu")
        // 
        btnKontuaEskatu.BackColor = Color.FromArgb(248, 250, 252);
        btnKontuaEskatu.Cursor = Cursors.Hand;
        btnKontuaEskatu.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnKontuaEskatu.FlatStyle = FlatStyle.Flat;
        btnKontuaEskatu.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        btnKontuaEskatu.ForeColor = Color.FromArgb(51, 65, 85);
        btnKontuaEskatu.Location = new Point(35, 486);
        btnKontuaEskatu.Name = "btnKontuaEskatu";
        btnKontuaEskatu.Size = new Size(610, 50);
        btnKontuaEskatu.TabIndex = 4;
        btnKontuaEskatu.Text = "Kontua Eskatu";
        btnKontuaEskatu.UseVisualStyleBackColor = false;
        // 
        // btnAldatuMahaiaz ("Aldatu Mahaiaz")
        // 
        btnAldatuMahaiaz.BackColor = Color.FromArgb(248, 250, 252);
        btnAldatuMahaiaz.Cursor = Cursors.Hand;
        btnAldatuMahaiaz.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnAldatuMahaiaz.FlatStyle = FlatStyle.Flat;
        btnAldatuMahaiaz.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        btnAldatuMahaiaz.ForeColor = Color.FromArgb(51, 65, 85);
        btnAldatuMahaiaz.Location = new Point(35, 548);
        btnAldatuMahaiaz.Name = "btnAldatuMahaiaz";
        btnAldatuMahaiaz.Size = new Size(610, 50);
        btnAldatuMahaiaz.TabIndex = 5;
        btnAldatuMahaiaz.Text = "Aldatu Mahaiaz";
        btnAldatuMahaiaz.UseVisualStyleBackColor = false;
        // 
        // btnItxi ("Itxi")
        // 
        btnItxi.BackColor = Color.FromArgb(241, 245, 249);
        btnItxi.Cursor = Cursors.Hand;
        btnItxi.DialogResult = DialogResult.Cancel;
        btnItxi.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnItxi.FlatStyle = FlatStyle.Flat;
        btnItxi.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        btnItxi.ForeColor = Color.FromArgb(100, 116, 139);
        btnItxi.Location = new Point(265, 616);
        btnItxi.Name = "btnItxi";
        btnItxi.Size = new Size(150, 46);
        btnItxi.TabIndex = 6;
        btnItxi.Text = "Itxi";
        btnItxi.UseVisualStyleBackColor = false;
        // 
        // MahaiaXehetasunakModal
        // 
        AutoScaleMode = AutoScaleMode.None;
        BackColor = Color.FromArgb(241, 245, 249);
        CancelButton = btnItxi;
        ClientSize = new Size(680, 690);
        Controls.Add(btnItxi);
        Controls.Add(btnAldatuMahaiaz);
        Controls.Add(btnKontuaEskatu);
        Controls.Add(btnGehituEskaera);
        Controls.Add(pnlTicketCard);
        Controls.Add(lblAzpititulua);
        Controls.Add(lblTitulua);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "MahaiaXehetasunakModal";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Mahaiaren Xehetasuna";
        pnlTicketCard.ResumeLayout(false);
        pnlTicketTotal.ResumeLayout(false);
        ResumeLayout(false);
    }

    private Label lblTitulua;
    private Label lblAzpititulua;
    private Panel pnlTicketCard;
    private Panel pnlTicketLines;
    private Panel pnlTicketTotal;
    private Label lblGuztira;
    private Button btnGehituEskaera;
    private Button btnKontuaEskatu;
    private Button btnAldatuMahaiaz;
    private Button btnItxi;
}

