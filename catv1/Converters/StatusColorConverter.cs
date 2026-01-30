using System.Globalization;
using Microsoft.Maui.Controls;

namespace catv1.Converters;

public class StatusColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string status)
        {
            return status switch
            {
                "Present" => Colors.Green, // or #22c55e
                "Absent" => Colors.Red,    // or #ef4444
                "Excused" => Colors.Orange,// or #f59e0b
                _ => Colors.Gray
            };
        }
        return Colors.Gray;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
