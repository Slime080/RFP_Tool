using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using in_houseLPIWeb.Data;
using in_houseLPIWeb.Model;
using in_houseLPIWeb.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace in_houseLPIWeb.Pages.adminSetup
{
    [Authorize]
    [BindProperties]
    public class admStoreType_EditModel : PageModel
    {
        private readonly WebDbContext _db;
        private readonly AuditChanges _auditChanges;////// Changes
        public StoreType StoreTypes { get; set; }
        public admStoreType_EditModel(WebDbContext db, AuditChanges auditChanges)////// Changes
        {
            _db = db;
            _auditChanges = auditChanges;////// Changes
        }
        public IActionResult OnGet(int id)
        {
            var userInfo = _db.Users.Where(u => u.Name == User.Identity.Name).Select(u => new { Id = u.Id, isActive = u.isActive }).FirstOrDefault();

            var hasPermission = _db.Permissions.Any(p => p.UserId == userInfo.Id && p.Util_StoreType_Edit == true);
            if (!hasPermission || !userInfo.isActive)
            {
                return RedirectToPage("/userLogin/userDenied");
            }

            // Below are the regular functions required to load the page

            StoreTypes = _db.StoreTypes.Find(id);

            return null;
        }
        public async Task<IActionResult> OnPost(StoreType StoreTypes)
        {
            
            if (!ModelState.IsValid)
            {
                string errorMessage = ModelState.Values.FirstOrDefault(v => v.Errors.Any())?.Errors.FirstOrDefault()?.ErrorMessage;
                TempData["Error"] = errorMessage ?? "Failed to update store type.";
            }

            if (ModelState.IsValid)
            {
                ModelState.Clear();
                var existing = _db.StoreTypes.Find(StoreTypes.Id);
                existing.TypeDescription = StoreTypes.TypeDescription;
                existing.IsActive = StoreTypes.IsActive;

                #region Changes starts here for OnPost

                var entry = _db.Entry(existing);
                var changes = new Dictionary<string, (string OldValue, string NewValue)>();

                // Collecting changes for existing records
                if (entry.State == EntityState.Modified)
                {
                    foreach (var prop in entry.Properties)
                    {
                        if (prop.IsModified)
                        {
                            changes.Add(prop.Metadata.Name, (prop.OriginalValue?.ToString(), prop.CurrentValue?.ToString()));
                        }
                    }
                }

                // Save the changes
                await _db.SaveChangesAsync();

                // For new entries, we log the current state as new values
                if (entry.State == EntityState.Added)
                {
                    foreach (var prop in entry.Properties)
                    {
                        changes.Add(prop.Metadata.Name, (null, prop.CurrentValue?.ToString()));
                    }
                }

                await _auditChanges.AddUpdateAuditAsync(_db, existing.Id, nameof(StoreType), changes, User.Identity.Name);

                #endregion

                TempData.Remove("Error");
                TempData["Success"] = "Store Type updated successfully.";
                return RedirectToPage("/adminSetup/admStoreType_Index");
            }// will be used to validate the input
            return Page();
        }
    }
}
