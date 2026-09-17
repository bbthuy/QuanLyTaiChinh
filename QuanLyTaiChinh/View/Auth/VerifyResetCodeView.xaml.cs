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
    /// Interaction logic for VerifyResetCodeView.xaml
    /// </summary>
    public partial class VerifyResetCodeView : UserControl
    {
        public VerifyResetCodeView()
        {
            InitializeComponent();
        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow)
                .Navigate(new ResetPasswordView());
        }
    }
}
