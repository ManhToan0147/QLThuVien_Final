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
    public partial class frmKhoSach : Form
    {
        string strCon = DBConfig.ConnectionString;
        SqlConnection con;
        SqlDataAdapter adapter;
        SqlCommand cmd;
        DataTable dt;
        DataView dv;
        public frmKhoSach()
        {
            InitializeComponent();
        }

        private void frmKhoSach_Load(object sender, EventArgs e)
        {
            dgvKhoSach.ColumnHeadersDefaultCellStyle.Font = new Font(dgvKhoSach.Font, FontStyle.Bold);
            LoadKhoSach();
        }

        private void LoadKhoSach()
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "Select * from KhoSach";
                adapter = new SqlDataAdapter(sql, con);
                dt = new DataTable();
                adapter.Fill(dt);
                dv = new DataView(dt);
                dgvKhoSach.DataSource = dv;
            }
        }

        private void dgvKhoSach_SelectionChanged(object sender, EventArgs e)
        {
            NapCT();
        }

        private string selectedMaKho;
        private void NapCT()
        {
            if (dgvKhoSach.CurrentRow != null && dgvKhoSach.CurrentRow.Index >= 0)
            {
                int i = dgvKhoSach.CurrentRow.Index;
                selectedMaKho = dgvKhoSach.Rows[i].Cells[0].Value.ToString();
                txtMaKho.Text = selectedMaKho;
                txtMaKho.Enabled = string.IsNullOrEmpty(txtMaKho.Text);

                txtTenKho.Text = dgvKhoSach.Rows[i].Cells[1].Value.ToString();
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            dv.RowFilter = $"TenKho like '%{txtSearch.Text}%'";
        }

        private void btnTaoMoi_Click(object sender, EventArgs e)
        {
            txtMaKho.Text = "";
            txtMaKho.Enabled = true;
            txtTenKho.Text = "";
            txtMaKho.Focus();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                using (con = new SqlConnection(strCon))
                {
                    con.Open();
                    string sql = $"Insert into KhoSach values ('{txtMaKho.Text}', N'{txtTenKho.Text}')";
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
            LoadKhoSach();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvKhoSach.SelectedRows.Count == 0)
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

                        foreach (DataGridViewRow row in dgvKhoSach.SelectedRows)
                        {
                            string maKho = row.Cells["MaKho"].Value.ToString();
                            string sql = "DELETE FROM KhoSach WHERE MaKho = @MaKho";

                            using (SqlCommand cmd = new SqlCommand(sql, con))
                            {
                                cmd.Parameters.AddWithValue("@MaKho", maKho);

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
            LoadKhoSach();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedMaKho))
            {
                MessageBox.Show("Chưa chọn bản ghi", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = $"Update KhoSach set TenKho = N'{txtTenKho.Text}' " +
                    $"Where MaKho = '{txtMaKho.Text}'";
                cmd = new SqlCommand(sql, con);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Cập nhật thành công", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            LoadKhoSach();

        }
    }
}
