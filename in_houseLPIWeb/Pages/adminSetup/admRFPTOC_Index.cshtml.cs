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
using Microsoft.AspNetCore.Mvc.Rendering;

namespace in_houseLPIWeb.Pages.adminSetup
{
    [Authorize]
    [BindProperties]
    public class admRFPTOC_IndexModel : PageModel
    {
        private readonly WebDbContext _db;
        private readonly AuditChanges _auditChanges;////// Changes
        public TOC TOCs { get; set; }
        public IEnumerable<TOC> TOCList { get; set; }
        public bool tocEditAccess { get; set; }
        public string udept { get; set; }
        public List<SelectListItem> Departments { get; set; }////// Changes
        public admRFPTOC_IndexModel(WebDbContext db, AuditChanges auditChanges)////// Changes
        {
            _db = db;
            _auditChanges = auditChanges;////// Changes
        }

        public IActionResult OnGet()
        {
            var userInfo = _db.Users.Where(u => u.Name == User.Identity.Name).Select(u => new { Id = u.Id, isActive = u.isActive }).FirstOrDefault();

            var hasPermission = _db.Permissions.Any(p => p.UserId == userInfo.Id && p.RFPutil_TOC_Index == true);
            if (!hasPermission || !userInfo.isActive)
            {
                return RedirectToPage("/userLogin/userDenied");
            }

            // Below are the regular functions required to load the page

            //TOCList = _db.TOCs;

            udept = _db.Users
                        .Where(u => u.Name == User.Identity.Name)
                        .Select(u => u.Department)
                        .FirstOrDefault();
            string userDept = _db.Users
                        .Where(u => u.Name == User.Identity.Name)
                        .Select(u => u.Department)
                        .FirstOrDefault();
            var userRole = _db.Users
                        .Where(u => u.Name == User.Identity.Name)
                        .Select(u => u.UserLevel)
                        .FirstOrDefault();
            if (userRole != null && userRole == "0")
            {
                TOCList = _db.TOCs?.ToList();
            }
            else if (userRole != null && new[] { "-0", "-1" }.Any(level => userRole.Contains(level)))
            {
                TOCList = _db.TOCs
                        .Where(user => user.TOCDepartment == userDept)
                        .ToList();
            }
            else
            {
                TOCList = null;
            }

            tocEditAccess = PagePermission.HasAccess(_db, User.Identity.Name, "RFPutil_TOC_Edit");

            Departments = GetDepartments();////// Changes

            return null;
        }

        public async Task<IActionResult> OnPost(TOC TOCs)
        {
            //var dept = _db.Users.Where(u => u.Name == User.Identity.Name).Select(u => u.Department).FirstOrDefault();

            if (string.IsNullOrEmpty(TOCs.TOCName))
            {
                ModelState.AddModelError("TOCs.TOCName", "The Type of Charge field is required.");
            }

            if (string.IsNullOrEmpty(TOCs.TOCDepartment))
            {
                ModelState.AddModelError("TOCs.TOCDepartment", "Please select a department.");
            }

            if (TOCExists(TOCs.TOCName, TOCs.TOCDepartment))
            {
                ModelState.AddModelError("TOCs.TOCName", "This Type of Charge is already existing and cannot be duplicated.");
            }

            if (ModelState.IsValid)
            {
                // Log form values
                foreach (var key in Request.Form.Keys)
                {
                    var value = Request.Form[key];
                    Console.WriteLine($"Form field: {key}, Value: {value}");
                }

                string userDept = _db.Users
                        .Where(u => u.Name == User.Identity.Name)
                        .Select(u => u.Department)
                        .FirstOrDefault();

                TOCs.TOCDepartment = userDept;

                await _db.TOCs.AddAsync(TOCs);
                await _db.SaveChangesAsync();

                #region Start of Changes for logging newly added data
                await _auditChanges.AddCreateAuditAsync(_db, TOCs.Id, nameof(TOC), User.Identity.Name);
                #endregion

                TempData["Success"] = "Type of Charge details added successfully.";
                return RedirectToPage("/adminSetup/admRFPTOC_Index");
            }
            string errorMessage = ModelState.Values.FirstOrDefault(v => v.Errors.Any())?.Errors.FirstOrDefault()?.ErrorMessage;
            TempData["Error"] = errorMessage ?? "Failed to add type of charge details.";
            return RedirectToPage("/adminSetup/admRFPTOC_Index");
        }

        private bool TOCExists(string TOCname, string dept)
        {
            // Perform a query to check if the StoreNumber already exists in the database
            return _db.TOCs.Any(s => s.TOCName == TOCname && s.TOCDepartment == dept);
        }

        private List<SelectListItem> GetDepartments()
        {
            var departments = _db.Departments
                .Where(e => e.isActive == true)
                .OrderBy(e => e.DeptName)
                .ToList();

            return departments.Select(d => new SelectListItem { Text = d.DeptName }).ToList();
        }
    }
}
