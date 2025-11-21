using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QL_ThuVien.Main_UC.CaiDat
{
    public partial class UC_SaoLuu_PhucHoi : UserControl
    {
        string strCon = DBConfig.ConnectionString;
        SqlConnection con;
        public UC_SaoLuu_PhucHoi()
        {
            InitializeComponent();
        }

        private void btnBrowse1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fb = new FolderBrowserDialog();
            fb.Description = "Chọn thư mục để lưu";
            if (fb.ShowDialog() == DialogResult.OK)
            {
                txtBackupPath.Text = fb.SelectedPath;
                btnBackup.Enabled = true;
            }
        }
        private async void btnBackup_Click(object sender, EventArgs e)
        {
            con = new SqlConnection(strCon);
            string db = con.Database.ToString();
            bool isSucess = false;

            if (string.IsNullOrEmpty(txtBackupPath.Text))
            {
                MessageBox.Show("Vui lòng chọn đường dẫn lưu file backup", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // Hiển thị thông báo đang xử lý
                MessageBox.Show("Đang thực hiện backup, vui lòng đợi...", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Thực hiện backup trong nền
                await Task.Run(() =>
                {
                    try
                    {
                        string backupQuery = $"BACKUP DATABASE {db} TO DISK = '{txtBackupPath.Text}\\{db}-{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.bak'";
                        using (SqlCommand cmd = new SqlCommand(backupQuery, con))
                        {
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                        isSucess = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Đã xảy ra lỗi trong quá trình backup: " + ex.Message, "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                });
                if (isSucess)
                {
                    MessageBox.Show("Backup thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void btnBrowse2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "File Backup (*.bak)|*.bak";
            openFile.Title = "Restore data";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                txtRestorePath.Text = openFile.FileName;
                btnRestore.Enabled = true;
            }
        }

        private async void btnRestore_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtRestorePath.Text))
            {
                MessageBox.Show("Vui lòng chọn file backup để khôi phục", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Hiển thị thông báo đang xử lý
            MessageBox.Show("Đang thực hiện restore, vui lòng đợi...", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

            string restoreFilePath = txtRestorePath.Text;

            await Task.Run(() =>
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(strCon))
                    {
                        con.Open();
                        string db = con.Database.ToString(); // Tên cơ sở dữ liệu cần restore
                        con.ChangeDatabase("master"); // Chuyển sang cơ sở dữ liệu master

                        string setSingleUserQuery = $"ALTER DATABASE {db} SET SINGLE_USER WITH ROLLBACK IMMEDIATE";
                        string restoreQuery = $"RESTORE DATABASE {db} FROM DISK = '{restoreFilePath}' WITH REPLACE";
                        string setMultiUserQuery = $"ALTER DATABASE {db} SET MULTI_USER";

                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = con;

                        // Đặt database về chế độ SINGLE_USER để thực hiện restore
                        cmd.CommandText = setSingleUserQuery;
                        cmd.ExecuteNonQuery();

                        // Thực hiện restore từ file backup
                        cmd.CommandText = restoreQuery;
                        cmd.ExecuteNonQuery();

                        // Đặt database về chế độ MULTI_USER sau khi restore xong
                        cmd.CommandText = setMultiUserQuery;
                        cmd.ExecuteNonQuery();

                        con.Close();
                    }

                    MessageBox.Show("Restore thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Đã xảy ra lỗi trong quá trình restore: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });
        }
    }
}
