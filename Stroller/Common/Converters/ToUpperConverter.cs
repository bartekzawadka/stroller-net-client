using System;
using System.Globalization;
using System.Windows.Data;

namespace Stroller.Common.Converters
{
    public class ToUpperConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            if (value is string)
            {
                return value.ToString().ToUpper();
            }
            return String.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            if (value is string)
            {
                return value.ToString().ToUpper();
            }
            return String.Empty;
        }
    }
}
