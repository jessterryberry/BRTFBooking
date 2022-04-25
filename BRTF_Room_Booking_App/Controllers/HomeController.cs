using BRTF_Room_Booking_App.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BRTF_Room_Booking_App.Data;
using BRTF_Room_Booking_App.Utilities;
using BRTF_Room_Booking_App.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using Microsoft.AspNetCore.Http.Features;
using System.IO;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace BRTF_Room_Booking_App.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly BTRFRoomBookingContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly IEmailSender _emailSender;

        public HomeController(ILogger<HomeController> logger, BTRFRoomBookingContext context, IEmailSender emailSender)
        {
            _logger = logger;
            _context = context;
            _emailSender = emailSender;
        }

        public bool canApprove(int userID, int roomGroupID)
        {
            var approver = _context.RoomGroupApprovers
                .Where(a => a.UserID == userID && a.RoomGroupID == roomGroupID)
                .FirstOrDefault();

            return approver == null ? false : true;
        }

        public async Task<IActionResult> Index()
        {
            //var roombookings = from r in _context.RoomBookings
            //                   .Include(r => r.Room)
            //                   .Include(r => r.User)
            //                   select r;
            //return View(roombookings);

            if (User.IsInRole("Top-level Admin"))
            {
                var bookings = from r in _context.RoomBookings
                           .Include(r => r.Room)
                           .ThenInclude(r => r.RoomGroup)
                           .ThenInclude(r => r.RoomGroupApprovers)
                           .Include(r => r.User)
                           .Where(r => r.ApprovalStatus == "Pending" && r.Room.RoomGroup.NeedsApproval == true)
                               select r;

                return View(await bookings.ToListAsync());
            }
            else if (User.IsInRole("Admin"))
            {
                User currentUser = await _context.Users
                .Where(u => u.Username == this.HttpContext.User.Identity.Name)
                .FirstOrDefaultAsync();

                var bookings = from r in _context.RoomBookings
                           .Include(r => r.Room)
                           .ThenInclude(r => r.RoomGroup)
                           .ThenInclude(r => r.RoomGroupApprovers)
                           .Include(r => r.User)
                           .Where(r => r.ApprovalStatus == "Pending" && r.Room.RoomGroup.NeedsApproval)
                               select r;

                List<RoomBooking> approverBookings = new List<RoomBooking>();
                foreach (RoomBooking r in bookings)
                {
                    if (canApprove(currentUser.ID, r.Room.RoomGroupID))
                    {
                        approverBookings.Add(r);
                    }
                }

                return View(approverBookings);
            }
            else
            {
                User currentUser = await _context.Users
                .Where(u => u.Username == this.HttpContext.User.Identity.Name)
                .FirstOrDefaultAsync();

                var bookings = from r in _context.RoomBookings
                               .Include(r => r.Room)
                               .ThenInclude(r => r.RoomGroup)
                               .Include(r => r.User)
                               .Where(r => r.User == currentUser && r.StartDate > DateTime.Now)
                               select r;

                return View(await bookings.ToListAsync());
            }
        }

        public async Task<IActionResult> Approve(int? id)
        {
            var roomBookingToUpdate = await _context.RoomBookings
                .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.ID == id);

            var globalSettings = _context.GlobalSettings.FirstOrDefault();

            roomBookingToUpdate.ApprovalStatus = "Approved";

            // Uncomment to test email functionality, but make sure to comment it again before pushing any changes to GitHub
            if (roomBookingToUpdate.User.EmailBookingNotifications || globalSettings.EmailBookingNotificationsOverride)
            {
                if (!globalSettings.PreventBookingNotificationsOverride)
                {
                    await _emailSender.SendEmailAsync(
                           roomBookingToUpdate.User.Email,
                           "Booking Approval",
                           "Your room booking for " + roomBookingToUpdate.Room.RoomName + " on " + roomBookingToUpdate.StartDate.ToShortDateString() + " has been approved.<br /><br />" +
                                   $"<a href='{Request.Scheme}://{Request.Host.Value}'>Log-in to BRTF Room Booking</a> to review your bookings.");
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Deny(int? id)
        {
            var roomBookingToUpdate = await _context.RoomBookings
                .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.ID == id);

            var globalSettings = _context.GlobalSettings.FirstOrDefault();

            roomBookingToUpdate.ApprovalStatus = "Denied";

            // Uncomment to test email functionality, but make sure to comment it again before pushing any changes to GitHub
            if (roomBookingToUpdate.User.EmailBookingNotifications || globalSettings.EmailBookingNotificationsOverride)
            {
                if (!globalSettings.PreventBookingNotificationsOverride)
                {
                    await _emailSender.SendEmailAsync(
                       roomBookingToUpdate.User.Email,
                       "Booking Denied",
                       $"Your room booking for " + roomBookingToUpdate.Room.RoomName + " on " + roomBookingToUpdate.StartDate.ToShortDateString() + " has been denied.<br /><br />" +
                                   $"<a href='{Request.Scheme}://{Request.Host.Value}'>Log-in to BRTF Room Booking</a> to review your bookings.");
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Help()
        {
            return View();
        }

        public IActionResult Reports()
        {
            return View();
        }

        public async Task<IActionResult> UsersBookings(int? page, int? pageSizeID, /* Paging */
            string SearchAfterDate, string SearchBeforeDate, string sortDirectionCheck, string sortFieldID, /* Filters/Search */
            string actionButton, string sortDirection = "asc", string sortField = "Start Date" /*Sorting*/)
        {
            //Clear the sort/filter/paging URL Cookie for Controller
            CookieHelper.CookieSet(HttpContext, ControllerName() + "URL", "", -1);

            //Toggle the background colour of the filter button depending on if something is being filtered
            ViewData["Filtering"] = "btn-outline-secondary"; //Assume nothing is filtered

            //NOTE: make sure this array has matching values to the column headings being sorted
            string[] sortOptions = new[] { "Start Date", "Approval Status" };

            // Start with Includes but make sure your expression returns an
            // IQueryable<> so we can add filter and sort 
            // options later.
            User currentUser = await _context.Users
                .Where(u => u.Username == this.HttpContext.User.Identity.Name)
                .FirstOrDefaultAsync();

            var bookings = from r in _context.RoomBookings
                           .Include(r => r.Room)
                           .ThenInclude(r => r.RoomGroup)
                           .Include(r => r.User)
                           .Where(r => r.User == currentUser && r.StartDate > DateTime.Now)
                           select r;

            //bool filtered = false;

            //Add as many filters as needed
            if (!String.IsNullOrEmpty(SearchAfterDate) && DateTime.TryParse(SearchAfterDate, out DateTime afterDate))
            {
                bookings = bookings.Where(r => afterDate <= r.StartDate);

                ViewData["Filtering"] = "btn-danger";
            }
            if (!String.IsNullOrEmpty(SearchBeforeDate) && DateTime.TryParse(SearchBeforeDate, out DateTime beforeDate))
            {
                bookings = bookings.Where(r => r.StartDate <= beforeDate);
                ViewData["Filtering"] = "btn-danger";
            }

            //Before we sort, see if we have called for a change of filtering or sorting
            if (!String.IsNullOrEmpty(actionButton)) //Form Submitted so lets sort!
            {
                if (sortOptions.Contains(actionButton))//Change of sort is requested
                {
                    if (actionButton == sortField) //Reverse order on same field
                    {
                        sortDirection = sortDirection == "asc" ? "desc" : "asc";
                    }
                    sortField = actionButton;//Sort by the button clicked
                }
                else //Sort by the controls in the filter area
                {
                    sortDirection = String.IsNullOrEmpty(sortDirectionCheck) ? "asc" : "desc";
                    sortField = sortFieldID;
                }
            }
            //Now we know which field and direction to sort by
            if (sortField == "Start Date")
            {
                if (sortDirection == "asc")
                {
                    bookings = bookings
                        .OrderBy(p => p.StartDate);
                }
                else
                {
                    bookings = bookings
                        .OrderByDescending(p => p.StartDate);
                }
            }
            else //Sorting by Approval Status
            {
                if (sortDirection == "asc")
                {
                    bookings = bookings
                        .OrderBy(p => p.ApprovalStatus);
                }
                else
                {
                    bookings = bookings
                        .OrderByDescending(p => p.ApprovalStatus);
                }
            }

            //Set sort for next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            //SelectList for Sorting Options
            ViewBag.sortFieldID = new SelectList(sortOptions, sortField.ToString());

            //Handle Paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);

            var pagedData = await PaginatedList<RoomBooking>.CreateAsync(bookings.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);

            //return View(await bookings.ToListAsync());
        }

        public async Task<IActionResult> DownloadUsersBookings(string SearchAfterDate, string SearchBeforeDate)
        {
            User currentUser = await _context.Users
                .Where(u => u.Username == this.HttpContext.User.Identity.Name)
                .FirstOrDefaultAsync();

            var _bookings = from r in _context.RoomBookings
                           .Include(r => r.Room)
                           .ThenInclude(r => r.RoomGroup)
                           .Include(r => r.User)
                           .Where(r => r.User == currentUser && r.StartDate > DateTime.Now)
                           select r;

            //bool filtered = false;

            //Add as many filters as needed
            if (!String.IsNullOrEmpty(SearchAfterDate) && DateTime.TryParse(SearchAfterDate, out DateTime afterDate))
            {
                _bookings = _bookings.Where(r => afterDate <= r.StartDate);
            }
            if (!String.IsNullOrEmpty(SearchBeforeDate) && DateTime.TryParse(SearchBeforeDate, out DateTime beforeDate))
            {
                _bookings = _bookings.Where(r => r.StartDate <= beforeDate);
            }

            List<BookingVM> bookings = new List<BookingVM>();

            foreach (RoomBooking b in _bookings){
                bookings.Add(new BookingVM {
                        Area = b.Room.RoomGroup.AreaName,
                        Room = b.Room.RoomName,
                        Start = b.StartDate.ToString("g"),
                        End = b.EndDate.ToString("g"),
                        Status = b.ApprovalStatus
                    }
                );
            }

            //How many rows?
            int numRows = bookings.Count();

            if (numRows > 0) //We have data
            {
                //Create a new spreadsheet from scratch.
                using (ExcelPackage excel = new ExcelPackage())
                {

                    var workSheet = excel.Workbook.Worksheets.Add("YourBookings");

                    //Note: Cells[row, column]
                    workSheet.Cells[3, 1].LoadFromCollection(bookings, true);

                    //Set Style and backgound colour of headings
                    using (ExcelRange headings = workSheet.Cells[3, 1, 3, 5])
                    {
                        headings.Style.Font.Bold = true;
                        var fill = headings.Style.Fill;
                        fill.PatternType = ExcelFillStyle.Solid;
                        fill.BackgroundColor.SetColor(Color.LightBlue);
                    }

                    //Autofit columns
                    workSheet.Cells.AutoFitColumns();

                    //Add a title and timestamp at the top of the report
                    workSheet.Cells[1, 1].Value = "Your Bookings";
                    using (ExcelRange Rng = workSheet.Cells[1, 1, 1, 5])
                    {
                        Rng.Merge = true; //Merge columns start and end range
                        Rng.Style.Font.Bold = true; //Font should be bold
                        Rng.Style.Font.Size = 18;
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }
                    //Since the time zone where the server is running can be different, adjust to 
                    //Local for us.
                    DateTime utcDate = DateTime.UtcNow;
                    TimeZoneInfo esTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                    DateTime localDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, esTimeZone);
                    using (ExcelRange Rng = workSheet.Cells[2, 9])
                    {
                        Rng.Value = "Created: " + localDate.ToShortTimeString() + " on " +
                            localDate.ToShortDateString();
                        Rng.Style.Font.Bold = true; //Font should be bold
                        Rng.Style.Font.Size = 12;
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }

                    //Ok, time to download the Excel

                    //I usually stream the response back to avoid possible
                    //out of memory errors on the server if you have a large spreadsheet.
                    //NOTE: Since .NET Core 3 most Web Servers disallow sync IO so we
                    //need to temporarily change the setting for the server.
                    //If we can't then we will try to build the file and return a FileContentResult
                    var syncIOFeature = HttpContext.Features.Get<IHttpBodyControlFeature>();
                    if (syncIOFeature != null)
                    {
                        syncIOFeature.AllowSynchronousIO = true;
                        using (var memoryStream = new MemoryStream())
                        {
                            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                            Response.Headers["content-disposition"] = "attachment;  filename=YourBookings.xlsx";
                            excel.SaveAs(memoryStream);
                            memoryStream.WriteTo(Response.Body);
                        }
                    }
                    else
                    {
                        try
                        {
                            Byte[] theData = excel.GetAsByteArray();
                            string filename = "YourBookings.xlsx";
                            string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                            return File(theData, mimeType, filename);
                        }
                        catch (Exception)
                        {
                            return NotFound();
                        }
                    }
                }
            }
            return NotFound();
        }


        private string ControllerName()
        {
            return this.ControllerContext.RouteData.Values["controller"].ToString();
        }
        private void ViewDataReturnURL()
        {
            ViewData["returnURL"] = MaintainURL.ReturnURL(HttpContext, ControllerName());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
