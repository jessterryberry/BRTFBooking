using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BRTF_Room_Booking_App.ViewModels
{
    public class CheckOptionVM
    {
        //Used for a Checkbox
        public int ID { get; set; }
        public string DisplayText { get; set; }
        public bool Assigned { get; set; }
    }
}
