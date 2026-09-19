using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuanLyTaiChinh.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace QuanLyTaiChinh.ViewModels
{
    public partial class SavingGoalsViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<SavingGoalItem> goals = new();

        public SavingGoalsViewModel()
        {
            // TODO: thay bang du lieu that lay tu SavingGoalService/database sau nay
            Goals = new ObservableCollection<SavingGoalItem>
            {
                new SavingGoalItem
                {
                    Id = 1, Icon = "🛵", Name = "Mua xe máy",
                    DeadlineLabel = "Mục tiêu: 12/2026", AccentColor = "Blue",
                    CurrentAmount = 28500000, TargetAmount = 45000000
                },
                new SavingGoalItem
                {
                    Id = 2, Icon = "🗾", Name = "Du lịch Nhật Bản",
                    DeadlineLabel = "Mục tiêu: 03/2027", AccentColor = "Purple",
                    CurrentAmount = 12000000, TargetAmount = 30000000
                },
                new SavingGoalItem
                {
                    Id = 3, Icon = "🛡️", Name = "Quỹ khẩn cấp",
                    DeadlineLabel = "Mục tiêu: Liên tục", AccentColor = "Green",
                    CurrentAmount = 52000000, TargetAmount = 60000000
                },
                new SavingGoalItem
                {
                    Id = 4, Icon = "💻", Name = "Mua laptop mới",
                    DeadlineLabel = "Mục tiêu: 06/2027", AccentColor = "Orange",
                    CurrentAmount = 8000000, TargetAmount = 25000000
                },
            };
        }

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
