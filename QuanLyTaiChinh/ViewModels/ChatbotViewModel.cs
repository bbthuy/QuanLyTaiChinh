using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuanLyTaiChinh.Models;
using QuanLyTaiChinh.Services;

namespace QuanLyTaiChinh.ViewModels
{
    public partial class ChatbotViewModel : ObservableObject
    {
        [ObservableProperty] private bool isOpen;
        [ObservableProperty] private bool greetingVisible;
        [ObservableProperty] private string greetingText = "Xin chào 👋";
        [ObservableProperty] private bool isAskingAmount;
        [ObservableProperty] private string amountInput = "";

        public ObservableCollection<ChatMessage> Messages { get; } = new();

        private DispatcherTimer? _greetingTimer;

        public ChatbotViewModel()
        {
            Messages.Add(new ChatMessage
            {
                Text = "Xin chào! Mình là trợ lý tài chính. Chọn 1 câu hỏi bên dưới nhé 👇",
                IsFromBot = true
            });
        }

        [RelayCommand]
        private void ToggleOpen()
        {
            IsOpen = !IsOpen;
            if (IsOpen) GreetingVisible = false;
        }

        // Goi tu MainViewModel moi khi chuyen tab
        public void ShowGreeting()
        {
            if (IsOpen) return; // dang mo chat san roi thi khong can hien bubble chao nua

            GreetingVisible = true;

            _greetingTimer?.Stop();
            _greetingTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
            _greetingTimer.Tick += (_, __) =>
            {
                GreetingVisible = false;
                _greetingTimer!.Stop();
            };
            _greetingTimer.Start();
        }

        private void AddUser(string text) => Messages.Add(new ChatMessage { Text = text, IsFromBot = false });
        private void AddBot(string text) => Messages.Add(new ChatMessage { Text = text, IsFromBot = true });

        private static (decimal income, decimal expense, decimal saving) GetTotals()
        {
            var all = TransactionStore.Instance.Transactions;
            decimal income = all.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
            decimal expense = all.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);
            decimal saving = all.Where(t => t.Type == TransactionType.Saving).Sum(t => t.Amount);
            return (income, expense, saving);
        }

        [RelayCommand]
        private void AskBalance()
        {
            AddUser("💰 Xem số dư");
            var (income, expense, saving) = GetTotals();
            decimal balance = income - expense - saving;
            AddBot($"Số dư hiện tại của bạn là {balance:N0}đ.\n(Thu {income:N0}đ − Chi {expense:N0}đ − Tiết kiệm {saving:N0}đ)");
        }

        [RelayCommand]
        private void AskAnalysis()
        {
            AddUser("📊 Phân tích");

            var top = TransactionStore.Instance.Transactions
                .Where(t => t.Type == TransactionType.Expense)
                .GroupBy(t => t.Category)
                .Select(g => new { Category = g.Key, Total = g.Sum(t => t.Amount) })
                .OrderByDescending(g => g.Total)
                .Take(3)
                .ToList();

            if (top.Count == 0)
            {
                AddBot("Bạn chưa có khoản chi tiêu nào để phân tích.");
                return;
            }

            var lines = top.Select(c => $"• {c.Category}: {c.Total:N0}đ");
            AddBot("3 danh mục chi nhiều nhất:\n" + string.Join("\n", lines));
        }

        [RelayCommand]
        private void AskGoals()
        {
            AddUser("🎯 Mục tiêu");

            var goals = SavingGoalStore.Instance.Goals;
            if (goals.Count == 0)
            {
                AddBot("Bạn chưa đặt mục tiêu tiết kiệm nào.");
                return;
            }

            var lines = goals.Select(g => $"• {g.Name}: {g.Percent:0}% ({g.CurrentAmount:N0}đ / {g.TargetAmount:N0}đ)");
            AddBot("Tiến độ các mục tiêu tiết kiệm:\n" + string.Join("\n", lines));
        }

        [RelayCommand]
        private void AskSpending()
        {
            AddUser("📉 Xem chi tiêu");

            var now = DateTime.Now;
            decimal expense = TransactionStore.Instance.Transactions
                .Where(t => t.Type == TransactionType.Expense && t.Date.Month == now.Month && t.Date.Year == now.Year)
                .Sum(t => t.Amount);

            AddBot($"Tháng {now.Month}/{now.Year} bạn đã chi tổng cộng {expense:N0}đ.");
        }

        [RelayCommand]
        private void AskWarnings()
        {
            AddUser("⚠️ Cảnh báo");

            var warnings = BudgetStore.Instance.Budgets.Where(b => b.Percent >= 80).ToList();
            if (warnings.Count == 0)
            {
                AddBot("Hiện không có ngân sách nào đáng lo, bạn đang chi tiêu ổn! ✅");
                return;
            }

            var lines = warnings.Select(b => $"{b.StatusIcon} {b.Category}: {b.StatusMessage}");
            AddBot("Cảnh báo ngân sách:\n" + string.Join("\n", lines));
        }

        [RelayCommand]
        private void AskWhatIf()
        {
            AddUser("🧪 Nếu tôi mua...");
            IsAskingAmount = true;
            AddBot("Bạn định chi bao nhiêu tiền? Nhập số tiền rồi bấm Gửi nhé.");
        }

        [RelayCommand]
        private void SubmitWhatIfAmount()
        {
            if (!decimal.TryParse(AmountInput, out var amount) || amount <= 0)
            {
                AddBot("Số tiền không hợp lệ, thử lại nhé.");
                return;
            }

            AddUser($"{amount:N0}đ");

            var (income, expense, saving) = GetTotals();
            decimal balance = income - expense - saving;

            AddBot(amount <= balance
                ? $"✅ Bạn đủ khả năng chi {amount:N0}đ. Số dư còn lại sau khi mua khoảng {balance - amount:N0}đ."
                : $"❌ Bạn không đủ số dư, đang thiếu {amount - balance:N0}đ so với khoản chi này.");

            IsAskingAmount = false;
            AmountInput = "";
        }
    }
}