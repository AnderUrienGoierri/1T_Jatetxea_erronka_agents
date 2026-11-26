namespace Jatetxea.Desktop.Forms;

partial class ErreserbaBerriaModal
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
        pnlHeader = new Panel();
        lblSub = new Label();
        lblTit = new Label();
        pnlFooter = new Panel();
        _btnUtzi = new Button();
        _btnGorde = new Button();
        pnlBody = new Panel();
        _lblErrorea = new Label();
        _txtOharrak = new TextBox();
        lblOharrak = new Label();
        _cmbJatorria = new ComboBox();
        lblJatorria = new Label();
        _cmbMahaia = new ComboBox();
        _nudPax = new NumericUpDown();
        lblMahaia = new Label();
        lblPax = new Label();
        _cmbTxanda = new ComboBox();
        _dtpData = new DateTimePicker();
        lblTxanda = new Label();
        lblData = new Label();
        _txtEmaila = new TextBox();
        _txtTelefonoa = new TextBox();
        lblMail = new Label();
        lblTel = new Label();
        _txtAbizena = new TextBox();
        _txtIzena = new TextBox();
        lblAbizena = new Label();
        lblIzena = new Label();
        pnlHeader.SuspendLayout();
        pnlFooter.SuspendLayout();
        pnlBody.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)_nudPax).BeginInit();
        SuspendLayout();
        // 
        // pnlHeader
        // 
        pnlHeader.BackColor = Color.FromArgb(136, 19, 55);
        pnlHeader.Controls.Add(lblSub);
        pnlHeader.Controls.Add(lblTit);
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Location = new Point(0, 0);
        pnlHeader.Name = "pnlHeader";
        pnlHeader.Padding = new Padding(20, 12, 20, 12);
        pnlHeader.Size = new Size(524, 70);
        pnlHeader.TabIndex = 2;
        // 
        // lblSub
        // 
        lblSub.Dock = DockStyle.Top;
        lblSub.Font = new Font("Segoe UI", 9F);
        lblSub.ForeColor = Color.FromArgb(254, 205, 211);
        lblSub.Location = new Point(20, 36);
        lblSub.Name = "lblSub";
        lblSub.Size = new Size(484, 20);
        lblSub.TabIndex = 1;
        lblSub.Text = "Telefonoz edo Pertsonan jasota";
        // 
        // lblTit
        // 
        lblTit.Dock = DockStyle.Top;
        lblTit.Font = new Font("Segoe UI", 13.5F, FontStyle.Bold);
        lblTit.ForeColor = Color.White;
        lblTit.Location = new Point(20, 12);
        lblTit.Name = "lblTit";
        lblTit.Size = new Size(484, 24);
        lblTit.TabIndex = 0;
        lblTit.Text = "Erreserba Berria Sartu";
        // 
        // pnlFooter
        // 
        pnlFooter.BackColor = Color.White;
        pnlFooter.Controls.Add(_btnUtzi);
        pnlFooter.Controls.Add(_btnGorde);
        pnlFooter.Dock = DockStyle.Bottom;
        pnlFooter.Location = new Point(0, 571);
        pnlFooter.Name = "pnlFooter";
        pnlFooter.Padding = new Padding(20, 14, 20, 14);
        pnlFooter.Size = new Size(800, 70);
        pnlFooter.TabIndex = 1;
        // 
        // _btnUtzi
        // 
        _btnUtzi.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        _btnUtzi.BackColor = Color.FromArgb(241, 245, 249);
        _btnUtzi.Cursor = Cursors.Hand;
        _btnUtzi.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        _btnUtzi.FlatStyle = FlatStyle.Flat;
        _btnUtzi.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        _btnUtzi.ForeColor = Color.FromArgb(51, 65, 85);
        _btnUtzi.Location = new Point(526, 14);
        _btnUtzi.Name = "_btnUtzi";
        _btnUtzi.Size = new Size(110, 42);
        _btnUtzi.TabIndex = 1;
        _btnUtzi.Text = "Utzi";
        _btnUtzi.UseVisualStyleBackColor = false;
        // 
        // _btnGorde
        // 
        _btnGorde.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        _btnGorde.BackColor = Color.FromArgb(136, 19, 55);
        _btnGorde.Cursor = Cursors.Hand;
        _btnGorde.FlatAppearance.BorderSize = 0;
        _btnGorde.FlatStyle = FlatStyle.Flat;
        _btnGorde.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        _btnGorde.ForeColor = Color.White;
        _btnGorde.Location = new Point(646, 14);
        _btnGorde.Name = "_btnGorde";
        _btnGorde.Size = new Size(134, 42);
        _btnGorde.TabIndex = 0;
        _btnGorde.Text = "Gorde Erreserba";
        _btnGorde.UseVisualStyleBackColor = false;
        // 
        // pnlBody
        // 
        pnlBody.AutoScroll = true;
        pnlBody.BackColor = Color.White;
        pnlBody.Controls.Add(_lblErrorea);
        pnlBody.Controls.Add(_txtOharrak);
        pnlBody.Controls.Add(lblOharrak);
        pnlBody.Controls.Add(_cmbJatorria);
        pnlBody.Controls.Add(lblJatorria);
        pnlBody.Controls.Add(_cmbMahaia);
        pnlBody.Controls.Add(_nudPax);
        pnlBody.Controls.Add(lblMahaia);
        pnlBody.Controls.Add(lblPax);
        pnlBody.Controls.Add(_cmbTxanda);
        pnlBody.Controls.Add(_dtpData);
        pnlBody.Controls.Add(lblTxanda);
        pnlBody.Controls.Add(lblData);
        pnlBody.Controls.Add(_txtEmaila);
        pnlBody.Controls.Add(_txtTelefonoa);
        pnlBody.Controls.Add(lblMail);
        pnlBody.Controls.Add(lblTel);
        pnlBody.Controls.Add(_txtAbizena);
        pnlBody.Controls.Add(_txtIzena);
        pnlBody.Controls.Add(lblAbizena);
        pnlBody.Controls.Add(lblIzena);
        pnlBody.Dock = DockStyle.Fill;
        pnlBody.Location = new Point(0, 70);
        pnlBody.Name = "pnlBody";
        pnlBody.Padding = new Padding(24, 16, 24, 16);
        pnlBody.Size = new Size(524, 501);
        pnlBody.TabIndex = 0;
        // 
        // _lblErrorea
        // 
        _lblErrorea.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        _lblErrorea.ForeColor = Color.FromArgb(220, 38, 38);
        _lblErrorea.Location = new Point(18, 460);
        _lblErrorea.Name = "_lblErrorea";
        _lblErrorea.Size = new Size(486, 26);
        _lblErrorea.TabIndex = 20;
        // 
        // _txtOharrak
        // 
        _txtOharrak.Font = new Font("Segoe UI", 9.5F);
        _txtOharrak.Location = new Point(18, 396);
        _txtOharrak.Multiline = true;
        _txtOharrak.Name = "_txtOharrak";
        _txtOharrak.ScrollBars = ScrollBars.Vertical;
        _txtOharrak.Size = new Size(486, 60);
        _txtOharrak.TabIndex = 19;
        // 
        // lblOharrak
        // 
        lblOharrak.AutoSize = true;
        lblOharrak.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblOharrak.ForeColor = Color.FromArgb(51, 65, 85);
        lblOharrak.Location = new Point(18, 374);
        lblOharrak.Name = "lblOharrak";
        lblOharrak.Size = new Size(257, 15);
        lblOharrak.TabIndex = 18;
        lblOharrak.Text = "Oharrak (Alergiak, lehentasunak, etab.):";
        // 
        // _cmbJatorria
        // 
        _cmbJatorria.DropDownStyle = ComboBoxStyle.DropDownList;
        _cmbJatorria.Font = new Font("Segoe UI", 10F);
        _cmbJatorria.FormattingEnabled = true;
        _cmbJatorria.Location = new Point(18, 326);
        _cmbJatorria.Name = "_cmbJatorria";
        _cmbJatorria.Size = new Size(486, 25);
        _cmbJatorria.TabIndex = 17;
        // 
        // lblJatorria
        // 
        lblJatorria.AutoSize = true;
        lblJatorria.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblJatorria.ForeColor = Color.FromArgb(51, 65, 85);
        lblJatorria.Location = new Point(18, 304);
        lblJatorria.Name = "lblJatorria";
        lblJatorria.Size = new Size(116, 15);
        lblJatorria.TabIndex = 16;
        lblJatorria.Text = "Erreserbaren Jatorria:";
        // 
        // _cmbMahaia
        // 
        _cmbMahaia.DropDownStyle = ComboBoxStyle.DropDownList;
        _cmbMahaia.Font = new Font("Segoe UI", 10F);
        _cmbMahaia.FormattingEnabled = true;
        _cmbMahaia.Location = new Point(268, 254);
        _cmbMahaia.Name = "_cmbMahaia";
        _cmbMahaia.Size = new Size(236, 25);
        _cmbMahaia.TabIndex = 15;
        // 
        // _nudPax
        // 
        _nudPax.Font = new Font("Segoe UI", 10.5F);
        _nudPax.Location = new Point(18, 254);
        _nudPax.Maximum = new decimal(new int[] { 30, 0, 0, 0 });
        _nudPax.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        _nudPax.Name = "_nudPax";
        _nudPax.Size = new Size(236, 26);
        _nudPax.TabIndex = 14;
        _nudPax.Value = new decimal(new int[] { 2, 0, 0, 0 });
        // 
        // lblMahaia
        // 
        lblMahaia.AutoSize = true;
        lblMahaia.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblMahaia.ForeColor = Color.FromArgb(51, 65, 85);
        lblMahaia.Location = new Point(268, 232);
        lblMahaia.Name = "lblMahaia";
        lblMahaia.Size = new Size(95, 15);
        lblMahaia.TabIndex = 13;
        lblMahaia.Text = "Mahaia Hautatu:";
        // 
        // lblPax
        // 
        lblPax.AutoSize = true;
        lblPax.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblPax.ForeColor = Color.FromArgb(51, 65, 85);
        lblPax.Location = new Point(18, 232);
        lblPax.Name = "lblPax";
        lblPax.Size = new Size(144, 15);
        lblPax.TabIndex = 12;
        lblPax.Text = "Pertsona Kopurua (Pax):";
        // 
        // _cmbTxanda
        // 
        _cmbTxanda.DropDownStyle = ComboBoxStyle.DropDownList;
        _cmbTxanda.Font = new Font("Segoe UI", 10.5F);
        _cmbTxanda.FormattingEnabled = true;
        _cmbTxanda.Location = new Point(268, 182);
        _cmbTxanda.Name = "_cmbTxanda";
        _cmbTxanda.Size = new Size(236, 27);
        _cmbTxanda.TabIndex = 11;
        // 
        // _dtpData
        // 
        _dtpData.Font = new Font("Segoe UI", 10.5F);
        _dtpData.Format = DateTimePickerFormat.Short;
        _dtpData.Location = new Point(18, 182);
        _dtpData.Name = "_dtpData";
        _dtpData.Size = new Size(236, 26);
        _dtpData.TabIndex = 10;
        // 
        // lblTxanda
        // 
        lblTxanda.AutoSize = true;
        lblTxanda.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblTxanda.ForeColor = Color.FromArgb(51, 65, 85);
        lblTxanda.Location = new Point(268, 160);
        lblTxanda.Name = "lblTxanda";
        lblTxanda.Size = new Size(68, 15);
        lblTxanda.TabIndex = 9;
        lblTxanda.Text = "Txanda (*):";
        // 
        // lblData
        // 
        lblData.AutoSize = true;
        lblData.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblData.ForeColor = Color.FromArgb(51, 65, 85);
        lblData.Location = new Point(18, 160);
        lblData.Name = "lblData";
        lblData.Size = new Size(111, 15);
        lblData.TabIndex = 8;
        lblData.Text = "Erreserba Data (*):";
        // 
        // _txtEmaila
        // 
        _txtEmaila.Font = new Font("Segoe UI", 10.5F);
        _txtEmaila.Location = new Point(268, 110);
        _txtEmaila.Name = "_txtEmaila";
        _txtEmaila.Size = new Size(236, 26);
        _txtEmaila.TabIndex = 7;
        // 
        // _txtTelefonoa
        // 
        _txtTelefonoa.Font = new Font("Segoe UI", 10.5F);
        _txtTelefonoa.Location = new Point(18, 110);
        _txtTelefonoa.Name = "_txtTelefonoa";
        _txtTelefonoa.Size = new Size(236, 26);
        _txtTelefonoa.TabIndex = 6;
        // 
        // lblMail
        // 
        lblMail.AutoSize = true;
        lblMail.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblMail.ForeColor = Color.FromArgb(51, 65, 85);
        lblMail.Location = new Point(268, 88);
        lblMail.Name = "lblMail";
        lblMail.Size = new Size(110, 15);
        lblMail.TabIndex = 5;
        lblMail.Text = "Emaila (aukerakoa):";
        // 
        // lblTel
        // 
        lblTel.AutoSize = true;
        lblTel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblTel.ForeColor = Color.FromArgb(51, 65, 85);
        lblTel.Location = new Point(18, 88);
        lblTel.Name = "lblTel";
        lblTel.Size = new Size(198, 15);
        lblTel.TabIndex = 4;
        lblTel.Text = "Telefonoa (* kontaktu zuzenerako):";
        // 
        // _txtAbizena
        // 
        _txtAbizena.Font = new Font("Segoe UI", 10.5F);
        _txtAbizena.Location = new Point(268, 38);
        _txtAbizena.Name = "_txtAbizena";
        _txtAbizena.Size = new Size(236, 26);
        _txtAbizena.TabIndex = 3;
        // 
        // _txtIzena
        // 
        _txtIzena.Font = new Font("Segoe UI", 10.5F);
        _txtIzena.Location = new Point(18, 38);
        _txtIzena.Name = "_txtIzena";
        _txtIzena.Size = new Size(236, 26);
        _txtIzena.TabIndex = 2;
        // 
        // lblAbizena
        // 
        lblAbizena.AutoSize = true;
        lblAbizena.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblAbizena.ForeColor = Color.FromArgb(51, 65, 85);
        lblAbizena.Location = new Point(268, 16);
        lblAbizena.Name = "lblAbizena";
        lblAbizena.Size = new Size(54, 15);
        lblAbizena.TabIndex = 1;
        lblAbizena.Text = "Abizena:";
        // 
        // lblIzena
        // 
        lblIzena.AutoSize = true;
        lblIzena.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblIzena.ForeColor = Color.FromArgb(51, 65, 85);
        lblIzena.Location = new Point(18, 16);
        lblIzena.Name = "lblIzena";
        lblIzena.Size = new Size(125, 15);
        lblIzena.TabIndex = 0;
        lblIzena.Text = "Bezeroaren Izena (*):";
        // 
        // ErreserbaBerriaModal
        // 
        AutoScaleMode = AutoScaleMode.None;
        BackColor = Color.FromArgb(248, 250, 252);
        ClientSize = new Size(760, 620);
        Controls.Add(pnlBody);
        Controls.Add(pnlFooter);
        Controls.Add(pnlHeader);
        Font = new Font("Segoe UI", 9.5F);
        FormBorderStyle = FormBorderStyle.Sizable;
        MaximizeBox = false;
        MinimizeBox = false;
        MinimumSize = new Size(620, 520);
        Name = "ErreserbaBerriaModal";
        StartPosition = FormStartPosition.CenterParent;
        Text = "T1 Jatetxea - Erreserba Berria Sartu";
        WindowState = FormWindowState.Normal;
        pnlHeader.ResumeLayout(false);
        pnlFooter.ResumeLayout(false);
        pnlBody.ResumeLayout(false);
        pnlBody.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)_nudPax).EndInit();
        ResumeLayout(false);
    }

    private Panel pnlHeader;
    private Label lblTit;
    private Label lblSub;
    private Panel pnlFooter;
    private Button _btnUtzi;
    private Button _btnGorde;
    private Panel pnlBody;
    private Label _lblErrorea;
    private TextBox _txtOharrak;
    private Label lblOharrak;
    private ComboBox _cmbJatorria;
    private Label lblJatorria;
    private ComboBox _cmbMahaia;
    private NumericUpDown _nudPax;
    private Label lblMahaia;
    private Label lblPax;
    private ComboBox _cmbTxanda;
    private DateTimePicker _dtpData;
    private Label lblTxanda;
    private Label lblData;
    private TextBox _txtEmaila;
    private TextBox _txtTelefonoa;
    private Label lblMail;
    private Label lblTel;
    private TextBox _txtAbizena;
    private TextBox _txtIzena;
    private Label lblAbizena;
    private Label lblIzena;
}
