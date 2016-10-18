using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace HomeScreen.Common.Converters
{
    public class DateTimeToDepartureConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is DateTime))
                return string.Empty;

            var dateValue = (DateTime)value;

            var timeToDeparture = dateValue.Subtract(DateTime.Now);

            if (timeToDeparture.TotalMinutes < 1) //Time passed
                return "Nu";

            if (timeToDeparture.TotalMinutes >= 60)
                return dateValue.ToString("HH:mm");

            return $"{timeToDeparture.TotalMinutes:0} min";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
