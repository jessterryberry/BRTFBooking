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

namespace BRTF_Room_Booking_App.Controllers
{
    [Authorize(Roles = "Top-level Admin, Admin")]
    public class TermAndProgramsController : Controller
    {
        private readonly BTRFRoomBookingContext _context;

        public TermAndProgramsController(BTRFRoomBookingContext context)
        {
            _context = context;
        }

        // GET: TermAndPrograms
        public async Task<IActionResult> Index(int? page, int? pageSizeID, string SearchProgCode, string SearchProgName, string SearchLevel, string sortDirectionCheck, string sortFieldID,
            string actionButton, string sortDirection = "asc", string sortField = "Program Code")
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            //Change colour of the button when filtering by setting this default
            ViewData["Filtering"] = "btn-outline-secondary";

            ////Change colour of the button when filtering by setting this default
            //ViewData["Filter"] = "btn-outline-secondary";

            //note: make sure this array has matching values to the column headings
            string[] sortOptions = new[] { "Program Code", "Program Name", "Term", "User Group"};

            // Start with Includes but make sure your expression returns an
            // IQueryable<> so we can add filter and sort 
            // options later.
            var termAndPrograms = (from t in _context.TermAndPrograms
                .Include(t => t.UserGroup)
                select t).AsNoTracking();

            //adding filters
            if (!String.IsNullOrEmpty(SearchProgCode))
            {
                termAndPrograms = termAndPrograms.Where(u => u.ProgramCode.ToUpper().Contains(SearchProgCode.ToUpper()));
                ViewData["Filtering"] = "btn-danger";
                //ViewData["Filter"] = "btn-danger";
            }
            if (!String.IsNullOrEmpty(SearchProgName))
            {
                termAndPrograms = termAndPrograms.Where(u => u.ProgramName.ToUpper().Contains(SearchProgName.ToUpper()));
                ViewData["Filtering"] = "btn-danger";
                //ViewData["Filter"] = "btn-danger";
            }
            if (!String.IsNullOrEmpty(SearchLevel))
            {
                termAndPrograms = termAndPrograms.Where(u => u.ProgramLevel.Equals(Convert.ToInt32(SearchLevel)));
                ViewData["Filtering"] = "btn-danger";
                //ViewData["Filter"] = "btn-danger";
            }

            //Before sorting, you need to check to see if there has been a change to of filter/sort
            if (!String.IsNullOrEmpty(actionButton)) //the form has been submitted
            {
                if (sortOptions.Contains(actionButton))
                {
                    if (actionButton == sortField) //reversing on the same field
                    {
                        sortDirection = sortDirection == "asc" ? "desc" : "asc";
                    }
                    sortField = actionButton; //sorting by the button that was clicked
                }
                else //Sort by the controls in the filter area
                {
                    sortDirection = String.IsNullOrEmpty(sortDirectionCheck) ? "asc" : "desc";
                    sortField = sortFieldID;
                }
            }
            //now the sort field and direction is known
            if (sortField == "Program Code")
            {
                if (sortDirection == "asc")
                {
                    termAndPrograms = termAndPrograms
                        .OrderBy(u => u.ProgramCode);
                }
                else
                {
                    termAndPrograms = termAndPrograms
                        .OrderByDescending(u => u.ProgramCode);
                }
            }
            if (sortField == "Program Name")//sorting by program name
            {
                if (sortDirection == "asc")
                {
                    termAndPrograms = termAndPrograms
                        .OrderBy(u => u.ProgramName);
                }
                else
                {
                    termAndPrograms = termAndPrograms
                        .OrderByDescending(u => u.ProgramName);
                }
            }
            if (sortField == "Term")//sorting by program name
            {
                if (sortDirection == "asc")
                {
                    termAndPrograms = termAndPrograms
                        .OrderBy(u => u.ProgramLevel)
                        .ThenBy(u => u.ProgramCode);
                }
                else
                {
                    termAndPrograms = termAndPrograms
                        .OrderByDescending(u => u.ProgramLevel)
                        .ThenBy(u => u.ProgramCode);
                }
            }
            else //sorting by User Group
            {
                if (sortDirection == "asc")
                {
                    termAndPrograms = termAndPrograms
                        .OrderBy(u => u.UserGroup.UserGroupName)
                        .ThenBy(u => u.ProgramCode);
                }
                else
                {
                    termAndPrograms = termAndPrograms
                        .OrderByDescending(u => u.UserGroup.UserGroupName)
                        .ThenBy(u => u.ProgramCode);
                }
            }
            //now to set the sort for the next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;
            //SelectList for Sorting Options
            ViewBag.sortFieldID = new SelectList(sortOptions, sortField.ToString());

            //Handle Paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);

            var pagedData = await PaginatedList<TermAndProgram>.CreateAsync(termAndPrograms.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }

        // GET: TermAndPrograms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            if (id == null)
            {
                return NotFound();
            }

            var termAndProgram = await _context.TermAndPrograms
                .Include(t => t.UserGroup)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (termAndProgram == null)
            {
                return NotFound();
            }

            return View(termAndProgram);
        }

        // GET: TermAndPrograms/Create
        [Authorize(Roles = "Top-level Admin")]
        public IActionResult Create()
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            PopulateDropDownLists();
            return View();
        }

        // POST: TermAndPrograms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Top-level Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,ProgramName,ProgramCode,ProgramLevel,UserGroupID")] TermAndProgram termAndProgram)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(termAndProgram);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Term & Program created Successfully!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: TermAndPrograms.ProgramLevel, TermAndPrograms.ProgramCode"))
                {
                    ModelState.AddModelError("ProgramCode", "Unable to save changes. This Program Code and Level combination already exists.");
                    ModelState.AddModelError("ProgramLevel", "Unable to save changes. This Program Code and Level combination already exists.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateDropDownLists(termAndProgram);
            return View(termAndProgram);
        }

        // GET: TermAndPrograms/Edit/5
        [Authorize(Roles = "Top-level Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            if (id == null)
            {
                return NotFound();
            }

            var termAndProgram = await _context.TermAndPrograms.FindAsync(id);
            if (termAndProgram == null)
            {
                return NotFound();
            }
            PopulateDropDownLists(termAndProgram);
            return View(termAndProgram);
        }

        // POST: TermAndPrograms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Top-level Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            // Get the TermAndProgram to update
            var termAndProgramToUpdate = await _context.TermAndPrograms.FirstOrDefaultAsync(p => p.ID == id);

            // Check that you got it or exit with a not found error
            if (termAndProgramToUpdate == null)
            {
                return NotFound();
            }
            
            // Try updating it with the values posted
            if (await TryUpdateModelAsync<TermAndProgram>(termAndProgramToUpdate, "",
                p => p.ProgramName, p => p.ProgramCode, p => p.ProgramLevel,
                p => p.UserGroupID))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Term & Program edited Successfully!";
                    return RedirectToAction("Details", new { termAndProgramToUpdate.ID });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TermAndProgramExists(termAndProgramToUpdate.ID))
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
                    if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: TermAndPrograms.ProgramLevel, TermAndPrograms.ProgramCode"))
                    {
                        ModelState.AddModelError("ProgramCode", "Unable to save changes. This Program Code and Level combination already exists.");
                        ModelState.AddModelError("ProgramLevel", "Unable to save changes. This Program Code and Level combination already exists.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                    }
                }
            }
            PopulateDropDownLists(termAndProgramToUpdate);
            return View(termAndProgramToUpdate);
        }

        // GET: TermAndPrograms/Delete/5
        [Authorize(Roles = "Top-level Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            if (id == null)
            {
                return NotFound();
            }

            var termAndProgram = await _context.TermAndPrograms
                .Include(t => t.UserGroup)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (termAndProgram == null)
            {
                return NotFound();
            }

            return View(termAndProgram);
        }

        // POST: TermAndPrograms/Delete/5
        [Authorize(Roles = "Top-level Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var termAndProgram = await _context.TermAndPrograms
                .Include(t => t.UserGroup)
                .FirstOrDefaultAsync(m => m.ID == id);
            try
            {
                _context.TermAndPrograms.Remove(termAndProgram);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Term & Program deleted Successfully!";
                return Redirect(ViewData["returnURL"].ToString());
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("FOREIGN KEY constraint failed"))
                {
                    ModelState.AddModelError("", "Unable to delete. You cannot delete a Term and Program that has Users assigned.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to delete. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(termAndProgram);
        }

        //This is a twist on the PopulateDropDownLists approach
        //  Create methods that return each SelectList separately
        //  and one method to put them all into ViewData.
        //This approach allows for AJAX requests to refresh
        //DDL Data at a later date.
        private SelectList UserGroupSelectList(int? selectedId)
        {
            return new SelectList(_context.UserGroups
                .OrderBy(u => u.UserGroupName), "ID", "UserGroupName", selectedId);
        }
        private void PopulateDropDownLists(TermAndProgram termAndProgram = null)
        {
            ViewData["UserGroupID"] = UserGroupSelectList(termAndProgram?.UserGroupID);
        }

        private string ControllerName()
        {
            return this.ControllerContext.RouteData.Values["controller"].ToString();
        }

        private void ViewDataReturnURL()
        {
            ViewData["returnURL"] = MaintainURL.ReturnURL(HttpContext, ControllerName());
        }

        private bool TermAndProgramExists(int id)
        {
            return _context.TermAndPrograms.Any(e => e.ID == id);
        }
    }
}
