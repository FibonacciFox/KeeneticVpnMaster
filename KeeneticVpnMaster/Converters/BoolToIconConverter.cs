using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace KeeneticVpnMaster.Converters
{
    public class BoolToIconConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is true ? "mdi-brightness-4" : "mdi-brightness-6";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return Avalonia.Data.BindingOperations.DoNothing;
        }
    }
}