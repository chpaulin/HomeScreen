using System;

namespace HomeScreen.Features.Agenda
{
    public class EventData
    {
        public DateTime End { get; internal set; }
        public bool IsAllDay { get; internal set; }
        public DateTime Start { get; internal set; }
        public string Subject { get; internal set; }
    }
}