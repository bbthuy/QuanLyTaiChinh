using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;


namespace QuanLyTaiChinh.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {

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
