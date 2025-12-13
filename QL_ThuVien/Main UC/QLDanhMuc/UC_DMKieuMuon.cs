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
    public partial class UC_DMKieuMuon : UserControl
    {
        string strCon = DBConfig.ConnectionString;
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataTable dt;
        DataView dv;
        private bool isCreatingNew = false;

        public UC_DMKieuMuon()
        {
            InitializeComponent();
        }

        private void UC_DMKieuMuon_Load(object sender, EventArgs e)
        {
            dgvKieuMuon.ColumnHeadersDefaultCellStyle.Font = new Font(dgvKieuMuon.Font, FontStyle.Bold);

            // Setup ComboBox TrangThai
            SetupComboBoxTrangThai();

            // CÀI ĐẶT MÀU DISABLED
            SetupButtonDisabledStyle(btnTaoMoi);
            SetupButtonDisabledStyle(btnThem);
            SetupButtonDisabledStyle(btnSua);
            SetupButtonDisabledStyle(btnXoa);

            LoadKieuMuon();

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

        private void SetupComboBoxTrangThai()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Value", typeof(int));
            dt.Columns.Add("Display", typeof(string));

            dt.Rows.Add(1, "Đang áp dụng");
            dt.Rows.Add(0, "Ngừng áp dụng");

            cboTrangThai.DataSource = dt;
            cboTrangThai.DisplayMember = "Display";
            cboTrangThai.ValueMember = "Value";
            cboTrangThai.SelectedValue = 1;
        }

        private void LoadKieuMuon()
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = @"
                    SELECT 
                        MaKieuMuon,
                        TenKieuMuon,
                        SoNgayMuon,
                        SoSachToiDa,
                        TrangThai,
                        CASE WHEN TrangThai = 1 THEN N'Đang áp dụng' ELSE N'Ngừng áp dụng' END AS TrangThaiText
                    FROM KieuMuon 
                    ORDER BY MaKieuMuon";

                adapter = new SqlDataAdapter(sql, con);
                dt = new DataTable();
                adapter.Fill(dt);
            }
            dv = new DataView(dt);
            dgvKieuMuon.DataSource = dv;

            // Ẩn cột TrangThai (bit), hiển thị TrangThaiText
            if (dgvKieuMuon.Columns.Contains("TrangThai"))
            {
                dgvKieuMuon.Columns["TrangThai"].Visible = false;
            }
        }

        private void dgvKieuMuon_SelectionChanged(object sender, EventArgs e)
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
            if (dgvKieuMuon.CurrentRow != null && dgvKieuMuon.CurrentRow.Index >= 0)
            {
                int i = dgvKieuMuon.CurrentRow.Index;

                txtMaKieuMuon.Text = dgvKieuMuon.Rows[i].Cells["MaKieuMuon"].Value.ToString();
                txtMaKieuMuon.Enabled = false;

                txtTenKieuMuon.Text = dgvKieuMuon.Rows[i].Cells["TenKieuMuon"].Value.ToString();
                txtSoNgayMuon.Text = dgvKieuMuon.Rows[i].Cells["SoNgayMuon"].Value.ToString();
                txtSoSachToiDa.Text = dgvKieuMuon.Rows[i].Cells["SoSachToiDa"].Value.ToString();

                int trangThai = Convert.ToInt32(dgvKieuMuon.Rows[i].Cells["TrangThai"].Value);
                cboTrangThai.SelectedValue = trangThai;
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
                dv.RowFilter = $"TenKieuMuon LIKE '%{search}%' OR MaKieuMuon LIKE '%{search}%'";
            }
        }

        private void btnTaoMoi_Click(object sender, EventArgs e)
        {
            // KHÔNG SINH MÃ TỰ ĐỘNG - Để trống cho người dùng nhập
            txtMaKieuMuon.Text = "";
            txtMaKieuMuon.Enabled = true;
            txtTenKieuMuon.Text = "";
            txtSoNgayMuon.Text = "";
            txtSoSachToiDa.Text = "";
            cboTrangThai.SelectedValue = 1;
            txtMaKieuMuon.Focus();  // ← Focus vào Mã để nhập

            isCreatingNew = true;
            EnableButtons(true, true, false, false);
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

            string maKM = txtMaKieuMuon.Text.Trim();
            string tenKM = txtTenKieuMuon.Text.Trim();

            // VALIDATE MÃ
            if (string.IsNullOrEmpty(maKM))
            {
                MessageBox.Show("Vui lòng nhập mã kiểu mượn!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMaKieuMuon.Focus();
                return;
            }

            // VALIDATE TÊN
            if (string.IsNullOrEmpty(tenKM))
            {
                MessageBox.Show("Vui lòng nhập tên kiểu mượn!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenKieuMuon.Focus();
                return;
            }

            // VALIDATE SỐ NGÀY MƯỢN
            if (string.IsNullOrEmpty(txtSoNgayMuon.Text.Trim()))
            {
                MessageBox.Show("Vui lòng nhập số ngày mượn!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSoNgayMuon.Focus();
                return;
            }

            if (!int.TryParse(txtSoNgayMuon.Text, out int soNgayMuon) || soNgayMuon < 0)
            {
                MessageBox.Show("Số ngày mượn phải là số nguyên >= 0!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSoNgayMuon.Focus();
                return;
            }

            // VALIDATE SỐ SÁCH TỐI ĐA
            if (string.IsNullOrEmpty(txtSoSachToiDa.Text.Trim()))
            {
                MessageBox.Show("Vui lòng nhập số sách tối đa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSoSachToiDa.Focus();
                return;
            }

            if (!int.TryParse(txtSoSachToiDa.Text, out int soSachToiDa) || soSachToiDa <= 0)
            {
                MessageBox.Show("Số sách tối đa phải là số nguyên > 0!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSoSachToiDa.Focus();
                return;
            }

            // KIỂM TRA MÃ ĐÃ TỒN TẠI CHƯA
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string checkSql = "SELECT COUNT(*) FROM KieuMuon WHERE MaKieuMuon = @MaKieuMuon";
                cmd = new SqlCommand(checkSql, con);
                cmd.Parameters.AddWithValue("@MaKieuMuon", maKM);
                int count = (int)cmd.ExecuteScalar();

                if (count > 0)
                {
                    MessageBox.Show("Mã kiểu mượn đã tồn tại!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtMaKieuMuon.Focus();
                    txtMaKieuMuon.SelectAll();
                    return;
                }
            }

            try
            {
                int trangThai = Convert.ToInt32(cboTrangThai.SelectedValue);

                using (con = new SqlConnection(strCon))
                {
                    con.Open();
                    string query = @"
                        INSERT INTO KieuMuon (MaKieuMuon, TenKieuMuon, SoNgayMuon, SoSachToiDa, TrangThai) 
                        VALUES (@MaKieuMuon, @TenKieuMuon, @SoNgayMuon, @SoSachToiDa, @TrangThai)";

                    cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@MaKieuMuon", maKM);
                    cmd.Parameters.AddWithValue("@TenKieuMuon", tenKM);
                    cmd.Parameters.AddWithValue("@SoNgayMuon", soNgayMuon);
                    cmd.Parameters.AddWithValue("@SoSachToiDa", soSachToiDa);
                    cmd.Parameters.AddWithValue("@TrangThai", trangThai);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Thêm thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                isCreatingNew = false;
                LoadKieuMuon();

                // Chọn dòng vừa thêm
                for (int i = 0; i < dgvKieuMuon.Rows.Count; i++)
                {
                    if (dgvKieuMuon.Rows[i].Cells["MaKieuMuon"].Value.ToString() == maKM)
                    {
                        dgvKieuMuon.ClearSelection();
                        dgvKieuMuon.CurrentCell = dgvKieuMuon.Rows[i].Cells[0];
                        dgvKieuMuon.FirstDisplayedScrollingRowIndex = i;
                        break;
                    }
                }
                NapCT();
                EnableButtons(true, true, true, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvKieuMuon.SelectedRows.Count == 0)
            {
                MessageBox.Show("Chưa chọn bản ghi nào để xóa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult rs = MessageBox.Show(
                "Bạn có chắc chắn muốn xóa các bản ghi đã chọn?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (rs == DialogResult.Yes)
            {
                int currentIndex = dgvKieuMuon.CurrentRow.Index;
                int successCount = 0;

                try
                {
                    using (con = new SqlConnection(strCon))
                    {
                        con.Open();

                        foreach (DataGridViewRow row in dgvKieuMuon.SelectedRows)
                        {
                            string maKM = row.Cells["MaKieuMuon"].Value.ToString();
                            string sql = "DELETE FROM KieuMuon WHERE MaKieuMuon = @MaKieuMuon";
                            cmd = new SqlCommand(sql, con);
                            cmd.Parameters.AddWithValue("@MaKieuMuon", maKM);

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
                                    MessageBox.Show($"Không thể xóa kiểu mượn '{maKM}' vì đang được sử dụng! ",
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

                LoadKieuMuon();

                if (dgvKieuMuon.Rows.Count > 0)
                {
                    int newIndex = Math.Min(currentIndex, dgvKieuMuon.Rows.Count - 1);
                    dgvKieuMuon.ClearSelection();
                    dgvKieuMuon.CurrentCell = dgvKieuMuon.Rows[newIndex].Cells[0];
                    dgvKieuMuon.FirstDisplayedScrollingRowIndex = newIndex;

                    NapCT();
                    EnableButtons(true, true, true, true);
                }
                else
                {
                    txtMaKieuMuon.Text = "";
                    txtTenKieuMuon.Text = "";
                    txtSoNgayMuon.Text = "";
                    txtSoSachToiDa.Text = "";
                    cboTrangThai.SelectedValue = 1;
                    EnableButtons(true, true, false, false);
                }
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            string maKM = txtMaKieuMuon.Text.Trim();
            string tenKM = txtTenKieuMuon.Text.Trim();

            if (string.IsNullOrEmpty(maKM))
            {
                MessageBox.Show("Vui lòng chọn kiểu mượn để cập nhật!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // VALIDATE TÊN
            if (string.IsNullOrEmpty(tenKM))
            {
                MessageBox.Show("Vui lòng nhập tên kiểu mượn!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenKieuMuon.Focus();
                return;
            }

            // VALIDATE SỐ NGÀY MƯỢN
            if (string.IsNullOrEmpty(txtSoNgayMuon.Text.Trim()))
            {
                MessageBox.Show("Vui lòng nhập số ngày mượn!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSoNgayMuon.Focus();
                return;
            }

            if (!int.TryParse(txtSoNgayMuon.Text, out int soNgayMuon) || soNgayMuon < 0)
            {
                MessageBox.Show("Số ngày mượn phải là số nguyên >= 0!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSoNgayMuon.Focus();
                return;
            }

            // VALIDATE SỐ SÁCH TỐI ĐA
            if (string.IsNullOrEmpty(txtSoSachToiDa.Text.Trim()))
            {
                MessageBox.Show("Vui lòng nhập số sách tối đa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSoSachToiDa.Focus();
                return;
            }

            if (!int.TryParse(txtSoSachToiDa.Text, out int soSachToiDa) || soSachToiDa <= 0)
            {
                MessageBox.Show("Số sách tối đa phải là số nguyên > 0!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSoSachToiDa.Focus();
                return;
            }

            int currentIndex = dgvKieuMuon.CurrentRow.Index;

            try
            {
                int trangThai = Convert.ToInt32(cboTrangThai.SelectedValue);

                string sql = @"
                    UPDATE KieuMuon 
                    SET TenKieuMuon = @TenKieuMuon, 
                        SoNgayMuon = @SoNgayMuon,
                        SoSachToiDa = @SoSachToiDa, 
                        TrangThai = @TrangThai 
                    WHERE MaKieuMuon = @MaKieuMuon";

                using (con = new SqlConnection(strCon))
                {
                    con.Open();
                    cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@MaKieuMuon", maKM);
                    cmd.Parameters.AddWithValue("@TenKieuMuon", tenKM);
                    cmd.Parameters.AddWithValue("@SoNgayMuon", soNgayMuon);
                    cmd.Parameters.AddWithValue("@SoSachToiDa", soSachToiDa);
                    cmd.Parameters.AddWithValue("@TrangThai", trangThai);

                    int kq = cmd.ExecuteNonQuery();

                    if (kq > 0)
                    {
                        MessageBox.Show("Cập nhật thành công!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        LoadKieuMuon();
                        dgvKieuMuon.ClearSelection();
                        dgvKieuMuon.CurrentCell = dgvKieuMuon.Rows[currentIndex].Cells[0];
                        dgvKieuMuon.FirstDisplayedScrollingRowIndex = currentIndex;

                        NapCT();
                        EnableButtons(true, true, true, true);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtSoNgayMuon_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtSoSachToiDa_KeyPress(object sender, KeyPressEventArgs e)
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
