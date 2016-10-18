using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScreen.Common
{
    public static class DateToStringUtility
    {
        public static string GetDayString(DateTime date)
        {
            if (date.Subtract(DateTime.Today) < TimeSpan.FromDays(1))
                return "I dag";

            if (date.Subtract(DateTime.Today) < TimeSpan.FromDays(2))
                return "I morgon";

            if (date.Subtract(DateTime.Today) < TimeSpan.FromDays(7))
                return date.ToString("dddd", new CultureInfo("sv-SE"));

            return date.ToString("'den' d MMMM yyyy", new CultureInfo("sv-SE"));
        }
    }
}
