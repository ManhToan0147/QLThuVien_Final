using QL_ThuVien.Form_support;
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

namespace QL_ThuVien.Main_UC.QLMuonTra
{
    public partial class UC_PhieuTra : UserControl
    {
        string strCon = DBConfig.ConnectionString;
        SqlConnection con;
        SqlDataAdapter adapter;
        SqlCommand cmd;
        DataTable dt;
        DataView dvPMCanTra;
        DataView dvPMDaTra;
        bool addNewFlag;
        private DateTime? filterTuNgay = null;
        private DateTime? filterDenNgay = null;
        public UC_PhieuTra()
        {
            InitializeComponent();
        }

        private void btnTraSach_Click(object sender, EventArgs e)
        {
            var f = new frmTraSach();
            f.MaPhieuMuon = txtMaPhieuMuon.Text;
            f.MaDocGia = txtMaDG.Text;

            f.ShowDialog();

            // ✅ SAU KHI ĐÓNG FORM → LOAD LẠI

            // Lưu vị trí
            int currentIndex = dgvPMDaTra.CurrentRow?.Index ?? -1;
            string currentMaPM = selectedMaPM ?? "";

            // Load lại dữ liệu
            LoadPMDaTra();
            LoadPMCanTra();

            // ✅ GỌI LẠI FILTER (giữ filter hiện tại)
            FilterDataPhieuTra();

            UpdateTongSo();

            // Chọn lại dòng
            if (dgvPMDaTra.Rows.Count > 0)
            {
                if (currentIndex >= 0 && currentIndex < dgvPMDaTra.Rows.Count)
                {
                    dgvPMDaTra.ClearSelection();
                    dgvPMDaTra.CurrentCell = dgvPMDaTra.Rows[currentIndex].Cells[0];
                    dgvPMDaTra.FirstDisplayedScrollingRowIndex = currentIndex;
                }
                else
                {
                    dgvPMDaTra.CurrentCell = dgvPMDaTra.Rows[0].Cells[0];
                }

                NapCT();
                LoadSachTra(selectedMaPM);
            }
        }

        private void UC_PhieuTra_Load(object sender, EventArgs e)
        {
            cboTruong1.SelectedIndex = 0;
            cboTruong2.SelectedIndex = 0;

            // Setup disabled style
            SetupButtonDisabledStyle(btnSua);
            SetupButtonDisabledStyle(btnXoa);
            SetupButtonDisabledStyle(btnTraSach);
            SetupButtonDisabledStyle(btnInPhieuTra);
            SetupButtonDisabledStyle(btnLocNgay);

            //Fix lỗi column header
            dgvPMCanTra.ColumnHeadersDefaultCellStyle.Font = new Font(dgvPMCanTra.Font, FontStyle.Bold);
            dgvPMDaTra.ColumnHeadersDefaultCellStyle.Font = new Font(dgvPMDaTra.Font, FontStyle.Bold);
            dgvSachTra.DefaultCellStyle.Font = new Font(dgvSachTra.Font, FontStyle.Regular);
            dgvSachTra.Columns["DaTraSach"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            LoadPMDaTra();
            LoadPMCanTra();
            SetStyle();
            UpdateTongSo();
        }
        private void UpdateTongSo()
        {
            lblTongSo.Text = dgvPMDaTra.Rows.Count.ToString();
        }

        private void SetupButtonDisabledStyle(dynamic btn)
        {
            btn.DisabledState.BorderColor = Color.FromArgb(180, 210, 230);
            btn.DisabledState.CustomBorderColor = Color.FromArgb(200, 200, 200);
            btn.DisabledState.FillColor = Color.FromArgb(240, 240, 240);
            btn.DisabledState.ForeColor = Color.FromArgb(160, 160, 160);
        }

        private void SetStyle()
        {
            if (addNewFlag)
            {
                // CHẾ ĐỘ TẠO MỚI - CHỈ TẮT SỬA/XÓA/IN/TRẢ SÁCH
                btnSua.Enabled = false;
                btnXoa.Enabled = false;
                btnInPhieuTra.Enabled = false;
                btnTraSach.Enabled = false;
                btnLocNgay.Enabled = false;
            }
            else
            {
                // CHẾ ĐỘ BÌNH THƯỜNG - BẬT HẾT
                btnSua.Enabled = true;
                btnXoa.Enabled = true;
                btnInPhieuTra.Enabled = true;
                btnTraSach.Enabled = true;
                btnLocNgay.Enabled = true;
            }
        }


        private void LoadPMCanTra()
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = @"
                    SELECT MaPhieuMuon, MaDocGia, NgayMuon, HanTra
                    FROM PhieuMuon
                    WHERE NgayThucTra IS NULL";
                adapter = new SqlDataAdapter(sql, con);
                dt = new DataTable();
                adapter.Fill(dt);
                dvPMCanTra = new DataView(dt);
                dgvPMCanTra.DataSource = dvPMCanTra;
            }
        }

        private void LoadPMDaTra()
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = @"
                    SELECT 
                        pm.MaPhieuMuon, 
                        pm.MaDocGia,
                        dg.HoTen AS TenDocGia,
                        pm.NgayMuon, 
                        pm.HanTra, 
                        pm.NgayThucTra,
                        CASE 
                            WHEN pm.HanTra < pm.NgayThucTra THEN DATEDIFF(DAY, pm.HanTra, pm.NgayThucTra) 
                            ELSE 0
                        END AS SoNgayTre
                    FROM PhieuMuon pm
                    LEFT JOIN DocGia dg ON pm.MaDocGia = dg.MaDocGia
                    WHERE pm.NgayThucTra IS NOT NULL
                    ORDER BY pm.NgayThucTra DESC, pm.MaPhieuMuon DESC";
                adapter = new SqlDataAdapter(sql, con);
                dt = new DataTable();
                adapter.Fill(dt);
                dvPMDaTra = new DataView(dt);
                dgvPMDaTra.DataSource = dvPMDaTra;
            }
        }

        private void LoadSachTra(string maPhieuMuon)
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "select MaSach, TinhTrangMuon, DaTraSach, TinhTrangTra from CT_PhieuMuon " +
                    $"where MaPhieuMuon = '{maPhieuMuon}'";
                adapter = new SqlDataAdapter(sql, con);
                dt = new DataTable();
                adapter.Fill(dt);
                dgvSachTra.DataSource = dt;
            }
        }

        private string selectedMaPM, selectedMaDG;
        private void dgvPMDaTra_SelectionChanged(object sender, EventArgs e)
        {
            if (addNewFlag)
            {
                DialogResult result = MessageBox.Show(
                    "Bạn có muốn hủy tạo mới? ",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    addNewFlag = false;
                    NapCT();
                    LoadSachTra(selectedMaPM);
                    SetStyle();  // ← GỌI
                }
                else
                {
                    return;
                }
            }
            else
            {
                NapCT();
                LoadSachTra(selectedMaPM);
            }
        }

        private void txtSearch1_TextChanged(object sender, EventArgs e)
        {
            if (cboTruong1.SelectedIndex == 0)
            {
                dvPMCanTra.RowFilter = $"MaPhieuMuon like '%{txtSearch1.Text}%'";
            }
            else
            {
                dvPMCanTra.RowFilter = $"MaDocGia like '%{txtSearch1.Text}%'";
            }
        }

        private void txtSearch2_TextChanged(object sender, EventArgs e)
        {
            FilterDataPhieuTra();
        }

        private void cboTrangThai_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterDataPhieuTra();
        }

        private void FilterDataPhieuTra()
        {
            if (dvPMDaTra == null)
                return;

            List<string> filters = new List<string>();

            // === LỌC THEO TRẠNG THÁI ===
            if (cboTrangThai.SelectedIndex > 0)  // Không phải "Tất cả"
            {
                string trangThai = cboTrangThai.SelectedItem.ToString();

                if (trangThai == "Trả đúng hạn")
                {
                    filters.Add("SoNgayTre <= 0");
                }
                else if (trangThai == "Trả trễ")
                {
                    filters.Add("SoNgayTre > 0");
                }
            }

            // === LỌC THEO TÌM KIẾM ===
            string searchText = txtSearch2.Text.Trim();
            if (!string.IsNullOrEmpty(searchText))
            {
                string column = "";

                switch (cboTruong2.SelectedIndex)
                {
                    case 0:  // Mã phiếu mượn
                        column = "MaPhieuMuon";
                        break;
                    case 1:  // Mã độc giả
                        column = "MaDocGia";
                        break;
                }

                if (!string.IsNullOrEmpty(column))
                {
                    filters.Add($"{column} LIKE '%{searchText}%'");
                }
            }

            // ✅ LỌC THEO NGÀY TRẢ (NẾU CÓ)
            if (filterTuNgay.HasValue && filterDenNgay.HasValue)
            {
                filters.Add($"NgayThucTra >= #{filterTuNgay.Value: MM/dd/yyyy}#");
                filters.Add($"NgayThucTra <= #{filterDenNgay.Value:MM/dd/yyyy}#");
            }

            // === KẾT HỢP CÁC BỘ LỌC ===
            dvPMDaTra.RowFilter = string.Join(" AND ", filters);
            UpdateTongSo();
        }

        private void btnTaoMoi_Click(object sender, EventArgs e)
        {
            txtMaPhieuMuon.Enabled = true;
            txtMaPhieuMuon.Text = "";
            txtMaPhieuMuon.Focus();

            txtSoNgayTre.Text = "";
            txtSoNgayTre.Enabled = false;
            txtMaDG.Text = "";
            dtNgayMuon.Value = DateTime.Now;
            dtHanTra.Value = DateTime.Now;
            dtNgayThucTra.Value = DateTime.Now;
            addNewFlag = true;
            SetStyle();
        }

        private void dgvPMCanTra_DoubleClick(object sender, EventArgs e)
        {
            if (addNewFlag)
            {
                if (dgvPMCanTra.SelectedRows.Count > 0)
                {
                    txtMaPhieuMuon.Text = dgvPMCanTra.SelectedRows[0].Cells[0].Value.ToString();
                    txtMaDG.Text = dgvPMCanTra.SelectedRows[0].Cells[1].Value.ToString();
                    dtNgayMuon.Text = dgvPMCanTra.SelectedRows[0].Cells[2].Value.ToString();
                    dtHanTra.Text = dgvPMCanTra.SelectedRows[0].Cells[3].Value.ToString();
                }
                else
                {
                    MessageBox.Show("Chọn cả dòng để thực hiện chức năng này");
                }
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (addNewFlag)
            {
                string maPhieuMuon = txtMaPhieuMuon.Text;
                if (string.IsNullOrEmpty(maPhieuMuon))
                {
                    MessageBox.Show("Chưa chọn phiếu để trả");
                    return;
                }
                using (con = new SqlConnection(strCon))
                {
                    con.Open();
                    string sql = "Update PhieuMuon set NgayThucTra = @NgayThucTra " +
                        $"where MaPhieuMuon = '{maPhieuMuon}'";
                    cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@NgayThucTra", dtNgayThucTra.Value.ToString("yyyy-MM-dd"));
                    try
                    {
                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            MessageBox.Show("Đã cập nhật ngày trả!, click Trả sách để lưu thông tin sách trả", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Trả thất bại!", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        addNewFlag = false;
                        LoadPMDaTra();
                        FilterDataPhieuTra();
                        LoadPMCanTra();
                        SetStyle();
                        //Tìm dòng chứa mã của bản ghi vừa thêm
                        foreach (DataGridViewRow row in dgvPMDaTra.Rows)
                        {
                            if (row.Cells[0].Value.ToString() == maPhieuMuon)
                            {
                                dgvPMDaTra.ClearSelection();
                                dgvPMDaTra.CurrentCell = row.Cells[0];
                                NapCT();
                                dgvPMDaTra.FirstDisplayedScrollingRowIndex = row.Index; // Cuộn đến dòng vừa thêm
                                break;
                            }
                        }
                        LoadSachTra(maPhieuMuon);
                        
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Ngày thực trả phải lớn hơn ngày mượn", "Lỗi",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedMaPM))
            {
                MessageBox.Show("Chưa chọn phiếu mượn để xóa thông tin trả");
                return;
            }

            int currentIndex = dgvPMDaTra.CurrentRow.Index;

            DialogResult rs = MessageBox.Show("Bạn có chắc chắn muốn xóa thông tin trả cho phiếu mượn này không? ",
                "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (rs == DialogResult.Yes)
            {
                using (con = new SqlConnection(strCon))
                {
                    con.Open();

                    try
                    {
                        // === KIỂM TRA CÓ PHIẾU PHẠT KHÔNG ===
                        string sqlCheck = "SELECT COUNT(*) FROM PhieuPhat WHERE MaPhieuMuon = @MaPhieuMuon";
                        cmd = new SqlCommand(sqlCheck, con);
                        cmd.Parameters.AddWithValue("@MaPhieuMuon", selectedMaPM);
                        int count = (int)cmd.ExecuteScalar();

                        if (count > 0)
                        {
                            MessageBox.Show("Không thể xóa!  Phiếu mượn này đã có phiếu phạt.", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // 1. Xóa thông tin trả trong PhieuMuon
                        string sql1 = "UPDATE PhieuMuon SET NgayThucTra = NULL WHERE MaPhieuMuon = @MaPhieuMuon";
                        cmd = new SqlCommand(sql1, con);
                        cmd.Parameters.AddWithValue("@MaPhieuMuon", selectedMaPM);
                        cmd.ExecuteNonQuery();

                        // 2. Xóa thông tin trả trong CT_PhieuMuon
                        string sql2 = "UPDATE CT_PhieuMuon SET DaTraSach = 0, TinhTrangTra = NULL WHERE MaPhieuMuon = @MaPhieuMuon";
                        cmd = new SqlCommand(sql2, con);
                        cmd.Parameters.AddWithValue("@MaPhieuMuon", selectedMaPM);
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Xóa thông tin trả sách thành công!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        LoadPMDaTra();
                        FilterDataPhieuTra();
                        LoadPMCanTra();

                        if (dgvPMDaTra.Rows.Count > 0)
                        {
                            int beforeRowIndex = Math.Max(0, currentIndex - 1);
                            dgvPMDaTra.ClearSelection();
                            dgvPMDaTra.CurrentCell = dgvPMDaTra.Rows[beforeRowIndex].Cells[0];
                            dgvPMDaTra.FirstDisplayedScrollingRowIndex = beforeRowIndex;
                            NapCT();
                            LoadSachTra(txtMaPhieuMuon.Text);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Xóa thất bại! " + ex.Message, "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            // === VALIDATION ===
            if (string.IsNullOrEmpty(selectedMaPM))
            {
                MessageBox.Show("Chưa chọn phiếu mượn để sửa thông tin trả", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // ✅ LƯU MÃ PHIẾU MƯỢN (không dùng index)
            string currentMaPM = selectedMaPM;

            // === UPDATE DATABASE ===
            using (con = new SqlConnection(strCon))
            {
                con.Open();

                // ✅ SỬA SQL - Dùng parameter thay vì string interpolation
                string sql = "UPDATE PhieuMuon SET NgayThucTra = @NgayThucTra WHERE MaPhieuMuon = @MaPhieuMuon";
                cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@NgayThucTra", dtNgayThucTra.Value.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@MaPhieuMuon", selectedMaPM);

                try
                {
                    int kq = cmd.ExecuteNonQuery();

                    if (kq > 0)
                    {
                        MessageBox.Show("Sửa thông tin trả thành công!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Sửa thất bại!", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // === LOAD LẠI DỮ LIỆU ===
                    LoadPMDaTra();
                    LoadPMCanTra();

                    // ✅ FILTER TRƯỚC KHI CHỌN DÒNG
                    FilterDataPhieuTra();  // ← Di chuyển lên trước

                    // === TÌM DÒNG VỪA SỬA ===
                    int newIndex = -1;
                    foreach (DataGridViewRow row in dgvPMDaTra.Rows)
                    {
                        if (row.Cells["MaPhieuMuon"].Value?.ToString() == currentMaPM)
                        {
                            newIndex = row.Index;
                            break;
                        }
                    }

                    // === CHỌN LẠI DÒNG ===
                    if (dgvPMDaTra.Rows.Count > 0)
                    {
                        dgvPMDaTra.ClearSelection();
                        dgvPMDaTra.CurrentCell = dgvPMDaTra.Rows[newIndex >= 0 ? newIndex : 0].Cells[0];
                    }

                    // === NẠP CHI TIẾT ===
                    NapCT();
                    LoadSachTra(selectedMaPM);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi:  " + ex.Message, "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {

        }

        private void btnInPhieuTra_Click(object sender, EventArgs e)
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "select ctpm.MaSach, ds.TenDauSach as TenSach, ctpm.TinhTrangMuon, ctpm.TinhTrangTra " +
                "from CT_PhieuMuon ctpm join CuonSach cs on ctpm.MaSach = cs.MaSach join DauSach ds on cs.MaDauSach = ds.MaDauSach " +
                $"where ctpm.MaPhieuMuon = '{txtMaPhieuMuon.Text}' and ctpm.DaTraSach = 1";
                adapter = new SqlDataAdapter(sql, con);
                dt = new DataTable();
                adapter.Fill(dt);
                string mapm = txtMaPhieuMuon.Text;
                string madg = txtMaDG.Text;
                string sqlHoTen = $"Select HoTen from DocGia where MaDocGia = '{madg}'";
                cmd = new SqlCommand(sqlHoTen, con);
                string hoten = cmd.ExecuteScalar().ToString();
                string ngayMuon = dtHanTra.Value.ToString("dd/MM/yyyy");
                string hanTra = dtNgayThucTra.Value.ToString("dd/MM/yyyy");
                string soNgayTre = txtSoNgayTre.Text;
                using (frmInPhieuTra reportForm = new frmInPhieuTra(dt, mapm, madg, hoten, ngayMuon, hanTra, soNgayTre))
                {
                    reportForm.ShowDialog();
                }
            }
        }

        private void btnLocNgay_Click(object sender, EventArgs e)
        {
            using (var f = new frmLocNgay())
            {
                if (f.ShowDialog() == DialogResult.OK)
                {
                    if (f.IsApplied && f.TuNgay.HasValue && f.DenNgay.HasValue)
                    {
                        // ✅ ÁP DỤNG LỌC NGÀY
                        filterTuNgay = f.TuNgay.Value;
                        filterDenNgay = f.DenNgay.Value;
                    }
                    else
                    {
                        // ✅ NGỪNG ÁP DỤNG
                        filterTuNgay = null;
                        filterDenNgay = null;
                    }

                    FilterDataPhieuTra();
                }
            }
        }

        private void NapCT()
        {
            // ✅ KIỂM TRA CÓ DÒNG KHÔNG
            if (dgvPMDaTra.Rows.Count == 0) return;

            if (dgvPMDaTra.CurrentCell != null && dgvPMDaTra.CurrentCell.RowIndex >= 0)
            {
                int i = dgvPMDaTra.CurrentRow.Index;

                // ✅ DÙNG TÊN CỘT THAY VÌ INDEX

                // Mã phiếu mượn
                selectedMaPM = dgvPMDaTra.Rows[i].Cells["MaPhieuMuon"]?.Value?.ToString() ?? "";
                txtMaPhieuMuon.Text = selectedMaPM;
                txtMaPhieuMuon.Enabled = string.IsNullOrEmpty(selectedMaPM);

                // Mã độc giả
                selectedMaDG = dgvPMDaTra.Rows[i].Cells["MaDocGia"]?.Value?.ToString() ?? "";
                txtMaDG.Text = selectedMaDG;

                // Ngày mượn
                var ngayMuon = dgvPMDaTra.Rows[i].Cells["NgayMuon"]?.Value;
                if (ngayMuon != null && ngayMuon != DBNull.Value)
                {
                    dtNgayMuon.Value = Convert.ToDateTime(ngayMuon);
                }
                else
                {
                    dtNgayMuon.Value = DateTime.Now;
                }

                // Hạn trả
                var hanTra = dgvPMDaTra.Rows[i].Cells["HanTra"]?.Value;
                if (hanTra != null && hanTra != DBNull.Value)
                {
                    dtHanTra.Value = Convert.ToDateTime(hanTra);
                }
                else
                {
                    dtHanTra.Value = DateTime.Now;
                }

                // Ngày thực trả
                var ngayThucTra = dgvPMDaTra.Rows[i].Cells["NgayThucTra"]?.Value;
                if (ngayThucTra != null && ngayThucTra != DBNull.Value)
                {
                    dtNgayThucTra.Value = Convert.ToDateTime(ngayThucTra);
                }
                else
                {
                    dtNgayThucTra.Value = DateTime.Now;
                }

                // Số ngày trễ
                var soNgayTre = dgvPMDaTra.Rows[i].Cells["SoNgayTre"]?.Value;
                if (soNgayTre != null && soNgayTre != DBNull.Value)
                {
                    txtSoNgayTre.Text = soNgayTre.ToString();
                }
                else
                {
                    txtSoNgayTre.Text = "0";
                }
                txtSoNgayTre.Enabled = !string.IsNullOrEmpty(txtSoNgayTre.Text);
            }
        }
    }
}
