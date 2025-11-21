using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Web.Hosting;

namespace QL_ThuVien
{
    public partial class frmLoaiSach : Form
    {
        string strCon = DBConfig.ConnectionString;
        SqlConnection con;
        SqlDataAdapter adapter;
        SqlCommand cmd;
        DataTable dt;
        DataView dv;
        public frmLoaiSach()
        {
            InitializeComponent();
        }

        private void frmLoaiSach_Load(object sender, EventArgs e)
        {
            dgvLoaiSach.ColumnHeadersDefaultCellStyle.Font = new Font(dgvLoaiSach.Font, FontStyle.Bold);
            LoadLoaiSach();
        }

        private void LoadLoaiSach()
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "Select * from LoaiSach";
                adapter = new SqlDataAdapter(sql, con);
                dt = new DataTable();
                adapter.Fill(dt);
                dv = new DataView(dt);
                dgvLoaiSach.DataSource = dv;
            }
        }

        private void dgvLoaiSach_SelectionChanged(object sender, EventArgs e)
        {
            NapCT();
        }

        private string selectedMaLoaiSach;
        private void NapCT()
        {
            if (dgvLoaiSach.CurrentRow != null && dgvLoaiSach.CurrentRow.Index >= 0)
            {
                int i = dgvLoaiSach.CurrentRow.Index;
                selectedMaLoaiSach = dgvLoaiSach.Rows[i].Cells[0].Value.ToString();
                txtMaLoaiSach.Text = selectedMaLoaiSach;
                txtMaLoaiSach.Enabled = string.IsNullOrEmpty(txtMaLoaiSach.Text);
                
                txtTenLoaiSach.Text = dgvLoaiSach.Rows[i].Cells[1].Value.ToString();
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            dv.RowFilter = $"TenLoaiSach like '%{txtSearch.Text}%'";
        }

        private void btnTaoMoi_Click(object sender, EventArgs e)
        {
            txtMaLoaiSach.Text = "";
            txtMaLoaiSach.Enabled = true;
            txtTenLoaiSach.Text = "";
            txtMaLoaiSach.Focus();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                using (con = new SqlConnection(strCon))
                {
                    con.Open();
                    string sql = $"Insert into LoaiSach values ('{txtMaLoaiSach.Text}', N'{txtTenLoaiSach.Text}')";
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
            LoadLoaiSach();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvLoaiSach.SelectedRows.Count == 0)
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

                        foreach (DataGridViewRow row in dgvLoaiSach.SelectedRows)
                        {
                            string maLoaiSach = row.Cells["MaLoaiSach"].Value.ToString();
                            string sql = "DELETE FROM LoaiSach WHERE MaLoaiSach = @MaLoaiSach";
                            cmd = new SqlCommand(sql, con);
                            cmd.Parameters.AddWithValue("@MaLoaiSach", maLoaiSach);

                            int kq = cmd.ExecuteNonQuery();
                            if (kq > 0)
                            {
                                successCount++;
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
            LoadLoaiSach();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedMaLoaiSach))
            {
                MessageBox.Show("Chưa chọn bản ghi", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = $"Update LoaiSach set TenLoaiSach = N'{txtTenLoaiSach.Text}' " +
                    $"Where MaLoaiSach = '{txtMaLoaiSach.Text}'";
                cmd = new SqlCommand(sql, con);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Cập nhật thành công", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);    
            }
            LoadLoaiSach();
        }
    }
}
