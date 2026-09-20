using System.Collections.ObjectModel;
using QuanLyTaiChinh.Models;

namespace QuanLyTaiChinh.Services
{
    public class BudgetStore
    {
        private static BudgetStore? _instance;
        public static BudgetStore Instance => _instance ??= new BudgetStore();

        public ObservableCollection<BudgetItem> Budgets { get; } = new();

        private BudgetStore()
        {
            // TODO: thay bang du lieu that (han muc nguoi dung tu dat) lay tu database
            Budgets.Add(new BudgetItem { Id = 1, Icon = "🍜", Category = "Ăn uống", LimitAmount = 3000000 });
            Budgets.Add(new BudgetItem { Id = 2, Icon = "🛵", Category = "Di chuyển", LimitAmount = 800000 });
            Budgets.Add(new BudgetItem { Id = 3, Icon = "🎬", Category = "Giải trí", LimitAmount = 500000 });
            Budgets.Add(new BudgetItem { Id = 4, Icon = "🏥", Category = "Sức khỏe", LimitAmount = 1000000 });
        }
    }
}