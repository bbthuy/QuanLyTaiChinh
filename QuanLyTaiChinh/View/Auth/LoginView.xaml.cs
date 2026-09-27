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
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void TogglePassword_Click(object sender, RoutedEventArgs e)
        {
            bool show = txtPasswordVisible.Visibility != Visibility.Visible;

            if (show)
            {
                txtPasswordVisible.Text = txtPassword.Password;
                txtPassword.Visibility = Visibility.Collapsed;
                txtPasswordVisible.Visibility = Visibility.Visible;
                btnEye.ToolTip = "Ẩn mật khẩu";
            }
            else
            {
                txtPassword.Password = txtPasswordVisible.Text;
                txtPasswordVisible.Visibility = Visibility.Collapsed;
                txtPassword.Visibility = Visibility.Visible;
                btnEye.ToolTip = "Hiện mật khẩu";
            }
        }

        private void VisiblePassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtPassword.Password = txtPasswordVisible.Text;
        }
        private void Register_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow =
                Application.Current.MainWindow as MainWindow;

            if (mainWindow != null)
            {
                mainWindow.Navigate(new RegisterView());
            }
        }

        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow =
                Application.Current.MainWindow as MainWindow;

            if (mainWindow != null)
            {
                mainWindow.Navigate(new ForgotPasswordView());
            }
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var authService = new AuthService();

                var user = await authService.LoginAsync(
                    txtEmail.Text,
                    txtPassword.Password);

                if (user == null)
                {
                    AuthMessageWindow.ShowMessage(
                        "Email hoặc mật khẩu không đúng, hoặc email chưa được xác minh.");
                    return;
                }

                ((MainWindow)Application.Current.MainWindow).SignIn(user);
            }
            catch (Exception ex)
            {
                AuthMessageWindow.ShowMessage($"Không đăng nhập được: {ex.Message}");
            }
        }
    }
}
