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
            cboTruong.SelectedIndexChanged += cboTruong_SelectedIndexChanged;

            cboTruong.SelectedIndex = 0;
        }

        private void LoadSachMuon(string MaPhieuMuon)
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = @"SELECT ctpm.MaSach, ds.TenDauSach, ctpm.TienCoc, ctpm.TinhTrangMuon 
                               FROM CT_PhieuMuon ctpm 
                               JOIN CuonSach cs ON ctpm.MaSach = cs.MaSach 
                               JOIN DauSach ds ON cs.MaDauSach = ds.MaDauSach 
                               WHERE ctpm.MaPhieuMuon = @MaPhieuMuon";
                adapter = new SqlDataAdapter(sql, con);
                adapter.SelectCommand.Parameters.AddWithValue("@MaPhieuMuon", MaPhieuMuon);
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
                string sql = "SELECT DISTINCT TinhTrang FROM CuonSach";
                adapter = new SqlDataAdapter(sql, con);
                dt = new DataTable();
                adapter.Fill(dt);

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
                string sql = @"SELECT cs.MaSach, ds.TenDauSach, ds.GiaBia, cs.TinhTrang 
                               FROM CuonSach cs 
                               JOIN DauSach ds ON cs. MaDauSach = ds. MaDauSach";
                adapter = new SqlDataAdapter(sql, con);
                dt = new DataTable();
                adapter.Fill(dt);
                dv = new DataView(dt);
                dgvCuonSach.DataSource = dv;
            }
        }

        private void FilterData(object sender, EventArgs e)
        {
            string trangThai = cboTrangThai.Text == "Tất cả" ? "" : cboTrangThai.Text;
            string search = txtSearch.Text.Trim();

            string filter = "";

            if (!string.IsNullOrEmpty(trangThai))
            {
                filter += $"TinhTrang = '{trangThai}'";
            }

            if (!string.IsNullOrEmpty(search))
            {
                if (!string.IsNullOrEmpty(filter))
                {
                    filter += " AND ";
                }

                if (cboTruong.SelectedIndex == 0)
                {
                    filter += $"TenDauSach LIKE '%{search}%'";
                }
                else
                {
                    filter += $"MaSach LIKE '%{search}%'";
                }
            }

            dv.RowFilter = filter;
        }

        private void cboTruong_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSearch.Clear();
            if (cboTruong.SelectedIndex == 0)
            {
                txtSearch.PlaceholderText = "Nhập tên đầu sách để tìm kiếm";
            }
            else
            {
                txtSearch.PlaceholderText = "Nhập mã sách để tìm kiếm";
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            dv.RowFilter = "";
            cboTrangThai.SelectedIndex = 0;
            cboTruong.SelectedIndex = 0;
            txtSearch.Clear();
        }

        private bool KiemTraGioiHanMuon()
        {
            try
            {
                using (con = new SqlConnection(strCon))
                {
                    con.Open();

                    // Đếm số sách hiện tại trong phiếu mượn
                    string sqlCount = "SELECT COUNT(*) FROM CT_PhieuMuon WHERE MaPhieuMuon = @MaPhieuMuon";
                    cmd = new SqlCommand(sqlCount, con);
                    cmd.Parameters.AddWithValue("@MaPhieuMuon", MaPhieuMuon);
                    int soLuongHienTai = (int)cmd.ExecuteScalar();

                    // Lấy giới hạn từ bảng KieuMuon
                    string sqlGioiHan = @"SELECT km.SoSachToiDa 
                                  FROM PhieuMuon pm
                                  JOIN KieuMuon km ON pm.MaKieuMuon = km. MaKieuMuon
                                  WHERE pm.MaPhieuMuon = @MaPhieuMuon";
                    cmd = new SqlCommand(sqlGioiHan, con);
                    cmd.Parameters.AddWithValue("@MaPhieuMuon", MaPhieuMuon);
                    object result = cmd.ExecuteScalar();

                    if (result == null || result == DBNull.Value)
                    {
                        MessageBox.Show("Không tìm thấy thông tin giới hạn mượn sách!", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    int gioiHan = Convert.ToInt32(result);

                    if (soLuongHienTai >= gioiHan)
                    {
                        MessageBox.Show(
                            $"Đã đạt giới hạn {gioiHan} cuốn cho kiểu mượn '{KieuMuon}'! ",
                            "Cảnh báo",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return false;
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kiểm tra giới hạn:\n{ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void btnChuyenXuong_Click(object sender, EventArgs e)
        {
            if (dgvCuonSach.SelectedRows.Count == 0)
            {
                MessageBox.Show("Chưa chọn sách để thêm!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int successCount = 0;
            int failCount = 0;

            foreach (DataGridViewRow row in dgvCuonSach.SelectedRows)
            {
                if (!KiemTraGioiHanMuon())
                {
                    break;
                }

                string maSach = row.Cells["MaSach"].Value?.ToString();
                string trangThai = row.Cells["TinhTrang"].Value?.ToString();

                if (string.IsNullOrEmpty(maSach))
                {
                    continue;
                }

                if (trangThai != "Còn")
                {
                    MessageBox.Show($"Sách '{maSach}' hiện không có sẵn (Trạng thái: {trangThai})!",
                        "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    failCount++;
                    continue;
                }

                try
                {
                    using (con = new SqlConnection(strCon))
                    {
                        con.Open();

                        string checkSql = "SELECT COUNT(*) FROM CT_PhieuMuon WHERE MaPhieuMuon = @MaPhieuMuon AND MaSach = @MaSach";
                        cmd = new SqlCommand(checkSql, con);
                        cmd.Parameters.AddWithValue("@MaPhieuMuon", MaPhieuMuon);
                        cmd.Parameters.AddWithValue("@MaSach", maSach);
                        int count = (int)cmd.ExecuteScalar();

                        if (count > 0)
                        {
                            MessageBox.Show($"Sách '{maSach}' đã có trong phiếu mượn! ",
                                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            failCount++;
                            continue;
                        }

                        string sqlMoTa = "SELECT MoTa FROM CuonSach WHERE MaSach = @MaSach";
                        cmd = new SqlCommand(sqlMoTa, con);
                        cmd.Parameters.AddWithValue("@MaSach", maSach);
                        string moTa = cmd.ExecuteScalar()?.ToString() ?? "";

                        string sqlGiaBia = @"SELECT ds.GiaBia FROM CuonSach cs 
                                             JOIN DauSach ds ON cs.MaDauSach = ds.MaDauSach 
                                             WHERE cs.MaSach = @MaSach";
                        cmd = new SqlCommand(sqlGiaBia, con);
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@MaSach", maSach);
                        object giaBiaObj = cmd.ExecuteScalar();

                        if (giaBiaObj == null || giaBiaObj == DBNull.Value)
                        {
                            MessageBox.Show($"Không tìm thấy giá bìa cho sách '{maSach}'!",
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            failCount++;
                            continue;
                        }

                        decimal giaBia = Convert.ToDecimal(giaBiaObj);
                        decimal tienCoc = giaBia * 2;

                        string sqlInsert = @"INSERT INTO CT_PhieuMuon (MaPhieuMuon, MaSach, TienCoc, TinhTrangMuon) 
                                             VALUES (@MaPhieuMuon, @MaSach, @TienCoc, @TinhTrangMuon)";
                        cmd = new SqlCommand(sqlInsert, con);
                        cmd.Parameters.AddWithValue("@MaPhieuMuon", MaPhieuMuon);
                        cmd.Parameters.AddWithValue("@MaSach", maSach);
                        cmd.Parameters.AddWithValue("@TienCoc", tienCoc);
                        cmd.Parameters.AddWithValue("@TinhTrangMuon", moTa);

                        cmd.ExecuteNonQuery();
                        successCount++;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi thêm sách '{maSach}':\n{ex.Message}",
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    failCount++;
                }
            }

            if (successCount > 0)
            {
                MessageBox.Show($"Đã thêm {successCount} sách thành công!" +
                    (failCount > 0 ? $"\n{failCount} sách không thể thêm." : ""),
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadCuonSach();
                LoadSachMuon(MaPhieuMuon);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvSachMuon.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một sách để xóa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult rs = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa {dgvSachMuon.SelectedRows.Count} sách đã chọn?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (rs == DialogResult.Yes)
            {
                int successCount = 0;

                using (con = new SqlConnection(strCon))
                {
                    con.Open();

                    foreach (DataGridViewRow row in dgvSachMuon.SelectedRows)
                    {
                        string maSach = row.Cells[0].Value?.ToString();

                        if (string.IsNullOrEmpty(maSach))
                        {
                            continue;
                        }

                        try
                        {
                            string sql = "DELETE FROM CT_PhieuMuon WHERE MaPhieuMuon = @MaPhieuMuon AND MaSach = @MaSach";
                            using (SqlCommand cmd = new SqlCommand(sql, con))
                            {
                                cmd.Parameters.AddWithValue("@MaPhieuMuon", MaPhieuMuon);
                                cmd.Parameters.AddWithValue("@MaSach", maSach);
                                cmd.ExecuteNonQuery();
                            }

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

                            successCount++;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Lỗi khi xóa sách '{maSach}':\n{ex.Message}",
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }

                if (successCount > 0)
                {
                    MessageBox.Show($"Đã xóa {successCount} sách thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadCuonSach();
                    LoadSachMuon(MaPhieuMuon);
                }
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            try
            {
                int successCount = 0;

                using (SqlConnection con = new SqlConnection(strCon))
                {
                    con.Open();

                    foreach (DataGridViewRow row in dgvSachMuon.Rows)
                    {
                        if (row.IsNewRow)
                        {
                            continue;
                        }

                        string maSach = row.Cells[0].Value?.ToString();
                        string tinhTrangMuon = row.Cells["TinhTrangMuon"].Value?.ToString() ?? "";
                        decimal tienCoc = Convert.ToDecimal(row.Cells["TienCoc"].Value);

                        string sql = @"UPDATE CT_PhieuMuon 
                                       SET TinhTrangMuon = @TinhTrangMuon, TienCoc = @TienCoc 
                                       WHERE MaPhieuMuon = @MaPhieuMuon AND MaSach = @MaSach";

                        using (SqlCommand cmd = new SqlCommand(sql, con))
                        {
                            cmd.Parameters.AddWithValue("@MaPhieuMuon", MaPhieuMuon);
                            cmd.Parameters.AddWithValue("@MaSach", maSach);
                            cmd.Parameters.AddWithValue("@TinhTrangMuon", tinhTrangMuon);
                            cmd.Parameters.AddWithValue("@TienCoc", tienCoc);

                            int kq = cmd.ExecuteNonQuery();
                            if (kq > 0)
                            {
                                successCount++;
                            }
                        }
                    }
                }

                if (successCount > 0)
                {
                    MessageBox.Show($"Đã lưu {successCount} sách thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadSachMuon(MaPhieuMuon);
                }
                else
                {
                    MessageBox.Show("Không có dữ liệu nào được cập nhật!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu dữ liệu:\n{ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}