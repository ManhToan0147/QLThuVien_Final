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
    public partial class UC_DMTacGia : UserControl
    {
        string strCon = DBConfig.ConnectionString;
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataTable dt;
        DataView dv;
        private bool isCreatingNew = false;

        public UC_DMTacGia()
        {
            InitializeComponent();
        }

        private void UC_DMTacGia_Load(object sender, EventArgs e)
        {
            dgvTacGia.ColumnHeadersDefaultCellStyle.Font = new Font(dgvTacGia.Font, FontStyle.Bold);

            // CÀI ĐẶT MÀU DISABLED
            SetupButtonDisabledStyle(btnTaoMoi);
            SetupButtonDisabledStyle(btnThem);
            SetupButtonDisabledStyle(btnSua);
            SetupButtonDisabledStyle(btnXoa);

            LoadTacGia();

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

        private void LoadTacGia()
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "SELECT * FROM TacGia ORDER BY MaTG";
                adapter = new SqlDataAdapter(sql, con);
                dt = new DataTable();
                adapter.Fill(dt);
            }
            dv = new DataView(dt);
            dgvTacGia.DataSource = dv;
        }

        private void dgvTacGia_SelectionChanged(object sender, EventArgs e)
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
            if (dgvTacGia.CurrentRow != null && dgvTacGia.CurrentRow.Index >= 0)
            {
                int i = dgvTacGia.CurrentRow.Index;
                txtMaTacGia.Text = dgvTacGia.Rows[i].Cells["MaTG"].Value.ToString();
                txtMaTacGia.Enabled = false;
                txtTenTacGia.Text = dgvTacGia.Rows[i].Cells["TenTG"].Value.ToString();

                // XỬ LÝ NULL
                cboGioiTinh.Text = dgvTacGia.Rows[i].Cells["GioiTinh"].Value != DBNull.Value
                    ? dgvTacGia.Rows[i].Cells["GioiTinh"].Value.ToString()
                    : "";

                txtNamSinh.Text = dgvTacGia.Rows[i].Cells["NamSinh"].Value != DBNull.Value
                    ? dgvTacGia.Rows[i].Cells["NamSinh"].Value.ToString()
                    : "";
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
                dv.RowFilter = $"TenTG LIKE '%{search}%' OR MaTG LIKE '%{search}%'";
            }
        }

        private void btnTaoMoi_Click(object sender, EventArgs e)
        {
            // SINH MÃ TỰ ĐỘNG
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "SELECT MAX(MaTG) FROM TacGia";
                cmd = new SqlCommand(sql, con);
                object rs = cmd.ExecuteScalar();

                if (rs != DBNull.Value && rs != null)
                {
                    string maTacGia = rs.ToString();
                    int number = int.Parse(maTacGia.Substring(2));
                    number++;
                    txtMaTacGia.Text = "TG" + number.ToString("D2");
                }
                else
                {
                    txtMaTacGia.Text = "TG01";
                }
            }

            txtMaTacGia.Enabled = false;
            txtTenTacGia.Text = "";
            cboGioiTinh.SelectedIndex = -1;
            txtNamSinh.Text = "";
            txtTenTacGia.Focus();

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

            string maTG = txtMaTacGia.Text.Trim();
            string tenTG = txtTenTacGia.Text.Trim();

            // CHỈ KIỂM TRA TÊN TÁC GIẢ
            if (string.IsNullOrEmpty(tenTG))
            {
                MessageBox.Show("Vui lòng nhập tên tác giả!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenTacGia.Focus();
                return;
            }

            // KIỂM TRA MÃ ĐÃ TỒN TẠI CHƯA
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string checkSql = "SELECT COUNT(*) FROM TacGia WHERE MaTG = @MaTG";
                cmd = new SqlCommand(checkSql, con);
                cmd.Parameters.AddWithValue("@MaTG", maTG);
                int count = (int)cmd.ExecuteScalar();

                if (count > 0)
                {
                    MessageBox.Show("Mã tác giả đã tồn tại!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtMaTacGia.Focus();
                    txtMaTacGia.SelectAll();
                    return;
                }
            }

            try
            {
                string gioiTinh = cboGioiTinh.Text.Trim();
                string namSinh = txtNamSinh.Text.Trim();

                using (con = new SqlConnection(strCon))
                {
                    con.Open();
                    string query = "INSERT INTO TacGia (MaTG, TenTG, GioiTinh, NamSinh) VALUES (@MaTG, @TenTG, @GioiTinh, @NamSinh)";
                    cmd = new SqlCommand(query, con);

                    cmd.Parameters.AddWithValue("@MaTG", maTG);
                    cmd.Parameters.AddWithValue("@TenTG", tenTG);

                    // INSERT NULL NẾU TRỐNG
                    cmd.Parameters.AddWithValue("@GioiTinh", string.IsNullOrEmpty(gioiTinh) ? (object)DBNull.Value : gioiTinh);
                    cmd.Parameters.AddWithValue("@NamSinh", string.IsNullOrEmpty(namSinh) ? (object)DBNull.Value : namSinh);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Thêm thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                isCreatingNew = false;
                LoadTacGia();

                // Chọn dòng vừa thêm
                for (int i = 0; i < dgvTacGia.Rows.Count; i++)
                {
                    if (dgvTacGia.Rows[i].Cells["MaTG"].Value.ToString() == maTG)
                    {
                        dgvTacGia.ClearSelection();
                        dgvTacGia.CurrentCell = dgvTacGia.Rows[i].Cells[0];
                        dgvTacGia.FirstDisplayedScrollingRowIndex = i;
                        break;
                    }
                }
                NapCT();
                EnableButtons(true, true, true, true);  // SÁNG HẾT
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm:  " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvTacGia.SelectedRows.Count == 0)
            {
                MessageBox.Show("Chưa chọn bản ghi nào để xóa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult rs = MessageBox.Show("Bạn có chắc chắn muốn xóa các bản ghi đã chọn? ", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (rs == DialogResult.Yes)
            {
                int currentIndex = dgvTacGia.CurrentRow.Index;
                int successCount = 0;

                try
                {
                    using (con = new SqlConnection(strCon))
                    {
                        con.Open();

                        foreach (DataGridViewRow row in dgvTacGia.SelectedRows)
                        {
                            string maTG = row.Cells["MaTG"].Value.ToString();
                            string sql = "DELETE FROM TacGia WHERE MaTG = @MaTG";
                            cmd = new SqlCommand(sql, con);
                            cmd.Parameters.AddWithValue("@MaTG", maTG);

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
                                    MessageBox.Show($"Không thể xóa tác giả '{maTG}' vì đang được sử dụng! ",
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

                LoadTacGia();

                if (dgvTacGia.Rows.Count > 0)
                {
                    int newIndex = Math.Min(currentIndex, dgvTacGia.Rows.Count - 1);
                    dgvTacGia.ClearSelection();
                    dgvTacGia.CurrentCell = dgvTacGia.Rows[newIndex].Cells[0];
                    dgvTacGia.FirstDisplayedScrollingRowIndex = newIndex;

                    NapCT();
                    EnableButtons(true, true, true, true);  // SÁNG HẾT
                }
                else
                {
                    txtMaTacGia.Text = "";
                    txtTenTacGia.Text = "";
                    cboGioiTinh.SelectedIndex = -1;
                    txtNamSinh.Text = "";
                    EnableButtons(true, true, false, false);  // TẮT SỬA/XÓA
                }
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            string maTG = txtMaTacGia.Text.Trim();
            string tenTG = txtTenTacGia.Text.Trim();

            if (string.IsNullOrEmpty(maTG))
            {
                MessageBox.Show("Vui lòng chọn tác giả để cập nhật!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // CHỈ KIỂM TRA TÊN TÁC GIẢ
            if (string.IsNullOrEmpty(tenTG))
            {
                MessageBox.Show("Vui lòng nhập tên tác giả!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenTacGia.Focus();
                return;
            }

            int currentIndex = dgvTacGia.CurrentRow.Index;

            try
            {
                string gioiTinh = cboGioiTinh.Text.Trim();
                string namSinh = txtNamSinh.Text.Trim();

                using (con = new SqlConnection(strCon))
                {
                    con.Open();
                    string sql = "UPDATE TacGia SET TenTG = @TenTG, GioiTinh = @GioiTinh, NamSinh = @NamSinh WHERE MaTG = @MaTG";
                    cmd = new SqlCommand(sql, con);

                    cmd.Parameters.AddWithValue("@MaTG", maTG);
                    cmd.Parameters.AddWithValue("@TenTG", tenTG);

                    // UPDATE NULL NẾU TRỐNG
                    cmd.Parameters.AddWithValue("@GioiTinh", string.IsNullOrEmpty(gioiTinh) ? (object)DBNull.Value : gioiTinh);
                    cmd.Parameters.AddWithValue("@NamSinh", string.IsNullOrEmpty(namSinh) ? (object)DBNull.Value : namSinh);

                    int kq = cmd.ExecuteNonQuery();
                    if (kq > 0)
                    {
                        MessageBox.Show("Cập nhật thành công!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        LoadTacGia();

                        dgvTacGia.ClearSelection();
                        dgvTacGia.CurrentCell = dgvTacGia.Rows[currentIndex].Cells[0];
                        dgvTacGia.FirstDisplayedScrollingRowIndex = currentIndex;
                        NapCT();
                        EnableButtons(true, true, true, true);  // SÁNG HẾT
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật:  " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtNamSinh_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                errorProvider1.SetError((Control)sender, "Chỉ được nhập số!");
            }
            else
            {
                errorProvider1.SetError((Control)sender, "");
            }
        }
    }
}