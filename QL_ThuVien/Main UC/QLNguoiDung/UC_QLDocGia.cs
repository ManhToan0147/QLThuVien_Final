using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QL_ThuVien.Main_UC.QLDocGia
{
    public partial class UC_QLDocGia : UserControl
    {
        string strCon = DBConfig.ConnectionString;
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataTable dt;
        DataView dv;
        string selectedMaDocGia;
        private bool isCreatingNew = false;

        public UC_QLDocGia()
        {
            InitializeComponent();
        }

        private void UC_QLDocGia_Load(object sender, EventArgs e)
        {
            dgvDocGia.ColumnHeadersDefaultCellStyle.Font = new Font(dgvDocGia.Font, FontStyle.Bold);
            cboTruong.SelectedIndex = 0;

            SetupButtonDisabledStyle(btnTaoMoi);
            SetupButtonDisabledStyle(btnThem);
            SetupButtonDisabledStyle(btnSua);
            SetupButtonDisabledStyle(btnXoa);
            SetupButtonDisabledStyle(btnBrowse);
            SetupButtonDisabledStyle(btnXoaAnh);

            SetupComboBoxTrangThai();
            AutoUpdateTrangThaiDocGia();
            LoadDocGia();

            EnableButtons(true, true, true, true);
        }

        // ✅ HÀM TỰ ĐỘNG CẬP NHẬT TRẠNG THÁI ĐỘC GIẢ
        private void AutoUpdateTrangThaiDocGia()
        {
            try
            {
                using (con = new SqlConnection(strCon))
                {
                    con.Open();

                    // ✅ UPDATE TrangThai = 0 (Bị khóa) KHI HẾT HẠN THẺ
                    string sql = @"
                        UPDATE DocGia 
                        SET TrangThai = 0 
                        WHERE NgayHanThe < CAST(GETDATE() AS DATE) 
                        AND TrangThai = 1";

                    cmd = new SqlCommand(sql, con);
                    int rowsAffected = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AUTO UPDATE ERROR] {ex.Message}");
            }
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

        // ✅ HÀM SETUP COMBOBOX TRẠNG THÁI
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

        private void LoadDocGia()
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = @"
                    SELECT 
                        MaDocGia,
                        HoTen,
                        HinhAnh,
                        GioiTinh,
                        NgaySinh,
                        Email,
                        SoDienThoai,
                        NgheNghiep,
                        NgayCapThe,
                        NgayHanThe,
                        TrangThai,
                        CASE 
                            WHEN TrangThai = 1 THEN N'Hoạt động'
                            WHEN TrangThai = 0 THEN N'Bị khóa'
                            ELSE N'Không xác định'
                        END AS TrangThaiText
                    FROM DocGia 
                    ORDER BY MaDocGia";

                adapter = new SqlDataAdapter(sql, con);
                dt = new DataTable();
                adapter.Fill(dt);
            }

            dv = new DataView(dt);
            dgvDocGia.DataSource = dv;

            // ✅ ẨN CỘT TrangThai (bit)
            dgvDocGia.Columns["TrangThai"].Visible = false;

            dgvDocGia.Columns["HinhAnh"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // DB chưa có ảnh thì để ảnh mặc định
            foreach (DataGridViewRow row in dgvDocGia.Rows)
            {
                if (row.Cells["HinhAnh"].Value == DBNull.Value)
                {
                    row.Cells["HinhAnh"].Value = Properties.Resources.avatar_default;
                }
            }
            ((DataGridViewImageColumn)dgvDocGia.Columns["HinhAnh"]).ImageLayout = DataGridViewImageCellLayout.Zoom;
        }

        private void cboTruong_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSearch.Clear();
            if (cboTruong.SelectedIndex == 0)
            {
                txtSearch.PlaceholderText = "Nhập tên độc giả để tìm kiếm";
            }
            else
            {
                txtSearch.PlaceholderText = "Nhập mã độc giả để tìm kiếm";
            }
        }

        private void dgvDocGia_SelectionChanged(object sender, EventArgs e)
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
                    EnableButtons(true, true, true, true);  // SÁNG HẾT - CHẾ ĐỘ XEM
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
            if (dgvDocGia.CurrentCell != null && dgvDocGia.CurrentCell.RowIndex >= 0)
            {
                int i = dgvDocGia.CurrentRow.Index;
                selectedMaDocGia = dgvDocGia.Rows[i].Cells["MaDocGia"]?.Value.ToString() ?? string.Empty;

                txtMaTheMuon.Text = selectedMaDocGia;
                txtMaTheMuon.Enabled = false;

                txtHoTen.Text = dgvDocGia.Rows[i].Cells["HoTen"]?.Value.ToString() ?? "";
                txtEmail.Text = dgvDocGia.Rows[i].Cells["Email"]?.Value.ToString() ?? "";
                txtSDT.Text = dgvDocGia.Rows[i].Cells["SoDienThoai"]?.Value.ToString() ?? "";

                // XỬ LÝ GIỚI TÍNH
                string gioiTinh = dgvDocGia.Rows[i].Cells["GioiTinh"]?.Value?.ToString() ?? "";
                if (!string.IsNullOrEmpty(gioiTinh))
                {
                    cboGioiTinh.Text = gioiTinh;
                }
                else
                {
                    cboGioiTinh.SelectedIndex = -1;
                }

                // XỬ LÝ NGÀY SINH
                if (dgvDocGia.Rows[i].Cells["NgaySinh"].Value != DBNull.Value)
                {
                    dateNgaySinh.Value = (DateTime)dgvDocGia.Rows[i].Cells["NgaySinh"].Value;
                }
                else
                {
                    dateNgaySinh.Value = DateTime.Now;
                }

                // XỬ LÝ NGHỀ NGHIỆP (CHỨC VỤ)
                string NgheNghiep = dgvDocGia.Rows[i].Cells["NgheNghiep"]?.Value?.ToString() ?? "";
                if (!string.IsNullOrEmpty(NgheNghiep))
                {
                    cboNgheNghiep.Text = NgheNghiep;
                }
                else
                {
                    cboNgheNghiep.SelectedIndex = -1;
                }

                // XỬ LÝ NGÀY CẤP THẺ
                if (dgvDocGia.Rows[i].Cells["NgayCapThe"].Value != DBNull.Value)
                {
                    dateNgayCap.Value = (DateTime)dgvDocGia.Rows[i].Cells["NgayCapThe"].Value;
                }
                else
                {
                    dateNgayCap.Value = DateTime.Now;
                }

                // XỬ LÝ NGÀY HẠN THẺ
                if (dgvDocGia.Rows[i].Cells["NgayHanThe"].Value != DBNull.Value)
                {
                    dateNgayHan.Value = (DateTime)dgvDocGia.Rows[i].Cells["NgayHanThe"].Value;
                }
                else
                {
                    dateNgayHan.Value = DateTime.Now;
                }

                // XỬ LÝ TRẠNG THÁI
                if (dgvDocGia.Rows[i].Cells["TrangThai"].Value != DBNull.Value)
                {
                    int trangThai = Convert.ToInt32(dgvDocGia.Rows[i].Cells["TrangThai"].Value);
                    cboTrangThai.SelectedValue = trangThai;
                }
                else
                {
                    cboTrangThai.SelectedValue = 1;
                }

                LoadImage(selectedMaDocGia);
            }
        }
        private void LoadImage(string maDocGia)
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "SELECT HinhAnh FROM DocGia WHERE MaDocGia = @MaDocGia";
                using (cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@MaDocGia", maDocGia);

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

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All Files|*.*";
            openFile.Title = "Chọn ảnh";

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                FileInfo fileInfo = new FileInfo(openFile.FileName);
                const long MAX_SIZE = 10 * 1024 * 1024; // 10MB

                if (fileInfo.Length > MAX_SIZE)
                {
                    MessageBox.Show(
                        $"Ảnh quá lớn!\nKích thước: {fileInfo.Length / (1024.0 * 1024.0):F2}MB\nGiới hạn: 10MB",
                        "Cảnh báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                picAvatar.Image = Image.FromFile(openFile.FileName);
            }
        }

        public byte[] ImageToByteArray(PictureBox pictureBox)
        {
            if (pictureBox.Image == null)
            {
                return null;
            }

            using (MemoryStream ms = new MemoryStream())
            {
                Image img = new Bitmap(pictureBox.Image);
                img.Save(ms, pictureBox.Image.RawFormat);
                return ms.ToArray();
            }
        }

        private void btnTaoMoi_Click(object sender, EventArgs e)
        {
            // TẠO MÃ ĐỘC GIẢ MỚI
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "SELECT MAX(MaDocGia) FROM DocGia";
                cmd = new SqlCommand(sql, con);
                object rs = cmd.ExecuteScalar();

                if (rs != DBNull.Value && rs != null)
                {
                    string maDocGia = rs.ToString();
                    int number = int.Parse(maDocGia.Substring(2));
                    ++number;
                    txtMaTheMuon.Text = "DG" + number.ToString("D3");
                }
                else
                {
                    txtMaTheMuon.Text = "DG001";
                }
            }

            // XÓA DỮ LIỆU CŨ
            txtHoTen.Text = "";
            txtSDT.Text = "";
            txtEmail.Text = "";

            cboGioiTinh.SelectedIndex = -1;
            cboNgheNghiep.SelectedIndex = -1;
            cboTrangThai.SelectedValue = 1;

            dateNgaySinh.Value = DateTime.Now;
            dateNgayCap.Value = DateTime.Now;
            dateNgayHan.Value = DateTime.Now;

            picAvatar.Image = Properties.Resources.avatar_default;

            txtMaTheMuon.Enabled = false;

            isCreatingNew = true;

            EnableButtons(true, true, false, false);

            txtHoTen.Focus();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            // ✅ KIỂM TRA PHẢI BẤM TẠO MỚI TRƯỚC
            if (!isCreatingNew)
            {
                MessageBox.Show("Vui lòng bấm 'Tạo mới' trước khi thêm!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // VALIDATE
            if (string.IsNullOrEmpty(txtMaTheMuon.Text.Trim()))
            {
                MessageBox.Show("Vui lòng nhập mã độc giả!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMaTheMuon.Focus();
                return;
            }

            if (string.IsNullOrEmpty(txtHoTen.Text.Trim()))
            {
                MessageBox.Show("Vui lòng nhập họ tên!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtHoTen.Focus();
                return;
            }

            if (string.IsNullOrEmpty(txtEmail.Text.Trim()))
            {
                MessageBox.Show("Vui lòng nhập email!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return;
            }

            // KIỂM TRA MÃ ĐÃ TỒN TẠI
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string checkSql = "SELECT COUNT(*) FROM DocGia WHERE MaDocGia = @MaDocGia";
                cmd = new SqlCommand(checkSql, con);
                cmd.Parameters.AddWithValue("@MaDocGia", txtMaTheMuon.Text.Trim());
                int count = (int)cmd.ExecuteScalar();

                if (count > 0)
                {
                    MessageBox.Show("Mã độc giả đã tồn tại!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtMaTheMuon.Focus();
                    txtMaTheMuon.SelectAll();
                    return;
                }
            }

            try
            {
                using (con = new SqlConnection(strCon))
                {
                    con.Open();
                    byte[] imageData = ImageToByteArray(picAvatar);
                    string ngaySinh = dateNgaySinh.Value.ToString("yyyy-MM-dd");
                    string ngayCap = dateNgayCap.Value.ToString("yyyy-MM-dd");
                    string ngayHan = dateNgayHan.Value.ToString("yyyy-MM-dd");

                    string sql = @"INSERT INTO DocGia 
                                   (MaDocGia, HoTen, HinhAnh, GioiTinh, NgaySinh, Email, SoDienThoai, NgheNghiep, NgayCapThe, NgayHanThe, TrangThai) 
                                   VALUES 
                                   (@MaDocGia, @HoTen, @HinhAnh, @GioiTinh, @NgaySinh, @Email, @SoDienThoai, @NgheNghiep, @NgayCapThe, @NgayHanThe, @TrangThai)";

                    using (cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@MaDocGia", txtMaTheMuon.Text.Trim());
                        cmd.Parameters.AddWithValue("@HoTen", txtHoTen.Text.Trim());
                        cmd.Parameters.Add("@HinhAnh", SqlDbType.VarBinary).Value = imageData ?? (object)DBNull.Value;
                        cmd.Parameters.AddWithValue("@GioiTinh", cboGioiTinh.Text);
                        cmd.Parameters.AddWithValue("@NgaySinh", ngaySinh);
                        cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                        cmd.Parameters.AddWithValue("@SoDienThoai", txtSDT.Text.Trim());
                        cmd.Parameters.AddWithValue("@NgheNghiep", cboNgheNghiep.Text);
                        cmd.Parameters.AddWithValue("@NgayCapThe", ngayCap);
                        cmd.Parameters.AddWithValue("@NgayHanThe", ngayHan);
                        cmd.Parameters.AddWithValue("@TrangThai", cboTrangThai.SelectedValue);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Thêm thành công!", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // ✅ TẮT CỜ TẠO MỚI
                            isCreatingNew = false;
                            LoadDocGia();

                            // ✅ CHỌN DÒNG CUỐI CÙNG (vừa thêm)
                            int lastRowIndex = dgvDocGia.RowCount - 1;
                            dgvDocGia.ClearSelection();
                            dgvDocGia.CurrentCell = dgvDocGia.Rows[lastRowIndex].Cells[0];
                            NapCT();
                            dgvDocGia.FirstDisplayedScrollingRowIndex = lastRowIndex;  // Cuộn xuống cuối

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
            if (string.IsNullOrEmpty(selectedMaDocGia))
            {
                MessageBox.Show("Chưa chọn bản ghi để sửa", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (string.IsNullOrEmpty(txtHoTen.Text.Trim()))
            {
                MessageBox.Show("Vui lòng nhập họ tên!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtHoTen.Focus();
                return;
            }

            if (string.IsNullOrEmpty(txtEmail.Text.Trim()))
            {
                MessageBox.Show("Vui lòng nhập email!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return;
            }

            int currentIndex = dgvDocGia.CurrentRow.Index;
            int currentScrollIndex = dgvDocGia.FirstDisplayedScrollingRowIndex;

            using (SqlConnection con = new SqlConnection(strCon))
            {
                con.Open();

                byte[] imageData = ImageToByteArray(picAvatar);
                string ngaySinh = dateNgaySinh.Value.ToString("yyyy-MM-dd");
                string ngayCap = dateNgayCap.Value.ToString("yyyy-MM-dd");
                string ngayHan = dateNgayHan.Value.ToString("yyyy-MM-dd");

                string sql = @"UPDATE DocGia 
                               SET HoTen = @HoTen, 
                                   HinhAnh = @HinhAnh,
                                   GioiTinh = @GioiTinh,
                                   NgaySinh = @NgaySinh,
                                   Email = @Email, 
                                   SoDienThoai = @SoDienThoai, 
                                   NgheNghiep = @NgheNghiep, 
                                   NgayCapThe = @NgayCapThe, 
                                   NgayHanThe = @NgayHanThe,
                                   TrangThai = @TrangThai
                               WHERE MaDocGia = @MaDocGia";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@HoTen", txtHoTen.Text.Trim());
                    cmd.Parameters.Add("@HinhAnh", SqlDbType.VarBinary).Value = imageData ?? (object)DBNull.Value;
                    cmd.Parameters.AddWithValue("@GioiTinh", cboGioiTinh.Text);
                    cmd.Parameters.AddWithValue("@NgaySinh", ngaySinh);
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@SoDienThoai", txtSDT.Text.Trim());
                    cmd.Parameters.AddWithValue("@NgheNghiep", cboNgheNghiep.Text);
                    cmd.Parameters.AddWithValue("@NgayCapThe", ngayCap);
                    cmd.Parameters.AddWithValue("@NgayHanThe", ngayHan);
                    cmd.Parameters.AddWithValue("@TrangThai", cboTrangThai.SelectedValue);
                    cmd.Parameters.AddWithValue("@MaDocGia", selectedMaDocGia);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Cập nhật thông tin thành công!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Cập nhật thất bại.", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            LoadDocGia();
            dgvDocGia.ClearSelection();
            dgvDocGia.CurrentCell = dgvDocGia.Rows[currentIndex].Cells[0];

            // ✅ GIỮ NGUYÊN VỊ TRÍ SCROLL (QUAN TRỌNG!)
            if (currentScrollIndex >= 0 && currentScrollIndex < dgvDocGia.Rows.Count)
            {
                dgvDocGia.FirstDisplayedScrollingRowIndex = currentScrollIndex;
            }
            NapCT();
            EnableButtons(true, true, true, true);
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
                if (cboTruong.SelectedIndex == 0)
                {
                    dv.RowFilter = $"HoTen LIKE '%{search}%'";
                }
                else
                {
                    dv.RowFilter = $"MaDocGia LIKE '%{search}%'";
                }
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvDocGia.SelectedRows.Count == 0)
            {
                MessageBox.Show("Chưa chọn bản ghi nào để xóa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult rs = MessageBox.Show("Bạn có chắc chắn muốn xóa các bản ghi đã chọn? ",
                   "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (rs == DialogResult.Yes)
            {
                int currentIndex = dgvDocGia.CurrentRow.Index;
                int successCount = 0;

                try
                {
                    using (con = new SqlConnection(strCon))
                    {
                        con.Open();

                        foreach (DataGridViewRow row in dgvDocGia.SelectedRows)
                        {
                            string maDocGia = row.Cells["MaDocGia"].Value.ToString();
                            string sql = "DELETE FROM DocGia WHERE MaDocGia = @MaDocGia";
                            cmd = new SqlCommand(sql, con);
                            cmd.Parameters.AddWithValue("@MaDocGia", maDocGia);

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
                                    MessageBox.Show($"Không thể xóa độc giả '{maDocGia}' vì đang được sử dụng! ",
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

                LoadDocGia();

                if (dgvDocGia.Rows.Count > 0)
                {
                    int newIndex = Math.Min(currentIndex, dgvDocGia.Rows.Count - 1);
                    dgvDocGia.ClearSelection();
                    dgvDocGia.CurrentCell = dgvDocGia.Rows[newIndex].Cells[0];
                    dgvDocGia.FirstDisplayedScrollingRowIndex = newIndex;
                    NapCT();
                    EnableButtons(true, true, true, true);
                }
                else
                {
                    txtMaTheMuon.Text = "";
                    txtHoTen.Text = "";
                    picAvatar.Image = null;

                    EnableButtons(true, true, false, false);
                }
            }
        }

        private void btnXoaAnh_Click(object sender, EventArgs e)
        {
            picAvatar.Image = null;
        }

        private void txtSDT_KeyPress(object sender, KeyPressEventArgs e)
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