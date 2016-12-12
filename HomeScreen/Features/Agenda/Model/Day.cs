using System;
using System.Collections.Generic;

namespace HomeScreen.Features.Agenda.Model
{
    public class Day
    {
        public DateTime Date { get; set; }

        public IList<Event> Events { get; } = new List<Event>();
    }
}
