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
using System.Windows.Forms.DataVisualization.Charting;

namespace QL_ThuVien.Main_UC.TrangChu
{
    public partial class UC_TrangChu : UserControl
    {
        string strCon = DBConfig.ConnectionString;
        SqlConnection conn;
        SqlDataAdapter adapter;
        SqlCommand cmd;
        DataTable dt;
        public UC_TrangChu()
        {
            InitializeComponent();
        }

        private void UC_TrangChu_Load(object sender, EventArgs e)
        {
            LoadChartDocGia();
            LoadChartNgheNghiep();
            LoadTop5ChuDe();
            LoadTop5DauSach();
            LoadSachChoMuon();
            LoadSachTraHu();
            LoadSachMat();
            LoadSoPhieuTreHan();
            dgvTop5ChuDe.ColumnHeadersDefaultCellStyle.Font = new Font(dgvTop5ChuDe.Font, FontStyle.Bold);
            dgvTop5DauSach.ColumnHeadersDefaultCellStyle.Font = new Font(dgvTop5DauSach.Font, FontStyle.Bold);
        }
        private void LoadSoPhieuTreHan()
        {
            using (conn = new SqlConnection(strCon))
            {
                conn.Open();
                string query = @"
                SELECT COUNT(*) AS SoLuongPhieuTreHan
                FROM PhieuMuon pm
                JOIN CT_PhieuMuon ct ON pm.MaPhieuMuon = ct.MaPhieuMuon
                WHERE pm.HanTra < GETDATE()
                  AND pm.NgayMuon >= '2024-01-01'
                  AND pm.NgayThucTra IS NULL
                ";

                // Thực thi truy vấn
                SqlCommand cmd = new SqlCommand(query, conn);
                int soPhieuTreHan = (int)cmd.ExecuteScalar();

                // Hiển thị kết quả lên Label
                lblSoPhieuTreHan.Text = soPhieuTreHan.ToString();
            }
        }
        private void LoadSachMat()
        {
            using (conn = new SqlConnection(strCon))
            {
                conn.Open();
                string query = @"
                SELECT COUNT(*) AS SoLuongSachMat
                FROM CT_PhieuMuon AS ct
                JOIN PhieuMuon AS pm ON pm.MaPhieuMuon = ct.MaPhieuMuon
                WHERE pm.NgayThucTra IS NOT NULL
                  AND pm.NgayMuon >= '2024-01-01'
                  AND ct.DaTraSach = 0;";

                // Thực thi truy vấn
                SqlCommand cmd = new SqlCommand(query, conn);
                int soLuong = (int)cmd.ExecuteScalar();

                // Hiển thị lên Label
                lblSachMat.Text = soLuong.ToString();
            }
        }
        
        private void LoadSachTraHu()
        {
            using (conn = new SqlConnection(strCon))
            {
                conn.Open();
                string query = @"
                SELECT COUNT(*) AS SoLuongSachTraHu
                FROM CT_PhieuMuon AS ct
                JOIN PhieuMuon AS pm ON pm.MaPhieuMuon = ct.MaPhieuMuon
                WHERE pm.NgayThucTra IS NOT NULL 
                  AND pm.NgayThucTra BETWEEN '2024-01-01' AND CAST(GETDATE() AS DATE)
                  AND ct.TinhTrangTra NOT LIKE 'Ok' and ct.TinhTrangTra is not null;";

                // Thực thi truy vấn
                SqlCommand cmd = new SqlCommand(query, conn);
                int soLuong = (int)cmd.ExecuteScalar();

                // Hiển thị lên Label
                lblSachTraHu.Text = soLuong.ToString();
            }
        }
        private void LoadSachChoMuon()
        {
            using (conn = new SqlConnection(strCon))
            {
                conn.Open();
                // Câu lệnh SQL
                string query = "SELECT COUNT(*) AS SoCuonSachChoMuon FROM CuonSach WHERE TinhTrang = N'Còn'";

                // Thực thi lệnh SQL
                SqlCommand cmd = new SqlCommand(query, conn);
                int soCuonSachChoMuon = (int)cmd.ExecuteScalar();

                // Hiển thị lên Label
                lblSachChoMuon.Text = soCuonSachChoMuon.ToString();
            }
        }
        private void LoadTop5DauSach()
        {
            using (conn = new SqlConnection(strCon))
            {
                conn.Open();
                string query = @"SELECT TOP 5 
                        ds.TenDauSach AS [Tên Đầu Sách],
                        COUNT(ct.MaSach) AS [Số Lượt Mượn]
                     FROM CT_PhieuMuon ct
                     INNER JOIN CuonSach cs ON ct.MaSach = cs.MaSach
                     INNER JOIN DauSach ds ON cs.MaDauSach = ds.MaDauSach
                     GROUP BY ds.TenDauSach
                     ORDER BY [Số Lượt Mượn] DESC";
                adapter = new SqlDataAdapter(query, conn);
                dt = new DataTable();
                adapter.Fill(dt);
                dgvTop5DauSach.DataSource = dt;
            }
        }

        private void LoadTop5ChuDe()
        {
            using (conn = new SqlConnection(strCon))
            {
                conn.Open();
                string query = @"SELECT TOP 5 
                        cd.TenChuDe AS [Tên Chủ Đề],
                        COUNT(ct.MaSach) AS [Số Lượt Mượn]
                     FROM CT_PhieuMuon ct
                     INNER JOIN CuonSach cs ON ct.MaSach = cs.MaSach
                     INNER JOIN DauSach ds ON cs.MaDauSach = ds.MaDauSach
                     INNER JOIN ChuDe cd ON ds.MaChuDe = cd.MaChuDe
                     GROUP BY cd.TenChuDe
                     ORDER BY [Số Lượt Mượn] DESC";
                adapter = new SqlDataAdapter(query, conn);
                dt = new DataTable();
                adapter.Fill(dt);
                dgvTop5ChuDe.DataSource = dt;
            }
        }

        private void LoadChartDocGia()
        {
            using (conn = new SqlConnection(strCon))
            {
                try
                {
                    conn.Open();

                    string query = @"
        SELECT 
            MONTH(NgayCapThe) AS Thang, 
            COUNT(*) AS SoLuongDocGia
        FROM 
            DocGia
        WHERE 
            YEAR(NgayCapThe) = YEAR(GETDATE()) 
            AND MONTH(NgayCapThe) <= 11 
        GROUP BY 
            MONTH(NgayCapThe)
        ORDER BY 
            Thang";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Xóa dữ liệu cũ trên biểu đồ
                    chartDocGia.Series.Clear();
                    chartDocGia.Titles.Clear(); // Xóa tiêu đề cũ (nếu có)

                    // Tạo Series cho biểu đồ
                    Series series = new Series("Số lượng độc giả mới");
                    series.ChartType = SeriesChartType.Column;
                    series.IsValueShownAsLabel = true; // Hiển thị giá trị trên cột

                    // Thêm dữ liệu vào Series
                    foreach (DataRow row in dt.Rows)
                    {
                        string thang = "Tháng " + row["Thang"].ToString();
                        int soLuong = Convert.ToInt32(row["SoLuongDocGia"]);
                        series.Points.AddXY(thang, soLuong);
                    }

                    // Thêm Series vào biểu đồ
                    chartDocGia.Series.Add(series);

                    // Thêm tiêu đề cho biểu đồ
                    Title title = new Title
                    {
                        Text = "Số lượng độc giả mới theo tháng",
                        Font = new Font("Segoe UI", 14, FontStyle.Bold),
                        ForeColor = Color.Navy, // Màu chữ
                        Alignment = ContentAlignment.TopCenter // Căn giữa
                    };
                    chartDocGia.Titles.Add(title);

                    // Tùy chỉnh trục X, Y
                    chartDocGia.ChartAreas[0].AxisX.Title = "Tháng";
                    chartDocGia.ChartAreas[0].AxisY.Title = "Số lượng độc giả";
                    chartDocGia.ChartAreas[0].AxisX.TitleFont = new Font("Segoe UI", 10, FontStyle.Bold);
                    chartDocGia.ChartAreas[0].AxisY.TitleFont = new Font("Segoe UI", 10, FontStyle.Bold);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tải dữ liệu biểu đồ: " + ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        private void LoadChartNgheNghiep()
        {
            using (conn = new SqlConnection(strCon))
            {
                try
                {
                    conn.Open();

                    string query = @"
            SELECT 
                NgheNghiep, 
                COUNT(*) AS SoLuong
            FROM 
                DocGia
            GROUP BY 
                NgheNghiep
            ORDER BY 
                SoLuong DESC";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Xóa dữ liệu cũ trên biểu đồ
                    chartNgheNghiep.Series.Clear();
                    chartNgheNghiep.Titles.Clear(); // Xóa tiêu đề cũ (nếu có)

                    // Tạo Series cho biểu đồ
                    Series series = new Series("Nghề Nghiệp Độc Giả");
                    series.ChartType = SeriesChartType.Pie;
                    series.IsValueShownAsLabel = true; // Hiển thị giá trị trên biểu đồ
                    series.Label = "#PERCENT"; // Hiển thị phần trăm trên các lát
                    series.LabelForeColor = Color.White; // Màu chữ trắng
                    series.Font = new Font("Segoe UI", 10, FontStyle.Bold); // Font chữ trên lát
                    series.BorderColor = Color.White; // Viền trắng
                    series.BorderWidth = 2; // Độ rộng viền

                    // Thêm dữ liệu vào Series
                    foreach (DataRow row in dt.Rows)
                    {
                        string ngheNghiep = row["NgheNghiep"].ToString();
                        int soLuong = Convert.ToInt32(row["SoLuong"]);
                        var point = series.Points.AddXY(ngheNghiep, soLuong);
                    }

                    // Sắp xếp lát giảm dần
                    chartNgheNghiep.DataManipulator.Sort(PointSortOrder.Descending, series);

                    // Thêm Series vào biểu đồ
                    chartNgheNghiep.Series.Add(series);

                    // Thêm tiêu đề cho biểu đồ
                    Title title = new Title
                    {
                        Text = "Biểu Đồ Nghề Nghiệp Độc Giả",
                        Font = new Font("Segoe UI", 14, FontStyle.Bold),
                        ForeColor = Color.Navy, // Màu chữ tiêu đề
                        Alignment = ContentAlignment.TopCenter // Căn giữa
                    };
                    chartNgheNghiep.Titles.Add(title);

                    // Tùy chỉnh Legend (Chú thích)
                    chartNgheNghiep.Legends.Clear();
                    Legend legend = new Legend
                    {
                        Docking = Docking.Right, // Đặt chú thích bên phải
                        Font = new Font("Segoe UI", 10, FontStyle.Regular), // Font chú thích
                        BackColor = Color.Transparent // Nền trong suốt
                    };
                    chartNgheNghiep.Legends.Add(legend);

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tải dữ liệu biểu đồ: " + ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
        }
    }
}
