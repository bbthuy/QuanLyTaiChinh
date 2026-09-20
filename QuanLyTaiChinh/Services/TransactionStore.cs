using System;
using System.Collections.ObjectModel;
using QuanLyTaiChinh.Models;

namespace QuanLyTaiChinh.Services
{
    public class TransactionStore
    {
        private static TransactionStore? _instance;
        public static TransactionStore Instance => _instance ??= new TransactionStore();

        public ObservableCollection<TransactionItem> Transactions { get; } = new();

        private TransactionStore()
        {
            // TODO: thay bang du lieu that lay tu database khi co
            Transactions.Add(new TransactionItem { Id = 1, Icon = "💼", Name = "Lương tháng " + DateTime.Now.Month, Category = "Thu nhập", Date = DateTime.Now.AddDays(-5), Amount = 24000000, Type = TransactionType.Income });
            Transactions.Add(new TransactionItem { Id = 2, Icon = "🛒", Name = "Siêu thị VinMart", Category = "Ăn uống", Date = DateTime.Now.AddDays(-6), Amount = 680000, Type = TransactionType.Expense });
            Transactions.Add(new TransactionItem { Id = 3, Icon = "☕", Name = "Grab Food", Category = "Ăn uống", Date = DateTime.Now.AddDays(-7), Amount = 125000, Type = TransactionType.Expense });
            Transactions.Add(new TransactionItem { Id = 4, Icon = "🏦", Name = "Tiết kiệm tháng " + DateTime.Now.Month, Category = "Tiết kiệm", Date = DateTime.Now.AddDays(-8), Amount = 4000000, Type = TransactionType.Saving });
            Transactions.Add(new TransactionItem { Id = 5, Icon = "🎨", Name = "Freelance Design", Category = "Thu nhập", Date = DateTime.Now.AddDays(-10), Amount = 3500000, Type = TransactionType.Income });
            Transactions.Add(new TransactionItem { Id = 6, Icon = "🛵", Name = "Grab Bike", Category = "Di chuyển", Date = DateTime.Now.AddDays(-10), Amount = 45000, Type = TransactionType.Expense });
            Transactions.Add(new TransactionItem { Id = 7, Icon = "🎬", Name = "Netflix Premium", Category = "Giải trí", Date = DateTime.Now.AddDays(-11), Amount = 260000, Type = TransactionType.Expense });
            Transactions.Add(new TransactionItem { Id = 8, Icon = "🏥", Name = "Phòng khám đa khoa", Category = "Sức khỏe", Date = DateTime.Now.AddDays(-13), Amount = 350000, Type = TransactionType.Expense });
            Transactions.Add(new TransactionItem { Id = 9, Icon = "☁️", Name = "Amazon Web Services", Category = "Chi phí nghề", Date = DateTime.Now.AddDays(-14), Amount = 180000, Type = TransactionType.Expense });
        }

        public int GetNextId()
        {
            int max = 0;
            foreach (var t in Transactions) if (t.Id > max) max = t.Id;
            return max + 1;
        }
    }
}