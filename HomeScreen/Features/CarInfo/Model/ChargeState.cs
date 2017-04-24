namespace HomeScreen.Features.CarInfo.Model
{
    public class ChargeState
    {
        public static ChargeState Empty => new ChargeState { response = new Response() };
        public Response response { get; set; }
    }

    public class Response
    {
        public double est_battery_range { get; set; } // range estimated from recent driving
        public int battery_level { get; set; } // integer charge percentage
        public double ideal_battery_range { get; set; } // ideal range
    }
}