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
    public partial class frmNhapSach : Form
    {
        string strCon = DBConfig.ConnectionString;
        SqlConnection con;
        SqlDataAdapter adapter;
        SqlCommand cmd;
        DataTable dt;
        DataView dv;

        public string MaPhieuMuon { get; set; }
        public string MaDocGia { get; set; }
        public string KieuMuon { get; set; }

        public frmNhapSach()
        {
            InitializeComponent();
        }

        private void frmNhapSach_Load(object sender, EventArgs e)
        {
            txtMaPhieuMuon.Text = MaPhieuMuon;
            txtMaDG.Text = MaDocGia;
            txtKieuMuon.Text = KieuMuon;

            dgvCuonSach.DefaultCellStyle.Font = new Font(dgvCuonSach.Font, FontStyle.Regular);
            dgvSachMuon.DefaultCellStyle.Font = new Font(dgvSachMuon.Font, FontStyle.Regular);
            LoadCuonSach();
            LoadCboTrangThai();
            LoadSachMuon(MaPhieuMuon);
            cboTrangThai.SelectedIndexChanged += FilterData;
            txtSearch.TextChanged += FilterData;
        }

        private void LoadSachMuon(string MaPhieuMuon)
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "select ctpm.MaSach, ds.TenDauSach, ctpm.TienCoc, ctpm.TinhTrangMuon " +
                    "from CT_PhieuMuon ctpm " +
                    "join CuonSach cs on ctpm.MaSach = cs.MaSach " +
                    "join DauSach ds on cs.MaDauSach = ds.MaDauSach " +
                    $"where ctpm.MaPhieuMuon = '{MaPhieuMuon}'";
                adapter = new SqlDataAdapter(sql, con);
                dt = new DataTable();
                adapter.Fill(dt);
                dgvSachMuon.DataSource = dt;
            }
        }

        private void LoadCboTrangThai()
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "Select distinct TinhTrang from CuonSach";
                adapter = new SqlDataAdapter(sql, con);
                dt = new DataTable();
                adapter.Fill(dt);

                // Thêm tùy chọn "Tất cả" vào DataTable
                DataRow row = dt.NewRow();
                row["TinhTrang"] = "Tất cả";
                dt.Rows.InsertAt(row, 0); 

                cboTrangThai.DataSource = dt;
                cboTrangThai.DisplayMember = "TinhTrang";
                cboTrangThai.SelectedIndex = 0;
            }
        }

        private void LoadCuonSach()
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "select cs.MaSach, ds.TenDauSach, ds.GiaBia, cs.TinhTrang " +
                    "from CuonSach cs join DauSach ds on cs.MaDauSach = ds.MaDauSach";
                adapter = new SqlDataAdapter(sql,con);
                dt = new DataTable();
                adapter.Fill(dt);
                dv = new DataView(dt);
                dgvCuonSach.DataSource = dv;
            }
        }

        private void FilterData(object sender, EventArgs e)
        {
            string trangThai = cboTrangThai.Text == "Tất cả" ? "" : cboTrangThai.Text;
            string search = txtSearch.Text;

            // Xây dựng bộ lọc
            string filter = "";
            if (!string.IsNullOrEmpty(trangThai))
            {
                filter += $"TinhTrang = '{trangThai}'";
            }
            if (!string.IsNullOrEmpty(search))
            {
                // Nếu đã có điều kiện lọc trạng thái, thêm "AND"
                filter += (string.IsNullOrEmpty(filter) ? "" : " AND ") + $"TenDauSach LIKE '%{search}%'";
            }
            // Áp dụng bộ lọc vào DataView
            dv.RowFilter = filter;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            dv.RowFilter = null;
            cboTrangThai.SelectedIndex = 0;
            txtSearch.Text = "";
        }

        private void btnChuyenXuong_Click(object sender, EventArgs e)
        {
            if (dgvCuonSach.SelectedRows.Count > 0)
            {
                int maxCount = KieuMuon == "Tại chỗ" ? 2 : 3;
                foreach (DataGridViewRow row in dgvCuonSach.SelectedRows)
                {
                    string maPM = MaPhieuMuon;
                    string maSach = row.Cells[0].Value.ToString();
                    string trangThai = row.Cells[3].Value.ToString();

                    if (trangThai != "Còn")
                    {
                        MessageBox.Show("Cuốn sách này hiện giờ không có sẵn.");
                        return;
                    }

                    string moTa = "";
                    float tienCoc = 0;
                    using (con = new SqlConnection(strCon))
                    {
                        con.Open();
                        //Lay mo ta
                        string sqlMoTa = $"Select MoTa from CuonSach where MaSach = '{maSach}'";
                        cmd = new SqlCommand(sqlMoTa, con);
                        moTa = cmd.ExecuteScalar()?.ToString() ?? string.Empty;

                        //Tinh tien coc = 2 * giabia
                        string sqlGiaBia = $"Select ds.GiaBia from CuonSach cs " +
                            $"join DauSach ds on cs.MaDauSach = ds.MaDauSach where cs.MaSach = '{maSach}'";
                        cmd = new SqlCommand(sqlGiaBia, con);
                        string rs = cmd.ExecuteScalar()?.ToString() ?? string.Empty;
                        tienCoc = 2 * float.Parse(rs); //Tien coc = 2 * gia bia

                        string sql = "Insert into CT_PhieuMuon(MaPhieuMuon, MaSach, TienCoc, TinhTrangMuon) values " +
                            $"('{MaPhieuMuon}', '{maSach}', {tienCoc} , N'{moTa}')";
                        cmd = new SqlCommand(sql, con);
                        try
                        {
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Lỗi " + ex.Message);
                        }
                    }
                }
                LoadCuonSach();
                LoadSachMuon(MaPhieuMuon);
            } 
            else
            {
                MessageBox.Show("Chưa chọn sách để thêm");
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvSachMuon.SelectedRows.Count > 0)
            {
                DialogResult rs = MessageBox.Show("Bạn có chắc chắn muốn xóa các bản ghi đã chọn không?", "Xác nhận", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (rs == DialogResult.Yes)
                {
                    using (con = new SqlConnection(strCon))
                    {
                        con.Open();
                        foreach (DataGridViewRow row in dgvSachMuon.SelectedRows)
                        {
                            string maPM = MaPhieuMuon;
                            string maSach = row.Cells[0].Value.ToString();
                            string sql = "DELETE FROM CT_PhieuMuon WHERE MaPhieuMuon = @MaPhieuMuon AND MaSach = @MaSach";
                            using (SqlCommand cmd = new SqlCommand(sql, con))
                            {
                                cmd.Parameters.AddWithValue("@MaPhieuMuon", maPM);
                                cmd.Parameters.AddWithValue("@MaSach", maSach);
                                cmd.ExecuteNonQuery();
                            }
                            // Cập nhật tình trạng trong CuonSach
                            string sqlUpdate = @"
                                UPDATE CuonSach
                                SET TinhTrang = N'Còn'
                                WHERE MaSach = @MaSach
                                  AND NOT EXISTS (
                                      SELECT 1
                                      FROM CT_PhieuMuon CT
                                      INNER JOIN PhieuMuon PM ON CT.MaPhieuMuon = PM.MaPhieuMuon
                                      WHERE CT.MaSach = @MaSach 
                                      AND PM.NgayThucTra IS NULL
                                  )";
                            using (SqlCommand cmd = new SqlCommand(sqlUpdate, con))
                            {
                                cmd.Parameters.AddWithValue("@MaSach", maSach);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    MessageBox.Show("Đã xóa thành công.");
                    LoadCuonSach();
                    LoadSachMuon(MaPhieuMuon);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn ít nhất một bản ghi để xóa.");
                return;
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(strCon))
            {
                con.Open();
                foreach (DataGridViewRow row in dgvSachMuon.Rows)
                {
                    if (!row.IsNewRow) // Loại trừ dòng trống
                    {
                        string maPM = txtMaPhieuMuon.Text;
                        string maSach = row.Cells[0].Value.ToString();
                        string tinhTrangMuon = row.Cells["TinhTrangMuon"].Value.ToString();
                        float tienCoc = float.Parse(row.Cells["TienCoc"].Value.ToString());

                        string sql = "UPDATE CT_PhieuMuon SET TinhTrangMuon = @TinhTrangMuon, TienCoc = @TienCoc " +
                            "WHERE MaPhieuMuon = @MaPhieuMuon AND MaSach = @MaSach";
                        // Cập nhật dữ liệu trong bảng `ct_phieumuon`
                        using (SqlCommand cmd = new SqlCommand(sql, con))
                        {
                            cmd.Parameters.AddWithValue("@MaPhieuMuon", maPM);
                            cmd.Parameters.AddWithValue("@MaSach", maSach);
                            cmd.Parameters.AddWithValue("@TinhTrangMuon", tinhTrangMuon);
                            cmd.Parameters.AddWithValue("@TienCoc", tienCoc);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            MessageBox.Show("Dữ liệu đã được lưu thành công.");
        }
    }
}
