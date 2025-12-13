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
    public partial class UC_DMNhaXB : UserControl
    {
        string strCon = DBConfig.ConnectionString;
        SqlConnection con;
        SqlDataAdapter adapter;
        SqlCommand cmd;
        DataTable dt;
        DataView dv;
        private bool isCreatingNew = false;

        public UC_DMNhaXB()
        {
            InitializeComponent();
        }

        private void UC_DMNhaXB_Load(object sender, EventArgs e)
        {
            dgvNXB.ColumnHeadersDefaultCellStyle.Font = new Font(dgvNXB.Font, FontStyle.Bold);

            // CÀI ĐẶT MÀU DISABLED
            SetupButtonDisabledStyle(btnTaoMoi);
            SetupButtonDisabledStyle(btnThem);
            SetupButtonDisabledStyle(btnSua);
            SetupButtonDisabledStyle(btnXoa);

            LoadNXB();

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

        private void LoadNXB()
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "SELECT * FROM NXB ORDER BY MaNXB";
                adapter = new SqlDataAdapter(sql, con);
                dt = new DataTable();
                adapter.Fill(dt);
            }
            dv = new DataView(dt);
            dgvNXB.DataSource = dv;
        }

        private void dgvNXB_SelectionChanged(object sender, EventArgs e)
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
            if (dgvNXB.CurrentRow != null && dgvNXB.CurrentRow.Index >= 0)
            {
                int i = dgvNXB.CurrentRow.Index;
                txtMaNXB.Text = dgvNXB.Rows[i].Cells["MaNXB"].Value.ToString();
                txtMaNXB.Enabled = false;
                txtTenNXB.Text = dgvNXB.Rows[i].Cells["TenNXB"].Value.ToString();
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
                dv.RowFilter = $"TenNXB LIKE '%{search}%' OR MaNXB LIKE '%{search}%'";
            }
        }

        private void btnTaoMoi_Click(object sender, EventArgs e)
        {
            txtMaNXB.Text = "";
            txtMaNXB.Enabled = true;
            txtTenNXB.Text = "";
            txtMaNXB.Focus();

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

            string maNXB = txtMaNXB.Text.Trim();
            string tenNXB = txtTenNXB.Text.Trim();

            if (string.IsNullOrEmpty(maNXB))
            {
                MessageBox.Show("Vui lòng nhập mã nhà xuất bản!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMaNXB.Focus();
                return;
            }

            if (string.IsNullOrEmpty(tenNXB))
            {
                MessageBox.Show("Vui lòng nhập tên nhà xuất bản!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenNXB.Focus();
                return;
            }

            // KIỂM TRA MÃ ĐÃ TỒN TẠI CHƯA
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string checkSql = "SELECT COUNT(*) FROM NXB WHERE MaNXB = @MaNXB";
                cmd = new SqlCommand(checkSql, con);
                cmd.Parameters.AddWithValue("@MaNXB", maNXB);
                int count = (int)cmd.ExecuteScalar();

                if (count > 0)
                {
                    MessageBox.Show("Mã nhà xuất bản đã tồn tại!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtMaNXB.Focus();
                    txtMaNXB.SelectAll();
                    return;
                }
            }

            // THÊM MỚI
            try
            {
                using (con = new SqlConnection(strCon))
                {
                    con.Open();
                    string sql = "INSERT INTO NXB (MaNXB, TenNXB) VALUES (@MaNXB, @TenNXB)";
                    cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@MaNXB", maNXB);
                    cmd.Parameters.AddWithValue("@TenNXB", tenNXB);

                    int kq = cmd.ExecuteNonQuery();
                    if (kq > 0)
                    {
                        MessageBox.Show("Thêm thành công!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        isCreatingNew = false;
                        LoadNXB();

                        // Chọn dòng vừa thêm
                        for (int i = 0; i < dgvNXB.Rows.Count; i++)
                        {
                            if (dgvNXB.Rows[i].Cells["MaNXB"].Value.ToString() == maNXB)
                            {
                                dgvNXB.ClearSelection();
                                dgvNXB.CurrentCell = dgvNXB.Rows[i].Cells[0];
                                dgvNXB.FirstDisplayedScrollingRowIndex = i;
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
            if (dgvNXB.SelectedRows.Count == 0)
            {
                MessageBox.Show("Chưa chọn bản ghi nào để xóa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult rs = MessageBox.Show("Bạn có chắc chắn muốn xóa các bản ghi đã chọn?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (rs == DialogResult.Yes)
            {
                int currentIndex = dgvNXB.CurrentRow.Index;
                int successCount = 0;

                try
                {
                    using (con = new SqlConnection(strCon))
                    {
                        con.Open();

                        foreach (DataGridViewRow row in dgvNXB.SelectedRows)
                        {
                            string maNXB = row.Cells["MaNXB"].Value.ToString();
                            string sql = "DELETE FROM NXB WHERE MaNXB = @MaNXB";
                            cmd = new SqlCommand(sql, con);
                            cmd.Parameters.AddWithValue("@MaNXB", maNXB);

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
                                    MessageBox.Show($"Không thể xóa nhà xuất bản '{maNXB}' vì đang được sử dụng! ",
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

                LoadNXB();

                if (dgvNXB.Rows.Count > 0)
                {
                    int newIndex = Math.Min(currentIndex, dgvNXB.Rows.Count - 1);
                    dgvNXB.ClearSelection();
                    dgvNXB.CurrentCell = dgvNXB.Rows[newIndex].Cells[0];
                    dgvNXB.FirstDisplayedScrollingRowIndex = newIndex;
                    NapCT();
                    EnableButtons(true, true, true, true);  // SÁNG HẾT
                }
                else
                {
                    txtMaNXB.Text = "";
                    txtTenNXB.Text = "";
                    EnableButtons(true, true, false, false);  // TẮT SỬA/XÓA
                }
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            string maNXB = txtMaNXB.Text.Trim();
            string tenNXB = txtTenNXB.Text.Trim();

            if (string.IsNullOrEmpty(maNXB))
            {
                MessageBox.Show("Vui lòng chọn nhà xuất bản để cập nhật!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(tenNXB))
            {
                MessageBox.Show("Vui lòng nhập tên nhà xuất bản!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenNXB.Focus();
                return;
            }

            int currentIndex = dgvNXB.CurrentRow.Index;

            try
            {
                using (con = new SqlConnection(strCon))
                {
                    con.Open();
                    string sql = "UPDATE NXB SET TenNXB = @TenNXB WHERE MaNXB = @MaNXB";
                    cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@MaNXB", maNXB);
                    cmd.Parameters.AddWithValue("@TenNXB", tenNXB);

                    int kq = cmd.ExecuteNonQuery();
                    if (kq > 0)
                    {
                        MessageBox.Show("Cập nhật thành công!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        LoadNXB();

                        dgvNXB.ClearSelection();
                        dgvNXB.CurrentCell = dgvNXB.Rows[currentIndex].Cells[0];
                        dgvNXB.FirstDisplayedScrollingRowIndex = currentIndex;
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