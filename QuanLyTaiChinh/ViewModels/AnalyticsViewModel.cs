using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using QuanLyTaiChinh.Core.Services;
using SkiaSharp;

namespace QuanLyTaiChinh.ViewModels
{
    public partial class AnalyticsViewModel : ObservableObject
    {
        private readonly AnalyticsService _analyticsService;

        private ISeries[] _incomeExpenseSeries;
        public ISeries[] IncomeExpenseSeries
        {
            get => _incomeExpenseSeries;
            set => SetProperty(ref _incomeExpenseSeries, value);
        }

        private Axis[] _incomeExpenseXAxes;
        public Axis[] IncomeExpenseXAxes
        {
            get => _incomeExpenseXAxes;
            set => SetProperty(ref _incomeExpenseXAxes, value);
        }

        public AnalyticsViewModel()
        {
            _analyticsService = new AnalyticsService();
            LoadChartDataFromDatabase();
        }

        private void LoadChartDataFromDatabase()
        {
            var dbData = _analyticsService.GetIncomeExpenseData(2026);

            IncomeExpenseSeries = new ISeries[]
            {
                new ColumnSeries<double>
                {
                    Name = "Thu nhập",
                    Values = dbData.Incomes,
                    Fill = new SolidColorPaint(SKColor.Parse("#38BDF8")),
                    MaxBarWidth = 35,
                    Rx = 4,
                    Ry = 4
                },

                new ColumnSeries<double>
                {
                    Name = "Chi tiêu",
                    Values = dbData.Expenses,
                    Fill = new SolidColorPaint(SKColor.Parse("#FCA5A5")),
                    MaxBarWidth = 35,
                    Rx = 4,
                    Ry = 4
                }
            };

            IncomeExpenseXAxes = new Axis[]
            {
                new Axis
                {
                    Labels = dbData.Months
                }
            };
        }
    }
}