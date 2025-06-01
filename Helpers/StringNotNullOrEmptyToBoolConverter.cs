using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace MyLibrary.Helpers;

public class StringNotNullOrEmptyToBoolConverter : IValueConverter
{
    public static readonly StringNotNullOrEmptyToBoolConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => !string.IsNullOrEmpty(value as string);

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class BooleanToPasswordCharConverter : IValueConverter
{
    public static readonly BooleanToPasswordCharConverter Instance = new();
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b && b)
            return null; // Show password
        return '●'; // Hide password
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}

public class BooleanToEyeIconConverter : IValueConverter
{
    public static readonly BooleanToEyeIconConverter Instance = new();
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b && b)
            return "🙈"; // Hide icon
        return "👁"; // Show icon
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}

public class ShowPasswordToIconGeometryConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool show = value is bool b && b;
        var key = show ? "eye_hide_regular" : "eye_show_regular";
        if (Avalonia.Application.Current?.Resources.TryGetResource(key, null, out var geometry) == true)
            return geometry;
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}
