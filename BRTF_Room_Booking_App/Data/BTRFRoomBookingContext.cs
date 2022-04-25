using BRTF_Room_Booking_App.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BRTF_Room_Booking_App.ViewModels;

namespace BRTF_Room_Booking_App.Data
{
    public class BTRFRoomBookingContext : DbContext
    {
        //To give access to IHttpContextAccessor for Audit Data with IAuditable
        private readonly IHttpContextAccessor _httpContextAccessor;

        //Property to hold the UserName value
        public string UserName
        {
            get; private set;
        }

        public BTRFRoomBookingContext(DbContextOptions<BTRFRoomBookingContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            UserName = _httpContextAccessor.HttpContext?.User.Identity.Name;
            UserName ??= "Unknown";
        }

        public BTRFRoomBookingContext(DbContextOptions<BTRFRoomBookingContext> options)
            : base(options)
        {
            UserName = "SeedData";
        }

        public DbSet<GlobalSetting> GlobalSettings { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomBooking> RoomBookings { get; set; }
        public DbSet<RoomGroup> RoomGroups { get; set; }
        public DbSet<RoomUserGroupPermission> RoomUserGroupPermissions { get; set; }
        public DbSet<TermAndProgram> TermAndPrograms { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<BookingSummary> BookingSummaries { get; set; }
        public DbSet<RoomGroupApprover> RoomGroupApprovers { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.HasDefaultSchema("BT");

            //For Booking Summary View Model
            modelBuilder
                .Entity<BookingSummary>()
                .ToView(nameof(BookingSummaries))
                .HasKey(a => a.ID);

            //Add a composite primary key to RoomUserGroupPermission
            modelBuilder.Entity<RoomUserGroupPermission>()
                .HasKey(p => new { p.UserGroupID, p.RoomGroupID });

            //Prevent Cascade Delete from TermAndProgram to User
            //so we are prevented from deleting a TermAndProgram with
            //Users assigned
            modelBuilder.Entity<TermAndProgram>()
                .HasMany<User>(d => d.Users)
                .WithOne(p => p.TermAndProgram)
                .HasForeignKey(p => p.TermAndProgramID)
                .OnDelete(DeleteBehavior.Restrict);

            //Prevent Cascade Delete from UserGroup to TermAndProgram
            //so we are prevented from deleting a UserGroup with
            //TermAndPrograms assigned
            modelBuilder.Entity<UserGroup>()
                .HasMany<TermAndProgram>(d => d.TermAndPrograms)
                .WithOne(p => p.UserGroup)
                .HasForeignKey(p => p.UserGroupID)
                .OnDelete(DeleteBehavior.Restrict);

            //Prevent Cascade Delete from RoomGroup to RoomUserGroupPermissions
            //so we are prevented from deleting a RoomGroup with
            //RoomUserGroupPermissions assigned
            modelBuilder.Entity<RoomGroup>()
                .HasMany<RoomUserGroupPermission>(d => d.RoomUserGroupPermissions)
                .WithOne(p => p.RoomGroup)
                .HasForeignKey(p => p.RoomGroupID)
                .OnDelete(DeleteBehavior.Restrict);

            //Prevent Cascade Delete from RoomGroup to Room
            //so we are prevented from deleting a RoomGroup with
            //Rooms assigned
            modelBuilder.Entity<RoomGroup>()
                .HasMany<Room>(d => d.Rooms)
                .WithOne(p => p.RoomGroup)
                .HasForeignKey(p => p.RoomGroupID)
                .OnDelete(DeleteBehavior.Restrict);

            //Prevent Cascade Delete from Room to RoomBooking
            //so we are prevented from deleting a Room with
            //RoomBookings assigned
            modelBuilder.Entity<Room>()
                .HasMany<RoomBooking>(d => d.RoomBookings)
                .WithOne(p => p.Room)
                .HasForeignKey(p => p.RoomID)
                .OnDelete(DeleteBehavior.Restrict);

            //Add a unique index to the UserGroup Name
            modelBuilder.Entity<UserGroup>()
                .HasIndex(p => p.UserGroupName)
                .IsUnique();

            //Add a unique index to the combined Term and Program Code
            modelBuilder.Entity<TermAndProgram>()
                .HasIndex(p => new { p.ProgramLevel, p.ProgramCode })
                .IsUnique();

            //Add a unique index to the Username
            modelBuilder.Entity<User>()
               .HasIndex(p => p.Username )
               .IsUnique();
            //Add a unique index to the Email
            modelBuilder.Entity<User>()
               .HasIndex(p => p.Email )
               .IsUnique();

            //Add a unique index to the RoomGroup Name
            modelBuilder.Entity<RoomGroup>()
                .HasIndex(p => p.AreaName)
                .IsUnique();

            //Add a unique index to the combined RoomGroupID and Room Name
            modelBuilder.Entity<Room>()
                .HasIndex(p => new { p.RoomGroupID, p.RoomName })
                .IsUnique();

            modelBuilder.Entity<RoomGroupApprover>()
                .HasKey(p => new { p.RoomGroupID, p.UserID });

            modelBuilder.Entity<RoomGroup>()
                .HasMany(p => p.RoomGroupApprovers)
                .WithOne(p => p.RoomGroup)
                .HasForeignKey(p => p.RoomGroupID);

            //modelBuilder.Entity<RoomGroupApprover>()
            //    .HasOne<RoomGroup>(p => p.RoomGroup)
            //    .WithMany(p => p.RoomGroupApprovers)
            //    .HasForeignKey(p => p.RoomGroupID);

            //modelBuilder.Entity<RoomGroupApprover>()
            //    .HasOne<User>(p => p.User)
            //    .WithMany(p => p.RoomGroupApprovers)
            //    .HasForeignKey(p => p.UserID);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void OnBeforeSaving()
        {
            var entries = ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                if (entry.Entity is IAuditable trackable)
                {
                    var now = DateTime.UtcNow;
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            trackable.UpdatedOn = now;
                            trackable.UpdatedBy = UserName;
                            break;

                        case EntityState.Added:
                            trackable.CreatedOn = now;
                            trackable.CreatedBy = UserName;
                            trackable.UpdatedOn = now;
                            trackable.UpdatedBy = UserName;
                            break;
                    }
                }
            }
        }

        public DbSet<BRTF_Room_Booking_App.ViewModels.BookingSummary> BookingSummary { get; set; }
    }
}
