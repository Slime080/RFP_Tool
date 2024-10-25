using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using in_houseLPIWeb.Data;
using in_houseLPIWeb.Model;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace in_houseLPIWeb.Pages.adminSetup
{
    [Authorize]
    public class admUserDep_EditModel : PageModel
    {
        private readonly WebDbContext _db;
        private readonly ILogger<admUserDep_EditModel> _logger;
        public Department Departmentx { get; set; }
        public admUserDep_EditModel(WebDbContext db, ILogger<admUserDep_EditModel> logger)
        {
            _db = db;
            _logger = logger;
        }

        public IActionResult OnGet(int id)
        {
            var userInfo = _db.Users.Where(u => u.Name == User.Identity.Name).Select(u => new { Id = u.Id, isActive = u.isActive }).FirstOrDefault();

            var hasPermission = _db.Permissions.Any(p => p.UserId == userInfo.Id && p.Util_UserDep_Edit == true);
            if (!hasPermission || !userInfo.isActive)
            {
                return RedirectToPage("/userLogin/userDenied");
            }

            // Below are the regular functions required to load the page

            Departmentx = _db.Departments.Find(id);

            return null;
        }
        public async Task<IActionResult> OnPost(Department Departmentx)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existing = _db.Departments.Find(Departmentx.Id);

                    existing.DeptName = Departmentx.DeptName;
                    existing.DeptHead = Departmentx.DeptHead;

                    await _db.SaveChangesAsync();
                    TempData["Success"] = "Department details edited successfully.";

                    return RedirectToPage("/adminSetup/admUserDep_Index");
                }
                TempData["Error"] = "Failed to edit department details.";
                return RedirectToPage("/adminSetup/admUserDep_Index");
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error - " + ex);
            }
            return Page();
        }
    }
}
