using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace QuanLyTaiChinh.Converters
{
    // value: "Red" | "Orange" | "Green" | "Blue"
    // parameter: "Fill" (mau dam, thanh tien do) | "Light" (mau nen nhat) | "Text" (mau chu tren nen Light)
    public class BudgetStatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value as string ?? "Blue";
            string aspect = parameter as string ?? "Fill";

            var (fill, light, text) = status switch
            {
                "Red" => ("#EF4444", "#FEE2E2", "#DC2626"),
                "Orange" => ("#F59E0B", "#FEF3C7", "#D97706"),
                "Green" => ("#10B981", "#D1FAE5", "#059669"),
                _ => ("#3B82F6", "#DBEAFE", "#2563EB"), // Blue
            };

            var hex = aspect switch
            {
                "Light" => light,
                "Text" => text,
                _ => fill
            };

            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}