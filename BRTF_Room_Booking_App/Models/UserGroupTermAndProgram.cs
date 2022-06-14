using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BRTF_Room_Booking_App.Models
{
    public class UserGroupTermAndProgram
    {
        [Display(Name = "User Group")]
        [Required(ErrorMessage = "You must assign a User Group.")]
        public int UserGroupID { get; set; }

        [Display(Name = "User Group")]
        public UserGroup UserGroup { get; set; }

        [Display(Name = "Term and Program")]
        [Required(ErrorMessage = "You must assign a Term and Program.")]
        public int TermAndProgramID { get; set; }

        [Display(Name = "Term and Program")]
        public TermAndProgram TermAndProgram { get; set; }
    }
}
