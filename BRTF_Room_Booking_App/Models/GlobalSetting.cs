using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BRTF_Room_Booking_App.Models
{
    public class GlobalSetting
    {
        public int ID { get; set; }

        [Display(Name = "Start of Term Date")]
        [Required(ErrorMessage = "The start of term date cannot be after the end of term date.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartOfTermDate { get; set; }

        [Display(Name = "End of Term Date")]
        [Required(ErrorMessage = "The end of term date cannot be before the start of term date.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EndOfTermDate { get; set; }

        [Display(Name = "Latest Allowable Day When Making Repeat Bookings")]
        [Required(ErrorMessage = "Please enter a number for the latest number of days to book in the future.")]
        public int LatestAllowableFutureBookingDay { get; set; }

        [Display(Name = "Email notifications for bookings even if user chooses not to receive notifications")]
        public bool EmailBookingNotificationsOverride { get; set; }

        [Display(Name = "Prevent email notifications for bookings even if user chooses to receive notifications")]
        public bool PreventBookingNotificationsOverride { get; set; }

        [Display(Name = "Email notifications for booking cancellations even if user chooses not to receive notifications")]
        public bool EmailCancelNotificationsOverride { get; set; }

        [Display(Name = "Prevent email notifications for booking cancellations even if user chooses to receive notifications")]
        public bool PreventCancelNotificationsOverride { get; set; }
    }
}
