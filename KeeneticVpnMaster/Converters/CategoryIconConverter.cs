using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;

namespace KeeneticVpnMaster.Converters;

public class ChildrenToIconConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // Ожидаем, что value — коллекция дочерних элементов
        if (value is IEnumerable<object> children && children.Any())
            return "mdi-folder"; // категория
        return "mdi-file-arrow-left-right-outline"; // сайт
    }
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}