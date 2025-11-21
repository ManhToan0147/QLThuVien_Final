using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QL_ThuVien.Main_UC.BaoCao
{
    public partial class UC_BaoCao : UserControl
    {
        public UC_BaoCao()
        {
            InitializeComponent();
        }
        bool isCollapsedDocGia;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (isCollapsedDocGia)
            {
                dropDown_DocGia.Height += 10;
                if (dropDown_DocGia.Size == dropDown_DocGia.MaximumSize)
                {
                    timer1.Stop();
                    isCollapsedDocGia = false;
                }
            }
            else
            {
                dropDown_DocGia.Height -= 10;
                if (dropDown_DocGia.Size == dropDown_DocGia.MinimumSize)
                {
                    timer1.Stop();
                    isCollapsedDocGia = true;
                }
            }
        }

        private void UC_BaoCao_Load(object sender, EventArgs e)
        {
            timer1.Start();
            timer2.Start();
            timer3.Start();
            timer4.Start();
        }

        private void btnBC_DocGia_Click(object sender, EventArgs e)
        {
            timer1.Start();
        }
        bool isCollapsedSach;
        private void timer2_Tick(object sender, EventArgs e)
        {
            if (isCollapsedSach)
            {
                dropDown_Sach.Height += 10;
                if (dropDown_Sach.Size == dropDown_Sach.MaximumSize)
                {
                    timer2.Stop();
                    isCollapsedSach = false;
                }
            }
            else
            {
                dropDown_Sach.Height -= 10;
                if (dropDown_Sach.Size == dropDown_Sach.MinimumSize)
                {
                    timer2.Stop();
                    isCollapsedSach = true;
                }
            }
        }
        bool isCollapsedMuonTra;
        private void timer3_Tick(object sender, EventArgs e)
        {
            if (isCollapsedMuonTra)
            {
                dropDownMuonTra.Height += 10;
                if (dropDownMuonTra.Size == dropDownMuonTra.MaximumSize)
                {
                    timer3.Stop();
                    isCollapsedMuonTra = false;
                }
            }
            else
            {
                dropDownMuonTra.Height -= 10;
                if (dropDownMuonTra.Size == dropDownMuonTra.MinimumSize)
                {
                    timer3.Stop();
                    isCollapsedMuonTra = true;
                }
            }
        }
        bool isCollapsedPhat;
        private void timer4_Tick(object sender, EventArgs e)
        {
            if (isCollapsedPhat)
            {
                dropDown_ViPham.Height += 10;
                if (dropDown_ViPham.Size == dropDown_ViPham.MaximumSize)
                {
                    timer4.Stop();
                    isCollapsedPhat = false;
                }
            }
            else
            {
                dropDown_ViPham.Height -= 10;
                if (dropDown_ViPham.Size == dropDown_ViPham.MinimumSize)
                {
                    timer4.Stop();
                    isCollapsedPhat = true;
                }
            }
        }

        private void btnMuonTra_Click(object sender, EventArgs e)
        {
            timer3.Start();
        }

        private void btnViPham_Click(object sender, EventArgs e)
        {
            timer4.Start();
        }

        private void btnSach_Click(object sender, EventArgs e)
        {
            timer2.Start();
        }

        private void btnDocGiaMoi_Click(object sender, EventArgs e)
        {
            var form = new frmBCDocGiaMoi();
            form.ShowDialog();
        }

        private void guna2Button16_Click(object sender, EventArgs e)
        {
            var form = new frmBCDGHetHan();
            form.ShowDialog();
        }

        private void guna2Button17_Click(object sender, EventArgs e)
        {
            var form = new frmBCDGViPham();
            form.ShowDialog();
        }

        private void guna2Button19_Click(object sender, EventArgs e)
        {
            var form = new frmBCLuotMuonDS();
            form.ShowDialog(); 
        }

        private void guna2Button20_Click(object sender, EventArgs e)
        {
            var form = new frmBCSachMat();
            form.ShowDialog();
        }

        private void guna2Button21_Click(object sender, EventArgs e)
        {
            var form = new frmBCSachTraHu();
            form.ShowDialog();
        }

        private void guna2Button23_Click(object sender, EventArgs e)
        {
            var form = new frmBCDGMuonSach();
            form.ShowDialog();
        }

        private void guna2Button24_Click(object sender, EventArgs e)
        {
            var form = new frmBCPMQuaHan();
            form.ShowDialog();
        }

        private void guna2Button26_Click(object sender, EventArgs e)
        {
            var form = new frmTongHopTheoLoaiVP();
            form.ShowDialog();
        }

        private void guna2Button27_Click(object sender, EventArgs e)
        {
            var form = new frmBCPhPhatTheoDG();
            form.ShowDialog();
        }
    }
}
