using QuanLyTaiChinh.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace QuanLyTaiChinh.Converters
{
    class TransactionTypeColorConverter : IValueConverter
    {
        private static readonly SolidColorBrush Green = new(Color.FromRgb(0x05, 0x96, 0x69));
        private static readonly SolidColorBrush Purple = new(Color.FromRgb(0x7C, 0x3A, 0xED));
        private static readonly SolidColorBrush Red = new(Color.FromRgb(0xDC, 0x26, 0x26));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                TransactionType.Income => Green,
                TransactionType.Saving => Purple,
                _ => Red
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
