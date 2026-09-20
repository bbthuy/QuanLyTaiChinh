using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Linq;
using QuanLyTaiChinh.Models;
using QuanLyTaiChinh.Services;
using System.Windows.Controls;


namespace QuanLyTaiChinh.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasNotifications))]
        private ObservableCollection<string> notifications = new();

        public bool HasNotifications => Notifications.Count > 0;

        // Goi ham nay moi lan bam mo chuong thong bao (xem huong dan XAML ben duoi)
        public void RefreshNotifications()
        {
            Notifications = new ObservableCollection<string>(
                BudgetStore.Instance.Budgets
                    .Where(b => b.Percent >= 80) // dung nguong da dat trong BudgetItem (Cam >=80, Do >=100)
                    .Select(b => $"{b.Icon} {b.Category}: {b.StatusMessage}")
            );
        }

        [ObservableProperty]
        private object currentViewModel;

        [ObservableProperty]
        private string selectedMenu = "Dashboard";

        public MainViewModel()
        {
            CurrentViewModel = new DashboardViewModel();
        }

        [RelayCommand]
        private void Navigate(string menu)
        {
            SelectedMenu = menu;

            CurrentViewModel = menu switch
            {
                "Dashboard" => new DashboardViewModel(),
                "Transactions" => new TransactionsViewModel(),
                "Budgets" => new BudgetsViewModel(),
                "Analytics" => new AnalyticsViewModel(),
                "SavingGoals" => new SavingGoalsViewModel(),
                _ => CurrentViewModel
            };
        }
    }
}
