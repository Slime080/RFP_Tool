using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using in_houseLPIWeb.Data;
using in_houseLPIWeb.Model;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using in_houseLPIWeb.Utilities;

namespace in_houseLPIWeb.Pages.adminSetup
{
    [Authorize]
    public class admUserDep_IndexModel : PageModel
    {
        private readonly WebDbContext _db;
        private readonly ILogger<admUserDep_IndexModel> _logger;

        public IEnumerable<Department> Departments { get; set; }
        [BindProperty]
        public Department Departmentx { get; set; }
        public string userRole { get; set; }
        public bool deptEditAccess { get; set; }
        public admUserDep_IndexModel(WebDbContext db, ILogger<admUserDep_IndexModel> logger)
        {
            _db = db;
            _logger = logger;
        }
        
        public IActionResult OnGet()
        {
            var userInfo = _db.Users.Where(u => u.Name == User.Identity.Name).Select(u => new { Id = u.Id, isActive = u.isActive }).FirstOrDefault();

            var hasPermission = _db.Permissions.Any(p => p.UserId == userInfo.Id && p.Util_UserDep_Index == true);
            if (!hasPermission || !userInfo.isActive)
            {
                return RedirectToPage("/userLogin/userDenied");
            }

            // Below are the regular functions required to load the page

            deptEditAccess = PagePermission.HasAccess(_db, User.Identity.Name, "Util_UserDep_Edit");

            userRole = _db.Users
                .Where(u => u.Name == User.Identity.Name)
                .Select(u => u.UserLevel)
                .FirstOrDefault();

            string userDept = _db.Users
                        .Where(u => u.Name == User.Identity.Name)
                        .Select(u => u.Department)
                        .FirstOrDefault();

            if (userRole != null && userRole == "0")
            {
                Departments = _db.Departments ?? Enumerable.Empty<Department>();
            }
            else if (userRole != null && new[] { "-0", "-1" }.Any(level => userRole.Contains(level)))
            {
                Departments = _db.Departments
                        .Where(user => user.DeptName == userDept)
                        .ToList();
            }
            else
            {
                Departments = null;
            }

            return null;
        }
        public async Task<IActionResult> OnPostAddDeptAsync(Department Departmentx)
        {
            if (Departments == null)
            {
                _logger.LogInformation("Walang Departments");
                //return Page();
            }

            if (!IsDeptUnique(Departmentx.DeptName))
            {
                Departments = _db.Departments;
                return Page();
            }
            if (ModelState.IsValid)
            {
                await _db.Departments.AddAsync(Departmentx);
                await _db.SaveChangesAsync();

                _logger.LogInformation("Department added successfully.");

                // Update Departments after adding a new department
                Departments = _db.Departments.ToList();
                //Departments = _db.Departments;
                return RedirectToPage("/adminSetup/admUserDep_Index");
            }
            if (!ModelState.IsValid)
            {
                _logger.LogInformation("may error");
            }

            // Update Departments if ModelState is not valid
            Departments = _db.Departments;

            return Page();
        }
        private bool IsDeptUnique(string dept)
        {
            return !_db.Departments.Any(u => u.DeptName == dept);
        }
    }
}
