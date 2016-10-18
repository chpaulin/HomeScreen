using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace HomeScreen.Common.Converters
{
    public class IntegerToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is int))
                return string.Empty;

            var intValue = (int)value;

            return intValue.ToString("D2");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            //DO NOTHING
            return value;
        }
    }
}
