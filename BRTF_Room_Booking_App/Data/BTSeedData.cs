using BRTF_Room_Booking_App.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BRTF_Room_Booking_App.Data
{
    public static class BTSeedData
    {
        /// <summary>
        /// This method seeds data for testing purposes. Write your test data inside this method.
        /// </summary>
        public static void SeedTestData(IServiceProvider serviceProvider)
        {
            using (var context = new BTRFRoomBookingContext(
                serviceProvider.GetRequiredService<DbContextOptions<BTRFRoomBookingContext>>()))
            {
                Random random = new Random();   // To randomly generate data

                // Seed Room Groups
                if (!context.RoomGroups.Any())
                {
                    // Construct Room Group details
                    var roomGroups = new List<RoomGroup>
                    {
                    new RoomGroup()
                    {
                        AreaName = "Edit 13 BRTF1435 & 3yr TV",
                        Description = "You can book up to 4 hours at a time and have up to 3 future bookingss.\r\nThis suite contains:\r\nMedia Composer, Adobe Suite, DaVinci Resolve, Pro Tools.\r\nBookable by 4th Term Film/TV or 5th Term TV.",
                        BlackoutTime = 1,
                        MaxHoursPerSingleBooking = 4,
                        MaxHoursTotal = Int32.MaxValue,
                        MaxNumberOfBookings = 3,
                        Enabled = true
                    },
                    new RoomGroup()
                    {
                        AreaName = "Edit 15 BRTF1435, Term 5",
                        Description = "You can book up to 4 hours at a time.\r\nThis suite contains:\r\nMedia Composer, Pro Tools, DaVinci Resolve, Creative Suite.\r\nBookable by 4th Term Film/TV or 5th Term TV.",
                        BlackoutTime = 1,
                        MaxHoursPerSingleBooking = 4,
                        MaxHoursTotal = Int32.MaxValue,
                        MaxNumberOfBookings = Int32.MaxValue,
                        Enabled = true
                    },
                    new RoomGroup()
                    {
                        AreaName = "Edit 6, 3rd Year only",
                        Description = "You can book up to 6 hours at a time and have up to 2 future bookings.\r\nThis suite contains:\r\nMedia Composer, Pro Tools, Resolve, Creative Suite.",
                        BlackoutTime = 1,
                        MaxHoursPerSingleBooking = 6,
                        MaxHoursTotal = Int32.MaxValue,
                        MaxNumberOfBookings = 2,
                        Enabled = true
                    },
                    new RoomGroup()
                    {
                        AreaName = "Edit 8 Inside Niagara",
                        Description = "You can book up to 2 hours at a time.\r\nThis suite contains:\r\nCreative Suite, Media Composer, DaVinci Resolve.\r\nBookable by 3rd term Presentation and 4th term TV.",
                        BlackoutTime = 1,
                        MaxHoursPerSingleBooking = 2,
                        MaxHoursTotal = Int32.MaxValue,
                        MaxNumberOfBookings = Int32.MaxValue,
                        Enabled = true
                    },
                    new RoomGroup()
                    {
                        AreaName = "Edit 9, 10 and 14, 2nd Years",
                        Description = "You can book up to 6 hours at a time.\r\nThis suite contains:\r\nCreative Suite, Media Composer, DaVinci Resolve, Pro Tools.\r\nBookable by 2nd Year students and 3rd Year Presentation.",
                        BlackoutTime = 1,
                        MaxHoursPerSingleBooking = 6,
                        MaxHoursTotal = Int32.MaxValue,
                        MaxNumberOfBookings = Int32.MaxValue,
                        Enabled = true
                    },
                    new RoomGroup()
                    {
                        AreaName = "Edits 1-5, 3rd Year Film",
                        Description = "You can book up to 6 hours at a time and have up to 3 future bookings.\r\nThis suite contains:\r\nCreative Suite, Media Composer, DaVinci Resolve, Pro Tools.\r\nBookable by 3rd Year Film only. All others won't be approved without a signed building pass.",
                        BlackoutTime = 1,
                        MaxHoursPerSingleBooking = 6,
                        MaxHoursTotal = Int32.MaxValue,
                        MaxNumberOfBookings = 3,
                        Enabled = true
                    },
                    new RoomGroup()
                    {
                        AreaName = "Film Studio V001",
                        Description = "",
                        BlackoutTime = 1,
                        MaxHoursPerSingleBooking = Int32.MaxValue,
                        MaxHoursTotal = Int32.MaxValue,
                        MaxNumberOfBookings = Int32.MaxValue,
                        Enabled = true
                    },
                    new RoomGroup()
                    {
                        AreaName = "Green Room",
                        Description = "Max bookable time is 12 hours.\r\nRoom typically for teams preparing for a TV or film shoot.",
                        BlackoutTime = 1,
                        MaxHoursPerSingleBooking = 12,
                        MaxHoursTotal = Int32.MaxValue,
                        MaxNumberOfBookings = Int32.MaxValue,
                        Enabled = true
                    },
                    new RoomGroup()
                    {
                        AreaName = "MAC Lab V106",
                        Description = "Max bookable time is 6 hours.\r\nAll MACs Contain:\r\nMS Office, Adobe Suite, Media Composer, DaVinci Resolve, Pro Tools.\r\n17 computers.",
                        BlackoutTime = 1,
                        MaxHoursPerSingleBooking = 6,
                        MaxHoursTotal = 6,
                        MaxNumberOfBookings = Int32.MaxValue,
                        Enabled = true
                    },
                    new RoomGroup()
                    {
                        AreaName = "Mixing Theatre V105",
                        Description = "Max bookable time is 8 hours.\r\nAvailable after class until midnight Monday to Friday. Weekends off limits.\r\nApproval from Luke Hutton before use.",
                        BlackoutTime = 1,
                        MaxHoursPerSingleBooking = 8,
                        MaxHoursTotal = 8,
                        MaxNumberOfBookings = Int32.MaxValue,
                        Enabled = true
                    },
                    new RoomGroup()
                    {
                        AreaName = "Radio Edit Suites V109",
                        Description = "You can book up to 4 hours at a time.\r\n8 audio editing computers.",
                        BlackoutTime = 1,
                        MaxHoursPerSingleBooking = 4,
                        MaxHoursTotal = Int32.MaxValue,
                        MaxNumberOfBookings = Int32.MaxValue,
                        Enabled = true
                    },
                    new RoomGroup()
                    {
                        AreaName = "Radio Recording Studios V109",
                        Description = "You can book up to 2 hours in a studio.\r\nAll studios have:\r\nPhone access for interviews.\r\nAnnounce booth 1 is used for news and sports.\r\nAnnounce booth 2 is used for voice tracking.",
                        BlackoutTime = 1,
                        MaxHoursPerSingleBooking = 2,
                        MaxHoursTotal = Int32.MaxValue,
                        MaxNumberOfBookings = Int32.MaxValue,
                        Enabled = true
                    },
                    new RoomGroup()
                    {
                        AreaName = "TV Studio V002",
                        Description = "You can book up to 18 hours.\r\nDifferent rooms have varying booking times. Please check to confirm:\r\nV2 TV Studio, Max Bookable Hours 2.\r\nV2 GreenRoom, Max Bookable Hours 6.\r\nV1 (Old Studio), Max Bookable Hours 2.\r\n1st year students may reserve the studio as per instructor instructions.\r\nAll others must obtain approval through Alysha Henderson.",
                        BlackoutTime = 1,
                        MaxHoursPerSingleBooking = 18,
                        MaxHoursTotal = 18,
                        MaxNumberOfBookings = Int32.MaxValue,
                        Enabled = true
                    },
                    new RoomGroup()
                    {
                        AreaName = "V110",
                        Description = "",
                        BlackoutTime = 1,
                        MaxHoursPerSingleBooking = Int32.MaxValue,
                        MaxHoursTotal = Int32.MaxValue,
                        MaxNumberOfBookings = Int32.MaxValue,
                        Enabled = true
                    },
                    new RoomGroup()
                    {
                        AreaName = "V110 Acting Lab",
                        Description = "You can book up to 2 hours at a time.\r\nBookings is off-limits from 12:30 A.M. to the end of classes, Monday-Friday.\r\nFor exceptions, approval must be granted by Lori Ravensborg.",
                        BlackoutTime = 1,
                        MaxHoursPerSingleBooking = 2,
                        MaxHoursTotal = Int32.MaxValue,
                        MaxNumberOfBookings = Int32.MaxValue,
                        Enabled = true
                    },
                    new RoomGroup()
                    {
                        AreaName = "V110f Acting Edit",
                        Description = "",
                        BlackoutTime = 1,
                        MaxHoursPerSingleBooking = Int32.MaxValue,
                        MaxHoursTotal = Int32.MaxValue,
                        MaxNumberOfBookings = Int32.MaxValue,
                        Enabled = true
                    },
                    new RoomGroup()
                    {
                        AreaName = "V204p Production Planning",
                        Description = "You can book up to 1 hour.\r\nBRTF Project Meeting Room.\r\nOnly available Monday-Friday between 8:30 A.M. - 5:30 P.M.",
                        BlackoutTime = 1,
                        MaxHoursPerSingleBooking = 1,
                        MaxHoursTotal = 1,
                        MaxNumberOfBookings = Int32.MaxValue,
                        Enabled = true
                    },
                    new RoomGroup()
                    {
                        AreaName = "Camera Test",
                        Description = "",
                        BlackoutTime = 1,
                        MaxHoursPerSingleBooking = Int32.MaxValue,
                        MaxHoursTotal = Int32.MaxValue,
                        MaxNumberOfBookings = Int32.MaxValue,
                        Enabled = false
                    },
                    new RoomGroup()
                    {
                        AreaName = "Edit 16 BRTF1435, Term 5 TV",
                        Description = "Max Booking 4-hours\r\nAll studios have:\r\nP2 Reader, Digitize/Log/Print Deck, SoundTrack, Avid, Final Cut Pro, DiffMerge, AdobeCS Suite, Aspera Connect\r\n4th term Film/TV or 5th term TV students only.",
                        BlackoutTime = 1,
                        MaxHoursPerSingleBooking = 4,
                        MaxHoursTotal = Int32.MaxValue,
                        MaxNumberOfBookings = Int32.MaxValue,
                        Enabled = false
                    },
                    new RoomGroup()
                    {
                        AreaName = "MultiTrack V1j",
                        Description = "Max Booking 2-hours\r\nAll studios have:\r\nMixing Board, Attached Audio Booth, SoundTrack Pro\r\n*Note* SoundTrack Pro is on all of the edit suites and Mac Lab",
                        BlackoutTime = 1,
                        MaxHoursPerSingleBooking = 2,
                        MaxHoursTotal = Int32.MaxValue,
                        MaxNumberOfBookings = Int32.MaxValue,
                        Enabled = false
                    },
                    new RoomGroup()
                    {
                        AreaName = "V011 Assignment/Offload",
                        Description = "Not bookable for meetings.\r\nOpen Access space for assignment finishing or media transfer.",
                        BlackoutTime = 1,
                        MaxHoursPerSingleBooking = Int32.MaxValue,
                        MaxHoursTotal = Int32.MaxValue,
                        MaxNumberOfBookings = Int32.MaxValue,
                        Enabled = false
                    },
                    new RoomGroup()
                    {
                        AreaName = "V2 and S339 Acting",
                        Description = "Max Booking 1-hour.",
                        BlackoutTime = 1,
                        MaxHoursPerSingleBooking = 1,
                        MaxHoursTotal = Int32.MaxValue,
                        MaxNumberOfBookings = Int32.MaxValue,
                        Enabled = false
                    },
                    new RoomGroup()
                    {
                        AreaName = "V3 Demonstration Lab",
                        Description = "Max Booking 6-hours.",
                        BlackoutTime = 1,
                        MaxHoursPerSingleBooking = 6,
                        MaxHoursTotal = Int32.MaxValue,
                        MaxNumberOfBookings = Int32.MaxValue,
                        Enabled = false
                    }
                    };

                    context.RoomGroups.AddRange(roomGroups);
                    context.SaveChanges();
                }

                // Seed Rooms
                if (!context.Rooms.Any())
                {
                    var rooms = new List<Room>
                    {
                        new Room()
                        {
                            RoomName = "Red Camera 1",
                            RoomMaxHoursTotal = Int32.MaxValue,
                            Enabled = false,
                            RoomGroupID = context.RoomGroups.FirstOrDefault(u => u.AreaName.ToUpper().Contains("CAMERA TEST")).ID
                        },
                        new Room()
                        {
                            RoomName = "Edit 13",
                            RoomMaxHoursTotal = 4,
                            Enabled = true,
                            RoomGroupID = context.RoomGroups.FirstOrDefault(u => u.AreaName.ToUpper().Contains("EDIT 13 BRTF1435 & 3YR TV")).ID
                        },
                        new Room()
                        {
                            RoomName = "Edit 15",
                            RoomMaxHoursTotal = 4,
                            Enabled = true,
                            RoomGroupID = context.RoomGroups.FirstOrDefault(u => u.AreaName.ToUpper().Contains("EDIT 15 BRTF1435, TERM 5")).ID
                        },
                        new Room()
                        {
                            RoomName = "Edit 6",
                            RoomMaxHoursTotal = 6,
                            Enabled = true,
                            RoomGroupID = context.RoomGroups.FirstOrDefault(u => u.AreaName.ToUpper().Contains("EDIT 6, 3RD YEAR ONLY")).ID
                        },
                        new Room()
                        {
                            RoomName = "Edit 8 V204i",
                            RoomMaxHoursTotal = 2,
                            Enabled = true,
                            RoomGroupID = context.RoomGroups.FirstOrDefault(u => u.AreaName.ToUpper().Contains("EDIT 8 INSIDE NIAGARA")).ID
                        },
                        new Room()
                        {
                            RoomName = "Edit 9",
                            RoomMaxHoursTotal = 6,
                            Enabled = true,
                            RoomGroupID = context.RoomGroups.FirstOrDefault(u => u.AreaName.ToUpper().Contains("EDIT 9, 10 AND 14, 2ND YEARS")).ID
                        },
                        new Room()
                        {
                            RoomName = "Edit 10",
                            RoomMaxHoursTotal = 6,
                            Enabled = true,
                            RoomGroupID = context.RoomGroups.FirstOrDefault(u => u.AreaName.ToUpper().Contains("EDIT 9, 10 AND 14, 2ND YEARS")).ID
                        },
                        new Room()
                        {
                            RoomName = "Edit 14",
                            RoomMaxHoursTotal = 6,
                            Enabled = true,
                            RoomGroupID = context.RoomGroups.FirstOrDefault(u => u.AreaName.ToUpper().Contains("EDIT 9, 10 AND 14, 2ND YEARS")).ID
                        },
                        new Room()
                        {
                            RoomName = "Edit 1/2 Colour Suite",
                            RoomMaxHoursTotal = 6,
                            Enabled = true,
                            RoomGroupID = context.RoomGroups.FirstOrDefault(u => u.AreaName.ToUpper().Contains("EDITS 1-5, 3RD YEAR FILM")).ID
                        },
                        new Room()
                        {
                            RoomName = "Edit 3",
                            RoomMaxHoursTotal = 6,
                            Enabled = false,
                            RoomGroupID = context.RoomGroups.FirstOrDefault(u => u.AreaName.ToUpper().Contains("EDITS 1-5, 3RD YEAR FILM")).ID
                        },
                        new Room()
                        {
                            RoomName = "Edit 4",
                            RoomMaxHoursTotal = 6,
                            Enabled = false,
                            RoomGroupID = context.RoomGroups.FirstOrDefault(u => u.AreaName.ToUpper().Contains("EDITS 1-5, 3RD YEAR FILM")).ID
                        },
                        new Room()
                        {
                            RoomName = "Edit 5",
                            RoomMaxHoursTotal = 6,
                            Enabled = true,
                            RoomGroupID = context.RoomGroups.FirstOrDefault(u => u.AreaName.ToUpper().Contains("EDITS 1-5, 3RD YEAR FILM")).ID
                        },
                        new Room()
                        {
                            RoomName = "Film Studio V001",
                            RoomMaxHoursTotal = Int32.MaxValue,
                            Enabled = true,
                            RoomGroupID = context.RoomGroups.FirstOrDefault(u => u.AreaName.ToUpper().Contains("FILM STUDIO V001")).ID
                        },
                        new Room()
                        {
                            RoomName = "Green Room",
                            RoomMaxHoursTotal = 12,
                            Enabled = true,
                            RoomGroupID = context.RoomGroups.FirstOrDefault(u => u.AreaName.ToUpper().Contains("GREEN ROOM")).ID
                        },
                        new Room()
                        {
                            RoomName = "Mixing Theatre V5",
                            RoomMaxHoursTotal = 8,
                            Enabled = true,
                            RoomGroupID = context.RoomGroups.FirstOrDefault(u => u.AreaName.ToUpper().Contains("MIXING THEATRE V105")).ID
                        },
                        new Room()
                        {
                            RoomName = "Studio & Talk A",
                            RoomMaxHoursTotal = 2,
                            Enabled = true,
                            RoomGroupID = context.RoomGroups.FirstOrDefault(u => u.AreaName.ToUpper().Contains("RADIO RECORDING STUDIOS V109")).ID
                        },
                        new Room()
                        {
                            RoomName = "Studio B",
                            RoomMaxHoursTotal = 2,
                            Enabled = true,
                            RoomGroupID = context.RoomGroups.FirstOrDefault(u => u.AreaName.ToUpper().Contains("RADIO RECORDING STUDIOS V109")).ID
                        },
                        new Room()
                        {
                            RoomName = "Studio C",
                            RoomMaxHoursTotal = 2,
                            Enabled = true,
                            RoomGroupID = context.RoomGroups.FirstOrDefault(u => u.AreaName.ToUpper().Contains("RADIO RECORDING STUDIOS V109")).ID
                        },
                        new Room()
                        {
                            RoomName = "Studio D",
                            RoomMaxHoursTotal = 2,
                            Enabled = true,
                            RoomGroupID = context.RoomGroups.FirstOrDefault(u => u.AreaName.ToUpper().Contains("RADIO RECORDING STUDIOS V109")).ID
                        },
                        new Room()
                        {
                            RoomName = "Annc. 2",
                            RoomMaxHoursTotal = 2,
                            Enabled = true,
                            RoomGroupID = context.RoomGroups.FirstOrDefault(u => u.AreaName.ToUpper().Contains("RADIO RECORDING STUDIOS V109")).ID
                        },
                        new Room()
                        {
                            RoomName = "V2 TV Studio",
                            RoomMaxHoursTotal = 2,
                            Enabled = true,
                            RoomGroupID = context.RoomGroups.FirstOrDefault(u => u.AreaName.ToUpper().Contains("TV STUDIO V002")).ID
                        },
                        new Room()
                        {
                            RoomName = "V2 GreenRoom",
                            RoomMaxHoursTotal = 6,
                            Enabled = true,
                            RoomGroupID = context.RoomGroups.FirstOrDefault(u => u.AreaName.ToUpper().Contains("TV STUDIO V002")).ID
                        },
                        new Room()
                        {
                            RoomName = "V1",
                            RoomMaxHoursTotal = 2,
                            Enabled = true,
                            RoomGroupID = context.RoomGroups.FirstOrDefault(u => u.AreaName.ToUpper().Contains("TV STUDIO V002")).ID
                        },
                        new Room()
                        {
                            RoomName = "TV Studio Control Room",
                            RoomMaxHoursTotal = 2,
                            Enabled = true,
                            RoomGroupID = context.RoomGroups.FirstOrDefault(u => u.AreaName.ToUpper().Contains("TV STUDIO V002")).ID
                        },
                        new Room()
                        {
                            RoomName = "V110",
                            RoomMaxHoursTotal = Int32.MaxValue,
                            Enabled = true,
                            RoomGroupID = context.RoomGroups.FirstOrDefault(u => u.AreaName.ToUpper() == "V110").ID
                        },
                        new Room()
                        {
                            RoomName = "Acting Lab V110",
                            RoomMaxHoursTotal = 2,
                            Enabled = true,
                            RoomGroupID = context.RoomGroups.FirstOrDefault(u => u.AreaName.ToUpper().Contains("V110 ACTING LAB")).ID
                        },
                        new Room()
                        {
                            RoomName = "V110g Acting Edit",
                            RoomMaxHoursTotal = 2,
                            Enabled = true,
                            RoomGroupID = context.RoomGroups.FirstOrDefault(u => u.AreaName.ToUpper().Contains("V110 ACTING LAB")).ID
                        },
                        new Room()
                        {
                            RoomName = "V110f Acting Edit",
                            RoomMaxHoursTotal = Int32.MaxValue,
                            Enabled = true,
                            RoomGroupID = context.RoomGroups.FirstOrDefault(u => u.AreaName.ToUpper().Contains("V110F ACTING EDIT")).ID
                        },
                        new Room()
                        {
                            RoomName = "V204p Production Planning",
                            RoomMaxHoursTotal = 1,
                            Enabled = true,
                            RoomGroupID = context.RoomGroups.FirstOrDefault(u => u.AreaName.ToUpper().Contains("V204P PRODUCTION PLANNING")).ID
                        },
                        new Room()
                        {
                            RoomName = "Edit 16 Avid/P2/DigLotPrt",
                            RoomMaxHoursTotal = 4,
                            Enabled = false,
                            RoomGroupID = context.RoomGroups.FirstOrDefault(u => u.AreaName.ToUpper().Contains("EDIT 16 BRTF1435, TERM 5 TV")).ID
                        },
                        new Room()
                        {
                            RoomName = "MultiTrack V1j",
                            RoomMaxHoursTotal = 2,
                            Enabled = false,
                            RoomGroupID = context.RoomGroups.FirstOrDefault(u => u.AreaName.ToUpper().Contains("MULTITRACK V1J")).ID
                        },
                        new Room()
                        {
                            RoomName = "V011 Assignment/Offload",
                            RoomMaxHoursTotal = Int32.MaxValue,
                            Enabled = false,
                            RoomGroupID = context.RoomGroups.FirstOrDefault(u => u.AreaName.ToUpper().Contains("V011 ASSIGNMENT/OFFLOAD")).ID
                        },
                        new Room()
                        {
                            RoomName = "V2 Acting",
                            RoomMaxHoursTotal = 1,
                            Enabled = false,
                            RoomGroupID = context.RoomGroups.FirstOrDefault(u => u.AreaName.ToUpper().Contains("V2 AND S339 ACTING")).ID
                        },
                        new Room()
                        {
                            RoomName = "S339",
                            RoomMaxHoursTotal = 1,
                            Enabled = false,
                            RoomGroupID = context.RoomGroups.FirstOrDefault(u => u.AreaName.ToUpper().Contains("V2 AND S339 ACTING")).ID
                        },
                        new Room()
                        {
                            RoomName = "V3 Demonstration Lab",
                            RoomMaxHoursTotal = 6,
                            Enabled = false,
                            RoomGroupID = context.RoomGroups.FirstOrDefault(u => u.AreaName.ToUpper().Contains("V3 DEMONSTRATION LAB")).ID
                        }
                    };
                    // Loop to seed 17 computers in MAC Lab V106
                    for (int i = 1; i <= 17; i++)
                {
                    // Construct Room details
                    Room room = new Room()
                    {
                        RoomName = "Computer " + i.ToString(),
                        RoomMaxHoursTotal = Int32.MaxValue,
                        Enabled = true,
                        RoomGroupID = context.RoomGroups.FirstOrDefault(u => u.AreaName.ToUpper().Contains("MAC LAB V106")).ID
                    };
                    rooms.Add(room);
                }
                //Loop to seed 8 computers in Radio Edit V109
                for (int i = 1; i <= 8; i++)
                {
                    //Construct Room details
                    Room room = new Room()
                    {
                        RoomName = "Audio Edit #" + i.ToString(),
                        RoomMaxHoursTotal = 4,
                        Enabled = true,
                        RoomGroupID = context.RoomGroups.FirstOrDefault(u => u.AreaName.ToUpper().Contains("RADIO EDIT SUITES V109")).ID
                    };
                    rooms.Add(room);
                }
                context.Rooms.AddRange(rooms);
                    context.SaveChanges();
                }

                // Seed Room User Group Permissions
                if (!context.RoomUserGroupPermissions.Any())
                {
                    // Give Admins access to all Areas
                    int[] adminGroupIDs = context.UserGroups.Where(u => u.UserGroupName == "top-level admin" || u.UserGroupName == "admin").Select(u => u.ID).ToArray();
                    int[] allRoomGroupIDs = context.RoomGroups.Select(r => r.ID).ToArray();

                    foreach (int adminGroupID in adminGroupIDs)
                    {
                        foreach (int roomGroupID in allRoomGroupIDs)
                        {
                            // Construct Permission details
                            RoomUserGroupPermission roomUserGroupPermission = new RoomUserGroupPermission()
                            {
                                UserGroupID = adminGroupID,
                                RoomGroupID = roomGroupID
                            };
                            try
                            {
                                // Could be duplicates
                                context.RoomUserGroupPermissions.Add(roomUserGroupPermission);
                                context.SaveChanges();
                            }
                            catch (Exception e)
                            {
                                var m = e.Message;
                                // So skip it and go on to the next
                            }
                        }
                    }

                    // Give all User Groups permission to access MAC Lab V106
                    int[] nonAdminUserGroupIDs = context.UserGroups.Where(u => u.UserGroupName != "top-level admin" && u.UserGroupName != "admin").Select(u => u.ID).ToArray();

                    foreach (int userGroupID in nonAdminUserGroupIDs)
                    {
                        // Construct Permission details
                        RoomUserGroupPermission roomUserGroupPermission = new RoomUserGroupPermission()
                        {
                            UserGroupID = userGroupID,
                            RoomGroupID = context.RoomGroups.FirstOrDefault(u => u.AreaName.ToUpper().Contains("MAC LAB V106")).ID
                        };
                        try
                        {
                            // Could be duplicates
                            context.RoomUserGroupPermissions.Add(roomUserGroupPermission);
                            context.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            var m = e.Message;
                            // So skip it and go on to the next
                        }
                    }

                    // Give Users in "pres" access to "Radio" Rooms
                    int[] presUserGroupIDs = context.UserGroups.Where(u => u.UserGroupName.ToUpper().Contains("PRES")).Select(u => u.ID).ToArray();
                    int[] radioRoomGroupIDs = context.RoomGroups.Where(r => r.AreaName.ToUpper().Contains("RADIO")).Select(r => r.ID).ToArray();

                    foreach (int presUserGroupID in presUserGroupIDs)
                    {
                        foreach (int radioRoomGroupID in radioRoomGroupIDs)
                        {
                            // Construct Permission details
                            RoomUserGroupPermission roomUserGroupPermission = new RoomUserGroupPermission()
                            {
                                UserGroupID = presUserGroupID,
                                RoomGroupID = radioRoomGroupID
                            };
                            try
                            {
                                // Could be duplicates
                                context.RoomUserGroupPermissions.Add(roomUserGroupPermission);
                                context.SaveChanges();
                            }
                            catch (Exception e)
                            {
                                var m = e.Message;
                                // So skip it and go on to the next
                            }
                        }
                    }
                }

                // Seed many Room Bookings
                if (!context.RoomBookings.Any())
                {
                    // Gets User IDs and Room IDs to seed Bookings with
                    var users = context.Users.Include(u => u.TermAndProgram).ThenInclude(t => t.UserGroup).Select(u => new { u.ID, u.TermAndProgram.UserGroupID, u.TermAndProgram.UserGroup.UserGroupName }).ToArray();
                    var rooms = context.Rooms.Include(r => r.RoomGroup).Where(r => r.Enabled == true && r.RoomGroup.Enabled == true).Select(r => new { r.ID, r.RoomGroupID }).ToArray();
                    int userCount = users.Count();
                    int roomCount = rooms.Count();

                    // Prepare dictionary of Permissions to prevent illegal Bookings from seeding
                    var roomGroupIDs = context.RoomGroups.Select(r => r.ID).ToArray();
                    Dictionary<int, int[]> permissionsDict = new Dictionary<int, int[]>();
                    foreach (int roomGroupID in roomGroupIDs)
                    {
                        int[] userPermissions = context.RoomUserGroupPermissions.Where(p => p.RoomGroupID == roomGroupID).Select(p => p.UserGroupID).ToArray();
                        permissionsDict.Add(roomGroupID, userPermissions);
                    }

                    // Loop through every room
                    for (int i = 0; i < roomCount; i++)
                    {
                        // Initialize the hour of the first booking in every room to 8 o'clock
                        int latestHour = 8;

                        // Keep track of previous user ID so users don't have 2 bookings in a row
                        int lastUserID = 0;

                        // Add 6 bookings to every room
                        for (int j = 0; j < 6; j++)
                        {
                            // Get a valid User
                            int userID;
                            int[] roomPermissions;
                            int index;
                            permissionsDict.TryGetValue(rooms[i].RoomGroupID, out roomPermissions);
                            do
                            {
                                index = random.Next(userCount);
                                userID = users[index].ID;   // Get a random User's ID
                            } while (
                                        (!roomPermissions.Contains(users[index].UserGroupID)) // Loop to another index if permissions does not contain that User's Group
                                     || (userID == lastUserID) // Loop to another index if we are about to make 2 bookings in a row for the same user
                                     || (!users[index].UserGroupName.ToLower().Contains("admin") ? (random.Next(100) > 5) : false)  // Only create bookings for non-admin's 5% of the time, to avoid breaking room time limits
                                     );

                            lastUserID = userID;    // Keep track of previous user ID so users don't have 2 bookings in a row

                            int bookingEndHour = latestHour + random.Next(1, 3);

                            // Construct Room Booking details
                            RoomBooking roomBooking = new RoomBooking()
                            {
                                StartDate = DateTime.Today.AddDays(1).AddHours(latestHour),
                                EndDate = DateTime.Today.AddDays(1).AddHours(bookingEndHour).AddMinutes(-1),
                                RoomID = rooms[i].ID,
                                UserID = userID,
                                ApprovalStatus = "Approved"
                            };
                            try
                            {
                                // Could be duplicates
                                context.RoomBookings.Add(roomBooking);
                                context.SaveChanges();
                            }
                            catch (Exception e)
                            {
                                var m = e.Message;
                                // So skip it and go on to the next
                            }

                            // Update latest hour for next loop
                            latestHour = bookingEndHour;
                        }
                    }

                }



            }
        }

        /// <summary>
        /// This method is for MANDATORY data only. ONLY write data in this method if it is NEEDED to run the application.
        /// </summary>
        public static void SeedMandatoryData(ApplicationDbContext identityContext, IServiceProvider serviceProvider)
        {
            using (var context = new BTRFRoomBookingContext(
                serviceProvider.GetRequiredService<DbContextOptions<BTRFRoomBookingContext>>()))
            {
                // Seed Global Settings
                if (!context.GlobalSettings.Any())
                {
                    // Construct Global Setting details
                    GlobalSetting globalSettings = new GlobalSetting()
                    {
                        StartOfTermDate = Convert.ToDateTime("2022-01-01"),
                        EndOfTermDate = Convert.ToDateTime("2022-04-30"),
                        LatestAllowableFutureBookingDay = 14,
                        EmailBookingNotificationsOverride = false,
                        PreventBookingNotificationsOverride = false,
                        EmailCancelNotificationsOverride = false,
                        PreventCancelNotificationsOverride = false
                    };
                    context.GlobalSettings.Add(globalSettings);
                    context.SaveChanges();
                }

                // Seed User Groups
                if (!context.UserGroups.Any())
                {
                    var userGroups = new List<UserGroup>
                    {
                        new UserGroup { UserGroupName = "top-level admin" },
                        new UserGroup { UserGroupName = "admin" },
                        new UserGroup { UserGroupName = "combo1" },
                        new UserGroup { UserGroupName = "combo2" },
                        new UserGroup { UserGroupName = "tv3" },
                        new UserGroup { UserGroupName = "tv4" },
                        new UserGroup { UserGroupName = "tv5" },
                        new UserGroup { UserGroupName = "tv6" },
                        new UserGroup { UserGroupName = "film3" },
                        new UserGroup { UserGroupName = "film4" },
                        new UserGroup { UserGroupName = "film5" },
                        new UserGroup { UserGroupName = "film6" },
                        new UserGroup { UserGroupName = "pres2" },
                        new UserGroup { UserGroupName = "pres3" },
                        new UserGroup { UserGroupName = "pres4" },
                        new UserGroup { UserGroupName = "pres5" },
                        new UserGroup { UserGroupName = "pres6" },
                        new UserGroup { UserGroupName = "acting" },
                        new UserGroup { UserGroupName = "acting3" },
                        new UserGroup { UserGroupName = "acting4" },
                    };
                    context.UserGroups.AddRange(userGroups);
                    context.SaveChanges();
                }

                // Seed Term and Programs
                if (!context.TermAndPrograms.Any())
                {
                    var termAndPrograms = new List<TermAndProgram>
                    {
                        new TermAndProgram {
                            ProgramName = "Top-level Admin",
                            ProgramCode = "P0000",
                            ProgramLevel = 2,
                            UserGroupID = context.UserGroups.FirstOrDefault(u => u.UserGroupName.ToUpper() == "TOP-LEVEL ADMIN").ID
                        },
                        new TermAndProgram {
                            ProgramName = "Admin",
                            ProgramCode = "P0000",
                            ProgramLevel = 1,
                            UserGroupID = context.UserGroups.FirstOrDefault(u => u.UserGroupName.ToUpper() == "ADMIN").ID
                        },
                        new TermAndProgram {
                            ProgramName = "TV Production",
                            ProgramCode = "P0164",
                            ProgramLevel = 1,
                            UserGroupID = context.UserGroups.FirstOrDefault(u => u.UserGroupName.ToUpper() == "COMBO1").ID
                        },
                        new TermAndProgram {
                            ProgramName = "TV Production",
                            ProgramCode = "P0164",
                            ProgramLevel = 2,
                            UserGroupID = context.UserGroups.FirstOrDefault(u => u.UserGroupName.ToUpper() == "COMBO2").ID
                        },
                        new TermAndProgram {
                            ProgramName = "TV Production",
                            ProgramCode = "P0164",
                            ProgramLevel = 3,
                            UserGroupID = context.UserGroups.FirstOrDefault(u => u.UserGroupName.ToUpper() == "TV3").ID
                        },
                        new TermAndProgram {
                            ProgramName = "TV Production",
                            ProgramCode = "P0164",
                            ProgramLevel = 4,
                            UserGroupID = context.UserGroups.FirstOrDefault(u => u.UserGroupName.ToUpper() == "TV4").ID
                        },
                        new TermAndProgram {
                            ProgramName = "TV Production",
                            ProgramCode = "P0164",
                            ProgramLevel = 5,
                            UserGroupID = context.UserGroups.FirstOrDefault(u => u.UserGroupName.ToUpper() == "TV5").ID
                        },
                        new TermAndProgram {
                            ProgramName = "TV Production",
                            ProgramCode = "P0164",
                            ProgramLevel = 6,
                            UserGroupID = context.UserGroups.FirstOrDefault(u => u.UserGroupName.ToUpper() == "TV6").ID
                        },
                        new TermAndProgram {
                            ProgramName = "Film Production",
                            ProgramCode = "P0165",
                            ProgramLevel = 1,
                            UserGroupID = context.UserGroups.FirstOrDefault(u => u.UserGroupName.ToUpper() == "COMBO1").ID
                        },
                        new TermAndProgram {
                            ProgramName = "Film Production",
                            ProgramCode = "P0165",
                            ProgramLevel = 2,
                            UserGroupID = context.UserGroups.FirstOrDefault(u => u.UserGroupName.ToUpper() == "COMBO2").ID
                        },
                        new TermAndProgram {
                            ProgramName = "Film Production",
                            ProgramCode = "P0165",
                            ProgramLevel = 3,
                            UserGroupID = context.UserGroups.FirstOrDefault(u => u.UserGroupName.ToUpper() == "FILM3").ID
                        },
                        new TermAndProgram {
                            ProgramName = "Film Production",
                            ProgramCode = "P0165",
                            ProgramLevel = 4,
                            UserGroupID = context.UserGroups.FirstOrDefault(u => u.UserGroupName.ToUpper() == "FILM4").ID
                        },
                        new TermAndProgram {
                            ProgramName = "Film Production",
                            ProgramCode = "P0165",
                            ProgramLevel = 5,
                            UserGroupID = context.UserGroups.FirstOrDefault(u => u.UserGroupName.ToUpper() == "FILM5").ID
                        },
                        new TermAndProgram {
                            ProgramName = "Film Production",
                            ProgramCode = "P0165",
                            ProgramLevel = 6,
                            UserGroupID = context.UserGroups.FirstOrDefault(u => u.UserGroupName.ToUpper() == "FILM6").ID
                        },
                        new TermAndProgram {
                            ProgramName = "Broadcasting: Radio, TV & Film",
                            ProgramCode = "P0122",
                            ProgramLevel = 1,
                            UserGroupID = context.UserGroups.FirstOrDefault(u => u.UserGroupName.ToUpper() == "COMBO1").ID
                        },
                        new TermAndProgram {
                            ProgramName = "Presentation / Radio",
                            ProgramCode = "P0163",
                            ProgramLevel = 1,
                            UserGroupID = context.UserGroups.FirstOrDefault(u => u.UserGroupName.ToUpper() == "COMBO1").ID
                        },
                        new TermAndProgram {
                            ProgramName = "Presentation / Radio",
                            ProgramCode = "P0163",
                            ProgramLevel = 2,
                            UserGroupID = context.UserGroups.FirstOrDefault(u => u.UserGroupName.ToUpper() == "PRES2").ID
                        },
                        new TermAndProgram {
                            ProgramName = "Presentation / Radio",
                            ProgramCode = "P0163",
                            ProgramLevel = 3,
                            UserGroupID = context.UserGroups.FirstOrDefault(u => u.UserGroupName.ToUpper() == "PRES3").ID
                        },
                        new TermAndProgram {
                            ProgramName = "Presentation / Radio",
                            ProgramCode = "P0163",
                            ProgramLevel = 4,
                            UserGroupID = context.UserGroups.FirstOrDefault(u => u.UserGroupName.ToUpper() == "PRES4").ID
                        },
                        new TermAndProgram {
                            ProgramName = "Presentation / Radio",
                            ProgramCode = "P0163",
                            ProgramLevel = 5,
                            UserGroupID = context.UserGroups.FirstOrDefault(u => u.UserGroupName.ToUpper() == "PRES5").ID
                        },
                        new TermAndProgram {
                            ProgramName = "Presentation / Radio",
                            ProgramCode = "P0163",
                            ProgramLevel = 6,
                            UserGroupID = context.UserGroups.FirstOrDefault(u => u.UserGroupName.ToUpper() == "PRES6").ID
                        },
                        new TermAndProgram {
                            ProgramName = "Acting for TV & Film",
                            ProgramCode = "P0198",
                            ProgramLevel = 1,
                            UserGroupID = context.UserGroups.FirstOrDefault(u => u.UserGroupName.ToUpper() == "ACTING").ID
                        },
                        new TermAndProgram {
                            ProgramName = "Acting for TV & Film",
                            ProgramCode = "P0198",
                            ProgramLevel = 2,
                            UserGroupID = context.UserGroups.FirstOrDefault(u => u.UserGroupName.ToUpper() == "ACTING").ID
                        },
                        new TermAndProgram {
                            ProgramName = "Digital Photography",
                            ProgramCode = "P0795",
                            ProgramLevel = 1,
                            UserGroupID = context.UserGroups.FirstOrDefault(u => u.UserGroupName.ToUpper() == "COMBO1").ID
                        },
                        new TermAndProgram {
                            ProgramName = "Joint BSc Game Programming",
                            ProgramCode = "P6801",
                            ProgramLevel = 4,
                            UserGroupID = context.UserGroups.FirstOrDefault(u => u.UserGroupName.ToUpper() == "COMBO1").ID
                        },
                        new TermAndProgram {
                            ProgramName = "Joint BA Game Design",
                            ProgramCode = "P6800",
                            ProgramLevel = 6,
                            UserGroupID = context.UserGroups.FirstOrDefault(u => u.UserGroupName.ToUpper() == "COMBO1").ID
                        },
                        new TermAndProgram {
                            ProgramName = "Game Development",
                            ProgramCode = "P0441",
                            ProgramLevel = 3,
                            UserGroupID = context.UserGroups.FirstOrDefault(u => u.UserGroupName.ToUpper() == "COMBO1").ID
                        },
                        new TermAndProgram {
                            ProgramName = "CST - Network and Cloud Tech",
                            ProgramCode = "P0474",
                            ProgramLevel = 3,
                            UserGroupID = context.UserGroups.FirstOrDefault(u => u.UserGroupName.ToUpper() == "COMBO1").ID
                        }
                    };
                    context.TermAndPrograms.AddRange(termAndPrograms);
                    context.SaveChanges();
                }

                // Seed Users
                if (!context.Users.Any())
                {
                    //Create Users
                    var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
                    if (userManager.FindByNameAsync("topadmin").Result == null)
                    {
                        IdentityUser user = new IdentityUser
                        {
                            UserName = "topadmin",
                            Email = "topadmin@niagaracollege.ca"
                        };

                        IdentityResult result = userManager.CreateAsync(user, "Password@1").Result;

                        if (result.Succeeded)
                        {
                            userManager.AddToRoleAsync(user, "Top-level Admin").Wait();

                            // Construct User details
                            User userDetails = new User()
                            {
                                Username = "topadmin",
                                FirstName = "Patrick",
                                LastName = "Topadmin",
                                Email = "topadmin@niagaracollege.ca",
                                EmailBookingNotifications = false,
                                EmailCancelNotifications = false,
                                TermAndProgramID = context.TermAndPrograms.FirstOrDefault(b => b.ProgramName.ToUpper() == "TOP-LEVEL ADMIN").ID
                            };
                            context.Users.Add(userDetails);
                            context.SaveChanges();
                        }
                    }
                    if (userManager.FindByNameAsync("admin").Result == null)
                    {
                        IdentityUser user = new IdentityUser
                        {
                            UserName = "admin",
                            Email = "admin@niagaracollege.ca"
                        };

                        IdentityResult result = userManager.CreateAsync(user, "Password@1").Result;

                        if (result.Succeeded)
                        {
                            userManager.AddToRoleAsync(user, "Admin").Wait();

                            // Construct User details
                            User userDetails = new User()
                            {
                                Username = "admin",
                                FirstName = "Amir",
                                LastName = "Adminbeer",
                                Email = "admin@niagaracollege.ca",
                                EmailBookingNotifications = false,
                                EmailCancelNotifications = false,
                                TermAndProgramID = context.TermAndPrograms.FirstOrDefault(b => b.ProgramName.ToUpper() == "ADMIN").ID
                            };
                            context.Users.Add(userDetails);
                            context.SaveChanges();
                        }
                    }
                    if (userManager.FindByNameAsync("user").Result == null)
                    {
                        IdentityUser user = new IdentityUser
                        {
                            UserName = "user",
                            Email = "user@ncstudents.niagaracollege.ca"
                        };

                        IdentityResult result = userManager.CreateAsync(user, "Password@1").Result;

                        if (result.Succeeded)
                        {
                            userManager.AddToRoleAsync(user, "User").Wait();

                            // Construct User details
                            User userDetails = new User()
                            {
                                Username = "user",
                                FirstName = "Tyler",
                                LastName = "Userguy",
                                Email = "user@ncstudents.niagaracollege.ca",
                                EmailBookingNotifications = false,
                                EmailCancelNotifications = false,
                                TermAndProgramID = context.TermAndPrograms.FirstOrDefault(b => !b.ProgramName.ToUpper().Contains("ADMIN")).ID
                            };
                            context.Users.Add(userDetails);
                            context.SaveChanges();
                        }
                    }
                }
            }
        }
    }
}
