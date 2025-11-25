using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QL_ThuVien.Form_support
{
    public partial class frmLocNgay : Form
    {
        private static DateTime? lastTuNgay = null;
        private static DateTime? lastDenNgay = null;
        private static bool lastIsApplied = false;

        public DateTime? TuNgay { get; private set; }
        public DateTime? DenNgay { get; private set; }
        public bool IsApplied { get; private set; }

        public frmLocNgay()
        {
            InitializeComponent();
            LoadLastFilter();
        }
        private void LoadLastFilter()
        {
            if (lastIsApplied && lastTuNgay.HasValue && lastDenNgay.HasValue)
            {
                dtpTuNgay.Value = lastTuNgay.Value;
                dtpDenNgay.Value = lastDenNgay.Value;
            }
            else
            {
                dtpTuNgay.Value = DateTime.Today;
                dtpDenNgay.Value = DateTime.Today;
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNgungApDung_Click(object sender, EventArgs e)
        {
            TuNgay = null;
            DenNgay = null;
            IsApplied = false;

            lastTuNgay = null;
            lastDenNgay = null;
            lastIsApplied = false;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnApDung_Click(object sender, EventArgs e)
        {
            if (dtpTuNgay.Value > dtpDenNgay.Value)
            {
                MessageBox.Show("Từ ngày phải nhỏ hơn hoặc bằng Đến ngày!", "Thông báo");
                return;
            }

            TuNgay = dtpTuNgay.Value.Date;
            DenNgay = dtpDenNgay.Value.Date;
            IsApplied = true;

            lastTuNgay = TuNgay;
            lastDenNgay = DenNgay;
            lastIsApplied = true;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void dtpTuNgay_ValueChanged(object sender, EventArgs e)
        {
            if (dtpTuNgay.Value > dtpDenNgay.Value)
            {
                dtpDenNgay.Value = dtpTuNgay.Value;
            }
        }

        private void dtpDenNgay_ValueChanged(object sender, EventArgs e)
        {
            if (dtpTuNgay.Value > dtpDenNgay.Value)
            {
                dtpTuNgay.Value = dtpDenNgay.Value;
            }
        }
    }
}
