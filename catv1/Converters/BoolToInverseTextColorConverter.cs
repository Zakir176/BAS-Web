using System.Globalization;
using Microsoft.Maui.Graphics;

namespace catv1.Converters;

public class BoolToInverseTextColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b && b)
        {
            return Colors.White;
        }
        return Application.Current?.Resources.TryGetValue("Secondary", out var color) == true ? (Color)color : Colors.Gray;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
