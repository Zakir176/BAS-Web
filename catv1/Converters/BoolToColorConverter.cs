using System.Globalization;
using Microsoft.Maui.Graphics;

namespace catv1.Converters;

public class BoolToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b && b)
        {
            return Application.Current?.Resources.TryGetValue("Primary", out var color) == true ? (Color)color : Colors.Blue;
        }
        return Application.Current?.Resources.TryGetValue("Surface", out var surface) == true ? (Color)surface : Colors.Transparent;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
