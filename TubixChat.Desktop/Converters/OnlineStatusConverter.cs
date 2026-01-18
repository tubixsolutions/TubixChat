using System;
using System.Globalization;
using System.Windows.Data;

namespace TubixChat.Desktop.Converters;

public class OnlineStatusConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool isOnline && isOnline ? "online" : "ko'rilgan: yaqinda";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
