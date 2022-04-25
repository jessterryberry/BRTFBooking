using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BRTF_Room_Booking_App.Utilities
{
    // Simplified class for a Room to make converting it to a JSON object for the calendar easier
    public class BookingJSON
    {
            public int id { get; set; }
            public int resourceId { get; set; }
            public string start { get; set; }
            public string end { get; set; }
            public string title { get; set; }
    }
}
