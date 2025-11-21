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
    public partial class UC_DMTacGia : UserControl
    {
        string strCon = DBConfig.ConnectionString;
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataTable dt;
        DataView dv;
        public UC_DMTacGia()
        {
            InitializeComponent();
        }

        private void UC_DMTacGia_Load(object sender, EventArgs e)
        {
            dgvTacGia.ColumnHeadersDefaultCellStyle.Font = new Font(dgvTacGia.Font, FontStyle.Bold);
            LoadTacGia();
        }

        private void LoadTacGia()
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "Select * from TacGia";
                adapter = new SqlDataAdapter(sql, con);
                dt = new DataTable();
                adapter.Fill(dt);
            }
            dv = new DataView(dt);
            dgvTacGia.DataSource = dv;
        }

        private void dgvTacGia_SelectionChanged(object sender, EventArgs e)
        {
            NapCT();
        }

        private void NapCT()
        {
            if (dgvTacGia.CurrentCell != null && dgvTacGia.CurrentCell.RowIndex >= 0)
            {
                int i = dgvTacGia.CurrentRow.Index;
                txtMaTacGia.Text = dgvTacGia.Rows[i].Cells["MaTG"].Value.ToString();
                txtMaTacGia.Enabled = string.IsNullOrEmpty(txtMaTacGia.Text);

                txtTenTacGia.Text = dgvTacGia.Rows[i].Cells["TenTG"].Value.ToString();
                cboGioiTinh.Text = dgvTacGia.Rows[i].Cells["GioiTinh"].Value.ToString();
                txtNamSinh.Text = dgvTacGia.Rows[i].Cells["NamSinh"].Value.ToString();
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string search = txtSearch.Text;
            dv.RowFilter = $"TenTG LIKE '%{search}%'";
        }

        private void btnTaoMoi_Click(object sender, EventArgs e)
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "Select max(MaTG) from TacGia";
                cmd = new SqlCommand(sql, con);
                object rs = cmd.ExecuteScalar();
                if (rs != DBNull.Value && rs != null)
                {
                    string maTacGia = rs.ToString();
                    int number = int.Parse(maTacGia.Substring(2)); //Lấy sau phầm "TG"
                    ++number;
                    txtMaTacGia.Text = "TG" + number.ToString("D2");
                }
            }

            txtTenTacGia.Text = "";
            cboGioiTinh.SelectedIndex = -1;
            txtNamSinh.Text = "";

            txtTenTacGia.Focus();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                string maTG = txtMaTacGia.Text.Trim();
                string tenTG = txtTenTacGia.Text.Trim();
                string gioiTinh = cboGioiTinh.Text.Trim();
                string namSinh = txtNamSinh.Text.Trim();

                using (con = new SqlConnection(strCon))
                {
                    con.Open();
                    string query = "INSERT INTO TacGia (MaTG, TenTG, GioiTinh, NamSinh) VALUES (@MaTG, @TenTG, @GioiTinh, @NamSinh)";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@MaTG", maTG);
                        cmd.Parameters.AddWithValue("@TenTG", tenTG);
                        cmd.Parameters.AddWithValue("@GioiTinh", gioiTinh);
                        cmd.Parameters.AddWithValue("@NamSinh", namSinh);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show($"Đã thêm thành công bản ghi", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadTacGia();
                int lastIndex = dgvTacGia.RowCount - 1;
                dgvTacGia.ClearSelection();
                dgvTacGia.CurrentCell = dgvTacGia.Rows[lastIndex].Cells[0];
                NapCT();
                dgvTacGia.FirstDisplayedScrollingRowIndex = lastIndex;


            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi khi them du lieu " + ex.Message);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvTacGia.SelectedRows.Count > 0)
            {
                DialogResult rs = MessageBox.Show("Bạn có chắc chắn muốn xóa các bản ghi đã chọn",
                    "Xác nhận yêu cầu", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                int currentIndex = dgvTacGia.CurrentRow.Index;
                int deletedCount = 0;
                if (rs == DialogResult.Yes)
                {
                    using (SqlConnection con = new SqlConnection(strCon))
                    {
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            cmd.Connection = con;



                            foreach (DataGridViewRow row in dgvTacGia.SelectedRows)
                            {
                                string maTG = row.Cells["MaTG"].Value.ToString();
                                string sql2 = $"DELETE FROM TacGia WHERE MaTG = '{maTG}';";

                                try
                                {
                                    // Thực hiện xóa trong bảng TacGia
                                    cmd.CommandText = sql2;
                                    int kq = cmd.ExecuteNonQuery();

                                    if (kq > 0)
                                    {
                                        deletedCount++;
                                    }
                                }
                                catch (Exception ex)
                                {

                                    MessageBox.Show("Lỗi khi xóa " + ex.Message);
                                }
                            }

                            MessageBox.Show($"Đã xóa {deletedCount} bản ghi", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    LoadTacGia();
                    int beforeRowIndex = currentIndex - deletedCount;
                    dgvTacGia.ClearSelection();
                    dgvTacGia.CurrentCell = dgvTacGia.Rows[beforeRowIndex].Cells[0];
                    NapCT();
                    dgvTacGia.FirstDisplayedScrollingRowIndex = beforeRowIndex;
                }
            }
            else
            {
                MessageBox.Show("Chưa chọn bản ghi nào để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            string maTG = txtMaTacGia.Text.Trim();

            if (string.IsNullOrEmpty(maTG))
            {
                MessageBox.Show("Vui lòng chọn một ô để cập nhật.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int currentIndex = dgvTacGia.CurrentRow.Index;

            string tenTG = txtTenTacGia.Text.Trim();
            string gioiTinh = cboGioiTinh.Text.Trim();
            string namSinh = txtNamSinh.Text.Trim();

            string sql = $"Update TacGia set TenTG = N'{tenTG}', " +
                $"GioiTinh = N'{gioiTinh}', " +
                $"NamSinh = {namSinh} where MaTG = '{maTG}'";

            using (con = new SqlConnection(strCon))
            {
                con.Open();
                cmd = new SqlCommand(sql, con);
                int kq = cmd.ExecuteNonQuery();
                if (kq > 0)
                {
                    MessageBox.Show("Cập nhật thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Cập nhật thất bại. Vui lòng kiểm tra lại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            LoadTacGia();
            dgvTacGia.ClearSelection();
            dgvTacGia.CurrentCell = dgvTacGia.Rows[currentIndex].Cells[0];
            NapCT();
            dgvTacGia.FirstDisplayedScrollingRowIndex = currentIndex;
        }

        private void txtNamSinh_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Ngăn không cho nhập ký tự không hợp lệ
                errorProvider1.SetError((Control)sender, "Chỉ được nhập số!");
            }
            else
            {
                errorProvider1.SetError((Control)sender, ""); // Xóa thông báo lỗi nếu nhập đúng
            }
        }
    }
}
