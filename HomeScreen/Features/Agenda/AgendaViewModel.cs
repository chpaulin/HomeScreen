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
using HomeScreen.Features.Agenda.Model;
using GalaSoft.MvvmLight.Threading;

namespace HomeScreen.Features.Agenda
{
    public class AgendaViewModel : ViewModelBase
    {
        public AgendaViewModel()
        {
            Messenger.Default.Register<AgendaChangedEvent>(this, OnAgendaChanged);
        }

        private void OnAgendaChanged(AgendaChangedEvent agendaChangedEventArgs)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                Agenda.Clear();

                foreach (var day in agendaChangedEventArgs.NewAgenda)
                {
                    var dayVM = new DayViewModel
                    {
                        Date = day.Date,
                        Day = DateToStringUtility.GetDayString(day.Date),
                        Events = CreateEventViewModels(day.Events)
                    };

                    Agenda.Add(dayVM);
                }
            });
        }

        private ObservableCollection<EventViewModel> CreateEventViewModels(IList<Event> events)
        {
            var eventVMs = new ObservableCollection<EventViewModel>();

            foreach (var @event in events)
            {
                var eventVM = new EventViewModel
                {
                    Start = @event.Start,
                    End = @event.End,
                    EventType = @event.EventType,
                    Subject = @event.Subject
                };

                eventVMs.Add(eventVM);
            }

            return eventVMs;
        }

        public ObservableCollection<DayViewModel> Agenda { get; private set; } = new ObservableCollection<DayViewModel>();
    }
}
