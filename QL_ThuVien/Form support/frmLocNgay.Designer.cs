namespace QL_ThuVien.Form_support
{
    partial class frmLocNgay
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnNgungApDung = new Guna.UI2.WinForms.Guna2Button();
            this.btnApDung = new Guna.UI2.WinForms.Guna2Button();
            this.dtpDenNgay = new Guna.UI2.WinForms.Guna2DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.dtpTuNgay = new Guna.UI2.WinForms.Guna2DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.btnThoat = new System.Windows.Forms.Button();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnNgungApDung);
            this.panel2.Controls.Add(this.btnApDung);
            this.panel2.Controls.Add(this.dtpDenNgay);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.dtpTuNgay);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 87);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(719, 350);
            this.panel2.TabIndex = 4;
            // 
            // btnNgungApDung
            // 
            this.btnNgungApDung.BorderColor = System.Drawing.Color.Transparent;
            this.btnNgungApDung.BorderThickness = 2;
            this.btnNgungApDung.CheckedState.FillColor = System.Drawing.Color.White;
            this.btnNgungApDung.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnNgungApDung.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnNgungApDung.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnNgungApDung.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnNgungApDung.FillColor = System.Drawing.Color.Black;
            this.btnNgungApDung.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNgungApDung.ForeColor = System.Drawing.Color.White;
            this.btnNgungApDung.HoverState.BorderColor = System.Drawing.Color.Black;
            this.btnNgungApDung.HoverState.FillColor = System.Drawing.Color.White;
            this.btnNgungApDung.HoverState.ForeColor = System.Drawing.Color.Black;
            this.btnNgungApDung.Location = new System.Drawing.Point(187, 239);
            this.btnNgungApDung.Name = "btnNgungApDung";
            this.btnNgungApDung.Size = new System.Drawing.Size(177, 55);
            this.btnNgungApDung.TabIndex = 44;
            this.btnNgungApDung.Text = "Ngừng áp dụng";
            this.btnNgungApDung.Click += new System.EventHandler(this.btnNgungApDung_Click);
            // 
            // btnApDung
            // 
            this.btnApDung.BorderColor = System.Drawing.Color.Transparent;
            this.btnApDung.BorderThickness = 2;
            this.btnApDung.CheckedState.FillColor = System.Drawing.Color.White;
            this.btnApDung.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnApDung.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnApDung.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnApDung.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnApDung.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(172)))), ((int)(((byte)(232)))));
            this.btnApDung.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnApDung.ForeColor = System.Drawing.Color.White;
            this.btnApDung.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(172)))), ((int)(((byte)(232)))));
            this.btnApDung.HoverState.FillColor = System.Drawing.Color.White;
            this.btnApDung.HoverState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(172)))), ((int)(((byte)(232)))));
            this.btnApDung.Location = new System.Drawing.Point(400, 239);
            this.btnApDung.Name = "btnApDung";
            this.btnApDung.Size = new System.Drawing.Size(177, 55);
            this.btnApDung.TabIndex = 43;
            this.btnApDung.Text = "Áp dụng";
            this.btnApDung.Click += new System.EventHandler(this.btnApDung_Click);
            // 
            // dtpDenNgay
            // 
            this.dtpDenNgay.BackColor = System.Drawing.Color.White;
            this.dtpDenNgay.BorderColor = System.Drawing.Color.Silver;
            this.dtpDenNgay.BorderThickness = 1;
            this.dtpDenNgay.Checked = true;
            this.dtpDenNgay.CheckedState.FillColor = System.Drawing.Color.White;
            this.dtpDenNgay.CustomFormat = "dd/MM/yyyy";
            this.dtpDenNgay.FillColor = System.Drawing.Color.White;
            this.dtpDenNgay.FocusedColor = System.Drawing.Color.White;
            this.dtpDenNgay.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpDenNgay.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDenNgay.Location = new System.Drawing.Point(258, 133);
            this.dtpDenNgay.MaxDate = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this.dtpDenNgay.MinDate = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this.dtpDenNgay.Name = "dtpDenNgay";
            this.dtpDenNgay.Size = new System.Drawing.Size(319, 50);
            this.dtpDenNgay.TabIndex = 42;
            this.dtpDenNgay.Value = new System.DateTime(2025, 11, 1, 0, 0, 0, 0);
            this.dtpDenNgay.ValueChanged += new System.EventHandler(this.dtpDenNgay_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(142, 145);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 25);
            this.label2.TabIndex = 41;
            this.label2.Text = "Đến ngày:";
            // 
            // dtpTuNgay
            // 
            this.dtpTuNgay.BackColor = System.Drawing.Color.White;
            this.dtpTuNgay.BorderColor = System.Drawing.Color.Silver;
            this.dtpTuNgay.BorderThickness = 1;
            this.dtpTuNgay.Checked = true;
            this.dtpTuNgay.CheckedState.FillColor = System.Drawing.Color.White;
            this.dtpTuNgay.CustomFormat = "dd/MM/yyyy";
            this.dtpTuNgay.FillColor = System.Drawing.Color.White;
            this.dtpTuNgay.FocusedColor = System.Drawing.Color.White;
            this.dtpTuNgay.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpTuNgay.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpTuNgay.Location = new System.Drawing.Point(258, 50);
            this.dtpTuNgay.MaxDate = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this.dtpTuNgay.MinDate = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this.dtpTuNgay.Name = "dtpTuNgay";
            this.dtpTuNgay.Size = new System.Drawing.Size(319, 50);
            this.dtpTuNgay.TabIndex = 40;
            this.dtpTuNgay.Value = new System.DateTime(2025, 11, 1, 0, 0, 0, 0);
            this.dtpTuNgay.ValueChanged += new System.EventHandler(this.dtpTuNgay_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(142, 63);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(87, 25);
            this.label5.TabIndex = 39;
            this.label5.Text = "Từ ngày:";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.Controls.Add(this.btnThoat);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(719, 87);
            this.panel1.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label1.Location = new System.Drawing.Point(30, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(352, 45);
            this.label1.TabIndex = 0;
            this.label1.Text = "LỌC THEO THỜI GIAN";
            // 
            // btnThoat
            // 
            this.btnThoat.BackColor = System.Drawing.Color.Transparent;
            this.btnThoat.BackgroundImage = global::QL_ThuVien.Properties.Resources.close_white;
            this.btnThoat.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnThoat.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnThoat.FlatAppearance.BorderSize = 0;
            this.btnThoat.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnThoat.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnThoat.ForeColor = System.Drawing.Color.Transparent;
            this.btnThoat.Location = new System.Drawing.Point(643, 21);
            this.btnThoat.Name = "btnThoat";
            this.btnThoat.Size = new System.Drawing.Size(45, 45);
            this.btnThoat.TabIndex = 3;
            this.btnThoat.UseVisualStyleBackColor = false;
            this.btnThoat.Click += new System.EventHandler(this.btnThoat_Click);
            // 
            // frmLocNgay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(719, 437);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "frmLocNgay";
            this.Text = "frmLocNgay";
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private Guna.UI2.WinForms.Guna2Button btnNgungApDung;
        private Guna.UI2.WinForms.Guna2Button btnApDung;
        private Guna.UI2.WinForms.Guna2DateTimePicker dtpDenNgay;
        private System.Windows.Forms.Label label2;
        private Guna.UI2.WinForms.Guna2DateTimePicker dtpTuNgay;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnThoat;
        private System.Windows.Forms.Label label1;
    }
}