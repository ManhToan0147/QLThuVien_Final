using QL_ThuVien.Form_support;
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
using TheArtOfDevHtmlRenderer.Adapters;

namespace QL_ThuVien.Main_UC.TrangChu
{
    public partial class UC_Dashboard : UserControl
    {
        string strCon = DBConfig.ConnectionString;
        SqlConnection conn;
        SqlCommand cmd;

        // Biến lưu trữ bộ lọc hiện tại
        private DateTime? currentTuNgay = null;
        private DateTime? currentDenNgay = null;
        public UC_Dashboard()
        {
            InitializeComponent();
        }

        private void UC_Dashboard_Load(object sender, EventArgs e)
        {
            //Format lai dgv
            dgvTop5ChuDe.ColumnHeadersDefaultCellStyle.Font = new Font(dgvTop5ChuDe.Font, FontStyle.Bold);
            dgvTop5DauSach.ColumnHeadersDefaultCellStyle.Font = new Font(dgvTop5DauSach.Font, FontStyle.Bold);

            LoadAllData(null, null);
        }


        public void LoadAllData(DateTime? tuNgay, DateTime? denNgay)
        {
            currentTuNgay = tuNgay;
            currentDenNgay = denNgay;

            // 5 KPI chính
            LoadSachCoTheMuon();                // KPI 1 - Không lọc
            LoadSoDocGiaMuon(tuNgay, denNgay);  // KPI 2 - Có lọc
            LoadSoPhieuMuon(tuNgay, denNgay);   // KPI 3 - Có lọc
            LoadTraQuaHan(tuNgay, denNgay);     // KPI 4 - Có lọc
            LoadSoPhieuPhat(tuNgay, denNgay);   // KPI 5 - Có lọc

            // Top 5
            LoadTop5ChuDe(tuNgay, denNgay);
            LoadTop5DauSach(tuNgay, denNgay);

            //Chart 
            LoadChartKieuMuon(tuNgay, denNgay);
            LoadChartNgheNghiep(tuNgay, denNgay);
            LoadChartTrendPhieuMuon(tuNgay, denNgay);
        }

        private void LoadChartTrendPhieuMuon(DateTime? tuNgay, DateTime? denNgay)
        {
            using (conn = new SqlConnection(strCon))
            {
                conn.Open();
                string query = @"
                    SELECT 
                        YEAR(NgayMuon) AS Nam,
                        MONTH(NgayMuon) AS Thang,
                        COUNT(*) AS SoPhieuMuon
                    FROM PhieuMuon
                    WHERE 1=1";

                if (tuNgay.HasValue && denNgay.HasValue)
                {
                    query += " AND NgayMuon BETWEEN @TuNgay AND @DenNgay";
                }

                query += @"
                    GROUP BY YEAR(NgayMuon), MONTH(NgayMuon)
                    ORDER BY Nam, Thang";

                cmd = new SqlCommand(query, conn);
                AddDateParameters(cmd, tuNgay, denNgay);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                // === Xóa dữ liệu cũ ===
                chartTrendPhieuMuon.Series.Clear();
                chartTrendPhieuMuon.Titles.Clear();
                chartTrendPhieuMuon.Legends.Clear();
                chartTrendPhieuMuon.ChartAreas.Clear();

                // === Tạo ChartArea ===
                ChartArea chartArea = new ChartArea();
                chartArea.AxisX.Title = "Tháng";
                chartArea.AxisX.TitleFont = new Font("Segoe UI", 10, FontStyle.Bold);
                chartArea.AxisX.LabelStyle.Font = new Font("Segoe UI", 9);
                chartArea.AxisX.MajorGrid.LineColor = Color.LightGray;

                chartArea.AxisY.Title = "Số Phiếu Mượn";
                chartArea.AxisY.TitleFont = new Font("Segoe UI", 10, FontStyle.Bold);
                chartArea.AxisY.LabelStyle.Font = new Font("Segoe UI", 9);
                chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;

                chartTrendPhieuMuon.ChartAreas.Add(chartArea);

                // === Tạo Series ===
                Series series = new Series
                {
                    Name = "Phiếu Mượn",
                    ChartType = SeriesChartType.Line,
                    BorderWidth = 3,
                    Color = Color.FromArgb(52, 152, 219), // Xanh dương
                    MarkerStyle = MarkerStyle.Circle,
                    MarkerSize = 8,
                    MarkerColor = Color.FromArgb(41, 128, 185)
                };

                // === Thêm dữ liệu ===
                foreach (DataRow row in dt.Rows)
                {
                    int nam = Convert.ToInt32(row["Nam"]);
                    int thang = Convert.ToInt32(row["Thang"]);
                    int soPhieuMuon = Convert.ToInt32(row["SoPhieuMuon"]);

                    string label = $"{thang:00}/{nam}";
                    series.Points.AddXY(label, soPhieuMuon);
                }

                chartTrendPhieuMuon.Series.Add(series);

                // === Tiêu đề ===
                chartTrendPhieuMuon.Titles.Add("XU HƯỚNG PHIẾU MƯỢN THEO THỜI GIAN");
                chartTrendPhieuMuon.Titles[0].Font = new Font("Segoe UI", 14, FontStyle.Bold);

                // === Legend (tùy chọn) ===
                Legend legend = new Legend
                {
                    Docking = Docking.Top,
                    Alignment = StringAlignment.Far,
                    Font = new Font("Segoe UI", 9)
                };
                chartTrendPhieuMuon.Legends.Add(legend);
            }
        }


        private void LoadChartNgheNghiep(DateTime? tuNgay, DateTime? denNgay)
        {
            using (conn = new SqlConnection(strCon))
            {
                conn.Open();
                string query = @"
                    SELECT 
                        dg.NgheNghiep AS NgheNghiep,
                        COUNT(pm.MaPhieuMuon) AS SoPhieuMuon
                    FROM PhieuMuon pm
                    INNER JOIN DocGia dg ON pm.MaDocGia = dg.MaDocGia
                    WHERE dg.NgheNghiep IS NOT NULL AND dg.NgheNghiep != ''";

                if (tuNgay.HasValue && denNgay.HasValue)
                {
                    query += " AND pm.NgayMuon BETWEEN @TuNgay AND @DenNgay";
                }

                query += @"
                    GROUP BY dg.NgheNghiep
                    ORDER BY SoPhieuMuon DESC";

                cmd = new SqlCommand(query, conn);
                AddDateParameters(cmd, tuNgay, denNgay);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                // === Xóa dữ liệu cũ ===
                chartNgheNghiep.Series.Clear();
                chartNgheNghiep.Titles.Clear();
                chartNgheNghiep.Legends.Clear();

                // === Tạo Series ===
                Series series = new Series
                {
                    Name = "NgheNghiep",
                    ChartType = SeriesChartType.Doughnut,
                    IsValueShownAsLabel = true,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    LabelForeColor = Color.White
                };

                // === Bảng màu đa dạng (7+ màu) ===
                Color[] colors = new Color[]
                {
                    Color.FromArgb(52, 152, 219),   // Xanh dương
                    Color.FromArgb(255, 193, 7),    // Vàng
                    Color.FromArgb(230, 74, 25),    // Đỏ cam
                    Color.FromArgb(0, 123, 167),    // Xanh đậm
                    Color.FromArgb(189, 195, 199),  // Xám
                    Color.FromArgb(44, 62, 80),     // Xanh navy
                    Color.FromArgb(46, 204, 113),   // Xanh lá
                    Color.FromArgb(155, 89, 182),   // Tím
                    Color.FromArgb(241, 196, 15),   // Vàng đậm
                    Color.FromArgb(231, 76, 60)     // Đỏ
                };

                // === Tính tổng để hiển thị % ===
                int tongSoPhieuMuon = 0;
                foreach (DataRow row in dt.Rows)
                {
                    tongSoPhieuMuon += Convert.ToInt32(row["SoPhieuMuon"]);
                }

                // === Thêm dữ liệu vào Chart ===
                int i = 0;
                foreach (DataRow row in dt.Rows)
                {
                    string ngheNghiep = row["NgheNghiep"].ToString();
                    int soPhieuMuon = Convert.ToInt32(row["SoPhieuMuon"]);
                    double tiLe = tongSoPhieuMuon > 0 ? (double)soPhieuMuon / tongSoPhieuMuon * 100 : 0;

                    DataPoint point = new DataPoint();
                    point.SetValueXY(ngheNghiep, soPhieuMuon);
                    point.Label = $"{soPhieuMuon}\n({tiLe:F1}%)";
                    point.LegendText = ngheNghiep;
                    point.Color = colors[i % colors.Length];

                    series.Points.Add(point);
                    i++;
                }

                chartNgheNghiep.Series.Add(series);

                // === Tiêu đề Chart ===
                chartNgheNghiep.Titles.Add("CƠ CẤU PHIẾU MƯỢN THEO NGHỀ NGHIỆP");
                chartNgheNghiep.Titles[0].Font = new Font("Segoe UI", 14, FontStyle.Bold);

                // === Legend ===
                Legend legend = new Legend
                {
                    Docking = Docking.Right,
                    Alignment = StringAlignment.Center,
                    Font = new Font("Segoe UI", 10)
                };
                chartNgheNghiep.Legends.Add(legend);
            }
        }


        private void LoadChartKieuMuon(DateTime? tuNgay, DateTime? denNgay)
        {
            using (conn = new SqlConnection(strCon))
            {
                conn.Open();
                string query = @"
                    SELECT 
                        km.TenKieuMuon,
                        COUNT(*) AS SoLuong
                    FROM PhieuMuon pm
                    INNER JOIN KieuMuon km ON pm.MaKieuMuon = km.MaKieuMuon
                    WHERE 1=1";

                if (tuNgay.HasValue && denNgay.HasValue)
                {
                    query += " AND pm.NgayMuon BETWEEN @TuNgay AND @DenNgay";
                }

                query += " GROUP BY km.TenKieuMuon";

                cmd = new SqlCommand(query, conn);
                AddDateParameters(cmd, tuNgay, denNgay);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                // === Xóa dữ liệu cũ ===
                chartKieuMuon.Series.Clear();
                chartKieuMuon.Titles.Clear();
                chartKieuMuon.Legends.Clear();

                // === Tạo Series ===
                Series series = new Series
                {
                    Name = "KieuMuon",
                    ChartType = SeriesChartType.Doughnut,
                    IsValueShownAsLabel = true,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    LabelForeColor = Color.White
                };

                // === Màu sắc ===
                Color[] colors = new Color[]
                {
                    Color.FromArgb(52, 152, 219),   // Xanh dương - Mang về
                    Color.FromArgb(46, 204, 113)    // Xanh lá - Tại chỗ
                };

                // === Tính tổng để hiển thị % ===
                int tongSoLuong = 0;
                foreach (DataRow row in dt.Rows)
                {
                    tongSoLuong += Convert.ToInt32(row["SoLuong"]);
                }

                // === Thêm dữ liệu vào Chart ===
                int i = 0;
                foreach (DataRow row in dt.Rows)
                {
                    string tenKieuMuon = row["TenKieuMuon"].ToString();
                    int soLuong = Convert.ToInt32(row["SoLuong"]);
                    double tiLe = tongSoLuong > 0 ? (double)soLuong / tongSoLuong * 100 : 0;

                    DataPoint point = new DataPoint();
                    point.SetValueXY(tenKieuMuon, soLuong);
                    point.Label = $"{soLuong}\n({tiLe:F1}%)";
                    point.LegendText = $"{tenKieuMuon}";
                    point.Color = colors[i % colors.Length];

                    series.Points.Add(point);
                    i++;
                }

                chartKieuMuon.Series.Add(series);

                // === Tiêu đề Chart ===
                chartKieuMuon.Titles.Add("CƠ CẤU PHIẾU MƯỢN THEO KIỂU MƯỢN");
                chartKieuMuon.Titles[0].Font = new Font("Segoe UI", 14, FontStyle.Bold);

                // === Legend ===
                Legend legend = new Legend
                {
                    Docking = Docking.Right,
                    Alignment = StringAlignment.Center,
                    Font = new Font("Segoe UI", 10)
                };
                chartKieuMuon.Legends.Add(legend);
            }
        }


        // ===== KPI 1: SỐ SÁCH CÓ THỂ CHO MƯỢN (KHÔNG LỌC) =====
        private void LoadSachCoTheMuon()
        {
            using (conn = new SqlConnection(strCon))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM CuonSach WHERE TinhTrang = N'Còn'";
                cmd = new SqlCommand(query, conn);
                lblSachChoMuon.Text = cmd.ExecuteScalar().ToString();
            }
        }

        // ===== KPI 2: SỐ ĐỘC GIẢ MƯỢN (CÓ LỌC) =====
        private void LoadSoDocGiaMuon(DateTime? tuNgay, DateTime? denNgay)
        {
            using (conn = new SqlConnection(strCon))
            {
                conn.Open();
                string query = "SELECT COUNT(DISTINCT MaDocGia) FROM PhieuMuon WHERE 1=1";

                if (tuNgay.HasValue && denNgay.HasValue)
                {
                    query += " AND NgayMuon BETWEEN @TuNgay AND @DenNgay";
                }

                cmd = new SqlCommand(query, conn);
                AddDateParameters(cmd, tuNgay, denNgay);
                lblSoDocGia.Text = cmd.ExecuteScalar().ToString();
            }
        }

        // ===== KPI 3: SỐ PHIẾU MƯỢN (CÓ LỌC) =====
        private void LoadSoPhieuMuon(DateTime? tuNgay, DateTime? denNgay)
        {
            using (conn = new SqlConnection(strCon))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM PhieuMuon WHERE 1=1";

                if (tuNgay.HasValue && denNgay.HasValue)
                {
                    query += " AND NgayMuon BETWEEN @TuNgay AND @DenNgay";
                }

                cmd = new SqlCommand(query, conn);
                AddDateParameters(cmd, tuNgay, denNgay);
                lblSoPhieuMuon.Text = cmd.ExecuteScalar().ToString();
            }
        }

        // ===== KPI 4: TRẢ QUÁ HẠN (CÓ LỌC) =====
        private void LoadTraQuaHan(DateTime? tuNgay, DateTime? denNgay)
        {
            using (conn = new SqlConnection(strCon))
            {
                conn.Open();
                string query = @"
                    SELECT COUNT(*) FROM PhieuMuon
                    WHERE NgayThucTra IS NOT NULL
                      AND NgayThucTra > HanTra";

                if (tuNgay.HasValue && denNgay.HasValue)
                {
                    query += " AND NgayMuon BETWEEN @TuNgay AND @DenNgay";
                }

                cmd = new SqlCommand(query, conn);
                AddDateParameters(cmd, tuNgay, denNgay);

                int soPhieuQuaHan = (int)cmd.ExecuteScalar();
                lblSoPhieuTreHan.Text = soPhieuQuaHan.ToString();
                lblSoPhieuTreHan.ForeColor = soPhieuQuaHan > 0 ? Color.Red : Color.Green;
                lblViewQuaHan.ForeColor = soPhieuQuaHan > 0 ? Color.Red : Color.Green;

            }
        }

        // ===== KPI 5: SỐ PHIẾU PHẠT (CÓ LỌC) =====
        private void LoadSoPhieuPhat(DateTime? tuNgay, DateTime? denNgay)
        {
            using (conn = new SqlConnection(strCon))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM PhieuPhat WHERE 1=1";

                if (tuNgay.HasValue && denNgay.HasValue)
                {
                    query += " AND NgayNopPhat BETWEEN @TuNgay AND @DenNgay";
                }

                cmd = new SqlCommand(query, conn);
                AddDateParameters(cmd, tuNgay, denNgay);

                int soPhieuPhat = (int)cmd.ExecuteScalar();
                lblSoPhieuPhat.Text = soPhieuPhat.ToString();
                lblSoPhieuPhat.ForeColor = soPhieuPhat > 0 ? Color.Red : Color.Green;
                lblViewPhieuPhat.ForeColor = soPhieuPhat > 0 ? Color.Red : Color.Green;
            }
        }

        // ===== TOP 5 CHỦ ĐỀ =====
        private void LoadTop5ChuDe(DateTime? tuNgay, DateTime? denNgay)
        {
            using (conn = new SqlConnection(strCon))
            {
                conn.Open();
                string query = @"
                    SELECT TOP 5 
                        cd.TenChuDe AS [Tên Chủ Đề],
                        COUNT(ct.MaSach) AS [Số Lượt Mượn]
                    FROM CT_PhieuMuon ct
                    INNER JOIN CuonSach cs ON ct.MaSach = cs.MaSach
                    INNER JOIN DauSach ds ON cs.MaDauSach = ds.MaDauSach
                    INNER JOIN ChuDe cd ON ds.MaChuDe = cd.MaChuDe
                    INNER JOIN PhieuMuon pm ON ct.MaPhieuMuon = pm.MaPhieuMuon
                    WHERE 1=1";

                if (tuNgay.HasValue && denNgay.HasValue)
                {
                    query += " AND pm.NgayMuon BETWEEN @TuNgay AND @DenNgay";
                }

                query += @"
                    GROUP BY cd.TenChuDe
                    ORDER BY [Số Lượt Mượn] DESC";

                cmd = new SqlCommand(query, conn);
                AddDateParameters(cmd, tuNgay, denNgay);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvTop5ChuDe.DataSource = dt;
            }
        }

        // ===== TOP 5 ĐẦU SÁCH =====
        private void LoadTop5DauSach(DateTime? tuNgay, DateTime? denNgay)
        {
            using (conn = new SqlConnection(strCon))
            {
                conn.Open();
                string query = @"
                    SELECT TOP 5 
                        ds.TenDauSach AS [Tên Đầu Sách],
                        COUNT(ct.MaSach) AS [Số Lượt Mượn]
                    FROM CT_PhieuMuon ct
                    INNER JOIN CuonSach cs ON ct.MaSach = cs.MaSach
                    INNER JOIN DauSach ds ON cs.MaDauSach = ds.MaDauSach
                    INNER JOIN PhieuMuon pm ON ct.MaPhieuMuon = pm.MaPhieuMuon
                    WHERE 1=1";

                if (tuNgay.HasValue && denNgay.HasValue)
                {
                    query += " AND pm.NgayMuon BETWEEN @TuNgay AND @DenNgay";
                }

                query += @"
                    GROUP BY ds.TenDauSach
                    ORDER BY [Số Lượt Mượn] DESC";

                cmd = new SqlCommand(query, conn);
                AddDateParameters(cmd, tuNgay, denNgay);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvTop5DauSach.DataSource = dt;
            }
        }

        // ===== HÀM HELPER =====
        private void AddDateParameters(SqlCommand cmd, DateTime? tuNgay, DateTime? denNgay)
        {
            if (tuNgay.HasValue && denNgay.HasValue)
            {
                cmd.Parameters.AddWithValue("@TuNgay", tuNgay.Value.Date);
                cmd.Parameters.AddWithValue("@DenNgay", denNgay.Value.Date.AddDays(1).AddSeconds(-1));
            }
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            frmLocNgay frmLoc = new frmLocNgay();

            if (frmLoc.ShowDialog() == DialogResult.OK)
            {
                if (frmLoc.IsApplied)
                {
                    LoadAllData(frmLoc.TuNgay, frmLoc.DenNgay);
                }
                else
                {
                    LoadAllData(null, null);
                }
            }
        }
    }
}
