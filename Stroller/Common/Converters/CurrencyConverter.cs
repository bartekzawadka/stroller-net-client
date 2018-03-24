using System;
using System.Globalization;
using System.Windows.Data;

namespace Stroller.Common.Converters
{
    public class CurrencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal @decimal)
            {
                return @decimal.ToString("C");
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            IFormatProvider provider = new CultureInfo("pl-PL");

            if (decimal.TryParse(value.ToString(), NumberStyles.Currency, provider, out var val))
            {
                return val;
            }

            return value;
        }
    }
}
