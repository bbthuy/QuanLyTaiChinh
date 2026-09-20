using QuanLyTaiChinh.View.Auth;
using QuanLyTaiChinh.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace QuanLyTaiChinh
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ShowLogin();
        }

        public void Navigate(UserControl view)
        {
            AppShell.Visibility = Visibility.Collapsed;

            AuthHost.Visibility = Visibility.Visible;
            AuthHost.Content = view;
        }

        public void ShowLogin()
        {
            DataContext = null;
            Navigate(new LoginView());
        }

        public void ShowMainApp()
        {
            AuthHost.Content = null;
            AuthHost.Visibility = Visibility.Collapsed;

            DataContext = new MainViewModel();

            AppShell.Visibility = Visibility.Visible;
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            Navigate(new ChangePasswordView());
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            ShowLogin();
        }

        private void BtnNotification_Checked(object sender, RoutedEventArgs e)
        {
            (DataContext as MainViewModel)?.RefreshNotifications();
        }
    }
}