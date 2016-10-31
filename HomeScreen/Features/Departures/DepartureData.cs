using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScreen.Features.Departures
{
    public class DepartureData
    {
        public static DepartureData Empty { get; } = new DepartureData();
        public List<Departure> Departure { get; set; } = new List<Departures.Departure>();
    }

    public class Product
    {
        public string name { get; set; }
        public string num { get; set; }
        public string catCode { get; set; }
        public string catOutS { get; set; }
        public string catOutL { get; set; }
        public string operatorCode { get; set; }
        public string @operator { get; set; }
        public string operatorUrl { get; set; }
    }

    public class Departure
    {
        public Product Product { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string stop { get; set; }
        public int stopid { get; set; }
        public string stopExtId { get; set; }
        public string time { get; set; }
        public string date { get; set; }
        public string direction { get; set; }
        public string transportNumber { get; set; }
        public string transportCategory { get; set; }
        public string rtTime { get; set; }
        public string rtDate { get; set; }
    }
}
