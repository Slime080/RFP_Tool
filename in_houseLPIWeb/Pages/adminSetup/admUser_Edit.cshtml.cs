using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using in_houseLPIWeb.Data;
using in_houseLPIWeb.Model;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using in_houseLPIWeb.Utilities;

namespace in_houseLPIWeb.Pages.adminSetup
{
    [Authorize]
    [BindProperties]
    public class admUser_EditModel : PageModel
    {
        private readonly WebDbContext _db;
        private readonly ILogger<admUser_EditModel> _logger;
        private readonly AuditChanges _auditChanges;////// Changes
        public User Users { get; set; }
        public List<SelectListItem> Departments { get; set; }
        public List<SelectListItem> Roles { get; set; }
        public string userDept { get; set; }
        public string userRole { get; set; }
        public string existingDept { get; set; }
        public admUser_EditModel(WebDbContext db, ILogger<admUser_EditModel> logger, AuditChanges auditChanges)////// Changes
        {
            _db = db;
            _logger = logger;
            _auditChanges = auditChanges;////// Changes
        }

        public IActionResult OnGet(int id)
        {
            var userInfo = _db.Users.Where(u => u.Name == User.Identity.Name).Select(u => new { Id = u.Id, isActive = u.isActive }).FirstOrDefault();

            var hasPermission = _db.Permissions.Any(p => p.UserId == userInfo.Id && p.Util_User_Edit == true);
            if (!hasPermission || !userInfo.isActive)
            {
                return RedirectToPage("/userLogin/userDenied");
            }

            // Below are the regular functions required to load the page

            Users = _db.Users.Find(id);
            Departments = GetDepartments();

            userDept = _db.Users
                .Where(u => u.Name == User.Identity.Name)
                .Select(u => u.Department)
                .FirstOrDefault();

            userRole = _db.Users
                .Where(u => u.Name == User.Identity.Name)
                .Select(u => u.UserLevel)
                .FirstOrDefault();

            if (userDept == "IT")
            {
                existingDept = _db.Users
                    .Where(u => u.Id == id)
                    .Select(u => u.Department)
                    .FirstOrDefault();
            }

            Roles = GetRolesDropDown();

            return null;
        }
        private List<SelectListItem> GetDepartments()
        {
            var departments = _db.Departments
                .Where(e => e.isActive == true)
                .OrderBy(e => e.DeptName)
                .ToList();

            return departments.Select(d => new SelectListItem { Text = d.DeptName }).ToList();
        }
        public async Task<IActionResult> OnPost(User Users)
        {
            try
            {
                // Clear validation errors for Email and Password
                ModelState.Remove("Users.Email");
                ModelState.Remove("Users.Password");

                //_logger.LogInformation(Request.Form["dept_name"]);
                Users.Department = Request.Form["dept_name"];

                if (ModelState.IsValid)
                {
                    // Retrieve the existing user from the database
                    var existingUser = _db.Users.Find(Users.Id);

                    //Update only the specific properties you need
                    if (!string.IsNullOrEmpty(Users.Department))
                    {
                        existingUser.Department = Users.Department;
                    }
                    //existingUser.Department = Request.Form["dept_name"];
                    existingUser.UserLevel = Users.UserLevel;
                    existingUser.isActive = Users.isActive;

                    #region Changes starts here for OnPost

                    var entry = _db.Entry(existingUser);
                    var changes = new Dictionary<string, (string OldValue, string NewValue)>();

                    // Collecting changes for existing records
                    if (entry.State == EntityState.Modified)
                    {
                        foreach (var prop in entry.Properties)
                        {
                            if (prop.IsModified)
                            {
                                changes.Add(prop.Metadata.Name, (prop.OriginalValue?.ToString(), prop.CurrentValue?.ToString()));
                            }
                        }
                    }

                    // Save the changes
                    await _db.SaveChangesAsync();

                    // For new entries, we log the current state as new values
                    if (entry.State == EntityState.Added)
                    {
                        foreach (var prop in entry.Properties)
                        {
                            changes.Add(prop.Metadata.Name, (null, prop.CurrentValue?.ToString()));
                        }
                    }

                    await _auditChanges.AddUpdateAuditAsync(_db, existingUser.Id, nameof(User), changes, User.Identity.Name);

                    #endregion

                    await AddPermissionRecordAsync(existingUser.Id);

                    TempData["Success"] = "User detail edited successfully.";

                    return RedirectToPage("/adminSetup/admUser_Index");
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error - " + ex);
            }

            string errorMessage = ModelState.Values.FirstOrDefault(v => v.Errors.Any())?.Errors.FirstOrDefault()?.ErrorMessage;
            TempData["Error"] = errorMessage ?? "Failed to edit User details.";

            Departments = GetDepartments();
            return Page();
        }

        private async Task AddPermissionRecordAsync(int userID)
        {
            var Users = await _db.Users.FirstOrDefaultAsync(u => u.Id == userID);
            int ifcDept = GetDepartmentId("ifc");
            int finDept = GetDepartmentId("finance");
            bool ifcIndex = Users.UserLevel.Contains($"{ifcDept}-") || Users.UserLevel.Contains($"{finDept}-");
            bool ifcStaff = Users.UserLevel.Contains($"{ifcDept}-");

            var existingPermission = await _db.Permissions.FirstOrDefaultAsync(p => p.UserId == userID);

            Permission permission;

            if (existingPermission != null) // If existing it will just modify the page permission of selected user
            {
                _logger.LogInformation("Existing user modifying page permissions");
                if (Users.UserLevel.Equals("0")) // This checks if the user is an admin
                {
                    existingPermission.Index = true;
                    existingPermission.Archive = true;
                    existingPermission.IFC_Index = true;
                    existingPermission.RFP_Dash = true;
                    existingPermission.RFP_Index = true;
                    existingPermission.RFP_Add = true;
                    existingPermission.RFP_Edit = true;
                    existingPermission.RFP_Archive = true;
                    existingPermission.RFP_Activate = true;
                    existingPermission.RFP_PoP_Edit = true;
                    existingPermission.RFP_PoP_Delete = true;
                    existingPermission.RFP_Print = true;
                    existingPermission.RFP_View = true;
                    existingPermission.RFPutil_Dash = true;
                    existingPermission.RFPutil_Payee_Index = true;
                    existingPermission.RFPutil_Payee_Edit = true;
                    existingPermission.RFPutil_TOC_Index = true;
                    existingPermission.RFPutil_TOC_Edit = true;
                    existingPermission.Util_Dash = true;
                    existingPermission.Util_Permission = true;
                    existingPermission.Util_Store_Index = true;
                    existingPermission.Util_Store_Edit = true;
                    existingPermission.Util_Store_Archive = true;
                    existingPermission.Util_Store_Actv = true;
                    existingPermission.Util_StoreEnt_Index = true;
                    existingPermission.Util_StoreEnt_Edit = true;
                    existingPermission.Util_StoreType_Index = true;
                    existingPermission.Util_StoreType_Edit = true;
                    existingPermission.Util_User_Index = true;
                    existingPermission.Util_User_Edit = true;
                    existingPermission.Util_UserDep_Index = true;
                    existingPermission.Util_UserDep_Edit = true;
                    existingPermission.Util_UserDep_Actv = true;
                    existingPermission.Util_UserDep_Archive = true;
                    existingPermission.ModifiedBy = User.Identity.Name;
                    existingPermission.ModifiedDate = DateTime.Now;
                }
                else if (Users.UserLevel.Contains("-0") || Users.UserLevel.Contains("-1")) // This checks if the user is a Manager or Officer
                {
                    existingPermission.Index = true;
                    existingPermission.Archive = true;
                    existingPermission.IFC_Index = ifcIndex;
                    existingPermission.RFP_Dash = true;
                    existingPermission.RFP_Index = true;
                    existingPermission.RFP_Add = true;
                    existingPermission.RFP_Edit = true;
                    existingPermission.RFP_Archive = true;
                    existingPermission.RFP_Activate = true;
                    existingPermission.RFP_PoP_Edit = true;
                    existingPermission.RFP_PoP_Delete = true;
                    existingPermission.RFP_Print = true;
                    existingPermission.RFP_View = true;
                    existingPermission.RFPutil_Dash = true;
                    existingPermission.RFPutil_Payee_Index = true;
                    existingPermission.RFPutil_Payee_Edit = true;
                    existingPermission.RFPutil_TOC_Index = true;
                    existingPermission.RFPutil_TOC_Edit = true;
                    existingPermission.Util_Dash = true;
                    existingPermission.Util_Permission = true;
                    existingPermission.Util_Store_Index = true;
                    existingPermission.Util_Store_Edit = true;
                    existingPermission.Util_Store_Archive = true;
                    existingPermission.Util_Store_Actv = true;
                    existingPermission.Util_StoreEnt_Index = false;
                    existingPermission.Util_StoreEnt_Edit = false;
                    existingPermission.Util_StoreType_Index = false;
                    existingPermission.Util_StoreType_Edit = false;
                    existingPermission.Util_User_Index = true;
                    existingPermission.Util_User_Edit = true;
                    existingPermission.Util_UserDep_Index = true;
                    existingPermission.Util_UserDep_Edit = true;
                    existingPermission.Util_UserDep_Actv = false;
                    existingPermission.Util_UserDep_Archive = false;
                    existingPermission.ModifiedBy = User.Identity.Name;
                    existingPermission.ModifiedDate = DateTime.Now;
                }
                else // default will be staff level
                {
                    existingPermission.Index = true;
                    existingPermission.Archive = false;
                    existingPermission.IFC_Index = ifcStaff;
                    existingPermission.RFP_Dash = true;
                    existingPermission.RFP_Index = true;
                    existingPermission.RFP_Add = true;
                    existingPermission.RFP_Edit = true;
                    existingPermission.RFP_Archive = false;
                    existingPermission.RFP_Activate = false;
                    existingPermission.RFP_PoP_Edit = true;
                    existingPermission.RFP_PoP_Delete = true;
                    existingPermission.RFP_Print = true;
                    existingPermission.RFP_View = true;
                    existingPermission.RFPutil_Dash = false;
                    existingPermission.RFPutil_Payee_Index = false;
                    existingPermission.RFPutil_Payee_Edit = false;
                    existingPermission.RFPutil_TOC_Index = false;
                    existingPermission.RFPutil_TOC_Edit = false;
                    existingPermission.Util_Dash = false;
                    existingPermission.Util_Permission = false;
                    existingPermission.Util_Store_Index = false;
                    existingPermission.Util_Store_Edit = false;
                    existingPermission.Util_Store_Archive = false;
                    existingPermission.Util_Store_Actv = false;
                    existingPermission.Util_StoreEnt_Index = false;
                    existingPermission.Util_StoreEnt_Edit = false;
                    existingPermission.Util_StoreType_Index = false;
                    existingPermission.Util_StoreType_Edit = false;
                    existingPermission.Util_User_Index = false;
                    existingPermission.Util_User_Edit = false;
                    existingPermission.Util_UserDep_Index = false;
                    existingPermission.Util_UserDep_Edit = false;
                    existingPermission.Util_UserDep_Actv = false;
                    existingPermission.Util_UserDep_Archive = false;
                    existingPermission.ModifiedBy = User.Identity.Name;
                    existingPermission.ModifiedDate = DateTime.Now;
                }

                #region Changes starts here for OnPost

                var entry = _db.Entry(existingPermission);
                var changes = new Dictionary<string, (string OldValue, string NewValue)>();

                // Collecting changes for existing records
                if (entry.State == EntityState.Modified)
                {
                    foreach (var prop in entry.Properties)
                    {
                        if (prop.IsModified)
                        {
                            changes.Add(prop.Metadata.Name, (prop.OriginalValue?.ToString(), prop.CurrentValue?.ToString()));
                        }
                    }
                }

                // Save the changes
                await _db.SaveChangesAsync();

                // For new entries, we log the current state as new values
                if (entry.State == EntityState.Added)
                {
                    foreach (var prop in entry.Properties)
                    {
                        changes.Add(prop.Metadata.Name, (null, prop.CurrentValue?.ToString()));
                    }
                }

                await _auditChanges.AddUpdateAuditAsync(_db, existingPermission.Id, nameof(Permission), changes, User.Identity.Name);

                #endregion
            }
            else // If not existing it will add new record
            {
                _logger.LogInformation("adding new record page permissions");

                if (Users.UserLevel.Equals("0")) // This checks if the user is an admin
                {
                    permission = new Permission
                    {
                        UserId = Users.Id,
                        Index = true,
                        Archive = true,
                        IFC_Index = true,
                        RFP_Dash = true,
                        RFP_Index = true,
                        RFP_Add = true,
                        RFP_Edit = true,
                        RFP_Archive = true,
                        RFP_Activate = true,
                        RFP_PoP_Edit = true,
                        RFP_PoP_Delete = true,
                        RFP_Print = true,
                        RFP_View = true,
                        RFPutil_Dash = true,
                        RFPutil_Payee_Index = true,
                        RFPutil_Payee_Edit = true,
                        RFPutil_TOC_Index = true,
                        RFPutil_TOC_Edit = true,
                        Util_Dash = true,
                        Util_Permission = true,
                        Util_Store_Index = true,
                        Util_Store_Edit = true,
                        Util_Store_Archive = true,
                        Util_Store_Actv = true,
                        Util_StoreEnt_Index = true,
                        Util_StoreEnt_Edit = true,
                        Util_StoreType_Index = true,
                        Util_StoreType_Edit = true,
                        Util_User_Index = true,
                        Util_User_Edit = true,
                        Util_UserDep_Index = true,
                        Util_UserDep_Edit = true,
                        Util_UserDep_Actv = true,
                        Util_UserDep_Archive = true,
                        AddedBy = User.Identity.Name,
                        AddedDate = DateTime.Now,
                    };
                }
                else if (Users.UserLevel.Contains("-0") || Users.UserLevel.Contains("-1")) // This checks if the user is a Manager or Officer
                {
                    permission = new Permission
                    {
                        UserId = Users.Id,
                        Index = true,
                        Archive = true,
                        IFC_Index = ifcIndex,
                        RFP_Dash = true,
                        RFP_Index = true,
                        RFP_Add = true,
                        RFP_Edit = true,
                        RFP_Archive = true,
                        RFP_Activate = true,
                        RFP_PoP_Edit = true,
                        RFP_PoP_Delete = true,
                        RFP_Print = true,
                        RFP_View = true,
                        RFPutil_Dash = true,
                        RFPutil_Payee_Index = true,
                        RFPutil_Payee_Edit = true,
                        RFPutil_TOC_Index = true,
                        RFPutil_TOC_Edit = true,
                        Util_Dash = true,
                        Util_Permission = true,
                        Util_Store_Index = true,
                        Util_Store_Edit = true,
                        Util_Store_Archive = true,
                        Util_Store_Actv = true,
                        Util_StoreEnt_Index = false,
                        Util_StoreEnt_Edit = false,
                        Util_StoreType_Index = false,
                        Util_StoreType_Edit = false,
                        Util_User_Index = true,
                        Util_User_Edit = true,
                        Util_UserDep_Index = true,
                        Util_UserDep_Edit = true,
                        Util_UserDep_Actv = false,
                        Util_UserDep_Archive = false,
                        AddedBy = User.Identity.Name,
                        AddedDate = DateTime.Now,
                    };
                }
                else // default will be staff level
                {
                    permission = new Permission
                    {
                        UserId = Users.Id,
                        Index = true,
                        Archive = false,
                        IFC_Index = ifcStaff,
                        RFP_Dash = true,
                        RFP_Index = true,
                        RFP_Add = true,
                        RFP_Edit = true,
                        RFP_Archive = false,
                        RFP_Activate = false,
                        RFP_PoP_Edit = true,
                        RFP_PoP_Delete = true,
                        RFP_Print = true,
                        RFP_View = true,
                        RFPutil_Dash = false,
                        RFPutil_Payee_Index = false,
                        RFPutil_Payee_Edit = false,
                        RFPutil_TOC_Index = false,
                        RFPutil_TOC_Edit = false,
                        Util_Dash = false,
                        Util_Permission = false,
                        Util_Store_Index = false,
                        Util_Store_Edit = false,
                        Util_Store_Archive = false,
                        Util_Store_Actv = false,
                        Util_StoreEnt_Index = false,
                        Util_StoreEnt_Edit = false,
                        Util_StoreType_Index = false,
                        Util_StoreType_Edit = false,
                        Util_User_Index = false,
                        Util_User_Edit = false,
                        Util_UserDep_Index = false,
                        Util_UserDep_Edit = false,
                        Util_UserDep_Actv = false,
                        Util_UserDep_Archive = false,
                        AddedBy = User.Identity.Name,
                        AddedDate = DateTime.Now,
                    };
                }

                // Add the new permission record to the database
                _db.Permissions.Add(permission);
                await _db.SaveChangesAsync();

                #region Start of Changes for logging newly added data
                await _auditChanges.AddCreateAuditAsync(_db, permission.Id, nameof(Permission), User.Identity.Name);
                #endregion
            }

        }

        private List<SelectListItem> GetRolesDropDown()
        {

            string userRole = _db.Users
                .Where(u => u.Name == User.Identity.Name)
                .Select(u => u.UserLevel)
                .FirstOrDefault();

            var department = userRole.Equals("0") ? "AllDepartments" : userDept;

            List<SelectListItem> Roles = new List<SelectListItem>();

            switch (department)
            {
                case "IT":
                    // Add role for IT department
                    Roles.Add(new SelectListItem { Text = "Administrator", Value = "0" });
                    break;
                case "Guest":
                    // Add role for Guest department
                    Roles.Add(new SelectListItem { Text = "Guest", Value = "999" });
                    break;
                case "AllDepartments": // Handle the case when userRole is "0"
                    Roles = GetAllRoles(existingDept);
                    break;
                default:
                    // For other departments, infer roles from values
                    if (department != null)
                    {
                        Roles.Add(new SelectListItem { Text = $"{department} Staff", Value = $"{GetDepartmentId(department.ToLower())}-2" });
                        Roles.Add(new SelectListItem { Text = $"{department} Officer", Value = $"{GetDepartmentId(department.ToLower())}-1" });
                        Roles.Add(new SelectListItem { Text = $"{department} Manager", Value = $"{GetDepartmentId(department.ToLower())}-0" });
                    }
                    break;
            }
            return Roles;
        }


        List<SelectListItem> GetAllRoles(string dept)
        {
            //_logger.LogInformation("existingDept: " + dept);
            List<SelectListItem> Roles = new List<SelectListItem>();

            switch (dept)
            {
                case "IT":
                    Roles.Add(new SelectListItem { Text = "Administrator", Value = "0" });
                    break;
                default:
                    if (dept != null)
                    {
                        Roles.Add(new SelectListItem { Text = $"{dept} Staff", Value = $"{GetDepartmentId(dept.ToLower())}-2" });
                        Roles.Add(new SelectListItem { Text = $"{dept} Officer", Value = $"{GetDepartmentId(dept.ToLower())}-1" });
                        Roles.Add(new SelectListItem { Text = $"{dept} Manager", Value = $"{GetDepartmentId(dept.ToLower())}-0" });
                    }
                    break;
            }

            return Roles;
        }

        private int GetDepartmentId(string departmentName)
        {
            return _db.Departments
                .Where(d => d.DeptName.ToLower() == departmentName)
                .Select(d => d.Id)
                .FirstOrDefault();
        }

        public JsonResult OnGetUserLevels(string deptName)
        {
            try
            {
                // Log the received departmentName
                // _logger.LogInformation("Received Department Name: ", deptName);

                List<SelectListItem> Roles = new List<SelectListItem>();

                switch (deptName)
                {
                    case "IT":
                        // Log that IT department is selected
                        _logger.LogInformation("IT department selected");

                        Roles.Add(new SelectListItem { Text = "Administrator", Value = "0" });
                        break;

                    default:
                        if (deptName != null)
                        {
                            Roles.Add(new SelectListItem { Text = $"{deptName} Staff", Value = $"{GetDepartmentId(deptName.ToLower())}-2" });
                            Roles.Add(new SelectListItem { Text = $"{deptName} Officer", Value = $"{GetDepartmentId(deptName.ToLower())}-1" });
                            Roles.Add(new SelectListItem { Text = $"{deptName} Manager", Value = $"{GetDepartmentId(deptName.ToLower())}-0" });
                        }
                        break;
                }

                return new JsonResult(Roles);
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur
                _logger.LogError(ex, "Error occurred in GetUserLevels action method");
                return new JsonResult(new List<SelectListItem>()); // Return an empty list in case of error
            }
        }

    }
}
