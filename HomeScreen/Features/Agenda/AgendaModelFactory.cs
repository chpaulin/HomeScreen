using HomeScreen.Features.Agenda.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScreen.Features.Agenda
{
    public static class AgendaModelFactory
    {
        public static Event CreateEvent(EventData @event)
        {
            return new Event
            {
                Start = @event.Start,
                End = @event.End,
                EventType = @event.IsAllDay ? Event.ALL_DAY_EVENT : Event.NORMAL_EVENT,
                Subject = @event.Subject
            };
        }

        public static Event CreateHolidayEvent(string subject)
        {
            return new Event
            {
                EventType = Event.ALL_DAY_EVENT,
                Subject = subject
            };
        }
    }
}
