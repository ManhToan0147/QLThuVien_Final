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

            LoadTop5ChuDe();
            LoadTop5DauSach();
            LoadSoPhieuTreHan();
            LoadSachChoMuon();
            LoadSachDangMuon();
            LoadSoPhieuMuon();
            LoadDocGiaDangMuon();
        }

        private void LoadDocGiaDangMuon()
        {
            using (conn = new SqlConnection(strCon))
            {
                conn.Open();
                // Câu lệnh SQL
                string query = "SELECT COUNT(*)  FROM DocGia_DaTraSach where DaTraSach = 'False'";

                // Thực thi lệnh SQL
                SqlCommand cmd = new SqlCommand(query, conn);
                int soDocGia = (int)cmd.ExecuteScalar();

                // Hiển thị lên Label
                lblSoDocGia.Text = soDocGia.ToString();
            }
        }


        private void LoadSoPhieuMuon()
        {
            using (conn = new SqlConnection(strCon))
            {
                conn.Open();
                // Câu lệnh SQL
                string query = "SELECT COUNT(*)  FROM PhieuMuon";

                // Thực thi lệnh SQL
                SqlCommand cmd = new SqlCommand(query, conn);
                int soPhieuMuon = (int)cmd.ExecuteScalar();

                // Hiển thị lên Label
                lblSoPhieuMuon.Text = soPhieuMuon.ToString();
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

        private void LoadSachDangMuon()
        {
            using (conn = new SqlConnection(strCon))
            {
                conn.Open();
                // Câu lệnh SQL
                string query = "SELECT COUNT(*) FROM CuonSach WHERE TinhTrang = N'Đang mượn'";

                // Thực thi lệnh SQL
                SqlCommand cmd = new SqlCommand(query, conn);
                int soSachDangMuon = (int)cmd.ExecuteScalar();

                // Hiển thị lên Label
                lblSachDangMuon.Text = soSachDangMuon.ToString();
            }
        }

        private void LoadSoPhieuTreHan()
        {
            using (conn = new SqlConnection(strCon))
            {
                conn.Open();
                string query = @"
                SELECT COUNT(*) AS SoLuongPhieuTreHan
                FROM PhieuMuon pm
                WHERE pm.HanTra < GETDATE()
                  AND pm.NgayThucTra IS NULL
                OR (pm.NgayThucTra > pm.HanTra)
                ";

                // Thực thi truy vấn
                SqlCommand cmd = new SqlCommand(query, conn);
                int soPhieuTreHan = (int)cmd.ExecuteScalar();

                // Hiển thị kết quả lên Label
                lblSoPhieuTreHan.Text = soPhieuTreHan.ToString();
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
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
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
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvTop5ChuDe.DataSource = dt;
            }
        }



        private void btnFilter_Click(object sender, EventArgs e)
        {
            frmLocNgay frmLoc = new frmLocNgay();

            if (frmLoc.ShowDialog() == DialogResult.OK)
            {
                if (frmLoc.IsApplied)
                {
                    currentTuNgay = frmLoc.TuNgay;
                    currentDenNgay = frmLoc.DenNgay;
                }
                else
                {
                    currentTuNgay = null;
                    currentDenNgay = null;
                }
            }
        }
    }
}
