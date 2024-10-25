using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using in_houseLPIWeb.Data;
using in_houseLPIWeb.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using in_houseLPIWeb.Utilities;

namespace in_houseLPIWeb.Pages.adminSetup
{
    [Authorize]
    [BindProperties]
    public class admStore_IndexModel : PageModel
    {
        private readonly WebDbContext _db;
        private readonly ILogger<admStore_IndexModel> _logger;
        private readonly AuditChanges _auditChanges;////// Changes
        public List<SelectListItem> cbEntities { get; set; }
        public List<SelectListItem> cbStoreType { get; set; }
        public IEnumerable<Entity> Entities { get; set; }
        public IEnumerable<Store> StoreList { get; set; }
        public IEnumerable<StoreType> StoreTypes { get; set; }
        public Store Stores { get; set; }
        public string userRole { get; set; }

        public bool stoEditAccess { get; set; }
        public bool sCloseAccess { get; set; }
        public bool sOpenAccess { get; set; }
        public bool entEditAccess { get; set; }
        public bool typeEditAccess { get; set; }
        public admStore_IndexModel(WebDbContext db, ILogger<admStore_IndexModel> logger, AuditChanges auditChanges)////// Changes
        {
            _db = db;
            _logger = logger;
            _auditChanges = auditChanges;////// Changes
        }

        public IActionResult OnGet()
        {
            var userInfo = _db.Users.Where(u => u.Name == User.Identity.Name).Select(u => new { Id = u.Id, isActive = u.isActive }).FirstOrDefault();

            var hasPermission = _db.Permissions.Any(p => p.UserId == userInfo.Id && p.Util_Store_Index == true);
            if (!hasPermission || !userInfo.isActive)
            {
                return RedirectToPage("/userLogin/userDenied");
            }

            // Below are the regular functions required to load the page

            StoreList = _db.Stores;

            cbEntities = GetEntities();
            cbStoreType = GetStoreTypes();

            userRole = _db.Users
                .Where(u => u.Name == User.Identity.Name)
                .Select(u => u.UserLevel)
                .FirstOrDefault();
            var uDept = _db.Users
                .Where(u => u.Name == User.Identity.Name)
                .Select(u => u.Department)
                .FirstOrDefault()
                .ToLower();

            // ========== FOR PAGE ACCESS PURPOSES =========

            stoEditAccess = PagePermission.HasAccess(_db, User.Identity.Name, "Util_Store_Edit");
            sCloseAccess = PagePermission.HasAccess(_db, User.Identity.Name, "Util_Store_Archive");
            sOpenAccess = PagePermission.HasAccess(_db, User.Identity.Name, "Util_Store_Actv");

            entEditAccess = PagePermission.HasAccess(_db, User.Identity.Name, "Util_StoreEnt_Edit");
            typeEditAccess = PagePermission.HasAccess(_db, User.Identity.Name, "Util_StoreType_Edit");

            return null;
        }

        public async Task<IActionResult> OnPostAddStoreAsync()
        {
            string NameInput = Request.Form["storeNameInput"];
            _logger.LogInformation("Name Input: " + NameInput);
            Stores.StoreName = Request.Form["Stores.StoreName"];
            Stores.isSDWAN = false;
            Stores.MRC = 0;

            if (!ModelState.IsValid)
            {
                foreach (var key in ModelState.Keys)
                {
                    foreach (var error in ModelState[key].Errors)
                    {
                        _logger.LogInformation($"{key}: {error.ErrorMessage}");
                    }
                }
            }

            if (string.IsNullOrEmpty(NameInput))
            {
                ModelState.AddModelError("Stores.StoreName", "Store Name field is required, it cannot be empty.");
            }
            if (StoreCodeExists(Stores.StoreCode))
            {
                ModelState.AddModelError("Stores.StoreCode", "The Store Code is already existing and cannot be duplicated.");
            }

            if (ModelState.IsValid)
            {
                await _db.Stores.AddAsync(Stores); // Use the parameter 'store' instead of 'Stores'
                await _db.SaveChangesAsync();

                #region Start of Changes for logging newly added data
                await _auditChanges.AddCreateAuditAsync(_db, Stores.Id, nameof(Store), User.Identity.Name);
                #endregion

                TempData["Success"] = "Store details added successfully.";
                return RedirectToPage("/adminSetup/admStore_Index");
            }

            string errorMessage = ModelState.Values.FirstOrDefault(v => v.Errors.Any())?.Errors.FirstOrDefault()?.ErrorMessage;
            TempData["Error"] = errorMessage ?? "Failed to add store details.";
            
            // Update table and dropdown
            StoreList = _db.Stores.ToList();
            cbEntities = GetEntities();
            cbStoreType = GetStoreTypes();

            return Page();
        }

        private bool StoreCodeExists(string storeCode)
        {
            return _db.Stores.Any(s => s.StoreCode == storeCode);
        }

        public JsonResult OnGetEntityInfo(string entityValue)
        {
            //_logger.LogInformation($"OnGetEntityInfo called with entityValue: {entityValue}");

            var entityInfo = _db.Entities
                .Where(e => e.EntityCode == entityValue && e.IsActive == true)
                .Select(e => new { EntityCode = e.EntityCode, EntityName = e.EntityName })
                .FirstOrDefault();

            if (entityInfo != null)
            {
                //_logger.LogInformation($"EntityInfo found: {entityInfo.EntityCode} - {entityInfo.EntityName}");
                return new JsonResult(entityInfo);
            }

            //_logger.LogInformation($"EntityInfo not found for entityValue: {entityValue}");
            return new JsonResult(null);
        }

        private List<SelectListItem> GetEntities()
        {
            var entities = _db.Entities.Where(e => e.IsActive == true).ToList();

            return entities.Select(d => new SelectListItem { Text = d.EntityCode }).ToList();
        }

        private List<SelectListItem> GetStoreTypes()
        {
            var storeTypes = _db.StoreTypes.Where(e => e.IsActive == true).ToList();

            return storeTypes.Select(d => new SelectListItem { Text = d.TypeCode + " - " + d.TypeName, Value = d.TypeCode }).ToList();
        }

        public int GetDepartmentId(string departmentName)
        {
            return _db.Departments
                .Where(d => d.DeptName.ToLower() == departmentName)
                .Select(d => d.Id)
                .FirstOrDefault();
        }
    }
}
