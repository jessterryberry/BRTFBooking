using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BRTF_Room_Booking_App.Models
{
    public class Room
    {
        public Room()
        {
            this.RoomBookings = new HashSet<RoomBooking>();
        }

        public int ID { get; set; }

        [Display(Name = "Enabled")]
        public string EnabledSummary
        {
            get
            {
                return (Enabled) ? "Enabled" : "Disabled";
            }
        }

        [Display(Name = "Room")]
        [Required(ErrorMessage = "Please enter a room name, up to 50 characters in length.")]
        [StringLength(50, ErrorMessage = "Cannot be more than 50 characters long.")]
        public string RoomName { get; set; }

        [Display(Name = "Maximum Total Hours That Can Be Booked in This Room")]
        public int? RoomMaxHoursTotal { get; set; }

        [Display(Name = "Room is Enabled")]
        public bool Enabled { get; set; }

        [Display(Name = "Area")]
        [Required(ErrorMessage = "You must assign an Area.")]
        public int RoomGroupID { get; set; }

        [Display(Name = "Area")]
        public RoomGroup RoomGroup { get; set; }

        [Display(Name = "Bookings")]
        public ICollection<RoomBooking> RoomBookings { get; set; }
    }
}
