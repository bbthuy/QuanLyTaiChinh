using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace QuanLyTaiChinh.Models
{
    public partial class BudgetItem : ObservableObject
    {
        public int Id { get; set; }
        public string Icon { get; set; } = "💰";
        public string Category { get; set; } = "";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Percent))]
        [NotifyPropertyChangedFor(nameof(ProgressPercent))]
        [NotifyPropertyChangedFor(nameof(RemainingProgressPercent))]
        [NotifyPropertyChangedFor(nameof(RemainingAmount))]
        [NotifyPropertyChangedFor(nameof(StatusColor))]
        [NotifyPropertyChangedFor(nameof(StatusMessage))]
        [NotifyPropertyChangedFor(nameof(StatusIcon))]
        private decimal limitAmount;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Percent))]
        [NotifyPropertyChangedFor(nameof(ProgressPercent))]
        [NotifyPropertyChangedFor(nameof(RemainingProgressPercent))]
        [NotifyPropertyChangedFor(nameof(RemainingAmount))]
        [NotifyPropertyChangedFor(nameof(StatusColor))]
        [NotifyPropertyChangedFor(nameof(StatusMessage))]
        [NotifyPropertyChangedFor(nameof(StatusIcon))]
        private decimal spentAmount;

        // % da chi so voi han muc, toi da hien thi 999% de khong vo layout khi vuot qua nhieu
        public double Percent => LimitAmount <= 0
            ? 0
            : Math.Min(999, Math.Round((double)(SpentAmount / LimitAmount) * 100));

        // Rieng thanh tien do: cham o 100% du chi vuot bao nhieu, tranh cot am
        public double ProgressPercent => Math.Min(100, Percent);
        public double RemainingProgressPercent => 100 - ProgressPercent;

        // Con lai duong neu chua tieu het, am neu da vuot (hien "vuot X d")
        public decimal RemainingAmount => LimitAmount - SpentAmount;

        public bool IsOverBudget => SpentAmount > LimitAmount;

        // Blue: binh thuong | Green: con du nhieu | Orange: sap het | Red: da vuot
        public string StatusColor => Percent switch
        {
            >= 100 => "Red",
            >= 80 => "Orange",
            <= 30 => "Green",
            _ => "Blue"
        };

        public string StatusIcon => Percent switch
        {
            >= 100 => "⚠️",
            >= 80 => "⏰",
            <= 30 => "✅",
            _ => "📊"
        };

        public string StatusMessage => Percent switch
        {
            >= 100 => $"Đã chi vượt {Math.Abs(RemainingAmount):N0}đ so với ngân sách!",
            >= 80 => $"Sắp đạt giới hạn, còn {RemainingAmount:N0}đ",
            <= 30 => $"Đang chi tiêu tiết kiệm, còn dư {RemainingAmount:N0}đ",
            _ => $"Trong tầm kiểm soát, còn {RemainingAmount:N0}đ"
        };
    }
}