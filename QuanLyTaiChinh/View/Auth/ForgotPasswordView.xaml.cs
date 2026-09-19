using System.Windows;
using System.Windows.Controls;

namespace QuanLyTaiChinh.View.Auth
{
    public partial class ForgotPasswordView : UserControl
    {
        public ForgotPasswordView()
        {
            InitializeComponent();
        }

        private void SendOtp_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Vui lòng nhập email.");
                return;
            }

            MainWindow mainWindow =
                Application.Current.MainWindow as MainWindow;

            if (mainWindow != null)
            {
                mainWindow.Navigate(new VerifyResetCodeView());
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow =
                Application.Current.MainWindow as MainWindow;

            if (mainWindow != null)
            {
                mainWindow.Navigate(new LoginView());
            }
        }
    }
}