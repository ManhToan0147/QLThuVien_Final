using QL_ThuVien.User_Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QL_ThuVien
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                SetProcessDPIAware();
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //Kiểm tra session TRƯỚC KHI mở form nào
            if (SessionManager.LoadSession())
            {
                Application.Run(new frmLayout());
            }
            else
            {
                Application.Run(new frmHienThiBanDau());
            }

        }
        [DllImport("user32.dll")]
        private static extern void SetProcessDPIAware();
    }
}
