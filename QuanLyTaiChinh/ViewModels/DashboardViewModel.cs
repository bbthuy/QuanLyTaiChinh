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

        // ===== 4 THE SO LIEU =====
        [ObservableProperty] private string balance = "16.400.000đ";
        [ObservableProperty] private string balanceTrend = "↑ +8.2% so tháng trước";

        [ObservableProperty] private string income = "32.500.000đ";
        [ObservableProperty] private string incomeTrend = "↑ Tăng 3.5tr so tháng trước";

        [ObservableProperty] private string expense = "16.100.000đ";
        [ObservableProperty] private string expenseTrend = "↓ Giảm 1.2tr so tháng trước";

        [ObservableProperty] private string savings = "4.000.000đ";
        [ObservableProperty] private string savingsTrend = "12.3% thu nhập";

        // ===== BIEU DO DUONG TONG QUAN (khong loc) =====
        public ISeries[] IncomeExpenseSeries { get; set; }
        public Axis[] XAxes { get; set; }
        public Axis[] YAxes { get; set; }

        // ===== BIEU DO DONUT: CO CAU CHI TIEU =====
        public ISeries[] CategorySeries { get; set; }
        public List<CategorySlice> CategoryLegend { get; set; }

        // ===== GIAO DICH GAN DAY =====
        public ObservableCollection<TransactionItem> RecentTransactions { get; set; }

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
            // TODO: thay toan bo du lieu mau ben duoi bang du lieu that
            // lay tu TransactionService/AnalyticsService khi co database

            int monthsSoFar = Now.Month;

            double[] incomeData = GenerateSample(monthsSoFar, baseValue: 22, amplitude: 3);
            double[] expenseData = GenerateSample(monthsSoFar, baseValue: 15, amplitude: 2);

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
                    Labeler = value => $"{value:N0}tr",
                    MinLimit = 0,
                    TextSize = 12,
                    SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#E5E7EB")) { StrokeThickness = 1 }
                }
            };

            // ---- Co cau chi tieu ----
            CategoryLegend = new List<CategorySlice>
            {
                new CategorySlice { Name = "Ăn uống",    Value = 35, ColorHex = "#3B82F6" },
                new CategorySlice { Name = "Di chuyển",  Value = 10, ColorHex = "#6EE7B7" },
                new CategorySlice { Name = "Mua sắm",    Value = 15, ColorHex = "#A78BFA" },
                new CategorySlice { Name = "Giải trí",   Value = 10, ColorHex = "#FBBF24" },
                new CategorySlice { Name = "Sức khỏe",   Value = 10, ColorHex = "#F87171" },
                new CategorySlice { Name = "Tiết kiệm",  Value = 20, ColorHex = "#93C5FD" },
            };

            CategorySeries = CategoryLegend.Select(c => (ISeries)new PieSeries<double>
            {
                Values = new[] { c.Value },
                Name = c.Name,
                Fill = new SolidColorPaint(SKColor.Parse(c.ColorHex)),
                InnerRadius = 60,
                Stroke = null
            }).ToArray();

            // ---- Giao dich gan day ----
            RecentTransactions = new ObservableCollection<TransactionItem>(
                TransactionStore.Instance.Transactions.OrderByDescending(t => t.Date).Take(3));

            // ---- Thong ke thu/chi co loc ----
            BuildCategoryOptions();
            RecalculateStats();
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

        private static double[] GenerateSample(int months, double baseValue, double amplitude)
        {
            var data = new double[months];
            for (int i = 0; i < months; i++)
                data[i] = Math.Round(baseValue + amplitude * Math.Sin(i * 0.9), 1);
            return data;
        }
    }
}