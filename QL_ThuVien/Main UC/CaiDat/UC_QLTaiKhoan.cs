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

namespace QL_ThuVien.Main_UC.CaiDat
{
    public partial class UC_QLTaiKhoan : UserControl
    {
        string strCon = DBConfig.ConnectionString;
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataTable dt;
        bool addNewFlag = false;
        public UC_QLTaiKhoan()
        {
            InitializeComponent();
        }

        private void UC_QLTaiKhoan_Load(object sender, EventArgs e)
        {
            LoadTaiKhoan();
            dgvTaiKhoan.ColumnHeadersDefaultCellStyle.Font = new Font(dgvTaiKhoan.Font, FontStyle.Bold);
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

        private void LoadTaiKhoan()
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "Select * from TaiKhoanDN";
                adapter = new SqlDataAdapter(sql, con);
                dt = new DataTable();
                adapter.Fill(dt);
                dgvTaiKhoan.DataSource = dt;
            }
        }
        string selectedID;
        private void NapCT()
        {
            if (dgvTaiKhoan.CurrentCell != null && dgvTaiKhoan.CurrentCell.RowIndex >= 0)
            {
                int i = dgvTaiKhoan.CurrentRow.Index;
                txtTenDN.Text = dgvTaiKhoan.Rows[i].Cells["TenDangNhap"].Value.ToString();
                txtEmail.Text = dgvTaiKhoan.Rows[i].Cells["Email"].Value.ToString();
                txtMatKhau.Text = dgvTaiKhoan.Rows[i].Cells["MatKhau"].Value.ToString();
                txtMaNguoiDung.Text = dgvTaiKhoan.Rows[i].Cells["MaNguoiDung"].Value.ToString();
                cboQuyen.Text = dgvTaiKhoan.Rows[i].Cells["Quyen"].Value.ToString();
                selectedID = dgvTaiKhoan.Rows[i].Cells["IDTaiKhoan"].Value.ToString();

                txtMaNguoiDung.Enabled = string.IsNullOrEmpty(txtMaNguoiDung.Text);
            }
        }

        private void dgvTaiKhoan_SelectionChanged(object sender, EventArgs e)
        {
            NapCT();
        }

        string insertedID;
        
        private void btnTaoMoi_Click(object sender, EventArgs e)
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "Select max(ID) from TaiKhoanDN";
                cmd = new SqlCommand(sql, con);
                object rs = cmd.ExecuteScalar();
                if (rs != DBNull.Value && rs != null)
                {
                    string id = rs.ToString();
                    int number = int.Parse(id.Substring(2)); //Lấy số sau phần "TT"
                    ++number;
                    insertedID = "ID" + number.ToString("D3"); // Dam bao co 2 chu so (01 thay vi 1)
                }
                else
                {
                    //TH không có bản ghi nào thì bắt đầu từ 01
                    insertedID = "ID001";
                }
            }
            txtMaNguoiDung.Enabled = true;

            txtTenDN.Text = "";
            txtEmail.Text = "";
            txtMatKhau.Text = "";
            txtMaNguoiDung.Text = "";
            cboQuyen.SelectedIndex = -1;
            txtTenDN.Focus();
            addNewFlag = true;
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (addNewFlag)
            {
                using (con = new SqlConnection(strCon))
                {
                    try
                    {
                        con.Open();

                        // Câu lệnh SQL
                        string sqlInsert = "INSERT INTO TaiKhoanDN (ID, TenDangNhap, Email, MatKhau, MaNguoiDung, Quyen) " +
                                           "VALUES (@ID, @TenDangNhap, @Email, @MatKhau, @MaNguoiDung, @Quyen)";

                        using (SqlCommand cmd = new SqlCommand(sqlInsert, con))
                        {
                            // Gán giá trị cho các tham số
                            cmd.Parameters.AddWithValue("@ID", insertedID);
                            cmd.Parameters.AddWithValue("@TenDangNhap", txtTenDN.Text);
                            cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                            cmd.Parameters.AddWithValue("@MatKhau", txtMatKhau.Text);
                            cmd.Parameters.AddWithValue("@MaNguoiDung", string.IsNullOrEmpty(txtMaNguoiDung.Text) ? (object)DBNull.Value : txtMaNguoiDung.Text);
                            cmd.Parameters.AddWithValue("@Quyen", string.IsNullOrEmpty(cboQuyen.Text) ? "Chưa có quyền" : cboQuyen.Text);

                            // Thực thi câu lệnh
                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Thêm tài khoản thành công!");
                                addNewFlag = false; // Reset trạng thái
                                LoadTaiKhoan();
                                int lastRowIndex = dgvTaiKhoan.RowCount - 1;
                                dgvTaiKhoan.ClearSelection();
                                dgvTaiKhoan.CurrentCell = dgvTaiKhoan.Rows[lastRowIndex].Cells[0];
                                dgvTaiKhoan.FirstDisplayedScrollingRowIndex = lastRowIndex;
                                NapCT();
                            }
                            else
                            {
                                MessageBox.Show("Không thể thêm tài khoản. Vui lòng thử lại!");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi: " + ex.Message);
                    }

                }
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show($"Bạn có chắc chắn muốn xóa tài khoản này không?",
                                          "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No)
            {
                return;
            }
            using (con = new SqlConnection(strCon))
            {
                try
                {
                    con.Open();
                    string sqlDelete = "DELETE FROM TaiKhoanDN WHERE ID = @ID";
                    using (cmd = new SqlCommand(sqlDelete, con))
                    {
                        cmd.Parameters.AddWithValue("@ID", selectedID);

                        // Thực thi câu lệnh xóa
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Xóa tài khoản thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadTaiKhoan();
                        }
                        else
                        {
                            MessageBox.Show("Xóa thất bại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(strCon))
            {
                try
                {
                    con.Open();
                    string sqlUpdate = "UPDATE TaiKhoanDN " +
                                       "SET TenDangNhap = @TenDangNhap, MatKhau = @MatKhau, Quyen = @Quyen " +
                                       "WHERE ID = @ID";

                    using (SqlCommand cmd = new SqlCommand(sqlUpdate, con))
                    {
                        // Gán giá trị cho các tham số
                        cmd.Parameters.AddWithValue("@TenDangNhap", txtTenDN.Text);
                        cmd.Parameters.AddWithValue("@MatKhau", txtMatKhau.Text);
                        cmd.Parameters.AddWithValue("@Quyen", string.IsNullOrEmpty(cboQuyen.Text) ? "Chưa có quyền" : cboQuyen.Text);
                        cmd.Parameters.AddWithValue("@ID", selectedID);

                        // Thực thi câu lệnh sửa
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Sửa bản ghi thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadTaiKhoan();
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy bản ghi để sửa.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
