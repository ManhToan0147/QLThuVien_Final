using QL_ThuVien.User_Management;
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
    public partial class frmChuDe : Form
    {
        private readonly string currentUserRole = UserSession.UserRole;

        string strCon = DBConfig.ConnectionString;
        SqlConnection con;
        SqlDataAdapter adapter;
        SqlCommand cmd;
        DataTable dt;
        DataView dv;
        public frmChuDe()
        {
            InitializeComponent();
        }

        private void frmChuDe_Load(object sender, EventArgs e)
        {
            dgvChuDe.ColumnHeadersDefaultCellStyle.Font = new Font(dgvChuDe.Font, FontStyle.Bold);
            LoadChuDe();
        }

        private void LoadChuDe()
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "Select * from ChuDe";
                adapter = new SqlDataAdapter(sql, con);
                dt = new DataTable();
                adapter.Fill(dt);
                dv = new DataView(dt);
                dgvChuDe.DataSource = dv;
            }
        }

        private void dgvChuDe_SelectionChanged(object sender, EventArgs e)
        {
            NapCT();
        }

        private string selectedMaChuDe;
        private void NapCT()
        {
            if (dgvChuDe.CurrentRow != null && dgvChuDe.CurrentRow.Index >= 0)
            {
                int i = dgvChuDe.CurrentRow.Index;
                selectedMaChuDe = dgvChuDe.Rows[i].Cells[0].Value.ToString();
                txtMaChuDe.Text = selectedMaChuDe;
                txtMaChuDe.Enabled = string.IsNullOrEmpty(txtMaChuDe.Text);

                txtTenChuDe.Text = dgvChuDe.Rows[i].Cells[1].Value.ToString();
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            dv.RowFilter = $"TenChuDe like '%{txtSearch.Text}%'";
        }

        private void btnTaoMoi_Click(object sender, EventArgs e)
        {
            txtMaChuDe.Text = "";
            txtMaChuDe.Enabled = true;
            txtTenChuDe.Text = "";
            txtMaChuDe.Focus();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                using (con = new SqlConnection(strCon))
                {
                    con.Open();
                    string sql = $"Insert into ChuDe values ('{txtMaChuDe.Text}', N'{txtTenChuDe.Text}')";
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
            LoadChuDe();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvChuDe.SelectedRows.Count == 0)
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

                        foreach (DataGridViewRow row in dgvChuDe.SelectedRows)
                        {
                            string maChuDe = row.Cells["MaChuDe"].Value.ToString();
                            string sql = $"DELETE FROM ChuDe WHERE MaChuDe = '{maChuDe}'";
                            cmd = new SqlCommand(sql, con);

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
            LoadChuDe();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedMaChuDe))
            {
                MessageBox.Show("Chưa chọn bản ghi", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = $"Update ChuDe set TenChuDe = N'{txtTenChuDe.Text}' " +
                    $"Where MaChuDe = '{txtMaChuDe.Text}'";
                cmd = new SqlCommand(sql, con);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Cập nhật thành công", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            LoadChuDe();
        }
    }
}
