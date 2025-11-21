using QL_ThuVien.Main_UC.QLMuonTra;
using QL_ThuVien.Main_UC.QLSach;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.WebSockets;
using System.Windows.Forms;

namespace QL_ThuVien.Ribbon
{
    public partial class UC_QLMuonTra_Ribbon : UserControl
    {
        private string userRole;
        private string maThuThu;
        private void addUserControl(UserControl userControl)
        {
            userControl.Dock = DockStyle.Fill;
            panelContainer.Controls.Clear();
            panelContainer.Controls.Add(userControl);
            userControl.BringToFront();
        }

        public UC_QLMuonTra_Ribbon(string role, string maThuThu)
        {
            InitializeComponent();
            userRole = role;
            //SetUpButtonsBasedOnRole();
            this.maThuThu = maThuThu;
        }
        //private void SetUpButtonsBasedOnRole()
        //{
        //    if (userRole == "thuthu")
        //    {
        //        // Ẩn nút "Quản lý Thủ Thư"
        //        btnThuThu.Visible = false;

        //        // Điều chỉnh lại vị trí các nút còn lại
        //        btnPhieuMuon.Location = new Point(53, 0); // Di chuyển nút đầu tiên
        //        btnPhieuTra.Location = new Point(300, 0);
        //        btnPhieuPhat.Location = new Point(547, 0);
        //        // Mặc định hiển thị tab "Phiếu Mượn" và chọn nút "Phiếu Mượn"
        //        btnPhieuMuon.Checked = true;
        //        var uc = new UC_PhieuMuon(userRole, maThuThu); // Thay bằng UserControl của tab "Phiếu Mượn"
        //        addUserControl(uc);
        //    }
        //    else
        //    {
        //        // Mặc định hiển thị tab "Thủ Thư" và chọn nút "Thủ Thư" nếu là admin
        //        btnThuThu.Checked = true;
        //        var uc = new UC_QLThuThu();
        //        addUserControl(uc);
        //    }
        //}

        private void btnPhieuMuon_Click(object sender, EventArgs e)
        {
            var uc = new UC_PhieuMuon(userRole, maThuThu);
            addUserControl(uc);
        }

        private void btnPhieuTra_Click(object sender, EventArgs e)
        {
            var uc = new UC_PhieuTra();
            addUserControl(uc);
        }

        private void btnPhieuPhat_Click(object sender, EventArgs e)
        {
            var uc = new UC_PhieuPhat(userRole, maThuThu);
            addUserControl(uc);
        }

        private void UC_QLMuonTra_Ribbon_Load(object sender, EventArgs e)
        {
            var uc = new UC_PhieuMuon(userRole, maThuThu);
            addUserControl(uc);
            btnPhieuMuon.Checked = true;
        }

        private void panelRibbon_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panelContainer_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
