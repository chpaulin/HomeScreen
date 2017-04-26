using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace HomeScreen.Common.Converters
{
    public class ChargeStateToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value is double)
            {
                var doubleValue = (double)value;

                if (doubleValue <= 0.06)
                    return App.Current.Resources["VeryLowBatteryBrush"] as SolidColorBrush;

                if (doubleValue > 0.06 && doubleValue <= 0.33)
                    return App.Current.Resources["LowBatteryBrush"] as SolidColorBrush;

                if (doubleValue > 0.30)
                    return App.Current.Resources["HighBatteryBrush"] as SolidColorBrush;
            }

            return App.Current.Resources["SystemControlForegroundBaseMediumBrush"] as SolidColorBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
