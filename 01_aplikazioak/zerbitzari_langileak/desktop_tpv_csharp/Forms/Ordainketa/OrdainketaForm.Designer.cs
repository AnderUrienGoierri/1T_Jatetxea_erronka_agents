namespace Jatetxea.Desktop.Forms;

partial class OrdainketaForm
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
        lblBezInfo = new Label();
        lblTotala = new Label();
        lblSub = new Label();
        lblTit = new Label();
        pnlTabs = new Panel();
        _btnBizumTab = new Button();
        _btnTxartelaTab = new Button();
        _btnEskudiruaTab = new Button();
        pnlBody = new Panel();
        _pnlBizum = new Panel();
        lblBizumInfo = new Label();
        btnPresetBizum = new Button();
        _lblBizumValid = new Label();
        _txtBizumTelefonoa = new TextBox();
        lblBizumTit = new Label();
        _pnlTxartela = new Panel();
        lblSegurtasuna = new Label();
        btnPresetMastercard = new Button();
        btnPresetVisa = new Button();
        _txtTitularra = new TextBox();
        lblTitularTit = new Label();
        _txtCvv = new TextBox();
        lblCvvTit = new Label();
        _txtIraungitzea = new TextBox();
        lblIraungTit = new Label();
        _lblTxartelMarka = new Label();
        _txtTxartelZenbakia = new TextBox();
        lblTxartelTit = new Label();
        _pnlEskudirua = new Panel();
        pnlKanbioaKutxa = new Panel();
        _lblKanbioa = new Label();
        lblKanbioTit = new Label();
        pnlAzkarrak = new FlowLayoutPanel();
        lblEuro = new Label();
        _txtJasotakoa = new TextBox();
        lblJasotakoaTit = new Label();
        pnlBottom = new Panel();
        _btnKobratu = new Button();
        btnUtzi = new Button();
        pnlHeader.SuspendLayout();
        pnlTabs.SuspendLayout();
        pnlBody.SuspendLayout();
        _pnlBizum.SuspendLayout();
        _pnlTxartela.SuspendLayout();
        _pnlEskudirua.SuspendLayout();
        pnlKanbioaKutxa.SuspendLayout();
        pnlBottom.SuspendLayout();
        SuspendLayout();
        // 
        // pnlHeader
        // 
        pnlHeader.BackColor = Color.FromArgb(136, 19, 55);
        pnlHeader.Controls.Add(lblBezInfo);
        pnlHeader.Controls.Add(lblTotala);
        pnlHeader.Controls.Add(lblSub);
        pnlHeader.Controls.Add(lblTit);
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Location = new Point(0, 0);
        pnlHeader.Name = "pnlHeader";
        pnlHeader.Padding = new Padding(24, 16, 24, 16);
        pnlHeader.Size = new Size(634, 110);
        pnlHeader.TabIndex = 0;
        // 
        // lblBezInfo
        // 
        lblBezInfo.Font = new Font("Segoe UI", 9F);
        lblBezInfo.ForeColor = Color.FromArgb(254, 226, 226);
        lblBezInfo.Location = new Point(350, 68);
        lblBezInfo.Name = "lblBezInfo";
        lblBezInfo.Size = new Size(260, 20);
        lblBezInfo.TabIndex = 3;
        lblBezInfo.Text = "Azpitotala: 0,00 € · BEZ (%10): 0,00 €";
        lblBezInfo.TextAlign = ContentAlignment.MiddleRight;
        // 
        // lblTotala
        // 
        lblTotala.Font = new Font("Segoe UI", 26F, FontStyle.Bold);
        lblTotala.ForeColor = Color.White;
        lblTotala.Location = new Point(350, 16);
        lblTotala.Name = "lblTotala";
        lblTotala.Size = new Size(260, 48);
        lblTotala.TabIndex = 2;
        lblTotala.Text = "0,00 €";
        lblTotala.TextAlign = ContentAlignment.MiddleRight;
        // 
        // lblSub
        // 
        lblSub.AutoSize = true;
        lblSub.Font = new Font("Segoe UI", 9.5F);
        lblSub.ForeColor = Color.FromArgb(254, 226, 226);
        lblSub.Location = new Point(24, 40);
        lblSub.Name = "lblSub";
        lblSub.Size = new Size(220, 17);
        lblSub.TabIndex = 1;
        lblSub.Text = "Eskaera #0 · 0 plater · Zerbitzaria: ...";
        // 
        // lblTit
        // 
        lblTit.AutoSize = true;
        lblTit.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        lblTit.ForeColor = Color.FromArgb(254, 205, 211);
        lblTit.Location = new Point(24, 14);
        lblTit.Name = "lblTit";
        lblTit.Size = new Size(230, 21);
        lblTit.TabIndex = 0;
        lblTit.Text = "KONTUA KOBRATU · MAHAIA";
        // 
        // pnlTabs
        // 
        pnlTabs.BackColor = Color.White;
        pnlTabs.Controls.Add(_btnBizumTab);
        pnlTabs.Controls.Add(_btnTxartelaTab);
        pnlTabs.Controls.Add(_btnEskudiruaTab);
        pnlTabs.Dock = DockStyle.Top;
        pnlTabs.Location = new Point(0, 110);
        pnlTabs.Name = "pnlTabs";
        pnlTabs.Padding = new Padding(24, 6, 24, 6);
        pnlTabs.Size = new Size(634, 54);
        pnlTabs.TabIndex = 1;
        // 
        // _btnBizumTab
        // 
        _btnBizumTab.Cursor = Cursors.Hand;
        _btnBizumTab.FlatStyle = FlatStyle.Flat;
        _btnBizumTab.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        _btnBizumTab.Location = new Point(424, 7);
        _btnBizumTab.Name = "_btnBizumTab";
        _btnBizumTab.Size = new Size(190, 40);
        _btnBizumTab.TabIndex = 2;
        _btnBizumTab.Text = "BIZUM";
        _btnBizumTab.UseVisualStyleBackColor = true;
        // 
        // _btnTxartelaTab
        // 
        _btnTxartelaTab.Cursor = Cursors.Hand;
        _btnTxartelaTab.FlatStyle = FlatStyle.Flat;
        _btnTxartelaTab.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        _btnTxartelaTab.Location = new Point(224, 7);
        _btnTxartelaTab.Name = "_btnTxartelaTab";
        _btnTxartelaTab.Size = new Size(190, 40);
        _btnTxartelaTab.TabIndex = 1;
        _btnTxartelaTab.Text = "TXARTELA (EMV)";
        _btnTxartelaTab.UseVisualStyleBackColor = true;
        // 
        // _btnEskudiruaTab
        // 
        _btnEskudiruaTab.Cursor = Cursors.Hand;
        _btnEskudiruaTab.FlatStyle = FlatStyle.Flat;
        _btnEskudiruaTab.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        _btnEskudiruaTab.Location = new Point(24, 7);
        _btnEskudiruaTab.Name = "_btnEskudiruaTab";
        _btnEskudiruaTab.Size = new Size(190, 40);
        _btnEskudiruaTab.TabIndex = 0;
        _btnEskudiruaTab.Text = "ESKUDIRUA (CASH)";
        _btnEskudiruaTab.UseVisualStyleBackColor = true;
        // 
        // pnlBody
        // 
        pnlBody.BackColor = Color.FromArgb(248, 250, 252);
        pnlBody.Controls.Add(_pnlEskudirua);
        pnlBody.Controls.Add(_pnlTxartela);
        pnlBody.Controls.Add(_pnlBizum);
        pnlBody.Dock = DockStyle.Fill;
        pnlBody.Location = new Point(0, 164);
        pnlBody.Name = "pnlBody";
        pnlBody.Padding = new Padding(24, 18, 24, 18);
        pnlBody.Size = new Size(634, 442);
        pnlBody.TabIndex = 2;
        // 
        // _pnlBizum
        // 
        _pnlBizum.BackColor = Color.Transparent;
        _pnlBizum.Controls.Add(lblBizumInfo);
        _pnlBizum.Controls.Add(btnPresetBizum);
        _pnlBizum.Controls.Add(_lblBizumValid);
        _pnlBizum.Controls.Add(_txtBizumTelefonoa);
        _pnlBizum.Controls.Add(lblBizumTit);
        _pnlBizum.Dock = DockStyle.Fill;
        _pnlBizum.Location = new Point(24, 18);
        _pnlBizum.Name = "_pnlBizum";
        _pnlBizum.Size = new Size(586, 406);
        _pnlBizum.TabIndex = 2;
        _pnlBizum.Visible = false;
        // 
        // lblBizumInfo
        // 
        lblBizumInfo.Font = new Font("Segoe UI", 8.5F, FontStyle.Italic);
        lblBizumInfo.ForeColor = Color.FromArgb(100, 116, 139);
        lblBizumInfo.Location = new Point(10, 175);
        lblBizumInfo.Name = "lblBizumInfo";
        lblBizumInfo.Size = new Size(560, 40);
        lblBizumInfo.TabIndex = 4;
        lblBizumInfo.Text = "Bezeroak bere mugikorrean jakinarazpen bat jasoko du ordainketa onartzeko. Baieztapena berehalakoa da.";
        // 
        // btnPresetBizum
        // 
        btnPresetBizum.BackColor = Color.FromArgb(236, 253, 245);
        btnPresetBizum.Cursor = Cursors.Hand;
        btnPresetBizum.FlatAppearance.BorderColor = Color.FromArgb(167, 243, 208);
        btnPresetBizum.FlatStyle = FlatStyle.Flat;
        btnPresetBizum.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnPresetBizum.ForeColor = Color.FromArgb(4, 120, 87);
        btnPresetBizum.Location = new Point(10, 120);
        btnPresetBizum.Name = "btnPresetBizum";
        btnPresetBizum.Size = new Size(220, 36);
        btnPresetBizum.TabIndex = 3;
        btnPresetBizum.Text = "Probako Bezero Telefonoa";
        btnPresetBizum.UseVisualStyleBackColor = false;
        // 
        // _lblBizumValid
        // 
        _lblBizumValid.AutoSize = true;
        _lblBizumValid.Font = new Font("Segoe UI", 9.5F);
        _lblBizumValid.ForeColor = Color.FromArgb(22, 101, 52);
        _lblBizumValid.Location = new Point(10, 85);
        _lblBizumValid.Name = "_lblBizumValid";
        _lblBizumValid.Size = new Size(335, 17);
        _lblBizumValid.TabIndex = 2;
        _lblBizumValid.Text = "Telefono baliozkoa (Berehalako Bizum push-a bidaliko da)";
        // 
        // _txtBizumTelefonoa
        // 
        _txtBizumTelefonoa.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        _txtBizumTelefonoa.Location = new Point(10, 36);
        _txtBizumTelefonoa.Name = "_txtBizumTelefonoa";
        _txtBizumTelefonoa.Size = new Size(260, 36);
        _txtBizumTelefonoa.TabIndex = 1;
        _txtBizumTelefonoa.Text = "644 12 34 56";
        // 
        // lblBizumTit
        // 
        lblBizumTit.AutoSize = true;
        lblBizumTit.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
        lblBizumTit.ForeColor = Color.FromArgb(30, 41, 59);
        lblBizumTit.Location = new Point(10, 8);
        lblBizumTit.Name = "lblBizumTit";
        lblBizumTit.Size = new Size(257, 19);
        lblBizumTit.TabIndex = 0;
        lblBizumTit.Text = "Bezeroaren Bizum Telefono Zenbakia:";
        // 
        // _pnlTxartela
        // 
        _pnlTxartela.BackColor = Color.Transparent;
        _pnlTxartela.Controls.Add(lblSegurtasuna);
        _pnlTxartela.Controls.Add(btnPresetMastercard);
        _pnlTxartela.Controls.Add(btnPresetVisa);
        _pnlTxartela.Controls.Add(_txtTitularra);
        _pnlTxartela.Controls.Add(lblTitularTit);
        _pnlTxartela.Controls.Add(_txtCvv);
        _pnlTxartela.Controls.Add(lblCvvTit);
        _pnlTxartela.Controls.Add(_txtIraungitzea);
        _pnlTxartela.Controls.Add(lblIraungTit);
        _pnlTxartela.Controls.Add(_lblTxartelMarka);
        _pnlTxartela.Controls.Add(_txtTxartelZenbakia);
        _pnlTxartela.Controls.Add(lblTxartelTit);
        _pnlTxartela.Dock = DockStyle.Fill;
        _pnlTxartela.Location = new Point(24, 18);
        _pnlTxartela.Name = "_pnlTxartela";
        _pnlTxartela.Size = new Size(586, 406);
        _pnlTxartela.TabIndex = 1;
        _pnlTxartela.Visible = false;
        // 
        // lblSegurtasuna
        // 
        lblSegurtasuna.Font = new Font("Segoe UI", 8.5F, FontStyle.Italic);
        lblSegurtasuna.ForeColor = Color.FromArgb(100, 116, 139);
        lblSegurtasuna.Location = new Point(10, 200);
        lblSegurtasuna.Name = "lblSegurtasuna";
        lblSegurtasuna.Size = new Size(560, 40);
        lblSegurtasuna.TabIndex = 11;
        lblSegurtasuna.Text = "Zibersegurtasuna: EMV txip bidezko tokenizazioa. Txartelaren datu gordinak ez dira datu-basean gordetzen (PCI-DSS araudia).";
        // 
        // btnPresetMastercard
        // 
        btnPresetMastercard.BackColor = Color.FromArgb(254, 242, 242);
        btnPresetMastercard.Cursor = Cursors.Hand;
        btnPresetMastercard.FlatAppearance.BorderColor = Color.FromArgb(254, 202, 202);
        btnPresetMastercard.FlatStyle = FlatStyle.Flat;
        btnPresetMastercard.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnPresetMastercard.ForeColor = Color.FromArgb(153, 27, 27);
        btnPresetMastercard.Location = new Point(180, 150);
        btnPresetMastercard.Name = "btnPresetMastercard";
        btnPresetMastercard.Size = new Size(180, 34);
        btnPresetMastercard.TabIndex = 10;
        btnPresetMastercard.Text = "Mastercard Baimendua";
        btnPresetMastercard.UseVisualStyleBackColor = false;
        // 
        // btnPresetVisa
        // 
        btnPresetVisa.BackColor = Color.FromArgb(238, 242, 255);
        btnPresetVisa.Cursor = Cursors.Hand;
        btnPresetVisa.FlatAppearance.BorderColor = Color.FromArgb(199, 210, 254);
        btnPresetVisa.FlatStyle = FlatStyle.Flat;
        btnPresetVisa.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnPresetVisa.ForeColor = Color.FromArgb(49, 46, 129);
        btnPresetVisa.Location = new Point(10, 150);
        btnPresetVisa.Name = "btnPresetVisa";
        btnPresetVisa.Size = new Size(160, 34);
        btnPresetVisa.TabIndex = 9;
        btnPresetVisa.Text = "VISA Baimendua";
        btnPresetVisa.UseVisualStyleBackColor = false;
        // 
        // _txtTitularra
        // 
        _txtTitularra.Font = new Font("Segoe UI", 11F);
        _txtTitularra.Location = new Point(230, 106);
        _txtTitularra.Name = "_txtTitularra";
        _txtTitularra.Size = new Size(340, 27);
        _txtTitularra.TabIndex = 8;
        _txtTitularra.Text = "Aitor Martinez Goikoetxea";
        // 
        // lblTitularTit
        // 
        lblTitularTit.AutoSize = true;
        lblTitularTit.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblTitularTit.Location = new Point(230, 84);
        lblTitularTit.Name = "lblTitularTit";
        lblTitularTit.Size = new Size(58, 15);
        lblTitularTit.TabIndex = 7;
        lblTitularTit.Text = "Titularra:";
        // 
        // _txtCvv
        // 
        _txtCvv.Font = new Font("Segoe UI", 11F);
        _txtCvv.Location = new Point(130, 106);
        _txtCvv.Name = "_txtCvv";
        _txtCvv.Size = new Size(80, 27);
        _txtCvv.TabIndex = 6;
        _txtCvv.Text = "842";
        _txtCvv.UseSystemPasswordChar = true;
        // 
        // lblCvvTit
        // 
        lblCvvTit.AutoSize = true;
        lblCvvTit.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblCvvTit.Location = new Point(130, 84);
        lblCvvTit.Name = "lblCvvTit";
        lblCvvTit.Size = new Size(82, 15);
        lblCvvTit.TabIndex = 5;
        lblCvvTit.Text = "CVV (3 digitu):";
        // 
        // _txtIraungitzea
        // 
        _txtIraungitzea.Font = new Font("Segoe UI", 11F);
        _txtIraungitzea.Location = new Point(10, 106);
        _txtIraungitzea.Name = "_txtIraungitzea";
        _txtIraungitzea.Size = new Size(100, 27);
        _txtIraungitzea.TabIndex = 4;
        _txtIraungitzea.Text = "12/28";
        // 
        // lblIraungTit
        // 
        lblIraungTit.AutoSize = true;
        lblIraungTit.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblIraungTit.Location = new Point(10, 84);
        lblIraungTit.Name = "lblIraungTit";
        lblIraungTit.Size = new Size(125, 15);
        lblIraungTit.TabIndex = 3;
        lblIraungTit.Text = "Iraungitzea (MM/UU):";
        // 
        // _lblTxartelMarka
        // 
        _lblTxartelMarka.AutoSize = true;
        _lblTxartelMarka.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        _lblTxartelMarka.ForeColor = Color.FromArgb(29, 78, 216);
        _lblTxartelMarka.Location = new Point(360, 42);
        _lblTxartelMarka.Name = "_lblTxartelMarka";
        _lblTxartelMarka.Size = new Size(130, 20);
        _lblTxartelMarka.TabIndex = 2;
        _lblTxartelMarka.Text = "VISA (Baimendua)";
        // 
        // _txtTxartelZenbakia
        // 
        _txtTxartelZenbakia.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
        _txtTxartelZenbakia.Location = new Point(10, 36);
        _txtTxartelZenbakia.Name = "_txtTxartelZenbakia";
        _txtTxartelZenbakia.Size = new Size(340, 31);
        _txtTxartelZenbakia.TabIndex = 1;
        _txtTxartelZenbakia.Text = "4532 8921 7842 4821";
        // 
        // lblTxartelTit
        // 
        lblTxartelTit.AutoSize = true;
        lblTxartelTit.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
        lblTxartelTit.ForeColor = Color.FromArgb(30, 41, 59);
        lblTxartelTit.Location = new Point(10, 8);
        lblTxartelTit.Name = "lblTxartelTit";
        lblTxartelTit.Size = new Size(428, 19);
        lblTxartelTit.TabIndex = 0;
        lblTxartelTit.Text = "Kreditu / Zordunketa Txartelaren Datuak (EMV / Contactless):";
        // 
        // _pnlEskudirua
        // 
        _pnlEskudirua.BackColor = Color.Transparent;
        _pnlEskudirua.Controls.Add(pnlKanbioaKutxa);
        _pnlEskudirua.Controls.Add(pnlAzkarrak);
        _pnlEskudirua.Controls.Add(lblEuro);
        _pnlEskudirua.Controls.Add(_txtJasotakoa);
        _pnlEskudirua.Controls.Add(lblJasotakoaTit);
        _pnlEskudirua.Dock = DockStyle.Fill;
        _pnlEskudirua.Location = new Point(24, 18);
        _pnlEskudirua.Name = "_pnlEskudirua";
        _pnlEskudirua.Size = new Size(586, 406);
        _pnlEskudirua.TabIndex = 0;
        // 
        // pnlKanbioaKutxa
        // 
        pnlKanbioaKutxa.BackColor = Color.White;
        pnlKanbioaKutxa.Controls.Add(_lblKanbioa);
        pnlKanbioaKutxa.Controls.Add(lblKanbioTit);
        pnlKanbioaKutxa.Location = new Point(10, 160);
        pnlKanbioaKutxa.Name = "pnlKanbioaKutxa";
        pnlKanbioaKutxa.Padding = new Padding(16);
        pnlKanbioaKutxa.Size = new Size(570, 90);
        pnlKanbioaKutxa.TabIndex = 4;
        // 
        // _lblKanbioa
        // 
        _lblKanbioa.AutoSize = true;
        _lblKanbioa.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
        _lblKanbioa.ForeColor = Color.FromArgb(22, 101, 52);
        _lblKanbioa.Location = new Point(16, 36);
        _lblKanbioa.Name = "_lblKanbioa";
        _lblKanbioa.Size = new Size(108, 45);
        _lblKanbioa.TabIndex = 1;
        _lblKanbioa.Text = "0,00 €";
        // 
        // lblKanbioTit
        // 
        lblKanbioTit.AutoSize = true;
        lblKanbioTit.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblKanbioTit.ForeColor = Color.FromArgb(71, 85, 105);
        lblKanbioTit.Location = new Point(16, 12);
        lblKanbioTit.Name = "lblKanbioTit";
        lblKanbioTit.Size = new Size(318, 15);
        lblKanbioTit.TabIndex = 0;
        lblKanbioTit.Text = "BEZEROARI ITZULI BEHARREKO KANBIOA / BUELTA:";
        // 
        // pnlAzkarrak
        // 
        pnlAzkarrak.BackColor = Color.Transparent;
        pnlAzkarrak.Location = new Point(10, 96);
        pnlAzkarrak.Name = "pnlAzkarrak";
        pnlAzkarrak.Size = new Size(580, 50);
        pnlAzkarrak.TabIndex = 3;
        // 
        // lblEuro
        // 
        lblEuro.AutoSize = true;
        lblEuro.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
        lblEuro.ForeColor = Color.FromArgb(100, 116, 139);
        lblEuro.Location = new Point(256, 40);
        lblEuro.Name = "lblEuro";
        lblEuro.Size = new Size(28, 32);
        lblEuro.TabIndex = 2;
        lblEuro.Text = "€";
        // 
        // _txtJasotakoa
        // 
        _txtJasotakoa.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
        _txtJasotakoa.Location = new Point(10, 36);
        _txtJasotakoa.Name = "_txtJasotakoa";
        _txtJasotakoa.Size = new Size(240, 39);
        _txtJasotakoa.TabIndex = 1;
        _txtJasotakoa.TextAlign = HorizontalAlignment.Right;
        // 
        // lblJasotakoaTit
        // 
        lblJasotakoaTit.AutoSize = true;
        lblJasotakoaTit.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
        lblJasotakoaTit.ForeColor = Color.FromArgb(30, 41, 59);
        lblJasotakoaTit.Location = new Point(10, 8);
        lblJasotakoaTit.Name = "lblJasotakoaTit";
        lblJasotakoaTit.Size = new Size(270, 19);
        lblJasotakoaTit.TabIndex = 0;
        lblJasotakoaTit.Text = "Bezeroak emandako dirua (Jasotakoa):";
        // 
        // pnlBottom
        // 
        pnlBottom.BackColor = Color.White;
        pnlBottom.Controls.Add(_btnKobratu);
        pnlBottom.Controls.Add(btnUtzi);
        pnlBottom.Dock = DockStyle.Bottom;
        pnlBottom.Location = new Point(0, 606);
        pnlBottom.Name = "pnlBottom";
        pnlBottom.Padding = new Padding(24, 14, 24, 14);
        pnlBottom.Size = new Size(634, 75);
        pnlBottom.TabIndex = 3;
        // 
        // _btnKobratu
        // 
        _btnKobratu.BackColor = Color.FromArgb(136, 19, 55);
        _btnKobratu.Cursor = Cursors.Hand;
        _btnKobratu.FlatAppearance.BorderSize = 0;
        _btnKobratu.FlatStyle = FlatStyle.Flat;
        _btnKobratu.Font = new Font("Segoe UI", 11.5F, FontStyle.Bold);
        _btnKobratu.ForeColor = Color.White;
        _btnKobratu.Location = new Point(148, 14);
        _btnKobratu.Name = "_btnKobratu";
        _btnKobratu.Size = new Size(460, 46);
        _btnKobratu.TabIndex = 1;
        _btnKobratu.Text = "KOBRATU ETA TIKETA SORTU";
        _btnKobratu.UseVisualStyleBackColor = false;
        // 
        // btnUtzi
        // 
        btnUtzi.BackColor = Color.FromArgb(241, 245, 249);
        btnUtzi.Cursor = Cursors.Hand;
        btnUtzi.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnUtzi.FlatStyle = FlatStyle.Flat;
        btnUtzi.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        btnUtzi.ForeColor = Color.FromArgb(71, 85, 105);
        btnUtzi.Location = new Point(24, 14);
        btnUtzi.Name = "btnUtzi";
        btnUtzi.Size = new Size(110, 46);
        btnUtzi.TabIndex = 0;
        btnUtzi.Text = "Utzi";
        btnUtzi.UseVisualStyleBackColor = false;
        // 
        // OrdainketaForm
        // 
        AutoScaleMode = AutoScaleMode.None;
        BackColor = Color.FromArgb(248, 250, 252);
        ClientSize = new Size(634, 681);
        Controls.Add(pnlBody);
        Controls.Add(pnlBottom);
        Controls.Add(pnlTabs);
        Controls.Add(pnlHeader);
        Font = new Font("Segoe UI", 9.5F);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "OrdainketaForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "T1 Jatetxea - Kontua Kobratu (POS)";
        pnlHeader.ResumeLayout(false);
        pnlHeader.PerformLayout();
        pnlTabs.ResumeLayout(false);
        pnlBody.ResumeLayout(false);
        _pnlBizum.ResumeLayout(false);
        _pnlBizum.PerformLayout();
        _pnlTxartela.ResumeLayout(false);
        _pnlTxartela.PerformLayout();
        _pnlEskudirua.ResumeLayout(false);
        _pnlEskudirua.PerformLayout();
        pnlKanbioaKutxa.ResumeLayout(false);
        pnlKanbioaKutxa.PerformLayout();
        pnlBottom.ResumeLayout(false);
        ResumeLayout(false);
    }

    private Panel pnlHeader;
    private Label lblTit;
    private Label lblSub;
    private Label lblTotala;
    private Label lblBezInfo;
    private Panel pnlTabs;
    private Button _btnEskudiruaTab;
    private Button _btnTxartelaTab;
    private Button _btnBizumTab;
    private Panel pnlBody;
    private Panel _pnlEskudirua;
    private Label lblJasotakoaTit;
    private TextBox _txtJasotakoa;
    private Label lblEuro;
    private FlowLayoutPanel pnlAzkarrak;
    private Panel pnlKanbioaKutxa;
    private Label lblKanbioTit;
    private Label _lblKanbioa;
    private Panel _pnlTxartela;
    private Label lblTxartelTit;
    private TextBox _txtTxartelZenbakia;
    private Label _lblTxartelMarka;
    private Label lblIraungTit;
    private TextBox _txtIraungitzea;
    private Label lblCvvTit;
    private TextBox _txtCvv;
    private Label lblTitularTit;
    private TextBox _txtTitularra;
    private Button btnPresetVisa;
    private Button btnPresetMastercard;
    private Label lblSegurtasuna;
    private Panel _pnlBizum;
    private Label lblBizumTit;
    private TextBox _txtBizumTelefonoa;
    private Label _lblBizumValid;
    private Button btnPresetBizum;
    private Label lblBizumInfo;
    private Panel pnlBottom;
    private Button btnUtzi;
    private Button _btnKobratu;
}
