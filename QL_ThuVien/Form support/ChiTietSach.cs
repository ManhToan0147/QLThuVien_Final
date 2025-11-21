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

namespace QL_ThuVien
{
    public partial class ChiTietSach : Form
    {
        string strCon = DBConfig.ConnectionString;
        private string maDauSach;
        public ChiTietSach(string maDauSach)
        {
            InitializeComponent();
            this.maDauSach = maDauSach;
        }

        private void ChiTietSach_Load(object sender, EventArgs e)
        {
            LoadBookDetails();
        }
        public void LoadBookDetails()
        {
            DataTable bookDetails = GetBookDetails(maDauSach);
            if (bookDetails.Rows.Count > 0)
            {
                DataRow row = bookDetails.Rows[0];

                txtMaDauSach.Text = row["MaDauSach"].ToString();
                txtTenDauSach.Text = row["TenDauSach"].ToString();
                txtTenTacGia.Text = row["TenTacGia"].ToString();
                txtNamXuatBan.Text = row["NamXuatBan"].ToString();
                txtGiaBia.Text = row["GiaBia"].ToString();
                txtSoTrang.Text = row["SoTrang"].ToString();
                txtSoLuong.Text = row["SoLuong"].ToString();
                txtLoaiSach.Text = row["TenLoaiSach"].ToString();
                txtChuDe.Text = row["TenChuDe"].ToString();
                txtNXB.Text = row["TenNXB"].ToString();
                txtKhoSach.Text = row["TenKho"].ToString();
            }
            else
            {
                MessageBox.Show("Không tìm thấy thông tin chi tiết cho đầu sách này.");
            }
        }
        private DataTable GetBookDetails(string maDauSach)
        {
            DataTable dt = new DataTable();
            string sql = "SELECT * FROM ChiTiet_DauSach WHERE MaDauSach = @MaDauSach";

            using (SqlConnection con = new SqlConnection(strCon))
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter(sql, con))
                {
                    adapter.SelectCommand.Parameters.AddWithValue("@MaDauSach", maDauSach);
                    adapter.Fill(dt);
                }
            }
            return dt;
        }
    }
}
