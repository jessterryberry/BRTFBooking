using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BRTF_Room_Booking_App.Data;
using BRTF_Room_Booking_App.Models;
using Microsoft.AspNetCore.Authorization;

namespace BRTF_Room_Booking_App.Controllers
{[Authorize(Roles ="Top-level Admin")]
    public class GlobalSettingsController : Controller
    {
        private readonly BTRFRoomBookingContext _context;

        public GlobalSettingsController(BTRFRoomBookingContext context)
        {
            _context = context;
        }

        //// GET: GlobalSettings
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.GlobalSettings.ToListAsync());
        //}

        // GET: GlobalSettings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var globalSetting = await _context.GlobalSettings
                .FirstOrDefaultAsync(m => m.ID == id);
            if (globalSetting == null)
            {
                return NotFound();
            }

            return View(globalSetting);
        }

        //// GET: GlobalSettings/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: GlobalSettings/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("ID,StartOfTermDate,EndOfTermDate,LatestAllowableFutureBookingDay,EmailBookingNotificationsOverride,PreventBookingNotificationsOverride,EmailCancelNotificationsOverride,PreventCancelNotificationsOverride")] GlobalSetting globalSetting)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(globalSetting);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(globalSetting);
        //}

        // GET: GlobalSettings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var globalSetting = await _context.GlobalSettings.FindAsync(id);
            if (globalSetting == null)
            {
                return NotFound();
            }
            return View(globalSetting);
        }

        // POST: GlobalSettings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            var gsToUpdate = await _context.GlobalSettings.FirstOrDefaultAsync(p => p.ID == id);

            if (gsToUpdate == null)
            {
                return NotFound();
            }

            //Trying to update
            if (await TryUpdateModelAsync<GlobalSetting>(gsToUpdate, "", p => p.StartOfTermDate, p => p.EndOfTermDate,
                p => p.LatestAllowableFutureBookingDay, p => p.EmailBookingNotificationsOverride,
                p => p.PreventBookingNotificationsOverride, p => p.EmailCancelNotificationsOverride,
                p => p.PreventCancelNotificationsOverride))
            {
                try
                {
                    if (gsToUpdate.LatestAllowableFutureBookingDay < 0)
                    {
                        ModelState.AddModelError("", "Latest Allowable Future Booking Day cannot be negative. Please enter a new number.");
                        throw new DbUpdateException("Error updating Global Settings.");
                    }
                    await _context.SaveChangesAsync();

                    var bookings = _context.RoomBookings;
                    foreach(RoomBooking r in bookings)
                    {
                        if (r.StartDate < gsToUpdate.StartOfTermDate || r.StartDate > gsToUpdate.EndOfTermDate || 
                            r.EndDate < gsToUpdate.StartOfTermDate || r.EndDate > gsToUpdate.EndOfTermDate)
                        {
                            _context.RoomBookings.Remove(r);
                        }
                        await _context.SaveChangesAsync();
                    }
                    TempData["AlertMessage"] = "Global Settings have been updated!";
                    return RedirectToAction("Details", new { gsToUpdate.ID });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GlobalSettingExists(gsToUpdate.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(gsToUpdate);
        }

        //// GET: GlobalSettings/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var globalSetting = await _context.GlobalSettings
        //        .FirstOrDefaultAsync(m => m.ID == id);
        //    if (globalSetting == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(globalSetting);
        //}

        //// POST: GlobalSettings/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var globalSetting = await _context.GlobalSettings.FindAsync(id);
        //    _context.GlobalSettings.Remove(globalSetting);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool GlobalSettingExists(int id)
        {
            return _context.GlobalSettings.Any(e => e.ID == id);
        }
    }
}
