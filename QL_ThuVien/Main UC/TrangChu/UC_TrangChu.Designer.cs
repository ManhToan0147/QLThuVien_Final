namespace QL_ThuVien.Main_UC.TrangChu
{
    partial class UC_TrangChu
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend4 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            this.chartDocGia = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartNgheNghiep = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dgvTop5ChuDe = new System.Windows.Forms.DataGridView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dgvTop5DauSach = new System.Windows.Forms.DataGridView();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.lblSoPhieuTreHan = new System.Windows.Forms.Label();
            this.panel6 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.lblSachTraHu = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.lblSachMat = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.lblSachChoMuon = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chartDocGia)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartNgheNghiep)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTop5ChuDe)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTop5DauSach)).BeginInit();
            this.panel3.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // chartDocGia
            // 
            chartArea3.Name = "ChartArea1";
            this.chartDocGia.ChartAreas.Add(chartArea3);
            legend3.Name = "Legend1";
            this.chartDocGia.Legends.Add(legend3);
            this.chartDocGia.Location = new System.Drawing.Point(52, 696);
            this.chartDocGia.Name = "chartDocGia";
            series3.ChartArea = "ChartArea1";
            series3.Legend = "Legend1";
            series3.Name = "Series1";
            this.chartDocGia.Series.Add(series3);
            this.chartDocGia.Size = new System.Drawing.Size(1042, 650);
            this.chartDocGia.TabIndex = 0;
            this.chartDocGia.Text = "chart1";
            // 
            // chartNgheNghiep
            // 
            chartArea4.Name = "ChartArea1";
            this.chartNgheNghiep.ChartAreas.Add(chartArea4);
            legend4.Name = "Legend1";
            this.chartNgheNghiep.Legends.Add(legend4);
            this.chartNgheNghiep.Location = new System.Drawing.Point(1120, 696);
            this.chartNgheNghiep.Name = "chartNgheNghiep";
            series4.ChartArea = "ChartArea1";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;
            series4.Legend = "Legend1";
            series4.Name = "Series1";
            this.chartNgheNghiep.Series.Add(series4);
            this.chartNgheNghiep.Size = new System.Drawing.Size(863, 650);
            this.chartNgheNghiep.TabIndex = 1;
            this.chartNgheNghiep.Text = "chart1";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.dgvTop5ChuDe);
            this.panel1.Location = new System.Drawing.Point(1120, 32);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(863, 307);
            this.panel1.TabIndex = 2;
            // 
            // dgvTop5ChuDe
            // 
            this.dgvTop5ChuDe.AllowUserToResizeRows = false;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.dgvTop5ChuDe.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle7;
            this.dgvTop5ChuDe.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTop5ChuDe.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvTop5ChuDe.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvTop5ChuDe.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(172)))), ((int)(((byte)(232)))));
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(172)))), ((int)(((byte)(232)))));
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvTop5ChuDe.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle8;
            this.dgvTop5ChuDe.ColumnHeadersHeight = 50;
            this.dgvTop5ChuDe.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTop5ChuDe.EnableHeadersVisualStyles = false;
            this.dgvTop5ChuDe.Location = new System.Drawing.Point(0, 0);
            this.dgvTop5ChuDe.Name = "dgvTop5ChuDe";
            this.dgvTop5ChuDe.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvTop5ChuDe.RowHeadersDefaultCellStyle = dataGridViewCellStyle9;
            this.dgvTop5ChuDe.RowHeadersWidth = 72;
            this.dgvTop5ChuDe.RowTemplate.Height = 50;
            this.dgvTop5ChuDe.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dgvTop5ChuDe.Size = new System.Drawing.Size(863, 307);
            this.dgvTop5ChuDe.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.dgvTop5DauSach);
            this.panel2.Location = new System.Drawing.Point(1120, 363);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(863, 307);
            this.panel2.TabIndex = 3;
            // 
            // dgvTop5DauSach
            // 
            this.dgvTop5DauSach.AllowUserToResizeRows = false;
            dataGridViewCellStyle10.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.dgvTop5DauSach.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle10;
            this.dgvTop5DauSach.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTop5DauSach.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvTop5DauSach.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvTop5DauSach.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(172)))), ((int)(((byte)(232)))));
            dataGridViewCellStyle11.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle11.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle11.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(172)))), ((int)(((byte)(232)))));
            dataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvTop5DauSach.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle11;
            this.dgvTop5DauSach.ColumnHeadersHeight = 50;
            this.dgvTop5DauSach.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTop5DauSach.EnableHeadersVisualStyles = false;
            this.dgvTop5DauSach.Location = new System.Drawing.Point(0, 0);
            this.dgvTop5DauSach.Name = "dgvTop5DauSach";
            this.dgvTop5DauSach.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle12.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle12.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle12.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle12.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle12.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvTop5DauSach.RowHeadersDefaultCellStyle = dataGridViewCellStyle12;
            this.dgvTop5DauSach.RowHeadersWidth = 72;
            this.dgvTop5DauSach.RowTemplate.Height = 50;
            this.dgvTop5DauSach.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dgvTop5DauSach.Size = new System.Drawing.Size(863, 307);
            this.dgvTop5DauSach.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.panel7);
            this.panel3.Controls.Add(this.panel6);
            this.panel3.Controls.Add(this.panel5);
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Location = new System.Drawing.Point(52, 32);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1042, 638);
            this.panel3.TabIndex = 4;
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.Color.White;
            this.panel7.Controls.Add(this.label2);
            this.panel7.Controls.Add(this.lblSoPhieuTreHan);
            this.panel7.Location = new System.Drawing.Point(540, 331);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(502, 307);
            this.panel7.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Segoe UI", 14.14286F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(28, 169);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(399, 106);
            this.label2.TabIndex = 0;
            this.label2.Text = "Phiếu mượn trễ hạn chưa trả sách";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblSoPhieuTreHan
            // 
            this.lblSoPhieuTreHan.AutoSize = true;
            this.lblSoPhieuTreHan.Font = new System.Drawing.Font("Segoe UI", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSoPhieuTreHan.Location = new System.Drawing.Point(21, 17);
            this.lblSoPhieuTreHan.Name = "lblSoPhieuTreHan";
            this.lblSoPhieuTreHan.Size = new System.Drawing.Size(109, 128);
            this.lblSoPhieuTreHan.TabIndex = 7;
            this.lblSoPhieuTreHan.Text = "0";
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.White;
            this.panel6.Controls.Add(this.label6);
            this.panel6.Controls.Add(this.lblSachTraHu);
            this.panel6.Location = new System.Drawing.Point(540, 3);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(502, 307);
            this.panel6.TabIndex = 10;
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Segoe UI", 14.14286F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(28, 202);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(359, 52);
            this.label6.TabIndex = 0;
            this.label6.Text = "Số cuốn sách trả bị hư hỏng";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblSachTraHu
            // 
            this.lblSachTraHu.AutoSize = true;
            this.lblSachTraHu.Font = new System.Drawing.Font("Segoe UI", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSachTraHu.Location = new System.Drawing.Point(21, 16);
            this.lblSachTraHu.Name = "lblSachTraHu";
            this.lblSachTraHu.Size = new System.Drawing.Size(109, 128);
            this.lblSachTraHu.TabIndex = 5;
            this.lblSachTraHu.Text = "0";
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.White;
            this.panel5.Controls.Add(this.lblSachMat);
            this.panel5.Controls.Add(this.label3);
            this.panel5.Location = new System.Drawing.Point(3, 331);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(502, 307);
            this.panel5.TabIndex = 9;
            // 
            // lblSachMat
            // 
            this.lblSachMat.AutoSize = true;
            this.lblSachMat.Font = new System.Drawing.Font("Segoe UI", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSachMat.Location = new System.Drawing.Point(36, 17);
            this.lblSachMat.Name = "lblSachMat";
            this.lblSachMat.Size = new System.Drawing.Size(109, 128);
            this.lblSachMat.TabIndex = 6;
            this.lblSachMat.Text = "0";
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Segoe UI", 14.14286F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(26, 178);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(360, 97);
            this.label3.TabIndex = 2;
            this.label3.Text = "Số cuốn sách bị mất khi cho mượn";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.White;
            this.panel4.Controls.Add(this.label1);
            this.panel4.Controls.Add(this.lblSachChoMuon);
            this.panel4.Location = new System.Drawing.Point(3, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(502, 307);
            this.panel4.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Segoe UI", 14.14286F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(27, 202);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(359, 52);
            this.label1.TabIndex = 0;
            this.label1.Text = "Sách có thể cho mượn";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblSachChoMuon
            // 
            this.lblSachChoMuon.AutoSize = true;
            this.lblSachChoMuon.Font = new System.Drawing.Font("Segoe UI", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSachChoMuon.Location = new System.Drawing.Point(36, 16);
            this.lblSachChoMuon.Name = "lblSachChoMuon";
            this.lblSachChoMuon.Size = new System.Drawing.Size(109, 128);
            this.lblSachChoMuon.TabIndex = 4;
            this.lblSachChoMuon.Text = "0";
            // 
            // UC_TrangChu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AliceBlue;
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.chartNgheNghiep);
            this.Controls.Add(this.chartDocGia);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Black;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "UC_TrangChu";
            this.Size = new System.Drawing.Size(2035, 1379);
            this.Load += new System.EventHandler(this.UC_TrangChu_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chartDocGia)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartNgheNghiep)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTop5ChuDe)).EndInit();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTop5DauSach)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chartDocGia;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartNgheNghiep;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView dgvTop5ChuDe;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridView dgvTop5DauSach;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label lblSachChoMuon;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblSachTraHu;
        private System.Windows.Forms.Label lblSachMat;
        private System.Windows.Forms.Label lblSoPhieuTreHan;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Label label2;
    }
}
