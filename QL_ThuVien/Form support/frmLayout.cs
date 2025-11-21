using QL_ThuVien.Main_UC.BaoCao;
using QL_ThuVien.Main_UC.CaiDat;
using QL_ThuVien.Main_UC.QLDocGia;
using QL_ThuVien.Main_UC.TrangChu;
using QL_ThuVien.Ribbon;
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
using System.Globalization;

namespace QL_ThuVien
{
    public partial class frmLayout : Form
    {
        private string currentUserId = UserSession.UserId;
        private string currentUserName = UserSession.UserName;
        private string currentUserRole = UserSession.UserRole;

        CultureInfo viVN = new CultureInfo("vi-VN");

        private void addUserControl(UserControl userControl)
        {
            userControl.Dock = DockStyle.Fill;
            panelContainer.Controls.Clear();
            panelContainer.Controls.Add(userControl);
            userControl.BringToFront();
        }
        public frmLayout()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            staThoiGian.Text = System.DateTime.Now.ToString("dddd, dd/MM/yyyy HH:mm:ss", viVN);
        }

        private void frmLayout_Load(object sender, EventArgs e)
        {
            btnHome.Checked = true;
            var uc = new UC_TrangChu();

            addUserControl(uc);
            if (currentUserRole == "thuthu")
            {
                // Ẩn các chức năng không được phép sử dụng
                btnCaiDat.Visible = false; // Ví dụ: Ẩn nút quản lý tài khoản
            }
            else if (currentUserRole == "admin")
            {
                // Admin có thể nhìn thấy tất cả
                btnCaiDat.Visible = true;
                btnBaoCao.Visible = true;
            }
            else if (currentUserRole == "docgia")
            {
                btnCaiDat.Visible = false; // Ví dụ: Ẩn nút quản lý tài khoản
                btnBaoCao.Visible = false;         // Ẩn nút báo cáo
                //btnQLTacGia.Visible = false;
                btnQLNguoiDung.Visible = false;
                btnQLMuonTra.Visible = false;
            }
        }

        private void btnSignIn_Out_Click(object sender, EventArgs e)
        {
            DialogResult rs = MessageBox.Show("Bạn có muốn đăng xuất khỏi hệ thống không", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (rs == DialogResult.Yes)
            {
                //Clear cả session trong memory và file
                UserSession.Clear();
                SessionManager.ClearSession(); // Quan trọng!

                frmHienThiBanDau StartForm = new frmHienThiBanDau();
                StartForm.Show();
                this.Hide();

            }
        }

        private void btnQLSach_Click(object sender, EventArgs e)
        {
            var uc = new UC_QLSach_Ribbon(currentUserRole);
            addUserControl(uc);
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            var uc = new UC_TrangChu();
            addUserControl(uc); 
        }

        private void btnCaiDat_Click(object sender, EventArgs e)
        {
            var uc = new UC_CaiDat_Ribbon();
            addUserControl(uc);
        }

        private void btnQLMuonTra_Click(object sender, EventArgs e)
        {
            var uc = new UC_QLMuonTra_Ribbon(currentUserRole, currentUserId);
            addUserControl(uc);
        }

        private void btnHDSD_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, "HuongDanSuDung.chm");
        }

        private void btnBaoCao_Click(object sender, EventArgs e)
        {
            var uc = new UC_BaoCao();
            addUserControl(uc);
        }

        private void btnQLDatPhong_Click(object sender, EventArgs e)
        {
            //var uc = new UC_QLDatPhong_Ribbon(); 
            //addUserControl(uc);
        }

        private void guna2ControlBox1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnQLNguoiDung_Click(object sender, EventArgs e)
        {
            var uc = new UC_QLNguoiDung_Ribbon();
            addUserControl(uc);
        }

        private void btnQLDanhMuc_Click(object sender, EventArgs e)
        {
            var uc = new UC_QLDanhMuc_Ribbon();
            addUserControl(uc);
        }
    }
}
