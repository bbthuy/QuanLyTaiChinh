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
using System.Linq;


namespace QuanLyTaiChinh.View
{
    /// <summary>
    /// Interaction logic for TransactionEditWindow.xaml
    /// </summary>
    public partial class TransactionEditWindow : Window
    {
        private void Header_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) DragMove();
        }

        private void IconTile_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb && IconBox != null)
                IconBox.Text = rb.Content?.ToString() ?? "";
        }

        private void IconBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IconPanel == null) return;
            foreach (var rb in IconPanel.Children.OfType<RadioButton>())
                rb.IsChecked = rb.Content?.ToString() == IconBox.Text;
        }
        public TransactionItem ResultTransaction { get; private set; } = new();
        public TransactionEditWindow()
        {
            InitializeComponent();
            TypeExpense.IsChecked = true;
            DatePickerCtrl.SelectedDate = DateTime.Today;
        }
        // Constructor dung khi Sua giao dich (truyen du lieu co san vao form)
        public TransactionEditWindow(TransactionItem existing) : this()
        {
            Title = "Sửa giao dịch";
            NameBox.Text = existing.Name;
            CategoryBox.Text = existing.Category;
            AmountBox.Text = existing.Amount.ToString();
            DatePickerCtrl.SelectedDate = existing.Date;
            IconBox.Text = existing.Icon;
            switch (existing.Type)
            {
                case TransactionType.Income: TypeIncome.IsChecked = true; break;
                case TransactionType.Saving: TypeSaving.IsChecked = true; break;
                default: TypeExpense.IsChecked = true; break;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Vui lòng nhập tên giao dịch.");
                return;
            }

            if (!decimal.TryParse(AmountBox.Text, out var amount) || amount <= 0)
            {
                MessageBox.Show("Số tiền không hợp lệ.");
                return;
            }

            ResultTransaction = new TransactionItem
            {
                Name = NameBox.Text.Trim(),
                Category = string.IsNullOrWhiteSpace(CategoryBox.Text) ? "Khác" : CategoryBox.Text.Trim(),
                Amount = amount,
                Date = DatePickerCtrl.SelectedDate ?? DateTime.Today,
                Type = TypeIncome.IsChecked == true ? TransactionType.Income
         : TypeSaving.IsChecked == true ? TransactionType.Saving
         : TransactionType.Expense,
                Icon = string.IsNullOrWhiteSpace(IconBox.Text) ? "💰" : IconBox.Text
            };

            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}
