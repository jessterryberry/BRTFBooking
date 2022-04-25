using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BRTF_Room_Booking_App.ViewModels
{
    public class BookingSummary
    {
        public int ID { get; set; }

        [Display(Name = "Room")]
        [Required(ErrorMessage = "Cannot be blank.")]
        [StringLength(50, ErrorMessage = "Cannot be more than 50 characters long.")]
        public string RoomName { get; set; }


        [Display(Name = "Number of Bookings")]
        public int NumberOfAppointments { get; set; }

        [Display(Name = "Total Hours Booked")]
        public int TotalHours { get; set; }

        [Display(Name = "Room Group")]
        public string RoomGroup { get; set; }

    }
}
