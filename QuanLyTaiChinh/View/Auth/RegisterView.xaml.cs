using QuanLyTaiChinh.Services;
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

namespace QuanLyTaiChinh.View.Auth;

public partial class RegisterView : UserControl
{
    private readonly AuthService _authService = new();

    public RegisterView()
    {
        InitializeComponent();
    }

    private async void Register_Click(object sender, RoutedEventArgs e)
    {
        if (txtPassword.Password != txtConfirmPassword.Password)
        {
            AuthMessageWindow.ShowMessage("Xác nhận mật khẩu không khớp.");
            return;
        }

        if (chkTerms.IsChecked != true)
        {
            AuthMessageWindow.ShowMessage("Vui lòng đồng ý với điều khoản sử dụng.");
            return;
        }

        string email = txtEmail.Text.Trim();

        try
        {
            var result = await _authService.RegisterAsync(
                txtFullName.Text,
                email,
                txtPassword.Password);
            if (!result.Success)
            {
                if (result.Message.Contains("đã được đăng ký",
    StringComparison.OrdinalIgnoreCase))
                {
                    bool? verified =
                        await _authService.GetEmailVerifiedStatusAsync(email);

                    if (verified == false)
                    {
                        AuthMessageWindow.ShowMessage(
                            "Email này đã tạo tài khoản nhưng chưa xác minh. Hãy gửi lại mã; " +
                            "sau khi xác minh, đăng nhập bằng mật khẩu đã đăng ký.");
                        ((MainWindow)Application.Current.MainWindow)
                            .Navigate(new VerifyEmailView(email));
                    }
                    else
                    {
                        AuthMessageWindow.ShowMessage(
                            "Email này đã có tài khoản. Hãy đăng nhập hoặc dùng Quên mật khẩu.");
                        ((MainWindow)Application.Current.MainWindow)
                            .Navigate(new LoginView());
                    }

                    return;
                }

                AuthMessageWindow.ShowMessage(result.Message);
                return;
            }

            string? code =
                await _authService.CreateEmailVerificationCodeAsync(email);

            if (code == null)
            {
                AuthMessageWindow.ShowMessage("Không tạo được mã xác minh.");
                return;
            }

            try
            {
                await new EmailService().SendOtpAsync(email, code, "VERIFY_EMAIL");
                AuthMessageWindow.ShowMessage("Đã gửi mã xác minh đến email của bạn.");
            }
            catch (Exception ex)
            {
                AuthMessageWindow.ShowMessage(
                    $"Tài khoản đã được lưu nhưng chưa gửi được email: {ex.Message}\n" +
                    "Hãy bấm Gửi lại mã ở màn hình tiếp theo.");
            }

            ((MainWindow)Application.Current.MainWindow)
                .Navigate(new VerifyEmailView(email));
        }
        catch (Exception ex)
        {
            AuthMessageWindow.ShowMessage($"Không kết nối/lưu được dữ liệu: {ex.Message}");
        }
    }

    private void TogglePassword_Click(object sender, RoutedEventArgs e)
    {
        bool confirm = ReferenceEquals(sender, btnConfirmEye);
        PasswordBox hidden = confirm ? txtConfirmPassword : txtPassword;
        TextBox visible = confirm ? txtConfirmPasswordVisible : txtPasswordVisible;
        Button eye = confirm ? btnConfirmEye : btnPasswordEye;

        bool show = visible.Visibility != Visibility.Visible;
        if (show)
        {
            visible.Text = hidden.Password;
            hidden.Visibility = Visibility.Collapsed;
            visible.Visibility = Visibility.Visible;
        }
        else
        {
            hidden.Password = visible.Text;
            visible.Visibility = Visibility.Collapsed;
            hidden.Visibility = Visibility.Visible;
        }

        eye.ToolTip = show ? "Ẩn mật khẩu" : "Hiện mật khẩu";
    }

    private void VisiblePassword_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (ReferenceEquals(sender, txtPasswordVisible))
            txtPassword.Password = txtPasswordVisible.Text;
        else if (ReferenceEquals(sender, txtConfirmPasswordVisible))
            txtConfirmPassword.Password = txtConfirmPasswordVisible.Text;
    }

    private void Login_Click(object sender, RoutedEventArgs e)
    {
        ((MainWindow)Application.Current.MainWindow)
            .Navigate(new LoginView());
    }
}