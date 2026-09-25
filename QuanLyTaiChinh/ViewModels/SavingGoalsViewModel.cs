using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuanLyTaiChinh.Models;
using QuanLyTaiChinh.Services;

namespace QuanLyTaiChinh.ViewModels
{
    public partial class SavingGoalsViewModel : ObservableObject
    {
        // Doc tu SavingGoalStore dung chung -> khong mat du lieu khi chuyen trang roi quay lai,
        // va chatbot cung doc duoc dung du lieu nay
        public ObservableCollection<SavingGoalItem> Goals => SavingGoalStore.Instance.Goals;

        [RelayCommand]
        private void AddGoal()
        {
            var window = new View.SavingGoalEditWindow();
            if (window.ShowDialog() == true)
            {
                var result = window.ResultGoal;
                result.Id = Goals.Count == 0 ? 1 : Goals.Max(g => g.Id) + 1;
                Goals.Add(result);
                // TODO: goi SavingGoalService.Add(result) de luu vao database
            }
        }

        [RelayCommand]
        private void EditGoal(SavingGoalItem? item)
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
                // TODO: goi SavingGoalService.Update(item) de cap nhat database
            }
        }

        [RelayCommand]
        private void DeleteGoal(SavingGoalItem? item)
        {
            if (item is null) return;

            var confirm = MessageBox.Show($"Xóa mục tiêu \"{item.Name}\"?", "Xác nhận",
                                           MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirm == MessageBoxResult.Yes)
            {
                Goals.Remove(item);
                // TODO: goi SavingGoalService.Delete(item.Id) de xoa khoi database
            }
        }
    }
}