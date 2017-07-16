using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WinCommandPalette.Converter
{
    public class InvertBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (bool.TryParse(value.ToString(), out bool boolValue))
            {
                return boolValue ? Visibility.Collapsed : Visibility.Visible;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
