using QL_ThuVien.User_Management;
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
    public partial class UC_DMChuDe : UserControl
    {
        private readonly string currentUserRole = UserSession.UserRole;

        string strCon = DBConfig.ConnectionString;
        SqlConnection con;
        SqlDataAdapter adapter;
        SqlCommand cmd;
        DataTable dt;
        DataView dv;
        private bool isCreatingNew = false;

        public UC_DMChuDe()
        {
            InitializeComponent();
        }

        private void UC_DMChuDe_Load(object sender, EventArgs e)
        {
            dgvChuDe.ColumnHeadersDefaultCellStyle.Font = new Font(dgvChuDe.Font, FontStyle.Bold);

            // CÀI ĐẶT MÀU DISABLED
            SetupButtonDisabledStyle(btnTaoMoi);
            SetupButtonDisabledStyle(btnThem);
            SetupButtonDisabledStyle(btnSua);
            SetupButtonDisabledStyle(btnXoa);

            LoadChuDe();

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

        private void LoadChuDe()
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "SELECT * FROM ChuDe ORDER BY MaChuDe";
                adapter = new SqlDataAdapter(sql, con);
                dt = new DataTable();
                adapter.Fill(dt);
            }
            dv = new DataView(dt);
            dgvChuDe.DataSource = dv;
        }

        private void dgvChuDe_SelectionChanged(object sender, EventArgs e)
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
            if (dgvChuDe.CurrentRow != null && dgvChuDe.CurrentRow.Index >= 0)
            {
                int i = dgvChuDe.CurrentRow.Index;
                txtMaChuDe.Text = dgvChuDe.Rows[i].Cells["MaChuDe"].Value.ToString();
                txtMaChuDe.Enabled = false;
                txtTenChuDe.Text = dgvChuDe.Rows[i].Cells["TenChuDe"].Value.ToString();
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
                dv.RowFilter = $"TenChuDe LIKE '%{search}%' OR MaChuDe LIKE '%{search}%'";
            }
        }

        private void btnTaoMoi_Click(object sender, EventArgs e)
        {
            txtMaChuDe.Text = "";
            txtMaChuDe.Enabled = true;
            txtTenChuDe.Text = "";
            txtMaChuDe.Focus();

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

            string maChuDe = txtMaChuDe.Text.Trim();
            string tenChuDe = txtTenChuDe.Text.Trim();

            if (string.IsNullOrEmpty(maChuDe))
            {
                MessageBox.Show("Vui lòng nhập mã chủ đề!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMaChuDe.Focus();
                return;
            }

            if (string.IsNullOrEmpty(tenChuDe))
            {
                MessageBox.Show("Vui lòng nhập tên chủ đề!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenChuDe.Focus();
                return;
            }

            // KIỂM TRA MÃ ĐÃ TỒN TẠI CHƯA
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string checkSql = "SELECT COUNT(*) FROM ChuDe WHERE MaChuDe = @MaChuDe";
                cmd = new SqlCommand(checkSql, con);
                cmd.Parameters.AddWithValue("@MaChuDe", maChuDe);
                int count = (int)cmd.ExecuteScalar();

                if (count > 0)
                {
                    MessageBox.Show("Mã chủ đề đã tồn tại!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtMaChuDe.Focus();
                    txtMaChuDe.SelectAll();
                    return;
                }
            }

            // THÊM MỚI
            try
            {
                using (con = new SqlConnection(strCon))
                {
                    con.Open();
                    string sql = "INSERT INTO ChuDe (MaChuDe, TenChuDe) VALUES (@MaChuDe, @TenChuDe)";
                    cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@MaChuDe", maChuDe);
                    cmd.Parameters.AddWithValue("@TenChuDe", tenChuDe);

                    int kq = cmd.ExecuteNonQuery();
                    if (kq > 0)
                    {
                        MessageBox.Show("Thêm thành công!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        isCreatingNew = false;
                        LoadChuDe();

                        // Chọn dòng vừa thêm
                        for (int i = 0; i < dgvChuDe.Rows.Count; i++)
                        {
                            if (dgvChuDe.Rows[i].Cells["MaChuDe"].Value.ToString() == maChuDe)
                            {
                                dgvChuDe.ClearSelection();
                                dgvChuDe.CurrentCell = dgvChuDe.Rows[i].Cells[0];
                                dgvChuDe.FirstDisplayedScrollingRowIndex = i;
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
            if (dgvChuDe.SelectedRows.Count == 0)
            {
                MessageBox.Show("Chưa chọn bản ghi nào để xóa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult rs = MessageBox.Show("Bạn có chắc chắn muốn xóa các bản ghi đã chọn?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (rs == DialogResult.Yes)
            {
                int currentIndex = dgvChuDe.CurrentRow.Index;
                int successCount = 0;

                try
                {
                    using (con = new SqlConnection(strCon))
                    {
                        con.Open();

                        foreach (DataGridViewRow row in dgvChuDe.SelectedRows)
                        {
                            string maChuDe = row.Cells["MaChuDe"].Value.ToString();
                            string sql = "DELETE FROM ChuDe WHERE MaChuDe = @MaChuDe";
                            cmd = new SqlCommand(sql, con);
                            cmd.Parameters.AddWithValue("@MaChuDe", maChuDe);

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
                                    MessageBox.Show($"Không thể xóa chủ đề '{maChuDe}' vì đang được sử dụng! ",
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

                LoadChuDe();

                if (dgvChuDe.Rows.Count > 0)
                {
                    int newIndex = Math.Min(currentIndex, dgvChuDe.Rows.Count - 1);
                    dgvChuDe.ClearSelection();
                    dgvChuDe.CurrentCell = dgvChuDe.Rows[newIndex].Cells[0];
                    dgvChuDe.FirstDisplayedScrollingRowIndex = newIndex;

                    NapCT();
                    EnableButtons(true, true, true, true);  // SÁNG HẾT
                }
                else
                {
                    txtMaChuDe.Text = "";
                    txtTenChuDe.Text = "";
                    EnableButtons(true, true, false, false);  // TẮT SỬA/XÓA
                }
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            string maChuDe = txtMaChuDe.Text.Trim();
            string tenChuDe = txtTenChuDe.Text.Trim();

            if (string.IsNullOrEmpty(maChuDe))
            {
                MessageBox.Show("Vui lòng chọn chủ đề để cập nhật!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(tenChuDe))
            {
                MessageBox.Show("Vui lòng nhập tên chủ đề!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenChuDe.Focus();
                return;
            }

            int currentIndex = dgvChuDe.CurrentRow.Index;

            try
            {
                using (con = new SqlConnection(strCon))
                {
                    con.Open();
                    string sql = "UPDATE ChuDe SET TenChuDe = @TenChuDe WHERE MaChuDe = @MaChuDe";
                    cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@MaChuDe", maChuDe);
                    cmd.Parameters.AddWithValue("@TenChuDe", tenChuDe);

                    int kq = cmd.ExecuteNonQuery();
                    if (kq > 0)
                    {
                        MessageBox.Show("Cập nhật thành công!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        LoadChuDe();

                        dgvChuDe.ClearSelection();
                        dgvChuDe.CurrentCell = dgvChuDe.Rows[currentIndex].Cells[0];
                        dgvChuDe.FirstDisplayedScrollingRowIndex = currentIndex;

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