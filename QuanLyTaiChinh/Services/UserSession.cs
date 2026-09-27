using QuanLyTaiChinh.Models;

namespace QuanLyTaiChinh.Services
{
    public static class UserSession
    {
        public static User? CurrentUser { get; set; }
        public static int CurrentUserId => CurrentUser?.UserId ?? 0;
    }
}