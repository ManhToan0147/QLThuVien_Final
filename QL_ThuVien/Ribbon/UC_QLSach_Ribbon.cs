using QL_ThuVien.Main_UC.QLSach;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QL_ThuVien.Ribbon
{
    public partial class UC_QLSach_Ribbon : UserControl
    {

        private void addUserControl(UserControl userControl)
        {
            userControl.Dock = DockStyle.Fill;
            panelContainer.Controls.Clear();
            panelContainer.Controls.Add(userControl);
            userControl.BringToFront();
        }
        string role;
        public UC_QLSach_Ribbon(string role)
        {
            InitializeComponent();
            this.role = role;
        }

        private void UC_QLSach_Ribbon_Load(object sender, EventArgs e)
        {
            btnChiTietSach.Checked = true;
            var uc = new UC_ChiTietSach();
            addUserControl(uc);
        }

        private void btnChiTietSach_Click(object sender, EventArgs e)
        {
            var uc = new UC_ChiTietSach();
            addUserControl(uc);
        }

        private void btnDauSach_Click_1(object sender, EventArgs e)
        {
            var uc = new UC_DauSach(role);
            addUserControl(uc);
        }

        private void btnCuonSach_Click(object sender, EventArgs e)
        {
            var uc = new UC_CuonSach(role);
            addUserControl(uc);
        }
    }
}
