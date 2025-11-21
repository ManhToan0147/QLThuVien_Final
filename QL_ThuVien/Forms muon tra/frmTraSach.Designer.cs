namespace QL_ThuVien
{
    partial class frmTraSach
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lblNXB = new System.Windows.Forms.Label();
            this.grbPhieuMuon = new System.Windows.Forms.GroupBox();
            this.txtMaDG = new Guna.UI2.WinForms.Guna2TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtMaPhieuMuon = new Guna.UI2.WinForms.Guna2TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.grbSachMuon = new System.Windows.Forms.GroupBox();
            this.dgvSachMuon = new System.Windows.Forms.DataGridView();
            this.MaSach = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TienCoc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DaTraSach = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.TinhTrangMuon = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.grbSachTra = new System.Windows.Forms.GroupBox();
            this.dgvSachTra = new System.Windows.Forms.DataGridView();
            this.MaSach2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TinhTrangTra = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnChuyen = new Guna.UI2.WinForms.Guna2Button();
            this.btnXoa = new Guna.UI2.WinForms.Guna2Button();
            this.btnLuu = new Guna.UI2.WinForms.Guna2Button();
            this.grbPhieuMuon.SuspendLayout();
            this.grbSachMuon.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSachMuon)).BeginInit();
            this.grbSachTra.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSachTra)).BeginInit();
            this.SuspendLayout();
            // 
            // lblNXB
            // 
            this.lblNXB.AutoSize = true;
            this.lblNXB.Font = new System.Drawing.Font("Segoe UI", 20.14286F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNXB.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(76)))), ((int)(((byte)(170)))));
            this.lblNXB.Location = new System.Drawing.Point(575, 27);
            this.lblNXB.Name = "lblNXB";
            this.lblNXB.Size = new System.Drawing.Size(434, 65);
            this.lblNXB.TabIndex = 31;
            this.lblNXB.Text = "TRẢ SÁCH MƯỢN";
            this.lblNXB.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // grbPhieuMuon
            // 
            this.grbPhieuMuon.Controls.Add(this.txtMaDG);
            this.grbPhieuMuon.Controls.Add(this.label3);
            this.grbPhieuMuon.Controls.Add(this.txtMaPhieuMuon);
            this.grbPhieuMuon.Controls.Add(this.label2);
            this.grbPhieuMuon.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grbPhieuMuon.ForeColor = System.Drawing.Color.Black;
            this.grbPhieuMuon.Location = new System.Drawing.Point(52, 133);
            this.grbPhieuMuon.Name = "grbPhieuMuon";
            this.grbPhieuMuon.Size = new System.Drawing.Size(1477, 121);
            this.grbPhieuMuon.TabIndex = 30;
            this.grbPhieuMuon.TabStop = false;
            this.grbPhieuMuon.Text = "Phiếu mượn";
            // 
            // txtMaDG
            // 
            this.txtMaDG.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtMaDG.DefaultText = "";
            this.txtMaDG.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtMaDG.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtMaDG.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtMaDG.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtMaDG.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtMaDG.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMaDG.ForeColor = System.Drawing.Color.Black;
            this.txtMaDG.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtMaDG.Location = new System.Drawing.Point(904, 42);
            this.txtMaDG.Margin = new System.Windows.Forms.Padding(0);
            this.txtMaDG.Name = "txtMaDG";
            this.txtMaDG.PasswordChar = '\0';
            this.txtMaDG.PlaceholderText = "";
            this.txtMaDG.SelectedText = "";
            this.txtMaDG.Size = new System.Drawing.Size(346, 50);
            this.txtMaDG.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(812, 54);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 30);
            this.label3.TabIndex = 8;
            this.label3.Text = "Mã ĐG";
            // 
            // txtMaPhieuMuon
            // 
            this.txtMaPhieuMuon.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtMaPhieuMuon.DefaultText = "";
            this.txtMaPhieuMuon.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtMaPhieuMuon.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtMaPhieuMuon.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtMaPhieuMuon.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtMaPhieuMuon.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtMaPhieuMuon.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMaPhieuMuon.ForeColor = System.Drawing.Color.Black;
            this.txtMaPhieuMuon.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtMaPhieuMuon.Location = new System.Drawing.Point(322, 42);
            this.txtMaPhieuMuon.Margin = new System.Windows.Forms.Padding(0);
            this.txtMaPhieuMuon.Name = "txtMaPhieuMuon";
            this.txtMaPhieuMuon.PasswordChar = '\0';
            this.txtMaPhieuMuon.PlaceholderText = "";
            this.txtMaPhieuMuon.SelectedText = "";
            this.txtMaPhieuMuon.Size = new System.Drawing.Size(346, 50);
            this.txtMaPhieuMuon.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(226, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 30);
            this.label2.TabIndex = 6;
            this.label2.Text = "Mã PM";
            // 
            // grbSachMuon
            // 
            this.grbSachMuon.Controls.Add(this.dgvSachMuon);
            this.grbSachMuon.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grbSachMuon.ForeColor = System.Drawing.Color.Black;
            this.grbSachMuon.Location = new System.Drawing.Point(49, 293);
            this.grbSachMuon.Name = "grbSachMuon";
            this.grbSachMuon.Size = new System.Drawing.Size(770, 248);
            this.grbSachMuon.TabIndex = 32;
            this.grbSachMuon.TabStop = false;
            this.grbSachMuon.Text = "Sách đã mượn";
            // 
            // dgvSachMuon
            // 
            this.dgvSachMuon.AllowUserToAddRows = false;
            this.dgvSachMuon.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.dgvSachMuon.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvSachMuon.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvSachMuon.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvSachMuon.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(172)))), ((int)(((byte)(232)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(172)))), ((int)(((byte)(232)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSachMuon.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvSachMuon.ColumnHeadersHeight = 50;
            this.dgvSachMuon.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.MaSach,
            this.TienCoc,
            this.DaTraSach,
            this.TinhTrangMuon});
            this.dgvSachMuon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSachMuon.EnableHeadersVisualStyles = false;
            this.dgvSachMuon.Location = new System.Drawing.Point(3, 31);
            this.dgvSachMuon.Name = "dgvSachMuon";
            this.dgvSachMuon.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSachMuon.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvSachMuon.RowHeadersWidth = 72;
            this.dgvSachMuon.RowTemplate.Height = 50;
            this.dgvSachMuon.Size = new System.Drawing.Size(764, 214);
            this.dgvSachMuon.TabIndex = 0;
            // 
            // MaSach
            // 
            this.MaSach.DataPropertyName = "MaSach";
            this.MaSach.HeaderText = "Mã sách";
            this.MaSach.MinimumWidth = 9;
            this.MaSach.Name = "MaSach";
            this.MaSach.Width = 150;
            // 
            // TienCoc
            // 
            this.TienCoc.DataPropertyName = "TienCoc";
            dataGridViewCellStyle3.Format = "#,###";
            this.TienCoc.DefaultCellStyle = dataGridViewCellStyle3;
            this.TienCoc.HeaderText = "Tiền cọc";
            this.TienCoc.MinimumWidth = 9;
            this.TienCoc.Name = "TienCoc";
            this.TienCoc.Width = 150;
            // 
            // DaTraSach
            // 
            this.DaTraSach.DataPropertyName = "DaTraSach";
            this.DaTraSach.HeaderText = "Đã trả sách";
            this.DaTraSach.MinimumWidth = 9;
            this.DaTraSach.Name = "DaTraSach";
            this.DaTraSach.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.DaTraSach.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.DaTraSach.Width = 160;
            // 
            // TinhTrangMuon
            // 
            this.TinhTrangMuon.DataPropertyName = "TinhTrangMuon";
            this.TinhTrangMuon.HeaderText = "Tình trạng mượn";
            this.TinhTrangMuon.MinimumWidth = 9;
            this.TinhTrangMuon.Name = "TinhTrangMuon";
            this.TinhTrangMuon.Width = 230;
            // 
            // grbSachTra
            // 
            this.grbSachTra.Controls.Add(this.dgvSachTra);
            this.grbSachTra.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grbSachTra.ForeColor = System.Drawing.Color.Black;
            this.grbSachTra.Location = new System.Drawing.Point(963, 293);
            this.grbSachTra.Name = "grbSachTra";
            this.grbSachTra.Size = new System.Drawing.Size(569, 245);
            this.grbSachTra.TabIndex = 33;
            this.grbSachTra.TabStop = false;
            this.grbSachTra.Text = "Sách đã trả";
            // 
            // dgvSachTra
            // 
            this.dgvSachTra.AllowUserToAddRows = false;
            this.dgvSachTra.AllowUserToResizeRows = false;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.dgvSachTra.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvSachTra.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSachTra.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvSachTra.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvSachTra.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(172)))), ((int)(((byte)(232)))));
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(172)))), ((int)(((byte)(232)))));
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSachTra.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dgvSachTra.ColumnHeadersHeight = 50;
            this.dgvSachTra.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.MaSach2,
            this.TinhTrangTra});
            this.dgvSachTra.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSachTra.EnableHeadersVisualStyles = false;
            this.dgvSachTra.Location = new System.Drawing.Point(3, 31);
            this.dgvSachTra.Name = "dgvSachTra";
            this.dgvSachTra.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSachTra.RowHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.dgvSachTra.RowHeadersWidth = 72;
            this.dgvSachTra.RowTemplate.Height = 50;
            this.dgvSachTra.Size = new System.Drawing.Size(563, 211);
            this.dgvSachTra.TabIndex = 0;
            // 
            // MaSach2
            // 
            this.MaSach2.DataPropertyName = "MaSach";
            this.MaSach2.HeaderText = "Mã sách";
            this.MaSach2.MinimumWidth = 9;
            this.MaSach2.Name = "MaSach2";
            // 
            // TinhTrangTra
            // 
            this.TinhTrangTra.DataPropertyName = "TinhTrangTra";
            this.TinhTrangTra.HeaderText = "Tình trạng trả";
            this.TinhTrangTra.MinimumWidth = 9;
            this.TinhTrangTra.Name = "TinhTrangTra";
            // 
            // btnChuyen
            // 
            this.btnChuyen.BorderColor = System.Drawing.Color.Transparent;
            this.btnChuyen.BorderThickness = 2;
            this.btnChuyen.CheckedState.FillColor = System.Drawing.Color.White;
            this.btnChuyen.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnChuyen.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnChuyen.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnChuyen.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnChuyen.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(172)))), ((int)(((byte)(232)))));
            this.btnChuyen.Font = new System.Drawing.Font("Segoe UI", 9.857143F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnChuyen.ForeColor = System.Drawing.Color.White;
            this.btnChuyen.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(172)))), ((int)(((byte)(232)))));
            this.btnChuyen.HoverState.FillColor = System.Drawing.Color.White;
            this.btnChuyen.HoverState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(172)))), ((int)(((byte)(232)))));
            this.btnChuyen.Location = new System.Drawing.Point(842, 400);
            this.btnChuyen.Name = "btnChuyen";
            this.btnChuyen.Size = new System.Drawing.Size(109, 63);
            this.btnChuyen.TabIndex = 34;
            this.btnChuyen.Text = ">>";
            this.btnChuyen.Click += new System.EventHandler(this.btnChuyen_Click);
            // 
            // btnXoa
            // 
            this.btnXoa.BorderColor = System.Drawing.Color.Transparent;
            this.btnXoa.BorderThickness = 2;
            this.btnXoa.CheckedState.FillColor = System.Drawing.Color.White;
            this.btnXoa.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnXoa.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnXoa.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnXoa.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnXoa.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(172)))), ((int)(((byte)(232)))));
            this.btnXoa.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnXoa.ForeColor = System.Drawing.Color.White;
            this.btnXoa.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(172)))), ((int)(((byte)(232)))));
            this.btnXoa.HoverState.FillColor = System.Drawing.Color.White;
            this.btnXoa.HoverState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(172)))), ((int)(((byte)(232)))));
            this.btnXoa.Location = new System.Drawing.Point(1189, 570);
            this.btnXoa.Name = "btnXoa";
            this.btnXoa.Size = new System.Drawing.Size(145, 63);
            this.btnXoa.TabIndex = 35;
            this.btnXoa.Text = "Xóa";
            this.btnXoa.Click += new System.EventHandler(this.btnXoa_Click);
            // 
            // btnLuu
            // 
            this.btnLuu.BorderColor = System.Drawing.Color.Transparent;
            this.btnLuu.BorderThickness = 2;
            this.btnLuu.CheckedState.FillColor = System.Drawing.Color.White;
            this.btnLuu.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnLuu.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnLuu.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnLuu.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnLuu.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(172)))), ((int)(((byte)(232)))));
            this.btnLuu.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLuu.ForeColor = System.Drawing.Color.White;
            this.btnLuu.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(172)))), ((int)(((byte)(232)))));
            this.btnLuu.HoverState.FillColor = System.Drawing.Color.White;
            this.btnLuu.HoverState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(172)))), ((int)(((byte)(232)))));
            this.btnLuu.Location = new System.Drawing.Point(1384, 570);
            this.btnLuu.Name = "btnLuu";
            this.btnLuu.Size = new System.Drawing.Size(145, 63);
            this.btnLuu.TabIndex = 36;
            this.btnLuu.Text = "Lưu";
            this.btnLuu.Click += new System.EventHandler(this.btnLuu_Click);
            // 
            // frmTraSach
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 30F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AliceBlue;
            this.ClientSize = new System.Drawing.Size(1585, 659);
            this.Controls.Add(this.btnLuu);
            this.Controls.Add(this.btnXoa);
            this.Controls.Add(this.btnChuyen);
            this.Controls.Add(this.grbSachTra);
            this.Controls.Add(this.grbSachMuon);
            this.Controls.Add(this.lblNXB);
            this.Controls.Add(this.grbPhieuMuon);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frmTraSach";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmTraSach";
            this.Load += new System.EventHandler(this.frmTraSach_Load);
            this.grbPhieuMuon.ResumeLayout(false);
            this.grbPhieuMuon.PerformLayout();
            this.grbSachMuon.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSachMuon)).EndInit();
            this.grbSachTra.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSachTra)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblNXB;
        private System.Windows.Forms.GroupBox grbPhieuMuon;
        private System.Windows.Forms.GroupBox grbSachMuon;
        private System.Windows.Forms.GroupBox grbSachTra;
        private System.Windows.Forms.DataGridView dgvSachMuon;
        private System.Windows.Forms.DataGridView dgvSachTra;
        private Guna.UI2.WinForms.Guna2Button btnChuyen;
        private Guna.UI2.WinForms.Guna2Button btnXoa;
        private Guna.UI2.WinForms.Guna2TextBox txtMaDG;
        private System.Windows.Forms.Label label3;
        private Guna.UI2.WinForms.Guna2TextBox txtMaPhieuMuon;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridViewTextBoxColumn MaSach2;
        private System.Windows.Forms.DataGridViewTextBoxColumn TinhTrangTra;
        private Guna.UI2.WinForms.Guna2Button btnLuu;
        private System.Windows.Forms.DataGridViewTextBoxColumn MaSach;
        private System.Windows.Forms.DataGridViewTextBoxColumn TienCoc;
        private System.Windows.Forms.DataGridViewCheckBoxColumn DaTraSach;
        private System.Windows.Forms.DataGridViewTextBoxColumn TinhTrangMuon;
    }
}