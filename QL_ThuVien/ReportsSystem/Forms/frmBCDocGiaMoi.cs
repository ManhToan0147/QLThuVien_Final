using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace QL_ThuVien
{
    public partial class frmBCDocGiaMoi : Form
    {
        SqlConnection conn = new SqlConnection();
        SqlDataAdapter adapter = new SqlDataAdapter();
        DataTable dt = new DataTable();
        string sql, constr, para1;

        public frmBCDocGiaMoi()
        {
            InitializeComponent();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmBCDocGiaMoi_Load(object sender, EventArgs e)
        {
            constr = DBConfig.ConnectionString;
            conn.ConnectionString = constr;
            conn.Open();

            // Tải dữ liệu nghề nghiệp vào ComboBox
            LoadComboBox(cboNgheNghiep, "DocGia", "NgheNghiep");

            // Làm mới báo cáo
            this.reportViewer1.RefreshReport();
        }

        private void LoadComboBox(ComboBox cbo, string tableName, string NgheNghiep)
        {
            try
            {
                // Lấy dữ liệu nghề nghiệp
                SqlDataAdapter adapter = new SqlDataAdapter($"SELECT DISTINCT {NgheNghiep} FROM {tableName}", conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                // Thêm tùy chọn "Tất cả" vào đầu danh sách
                DataRow row = dt.NewRow();
                row[NgheNghiep] = "Tất cả"; // Hiển thị "Tất cả"
                dt.Rows.InsertAt(row, 0);

                // Gán dữ liệu vào ComboBox
                cbo.DataSource = dt;
                cbo.ValueMember = NgheNghiep; // Giá trị thực tế
                cbo.DisplayMember = NgheNghiep; // Hiển thị "Tất cả" và các nghề nghiệp khác
                cbo.SelectedIndex = 0; // Mặc định chọn "Tất cả"
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu ComboBox: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnInBC_Click(object sender, EventArgs e)
        {
            string ngheNghiepCondition = "";
            if (cboNgheNghiep.SelectedIndex > 0 && cboNgheNghiep.Text != "Tất cả")
            {
                ngheNghiepCondition = $" AND NgheNghiep = N'{cboNgheNghiep.Text}'";
            }

            sql = @"
                SELECT 
                    MaDocGia, 
                    HoTen, 
                    NgheNghiep, 
                    NgayCapThe,
                    NgayHanThe,
                    FORMAT(NgayCapThe, 'dd/MM/yyyy') AS NgayCapThe_Display,
                    FORMAT(NgayHanThe, 'dd/MM/yyyy') AS NgayHanThe_Display
                FROM dbo.DocGia
                WHERE NgayCapThe BETWEEN CONVERT(date, '" + dtTuNgay.Text + @"', 103) 
                                      AND CONVERT(date, '" + dtDenNgay.Text + @"', 103) "
                            + ngheNghiepCondition + @"
                ORDER BY NgayCapThe";

            adapter = new SqlDataAdapter(sql, conn);
            DataTable dtTemp = new DataTable();
            adapter.Fill(dtTemp);

            dt = new DataTable();
            dt.Columns.Add("MaDocGia", typeof(string));
            dt.Columns.Add("HoTen", typeof(string));
            dt.Columns.Add("NgheNghiep", typeof(string));
            dt.Columns.Add("NgayCapThe", typeof(string));
            dt.Columns.Add("NgayHanThe", typeof(string));

            foreach (DataRow row in dtTemp.Rows)
            {
                DataRow newRow = dt.NewRow();
                newRow["MaDocGia"] = row["MaDocGia"];
                newRow["HoTen"] = row["HoTen"];
                newRow["NgheNghiep"] = row["NgheNghiep"];
                newRow["NgayCapThe"] = row["NgayCapThe_Display"];
                newRow["NgayHanThe"] = row["NgayHanThe_Display"];
                dt.Rows.Add(newRow);
            }

            ReportDataSource reportDataSource = new ReportDataSource("DataSetDG", dt);
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(reportDataSource);
            reportViewer1.LocalReport.ReportEmbeddedResource = "QL_ThuVien.ReportsSystem.Reports.rptDSDocGiaMoi.rdlc";

            string topNgheNghiep = GetTopNgheNghiep();
            para1 = "Từ ngày " + dtTuNgay.Text + " Đến ngày " + dtDenNgay.Text;
            ReportParameter[] reportParameters = new ReportParameter[]
            {
                new ReportParameter("prThoiGian", para1),
                new ReportParameter("prNgheNghiepTop", topNgheNghiep)
            };
            reportViewer1.LocalReport.SetParameters(reportParameters);
            reportViewer1.RefreshReport();
        }

        // Hàm lấy nghề nghiệp đăng ký thẻ nhiều nhất
        private string GetTopNgheNghiep()
        {
            try
            {
                // Nếu người dùng chọn một nghề nghiệp cụ thể, hiển thị "."
                if (cboNgheNghiep.SelectedIndex > 0 && cboNgheNghiep.Text != "Tất cả")
                {
                    return ".";
                }

                // Truy vấn nghề nghiệp nhiều nhất
                string sqlTopNgheNghiep = "SELECT TOP 1 NgheNghiep, COUNT(*) AS SoLuong " +
                                          "FROM dbo.DocGia " +
                                          $"WHERE NgayCapThe BETWEEN CONVERT(date, '{dtTuNgay.Text}', 103) AND CONVERT(date, '{dtDenNgay.Text}', 103) " +
                                          "GROUP BY NgheNghiep " +
                                          "ORDER BY SoLuong DESC";

                SqlDataAdapter topAdapter = new SqlDataAdapter(sqlTopNgheNghiep, conn);
                DataTable topDt = new DataTable();
                topAdapter.Fill(topDt);

                if (topDt.Rows.Count > 0)
                {
                    string ngheNghiep = topDt.Rows[0]["NgheNghiep"].ToString();
                    int soLuong = Convert.ToInt32(topDt.Rows[0]["SoLuong"]);
                    return $"Đối tượng đăng ký thẻ độc giả mới nhiều nhất: {ngheNghiep} ({soLuong} người)";
                }
                else
                {
                    return "Không có dữ liệu";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi lấy nghề nghiệp nhiều nhất: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "Lỗi";
            }
        }
    }
}
