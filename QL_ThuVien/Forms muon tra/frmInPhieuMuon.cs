using Microsoft.Reporting.WinForms;
using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
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
    
    public partial class frmInPhieuMuon : Form
    {
        string mapm, madg, hoten, kieumuon, ngaymuon, hantra, thuthu;
        public frmInPhieuMuon(DataTable data, string ts1, string ts2, string ts3, string ts4, string ts5, string ts6, string ts7) 
        {
            InitializeComponent();
            mapm = ts1; madg = ts2; hoten = ts3; kieumuon = ts4; ngaymuon = ts5; hantra = ts6; thuthu = ts7;
            ReportDataSource rds = new ReportDataSource("DataSet1", data);
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(rds);
            reportViewer1.LocalReport.ReportEmbeddedResource = "QL_ThuVien.rptInPhieuMuon.rdlc";
        }

        private void frmInPhieuMuon_Load(object sender, EventArgs e)
        {
            ReportParameter[] parameters = new ReportParameter[]
            {
                new ReportParameter("maPhieuMuon", mapm),
                new ReportParameter("maDocGia", madg),  
                new ReportParameter("hoTen", hoten),   
                new ReportParameter("kieuMuon", kieumuon),
                new ReportParameter("ngayMuon", ngaymuon), 
                new ReportParameter("hanTra", hantra),   
                new ReportParameter("thuThu", thuthu)
            };
            this.reportViewer1.LocalReport.SetParameters(parameters);
            this.reportViewer1.RefreshReport();
        }
    }
}
