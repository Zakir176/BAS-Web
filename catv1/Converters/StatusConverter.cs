using System.Globalization;
using Microsoft.Maui.Controls;

namespace catv1.Converters;

public class StatusConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isPresent && parameter is string param)
        {
            if (param == "Text")
            {
                return isPresent ? "PRESENT" : "MARK";
            }
            else if (param == "Color")
            {
                // Green if present, Dark Gray if not
                return isPresent ? Color.FromArgb("#10b981") : Color.FromArgb("#334155");
            }
        }
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
