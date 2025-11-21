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

namespace QL_ThuVien.Form_support
{
    public partial class frmNhapTacGia : Form
    {
        string strCon = DBConfig.ConnectionString;
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataTable dt;
        DataView dvDSTG;      // Tác giả đã gán
        DataView dvTacGia;    // Tất cả tác giả

        public string MaDauSach { get; set; }
        public frmNhapTacGia()
        {
            InitializeComponent();
        }

        private void frmNhapTacGia_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(MaDauSach))
            {
                MessageBox.Show("Không có thông tin đầu sách!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            LoadThongTinDauSach();
            LoadDauSach_TacGia();
            LoadTacGia();
        }

        private void LoadThongTinDauSach()
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "SELECT MaDauSach, TenDauSach FROM DauSach WHERE MaDauSach = @MaDauSach";
                cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@MaDauSach", MaDauSach);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    string tenDauSach = reader["TenDauSach"].ToString();
                    txtMaDauSach.Text = MaDauSach;
                    txtTenDauSach.Text = tenDauSach;
                }
                reader.Close();
            }
        }

        private void LoadTacGia()
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "SELECT MaTG AS MaTG2, TenTG AS TenTG2 FROM TacGia";

                adapter = new SqlDataAdapter(sql, con);
                dt = new DataTable();
                adapter.Fill(dt);

                dvTacGia = new DataView(dt);
                dgvTacGia.DataSource = dvTacGia;
            }
        }

        private void LoadDauSach_TacGia()
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = @"SELECT tg.MaTG, tg.TenTG
                               FROM TacGia tg
                               INNER JOIN DauSach_TacGia dstg ON tg.MaTG = dstg.MaTG
                               WHERE dstg.MaDauSach = @MaDauSach";

                adapter = new SqlDataAdapter(sql, con);
                adapter.SelectCommand.Parameters.AddWithValue("@MaDauSach", MaDauSach);

                dt = new DataTable();
                adapter.Fill(dt);

                dvDSTG = new DataView(dt);
                dgvDSTG.DataSource = dvDSTG;

                lblTongSo.Text = $"{dt.Rows.Count}";
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (dvTacGia == null) return;

            string keyword = txtSearch.Text.Trim();

            if (string.IsNullOrEmpty(keyword))
            {
                dvTacGia.RowFilter = null;
            }
            else
            {
                dvTacGia.RowFilter = $"TenTG2 LIKE '%{keyword}%' OR MaTG2 LIKE '%{keyword}%'";
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (dgvTacGia.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một tác giả để thêm!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int count = 0;
            int countExist = 0;

            using (con = new SqlConnection(strCon))
            {
                con.Open();

                foreach (DataGridViewRow row in dgvTacGia.SelectedRows)
                {
                    var cellMaTG = row.Cells["MaTG2"].Value;
                    if (cellMaTG == null) continue;

                    string maTG = cellMaTG.ToString();

                    try
                    {
                        cmd = new SqlCommand("ThemDauSachTacGia", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@MaDauSach", MaDauSach);
                        cmd.Parameters.AddWithValue("@MaTG", maTG);

                        int kq = cmd.ExecuteNonQuery();
                        if (kq > 0)
                        {
                            count++;
                        }
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == 2627)
                        {
                            countExist++;
                        }
                        else
                        {
                            MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }

            LoadDauSach_TacGia();

            string message = $"Đã thêm mới {count} tác giả";
            if (countExist > 0)
            {
                message += $"\n({countExist} tác giả đã tồn tại)";
            }

            MessageBox.Show(message, "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvDSTG.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một tác giả để xóa!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult rs = MessageBox.Show(
                "Bạn có chắc chắn muốn xóa các tác giả đã chọn khỏi đầu sách này?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (rs == DialogResult.No) return;

            int count = 0;

            using (con = new SqlConnection(strCon))
            {
                con.Open();

                foreach (DataGridViewRow row in dgvDSTG.SelectedRows)
                {
                    string maTG = row.Cells["MaTG"].Value?.ToString() ?? string.Empty;
                    if (string.IsNullOrEmpty(maTG)) continue;

                    string sql = "DELETE FROM DauSach_TacGia WHERE MaDauSach = @MaDauSach AND MaTG = @MaTG";
                    cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@MaDauSach", MaDauSach);
                    cmd.Parameters.AddWithValue("@MaTG", maTG);

                    int kq = cmd.ExecuteNonQuery();
                    if (kq > 0)
                    {
                        count++;
                    }
                }
            }

            MessageBox.Show($"Đã xóa {count} tác giả", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            LoadDauSach_TacGia();
        }

        private void dgvTacGia_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || dvDSTG == null) return;

            string maTG = dgvTacGia.Rows[e.RowIndex].Cells["MaTG2"].Value?.ToString();
            if (string.IsNullOrEmpty(maTG)) return;

            bool daTonTai = false;
            foreach (DataRowView row in dvDSTG)
            {
                if (row["MaTG"].ToString() == maTG)
                {
                    daTonTai = true;
                    break;
                }
            }

            if (daTonTai)
            {
                dgvTacGia.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Gray;
                dgvTacGia.Rows[e.RowIndex].DefaultCellStyle.Font =
                    new Font(dgvTacGia.Font, FontStyle.Italic);
            }
            else
            {
                dgvTacGia.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                dgvTacGia.Rows[e.RowIndex].DefaultCellStyle.Font =
                    new Font(dgvTacGia.Font, FontStyle.Regular);
            }
        }
    }
}
