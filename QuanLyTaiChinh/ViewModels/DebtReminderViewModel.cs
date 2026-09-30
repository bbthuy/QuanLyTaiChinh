using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualBasic;
using QuanLyTaiChinh.Data;
using QuanLyTaiChinh.Models;
using QuanLyTaiChinh.Services;
using System;
using System.Collections.Generic;
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
        private List<DebtReminderItem> _rawList = new();

        [ObservableProperty]
        private ObservableCollection<DebtReminderItem> _items = new();

        [ObservableProperty]
        private string _alertMessage = string.Empty;

        [ObservableProperty]
        private bool _hasAlerts;

        private string _currentFilter = "All";
        public string CurrentFilter
        {
            get => _currentFilter;
            set => SetProperty(ref _currentFilter, value);
        }

        [ObservableProperty] private string _title = string.Empty;
        [ObservableProperty] private string _partnerName = string.Empty;
        [ObservableProperty] private string _categoryType = "Borrowing";
        [ObservableProperty] private decimal _principalAmount;
        [ObservableProperty] private double _interestRate;
        [ObservableProperty] private DateTime _dueDate = DateTime.Today.AddDays(7);
        [ObservableProperty] private string _recurrenceCycle = "Monthly";

        private string _note = string.Empty;
        public string Note
        {
            get => _note;
            set => SetProperty(ref _note, value);
        }

        public DebtReminderViewModel()
        {
            _service = new DebtReminderService(new FinanceWiseDbContext());
            _currentUserId = UserSession.CurrentUserId;

            _ = LoadDataAsync();
        }

        [RelayCommand]
        public async Task LoadDataAsync()
        {
            _rawList = await _service.GetItemsAsync(_currentUserId);
            ApplyFilter();


            var urgent = _rawList.Where(x => x.IsOverdue || x.IsDueSoon).ToList();
            if (urgent.Any())
            {
                HasAlerts = true;
                AlertMessage = $"CẢNH BÁO: Bạn có {urgent.Count} khoản hẹn/chi phí sắp đến hạn hoặc đã quá hạn!";
            }
            else
            {
                HasAlerts = false;
            }

            PushNotificationsToMain();
        }

        [RelayCommand]
        public void SetFilter(string filter)
        {
            CurrentFilter = filter;
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            IEnumerable<DebtReminderItem> list = _rawList;

            list = CurrentFilter switch
            {
                "Active" => list.Where(x => x.Status != "Completed"),
                "Overdue" => list.Where(x => x.IsOverdue),
                "Completed" => list.Where(x => x.Status == "Completed"),
                "Debt" => list.Where(x => x.CategoryType == "Borrowing" || x.CategoryType == "Lending"),
                "Recurring" => list.Where(x => x.CategoryType == "Recurring"),
                _ => list
            };

            Items = new ObservableCollection<DebtReminderItem>(list);
        }

        private void PushNotificationsToMain()
        {
            if (Application.Current.MainWindow?.DataContext is MainViewModel mainVm)
            {
                try
                {
                    var notiProp = mainVm.GetType().GetProperty("Notifications");
                    if (notiProp != null && notiProp.GetValue(mainVm) is ObservableCollection<string> currentNotis)
                    {
                        var updated = currentNotis
                            .Where(n => !n.StartsWith("[Nhắc nợ]") && !n.StartsWith("[Hóa đơn]"))
                            .ToList();

                        foreach (var item in _rawList.Where(x => x.Status != "Completed"))
                        {
                            if (item.IsOverdue)
                            {
                                updated.Add($"[Nhắc nợ] '{item.Title}' đã quá hạn {(DateTime.Today - item.DueDate.Date).Days} ngày! Cần thanh toán {item.RemainingAmount:N0} đ.");
                            }
                            else if (item.IsDueSoon)
                            {
                                updated.Add($"[Hóa đơn] '{item.Title}' sắp đến hạn ({item.DueDate:dd/MM/yyyy}). Số tiền: {item.RemainingAmount:N0} đ.");
                            }
                        }

                        notiProp.SetValue(mainVm, new ObservableCollection<string>(updated));
                    }
                }
                catch
                {

                }
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

            try
            {
                var item = new DebtReminderItem
                {
                    Title = Title,
                    PartnerName = PartnerName,
                    CategoryType = CategoryType,
                    PrincipalAmount = PrincipalAmount,
                    InterestRate = InterestRate,
                    DueDate = DueDate,
                    RecurrenceCycle = RecurrenceCycle,
                    Note = Note
                };

                await _service.AddItemAsync(_currentUserId, item);
                await LoadDataAsync();

                Title = string.Empty;
                PartnerName = string.Empty;
                PrincipalAmount = 0;
                InterestRate = 0;
                Note = string.Empty;
            }
            catch (Exception ex)
            {
                var inner = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                MessageBox.Show($"Lỗi lưu dữ liệu: {inner}", "Lỗi Database", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task CompleteItemAsync(DebtReminderItem item)
        {
            if (item == null) return;
            await _service.MarkAsCompletedAsync(item.Id);
            await LoadDataAsync();
        }

        [RelayCommand]
        private async Task DeleteItemAsync(DebtReminderItem item)
        {
            if (item == null) return;

            var confirm = MessageBox.Show($"Bạn có chắc muốn xóa '{item.Title}'?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirm == MessageBoxResult.Yes)
            {
                await _service.DeleteItemAsync(item.Id);
                await LoadDataAsync();
            }
        }

        [RelayCommand]
        private async Task PayPartialAsync(DebtReminderItem item)
        {
            if (item == null) return;

            string input = Interaction.InputBox(
                $"Nhập số tiền muốn thanh toán/thu trước cho '{item.Title}' (Dư nợ còn: {item.RemainingAmount:N0} đ):",
                "Thanh toán từng phần",
                item.RemainingAmount.ToString());

            if (decimal.TryParse(input, out decimal amount) && amount > 0)
            {
                if (amount > item.RemainingAmount)
                {
                    MessageBox.Show("Số tiền nhập vào vượt quá số tiền còn nợ!", "Lỗi nhập", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                await _service.RecordPaymentAsync(item.Id, amount);
                await LoadDataAsync();
            }
        }
    }
}