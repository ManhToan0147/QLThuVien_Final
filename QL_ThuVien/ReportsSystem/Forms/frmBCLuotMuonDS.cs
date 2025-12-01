using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

namespace QL_ThuVien
{
    public partial class frmBCLuotMuonDS : Form
    {
        SqlConnection conn = new SqlConnection();
        SqlDataAdapter adapter = new SqlDataAdapter();
        DataTable dt = new DataTable();
        string sql, constr, para1;

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnInBC_Click(object sender, EventArgs e)
        {
            sql = @"
                SELECT 
                    cd.MaChuDe,
                    cd.TenChuDe, 
                    ds.MaDauSach, 
                    ds.TenDauSach,
                    NULL AS MaPhieuMuon,
                    NULL AS NgayMuon,
                    NULL AS MaDocGia,
                    ISNULL(COUNT(DISTINCT pm.MaPhieuMuon), 0) AS SoLuotMuon
                FROM DauSach AS ds
                JOIN ChuDe AS cd ON ds.MaChuDe = cd.MaChuDe
                LEFT JOIN CuonSach AS cs ON ds.MaDauSach = cs.MaDauSach
                LEFT JOIN CT_PhieuMuon AS ct ON cs.MaSach = ct.MaSach
                LEFT JOIN PhieuMuon AS pm ON ct.MaPhieuMuon = pm.MaPhieuMuon
                    AND pm.NgayMuon BETWEEN CONVERT(date, '" + dtTuNgay.Text + @"', 103) 
                                         AND CONVERT(date, '" + dtDenNgay.Text + @"', 103)
                GROUP BY cd.MaChuDe, cd.TenChuDe, ds.MaDauSach, ds.TenDauSach
                ORDER BY cd.TenChuDe, SoLuotMuon DESC, ds.MaDauSach";

            adapter = new SqlDataAdapter(sql, conn);
            dt = new DataTable();
            adapter.Fill(dt);

            ReportDataSource reportDataSource = new ReportDataSource("DataSetLuotMuonDS", dt);
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(reportDataSource);
            reportViewer1.LocalReport.ReportEmbeddedResource = "QL_ThuVien.ReportsSystem.Reports.rptLuotMuonDS.rdlc";

            para1 = "Từ ngày " + dtTuNgay.Text + " Đến ngày " + dtDenNgay.Text;
            ReportParameter[] reportParameters = new ReportParameter[]
            {
                new ReportParameter("prThoiGian", para1),
                new ReportParameter("prTopChuDeNhieu", GetTopResult("Ba chủ đề có lượt mượn nhiều nhất", GetData(GetSqlTopChuDeNhieu()))),
                new ReportParameter("prTopChuDeIt", GetTopResult("Ba chủ đề có lượt mượn ít nhất", GetData(GetSqlTopChuDeIt()))),
                new ReportParameter("prTopDauSachNhieu", GetTopResult("Ba đầu sách có lượt mượn nhiều nhất", GetData(GetSqlTopDauSachNhieu()))),
                new ReportParameter("prTopDauSachIt", GetTopResult("Ba đầu sách có lượt mượn ít nhất", GetData(GetSqlTopDauSachIt()))),
            };

            reportViewer1.LocalReport.SetParameters(reportParameters);
            reportViewer1.RefreshReport();
        }

        private DataTable GetData(string query)
        {
            SqlCommand cmd = new SqlCommand(query, conn);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable result = new DataTable();
            adapter.Fill(result);
            return result;
        }

        private string GetTopResult(string title, DataTable data)
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine("- " + title + ":");
            foreach (DataRow row in data.Rows)
            {
                result.AppendLine($"+ {row[0]}: {row[1]} lượt mượn");
            }
            return result.ToString();
        }

        private string GetSqlTopChuDeNhieu()
        {
            return "SELECT TOP 3 cd.TenChuDe, " +
                   $"SUM(CASE WHEN pm.NgayMuon BETWEEN CONVERT(date, '{dtTuNgay.Text}', 103) AND CONVERT(date, '{dtDenNgay.Text}', 103) THEN 1 ELSE 0 END) AS SoLuotMuon " +
                   "FROM ChuDe AS cd " +
                   "JOIN DauSach AS ds ON cd.MaChuDe = ds.MaChuDe " +
                   "JOIN CuonSach AS cs ON ds.MaDauSach = cs.MaDauSach " +
                   "LEFT JOIN CT_PhieuMuon AS ct ON cs.MaSach = ct.MaSach " +
                   "LEFT JOIN PhieuMuon AS pm ON ct.MaPhieuMuon = pm.MaPhieuMuon " +
                   "GROUP BY cd.TenChuDe " +
                   "ORDER BY SoLuotMuon DESC";
        }

        private string GetSqlTopChuDeIt()
        {
            return "SELECT TOP 3 cd.TenChuDe, " +
                   $"SUM(CASE WHEN pm.NgayMuon BETWEEN CONVERT(date, '{dtTuNgay.Text}', 103) AND CONVERT(date, '{dtDenNgay.Text}', 103) THEN 1 ELSE 0 END) AS SoLuotMuon " +
                   "FROM ChuDe AS cd " +
                   "JOIN DauSach AS ds ON cd.MaChuDe = ds.MaChuDe " +
                   "JOIN CuonSach AS cs ON ds.MaDauSach = cs.MaDauSach " +
                   "LEFT JOIN CT_PhieuMuon AS ct ON cs.MaSach = ct.MaSach " +
                   "LEFT JOIN PhieuMuon AS pm ON ct.MaPhieuMuon = pm.MaPhieuMuon " +
                   "GROUP BY cd.TenChuDe " +
                   "ORDER BY SoLuotMuon ASC";
        }

        private string GetSqlTopDauSachNhieu()
        {
            return "SELECT TOP 3 ds.TenDauSach, " +
                   $"SUM(CASE WHEN pm.NgayMuon BETWEEN CONVERT(date, '{dtTuNgay.Text}', 103) AND CONVERT(date, '{dtDenNgay.Text}', 103) THEN 1 ELSE 0 END) AS SoLuotMuon " +
                   "FROM DauSach AS ds " +
                   "JOIN CuonSach AS cs ON ds.MaDauSach = cs.MaDauSach " +
                   "LEFT JOIN CT_PhieuMuon AS ct ON cs.MaSach = ct.MaSach " +
                   "LEFT JOIN PhieuMuon AS pm ON ct.MaPhieuMuon = pm.MaPhieuMuon " +
                   "GROUP BY ds.TenDauSach " +
                   "ORDER BY SoLuotMuon DESC";
        }

        private string GetSqlTopDauSachIt()
        {
            return "SELECT TOP 3 ds.TenDauSach, " +
                   $"SUM(CASE WHEN pm.NgayMuon BETWEEN CONVERT(date, '{dtTuNgay.Text}', 103) AND CONVERT(date, '{dtDenNgay.Text}', 103) THEN 1 ELSE 0 END) AS SoLuotMuon " +
                   "FROM DauSach AS ds " +
                   "JOIN CuonSach AS cs ON ds.MaDauSach = cs.MaDauSach " +
                   "LEFT JOIN CT_PhieuMuon AS ct ON cs.MaSach = ct.MaSach " +
                   "LEFT JOIN PhieuMuon AS pm ON ct.MaPhieuMuon = pm.MaPhieuMuon " +
                   "GROUP BY ds.TenDauSach " +
                   "ORDER BY SoLuotMuon ASC";
        }

        public frmBCLuotMuonDS()
        {
            InitializeComponent();
        }

        private void frmLuotMuonDS_Load(object sender, EventArgs e)
        {
            // Thiết lập chuỗi kết nối
            constr = DBConfig.ConnectionString;
            conn.ConnectionString = constr;
            conn.Open();

            // Làm mới báo cáo
            this.reportViewer1.RefreshReport();
        }
    }
}