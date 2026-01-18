using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TubixChat.Desktop.Converters
{
    public class MessageBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool isMine && isMine
                ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0088cc"))
                : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#34495E"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
