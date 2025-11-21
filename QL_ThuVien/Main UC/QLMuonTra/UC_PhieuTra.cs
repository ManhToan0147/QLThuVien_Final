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
        }

        private void UC_PhieuTra_Load(object sender, EventArgs e)
        {
            cboTruong1.SelectedIndex = 0;
            cboTruong2.SelectedIndex = 0;

            //Fix lỗi column header
            dgvPMCanTra.ColumnHeadersDefaultCellStyle.Font = new Font(dgvPMCanTra.Font, FontStyle.Bold);
            dgvPMDaTra.ColumnHeadersDefaultCellStyle.Font = new Font(dgvPMDaTra.Font, FontStyle.Bold);
            dgvSachTra.DefaultCellStyle.Font = new Font(dgvSachTra.Font, FontStyle.Regular);
            dgvSachTra.Columns["DaTraSach"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            LoadPMDaTra();
            LoadPMCanTra();
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
                    SELECT MaPhieuMuon, MaDocGia, NgayMuon, HanTra, NgayThucTra,
                           CASE 
                               WHEN HanTra < NgayThucTra THEN DATEDIFF(DAY, HanTra, NgayThucTra) 
                               ELSE 0
                           END AS SoNgayTre
                    FROM PhieuMuon
                    WHERE NgayThucTra IS NOT NULL";
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
            NapCT();
            LoadSachTra(selectedMaPM);
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
            if (cboTruong2.SelectedIndex == 0)
            {
                dvPMDaTra.RowFilter = $"MaPhieuMuon like '%{txtSearch2.Text}%'";
            }
            else
            {
                dvPMDaTra.RowFilter = $"MaDocGia like '%{txtSearch2.Text}%'";
            }
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
                        LoadPMCanTra();
                        // Tìm dòng chứa mã của bản ghi vừa thêm
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

            DialogResult rs = MessageBox.Show("Bạn có chắc chắn muốn xóa thông tin trả cho phiếu mượn này không?",
             "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (rs == DialogResult.Yes)
            {
                using (con = new SqlConnection(strCon))
                {
                    con.Open();
                    string sql = $"UPDATE PhieuMuon SET NgayThucTra = NULL WHERE MaPhieuMuon = '{selectedMaPM}'";
                    cmd = new SqlCommand(sql, con);
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        MessageBox.Show("Xóa thông tin trả sách thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadPMDaTra();
                        LoadPMCanTra();

                        int beforeRowIndex = currentIndex - 1;
                        dgvPMDaTra.ClearSelection();
                        dgvPMDaTra.CurrentCell = dgvPMDaTra.Rows[beforeRowIndex].Cells[0];
                        NapCT();
                        dgvPMDaTra.FirstDisplayedScrollingRowIndex = beforeRowIndex;
                        LoadSachTra(txtMaPhieuMuon.Text);
                    }
                    else
                    {
                        MessageBox.Show("Xóa thất bại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                }
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedMaPM))
            {
                MessageBox.Show("Chưa chọn phiếu mượn để sửa thông tin trả");
                return;
            }
            int currentIndex = dgvPMDaTra.CurrentRow.Index;
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "Update PhieuMuon set NgayThucTra = @NgayThucTra " +
                        $"where MaPhieuMuon = '{selectedMaPM}'";
                cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@NgayThucTra", dtNgayThucTra.Value.ToString("yyyy-MM-dd"));
                try
                {
                    if (cmd.ExecuteNonQuery() > 0)
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
                    LoadPMDaTra();
                    LoadPMCanTra();
                    dgvPMDaTra.ClearSelection();
                    dgvPMDaTra.CurrentCell = dgvPMDaTra.Rows[currentIndex].Cells[0];
                    dgvPMDaTra.FirstDisplayedScrollingRowIndex = currentIndex;
                    NapCT();
                    LoadSachTra(selectedMaPM);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi " + ex.Message, "Lỗi",
                           MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            int currentIndex = dgvPMDaTra.CurrentRow.Index;
            string currentMaPM = selectedMaPM;

            // 1. Load lại danh sách phiếu mượn đã trả (cập nhật NgayThucTra, SoNgayTre)
            LoadPMDaTra();

            // 2. Load lại danh sách phiếu mượn cần trả (cập nhật danh sách)
            LoadPMCanTra();

            // 3. Chọn lại dòng trong dgvPMDaTra (giữ selection)
            if (currentIndex >= 0 && currentIndex < dgvPMDaTra.Rows.Count)
            {
                dgvPMDaTra.ClearSelection();
                dgvPMDaTra.CurrentCell = dgvPMDaTra.Rows[currentIndex].Cells[0];
                dgvPMDaTra.FirstDisplayedScrollingRowIndex = currentIndex;
            }

            // 4. Nạp lại thông tin chi tiết groupbox 
            NapCT();

            // 5. Load lại danh sách sách trả (dgvSachTra)
            LoadSachTra(currentMaPM);
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
        private void NapCT()
        {
            if (dgvPMDaTra.CurrentCell != null && dgvPMDaTra.CurrentCell.RowIndex >= 0)
            {
                int i = dgvPMDaTra.CurrentRow.Index;
                selectedMaPM = dgvPMDaTra.Rows[i].Cells[0].Value.ToString();
                txtMaPhieuMuon.Text = selectedMaPM;
                txtMaPhieuMuon.Enabled = string.IsNullOrEmpty(selectedMaPM);

                selectedMaDG = dgvPMDaTra.Rows[i].Cells[1].Value.ToString();
                txtMaDG.Text = selectedMaDG;

                dtNgayMuon.Text = dgvPMDaTra.Rows[i].Cells[2].Value.ToString();
                dtHanTra.Text = dgvPMDaTra.Rows[i].Cells[3].Value.ToString();
                dtNgayThucTra.Text = dgvPMDaTra.Rows[i].Cells[4].Value.ToString();

                txtSoNgayTre.Text = dgvPMDaTra.Rows[i].Cells[5].Value.ToString();
                txtSoNgayTre.Enabled = !string.IsNullOrEmpty(txtSoNgayTre.Text);

            }
        }
    }
}
