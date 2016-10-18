using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace HomeScreen.Common.Converters
{
    public class DateTimeToDateTimeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is DateTime))
                return string.Empty;

            var date = (DateTime)value;

            return $"{date.ToString("ddd", new CultureInfo("sv-SE"))} {date.ToString("HH:mm")}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
