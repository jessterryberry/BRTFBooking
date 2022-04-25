using BRTF_Room_Booking_App.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BRTF_Room_Booking_App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var identityContext = services.GetRequiredService<ApplicationDbContext>();
                    identityContext.Database.Migrate();
                    ApplicationSeedData.SeedMandatoryData(identityContext, services).Wait();  // Seeds necessary data for ApplicationDbContext. Always seed this data the first time you create the database

                    var context = services.GetRequiredService<BTRFRoomBookingContext>();
                    context.Database.Migrate();
                    BTSeedData.SeedMandatoryData(identityContext, services);  // Seeds necessary data for BTRFRoomBookingContext. Always seed this data the first time you create the database
                    BTSeedData.SeedTestData(services);  // This data is for testing-only. Do not seed this data in the final production
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred seeding the DB.");
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
