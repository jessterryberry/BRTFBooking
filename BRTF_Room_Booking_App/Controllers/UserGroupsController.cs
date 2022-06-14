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
using BRTF_Room_Booking_App.ViewModels;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.AspNetCore.Authorization;

namespace BRTF_Room_Booking_App.Controllers
{
    [Authorize(Roles = "Top-level Admin, Admin")]
    public class UserGroupsController : Controller
    {
        private readonly BTRFRoomBookingContext _context;

        public UserGroupsController(BTRFRoomBookingContext context)
        {
            _context = context;
        }

        // GET: UserGroups
        public async Task<IActionResult> Index(int? page, int? pageSizeID, string SearchStringUserGroup, string SearchStringArea, string SearchStringTermProgram, string sortDirectionCheck, string sortFieldID,
            string actionButton, string sortDirection = "asc", string sortField = "UserGroup")
        {
            //Clear the sort/filter/paging URL Cookie for Controller
            CookieHelper.CookieSet(HttpContext, ControllerName() + "URL", "", -1);

            //Toggle the Open/Closed state of the collapse depending on if we are filtering
            ViewData["Filtering"] = "btn-outline-secondary"; //Asume not filtering
                                        //Then in each "test" for filtering, add ViewData["Filtering"] = " show" if true;

            ////Change colour of the button when filtering by setting this default
            //ViewData["Filter"] = "btn-outline-secondary";

            string[] sortOptions = new[] { "User Group" };

            var userGroups = from u in _context.UserGroups
                             .Include(r => r.RoomUserGroupPermissions).ThenInclude(r => r.Area)
                             .Include(r => r.AssignedTermAndPrograms).ThenInclude(r => r.TermAndProgram)
                             .OrderBy(r => r.UserGroupName)
                             select u;

            //Filter
            if (!String.IsNullOrEmpty(SearchStringUserGroup))
            {
                userGroups = userGroups.Where(u => u.UserGroupName.ToUpper().Contains(SearchStringUserGroup.ToUpper()));
                ViewData["Filtering"] = "btn-danger";
                //ViewData["Filter"] = "btn-danger";
            }            
            if (!String.IsNullOrEmpty(SearchStringArea))
            {
                userGroups = userGroups.Where(u => u.RoomUserGroupPermissions.Any(r => r.Area.AreaName.ToUpper().Contains(SearchStringArea.ToUpper())));
                ViewData["Filtering"] = "btn-danger";
                //ViewData["Filter"] = "btn-danger";
            }            
            if (!String.IsNullOrEmpty(SearchStringTermProgram))
            {
                userGroups = userGroups.Where(u => u.AssignedTermAndPrograms.Any(r => 
                   r.TermAndProgram.ProgramCode.ToUpper().Contains(SearchStringTermProgram.ToUpper())
                || r.TermAndProgram.ProgramName.ToUpper().Contains(SearchStringTermProgram.ToUpper())));
                ViewData["Filtering"] = "btn-danger";
                //ViewData["Filter"] = "btn-danger";
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
            if (sortField == "User Group")
            {
                if (sortDirection == "asc")
                {
                    userGroups = userGroups
                        .OrderBy(u => u.UserGroupName);
                }
                else
                {
                    userGroups = userGroups
                        .OrderByDescending(u => u.UserGroupName);
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

            var pagedData = await PaginatedList<UserGroup>.CreateAsync(userGroups.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }

        // GET: UserGroups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            if (id == null)
            {
                return NotFound();
            }

            var userGroup = await _context.UserGroups
                .Include(r => r.RoomUserGroupPermissions).ThenInclude(r => r.Area)
                .Include(r => r.AssignedTermAndPrograms).ThenInclude(r => r.TermAndProgram)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (userGroup == null)
            {
                return NotFound();
            }

            return View(userGroup);
        }

        // GET: UserGroups/Create
        [Authorize(Roles = "Top-level Admin")]
        public IActionResult Create()
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();
            UserGroup usergroup = new UserGroup();
            PopulateAssignedSpecialtyData(usergroup);
            PopulateTermAndProgramData(usergroup);
            return View();
        }

        // POST: UserGroups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Top-level Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,UserGroupName")] UserGroup userGroup, string[] selectedOptions, string[] selectedOptionsTP)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            try
            {
                UpdateRoomUserGroupPermissions(selectedOptions, userGroup);
                UpdateAssignedTermAndPrograms(selectedOptionsTP, userGroup);
                if (ModelState.IsValid)
                {
                    _context.Add(userGroup);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "User Group created successfully!";
                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: UserGroups.UserGroupName"))
                {
                    ModelState.AddModelError("UserGroupName", "Unable to save changes. A User Group with this Name already exists.");
                }
                else if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: UserGroupTermAndPrograms.TermAndProgramID"))
                {
                    ModelState.AddModelError("UserGroupName", "Unable to save changes. A selected Term and Program is already assigned to a User Group.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateAssignedSpecialtyData(userGroup);
            PopulateTermAndProgramData(userGroup);
            return View(userGroup);
        }

        // GET: UserGroups/Edit/5
        [Authorize(Roles = "Top-level Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            if (id == null)
            {
                return NotFound();
            }

            var userGroup = await _context.UserGroups
                .Include(u => u.RoomUserGroupPermissions).ThenInclude(u => u.Area)
                .Include(u => u.AssignedTermAndPrograms).ThenInclude(u => u.TermAndProgram)
                .FirstOrDefaultAsync(u => u.ID == id);

            if (userGroup == null)
            {
                return NotFound();
            }

            PopulateAssignedSpecialtyData(userGroup);
            PopulateTermAndProgramData(userGroup);

            return View(userGroup);
        }

        // POST: UserGroups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Top-level Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string[] selectedOptions, string[] selectedOptionsTP)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            // Get the UserGroup to update
            var userGroupToUpdate = await _context.UserGroups
                .Include(u => u.RoomUserGroupPermissions).ThenInclude(u => u.Area)
                .Include(u => u.AssignedTermAndPrograms).ThenInclude(u => u.TermAndProgram)
                .FirstOrDefaultAsync(u => u.ID == id);

            // Check that you got it or exit with a not found error
            if (userGroupToUpdate == null)
            {
                return NotFound();
            }

            // Update the UserGroup's RoomUserGroupPermissions
            UpdateRoomUserGroupPermissions(selectedOptions, userGroupToUpdate);
            UpdateAssignedTermAndPrograms(selectedOptionsTP, userGroupToUpdate);

            // Try updating it with the values posted
            if (await TryUpdateModelAsync<UserGroup>(userGroupToUpdate, "",
                p => p.UserGroupName, p=>p.AssignedTermAndPrograms, p => p.RoomUserGroupPermissions))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "User Group edited successfully!";
                    return RedirectToAction("Details", new { userGroupToUpdate.ID });
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserGroupExists(userGroupToUpdate.ID))
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
                    if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: UserGroups.UserGroupName"))
                    {
                        ModelState.AddModelError("UserGroupName", "Unable to save changes. A User Group with this Name already exists.");
                    }
                    else if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: UserGroupTermAndPrograms.TermAndProgramID"))
                    {
                        ModelState.AddModelError("UserGroupName", "Unable to save changes. A selected Term and Program is already assigned to a User Group.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                    }
                }
            }

            PopulateAssignedSpecialtyData(userGroupToUpdate);
            PopulateTermAndProgramData(userGroupToUpdate);
            return View(userGroupToUpdate);
        }

        // GET: UserGroups/Delete/5
        [Authorize(Roles = "Top-level Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            if (id == null)
            {
                return NotFound();
            }

            var userGroup = await _context.UserGroups
                .Include(u => u.RoomUserGroupPermissions).ThenInclude(u => u.Area)
                .Include(u => u.AssignedTermAndPrograms).ThenInclude(u => u.TermAndProgram)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (userGroup == null)
            {
                return NotFound();
            }

            return View(userGroup);
        }

        // POST: UserGroups/Delete/5
        [Authorize(Roles = "Top-level Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            var userGroup = await _context.UserGroups
                .Include(u => u.RoomUserGroupPermissions).ThenInclude(u => u.Area)
                .Include(u => u.AssignedTermAndPrograms).ThenInclude(u => u.TermAndProgram)
                .FirstOrDefaultAsync(m => m.ID == id);
            try
            {
                _context.UserGroups.Remove(userGroup);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Success! User Group deleted successfully!";
                return Redirect(ViewData["returnURL"].ToString());
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("FOREIGN KEY constraint failed"))
                {
                    ModelState.AddModelError("", "Unable to delete. You cannot delete a User Group that has Terms and Programs assigned.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to delete. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(userGroup);
        }

        // Populate Area Permissions Listbox
        private void PopulateAssignedSpecialtyData(UserGroup usergroup)
        {
            var allOptions = _context.Areas;
            var currentOptionsHS = new HashSet<int>(usergroup.RoomUserGroupPermissions.Select(b => b.AreaID));
            
            var selected = new List<ListOptionVM>();
            var available = new List<ListOptionVM>();

            foreach (var s in allOptions)
            {
                if (currentOptionsHS.Contains(s.ID))
                {
                    selected.Add(new ListOptionVM
                    {
                        ID = s.ID,
                        DisplayText = s.AreaName
                    });
                }
                else
                {
                    available.Add(new ListOptionVM
                    {
                        ID = s.ID,
                        DisplayText = s.AreaName
                    });
                }
            }

            ViewData["selOpts"] = new MultiSelectList(selected.OrderBy(s => s.DisplayText), "ID", "DisplayText");
            ViewData["availOpts"] = new MultiSelectList(available.OrderBy(s => s.DisplayText), "ID", "DisplayText");
        }       
        
        private void PopulateTermAndProgramData(UserGroup usergroup)
        {
            var allOptions = _context.TermAndPrograms;
            var currentOptionsHS = new HashSet<int>(usergroup.AssignedTermAndPrograms.Select(b => b.TermAndProgramID));
            
            var selected = new List<ListOptionVM>();
            var available = new List<ListOptionVM>();

            foreach (var s in allOptions)
            {
                if (currentOptionsHS.Contains(s.ID))
                {
                    selected.Add(new ListOptionVM
                    {
                        ID = s.ID,
                        DisplayText = s.TermAndProgramSummary
                    });
                }
                else
                {
                    available.Add(new ListOptionVM
                    {
                        ID = s.ID,
                        DisplayText = s.TermAndProgramSummary
                    });
                }
            }

            ViewData["selOptsTP"] = new MultiSelectList(selected.OrderBy(s => s.DisplayText), "ID", "DisplayText");
            ViewData["availOptsTP"] = new MultiSelectList(available.OrderBy(s => s.DisplayText), "ID", "DisplayText");
        }

        // Update Room User Group Permissions Listbox
        private void UpdateRoomUserGroupPermissions(string[] selectedOptions, UserGroup userGroupToUpdate)
        {
            if (selectedOptions == null)
            {
                userGroupToUpdate.RoomUserGroupPermissions = new List<RoomUserGroupPermission>();
                return;
            }

            var selectedOptionsHS = new HashSet<string>(selectedOptions);
            var currentOptionsHS = new HashSet<int>(userGroupToUpdate.RoomUserGroupPermissions.Select(b => b.AreaID));

            foreach (var s in _context.Areas)
            {
                if (selectedOptionsHS.Contains(s.ID.ToString())) //if selected
                {
                    if (!currentOptionsHS.Contains(s.ID)) //add if not in the Usergroup's collection
                    {
                        userGroupToUpdate.RoomUserGroupPermissions.Add(new RoomUserGroupPermission
                        {
                            AreaID = s.ID,
                            UserGroupID = userGroupToUpdate.ID
                        });
                    }
                }
                else //not selected
                {
                    if (currentOptionsHS.Contains(s.ID))//remove if currently in the UserGroup's collection
                    {
                        RoomUserGroupPermission specToRemove = userGroupToUpdate.RoomUserGroupPermissions.FirstOrDefault(d => d.AreaID == s.ID);
                        _context.Remove(specToRemove);
                    }
                }
            }
        }

        private void UpdateAssignedTermAndPrograms(string[] selectedOptions, UserGroup userGroupToUpdate)
        {
            if (selectedOptions == null)
            {
                userGroupToUpdate.AssignedTermAndPrograms = new List<UserGroupTermAndProgram>();
                return;
            }

            var selectedOptionsHS = new HashSet<string>(selectedOptions);
            var currentOptionsHS = new HashSet<int>(userGroupToUpdate.AssignedTermAndPrograms.Select(b => b.TermAndProgramID));

            foreach (var s in _context.TermAndPrograms)
            {
                if (selectedOptionsHS.Contains(s.ID.ToString())) //if selected
                {
                    if (!currentOptionsHS.Contains(s.ID)) //add if not in the Usergroup's collection
                    {
                        userGroupToUpdate.AssignedTermAndPrograms.Add(new UserGroupTermAndProgram
                        {
                            TermAndProgramID = s.ID,
                            UserGroupID = userGroupToUpdate.ID
                        });
                    }
                }
                else //not selected
                {
                    if (currentOptionsHS.Contains(s.ID))//remove if currently in the UserGroup's collection
                    {
                        UserGroupTermAndProgram termToRemove = userGroupToUpdate.AssignedTermAndPrograms.FirstOrDefault(d => d.TermAndProgramID == s.ID);
                        _context.Remove(termToRemove);
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

        private bool UserGroupExists(int id)
        {
            return _context.UserGroups.Any(e => e.ID == id);
        }
    }
}
