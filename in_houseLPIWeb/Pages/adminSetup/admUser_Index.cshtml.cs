using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using in_houseLPIWeb.Data;
using in_houseLPIWeb.Model;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using in_houseLPIWeb.Utilities;

namespace in_houseLPIWeb.Pages.adminSetup
{
    [Authorize]
    public class admUser_IndexModel : PageModel
    {
        private readonly WebDbContext _db;
        private readonly ILogger<admUser_IndexModel> _logger;
        public IEnumerable<User> Users { get; set; }
        public string userRole { get; set; }
        public bool userEditAccess { get; set; }
        public bool pageAccessEdit { get; set; }
        public admUser_IndexModel(WebDbContext db, ILogger<admUser_IndexModel> logger)
        {
            _db = db;
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            var userInfo = _db.Users.Where(u => u.Name == User.Identity.Name).Select(u => new { Id = u.Id, isActive = u.isActive }).FirstOrDefault();

            var hasPermission = _db.Permissions.Any(p => p.UserId == userInfo.Id && p.Util_User_Index == true);
            if (!hasPermission || !userInfo.isActive)
            {
                return RedirectToPage("/userLogin/userDenied");
            }

            // Below are the regular functions required to load the page

            string userDept = _db.Users
                        .Where(u => u.Name == User.Identity.Name)
                        .Select(u => u.Department)
                        .FirstOrDefault();
            userRole = _db.Users
                        .Where(u => u.Name == User.Identity.Name)
                        .Select(u => u.UserLevel)
                        .FirstOrDefault();
            if (userRole != null && userRole == "0")
            {
                Users = _db.Users?.ToList();
            }
            else if (userRole != null && new[] { "-0", "-1" }.Any(level => userRole.Contains(level)))
            {
                Users = _db.Users
                        .Where(user => user.Department == userDept)
                        .ToList();
            }
            else
            {
                Users = null;
            }

            userEditAccess = PagePermission.HasAccess(_db, User.Identity.Name, "Util_User_Edit");
            pageAccessEdit = PagePermission.HasAccess(_db, User.Identity.Name, "Util_Permission");

            return null;
        }

        // ============================== UTILITIES FOR VIEWING =======================================
        public string GetUserLevelText(string userLevel)
        {
            Dictionary<int, string> departmentIdNameMapping = GetDepartmentIdNameMapping();
            string[] parts = userLevel.Split('-');
            string dept;
            int deptId;
            string roleIndex;
            string deptName;

            if (parts.Length == 2)
            {
                dept = parts[0];
                int.TryParse(dept, out deptId);
                roleIndex = parts[1];

                if (departmentIdNameMapping.TryGetValue(deptId, out deptName))
                {
                    // Now you have the department name corresponding to the department ID
                    if (roleIndex == "0")
                    {
                        return $"{deptName} Manager";
                    }
                    else if (roleIndex == "1")
                    {
                        return $"{deptName} Officer";
                    }
                    else if (roleIndex == "2")
                    {
                        return $"{deptName} Staff";
                    }
                }
            }
            else
            {
                if (userLevel == "0")
                {
                    return "Administrator";
                }
            }

            return "Guest";
        }

        public Dictionary<int, string> GetDepartmentIdNameMapping()
        {
            Dictionary<int, string> departmentIdNameMapping = new Dictionary<int, string>();

            // Assuming _db is your DbContext instance
            var departments = _db.Departments.ToList(); // Retrieve all departments from the database

            foreach (var department in departments)
            {
                departmentIdNameMapping.Add(department.Id, department.DeptName);
            }

            return departmentIdNameMapping;
        }
    }
}
