using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuanLyTaiChinh.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Xml.Linq;

namespace QuanLyTaiChinh.ViewModels
{
    public partial class TransactionsViewModel : ObservableObject
    {
        // TODO: thay bang du lieu that lay tu TransactionService/database sau nay
        private readonly List<TransactionItem> _allTransactions = new()
        {
            new TransactionItem { Id = 1, Icon = "💼", Name = "Lương tháng 9",        Category = "Thu nhập",     Date = new DateTime(2026,9,15), Amount = 24000000, Type = TransactionType.Income  },
            new TransactionItem { Id = 2, Icon = "🛒", Name = "Siêu thị VinMart",     Category = "Ăn uống",      Date = new DateTime(2026,9,14), Amount = 680000,   Type = TransactionType.Expense },
            new TransactionItem { Id = 3, Icon = "☕", Name = "Grab Food",            Category = "Ăn uống",      Date = new DateTime(2026,9,13), Amount = 125000,   Type = TransactionType.Expense },
            new TransactionItem { Id = 4, Icon = "🏦", Name = "Tiết kiệm tháng 9",    Category = "Tiết kiệm",    Date = new DateTime(2026,9,12), Amount = 4000000,  Type = TransactionType.Saving  },
            new TransactionItem { Id = 5, Icon = "🎨", Name = "Freelance Design",     Category = "Thu nhập",     Date = new DateTime(2026,9,10), Amount = 3500000,  Type = TransactionType.Income  },
            new TransactionItem { Id = 6, Icon = "🛵", Name = "Grab Bike",            Category = "Di chuyển",    Date = new DateTime(2026,9,10), Amount = 45000,    Type = TransactionType.Expense },
            new TransactionItem { Id = 7, Icon = "🎬", Name = "Netflix Premium",      Category = "Giải trí",     Date = new DateTime(2026,9,9),  Amount = 260000,   Type = TransactionType.Expense },
            new TransactionItem { Id = 8, Icon = "🏥", Name = "Phòng khám đa khoa",   Category = "Sức khỏe",     Date = new DateTime(2026,9,7),  Amount = 350000,   Type = TransactionType.Expense },
            new TransactionItem { Id = 9, Icon = "☁️", Name = "Amazon Web Services",  Category = "Chi phí nghề", Date = new DateTime(2026,9,6),  Amount = 180000,   Type = TransactionType.Expense },
        };

        [ObservableProperty]
        private ObservableCollection<TransactionItem> filteredTransactions = new();

        [ObservableProperty]
        private string searchText = "";

        [ObservableProperty]
        private string selectedFilter = "Tất cả";

        public TransactionsViewModel()
        {
            Refresh();
        }

        partial void OnSearchTextChanged(string value) => Refresh();
        partial void OnSelectedFilterChanged(string value) => Refresh();

        private void Refresh()
        {
            var query = _allTransactions.AsEnumerable();

            query = SelectedFilter switch
            {
                "Thu nhập" => query.Where(t => t.Type == TransactionType.Income),
                "Chi tiêu" => query.Where(t => t.Type == TransactionType.Expense),
                "Tiết kiệm" => query.Where(t => t.Type == TransactionType.Saving),
                _ => query
            };

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                query = query.Where(t =>
                    t.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    t.Category.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }

            FilteredTransactions = new ObservableCollection<TransactionItem>(query.OrderByDescending(t => t.Date));
        }

        [RelayCommand]
        private void SetFilter(string filter) => SelectedFilter = filter;

        [RelayCommand]
        private void AddTransaction()
        {
            var window = new View.TransactionEditWindow();
            if (window.ShowDialog() == true)
            {
                var result = window.ResultTransaction;
                result.Id = _allTransactions.Count == 0 ? 1 : _allTransactions.Max(t => t.Id) + 1;
                _allTransactions.Add(result);
                Refresh();
                // TODO: goi TransactionService.Add(result) de luu vao database
            }
        }

        [RelayCommand]
        private void EditTransaction(TransactionItem? item)
        {
            if (item is null) return;

            var window = new View.TransactionEditWindow(item);
            if (window.ShowDialog() == true)
            {
                var edited = window.ResultTransaction;
                item.Name = edited.Name;
                item.Category = edited.Category;
                item.Date = edited.Date;
                item.Amount = edited.Amount;
                item.Type = edited.Type;
                Refresh();
                // TODO: goi TransactionService.Update(item) de cap nhat database
            }
        }

        [RelayCommand]
        private void DeleteTransaction(TransactionItem? item)
        {
            if (item is null) return;

            var confirm = MessageBox.Show($"Xóa giao dịch \"{item.Name}\"?", "Xác nhận",
                                           MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirm == MessageBoxResult.Yes)
            {
                _allTransactions.Remove(item);
                Refresh();
                // TODO: goi TransactionService.Delete(item.Id) de xoa khoi database
            }
        }
    }
}
