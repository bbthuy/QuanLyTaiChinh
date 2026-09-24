using Microsoft.Win32;
using QuanLyTaiChinh.Services;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace QuanLyTaiChinh.View.Auth;

public partial class EditProfileView : UserControl
{
    private readonly AuthService _authService = new();
    private byte[]? _newAvatarData;

    public EditProfileView()
    {
        InitializeComponent();

        var user = ((MainWindow)Application.Current.MainWindow).CurrentUser;
        if (user == null)
            return;

        txtFullName.Text = user.FullName;
        txtEmail.Text = user.Email;

        PreviewInitial.Text = string.IsNullOrWhiteSpace(user.FullName)
            ? "?"
            : user.FullName.Trim()[..1].ToUpperInvariant();

        ShowAvatar(user.AvatarData);
    }

    private void ChooseAvatar_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "Chọn ảnh đại diện",
            Filter = "Ảnh PNG, JPG|*.png;*.jpg;*.jpeg"
        };

        if (dialog.ShowDialog() != true)
            return;

        try
        {
            var file = new FileInfo(dialog.FileName);

            if (file.Length == 0 || file.Length > 2 * 1024 * 1024)
            {
                AuthMessageWindow.ShowMessage("Vui lòng chọn ảnh không quá 2 MB.");
                return;
            }

            byte[] bytes = File.ReadAllBytes(dialog.FileName);

            if (AvatarImageHelper.FromBytes(bytes) == null)
            {
                AuthMessageWindow.ShowMessage("Không đọc được ảnh này.");
                return;
            }

            _newAvatarData = bytes;
            ShowAvatar(bytes);
        }
        catch (Exception ex)
        {
            AuthMessageWindow.ShowMessage($"Không mở được ảnh: {ex.Message}");
        }
    }

    private void ShowAvatar(byte[]? data)
    {
        var image = AvatarImageHelper.FromBytes(data);
        if (image == null)
            return;

        AvatarPreview.Fill = new ImageBrush(image)
        {
            Stretch = Stretch.UniformToFill
        };
        PreviewInitial.Visibility = Visibility.Collapsed;
    }

    private async void Save_Click(object sender, RoutedEventArgs e)
    {
        var main = (MainWindow)Application.Current.MainWindow;
        if (main.CurrentUser == null)
        {
            main.ShowLogin();
            return;
        }

        btnSave.IsEnabled = false;

        try
        {
            var updated = await _authService.UpdateProfileAsync(
                main.CurrentUser.UserId,
                txtFullName.Text,
                _newAvatarData);

            if (updated == null)
            {
                AuthMessageWindow.ShowMessage("Họ tên phải có từ 1 đến 100 ký tự; ảnh tối đa 2 MB.");
                return;
            }

            main.UpdateCurrentUser(updated);
            AuthMessageWindow.ShowMessage("Đã cập nhật thông tin tài khoản.");
            main.ShowMainApp();
        }
        catch (Exception ex)
        {
            AuthMessageWindow.ShowMessage($"Không lưu được thông tin: {ex.Message}");
        }
        finally
        {
            btnSave.IsEnabled = true;
        }
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        ((MainWindow)Application.Current.MainWindow).ShowMainApp();
    }
}