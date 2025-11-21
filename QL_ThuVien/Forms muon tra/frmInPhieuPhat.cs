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
    public partial class frmInPhieuPhat : Form
    {
        string mapp, mapm, ngaynopphat, madg, hoten, thuthu;
        public frmInPhieuPhat(DataTable data, string ts1, string ts2, string ts3, string ts4, string ts5, string ts6)
        {
            InitializeComponent();
            mapp = ts1; mapm = ts2; ngaynopphat = ts3; madg = ts4; hoten = ts5; thuthu = ts6;
            ReportDataSource rds = new ReportDataSource("DataSet1", data);
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(rds);
            reportViewer1.LocalReport.ReportEmbeddedResource = "QL_ThuVien.rptInPhieuPhat.rdlc";
        }

        private void frmInPhieuPhat_Load(object sender, EventArgs e)
        {
            ReportParameter[] parameters = new ReportParameter[]
            {
                new ReportParameter("maPhieuPhat", mapp),
                new ReportParameter("maPhieuMuon", mapm),
                new ReportParameter("ngayNopPhat", ngaynopphat),
                new ReportParameter("maDocGia", madg),
                new ReportParameter("hoTen", hoten),
                new ReportParameter("thuThu", thuthu)
            };
            this.reportViewer1.LocalReport.SetParameters(parameters);
            this.reportViewer1.RefreshReport();
        }
    }
}
