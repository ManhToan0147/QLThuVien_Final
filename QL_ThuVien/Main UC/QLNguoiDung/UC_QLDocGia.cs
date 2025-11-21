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

namespace QL_ThuVien.Main_UC.QLDocGia
{
    public partial class UC_QLDocGia : UserControl
    {
        string strCon = DBConfig.ConnectionString;
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter adapter;
        DataTable dt;
        DataView dv;
        public UC_QLDocGia()
        {
            InitializeComponent();
        }

        private void UC_QLDocGia_Load(object sender, EventArgs e)
        {
            dgvDocGia.ColumnHeadersDefaultCellStyle.Font = new Font(dgvDocGia.Font, FontStyle.Bold);
            cboTruong.SelectedIndex = 0;
            LoadDocGia();
        }
        private void LoadDocGia()
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "Select * from DocGia";
                adapter = new SqlDataAdapter(sql, con);
                dt = new DataTable();
                adapter.Fill(dt);
                dv = new DataView(dt);
                dgvDocGia.DataSource = dv;
                dgvDocGia.Columns["HinhAnh"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                //DB chưa có ảnh thì để ảnh mặc định
                foreach (DataGridViewRow row in dgvDocGia.Rows)
                {
                    if (row.Cells["HinhAnh"].Value == DBNull.Value)
                    {
                        row.Cells["HinhAnh"].Value = Properties.Resources.avatar_default;
                    }
                }
                ((DataGridViewImageColumn)dgvDocGia.Columns["HinhAnh"]).ImageLayout = DataGridViewImageCellLayout.Zoom;
            }
        }

        private void cboTruong_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSearch.Clear();
            if (cboTruong.SelectedIndex == 0)
            {
                txtSearch.PlaceholderText = "Nhập tên độc giả để tìm kiếm";
            }
            else
            {
                txtSearch.PlaceholderText = "Nhập mã độc giả để tìm kiếm";
            }
        }

        private void dgvDocGia_SelectionChanged(object sender, EventArgs e)
        {
            NapCT();
        }
        string selectedMaDocGia;
        private void NapCT()
        {
            if (dgvDocGia.CurrentCell != null && dgvDocGia.CurrentCell.RowIndex >= 0)
            {
                //Xu ly chu
                int i = dgvDocGia.CurrentRow.Index;
                selectedMaDocGia = dgvDocGia.Rows[i].Cells["MaDocGia"]?.Value.ToString() ?? string.Empty;
                txtMaTheMuon.Text = selectedMaDocGia;
                txtMaTheMuon.Enabled = string.IsNullOrEmpty(txtMaTheMuon.Text);

                txtHoTen.Text = dgvDocGia.Rows[i].Cells["HoTen"].Value.ToString();
                txtEmail.Text = dgvDocGia.Rows[i].Cells["Email"].Value.ToString();
                txtSDT.Text = dgvDocGia.Rows[i].Cells["SoDienThoai"].Value.ToString();
                txtNgheNghiep.Text = dgvDocGia.Rows[i].Cells["NgheNghiep"].Value.ToString();

                dateNgayCap.Value = dgvDocGia.Rows[i].Cells["NgayCapThe"].Value is DBNull ? DateTime.Now : (DateTime)dgvDocGia.Rows[i].Cells["NgayCapThe"].Value;
                dateNgayHan.Value = dgvDocGia.Rows[i].Cells["NgayHanThe"].Value is DBNull ? DateTime.Now : (DateTime)dgvDocGia.Rows[i].Cells["NgayHanThe"].Value;

                LoadImage(selectedMaDocGia);
            }
        }
        private void LoadImage(string maDocGia)
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "Select HinhAnh from DocGia where MaDocGia = @MaDocGia";
                using (cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@MaDocGia", maDocGia);

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
            if (string.IsNullOrEmpty(selectedMaDocGia))
            {
                MessageBox.Show("Chưa chọn bản ghi để sửa", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            int currentIndex = dgvDocGia.CurrentRow.Index;
            using (SqlConnection con = new SqlConnection(strCon))
            {
                con.Open();

                byte[] imageData = ImageToByteArray(picAvatar);
                string ngayCap = dateNgayCap.Value.ToString("yyyy-MM-dd");
                string ngayHan = dateNgayHan.Value.ToString("yyyy-MM-dd");

                string sql = "UPDATE DocGia SET HoTen = @HoTen, Email = @Email, SoDienThoai = @SoDienThoai, NgheNghiep = @NgheNghiep, " +
                             "NgayCapThe = @NgayCapThe, NgayHanThe = @NgayHanThe, HinhAnh = @HinhAnh " +
                             "WHERE MaDocGia = @MaDocGia";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    // Thêm các tham số cho câu lệnh SQL
                    cmd.Parameters.AddWithValue("@HoTen", txtHoTen.Text);
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@SoDienThoai", txtSDT.Text);
                    cmd.Parameters.AddWithValue("@NgheNghiep", txtNgheNghiep.Text);
                    cmd.Parameters.AddWithValue("@NgayCapThe", ngayCap);
                    cmd.Parameters.AddWithValue("@NgayHanThe", ngayHan);

                    // Xử lý ảnh (nếu không có ảnh thì gán null)
                    cmd.Parameters.Add("@HinhAnh", SqlDbType.VarBinary).Value = imageData ?? (object)DBNull.Value;

                    cmd.Parameters.AddWithValue("@MaDocGia", selectedMaDocGia);

                    // Thực thi câu lệnh
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
            LoadDocGia();
            dgvDocGia.ClearSelection();
            dgvDocGia.CurrentCell = dgvDocGia.Rows[currentIndex].Cells[0];
            NapCT();
            dgvDocGia.FirstDisplayedScrollingRowIndex = currentIndex;
        }

        private void btnTaoMoi_Click(object sender, EventArgs e)
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "Select max(MaDocGia) from DocGia";
                cmd = new SqlCommand(sql, con);
                object rs = cmd.ExecuteScalar();
                if (rs != DBNull.Value && rs != null)
                {
                    string maTheMuon = rs.ToString();
                    int number = int.Parse(maTheMuon.Substring(2)); //Lấy sau phầm "DG"
                    ++number;
                    txtMaTheMuon.Text = "DG" + number.ToString("D3"); 
                }
            }

            txtHoTen.Text = "";
            txtSDT.Text = "";
            txtEmail.Text = "";
            txtNgheNghiep.Text = "";
            dateNgayCap.Value = DateTime.Now;
            dateNgayHan.Value = DateTime.Now;
            txtMaTheMuon.Focus();
            picAvatar.Image = null;
            txtHoTen.Focus();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                using (con = new SqlConnection(strCon))
                {
                    con.Open();
                    byte[] imageData = ImageToByteArray(picAvatar);
                    string ngayCap = dateNgayCap.Value.ToString("yyyy-MM-dd");
                    string ngayHan = dateNgayHan.Value.ToString("yyyy-MM-dd");

                    string sql = "Insert into DocGia " +
                    "Values (@MaDocGia, @HoTen, @HinhAnh,@Email, @SoDienThoai, @NgheNghiep, @NgayCapThe, @NgayHanThe)";
                    using (cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@MaDocGia", txtMaTheMuon.Text.Trim());
                        cmd.Parameters.AddWithValue("@HoTen", txtHoTen.Text.Trim());
                        cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                        cmd.Parameters.AddWithValue("@SoDienThoai", txtSDT.Text.Trim());
                        cmd.Parameters.AddWithValue("@NgheNghiep", txtNgheNghiep.Text.Trim());
                        cmd.Parameters.AddWithValue("@NgayCapThe", ngayCap);
                        cmd.Parameters.AddWithValue("@NgayHanThe", ngayHan);

                        cmd.Parameters.Add("@HinhAnh", SqlDbType.VarBinary).Value = imageData ?? (object)DBNull.Value;

                        // Thực thi câu lệnh

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Thêm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Thêm thất bại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                LoadDocGia();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm " + ex.Message);
            }
            int lastRowIndex = dgvDocGia.RowCount - 1;
            dgvDocGia.ClearSelection();
            dgvDocGia.CurrentCell = dgvDocGia.Rows[lastRowIndex].Cells[0];
            NapCT();
            dgvDocGia.FirstDisplayedScrollingRowIndex = lastRowIndex;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (cboTruong.SelectedIndex == 0)
            {
                dv.RowFilter = $"HoTen like '%{txtSearch.Text}%'";
            }
            else
            {
                dv.RowFilter = $"MaDocGia like '%{txtSearch.Text}%'";
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedMaDocGia))
            {
                MessageBox.Show("Chưa chọn bản ghi để xóa", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int currentIndex = dgvDocGia.CurrentRow.Index;

            DialogResult rs = MessageBox.Show("Bạn có chắc chắn muốn xóa bản ghi đã chọn",
                   "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (rs == DialogResult.Yes)
            {
                using (con = new SqlConnection(strCon))
                {
                    con.Open();
                    cmd = new SqlCommand();
                    cmd.Connection = con;
                    string sql = $"Delete from DocGia where MaDocGia = '{selectedMaDocGia}'";
                    cmd.CommandText = sql;
                    try
                    {
                        int kq = cmd.ExecuteNonQuery();
                        if (kq > 0)
                        {
                            MessageBox.Show("Xóa bản ghi thành công",
                            "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        } else
                        {
                            MessageBox.Show("Xóa bản ghi thất bại", 
                            "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi xóa " + ex.Message);
                    }
                }
            }
            LoadDocGia();
            int beforeRowIndex = currentIndex - 1;
            dgvDocGia.ClearSelection();
            dgvDocGia.CurrentCell = dgvDocGia.Rows[beforeRowIndex].Cells[0];
            NapCT();
            dgvDocGia.FirstDisplayedScrollingRowIndex = beforeRowIndex;
        }

        private void btnXoaAnh_Click(object sender, EventArgs e)
        {
            picAvatar.Image = null;
        }

        private void txtSDT_KeyPress(object sender, KeyPressEventArgs e)
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
    }
}
