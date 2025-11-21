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
            LoadComboBox(cboThuThu, "ThuThu", "MaThuThu", "TenThuThu");
            cboTruong1.SelectedIndex = 0;
            cboTruong2.SelectedIndex = 0;

            //Fix lỗi dgv
            dgvSachTra.Columns["DaTraSach"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvSachTra.DefaultCellStyle.Font = new Font(dgvSachTra.Font, FontStyle.Regular);
            dgvSachPhat.DefaultCellStyle.Font = new Font(dgvSachPhat.Font, FontStyle.Regular);
            dgvPhieuMuon.ColumnHeadersDefaultCellStyle.Font = new Font(dgvPhieuMuon.Font, FontStyle.Bold);
            dgvPhieuPhat.ColumnHeadersDefaultCellStyle.Font = new Font(dgvSachPhat.Font, FontStyle.Bold);

            LoadPhieuPhat();
            LoadPMDaTra();
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
        string selectedMaPP, selectedMaPM2;
        private void NapCT()
        {
            if (dgvPhieuPhat.CurrentCell != null && dgvPhieuPhat.CurrentCell.RowIndex >= 0)
            {
                int i = dgvPhieuPhat.CurrentRow.Index;

                selectedMaPP = dgvPhieuPhat.Rows[i].Cells[0].Value.ToString();
                txtMaPhieuPhat.Text = selectedMaPP;
                txtMaPhieuPhat.Enabled = string.IsNullOrEmpty(selectedMaPP);

                selectedMaPM2 = dgvPhieuPhat.Rows[i].Cells[1].Value.ToString();
                txtMaPhieuMuon.Text = selectedMaPM2;
                txtMaPhieuMuon.Enabled = string.IsNullOrEmpty(selectedMaPM2);

                txtMaDocGia.Text = dgvPhieuPhat.Rows[i].Cells[2].Value.ToString();

                dtNgayNopPhat.Text = dgvPhieuPhat.Rows[i].Cells[3].Value.ToString();
                txtTongTienPhat.Text = dgvPhieuPhat.Rows[i].Cells[4].Value.ToString();
                txtTongTienPhat.Enabled = !string.IsNullOrEmpty(txtTongTienPhat.Text);
                cboThuThu.SelectedValue = dgvPhieuPhat.Rows[i].Cells[5].Value.ToString();
                cboThuThu.Enabled = true;
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
                string sql = "select pm.MaPhieuMuon, pm.MaDocGia, sum(ct_pm.TienCoc) as TongTienCoc, pm.NgayThucTra " +
                    "from PhieuMuon pm join CT_PhieuMuon ct_pm on pm.MaPhieuMuon = ct_pm.MaPhieuMuon " +
                    "where pm.NgayThucTra is not null " +
                    "group by pm.MaPhieuMuon, pm.MaDocGia, pm.NgayThucTra";
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
            NapCT();
            LoadSachPhat(selectedMaPP);
            LoadPhieuPhat_PhieuMuon(selectedMaPM2);
            LoadSachTra(selectedMaPM2);
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
            txtTongTienPhat.Text = "";
            txtTongTienPhat.Enabled = false;

            addNewFlag = true;
        }

        private void dgvPhieuMuon_DoubleClick(object sender, EventArgs e)
        {
            if (addNewFlag)
            {
                if (dgvPhieuMuon.SelectedRows.Count > 0)
                {
                    string maPhieuMuon = dgvPhieuMuon.SelectedRows[0].Cells[0].Value.ToString();
                    string maDocGia = dgvPhieuMuon.SelectedRows[0].Cells[1].Value.ToString();
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
                string sql = "Insert into PhieuPhat values " +
                    "(@MaPhieuPhat, @MaPhieuMuon, @NgayNopPhat, @MaThuThu)";
                cmd = new SqlCommand(sql, con);

                // Thêm tham số vào câu lệnh SQL
                cmd.Parameters.AddWithValue("@MaPhieuPhat", txtMaPhieuPhat.Text);
                cmd.Parameters.AddWithValue("@MaPhieuMuon", txtMaPhieuMuon.Text);
                cmd.Parameters.AddWithValue("@NgayNopPhat", dtNgayNopPhat.Value.ToString("yyyy-MM-dd")); 
                cmd.Parameters.AddWithValue("@MaThuThu", cboThuThu.SelectedValue);

                int kq = cmd.ExecuteNonQuery();
                if (kq > 0)
                {
                    MessageBox.Show("Thêm phiếu phạt thành công, click Phạt sách để lưu thông tin sách bị phạt", "Thông báo",
                         MessageBoxButtons.OK, MessageBoxIcon.Information);
                    addNewFlag = false;
                    LoadPhieuPhat();
                    int lastRowIndex = dgvPhieuPhat.RowCount - 1;
                    dgvPhieuPhat.ClearSelection();
                    dgvPhieuPhat.CurrentCell = dgvPhieuPhat.Rows[lastRowIndex].Cells[0];
                    dgvPhieuPhat.FirstDisplayedScrollingRowIndex = lastRowIndex;
                    NapCT();
                    LoadSachPhat(txtMaPhieuPhat.Text);
                    LoadPhieuPhat_PhieuMuon(txtMaPhieuMuon.Text);
                    LoadSachTra(txtMaPhieuMuon.Text);
                }
                else
                {
                    MessageBox.Show("Không thể thêm phiếu phạt!", "Lỗi");
                }
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedMaPP))
            {
                MessageBox.Show("Vui lòng chọn một phiếu phạt để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    string sql = $"Delete from PhieuPhat where MaPhieuPhat = '{selectedMaPP}'";
                    cmd = new SqlCommand(sql, con);
                    try
                    {
                        if (cmd.ExecuteNonQuery() > 0 )
                        {
                            MessageBox.Show("Xóa phiếu phạt thành công!", "Thông báo", 
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadPhieuPhat();
                            int beforeRowIndex = currentIndex - 1;
                            dgvPhieuPhat.ClearSelection();
                            dgvPhieuPhat.CurrentCell = dgvPhieuPhat.Rows[beforeRowIndex].Cells[0];
                            dgvPhieuPhat.FirstDisplayedScrollingRowIndex = beforeRowIndex;
                            NapCT();
                            LoadSachPhat(txtMaPhieuPhat.Text);
                            LoadPhieuPhat_PhieuMuon(txtMaPhieuMuon.Text);
                            LoadSachTra(txtMaPhieuMuon.Text);
                        } 
                        else
                        {
                            MessageBox.Show("Xóa phiếu phạt không thành công!", "Thông báo", 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Phiếu phạt liên quan tới nhiều bảng dữ liệu khác", "Lỗi", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            int currentIndex = dgvPhieuPhat.CurrentRow.Index;
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "Update PhieuPhat set NgayNopPhat = @NgayNopPhat, MaThuThu = @MaThuThu " +
                    $"where MaPhieuPhat = '{selectedMaPP}'";
                cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@NgayNopPhat", dtNgayNopPhat.Value.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@MaThuThu", cboThuThu.SelectedValue);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Sửa phiếu phạt thành công!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadPhieuPhat();
                dgvPhieuPhat.ClearSelection();
                dgvPhieuPhat.CurrentCell = dgvPhieuPhat.Rows[currentIndex].Cells[0];
                dgvPhieuPhat.FirstDisplayedScrollingRowIndex = currentIndex;
                NapCT();
                LoadSachPhat(txtMaPhieuPhat.Text);
                LoadPhieuPhat_PhieuMuon(txtMaPhieuMuon.Text);
                LoadSachTra(txtMaPhieuMuon.Text);
            }
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
            if (cboTruong2.SelectedIndex == 0)
            {
                dvPP.RowFilter = $"MaPhieuPhat like '%{txtSearch2.Text}%'";
            }
            else if (cboTruong2.SelectedIndex == 1)
            {
                dvPP.RowFilter = $"MaPhieuMuon like '%{txtSearch2.Text}%'";
            }
            else
            {
                dvPP.RowFilter = $"MaDocGia like '%{txtSearch2.Text}%'";
            }
        }

        private void btnPhatSach_Click(object sender, EventArgs e)
        {
            var f = new frmPhieuTra();
            f.MaPhieuPhat = txtMaPhieuPhat.Text;
            f.MaPhieuMuon = txtMaPhieuMuon.Text;
            f.MaDocGia = txtMaDocGia.Text;
            f.ShowDialog();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            int currentIndex = dgvPhieuPhat.CurrentRow.Index;
            string currentMaPP = selectedMaPP;

            // 1. Load lại danh sách phiếu phạt (cập nhật TongTienPhat)
            LoadPhieuPhat();

            // 2. Chọn lại dòng trong dgvPhieuPhat (giữ selection)
            if (currentIndex >= 0 && currentIndex < dgvPhieuPhat.Rows.Count)
            {
                dgvPhieuPhat.ClearSelection();
                dgvPhieuPhat.CurrentCell = dgvPhieuPhat.Rows[currentIndex].Cells[0];
                dgvPhieuPhat.FirstDisplayedScrollingRowIndex = currentIndex;
            }

            // 3. Nạp lại chi tiết (txtTongTienPhat và các thông tin khác)
            NapCT();

            // 4. Load lai phieu muon
            LoadPhieuPhat_PhieuMuon(txtMaPhieuMuon.Text);

            // 5. Load lại danh sách sách phạt (dgvSachPhat)
            LoadSachPhat(currentMaPP);

            // 6. Load lại danh sách sách trả (dgvSachTra)
            LoadSachTra(txtMaPhieuMuon.Text);
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

        private void LoadPhieuPhat()
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "select pp.MaPhieuPhat, pp.MaPhieuMuon, pm.MaDocGia, pp.NgayNopPhat, coalesce(sum(ct_pp.NopPhat), 0) as TongTienPhat, pp.MaThuThu " +
                    "from PhieuPhat pp left join CT_PhieuPhat ct_pp on pp.MaPhieuPhat = ct_pp.MaPhieuPhat " +
                    "join PhieuMuon pm on pp.MaPhieuMuon = pm.MaPhieuMuon " +
                    "group by pp.MaPhieuPhat, pp.MaPhieuMuon,pm.MaDocGia, pp.NgayNopPhat, pp.MaThuThu";
                adapter = new SqlDataAdapter(sql, con);
                dt = new DataTable();
                adapter.Fill(dt);
                dvPP = new DataView(dt);
                dgvPhieuPhat.DataSource = dvPP;
            }
        }
    }
}
