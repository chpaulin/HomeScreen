using System;

namespace HomeScreen.Features.Departures
{
    public static class DepartureExtensions
    {
        public static DateTime GetDepartureTime(this Departure departure)
        {
            return DateTime.Parse(departure.date).Add(DateTime.Parse(departure.rtTime ?? departure.time).TimeOfDay);
        }

        public static DateTime GetOriginalDepartureTime(this Departure departure)
        {
            return DateTime.Parse(departure.date).Add(DateTime.Parse(departure.time).TimeOfDay);
        }
    }
}
