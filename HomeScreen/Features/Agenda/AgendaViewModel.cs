using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using HomeScreen.Common;
using HomeScreen.Messages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace HomeScreen.Features.Agenda
{
    public class AgendaViewModel : ViewModelBase
    {
        private readonly AgendaService _agendaService;

        public AgendaViewModel(Configuration configuration)
        {
            _agendaService = new AgendaService(configuration);

            if (!configuration.Loaded)
                Messenger.Default.Register<ConfigurationLoadedEvent>(this, async (_) => await SetUpAgendaChecker());
            else
                Task.Run(SetUpAgendaChecker);
        }

        public List<DayViewModel> Agenda { get; private set; } = new List<DayViewModel>();

        private async Task SetUpAgendaChecker()
        {
            await UpdateAgendaData();

            Observable
                .Interval(TimeSpan.FromMinutes(5))
                .ObserveOnDispatcher()
                .Subscribe(async (_) => await UpdateAgendaData());            
        }

        private async Task UpdateAgendaData()
        {
            await UpdateEvents();
        }

        private async Task UpdateHolidayInfo(DayViewModel day)
        {
            var info = await _agendaService.RetrieveDateSigificanceData(DateTime.Today);

            var dayInfo = info.dagar.FirstOrDefault();

            if (dayInfo == null)
                return;

            if (!string.IsNullOrEmpty(dayInfo.helgdag))
                day.Events.Insert(0, EventViewModel.CreateHoliday(dayInfo.helgdag));
            else if (!string.IsNullOrEmpty(dayInfo.helgdagsafton))
                day.Events.Insert(0, EventViewModel.CreateHoliday(dayInfo.helgdagsafton));
            else if (!string.IsNullOrEmpty(dayInfo.flaggdag))
                day.Events.Insert(0, EventViewModel.CreateHoliday(dayInfo.flaggdag));
        }

        private async Task UpdateEvents()
        {
            var events = await _agendaService.RetrieveCalendarData();

            var agenda = new List<DayViewModel>();

            var entryCount = 0;
            var date = DateTime.Today;

            while (entryCount < 7)
            {
                var day = new DayViewModel { Day = DateToStringUtility.GetDayString(date) };

                foreach (var @event in events.Where(e => e.Start.Date == date && e.Start.Add(e.End - e.Start) >= DateTime.Now || (e.Start.Date < date && date < e.End.Date)).OrderBy(e => e.Start))
                {
                    var eventVM = CreateEventViewModel(@event);

                    day.Events.Add(eventVM);
                }

                await UpdateHolidayInfo(day);

                if (day.Events.Count > 0)
                {
                    agenda.Add(day);
                    entryCount += day.Events.Count + 1;
                }

                date += TimeSpan.FromDays(1);
            }

            if (entryCount > 8)
                agenda.RemoveAt(agenda.Count - 1);

            Agenda = agenda;
            RaisePropertyChanged(nameof(Agenda));
        }

        private EventViewModel CreateEventViewModel(EventData @event)
        {
            return new EventViewModel
            {
                Start = @event.Start,
                End = @event.End,
                EventType = @event.IsAllDay ? EventViewModel.ALL_DAY_EVENT : EventViewModel.NORMAL_EVENT,
                Subject = @event.Subject
            };
        }
    }
}
