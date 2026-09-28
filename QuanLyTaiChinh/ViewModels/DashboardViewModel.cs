using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using QuanLyTaiChinh.Models;
using QuanLyTaiChinh.Services;
using SkiaSharp;

namespace QuanLyTaiChinh.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        // ===== NGAY THANG HIEN TAI =====
        private static readonly DateTime Now = DateTime.Now;

        public string CurrentMonthLabel => $"Tháng {Now.Month}, {Now.Year}";
        public string CurrentMonthShort => $"T{Now.Month}";
        public string ChartTitle => $"Thu chi {Now.Month} tháng đầu năm {Now.Year}";
        public string CategoryMonthLabel => $"Tháng {Now.Month}/{Now.Year}";

        // Bang mau xoay vong cho danh muc chi tieu (vi danh muc la chuoi tu do, khong co san mau co dinh)
        private static readonly string[] Palette =
        {
            "#3B82F6", "#F87171", "#A78BFA", "#FBBF24", "#6EE7B7",
            "#93C5FD", "#FDBA74", "#34D399", "#F472B6", "#60A5FA"
        };

        // ===== 4 THE SO LIEU (tinh that tu du lieu trong TransactionStore) =====
        [ObservableProperty] private string balance = "0đ";
        [ObservableProperty] private string balanceTrend = "";

        [ObservableProperty] private string income = "0đ";
        [ObservableProperty] private string incomeTrend = "";

        [ObservableProperty] private string expense = "0đ";
        [ObservableProperty] private string expenseTrend = "";

        [ObservableProperty] private string savings = "0đ";
        [ObservableProperty] private string savingsTrend = "";

        // ===== BIEU DO DUONG TONG QUAN (khong loc) =====
        public ISeries[] IncomeExpenseSeries { get; set; } = Array.Empty<ISeries>();
        public Axis[] XAxes { get; set; } = Array.Empty<Axis>();
        public Axis[] YAxes { get; set; } = Array.Empty<Axis>();

        // ===== BIEU DO DONUT: CO CAU CHI TIEU THANG HIEN TAI =====
        public ISeries[] CategorySeries { get; set; } = Array.Empty<ISeries>();
        public List<CategorySlice> CategoryLegend { get; set; } = new();

        // ===== GIAO DICH GAN DAY =====
        public ObservableCollection<TransactionItem> RecentTransactions { get; set; } = new();

        // ===== THONG KE THU/CHI CO LOC (theo Ngay/Tuan/Thang + Danh muc) =====
        [ObservableProperty]
        private string selectedPeriod = "Tháng";

        [ObservableProperty]
        private string selectedCategory = "Tất cả";

        public ObservableCollection<string> CategoryOptions { get; } = new();

        public ISeries[] StatsSeries { get; set; } = Array.Empty<ISeries>();
        public Axis[] StatsXAxes { get; set; } = Array.Empty<Axis>();
        public Axis[] StatsYAxes { get; set; } = Array.Empty<Axis>();

        public DashboardViewModel()
        {
            BuildStatCards();
            BuildOverviewChart();
            BuildCategoryDonut();

            RecentTransactions = new ObservableCollection<TransactionItem>(
                TransactionStore.Instance.Transactions.OrderByDescending(t => t.Date).Take(3));

            BuildCategoryOptions();
            RecalculateStats();
        }

        // Thu nhap: +Amount | Chi tieu & Tiet kiem: -Amount (deu lam giam so du kha dung)
        private static decimal SignedAmount(TransactionItem t) =>
            t.Type == TransactionType.Income ? t.Amount : -t.Amount;

        private static decimal SumFor(TransactionType type, int month, int year) =>
            TransactionStore.Instance.Transactions
                .Where(t => t.Type == type && t.Date.Month == month && t.Date.Year == year)
                .Sum(t => t.Amount);

        private void BuildStatCards()
        {
            var lastMonth = Now.AddMonths(-1);

            decimal incomeThis = SumFor(TransactionType.Income, Now.Month, Now.Year);
            decimal expenseThis = SumFor(TransactionType.Expense, Now.Month, Now.Year);
            decimal savingThis = SumFor(TransactionType.Saving, Now.Month, Now.Year);

            decimal incomeLast = SumFor(TransactionType.Income, lastMonth.Month, lastMonth.Year);
            decimal expenseLast = SumFor(TransactionType.Expense, lastMonth.Month, lastMonth.Year);

            decimal balanceNow = TransactionStore.Instance.Transactions.Sum(SignedAmount);
            var firstDayThisMonth = new DateTime(Now.Year, Now.Month, 1);
            decimal balanceBeforeThisMonth = TransactionStore.Instance.Transactions
                .Where(t => t.Date < firstDayThisMonth)
                .Sum(SignedAmount);

            Balance = $"{balanceNow:N0}đ";
            if (balanceBeforeThisMonth != 0)
            {
                double pct = (double)((balanceNow - balanceBeforeThisMonth) / Math.Abs(balanceBeforeThisMonth) * 100);
                BalanceTrend = pct >= 0 ? $"↑ +{pct:0.#}% so tháng trước" : $"↓ {pct:0.#}% so tháng trước";
            }
            else
            {
                BalanceTrend = "Chưa có dữ liệu tháng trước";
            }

            Income = $"{incomeThis:N0}đ";
            var incomeDiff = incomeThis - incomeLast;
            IncomeTrend = incomeDiff >= 0
                ? $"↑ Tăng {incomeDiff:N0}đ so tháng trước"
                : $"↓ Giảm {Math.Abs(incomeDiff):N0}đ so tháng trước";

            Expense = $"{expenseThis:N0}đ";
            var expenseDiff = expenseThis - expenseLast;
            ExpenseTrend = expenseDiff >= 0
                ? $"↑ Tăng {expenseDiff:N0}đ so tháng trước"
                : $"↓ Giảm {Math.Abs(expenseDiff):N0}đ so tháng trước";

            Savings = $"{savingThis:N0}đ";
            SavingsTrend = incomeThis > 0
                ? $"{(double)(savingThis / incomeThis * 100):0.#}% thu nhập"
                : "Chưa có thu nhập tháng này";
        }

        private void BuildOverviewChart()
        {
            int monthsSoFar = Now.Month;

            double[] BuildMonthlySeries(TransactionType type)
            {
                var arr = new double[monthsSoFar];
                for (int m = 1; m <= monthsSoFar; m++)
                    arr[m - 1] = (double)SumFor(type, m, Now.Year);
                return arr;
            }

            double[] incomeData = BuildMonthlySeries(TransactionType.Income);
            double[] expenseData = BuildMonthlySeries(TransactionType.Expense);

            IncomeExpenseSeries = new ISeries[]
            {
                new LineSeries<double>
                {
                    Name = "Thu nhập",
                    Values = incomeData,
                    Stroke = new SolidColorPaint(SKColor.Parse("#3B82F6"), 3),
                    Fill = new SolidColorPaint(SKColor.Parse("#3B82F6").WithAlpha(40)),
                    GeometrySize = 0,
                    LineSmoothness = 0.5
                },
                new LineSeries<double>
                {
                    Name = "Chi tiêu",
                    Values = expenseData,
                    Stroke = new SolidColorPaint(SKColor.Parse("#F97316"), 3),
                    Fill = new SolidColorPaint(SKColor.Parse("#F97316").WithAlpha(30)),
                    GeometrySize = 0,
                    LineSmoothness = 0.5
                }
            };

            XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = Enumerable.Range(1, monthsSoFar).Select(i => $"T{i}").ToArray(),
                    SeparatorsPaint = null,
                    TextSize = 12
                }
            };

            YAxes = new Axis[]
            {
                new Axis
                {
                    Labeler = v => v >= 1_000_000 ? $"{v / 1_000_000:0.#}tr" : $"{v:N0}",
                    MinLimit = 0,
                    TextSize = 12,
                    SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#E5E7EB")) { StrokeThickness = 1 }
                }
            };
        }

        private void BuildCategoryDonut()
        {
            var expenseByCategory = TransactionStore.Instance.Transactions
                .Where(t => t.Type == TransactionType.Expense && t.Date.Month == Now.Month && t.Date.Year == Now.Year)
                .GroupBy(t => t.Category)
                .Select(g => new { Category = g.Key, Total = g.Sum(t => t.Amount) })
                .OrderByDescending(g => g.Total)
                .ToList();

            CategoryLegend = expenseByCategory.Select((c, i) => new CategorySlice
            {
                Name = c.Category,
                Value = (double)c.Total,
                ColorHex = Palette[i % Palette.Length]
            }).ToList();

            CategorySeries = CategoryLegend.Select(c => (ISeries)new PieSeries<double>
            {
                Values = new[] { c.Value },
                Name = c.Name,
                Fill = new SolidColorPaint(SKColor.Parse(c.ColorHex)),
                InnerRadius = 60,
                Stroke = null
            }).ToArray();
        }

        partial void OnSelectedPeriodChanged(string value) => RecalculateStats();
        partial void OnSelectedCategoryChanged(string value) => RecalculateStats();

        [RelayCommand]
        private void SetStatsPeriod(string period) => SelectedPeriod = period;

        private void BuildCategoryOptions()
        {
            CategoryOptions.Clear();
            CategoryOptions.Add("Tất cả");

            foreach (var category in TransactionStore.Instance.Transactions
                         .Select(t => t.Category).Distinct().OrderBy(c => c))
            {
                CategoryOptions.Add(category);
            }
        }

        private void RecalculateStats()
        {
            var filtered = TransactionStore.Instance.Transactions.AsEnumerable();

            if (SelectedCategory != "Tất cả")
                filtered = filtered.Where(t => t.Category == SelectedCategory);

            var data = filtered.ToList();

            List<string> labels;
            List<double> incomeValues;
            List<double> expenseValues;

            switch (SelectedPeriod)
            {
                case "Ngày":
                    (labels, incomeValues, expenseValues) = GroupByDay(data, days: 14);
                    break;
                case "Tuần":
                    (labels, incomeValues, expenseValues) = GroupByWeek(data, weeks: 8);
                    break;
                default: // "Tháng"
                    (labels, incomeValues, expenseValues) = GroupByMonth(data, months: 6);
                    break;
            }

            StatsSeries = new ISeries[]
            {
                new ColumnSeries<double>
                {
                    Name = "Thu nhập", Values = incomeValues,
                    Fill = new SolidColorPaint(SKColor.Parse("#3B82F6")),
                    Rx = 4, Ry = 4
                },
                new ColumnSeries<double>
                {
                    Name = "Chi tiêu", Values = expenseValues,
                    Fill = new SolidColorPaint(SKColor.Parse("#F97316")),
                    Rx = 4, Ry = 4
                }
            };

            StatsXAxes = new Axis[]
            {
                new Axis { Labels = labels.ToArray(), TextSize = 11, LabelsRotation = 0 }
            };

            StatsYAxes = new Axis[]
            {
                new Axis
                {
                    Labeler = v => v >= 1_000_000 ? $"{v / 1_000_000:0.#}tr" : $"{v:N0}",
                    MinLimit = 0,
                    TextSize = 11,
                    SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#E5E7EB")) { StrokeThickness = 1 }
                }
            };

            OnPropertyChanged(nameof(StatsSeries));
            OnPropertyChanged(nameof(StatsXAxes));
            OnPropertyChanged(nameof(StatsYAxes));
        }

        private static (List<string>, List<double>, List<double>) GroupByDay(List<TransactionItem> data, int days)
        {
            var labels = new List<string>();
            var income = new List<double>();
            var expense = new List<double>();
            var today = Now.Date;

            for (int i = days - 1; i >= 0; i--)
            {
                var day = today.AddDays(-i);
                labels.Add(day.ToString("dd/MM"));
                income.Add((double)data.Where(t => t.Type == TransactionType.Income && t.Date.Date == day).Sum(t => t.Amount));
                expense.Add((double)data.Where(t => t.Type == TransactionType.Expense && t.Date.Date == day).Sum(t => t.Amount));
            }
            return (labels, income, expense);
        }

        private static (List<string>, List<double>, List<double>) GroupByWeek(List<TransactionItem> data, int weeks)
        {
            var labels = new List<string>();
            var income = new List<double>();
            var expense = new List<double>();
            var today = Now.Date;

            int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday) % 7) % 7;
            var currentWeekStart = today.AddDays(-diff);

            for (int i = weeks - 1; i >= 0; i--)
            {
                var weekStart = currentWeekStart.AddDays(-7 * i);
                var weekEnd = weekStart.AddDays(6);
                labels.Add(weekStart.ToString("dd/MM"));
                income.Add((double)data.Where(t => t.Type == TransactionType.Income && t.Date.Date >= weekStart && t.Date.Date <= weekEnd).Sum(t => t.Amount));
                expense.Add((double)data.Where(t => t.Type == TransactionType.Expense && t.Date.Date >= weekStart && t.Date.Date <= weekEnd).Sum(t => t.Amount));
            }
            return (labels, income, expense);
        }

        private static (List<string>, List<double>, List<double>) GroupByMonth(List<TransactionItem> data, int months)
        {
            var labels = new List<string>();
            var income = new List<double>();
            var expense = new List<double>();
            var current = new DateTime(Now.Year, Now.Month, 1);

            for (int i = months - 1; i >= 0; i--)
            {
                var month = current.AddMonths(-i);
                labels.Add(month.ToString("MM/yyyy"));
                income.Add((double)data.Where(t => t.Type == TransactionType.Income && t.Date.Year == month.Year && t.Date.Month == month.Month).Sum(t => t.Amount));
                expense.Add((double)data.Where(t => t.Type == TransactionType.Expense && t.Date.Year == month.Year && t.Date.Month == month.Month).Sum(t => t.Amount));
            }
            return (labels, income, expense);
        }
    }
}