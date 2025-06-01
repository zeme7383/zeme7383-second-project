using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace MyLibrary.Helpers;

public class ReturnDateToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime dt && dt != DateTime.MinValue)
            return dt.ToString("yyyy-MM-dd");
        return "Not returned yet";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
} 