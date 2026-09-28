using QuanLyTaiChinh.Services;
using System.Windows;
using System.Windows.Controls;

namespace QuanLyTaiChinh.View.Auth;

public partial class ForgotPasswordView : UserControl
{
    public ForgotPasswordView()
    {
        InitializeComponent();
    }

    private async void SendOtp_Click(object sender, RoutedEventArgs e)
    {
        string email = txtEmail.Text.Trim();

        if (string.IsNullOrWhiteSpace(email))
        {
            AuthMessageWindow.ShowMessage("Vui lòng nhập email.");
            return;
        }

        try
        {
            var authService = new AuthService();
            string? code = await authService.RequestPasswordResetAsync(email);

            if (code != null)
            {
                await new EmailService().SendOtpAsync(
                    email, code, "RESET_PASSWORD");
            }

            AuthMessageWindow.ShowMessage("Nếu email có tài khoản, mã xác nhận đã được gửi.");

            ((MainWindow)Application.Current.MainWindow)
                .Navigate(new VerifyResetCodeView(email));
        }
        catch (Exception ex)
        {
            AuthMessageWindow.ShowMessage($"Không gửi được mã: {ex.Message}");
        }
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        ((MainWindow)Application.Current.MainWindow)
            .Navigate(new LoginView());
    }
}