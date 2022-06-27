using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BRTF_Room_Booking_App.Models
{
    public class RoomBooking : IValidatableObject
    {
        public int ID { get; set; }

        [Display(Name = "Special Notes")]
        [StringLength(1000, ErrorMessage = "Cannot be more than 1000 characters long.")]
        [DataType(DataType.MultilineText)]
        [DisplayFormat(NullDisplayText = "None.")]
        public string SpecialNotes { get; set; }

        [Display(Name = "Approval Status")]
        [StringLength(50, ErrorMessage = "Cannot be more than 50 characters long.")]
        public string ApprovalStatus { get; set; }

        [Display(Name = "Start Date")]
        [Required(ErrorMessage = "Cannot be blank.")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy HH:mm}", HtmlEncode = false, ApplyFormatInEditMode = false)]
        public DateTime StartDate { get; set; }

        [Display(Name = "Start Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy HH:mm}", HtmlEncode = false, ApplyFormatInEditMode = false)]
        public DateTime RoundedStartDate 
        { 
            get
            {
                return RoundUp(StartDate, TimeSpan.FromMinutes(30));
            } 
        }


        [Display(Name = "End Date")]
        [Required(ErrorMessage = "Cannot be blank.")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy HH:mm}", HtmlEncode = false, ApplyFormatInEditMode = false)]
        public DateTime EndDate { get; set; }

        [Display(Name = "Start Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy HH:mm}", HtmlEncode = false, ApplyFormatInEditMode = false)]
        public DateTime RoundedEndDate
        {
            get
            {
                return RoundUp(EndDate, TimeSpan.FromMinutes(30));
            }
        }


        [Display(Name = "Room")]
        [Required(ErrorMessage = "You must select a Room.")]
        public int RoomID { get; set; }

        [Display(Name = "Room")]
        public Room Room { get; set; }

        [Display(Name = "User")]
        [Required(ErrorMessage = "You must assign a User.")]
        public int UserID { get; set; }

        [Display(Name = "User")]
        public User User { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Test for invalid Start-End Time
            if (EndDate <= StartDate)
            {
                yield return new ValidationResult("End Time must be later than Start Time.", new[] { "EndDate" });
            }
        }

        private DateTime RoundUp(DateTime dt, TimeSpan d)
        {
            return new DateTime((dt.Ticks + d.Ticks - 1) / d.Ticks * d.Ticks, dt.Kind);
        }
    }
}
