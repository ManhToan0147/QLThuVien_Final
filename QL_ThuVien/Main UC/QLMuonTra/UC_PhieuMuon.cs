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
            cboTruong1.SelectedIndex = 0;
            cboTruong2.SelectedIndex = 0;

            LoadComboBox(cboKieuMuon,"KieuMuon", "MaKieuMuon", "TenKieuMuon");
            LoadComboBox(cboThuThu, "ThuThu", "MaThuThu", "TenThuThu");
            //Fix lỗi column header
            dgvDocGia.ColumnHeadersDefaultCellStyle.Font = new Font(dgvDocGia.Font, FontStyle.Bold);
            dgvPhieuMuon.ColumnHeadersDefaultCellStyle.Font = new Font(dgvPhieuMuon.Font, FontStyle.Bold);
            dgvDocGia.Columns["DaTraSach"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvSachMuon.DefaultCellStyle.Font = new Font(dgvSachMuon.Font, FontStyle.Regular);

            LoadDocGia();
            LoadPhieuMuon();
        }

        private void LoadComboBox(ComboBox cbo, string tableName, string Ma, string TenMa)
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = $"SELECT * FROM {tableName}";
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
                string sql = "select * from TongQuanPhieuMuon";
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
                //string sql = "SELECT DocGia.MaDocGia, DocGia.HoTen, " +
                //    "CASE WHEN EXISTS " +
                //    "(SELECT 1 FROM PhieuMuon WHERE PhieuMuon.MaDocGia = DocGia.MaDocGia AND PhieuMuon.NgayThucTra IS NULL) " +
                //    "THEN 'False' ELSE 'True' END AS DaTraSach " +
                //    "FROM DocGia;";
                string sql = "Select * from DocGia_DaTraSach";
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

        private void txtSearch2_TextChanged(object sender, EventArgs e)
        {
            if (cboTruong2.SelectedIndex == 0)
            {
                dvPM.RowFilter = $"MaPhieuMuon like '%{txtSearch2.Text}%'";
            }
            else if (cboTruong2.SelectedIndex == 1)
            {
                dvPM.RowFilter = $"MaDocGia like '%{txtSearch2.Text}%'";
            }
            else
            {
                dvPM.RowFilter = $"MaThuThu like '%{txtSearch2.Text}%'";
            }
        }

        private string selectedMaPM, selectedMaDG;
        private void NapCT()
        {
            if (dgvPhieuMuon.CurrentCell != null && dgvPhieuMuon.CurrentCell.RowIndex >= 0)
            {
                int i = dgvPhieuMuon.CurrentRow.Index;
                selectedMaPM = dgvPhieuMuon.Rows[i].Cells[0].Value.ToString();
                txtMaPhieuMuon.Text = selectedMaPM;
                txtMaPhieuMuon.Enabled = string.IsNullOrEmpty(selectedMaPM);
                
                selectedMaDG = dgvPhieuMuon.Rows[i].Cells[1].Value.ToString();
                txtMaDG.Text = selectedMaDG;
                cboKieuMuon.SelectedValue = dgvPhieuMuon.Rows[i].Cells[2].Value.ToString();
                dtNgayMuon.Text = dgvPhieuMuon.Rows[i].Cells[3].Value.ToString();
                dtHanTra.Text = dgvPhieuMuon.Rows[i].Cells[4].Value.ToString();

                txtTienCoc.Text = dgvPhieuMuon.Rows[i].Cells[5].Value.ToString();
                cboThuThu.SelectedValue = dgvPhieuMuon.Rows[i].Cells[6].Value.ToString();
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
            
            txtMaDG.Focus();
            addNewFlag = true;
            dtHanTra.Enabled = false;
            txtSoLuongMuon.Enabled = false;
            txtTienCoc.Enabled = false;
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedMaPM))
            {
                MessageBox.Show("Vui lòng chọn một phiếu mượn để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int currentIndex = dgvPhieuMuon.CurrentRow.Index;

            DialogResult result = MessageBox.Show($"Bạn có chắc chắn muốn xóa phiếu mượn với mã {selectedMaPM} không?",
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
                                MessageBox.Show("Xóa phiếu mượn thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LoadPhieuMuon();
                                LoadDocGia();
                                int beforeRowIndex = currentIndex - 1;
                                dgvPhieuMuon.ClearSelection();
                                dgvPhieuMuon.CurrentCell = dgvPhieuMuon.Rows[beforeRowIndex].Cells[0];
                                NapCT();

                                LoadSachMuon(txtMaPhieuMuon.Text);
                                LoadPhieuMuon_DocGia(txtMaDG.Text);
                                dgvPhieuMuon.FirstDisplayedScrollingRowIndex = beforeRowIndex;
                            }
                            else
                            {
                                MessageBox.Show("Xóa phiếu mượn không thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Phiếu mượn liên quan tới nhiều bảng dữ liệu khác", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "Insert into PhieuMuon(MaPhieuMuon, MaDocGia, MaKieuMuon, NgayMuon, MaThuThu) values " +
                    "(@MaPhieuMuon, @MaDocGia, @MaKieuMuon, @NgayMuon, @MaThuThu)";
                cmd = new SqlCommand(sql, con);

                // Thêm tham số vào câu lệnh SQL
                cmd.Parameters.AddWithValue("@MaPhieuMuon", txtMaPhieuMuon.Text);
                cmd.Parameters.AddWithValue("@MaDocGia", txtMaDG.Text);
                cmd.Parameters.AddWithValue("@MaKieuMuon", cboKieuMuon.SelectedValue); // Lấy ValueMember của ComboBox
                cmd.Parameters.AddWithValue("@NgayMuon", dtNgayMuon.Value.ToString("yyyy-MM-dd")); // Định dạng ngày tháng cho SQL
                cmd.Parameters.AddWithValue("@MaThuThu", cboThuThu.SelectedValue); // Lấy ValueMember của ComboBox

                int kq = cmd.ExecuteNonQuery();
                if (kq > 0)
                {
                    MessageBox.Show("Thêm phiếu mượn thành công, click Mượn sách để lưu thông tin sách mượn!", "Thông báo", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Không thể thêm phiếu mượn!", "Lỗi");
                }
                LoadPhieuMuon();
                LoadDocGia();
                int lastRowIndex = dgvPhieuMuon.RowCount - 1;
                dgvPhieuMuon.ClearSelection();
                dgvPhieuMuon.CurrentCell = dgvPhieuMuon.Rows[lastRowIndex].Cells[0];
                NapCT();
                LoadSachMuon(txtMaPhieuMuon.Text);
                LoadPhieuMuon_DocGia(txtMaDG.Text);
                dgvPhieuMuon.FirstDisplayedScrollingRowIndex = lastRowIndex;

                addNewFlag = false;
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedMaPM))
            {
                MessageBox.Show("Chưa chọn bản ghi để sửa", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int currentIndex = dgvPhieuMuon.CurrentRow.Index;

            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "UPDATE PhieuMuon " +
                             "SET MaDocGia = @MaDocGia, MaKieuMuon = @MaKieuMuon, NgayMuon = @NgayMuon, MaThuThu = @MaThuThu " +
                             "WHERE MaPhieuMuon = @MaPhieuMuon";
                cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@MaPhieuMuon", selectedMaPM); 
                cmd.Parameters.AddWithValue("@MaDocGia", txtMaDG.Text);
                cmd.Parameters.AddWithValue("@MaKieuMuon", cboKieuMuon.SelectedValue); 
                cmd.Parameters.AddWithValue("@NgayMuon", dtNgayMuon.Value.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@MaThuThu", cboThuThu.SelectedValue);
                int rowsAffected = cmd.ExecuteNonQuery();

                // Kiểm tra kết quả
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Sửa phiếu mượn thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadPhieuMuon();
                    LoadDocGia();
                    dgvPhieuMuon.ClearSelection();
                    dgvPhieuMuon.CurrentCell = dgvPhieuMuon.Rows[currentIndex].Cells[0];
                    NapCT();
                    LoadSachMuon(selectedMaPM);
                    LoadPhieuMuon_DocGia(selectedMaDG);
                    dgvPhieuMuon.FirstDisplayedScrollingRowIndex = currentIndex;
                }
                else
                {
                    MessageBox.Show("Sửa thất bại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dgvDocGia_DoubleClick(object sender, EventArgs e)
        {
            if (addNewFlag)
            {
                if (dgvDocGia.SelectedRows.Count > 0)
                {
                    string maDocGia = dgvDocGia.SelectedRows[0].Cells[0].Value.ToString();
                    bool dkMuon = bool.Parse(dgvDocGia.SelectedRows[0].Cells[2].Value.ToString());
                    if (!dkMuon)
                    {
                        MessageBox.Show("Độc giả chưa trả sách, không tạo phiếu mượn", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        txtMaDG.Text = maDocGia;
                    }
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
            if (dgvPhieuMuon.CurrentRow == null) return;

            int currentIndex = dgvPhieuMuon.CurrentRow.Index;
            string currentMaPM = selectedMaPM;
            string currentMaDG = selectedMaDG;

            //Load lại danh sách phiếu mượn (cập nhật TienCoc)
            LoadPhieuMuon();
            //Load lại danh sách độc giả (cập nhật DaTraSach)
            LoadDocGia();

            // Chọn lại dòng trong dgvPhieuMuon
            if (currentIndex >= 0 && currentIndex < dgvPhieuMuon.Rows.Count)
            {
                dgvPhieuMuon.ClearSelection();
                dgvPhieuMuon.CurrentCell = dgvPhieuMuon.Rows[currentIndex].Cells[0];
                dgvPhieuMuon.FirstDisplayedScrollingRowIndex = currentIndex;
            }
            // Load lại thông tin chi tiết
            NapCT();

            // Load lại danh sách sách mượn
            LoadSachMuon(currentMaPM);

            //Highlight lại độc giả trong dgvDocGia
            LoadPhieuMuon_DocGia(currentMaDG);


        }

        private void btnMuonSach_Click(object sender, EventArgs e)
        {
            var f = new frmNhapSach();
            f.MaPhieuMuon = txtMaPhieuMuon.Text;
            f.MaDocGia = txtMaDG.Text;
            f.KieuMuon = cboKieuMuon.Text.Substring(5);
            f.ShowDialog();
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

        private void dgvPhieuMuon_SelectionChanged(object sender, EventArgs e)
        {
            NapCT();
            txtSoLuongMuon.Enabled = true;
            txtTienCoc.Enabled = true;
            dtHanTra.Enabled = true;

            LoadSachMuon(selectedMaPM);
            LoadPhieuMuon_DocGia(selectedMaDG);
        }
    }
}
