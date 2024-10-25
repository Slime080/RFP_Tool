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
    public class admStore_ActivateModel : PageModel
    {
        private readonly WebDbContext _db;

        private readonly ILogger<admStore_ActivateModel> _logger;

        public Store Stores { get; set; }
        public admStore_ActivateModel(WebDbContext db, ILogger<admStore_ActivateModel> logger)
        {
            _db = db;
            _logger = logger;
        }
        public string userDept { get; set; }
        
        public IActionResult OnGet(int id)
        {
            var userInfo = _db.Users.Where(u => u.Name == User.Identity.Name).Select(u => new { Id = u.Id, isActive = u.isActive }).FirstOrDefault();

            var hasPermission = _db.Permissions.Any(p => p.UserId == userInfo.Id && p.Util_Store_Actv == true);
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
        public async Task<IActionResult> OnPost()
        {
            var StoreDetailFromDb = _db.Stores.Find(Stores.Id);
            _logger.LogInformation($"StoreDetailFromDb found: {StoreDetailFromDb}");

            if (StoreDetailFromDb == null)
            {
                TempData["Error"] = "Invalid Store detail selected.";
                return RedirectToPage("/adminSetup/admStore_Index");
            }

            if (StoreDetailFromDb.IsOpen != true)
            {
                StoreDetailFromDb.IsOpen = true;
                await _db.SaveChangesAsync();
                TempData["Success"] = "Store detail re-activated successfully.";
            }
            else
            {
                TempData["Error"] = "Failed to re-activate store details.";
                return Page();
            }

            return RedirectToPage("/adminSetup/admStore_Index");
        }
    }
}
