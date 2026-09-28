using QuanLyTaiChinh.Services;
using System.Windows;
using System.Windows.Controls;

namespace QuanLyTaiChinh.View.Auth;

public partial class ResetPasswordView : UserControl
{
    private readonly string _email;
    private readonly string _code;

    public ResetPasswordView(string email, string code)
    {
        InitializeComponent();
        _email = email;
        _code = code;
    }

    private async void Reset_Click(object sender, RoutedEventArgs e)
    {
        string newPassword = txtNewPasswordVisible.Visibility == Visibility.Visible
     ? txtNewPasswordVisible.Text
     : txtNewPassword.Password;

        string confirmPassword = txtConfirmPasswordVisible.Visibility == Visibility.Visible
            ? txtConfirmPasswordVisible.Text
            : txtConfirmPassword.Password;

        if (newPassword.Length < 8)
        {
            AuthMessageWindow.ShowMessage("Mật khẩu mới phải có ít nhất 8 ký tự.");
            return;
        }

        if (newPassword != confirmPassword)
        {
            AuthMessageWindow.ShowMessage("Xác nhận mật khẩu không khớp.");
            return;
        }

        try
        {
            var authService = new AuthService();

            bool success = await authService.ResetPasswordAsync(
                _email, _code, newPassword);

            if (!success)
            {
                AuthMessageWindow.ShowMessage("Mã đã dùng hoặc hết hạn. Vui lòng yêu cầu mã mới.");
                return;
            }

            AuthMessageWindow.ShowMessage("Đặt lại mật khẩu thành công.");
            ((MainWindow)Application.Current.MainWindow)
                .Navigate(new LoginView());
        }
        catch (Exception ex)
        {
            AuthMessageWindow.ShowMessage($"Không đặt lại được mật khẩu: {ex.Message}");
        }
    }

    private static void TogglePassword(PasswordBox hidden, TextBox visible)
    {
        if (visible.Visibility == Visibility.Collapsed)
        {
            visible.Text = hidden.Password;
            hidden.Visibility = Visibility.Collapsed;
            visible.Visibility = Visibility.Visible;
            visible.Focus();
            visible.CaretIndex = visible.Text.Length;
        }
        else
        {
            hidden.Password = visible.Text;
            visible.Visibility = Visibility.Collapsed;
            hidden.Visibility = Visibility.Visible;
            hidden.Focus();
        }
    }

    private void ToggleNewPassword_Click(object sender, RoutedEventArgs e)
        => TogglePassword(txtNewPassword, txtNewPasswordVisible);

    private void ToggleConfirmPassword_Click(object sender, RoutedEventArgs e)
        => TogglePassword(txtConfirmPassword, txtConfirmPasswordVisible);
}