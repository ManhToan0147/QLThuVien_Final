using QL_ThuVien.Form_support;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace QL_ThuVien.Main_UC.QLMuonTra
{
    public partial class UC_PhieuPhat : UserControl
    {
        string strCon = DBConfig.ConnectionString;
        SqlConnection con;
        SqlDataAdapter adapter;
        SqlCommand cmd;
        DataTable dt;
        DataView dvPM;
        DataView dvPP;
        bool addNewFlag = false;

        private DateTime? filterTuNgay = null;
        private DateTime? filterDenNgay = null;

        private string userRole;
        private string maThuThu;
        public UC_PhieuPhat(string userRole, string maThuThu)
        {
            InitializeComponent();
            this.userRole = userRole;
            this.maThuThu = maThuThu;
        }

        private void UC_PhieuPhat_Load(object sender, EventArgs e)
        {
            // Setup disabled style
            SetupButtonDisabledStyle(btnSua);
            SetupButtonDisabledStyle(btnXoa);
            SetupButtonDisabledStyle(btnPhatSach);
            SetupButtonDisabledStyle(btnInPhieuPhat);
            SetupButtonDisabledStyle(btnLocNgay);

            LoadCboTrangThai2();
            LoadComboBox(cboThuThu, "ThuThu", "MaThuThu", "TenThuThu");
            cboTruong1.SelectedIndex = 0;
            cboTruong2.SelectedIndex = 0;

            //Fix lỗi dgv
            dgvSachTra.Columns["DaTraSach"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvSachTra.DefaultCellStyle.Font = new Font(dgvSachTra.Font, FontStyle.Regular);
            dgvSachPhat.DefaultCellStyle.Font = new Font(dgvSachPhat.Font, FontStyle.Regular);
            dgvPhieuMuon.ColumnHeadersDefaultCellStyle.Font = new Font(dgvPhieuMuon.Font, FontStyle.Bold);
            dgvPhieuMuon.Columns["QuaHan"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPhieuMuon.Columns["TinhTrangThayDoi"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPhieuMuon.Columns["MatSach"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvPhieuPhat.ColumnHeadersDefaultCellStyle.Font = new Font(dgvSachPhat.Font, FontStyle.Bold);

            LoadPhieuPhat();
            LoadPMDaTra();
            UpdateTongSo();
        }
        private void UpdateTongSo()
        {
            lblTongSo.Text = dgvPhieuPhat.Rows.Count.ToString();
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
                // CHẾ ĐỘ TẠO MỚI - TẮT HẾT TRỪ TẠO MỚI/THÊM
                btnSua.Enabled = false;
                btnXoa.Enabled = false;
                btnInPhieuPhat.Enabled = false;
                btnPhatSach.Enabled = false;
                btnLocNgay.Enabled = false;
            }
            else
            {
                // CHẾ ĐỘ BÌNH THƯỜNG - BẬT HẾT
                btnSua.Enabled = true;
                btnXoa.Enabled = true;
                btnInPhieuPhat.Enabled = true;
                btnPhatSach.Enabled = true;
                btnLocNgay.Enabled = true;

                // RELOAD COMBO - HIỂN THỊ TẤT CẢ
            }
        }

        private void LoadCboTrangThai2()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Value", typeof(int));
            dt.Columns.Add("Display", typeof(string));

            dt.Rows.Add(0, "Chưa nộp");
            dt.Rows.Add(1, "Đã nộp");

            cboTrangThai2.DataSource = dt;
            cboTrangThai2.DisplayMember = "Display";
            cboTrangThai2.ValueMember = "Value";
            cboTrangThai2.SelectedValue = 0;  // Mặc định:  Chưa nộp
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
                }
                else
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
        string selectedMaPP, selectedMaPM2;
        private void NapCT()
        {
            // ✅ KIỂM TRA CÓ DÒNG KHÔNG
            if (dgvPhieuPhat.Rows.Count == 0) return;

            if (dgvPhieuPhat.CurrentCell != null && dgvPhieuPhat.CurrentCell.RowIndex >= 0)
            {
                int i = dgvPhieuPhat.CurrentRow.Index;

                // ✅ DÙNG TÊN CỘT THAY VÌ INDEX

                // Mã phiếu phạt
                selectedMaPP = dgvPhieuPhat.Rows[i].Cells["MaPhieuPhat"]?.Value?.ToString() ?? "";
                txtMaPhieuPhat.Text = selectedMaPP;
                txtMaPhieuPhat.Enabled = string.IsNullOrEmpty(selectedMaPP);

                // Mã phiếu mượn
                selectedMaPM2 = dgvPhieuPhat.Rows[i].Cells["MaPhieuMuon"]?.Value?.ToString() ?? "";
                txtMaPhieuMuon.Text = selectedMaPM2;
                txtMaPhieuMuon.Enabled = string.IsNullOrEmpty(selectedMaPM2);

                // Mã độc giả
                txtMaDocGia.Text = dgvPhieuPhat.Rows[i].Cells["MaDocGia2"]?.Value?.ToString() ?? "";

                // Ngày nộp phạt
                var ngayNopPhat = dgvPhieuPhat.Rows[i].Cells["NgayNopPhat"]?.Value;
                if (ngayNopPhat != null && ngayNopPhat != DBNull.Value)
                {
                    dtNgayNopPhat.Value = Convert.ToDateTime(ngayNopPhat);
                }
                else
                {
                    dtNgayNopPhat.Value = DateTime.Now;
                }

                // Tổng tiền phạt
                var tongTienPhat = dgvPhieuPhat.Rows[i].Cells["TongTienPhat"]?.Value;
                if (tongTienPhat != null && tongTienPhat != DBNull.Value)
                {
                    txtTongTienPhat.Text = tongTienPhat.ToString();
                }
                else
                {
                    txtTongTienPhat.Text = "0";
                }
                txtTongTienPhat.Enabled = !string.IsNullOrEmpty(txtTongTienPhat.Text);

                // Mã thủ thư
                var maThuThu = dgvPhieuPhat.Rows[i].Cells["ThuThu"]?.Value;
                if (maThuThu != null && maThuThu != DBNull.Value)
                {
                    cboThuThu.SelectedValue = maThuThu.ToString();
                }
                else
                {
                    cboThuThu.SelectedIndex = -1;
                }
                cboThuThu.Enabled = true;

                // Trạng thái
                string trangThaiText = dgvPhieuPhat.Rows[i].Cells["TrangThaiText"]?.Value?.ToString();
                if (trangThaiText == "Đã nộp")
                    cboTrangThai2.SelectedValue = 1;
                else
                    cboTrangThai2.SelectedValue = 0;
            }
        }

        private void LoadSachTra(string maPhieuMuon)
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "select MaSach, TinhTrangMuon, DaTraSach, TinhTrangTra, " +
                    "case when pm.HanTra < pm.NgayThucTra then DATEDIFF(day, pm.HanTra, pm.NgayThucTra) " +
                    "else 0 end as SoNgayTre " +
                    "from CT_PhieuMuon ct_pm join PhieuMuon pm on ct_pm.MaPhieuMuon = pm.MaPhieuMuon " +
                    $"where ct_pm.MaPhieuMuon = '{maPhieuMuon}'";
                adapter = new SqlDataAdapter(sql, con);
                dt = new DataTable();
                adapter.Fill(dt);
                dgvSachTra.DataSource = dt;
            }
        }

        private void LoadSachPhat(string maPhieuPhat)
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "select MaSach, vp.TenViPham, NopPhat " +
                    "from CT_PhieuPhat ct_pp join ViPham vp on ct_pp.MaViPham = vp.MaViPham " +
                    $"where MaPhieuPhat = '{maPhieuPhat}'";
                adapter = new SqlDataAdapter(sql, con);
                dt = new DataTable();
                adapter.Fill(dt);
                dgvSachPhat.DataSource = dt;

                // Tính tổng tiền phạt
                float tongTienPhat = 0;
                foreach (DataRow row in dt.Rows)
                {
                    if (float.TryParse(row["NopPhat"].ToString(), out float tienPhat))
                    {
                        tongTienPhat += tienPhat;
                    }
                }
                txtTongTienPhat.Text = tongTienPhat.ToString();
            }
        }

        private void LoadPMDaTra()
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = @"
                    WITH PhieuCoKhaNangPhat AS (
                        SELECT 
                            pm.MaPhieuMuon,
                            pm.MaDocGia,
                            pm.NgayMuon,
                            pm.HanTra,
                            pm.NgayThucTra,
                            COALESCE(SUM(ct_pm.TienCoc), 0) AS TongTienCoc,
            
                            -- Quá hạn
                            CASE WHEN CAST(pm.NgayThucTra AS DATE) > CAST(pm.HanTra AS DATE) THEN 1 ELSE 0 END AS QuaHan,
            
                            -- Tình trạng thay đổi
                            CASE WHEN EXISTS (
                                SELECT 1 FROM CT_PhieuMuon ct 
                                WHERE ct. MaPhieuMuon = pm.MaPhieuMuon AND ct.TinhTrangMuon != ct.TinhTrangTra
                            ) THEN 1 ELSE 0 END AS TinhTrangThayDoi,
            
                            -- Mất sách
                            CASE WHEN EXISTS (
                                SELECT 1 FROM CT_PhieuMuon ct 
                                WHERE ct.MaPhieuMuon = pm.MaPhieuMuon AND ct.DaTraSach = 0
                            ) THEN 1 ELSE 0 END AS MatSach
            
                        FROM PhieuMuon pm 
                        LEFT JOIN CT_PhieuMuon ct_pm ON pm.MaPhieuMuon = ct_pm.MaPhieuMuon
                        WHERE pm.NgayThucTra IS NOT NULL
                        GROUP BY pm.MaPhieuMuon, pm.MaDocGia, pm.NgayMuon, pm.HanTra, pm.NgayThucTra
                    )
                    SELECT *
                    FROM PhieuCoKhaNangPhat
                    WHERE (QuaHan + TinhTrangThayDoi + MatSach) > 0";

                adapter = new SqlDataAdapter(sql, con);
                dt = new DataTable();
                adapter.Fill(dt);
                dvPM = new DataView(dt);
                dgvPhieuMuon.DataSource = dvPM;
            }
        }

        private void LoadPhieuPhat_PhieuMuon(string maPhieuMuon)
        {
            foreach (DataGridViewRow row in dgvPhieuMuon.Rows)
            {
                // Kiểm tra nếu giá trị trong cột MaDocGia khớp với mã cần tìm
                if (row.Cells[0].Value != null && row.Cells[0].Value.ToString() == maPhieuMuon)
                {
                    dgvPhieuMuon.ClearSelection();
                    row.Cells[0].Selected = true;
                    //dgvPhieuMuon.CurrentCell = dgvPhieuMuon.Rows[row.Index].Cells[0];
                    dgvPhieuMuon.FirstDisplayedScrollingRowIndex = row.Index;
                    return;
                }
            }
        }

        private void dgvPhieuPhat_SelectionChanged(object sender, EventArgs e)
        {
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
                    LoadComboBox(cboThuThu, "ThuThu", "MaThuThu", "TenThuThu");
                    NapCT();
                    LoadSachPhat(selectedMaPP);
                    LoadSachTra(txtMaPhieuMuon.Text);
                    SetStyle();
                }
                else
                {
                    return;
                }
            }
            else
            {
                LoadComboBox(cboThuThu, "ThuThu", "MaThuThu", "TenThuThu");
                NapCT();
                LoadSachPhat(selectedMaPP);
                LoadPhieuPhat_PhieuMuon(selectedMaPM2);
                LoadSachTra(selectedMaPM2);
            }
        }
        private void dgvPhieuMuon_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvPhieuMuon.CurrentCell != null && dgvPhieuMuon.CurrentCell.RowIndex >= 0)
            {
                int i = dgvPhieuMuon.CurrentRow.Index;
                string maPhieuMuon = dgvPhieuMuon.Rows[i].Cells[0].Value.ToString();
                LoadSachTra(maPhieuMuon);
            }
        }

        private void btnTaoMoi_Click(object sender, EventArgs e)
        {
            //gen mã phiếu mượn
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "Select max(MaPhieuPhat) from PhieuPhat";
                cmd = new SqlCommand(sql, con);
                object rs = cmd.ExecuteScalar();
                if (rs != DBNull.Value && rs != null)
                {
                    string maPhieuPhat = rs.ToString();
                    int number = int.Parse(maPhieuPhat.Substring(2)); //Lấy sau phầm "PP"
                    ++number;
                    txtMaPhieuPhat.Text = "PP" + number.ToString("D4");
                }
            }
            txtMaPhieuMuon.Text = "";
            txtMaPhieuMuon.Enabled = true;
            txtMaPhieuMuon.Focus();
            txtMaDocGia.Text = "";

            dtNgayNopPhat.Value = DateTime.Now;
            if (userRole == "thuthu")
            {
                cboThuThu.SelectedValue = maThuThu; // Gán mã thủ thư vào combo box
                cboThuThu.Enabled = false; // Không cho phép sửa
            }
            else
            {
                cboThuThu.SelectedIndex = -1;
            }
            cboTrangThai2.SelectedValue = 0; // Mặc định: Chưa nộp
            txtTongTienPhat.Text = "";
            txtTongTienPhat.Enabled = false;

            addNewFlag = true;
            SetStyle();
            LoadComboBox(cboThuThu, "ThuThu", "MaThuThu", "TenThuThu");
        }

        private void dgvPhieuMuon_DoubleClick(object sender, EventArgs e)
        {
            if (addNewFlag)
            {
                if (dgvPhieuMuon.SelectedRows.Count > 0)
                {
                    string maPhieuMuon = dgvPhieuMuon.SelectedRows[0].Cells[0].Value.ToString();
                    string maDocGia = dgvPhieuMuon.SelectedRows[0].Cells[1].Value.ToString();

                    // === KIỂM TRA PHIẾU MƯỢN ĐÃ CÓ PHIẾU PHẠT CHƯA ===
                    using (con = new SqlConnection(strCon))
                    {
                        con.Open();
                        string checkSql = "SELECT COUNT(*) FROM PhieuPhat WHERE MaPhieuMuon = @MaPhieuMuon";
                        SqlCommand checkCmd = new SqlCommand(checkSql, con);
                        checkCmd.Parameters.AddWithValue("@MaPhieuMuon", maPhieuMuon);

                        int count = (int)checkCmd.ExecuteScalar();

                        if (count > 0)
                        {
                            MessageBox.Show($"Phiếu mượn '{maPhieuMuon}' đã có phiếu phạt rồi!",
                                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    // === CHƯA CÓ → CHỌN ĐƯỢC ===
                    txtMaPhieuMuon.Text = maPhieuMuon;
                    txtMaDocGia.Text = maDocGia;


                    txtMaPhieuMuon.Text = maPhieuMuon;
                    txtMaDocGia.Text= maDocGia;
                }
                else
                {
                    MessageBox.Show("Chọn cả dòng để thực hiện chức năng này");
                }
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = @"
                    INSERT INTO PhieuPhat (MaPhieuPhat, MaPhieuMuon, NgayNopPhat, TrangThai, MaThuThu) 
                    VALUES (@MaPhieuPhat, @MaPhieuMuon, @NgayNopPhat, @TrangThai, @MaThuThu)";

                cmd = new SqlCommand(sql, con);

                cmd.Parameters.AddWithValue("@MaPhieuPhat", txtMaPhieuPhat.Text);
                cmd.Parameters.AddWithValue("@MaPhieuMuon", txtMaPhieuMuon.Text);
                cmd.Parameters.AddWithValue("@NgayNopPhat", dtNgayNopPhat.Value.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@TrangThai", cboTrangThai2.SelectedIndex);  // ← THÊM
                cmd.Parameters.AddWithValue("@MaThuThu", cboThuThu.SelectedValue);

                int kq = cmd.ExecuteNonQuery();

                if (kq > 0)
                {
                    MessageBox.Show("Thêm phiếu phạt thành công, click Phạt sách để lưu thông tin sách bị phạt",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    addNewFlag = false;
                    LoadPhieuPhat();
                    FilterData();

                    //int lastRowIndex = dgvPhieuPhat.RowCount - 1;
                    //dgvPhieuPhat.ClearSelection();
                    //dgvPhieuPhat.CurrentCell = dgvPhieuPhat.Rows[lastRowIndex].Cells[0];
                    //dgvPhieuPhat.FirstDisplayedScrollingRowIndex = lastRowIndex;

                    LoadComboBox(cboThuThu, "ThuThu", "MaThuThu", "TenThuThu");

                    NapCT();
                    LoadSachPhat(txtMaPhieuPhat.Text);
                    LoadPhieuPhat_PhieuMuon(txtMaPhieuMuon.Text);
                    LoadSachTra(txtMaPhieuMuon.Text);
                    SetStyle();
                }
                else
                {
                    MessageBox.Show("Không thể thêm phiếu phạt!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedMaPP))
            {
                MessageBox.Show("Vui lòng chọn một phiếu phạt để xóa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int currentIndex = dgvPhieuPhat.CurrentRow.Index;

            DialogResult rs = MessageBox.Show($"Bạn có chắc chắn muốn xóa phiếu phạt này không?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (rs == DialogResult.Yes)
            {
                using (con = new SqlConnection(strCon))
                {
                    con.Open();
                    string sql = "DELETE FROM PhieuPhat WHERE MaPhieuPhat = @MaPhieuPhat";
                    cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@MaPhieuPhat", selectedMaPP);

                    try
                    {
                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            MessageBox.Show("Xóa phiếu phạt thành công!", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            LoadPhieuPhat();
                            FilterData();

                            // === FIX:  KIỂM TRA TRƯỚC KHI CHỌN ===
                            if (dgvPhieuPhat.Rows.Count > 0)
                            {
                                int newIndex;

                                // Nếu index cũ >= số dòng hiện tại → chọn dòng cuối
                                if (currentIndex >= dgvPhieuPhat.Rows.Count)
                                {
                                    newIndex = dgvPhieuPhat.Rows.Count - 1;
                                }
                                else
                                {
                                    // Giữ nguyên vị trí (dòng mới sẽ lên thay chỗ)
                                    newIndex = currentIndex;
                                }

                                dgvPhieuPhat.ClearSelection();
                                dgvPhieuPhat.CurrentCell = dgvPhieuPhat.Rows[newIndex].Cells[0];
                                dgvPhieuPhat.FirstDisplayedScrollingRowIndex = newIndex;

                                NapCT();
                                LoadSachPhat(txtMaPhieuPhat.Text);
                                LoadPhieuPhat_PhieuMuon(txtMaPhieuMuon.Text);
                                LoadSachTra(txtMaPhieuMuon.Text);
                            }
                            else
                            {
                                // Không còn dòng nào → clear form
                                selectedMaPP = "";
                                txtMaPhieuPhat.Clear();
                                txtMaPhieuMuon.Clear();
                                txtMaDocGia.Clear();
                                txtTongTienPhat.Clear();
                                // Clear các field khác nếu cần
                            }
                        }
                        else
                        {
                            MessageBox.Show("Xóa phiếu phạt không thành công!", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Phiếu phạt liên quan tới nhiều bảng dữ liệu khác",
                            "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            // === VALIDATION ===
            if (string.IsNullOrEmpty(selectedMaPP))
            {
                MessageBox.Show("Chưa chọn bản ghi để sửa", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string currentMaPP = selectedMaPP;

            // === UPDATE DATABASE ===
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = @"
                    UPDATE PhieuPhat 
                    SET NgayNopPhat = @NgayNopPhat, 
                        TrangThai = @TrangThai, 
                        MaThuThu = @MaThuThu 
                    WHERE MaPhieuPhat = @MaPhieuPhat";

                cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@NgayNopPhat", dtNgayNopPhat.Value.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@TrangThai", cboTrangThai2.SelectedIndex);
                cmd.Parameters.AddWithValue("@MaThuThu", cboThuThu.SelectedValue);
                cmd.Parameters.AddWithValue("@MaPhieuPhat", selectedMaPP);

                int kq = cmd.ExecuteNonQuery();

                if (kq > 0)
                {
                    MessageBox.Show("Sửa phiếu phạt thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Không thể sửa phiếu phạt!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // === LOAD LẠI ===
            LoadPhieuPhat();
            FilterData();  // ← Hàm này ĐÃ GỌI UpdateTongSo()

            // === TÌM VÀ CHỌN LẠI DÒNG ===
            int newIndex = -1;
            foreach (DataGridViewRow row in dgvPhieuPhat.Rows)
            {
                if (row.Cells["MaPhieuPhat"].Value?.ToString() == currentMaPP)
                {
                    newIndex = row.Index;
                    break;
                }
            }

            if (dgvPhieuPhat.Rows.Count > 0)
            {
                dgvPhieuPhat.ClearSelection();
                dgvPhieuPhat.CurrentCell = dgvPhieuPhat.Rows[newIndex >= 0 ? newIndex : 0].Cells[0];
            }

            // === NẠP CHI TIẾT ===
            NapCT();
            LoadSachPhat(selectedMaPP);
            LoadPhieuPhat_PhieuMuon(txtMaPhieuMuon.Text);
            LoadSachTra(txtMaPhieuMuon.Text);
            SetStyle();
        }

        private void txtSearch1_TextChanged(object sender, EventArgs e)
        {
            if (cboTruong1.SelectedIndex == 0)
            {
                dvPM.RowFilter = $"MaPhieuMuon like '%{txtSearch1.Text}%'";
            }
            else
            {
                dvPM.RowFilter = $"MaDocGia like '%{txtSearch1.Text}%'";
            }
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
            if (dvPP == null)
                return;

            List<string> filters = new List<string>();

            // === LỌC THEO TRẠNG THÁI ===
            if (cboTrangThai.SelectedIndex > 0)  // Không phải "Tất cả"
            {
                string trangThai = cboTrangThai.SelectedItem.ToString();
                filters.Add($"TrangThaiText = '{trangThai}'");
            }

            // === LỌC THEO TÌM KIẾM ===
            string searchText = txtSearch2.Text.Trim();
            if (!string.IsNullOrEmpty(searchText))
            {
                string column = "";

                switch (cboTruong2.SelectedIndex)
                {
                    case 0:  // Mã phiếu phạt
                        column = "MaPhieuPhat";
                        break;
                    case 1:  // Mã phiếu mượn
                        column = "MaPhieuMuon";
                        break;
                    case 2:  // Mã độc giả
                        column = "MaDocGia";
                        break;
                }

                if (!string.IsNullOrEmpty(column))
                {
                    filters.Add($"{column} LIKE '%{searchText}%'");
                }
            }

            // ✅ LỌC THEO NGÀY NỘP PHẠT (NẾU CÓ)
            if (filterTuNgay.HasValue && filterDenNgay.HasValue)
            {
                filters.Add($"NgayNopPhat >= #{filterTuNgay.Value:MM/dd/yyyy}#");
                filters.Add($"NgayNopPhat <= #{filterDenNgay.Value:MM/dd/yyyy}#");
            }

            // === KẾT HỢP CÁC BỘ LỌC ===
            dvPP.RowFilter = string.Join(" AND ", filters);
            UpdateTongSo();
        }


        private void btnPhatSach_Click(object sender, EventArgs e)
        {
            var f = new frmPhieuTra();
            f.MaPhieuPhat = txtMaPhieuPhat.Text;
            f.MaPhieuMuon = txtMaPhieuMuon.Text;
            f.MaDocGia = txtMaDocGia.Text;

            f.ShowDialog();

            // ✅ SAU KHI ĐÓNG FORM → LOAD LẠI

            // Lưu vị trí
            int currentIndex = dgvPhieuPhat.CurrentRow?.Index ?? -1;
            string currentMaPP = selectedMaPP ?? "";
            string currentMaPM = txtMaPhieuMuon.Text;

            // Load lại dữ liệu
            LoadPhieuPhat();

            // ✅ GỌI LẠI FILTER (giữ filter hiện tại)
            FilterData();

            UpdateTongSo();

            // Chọn lại dòng
            if (dgvPhieuPhat.Rows.Count > 0)
            {
                if (currentIndex >= 0 && currentIndex < dgvPhieuPhat.Rows.Count)
                {
                    dgvPhieuPhat.ClearSelection();
                    dgvPhieuPhat.CurrentCell = dgvPhieuPhat.Rows[currentIndex].Cells[0];
                    dgvPhieuPhat.FirstDisplayedScrollingRowIndex = currentIndex;
                }
                else
                {
                    dgvPhieuPhat.CurrentCell = dgvPhieuPhat.Rows[0].Cells[0];
                }

                NapCT();
                LoadPhieuPhat_PhieuMuon(txtMaPhieuMuon.Text);
                LoadSachPhat(selectedMaPP);
                LoadSachTra(txtMaPhieuMuon.Text);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {

        }

        private void btnInPhieuPhat_Click(object sender, EventArgs e)
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "select ctpp.MaSach, ds.TenDauSach as TenSach, vp.TenViPham, ctpp.NopPhat as TienNopPhat " +
                    "from CT_PhieuPhat ctpp join ViPham vp on ctpp.MaViPham = vp.MaViPham " +
                    "join CuonSach cs on ctpp.MaSach = cs.MaSach join DauSach ds on cs.MaDauSach = ds.MaDauSach " +
                    $"where ctpp.MaPhieuPhat = '{txtMaPhieuPhat.Text}'";
                adapter = new SqlDataAdapter(sql, con);
                dt = new DataTable();
                adapter.Fill(dt);
                string mapp = txtMaPhieuPhat.Text;
                string mapm = txtMaPhieuMuon.Text;
                string madg = txtMaDocGia.Text;
                string sqlHoTen = $"Select HoTen from DocGia where MaDocGia = '{madg}'";
                cmd = new SqlCommand(sqlHoTen, con);
                string hoten = cmd.ExecuteScalar().ToString();
                string ngaynophat = dtNgayNopPhat.Value.ToString("dd/MM/yyyy");
                string thuthu = cboThuThu.Text.Substring(7);
                using (frmInPhieuPhat reportForm = new frmInPhieuPhat(dt, mapp, mapm, ngaynophat, madg, hoten, thuthu))
                {
                    reportForm.ShowDialog();
                }
            }
        }

        private void dgvPhieuMuon_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || dvPP == null) return;

            string maPhieuMuon = dgvPhieuMuon.Rows[e.RowIndex].Cells["MaPhieuMuon1"].Value?.ToString();
            if (string.IsNullOrEmpty(maPhieuMuon)) return;

            // Check xem phiếu mượn này có trong dvPP (danh sách phiếu phạt) không
            bool daTonTai = false;
            foreach (DataRowView row in dvPP)
            {
                if (row["MaPhieuMuon"].ToString() == maPhieuMuon)
                {
                    daTonTai = true;
                    break;
                }
            }

            // Nếu đã có phiếu phạt → màu đỏ, in nghiêng
            if (daTonTai)
            {
                dgvPhieuMuon.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Red;
                dgvPhieuMuon.Rows[e.RowIndex].DefaultCellStyle.Font =
                    new Font(dgvPhieuMuon.Font, FontStyle.Regular);
            }
            else
            {
                dgvPhieuMuon.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                dgvPhieuMuon.Rows[e.RowIndex].DefaultCellStyle.Font =
                    new Font(dgvPhieuMuon.Font, FontStyle.Regular);
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

                    FilterData();
                }
            }
        }

        private void LoadPhieuPhat()
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = @"
                    SELECT 
                        pp. MaPhieuPhat, 
                        pp.MaPhieuMuon, 
                        pm.MaDocGia,
                        dg.HoTen AS TenDocGia,
                        pp.NgayNopPhat, 
                        COALESCE(SUM(ct_pp.NopPhat), 0) AS TongTienPhat, 
                        pp.MaThuThu AS ThuThu,
                        tt.TenThuThu,
                        CASE 
                            WHEN pp.TrangThai = 1 THEN N'Đã nộp'
                            ELSE N'Chưa nộp'
                        END AS TrangThaiText
                    FROM PhieuPhat pp 
                    LEFT JOIN CT_PhieuPhat ct_pp ON pp.MaPhieuPhat = ct_pp.MaPhieuPhat 
                    JOIN PhieuMuon pm ON pp.MaPhieuMuon = pm.MaPhieuMuon
                    LEFT JOIN DocGia dg ON pm.MaDocGia = dg. MaDocGia
                    LEFT JOIN ThuThu tt ON pp.MaThuThu = tt.MaThuThu
                    GROUP BY 
                        pp.MaPhieuPhat, 
                        pp.MaPhieuMuon, 
                        pm.MaDocGia,
                        dg.HoTen,
                        pp.NgayNopPhat, 
                        pp.MaThuThu,
                        tt.TenThuThu,
                        pp.TrangThai
                    ORDER BY pp.MaPhieuPhat DESC";

                adapter = new SqlDataAdapter(sql, con);
                dt = new DataTable();
                adapter.Fill(dt);
                dvPP = new DataView(dt);
                dgvPhieuPhat.DataSource = dvPP;
            }
        }
    }
}
