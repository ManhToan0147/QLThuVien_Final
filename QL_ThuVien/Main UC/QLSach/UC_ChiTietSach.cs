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

namespace QL_ThuVien.Main_UC.QLSach
{
    public partial class UC_ChiTietSach : UserControl
    {
        string strCon = DBConfig.ConnectionString;
        SqlConnection con;
        SqlDataAdapter adapter;
        DataTable dt;
        DataView dv;
        public UC_ChiTietSach()
        {
            InitializeComponent();
        }

        private void ShowChiTietDauSach()
        {
            using (con = new SqlConnection(strCon))
            {
                con.Open();
                string sql = "Select * from ChiTiet_DauSach";
                adapter = new SqlDataAdapter(sql, con);
                dt = new DataTable();
                adapter.Fill(dt);
                dv = new DataView(dt);
                dgvChiTietDauSach.DataSource = dv;
            }
        }

        private void UC_ChiTietSach_Load(object sender, EventArgs e)
        {
            dgvChiTietDauSach.ColumnHeadersDefaultCellStyle.Font = new Font(dgvChiTietDauSach.Font, FontStyle.Bold);
            ShowChiTietDauSach();
            LoadComboBox(cboLoaiSach, "LoaiSach", "TenLoaiSach");
            LoadComboBox(cboChuDe, "ChuDe", "TenChuDe");
            LoadComboBox(cboNXB, "NXB", "TenNXB");
            LoadComboBox(cboKho, "KhoSach", "TenKho");
            LoadComboBox(cboTacGia, "TacGia", "TenTG");
            // Đăng ký sự kiện SelectedIndexChanged cho ComboBox và TextChanged cho TextBox
            cboTacGia.SelectedIndexChanged += FilterData;
            cboLoaiSach.SelectedIndexChanged += FilterData;
            cboChuDe.SelectedIndexChanged += FilterData;
            cboNXB.SelectedIndexChanged += FilterData;
            cboKho.SelectedIndexChanged += FilterData;
            txtSearch.TextChanged += FilterData;
        }

        private void FilterData(object sender, EventArgs e)
        {
            string tacGia = cboTacGia.Text == "Tất cả" ? "" : cboTacGia.Text;
            string loaiSach = cboLoaiSach.Text == "Tất cả" ? "" : cboLoaiSach.Text;
            string chuDe = cboChuDe.Text == "Tất cả" ? "" : cboChuDe.Text;
            string nxb = cboNXB.Text == "Tất cả" ? "" : cboNXB.Text;
            string kho = cboKho.Text == "Tất cả" ? "" : cboKho.Text;
            string search = txtSearch.Text;

            // Xây dựng bộ lọc
            string filter = "";
            if (!string.IsNullOrEmpty(loaiSach))
                filter += $"TenLoaiSach = '{loaiSach}'";
            if (!string.IsNullOrEmpty(tacGia))
                filter += (filter == "" ? "" : " AND ") + $"TenTacGia LIKE '%{tacGia}%'";
            if (!string.IsNullOrEmpty(chuDe))
                filter += (filter == "" ? "" : " AND ") + $"TenChuDe = '{chuDe}'";
            if (!string.IsNullOrEmpty(nxb))
                filter += (filter == "" ? "" : " AND ") + $"TenNXB = '{nxb}'";
            if (!string.IsNullOrEmpty(kho))
                filter += (filter == "" ? "" : " AND ") + $"TenKho = '{kho}'";
            if (!string.IsNullOrEmpty(search))
                filter += (filter == "" ? "" : " AND ") + $"TenDauSach LIKE '%{search}%'";

            // Áp dụng bộ lọc vào DataView
            dv.RowFilter = filter;
        }

        private void LoadComboBox(ComboBox cbo, string tableName, string columnName)
        {
            try
            {
                using (con = new SqlConnection(strCon))
                {
                    con.Open();
                    string sql = $"SELECT {columnName} FROM {tableName}";
                    SqlCommand cmd = new SqlCommand(sql, con);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Thêm tùy chọn "Tất cả" vào DataTable
                    DataRow allRow = dt.NewRow();
                    allRow[columnName] = "Tất cả";
                    dt.Rows.InsertAt(allRow, 0); // Đưa vào đầu danh sách

                    // Gán dữ liệu vào ComboBox
                    cbo.DataSource = dt;
                    cbo.DisplayMember = columnName; // Hiển thị tên
                    cbo.SelectedIndex = 0; // Mặc định chọn "Tất cả"
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối: " + ex.Message);
            }
        }

        private void btnXemChiTiet_Click(object sender, EventArgs e)
        {
            if (dgvChiTietDauSach.SelectedRows.Count > 0) 
            {
                DataGridViewRow selectedRow = dgvChiTietDauSach.SelectedRows[0];
                string maDauSach;
                if (selectedRow.Cells["MaDauSach"].Value != null)
                {
                    maDauSach = selectedRow.Cells["MaDauSach"].Value.ToString();
                    // Khởi tạo form chi tiết và truyền mã đầu sách qua constructor
                    ChiTietSach cts = new ChiTietSach(maDauSach);
                    cts.ShowDialog(); // Hiển thị form chi tiết dưới dạng dialog
                }
                else
                {
                    MessageBox.Show("Dòng được chọn không chứa thông tin.");
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một đầu sách để xem chi tiết.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (dgvChiTietDauSach == null)
            {
                return;
            }
            txtSearch.Text = string.Empty;
            cboChuDe.SelectedIndex = 0;
            cboLoaiSach.SelectedIndex = 0;
            cboTacGia.SelectedIndex = 0;
            cboNXB.SelectedIndex = 0;
            cboKho.SelectedIndex = 0;
            dv.RowFilter = null;
        }
    }
}
