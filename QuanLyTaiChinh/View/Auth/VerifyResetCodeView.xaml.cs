using QuanLyTaiChinh.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace QuanLyTaiChinh.View.Auth;

public partial class VerifyResetCodeView : UserControl
{
    private readonly string _email;

    public VerifyResetCodeView(string email)
    {
        InitializeComponent();
        _email = email;
        _otpTimer.Tick += (_, _) => UpdateCountdown();
        Loaded += async (_, _) => await RefreshCountdownAsync();
        Unloaded += (_, _) => _otpTimer.Stop();
    }

    private readonly DispatcherTimer _otpTimer = new()
    {
        Interval = TimeSpan.FromSeconds(1)
    };

    private DateTime? _expiresAtUtc;

    private async Task RefreshCountdownAsync()
    {
        _otpTimer.Stop();
        _expiresAtUtc = await new AuthService().GetResetExpiryAsync(_email);
        UpdateCountdown();

        if (_expiresAtUtc > DateTime.UtcNow)
            _otpTimer.Start();
    }

    private void UpdateCountdown()
    {
        if (_expiresAtUtc == null)
        {
            txtCountdown.Text = "Chưa có mã đặt lại mật khẩu.";
            return;
        }

        int seconds = Math.Max(0,
            (int)Math.Ceiling((_expiresAtUtc.Value - DateTime.UtcNow).TotalSeconds));

        txtCountdown.Text = seconds == 0
            ? "Mã đã hết hạn. Hãy yêu cầu mã mới."
            : $"Mã còn hiệu lực {seconds / 60:00}:{seconds % 60:00}";

        if (seconds == 0) _otpTimer.Stop();
    }
    private async void Continue_Click(object sender, RoutedEventArgs e)
    {
        string code = txtOtp.Text.Trim();

        if (code.Length != 6 || !code.All(char.IsDigit))
        {
            AuthMessageWindow.ShowMessage("Vui lòng nhập mã gồm 6 chữ số.");
            return;
        }

        try
        {
            var authService = new AuthService();
            bool valid = await authService.VerifyResetCodeAsync(_email, code);

            if (!valid)
            {
                AuthMessageWindow.ShowMessage("Mã không đúng hoặc đã hết hạn.");
                return;
            }

            ((MainWindow)Application.Current.MainWindow)
                .Navigate(new ResetPasswordView(_email, code));
        }
        catch (Exception ex)
        {
            AuthMessageWindow.ShowMessage($"Không kiểm tra được mã: {ex.Message}");
        }
    }


}