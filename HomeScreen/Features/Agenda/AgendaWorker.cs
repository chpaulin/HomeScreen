using GalaSoft.MvvmLight.Messaging;
using HomeScreen.Common;
using HomeScreen.Common.Configuration;
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
        private const double MAXIMUM_CONTENT = 80;

        private readonly FeatureConfig _configuration;
        private readonly AgendaService _agendaService;

        public AgendaWorker(FeatureConfig configuration)
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
            }

            return day;
        }

        private async Task<IReadOnlyList<Day>> GetCurrentEvents()
        {
            var events = await _agendaService.RetrieveCalendarData();

            var entryCount = 0d;
            var date = DateTime.Today;

            var currentEvents = new List<Day>();

            while (entryCount < MAXIMUM_CONTENT)
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
                    entryCount += GetDayEntryCount(day);
                }

                date += TimeSpan.FromDays(1);                
            }

            if (entryCount > MAXIMUM_CONTENT)
                currentEvents.RemoveAt(currentEvents.Count - 1);

            return currentEvents;
        }

        private double GetDayEntryCount(Day day)
        {
            double entryCount = 6;

            foreach(var @event in day.Events)
            {
                switch (@event.EventType)
                {
                    case Event.ALL_DAY_EVENT:
                    case Event.NO_EVENTS:
                    case Event.PERIODIC_EVENT:
                        entryCount += 8;
                        break;
                    case Event.NORMAL_EVENT:
                        entryCount += 12;
                        break;
                }
            }

            return entryCount;
        }

        public async Task RunAsync()
        {
            await SetUpAgendaChecker();
        }
    }
}
