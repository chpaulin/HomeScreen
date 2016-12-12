using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeScreen.Features.Agenda.Model;

namespace HomeScreen.Messages
{
    public class AgendaChangedEvent
    {
        public IReadOnlyList<Day> NewAgenda { get; internal set; }
    }
}
