namespace Jatetxea.Desktop.Forms;

partial class ErreserbakForm
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
        btnBerriaTop = new Button();
        btnFreskatu = new Button();
        lblSub = new Label();
        lblTit = new Label();
        pnlMain = new Panel();
        _pnlRightDetails = new Panel();
        _pnlTurnosContainer = new FlowLayoutPanel();
        pnlSelectedDayHeader = new Panel();
        btnEgunekoErreserba = new Button();
        _lblSelectedDayStats = new Label();
        _lblSelectedDayTitle = new Label();
        pnlLeftCalendar = new Panel();
        _lblCalendarFooter = new Label();
        _pnlDayGrid = new TableLayoutPanel();
        pnlDayHeaders = new TableLayoutPanel();
        _pnlMonthPills = new FlowLayoutPanel();
        pnlMonthNav = new Panel();
        btnNextMonth = new Button();
        _lblMonthTitle = new Label();
        btnPrevMonth = new Button();
        pnlHeader.SuspendLayout();
        pnlMain.SuspendLayout();
        _pnlRightDetails.SuspendLayout();
        pnlSelectedDayHeader.SuspendLayout();
        pnlLeftCalendar.SuspendLayout();
        pnlMonthNav.SuspendLayout();
        SuspendLayout();
        // 
        // pnlHeader
        // 
        pnlHeader.BackColor = Color.FromArgb(136, 19, 55);
        pnlHeader.Controls.Add(btnBerriaTop);
        pnlHeader.Controls.Add(btnFreskatu);
        pnlHeader.Controls.Add(lblSub);
        pnlHeader.Controls.Add(lblTit);
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Location = new Point(0, 0);
        pnlHeader.Name = "pnlHeader";
        pnlHeader.Padding = new Padding(24, 12, 24, 12);
        pnlHeader.Size = new Size(1234, 72);
        pnlHeader.TabIndex = 0;
        // 
        // btnBerriaTop
        // 
        btnBerriaTop.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnBerriaTop.BackColor = Color.FromArgb(22, 163, 74);
        btnBerriaTop.Cursor = Cursors.Hand;
        btnBerriaTop.FlatAppearance.BorderSize = 0;
        btnBerriaTop.FlatStyle = FlatStyle.Flat;
        btnBerriaTop.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnBerriaTop.ForeColor = Color.White;
        btnBerriaTop.Location = new Point(1019, 14);
        btnBerriaTop.Name = "btnBerriaTop";
        btnBerriaTop.Size = new Size(190, 42);
        btnBerriaTop.TabIndex = 3;
        btnBerriaTop.Text = "+ Erreserba Berria";
        btnBerriaTop.UseVisualStyleBackColor = false;
        // 
        // btnFreskatu
        // 
        btnFreskatu.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnFreskatu.BackColor = Color.FromArgb(159, 18, 57);
        btnFreskatu.Cursor = Cursors.Hand;
        btnFreskatu.FlatAppearance.BorderColor = Color.FromArgb(225, 29, 72);
        btnFreskatu.FlatStyle = FlatStyle.Flat;
        btnFreskatu.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnFreskatu.ForeColor = Color.White;
        btnFreskatu.Location = new Point(909, 14);
        btnFreskatu.Name = "btnFreskatu";
        btnFreskatu.Size = new Size(100, 42);
        btnFreskatu.TabIndex = 2;
        btnFreskatu.Text = "Freskatu";
        btnFreskatu.UseVisualStyleBackColor = false;
        // 
        // lblSub
        // 
        lblSub.AutoSize = true;
        lblSub.Font = new Font("Segoe UI", 9.5F);
        lblSub.ForeColor = Color.FromArgb(254, 205, 211);
        lblSub.Location = new Point(25, 38);
        lblSub.Name = "lblSub";
        lblSub.Size = new Size(318, 17);
        lblSub.TabIndex = 1;
        lblSub.Text = "Telefonoz eta Pertsonan jasotako erreserbak";
        // 
        // lblTit
        // 
        lblTit.AutoSize = true;
        lblTit.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        lblTit.ForeColor = Color.White;
        lblTit.Location = new Point(24, 10);
        lblTit.Name = "lblTit";
        lblTit.Size = new Size(335, 25);
        lblTit.TabIndex = 0;
        lblTit.Text = "Erreserben Kudeaketa Zentrala (TPV)";
        // 
        // pnlMain
        // 
        pnlMain.BackColor = Color.FromArgb(248, 250, 252);
        pnlMain.Controls.Add(_pnlRightDetails);
        pnlMain.Controls.Add(pnlLeftCalendar);
        pnlMain.Dock = DockStyle.Fill;
        pnlMain.Location = new Point(0, 72);
        pnlMain.Name = "pnlMain";
        pnlMain.Size = new Size(1234, 689);
        pnlMain.TabIndex = 1;
        // 
        // _pnlRightDetails
        // 
        _pnlRightDetails.BackColor = Color.FromArgb(248, 250, 252);
        _pnlRightDetails.Controls.Add(_pnlTurnosContainer);
        _pnlRightDetails.Controls.Add(pnlSelectedDayHeader);
        _pnlRightDetails.Dock = DockStyle.Fill;
        _pnlRightDetails.Location = new Point(490, 0);
        _pnlRightDetails.Name = "_pnlRightDetails";
        _pnlRightDetails.Padding = new Padding(24, 16, 24, 20);
        _pnlRightDetails.Size = new Size(744, 689);
        _pnlRightDetails.TabIndex = 1;
        // 
        // _pnlTurnosContainer
        // 
        _pnlTurnosContainer.AutoScroll = true;
        _pnlTurnosContainer.BackColor = Color.FromArgb(248, 250, 252);
        _pnlTurnosContainer.Dock = DockStyle.Fill;
        _pnlTurnosContainer.FlowDirection = FlowDirection.TopDown;
        _pnlTurnosContainer.Location = new Point(24, 86);
        _pnlTurnosContainer.Name = "_pnlTurnosContainer";
        _pnlTurnosContainer.Padding = new Padding(0, 14, 0, 0);
        _pnlTurnosContainer.Size = new Size(696, 583);
        _pnlTurnosContainer.TabIndex = 1;
        _pnlTurnosContainer.WrapContents = false;
        // 
        // pnlSelectedDayHeader
        // 
        pnlSelectedDayHeader.BackColor = Color.White;
        pnlSelectedDayHeader.Controls.Add(btnEgunekoErreserba);
        pnlSelectedDayHeader.Controls.Add(_lblSelectedDayStats);
        pnlSelectedDayHeader.Controls.Add(_lblSelectedDayTitle);
        pnlSelectedDayHeader.Dock = DockStyle.Top;
        pnlSelectedDayHeader.Location = new Point(24, 16);
        pnlSelectedDayHeader.Name = "pnlSelectedDayHeader";
        pnlSelectedDayHeader.Padding = new Padding(20, 10, 20, 10);
        pnlSelectedDayHeader.Size = new Size(696, 70);
        pnlSelectedDayHeader.TabIndex = 0;
        // 
        // btnEgunekoErreserba
        // 
        btnEgunekoErreserba.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnEgunekoErreserba.BackColor = Color.FromArgb(136, 19, 55);
        btnEgunekoErreserba.Cursor = Cursors.Hand;
        btnEgunekoErreserba.FlatAppearance.BorderSize = 0;
        btnEgunekoErreserba.FlatStyle = FlatStyle.Flat;
        btnEgunekoErreserba.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnEgunekoErreserba.ForeColor = Color.White;
        btnEgunekoErreserba.Location = new Point(471, 16);
        btnEgunekoErreserba.Name = "btnEgunekoErreserba";
        btnEgunekoErreserba.Size = new Size(205, 38);
        btnEgunekoErreserba.TabIndex = 2;
        btnEgunekoErreserba.Text = "+ Egun honetan erreserbatu";
        btnEgunekoErreserba.UseVisualStyleBackColor = false;
        // 
        // _lblSelectedDayStats
        // 
        _lblSelectedDayStats.AutoSize = true;
        _lblSelectedDayStats.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        _lblSelectedDayStats.ForeColor = Color.FromArgb(136, 19, 55);
        _lblSelectedDayStats.Location = new Point(19, 38);
        _lblSelectedDayStats.Name = "_lblSelectedDayStats";
        _lblSelectedDayStats.Size = new Size(130, 17);
        _lblSelectedDayStats.TabIndex = 1;
        _lblSelectedDayStats.Text = "Guztira: 0 erreserba";
        // 
        // _lblSelectedDayTitle
        // 
        _lblSelectedDayTitle.AutoSize = true;
        _lblSelectedDayTitle.Font = new Font("Segoe UI", 13.5F, FontStyle.Bold);
        _lblSelectedDayTitle.ForeColor = Color.FromArgb(15, 23, 42);
        _lblSelectedDayTitle.Location = new Point(18, 10);
        _lblSelectedDayTitle.Name = "_lblSelectedDayTitle";
        _lblSelectedDayTitle.Size = new Size(140, 25);
        _lblSelectedDayTitle.TabIndex = 0;
        _lblSelectedDayTitle.Text = "📅 Egun hautatua";
        // 
        // pnlLeftCalendar
        // 
        pnlLeftCalendar.AutoScroll = true;
        pnlLeftCalendar.BackColor = Color.White;
        pnlLeftCalendar.Controls.Add(_lblCalendarFooter);
        pnlLeftCalendar.Controls.Add(_pnlDayGrid);
        pnlLeftCalendar.Controls.Add(pnlDayHeaders);
        pnlLeftCalendar.Controls.Add(_pnlMonthPills);
        pnlLeftCalendar.Controls.Add(pnlMonthNav);
        pnlLeftCalendar.Dock = DockStyle.Left;
        pnlLeftCalendar.Location = new Point(0, 0);
        pnlLeftCalendar.Name = "pnlLeftCalendar";
        pnlLeftCalendar.Padding = new Padding(14, 10, 14, 10);
        pnlLeftCalendar.Size = new Size(460, 689);
        pnlLeftCalendar.TabIndex = 0;
        // 
        // _lblCalendarFooter
        // 
        _lblCalendarFooter.Dock = DockStyle.Bottom;
        _lblCalendarFooter.Font = new Font("Segoe UI", 8.5F);
        _lblCalendarFooter.ForeColor = Color.FromArgb(100, 116, 139);
        _lblCalendarFooter.Location = new Point(16, 619);
        _lblCalendarFooter.Name = "_lblCalendarFooter";
        _lblCalendarFooter.Size = new Size(458, 58);
        _lblCalendarFooter.TabIndex = 4;
        _lblCalendarFooter.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // _pnlDayGrid
        // 
        _pnlDayGrid.BackColor = Color.White;
        _pnlDayGrid.ColumnCount = 7;
        _pnlDayGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.28571F));
        _pnlDayGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.28571F));
        _pnlDayGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.28571F));
        _pnlDayGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.28571F));
        _pnlDayGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.28571F));
        _pnlDayGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.28571F));
        _pnlDayGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.28571F));
        _pnlDayGrid.Dock = DockStyle.Top;
        _pnlDayGrid.Location = new Point(16, 130);
        _pnlDayGrid.Margin = new Padding(0);
        _pnlDayGrid.Name = "_pnlDayGrid";
        _pnlDayGrid.RowCount = 6;
        _pnlDayGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 16.66667F));
        _pnlDayGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 16.66667F));
        _pnlDayGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 16.66667F));
        _pnlDayGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 16.66667F));
        _pnlDayGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 16.66667F));
        _pnlDayGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 16.66667F));
        _pnlDayGrid.Size = new Size(458, 310);
        _pnlDayGrid.TabIndex = 3;
        // 
        // pnlDayHeaders
        // 
        pnlDayHeaders.BackColor = Color.FromArgb(241, 245, 249);
        pnlDayHeaders.ColumnCount = 7;
        pnlDayHeaders.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.28571F));
        pnlDayHeaders.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.28571F));
        pnlDayHeaders.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.28571F));
        pnlDayHeaders.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.28571F));
        pnlDayHeaders.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.28571F));
        pnlDayHeaders.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.28571F));
        pnlDayHeaders.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.28571F));
        pnlDayHeaders.Dock = DockStyle.Top;
        pnlDayHeaders.Location = new Point(16, 100);
        pnlDayHeaders.Margin = new Padding(0, 8, 0, 0);
        pnlDayHeaders.Name = "pnlDayHeaders";
        pnlDayHeaders.RowCount = 1;
        pnlDayHeaders.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        pnlDayHeaders.Size = new Size(458, 30);
        pnlDayHeaders.TabIndex = 2;
        // 
        // _pnlMonthPills
        // 
        _pnlMonthPills.BackColor = Color.FromArgb(248, 250, 252);
        _pnlMonthPills.Dock = DockStyle.Top;
        _pnlMonthPills.Location = new Point(16, 56);
        _pnlMonthPills.Name = "_pnlMonthPills";
        _pnlMonthPills.Padding = new Padding(2, 4, 2, 4);
        _pnlMonthPills.Size = new Size(458, 44);
        _pnlMonthPills.TabIndex = 1;
        _pnlMonthPills.WrapContents = false;
        // 
        // pnlMonthNav
        // 
        pnlMonthNav.BackColor = Color.White;
        pnlMonthNav.Controls.Add(btnNextMonth);
        pnlMonthNav.Controls.Add(_lblMonthTitle);
        pnlMonthNav.Controls.Add(btnPrevMonth);
        pnlMonthNav.Dock = DockStyle.Top;
        pnlMonthNav.Location = new Point(16, 12);
        pnlMonthNav.Name = "pnlMonthNav";
        pnlMonthNav.Size = new Size(458, 44);
        pnlMonthNav.TabIndex = 0;
        // 
        // btnNextMonth
        // 
        btnNextMonth.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnNextMonth.BackColor = Color.FromArgb(241, 245, 249);
        btnNextMonth.Cursor = Cursors.Hand;
        btnNextMonth.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnNextMonth.FlatStyle = FlatStyle.Flat;
        btnNextMonth.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnNextMonth.ForeColor = Color.FromArgb(71, 85, 105);
        btnNextMonth.Location = new Point(392, 4);
        btnNextMonth.Name = "btnNextMonth";
        btnNextMonth.Size = new Size(38, 36);
        btnNextMonth.TabIndex = 2;
        btnNextMonth.Text = "▶";
        btnNextMonth.UseVisualStyleBackColor = false;
        // 
        // _lblMonthTitle
        // 
        _lblMonthTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        _lblMonthTitle.Font = new Font("Segoe UI", 12.5F, FontStyle.Bold);
        _lblMonthTitle.ForeColor = Color.FromArgb(15, 23, 42);
        _lblMonthTitle.Location = new Point(46, 4);
        _lblMonthTitle.Name = "_lblMonthTitle";
        _lblMonthTitle.Size = new Size(340, 36);
        _lblMonthTitle.TabIndex = 1;
        _lblMonthTitle.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // btnPrevMonth
        // 
        btnPrevMonth.BackColor = Color.FromArgb(241, 245, 249);
        btnPrevMonth.Cursor = Cursors.Hand;
        btnPrevMonth.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnPrevMonth.FlatStyle = FlatStyle.Flat;
        btnPrevMonth.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnPrevMonth.ForeColor = Color.FromArgb(71, 85, 105);
        btnPrevMonth.Location = new Point(2, 4);
        btnPrevMonth.Name = "btnPrevMonth";
        btnPrevMonth.Size = new Size(38, 36);
        btnPrevMonth.TabIndex = 0;
        btnPrevMonth.Text = "◀";
        btnPrevMonth.UseVisualStyleBackColor = false;
        // 
        // ErreserbakForm
        // 
        AutoScaleMode = AutoScaleMode.None;
        BackColor = Color.FromArgb(248, 250, 252);
        ClientSize = new Size(1234, 761);
        Controls.Add(pnlMain);
        Controls.Add(pnlHeader);
        Font = new Font("Segoe UI", 9.5F);
        FormBorderStyle = FormBorderStyle.Sizable;
        MaximizeBox = true;
        MinimizeBox = true;
        MinimumSize = new Size(800, 500);
        Name = "ErreserbakForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "T1 Jatetxea - Erreserben Kudeaketa Zentrala (TPV)";
        WindowState = FormWindowState.Maximized;
        pnlHeader.ResumeLayout(false);
        pnlHeader.PerformLayout();
        pnlMain.ResumeLayout(false);
        _pnlRightDetails.ResumeLayout(false);
        pnlSelectedDayHeader.ResumeLayout(false);
        pnlSelectedDayHeader.PerformLayout();
        pnlLeftCalendar.ResumeLayout(false);
        pnlMonthNav.ResumeLayout(false);
        ResumeLayout(false);
    }

    private Panel pnlHeader;
    private Label lblTit;
    private Label lblSub;
    private Button btnFreskatu;
    private Button btnBerriaTop;
    private Panel pnlMain;
    private Panel pnlLeftCalendar;
    private Panel pnlMonthNav;
    private Button btnPrevMonth;
    private Label _lblMonthTitle;
    private Button btnNextMonth;
    private FlowLayoutPanel _pnlMonthPills;
    private TableLayoutPanel pnlDayHeaders;
    private TableLayoutPanel _pnlDayGrid;
    private Label _lblCalendarFooter;
    private Panel _pnlRightDetails;
    private Panel pnlSelectedDayHeader;
    private Label _lblSelectedDayTitle;
    private Label _lblSelectedDayStats;
    private Button btnEgunekoErreserba;
    private FlowLayoutPanel _pnlTurnosContainer;
}
