using QL_ThuVien.Form_support;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QL_ThuVien.Main_UC.QLSach
{
    public partial class UC_DauSach : UserControl
    {
        string strCon = DBConfig.ConnectionString;
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataTable dt;
        DataView dv;
        bool addNewFlag = false;
        string role;
        string selectedMaDauSach;

        public UC_DauSach(string role)
        {
            InitializeComponent();
            this.role = role;
        }

        // ✅ HÀM SETUP MÀU DISABLED CHO BUTTON
        private void SetupButtonDisabledStyle(dynamic btn)
        {
            btn.DisabledState.BorderColor = Color.FromArgb(180, 210, 230);
            btn.DisabledState.CustomBorderColor = Color.FromArgb(200, 200, 200);
            btn.DisabledState.FillColor = Color.FromArgb(240, 240, 240);
            btn.DisabledState.ForeColor = Color.FromArgb(160, 160, 160);
        }

        // ✅ HÀM BẬT/TẮT BUTTON
        private void EnableButtons(bool taoMoi, bool them, bool sua, bool xoa, bool nhapTacGia)
        {
            btnTaoMoi.Enabled = taoMoi;
            btnThem.Enabled = them;
            btnSua.Enabled = sua;
            btnXoa.Enabled = xoa;
            btnNhapTacGia.Enabled = nhapTacGia;
        }

        private void LoadComboBox(ComboBox cbo, string tableName, string Ma, string TenMa)
        {
            try
            {
                using (con = new SqlConnection(strCon))
                {
                    con.Open();
                    string sql = $"SELECT * FROM {tableName}";
                    SqlDataAdapter adapter = new SqlDataAdapter(sql, con);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    cbo.DataSource = dt;
                    cbo.ValueMember = Ma;
                    cbo.DisplayMember = TenMa;
                    cbo.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu:\n{ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                dv.RowFilter = $"TenDauSach LIKE '%{search}%'";
            }
            UpdateTongSo();
        }

        private void UC_DauSach_Load(object sender, EventArgs e)
        {
            if (role != "admin" && role != "thuthu")
            {
                btnTaoMoi.Visible = false;
                btnThem.Visible = false;
                btnXoa.Visible = false;
                btnSua.Visible = false;
            }

            // ✅ SETUP BUTTON DISABLED STYLE
            SetupButtonDisabledStyle(btnTaoMoi);
            SetupButtonDisabledStyle(btnThem);
            SetupButtonDisabledStyle(btnSua);
            SetupButtonDisabledStyle(btnXoa);
            SetupButtonDisabledStyle(btnNhapTacGia);

            dgvDSDauSach.ColumnHeadersDefaultCellStyle.Font = new Font(dgvDSDauSach.Font, FontStyle.Bold);

            ShowDauSach();
            LoadComboBox(cboMaLoaiSach, "LoaiSach", "MaLoaiSach", "TenLoaiSach");
            LoadComboBox(cboMaChuDe, "ChuDe", "MaChuDe", "TenChuDe");
            LoadComboBox(cboMaNXB, "NXB", "MaNXB", "TenNXB");
            LoadComboBox(cboMaKho, "KhoSach", "MaKho", "TenKho");

            // ✅ VỪA VÀO - SÁNG HẾT
            EnableButtons(true, true, true, true, true);
            UpdateTongSo();
        }

        private void UpdateTongSo()
        {
            lblTongSo.Text = dgvDSDauSach.Rows.Count.ToString();
        }

        private void ShowDauSach()
        {
            try
            {
                using (con = new SqlConnection(strCon))
                {
                    string sql = "SELECT * FROM DauSach ORDER BY MaDauSach";
                    adapter = new SqlDataAdapter(sql, con);
                    dt = new DataTable();
                    adapter.Fill(dt);
                }
                dv = new DataView(dt);
                dgvDSDauSach.DataSource = dv;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu đầu sách:\n{ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TaoMaDS()
        {
            if (cboMaLoaiSach.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn loại sách trước!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string selectedLoaiSach = cboMaLoaiSach.SelectedValue.ToString();
                using (con = new SqlConnection(strCon))
                {
                    con.Open();
                    string sql = $"SELECT MAX(MaDauSach) FROM DauSach WHERE MaDauSach LIKE '{selectedLoaiSach}%'";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    var result = cmd.ExecuteScalar();

                    if (result != DBNull.Value && result != null)
                    {
                        string maxMaDauSach = result.ToString();
                        int number = int.Parse(maxMaDauSach.Substring(selectedLoaiSach.Length));
                        number++;
                        txtMaDauSach.Text = selectedLoaiSach + number.ToString("D2");
                    }
                    else
                    {
                        txtMaDauSach.Text = selectedLoaiSach + "01";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo mã đầu sách:\n{ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cboMaLoaiSach_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (addNewFlag)
            {
                TaoMaDS();
            }
        }

        private void NapCT()
        {
            if (dgvDSDauSach.CurrentCell != null && dgvDSDauSach.CurrentCell.RowIndex >= 0)
            {
                int i = dgvDSDauSach.CurrentRow.Index;
                selectedMaDauSach = dgvDSDauSach.Rows[i].Cells["MaDauSach"]?.Value?.ToString() ?? "";

                txtMaDauSach.Text = selectedMaDauSach;
                txtMaDauSach.Enabled = false;

                txtTenDauSach.Text = dgvDSDauSach.Rows[i].Cells["TenDauSach"]?.Value?.ToString() ?? "";
                txtNamXB.Text = dgvDSDauSach.Rows[i].Cells["NamXuatBan"]?.Value?.ToString() ?? "";
                txtGiaBia.Text = dgvDSDauSach.Rows[i].Cells["GiaBia"]?.Value?.ToString() ?? "";
                txtSoTrang.Text = dgvDSDauSach.Rows[i].Cells["SoTrang"]?.Value?.ToString() ?? "";

                // ComboBox
                if (dgvDSDauSach.Rows[i].Cells["MaLoaiSach"].Value != null)
                    cboMaLoaiSach.SelectedValue = dgvDSDauSach.Rows[i].Cells["MaLoaiSach"].Value;

                if (dgvDSDauSach.Rows[i].Cells["MaChuDe"].Value != null)
                    cboMaChuDe.SelectedValue = dgvDSDauSach.Rows[i].Cells["MaChuDe"].Value;

                if (dgvDSDauSach.Rows[i].Cells["MaNXB"].Value != null)
                    cboMaNXB.SelectedValue = dgvDSDauSach.Rows[i].Cells["MaNXB"].Value;

                if (dgvDSDauSach.Rows[i].Cells["MaKho"].Value != null)
                    cboMaKho.SelectedValue = dgvDSDauSach.Rows[i].Cells["MaKho"].Value;
            }
        }

        private void dgvDSDauSach_SelectionChanged(object sender, EventArgs e)
        {
            // ✅ KIỂM TRA NẾU ĐANG TẠO MỚI
            if (addNewFlag)
            {
                DialogResult result = MessageBox.Show(
                    "Bạn có muốn hủy tạo mới?  ",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    addNewFlag = false;
                    NapCT();
                    // ✅ SÁNG HẾT
                    EnableButtons(true, true, true, true, true);
                }
                else
                {
                    // ✅ KHÔNG HỦY → GIỮ NGUYÊN CHẾ ĐỘ TẠO MỚI
                    return;
                }
            }
            else
            {
                NapCT();
            }
        }

        private void btnTaoMoi_Click(object sender, EventArgs e)
        {
            // ✅ XÓA DỮ LIỆU CŨ
            txtMaDauSach.Text = "";
            txtTenDauSach.Text = "";
            txtNamXB.Text = "";
            txtGiaBia.Text = "";
            txtSoTrang.Text = "";

            if (cboMaLoaiSach.Items.Count > 0) cboMaLoaiSach.SelectedIndex = 0;
            if (cboMaChuDe.Items.Count > 0) cboMaChuDe.SelectedIndex = 0;
            if (cboMaNXB.Items.Count > 0) cboMaNXB.SelectedIndex = 0;
            if (cboMaKho.Items.Count > 0) cboMaKho.SelectedIndex = 0;

            // ✅ TẠO MÃ
            TaoMaDS();

            txtTenDauSach.Focus();
            addNewFlag = true;

            // ✅ XÁM:  XÓA, SỬA, NHẬP TÁC GIẢ
            EnableButtons(true, true, false, false, false);
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            // ✅ KIỂM TRA PHẢI BẤM TẠO MỚI TRƯỚC
            if (!addNewFlag)
            {
                MessageBox.Show("Vui lòng bấm 'Tạo mới' trước khi thêm!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ✅ VALIDATE
            if (string.IsNullOrWhiteSpace(txtMaDauSach.Text))
            {
                MessageBox.Show("Vui lòng nhập mã đầu sách!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMaDauSach.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTenDauSach.Text))
            {
                MessageBox.Show("Vui lòng nhập tên đầu sách!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenDauSach.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNamXB.Text))
            {
                MessageBox.Show("Vui lòng nhập năm xuất bản!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNamXB.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtGiaBia.Text))
            {
                MessageBox.Show("Vui lòng nhập giá bìa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtGiaBia.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtSoTrang.Text))
            {
                MessageBox.Show("Vui lòng nhập số trang!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSoTrang.Focus();
                return;
            }

            if (cboMaLoaiSach.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn loại sách!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboMaLoaiSach.Focus();
                return;
            }

            if (cboMaChuDe.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn chủ đề!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboMaChuDe.Focus();
                return;
            }

            if (cboMaNXB.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn nhà xuất bản!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboMaNXB.Focus();
                return;
            }

            if (cboMaKho.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn kho sách!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboMaKho.Focus();
                return;
            }

            try
            {
                string maDauSach = txtMaDauSach.Text.Trim();
                string tenDauSach = txtTenDauSach.Text.Trim().Replace("'", "''"); // Escape single quote
                string namXB = txtNamXB.Text.Trim();
                string giaBia = txtGiaBia.Text.Trim();
                string soTrang = txtSoTrang.Text.Trim();
                string maLoaiSach = cboMaLoaiSach.SelectedValue.ToString();
                string maChuDe = cboMaChuDe.SelectedValue.ToString();
                string maNXB = cboMaNXB.SelectedValue.ToString();
                string maKho = cboMaKho.SelectedValue.ToString();

                using (con = new SqlConnection(strCon))
                {
                    con.Open();

                    // ✅ KIỂM TRA MÃ ĐÃ TỒN TẠI
                    string checkSql = "SELECT COUNT(*) FROM DauSach WHERE MaDauSach = @MaDauSach";
                    cmd = new SqlCommand(checkSql, con);
                    cmd.Parameters.AddWithValue("@MaDauSach", maDauSach);
                    int count = (int)cmd.ExecuteScalar();

                    if (count > 0)
                    {
                        MessageBox.Show("Mã đầu sách đã tồn tại!", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtMaDauSach.Focus();
                        txtMaDauSach.SelectAll();
                        return;
                    }

                    // ✅ INSERT
                    string sql = @"INSERT INTO DauSach 
                                   (MaDauSach, TenDauSach, NamXuatBan, GiaBia, SoTrang, MaLoaiSach, MaChuDe, MaNXB, MaKho) 
                                   VALUES 
                                   (@MaDauSach, @TenDauSach, @NamXuatBan, @GiaBia, @SoTrang, @MaLoaiSach, @MaChuDe, @MaNXB, @MaKho)";

                    cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@MaDauSach", maDauSach);
                    cmd.Parameters.AddWithValue("@TenDauSach", tenDauSach);
                    cmd.Parameters.AddWithValue("@NamXuatBan", namXB);
                    cmd.Parameters.AddWithValue("@GiaBia", giaBia);
                    cmd.Parameters.AddWithValue("@SoTrang", soTrang);
                    cmd.Parameters.AddWithValue("@MaLoaiSach", maLoaiSach);
                    cmd.Parameters.AddWithValue("@MaChuDe", maChuDe);
                    cmd.Parameters.AddWithValue("@MaNXB", maNXB);
                    cmd.Parameters.AddWithValue("@MaKho", maKho);

                    int kq = cmd.ExecuteNonQuery();

                    if (kq > 0)
                    {
                        MessageBox.Show("Thêm đầu sách thành công!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        addNewFlag = false;
                        txtSearch.Clear();
                        ShowDauSach();
                        UpdateTongSo();
                        // ✅ SÁNG HẾT
                        EnableButtons(true, true, true, true, true);

                        // ✅ TÌM VÀ CHỌN DÒNG VỪA THÊM
                        foreach (DataGridViewRow row in dgvDSDauSach.Rows)
                        {
                            if (row.Cells["MaDauSach"].Value?.ToString() == maDauSach)
                            {
                                dgvDSDauSach.ClearSelection();
                                dgvDSDauSach.CurrentCell = row.Cells[0];
                                NapCT();
                                dgvDSDauSach.FirstDisplayedScrollingRowIndex = row.Index;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm đầu sách:\n{ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvDSDauSach.SelectedRows.Count == 0)
            {
                MessageBox.Show("Chưa chọn bản ghi nào để xóa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult rs = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa {dgvDSDauSach.SelectedRows.Count} đầu sách đã chọn?  ",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (rs == DialogResult.Yes)
            {
                int currentIndex = dgvDSDauSach.CurrentRow.Index;
                int successCount = 0;
                int failCount = 0;

                foreach (DataGridViewRow row in dgvDSDauSach.SelectedRows)
                {
                    string maDauSach = row.Cells["MaDauSach"].Value?.ToString();
                    if (string.IsNullOrEmpty(maDauSach)) continue;

                    try
                    {
                        using (con = new SqlConnection(strCon))
                        {
                            con.Open();
                            string sql = "DELETE FROM DauSach WHERE MaDauSach = @MaDauSach";
                            cmd = new SqlCommand(sql, con);
                            cmd.Parameters.AddWithValue("@MaDauSach", maDauSach);

                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                successCount++;
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        failCount++;
                        if (ex.Number == 547) // Foreign key constraint
                        {
                            MessageBox.Show(
                                $"Không thể xóa đầu sách '{maDauSach}' vì đang được sử dụng! ",
                                "Lỗi",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        }
                        else
                        {
                            MessageBox.Show($"Lỗi khi xóa đầu sách '{maDauSach}':\n{ex.Message}", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }

                if (successCount > 0)
                {
                    MessageBox.Show(
                        $"Đã xóa {successCount} đầu sách thành công!" +
                        (failCount > 0 ? $"\n{failCount} đầu sách không thể xóa." : ""),
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    ShowDauSach();
                    UpdateTongSo();

                    if (dgvDSDauSach.Rows.Count > 0)
                    {
                        int newIndex = Math.Min(currentIndex, dgvDSDauSach.Rows.Count - 1);
                        dgvDSDauSach.ClearSelection();
                        dgvDSDauSach.CurrentCell = dgvDSDauSach.Rows[newIndex].Cells[0];
                        NapCT();
                        dgvDSDauSach.FirstDisplayedScrollingRowIndex = newIndex;
                    }
                }
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedMaDauSach))
            {
                MessageBox.Show("Chưa chọn bản ghi để sửa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ✅ VALIDATE
            if (string.IsNullOrWhiteSpace(txtTenDauSach.Text))
            {
                MessageBox.Show("Vui lòng nhập tên đầu sách!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenDauSach.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNamXB.Text))
            {
                MessageBox.Show("Vui lòng nhập năm xuất bản!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNamXB.Focus();
                return;
            }

            int currentIndex = dgvDSDauSach.CurrentRow.Index;
            int currentScrollIndex = dgvDSDauSach.FirstDisplayedScrollingRowIndex;

            try
            {
                string tenDauSach = txtTenDauSach.Text.Trim().Replace("'", "''");

                using (con = new SqlConnection(strCon))
                {
                    con.Open();
                    string sql = @"UPDATE DauSach SET 
                                   TenDauSach = @TenDauSach, 
                                   NamXuatBan = @NamXuatBan, 
                                   GiaBia = @GiaBia, 
                                   SoTrang = @SoTrang, 
                                   MaLoaiSach = @MaLoaiSach, 
                                   MaChuDe = @MaChuDe, 
                                   MaNXB = @MaNXB, 
                                   MaKho = @MaKho 
                                   WHERE MaDauSach = @MaDauSach";

                    cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@TenDauSach", tenDauSach);
                    cmd.Parameters.AddWithValue("@NamXuatBan", txtNamXB.Text.Trim());
                    cmd.Parameters.AddWithValue("@GiaBia", txtGiaBia.Text.Trim());
                    cmd.Parameters.AddWithValue("@SoTrang", txtSoTrang.Text.Trim());
                    cmd.Parameters.AddWithValue("@MaLoaiSach", cboMaLoaiSach.SelectedValue);
                    cmd.Parameters.AddWithValue("@MaChuDe", cboMaChuDe.SelectedValue);
                    cmd.Parameters.AddWithValue("@MaNXB", cboMaNXB.SelectedValue);
                    cmd.Parameters.AddWithValue("@MaKho", cboMaKho.SelectedValue);
                    cmd.Parameters.AddWithValue("@MaDauSach", selectedMaDauSach);

                    int kq = cmd.ExecuteNonQuery();
                    if (kq > 0)
                    {
                        MessageBox.Show("Cập nhật thông tin đầu sách thành công!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Cập nhật thất bại!", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                ShowDauSach();
                dgvDSDauSach.ClearSelection();
                dgvDSDauSach.CurrentCell = dgvDSDauSach.Rows[currentIndex].Cells[0];

                if (currentScrollIndex >= 0 && currentScrollIndex < dgvDSDauSach.Rows.Count)
                {
                    dgvDSDauSach.FirstDisplayedScrollingRowIndex = currentScrollIndex;
                }

                NapCT();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật đầu sách:\n{ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtNamXB_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtGiaBia_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtSoTrang_KeyPress(object sender, KeyPressEventArgs e)
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

        private void btnNhapTacGia_Click(object sender, EventArgs e)
        {
            if (dgvDSDauSach.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một đầu sách để nhập tác giả!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow selectedRow = dgvDSDauSach.SelectedRows[0];

            if (selectedRow.Cells["MaDauSach"].Value == null)
            {
                MessageBox.Show("Dòng được chọn không chứa thông tin đầu sách!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string maDauSach = selectedRow.Cells["MaDauSach"].Value.ToString();

            if (!KiemTraDauSachTonTai(maDauSach))
            {
                MessageBox.Show(
                    "Đầu sách chưa được lưu vào hệ thống!\n\nVui lòng lưu đầu sách trước khi nhập tác giả.",
                    "Cảnh báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            frmNhapTacGia frm = new frmNhapTacGia();
            frm.MaDauSach = maDauSach;
            frm.ShowDialog();
        }

        private bool KiemTraDauSachTonTai(string maDauSach)
        {
            try
            {
                using (con = new SqlConnection(strCon))
                {
                    con.Open();
                    string sql = "SELECT COUNT(*) FROM DauSach WHERE MaDauSach = @MaDauSach";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@MaDauSach", maDauSach);

                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi kiểm tra đầu sách:\n{ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void dgvDSDauSach_DoubleClick(object sender, EventArgs e)
        {
            if (dgvDSDauSach.CurrentRow == null) return;

            string maDauSach = dgvDSDauSach.CurrentRow.Cells["MaDauSach"].Value?.ToString();

            if (string.IsNullOrEmpty(maDauSach)) return;

            ChiTietSach cts = new ChiTietSach(maDauSach);
            cts.ShowDialog();
        }
    }
}