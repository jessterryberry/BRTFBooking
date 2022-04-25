using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BRTF_Room_Booking_App.Utilities
{
    // Simplified class for a Booking Summary to make converting it to a JSON object for the calendar easier
    public class ChartJSON
    {
            public int id { get; set; }
            public string room { get; set; }
            public int numBookings { get; set; }
            public double hrsBookings { get; set; }
    }
}
