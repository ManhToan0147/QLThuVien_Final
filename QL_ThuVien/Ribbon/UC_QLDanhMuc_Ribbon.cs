using QL_ThuVien.Main_UC.QLDanhMuc;
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
    public partial class UC_QLDanhMuc_Ribbon : UserControl
    {
        private void addUserControl(UserControl userControl)
        {
            userControl.Dock = DockStyle.Fill;
            panelContainer.Controls.Clear();
            panelContainer.Controls.Add(userControl);
            userControl.BringToFront();
        }
        public UC_QLDanhMuc_Ribbon()
        {
            InitializeComponent();
        }

        private void btnLoaiSach_Click(object sender, EventArgs e)
        {
            var uc = new UC_DMLoaiSach();
            addUserControl(uc);
        }

        private void btnChuDe_Click(object sender, EventArgs e)
        {
            var uc = new UC_DMChuDe();
            addUserControl(uc);
        }

        private void btnNhaXB_Click(object sender, EventArgs e)
        {
            var uc = new UC_DMNhaXB();
            addUserControl(uc);
        }

        private void btnKhoSach_Click(object sender, EventArgs e)
        {
            var uc = new UC_DMKhoSach(); 
            addUserControl(uc);
        }

        private void btnTacGia_Click(object sender, EventArgs e)
        {
            var uc = new UC_DMTacGia();
            addUserControl(uc);
        }

        private void btnViPham_Click(object sender, EventArgs e)
        {
            var uc = new UC_DMViPham();
            addUserControl(uc);
        }

        private void UC_QLDanhMuc_Ribbon_Load(object sender, EventArgs e)
        {
            btnLoaiSach.Checked = true;
            var uc = new UC_DMLoaiSach();
            addUserControl(uc); 
        }
    }
}
