namespace Jatetxea.Desktop.Forms;

partial class OharraModal
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
        lblTitulua = new Label();
        lblPlaterIzena = new Label();
        lblPresetsTit = new Label();
        pnlPresets = new FlowLayoutPanel();
        lblIdatziTit = new Label();
        lblKaraktereak = new Label();
        txtOharra = new TextBox();
        pnlBehekoa = new Panel();
        btnGorde = new Button();
        btnKendu = new Button();
        btnUtzi = new Button();
        pnlGoiburua.SuspendLayout();
        pnlBehekoa.SuspendLayout();
        SuspendLayout();
        // 
        // pnlGoiburua
        // 
        pnlGoiburua.BackColor = Color.FromArgb(248, 250, 252);
        pnlGoiburua.Controls.Add(lblPlaterIzena);
        pnlGoiburua.Controls.Add(lblTitulua);
        pnlGoiburua.Dock = DockStyle.Top;
        pnlGoiburua.Location = new Point(0, 0);
        pnlGoiburua.Name = "pnlGoiburua";
        pnlGoiburua.Padding = new Padding(20, 12, 20, 12);
        pnlGoiburua.Size = new Size(590, 80);
        pnlGoiburua.TabIndex = 0;
        // 
        // lblTitulua
        // 
        lblTitulua.Font = new Font("Segoe UI", 13.5F, FontStyle.Bold);
        lblTitulua.ForeColor = Color.FromArgb(30, 41, 59);
        lblTitulua.Location = new Point(18, 10);
        lblTitulua.Name = "lblTitulua";
        lblTitulua.Size = new Size(550, 28);
        lblTitulua.TabIndex = 0;
        lblTitulua.Text = "Plateraren Oharra / Adierazpena";
        lblTitulua.TextAlign = ContentAlignment.MiddleLeft;
        lblTitulua.UseCompatibleTextRendering = false;
        // 
        // lblPlaterIzena
        // 
        lblPlaterIzena.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblPlaterIzena.ForeColor = Color.FromArgb(136, 19, 55);
        lblPlaterIzena.Location = new Point(18, 40);
        lblPlaterIzena.Name = "lblPlaterIzena";
        lblPlaterIzena.Size = new Size(550, 26);
        lblPlaterIzena.TabIndex = 1;
        lblPlaterIzena.Text = "Plateraren Izena";
        lblPlaterIzena.TextAlign = ContentAlignment.MiddleLeft;
        lblPlaterIzena.UseCompatibleTextRendering = false;
        // 
        // lblPresetsTit
        // 
        lblPresetsTit.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        lblPresetsTit.ForeColor = Color.FromArgb(71, 85, 105);
        lblPresetsTit.Location = new Point(20, 92);
        lblPresetsTit.Name = "lblPresetsTit";
        lblPresetsTit.Size = new Size(550, 22);
        lblPresetsTit.TabIndex = 1;
        lblPresetsTit.Text = "Aukera azkarrak (ukitu gehitzeko edo kentzeko):";
        lblPresetsTit.TextAlign = ContentAlignment.MiddleLeft;
        lblPresetsTit.UseCompatibleTextRendering = false;
        // 
        // pnlPresets
        // 
        pnlPresets.AutoScroll = true;
        pnlPresets.Location = new Point(20, 116);
        pnlPresets.Name = "pnlPresets";
        pnlPresets.Size = new Size(550, 150);
        pnlPresets.TabIndex = 2;
        // 
        // lblIdatziTit
        // 
        lblIdatziTit.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        lblIdatziTit.ForeColor = Color.FromArgb(71, 85, 105);
        lblIdatziTit.Location = new Point(20, 274);
        lblIdatziTit.Name = "lblIdatziTit";
        lblIdatziTit.Size = new Size(340, 22);
        lblIdatziTit.TabIndex = 3;
        lblIdatziTit.Text = "Adierazpen pertsonalizatua (sukalderako/barrarako):";
        lblIdatziTit.TextAlign = ContentAlignment.MiddleLeft;
        lblIdatziTit.UseCompatibleTextRendering = false;
        // 
        // lblKaraktereak
        // 
        lblKaraktereak.Font = new Font("Segoe UI", 8.5F);
        lblKaraktereak.ForeColor = Color.FromArgb(148, 163, 184);
        lblKaraktereak.Location = new Point(360, 274);
        lblKaraktereak.Name = "lblKaraktereak";
        lblKaraktereak.Size = new Size(210, 22);
        lblKaraktereak.TabIndex = 4;
        lblKaraktereak.Text = "0 / 150 karaktere";
        lblKaraktereak.TextAlign = ContentAlignment.MiddleRight;
        lblKaraktereak.UseCompatibleTextRendering = false;
        // 
        // txtOharra
        // 
        txtOharra.BorderStyle = BorderStyle.FixedSingle;
        txtOharra.Font = new Font("Segoe UI", 11F);
        txtOharra.Location = new Point(20, 300);
        txtOharra.MaxLength = 150;
        txtOharra.Multiline = true;
        txtOharra.Name = "txtOharra";
        txtOharra.PlaceholderText = "Idatzi hemen eskaera berezia (adib: gazta gutxi, piperrik gabe...)";
        txtOharra.ScrollBars = ScrollBars.Vertical;
        txtOharra.Size = new Size(550, 80);
        txtOharra.TabIndex = 5;
        // 
        // pnlBehekoa
        // 
        pnlBehekoa.BackColor = Color.FromArgb(248, 250, 252);
        pnlBehekoa.Controls.Add(btnUtzi);
        pnlBehekoa.Controls.Add(btnKendu);
        pnlBehekoa.Controls.Add(btnGorde);
        pnlBehekoa.Dock = DockStyle.Bottom;
        pnlBehekoa.Location = new Point(0, 436);
        pnlBehekoa.Name = "pnlBehekoa";
        pnlBehekoa.Padding = new Padding(20, 14, 20, 14);
        pnlBehekoa.Size = new Size(590, 84);
        pnlBehekoa.TabIndex = 6;
        // 
        // btnGorde
        // 
        btnGorde.BackColor = Color.FromArgb(136, 19, 55);
        btnGorde.Cursor = Cursors.Hand;
        btnGorde.FlatAppearance.BorderSize = 0;
        btnGorde.FlatStyle = FlatStyle.Flat;
        btnGorde.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        btnGorde.ForeColor = Color.White;
        btnGorde.Location = new Point(20, 16);
        btnGorde.Name = "btnGorde";
        btnGorde.Size = new Size(220, 50);
        btnGorde.TabIndex = 0;
        btnGorde.Text = "GORDE OHARRA";
        btnGorde.UseVisualStyleBackColor = false;
        // 
        // btnKendu
        // 
        btnKendu.BackColor = Color.FromArgb(254, 242, 242);
        btnKendu.Cursor = Cursors.Hand;
        btnKendu.FlatAppearance.BorderColor = Color.FromArgb(254, 202, 202);
        btnKendu.FlatStyle = FlatStyle.Flat;
        btnKendu.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnKendu.ForeColor = Color.FromArgb(220, 38, 38);
        btnKendu.Location = new Point(252, 16);
        btnKendu.Name = "btnKendu";
        btnKendu.Size = new Size(180, 50);
        btnKendu.TabIndex = 1;
        btnKendu.Text = "Ezabatu Oharra";
        btnKendu.UseVisualStyleBackColor = false;
        // 
        // btnUtzi
        // 
        btnUtzi.BackColor = Color.FromArgb(241, 245, 249);
        btnUtzi.Cursor = Cursors.Hand;
        btnUtzi.DialogResult = DialogResult.Cancel;
        btnUtzi.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnUtzi.FlatStyle = FlatStyle.Flat;
        btnUtzi.Font = new Font("Segoe UI", 10.5F);
        btnUtzi.ForeColor = Color.FromArgb(71, 85, 105);
        btnUtzi.Location = new Point(444, 16);
        btnUtzi.Name = "btnUtzi";
        btnUtzi.Size = new Size(126, 50);
        btnUtzi.TabIndex = 2;
        btnUtzi.Text = "Utzi";
        btnUtzi.UseVisualStyleBackColor = false;
        // 
        // OharraModal
        // 
        AcceptButton = btnGorde;
        AutoScaleMode = AutoScaleMode.None;
        BackColor = Color.White;
        CancelButton = btnUtzi;
        ClientSize = new Size(590, 520);
        Controls.Add(pnlBehekoa);
        Controls.Add(txtOharra);
        Controls.Add(lblKaraktereak);
        Controls.Add(lblIdatziTit);
        Controls.Add(pnlPresets);
        Controls.Add(lblPresetsTit);
        Controls.Add(pnlGoiburua);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "OharraModal";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Plateraren Oharra / Adierazpena";
        pnlGoiburua.ResumeLayout(false);
        pnlBehekoa.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    private Panel pnlGoiburua;
    private Label lblTitulua;
    private Label lblPlaterIzena;
    private Label lblPresetsTit;
    private FlowLayoutPanel pnlPresets;
    private Label lblIdatziTit;
    private Label lblKaraktereak;
    private TextBox txtOharra;
    private Panel pnlBehekoa;
    private Button btnGorde;
    private Button btnKendu;
    private Button btnUtzi;
}
