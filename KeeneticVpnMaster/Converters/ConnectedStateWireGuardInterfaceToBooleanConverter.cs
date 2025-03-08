using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace KeeneticVpnMaster.Converters;

public class ConnectedStateWireGuardInterfaceToBooleanConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string state)
        {
            return state.Equals("up", StringComparison.OrdinalIgnoreCase);
        }
        return false;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isChecked)
        {
            return isChecked ? "up" : "down";
        }
        return "down";
    }
}