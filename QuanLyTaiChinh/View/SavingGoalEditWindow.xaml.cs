using QuanLyTaiChinh.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace QuanLyTaiChinh.View
{
    public partial class SavingGoalEditWindow : Window
    {
        public SavingGoalItem ResultGoal { get; private set; } = new();

        public SavingGoalEditWindow()
        {
            InitializeComponent();
            ColorCombo.SelectedIndex = 0;
        }

        // Constructor dung khi Sua muc tieu (truyen du lieu co san vao form)
        public SavingGoalEditWindow(SavingGoalItem existing) : this()
        {
            Title = "Sửa mục tiêu";
            NameBox.Text = existing.Name;
            IconBox.Text = existing.Icon;
            CurrentAmountBox.Text = existing.CurrentAmount.ToString();
            TargetAmountBox.Text = existing.TargetAmount.ToString();

            if (existing.DeadlineLabel.Contains("Liên tục"))
            {
                NoDeadlineCheck.IsChecked = true;
                DeadlineBox.Text = "";
            }
            else
            {
                // DeadlineLabel dang luu dang "Mục tiêu: 12/2026" -> chi lay phan sau dau ":"
                var parts = existing.DeadlineLabel.Split(':');
                DeadlineBox.Text = parts.Length > 1 ? parts[1].Trim() : "";
            }

            ColorCombo.SelectedIndex = existing.AccentColor switch
            {
                "Purple" => 1,
                "Green" => 2,
                "Orange" => 3,
                _ => 0
            };
        }

        private void NoDeadlineCheck_Changed(object sender, RoutedEventArgs e)
        {
            bool noDeadline = NoDeadlineCheck.IsChecked == true;
            DeadlineBox.IsEnabled = !noDeadline;
            if (noDeadline) DeadlineBox.Text = "";
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Vui lòng nhập tên mục tiêu.");
                return;
            }

            if (!decimal.TryParse(TargetAmountBox.Text, out var target) || target <= 0)
            {
                MessageBox.Show("Số tiền mục tiêu không hợp lệ.");
                return;
            }

            decimal.TryParse(CurrentAmountBox.Text, out var current); // mac dinh 0 neu nhap sai

            string accentColor = (ColorCombo.SelectedItem as ComboBoxItem)?.Content?.ToString() switch
            {
                "Tím" => "Purple",
                "Xanh lá" => "Green",
                "Cam" => "Orange",
                _ => "Blue"
            };

            string deadlineLabel = NoDeadlineCheck.IsChecked == true
                ? "Mục tiêu: Liên tục"
                : $"Mục tiêu: {DeadlineBox.Text.Trim()}";

            ResultGoal = new SavingGoalItem
            {
                Name = NameBox.Text.Trim(),
                Icon = string.IsNullOrWhiteSpace(IconBox.Text) ? "🎯" : IconBox.Text.Trim(),
                DeadlineLabel = deadlineLabel,
                AccentColor = accentColor,
                CurrentAmount = current,
                TargetAmount = target
            };

            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}
