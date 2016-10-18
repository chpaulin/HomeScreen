using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace HomeScreen.Common.Converters
{
    public class IntegerToVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is int) || parameter == null)
                return Visibility.Collapsed;

            int paramValue = 0;

            if (!int.TryParse(parameter?.ToString(), out paramValue))
                return Visibility.Collapsed;

            return paramValue == (int)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return 0;
        }
    }
}
