using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BRTF_Room_Booking_App.Models
{
    public class AreaApprover
    {
        [Display(Name = "User")]
        [Required(ErrorMessage = "You must assign a User.")]
        public int UserID { get; set; }

        [Display(Name = "User")]
        public User User { get; set; }

        [Display(Name = "Area")]
        [Required(ErrorMessage = "You must assign a Area.")]
        public int AreaID { get; set; }

        [Display(Name = "Area")]
        public Area Area { get; set; }


    }
}
