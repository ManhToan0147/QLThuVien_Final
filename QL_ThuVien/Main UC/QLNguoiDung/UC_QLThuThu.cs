using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QL_ThuVien.Main_UC.QLMuonTra
{
    public partial class UC_QLThuThu : UserControl
    {
        string strCon = DBConfig.ConnectionString;
        SqlConnection con;
        SqlDataAdapter adpater;
        SqlCommand cmd;
        DataTable dt;
        DataView dv;
        public UC_QLThuThu()
        {
            InitializeComponent();
        }

        private void UC_QLThuThu_Load(object sender, EventArgs e)
        {
            dgvThuThu.ColumnHeadersDefaultCellStyle.Font = new Font(dgvThuThu.Font, FontStyle.Bold);
            cboTruong.SelectedIndex = 0;
            LoadThuThu();
        }

        private void LoadThuThu()
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "Select * from ThuThu";
                adpater = new SqlDataAdapter(sql, con);
                dt = new DataTable();
                adpater.Fill(dt);
                dv = new DataView(dt);
                dgvThuThu.DataSource = dv;
                dgvThuThu.Columns["HinhAnh"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                foreach (DataGridViewRow row in dgvThuThu.Rows)
                {
                    if (row.Cells["HinhAnh"].Value == DBNull.Value)
                    {
                        row.Cells["HinhAnh"].Value = Properties.Resources.avatar_default;
                    }
                }
                ((DataGridViewImageColumn)dgvThuThu.Columns["HinhAnh"]).ImageLayout = DataGridViewImageCellLayout.Zoom;
            }
        }

        private void dgvThuThu_SelectionChanged(object sender, EventArgs e)
        {
            NapCT();
        }

        string selectedMaThuThu;
        private void NapCT()
        {
            if (dgvThuThu.CurrentCell != null && dgvThuThu.CurrentCell.RowIndex >= 0)
            {
                //Xu ly chu
                int i = dgvThuThu.CurrentRow.Index;
                selectedMaThuThu = dgvThuThu.Rows[i].Cells["MaThuThu"]?.Value.ToString() ?? string.Empty;
                txtMaThuThu.Text = selectedMaThuThu;
                txtMaThuThu.Enabled = string.IsNullOrEmpty(txtMaThuThu.Text);

                txtTenThuThu.Text = dgvThuThu.Rows[i].Cells["TenThuThu"]?.Value.ToString() ?? string.Empty;
                txtEmail.Text = dgvThuThu.Rows[i].Cells["Email"]?.Value.ToString() ?? string.Empty;
                txtSDT.Text = dgvThuThu.Rows[i].Cells["SDT"]?.Value.ToString() ?? string.Empty;
                LoadImage(selectedMaThuThu);
            }
        }

        private void LoadImage(string maThuThu)
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "Select HinhAnh from ThuThu where MaThuThu = @MaTheMuon";
                using (cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@MaTheMuon", maThuThu);

                    object imageData = cmd.ExecuteScalar();
                    if (imageData != null && imageData != DBNull.Value)
                    {
                        // Có dữ liệu ảnh, chuyển đổi từ byte array sang Image
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
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All Files|*.*";
            openFile.Title = "Chọn ảnh";

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                picAvatar.Image = Image.FromFile(openFile.FileName);
            }
        }
        public byte[] ImageToByteArray(PictureBox pictureBox)
        {
            if (pictureBox.Image == null)
            {
                return null; // Hoặc trả về một mảng byte mặc định nếu cần
            }

            using (MemoryStream ms = new MemoryStream())
            {
                Image img = new Bitmap(pictureBox.Image);
                img.Save(ms, pictureBox.Image.RawFormat);
                return ms.ToArray();
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedMaThuThu))
            {
                MessageBox.Show("Chưa chọn bản ghi để sửa", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            int currentIndex = dgvThuThu.CurrentRow.Index;

            using (SqlConnection con = new SqlConnection(strCon))
            {
                con.Open();
                byte[] imageData = ImageToByteArray(picAvatar);

                string sql = "UPDATE ThuThu SET TenThuThu = @TenThuThu, Email = @Email, " +
                             "SDT = @SDT, HinhAnh = @HinhAnh " +
                             "WHERE MaThuThu = @MaThuThu";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@TenThuThu", txtTenThuThu.Text);
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@SDT", txtSDT.Text);

                    // Xử lý ảnh (nếu không có ảnh thì gán null)
                    cmd.Parameters.Add("@HinhAnh", SqlDbType.VarBinary).Value = imageData ?? (object)DBNull.Value;

                    cmd.Parameters.AddWithValue("@MaThuThu", selectedMaThuThu);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Cập nhật thông tin thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Cập nhật thất bại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            LoadThuThu();
            dgvThuThu.ClearSelection();
            dgvThuThu.CurrentCell = dgvThuThu.Rows[currentIndex].Cells[0];//Đặt cell đầu tiên là cell của dòng sửa
            NapCT();
            dgvThuThu.FirstDisplayedScrollingRowIndex = currentIndex; //Cuộn đến dòng đó
        }

        private void btnTaoMoi_Click(object sender, EventArgs e)
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "Select max(MaThuThu) from ThuThu";
                cmd = new SqlCommand(sql, con);
                object rs = cmd.ExecuteScalar();
                if (rs != DBNull.Value && rs != null)
                {
                    string maThuThu = rs.ToString();
                    int number = int.Parse(maThuThu.Substring(2)); //Lấy số sau phần "TT"
                    ++number;
                    txtMaThuThu.Text = "TT" + number.ToString("D2"); // Dam bao co 2 chu so (01 thay vi 1)
                }
                else
                {
                    //TH không có bản ghi nào thì bắt đầu từ 01
                    txtMaThuThu.Text = "TT01";
                }
            }

            txtTenThuThu.Text = "";
            txtSDT.Text = "";
            txtEmail.Text = "";
            picAvatar.Image = null;
            txtTenThuThu.Focus();
        }

        private void btnXoaAnh_Click(object sender, EventArgs e)
        {
            picAvatar.Image = null;
        }

        private void cboTruong_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSearch.Clear();
            if (cboTruong.SelectedIndex == 0)
            {
                txtSearch.PlaceholderText = "Nhập tên thủ thư để tìm kiếm";
            }
            else
            {
                txtSearch.PlaceholderText = "Nhập mã thủ thư để tìm kiếm";
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (cboTruong.SelectedIndex == 0)
            {
                dv.RowFilter = $"TenThuThu like '%{txtSearch.Text}%'";
            }
            else
            {
                dv.RowFilter = $"MaThuThu like '%{txtSearch.Text}%'";
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                using (con = new SqlConnection(strCon))
                {
                    byte[] imageData = ImageToByteArray(picAvatar);
                    con.Open();

                    string sql = "Insert into ThuThu " +
                    "values (@MaThuThu, @TenThuThu, @HinhAnh, @Email, @SDT) ";
                    cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@MaThuThu", txtMaThuThu.Text.Trim());
                    cmd.Parameters.AddWithValue("@TenThuThu", txtTenThuThu.Text.Trim());
                    cmd.Parameters.Add("@HinhAnh", SqlDbType.Image).Value = imageData ?? (object)DBNull.Value;
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@SDT", txtSDT.Text.Trim());

                    int kq = cmd.ExecuteNonQuery();
                    if (kq > 0)
                    {
                        MessageBox.Show("Thêm bản ghi thành công", "Thông báo", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Thêm bản ghi thất bại", "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm " + ex.Message);
            }
            LoadThuThu();
            int lastIndex = dgvThuThu.RowCount - 1;
            dgvThuThu.ClearSelection();
            dgvThuThu.CurrentCell = dgvThuThu.Rows[lastIndex].Cells[0];//Đặt cell đầu tiên là cell của dòng sửa
            NapCT();
            dgvThuThu.FirstDisplayedScrollingRowIndex = lastIndex; //Cuộn đến dòng đó
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedMaThuThu))
            {
                MessageBox.Show("Chưa chọn bản ghi để xóa", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int currentIndex = dgvThuThu.CurrentRow.Index;

            DialogResult rs = MessageBox.Show("Bạn có chắc chắn muốn xóa bản ghi đã chọn và các bản ghi khác liên quan?",
                   "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (rs == DialogResult.Yes)
            {
                try
                {
                    using (con = new SqlConnection(strCon))
                    {
                        con.Open();
                        string sql = $"Delete from ThuThu where MaThuThu = '{selectedMaThuThu}'";
                        cmd = new SqlCommand(sql, con);
                        int kq = cmd.ExecuteNonQuery();
                        if (kq > 0)
                        {
                            MessageBox.Show("Xóa bản ghi thành công",
                            "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Xóa bản ghi thất bại",
                            "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa " + ex.Message);
                }
            }
            LoadThuThu();
            int beforeRowIndex = currentIndex - 1;
            dgvThuThu.ClearSelection();
            dgvThuThu.CurrentCell = dgvThuThu.Rows[beforeRowIndex].Cells[0];//Đặt cell đầu tiên là cell của dòng sửa
            NapCT();
            dgvThuThu.FirstDisplayedScrollingRowIndex = beforeRowIndex; //Cuộn đến dòng đó
        }

        private void txtEmail_Leave(object sender, EventArgs e)
        {
            if (!txtEmail.Text.Contains("@") || !txtEmail.Text.Contains("."))
            {
                errorProvider1.SetError(txtEmail, "Email không hợp lệ! Phải chứa '@' và '.'");
            }
            else
            {
                errorProvider1.SetError(txtEmail, ""); // Xóa lỗi nếu hợp lệ
            }
        }

        private void txtSDT_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                errorProvider1.SetError(txtSDT, "Chỉ được nhập số!");
            }
            else
            {
                errorProvider1.SetError(txtSDT, ""); // Xóa lỗi nếu hợp lệ
            }
        }

        private void txtSDT_Leave(object sender, EventArgs e)
        {
            if (txtSDT.Text.Length < 10 || txtSDT.Text.Length > 11)
            {
                errorProvider1.SetError(txtSDT, "Số điện thoại phải có 10-11 chữ số!");
            }
            else
            {
                errorProvider1.SetError(txtSDT, ""); // Xóa lỗi nếu hợp lệ
            }
        }
    }
}
