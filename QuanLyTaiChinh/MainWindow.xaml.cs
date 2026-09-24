using QuanLyTaiChinh.View.Auth;
using QuanLyTaiChinh.ViewModels;
using System.Windows;
using System.Windows.Controls;
using QuanLyTaiChinh.Models;
using QuanLyTaiChinh.Services;
using System.Windows.Media;

namespace QuanLyTaiChinh
{
    public partial class MainWindow : Window
    {
        public User? CurrentUser { get; private set; }
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
            CurrentUser = null;
            DataContext = null;
            Navigate(new LoginView());
        }

        public void ShowMainApp()
        {
            if (CurrentUser == null)
            {
                ShowLogin();
                return;
            }
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

        public void SignIn(User user)
        {
            UpdateCurrentUser(user);
            ShowMainApp();
        }

        public void UpdateCurrentUser(User user)
        {
            CurrentUser = user;
            txtProfileName.Text = user.FullName;
            txtProfileEmail.Text = user.Email;

            string name = user.FullName.Trim();
            txtAvatarInitial.Text = name.Length > 0
                ? name[..1].ToUpperInvariant()
                : "?";

            var image = AvatarImageHelper.FromBytes(user.AvatarData);

            if (image != null)
            {
                AvatarPhoto.Fill = new ImageBrush(image)
                {
                    Stretch = Stretch.UniformToFill
                };
                AvatarPhoto.Visibility = Visibility.Visible;
                txtAvatarInitial.Visibility = Visibility.Collapsed;
            }
            else
            {
                AvatarPhoto.Visibility = Visibility.Collapsed;
                txtAvatarInitial.Visibility = Visibility.Visible;
            }
        }

        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            BtnProfile.IsChecked = false;
            Navigate(new EditProfileView());
        }
    }
}