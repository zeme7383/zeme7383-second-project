using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace MyLibrary.Helpers;

public class BooleanToColorConverter : IValueConverter
{
    // ConverterParameter: "#D32F2F|#1976D2|#388E3C"
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var param = (parameter as string)?.Split('|');
        if (param == null || param.Length < 3)
            return Brushes.Black;
        if (value is bool b)
            return b ? Brush.Parse(param[0]) : Brush.Parse(param[1]);
        if (value == null)
            return Brush.Parse(param[2]);
        return Brush.Parse(param[1]);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
} 