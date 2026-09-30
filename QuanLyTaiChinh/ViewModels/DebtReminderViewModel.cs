using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualBasic;
using QuanLyTaiChinh.Data;
using QuanLyTaiChinh.Models;
using QuanLyTaiChinh.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace QuanLyTaiChinh.ViewModels
{
    public partial class DebtReminderViewModel : ObservableObject
    {
        private readonly DebtReminderService _service;
        private readonly int _currentUserId;

        [ObservableProperty]
        private ObservableCollection<DebtReminderItem> _items = new();

        [ObservableProperty]
        private string _alertMessage = string.Empty;

        [ObservableProperty]
        private bool _hasAlerts;

        // Input Form
        [ObservableProperty] private string _title = string.Empty;
        [ObservableProperty] private string _partnerName = string.Empty;
        [ObservableProperty] private string _categoryType = "Borrowing";
        [ObservableProperty] private decimal _principalAmount;
        [ObservableProperty] private double _interestRate;
        [ObservableProperty] private DateTime _dueDate = DateTime.Today;
        [ObservableProperty] private string _recurrenceCycle = "Monthly";

        public DebtReminderViewModel()
        {
            _service = new DebtReminderService(new FinanceWiseDbContext());
            _currentUserId = UserSession.CurrentUserId;

            _ = LoadDataAsync();
        }

        public async Task LoadDataAsync()
        {
            var list = await _service.GetItemsAsync(_currentUserId);
            Items = new ObservableCollection<DebtReminderItem>(list);

            var urgent = Items.Where(x => x.IsOverdue || x.IsDueSoon).ToList();
            if (urgent.Any())
            {
                HasAlerts = true;
                AlertMessage = $"CẢNH BÁO: Bạn có {urgent.Count} khoản hẹn/chi phí sắp đến hạn hoặc đã quá hạn!";
            }
            else
            {
                HasAlerts = false;
            }
        }

        [RelayCommand]
        private async Task AddNewAsync()
        {
            if (string.IsNullOrWhiteSpace(Title) || PrincipalAmount <= 0)
            {
                MessageBox.Show("Vui lòng nhập tên khoản và số tiền hợp lệ!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var item = new DebtReminderItem
            {
                Title = Title,
                PartnerName = PartnerName,
                CategoryType = CategoryType,
                PrincipalAmount = PrincipalAmount,
                InterestRate = InterestRate,
                DueDate = DueDate,
                RecurrenceCycle = RecurrenceCycle
            };

            await _service.AddItemAsync(_currentUserId, item);
            await LoadDataAsync();

            Title = string.Empty;
            PartnerName = string.Empty;
            PrincipalAmount = 0;
            InterestRate = 0;
        }

        [RelayCommand]
        private async Task CompleteItemAsync(DebtReminderItem item)
        {
            if (item == null) return;
            await _service.MarkAsCompletedAsync(item.Id);
            await LoadDataAsync();
        }
    }
}