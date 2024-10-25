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

namespace in_houseLPIWeb.Pages
{
    [Authorize]
    public class ArchiveModel : PageModel
    {
        private readonly WebDbContext _db;
        public IEnumerable<rfpForm> RFPList { get; set; }
        public IEnumerable<Department> DeptList { get; set; }
        public IEnumerable<Payee> PayeeList { get; set; }
        public IEnumerable<TOC> ChargeList { get; set; }
        public IEnumerable<Store> StoreList { get; set; }
        public IEnumerable<Entity> EntityList { get; set; }
        public IEnumerable<StoreType> TypeList { get; set; }
        public bool rfpActvtAccess { get; set; }
        public ArchiveModel(WebDbContext db)
        {
            _db = db;
        }

        public IActionResult OnGet()
        {
            var userInfo = _db.Users.Where(u => u.Name == User.Identity.Name).Select(u => new { Id = u.Id, isActive = u.isActive }).FirstOrDefault();

            var hasPermission = _db.Permissions.Any(p => p.UserId == userInfo.Id && p.Archive == true);
            if (!hasPermission || !userInfo.isActive)
            {
                return RedirectToPage("/userLogin/userDenied");
            }

            // Below are the regular functions required to load the page

            RFPList = _db.rfpForms ?? Enumerable.Empty<rfpForm>();
            DeptList = _db.Departments ?? Enumerable.Empty<Department>();
            PayeeList = _db.Payees ?? Enumerable.Empty<Payee>();
            ChargeList = _db.TOCs ?? Enumerable.Empty<TOC>();
            StoreList = _db.Stores ?? Enumerable.Empty<Store>();
            EntityList = _db.Entities ?? Enumerable.Empty<Entity>();
            TypeList = _db.StoreTypes ?? Enumerable.Empty<StoreType>();

            rfpActvtAccess = PagePermission.HasAccess(_db, User.Identity.Name, "RFP_Activate");

            return null;
        }
    }
}
