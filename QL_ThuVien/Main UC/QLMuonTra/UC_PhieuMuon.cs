using QL_ThuVien.Form_support;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QL_ThuVien.Main_UC.QLMuonTra
{
    public partial class UC_PhieuMuon : UserControl
    {
        string strCon = DBConfig.ConnectionString;
        SqlConnection con;
        SqlDataAdapter adapter;
        SqlCommand cmd;
        DataTable dt;
        DataView dvDG;
        DataView dvPM;
        bool addNewFlag = false;

        private DateTime? filterTuNgay = null;
        private DateTime? filterDenNgay = null;

        private string userRole;
        private string maThuThu;
        public UC_PhieuMuon(string role, string maThuThu)
        {
            InitializeComponent();
            userRole = role;
            this.maThuThu = maThuThu;
        }

        private void UC_PhieuMuon_Load(object sender, EventArgs e)
        {
            // Setup disabled style cho các nút
            SetupButtonDisabledStyle(btnSua);
            SetupButtonDisabledStyle(btnXoa);
            SetupButtonDisabledStyle(btnMuonSach);
            SetupButtonDisabledStyle(btnInPhieuMuon);
            SetupButtonDisabledStyle(btnLocNgay);


            cboTruong1.SelectedIndex = 0;
            cboTruong2.SelectedIndex = 0;

            LoadComboBox(cboKieuMuon,"KieuMuon", "MaKieuMuon", "TenKieuMuon");
            LoadComboBox(cboThuThu, "ThuThu", "MaThuThu", "TenThuThu");
            //Fix lỗi column header
            dgvDocGia.ColumnHeadersDefaultCellStyle.Font = new Font(dgvDocGia.Font, FontStyle.Bold);
            dgvPhieuMuon.ColumnHeadersDefaultCellStyle.Font = new Font(dgvPhieuMuon.Font, FontStyle.Bold);
            dgvDocGia.Columns["DangMuonSach"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvDocGia.Columns["HoatDong"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvSachMuon.DefaultCellStyle.Font = new Font(dgvSachMuon.Font, FontStyle.Regular);

            LoadDocGia();
            LoadPhieuMuon();
            SetStyle();
            UpdateTongSo();
        }
        private void UpdateTongSo()
        {
            lblTongSo.Text = dgvPhieuMuon.Rows.Count.ToString();
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
            bool isTaoMoi = addNewFlag;
            bool hasRow = dgvPhieuMuon.CurrentRow != null;
            bool daTra = false;

            if (!isTaoMoi && hasRow)
            {
                string trangThai = dgvPhieuMuon.CurrentRow.Cells["TrangThai"].Value?.ToString() ?? "";
                daTra = (trangThai == "Trả đúng hạn" || trangThai == "Trả trễ");
            }

            // CHỈ SET ENABLED - Guna2 tự động áp dụng DisabledState
            btnSua.Enabled = !isTaoMoi && hasRow && !daTra;
            btnXoa.Enabled = !isTaoMoi && hasRow;
            btnMuonSach.Enabled = !isTaoMoi && hasRow;
            btnInPhieuMuon.Enabled = !isTaoMoi && hasRow;
            btnLocNgay.Enabled = !isTaoMoi;
        }


        private void LoadComboBox(ComboBox cbo, string tableName, string Ma, string TenMa)
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "";
                if (addNewFlag)
                {
                    sql = $"SELECT * FROM {tableName} WHERE TRANGTHAI = 1";
                } else
                {
                    sql = $"SELECT * FROM {tableName}";
                }
                SqlDataAdapter adapter = new SqlDataAdapter(sql, con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                // Thêm cột mới kết hợp mã và tên
                dt.Columns.Add("DisplayColumn", typeof(string), $"{Ma} + ' - ' + {TenMa}");

                cbo.DataSource = dt;
                cbo.ValueMember = Ma;
                cbo.DisplayMember = "DisplayColumn";
                cbo.SelectedIndex = -1;
            }
        }

        private void LoadPhieuMuon()
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();

                string sql = @"
                    SELECT 
                        pm.MaPhieuMuon, 
                        pm.MaDocGia,
                        dg.HoTen AS TenDocGia,
                        pm.MaKieuMuon, 
                        pm.NgayMuon, 
                        pm. HanTra, 
                        COALESCE(SUM(ctpm.TienCoc), 0) AS TongTienCoc, 
                        pm. MaThuThu as ThuThu,
                        tt.TenThuThu,
                        CASE 
                            WHEN pm.NgayThucTra IS NULL AND CAST(GETDATE() AS DATE) <= CAST(pm.HanTra AS DATE)
                                THEN N'Còn hạn mượn'
                            WHEN pm.NgayThucTra IS NULL AND CAST(GETDATE() AS DATE) > CAST(pm.HanTra AS DATE)
                                THEN N'Quá hạn mượn'
                            WHEN pm.NgayThucTra IS NOT NULL AND CAST(pm.NgayThucTra AS DATE) <= CAST(pm.HanTra AS DATE)
                                THEN N'Trả đúng hạn'
                            WHEN pm.NgayThucTra IS NOT NULL AND CAST(pm.NgayThucTra AS DATE) > CAST(pm.HanTra AS DATE)
                                THEN N'Trả trễ'
                        END AS TrangThai
                    FROM PhieuMuon pm 
                    LEFT JOIN CT_PhieuMuon ctpm ON pm.MaPhieuMuon = ctpm.MaPhieuMuon
                    LEFT JOIN DocGia dg ON pm.MaDocGia = dg.MaDocGia
                    LEFT JOIN ThuThu tt ON pm.MaThuThu = tt.MaThuThu
                    GROUP BY 
                        pm.MaPhieuMuon, 
                        pm.MaDocGia,
                        dg.HoTen,
                        pm.MaKieuMuon, 
                        pm.NgayMuon, 
                        pm. HanTra, 
                        pm.NgayThucTra,
                        pm.MaThuThu,
                        tt.TenThuThu
                    ORDER BY pm.MaPhieuMuon DESC";

                adapter = new SqlDataAdapter(sql, con);
                dt = new DataTable();
                adapter.Fill(dt);
                dvPM = new DataView(dt);
                dgvPhieuMuon.DataSource = dvPM;
            }
        }

        private void LoadSachMuon(string maPhieuMuon)
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "select ctpm.MaSach, ds.TenDauSach, ctpm.TienCoc, ctpm.TinhTrangMuon " +
                    "from CT_PhieuMuon ctpm " +
                    "join CuonSach cs on ctpm.MaSach = cs.MaSach " +
                    "join DauSach ds on cs.MaDauSach = ds.MaDauSach " +
                    $"where ctpm.MaPhieuMuon = '{maPhieuMuon}'";
                adapter = new SqlDataAdapter(sql, con);
                dt = new DataTable();
                adapter.Fill(dt);
                dgvSachMuon.DataSource= dt;
                txtSoLuongMuon.Text = dgvSachMuon.RowCount.ToString();

                // Tính tổng tiền cọc
                float totalTienCoc = 0;
                foreach (DataRow row in dt.Rows)
                {
                    if (float.TryParse(row["TienCoc"].ToString(), out float tienCoc))
                    {
                        totalTienCoc += tienCoc;
                    }
                }

                txtTienCoc.Text = totalTienCoc.ToString();

            }
        }

        private void LoadDocGia()
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();

                string sql = @"
                    SELECT 
                        dg.MaDocGia, 
                        dg.HoTen,
                        CASE 
                            WHEN EXISTS (
                                SELECT 1 
                                FROM PhieuMuon pm 
                                WHERE pm.MaDocGia = dg.MaDocGia 
                                AND pm.NgayThucTra IS NULL
                            ) 
                            THEN 1
                            ELSE 0 
                        END AS DangMuonSach,
                        dg.TrangThai AS HoatDong
                    FROM DocGia dg
                    ORDER BY dg.MaDocGia";

                adapter = new SqlDataAdapter(sql, con);
                dt = new DataTable();
                adapter.Fill(dt);
                dvDG = new DataView(dt);
                dgvDocGia.DataSource = dvDG;
            }
        }

        private void txtSearch1_TextChanged(object sender, EventArgs e)
        {
            if (cboTruong1.SelectedIndex == 0)
            {
                dvDG.RowFilter = $"MaDocGia like '%{txtSearch1.Text}%'";
            }
            else
            {
                dvDG.RowFilter = $"HoTen like '%{txtSearch1.Text}%'";
            }
        }

        private void cboTruong2_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterData();
        }

        private void txtSearch2_TextChanged(object sender, EventArgs e)
        {
            FilterData();
        }

        private void cboTrangThai_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterData();
        }

        private void FilterData()
        {
            if (dvPM == null)
                return;

            List<string> filters = new List<string>();

            // === LỌC THEO TRẠNG THÁI ===
            if (cboTrangThai.SelectedIndex > 0)
            {
                string trangThai = cboTrangThai.SelectedItem.ToString();
                filters.Add($"TrangThai = '{trangThai}'");
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
                    case 2:  // Mã thủ thư
                        column = "ThuThu";
                        break;
                }

                if (!string.IsNullOrEmpty(column))
                {
                    filters.Add($"{column} LIKE '%{searchText}%'");
                }
            }

            // ✅ LỌC THEO NGÀY MƯỢN (NẾU CÓ)
            if (filterTuNgay.HasValue && filterDenNgay.HasValue)
            {
                filters.Add($"NgayMuon >= #{filterTuNgay.Value: MM/dd/yyyy}#");
                filters.Add($"NgayMuon <= #{filterDenNgay.Value:MM/dd/yyyy}#");
            }

            // === KẾT HỢP CÁC BỘ LỌC ===
            dvPM.RowFilter = string.Join(" AND ", filters);
            UpdateTongSo();
        }

        private string selectedMaPM, selectedMaDG;
        private void NapCT()
        {
            if (dgvPhieuMuon.CurrentCell != null && dgvPhieuMuon.CurrentCell.RowIndex >= 0)
            {
                int i = dgvPhieuMuon.CurrentRow.Index;

                // ✅ DÙNG DataPropertyName THAY VÌ INDEX
                selectedMaPM = dgvPhieuMuon.Rows[i].Cells["MaPhieuMuon"]?.Value?.ToString() ?? "";
                txtMaPhieuMuon.Text = selectedMaPM;
                txtMaPhieuMuon.Enabled = string.IsNullOrEmpty(selectedMaPM);

                selectedMaDG = dgvPhieuMuon.Rows[i].Cells["MaDG2"]?.Value?.ToString() ?? "";
                txtMaDG.Text = selectedMaDG;

                // ✅ XỬ LÝ AN TOÀN CHO COMBOBOX
                string maKieuMuon = dgvPhieuMuon.Rows[i].Cells["MaKieuMuon"]?.Value?.ToString();
                if (!string.IsNullOrEmpty(maKieuMuon))
                {
                    cboKieuMuon.SelectedValue = maKieuMuon;
                }
                else
                {
                    cboKieuMuon.SelectedIndex = -1;
                }

                // ✅ XỬ LÝ NGÀY THÁNG
                if (dgvPhieuMuon.Rows[i].Cells["NgayMuon"].Value != null &&
                    dgvPhieuMuon.Rows[i].Cells["NgayMuon"].Value != DBNull.Value)
                {
                    dtNgayMuon.Value = Convert.ToDateTime(dgvPhieuMuon.Rows[i].Cells["NgayMuon"].Value);
                }
                else
                {
                    dtNgayMuon.Value = DateTime.Now;
                }

                if (dgvPhieuMuon.Rows[i].Cells["HanTra"].Value != null &&
                    dgvPhieuMuon.Rows[i].Cells["HanTra"].Value != DBNull.Value)
                {
                    dtHanTra.Value = Convert.ToDateTime(dgvPhieuMuon.Rows[i].Cells["HanTra"].Value);
                }
                else
                {
                    dtHanTra.Value = DateTime.Now;
                }

                // ✅ TIỀN CỌC
                txtTienCoc.Text = dgvPhieuMuon.Rows[i].Cells["TongTienCoc"]?.Value?.ToString() ?? "0";
                txtTrangThai.Text = dgvPhieuMuon.Rows[i].Cells["TrangThai"]?.Value?.ToString() ?? "";

                // ✅ THỦ THƯ
                string maThuThu = dgvPhieuMuon.Rows[i].Cells["ThuThu"]?.Value?.ToString();
                if (!string.IsNullOrEmpty(maThuThu))
                {
                    cboThuThu.SelectedValue = maThuThu;
                }
                else
                {
                    cboThuThu.SelectedIndex = -1;
                }
                cboThuThu.Enabled = true;
            }
        }

        private void btnTaoMoi_Click(object sender, EventArgs e)
        {

            //gen mã phiếu mượn
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "Select max(MaPhieuMuon) from PhieuMuon";
                cmd = new SqlCommand(sql, con);
                object rs = cmd.ExecuteScalar();
                if (rs != DBNull.Value && rs != null)
                {
                    string maPhieuMuon = rs.ToString();
                    int number = int.Parse(maPhieuMuon.Substring(2)); //Lấy sau phầm "PM"
                    ++number;
                    txtMaPhieuMuon.Text = "PM" + number.ToString("D4");
                }
            }
            LoadSachMuon("");

            txtMaDG.Text = "";
            cboKieuMuon.SelectedIndex = -1;
            dtNgayMuon.Value = DateTime.Now;
            dtHanTra.Value = DateTime.Now;
            if (userRole == "thuthu")
            {
                cboThuThu.SelectedValue = maThuThu; // Gán mã thủ thư vào combo box
                cboThuThu.Enabled = false; // Không cho phép sửa
            }
            else
            {
                cboThuThu.SelectedIndex = -1;
            }

            txtSoLuongMuon.Text = "";
            txtTienCoc.Text = "";
            txtTrangThai.Text = "Còn hạn mượn";

            txtMaDG.Focus();
            addNewFlag = true;
            dtHanTra.Enabled = false;
            txtSoLuongMuon.Enabled = false;
            txtTienCoc.Enabled = false;
            txtTrangThai.Enabled = false;
            SetStyle();
            LoadComboBox(cboKieuMuon, "KieuMuon", "MaKieuMuon", "TenKieuMuon");
            LoadComboBox(cboThuThu, "ThuThu", "MaThuThu", "TenThuThu");
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedMaPM))
            {
                MessageBox.Show("Vui lòng chọn một phiếu mượn để xóa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int currentIndex = dgvPhieuMuon.CurrentRow.Index;

            DialogResult result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa phiếu mượn với mã {selectedMaPM} không?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                using (SqlConnection con = new SqlConnection(strCon))
                {
                    try
                    {
                        con.Open();
                        string sql = "DELETE FROM PhieuMuon WHERE MaPhieuMuon = @MaPhieuMuon";
                        using (SqlCommand cmd = new SqlCommand(sql, con))
                        {
                            cmd.Parameters.AddWithValue("@MaPhieuMuon", selectedMaPM);
                            int kq = cmd.ExecuteNonQuery();

                            if (kq > 0)
                            {
                                MessageBox.Show("Xóa phiếu mượn thành công!", "Thông báo",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                                LoadPhieuMuon();
                                LoadDocGia();
                                FilterData();


                                // === CHỌN LẠI DÒNG SAU KHI XÓA ===
                                if (dgvPhieuMuon.Rows.Count > 0)
                                {
                                    int newIndex;

                                    // Nếu index cũ >= số dòng hiện tại → chọn dòng cuối
                                    if (currentIndex >= dgvPhieuMuon.Rows.Count)
                                    {
                                        newIndex = dgvPhieuMuon.Rows.Count - 1;
                                    }
                                    else
                                    {
                                        // Giữ nguyên vị trí (dòng mới sẽ lên thay chỗ dòng bị xóa)
                                        newIndex = currentIndex;
                                    }

                                    dgvPhieuMuon.ClearSelection();
                                    dgvPhieuMuon.CurrentCell = dgvPhieuMuon.Rows[newIndex].Cells[0];
                                    dgvPhieuMuon.FirstDisplayedScrollingRowIndex = newIndex;

                                    NapCT();
                                    LoadSachMuon(txtMaPhieuMuon.Text);
                                    LoadPhieuMuon_DocGia(txtMaDG.Text);
                                }
                                else
                                {
                                    // Không còn dòng nào
                                    selectedMaPM = "";
                                    txtMaPhieuMuon.Clear();
                                    txtMaDG.Clear();
                                    // Clear các field khác nếu cần
                                }
                            }
                            else
                            {
                                MessageBox.Show("Xóa phiếu mượn không thành công!", "Thông báo",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Phiếu mượn liên quan tới nhiều bảng dữ liệu khác",
                            "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();

                // ← THÊM HanTra VÀO CÂU SQL
                string sql = "INSERT INTO PhieuMuon(MaPhieuMuon, MaDocGia, MaKieuMuon, NgayMuon, HanTra, MaThuThu) " +
                             "VALUES (@MaPhieuMuon, @MaDocGia, @MaKieuMuon, @NgayMuon, @HanTra, @MaThuThu)";
                cmd = new SqlCommand(sql, con);

                cmd.Parameters.AddWithValue("@MaPhieuMuon", txtMaPhieuMuon.Text);
                cmd.Parameters.AddWithValue("@MaDocGia", txtMaDG.Text);
                cmd.Parameters.AddWithValue("@MaKieuMuon", cboKieuMuon.SelectedValue);
                cmd.Parameters.AddWithValue("@NgayMuon", dtNgayMuon.Value.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@HanTra", dtHanTra.Value.ToString("yyyy-MM-dd"));  // ← THÊM DÒNG NÀY
                cmd.Parameters.AddWithValue("@MaThuThu", cboThuThu.SelectedValue);

                int kq = cmd.ExecuteNonQuery();

                if (kq > 0)
                {
                    MessageBox.Show("Thêm phiếu mượn thành công, click Mượn sách để lưu thông tin sách mượn!",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    addNewFlag = false;

                    LoadPhieuMuon();
                    LoadDocGia();
                    FilterData();


                    //int lastRowIndex = dgvPhieuMuon.RowCount - 1;
                    //dgvPhieuMuon.ClearSelection();
                    //dgvPhieuMuon.CurrentCell = dgvPhieuMuon.Rows[lastRowIndex].Cells[0];
                    //dgvPhieuMuon.FirstDisplayedScrollingRowIndex = lastRowIndex;

                    LoadComboBox(cboKieuMuon, "KieuMuon", "MaKieuMuon", "TenKieuMuon");
                    LoadComboBox(cboThuThu, "ThuThu", "MaThuThu", "TenThuThu");

                    NapCT();
                    LoadSachMuon(txtMaPhieuMuon.Text);
                    LoadPhieuMuon_DocGia(txtMaDG.Text);
                    SetStyle();

                }
                else
                {
                    MessageBox.Show("Không thể thêm phiếu mượn!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            // === VALIDATION ===
            if (string.IsNullOrEmpty(selectedMaPM))
            {
                MessageBox.Show("Chưa chọn bản ghi để sửa", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // ✅ LƯU MÃ PHIẾU MƯỢN (không dùng index)
            string currentMaPM = selectedMaPM;

            // === UPDATE DATABASE ===
            using (con = new SqlConnection(strCon))
            {
                con.Open();

                string sql = "UPDATE PhieuMuon " +
                             "SET MaDocGia = @MaDocGia, MaKieuMuon = @MaKieuMuon, NgayMuon = @NgayMuon, HanTra = @HanTra, MaThuThu = @MaThuThu " +
                             "WHERE MaPhieuMuon = @MaPhieuMuon";
                cmd = new SqlCommand(sql, con);

                cmd.Parameters.AddWithValue("@MaPhieuMuon", selectedMaPM);
                cmd.Parameters.AddWithValue("@MaDocGia", txtMaDG.Text);
                cmd.Parameters.AddWithValue("@MaKieuMuon", cboKieuMuon.SelectedValue);
                cmd.Parameters.AddWithValue("@NgayMuon", dtNgayMuon.Value.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@HanTra", dtHanTra.Value.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@MaThuThu", cboThuThu.SelectedValue);

                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Sửa phiếu mượn thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Sửa thất bại!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // === LOAD LẠI DỮ LIỆU ===
            LoadPhieuMuon();
            LoadDocGia();

            // ✅ LOAD COMBOBOX TRƯỚC FilterData()
            LoadComboBox(cboKieuMuon, "KieuMuon", "MaKieuMuon", "TenKieuMuon");
            LoadComboBox(cboThuThu, "ThuThu", "MaThuThu", "TenThuThu");

            FilterData();

            // === TÌM DÒNG VỪA SỬA ===
            int newIndex = -1;
            foreach (DataGridViewRow row in dgvPhieuMuon.Rows)
            {
                if (row.Cells["MaPhieuMuon"].Value?.ToString() == currentMaPM)
                {
                    newIndex = row.Index;
                    break;
                }
            }

            // === CHỌN LẠI DÒNG ===
            if (dgvPhieuMuon.Rows.Count > 0)
            {
                dgvPhieuMuon.ClearSelection();
                dgvPhieuMuon.CurrentCell = dgvPhieuMuon.Rows[newIndex >= 0 ? newIndex : 0].Cells[0];
            }

            // === NẠP CHI TIẾT ===
            NapCT();
            LoadSachMuon(selectedMaPM);
            LoadPhieuMuon_DocGia(selectedMaDG);
            SetStyle();
        }

        private void dgvDocGia_DoubleClick(object sender, EventArgs e)
        {
            if (addNewFlag)
            {
                if (dgvDocGia.SelectedRows.Count > 0)
                {
                    string maDocGia = dgvDocGia.SelectedRows[0].Cells[0].Value.ToString();

                    bool hoatDong = Convert.ToBoolean(dgvDocGia.SelectedRows[0].Cells["HoatDong"].Value);

                    if (!hoatDong)
                    {
                        MessageBox.Show("Độc giả không hoạt động, không thể tạo phiếu mượn!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    bool dangMuonSach = Convert.ToBoolean(dgvDocGia.SelectedRows[0].Cells["DangMuonSach"].Value);

                    if (dangMuonSach)
                    {
                        MessageBox.Show("Độc giả chưa trả sách, không tạo phiếu mượn", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    txtMaDG.Text = maDocGia;
                }
                else
                {
                    MessageBox.Show("Chọn cả dòng để thực hiện chức năng này");
                }
            }
        }

        private void LoadPhieuMuon_DocGia (string maDocGia)
        {
            if (dgvDocGia == null) { return; }
            foreach (DataGridViewRow row in dgvDocGia.Rows)
            {
                // Kiểm tra nếu giá trị trong cột MaDocGia khớp với mã cần tìm
                if (row.Cells[0].Value != null && row.Cells[0].Value.ToString() == maDocGia)
                {
                    dgvDocGia.ClearSelection();
                    row.Cells[0].Selected = true;
                    dgvDocGia.FirstDisplayedScrollingRowIndex = row.Index;
                    return;
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {

        }

        private void btnMuonSach_Click(object sender, EventArgs e)
        {
            var f = new frmNhapSach();
            f.MaPhieuMuon = txtMaPhieuMuon.Text;
            f.MaDocGia = txtMaDG.Text;
            f.KieuMuon = cboKieuMuon.Text;

            f.ShowDialog();

            // ✅ THÊM CODE NÀY - SAU KHI ĐÓNG FORM
            // Lưu vị trí
            int currentIndex = dgvPhieuMuon.CurrentRow?.Index ?? -1;
            string currentMaPM = selectedMaPM;
            string currentMaDG = selectedMaDG;

            // Load lại dữ liệu
            LoadPhieuMuon();
            LoadDocGia();
            FilterData();

            // Chọn lại dòng
            if (currentIndex >= 0 && currentIndex < dgvPhieuMuon.Rows.Count)
            {
                dgvPhieuMuon.ClearSelection();
                dgvPhieuMuon.CurrentCell = dgvPhieuMuon.Rows[currentIndex].Cells[0];
                dgvPhieuMuon.FirstDisplayedScrollingRowIndex = currentIndex;
            }

            // Load lại chi tiết
            NapCT();
            LoadSachMuon(currentMaPM);
            LoadPhieuMuon_DocGia(currentMaDG);
            SetStyle();
            UpdateTongSo();
        }

        private void btnInPhieuMuon_Click(object sender, EventArgs e)
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "select ctpm.MaSach, ds.TenDauSach as TenSach, ctpm.TienCoc, ctpm.TinhTrangMuon " +
                "from CT_PhieuMuon ctpm join CuonSach cs on ctpm.MaSach = cs.MaSach join DauSach ds on cs.MaDauSach = ds.MaDauSach " +
                $"where ctpm.MaPhieuMuon = '{selectedMaPM}'";
                adapter = new SqlDataAdapter(sql, con);
                dt = new DataTable();
                adapter.Fill(dt);
                string mapm = selectedMaPM;
                string madg = txtMaDG.Text;
                string sqlHoTen = $"Select HoTen from DocGia where MaDocGia = '{madg}'";
                cmd = new SqlCommand(sqlHoTen, con);
                string hoten = cmd.ExecuteScalar().ToString();
                string kieumuon = cboKieuMuon.Text.Substring(5);
                string ngayMuon = dtNgayMuon.Value.ToString("dd/MM/yyyy");
                string hanTra = dtHanTra.Value.ToString("dd/MM/yyyy");
                string thuthu = cboThuThu.Text.Substring(7);
                using (frmInPhieuMuon reportForm = new frmInPhieuMuon(dt, mapm, madg, hoten, kieumuon, ngayMuon, hanTra, thuthu))
                {
                    reportForm.ShowDialog();
                }
            }
        }

        private void dtNgayMuon_ValueChanged(object sender, EventArgs e)
        {
            TinhHanTra();
        }

        private void dgvPhieuMuon_SelectionChanged(object sender, EventArgs e)
        {
            if (addNewFlag)
            {
                // === ĐANG TẠO MỚI → HỎI CÓ HỦY KHÔNG ===
                DialogResult result = MessageBox.Show(
                    "Bạn có muốn hủy tạo mới?",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // HỦY TẠO MỚI
                    addNewFlag = false;
                    btnTaoMoi.Text = "Tạo mới";

                    NapCT();

                    txtSoLuongMuon.Enabled = true;
                    txtTienCoc.Enabled = true;
                    dtHanTra.Enabled = true;

                    LoadSachMuon(selectedMaPM);
                    LoadPhieuMuon_DocGia(selectedMaDG);

                    SetStyle();
                }
                else
                {
                    // KHÔNG HỦY → KHÔNG LÀM GÌ
                    return;
                }
            }
            else
            {
                // === CHẾ ĐỘ XEM BÌNH THƯỜNG ===
                NapCT();

                txtSoLuongMuon.Enabled = true;
                txtTienCoc.Enabled = true;
                dtHanTra.Enabled = true;

                LoadSachMuon(selectedMaPM);
                LoadPhieuMuon_DocGia(selectedMaDG);

                SetStyle();
            }
        }

        private void dgvPhieuMuon_DataSourceChanged(object sender, EventArgs e)
        {
            lblTongSo.Text = dgvPhieuMuon.Rows.Count.ToString();
        }

        private void dgvPhieuMuon_DataMemberChanged(object sender, EventArgs e)
        {
            lblTongSo.Text = dgvPhieuMuon.Rows.Count.ToString();
        }

        private void cboKieuMuon_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            TinhHanTra();
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
                        // ✅ NGỪNG ÁP DỤNG - XÓA FILTER NGÀY
                        filterTuNgay = null;
                        filterDenNgay = null;
                    }

                    // ✅ GỌI LẠI FilterData() ĐỂ ÁP DỤNG
                    FilterData();
                }
            }
        }

        private void dtNgayMuon_ValueChanged_1(object sender, EventArgs e)
        {
            TinhHanTra();
        }

        private void TinhHanTra()
        {
            // KIỂM TRA KIỂU MƯỢN ĐÃ ĐƯỢC CHỌN CHƯA
            if (cboKieuMuon.SelectedValue == null || cboKieuMuon.SelectedIndex == -1)
            {
                // CHƯA CHỌN KIỂU MƯỢN → HẠN TRẢ = NGÀY MƯỢN
                dtHanTra.Value = dtNgayMuon.Value;
                return;
            }

            try
            {
                // LẤY SỐ NGÀY MƯỢN TỪ DATABASE
                string maKieuMuon = cboKieuMuon.SelectedValue.ToString();
                int soNgayMuon = 0;

                using (SqlConnection con = new SqlConnection(strCon))
                {
                    con.Open();
                    string sql = "SELECT SoNgayMuon FROM KieuMuon WHERE MaKieuMuon = @MaKieuMuon";
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@MaKieuMuon", maKieuMuon);
                        object result = cmd.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            soNgayMuon = Convert.ToInt32(result);
                        }
                    }
                }

                // TÍNH HẠN TRẢ = NGÀY MƯỢN + SỐ NGÀY MƯỢN
                dtHanTra.Value = dtNgayMuon.Value.AddDays(soNgayMuon);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tính hạn trả:\n{ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                dtHanTra.Value = dtNgayMuon.Value;
            }
        }
    }
}
