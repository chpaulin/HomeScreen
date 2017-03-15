namespace HomeScreen.Features.CarInfo.Model
{
    public class ChargeState
    {
        public Response response { get; set; }
    }

    public class Response
    {
        public string charging_state { get; set; } //"Complete", "Charging", ??
        public bool charge_to_max_range { get; set; }
        public int max_range_charge_counter { get; set; }
        public bool fast_charger_present { get; set; }
        public double battery_range { get; set; } // rated miles
        public double est_battery_range { get; set; } // range estimated from recent driving
        public double ideal_battery_range { get; set; } // ideal miles
        public int battery_level { get; set; } // integer charge percentage
        public double battery_current { get; set; } // current flowing into battery
        public object charge_starting_range { get; set; }
        public object charge_starting_soc { get; set; }
        public int charger_voltage { get; set; } // only has value while charging
        public int charger_pilot_current { get; set; } // max current allowed by charger & adapter
        public int charger_actual_current { get; set; } // current actually being drawn
        public int charger_power { get; set; } // kW (rounded down) of charger
        public object time_to_full_charge { get; set; } // valid only while charging
        public double charge_rate { get; set; } // float mi/hr charging or -1 if not charging
        public bool charge_port_door_open { get; set; }
    }
}