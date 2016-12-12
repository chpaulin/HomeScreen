using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScreen.Features.Agenda
{
    public class DayViewModel : ViewModelBase
    {
        public DateTime Date { get; set; }

        public string Day { get; set; }

        public ObservableCollection<EventViewModel> Events { get; set; }
    }
}
