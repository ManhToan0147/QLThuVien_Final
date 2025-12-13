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
    public partial class UC_DMKhoSach : UserControl
    {
        string strCon = DBConfig.ConnectionString;
        SqlConnection con;
        SqlDataAdapter adapter;
        SqlCommand cmd;
        DataTable dt;
        DataView dv;
        private bool isCreatingNew = false;

        public UC_DMKhoSach()
        {
            InitializeComponent();
        }

        private void UC_DMKhoSach_Load(object sender, EventArgs e)
        {
            dgvKhoSach.ColumnHeadersDefaultCellStyle.Font = new Font(dgvKhoSach.Font, FontStyle.Bold);

            // CÀI ĐẶT MÀU DISABLED
            SetupButtonDisabledStyle(btnTaoMoi);
            SetupButtonDisabledStyle(btnThem);
            SetupButtonDisabledStyle(btnSua);
            SetupButtonDisabledStyle(btnXoa);

            LoadKhoSach();

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

        private void LoadKhoSach()
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "SELECT * FROM KhoSach ORDER BY MaKho";
                adapter = new SqlDataAdapter(sql, con);
                dt = new DataTable();
                adapter.Fill(dt);
            }
            dv = new DataView(dt);
            dgvKhoSach.DataSource = dv;
        }

        private void dgvKhoSach_SelectionChanged(object sender, EventArgs e)
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
            if (dgvKhoSach.CurrentRow != null && dgvKhoSach.CurrentRow.Index >= 0)
            {
                int i = dgvKhoSach.CurrentRow.Index;
                txtMaKho.Text = dgvKhoSach.Rows[i].Cells["MaKho"].Value.ToString();
                txtMaKho.Enabled = false;
                txtTenKho.Text = dgvKhoSach.Rows[i].Cells["TenKho"].Value.ToString();
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
                dv.RowFilter = $"TenKho LIKE '%{search}%' OR MaKho LIKE '%{search}%'";
            }
        }

        private void btnTaoMoi_Click(object sender, EventArgs e)
        {
            txtMaKho.Text = "";
            txtMaKho.Enabled = true;
            txtTenKho.Text = "";
            txtMaKho.Focus();

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

            string maKho = txtMaKho.Text.Trim();
            string tenKho = txtTenKho.Text.Trim();

            if (string.IsNullOrEmpty(maKho))
            {
                MessageBox.Show("Vui lòng nhập mã kho!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMaKho.Focus();
                return;
            }

            if (string.IsNullOrEmpty(tenKho))
            {
                MessageBox.Show("Vui lòng nhập tên kho!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenKho.Focus();
                return;
            }

            // KIỂM TRA MÃ ĐÃ TỒN TẠI CHƯA
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string checkSql = "SELECT COUNT(*) FROM KhoSach WHERE MaKho = @MaKho";
                cmd = new SqlCommand(checkSql, con);
                cmd.Parameters.AddWithValue("@MaKho", maKho);
                int count = (int)cmd.ExecuteScalar();

                if (count > 0)
                {
                    MessageBox.Show("Mã kho đã tồn tại!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtMaKho.Focus();
                    txtMaKho.SelectAll();
                    return;
                }
            }

            // THÊM MỚI
            try
            {
                using (con = new SqlConnection(strCon))
                {
                    con.Open();
                    string sql = "INSERT INTO KhoSach (MaKho, TenKho) VALUES (@MaKho, @TenKho)";
                    cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@MaKho", maKho);
                    cmd.Parameters.AddWithValue("@TenKho", tenKho);

                    int kq = cmd.ExecuteNonQuery();
                    if (kq > 0)
                    {
                        MessageBox.Show("Thêm thành công!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        isCreatingNew = false;
                        LoadKhoSach();

                        // Chọn dòng vừa thêm
                        for (int i = 0; i < dgvKhoSach.Rows.Count; i++)
                        {
                            if (dgvKhoSach.Rows[i].Cells["MaKho"].Value.ToString() == maKho)
                            {
                                dgvKhoSach.ClearSelection();
                                dgvKhoSach.CurrentCell = dgvKhoSach.Rows[i].Cells[0];
                                dgvKhoSach.FirstDisplayedScrollingRowIndex = i;
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
            if (dgvKhoSach.SelectedRows.Count == 0)
            {
                MessageBox.Show("Chưa chọn bản ghi nào để xóa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult rs = MessageBox.Show("Bạn có chắc chắn muốn xóa các bản ghi đã chọn? ", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (rs == DialogResult.Yes)
            {
                int currentIndex = dgvKhoSach.CurrentRow.Index;
                int successCount = 0;

                try
                {
                    using (con = new SqlConnection(strCon))
                    {
                        con.Open();

                        foreach (DataGridViewRow row in dgvKhoSach.SelectedRows)
                        {
                            string maKho = row.Cells["MaKho"].Value.ToString();
                            string sql = "DELETE FROM KhoSach WHERE MaKho = @MaKho";
                            cmd = new SqlCommand(sql, con);
                            cmd.Parameters.AddWithValue("@MaKho", maKho);

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
                                    MessageBox.Show($"Không thể xóa kho '{maKho}' vì đang được sử dụng! ",
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

                LoadKhoSach();

                if (dgvKhoSach.Rows.Count > 0)
                {
                    int newIndex = Math.Min(currentIndex, dgvKhoSach.Rows.Count - 1);
                    dgvKhoSach.ClearSelection();
                    dgvKhoSach.CurrentCell = dgvKhoSach.Rows[newIndex].Cells[0];
                    dgvKhoSach.FirstDisplayedScrollingRowIndex = newIndex;

                    NapCT();
                    EnableButtons(true, true, true, true);  // SÁNG HẾT
                }
                else
                {
                    txtMaKho.Text = "";
                    txtTenKho.Text = "";
                    EnableButtons(true, true, false, false);  // TẮT SỬA/XÓA
                }
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            string maKho = txtMaKho.Text.Trim();
            string tenKho = txtTenKho.Text.Trim();

            if (string.IsNullOrEmpty(maKho))
            {
                MessageBox.Show("Vui lòng chọn kho để cập nhật!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(tenKho))
            {
                MessageBox.Show("Vui lòng nhập tên kho!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenKho.Focus();
                return;
            }

            int currentIndex = dgvKhoSach.CurrentRow.Index;

            try
            {
                using (con = new SqlConnection(strCon))
                {
                    con.Open();
                    string sql = "UPDATE KhoSach SET TenKho = @TenKho WHERE MaKho = @MaKho";
                    cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@MaKho", maKho);
                    cmd.Parameters.AddWithValue("@TenKho", tenKho);

                    int kq = cmd.ExecuteNonQuery();
                    if (kq > 0)
                    {
                        MessageBox.Show("Cập nhật thành công!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        LoadKhoSach();

                        dgvKhoSach.ClearSelection();
                        dgvKhoSach.CurrentCell = dgvKhoSach.Rows[currentIndex].Cells[0];
                        dgvKhoSach.FirstDisplayedScrollingRowIndex = currentIndex;

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