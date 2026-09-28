using System.Windows;

namespace QuanLyTaiChinh.View.Auth;

public partial class AuthMessageWindow : Window
{
    public AuthMessageWindow(string message)
    {
        InitializeComponent();
        txtMessage.Text = message;
    }

    public static void ShowMessage(string message)
    {
        var dialog = new AuthMessageWindow(message);
        var mainWindow = Application.Current.MainWindow;

        if (mainWindow?.IsVisible == true)
            dialog.Owner = mainWindow;
        else
            dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;

        dialog.ShowDialog();
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}