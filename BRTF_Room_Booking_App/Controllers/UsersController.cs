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
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using BRTF_Room_Booking_App.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace BRTF_Room_Booking_App.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly BTRFRoomBookingContext _context;
        private readonly ApplicationDbContext _identityContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;

        public UsersController(BTRFRoomBookingContext context, ApplicationDbContext identityContext,
            UserManager<IdentityUser> userManager, IEmailSender emailSender)
        {
            _context = context;
            _identityContext = identityContext;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [Authorize(Roles = "Top-level Admin, Admin")]
        // GET: Users
        public async Task<IActionResult> Index(string SearchName, string SearchEmail, int? UserGroupID, int? page, int? pageSizeID, string sortDirectionCheck, string sortFieldID,
            string actionButton, string sortDirection = "asc", string sortField = "Full Name")
        {
            //Clear the sort/filter/paging URL Cookie for Controller
            CookieHelper.CookieSet(HttpContext, ControllerName() + "URL", "", -1);

            //Toggle the open/closed state of the collapse depending on if something is being filtered
            ViewData["Filtering"] = "btn-outline-secondary"; //Assume nothing is filtered

            //note: make sure this array has matching values to the column headings
            string[] sortOptions = new[] { "Full Name", "User Group", };

            PopulateDropDownLists(); //data for User Filter DDL

            // Start with Includes but make sure your expression returns an
            // IQueryable<> so we can add filter and sort 
            // options later.
            var users = from u in _context.Users
                .Include(u => u.TermAndProgram).ThenInclude(t => t.UserGroup)
                .AsNoTracking()
                        select u;

            //adding filters
            if (UserGroupID.HasValue)
            {
                users = users.Where(u => u.TermAndProgram.UserGroupID == UserGroupID);
                ViewData["Filtering"] = "btn-danger";
            }
            if (!String.IsNullOrEmpty(SearchName))
            {
                users = users.Where(u => u.LastName.ToUpper().Contains(SearchName.ToUpper()) ||
                                    u.MiddleName.ToUpper().Contains(SearchName.ToUpper()) ||
                                    u.FirstName.ToUpper().Contains(SearchName.ToUpper()));
                ViewData["Filtering"] = "btn-danger";
            }
            if (!String.IsNullOrEmpty(SearchEmail))
            {
                users = users.Where(u => u.Email.ToUpper().Contains(SearchEmail.ToUpper()));
                ViewData["Filtering"] = "btn-danger";
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
            if (sortField == "Full Name")
            {
                if (sortDirection == "asc")
                {
                    users = users
                        .OrderBy(u => u.FirstName)
                        .ThenBy(u => u.LastName);
                }
                else
                {
                    users = users
                        .OrderByDescending(u => u.FirstName)
                        .ThenByDescending(u => u.LastName);
                }
            }
            else //sorting by Term and Program
            {
                if (sortDirection == "asc")
                {
                    users = users
                        .OrderBy(u => u.TermAndProgram.UserGroup.UserGroupName);
                }
                else
                {
                    users = users
                        .OrderByDescending(u => u.TermAndProgram.UserGroup.UserGroupName);
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

            var pagedData = await PaginatedList<User>.CreateAsync(users.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }


        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.TermAndProgram).ThenInclude(t => t.UserGroup)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            var identityUser = await _identityContext.Users.Where(u => u.UserName == user.Username).FirstOrDefaultAsync();

            if (user == null || identityUser == null)
            {
                return NotFound();
            }

            //User can only view thier own data
            if (User.IsInRole("User"))
            {
                if (User.Identity.Name != user.Username)
                {
                    TempData["Message"] = "You are not authorized to view other users details.";
                    return Redirect(ViewData["returnURL"].ToString());
                }
            }

            ViewData["Role"] = _userManager.GetRolesAsync(identityUser).Result.FirstOrDefault();
            return View(user);
        }

        // GET: Users/Create
        [Authorize(Roles = "Top-level Admin, Admin")]
        public IActionResult Create()
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            ViewData["Role"] = RoleSelectList("User");
            PopulateDropDownLists();
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Top-level Admin, Admin")]
        public async Task<IActionResult> Create(
            [Bind("ID,Username,FirstName,MiddleName,LastName,Email,EmailBookingNotifications,EmailCancelNotifications,TermAndProgramID,RoleID")] User user,
            string Role)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(user);

                    IdentityUser identityUser = new IdentityUser
                    {
                        UserName = user.Username,
                        Email = user.Email
                    };

                    // Generate random password for Identity
                    Random random = new Random();
                    char randomChar;
                    string randomPassword = "";
                    var passwordValidator = new PasswordValidator<IdentityUser>();
                    bool isValidPassword = false;

                    while (isValidPassword == false)    // Loop to re-generate password if it fails rules
                    {
                        randomPassword = "";
                        for (int i = 0; i < 18; i++)
                        {
                            randomChar = (char)random.Next(48, 122);
                            randomPassword += randomChar;
                        }

                        randomPassword += "aByZ+@";   // Append special characters to the end to ensure random string passes Identity rules

                        var passwordResult = passwordValidator.ValidateAsync(_userManager, null, randomPassword).Result;

                        isValidPassword = passwordResult.Succeeded;
                    }

                    // Create Identity user account with random password
                    IdentityResult result = _userManager.CreateAsync(identityUser, randomPassword).Result;

                    if (!result.Succeeded)
                    {
                        // Add each error to the alert message
                        string alertMessage = "Unable to save changes due to the following reasons:<br /><ul>";

                        foreach (var error in result.Errors)
                        {
                            string errorCode = error.Code;
                            string errorDescription = error.Description;

                            alertMessage += "<li>" + errorDescription + "</li>";
                        }

                        alertMessage += "</ul>";

                        TempData["AlertMessage"] = alertMessage;
                        return Redirect(ViewData["returnURL"].ToString());
                    }
                    else
                    {
                        IdentityResult addRoleResult = await _userManager.AddToRoleAsync(identityUser, Role);

                        if (!addRoleResult.Succeeded)
                        {
                            TempData["AlertMessage"] = "Error adding User to new Role.";
                            return Redirect(ViewData["returnURL"].ToString());
                            // ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                            // throw new Exception("Error adding User to new Role.");
                        }
                    }

                    await _context.SaveChangesAsync();

                    // Send email for user to set their Identity password
                    string forgotPasswordUrl = $"{Request.Scheme}://{Request.Host.Value}" + "/Identity/Account/ForgotPassword";
                    await _emailSender.SendEmailAsync(user.Email, user.FirstName + ", your BRTF Room Booking account is ready!",
                        "Hi, " + user.FirstName + "!<br /><br />" +
                        "Your BRTF Room Booking account was created with the following username:<br /><br />" +
                        "Username: <b>" + user.Username + "</b><br /><br />" +
                        "<b>Please use the <a href='" + forgotPasswordUrl + "'>Forgot Password link</a> to set your password.</b>");

                    TempData["Message"] = "User Created Successfully!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: Users.Username"))
                {
                    // ModelState.AddModelError("Username", "Unable to save changes. A User with this Username already exists.");
                    TempData["AlertMessage"] = "Unable to save changes. A User with this Username already exists.";
                    return Redirect(ViewData["returnURL"].ToString());
                }
                if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: Users.Email"))
                {
                    // ModelState.AddModelError("Username", "Unable to save changes. A User with this Username already exists.");
                    TempData["AlertMessage"] = "Unable to save changes. A User with this Email already exists.";
                    return Redirect(ViewData["returnURL"].ToString());
                }
                else
                {
                    TempData["AlertMessage"] = "Unable to save changes. Try again, and if the problem persists, see your system administrator.";
                    return Redirect(ViewData["returnURL"].ToString());
                    //ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            //Get the full TermAndProgram object for the User and then populate DDL
            ViewData["Role"] = RoleSelectList(Role);
            user.TermAndProgram = await _context.TermAndPrograms.FindAsync(user.TermAndProgramID);
            PopulateDropDownLists(user);
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.TermAndProgram).ThenInclude(t => t.UserGroup)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            var identityUser = await _identityContext.Users.Where(u => u.UserName == user.Username).FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            if (User.IsInRole("User")
                || (User.IsInRole("Admin") && _userManager.GetRolesAsync(identityUser).Result.FirstOrDefault() == "Top-level Admin"))
            {
                if (User.Identity.Name != user.Username)
                {
                    TempData["AlertMessage"] = "You are not authorized to edit this user's details.";
                    return Redirect(ViewData["returnURL"].ToString());
                }
            }

            ViewData["Role"] = RoleSelectList(_userManager.GetRolesAsync(identityUser).Result.FirstOrDefault());
            PopulateDropDownLists(user);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string Password, string Role)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            // Get the User to update
            var userToUpdate = await _context.Users.FirstOrDefaultAsync(p => p.ID == id);
            var identityUserToUpdate = await _identityContext.Users.Where(u => u.UserName == userToUpdate.Username).FirstOrDefaultAsync();
            string identityUserRole = _userManager.GetRolesAsync(identityUserToUpdate).Result.FirstOrDefault();

            // Check that you got it or exit with a not found error
            if (userToUpdate == null || identityUserToUpdate == null)
            {
                return NotFound();
            }

            if (User.IsInRole("User")
                || (User.IsInRole("Admin") && identityUserRole == "Top-level Admin"))
            {
                if (User.Identity.Name != userToUpdate.Username)
                {
                    TempData["AlertMessage"] = "You are not authorized to edit this user's details.";
                    return Redirect(ViewData["returnURL"].ToString());
                }
            }

            // Try updating it with the values posted
            if (await TryUpdateModelAsync<User>(userToUpdate, "",
                p => p.FirstName, p => p.MiddleName, p => p.LastName,
                p => p.Email, p => p.EmailBookingNotifications, p => p.EmailCancelNotifications,
                p => p.TermAndProgramID)
                )
            {
                try
                {
                    // Check if we should update password
                    if (!String.IsNullOrEmpty(Password))
                    {
                        // Try updating password
                        string resetToken = await _userManager.GeneratePasswordResetTokenAsync(identityUserToUpdate);
                        IdentityResult passwordChangeResult = await _userManager.ResetPasswordAsync(identityUserToUpdate, resetToken, Password);
                        if (!passwordChangeResult.Succeeded)
                        {
                            ModelState.AddModelError("", "Unable to save Password.");

                            foreach (var error in passwordChangeResult.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }

                            throw new Exception("Error updating User password in Identity.");
                        }
                    }

                    // Update User email in Identity
                    if (!await TryUpdateModelAsync<IdentityUser>(identityUserToUpdate, "", p => p.Email))
                    {
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                        throw new Exception("Error updating User email in Identity.");
                    }

                    // Update User role
                    //if (Role != identityUserRole)
                    //{
                    //    IdentityResult removeRoleResult = await _userManager.RemoveFromRoleAsync(identityUserToUpdate, identityUserRole);

                    //    if (!removeRoleResult.Succeeded)
                    //    {
                    //        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                    //        throw new Exception("Error removing User from previous Role.");
                    //    }

                    //    IdentityResult addRoleResult = await _userManager.AddToRoleAsync(identityUserToUpdate, Role);

                    //    if (!addRoleResult.Succeeded)
                    //    {
                    //        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                    //        throw new Exception("Error adding User to new Role.");
                    //    }
                    //}

                    await _identityContext.SaveChangesAsync();
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "User Edited Successfully!";
                    return RedirectToAction("Details", new { userToUpdate.ID });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(userToUpdate.ID))
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
                    if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: Users.Username"))
                    {
                        ModelState.AddModelError("Username", "Unable to save changes. A User with this Username already exists.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                    }
                }
                catch (Exception ex)
                {
                    if (ex.GetBaseException().Message.Contains("Error updating User password in Identity."))
                    {
                        // We already added this error message to the model earlier
                    }
                    else if (ex.GetBaseException().Message.Contains("Error updating User email in Identity."))
                    {
                        // We already added this error message to the model earlier
                    }
                }
            }
            ViewData["Role"] = RoleSelectList(Role);
            ViewData["TermAndProgramID"] = TermAndProgramSelectList(userToUpdate.TermAndProgramID);
            return View(userToUpdate);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.TermAndProgram).ThenInclude(t => t.UserGroup)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            var identityUser = await _identityContext.Users.Where(u => u.UserName == user.Username).FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            if (User.IsInRole("Admin") || User.IsInRole("User"))
            {
                if (User.Identity.Name != user.Username)
                {
                    TempData["AlertMessage"] = "You are not authorized to delete other users.";
                    return Redirect(ViewData["returnURL"].ToString());
                }
            }
            if (User.Identity.Name == user.Username)
            {
                TempData["AlertMessage"] = "You cannot delete your own account.";
                return Redirect(ViewData["returnURL"].ToString());
            }

            ViewData["Role"] = _userManager.GetRolesAsync(identityUser).Result.FirstOrDefault();
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            // Get the User to delete from BTRF Context and Identity
            var user = await _context.Users
                .Include(u => u.TermAndProgram).ThenInclude(t => t.UserGroup)
                .FirstOrDefaultAsync(m => m.ID == id);
            var identityUser = await _identityContext.Users.Where(u => u.UserName == user.Username).FirstOrDefaultAsync();

            // Check if the User being deleted is the currently logged-in User
            if (User.Identity.Name == user.Username)
            {
                // Do not allow logged-in User to delete themself
                ModelState.AddModelError("", "Unable to delete. You cannot delete your own account.");
            }

            if (User.IsInRole("Admin") || User.IsInRole("User"))
            {
                if (User.Identity.Name != user.Username)
                {
                    TempData["AlertMessage"] = "You are not authorized to delete other users.";
                    return Redirect(ViewData["returnURL"].ToString());
                }
            }

            try
            {
                if (ModelState.IsValid)
                {
                    _context.Users.Remove(user);
                    await DeleteIdentityUser(user.Username);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "<strong>Success!</strong> User deleted successfully!";
                    return Redirect(ViewData["returnURL"].ToString());
                }
            }
            catch (DbUpdateException)
            {
                //Note: there is really no reason a delete should fail if you can "talk" to the database.
                ModelState.AddModelError("", "Unable to delete. Try again, and if the problem persists, see your system administrator.");
            }
            ViewData["Role"] = _userManager.GetRolesAsync(identityUser).Result.FirstOrDefault();
            return View(user);
        }

        /// <summary>
        /// Deletes a User from Identity database.
        /// </summary>
        /// <param name="userName">Username of User to delete.</param>
        private async Task DeleteIdentityUser(string userName)
        {
            var userToDelete = await _identityContext.Users.Where(u => u.UserName == userName).FirstOrDefaultAsync();
            if (userToDelete != null)
            {
                _identityContext.Users.Remove(userToDelete);
                await _identityContext.SaveChangesAsync();
            }
        }

        // GET: Users/DeleteBulk
        public IActionResult DeleteBulk()
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();
            if (User.IsInRole("Admin"))
            {
                TempData["AlertMessage"] = "You are not authorized to delete other users.";
                return Redirect(ViewData["returnURL"].ToString());
            }
            else
            {
                PopulateSelectedRoleData(new string[0]);
                PopulateSelectedGroupData(new string[0]);
                return View();
            }
        }

        // POST: Users/DeleteBulk
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBulk(string[] selectedRoles, int[] selectedGroups)
        {
            // Track counts to output message at the end
            int deletedUserCount = 0;
            bool skippedLoggedInUser = false;

            // Check if we should delete Users from Roles
            if (selectedRoles.Count() > 0)
            {
                var identityUsers = from u in _identityContext.Users select u;

                // Delete Users within selected Roles
                foreach (var identityUser in identityUsers)
                {
                    // Check if the User is in the Role
                    string identityUserRole = _userManager.GetRolesAsync(identityUser).Result.FirstOrDefault();
                    if (selectedRoles.Contains(identityUserRole))
                    {
                        // Check if the User being deleted is the currently logged-in User
                        if (identityUser.UserName == User.Identity.Name)
                        {
                            // Don't delete logged-in User
                            skippedLoggedInUser = true;
                        }
                        else
                        {
                            var user = await _context.Users
                                .Where(u => u.Username == identityUser.UserName)
                                .FirstOrDefaultAsync();

                            // Delete the User from both Db Contexts
                            _identityContext.Users.Remove(identityUser);
                            _context.Users.Remove(user);
                            deletedUserCount++;
                        }
                    }
                }
            }

            // Save changes before beginning second pass
            await _identityContext.SaveChangesAsync();
            await _context.SaveChangesAsync();

            // Check if we should delete Users from User Groups
            if (selectedGroups.Count() > 0)
            {
                // Get Terms and Programs corresponding to the selected User Groups
                var selectedTermsAndPrograms = from t in _context.TermAndPrograms
                    .Where(t => selectedGroups.Contains(t.UserGroupID))
                                               select t.ID;

                var users = from u in _context.Users select u;

                // Delete Users whose Term and Programs is in a selected User Group
                foreach (var user in users)
                {
                    if (selectedTermsAndPrograms.Contains(user.TermAndProgramID))
                    {
                        // Check if the User being deleted is the currently logged-in User
                        if (user.Username == User.Identity.Name)
                        {
                            // Don't delete logged-in User
                            skippedLoggedInUser = true;
                        }
                        else
                        {
                            var identityUser = await _identityContext.Users
                            .Where(u => u.UserName == user.Username)
                            .FirstOrDefaultAsync();

                            // Delete the User from both Db Contexts
                            _identityContext.Users.Remove(identityUser);
                            _context.Users.Remove(user);
                            deletedUserCount++;
                        }
                    }
                }
            }

            // Save changes and return to Index
            await _identityContext.SaveChangesAsync();
            await _context.SaveChangesAsync();
            string alertMessage = "<strong>Success!</strong><ul><li>" + deletedUserCount.ToString() + " Users deleted.</li>";
            if (skippedLoggedInUser)
            {
                // Add extra comment when logged-in User was skipped
                alertMessage += "<li>1 User skipped. You cannot delete your own account.</li>";
            }
            alertMessage += "</ul>";
            TempData["AlertMessage"] = alertMessage;
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Creates or updates a batch of Users using a .XLSX or .CSV file.
        /// </summary>
        /// <param name="theExcel">The .XLSX or .CSV file containing User data.</param>
        [HttpPost]
        public async Task<IActionResult> InsertFromExcel(IFormFile theExcel)
        {
            string uploadMessage = "";//Return status message for application
            int insertedCount = 0;//Counter for inserted records
            int duplicateCount = 0;//Counter for duplicate records
            int skippedCount = 0;//Counter for skipped records (Example: non-BRTF students should be skipped)
            HashSet<string> skippedPrograms = new HashSet<string>();

            try
            {
                ExcelPackage excel = new ExcelPackage();

                // Check the file extension of uploaded file
                if (theExcel == null)
                {
                    // No file was uploaded
                    // so throw an error
                    throw new Exception("User did not upload a file.");
                }
                else if (theExcel.FileName.ToLower().Contains(".xlsx"))
                {
                    // If it is an Excel file, read it directly via memory stream
                    using (var memoryStream = new MemoryStream())
                    {
                        await theExcel.CopyToAsync(memoryStream);
                        excel = new ExcelPackage(memoryStream);
                    }
                }
                else if (theExcel.FileName.ToLower().Contains(".csv"))
                {
                    // If it is a .CSV, convert contents to a string, then load it into a new Worksheet

                    // Create StringBuilder to convert .CSV contents to string
                    var readResult = new StringBuilder();
                    using (var streamReader = new StreamReader(theExcel.OpenReadStream()))
                    {
                        while (streamReader.Peek() >= 0)
                        {
                            string currentLine = await streamReader.ReadLineAsync();// Read each row from .CSV line-by-line

                            // Check currentLine for the character " (quotation)
                            while (currentLine.Contains("\""))
                            {
                                // The character " (quotation) denotes a value that contains a , (comma)
                                // This is dangerous because the delimiter of a .CSV (comma-separated values) is a , (comma)
                                // EPPlus reading .CSV values is primitive, so the safest thing to do is replace the , (comma) with a ⸴ (comma symbol)

                                // Use Regex to find pattern that begins and ends with " (quotation)
                                var match = Regex.Match(currentLine, "\".*?\"");

                                // Pull only the value from the Regex match by removing the " (quotation)
                                // Then replace the , (comma) with a ⸴ (comma symbol)
                                string value = match.Value.Replace("\"", "").Replace(",", "٬");

                                // Inside the original line, replace the value with the same value where the , (comma) is replaced with a ٬ (comma symbol)
                                currentLine = currentLine.Replace(match.Value, value);
                            }

                            readResult.AppendLine(currentLine);
                        }
                    }
                    string theContents = readResult.ToString(); // Store StringBuilder's reading of .CSV contents into string

                    // Create a new WorkSheet
                    ExcelWorksheet worksheet = excel.Workbook.Worksheets.Add("Sheet 1");

                    // Load the CSV data into cell A1
                    worksheet.Cells["A1"].LoadFromText(theContents);
                }
                else
                {
                    // The uploaded file is neither an Excel file nor a .CSV file
                    // so throw an error
                    throw new Exception("Uploaded file is incorrect type.");
                }

                // Set the Worksheet that EPPlus will read values from
                var workSheet = excel.Workbook.Worksheets[0];
                var start = workSheet.Dimension.Start;
                var end = workSheet.Dimension.End;

                // Get existing Usernames to check for duplicates
                var existingUsers = (_context.Users
                    .Select(c => new { Username = c.Username }))
                    .ToList().Select(c => c.Username).ToHashSet();

                // Get an IQueryable all Terms and Programs
                var programs = from p in _context.TermAndPrograms
                               select p;

                // Loop to read values row-by-row starting from the 2nd row (We skip the 1st row which is only column headings)
                for (int row = start.Row + 1; row <= end.Row; row++)
                {
                    // Skip blank rows
                    if (String.IsNullOrEmpty(workSheet.Cells[row, 1].Text) || !Int32.TryParse(workSheet.Cells[row, 1].Text, out int testStuID)) continue;

                    // Look up student's Term and Program, or skip this student if their program is not found (Their program is not part of BRTF)

                    // Check if we have the specific combination of Program Code and Level already
                    var newUserProgram = programs.Where(p => p.ProgramCode.ToUpper().Equals(workSheet.Cells[row, 7].Text.ToUpper())
                                                && p.ProgramLevel.Equals(int.Parse(workSheet.Cells[row, 9].Text)));
                    int newUserProgID;

                    if (newUserProgram.Count() > 0)
                    {
                        //If we have matches, we grab the ID (FirstOrDefault as there should only be one result) so we can assign it on the User
                        newUserProgID = newUserProgram.FirstOrDefault().ID;
                    }
                    else
                    {
                        skippedCount++; // Skip this row if their program is not found (Their program is not part of BRTF)
                        skippedPrograms.Add(workSheet.Cells[row, 7].Text + " Level " + workSheet.Cells[row, 9].Text + (!string.IsNullOrEmpty(workSheet.Cells[row, 8].Text) ? " - " + workSheet.Cells[row, 8].Text : ""));
                        continue;
                    }

                    // Row by row...

                    // Check if Username already exists
                    string newUsername = workSheet.Cells[row, 1].Text;

                    if (existingUsers.Contains(newUsername))
                    {
                        // Update existing Users

                        // Get the User to update
                        var userToUpdate = await _context.Users.FirstOrDefaultAsync(p => p.Username == newUsername);
                        var identityUserToUpdate = await _identityContext.Users.FirstOrDefaultAsync(p => p.UserName == newUsername);

                        userToUpdate.FirstName = workSheet.Cells[row, 2].Text;
                        userToUpdate.MiddleName = workSheet.Cells[row, 3].Text;
                        userToUpdate.LastName = workSheet.Cells[row, 4].Text;
                        userToUpdate.TermAndProgramID = newUserProgID;
                        userToUpdate.Email = workSheet.Cells[row, 6].Text;
                        userToUpdate.Username = newUsername;

                        identityUserToUpdate.Email = workSheet.Cells[row, 6].Text;

                        // Update the model in the database (Note: We don't Save Changes here. All changes will be saved at once at the end)
                        await TryUpdateModelAsync(userToUpdate);
                        await TryUpdateModelAsync(identityUserToUpdate);

                        duplicateCount++;
                    }
                    else
                    {
                        // Insert new Users

                        // For new Users, we set their password using their birthdate
                        // Excel stores dates in multiple ways, which are not easily parsed with a single method
                        // As such, we will first attempt to parse the date as a string, then if that fails, as a number
                        string dobCell = workSheet.Cells[row, 5].Text.Replace("٬", ",");    // In case the date is supposed to have a comma in it, turn the comma symbol back into a comma
                        DateTime dob = new DateTime();
                        if (!DateTime.TryParse(dobCell, out dob))//Attempt to parse cell contents as string
                        {
                            // It failed, so try parsing the cell contents as a number
                            if (Double.TryParse(dobCell, out double dobCellAsDouble))
                            {
                                dob = DateTime.FromOADate(dobCellAsDouble);
                            }
                            else
                            {
                                // DOB failed to be parsed as a string and as a number
                                // so throw an error
                                throw new Exception("DOB on row " + row.ToString() + " is not a readable format.");
                            }
                        }

                        // Generate random password for Identity
                        Random random = new Random();
                        char randomChar;
                        string randomPassword = "";
                        var passwordValidator = new PasswordValidator<IdentityUser>();
                        bool isValidPassword = false;

                        while (isValidPassword == false)    // Loop to re-generate password if it fails rules
                        {
                            randomPassword = "";
                            for (int i = 0; i < 18; i++)
                            {
                                randomChar = (char)random.Next(48, 122);
                                randomPassword += randomChar;
                            }

                            randomPassword += "aByZ+@";   // Append special characters to the end to ensure random string passes Identity rules

                            var passwordResult = passwordValidator.ValidateAsync(_userManager, null, randomPassword).Result;

                            isValidPassword = passwordResult.Succeeded;
                        }

                        // Build User
                        User newUser = new User
                        {
                            FirstName = workSheet.Cells[row, 2].Text,
                            MiddleName = workSheet.Cells[row, 3].Text,
                            LastName = workSheet.Cells[row, 4].Text,
                            TermAndProgramID = newUserProgID,
                            Email = workSheet.Cells[row, 6].Text,
                            EmailBookingNotifications = true,
                            EmailCancelNotifications = true,
                            Username = newUsername
                        };

                        IdentityUser newIdentityUser = new IdentityUser
                        {
                            UserName = newUsername,
                            Email = workSheet.Cells[row, 6].Text
                        };

                        // Insert User (Note: We don't Save Changes here. All changes will be saved at once at the end)
                        _context.Users.Add(newUser);
                        await _userManager.CreateAsync(newIdentityUser, randomPassword);

                        // Set inserted User's role as "User"
                        _userManager.AddToRoleAsync(newIdentityUser, "User").Wait();

                        // Send email for user to set their Identity password
                        string forgotPasswordUrl = $"{Request.Scheme}://{Request.Host.Value}" + "/Identity/Account/ForgotPassword";
                        await _emailSender.SendEmailAsync(newUser.Email, newUser.FirstName + ", your BRTF Room Booking account is ready!",
                            "Hi, " + newUser.FirstName + "!<br /><br />" +
                            "Your BRTF Room Booking account was created with the following username:<br /><br />" +
                            "Username: <b>" + newUser.Username + "</b><br /><br />" +
                            "<b>Please use the <a href='" + forgotPasswordUrl + "'>Forgot Password link</a> to set your password.</b>");

                        insertedCount++;
                    }
                }
                // Generate status message to return and save changes
                uploadMessage = insertedCount + " new Users were created.<br />"
                                + ((duplicateCount > 0) ? duplicateCount + " existing Users were updated.<br />" : "")
                                + ((skippedCount > 0) ? skippedCount + " student datas were skipped (Students whose Term and Program are not in the database get skipped).<br />The following Terms and Programs were skipped:<ul>" : "");
                foreach (string s in skippedPrograms)
                {
                    uploadMessage += "<li>" + s + "</li>";
                }
                uploadMessage += ((skippedCount > 0) ? "</ul>Please add these Terms and Programs if you wish to include them." : "");
                _context.SaveChanges();
                _identityContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetBaseException().Message);
                uploadMessage = "Failed to import data. Check that your selected file is in the correct format.";
            }
            TempData["Message"] = uploadMessage;
            return RedirectToAction("Index");
        }

        public IActionResult DownloadExampleExcel()
        {
            //Create a new spreadsheet from scratch.
            using (ExcelPackage excel = new ExcelPackage())
            {
                var workSheet = excel.Workbook.Worksheets.Add("ImportStudent");

                // Manually write contents as plain string
                var excelContents = new StringBuilder();
                excelContents.AppendLine("ID,First Name,Middle Name,Last Name,DOB,CAMP Email,Acad Plan,Description,Strt Level");
                excelContents.AppendLine("4362670,Cat,Sun,Al-Bahrani,10/1/1990,cstevens@ncstudents.niagaracollege.ca,P0122,Broadcasting: Radio٬ TV & Film,01");
                excelContents.AppendLine("9311011,Sterling,,Archer,8/22/2011,sarcher4@ncstudents.niagaracollege.ca,P0163,Presentation / Radio,01");
                excelContents.AppendLine("9508725,Matt,Elmi Johanna,Parker,2/23/2011,maparker1@ncstudents.niagaracollege.ca,P0164,TV Production,03");
                excelContents.AppendLine("4407393,James,Bullough,Lansing,5/22/2011,jblansing@ncstudents.niagaracollege.ca,P0165,Film Production,05");
                excelContents.AppendLine("4105233,Judy,Ugonna,Garland,8/22/2011,jgarland@ncstudents.niagaracollege.ca,P0198,Acting for TV & Film,02");
                excelContents.AppendLine("4242885,Marge,,Simpson,2/23/2011,masimpson@ncstudents.niagaracollege.ca,P0795,Digital Photography,01");
                excelContents.AppendLine("4035763,Sarah,Wing-Hay,Slean,5/22/2011,sslean13@ncstudents.niagaracollege.ca,P6801,Joint BSc Game Programming,04");
                excelContents.AppendLine("4444312,Morag,Leah-Grace,Smith,8/22/2011,msmith11@ncstudents.niagaracollege.ca,P6800,Joint BA Game Design,06");
                excelContents.AppendLine("4398123,Akira,Kaur,Kurosawa,2/23/2011,akurosawa@ncstudents.niagaracollege.ca,P0441,Game Development,03");
                excelContents.AppendLine("9470695,Zhuo Chang,,Wu,2/23/2011,zcwu@ncstudents.niagaracollege.ca,P0474,CST – Network and Cloud Tech,03");

                // Load Excel contents into Worksheet starting at first cell of Worksheet
                workSheet.Cells["A1"].LoadFromText(excelContents.ToString());

                // Format the DOB column as date
                workSheet.Cells[1, 5, 11, 5].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;

                //Autofit columns
                workSheet.Cells.AutoFitColumns();

                //Download the Excel

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
                        Response.Headers["content-disposition"] = "attachment;  filename=ImportStudent.xlsx";
                        excel.SaveAs(memoryStream);
                        memoryStream.WriteTo(Response.Body);
                    }
                }
                else
                {
                    try
                    {
                        Byte[] theData = excel.GetAsByteArray();
                        string filename = "ImportStudent.xlsx";
                        string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        return File(theData, mimeType, filename);
                    }
                    catch (Exception)
                    {
                        return NotFound();
                    }
                }
            }
            return NotFound();
        }

        /// <summary>
        /// Generates check boxes for User Roles and stores them in ViewData["RoleOptions"]. Roles passed in "selectedRoles" are checked "On" by default.
        /// </summary>
        /// <param name="selectedRoles">Roles to check "On" by default.</param>
        private void PopulateSelectedRoleData(string[] selectedRoles)
        {
            var allOptions = _identityContext.Roles.OrderBy(r => r.Name);
            var currentOptions = selectedRoles.ToHashSet<string>();
            var checkBoxes = new List<CheckOptionVM>();
            foreach (var option in allOptions)
            {
                checkBoxes.Add(new CheckOptionVM
                {
                    DisplayText = option.Name,
                    Assigned = currentOptions.Contains(option.Name)
                });
            }
            ViewData["RoleOptions"] = checkBoxes;
        }

        /// <summary>
        /// Generates check boxes for User Groups and stores them in ViewData["GroupOptions"]. Groups passed in "selectedGroups" are checked "On" by default.
        /// </summary>
        /// <param name="selectedGroups">Groups to check "On" by default.</param>
        private void PopulateSelectedGroupData(string[] selectedGroups)
        {
            var allOptions = _context.UserGroups.OrderBy(g => g.UserGroupName);
            var currentOptions = selectedGroups.ToHashSet<string>();
            var checkBoxes = new List<CheckOptionVM>();
            foreach (var option in allOptions)
            {
                checkBoxes.Add(new CheckOptionVM
                {
                    ID = option.ID,
                    DisplayText = option.UserGroupName,
                    Assigned = currentOptions.Contains(option.UserGroupName)
                });
            }
            ViewData["GroupOptions"] = checkBoxes;
        }

        //This is a twist on the PopulateDropDownLists approach
        //  Create methods that return each SelectList separately
        //  and one method to put them all into ViewData.
        //This approach allows for AJAX requests to refresh
        //DDL Data at a later date.
        private SelectList RoleSelectList(string selectedId = null)
        {
            //string[] validList = { "Admin", "User" };
            //List<string> validRoleList = new List<string>(validList);

            if (User.IsInRole("Admin"))
            {
                return new SelectList(_identityContext.Roles
                    .Where(r => r.Name != "Top-level Admin")
                    .OrderBy(r => r.Name), "Name", "Name", selectedId);
            }
            //return new SelectList(validRoleList, "Name", "Name", selectedId);

            return new SelectList(_identityContext.Roles
                .OrderBy(r => r.Name), "Name", "Name", selectedId);
        }
        private SelectList UserGroupsSelectList(int? selectedId)
        {
            return new SelectList(_context.UserGroups
                .OrderBy(g => g.UserGroupName), "ID", "UserGroupName", selectedId);
        }
        private SelectList TermAndProgramSelectList(int? selectedId)
        {
            if (User.IsInRole("Admin"))
            {
                return new SelectList(_context.TermAndPrograms
                    .Where(u => u.ProgramName != "Top-level Admin")
                    .OrderBy(u => u.ProgramCode).ThenBy(u => u.ProgramLevel), "ID", "TermAndProgramSummary", selectedId);
            }

            return new SelectList(_context.TermAndPrograms
                .OrderBy(u => u.ProgramCode).ThenBy(u => u.ProgramLevel), "ID", "TermAndProgramSummary", selectedId);
        }
        private void PopulateDropDownLists(User user = null)
        {
            ViewData["TermAndProgramID"] = TermAndProgramSelectList(user?.TermAndProgramID);
            ViewData["UserGroupID"] = UserGroupsSelectList(user?.TermAndProgram.UserGroupID);
        }

        private string ControllerName()
        {
            return this.ControllerContext.RouteData.Values["controller"].ToString();
        }

        private void ViewDataReturnURL()
        {
            ViewData["returnURL"] = MaintainURL.ReturnURL(HttpContext, ControllerName());
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.ID == id);
        }
    }
}
