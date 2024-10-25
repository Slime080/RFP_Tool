using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using in_houseLPIWeb.Data;
using in_houseLPIWeb.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace in_houseLPIWeb.Pages.adminSetup
{
    [Authorize]
    [BindProperties]
    public class admUserDep_ArchiveModel : PageModel
    {
        private readonly WebDbContext _db;
        public Department Departmentx { get; set; }
        public admUserDep_ArchiveModel(WebDbContext db)
        {
            _db = db;
        }
        
        public IActionResult OnGet(int id)
        {
            var userInfo = _db.Users.Where(u => u.Name == User.Identity.Name).Select(u => new { Id = u.Id, isActive = u.isActive }).FirstOrDefault();

            var hasPermission = _db.Permissions.Any(p => p.UserId == userInfo.Id && p.Util_UserDep_Archive == true);
            if (!hasPermission || !userInfo.isActive)
            {
                return RedirectToPage("/userLogin/userDenied");
            }

            // Below are the regular functions required to load the page

            Departmentx = _db.Departments.Find(id);

            return null;
        }
        public async Task<IActionResult> OnPost()
        {
            var DepartmentFromDb = _db.Departments.Find(Departmentx.Id);

            if (DepartmentFromDb == null)
            {
                TempData["Error"] = "Invalid Department selected.";
                return RedirectToPage("/adminSetup/admUserDep_Index");
            }

            if (DepartmentFromDb.isActive == true)
            {
                DepartmentFromDb.isActive = false;
                await _db.SaveChangesAsync();
                TempData["Success"] = "Department archived successfully.";
            }
            else
            {
                TempData["Error"] = "Failed to archive Department.";
                return Page();
            }

            return RedirectToPage("/adminSetup/admUserDep_Index");
        }
    }
}
