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
using QuanLyTaiChinh.Services;

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

        private async void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            var main = (MainWindow)Application.Current.MainWindow;

            if (main.CurrentUser == null)
            {
                main.ShowLogin();
                return;
            }

            string current = txtCurrentPassword.Password;
            string newPassword = txtNewPassword.Password;

            if (string.IsNullOrWhiteSpace(current))
            {
                AuthMessageWindow.ShowMessage("Vui lòng nhập mật khẩu hiện tại.");
                return;
            }

            if (newPassword.Length < 8)
            {
                AuthMessageWindow.ShowMessage("Mật khẩu mới phải có ít nhất 8 ký tự.");
                return;
            }

            if (newPassword != txtConfirmPassword.Password)
            {
                AuthMessageWindow.ShowMessage("Xác nhận mật khẩu không khớp.");
                return;
            }

            if (current == newPassword)
            {
                AuthMessageWindow.ShowMessage("Mật khẩu mới phải khác mật khẩu hiện tại.");
                return;
            }

            try
            {
                var authService = new AuthService();

                bool success = await authService.ChangePasswordAsync(
                    main.CurrentUser.UserId, current, newPassword);

                if (!success)
                {
                    AuthMessageWindow.ShowMessage("Mật khẩu hiện tại không đúng.");
                    return;
                }

                AuthMessageWindow.ShowMessage("Đổi mật khẩu thành công.");
                main.ShowMainApp();
            }
            catch (Exception ex)
            {
                AuthMessageWindow.ShowMessage($"Không đổi được mật khẩu: {ex.Message}");
            }
        }
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow =
                Application.Current.MainWindow as MainWindow;

            if (mainWindow != null)
            {
                mainWindow.ShowMainApp();
            }
        }
    }
}
