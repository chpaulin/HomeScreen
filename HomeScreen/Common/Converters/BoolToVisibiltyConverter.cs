using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace HomeScreen.Common.Converters
{
    public class BoolToVisibiltyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is bool))
                return Visibility.Visible;

            bool reverse = false;

            if (bool.TryParse(parameter?.ToString(), out reverse))
                return reverse ^ (bool)value ? Visibility.Visible : Visibility.Collapsed;

            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return true;
        }
    }
}
