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
    public partial class frmBCSachMat : Form
    {
        SqlConnection conn = new SqlConnection();
        SqlDataAdapter adapter = new SqlDataAdapter();
        DataTable dt = new DataTable();
        string sql, constr, para1;

        public frmBCSachMat()
        {
            InitializeComponent();
        }

        private void frmBCSachMat_Load(object sender, EventArgs e)
        {
            // Thiết lập chuỗi kết nối
            constr = @"Data Source=DESKTOP-HPGDAGQ\SQLEXPRESS;Initial Catalog=QuanLyThuVien3;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
            conn.ConnectionString = constr;
            conn.Open();

            // Gọi hàm LoadComboBox để tải dữ liệu vào ComboBox cboKieuMuon
            LoadComboBox(cboKieuMuon, "KieuMuon", "MaKieuMuon", "TenKieuMuon");

            // Làm mới báo cáo
            this.reportViewer1.RefreshReport();
        }

        private void LoadComboBox(ComboBox cbo, string tableName, string Ma, string TenMa)
        {
            try
            {
                // Lấy dữ liệu từ bảng
                SqlDataAdapter adapter = new SqlDataAdapter($"SELECT {Ma}, {TenMa} FROM {tableName}", conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                // Thêm tùy chọn "Tất cả" vào đầu danh sách
                DataRow row = dt.NewRow();
                row[Ma] = DBNull.Value; // Giá trị NULL để không bị lọc
                row[TenMa] = "Tất cả"; // Hiển thị "Tất cả"
                dt.Rows.InsertAt(row, 0);

                // Gán dữ liệu vào ComboBox
                cbo.DataSource = dt;
                cbo.ValueMember = Ma; // Giá trị lưu trữ thực tế
                cbo.DisplayMember = TenMa; // Hiển thị
                cbo.SelectedIndex = 0; // Mặc định chọn "Tất cả"
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu ComboBox: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnInBC_Click(object sender, EventArgs e)
        {
            // Điều kiện lọc kiểu mượn
            string kieuMuonCondition = "";
            if (cboKieuMuon.SelectedIndex > 0 && cboKieuMuon.Text != "Tất cả") // Nếu không phải "Tất cả"
            {
                kieuMuonCondition = $" AND km.TenKieuMuon = N'{cboKieuMuon.Text}'";
            }

            // Câu lệnh SQL với điều kiện lọc bổ sung
            sql = "SELECT pm.MaPhieuMuon, pm.NgayThucTra, km.TenKieuMuon, ds.MaDauSach, " +
                  "ds.TenDauSach, cs.MaSach, ds.GiaBia, ds.MaNXB, ds.MaKho " +
                  "FROM CT_PhieuMuon AS ct " +
                  "JOIN PhieuMuon AS pm ON pm.MaPhieuMuon = ct.MaPhieuMuon " +
                  "JOIN CuonSach AS cs ON cs.MaSach = ct.MaSach " +
                  "JOIN DauSach AS ds ON cs.MaDauSach = ds.MaDauSach " +
                  "JOIN KieuMuon AS km ON pm.MaKieuMuon = km.MaKieuMuon " +
                  $"WHERE pm.NgayThucTra IS NOT NULL AND pm.NgayThucTra BETWEEN CONVERT(date, '{dtTuNgay.Text}', 103) " +
                  $"AND CONVERT(date, '{dtDenNgay.Text}', 103) AND ct.DaTraSach = 0 " +
                  kieuMuonCondition +
                  " ORDER BY pm.NgayThucTra";

            // Thực hiện truy vấn và lấy dữ liệu
            adapter = new SqlDataAdapter(sql, conn);
            dt.Clear();
            adapter.Fill(dt);

            // Đưa dữ liệu vào báo cáo
            ReportDataSource reportDataSource = new ReportDataSource("DataSetSachMat", dt);
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(reportDataSource);
            reportViewer1.LocalReport.ReportEmbeddedResource = "QL_ThuVien.ReportsSystem.Reports.rptSachBiMat.rdlc";

            // Thêm tham số cho báo cáo
            para1 = "Từ ngày " + dtTuNgay.Text + " Đến ngày " + dtDenNgay.Text;
            ReportParameter[] reportParameters = new ReportParameter[]
            {
                new ReportParameter("prThoiGian", para1)
            };
            reportViewer1.LocalReport.SetParameters(reportParameters);
            reportViewer1.RefreshReport();
        }
        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
