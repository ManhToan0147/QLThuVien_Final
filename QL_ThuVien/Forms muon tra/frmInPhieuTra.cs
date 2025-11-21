using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QL_ThuVien
{
    public partial class frmInPhieuTra : Form
    {
        string mapm, madg, hoten, hantra, ngaytra, songaytre;
        public frmInPhieuTra(DataTable data, string ts1, string ts2, string ts3, string ts4, string ts5, string ts6)
        {
            InitializeComponent();
            mapm = ts1; madg = ts2; hoten = ts3; hantra = ts4; ngaytra = ts5; songaytre = ts6; 
            ReportDataSource rds = new ReportDataSource("DataSet1", data);
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(rds);
            reportViewer1.LocalReport.ReportEmbeddedResource = "QL_ThuVien.rptInPhieuTra.rdlc";
        }
        private void frmInPhieuTra_Load(object sender, EventArgs e)
        {
            ReportParameter[] parameters = new ReportParameter[]
            {
                new ReportParameter("maPhieuMuon", mapm),
                new ReportParameter("maDocGia", madg),
                new ReportParameter("hoTen", hoten),
                new ReportParameter("hanTra", hantra),
                new ReportParameter("ngayTra", ngaytra),
                new ReportParameter("soNgayTre", songaytre),
            };
            this.reportViewer1.LocalReport.SetParameters(parameters);
            this.reportViewer1.RefreshReport();
        }
    }
}
