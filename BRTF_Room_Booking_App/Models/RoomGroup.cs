using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BRTF_Room_Booking_App.Models
{
    public class RoomGroup : IValidatableObject
    {
        public RoomGroup()
        {
            this.Rooms = new HashSet<Room>();
            this.RoomUserGroupPermissions = new HashSet<RoomUserGroupPermission>();
            this.RoomGroupApprovers = new HashSet<RoomGroupApprover>();
        }

        public int ID { get; set; }

        [Display(Name = "Area")]
        [Required(ErrorMessage = "Please enter an area name, up to a maximum of 100 characters.")]
        [StringLength(100, ErrorMessage = "Cannot be more than 100 characters long.")]
        public string AreaName { get; set; }

        [Display(Name = "Description")]
        [StringLength(1000, ErrorMessage = "Cannot be more than 1000 characters long.")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(Name = "Hours of Blackout Time Between Consecutive Bookings in This Area")]
        [Required(ErrorMessage = "Please enter a blackout time, in hours.")]
        public int BlackoutTime { get; set; }

        [Display(Name = "Maximum Hours Per Single Booking in This Area")]
        public int? MaxHoursPerSingleBooking { get; set; }

        [Display(Name = "Maximum Total Hours That Can Be Booked in This Area")]
        public int? MaxHoursTotal { get; set; }

        [Display(Name = "Maximum Number of Separate Bookings in This Area")]
        public int? MaxNumberOfBookings { get; set; }

        [Display(Name = "Apply Time-of-Day Restrictions")]
        public bool TimeOfDayRestrictions { get; set; }

        [Required(ErrorMessage = "Cannot be blank.")]
        [Display(Name = "Earliest Allowed Start Time in This Area")]
        [DataType(DataType.Time)]
        public DateTime EarliestTime { get; set; }

        [DataType(DataType.Time)]
        private DateTime _latestTime = new DateTime(3000, 1, 1, 23, 59, 0); //this enables us to set the LatestTime to default to 11:59pm

        [Required(ErrorMessage = "Cannot be blank.")]
        [Display(Name = "Latest Allowed End Time in This Area")]
        [DataType(DataType.Time)]
        public DateTime LatestTime
        {
            get { return _latestTime; }
            set { _latestTime = value; }
        }

        [Display(Name = "Area is Enabled")]
        public bool Enabled { get; set; }

        [Display(Name = "Bookings in this Area need Approval")]
        public bool NeedsApproval { get; set; }

        [Display(Name = "Rooms")]
        public ICollection<Room> Rooms { get; set; }

        [Display(Name = "Users that are allowed to approve Bookings in this Area")]
        public ICollection<RoomGroupApprover> RoomGroupApprovers { get; set; }

        [Display(Name = "Room User Group Permissions")]
        public ICollection<RoomUserGroupPermission> RoomUserGroupPermissions { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Test for invalid Start-End Time
            if (LatestTime <= EarliestTime)
            {
                yield return new ValidationResult("Latest Time a Booking Can be Made in This Area must be later than Earliest Time a Booking Can be Made in This Area.", new[] { "LatestTime" });
            }
        }
    }
}
