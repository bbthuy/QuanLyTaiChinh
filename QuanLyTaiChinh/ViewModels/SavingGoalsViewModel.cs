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
    public partial class SavingGoalsViewModel : ObservableObject
    {
        public ObservableCollection<SavingGoalItem> Goals => SavingGoalStore.Instance.Goals;

        [RelayCommand]
        private async Task AddGoal()
        {
            var window = new View.SavingGoalEditWindow();
            if (window.ShowDialog() == true)
            {
                await SavingGoalStore.Instance.AddAsync(window.ResultGoal);
            }
        }

        [RelayCommand]
        private async Task EditGoal(SavingGoalItem? item)
        {
            if (item is null) return;

            var window = new View.SavingGoalEditWindow(item);
            if (window.ShowDialog() == true)
            {
                var edited = window.ResultGoal;
                item.Name = edited.Name;
                item.Icon = edited.Icon;
                item.DeadlineLabel = edited.DeadlineLabel;
                item.AccentColor = edited.AccentColor;
                item.CurrentAmount = edited.CurrentAmount;
                item.TargetAmount = edited.TargetAmount;

                await SavingGoalStore.Instance.UpdateAsync(item);
            }
        }

        [RelayCommand]
        private async Task DeleteGoal(SavingGoalItem? item)
        {
            if (item is null) return;

            var confirm = MessageBox.Show($"Xóa mục tiêu \"{item.Name}\"?", "Xác nhận",
                                           MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirm == MessageBoxResult.Yes)
            {
                await SavingGoalStore.Instance.DeleteAsync(item);
            }
        }
    }
}