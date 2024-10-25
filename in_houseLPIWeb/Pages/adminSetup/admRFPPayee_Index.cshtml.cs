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
    public class admRFPPayee_IndexModel : PageModel
    {
        private readonly WebDbContext _db;
        private readonly AuditChanges _auditChanges;////// Changes
        public Payee Payees { get; set; }
        public IEnumerable<Payee> PayeeList { get; set; }
        public bool payeeEditAccess { get; set; }
        public string udept { get; set; }
        public List<SelectListItem> Departments { get; set; }////// Changes
        public admRFPPayee_IndexModel(WebDbContext db, AuditChanges auditChanges)////// Changes
        {
            _db = db;
            _auditChanges = auditChanges;////// Changes
        }


        public IActionResult OnGet()
        {
            var userInfo = _db.Users.Where(u => u.Name == User.Identity.Name).Select(u => new { Id = u.Id, isActive = u.isActive }).FirstOrDefault();

            var hasPermission = _db.Permissions.Any(p => p.UserId == userInfo.Id && p.RFPutil_Payee_Index == true);
            if (!hasPermission || !userInfo.isActive)
            {
                return RedirectToPage("/userLogin/userDenied");
            }

            // Below are the regular functions required to load the page

            // PayeeList = _db.Payees;  // PayeeList = _db.Payees ?? Enumerable.Empty<Payee>();

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
                PayeeList = _db.Payees?.ToList();
            }
            else if (userRole != null && new[] { "-0", "-1" }.Any(level => userRole.Contains(level)))
            {
                PayeeList = _db.Payees
                        .Where(user => user.PayeeDepartment == userDept)
                        .ToList();
            }
            else
            {
                PayeeList = null;
            }


            payeeEditAccess = PagePermission.HasAccess(_db, User.Identity.Name, "RFPutil_Payee_Edit");

            Departments = GetDepartments();////// Changes

            return null;
        }

        public async Task<IActionResult> OnPost(Payee Payees)
        {
            //var dept = _db.Users.Where(u => u.Name == User.Identity.Name).Select(u => u.Department).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(Payees.PayeeDepartment))
            {
                ModelState.AddModelError("Payees.PayeeDepartment", "Please select a department.");
            }

            if (PayeeExists(Payees.PayeeName, Payees.PayeeDepartment))
            {
                ModelState.AddModelError("Payees.PayeeName", "This payee is already existing and cannot be duplicated.");
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

                Payees.PayeeName = Payees.PayeeName.ToUpper(); // To maintain the letter case for dropdown
                Payees.PayeeDepartment = userDept;
                Payees.IsActive = true;

                await _db.Payees.AddAsync(Payees);
                await _db.SaveChangesAsync();

                #region Start of Changes for logging newly added data
                await _auditChanges.AddCreateAuditAsync(_db, Payees.Id, nameof(Payee), User.Identity.Name);
                #endregion

                TempData["Success"] = "Payee details added successfully.";
                return RedirectToPage("/adminSetup/admRFPPayee_Index");
            }
            string errorMessage = ModelState.Values.FirstOrDefault(v => v.Errors.Any())?.Errors.FirstOrDefault()?.ErrorMessage;
            TempData["Error"] = errorMessage ?? "Failed to add payee details.";
            // Update table
            PayeeList = _db.Payees.ToList();
            return Page();
        }

        private bool PayeeExists(string payeeName, string dept)
        {
            // Perform a query to check if the StoreNumber already exists in the database
            return _db.Payees.Any(s => s.PayeeName == payeeName && s.PayeeDepartment == dept);
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
