using QL_ThuVien.User_Management;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QL_ThuVien.Support_UC
{
    public partial class UC_ThongTinCaNhan : UserControl
    {
        private string connectionString = DBConfig.ConnectionString;
        private bool isEditMode = false;

        private string currentUserId;
        private string currentUserRole;

        public UC_ThongTinCaNhan()
        {
            InitializeComponent();
        }

        private void UC_ThongTinCaNhan_Load(object sender, EventArgs e)
        {
            currentUserId = UserSession.UserId;
            currentUserRole = UserSession.UserRole;
            LoadThuThu();

            // ✅ Đặt View Mode
            SetViewMode();

        }

        private void LoadThuThu()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string sql = @"
                        SELECT MaThuThu, TenThuThu, Email, SDT, HinhAnh
                        FROM ThuThu
                        WHERE MaThuThu = @MaThuThu";

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@MaThuThu", currentUserId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // ✅ Cấu hình UI cho Thủ thư
                                lblMa.Text = "Mã thủ thư:";

                                // ✅ Fill dữ liệu
                                txtMaNguoiDung.Text = reader["MaThuThu"].ToString();
                                txtHoTen.Text = reader["TenThuThu"].ToString();
                                txtEmail.Text = reader["Email"].ToString();
                                txtSDT.Text = reader["SDT"].ToString();

                                // ✅ Load ảnh
                                LoadImageFromDB(reader["HinhAnh"]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi load thông tin thủ thư: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============================================
        // LOAD ẢNH TỪ DATABASE
        // ============================================
        private void LoadImageFromDB(object imageData)
        {
            if (imageData != null && imageData != DBNull.Value)
            {
                byte[] bytes = (byte[])imageData;
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    picAvatar.Image = Image.FromStream(ms);
                }
            }
            else
            {
                picAvatar.Image = Properties.Resources.avatar_default;
            }
        }

        private byte[] ImageToByteArray(PictureBox pictureBox)
        {
            if (pictureBox.Image == null)
            {
                return null;
            }

            using (MemoryStream ms = new MemoryStream())
            {
                Image img = new Bitmap(pictureBox.Image);
                img.Save(ms, pictureBox.Image.RawFormat);
                return ms.ToArray();
            }
        }


        // ============================================
        // VIEW MODE
        // ============================================
        private void SetViewMode()
        {
            isEditMode = false;

            // ✅ Ẩn buttons
            btnHuy.Visible = false;
            btnLuu.Visible = false;
            btnBrowse.Visible = false;
            btnXoaAnh.Visible = false;

            // ✅ Set textbox read-only
            SetTextBoxReadOnly(txtMaNguoiDung, true);
            SetTextBoxReadOnly(txtHoTen, true);
            SetTextBoxReadOnly(txtEmail, true);
            SetTextBoxReadOnly(txtSDT, true);
        }

        // ============================================
        // EDIT MODE
        // ============================================
        private void SetEditMode()
        {
            isEditMode = true;

            // ✅ Hiện buttons
            btnHuy.Visible = true;
            btnLuu.Visible = true;
            btnBrowse.Visible = true;
            btnXoaAnh.Visible = true;

            // ✅ Set textbox editable (trừ Mã)
            SetTextBoxReadOnly(txtMaNguoiDung, true);  // Mã không cho sửa
            SetTextBoxReadOnly(txtHoTen, false);
            SetTextBoxReadOnly(txtEmail, false);
            SetTextBoxReadOnly(txtSDT, false);

            // ✅ Focus ô đầu tiên
            txtHoTen.Focus();
        }

        // ============================================
        // SET TEXTBOX STYLE
        // ============================================
        private void SetTextBoxReadOnly(Guna.UI2.WinForms.Guna2TextBox textBox, bool isReadOnly)
        {
            if (textBox == null) return;

            if (isReadOnly)
            {
                textBox.ReadOnly = true;
                textBox.FillColor = Color.AliceBlue;
                textBox.BorderThickness = 0;
                panelThongTin.BorderStyle = BorderStyle.None;
            }
            else
            {
                textBox.ReadOnly = false;
                textBox.FillColor = Color.White;
                textBox.BorderThickness = 1;
                panelThongTin.BorderStyle = BorderStyle.FixedSingle;
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            SetEditMode();
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Bạn có chắc muốn hủy các thay đổi?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                LoadThuThu();
                SetViewMode();
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            // ✅ Lưu theo role
            bool success = false;
            success = SaveThuThu();

            if (success)
            {
                MessageBox.Show("Lưu thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetViewMode();
            }
        }

        // ============================================
        // LƯU THÔNG TIN THỦ THƯ
        // ============================================
        private bool SaveThuThu()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    byte[] imageData = ImageToByteArray(picAvatar);

                    string sql = @"
                        UPDATE ThuThu 
                        SET TenThuThu = @TenThuThu, 
                            Email = @Email, 
                            SDT = @SDT, 
                            HinhAnh = @HinhAnh
                        WHERE MaThuThu = @MaThuThu";

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@TenThuThu", txtHoTen.Text.Trim());
                        cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                        cmd.Parameters.AddWithValue("@SDT", txtSDT.Text.Trim());
                        cmd.Parameters.Add("@HinhAnh", SqlDbType.VarBinary).Value = imageData ?? (object)DBNull.Value;
                        cmd.Parameters.AddWithValue("@MaThuThu", currentUserId);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            // ✅ CHỈ CẬP NHẬT EMAIL
                            UserSession.UserEmail = txtEmail.Text.Trim();
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi lưu dữ liệu: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }


        private void btnBrowse_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All Files|*.*";
            openFile.Title = "Chọn ảnh";

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                picAvatar.Image = Image.FromFile(openFile.FileName);
            }
        }

        private void btnXoaAnh_Click(object sender, EventArgs e)
        {
            picAvatar.Image = Properties.Resources.avatar_default;
        }
    }
}
