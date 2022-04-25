using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BRTF_Room_Booking_App.Data;
using BRTF_Room_Booking_App.Models;
using BRTF_Room_Booking_App.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using BRTF_Room_Booking_App.ViewModels;

namespace BRTF_Room_Booking_App.Controllers
{
    [Authorize(Roles = "Top-level Admin, Admin")]
    public class RoomGroupsController : Controller
    {
        private readonly BTRFRoomBookingContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public RoomGroupsController(BTRFRoomBookingContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: RoomGroups
        public async Task<IActionResult> Index(string SearchName, string sortFieldID, string sortDirectionCheck, int? page, int? pageSizeID,
            string actionButton, string sortDirection = "asc", string sortField = "Is Enabled", string EnabledFilterString = "All")
        {
            //Clear the sort/filter/paging URL Cookie for Controller
            CookieHelper.CookieSet(HttpContext, ControllerName() + "URL", "", -1);

            ViewData["Filtering"] = "btn-outline-secondary";

            //array for string options
            string[] sortOptions = new[] { "Area", "Is Enabled", "Needs Approval" };

            PopulateDropDownLists(); //data for User Filter DDL

            var roomGroups = from u in _context.RoomGroups.Where(u =>
            (u.Enabled == true && EnabledFilterString == "True") || (u.Enabled == false && EnabledFilterString == "False") || (EnabledFilterString == "All"))
                             select u;

            //  Filtering
            if (EnabledFilterString != "All")
            {
                ViewData["Filtering"] = "btn-danger";
            }
            if (!String.IsNullOrEmpty(SearchName))
            {
                roomGroups = roomGroups.Where(r => r.AreaName.ToUpper().Contains(SearchName.ToUpper()));
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

            if (sortField == "Area") //Sorting by Area
            {
                if (sortDirection == "asc")
                {
                    roomGroups = roomGroups
                        .OrderBy(r => r.AreaName);
                }
                else
                {
                    roomGroups = roomGroups
                        .OrderByDescending(r => r.AreaName);
                }
            }
            else if (sortField == "Needs Approval") //Sorting by Area
            {
                if (sortDirection == "asc")
                {
                    roomGroups = roomGroups
                        .OrderBy(r => r.NeedsApproval);
                }
                else
                {
                    roomGroups = roomGroups
                        .OrderByDescending(r => r.NeedsApproval);
                }
            }
            else //Sorting by Enabled
            {
                if (sortDirection == "asc")
                {
                    roomGroups = roomGroups
                        .OrderByDescending(r => r.Enabled);
                }
                else
                {
                    roomGroups = roomGroups
                        .OrderBy(r => r.Enabled);
                }
            }

            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            ViewBag.sortFieldID = new SelectList(sortOptions, sortField.ToString());

            //Handle Paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);

            var pagedData = await PaginatedList<RoomGroup>.CreateAsync(roomGroups.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }


        // GET: RoomGroups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            if (id == null)
            {
                return NotFound();
            }

            var roomGroup = await _context.RoomGroups
                .Include(r => r.RoomGroupApprovers)
                .ThenInclude(r => r.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (roomGroup == null)
            {
                return NotFound();
            }

            return View(roomGroup);
        }

        // GET: RoomGroups/Create
        [Authorize(Roles = "Top-level Admin")]
        public IActionResult Create()
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            RoomGroup roomGroup = new RoomGroup();
            PopulateAdminList(roomGroup);
            return View(roomGroup);
        }

        // POST: RoomGroups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Top-level Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,AreaName,Description,BlackoutTime,MaxHoursPerSingleBooking,MaxHoursTotal,MaxNumberOfBookings,EarliestTime,LatestTime,Enabled,NeedsApproval")] RoomGroup roomGroup, string[] selectedOptions)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            try
            {
                if (ModelState.IsValid)
                {
                    if (selectedOptions.Count() > 0)
                    {
                        foreach (string s in selectedOptions)
                        {
                            User approver = _context.Users.Where(u => u.ID == Int32.Parse(s)).FirstOrDefault();
                            roomGroup.RoomGroupApprovers.Add(new RoomGroupApprover
                            {
                                UserID = approver.ID,
                                RoomGroupID = roomGroup.ID
                            });
                        }
                    }
                    _context.Add(roomGroup);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Room Group created successfully!";
                    return RedirectToAction("Index");
                }
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: RoomGroups.AreaName"))
                {
                    ModelState.AddModelError("AreaName", "Unable to save changes. A Room Group with this Name already exists.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateAdminList(roomGroup);
            return View(roomGroup);
        }

        // GET: RoomGroups/Edit/5
        [Authorize(Roles = "Top-level Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            if (id == null)
            {
                return NotFound();
            }

            var roomGroup = await _context.RoomGroups
                .Include(r => r.RoomGroupApprovers)
                .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(r => r.ID == id);

            if (roomGroup == null)
            {
                return NotFound();
            }
            
            ViewData["collapseApprovers"] = roomGroup.NeedsApproval ? " show " : "";
            ViewData["collapseTimeRestrictions"] = roomGroup.TimeOfDayRestrictions ? " show " : "";

            PopulateAdminList(roomGroup);
            return View(roomGroup);
        }

        // POST: RoomGroups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Top-level Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string[] selectedOptions)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            var roomGroupToUpdate = await _context.RoomGroups
                .Include(r => r.RoomGroupApprovers)
                .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(r => r.ID == id);

            if (roomGroupToUpdate == null)
            {
                return NotFound();
            }

            UpdateRoomGroupApprovers(selectedOptions, roomGroupToUpdate);

            if (await TryUpdateModelAsync<RoomGroup>(roomGroupToUpdate, "",
                r => r.AreaName, r => r.Description, r => r.BlackoutTime,
                r => r.MaxHoursPerSingleBooking, r => r.MaxHoursTotal,
                r => r.MaxNumberOfBookings, r => r.TimeOfDayRestrictions, r => r.EarliestTime, r => r.LatestTime,
                r => r.Enabled, r => r.NeedsApproval))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Room Group edited successfully!";
                    return RedirectToAction("Details", new { roomGroupToUpdate.ID });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomGroupExists(roomGroupToUpdate.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException dex)
                {
                    if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: RoomGroups.AreaName"))
                    {
                        ModelState.AddModelError("AreaName", "Unable to save changes. An area with this Name already exists.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                    }
                }
            }
            PopulateAdminList(roomGroupToUpdate);
            return View(roomGroupToUpdate);

        }

        // GET: RoomGroups/Delete/5
        [Authorize(Roles = "Top-level Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            if (id == null)
            {
                return NotFound();
            }

            var roomGroup = await _context.RoomGroups
                .Include(r => r.RoomGroupApprovers)
                .ThenInclude(r => r.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (roomGroup == null)
            {
                return NotFound();
            }

            return View(roomGroup);
        }

        // POST: RoomGroups/Delete/5
        [Authorize(Roles = "Top-level Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            var roomGroup = await _context.RoomGroups.FindAsync(id);
            try
            {
                _context.RoomGroups.Remove(roomGroup);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Room Group deleted successfully!";
                return Redirect(ViewData["returnURL"].ToString());
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("FOREIGN KEY constraint failed"))
                {
                    ModelState.AddModelError("", "Unable to delete. You cannot delete a Room Group that has Rooms assigned.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to delete. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(roomGroup);
        }
        private SelectList EnabledSelectList(int? selectedId)
        {
            return new SelectList(_context.RoomGroups
                .OrderBy(u => u.Enabled), "ID", "Enabled", selectedId);
        }

        private void PopulateDropDownLists(RoomGroup roomGroup = null)
        {
            ViewData["Enabled"] = EnabledSelectList(roomGroup?.ID);
        }

        private void PopulateAdminList(RoomGroup roomGroup)
        {
            var allUsers = _context.Users;
            var allAdmins = new HashSet<User>();
            var currentOptionsHS = new HashSet<int>(roomGroup.RoomGroupApprovers.Select(b => b.UserID));

            var selected = new List<ListOptionVM>();
            var available = new List<ListOptionVM>();

            // Get all Admins
            foreach (User u in allUsers)
            {
                var identityUser = _userManager.FindByNameAsync(u.Username).Result;
                var roles = _userManager.GetRolesAsync(identityUser).Result;
                if (roles.Contains("Admin"))
                {
                    allAdmins.Add(u);
                }
            }

            foreach (var s in allAdmins)
            {
                if (currentOptionsHS.Contains(s.ID))
                {
                    selected.Add(new ListOptionVM
                    {
                        ID = s.ID,
                        DisplayText = s.FullName
                    });
                }
                else
                {
                    available.Add(new ListOptionVM
                    {
                        ID = s.ID,
                        DisplayText = s.FullName
                    });
                }
            }

            ViewData["selOpts"] = new MultiSelectList(selected.OrderBy(s => s.DisplayText), "ID", "DisplayText");
            ViewData["availOpts"] = new MultiSelectList(available.OrderBy(s => s.DisplayText), "ID", "DisplayText");
        }

        //Update the Room Group's assigned Approvers
        private void UpdateRoomGroupApprovers(string[] selectedOptions, RoomGroup roomGroupToUpdate)
        {
            if (selectedOptions == null)
            {
                roomGroupToUpdate.RoomGroupApprovers = new List<RoomGroupApprover>();
                return;
            }

            var selectedOptionsHS = new HashSet<string>(selectedOptions);
            var currentOptionsHS = new HashSet<int>(roomGroupToUpdate.RoomGroupApprovers.Select(b => b.UserID));

            foreach (var u in _context.Users)
            {
                if (selectedOptionsHS.Contains(u.ID.ToString())) //if selected
                {
                    if (!currentOptionsHS.Contains(u.ID)) //add if not in the Usergroup's collection
                    {
                        roomGroupToUpdate.RoomGroupApprovers.Add(new RoomGroupApprover
                        {
                            UserID = u.ID,
                            RoomGroupID = roomGroupToUpdate.ID
                        });
                    }
                }
                else //not selected
                {
                    if (currentOptionsHS.Contains(u.ID))//remove if currently in the UserGroup's collection
                    {
                        RoomGroupApprover approverToRemove = roomGroupToUpdate.RoomGroupApprovers.FirstOrDefault(d => d.UserID == u.ID);
                        _context.Remove(approverToRemove);
                    }
                }
            }
        }

        private string ControllerName()
        {
            return this.ControllerContext.RouteData.Values["controller"].ToString();
        }

        private void ViewDataReturnURL()
        {
            ViewData["returnURL"] = MaintainURL.ReturnURL(HttpContext, ControllerName());
        }

        private bool RoomGroupExists(int id)
        {
            return _context.RoomGroups.Any(e => e.ID == id);
        }
    }
}
