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
using Microsoft.Extensions.Logging;

namespace in_houseLPIWeb.Pages.adminSetup
{
    [Authorize]
    [BindProperties]
    public class admStore_EditModel : PageModel
    {
        private readonly WebDbContext _db;
        private readonly AuditChanges _auditChanges;////// Changes
        private readonly ILogger<admStore_EditModel> _logger;

        public Store Stores { get; set; }
        public IEnumerable<StoreType> StoreTypes { get; set; }
        public List<StoreType> StoreTypeList { get; set; }
        public admStore_EditModel(WebDbContext db, ILogger<admStore_EditModel> logger, AuditChanges auditChanges)////// Changes
        {
            _db = db;
            _logger = logger;
            StoreTypeList = _db.StoreTypes.ToList();
            _auditChanges = auditChanges;////// Changes
        }
        public string userDept { get; set; }
        public IActionResult OnGet(int id)
        {
            var userInfo = _db.Users.Where(u => u.Name == User.Identity.Name).Select(u => new { Id = u.Id, isActive = u.isActive }).FirstOrDefault();

            var hasPermission = _db.Permissions.Any(p => p.UserId == userInfo.Id && p.Util_Store_Edit == true);
            if (!hasPermission || !userInfo.isActive)
            {
                return RedirectToPage("/userLogin/userDenied");
            }

            // Below are the regular functions required to load the page

            Stores = _db.Stores.Find(id) ?? new Store();

            userDept = _db.Users
                .Where(e => e.Name == User.Identity.Name)
                .Select(e => e.Department)
                .FirstOrDefault();

            return null;
        }
        public async Task<IActionResult> OnPost(Store Stores)
        {
            var uRole = _db.Users
                .Where(u => u.Name == User.Identity.Name)
                .Select(u => u.UserLevel)
                .FirstOrDefault();

            if ((uRole.Contains($"{GetDepartmentId("ifc")}") && Stores.Entity == "1003") ||
                (!uRole.Contains($"{GetDepartmentId("ifc")}") && Stores.Entity != "1003") ||
                (uRole == "0"))
            {
                string NameInput = Request.Form["storeNameInput"];
                Stores.StoreName = Request.Form["Stores.StoreName"];

                if (string.IsNullOrEmpty(NameInput))
                {
                    ModelState.AddModelError("Stores.StoreName", "Store Name field is required, it cannot be empty.");
                }
                if (!ModelState.IsValid)
                {
                    string errorMessage = ModelState.Values.FirstOrDefault(v => v.Errors.Any())?.Errors.FirstOrDefault()?.ErrorMessage;
                    TempData["Error"] = errorMessage ?? "Failed to edit type of charge.";
                }

                if (ModelState.IsValid)
                {
                    ModelState.Clear();
                    var existing = _db.Stores.Find(Stores.Id);
                    existing.StoreName = Stores.StoreName;
                    existing.OpenDate = Stores.OpenDate;
                    existing.CloseDate = Stores.CloseDate;
                    existing.isSDWAN = Stores.isSDWAN;
                    existing.MRC = Stores.MRC;
                    existing.StoreType = Stores.StoreType;
                    existing.ContractCode = Stores.ContractCode;
                    existing.BusinessName = Stores.BusinessName;
                    existing.ContractName = Stores.ContractName;

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

                    // Update Changes
                    //_db.Stores.Update(Stores);
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

                    await _auditChanges.AddUpdateAuditAsync(_db, existing.Id, nameof(Store), changes, User.Identity.Name);

                    #endregion

                    TempData.Remove("Error");
                    TempData["Success"] = "Store detail edited successfully.";
                    return RedirectToPage("/adminSetup/admStore_Index");
                }// will be used to validate the input

                return Page();
            }
            else
            {
                TempData["Error"] = "Warming: Not authorized to perform this action.";
            }

            return RedirectToPage("/adminSetup/admStore_Index");
        }

        public JsonResult OnGetEntityInfo(string entityValue)
        {
            var entityInfo = _db.Entities
                .Where(e => e.EntityCode == entityValue && e.IsActive == true)
                .Select(e => new { EntityCode = e.EntityCode, EntityName = e.EntityName })
                .FirstOrDefault();

            if (entityInfo != null)
            {
                return new JsonResult(entityInfo);
            }

            return new JsonResult(null);
        }

        public JsonResult OnGetStoreFromDb(string name)
        {

            // Handle the change event on the server side
            var storeDetail = _db.Stores
                .Where(p => p.StoreName == name && p.IsOpen == true)
                .Select(p => new { StoreCode = p.StoreCode, StoreName = p.StoreName, Entity = p.Entity, StoreType = p.StoreType, ContractCode = p.ContractCode, BusinessName = p.BusinessName, ContractName = p.ContractName, IsOpen = p.IsOpen })
                .FirstOrDefault();

            if (storeDetail != null)
            {
                return new JsonResult(storeDetail);
            }

            return new JsonResult(null);
        }

        private int GetDepartmentId(string departmentName)
        {
            return _db.Departments
                .Where(d => d.DeptName.ToLower() == departmentName)
                .Select(d => d.Id)
                .FirstOrDefault();
        }
    }
}
