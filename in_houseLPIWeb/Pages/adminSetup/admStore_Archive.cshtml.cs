using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using in_houseLPIWeb.Data;
using in_houseLPIWeb.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace in_houseLPIWeb.Pages.adminSetup
{
    [Authorize]
    [BindProperties]
    public class admStore_ArchiveModel : PageModel
    {
        private readonly WebDbContext _db;

        private readonly ILogger<admStore_ArchiveModel> _logger;

        public Store Stores { get; set; }
        public IEnumerable<StoreType> StoreTypes { get; set; }
        public List<StoreType> StoreTypeList { get; set; }
        public admStore_ArchiveModel(WebDbContext db, ILogger<admStore_ArchiveModel> logger)
        {
            _db = db;
            _logger = logger;
            StoreTypeList = _db.StoreTypes.ToList();
        }
        public string userDept { get; set; }

        public IActionResult OnGet(int id)
        {
            var userInfo = _db.Users.Where(u => u.Name == User.Identity.Name).Select(u => new { Id = u.Id, isActive = u.isActive }).FirstOrDefault();

            var hasPermission = _db.Permissions.Any(p => p.UserId == userInfo.Id && p.Util_Store_Archive == true);
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

            if ( (uRole.Contains($"{GetDepartmentId("ifc")}") && Stores.Entity == "1003") || 
                (!uRole.Contains($"{GetDepartmentId("ifc")}") && Stores.Entity != "1003") ||
                (uRole == "0") )
            {
                var StoreDetailFromDb = _db.Stores.Find(Stores.Id);
                
                if (StoreDetailFromDb == null)
                {
                    TempData["Error"] = "Invalid Store detail selected.";
                    return RedirectToPage("/adminSetup/admStore_Index");
                }

                if (StoreDetailFromDb.IsOpen)
                {
                    if (Stores.CloseDate != null)
                    {
                        StoreDetailFromDb.CloseDate = Stores.CloseDate;
                        StoreDetailFromDb.IsOpen = false;
                        await _db.SaveChangesAsync();
                        TempData["Success"] = "Store close detail updated successfully.";
                    }
                    else
                    {
                        TempData["Error"] = "Please input the close date.";
                        return Page();
                    }
                    
                }
                else
                {
                    TempData["Error"] = "Failed to update store close details.";
                    return Page();
                }
            }
            else
            {
                TempData["Error"] = "Warming: Not authorized to perform this action.";
            }

            return RedirectToPage("/adminSetup/admStore_Index");
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
