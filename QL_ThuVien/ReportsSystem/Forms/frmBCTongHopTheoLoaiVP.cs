using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows.Forms;

namespace QL_ThuVien
{
    public partial class frmTongHopTheoLoaiVP : Form
    {
        SqlConnection conn = new SqlConnection();
        SqlDataAdapter adapter = new SqlDataAdapter();
        DataTable dt = new DataTable();
        string sql, constr, para1;

        public frmTongHopTheoLoaiVP()
        {
            InitializeComponent();
        }

        private void btnInBC_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra định dạng ngày tháng
                if (!DateTime.TryParseExact(dtTuNgay.Text, "dd/MM/yyyy", null, DateTimeStyles.None, out DateTime tuNgay))
                {
                    MessageBox.Show("Định dạng ngày tháng 'Từ ngày' không đúng. Vui lòng nhập lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!DateTime.TryParseExact(dtDenNgay.Text, "dd/MM/yyyy", null, DateTimeStyles.None, out DateTime denNgay))
                {
                    MessageBox.Show("Định dạng ngày tháng 'Đến ngày' không đúng. Vui lòng nhập lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Kiểm tra nếu người dùng chọn loại vi phạm
                string loaiViPhamCondition = "";
                if (cboLoaiViPham.SelectedIndex > 0) // Nếu không phải "Tất cả"
                {
                    loaiViPhamCondition = $" AND vp.TenViPham = @TenViPham";
                }
                string tungay = tuNgay.ToString("yyyy-MM-dd");
                string dengay = denNgay.ToString("yyyy-MM-dd");
                // Câu lệnh SQL tổng hợp theo loại vi phạm
                sql = "SELECT " +
                      "    vp.MaViPham, " +
                      "    vp.TenViPham, " +
                      "    vp.HinhThucPhat, " +
                      "    COALESCE(COUNT(DISTINCT pm.MaDocGia), 0) AS SoNguoiViPham, " +
                      "    COALESCE(COUNT(ct.MaViPham), 0) AS SoLanViPham, " +
                      "    COALESCE(SUM(ct.NopPhat), 0) AS TongTienPhat " +
                      "FROM ViPham AS vp " +
                      "    LEFT JOIN CT_PhieuPhat AS ct ON vp.MaViPham = ct.MaViPham " +
                      "    LEFT JOIN PhieuPhat AS pp ON pp.MaPhieuPhat = ct.MaPhieuPhat " +
                      "    LEFT JOIN PhieuMuon AS pm ON pm.MaPhieuMuon = pp.MaPhieuMuon " +
                      "    LEFT JOIN DocGia AS dg ON dg.MaDocGia = pm.MaDocGia " +
                      $"WHERE (pp.NgayNopPhat IS NULL OR pp.NgayNopPhat BETWEEN '{tuNgay}' AND '{denNgay}') " +
                      loaiViPhamCondition +
                      " GROUP BY vp.MaViPham, vp.TenViPham, vp.HinhThucPhat " +
                      " ORDER BY SoLanViPham DESC, TongTienPhat DESC";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    // Thêm tham số cho câu lệnh SQL
                    cmd.ExecuteNonQuery();
                    //cmd.Parameters.AddWithValue("@TuNgay", DateTime.Parse(tuNgay.ToString("yyyy-MM-dd")));
                    //cmd.Parameters.AddWithValue("@DenNgay", DateTime.Parse(denNgay.ToString("yyyy-MM-dd")));

                    if (cboLoaiViPham.SelectedIndex > 0)
                    {
                        cmd.Parameters.AddWithValue("@TenViPham", cboLoaiViPham.Text);
                    }

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    dt.Clear();
                    adapter.Fill(dt);
                }

                // Cấu hình hiển thị báo cáo
                ReportDataSource reportDataSource = new ReportDataSource("DataSetTongHopTheoLoaiVP", dt);
                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(reportDataSource);
                reportViewer1.LocalReport.ReportEmbeddedResource = "QL_ThuVien.ReportsSystem.Reports.rptTongHopTheoLoaiVP.rdlc";

                // Lấy loại vi phạm phổ biến nhất
                string mostPopularViolation = GetMostPopularViolation();

                // Thêm tham số vào báo cáo
                para1 = "Từ ngày " + dtTuNgay.Text + " Đến ngày " + dtDenNgay.Text;
                ReportParameter[] reportParameters = new ReportParameter[]
                {
                    new ReportParameter("prThoiGian", para1),
                    new ReportParameter("prLoaiViPhamPhoBien", mostPopularViolation)
                };
                reportViewer1.LocalReport.SetParameters(reportParameters);
                reportViewer1.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo báo cáo: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetMostPopularViolation()
        {
            try
            {
                // Nếu chọn loại vi phạm cụ thể (khác "Tất cả"), trả về "."
                if (cboLoaiViPham.SelectedIndex > 0 && cboLoaiViPham.Text != "Tất cả")
                {
                    return ".";
                }

                string tungay = dtTuNgay.Value.ToString("yyyy-MM-dd");
                string denngay = dtDenNgay.Value.ToString("yyyy-MM-dd");

                // Nếu không, truy vấn loại vi phạm phổ biến nhất
                string sqlMostPopular = 
                "SELECT TOP 1 " +
                   " vp.TenViPham, " +
                    "COUNT(DISTINCT pm.MaDocGia) AS SoNguoiViPham, " +
                    "COUNT(ct.MaViPham) AS SoLanViPham " +
                "FROM ViPham AS vp " +
                "LEFT JOIN CT_PhieuPhat AS ct ON vp.MaViPham = ct.MaViPham " +
                "LEFT JOIN PhieuPhat AS pp ON pp.MaPhieuPhat = ct.MaPhieuPhat " +
                "LEFT JOIN PhieuMuon AS pm ON pm.MaPhieuMuon = pp.MaPhieuMuon " +
                "LEFT JOIN DocGia AS dg ON dg.MaDocGia = pm.MaDocGia " +
                "WHERE " +
                   $" (pp.NgayNopPhat IS NULL OR pp.NgayNopPhat BETWEEN '{tungay}' AND '{denngay}') " +
                 "GROUP BY vp.TenViPham " +
                  "ORDER BY COUNT(DISTINCT pm.MaDocGia) DESC, COUNT(ct.MaViPham) DESC";

                using (SqlCommand cmd = new SqlCommand(sqlMostPopular, conn))
                {
                    cmd.ExecuteNonQuery();
                    //cmd.Parameters.AddWithValue("@TuNgay", DateTime.Parse(dtTuNgay.Text).ToString("yyyy-MM-dd"));
                    //cmd.Parameters.AddWithValue("@DenNgay", DateTime.Parse(dtDenNgay.Text).ToString("yyyy-MM-dd"));

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        string tenViPham = dt.Rows[0]["TenViPham"].ToString();
                        int soNguoiViPham = Convert.ToInt32(dt.Rows[0]["SoNguoiViPham"]);
                        int soLanViPham = Convert.ToInt32(dt.Rows[0]["SoLanViPham"]);

                        return $"Loại vi phạm phổ biến nhất: {tenViPham} - {soNguoiViPham} người vi phạm, {soLanViPham} lần vi phạm";
                    }
                    else
                    {
                        return "Không có dữ liệu";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi lấy loại vi phạm phổ biến nhất: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "Lỗi";
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmTongHopTheoLoaiVP_Load(object sender, EventArgs e)
        {
            // Thiết lập chuỗi kết nối
            constr = @"Data Source=DESKTOP-HPGDAGQ\SQLEXPRESS;Initial Catalog=QuanLyThuVien3;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
            conn.ConnectionString = constr;
            conn.Open();

            // Gọi hàm LoadComboBox để tải dữ liệu vào ComboBox cboLoaiViPham
            LoadComboBox(cboLoaiViPham, "ViPham", "MaViPham", "TenViPham");

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

                DataRow row = dt.NewRow();
                row[Ma] = DBNull.Value;
                row[TenMa] = "Tất cả";
                dt.Rows.InsertAt(row, 0);

                cbo.DataSource = dt;
                cbo.ValueMember = Ma;
                cbo.DisplayMember = TenMa;
                cbo.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu ComboBox: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}