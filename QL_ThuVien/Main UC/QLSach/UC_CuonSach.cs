using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QL_ThuVien.Main_UC.QLSach
{
    public partial class UC_CuonSach : UserControl
    {
        string strCon = DBConfig.ConnectionString;
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataTable dt;
        DataView dv;
        bool addNewFlag = false;

        string role;
        public UC_CuonSach(string role)
        {
            InitializeComponent();
            this.role = role;
        }

        private void UC_CuonSach_Load(object sender, EventArgs e)
        {
            if (role != "admin" && role != "thuthu")
            {
                btnThem.Visible = false;
                btnXoa.Visible = false;
                btnSua.Visible = false;
            }
            //dgvDauSach.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            //dgvCuonSach.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            dgvDauSach.DefaultCellStyle.Font = new Font(dgvDauSach.Font, FontStyle.Regular);
            dgvCuonSach.DefaultCellStyle.Font = new Font(dgvCuonSach.Font, FontStyle.Regular);
            cboTruong.SelectedIndex = 0;
            showDauSach();
            showCuonSach();
        }

        private void showCuonSach()
        {
            using (con = new SqlConnection(strCon))
            {
                string sql = "Select MaSach, TinhTrang, MoTa from CuonSach";
                adapter = new SqlDataAdapter(sql, con);
                dt = new DataTable();
                adapter.Fill(dt);
            }
            dgvCuonSach.DataSource = dt;
        }

        private void showDauSach()
        {
            using (con = new SqlConnection(strCon))
            {
                string sql = "Select MaDauSach, TenDauSach, ks.TenKho from DauSach ds left join KhoSach ks on ds.MaKho = ks.MaKho";
                adapter = new SqlDataAdapter(sql, con);
                dt = new DataTable();
                adapter.Fill(dt);
            }
            dv = new DataView(dt);
            dgvDauSach.DataSource = dv;
        }

        private string selectedMaDauSach;
        private void ShowCuonSach_DauSach()
        {
            if (dgvDauSach.CurrentRow != null && dgvDauSach.CurrentRow.Index >= 0)
            {
                int i = dgvDauSach.CurrentRow.Index;
                var maDauSachCell = dgvDauSach.Rows[i].Cells["MaDauSach"].Value;
                if (maDauSachCell != null && !string.IsNullOrWhiteSpace(maDauSachCell.ToString()))
                {
                    selectedMaDauSach = maDauSachCell.ToString();

                    using (con = new SqlConnection(strCon))
                    {
                        con.Open();
                        string sql = $"Select MaSach, TinhTrang, MoTa from CuonSach where MaDauSach = '{selectedMaDauSach}'";
                        using (adapter = new SqlDataAdapter(sql, con))
                        {
                            adapter.SelectCommand.Parameters.AddWithValue("@MaDauSach", selectedMaDauSach);
                            dt = new DataTable();
                            adapter.Fill(dt);
                            dgvCuonSach.DataSource = dt;
                        }
                    }
                }
                else
                {
                    showCuonSach();
                }
            }
        }

        private void btnXemChiTiet_Click(object sender, EventArgs e)
        {
            if (dgvDauSach.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dgvDauSach.SelectedRows[0]; // Lấy hàng được chọn
                string maDauSach;

                if (selectedRow.Cells["MaDauSach"].Value != null)
                {
                    maDauSach = selectedRow.Cells["MaDauSach"].Value.ToString();
                    // Khởi tạo form chi tiết và truyền mã đầu sách qua constructor
                    ChiTietSach cts = new ChiTietSach(maDauSach);
                    cts.ShowDialog(); 
                }
                else
                {
                    MessageBox.Show("Dòng được chọn không chứa thông tin.");
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một đầu sách để xem chi tiết.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (cboTruong.SelectedIndex == 0)
            {
                dv.RowFilter = $"MaDauSach like '%{txtSearch.Text}%'";
            }
            else
            {
                dv.RowFilter = $"TenDauSach like '%{txtSearch.Text}%'";
            }
        }

        void DoSQL(string sql)
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                cmd = new SqlCommand(sql, con);
                cmd.ExecuteNonQuery();
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedMaDauSach))
            {
                MessageBox.Show("Vui lòng chọn một đầu sách trước khi thêm các cuốn sách.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Lấy số lượng từ NumericUpDown
            int soLuong = (int)numSoLuongNhap.Value;

            if (soLuong <= 0)
            {
                MessageBox.Show("Số lượng phải lớn hơn 0!",
                    "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                numSoLuongNhap.Focus();
                return;
            }

            // Xác nhận nếu thêm nhiều hơn 1 cuốn
            if (soLuong > 1)
            {
                DialogResult rs = MessageBox.Show(
                    $"Thêm {soLuong} cuốn sách cho đầu sách '{selectedMaDauSach}'?",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (rs == DialogResult.No) return;
            }

            // Thực hiện thêm
            int successCount = 0;

            try
            {
                using (con = new SqlConnection(strCon))
                {
                    con.Open();

                    for (int i = 0; i < soLuong; i++)
                    {
                        // Sinh mã sách tự động
                        string insertMaSach;
                        string sql1 = $"SELECT MAX(MaSach) FROM CuonSach WHERE MaDauSach = '{selectedMaDauSach}'";
                        cmd = new SqlCommand(sql1, con);
                        object rs = cmd.ExecuteScalar();

                        if (rs != DBNull.Value && rs != null)
                        {
                            string maSach = rs.ToString();
                            int number = int.Parse(maSach.Substring(maSach.IndexOf('_') + 1)); // Lấy số sau dấu "_"
                            number++;

                            // CHẶN GIỚI HẠN 99
                            if (number >= 100)
                            {
                                MessageBox.Show(
                                    $"Đầu sách '{selectedMaDauSach}' đã đạt giới hạn 99 cuốn!\n\n" +
                                    $"Đã thêm được {successCount} cuốn trước khi đạt giới hạn.",
                                    "Cảnh báo",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                                break;
                            }

                            insertMaSach = selectedMaDauSach + "_" + number.ToString("D2");
                        }
                        else
                        {
                            // TH không có bản ghi nào thì bắt đầu từ 01
                            insertMaSach = selectedMaDauSach + "_01";
                        }

                        // INSERT
                        string sql2 = $"INSERT INTO CuonSach VALUES ('{insertMaSach}', '{selectedMaDauSach}', N'Còn', N'OK')";
                        cmd = new SqlCommand(sql2, con);
                        cmd.ExecuteNonQuery();

                        successCount++;
                    }
                }

                // Thông báo kết quả
                if (soLuong > 1)
                {
                    MessageBox.Show($"Đã thêm thành công {successCount} cuốn sách!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }

                // Reload
                ShowCuonSach_DauSach();

                // Reset về 1
                numSoLuongNhap.Value = 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvCuonSach.SelectedRows.Count > 0)
            {
                DialogResult rs = MessageBox.Show("Bạn có chắc chắn muốn xóa bản ghi được lựa chọn?", "Xác nhận yêu cầu",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (rs == DialogResult.Yes)
                {
                    int count = 0;
                    foreach (DataGridViewRow row in dgvCuonSach.SelectedRows)
                    {
                        string maSach = row.Cells["MaSach"].Value?.ToString() ?? string.Empty;
                        string sql = $"Delete from CuonSach where MaSach = '{maSach}'";
                        using (con = new SqlConnection(strCon))
                        {
                            con.Open();
                            cmd = new SqlCommand(sql, con);
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                count++;
                            }
                        }
                    }
                    MessageBox.Show($"Đã xóa thành công {count} bản ghi.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ShowCuonSach_DauSach();
                }
            }
            else
            {
                MessageBox.Show("Chưa chọn bản ghi");
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedMaDauSach))
            {
                MessageBox.Show("Vui lòng chọn một đầu sách trước khi sửa các cuốn sách.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (con = new SqlConnection(strCon))
            {
                int records = 0;
                con.Open();
                foreach (DataGridViewRow row in dgvCuonSach.Rows)
                {

                    if (row.IsNewRow || row.Cells["MaSach"].Value == null || row.Cells["TinhTrang"].Value == null)
                    {
                        continue;
                    }

                    string maSach = row.Cells["MaSach"].Value.ToString();
                    string tinhTrang = row.Cells["TinhTrang"].Value.ToString();
                    string moTa = row.Cells["MoTa"].Value.ToString();
                    string sql = "Update CuonSach set TinhTrang = @TinhTrang, MoTa = @MoTa where MaSach = @MaSach";

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@MaSach", maSach);
                        cmd.Parameters.AddWithValue("@TinhTrang", tinhTrang);
                        cmd.Parameters.AddWithValue("@MoTa", moTa);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            records++;
                        }
                    }
                }
                MessageBox.Show($"Đã sửa thành công bản ghi", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            ShowCuonSach_DauSach();
        }

        private void dgvDauSach_SelectionChanged_1(object sender, EventArgs e)
        {
            ShowCuonSach_DauSach();
        }

        private void cboTruong_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboTruong.SelectedIndex == 0 )
            {
                txtSearch.PlaceholderText = "Nhập mã đầu sách để tìm kiếm";
            }
            else
            {
                txtSearch.PlaceholderText = "Nhập tên đầu sách để tìm kiếm";
            }
        }
    }
}
