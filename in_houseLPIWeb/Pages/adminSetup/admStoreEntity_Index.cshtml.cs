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

namespace in_houseLPIWeb.Pages.adminSetup
{
    [Authorize]
    [BindProperties]
    public class admStoreEntity_IndexModel : PageModel
    {
        private readonly WebDbContext _db;

        private readonly AuditChanges _auditChanges;////// Changes
        public Entity Entities { get; set; }
        public IEnumerable<Entity> EntityList { get; set; }
        public string userRole { get; set; }
        public bool entEditAccess { get; set; }
        public bool typeEditAccess { get; set; }
        public bool admin { get; set; }
        public admStoreEntity_IndexModel(WebDbContext db, AuditChanges auditChanges)////// Changes
        {
            _db = db;
            _auditChanges = auditChanges;////// Changes
        }

        public IActionResult OnGet()
        {
            var userInfo = _db.Users.Where(u => u.Name == User.Identity.Name).Select(u => new { Id = u.Id, isActive = u.isActive }).FirstOrDefault();

            var hasPermission = _db.Permissions.Any(p => p.UserId == userInfo.Id && p.Util_StoreEnt_Edit == true);
            if (!hasPermission || !userInfo.isActive)
            {
                return RedirectToPage("/userLogin/userDenied");
            }

            // Below are the regular functions required to load the page

            EntityList = _db.Entities;

            userRole = _db.Users
                .Where(u => u.Name == User.Identity.Name)
                .Select(u => u.UserLevel)
                .FirstOrDefault();

            if (userRole == "0")
            {
                admin = true;
            }

            entEditAccess = PagePermission.HasAccess(_db, User.Identity.Name, "Util_StoreEnt_Edit");
            typeEditAccess = PagePermission.HasAccess(_db, User.Identity.Name, "Util_StoreType_Edit");

            return null;
        }

        public async Task<IActionResult> OnPost(Entity Entities)
        {
            if (string.IsNullOrEmpty(Entities.EntityName))
            {
                ModelState.AddModelError("Entities.EntityName", "The Entity field is required.");
            }
            if (EntitycodeExists(Entities.EntityCode) || EntitynameExists(Entities.EntityName))
            {
                ModelState.AddModelError("Entities.EntityCode", "The Entity Code field is already existing and cannot be duplicated.");
                ModelState.AddModelError("Entities.EntityName", "The Entity Name field is already existing and cannot be duplicated.");
            }

            if (ModelState.IsValid)
            {
                Entities.EntityName = Entities.EntityName.ToUpper(); // To maintain the letter case for dropdown

                await _db.Entities.AddAsync(Entities);
                await _db.SaveChangesAsync();

                #region Start of Changes for logging newly added data
                await _auditChanges.AddCreateAuditAsync(_db, Entities.Id, nameof(Entity), User.Identity.Name);
                #endregion

                TempData["Success"] = "Entity details added successfully.";
                return RedirectToPage("/adminSetup/admStoreEntity_Index");
            }
            string errorMessage = ModelState.Values.FirstOrDefault(v => v.Errors.Any())?.Errors.FirstOrDefault()?.ErrorMessage;
            TempData["Error"] = errorMessage ?? "Failed to add Entity details.";
            return RedirectToPage("/adminSetup/admStoreEntity_Index");
        }
        private bool EntitycodeExists(string Entitycode)
        {
            // Perform a query to check if the StoreNumber already exists in the database
            return _db.Entities.Any(s => s.EntityCode == Entitycode);
        }

        private bool EntitynameExists(string Entityname)
        {
            // Perform a query to check if the StoreNumber already exists in the database
            return _db.Entities.Any(s => s.EntityName == Entityname);
        }
    }
}
