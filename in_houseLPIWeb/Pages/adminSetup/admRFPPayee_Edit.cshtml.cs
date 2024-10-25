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
using Microsoft.EntityFrameworkCore;////// Changes

namespace in_houseLPIWeb.Pages.adminSetup
{
    [Authorize]
    [BindProperties]
    public class admRFPPayee_EditModel : PageModel
    {
        private readonly WebDbContext _db;
        private readonly AuditChanges _auditChanges;////// Changes
        public Payee Payees { get; set; }
        public admRFPPayee_EditModel(WebDbContext db, AuditChanges auditChanges)////// Changes
        {
            _db = db;
            _auditChanges = auditChanges;////// Changes
        }
        public IActionResult OnGet(int id)
        {
            var userInfo = _db.Users.Where(u => u.Name == User.Identity.Name).Select(u => new { Id = u.Id, isActive = u.isActive }).FirstOrDefault();

            var hasPermission = _db.Permissions.Any(p => p.UserId == userInfo.Id && p.RFPutil_Payee_Edit == true);
            if (!hasPermission || !userInfo.isActive)
            {
                return RedirectToPage("/userLogin/userDenied");
            }

            // Below are the regular functions required to load the page

            Payees = _db.Payees.Find(id);

            return null;
        }
        public async Task<IActionResult> OnPost(Payee Payees)
        {
            if (!ModelState.IsValid)
            {
                string errorMessage = ModelState.Values.FirstOrDefault(v => v.Errors.Any())?.Errors.FirstOrDefault()?.ErrorMessage;
                TempData["Error"] = errorMessage ?? "Failed to edit payee details.";
            }

            if (ModelState.IsValid)
            {
                ModelState.Clear();
                var existing = _db.Payees.Find(Payees.Id);
                existing.PayeeAddress = Payees.PayeeAddress;
                existing.IsActive = Payees.IsActive;


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

                await _auditChanges.AddUpdateAuditAsync(_db, existing.Id, nameof(Payee), changes, User.Identity.Name);

                #endregion

                TempData.Remove("Error");
                TempData["Success"] = "Payee detail edited successfully.";
                return RedirectToPage("/adminSetup/admRFPPayee_Index");
            }// will be used to validate the input
            return Page();
        }
    }
}
