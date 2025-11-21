using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

namespace QL_ThuVien
{
    public partial class frmBCDGViPham : Form
    {
        SqlConnection conn = new SqlConnection();
        SqlDataAdapter adapter = new SqlDataAdapter();
        DataTable dt = new DataTable();
        string sql, sqlTop3DocGia, sqlTopNgheNghiep, constr, para1, paraLoaiViPham;

        public frmBCDGViPham()
        {
            InitializeComponent();
        }

        private void frmBCDGViPham_Load(object sender, EventArgs e)
        {
            // Thiết lập chuỗi kết nối
            constr = @"Data Source=DESKTOP-HPGDAGQ\SQLEXPRESS;Initial Catalog=QuanLyThuVien3;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
            conn.ConnectionString = constr;
            conn.Open();

            // Load combobox loại vi phạm
            LoadComboBox(cboLoaiViPham, "ViPham", "MaViPham", "TenViPham");

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

                // Thêm dòng "Tất cả" vào đầu bảng
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
            // Thiết lập tham số prLoaiViPham
            if (cboLoaiViPham.SelectedIndex > 0) // Nếu chọn một loại vi phạm cụ thể
            {
                paraLoaiViPham = $"Loại vi phạm: {cboLoaiViPham.Text}";
            }
            else
            {
                paraLoaiViPham = "Tất cả các loại vi phạm"; // Khi là "Tất cả", đặt một giá trị mặc định
            }

            // Thêm điều kiện lọc loại vi phạm
            string loaiViPhamCondition = "";
            if (cboLoaiViPham.SelectedIndex > 0) // Nếu không phải "Tất cả"
            {
                loaiViPhamCondition = $" AND vp.MaViPham = '{cboLoaiViPham.SelectedValue}'";
            }

            // Câu lệnh SQL báo cáo danh sách độc giả vi phạm
            sql = "SELECT " +
                  "    dg.MaDocGia, " +
                  "    dg.HoTen, " +
                  "    dg.NgheNghiep, " +
                  "    COUNT(ct.MaViPham) AS SoLanViPham, " +
                  "    SUM(ct.NopPhat) AS TongTienPhat " +
                  "FROM DocGia AS dg " +
                  "JOIN PhieuMuon AS pm ON dg.MaDocGia = pm.MaDocGia " +
                  "JOIN PhieuPhat AS pp ON pp.MaPhieuMuon = pm.MaPhieuMuon " +
                  "JOIN CT_PhieuPhat AS ct ON pp.MaPhieuPhat = ct.MaPhieuPhat " +
                  "JOIN ViPham AS vp ON vp.MaViPham = ct.MaViPham " +
                  $"WHERE (pp.NgayNopPhat IS NULL OR pp.NgayNopPhat BETWEEN CONVERT(date, '{dtTuNgay.Text}', 103) AND CONVERT(date, '{dtDenNgay.Text}', 103)) " +
                  loaiViPhamCondition +
                  "GROUP BY dg.MaDocGia, dg.HoTen, dg.NgheNghiep " +
                  "ORDER BY SoLanViPham DESC, TongTienPhat DESC";

            // Câu lệnh SQL lấy Top 3 độc giả
            sqlTop3DocGia = "SELECT TOP 3 " +
                            "    dg.HoTen, " +
                            "    COUNT(ct.MaViPham) AS SoLanViPham, " +
                            "    SUM(ct.NopPhat) AS TongTienPhat " +
                            "FROM DocGia AS dg " +
                            "JOIN PhieuMuon AS pm ON dg.MaDocGia = pm.MaDocGia " +
                            "JOIN PhieuPhat AS pp ON pp.MaPhieuMuon = pm.MaPhieuMuon " +
                            "JOIN CT_PhieuPhat AS ct ON pp.MaPhieuPhat = ct.MaPhieuPhat " +
                            "JOIN ViPham AS vp ON vp.MaViPham = ct.MaViPham " +
                            $"WHERE (pp.NgayNopPhat IS NULL OR pp.NgayNopPhat BETWEEN CONVERT(date, '{dtTuNgay.Text}', 103) AND CONVERT(date, '{dtDenNgay.Text}', 103)) " +
                            loaiViPhamCondition +
                            "GROUP BY dg.HoTen " +
                            "ORDER BY SoLanViPham DESC, TongTienPhat DESC";

            // Câu lệnh SQL lấy Nghề nghiệp vi phạm nhiều nhất
            sqlTopNgheNghiep = "SELECT TOP 1 " +
                               "    dg.NgheNghiep, " +
                               "    COUNT(ct.MaViPham) AS SoLanViPham " +
                               "FROM DocGia AS dg " +
                               "JOIN PhieuMuon AS pm ON dg.MaDocGia = pm.MaDocGia " +
                               "JOIN PhieuPhat AS pp ON pp.MaPhieuMuon = pm.MaPhieuMuon " +
                               "JOIN CT_PhieuPhat AS ct ON pp.MaPhieuPhat = ct.MaPhieuPhat " +
                               "JOIN ViPham AS vp ON vp.MaViPham = ct.MaViPham " +
                               $"WHERE (pp.NgayNopPhat IS NULL OR pp.NgayNopPhat BETWEEN CONVERT(date, '{dtTuNgay.Text}', 103) AND CONVERT(date, '{dtDenNgay.Text}', 103)) " +
                               loaiViPhamCondition +
                               "GROUP BY dg.NgheNghiep " +
                               "ORDER BY SoLanViPham DESC";

            // Thực thi câu lệnh SQL
            adapter = new SqlDataAdapter(sql, conn);
            dt.Clear();
            adapter.Fill(dt);

            // Lấy dữ liệu cho Top 3 độc giả
            string top3DocGia = GetTop3DocGia(sqlTop3DocGia);

            // Lấy dữ liệu cho Nghề nghiệp vi phạm nhiều nhất
            string topNgheNghiep = GetTopNgheNghiep(sqlTopNgheNghiep);

            // Đưa dữ liệu vào ReportViewer
            ReportDataSource reportDataSource = new ReportDataSource("DataSetDGViPham", dt);
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(reportDataSource);
            reportViewer1.LocalReport.ReportEmbeddedResource = "QL_ThuVien.ReportsSystem.Reports.rptDGViPham.rdlc";

            // Thiết lập tham số thời gian, loại vi phạm, top 3 độc giả và đối tượng vi phạm nhiều nhất
            para1 = "Từ ngày " + dtTuNgay.Text + " đến ngày " + dtDenNgay.Text;
            ReportParameter[] reportParameters = new ReportParameter[]
            {
                new ReportParameter("prThoiGian", para1),
                new ReportParameter("prLoaiViPham", paraLoaiViPham),
                new ReportParameter("prTop3DocGia", top3DocGia),
                new ReportParameter("prNgheNghiep", topNgheNghiep)
            };

            reportViewer1.LocalReport.SetParameters(reportParameters);
            reportViewer1.RefreshReport();
        }

        private string GetTop3DocGia(string query)
        {
            SqlDataAdapter top3Adapter = new SqlDataAdapter(query, conn);
            DataTable top3Table = new DataTable();
            top3Adapter.Fill(top3Table);

            StringBuilder result = new StringBuilder();
            if (top3Table.Rows.Count > 0)
            {
                result.AppendLine("Ba độc giả vi phạm nhiều nhất:");
                foreach (DataRow row in top3Table.Rows)
                {
                    result.AppendLine($"- {row["HoTen"]}: {row["SoLanViPham"]} lần, đã nộp phạt {row["TongTienPhat"]} VND");
                }
            }
            else
            {
                result.AppendLine("Không có dữ liệu phù hợp.");
            }

            return result.ToString();
        }

        private string GetTopNgheNghiep(string query)
        {
            SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                string ngheNghiep = dt.Rows[0]["NgheNghiep"].ToString();
                string soLanViPham = dt.Rows[0]["SoLanViPham"].ToString();
                return $"Đối tượng vi phạm nhiều nhất: {ngheNghiep} ({soLanViPham} lần)";
            }
            else
            {
                return "Không có dữ liệu về đối tượng vi phạm nhiều nhất.";
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
