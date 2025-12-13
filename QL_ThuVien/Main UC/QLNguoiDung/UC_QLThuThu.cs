using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace QL_ThuVien.Main_UC.QLMuonTra
{
    public partial class UC_QLThuThu : UserControl
    {
        string strCon = DBConfig.ConnectionString;
        SqlConnection con;
        SqlDataAdapter adapter;
        SqlCommand cmd;
        DataTable dt;
        DataView dv;
        string selectedMaThuThu;
        private bool isCreatingNew = false;

        public UC_QLThuThu()
        {
            InitializeComponent();
        }

        private void UC_QLThuThu_Load(object sender, EventArgs e)
        {
            dgvThuThu.ColumnHeadersDefaultCellStyle.Font = new Font(dgvThuThu.Font, FontStyle.Bold);
            cboTruong.SelectedIndex = 0;

            // ✅ SETUP BUTTON DISABLED STYLE (CHỈ NÚT THỰC SỰ CẦN)
            SetupButtonDisabledStyle(btnTaoMoi);
            SetupButtonDisabledStyle(btnThem);
            SetupButtonDisabledStyle(btnSua);
            SetupButtonDisabledStyle(btnXoa);

            // ✅ SETUP COMBOBOX TRẠNG THÁI
            SetupComboBoxTrangThai();

            LoadThuThu();
            EnableButtons(true, true, true, true);
        }

        // ✅ HÀM SETUP MÀU DISABLED CHO BUTTON
        private void SetupButtonDisabledStyle(dynamic btn)
        {
            btn.DisabledState.BorderColor = Color.FromArgb(180, 210, 230);
            btn.DisabledState.CustomBorderColor = Color.FromArgb(200, 200, 200);
            btn.DisabledState.FillColor = Color.FromArgb(240, 240, 240);
            btn.DisabledState.ForeColor = Color.FromArgb(160, 160, 160);
        }

        // ✅ BẬT/TẮT BUTTON
        private void EnableButtons(bool taoMoi, bool them, bool sua, bool xoa)
        {
            btnTaoMoi.Enabled = taoMoi;
            btnThem.Enabled = them;
            btnSua.Enabled = sua;
            btnXoa.Enabled = xoa;
        }

        // ✅ SETUP COMBOBOX TRẠNG THÁI
        private void SetupComboBoxTrangThai()
        {
            DataTable dtTrangThai = new DataTable();
            dtTrangThai.Columns.Add("Value", typeof(int));
            dtTrangThai.Columns.Add("Display", typeof(string));
            dtTrangThai.Rows.Add(1, "Hoạt động");
            dtTrangThai.Rows.Add(0, "Bị khóa");

            cboTrangThai.DataSource = dtTrangThai;
            cboTrangThai.DisplayMember = "Display";
            cboTrangThai.ValueMember = "Value";
            cboTrangThai.SelectedValue = 1;
        }

        private void LoadThuThu()
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = @"
                    SELECT 
                        MaThuThu, TenThuThu, HinhAnh, GioiTinh, NgaySinh, 
                        Email, SDT, ChucVu, TrangThai,
                        CASE WHEN TrangThai = 1 THEN N'Hoạt động' ELSE N'Bị khóa' END AS TrangThaiText
                    FROM ThuThu ORDER BY MaThuThu";

                adapter = new SqlDataAdapter(sql, con);
                dt = new DataTable();
                adapter.Fill(dt);
            }

            dv = new DataView(dt);
            dgvThuThu.DataSource = dv;

            // ✅ ẨN CỘT TrangThai (bit)
            dgvThuThu.Columns["TrangThai"].Visible = false;
            dgvThuThu.Columns["HinhAnh"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // ✅ ẢNH MẶC ĐỊNH
            foreach (DataGridViewRow row in dgvThuThu.Rows)
            {
                if (row.Cells["HinhAnh"].Value == DBNull.Value)
                    row.Cells["HinhAnh"].Value = Properties.Resources.avatar_default;
            }

            ((DataGridViewImageColumn)dgvThuThu.Columns["HinhAnh"]).ImageLayout = DataGridViewImageCellLayout.Zoom;
        }

        private void dgvThuThu_SelectionChanged(object sender, EventArgs e)
        {
            if (isCreatingNew)
            {
                DialogResult result = MessageBox.Show("Bạn có muốn hủy tạo mới? ", "Xác nhận",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    isCreatingNew = false;
                    NapCT();
                    EnableButtons(true, true, true, true);
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
            if (dgvThuThu.CurrentCell != null && dgvThuThu.CurrentCell.RowIndex >= 0)
            {
                int i = dgvThuThu.CurrentRow.Index;
                selectedMaThuThu = dgvThuThu.Rows[i].Cells["MaThuThu"]?.Value.ToString() ?? "";

                txtMaThuThu.Text = selectedMaThuThu;
                txtMaThuThu.Enabled = false;

                txtTenThuThu.Text = dgvThuThu.Rows[i].Cells["TenThuThu"]?.Value.ToString() ?? "";
                txtEmail.Text = dgvThuThu.Rows[i].Cells["Email"]?.Value.ToString() ?? "";
                txtSDT.Text = dgvThuThu.Rows[i].Cells["SDT"]?.Value.ToString() ?? "";

                // Giới tính
                string gioiTinh = dgvThuThu.Rows[i].Cells["GioiTinh"]?.Value?.ToString() ?? "";
                if (!string.IsNullOrEmpty(gioiTinh))
                {
                    cboGioiTinh.Text = gioiTinh;
                }

                // ✅ NGÀY SINH - NẾU NULL THÌ LẤY NGÀY HIỆN TẠI
                if (dgvThuThu.Rows[i].Cells["NgaySinh"].Value != DBNull.Value &&
                    dgvThuThu.Rows[i].Cells["NgaySinh"].Value != null)
                {
                    dateNgaySinh.Value = (DateTime)dgvThuThu.Rows[i].Cells["NgaySinh"].Value;
                }
                else
                {
                    dateNgaySinh.Value = DateTime.Now;  // ✅ NGÀY HIỆN TẠI
                }

                // Chức vụ
                string chucVu = dgvThuThu.Rows[i].Cells["ChucVu"]?.Value?.ToString() ?? "";
                if (!string.IsNullOrEmpty(chucVu))
                {
                    cboChucVu.Text = chucVu;
                }

                // Trạng thái
                if (dgvThuThu.Rows[i].Cells["TrangThai"].Value != DBNull.Value)
                {
                    int trangThai = Convert.ToInt32(dgvThuThu.Rows[i].Cells["TrangThai"].Value);
                    cboTrangThai.SelectedValue = trangThai;
                }
                else
                {
                    cboTrangThai.SelectedValue = 1;
                }

                LoadImage(selectedMaThuThu);
            }
        }

        private void LoadImage(string maThuThu)
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "SELECT HinhAnh FROM ThuThu WHERE MaThuThu = @MaThuThu";
                using (cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@MaThuThu", maThuThu);
                    object imageData = cmd.ExecuteScalar();

                    if (imageData != null && imageData != DBNull.Value)
                    {
                        byte[] bytes = (byte[])imageData;
                        using (MemoryStream ms = new MemoryStream(bytes))
                        {
                            picAvatar.Image = Image.FromStream(ms);
                        }
                    }
                    else
                    {
                        picAvatar.Image = Properties.Resources.avatar_default;
                    }
                }
            }
        }

        public byte[] ImageToByteArray(PictureBox pictureBox)
        {
            if (pictureBox.Image == null) return null;

            using (MemoryStream ms = new MemoryStream())
            {
                Image img = new Bitmap(pictureBox.Image);
                img.Save(ms, pictureBox.Image.RawFormat);
                return ms.ToArray();
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Image Files|*.jpg;*.jpeg;*.png;*. bmp;*.gif|All Files|*.*";
            openFile.Title = "Chọn ảnh";

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                picAvatar.Image = Image.FromFile(openFile.FileName);
            }
        }

        private void btnXoaAnh_Click(object sender, EventArgs e)
        {
            picAvatar.Image = Properties.Resources.avatar_default;
        }

        private void btnTaoMoi_Click(object sender, EventArgs e)
        {
            // ✅ SINH MÃ TỰ ĐỘNG
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "SELECT MAX(MaThuThu) FROM ThuThu";
                cmd = new SqlCommand(sql, con);
                object rs = cmd.ExecuteScalar();

                if (rs != DBNull.Value && rs != null)
                {
                    string maThuThu = rs.ToString();
                    int number = int.Parse(maThuThu.Substring(2));
                    txtMaThuThu.Text = "TT" + (++number).ToString("D3");
                }
                else
                {
                    txtMaThuThu.Text = "TT001";
                }
            }

            // ✅ XÓA DỮ LIỆU
            txtTenThuThu.Text = "";
            txtSDT.Text = "";
            txtEmail.Text = "";

            // ComboBox GioiTinh, ChucVu đã có sẵn - set mặc định
            if (cboGioiTinh.Items.Count > 0) cboGioiTinh.SelectedIndex = 0;
            if (cboChucVu.Items.Count > 0) cboChucVu.SelectedIndex = 0;

            cboTrangThai.SelectedValue = 1;
            dateNgaySinh.Value = DateTime.Now;
            picAvatar.Image = Properties.Resources.avatar_default;

            txtMaThuThu.Enabled = false;  // ✅ DISABLE MÃ
            isCreatingNew = true;
            EnableButtons(true, true, false, false);
            txtTenThuThu.Focus();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (!isCreatingNew)
            {
                MessageBox.Show("Vui lòng bấm 'Tạo mới' trước khi thêm!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(txtTenThuThu.Text.Trim()))
            {
                MessageBox.Show("Vui lòng nhập tên thủ thư!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenThuThu.Focus();
                return;
            }

            if (string.IsNullOrEmpty(txtEmail.Text.Trim()))
            {
                MessageBox.Show("Vui lòng nhập email!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return;
            }

            try
            {
                using (con = new SqlConnection(strCon))
                {
                    con.Open();
                    byte[] imageData = ImageToByteArray(picAvatar);

                    string sql = @"INSERT INTO ThuThu 
                                   (MaThuThu, TenThuThu, HinhAnh, GioiTinh, NgaySinh, Email, SDT, ChucVu, TrangThai) 
                                   VALUES 
                                   (@MaThuThu, @TenThuThu, @HinhAnh, @GioiTinh, @NgaySinh, @Email, @SDT, @ChucVu, @TrangThai)";

                    using (cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@MaThuThu", txtMaThuThu.Text.Trim());
                        cmd.Parameters.AddWithValue("@TenThuThu", txtTenThuThu.Text.Trim());
                        cmd.Parameters.Add("@HinhAnh", SqlDbType.VarBinary).Value = imageData ?? (object)DBNull.Value;
                        cmd.Parameters.AddWithValue("@GioiTinh", cboGioiTinh.Text);
                        cmd.Parameters.AddWithValue("@NgaySinh", dateNgaySinh.Value.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                        cmd.Parameters.AddWithValue("@SDT", txtSDT.Text.Trim());
                        cmd.Parameters.AddWithValue("@ChucVu", cboChucVu.Text);
                        cmd.Parameters.AddWithValue("@TrangThai", cboTrangThai.SelectedValue);

                        int kq = cmd.ExecuteNonQuery();
                        if (kq > 0)
                        {
                            MessageBox.Show("Thêm thành công!", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            isCreatingNew = false;
                            txtSearch.Clear();
                            LoadThuThu();

                            // ✅ CHỌN DÒNG CUỐI
                            if (dgvThuThu.Rows.Count > 0)
                            {
                                int lastIndex = dgvThuThu.RowCount - 1;
                                dgvThuThu.ClearSelection();
                                dgvThuThu.CurrentCell = dgvThuThu.Rows[lastIndex].Cells[0];
                                NapCT();
                                dgvThuThu.FirstDisplayedScrollingRowIndex = lastIndex;
                            }

                            EnableButtons(true, true, true, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm:  " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedMaThuThu))
            {
                MessageBox.Show("Chưa chọn bản ghi để sửa", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (string.IsNullOrEmpty(txtTenThuThu.Text.Trim()))
            {
                MessageBox.Show("Vui lòng nhập tên thủ thư!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenThuThu.Focus();
                return;
            }

            int currentIndex = dgvThuThu.CurrentRow.Index;
            int currentScrollIndex = dgvThuThu.FirstDisplayedScrollingRowIndex;

            using (con = new SqlConnection(strCon))
            {
                con.Open();
                byte[] imageData = ImageToByteArray(picAvatar);

                string sql = @"UPDATE ThuThu 
                               SET TenThuThu = @TenThuThu, HinhAnh = @HinhAnh, GioiTinh = @GioiTinh,
                                   NgaySinh = @NgaySinh, Email = @Email, SDT = @SDT, 
                                   ChucVu = @ChucVu, TrangThai = @TrangThai
                               WHERE MaThuThu = @MaThuThu";

                using (cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@TenThuThu", txtTenThuThu.Text.Trim());
                    cmd.Parameters.Add("@HinhAnh", SqlDbType.VarBinary).Value = imageData ?? (object)DBNull.Value;
                    cmd.Parameters.AddWithValue("@GioiTinh", cboGioiTinh.Text);
                    cmd.Parameters.AddWithValue("@NgaySinh", dateNgaySinh.Value.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@SDT", txtSDT.Text.Trim());
                    cmd.Parameters.AddWithValue("@ChucVu", cboChucVu.Text);
                    cmd.Parameters.AddWithValue("@TrangThai", cboTrangThai.SelectedValue);
                    cmd.Parameters.AddWithValue("@MaThuThu", selectedMaThuThu);

                    int kq = cmd.ExecuteNonQuery();
                    if (kq > 0)
                    {
                        MessageBox.Show("Cập nhật thành công!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }

            LoadThuThu();

            // ✅ GIỮ NGUYÊN VỊ TRÍ
            dgvThuThu.ClearSelection();
            dgvThuThu.CurrentCell = dgvThuThu.Rows[currentIndex].Cells[0];
            if (currentScrollIndex >= 0 && currentScrollIndex < dgvThuThu.Rows.Count)
            {
                dgvThuThu.FirstDisplayedScrollingRowIndex = currentScrollIndex;
            }
            NapCT();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvThuThu.SelectedRows.Count == 0)
            {
                MessageBox.Show("Chưa chọn bản ghi nào để xóa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult rs = MessageBox.Show("Bạn có chắc chắn muốn xóa các bản ghi đã chọn? ",
                   "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (rs == DialogResult.Yes)
            {
                int currentIndex = dgvThuThu.CurrentRow.Index;
                int currentScrollIndex = dgvThuThu.FirstDisplayedScrollingRowIndex;
                int successCount = 0;

                try
                {
                    using (con = new SqlConnection(strCon))
                    {
                        con.Open();

                        foreach (DataGridViewRow row in dgvThuThu.SelectedRows)
                        {
                            string maThuThu = row.Cells["MaThuThu"].Value.ToString();
                            string sql = "DELETE FROM ThuThu WHERE MaThuThu = @MaThuThu";
                            cmd = new SqlCommand(sql, con);
                            cmd.Parameters.AddWithValue("@MaThuThu", maThuThu);

                            try
                            {
                                int kq = cmd.ExecuteNonQuery();
                                if (kq > 0) successCount++;
                            }
                            catch (SqlException ex)
                            {
                                if (ex.Number == 547)
                                {
                                    MessageBox.Show($"Không thể xóa thủ thư '{maThuThu}' vì đang được sử dụng! ",
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

                if (successCount > 0)
                {
                    LoadThuThu();

                    if (dgvThuThu.Rows.Count > 0)
                    {
                        int newIndex = Math.Min(currentIndex, dgvThuThu.Rows.Count - 1);
                        dgvThuThu.ClearSelection();
                        dgvThuThu.CurrentCell = dgvThuThu.Rows[newIndex].Cells[0];

                        if (currentScrollIndex >= 0 && currentScrollIndex < dgvThuThu.Rows.Count)
                        {
                            dgvThuThu.FirstDisplayedScrollingRowIndex = currentScrollIndex;
                        }

                        NapCT();
                        EnableButtons(true, true, true, true);
                    }
                    else
                    {
                        txtMaThuThu.Text = "";
                        txtTenThuThu.Text = "";
                        txtEmail.Text = "";
                        txtSDT.Text = "";
                        picAvatar.Image = Properties.Resources.avatar_default;
                        EnableButtons(true, true, false, false);
                    }
                }
            }
        }

        private void cboTruong_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSearch.Clear();
            txtSearch.PlaceholderText = cboTruong.SelectedIndex == 0
                ? "Nhập tên thủ thư để tìm kiếm"
                : "Nhập mã thủ thư để tìm kiếm";
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (cboTruong.SelectedIndex == 0)
            {
                dv.RowFilter = $"TenThuThu LIKE '%{txtSearch.Text}%'";
            }
            else
            {
                dv.RowFilter = $"MaThuThu LIKE '%{txtSearch.Text}%'";
            }
        }

        private void txtSDT_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                errorProvider1.SetError(txtSDT, "Chỉ được nhập số!");
            }
            else
            {
                errorProvider1.SetError(txtSDT, "");
            }
        }

        private void txtSDT_Leave(object sender, EventArgs e)
        {
            if (txtSDT.Text.Length < 10 || txtSDT.Text.Length > 11)
            {
                errorProvider1.SetError(txtSDT, "Số điện thoại phải có 10-11 chữ số!");
            }
            else
            {
                errorProvider1.SetError(txtSDT, "");
            }
        }

        private void txtEmail_Leave(object sender, EventArgs e)
        {
            if (!txtEmail.Text.Contains("@") || !txtEmail.Text.Contains("."))
            {
                errorProvider1.SetError(txtEmail, "Email không hợp lệ!  Phải chứa '@' và '.'");
            }
            else
            {
                errorProvider1.SetError(txtEmail, "");
            }
        }
    }
}