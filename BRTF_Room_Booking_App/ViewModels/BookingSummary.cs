using OfficeOpenXml.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BRTF_Room_Booking_App.ViewModels
{
    public class BookingSummary
    {
        [EpplusIgnore]
        public int ID { get; set; }

        [DisplayName("Room")]
        [Display(Name = "Room")]
        [Required(ErrorMessage = "Cannot be blank.")]
        [StringLength(50, ErrorMessage = "Cannot be more than 50 characters long.")]
        public string RoomName { get; set; }

        [DisplayName("Number of Bookings")]
        [Display(Name = "Number of Bookings")]
        public int NumberOfAppointments { get; set; }

        [DisplayName("Total Hours Booked")]
        [Display(Name = "Total Hours Booked")]
        public int TotalHours { get; set; }

        [DisplayName("Area")]
        [Display(Name = "Area")]
        public string Area { get; set; }

    }
}
