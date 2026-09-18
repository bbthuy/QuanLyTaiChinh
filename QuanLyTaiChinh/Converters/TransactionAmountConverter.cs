using QuanLyTaiChinh.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace QuanLyTaiChinh.Converters
{
    class TransactionAmountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not TransactionItem item) return string.Empty;
            var sign = item.Type == TransactionType.Income ? "+" : "-";
            return $"{sign}{item.Amount:N0}đ";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}

