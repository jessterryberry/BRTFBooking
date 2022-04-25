using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BRTF_Room_Booking_App.Models
{
    public class TermAndProgram
    {
        public TermAndProgram()
        {
            this.Users = new HashSet<User>();
        }

        public int ID { get; set; }

        [Display(Name = "Term and Program")]
        public string TermAndProgramSummary
        {
            get
            {
                // Generate return string.
                // Example output: "P0164 - Level 1, TV Production"
                return (!ProgramName.ToUpper().Contains("ADMIN") ? ProgramCode + " - Level " + ProgramLevel.ToString() + ", " : "") + ProgramName;
            }
        }

        [Display(Name = "Program Name")]
        [Required(ErrorMessage = "Please enter a program name, up to a maximum of 50 characters.")]
        [StringLength(50, ErrorMessage = "Cannot be more than 50 characters long.")]
        public string ProgramName { get; set; }

        [Display(Name = "Program Code")]
        [Required(ErrorMessage = "Please enter a valid 5 character Code (e.g. P0122).")]
        [StringLength(5, ErrorMessage = "Cannot be more than 5 characters long.")]
        [RegularExpression("^P\\d{4}$", ErrorMessage = "Please enter a valid 5 character Code (e.g. P0122).")]
        public string ProgramCode { get; set; }

        [Display(Name = "Program Level")]
        [Required(ErrorMessage = "Please enter a program level greater than 1.")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Must be greater than or equal to 1.")]
        public int ProgramLevel { get; set; }

        [Display(Name = "User Group")]
        [Required(ErrorMessage = "You must assign a User Group.")]        
        public int UserGroupID { get; set; }

        [Display(Name = "User Group")]
        public UserGroup UserGroup { get; set; }

        [Display(Name = "Users")]
        public ICollection<User> Users { get; set; }
    }
}
