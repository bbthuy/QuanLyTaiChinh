using System.Collections.Generic;
using System.Linq;
using System.Windows;
using QuanLyTaiChinh.Models;

namespace QuanLyTaiChinh.View
{
    public partial class BudgetEditWindow : Window
    {
        public BudgetItem ResultBudget { get; private set; } = new();

        private readonly List<string> _existingCategories;

        // Them moi: truyen vao danh sach danh muc DA co ngan sach, de chan trung
        public BudgetEditWindow(List<string> existingCategories)
        {
            InitializeComponent();
            _existingCategories = existingCategories;
        }

        // Sua: truyen them ngan sach dang sua de dien san du lieu
        public BudgetEditWindow(List<string> existingCategories, BudgetItem existing) : this(existingCategories)
        {
            Title = "Sửa ngân sách";
            CategoryCombo.Text = existing.Category;
            IconBox.Text = existing.Icon;
            LimitAmountBox.Text = existing.LimitAmount.ToString();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string category = CategoryCombo.Text?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(category))
            {
                ShowError("Vui lòng chọn hoặc nhập danh mục.");
                return;
            }

            if (_existingCategories.Any(c => c.Equals(category, System.StringComparison.OrdinalIgnoreCase)))
            {
                ShowError($"Danh mục \"{category}\" đã có ngân sách rồi. Mỗi danh mục chỉ đặt 1 ngân sách/tháng.");
                return;
            }

            if (!decimal.TryParse(LimitAmountBox.Text, out var limit) || limit <= 0)
            {
                ShowError("Hạn mức không hợp lệ.");
                return;
            }

            ResultBudget = new BudgetItem
            {
                Category = category,
                Icon = string.IsNullOrWhiteSpace(IconBox.Text) ? "💰" : IconBox.Text.Trim(),
                LimitAmount = limit
            };

            DialogResult = true;
        }

        private void ShowError(string message)
        {
            ErrorText.Text = message;
            ErrorText.Visibility = Visibility.Visible;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}