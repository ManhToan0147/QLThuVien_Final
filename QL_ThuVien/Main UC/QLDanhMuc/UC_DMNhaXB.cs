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
    public partial class UC_DMNhaXB : UserControl
    {
        string strCon = DBConfig.ConnectionString;
        SqlConnection con;
        SqlDataAdapter adapter;
        SqlCommand cmd;
        DataTable dt;
        DataView dv;
        public UC_DMNhaXB()
        {
            InitializeComponent();
        }

        private void UC_DMNhaXB_Load(object sender, EventArgs e)
        {
            dgvNXB.ColumnHeadersDefaultCellStyle.Font = new Font(dgvNXB.Font, FontStyle.Bold);
            LoadNXB();
        }
        private void LoadNXB()
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "Select * from NXB";
                adapter = new SqlDataAdapter(sql, con);
                dt = new DataTable();
                adapter.Fill(dt);
                dv = new DataView(dt);
                dgvNXB.DataSource = dv;
            }
        }

        private void dgvNXB_SelectionChanged(object sender, EventArgs e)
        {
            NapCT();
        }

        private string selectedMaNXB;
        private void NapCT()
        {
            if (dgvNXB.CurrentRow != null && dgvNXB.CurrentRow.Index >= 0)
            {
                int i = dgvNXB.CurrentRow.Index;
                selectedMaNXB = dgvNXB.Rows[i].Cells[0].Value.ToString();
                txtMaNXB.Text = selectedMaNXB;
                txtMaNXB.Enabled = string.IsNullOrEmpty(txtMaNXB.Text);

                txtTenNXB.Text = dgvNXB.Rows[i].Cells[1].Value.ToString();
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            dv.RowFilter = $"TenNXB like '%{txtSearch.Text}%'";
        }

        private void btnTaoMoi_Click(object sender, EventArgs e)
        {
            txtMaNXB.Text = "";
            txtMaNXB.Enabled = true;
            txtTenNXB.Text = "";
            txtMaNXB.Focus();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                using (con = new SqlConnection(strCon))
                {
                    con.Open();
                    string sql = $"Insert into NXB values ('{txtMaNXB.Text}', N'{txtTenNXB.Text}')";
                    cmd = new SqlCommand(sql, con);
                    int kq = cmd.ExecuteNonQuery();
                    if (kq > 0)
                    {
                        MessageBox.Show("Thêm thành công bản ghi", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Thêm không thành công bản ghi", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm " + ex.Message);
            }
            LoadNXB();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvNXB.SelectedRows.Count == 0)
            {
                MessageBox.Show("Chưa chọn bản ghi nào", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult rs = MessageBox.Show("Bạn có chắc chắn muốn xóa các bản ghi đã chọn?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (rs == DialogResult.Yes)
            {
                try
                {
                    using (con = new SqlConnection(strCon))
                    {
                        con.Open();
                        int successCount = 0;

                        foreach (DataGridViewRow row in dgvNXB.SelectedRows)
                        {
                            string maNXB = row.Cells["MaNXB"].Value.ToString();
                            string sql = "DELETE FROM NXB WHERE MaNXB = @MaNXB";

                            using (SqlCommand cmd = new SqlCommand(sql, con))
                            {
                                cmd.Parameters.AddWithValue("@MaNXB", maNXB);

                                int kq = cmd.ExecuteNonQuery();
                                if (kq > 0)
                                {
                                    successCount++;
                                }
                            }
                        }

                        MessageBox.Show($"Xóa thành công {successCount} bản ghi.", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa: " + ex.Message, "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            LoadNXB();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedMaNXB))
            {
                MessageBox.Show("Chưa chọn bản ghi", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = $"Update NXB set TenNXB = N'{txtTenNXB.Text}' " +
                    $"Where MaNXB = '{txtMaNXB.Text}'";
                cmd = new SqlCommand(sql, con);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Cập nhật thành công", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            LoadNXB();
        }
    }
}
