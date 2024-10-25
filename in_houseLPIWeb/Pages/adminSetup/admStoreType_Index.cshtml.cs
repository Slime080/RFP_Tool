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
    public class admStoreType_IndexModel : PageModel
    {
        private readonly WebDbContext _db;
        private readonly AuditChanges _auditChanges;////// Changes
        public StoreType StoreTypes { get; set; }
        public IEnumerable<StoreType> TypeList { get; set; }
        public string userRole { get; set; }
        public bool entEditAccess { get; set; }
        public bool typeEditAccess { get; set; }
        public bool admin { get; set; }
        public admStoreType_IndexModel(WebDbContext db, AuditChanges auditChanges)////// Changes
        {
            _db = db;
            _auditChanges = auditChanges;////// Changes
        }

        public IActionResult OnGet()
        {
            var userInfo = _db.Users.Where(u => u.Name == User.Identity.Name).Select(u => new { Id = u.Id, isActive = u.isActive }).FirstOrDefault();

            var hasPermission = _db.Permissions.Any(p => p.UserId == userInfo.Id && p.Util_StoreType_Index == true);
            if (!hasPermission || !userInfo.isActive)
            {
                return RedirectToPage("/userLogin/userDenied");
            }

            // Below are the regular functions required to load the page

            TypeList = _db.StoreTypes;

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

        public async Task<IActionResult> OnPost(StoreType StoreTypes)
        {
            if (TypecodeExists(StoreTypes.TypeCode) || TypenameExists(StoreTypes.TypeName))
            {
                ModelState.AddModelError("StoreTypes.TypeCode", "The Store Type Code field is already existing and cannot be duplicated.");
                ModelState.AddModelError("StoreTypes.TypeName", "The Store Type Name field is already existing and cannot be duplicated.");
            }

            if (ModelState.IsValid)
            {
                StoreTypes.TypeCode = StoreTypes.TypeCode.ToUpper(); // To maintain the letter case for dropdown

                await _db.StoreTypes.AddAsync(StoreTypes);
                await _db.SaveChangesAsync();

                #region Start of Changes for logging newly added data
                await _auditChanges.AddCreateAuditAsync(_db, StoreTypes.Id, nameof(StoreType), User.Identity.Name);
                #endregion

                TempData["Success"] = "Store Type details added successfully.";
                return RedirectToPage("/adminSetup/admStoreType_Index");
            }
            string errorMessage = ModelState.Values.FirstOrDefault(v => v.Errors.Any())?.Errors.FirstOrDefault()?.ErrorMessage;
            TempData["Error"] = errorMessage ?? "Failed to add store type details.";
            return RedirectToPage("/adminSetup/admStoreType_Index");
        }
        private bool TypecodeExists(string Typecode)
        {
            // Perform a query to check if the StoreNumber already exists in the database
            return _db.StoreTypes.Any(s => s.TypeCode == Typecode);
        }

        private bool TypenameExists(string Typename)
        {
            // Perform a query to check if the StoreNumber already exists in the database
            return _db.StoreTypes.Any(s => s.TypeName == Typename);
        }
    }
}
