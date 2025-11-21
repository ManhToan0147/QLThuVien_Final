using Microsoft.Reporting.WinForms;
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
    public partial class frmBCPMQuaHan : Form
    {
        SqlConnection conn = new SqlConnection();
        SqlDataAdapter adapter = new SqlDataAdapter();
        DataTable dt = new DataTable();
        string sql, constr, para1;

        public frmBCPMQuaHan()
        {
            InitializeComponent();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmBCPMQuaHan_Load(object sender, EventArgs e)
        {
            // Thiết lập chuỗi kết nối
            constr = @"Data Source=DESKTOP-HPGDAGQ\SQLEXPRESS;Initial Catalog=QuanLyThuVien3;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
            conn.ConnectionString = constr;
            conn.Open();

            // Làm mới báo cáo
            this.reportViewer1.RefreshReport();
        }

        private void btnInBC_Click(object sender, EventArgs e)
        {
            // Tạo câu lệnh SQL với tham số ngày
            sql = "SELECT pm.MaPhieuMuon, pm.MaDocGia, dg.HoTen, ct.MaSach, ds.TenDauSach, " +
                  "pm.NgayMuon, pm.HanTra, DATEDIFF(DAY, pm.HanTra, GETDATE()) AS SoNgayQuaHan " +
                  "FROM PhieuMuon AS pm " +
                  "JOIN CT_PhieuMuon AS ct ON pm.MaPhieuMuon = ct.MaPhieuMuon " +
                  "JOIN DocGia AS dg ON pm.MaDocGia = dg.MaDocGia " +
                  "JOIN KieuMuon AS km ON pm.MaKieuMuon = km.MaKieuMuon " +
                  "JOIN CuonSach AS cs ON cs.MaSach = ct.MaSach " +
                  "JOIN DauSach AS ds ON ds.MaDauSach = cs.MaDauSach " +
                  $"WHERE pm.HanTra BETWEEN CONVERT(date, '{dtTuNgay.Text}', 103) AND CONVERT(date, '{dtDenNgay.Text}', 103) " +
                  "AND pm.HanTra < GETDATE() " +
                  "AND ct.DaTraSach = 0 AND " +
                  "pm.NgayThucTra IS NULL " +
                  "ORDER BY pm.HanTra ASC ";

            // Truy vấn dữ liệu
            adapter = new SqlDataAdapter(sql, conn);
            dt.Clear();
            adapter.Fill(dt);

            // Đưa dữ liệu vào Report Viewer
            ReportDataSource reportDataSource = new ReportDataSource("DataSetPMQuaHan", dt);
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(reportDataSource);
            reportViewer1.LocalReport.ReportEmbeddedResource = "QL_ThuVien.ReportsSystem.Reports.rptPMQuaHan.rdlc";

            // Thêm tham số thời gian vào báo cáo
            para1 = "Từ ngày " + dtTuNgay.Text + " Đến ngày " + dtDenNgay.Text;
            ReportParameter[] reportParameters = new ReportParameter[]
            {
                new ReportParameter("prThoiGian", para1)
            };
            reportViewer1.LocalReport.SetParameters(reportParameters);
            reportViewer1.RefreshReport();
        }
    }
}
