using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace QL_ThuVien.User_Management
{
    public static class SessionManager
    {
        private static readonly string SessionFile = Path.Combine(
            Application.StartupPath, "session.json");

        public class SessionData
        {
            public string UserId { get; set; }
            public string UserName { get; set; }
            public string UserRole { get; set; }
            public string UserEmail { get; set; }
            public DateTime LoginTime { get; set; }
            public bool RememberMe { get; set; }
        }

        // ✅ Lưu session ra file
        public static void SaveSession(bool rememberMe = false)
        {
            try
            {
                var sessionData = new SessionData
                {
                    UserId = UserSession.UserId,
                    UserName = UserSession.UserName,
                    UserRole = UserSession.UserRole,
                    UserEmail = UserSession.UserEmail,
                    LoginTime = DateTime.Now,
                    RememberMe = rememberMe
                };

                // Tạo thư mục nếu chưa có
                Directory.CreateDirectory(Path.GetDirectoryName(SessionFile));

                string json = JsonConvert.SerializeObject(sessionData, Formatting.Indented);
                File.WriteAllText(SessionFile, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi lưu session: {ex.Message}");
            }
        }

        // ✅ Load session từ file
        public static bool LoadSession()
        {
            try
            {
                if (!File.Exists(SessionFile))
                    return false;

                string json = File.ReadAllText(SessionFile);
                var sessionData = JsonConvert.DeserializeObject<SessionData>(json);

                if (sessionData == null || !sessionData.RememberMe)
                    return false;

                // Kiểm tra session có quá cũ không (7 ngày)
                if (DateTime.Now - sessionData.LoginTime > TimeSpan.FromDays(7))
                {
                    ClearSession();
                    return false;
                }

                // ✅ Khôi phục UserSession
                UserSession.UserId = sessionData.UserId;
                UserSession.UserName = sessionData.UserName;
                UserSession.UserRole = sessionData.UserRole;
                UserSession.UserEmail = sessionData.UserEmail;

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi load session: {ex.Message}");
                return false;
            }
        }

        // ✅ Xóa session file
        public static void ClearSession()
        {
            try
            {
                if (File.Exists(SessionFile))
                    File.Delete(SessionFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi xóa session: {ex.Message}");
            }
        }

    }
}

