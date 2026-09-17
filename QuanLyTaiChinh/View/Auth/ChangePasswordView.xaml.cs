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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuanLyTaiChinh.View.Auth
{
    /// <summary>
    /// Interaction logic for ChangePasswordView.xaml
    /// </summary>
    public partial class ChangePasswordView : UserControl
    {
        public ChangePasswordView()
        {
            InitializeComponent();
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            // UI test trước, DB làm sau

            if (string.IsNullOrWhiteSpace(txtCurrentPassword.Password))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu hiện tại.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNewPassword.Password))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu mới.");
                return;
            }

            if (txtNewPassword.Password.Length < 8)
            {
                MessageBox.Show("Mật khẩu mới phải có ít nhất 8 ký tự.");
                return;
            }

            if (txtNewPassword.Password != txtConfirmPassword.Password)
            {
                MessageBox.Show("Xác nhận mật khẩu không khớp.");
                return;
            }

            MessageBox.Show(
                "Đổi mật khẩu thành công!",
                "FinanceWise",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            // Sau này:
            // kiểm tra mật khẩu cũ từ DB
            // hash password mới bằng BCrypt
            // update Users.PasswordHash
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            // Tạm thời quay về Dashboard
            ((MainWindow)Application.Current.MainWindow)
                .Navigate(new QuanLyTaiChinh.View.Dashboard.DashboardView());
        }
    }
}
