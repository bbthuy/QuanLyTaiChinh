using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace QuanLyTaiChinh.Converters
{
    public class GoalColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string color = value as string ?? "Blue";
            string aspect = parameter as string ?? "Fill";

            var (fill, light, text) = color switch
            {
                "Purple" => ("#7C3AED", "#EDE9FE", "#7C3AED"),
                "Green" => ("#10B981", "#D1FAE5", "#059669"),
                "Orange" => ("#F59E0B", "#FEF3C7", "#D97706"),
                _ => ("#3B82F6", "#DBEAFE", "#2563EB"), // Blue (mac dinh)
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

