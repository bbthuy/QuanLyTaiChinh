using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuanLyTaiChinh.Models;
using QuanLyTaiChinh.Services;

namespace QuanLyTaiChinh.ViewModels
{
    public partial class TransactionsViewModel : ObservableObject
    {
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
            var query = TransactionStore.Instance.Transactions.AsEnumerable();

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
                result.Id = TransactionStore.Instance.GetNextId();
                TransactionStore.Instance.Transactions.Add(result);
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
                TransactionStore.Instance.Transactions.Remove(item);
                Refresh();
                // TODO: goi TransactionService.Delete(item.Id) de xoa khoi database
            }
        }
    }
}