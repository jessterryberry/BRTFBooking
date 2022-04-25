using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BRTF_Room_Booking_App.Models
{
    public class UserGroup
    {
        public UserGroup()
        {
            this.TermAndPrograms = new HashSet<TermAndProgram>();
            this.RoomUserGroupPermissions = new HashSet<RoomUserGroupPermission>();
        }

        public int ID { get; set; }

        [Display(Name = "User Group")]
        [Required(ErrorMessage = "Please enter a User Group name, up to a maximum of 20 characters.")]
        [StringLength(20, ErrorMessage = "Cannot be more than 20 characters long.")]
        public string UserGroupName { get; set; }

        [Display(Name = "Terms and Programs")]
        public ICollection<TermAndProgram> TermAndPrograms { get; set; }

        [Display(Name = "Room Group Permissions")]
        public ICollection<RoomUserGroupPermission> RoomUserGroupPermissions { get; set; }
    }
}
