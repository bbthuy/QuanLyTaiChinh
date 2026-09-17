using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using QuanLyTaiChinh.Core.Services; // Thêm namespace chứa Service

namespace QuanLyTaiChinh.ViewModels
{
    public class AnalyticsViewModel : INotifyPropertyChanged
    {
        private readonly AnalyticsService _analyticsService;

        public ISeries[] IncomeExpenseSeries { get; set; }
        public Axis[] IncomeExpenseXAxes { get; set; }

        public AnalyticsViewModel()
        {
            // Khởi tạo service (Trong thực tế nên dùng Dependency Injection)
            _analyticsService = new AnalyticsService();

            LoadChartDataFromDatabase();
        }

        private void LoadChartDataFromDatabase()
        {
            // 1. Gọi Service để kéo dữ liệu từ DB (Ví dụ năm 2026)
            var dbData = _analyticsService.GetIncomeExpenseData(2026);

            // 2. Map dữ liệu vào Chart
            IncomeExpenseSeries = new ISeries[]
            {
                new ColumnSeries<double>
                {
                    Name = "Thu nhập",
                    Values = dbData.Incomes, // <-- Đẩy mảng double từ DB vào đây
                    Fill = new SolidColorPaint(SKColor.Parse("#38BDF8")),
                    MaxBarWidth = 35,
                    Rx = 4, Ry = 4
                },
                new ColumnSeries<double>
                {
                    Name = "Chi tiêu",
                    Values = dbData.Expenses, // <-- Đẩy mảng double từ DB vào đây
                    Fill = new SolidColorPaint(SKColor.Parse("#FCA5A5")),
                    MaxBarWidth = 35,
                    Rx = 4, Ry = 4
                }
            };

            // 3. Map trục X (Tên tháng)
            IncomeExpenseXAxes = new Axis[]
            {
                new Axis { Labels = dbData.Months }
            };

            // Thông báo cho giao diện biết dữ liệu đã thay đổi (nếu lấy DB bất đồng bộ)
            OnPropertyChanged(nameof(IncomeExpenseSeries));
            OnPropertyChanged(nameof(IncomeExpenseXAxes));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}