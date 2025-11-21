using QL_ThuVien.Main_UC.QLSach;
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
    public partial class frmHienThiBanDau : Form
    {
        private void addUserControl(UserControl userControl)
        {
            userControl.Dock = DockStyle.Fill;
            panelContainer.Controls.Clear();
            panelContainer.Controls.Add(userControl);
            userControl.BringToFront();
        }
        public frmHienThiBanDau()
        {
            InitializeComponent();
        }

        private void btnChiTietSach_Click(object sender, EventArgs e)
        {
            var uc = new UC_ChiTietSach();
            addUserControl(uc);
        }

        private void btnDauSach_Click(object sender, EventArgs e)
        {
            var uc = new UC_DauSach("");
            addUserControl(uc);
        }

        private void btnCuonSach_Click(object sender, EventArgs e)
        {
            var uc = new UC_CuonSach("");
            addUserControl(uc);
        }

        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            this.Hide();
            var f = new frmSignIn();
            f.ShowDialog();
        }

        private void frmHienThiBanDau_Load(object sender, EventArgs e)
        {
            btnChiTietSach.Checked = true;
            var uc = new UC_ChiTietSach();
            addUserControl(uc);

            // Tạo đối tượng ToolTip
            ToolTip toolTip = new ToolTip();
            // Cài đặt Tooltip cho Button
            toolTip.SetToolTip(btnDangNhap, "Chỉ dành cho Thủ thư hoặc Admin đăng nhập để quản lý hệ thống.");
        }
    }
}
