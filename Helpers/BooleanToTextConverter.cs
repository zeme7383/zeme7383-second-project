using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace MyLibrary.Helpers;

public class BooleanToTextConverter : IValueConverter
{
    // ConverterParameter: "Overdue|Active|Returned"
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var param = (parameter as string)?.Split('|');
        if (param == null || param.Length < 3)
            return value?.ToString() ?? "";
        if (value is bool b)
            return b ? param[0] : param[1];
        if (value == null)
            return param[2];
        return param[1];
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
} 