using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLyTaiChinh.Models
{
    public partial class SavingGoalItem : ObservableObject
    {
        public int Id { get; set; }
        public string Icon { get; set; } = "🎯";
        public string Name { get; set; } = "";
        public string DeadlineLabel { get; set; } = ""; 
        public string AccentColor { get; set; } = "Blue"; 

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Percent))]
        [NotifyPropertyChangedFor(nameof(RemainingPercent))]
        [NotifyPropertyChangedFor(nameof(RemainingAmount))]
        private decimal currentAmount;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Percent))]
        [NotifyPropertyChangedFor(nameof(RemainingPercent))]
        [NotifyPropertyChangedFor(nameof(RemainingAmount))]
        private decimal targetAmount;

        public double Percent => TargetAmount <= 0
            ? 0
            : Math.Min(100, Math.Round((double)(CurrentAmount / TargetAmount) * 100));

        public double RemainingPercent => 100 - Percent;

        public decimal RemainingAmount => Math.Max(0, TargetAmount - CurrentAmount);
    }
}

