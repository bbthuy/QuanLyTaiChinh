using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuanLyTaiChinh.View.Auth
{
    /// <summary>
    /// Interaction logic for VerifyEmailView.xaml
    /// </summary>
    public partial class VerifyEmailView : UserControl
    {
        public VerifyEmailView()
        {
            InitializeComponent();
        }

        private void Verify_Click(object sender, RoutedEventArgs e)
        {
            // Sau này verify OTP từ DB.
            MessageBox.Show(
                "Xác minh email thành công!",
                "FinanceWise",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            ((MainWindow)Application.Current.MainWindow)
                .Navigate(new LoginView());
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow)
                .Navigate(new LoginView());
        }
    }
}
