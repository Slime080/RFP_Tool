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
    public class admRFPUtil_DashboardModel : PageModel
    {
        private readonly WebDbContext _db;
        public string userRole { get; set; }
        public bool payeeIndexAccess { get; set; }
        public bool tocIndexAccess { get; set; }
        public admRFPUtil_DashboardModel(WebDbContext db)
        {
            _db = db;
        }

        public IActionResult OnGet()
        {
            var userInfo = _db.Users.Where(u => u.Name == User.Identity.Name).Select(u => new { Id = u.Id, isActive = u.isActive }).FirstOrDefault();

            var hasPermission = _db.Permissions.Any(p => p.UserId == userInfo.Id && p.RFPutil_Dash == true);
            if (!hasPermission || !userInfo.isActive)
            {
                return RedirectToPage("/userLogin/userDenied");
            }

            // Below are the regular functions required to load the page

            userRole = _db.Users
                .Where(u => u.Name == User.Identity.Name)
                .Select(u => u.UserLevel)
                .FirstOrDefault();

            // This sets if buttons are accessible
            payeeIndexAccess = PagePermission.HasAccess(_db, User.Identity.Name, "RFPutil_Payee_Index");
            tocIndexAccess = PagePermission.HasAccess(_db, User.Identity.Name, "RFPutil_TOC_Index");

            return null;
        }
    }
}
