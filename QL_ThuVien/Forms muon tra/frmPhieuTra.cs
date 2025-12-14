using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QL_ThuVien
{
    public partial class frmPhieuTra : Form
    {
        string strCon = DBConfig.ConnectionString;
        SqlConnection con;
        SqlDataAdapter adapter;
        SqlCommand cmd;
        DataTable dt;
        public string MaPhieuPhat { get; set; }
        public string MaPhieuMuon { get; set; }
        public string MaDocGia { get; set; }

        public frmPhieuTra()
        {
            InitializeComponent();
        }

        private void frmPhatSach_Load(object sender, EventArgs e)
        {
            txtMaPhieuPhat.Text = MaPhieuPhat;
            txtMaPhieuMuon.Text = MaPhieuMuon;
            txtMaDocGia.Text = MaDocGia;

            dgvViPham.DefaultCellStyle.Font = new Font(dgvViPham.Font, FontStyle.Regular);
            dgvSachPhat.DefaultCellStyle.Font = new Font(dgvSachPhat.Font, FontStyle.Regular);
            dgvSachTra.DefaultCellStyle.Font = new Font(dgvSachTra.Font, FontStyle.Regular);

            LoadViPham();
            LoadSachTra();
            LoadSachPhat();

        }

        private void LoadSachPhat()
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = $"select MaSach, MaViPham, MoTa, NopPhat" +
                    $" from CT_PhieuPhat where MaPhieuPhat = '{MaPhieuPhat}'";
                adapter = new SqlDataAdapter(sql, con);
                dt = new DataTable();
                adapter.Fill(dt);
                dgvSachPhat.DataSource = dt;
            }
        }

        private void LoadSachTra()
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "select MaSach, TinhTrangMuon, DaTraSach, TinhTrangTra, " +
                    "case when pm.HanTra < pm.NgayThucTra then DATEDIFF(day, pm.HanTra, pm.NgayThucTra) " +
                    "else 0 end as SoNgayTre, TienCoc " +
                    "from CT_PhieuMuon ct_pm join PhieuMuon pm on ct_pm.MaPhieuMuon = pm.MaPhieuMuon " +
                    $"where ct_pm.MaPhieuMuon = '{MaPhieuMuon}'";
                adapter = new SqlDataAdapter(sql, con);
                dt = new DataTable();
                adapter.Fill(dt);
                dgvSachTra.DataSource = dt;
            }
        }

        private void LoadViPham()
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "Select MaViPham, TenViPham, HinhThucPhat, LoaiTinhPhat, GiaTri from ViPham WHERE TrangThai = 1";
                adapter = new SqlDataAdapter(sql, con);
                dt = new DataTable();
                adapter.Fill(dt);
                dgvViPham.DataSource = dt;
            }
        }

        private void btnChuyenXuong_Click(object sender, EventArgs e)
        {
            if (dgvSachTra.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một sách trong bảng 'Tình trạng mượn trả'.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dgvViPham.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một vi phạm trong bảng 'Thông tin vi phạm'.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (DataGridViewRow row1 in dgvSachTra.SelectedRows)
            {
                string maSach = row1.Cells[0].Value.ToString();
                string soNgayTreStr = row1.Cells["SoNgayTre"].Value.ToString();
                string tienCocStr = row1.Cells["TienCoc"].Value.ToString();
                bool daTraSach = bool.Parse(row1.Cells["DaTraSach"].Value.ToString());

                int soNgayTre = string.IsNullOrEmpty(soNgayTreStr) ? 0 : int.Parse(soNgayTreStr);
                float tienCoc = string.IsNullOrEmpty(tienCocStr) ? 0 : float.Parse(tienCocStr);
                float giaBia = tienCoc / 2;

                foreach (DataGridViewRow row2 in dgvViPham.SelectedRows)
                {
                    string maViPham = row2.Cells["MaViPham"].Value.ToString();
                    string tenViPham = row2.Cells["TenViPham"].Value.ToString().ToLower(); // Chuyển thành chữ thường
                    string loaiTinhPhat = row2.Cells["LoaiTinhPhat"].Value?.ToString();
                    float giaTri = row2.Cells["GiaTri"].Value != DBNull.Value
                        ? Convert.ToSingle(row2.Cells["GiaTri"].Value)
                        : 0f;

                    // === KIỂM TRA TRÙNG LẶP ===
                    using (con = new SqlConnection(strCon))
                    {
                        con.Open();
                        string checkSql = @"
                            SELECT COUNT(*) 
                            FROM CT_PhieuPhat 
                            WHERE MaPhieuPhat = @MaPhieuPhat 
                              AND MaSach = @MaSach 
                              AND MaViPham = @MaViPham";

                        SqlCommand checkCmd = new SqlCommand(checkSql, con);
                        checkCmd.Parameters.AddWithValue("@MaPhieuPhat", MaPhieuPhat);
                        checkCmd.Parameters.AddWithValue("@MaSach", maSach);
                        checkCmd.Parameters.AddWithValue("@MaViPham", maViPham);

                        int count = (int)checkCmd.ExecuteScalar();

                        if (count > 0)
                        {
                            MessageBox.Show($"Sách '{maSach}' đã có vi phạm '{tenViPham}' rồi!",
                                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            continue;
                        }
                    }

                    float tienNopPhat = 0;

                    // TÍNH TIỀN PHẠT
                    if (loaiTinhPhat == "Cố định")
                    {
                        tienNopPhat = giaTri;
                    }
                    else if (loaiTinhPhat == "Theo ngày")
                    {
                        // "THEO NGÀY" BẮT BUỘC PHẢI CÓ SỐ NGÀY TRỀ
                        if (soNgayTre > 0)
                        {
                            tienNopPhat = soNgayTre * giaTri;
                        }
                        else
                        {
                            MessageBox.Show("Sách không trả trễ hạn", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            continue;
                        }
                    }
                    else if (loaiTinhPhat == "Theo giá bìa")
                    {
                        // KIỂM TRA ĐẶC THÙ: "mất sách"
                        if (tenViPham.Contains("mất sách"))
                        {
                            if (!daTraSach)
                            {
                                tienNopPhat = giaBia * giaTri;
                            }
                            else
                            {
                                MessageBox.Show("Sách đã được trả", "Thông báo",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                continue;
                            }
                        }
                        else
                        {
                            // Các vi phạm khác dùng "Theo giá bìa" (vd: hư hại)
                            tienNopPhat = giaBia * giaTri;
                        }
                    }
                    else
                    {
                        // Không có loại tính phạt → tiền = 0
                        tienNopPhat = 0;
                    }

                    // THÊM VÀO DB
                    using (con = new SqlConnection(strCon))
                    {
                        try
                        {
                            con.Open();
                            string sql = @"
                        INSERT INTO CT_PhieuPhat (MaPhieuPhat, MaSach, MaViPham, NopPhat)
                        VALUES (@MaPhieuPhat, @MaSach, @MaViPham, @NopPhat)";

                            using (SqlCommand cmd = new SqlCommand(sql, con))
                            {
                                cmd.Parameters.AddWithValue("@MaPhieuPhat", MaPhieuPhat);
                                cmd.Parameters.AddWithValue("@MaSach", maSach);
                                cmd.Parameters.AddWithValue("@MaViPham", maViPham);
                                cmd.Parameters.AddWithValue("@NopPhat", tienNopPhat);

                                cmd.ExecuteNonQuery();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Lỗi khi ghi dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            LoadSachPhat();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvSachPhat.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một bản ghi để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult confirm = MessageBox.Show("Bạn có chắc chắn muốn xóa các bản ghi đã chọn?",
                                          "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                using (SqlConnection con = new SqlConnection(strCon))
                {
                    try
                    {
                        con.Open();

                        foreach (DataGridViewRow row in dgvSachPhat.SelectedRows)
                        {
                            string maSach = row.Cells[0].Value.ToString();
                            string maViPham = row.Cells[1].Value.ToString();

                            string sql = "Delete from CT_PhieuPhat " +
                                $"where MaPhieuPhat = '{MaPhieuPhat}' and MaSach = '{maSach}' and MaViPham = '{maViPham}'";
                            cmd = new SqlCommand(sql, con);
                            cmd.ExecuteNonQuery();
                        }
                        MessageBox.Show("Đã xóa thành công.");
                        LoadSachPhat();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi xóa: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (dgvSachPhat.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để lưu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (SqlConnection con = new SqlConnection(strCon))
            {
                try
                {
                    con.Open();

                    foreach (DataGridViewRow row in dgvSachPhat.Rows)
                    {
                        if (row.IsNewRow) continue;

                        // Lấy dữ liệu từ DataGridView
                        string maSach = row.Cells[0].Value.ToString();
                        string maViPham = row.Cells[1].Value.ToString();
                        object moTa = string.IsNullOrEmpty(row.Cells["MoTa"].Value?.ToString()) ? DBNull.Value : row.Cells["MoTa"].Value; // Để NULL nếu không có dữ liệu
                        float nopPhat = row.Cells["NopPhat"].Value != null ? Convert.ToSingle(row.Cells["NopPhat"].Value) : 0f;

                        // Câu lệnh SQL để cập nhật
                        string sql = @"
                        UPDATE CT_PhieuPhat
                        SET MoTa = @MoTa, NopPhat = @NopPhat
                        WHERE MaPhieuPhat = @MaPhieuPhat AND MaSach = @MaSach AND MaViPham = @MaViPham";

                        using (SqlCommand cmd = new SqlCommand(sql, con))
                        {
                            cmd.Parameters.AddWithValue("@MaPhieuPhat", MaPhieuPhat);
                            cmd.Parameters.AddWithValue("@MaSach", maSach);
                            cmd.Parameters.AddWithValue("@MaViPham", maViPham);
                            cmd.Parameters.AddWithValue("@MoTa", moTa);
                            cmd.Parameters.AddWithValue("@NopPhat", nopPhat);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Lưu dữ liệu thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadSachPhat();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi lưu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
