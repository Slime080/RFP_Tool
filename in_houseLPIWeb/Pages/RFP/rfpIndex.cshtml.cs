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

namespace in_houseLPIWeb.Pages.RFP
{
    [Authorize]
    [BindProperties]
    public class rfpIndexModel : PageModel
    {
        private readonly WebDbContext _db;
        private readonly ILogger<rfpIndexModel> _logger;
        public IEnumerable<rfpForm> rfpForms { get; set; }
        public Dictionary<string, decimal> Sums { get; set; }
        public Dictionary<string, PoPList> PoPDetails { get; set; }
        public rfpForm rfpFormx { get; set; }
        public string userRole { get; set; }
        public bool rfpAddAccess { get; set; }
        public bool rfpViewAccess { get; set; }
        public bool rfpArchiveAccess { get; set; }
        public bool rfpActvtAccess { get; set; }

        public rfpIndexModel(WebDbContext db, ILogger<rfpIndexModel> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var userInfo = _db.Users
                .Where(u => u.Name == User.Identity.Name)
                .Select(u => new { Id = u.Id, isActive = u.isActive })
                .FirstOrDefault();

            var hasPermission = _db.Permissions.Any(p => p.UserId == userInfo.Id && p.RFP_Index == true);
            if (!hasPermission || !userInfo.isActive)
            {
                return RedirectToPage("/userLogin/userDenied");
            }

            rfpForms = await _db.rfpForms.ToListAsync();

            Sums = new Dictionary<string, decimal>();
            PoPDetails = new Dictionary<string, PoPList>();
            foreach (var obj in rfpForms)
            {
                //Updated by kurt 8/30/2024
                var sum = _db.PoP.Where(p => p.PoP_Id == obj.PoPCode).Sum(p => p.Amount);
                Sums[obj.PoPCode] = sum;

                var popDetails = _db.PoP
                    .Where(p => p.PoP_Id == obj.PoPCode)
                    .FirstOrDefault();
                if (popDetails != null)
                {
                    PoPDetails[obj.PoPCode] = popDetails;
                }
            }

            userRole = _db.Users
                .Where(u => u.Name == User.Identity.Name)
                .Select(u => u.UserLevel)
                .FirstOrDefault();

            rfpAddAccess = PagePermission.HasAccess(_db, User.Identity.Name, "RFP_Add");
            rfpViewAccess = PagePermission.HasAccess(_db, User.Identity.Name, "RFP_View");
            rfpArchiveAccess = PagePermission.HasAccess(_db, User.Identity.Name, "RFP_Archive");
            rfpActvtAccess = PagePermission.HasAccess(_db, User.Identity.Name, "RFP_Activate");

            return null;
        }
    }
}
