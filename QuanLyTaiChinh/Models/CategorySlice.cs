using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace QuanLyTaiChinh.Models
{
    public class CategorySlice
    {
        public string Name { get; set; } = "";
        public double Value { get; set; }
        public string ColorHex { get; set; } = "#3B82F6";

        public Brush ColorBrush => new SolidColorBrush(
            (Color)ColorConverter.ConvertFromString(ColorHex));
    }
}

