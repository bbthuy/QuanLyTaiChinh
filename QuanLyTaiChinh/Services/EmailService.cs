using MailKit.Security;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace QuanLyTaiChinh.Services;

public class EmailService
{
    public async Task SendOtpAsync(string toEmail, string code, string purpose)
    {
        string? username = Environment.GetEnvironmentVariable("MAIL_USERNAME");
        string? password = Environment.GetEnvironmentVariable("MAIL_PASSWORD");

        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "Chưa cấu hình tài khoản gửi email.");
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("FinanceWise", username));
        message.To.Add(MailboxAddress.Parse(toEmail));

        message.Subject = purpose == "VERIFY_EMAIL"
            ? "FinanceWise - Xác minh tài khoản"
            : "FinanceWise - Đặt lại mật khẩu";

        message.Body = new TextPart("plain")
        {
            Text = $"Mã xác nhận của bạn: {code}\n" +
                   "Mã có hiệu lực trong 5 phút. Nếu không yêu cầu, hãy bỏ qua email này."
        };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(
            "smtp.gmail.com", 587, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(username, password);
        await smtp.SendAsync(message);
        await smtp.DisconnectAsync(true);
    }
}