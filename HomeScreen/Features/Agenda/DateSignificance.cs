using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScreen.Features.Agenda
{
    public class DateSignificanceData
    {
        public DateSignificanceData()
        {
            dagar = new List<Dagar>();
        }

        public string cachetid { get; set; }
        public string version { get; set; }
        public string uri { get; set; }
        public string startdatum { get; set; }
        public string slutdatum { get; set; }
        public List<Dagar> dagar { get; set; }
        public static DateSignificanceData Empty { get; } = new DateSignificanceData();
    }
    public class Dagar
    {
        public string datum { get; set; }
        public string veckodag { get; set; }
        public string vecka { get; set; }
        public string helgdag { get; set; }
        public string helgdagsafton { get; set; }
        public List<string> namnsdag { get; set; }
        public string flaggdag { get; set; }
    }    
}
