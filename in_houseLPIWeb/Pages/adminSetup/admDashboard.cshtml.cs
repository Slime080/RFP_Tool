using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using in_houseLPIWeb.Data;
using in_houseLPIWeb.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace in_houseLPIWeb.Pages.adminSetup
{
    [Authorize]
    public class admDashboardModel : PageModel
    {
        private readonly WebDbContext _db;
        public string userRole { get; set; }
        public bool storeIndexAccess { get; set; }
        public bool deptIndexAccess { get; set; }
        public bool userIndexAcces { get; set; }
        public admDashboardModel(WebDbContext db)
        {
            _db = db;
        }

        public IActionResult OnGet()
        {
            var userInfo = _db.Users.Where(u => u.Name == User.Identity.Name).Select(u => new { Id = u.Id, isActive = u.isActive }).FirstOrDefault();

            var hasPermission = _db.Permissions.Any(p => p.UserId == userInfo.Id && p.Util_Dash == true);
            if (!hasPermission || !userInfo.isActive)
            {
                return RedirectToPage("/userLogin/userDenied");
            }

            userRole = _db.Users
                .Where(u => u.Name == User.Identity.Name)
                .Select(u => u.UserLevel)
                .FirstOrDefault();

            // This sets if buttons are accessible
            storeIndexAccess = PagePermission.HasAccess(_db, User.Identity.Name, "Util_Store_Index");
            deptIndexAccess = PagePermission.HasAccess(_db, User.Identity.Name, "Util_UserDep_Index");
            userIndexAcces = PagePermission.HasAccess(_db, User.Identity.Name, "Util_User_Index");

            return null;
        }
    }
}
