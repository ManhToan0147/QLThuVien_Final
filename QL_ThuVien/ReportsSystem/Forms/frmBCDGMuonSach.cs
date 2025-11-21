using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

namespace QL_ThuVien
{
    public partial class frmBCDGMuonSach : Form
    {
        SqlConnection conn = new SqlConnection();
        SqlDataAdapter adapter = new SqlDataAdapter();
        DataTable dt = new DataTable();
        string sql, constr, para1;

        public frmBCDGMuonSach()
        {
            InitializeComponent();
        }

        private void frmBCDGMuonSach_Load(object sender, EventArgs e)
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
                SqlDataAdapter adapter = new SqlDataAdapter($"SELECT {Ma}, {TenMa} FROM {tableName}", conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                // Thêm dòng "Tất cả" vào đầu danh sách
                DataRow row = dt.NewRow();
                row[Ma] = DBNull.Value; // Giá trị NULL để không bị lọc
                row[TenMa] = "Tất cả"; // Hiển thị "Tất cả"
                dt.Rows.InsertAt(row, 0);

                // Gán dữ liệu cho ComboBox
                cbo.DataSource = dt;
                cbo.ValueMember = Ma; // Giá trị lưu trữ thực tế
                cbo.DisplayMember = TenMa; // Giá trị hiển thị
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
            if (cboKieuMuon.SelectedIndex > 0 && cboKieuMuon.Text != "Tất cả") // Không chọn "Tất cả"
            {
                kieuMuonCondition = $" AND km.TenKieuMuon = N'{cboKieuMuon.Text}'";
            }

            // Câu lệnh SQL chính
            sql = "SELECT pm.MaPhieuMuon, dg.HoTen, dg.MaDocGia, km.TenKieuMuon, ds.TenDauSach, pm.NgayMuon, pm.HanTra, ctpm.TienCoc " +
                  "FROM dbo.PhieuMuon AS pm " +
                  "JOIN dbo.DocGia AS dg ON pm.MaDocGia = dg.MaDocGia " +
                  "JOIN dbo.KieuMuon AS km ON km.MaKieuMuon = pm.MaKieuMuon " +
                  "JOIN dbo.CT_PhieuMuon AS ctpm ON ctpm.MaPhieuMuon = pm.MaPhieuMuon " +
                  "JOIN dbo.CuonSach AS cs ON cs.MaSach = ctpm.MaSach " +
                  "JOIN dbo.DauSach AS ds ON ds.MaDauSach = cs.MaDauSach " +
                  $"WHERE pm.NgayMuon BETWEEN CONVERT(date, '{dtTuNgay.Text}', 103) AND CONVERT(date, '{dtDenNgay.Text}', 103) " +
                  kieuMuonCondition +
                  " ORDER BY pm.NgayMuon";

            // Câu lệnh SQL lấy Top 5 độc giả
            string sqlTop5DocGia = "SELECT TOP 5 " +
                                   "    dg.HoTen, " +
                                   "    dg.MaDocGia, " +
                                   "    COUNT(ctpm.MaSach) AS SoLuongSachMuon " +
                                   "FROM dbo.DocGia AS dg " +
                                   "JOIN dbo.PhieuMuon AS pm ON dg.MaDocGia = pm.MaDocGia " +
                                   "JOIN dbo.CT_PhieuMuon AS ctpm ON pm.MaPhieuMuon = ctpm.MaPhieuMuon " +
                                   "JOIN dbo.KieuMuon AS km ON pm.MaKieuMuon = km.MaKieuMuon " +
                                   $"WHERE pm.NgayMuon BETWEEN CONVERT(date, '{dtTuNgay.Text}', 103) AND CONVERT(date, '{dtDenNgay.Text}', 103) " +
                                   $"{kieuMuonCondition} " +
                                   "GROUP BY dg.HoTen, dg.MaDocGia " +
                                   "ORDER BY SoLuongSachMuon DESC";

            // Lấy dữ liệu Top 5
            string top5DocGia = GetTop5DocGia(sqlTop5DocGia);

            // Thực thi câu lệnh SQL chính
            adapter = new SqlDataAdapter(sql, conn);
            dt.Clear();
            adapter.Fill(dt);

            // Đưa dữ liệu vào báo cáo
            ReportDataSource reportDataSource = new ReportDataSource("DataSetPMTheoDG", dt);
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(reportDataSource);
            reportViewer1.LocalReport.ReportEmbeddedResource = "QL_ThuVien.ReportsSystem.Reports.rptDSPhMuonDG.rdlc";

            // Thêm tham số cho báo cáo
            para1 = "Từ ngày " + dtTuNgay.Text + " Đến ngày " + dtDenNgay.Text;
            ReportParameter[] reportParameters = new ReportParameter[]
            {
                new ReportParameter("prThoiGian", para1),
                new ReportParameter("prTop5DocGia", top5DocGia)
            };

            reportViewer1.LocalReport.SetParameters(reportParameters);
            reportViewer1.RefreshReport();
        }

        private string GetTop5DocGia(string query)
        {
            SqlDataAdapter top5Adapter = new SqlDataAdapter(query, conn);
            DataTable top5Table = new DataTable();
            top5Adapter.Fill(top5Table);

            StringBuilder result = new StringBuilder();
            if (top5Table.Rows.Count > 0)
            {
                result.AppendLine("Năm độc giả mượn nhiều sách nhất:");
                foreach (DataRow row in top5Table.Rows)
                {
                    result.AppendLine($"- {row["HoTen"]} (Mã: {row["MaDocGia"]}): {row["SoLuongSachMuon"]} cuốn");
                }
            }
            else
            {
                result.AppendLine("Không có dữ liệu phù hợp.");
            }

            return result.ToString();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
