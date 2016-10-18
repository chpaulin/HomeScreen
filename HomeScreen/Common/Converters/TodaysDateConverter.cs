using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace HomeScreen.Common.Converters
{
    public class TodaysDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is DateTime))
                return string.Empty;

            var date = (DateTime)value;

            var culture = new CultureInfo("sv-SE");

            return $"DEN {date.ToString("dd")} {date.ToString("MMMM", culture).ToUpperInvariant()} { date.ToString("yyyy")}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
