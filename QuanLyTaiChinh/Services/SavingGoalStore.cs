using System.Collections.ObjectModel;
using QuanLyTaiChinh.Models;

namespace QuanLyTaiChinh.Services
{
    public class SavingGoalStore
    {
        private static SavingGoalStore? _instance;
        public static SavingGoalStore Instance => _instance ??= new SavingGoalStore();

        public ObservableCollection<SavingGoalItem> Goals { get; } = new();

        private SavingGoalStore()
        {
            // TODO: thay bang du lieu that lay tu SavingGoalService/database
            Goals.Add(new SavingGoalItem { Id = 1, Icon = "🛵", Name = "Mua xe máy", DeadlineLabel = "Mục tiêu: 12/2026", AccentColor = "Blue", CurrentAmount = 28500000, TargetAmount = 45000000 });
            Goals.Add(new SavingGoalItem { Id = 2, Icon = "🗾", Name = "Du lịch Nhật Bản", DeadlineLabel = "Mục tiêu: 03/2027", AccentColor = "Purple", CurrentAmount = 12000000, TargetAmount = 30000000 });
            Goals.Add(new SavingGoalItem { Id = 3, Icon = "🛡️", Name = "Quỹ khẩn cấp", DeadlineLabel = "Mục tiêu: Liên tục", AccentColor = "Green", CurrentAmount = 52000000, TargetAmount = 60000000 });
            Goals.Add(new SavingGoalItem { Id = 4, Icon = "💻", Name = "Mua laptop mới", DeadlineLabel = "Mục tiêu: 06/2027", AccentColor = "Orange", CurrentAmount = 8000000, TargetAmount = 25000000 });
        }
    }
}