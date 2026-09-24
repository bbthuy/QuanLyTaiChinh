using Microsoft.EntityFrameworkCore;
using QuanLyTaiChinh.Data;
using QuanLyTaiChinh.Models;

namespace QuanLyTaiChinh.Services;

public class AuthService
{
    public async Task<(bool Success, string Message)> RegisterAsync(
        string fullName, string email, string password)
    {
        fullName = fullName.Trim();
        email = email.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(fullName))
            return (false, "Vui lòng nhập họ tên.");

        if (!System.Net.Mail.MailAddress.TryCreate(email, out _))
            return (false, "Email không hợp lệ.");

        if (password.Length < 8)
            return (false, "Mật khẩu phải có ít nhất 8 ký tự.");

        await using var db = FinanceWiseContextFactory.Create();

        if (await db.Users.AnyAsync(u => u.Email == email))
            return (false, "Email này đã được đăng ký.");

        var user = new User
        {
            FullName = fullName,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            IsEmailVerified = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return (true, "Đã tạo tài khoản. Cần xác minh email trước khi đăng nhập.");
    }

    public async Task<User?> LoginAsync(string email, string password)
    {
        email = email.Trim().ToLowerInvariant();

        await using var db = FinanceWiseContextFactory.Create();

        var user = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);

        if (user == null || !user.IsEmailVerified)
            return null;

        return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)
            ? user
            : null;
    }

    public async Task<string?> CreateEmailVerificationCodeAsync(string email)
    {
        email = email.Trim().ToLowerInvariant();

        await using var db = FinanceWiseContextFactory.Create();

        var user = await db.Users
            .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);

        if (user == null || user.IsEmailVerified)
            return null;

        string code = System.Security.Cryptography.RandomNumberGenerator
            .GetInt32(0, 1_000_000)
            .ToString("D6");

        db.OtpCodes.Add(new OtpCode
        {
            UserId = user.UserId,
            CodeHash = BCrypt.Net.BCrypt.HashPassword(code),
            Type = "VERIFY_EMAIL",
            ExpiresAt = DateTime.UtcNow.AddMinutes(5),
            IsUsed = false,
            CreatedAt = DateTime.UtcNow
        });

        await db.SaveChangesAsync();
        return code;
    }

    public async Task<bool> VerifyEmailAsync(string email, string code)
    {
        email = email.Trim().ToLowerInvariant();
        code = code.Trim();

        await using var db = FinanceWiseContextFactory.Create();

        var user = await db.Users
            .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);

        if (user == null || user.IsEmailVerified)
            return false;

        var otp = await db.OtpCodes
            .Where(o => o.UserId == user.UserId
                     && o.Type == "VERIFY_EMAIL"
                     && !o.IsUsed)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync();

        if (otp == null
            || otp.ExpiresAt <= DateTime.UtcNow
            || !BCrypt.Net.BCrypt.Verify(code, otp.CodeHash))
            return false;

        otp.IsUsed = true;
        user.IsEmailVerified = true;
        user.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return true;
    }

    public async Task<User?> UpdateProfileAsync(
     int userId, string fullName, byte[]? newAvatarData)
    {
        fullName = fullName.Trim();

        if (string.IsNullOrWhiteSpace(fullName) || fullName.Length > 100)
            return null;

        if (newAvatarData is { Length: > 2 * 1024 * 1024 })
            return null;

        await using var db = FinanceWiseContextFactory.Create();

        var user = await db.Users
            .FirstOrDefaultAsync(u => u.UserId == userId && u.IsActive);

        if (user == null)
            return null;

        user.FullName = fullName;

        // Không chọn ảnh mới thì giữ nguyên ảnh cũ.
        if (newAvatarData != null)
            user.AvatarData = newAvatarData;

        user.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        return user;
    }

    public async Task<bool> ChangePasswordAsync(
    int userId, string currentPassword, string newPassword)
    {
        if (newPassword.Length < 8 || currentPassword == newPassword)
            return false;

        await using var db = FinanceWiseContextFactory.Create();

        var user = await db.Users
            .FirstOrDefaultAsync(u => u.UserId == userId && u.IsActive);

        if (user == null ||
            !BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
            return false;

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        user.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return true;
    }

    public async Task<string?> RequestPasswordResetAsync(string email)
    {
        email = email.Trim().ToLowerInvariant();

        await using var db = FinanceWiseContextFactory.Create();

        var user = await db.Users
            .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);

        if (user == null)
            return null;

        // Mã mới làm các mã reset cũ hết hiệu lực.
        var oldCodes = await db.OtpCodes
            .Where(o => o.UserId == user.UserId
                     && o.Type == "RESET_PASSWORD"
                     && !o.IsUsed)
            .ToListAsync();

        foreach (var oldCode in oldCodes)
            oldCode.IsUsed = true;

        string code = System.Security.Cryptography.RandomNumberGenerator
            .GetInt32(0, 1_000_000)
            .ToString("D6");

        db.OtpCodes.Add(new OtpCode
        {
            UserId = user.UserId,
            CodeHash = BCrypt.Net.BCrypt.HashPassword(code),
            Type = "RESET_PASSWORD",
            ExpiresAt = DateTime.UtcNow.AddMinutes(5),
            IsUsed = false,
            CreatedAt = DateTime.UtcNow
        });

        await db.SaveChangesAsync();
        return code;
    }

    public async Task<bool> VerifyResetCodeAsync(string email, string code)
    {
        email = email.Trim().ToLowerInvariant();
        code = code.Trim();

        if (code.Length != 6 || !code.All(char.IsDigit))
            return false;

        await using var db = FinanceWiseContextFactory.Create();

        var user = await db.Users
            .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);

        if (user == null)
            return false;

        var otp = await db.OtpCodes
            .Where(o => o.UserId == user.UserId
                     && o.Type == "RESET_PASSWORD")
            .OrderByDescending(o => o.OtpId)
            .FirstOrDefaultAsync();

        return otp != null
            && !otp.IsUsed
            && otp.ExpiresAt > DateTime.UtcNow
            && BCrypt.Net.BCrypt.Verify(code, otp.CodeHash);
    }

    public async Task<bool> ResetPasswordAsync(
        string email, string code, string newPassword)
    {
        if (newPassword.Length < 8)
            return false;

        email = email.Trim().ToLowerInvariant();

        await using var db = FinanceWiseContextFactory.Create();

        var user = await db.Users
            .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);

        if (user == null)
            return false;

        var otp = await db.OtpCodes
            .Where(o => o.UserId == user.UserId
                     && o.Type == "RESET_PASSWORD")
            .OrderByDescending(o => o.OtpId)
            .FirstOrDefaultAsync();

        // Kiểm tra lại mã ở bước lưu mật khẩu: không tin riêng màn hình trước.
        if (otp == null
            || otp.IsUsed
            || otp.ExpiresAt <= DateTime.UtcNow
            || !BCrypt.Net.BCrypt.Verify(code.Trim(), otp.CodeHash))
            return false;

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        user.UpdatedAt = DateTime.UtcNow;
        otp.IsUsed = true;

        await db.SaveChangesAsync();
        return true;
    }

    public async Task<DateTime?> GetVerificationExpiryAsync(string email)
    {
        email = email.Trim().ToLowerInvariant();

        await using var db = FinanceWiseContextFactory.Create();

        var user = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);

        if (user == null) return null;

        return await db.OtpCodes
            .Where(o => o.UserId == user.UserId
                     && o.Type == "VERIFY_EMAIL"
                     && !o.IsUsed)
            .OrderByDescending(o => o.OtpId)
            .Select(o => (DateTime?)o.ExpiresAt)
            .FirstOrDefaultAsync();
    }

    public async Task<DateTime?> GetResetExpiryAsync(string email)
    {
        email = email.Trim().ToLowerInvariant();
        await using var db = FinanceWiseContextFactory.Create();

        var userId = await db.Users
            .Where(u => u.Email == email && u.IsActive)
            .Select(u => (int?)u.UserId)
            .FirstOrDefaultAsync();

        if (userId == null) return null;

        return await db.OtpCodes
            .Where(o => o.UserId == userId.Value
                     && o.Type == "RESET_PASSWORD"
                     && !o.IsUsed)
            .OrderByDescending(o => o.OtpId)
            .Select(o => (DateTime?)o.ExpiresAt)
            .FirstOrDefaultAsync();
    }

    public async Task<bool?> GetEmailVerifiedStatusAsync(string email)
    {
        email = email.Trim().ToLowerInvariant();
        await using var db = FinanceWiseContextFactory.Create();

        return await db.Users
            .Where(u => u.Email == email && u.IsActive)
            .Select(u => (bool?)u.IsEmailVerified)
            .FirstOrDefaultAsync();
    }
}