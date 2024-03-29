﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BRTF_Room_Booking_App.Data.BTMigrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Areas",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AreaName = table.Column<string>(maxLength: 100, nullable: false),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    BlackoutTime = table.Column<int>(nullable: false),
                    MaxHoursPerSingleBooking = table.Column<int>(nullable: true),
                    MaxHoursTotal = table.Column<int>(nullable: true),
                    MaxNumberOfBookings = table.Column<int>(nullable: true),
                    TimeOfDayRestrictions = table.Column<bool>(nullable: false),
                    EarliestTime = table.Column<DateTime>(nullable: false),
                    LatestTime = table.Column<DateTime>(nullable: false),
                    Enabled = table.Column<bool>(nullable: false),
                    NeedsApproval = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Areas", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "GlobalSettings",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StartOfTermDate = table.Column<DateTime>(nullable: false),
                    EndOfTermDate = table.Column<DateTime>(nullable: false),
                    LatestAllowableFutureBookingDay = table.Column<int>(nullable: false),
                    EmailBookingNotificationsOverride = table.Column<bool>(nullable: false),
                    PreventBookingNotificationsOverride = table.Column<bool>(nullable: false),
                    EmailCancelNotificationsOverride = table.Column<bool>(nullable: false),
                    PreventCancelNotificationsOverride = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalSettings", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TermAndPrograms",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProgramName = table.Column<string>(maxLength: 50, nullable: false),
                    ProgramCode = table.Column<string>(maxLength: 5, nullable: false),
                    ProgramLevel = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TermAndPrograms", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "UserGroups",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserGroupName = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroups", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoomName = table.Column<string>(maxLength: 50, nullable: false),
                    RoomMaxHoursTotal = table.Column<int>(nullable: true),
                    Enabled = table.Column<bool>(nullable: false),
                    AreaID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Rooms_Areas_AreaID",
                        column: x => x.AreaID,
                        principalTable: "Areas",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(maxLength: 50, nullable: false),
                    FirstName = table.Column<string>(maxLength: 50, nullable: false),
                    MiddleName = table.Column<string>(maxLength: 50, nullable: true),
                    LastName = table.Column<string>(maxLength: 100, nullable: false),
                    Email = table.Column<string>(maxLength: 200, nullable: false),
                    EmailBookingNotifications = table.Column<bool>(nullable: false),
                    EmailCancelNotifications = table.Column<bool>(nullable: false),
                    TermAndProgramID = table.Column<int>(nullable: false),
                    TimeFormat24Hours = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Users_TermAndPrograms_TermAndProgramID",
                        column: x => x.TermAndProgramID,
                        principalTable: "TermAndPrograms",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoomUserGroupPermissions",
                columns: table => new
                {
                    UserGroupID = table.Column<int>(nullable: false),
                    AreaID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomUserGroupPermissions", x => new { x.UserGroupID, x.AreaID });
                    table.ForeignKey(
                        name: "FK_RoomUserGroupPermissions_Areas_AreaID",
                        column: x => x.AreaID,
                        principalTable: "Areas",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoomUserGroupPermissions_UserGroups_UserGroupID",
                        column: x => x.UserGroupID,
                        principalTable: "UserGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserGroupTermAndPrograms",
                columns: table => new
                {
                    UserGroupID = table.Column<int>(nullable: false),
                    TermAndProgramID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroupTermAndPrograms", x => new { x.TermAndProgramID, x.UserGroupID });
                    table.ForeignKey(
                        name: "FK_UserGroupTermAndPrograms_TermAndPrograms_TermAndProgramID",
                        column: x => x.TermAndProgramID,
                        principalTable: "TermAndPrograms",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserGroupTermAndPrograms_UserGroups_UserGroupID",
                        column: x => x.UserGroupID,
                        principalTable: "UserGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AreaApprovers",
                columns: table => new
                {
                    UserID = table.Column<int>(nullable: false),
                    AreaID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AreaApprovers", x => new { x.AreaID, x.UserID });
                    table.ForeignKey(
                        name: "FK_AreaApprovers_Areas_AreaID",
                        column: x => x.AreaID,
                        principalTable: "Areas",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AreaApprovers_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoomBookings",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SpecialNotes = table.Column<string>(maxLength: 1000, nullable: true),
                    ApprovalStatus = table.Column<string>(maxLength: 50, nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    RoomID = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomBookings", x => x.ID);
                    table.ForeignKey(
                        name: "FK_RoomBookings_Rooms_RoomID",
                        column: x => x.RoomID,
                        principalTable: "Rooms",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoomBookings_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AreaApprovers_UserID",
                table: "AreaApprovers",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Areas_AreaName",
                table: "Areas",
                column: "AreaName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoomBookings_RoomID",
                table: "RoomBookings",
                column: "RoomID");

            migrationBuilder.CreateIndex(
                name: "IX_RoomBookings_UserID",
                table: "RoomBookings",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_AreaID_RoomName",
                table: "Rooms",
                columns: new[] { "AreaID", "RoomName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoomUserGroupPermissions_AreaID",
                table: "RoomUserGroupPermissions",
                column: "AreaID");

            migrationBuilder.CreateIndex(
                name: "IX_TermAndPrograms_ProgramLevel_ProgramCode",
                table: "TermAndPrograms",
                columns: new[] { "ProgramLevel", "ProgramCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserGroups_UserGroupName",
                table: "UserGroups",
                column: "UserGroupName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserGroupTermAndPrograms_TermAndProgramID",
                table: "UserGroupTermAndPrograms",
                column: "TermAndProgramID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserGroupTermAndPrograms_UserGroupID",
                table: "UserGroupTermAndPrograms",
                column: "UserGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_TermAndProgramID",
                table: "Users",
                column: "TermAndProgramID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AreaApprovers");

            migrationBuilder.DropTable(
                name: "GlobalSettings");

            migrationBuilder.DropTable(
                name: "RoomBookings");

            migrationBuilder.DropTable(
                name: "RoomUserGroupPermissions");

            migrationBuilder.DropTable(
                name: "UserGroupTermAndPrograms");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "UserGroups");

            migrationBuilder.DropTable(
                name: "Areas");

            migrationBuilder.DropTable(
                name: "TermAndPrograms");
        }
    }
}
