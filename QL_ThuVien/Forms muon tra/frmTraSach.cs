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

namespace QL_ThuVien
{
    public partial class frmTraSach : Form
    {
        string strCon = DBConfig.ConnectionString;
        SqlConnection con;
        SqlDataAdapter adapter;
        SqlCommand cmd;
        DataTable dt;
        DataView dv;

        public string MaPhieuMuon { get; set; }
        public string MaDocGia { get; set; }
        public frmTraSach()
        {
            InitializeComponent();
        }

        private void frmTraSach_Load(object sender, EventArgs e)
        {
            txtMaPhieuMuon.Text = MaPhieuMuon;
            txtMaDG.Text = MaDocGia;
            //Can giua header
            dgvSachMuon.Columns["DaTraSach"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvSachTra.DefaultCellStyle.Font = new Font(dgvSachTra.Font, FontStyle.Regular);
            dgvSachMuon.DefaultCellStyle.Font = new Font(dgvSachMuon.Font, FontStyle.Regular);
            LoadSachMuon();
            LoadSachTra();
        }

        private void LoadSachTra()
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "select MaSach, TinhTrangTra from CT_PhieuMuon " +
                    $"where MaPhieuMuon = '{MaPhieuMuon}' and DaTraSach = 1";
                adapter = new SqlDataAdapter(sql, con);
                dt = new DataTable();
                adapter.Fill(dt);
                dgvSachTra.DataSource = dt;
            }
        }

        private void LoadSachMuon()
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "select MaSach, TienCoc, DaTraSach, TinhTrangMuon from CT_PhieuMuon " +
                    $"where MaPhieuMuon = '{MaPhieuMuon}'";
                adapter = new SqlDataAdapter(sql, con);
                dt = new DataTable();
                adapter.Fill(dt);
                dgvSachMuon.DataSource = dt;
            }
        }

        private void btnChuyen_Click(object sender, EventArgs e)
        {
            if (dgvSachMuon.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvSachMuon.SelectedRows)
                {
                    string maSach = row.Cells[0].Value.ToString();
                    string moTa = string.Empty;
                    string tinhTrangTra = row.Cells[3].Value.ToString();
                    using (con = new SqlConnection(strCon))
                    {
                        con.Open();

                        string sql = $"Update CT_PhieuMuon set DaTraSach = 1, TinhTrangTra = N'{tinhTrangTra}' " +
                            $"where MaPhieuMuon = '{MaPhieuMuon}' and MaSach = '{maSach}' ";
                        cmd = new SqlCommand(sql, con);
                        try
                        {
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Lỗi " + ex.Message);
                        }
                    }
                }
                LoadSachTra();
                LoadSachMuon();
            }
            else
            {
                MessageBox.Show("Chưa chọn sách để trả");
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvSachTra.SelectedRows.Count > 0)
            {
                DialogResult rs = MessageBox.Show("Bạn có chắc chắn muốn xóa thông tin trả cuốn sách này không?", "Xác nhận",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (rs == DialogResult.Yes)
                {
                    foreach (DataGridViewRow row in dgvSachTra.SelectedRows)
                    {
                        string maSach = row.Cells[0].Value.ToString();
                        using (con = new SqlConnection(strCon))
                        {
                            con.Open();
                            string sql = "Update CT_PhieuMuon set DaTraSach = 0, TinhTrangTra = NULL " +
                                $"where MaPhieuMuon = '{MaPhieuMuon}' and MaSach = '{maSach}' ";
                            cmd = new SqlCommand(sql, con);
                            try
                            {
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Lỗi " + ex.Message);
                            }
                        }
                    }
                    LoadSachTra();
                    LoadSachMuon();
                }
            }
            else
            {
                MessageBox.Show("Chưa chọn sách để xóa thông tin trả");
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                foreach (DataGridViewRow row in dgvSachTra.Rows)
                {
                    if (!row.IsNewRow) // Loại trừ dòng trống
                    {
                        string maSach = row.Cells[0].Value.ToString();
                        string tinhTrangTra = row.Cells["TinhTrangTra"].Value.ToString();

                        string sql = "UPDATE CT_PhieuMuon SET TinhTrangTra = @TinhTrangTra " +
                            "WHERE MaPhieuMuon = @MaPhieuMuon AND MaSach = @MaSach";
                        cmd = new SqlCommand(sql, con);
                        cmd.Parameters.AddWithValue("@TinhTrangTra", tinhTrangTra);
                        cmd.Parameters.AddWithValue("@MaPhieuMuon", MaPhieuMuon);
                        cmd.Parameters.AddWithValue("@MaSach", maSach);
                        cmd.ExecuteNonQuery();

                        string updateMoTa = $"Update CuonSach Set MoTa = N'{tinhTrangTra}' " +
                            $"where MaSach = '{maSach}'";
                        cmd = new SqlCommand(updateMoTa, con);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            MessageBox.Show("Dữ liệu đã được lưu thành công.");
        }
    }
}
