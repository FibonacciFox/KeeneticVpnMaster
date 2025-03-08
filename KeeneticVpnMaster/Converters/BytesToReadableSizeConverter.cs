using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace KeeneticVpnMaster.Converters;

public class BytesToReadableSizeConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null || !(value is long bytes))
            return "-";

        const long Kb = 1024;
        const long Mb = Kb * 1024;
        const long Gb = Mb * 1024;

        if (bytes >= Gb)
            return $"{bytes / (double)Gb:F2} ГБ";
        if (bytes >= Mb)
            return $"{bytes / (double)Mb:F2} МБ";
        if (bytes >= Kb)
            return $"{bytes / (double)Kb:F2} КБ";

        return $"{bytes} Б";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException("Обратное преобразование не поддерживается.");
    }
}