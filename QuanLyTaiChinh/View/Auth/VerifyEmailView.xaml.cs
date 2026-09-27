using QuanLyTaiChinh.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace QuanLyTaiChinh.View.Auth;

public partial class VerifyEmailView : UserControl
{
    private readonly string _email;
    private readonly AuthService _authService = new();

    public VerifyEmailView(string email)
    {
        InitializeComponent();
        _email = email;
        _otpTimer.Tick += (_, _) => UpdateCountdown();
        Loaded += async (_, _) => await RefreshCountdownAsync();
        Unloaded += (_, _) => _otpTimer.Stop();
    }

    private async void Verify_Click(object sender, RoutedEventArgs e)
    {
        string code = txtOtp.Text.Trim();

        if (code.Length != 6 || !code.All(char.IsDigit))
        {
            AuthMessageWindow.ShowMessage("Vui lòng nhập mã gồm 6 chữ số.");
            return;
        }

        try
        {
            bool success = await _authService.VerifyEmailAsync(_email, code);

            if (!success)
            {
                AuthMessageWindow.ShowMessage("Mã không đúng hoặc đã hết hạn.");
                return;
            }

            AuthMessageWindow.ShowMessage("Xác minh email thành công!");
            ((MainWindow)Application.Current.MainWindow)
                .Navigate(new LoginView());
        }
        catch (Exception ex)
        {
            AuthMessageWindow.ShowMessage($"Không xác minh được: {ex.Message}");
        }
    }

    private DateTime _lastResendUtc = DateTime.MinValue;

    private async void Resend_Click(object sender, RoutedEventArgs e)
    {
        if (DateTime.UtcNow - _lastResendUtc < TimeSpan.FromSeconds(60))
        {
            AuthMessageWindow.ShowMessage("Vui lòng chờ 60 giây trước khi gửi lại.");
            return;
        }

        var button = (Button)sender;
        button.IsEnabled = false;

        try
        {
            string? code =
                await _authService.CreateEmailVerificationCodeAsync(_email);

            if (code == null)
            {
                AuthMessageWindow.ShowMessage("Tài khoản không cần xác minh hoặc email không tồn tại.");
                return;
            }

            await new EmailService().SendOtpAsync(_email, code, "VERIFY_EMAIL");
            _lastResendUtc = DateTime.UtcNow;
            await RefreshCountdownAsync();
            AuthMessageWindow.ShowMessage("Đã gửi mã mới. Hãy kiểm tra hộp thư và thư rác.");
        }
        catch (Exception ex)
        {
            AuthMessageWindow.ShowMessage($"Chưa gửi được mã: {ex.Message}");
        }
        finally
        {
            button.IsEnabled = true;
        }
    }

    private readonly DispatcherTimer _otpTimer = new()
    {
        Interval = TimeSpan.FromSeconds(1)
    };

    private DateTime? _expiresAtUtc;

    private async Task RefreshCountdownAsync()
    {
        _otpTimer.Stop();
        _expiresAtUtc = await _authService.GetVerificationExpiryAsync(_email);
        UpdateCountdown();

        if (_expiresAtUtc > DateTime.UtcNow)
            _otpTimer.Start();
    }

    private void UpdateCountdown()
    {
        if (_expiresAtUtc == null)
        {
            txtCountdown.Text = "Chưa có mã xác minh.";
            return;
        }

        int seconds = Math.Max(0,
            (int)Math.Ceiling((_expiresAtUtc.Value - DateTime.UtcNow).TotalSeconds));

        txtCountdown.Text = seconds == 0
            ? "Mã đã hết hạn. Hãy gửi lại mã."
            : $"Mã còn hiệu lực {seconds / 60:00}:{seconds % 60:00}";

        if (seconds == 0) _otpTimer.Stop();
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        ((MainWindow)Application.Current.MainWindow)
            .Navigate(new LoginView());
    }
}