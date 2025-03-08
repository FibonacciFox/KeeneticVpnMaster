using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace KeeneticVpnMaster.Converters;

public class ConnectedToIconConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string connected)
        {
            return connected.Equals("yes", StringComparison.OrdinalIgnoreCase) ? "mdi-moon-full" : "mdi-moon-new";
        }
        return "mdi-moon-new"; // Значение по умолчанию
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException("Обратное преобразование не поддерживается.");
    }
}