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
    public partial class BudgetsViewModel : ObservableObject
    {
        // Doc thang tu BudgetStore dung chung -> khong bi mat du lieu khi chuyen trang roi quay lai
        public ObservableCollection<BudgetItem> Budgets => BudgetStore.Instance.Budgets;

        public string CurrentMonthLabel => $"Tháng {DateTime.Now.Month}, {DateTime.Now.Year}";

        public BudgetsViewModel()
        {
            RecalculateSpent();

            // Chi bat cap nhat khi co Them/Xoa giao dich (CollectionChanged).
            // Neu SUA mot giao dich co san, can roi trang Ngan sach roi quay lai
            // (luc do constructor nay chay lai tu dau) de thay so lieu moi nhat.
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
        private void AddBudget()
        {
            var existingCategories = Budgets.Select(b => b.Category).ToList();
            var window = new View.BudgetEditWindow(existingCategories);

            if (window.ShowDialog() == true)
            {
                var result = window.ResultBudget;
                result.Id = Budgets.Count == 0 ? 1 : Budgets.Max(b => b.Id) + 1;
                Budgets.Add(result);
                RecalculateSpent();
                // TODO: goi BudgetService.Add(result) de luu vao database
            }
        }

        [RelayCommand]
        private void EditBudget(BudgetItem? item)
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
                RecalculateSpent();
                // TODO: goi BudgetService.Update(item) de cap nhat database
            }
        }

        [RelayCommand]
        private void DeleteBudget(BudgetItem? item)
        {
            if (item is null) return;

            var confirm = MessageBox.Show($"Xóa ngân sách \"{item.Category}\"?", "Xác nhận",
                                           MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirm == MessageBoxResult.Yes)
            {
                Budgets.Remove(item);
                // TODO: goi BudgetService.Delete(item.Id) de xoa khoi database
            }
        }
    }
}