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
    public partial class frmSignIn : Form
    {
        private string connectionString = DBConfig.ConnectionString;
        private int loginAttempts = 0;
        private const int MAX_ATTEMPTS = 3;

        public frmSignIn()
        {
            InitializeComponent();
        }

        int count = 0;
        private void btnSignIn_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string matKhau = txtMatKhau.Text.Trim();

            //Kiểm tra input trống
            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Vui lòng nhập email!", "Thiếu thông tin",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return;
            }

            if (string.IsNullOrEmpty(matKhau))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu!", "Thiếu thông tin",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMatKhau.Focus();
                return;
            }

            if (AuthenticateUser(email, matKhau))
            {
                bool rememberMe = chkRememberMe.Checked;
                SessionManager.SaveSession(rememberMe);

                MessageBox.Show("Đăng nhập thành công!", "Thành công",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);

                // ✅ Mở Form1 và đóng form đăng nhập
                frmLayout mainForm = new frmLayout();
                mainForm.Show();
                this.Hide();
            }
            else
            {
                loginAttempts++;
                int remainingAttempts = MAX_ATTEMPTS - loginAttempts;

                if (remainingAttempts > 0)
                {
                    MessageBox.Show($"Email hoặc mật khẩu không đúng!\nCòn lại {remainingAttempts} lần thử.",
                                  "Đăng nhập thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // ✅ Clear password và focus lại
                    txtMatKhau.Clear();
                    txtEmail.Focus();
                }
                else
                {
                    MessageBox.Show("Bạn đã nhập sai quá 3 lần. Ứng dụng sẽ đóng!",
                                  "Hết lần thử", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Application.Exit();
                }
            }
        }

        private bool AuthenticateUser(string email, string matKhau)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"SELECT MaNguoiDung, TenDangNhap, Quyen 
                                   FROM TaiKhoanDN 
                                   WHERE Email = @Email AND MatKhau = @MatKhau";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@MatKhau", matKhau);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // ✅ Lưu thông tin user vào session (static class)
                                UserSession.UserId = reader["MaNguoiDung"].ToString();
                                UserSession.UserName = reader["TenDangNhap"].ToString();
                                UserSession.UserRole = reader["Quyen"].ToString();
                                UserSession.LoginTime = DateTime.Now;
                                UserSession.UserEmail = email;
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi kết nối: {ex.Message}", "Lỗi",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }

        private void btnTogglePassword_Click(object sender, EventArgs e)
        {
            if (txtMatKhau.PasswordChar == '*')
            {
                txtMatKhau.PasswordChar = '\0'; // Hiển thị ký tự bình thường
                btnTogglePassword.Image = Properties.Resources.eye__1_; // Đổi icon thành mắt mở
            }
            else
            {
                txtMatKhau.PasswordChar = '*'; // Ẩn ký tự bằng '*'
                btnTogglePassword.Image = Properties.Resources.eye_crossed__1_; // Đổi icon thành mắt gạch chéo
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult rs = MessageBox.Show("Bạn có muốn thoát khỏi hệ thống không", "Xác nhận", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (rs == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}
