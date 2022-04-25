using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BRTF_Room_Booking_App.Data
{
    public static class ExtraMigration
    {
        public static void Steps(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
            @" 
                Create View BookingSummaries as 
                Select r.ID, r.RoomName, Count(*) as NumberOfAppointments,Sum(Cast((
                (JulianDay(b.EndDate)-JulianDay(b.StartDate)) * 24 )+0.9 as int)) as TotalHours
                From Rooms r Join RoomBookings b
                on r.ID = b.RoomID
                Group By r.ID,r.RoomName
                
             ");

        }
    }
}
