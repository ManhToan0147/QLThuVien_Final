using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

namespace QL_ThuVien
{
    public partial class frmBCPhPhatTheoDG : Form
    {
        SqlConnection conn = new SqlConnection();
        SqlDataAdapter adapter = new SqlDataAdapter();
        DataTable dt = new DataTable();
        string sql, constr, para1;

        public frmBCPhPhatTheoDG()
        {
            InitializeComponent();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmBCPhPhatTheoDG_Load(object sender, EventArgs e)
        {
            // Thiết lập chuỗi kết nối
            constr = @"Data Source=DESKTOP-HPGDAGQ\SQLEXPRESS;Initial Catalog=QuanLyThuVien3;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
            conn.ConnectionString = constr;
            conn.Open();

            // Làm mới báo cáo
            this.reportViewer1.RefreshReport();
        }

        private void btnInBC_Click(object sender, EventArgs e)
        {
            // Câu lệnh SQL để lấy dữ liệu chính
            string sql = "SELECT " +
                         "dg.MaDocGia, dg.HoTen, pp.MaPhieuPhat, vp.TenViPham, " +
                         "ctp.MaSach, ds.TenDauSach, pp.NgayNopPhat, ctp.NopPhat, pp.MaThuThu " +
                         "FROM PhieuPhat AS pp " +
                         "JOIN CT_PhieuPhat AS ctp ON pp.MaPhieuPhat = ctp.MaPhieuPhat " +
                         "JOIN PhieuMuon AS pm ON pp.MaPhieuMuon = pm.MaPhieuMuon " +
                         "JOIN DocGia AS dg ON pm.MaDocGia = dg.MaDocGia " +
                         "JOIN CuonSach AS cs ON ctp.MaSach = cs.MaSach " +
                         "JOIN DauSach AS ds ON cs.MaDauSach = ds.MaDauSach " +
                         "JOIN ViPham AS vp ON ctp.MaViPham = vp.MaViPham " +
                         $"WHERE pp.NgayNopPhat BETWEEN CONVERT(date, '{dtTuNgay.Text}', 103) AND CONVERT(date, '{dtDenNgay.Text}', 103) " +
                         "ORDER BY dg.MaDocGia";

            // Thực hiện truy vấn chính
            SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
            dt.Clear();
            adapter.Fill(dt);

            // Đưa dữ liệu vào báo cáo
            ReportDataSource reportDataSource = new ReportDataSource("DataSetPhPhatTheoDG", dt);
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(reportDataSource);
            reportViewer1.LocalReport.ReportEmbeddedResource = "QL_ThuVien.ReportsSystem.Reports.rptPhPhatTheoDG.rdlc";

            // Gọi hàm lấy Top 5 độc giả
            string top5DocGia = GetTop5DocGia();

            // Thiết lập tham số cho báo cáo
            string para1 = "Từ ngày " + dtTuNgay.Text + " Đến ngày " + dtDenNgay.Text;
            ReportParameter[] reportParameters = new ReportParameter[]
            {
                new ReportParameter("prThoiGian", para1), // Thời gian
                   new ReportParameter("prTop5DocGia", top5DocGia) // Top 5 độc giả
            };
            reportViewer1.LocalReport.SetParameters(reportParameters);

            // Làm mới báo cáo
            reportViewer1.RefreshReport();
        }


        // Hàm lấy Top 5 độc giả vi phạm nhiều nhất
        // Hàm lấy Top 5 độc giả vi phạm nhiều nhất và tổng số tiền nộp phạt
        private string GetTop5DocGia()
        {
            try
            {
                string sqlTop5DocGia = "SELECT TOP 5 dg.HoTen, COUNT(ctp.MaViPham) AS SoLanViPham, SUM(ctp.NopPhat) AS TongTienPhat " +
                                       "FROM DocGia AS dg " +
                                       "JOIN PhieuMuon AS pm ON dg.MaDocGia = pm.MaDocGia " +
                                       "JOIN PhieuPhat AS pp ON pm.MaPhieuMuon = pp.MaPhieuMuon " +
                                       "JOIN CT_PhieuPhat AS ctp ON pp.MaPhieuPhat = ctp.MaPhieuPhat " +
                                       $"WHERE pp.NgayNopPhat BETWEEN CONVERT(date, '{dtTuNgay.Text}', 103) AND CONVERT(date, '{dtDenNgay.Text}', 103) " +
                                       "GROUP BY dg.HoTen " +
                                       "ORDER BY SoLanViPham DESC, TongTienPhat DESC";

                SqlDataAdapter adapter = new SqlDataAdapter(sqlTop5DocGia, conn);
                DataTable dtTop5 = new DataTable();
                adapter.Fill(dtTop5);

                if (dtTop5.Rows.Count > 0)
                {
                    StringBuilder top5DocGia = new StringBuilder("Top 5 độc giả vi phạm nhiều nhất:\n");
                    foreach (DataRow row in dtTop5.Rows)
                    {
                        top5DocGia.AppendLine($"- {row["HoTen"]}: {row["SoLanViPham"]} lần, nộp {row["TongTienPhat"]} VND");
                    }
                    return top5DocGia.ToString();
                }
                else
                {
                    return "Không có dữ liệu";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lấy top 5 độc giả: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "Lỗi";
            }
        }
    }
}
