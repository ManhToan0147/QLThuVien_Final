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

namespace QL_ThuVien.Main_UC.QLDanhMuc
{
    public partial class UC_DMLoaiSach : UserControl
    {
        string strCon = DBConfig.ConnectionString;
        SqlConnection con;
        SqlDataAdapter adapter;
        SqlCommand cmd;
        DataTable dt;
        DataView dv;
        private bool isCreatingNew = false;

        public UC_DMLoaiSach()
        {
            InitializeComponent();
        }

        private void UC_DMLoaiSach_Load(object sender, EventArgs e)
        {
            dgvLoaiSach.ColumnHeadersDefaultCellStyle.Font = new Font(dgvLoaiSach.Font, FontStyle.Bold);

            // CÀI ĐẶT MÀU DISABLED
            SetupButtonDisabledStyle(btnTaoMoi);
            SetupButtonDisabledStyle(btnThem);
            SetupButtonDisabledStyle(btnSua);
            SetupButtonDisabledStyle(btnXoa);

            LoadLoaiSach();

            // TẤT CẢ NÚT SÁNG
            EnableButtons(true, true, true, true);
        }

        private void SetupButtonDisabledStyle(dynamic btn)
        {
            btn.DisabledState.BorderColor = Color.FromArgb(180, 210, 230);
            btn.DisabledState.CustomBorderColor = Color.FromArgb(200, 200, 200);
            btn.DisabledState.FillColor = Color.FromArgb(240, 240, 240);
            btn.DisabledState.ForeColor = Color.FromArgb(160, 160, 160);
        }

        private void EnableButtons(bool taoMoi, bool them, bool sua, bool xoa)
        {
            btnTaoMoi.Enabled = taoMoi;
            btnThem.Enabled = them;
            btnSua.Enabled = sua;
            btnXoa.Enabled = xoa;
        }

        private void LoadLoaiSach()
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "SELECT * FROM LoaiSach ORDER BY MaLoaiSach";
                adapter = new SqlDataAdapter(sql, con);
                dt = new DataTable();
                adapter.Fill(dt);
            }
            dv = new DataView(dt);
            dgvLoaiSach.DataSource = dv;
        }

        private void dgvLoaiSach_SelectionChanged(object sender, EventArgs e)
        {
            if (isCreatingNew)
            {
                DialogResult result = MessageBox.Show(
                    "Bạn có muốn hủy tạo mới? ",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    isCreatingNew = false;
                    NapCT();
                    EnableButtons(true, true, true, true);  // SÁNG HẾT
                }
                else
                {
                    return;
                }
            }
            else
            {
                NapCT();
            }
        }

        private void NapCT()
        {
            if (dgvLoaiSach.CurrentRow != null && dgvLoaiSach.CurrentRow.Index >= 0)
            {
                int i = dgvLoaiSach.CurrentRow.Index;
                txtMaLoaiSach.Text = dgvLoaiSach.Rows[i].Cells["MaLoaiSach"].Value.ToString();
                txtMaLoaiSach.Enabled = false;
                txtTenLoaiSach.Text = dgvLoaiSach.Rows[i].Cells["TenLoaiSach"].Value.ToString();
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string search = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(search))
            {
                dv.RowFilter = "";
            }
            else
            {
                dv.RowFilter = $"TenLoaiSach LIKE '%{search}%' OR MaLoaiSach LIKE '%{search}%'";
            }
        }

        private void btnTaoMoi_Click(object sender, EventArgs e)
        {
            txtMaLoaiSach.Text = "";
            txtMaLoaiSach.Enabled = true;
            txtTenLoaiSach.Text = "";
            txtMaLoaiSach.Focus();

            isCreatingNew = true;
            EnableButtons(true, true, false, false);  // TẮT SỬA/XÓA
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            // KIỂM TRA PHẢI BẤM TẠO MỚI TRƯỚC
            if (!isCreatingNew)
            {
                MessageBox.Show("Vui lòng bấm 'Tạo mới' trước khi thêm!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string maLoaiSach = txtMaLoaiSach.Text.Trim();
            string tenLoaiSach = txtTenLoaiSach.Text.Trim();

            if (string.IsNullOrEmpty(maLoaiSach))
            {
                MessageBox.Show("Vui lòng nhập mã loại sách!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMaLoaiSach.Focus();
                return;
            }

            if (string.IsNullOrEmpty(tenLoaiSach))
            {
                MessageBox.Show("Vui lòng nhập tên loại sách!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenLoaiSach.Focus();
                return;
            }

            // KIỂM TRA MÃ ĐÃ TỒN TẠI CHƯA
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string checkSql = "SELECT COUNT(*) FROM LoaiSach WHERE MaLoaiSach = @MaLoaiSach";
                cmd = new SqlCommand(checkSql, con);
                cmd.Parameters.AddWithValue("@MaLoaiSach", maLoaiSach);
                int count = (int)cmd.ExecuteScalar();

                if (count > 0)
                {
                    MessageBox.Show("Mã loại sách đã tồn tại!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtMaLoaiSach.Focus();
                    txtMaLoaiSach.SelectAll();
                    return;
                }
            }

            // THÊM MỚI
            try
            {
                using (con = new SqlConnection(strCon))
                {
                    con.Open();
                    string sql = "INSERT INTO LoaiSach (MaLoaiSach, TenLoaiSach) VALUES (@MaLoaiSach, @TenLoaiSach)";
                    cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@MaLoaiSach", maLoaiSach);
                    cmd.Parameters.AddWithValue("@TenLoaiSach", tenLoaiSach);

                    int kq = cmd.ExecuteNonQuery();
                    if (kq > 0)
                    {
                        MessageBox.Show("Thêm thành công!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        isCreatingNew = false;
                        LoadLoaiSach();

                        // Chọn dòng vừa thêm
                        for (int i = 0; i < dgvLoaiSach.Rows.Count; i++)
                        {
                            if (dgvLoaiSach.Rows[i].Cells["MaLoaiSach"].Value.ToString() == maLoaiSach)
                            {
                                dgvLoaiSach.ClearSelection();
                                dgvLoaiSach.CurrentCell = dgvLoaiSach.Rows[i].Cells[0];
                                dgvLoaiSach.FirstDisplayedScrollingRowIndex = i;
                                break;
                            }
                        }
                        NapCT();
                        EnableButtons(true, true, true, true);  // SÁNG HẾT
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm:  " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvLoaiSach.SelectedRows.Count == 0)
            {
                MessageBox.Show("Chưa chọn bản ghi nào để xóa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult rs = MessageBox.Show("Bạn có chắc chắn muốn xóa các bản ghi đã chọn?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (rs == DialogResult.Yes)
            {
                int currentIndex = dgvLoaiSach.CurrentRow.Index;
                int successCount = 0;

                try
                {
                    using (con = new SqlConnection(strCon))
                    {
                        con.Open();

                        foreach (DataGridViewRow row in dgvLoaiSach.SelectedRows)
                        {
                            string maLoaiSach = row.Cells["MaLoaiSach"].Value.ToString();
                            string sql = "DELETE FROM LoaiSach WHERE MaLoaiSach = @MaLoaiSach";
                            cmd = new SqlCommand(sql, con);
                            cmd.Parameters.AddWithValue("@MaLoaiSach", maLoaiSach);

                            try
                            {
                                int kq = cmd.ExecuteNonQuery();
                                if (kq > 0)
                                {
                                    successCount++;
                                }
                            }
                            catch (SqlException ex)
                            {
                                if (ex.Number == 547)
                                {
                                    MessageBox.Show($"Không thể xóa loại sách '{maLoaiSach}' vì đang được sử dụng! ",
                                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }

                        if (successCount > 0)
                        {
                            MessageBox.Show($"Xóa thành công {successCount} bản ghi.", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa: " + ex.Message, "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                LoadLoaiSach();

                if (dgvLoaiSach.Rows.Count > 0)
                {
                    int newIndex = Math.Min(currentIndex, dgvLoaiSach.Rows.Count - 1);
                    dgvLoaiSach.ClearSelection();
                    dgvLoaiSach.CurrentCell = dgvLoaiSach.Rows[newIndex].Cells[0];
                    dgvLoaiSach.FirstDisplayedScrollingRowIndex = newIndex;

                    NapCT();
                    EnableButtons(true, true, true, true);  // SÁNG HẾT
                }
                else
                {
                    txtMaLoaiSach.Text = "";
                    txtTenLoaiSach.Text = "";
                    EnableButtons(true, true, false, false);  // TẮT SỬA/XÓA (vì không còn data)
                }
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            string maLoaiSach = txtMaLoaiSach.Text.Trim();
            string tenLoaiSach = txtTenLoaiSach.Text.Trim();

            if (string.IsNullOrEmpty(maLoaiSach))
            {
                MessageBox.Show("Vui lòng chọn loại sách để cập nhật!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(tenLoaiSach))
            {
                MessageBox.Show("Vui lòng nhập tên loại sách!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenLoaiSach.Focus();
                return;
            }

            int currentIndex = dgvLoaiSach.CurrentRow.Index;

            try
            {
                using (con = new SqlConnection(strCon))
                {
                    con.Open();
                    string sql = "UPDATE LoaiSach SET TenLoaiSach = @TenLoaiSach WHERE MaLoaiSach = @MaLoaiSach";
                    cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@MaLoaiSach", maLoaiSach);
                    cmd.Parameters.AddWithValue("@TenLoaiSach", tenLoaiSach);

                    int kq = cmd.ExecuteNonQuery();
                    if (kq > 0)
                    {
                        MessageBox.Show("Cập nhật thành công!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        LoadLoaiSach();

                        dgvLoaiSach.ClearSelection();
                        dgvLoaiSach.CurrentCell = dgvLoaiSach.Rows[currentIndex].Cells[0];
                        dgvLoaiSach.FirstDisplayedScrollingRowIndex = currentIndex;

                        NapCT();
                        EnableButtons(true, true, true, true);  // SÁNG HẾT
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}