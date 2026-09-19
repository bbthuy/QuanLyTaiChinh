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
    /// Interaction logic for ResetPasswordView.xaml
    /// </summary>
    public partial class ResetPasswordView : UserControl
    {
        public ResetPasswordView()
        {
            InitializeComponent();
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Đổi mật khẩu thành công!",
                "FinanceWise",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            ((MainWindow)Application.Current.MainWindow)
                .Navigate(new LoginView());
        }
    }
}
