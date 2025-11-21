using QL_ThuVien.Main_UC.QLDocGia;
using QL_ThuVien.Main_UC.QLMuonTra;
using QL_ThuVien.Support_UC;
using QL_ThuVien.User_Management;
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
    public partial class UC_QLNguoiDung_Ribbon : UserControl
    {
        private string currentUserRole = UserSession.UserRole;

        public UC_QLNguoiDung_Ribbon()
        {
            InitializeComponent();
        }
        private void addUserControl(UserControl userControl)
        {
            userControl.Dock = DockStyle.Fill;
            panelContainer.Controls.Clear();
            panelContainer.Controls.Add(userControl);
            userControl.BringToFront();
        }

        private void btnDocGia_Click(object sender, EventArgs e)
        {
            var uc = new UC_QLDocGia();
            addUserControl(uc);
        }

        private void btnThuThu_Click(object sender, EventArgs e)
        {
            if (currentUserRole == "thuthu")
            {
                var uc = new UC_ThongTinCaNhan();
                addUserControl(uc);
            } 
            else if (currentUserRole == "admin")
            {
                var uc = new UC_QLThuThu();
                addUserControl(uc);
            }
        }

        private void UC_QLNguoiDung_Ribbon_Load(object sender, EventArgs e)
        {
            var uc = new UC_QLDocGia();
            addUserControl(uc);
            btnDocGia.Checked = true;
        }
    }
}
