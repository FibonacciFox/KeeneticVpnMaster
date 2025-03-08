using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace KeeneticVpnMaster.Converters;

public class ConnectedToBrushConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string connected)
        {
            return connected.Equals("yes", StringComparison.OrdinalIgnoreCase) ? Brushes.LimeGreen : Brushes.Red;
        }
        return Brushes.Red; // Значение по умолчанию
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException("Обратное преобразование не поддерживается.");
    }
}