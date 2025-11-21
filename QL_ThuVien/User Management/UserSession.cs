using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QL_ThuVien.User_Management
{
    public static class UserSession
    {
        public static string UserId { get; set; }
        public static string UserName { get; set; }
        public static string UserRole { get; set; }
        //Thời gian đăng nhập (để kiểm tra timeout)
        public static DateTime LoginTime { get; set; }

        public static string UserEmail { get; set; }

        public static bool IsLoggedIn => !string.IsNullOrEmpty(UserId);
        public static bool IsAdmin => UserRole?.ToLower() == "admin";
        public static bool IsLibrarian => UserRole?.ToLower() == "thuthu";
        public static bool IsReader => UserRole?.ToLower() == "docgia";
        public static void Clear()
        {
            UserId = null;
            UserName = null;
            UserRole = null;
            UserEmail = null;
        }
    }
}
