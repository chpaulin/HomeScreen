using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace HomeScreen.Common.Converters
{
    public class TemperatureConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is double))
                return "13,4°";

            int decimalPlaces = 1;

            if (parameter != null)
                int.TryParse(parameter?.ToString(), out decimalPlaces);

            var roundedNumber = Math.Round((double)value, decimalPlaces, MidpointRounding.AwayFromZero);

            return string.Format(new CultureInfo("sv-SE"), "{0}°", roundedNumber);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
