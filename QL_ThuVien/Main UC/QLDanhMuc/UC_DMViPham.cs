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
    public partial class UC_DMViPham : UserControl
    {
        string strCon = DBConfig.ConnectionString;
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataTable dt;
        DataView dv;
        private bool isCreatingNew = false;

        public UC_DMViPham()
        {
            InitializeComponent();
        }

        private void UC_DMViPham_Load(object sender, EventArgs e)
        {
            dgvViPham.ColumnHeadersDefaultCellStyle.Font = new Font(dgvViPham.Font, FontStyle.Bold);

            // Setup ComboBox
            SetupComboBoxLoaiTinhPhat();
            SetupComboBoxTrangThai();

            // CÀI ĐẶT MÀU DISABLED
            SetupButtonDisabledStyle(btnTaoMoi);
            SetupButtonDisabledStyle(btnThem);
            SetupButtonDisabledStyle(btnSua);
            SetupButtonDisabledStyle(btnXoa);

            LoadViPham();

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

        private void SetupComboBoxLoaiTinhPhat()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("LoaiTinhPhat", typeof(string));

            dt.Rows.Add("Cố định");
            dt.Rows.Add("Theo ngày");
            dt.Rows.Add("Theo giá bìa");

            cboLoaiTinhPhat.DataSource = dt;
            cboLoaiTinhPhat.DisplayMember = "LoaiTinhPhat";
            cboLoaiTinhPhat.ValueMember = "LoaiTinhPhat";
            cboLoaiTinhPhat.SelectedIndex = -1;
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

        private void LoadViPham()
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = @"
                    SELECT 
                        MaViPham,
                        TenViPham,
                        HinhThucPhat,
                        LoaiTinhPhat,
                        GiaTri,
                        TrangThai,
                        CASE WHEN TrangThai = 1 THEN N'Đang áp dụng' ELSE N'Ngừng áp dụng' END AS TrangThaiText
                    FROM ViPham 
                    ORDER BY MaViPham";

                adapter = new SqlDataAdapter(sql, con);
                dt = new DataTable();
                adapter.Fill(dt);
            }
            dv = new DataView(dt);
            dgvViPham.DataSource = dv;

            // Ẩn cột TrangThai (bit), hiển thị TrangThaiText
            if (dgvViPham.Columns.Contains("TrangThai"))
            {
                dgvViPham.Columns["TrangThai"].Visible = false;
            }
        }

        private void dgvViPham_SelectionChanged(object sender, EventArgs e)
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
            if (dgvViPham.CurrentRow != null && dgvViPham.CurrentRow.Index >= 0)
            {
                int i = dgvViPham.CurrentRow.Index;

                txtMaViPham.Text = dgvViPham.Rows[i].Cells["MaViPham"].Value.ToString();
                txtMaViPham.Enabled = false;

                txtTenViPham.Text = dgvViPham.Rows[i].Cells["TenViPham"].Value.ToString();

                // Load HinhThucPhat - XỬ LÝ NULL
                txtHinhThucPhat.Text = dgvViPham.Rows[i].Cells["HinhThucPhat"].Value != DBNull.Value
                    ? dgvViPham.Rows[i].Cells["HinhThucPhat"].Value.ToString()
                    : "";

                // Load LoaiTinhPhat - XỬ LÝ NULL
                if (dgvViPham.Rows[i].Cells["LoaiTinhPhat"].Value != DBNull.Value)
                {
                    string loaiTinhPhat = dgvViPham.Rows[i].Cells["LoaiTinhPhat"].Value.ToString();
                    cboLoaiTinhPhat.SelectedValue = loaiTinhPhat;
                }
                else
                {
                    cboLoaiTinhPhat.SelectedIndex = -1;
                }

                // Load GiaTri - XỬ LÝ NULL
                txtGiaTri.Text = dgvViPham.Rows[i].Cells["GiaTri"].Value != DBNull.Value
                    ? dgvViPham.Rows[i].Cells["GiaTri"].Value.ToString()
                    : "";

                // Load TrangThai
                int trangThai = Convert.ToInt32(dgvViPham.Rows[i].Cells["TrangThai"].Value);
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
                dv.RowFilter = $"TenViPham LIKE '%{search}%' OR MaViPham LIKE '%{search}%'";
            }
        }

        private void btnTaoMoi_Click(object sender, EventArgs e)
        {
            // Sinh mã tự động
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "SELECT MAX(MaViPham) FROM ViPham";
                cmd = new SqlCommand(sql, con);
                object rs = cmd.ExecuteScalar();

                if (rs != DBNull.Value && rs != null)
                {
                    string maViPham = rs.ToString();
                    int number = int.Parse(maViPham.Substring(2));
                    number++;
                    txtMaViPham.Text = "VP" + number.ToString("D2");
                }
                else
                {
                    txtMaViPham.Text = "VP01";
                }
            }

            txtMaViPham.Enabled = false;
            txtTenViPham.Text = "";
            txtHinhThucPhat.Text = "";
            txtGiaTri.Text = "";
            cboLoaiTinhPhat.SelectedIndex = -1;
            cboTrangThai.SelectedValue = 1;
            txtTenViPham.Focus();

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

            string maVP = txtMaViPham.Text.Trim();
            string tenVP = txtTenViPham.Text.Trim();

            // CHỈ VALIDATE TÊN VI PHẠM
            if (string.IsNullOrEmpty(tenVP))
            {
                MessageBox.Show("Vui lòng nhập tên vi phạm!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenViPham.Focus();
                return;
            }

            // KIỂM TRA MÃ ĐÃ TỒN TẠI CHƯA
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string checkSql = "SELECT COUNT(*) FROM ViPham WHERE MaViPham = @MaViPham";
                cmd = new SqlCommand(checkSql, con);
                cmd.Parameters.AddWithValue("@MaViPham", maVP);
                int count = (int)cmd.ExecuteScalar();

                if (count > 0)
                {
                    MessageBox.Show("Mã vi phạm đã tồn tại!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtMaViPham.Focus();
                    txtMaViPham.SelectAll();
                    return;
                }
            }

            try
            {
                string hinhThucPhat = txtHinhThucPhat.Text.Trim();
                string loaiTinhPhat = cboLoaiTinhPhat.SelectedIndex >= 0
                    ? cboLoaiTinhPhat.SelectedValue.ToString()
                    : null;

                float? giaTri = null;
                if (!string.IsNullOrWhiteSpace(txtGiaTri.Text))
                {
                    if (float.TryParse(txtGiaTri.Text, out float temp))
                    {
                        giaTri = temp;
                    }
                }

                int trangThai = Convert.ToInt32(cboTrangThai.SelectedValue);

                using (con = new SqlConnection(strCon))
                {
                    con.Open();
                    string query = @"
                        INSERT INTO ViPham (MaViPham, TenViPham, HinhThucPhat, LoaiTinhPhat, GiaTri, TrangThai) 
                        VALUES (@MaViPham, @TenViPham, @HinhThucPhat, @LoaiTinhPhat, @GiaTri, @TrangThai)";

                    cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@MaViPham", maVP);
                    cmd.Parameters.AddWithValue("@TenViPham", tenVP);

                    // INSERT NULL NẾU TRỐNG
                    cmd.Parameters.AddWithValue("@HinhThucPhat", string.IsNullOrEmpty(hinhThucPhat) ? (object)DBNull.Value : hinhThucPhat);
                    cmd.Parameters.AddWithValue("@LoaiTinhPhat", string.IsNullOrEmpty(loaiTinhPhat) ? (object)DBNull.Value : loaiTinhPhat);
                    cmd.Parameters.AddWithValue("@GiaTri", giaTri.HasValue ? (object)giaTri.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@TrangThai", trangThai);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Thêm thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                isCreatingNew = false;
                LoadViPham();

                // Chọn dòng vừa thêm
                for (int i = 0; i < dgvViPham.Rows.Count; i++)
                {
                    if (dgvViPham.Rows[i].Cells["MaViPham"].Value.ToString() == maVP)
                    {
                        dgvViPham.ClearSelection();
                        dgvViPham.CurrentCell = dgvViPham.Rows[i].Cells[0];
                        dgvViPham.FirstDisplayedScrollingRowIndex = i;
                        break;
                    }
                }
                NapCT();
                EnableButtons(true, true, true, true);  // SÁNG HẾT
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvViPham.SelectedRows.Count == 0)
            {
                MessageBox.Show("Chưa chọn bản ghi nào để xóa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult rs = MessageBox.Show(
                "Bạn có chắc chắn muốn xóa các bản ghi đã chọn? ",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (rs == DialogResult.Yes)
            {
                int currentIndex = dgvViPham.CurrentRow.Index;
                int successCount = 0;

                try
                {
                    using (con = new SqlConnection(strCon))
                    {
                        con.Open();

                        foreach (DataGridViewRow row in dgvViPham.SelectedRows)
                        {
                            string maVP = row.Cells["MaViPham"].Value.ToString();
                            string sql = "DELETE FROM ViPham WHERE MaViPham = @MaViPham";
                            cmd = new SqlCommand(sql, con);
                            cmd.Parameters.AddWithValue("@MaViPham", maVP);

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
                                    MessageBox.Show($"Không thể xóa vi phạm '{maVP}' vì đang được sử dụng! ",
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

                LoadViPham();

                if (dgvViPham.Rows.Count > 0)
                {
                    int newIndex = Math.Min(currentIndex, dgvViPham.Rows.Count - 1);
                    dgvViPham.ClearSelection();
                    dgvViPham.CurrentCell = dgvViPham.Rows[newIndex].Cells[0];
                    dgvViPham.FirstDisplayedScrollingRowIndex = newIndex;
                    NapCT();
                    EnableButtons(true, true, true, true);  // SÁNG HẾT
                }
                else
                {
                    txtMaViPham.Text = "";
                    txtTenViPham.Text = "";
                    txtHinhThucPhat.Text = "";
                    txtGiaTri.Text = "";
                    cboLoaiTinhPhat.SelectedIndex = -1;
                    cboTrangThai.SelectedValue = 1;
                    EnableButtons(true, true, false, false);  // TẮT SỬA/XÓA
                }
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            string maVP = txtMaViPham.Text.Trim();
            string tenVP = txtTenViPham.Text.Trim();

            if (string.IsNullOrEmpty(maVP))
            {
                MessageBox.Show("Vui lòng chọn vi phạm để cập nhật!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // CHỈ VALIDATE TÊN VI PHẠM
            if (string.IsNullOrEmpty(tenVP))
            {
                MessageBox.Show("Vui lòng nhập tên vi phạm!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenViPham.Focus();
                return;
            }

            int currentIndex = dgvViPham.CurrentRow.Index;

            try
            {
                string hinhThucPhat = txtHinhThucPhat.Text.Trim();
                string loaiTinhPhat = cboLoaiTinhPhat.SelectedIndex >= 0
                    ? cboLoaiTinhPhat.SelectedValue.ToString()
                    : null;

                float? giaTri = null;
                if (!string.IsNullOrWhiteSpace(txtGiaTri.Text))
                {
                    float.TryParse(txtGiaTri.Text, out float temp);
                    giaTri = temp;
                }

                int trangThai = Convert.ToInt32(cboTrangThai.SelectedValue);

                string sql = @"
                    UPDATE ViPham 
                    SET TenViPham = @TenViPham, 
                        HinhThucPhat = @HinhThucPhat,
                        LoaiTinhPhat = @LoaiTinhPhat, 
                        GiaTri = @GiaTri, 
                        TrangThai = @TrangThai 
                    WHERE MaViPham = @MaViPham";

                using (con = new SqlConnection(strCon))
                {
                    con.Open();
                    cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@MaViPham", maVP);
                    cmd.Parameters.AddWithValue("@TenViPham", tenVP);

                    // UPDATE NULL NẾU TRỐNG
                    cmd.Parameters.AddWithValue("@HinhThucPhat", string.IsNullOrEmpty(hinhThucPhat) ? (object)DBNull.Value : hinhThucPhat);
                    cmd.Parameters.AddWithValue("@LoaiTinhPhat", string.IsNullOrEmpty(loaiTinhPhat) ? (object)DBNull.Value : loaiTinhPhat);
                    cmd.Parameters.AddWithValue("@GiaTri", giaTri.HasValue ? (object)giaTri.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@TrangThai", trangThai);

                    int kq = cmd.ExecuteNonQuery();

                    if (kq > 0)
                    {
                        MessageBox.Show("Cập nhật thành công!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        LoadViPham();
                        dgvViPham.ClearSelection();
                        dgvViPham.CurrentCell = dgvViPham.Rows[currentIndex].Cells[0];
                        dgvViPham.FirstDisplayedScrollingRowIndex = currentIndex;
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