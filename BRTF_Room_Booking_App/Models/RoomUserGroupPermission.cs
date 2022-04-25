using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BRTF_Room_Booking_App.Models
{
    public class RoomUserGroupPermission
    {
        [Display(Name = "User Group")]
        [Required(ErrorMessage = "You must assign a User Group.")]
        public int UserGroupID { get; set; }

        [Display(Name = "User Group")]
        public UserGroup UserGroup { get; set; }

        [Display(Name = "Room Group")]
        [Required(ErrorMessage = "You must assign a Room Group.")]
        public int RoomGroupID { get; set; }

        [Display(Name = "Room Group")]
        public RoomGroup RoomGroup { get; set; }
    }
}
