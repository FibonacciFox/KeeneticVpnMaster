using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace KeeneticVpnMaster.Converters;

public class LastHandshakeToReadableFormatConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        long totalSeconds = value switch
        {
            long longValue => longValue,
            int intValue => intValue,
            _ => -1 // Если null или не число
        };

        if (totalSeconds < 0)
            return "-"; // Значение по умолчанию, если входные данные некорректны

        long hours = totalSeconds / 3600;
        long minutes = (totalSeconds % 3600) / 60;
        long secs = totalSeconds % 60;

        if (hours > 0)
            return $"{hours} ч {minutes} м {secs} с";
        if (minutes > 0)
            return $"{minutes} м {secs} с";

        return $"{secs} с";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException("Обратное преобразование не поддерживается.");
    }
}