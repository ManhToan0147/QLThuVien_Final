using QL_ThuVien.Main_UC.CaiDat;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QL_ThuVien.Ribbon
{
    public partial class UC_CaiDat_Ribbon : UserControl
    {
        private void addUserControl(UserControl userControl)
        {
            userControl.Dock = DockStyle.Fill;
            panelContainer.Controls.Clear();
            panelContainer.Controls.Add(userControl);
            userControl.BringToFront();
        }
        public UC_CaiDat_Ribbon()
        {
            InitializeComponent();
        }

        private void UC_CaiDat_Ribbon_Load(object sender, EventArgs e)
        {
            btnTaiKhoan.Checked = true;
            var uc = new UC_QLTaiKhoan();
            addUserControl(uc);
        }

        private void btnDuLieu_Click(object sender, EventArgs e)
        {
            var uc = new UC_SaoLuu_PhucHoi();
            addUserControl(uc);
        }

        private void btnTaiKhoan_Click(object sender, EventArgs e)
        {
            var uc = new UC_QLTaiKhoan();
            addUserControl(uc);
        }
    }
}
