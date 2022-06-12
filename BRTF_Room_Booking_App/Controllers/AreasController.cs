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
    public class AreasController : Controller
    {
        private readonly BTRFRoomBookingContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AreasController(BTRFRoomBookingContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: Areas
        public async Task<IActionResult> Index(string SearchName, string sortFieldID, string sortDirectionCheck, int? page, int? pageSizeID,
            string actionButton, string sortDirection = "asc", string sortField = "Is Enabled", string EnabledFilterString = "All")
        {
            //Clear the sort/filter/paging URL Cookie for Controller
            CookieHelper.CookieSet(HttpContext, ControllerName() + "URL", "", -1);

            ViewData["Filtering"] = "btn-outline-secondary";

            //array for string options
            string[] sortOptions = new[] { "Area", "Is Enabled", "Needs Approval" };

            PopulateDropDownLists(); //data for User Filter DDL

            var areas = from u in _context.Areas.Where(u =>
            (u.Enabled == true && EnabledFilterString == "True") || (u.Enabled == false && EnabledFilterString == "False") || (EnabledFilterString == "All"))
                             select u;

            //  Filtering
            if (EnabledFilterString != "All")
            {
                ViewData["Filtering"] = "btn-danger";
            }
            if (!String.IsNullOrEmpty(SearchName))
            {
                areas = areas.Where(r => r.AreaName.ToUpper().Contains(SearchName.ToUpper()));
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
                    areas = areas
                        .OrderBy(r => r.AreaName);
                }
                else
                {
                    areas = areas
                        .OrderByDescending(r => r.AreaName);
                }
            }
            else if (sortField == "Needs Approval") //Sorting by Area
            {
                if (sortDirection == "asc")
                {
                    areas = areas
                        .OrderBy(r => r.NeedsApproval);
                }
                else
                {
                    areas = areas
                        .OrderByDescending(r => r.NeedsApproval);
                }
            }
            else //Sorting by Enabled
            {
                if (sortDirection == "asc")
                {
                    areas = areas
                        .OrderByDescending(r => r.Enabled);
                }
                else
                {
                    areas = areas
                        .OrderBy(r => r.Enabled);
                }
            }

            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            ViewBag.sortFieldID = new SelectList(sortOptions, sortField.ToString());

            //Handle Paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);

            var pagedData = await PaginatedList<Area>.CreateAsync(areas.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }


        // GET: Areas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            if (id == null)
            {
                return NotFound();
            }

            var area = await _context.Areas
                .Include(r => r.AreaApprovers)
                .ThenInclude(r => r.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (area == null)
            {
                return NotFound();
            }

            return View(area);
        }

        // GET: Areas/Create
        [Authorize(Roles = "Top-level Admin")]
        public IActionResult Create()
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            Area area = new Area();
            PopulateAdminList(area);
            return View(area);
        }

        // POST: Areas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Top-level Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,AreaName,Description,BlackoutTime,MaxHoursPerSingleBooking,MaxHoursTotal,MaxNumberOfBookings,EarliestTime,LatestTime,Enabled,NeedsApproval")] Area area, string[] selectedOptions)
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
                            area.AreaApprovers.Add(new AreaApprover
                            {
                                UserID = approver.ID,
                                AreaID = area.ID
                            });
                        }
                    }
                    _context.Add(area);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Area created successfully!";
                    return RedirectToAction("Index");
                }
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: Areas.AreaName"))
                {
                    ModelState.AddModelError("AreaName", "Unable to save changes. A Area with this Name already exists.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateAdminList(area);
            return View(area);
        }

        // GET: Areas/Edit/5
        [Authorize(Roles = "Top-level Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            if (id == null)
            {
                return NotFound();
            }

            var area = await _context.Areas
                .Include(r => r.AreaApprovers)
                .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(r => r.ID == id);

            if (area == null)
            {
                return NotFound();
            }
            
            ViewData["collapseApprovers"] = area.NeedsApproval ? " show " : "";
            ViewData["collapseTimeRestrictions"] = area.TimeOfDayRestrictions ? " show " : "";

            PopulateAdminList(area);
            return View(area);
        }

        // POST: Areas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Top-level Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string[] selectedOptions)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            var areaToUpdate = await _context.Areas
                .Include(r => r.AreaApprovers)
                .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(r => r.ID == id);

            if (areaToUpdate == null)
            {
                return NotFound();
            }

            UpdateAreaApprovers(selectedOptions, areaToUpdate);

            if (await TryUpdateModelAsync<Area>(areaToUpdate, "",
                r => r.AreaName, r => r.Description, r => r.BlackoutTime,
                r => r.MaxHoursPerSingleBooking, r => r.MaxHoursTotal,
                r => r.MaxNumberOfBookings, r => r.TimeOfDayRestrictions, r => r.EarliestTime, r => r.LatestTime,
                r => r.Enabled, r => r.NeedsApproval))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Area edited successfully!";
                    return RedirectToAction("Details", new { areaToUpdate.ID });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AreaExists(areaToUpdate.ID))
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
                    if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: Areas.AreaName"))
                    {
                        ModelState.AddModelError("AreaName", "Unable to save changes. An area with this Name already exists.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                    }
                }
            }
            PopulateAdminList(areaToUpdate);
            return View(areaToUpdate);

        }

        // GET: Areas/Delete/5
        [Authorize(Roles = "Top-level Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            if (id == null)
            {
                return NotFound();
            }

            var area = await _context.Areas
                .Include(r => r.AreaApprovers)
                .ThenInclude(r => r.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (area == null)
            {
                return NotFound();
            }

            return View(area);
        }

        // POST: Areas/Delete/5
        [Authorize(Roles = "Top-level Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            var area = await _context.Areas.FindAsync(id);
            try
            {
                _context.Areas.Remove(area);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Area deleted successfully!";
                return Redirect(ViewData["returnURL"].ToString());
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("FOREIGN KEY constraint failed"))
                {
                    ModelState.AddModelError("", "Unable to delete. You cannot delete a Area that has Rooms assigned.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to delete. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(area);
        }
        private SelectList EnabledSelectList(int? selectedId)
        {
            return new SelectList(_context.Areas
                .OrderBy(u => u.Enabled), "ID", "Enabled", selectedId);
        }

        private void PopulateDropDownLists(Area area = null)
        {
            ViewData["Enabled"] = EnabledSelectList(area?.ID);
        }

        private void PopulateAdminList(Area area)
        {
            var allUsers = _context.Users;
            var allAdmins = new HashSet<User>();
            var currentOptionsHS = new HashSet<int>(area.AreaApprovers.Select(b => b.UserID));

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

        //Update the Area's assigned Approvers
        private void UpdateAreaApprovers(string[] selectedOptions, Area areaToUpdate)
        {
            if (selectedOptions == null)
            {
                areaToUpdate.AreaApprovers = new List<AreaApprover>();
                return;
            }

            var selectedOptionsHS = new HashSet<string>(selectedOptions);
            var currentOptionsHS = new HashSet<int>(areaToUpdate.AreaApprovers.Select(b => b.UserID));

            foreach (var u in _context.Users)
            {
                if (selectedOptionsHS.Contains(u.ID.ToString())) //if selected
                {
                    if (!currentOptionsHS.Contains(u.ID)) //add if not in the Usergroup's collection
                    {
                        areaToUpdate.AreaApprovers.Add(new AreaApprover
                        {
                            UserID = u.ID,
                            AreaID = areaToUpdate.ID
                        });
                    }
                }
                else //not selected
                {
                    if (currentOptionsHS.Contains(u.ID))//remove if currently in the UserGroup's collection
                    {
                        AreaApprover approverToRemove = areaToUpdate.AreaApprovers.FirstOrDefault(d => d.UserID == u.ID);
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

        private bool AreaExists(int id)
        {
            return _context.Areas.Any(e => e.ID == id);
        }
    }
}
