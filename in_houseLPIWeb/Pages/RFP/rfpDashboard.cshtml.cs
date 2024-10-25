using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using in_houseLPIWeb.Data;
using in_houseLPIWeb.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace in_houseLPIWeb.Pages.RFP
{
    [Authorize]
    public class rfpDashboardModel : PageModel
    {
        private readonly WebDbContext _db;
        public bool rfpIndexAccess { get; set; }
        public bool rfpAddAccess { get; set; }

        public rfpDashboardModel(WebDbContext db)
        {
            _db = db;
        }
        //public void OnGet()
        //{
        //}
        public IActionResult OnGet()
        {
            var userInfo = _db.Users.Where(u => u.Name == User.Identity.Name).Select(u => new { Id = u.Id, isActive = u.isActive }).FirstOrDefault();

            var hasPermission = _db.Permissions.Any(p => p.UserId == userInfo.Id && p.RFP_Dash == true);
            if (!hasPermission || !userInfo.isActive)
            {
                return RedirectToPage("/userLogin/userDenied");
            }

            // This sets if buttons are accessible
            rfpIndexAccess = PagePermission.HasAccess(_db, User.Identity.Name, "RFP_Index");
            rfpAddAccess = PagePermission.HasAccess(_db, User.Identity.Name, "RFP_Add");

            return null;
        }
    }
}
