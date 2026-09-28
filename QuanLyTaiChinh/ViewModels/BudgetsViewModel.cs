using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuanLyTaiChinh.Models;
using QuanLyTaiChinh.Services;

namespace QuanLyTaiChinh.ViewModels
{
    public partial class BudgetsViewModel : ObservableObject
    {
        public ObservableCollection<BudgetItem> Budgets => BudgetStore.Instance.Budgets;

        public string CurrentMonthLabel => $"Tháng {DateTime.Now.Month}, {DateTime.Now.Year}";

        public BudgetsViewModel()
        {
            RecalculateSpent();
            TransactionStore.Instance.Transactions.CollectionChanged += (_, __) => RecalculateSpent();
        }

        private void RecalculateSpent()
        {
            var now = DateTime.Now;

            foreach (var budget in Budgets)
            {
                budget.SpentAmount = TransactionStore.Instance.Transactions
                    .Where(t => t.Type == TransactionType.Expense
                             && t.Category == budget.Category
                             && t.Date.Month == now.Month
                             && t.Date.Year == now.Year)
                    .Sum(t => t.Amount);
            }
        }

        [RelayCommand]
        private async Task AddBudget()
        {
            var existingCategories = Budgets.Select(b => b.Category).ToList();
            var window = new View.BudgetEditWindow(existingCategories);

            if (window.ShowDialog() == true)
            {
                await BudgetStore.Instance.AddAsync(window.ResultBudget);
                RecalculateSpent();
            }
        }

        [RelayCommand]
        private async Task EditBudget(BudgetItem? item)
        {
            if (item is null) return;

            var otherCategories = Budgets.Where(b => b.Id != item.Id).Select(b => b.Category).ToList();
            var window = new View.BudgetEditWindow(otherCategories, item);

            if (window.ShowDialog() == true)
            {
                var edited = window.ResultBudget;
                item.Category = edited.Category;
                item.Icon = edited.Icon;
                item.LimitAmount = edited.LimitAmount;

                await BudgetStore.Instance.UpdateAsync(item);
                RecalculateSpent();
            }
        }

        [RelayCommand]
        private async Task DeleteBudget(BudgetItem? item)
        {
            if (item is null) return;

            var confirm = MessageBox.Show($"Xóa ngân sách \"{item.Category}\"?", "Xác nhận",
                                           MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirm == MessageBoxResult.Yes)
            {
                await BudgetStore.Instance.DeleteAsync(item);
            }
        }
    }
}