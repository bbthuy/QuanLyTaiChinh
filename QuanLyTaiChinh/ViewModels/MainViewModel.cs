using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Linq;
using QuanLyTaiChinh.Data;
using QuanLyTaiChinh.Models;
using QuanLyTaiChinh.Services;
using QuanLyTaiChinh.ViewModels;
using System.Windows.Controls;

namespace QuanLyTaiChinh.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasNotifications))]
        private ObservableCollection<string> notifications = new();
        public ChatbotViewModel Chatbot { get; } = new();

        public bool HasNotifications => Notifications.Count > 0;

        // Goi ham nay moi lan bam mo chuong thong bao (xem huong dan XAML ben duoi)
        public void RefreshNotifications()
        {
            var list = new List<string>();

            // --- GIỮ NGUYÊN CODE CŨ: Cảnh báo Ngân sách ---
            if (BudgetStore.Instance?.Budgets != null)
            {
                var budgetAlerts = BudgetStore.Instance.Budgets
                    .Where(b => b.Percent >= 80) // dung nguong da dat trong BudgetItem (Cam >=80, Do >=100)
                    .Select(b => $"{b.Icon} {b.Category}: {b.StatusMessage}");
                list.AddRange(budgetAlerts);
            }

            // --- CHÈN THÊM CÁI MỚI: Cảnh báo Nhắc nợ / Chi phí định kỳ ---
            try
            {
                using (var db = new FinanceWiseDbContext())
                {
                    int userId = UserSession.CurrentUserId;
                    var today = DateTime.Today;

                    var debts = db.DebtReminders
                        .Where(d => d.UserId == userId && d.Status != "Completed")
                        .ToList();

                    foreach (var item in debts)
                    {
                        var dueDate = item.DueDate.Date;
                        decimal remaining = Math.Max(0, item.PrincipalAmount - item.PaidAmount);

                        if (dueDate < today)
                        {
                            int overdueDays = (today - dueDate).Days;
                            list.Add($"⚠️ [Quá hạn] '{item.Title}' quá hạn {overdueDays} ngày (Còn nợ: {remaining:N0} đ)");
                        }
                        else if ((dueDate - today).TotalDays <= 3)
                        {
                            int daysLeft = (dueDate - today).Days;
                            string timeText = daysLeft == 0 ? "hôm nay" : $"còn {daysLeft} ngày";
                            list.Add($"⏰ [Sắp đến hạn] '{item.Title}' ({timeText}, Cần trả: {remaining:N0} đ)");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi quét nhắc nợ: {ex.Message}");
            }

            // Gán lại danh sách thông báo
            Notifications = new ObservableCollection<string>(list);
        }

        [ObservableProperty]
        private object currentViewModel;

        [ObservableProperty]
        private string selectedMenu = "Dashboard";

        public MainViewModel()
        {
            CurrentViewModel = new DashboardViewModel();
            RefreshNotifications();
        }

        [RelayCommand]
        private void Navigate(string menu)
        {
            Chatbot.ShowGreeting();
            SelectedMenu = menu;


            CurrentViewModel = menu switch
            {
                "Dashboard" => new DashboardViewModel(),
                "Transactions" => new TransactionsViewModel(),
                "Budgets" => new BudgetsViewModel(),
                "Analytics" => new AnalyticsViewModel(),
                "SavingGoals" => new SavingGoalsViewModel(),
                "DebtReminder" => new DebtReminderViewModel(),
                _ => CurrentViewModel
            };
        }
    }
}