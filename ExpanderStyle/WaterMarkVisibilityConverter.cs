using System;
using System.Windows;
using System.Windows.Data;

namespace ExpanderStyle
{
    public class WaterMarkVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values[0] is bool b && values[1] is bool)
            {
                var hasText = !b;
                var hasFocus = (bool)values[1];
                if (hasFocus || hasText)
                    return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
