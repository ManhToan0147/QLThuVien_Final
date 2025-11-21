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
        public UC_DMViPham()
        {
            InitializeComponent();
        }

        private void UC_DMViPham_Load(object sender, EventArgs e)
        {
            dgvViPham.ColumnHeadersDefaultCellStyle.Font = new Font(dgvViPham.Font, FontStyle.Bold);
            LoadViPham();
        }
        private void LoadViPham()
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "SELECT * FROM ViPham ORDER BY MaViPham";
                adapter = new SqlDataAdapter(sql, con);
                dt = new DataTable();
                adapter.Fill(dt);
            }
            dv = new DataView(dt);
            dgvViPham.DataSource = dv;
        }

        private void dgvViPham_SelectionChanged(object sender, EventArgs e)
        {
            NapCT();
        }

        private void NapCT()
        {
            if (dgvViPham.CurrentCell != null && dgvViPham.CurrentCell.RowIndex >= 0)
            {
                int i = dgvViPham.CurrentRow.Index;
                txtMaViPham.Text = dgvViPham.Rows[i].Cells["MaViPham"].Value.ToString();
                txtTenViPham.Text = dgvViPham.Rows[i].Cells["TenViPham"].Value.ToString();
                txtHinhThucPhat.Text = dgvViPham.Rows[i].Cells["HinhThucPhat"].Value.ToString();
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
                    int number = int.Parse(maViPham.Substring(2)); // Lấy sau "VP"
                    number++;
                    txtMaViPham.Text = "VP" + number.ToString("D2");
                }
                else
                {
                    txtMaViPham.Text = "VP01";
                }
            }

            // Clear các field khác
            txtTenViPham.Text = "";
            txtHinhThucPhat.Text = "";

            // Focus vào tên vi phạm
            txtTenViPham.Focus();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                string maVP = txtMaViPham.Text.Trim();
                string tenVP = txtTenViPham.Text.Trim();
                string hinhThucPhat = txtHinhThucPhat.Text.Trim();

                using (con = new SqlConnection(strCon))
                {
                    con.Open();
                    string query = "INSERT INTO ViPham (MaViPham, TenViPham, HinhThucPhat) VALUES (@MaViPham, @TenViPham, @HinhThucPhat)";

                    cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@MaViPham", maVP);
                    cmd.Parameters.AddWithValue("@TenViPham", tenVP);
                    cmd.Parameters.AddWithValue("@HinhThucPhat", string.IsNullOrEmpty(hinhThucPhat) ? (object)DBNull.Value : hinhThucPhat);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Đã thêm thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadViPham();

                int lastIndex = dgvViPham.RowCount - 1;
                dgvViPham.ClearSelection();
                dgvViPham.CurrentCell = dgvViPham.Rows[lastIndex].Cells[0];
                NapCT();
                dgvViPham.FirstDisplayedScrollingRowIndex = lastIndex;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm dữ liệu: " + ex.Message);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvViPham.SelectedRows.Count > 0)
            {
                DialogResult rs = MessageBox.Show(
                    "Bạn có chắc chắn muốn xóa các bản ghi đã chọn?",
                    "Xác nhận yêu cầu",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (rs == DialogResult.No) return;

                int currentIndex = dgvViPham.CurrentRow.Index;
                int deletedCount = 0;

                using (SqlConnection con = new SqlConnection(strCon))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = con;

                        foreach (DataGridViewRow row in dgvViPham.SelectedRows)
                        {
                            string maVP = row.Cells["MaViPham"].Value.ToString();
                            string sql = $"DELETE FROM ViPham WHERE MaViPham = '{maVP}'";

                            try
                            {
                                cmd.CommandText = sql;
                                int kq = cmd.ExecuteNonQuery();

                                if (kq > 0)
                                {
                                    deletedCount++;
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Lỗi khi xóa: " + ex.Message);
                            }
                        }

                        MessageBox.Show($"Đã xóa {deletedCount} bản ghi", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                LoadViPham();

                int beforeRowIndex = currentIndex - deletedCount;
                dgvViPham.ClearSelection();
                dgvViPham.CurrentCell = dgvViPham.Rows[beforeRowIndex].Cells[0];
                NapCT();
                dgvViPham.FirstDisplayedScrollingRowIndex = beforeRowIndex;
            }
            else
            {
                MessageBox.Show("Chưa chọn bản ghi nào để xóa!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            string maVP = txtMaViPham.Text.Trim();

            if (string.IsNullOrEmpty(maVP))
            {
                MessageBox.Show("Vui lòng chọn một vi phạm để cập nhật!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int currentIndex = dgvViPham.CurrentRow.Index;

            string tenVP = txtTenViPham.Text.Trim();
            string hinhThucPhat = txtHinhThucPhat.Text.Trim();

            string sql = "UPDATE ViPham SET TenViPham = @TenViPham, HinhThucPhat = @HinhThucPhat WHERE MaViPham = @MaViPham";

            using (con = new SqlConnection(strCon))
            {
                con.Open();
                cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@MaViPham", maVP);
                cmd.Parameters.AddWithValue("@TenViPham", tenVP);
                cmd.Parameters.AddWithValue("@HinhThucPhat", string.IsNullOrEmpty(hinhThucPhat) ? (object)DBNull.Value : hinhThucPhat);

                int kq = cmd.ExecuteNonQuery();

                if (kq > 0)
                {
                    MessageBox.Show("Cập nhật thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Cập nhật thất bại. Vui lòng kiểm tra lại!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            LoadViPham();
            dgvViPham.ClearSelection();
            dgvViPham.CurrentCell = dgvViPham.Rows[currentIndex].Cells[0];
            NapCT();
            dgvViPham.FirstDisplayedScrollingRowIndex = currentIndex;
        }
    }
}
