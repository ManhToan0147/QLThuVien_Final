using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QL_ThuVien
{
    public partial class frmBCDGHetHan : Form
    {
        SqlConnection conn = new SqlConnection();
        SqlDataAdapter adapter = new SqlDataAdapter();
        DataTable dt = new DataTable();
        string sql, constr, para1;

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public frmBCDGHetHan()
        {
            InitializeComponent();
        }

        private void btnInBC_Click(object sender, EventArgs e)
        {
            // Điều kiện lọc nghề nghiệp
            string ngheNghiepCondition = "";
            if (cboNgheNghiep.SelectedIndex > 0 && cboNgheNghiep.Text != "Tất cả")
            {
                ngheNghiepCondition = $" AND NgheNghiep = N'{cboNgheNghiep.Text}'";
            }

            // Câu lệnh SQL với điều kiện lọc nghề nghiệp
            sql = $"SELECT MaDocGia, HoTen, NgheNghiep, NgayCapThe, NgayHanThe, " +
                  $"DATEDIFF(DAY, GETDATE(), NgayHanThe) AS SoNgayConLai " +
                  $"FROM DocGia " +
                  $"WHERE NgayHanThe BETWEEN CONVERT(date, '{dtTuNgay.Text}', 103) AND CONVERT(date, '{dtDenNgay.Text}', 103) " +
                  ngheNghiepCondition + // Thêm điều kiện nghề nghiệp
                  $" ORDER BY NgayHanThe";

            // Truy vấn dữ liệu
            adapter = new SqlDataAdapter(sql, conn);
            dt.Clear();
            adapter.Fill(dt);

            // Đưa dữ liệu vào báo cáo
            ReportDataSource reportDataSource = new ReportDataSource("DataSetDGHetHan", dt);
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(reportDataSource);
            reportViewer1.LocalReport.ReportEmbeddedResource = "QL_ThuVien.ReportsSystem.Reports.rptDGHetHan.rdlc";

            // Thêm tham số thời gian cho báo cáo
            para1 = "Từ ngày " + dtTuNgay.Text + " Đến ngày " + dtDenNgay.Text;
            ReportParameter[] reportParameters = new ReportParameter[]
            {
        new ReportParameter("prThoiGian", para1)
            };
            reportViewer1.LocalReport.SetParameters(reportParameters);
            reportViewer1.RefreshReport();
        }



        private void frmDGHetHan_Load(object sender, EventArgs e)
        {
            constr = @"Data Source=DESKTOP-HPGDAGQ\SQLEXPRESS;Initial Catalog=QuanLyThuVien3;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
            conn.ConnectionString = constr;
            conn.Open();

            // Tải dữ liệu nghề nghiệp vào ComboBox
            LoadComboBox(cboNgheNghiep, "DocGia", "NgheNghiep");

            // Làm mới báo cáo
            this.reportViewer1.RefreshReport();
        }

        private void LoadComboBox(ComboBox cbo, string tableName, string columnName)
        {
            try
            {
                // Truy vấn để lấy danh sách nghề nghiệp
                SqlDataAdapter adapter = new SqlDataAdapter($"SELECT DISTINCT {columnName} FROM {tableName}", conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                // Thêm dòng "Tất cả" vào đầu danh sách
                DataRow row = dt.NewRow();
                row[columnName] = "Tất cả"; // Gán giá trị "Tất cả"
                dt.Rows.InsertAt(row, 0);

                // Gán dữ liệu cho ComboBox
                cbo.DataSource = dt;
                cbo.ValueMember = columnName;
                cbo.DisplayMember = columnName;
                cbo.SelectedIndex = 0; // Mặc định chọn "Tất cả"
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu ComboBox: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
