using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BRTF_Room_Booking_App.Utilities
{
    // Simplified class for a Room Booking to make converting it to a JSON object for the calendar easier
    public class RoomJSON
    {
        public int id { get; set; }
        public string building { get; set; }
        public string title { get; set; }
    }

}
