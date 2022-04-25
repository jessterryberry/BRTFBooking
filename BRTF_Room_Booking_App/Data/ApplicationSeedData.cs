using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BRTF_Room_Booking_App.Data
{
    public static class ApplicationSeedData
    {
        public static async Task SeedMandatoryData(ApplicationDbContext context, IServiceProvider serviceProvider)
        {
            //Seed Roles
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roleNames = { "Top-level Admin", "Admin", "User" };
            IdentityResult roleResult;
            foreach (var roleName in roleNames)
            {
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
            // DO NOT SEED USERS HERE. SEED IN BTSeedData SeedMandatoryData() FUNCTION
        }
    }
}
