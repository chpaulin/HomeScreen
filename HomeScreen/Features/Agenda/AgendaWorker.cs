using GalaSoft.MvvmLight.Messaging;
using HomeScreen.Common;
using HomeScreen.Features.Agenda.Model;
using HomeScreen.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScreen.Features.Agenda
{
    public class AgendaWorker : IAsyncWorker
    {
        private readonly Configuration _configuration;
        private readonly AgendaService _agendaService;

        public AgendaWorker(Configuration configuration)
        {
            _configuration = configuration;
            _agendaService = new AgendaService(_configuration);
        }

        private async Task SetUpAgendaChecker()
        {
            await UpdateAgendaData();

            Observable
                .Interval(TimeSpan.FromMinutes(5))
                .Subscribe(async (_) => await UpdateAgendaData());
        }

        private async Task UpdateAgendaData()
        {
            var currentEvents = await GetCurrentEvents();   

            Messenger.Default.Send(new AgendaChangedEvent
            {
                NewAgenda = currentEvents
            });
        }

        private async Task<Day> UpdateHolidayInfo(Day day)
        {
            var info = await _agendaService.RetrieveDateSigificanceData(day.Date);

            var dayInfo = info.dagar.FirstOrDefault();

            if (dayInfo != null)
            {
                if (!string.IsNullOrEmpty(dayInfo.helgdag))
                    day.Events.Insert(0, AgendaModelFactory.CreateHolidayEvent(dayInfo.helgdag));
                else if (!string.IsNullOrEmpty(dayInfo.helgdagsafton))
                    day.Events.Insert(0, AgendaModelFactory.CreateHolidayEvent(dayInfo.helgdagsafton));
                else if (!string.IsNullOrEmpty(dayInfo.flaggdag))
                    day.Events.Insert(0, AgendaModelFactory.CreateHolidayEvent(dayInfo.flaggdag));
            }

            return day;
        }

        private async Task<IReadOnlyList<Day>> GetCurrentEvents()
        {
            var events = await _agendaService.RetrieveCalendarData();

            var entryCount = 0;
            var date = DateTime.Today;

            var currentEvents = new List<Day>();

            while (entryCount < 7)
            {
                var day = new Day
                {
                    Date = date
                };

                foreach (var eventData in events.Where(e => e.Start.Date == date && e.Start.Add(e.End - e.Start) >= DateTime.Now || (e.Start.Date < date && date < e.End.Date)).OrderBy(e => e.Start))
                {

                    var @event = AgendaModelFactory.CreateEvent(eventData);

                    day.Events.Add(@event);
                }

                await UpdateHolidayInfo(day);

                if (day.Events.Count > 0)
                {
                    currentEvents.Add(day);
                    entryCount += day.Events.Count + 1;
                }

                date += TimeSpan.FromDays(1);
            }

            if (entryCount > 8)
                currentEvents.RemoveAt(currentEvents.Count - 1);

            return currentEvents;
        }

        public async Task RunAsync()
        {
            await SetUpAgendaChecker();
        }
    }
}
