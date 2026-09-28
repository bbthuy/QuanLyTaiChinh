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
    /// <summary>
    /// Interaction logic for TransactionEditWindow.xaml
    /// </summary>
    public partial class TransactionEditWindow : Window
    {
        public TransactionItem ResultTransaction { get; private set; } = new();
        public TransactionEditWindow()
        {
            InitializeComponent();
            TypeCombo.SelectedIndex = 1; // mac dinh "Chi tieu"
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
            TypeCombo.SelectedIndex = existing.Type switch
            {
                TransactionType.Income => 0,
                TransactionType.Saving => 2,
                _ => 1
            };
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
                Type = TypeCombo.SelectedIndex switch
                {
                    0 => TransactionType.Income,
                    2 => TransactionType.Saving,
                    _ => TransactionType.Expense
                },
                Icon = "💰"
            };

            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}
