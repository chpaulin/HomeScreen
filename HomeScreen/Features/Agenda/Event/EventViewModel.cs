using System;
using GalaSoft.MvvmLight;
using System.Text;
using HomeScreen.Features.Agenda.Model;

namespace HomeScreen.Features.Agenda
{
    public class EventViewModel : ViewModelBase
    {       
        public DateTime End { get; internal set; }
        public DateTime Start { get; internal set; }
        public string Subject { get; internal set; }
        public string Duration
        {
            get
            {
                return GetDuration(Start, End);
            }
        }

        public int EventType { get; set; }

        public static EventViewModel NoEvents { get; } = new EventViewModel { EventType = Event.NO_EVENTS };

        private string GetDuration(DateTime start, DateTime end)
        {
            var duration = end.Subtract(start);

            var builder = new StringBuilder();

            if (duration.Hours == 1)
                builder.Append($"{duration.Hours} timme");
            else if (duration.Hours > 1)
                builder.Append($"{duration.Hours} timmar");

            if (duration.Hours > 0 && duration.Minutes > 0)
                builder.Append($", {duration.Minutes} minuter");
            else if (duration.Minutes > 0)
                builder.Append($"{duration.Minutes} minuter");

            return builder.ToString();
        }        
    }
}