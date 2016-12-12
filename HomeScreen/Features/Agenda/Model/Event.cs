using System;

namespace HomeScreen.Features.Agenda.Model
{
    public class Event
    {
        public const int NO_EVENTS = 0;
        public const int ALL_DAY_EVENT = 1;
        public const int NORMAL_EVENT = 2;
        public const int PERIODIC_EVENT = 3;

        public DateTime End { get; internal set; }
        public DateTime Start { get; internal set; }
        public string Subject { get; internal set; }
        public int EventType { get; set; }        
    }
}