using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using in_houseLPIWeb.Data;
using in_houseLPIWeb.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Reflection; // For debugging only

namespace in_houseLPIWeb.Pages.adminSetup
{
    [Authorize]
    [BindProperties]
    public class admPermission_EditModel : PageModel
    {
        private readonly WebDbContext _db;
        private readonly ILogger<admUser_EditModel> _logger;
        public Permission permissionX { get; set; }
        public int userId { get; set; }
        public string userLevel { get; set; }
        public string userRole { get; set; }
        public string currentUlvl { get; set; }
        public string userName { get; set; }
        public bool existingPerm { get; set; }
        public admPermission_EditModel(WebDbContext db, ILogger<admUser_EditModel> logger)
        {
            _db = db;
            _logger = logger;
        }

        public IActionResult OnGet(int id)
        {
            var currentUID = _db.Users.Where(u => u.Name == User.Identity.Name).Select(u => u.Id).FirstOrDefault();

            var hasPermission = _db.Permissions.Any(p => p.UserId == currentUID && p.Util_Permission == true);
            if (!hasPermission)
            {
                return RedirectToPage("/userLogin/userDenied");
            }

            permissionX = _db.Permissions.FirstOrDefault(p => p.UserId == id);
            userId = id;
            userName = _db.Users.Where(n => n.Id == id).Select(n => n.Name).FirstOrDefault();

            userLevel = _db.Users.Where(r => r.Id == id).Select(r => r.UserLevel).FirstOrDefault();
            var dept = "";

            int deptId = 0; // Default value if conversion fails

            // Check if userLevel is not empty and its first character is a digit
            if (!string.IsNullOrEmpty(userLevel) && char.IsDigit(userLevel[0]))
            {
                // Convert the first character to a string and then parse it to an integer
                deptId = int.Parse(userLevel[0].ToString());
                dept = _db.Departments.Where(d => d.Id == deptId).Select(d => d.DeptName).FirstOrDefault();
            }

            // Getting the Role of user
            if (userLevel.StartsWith("0"))
            {
                userRole = "Administrator";
            }
            else if (userLevel.Contains("-0"))
            {
                userRole = $"{dept} Manager";
            }
            else if (userLevel.Contains("-1"))
            {
                userRole = $"{dept} Officer";
            }
            else if (userLevel.Contains("-2"))
            {
                userRole = $"{dept} Staff";
            }
            else
            {
                userRole = "Guest";
            }

            currentUlvl = _db.Users.Where(u => u.Name == User.Identity.Name).Select(u => u.UserLevel).FirstOrDefault();


            return null;
        }

        public async Task<IActionResult> OnPost(Permission permissionX)
        {
            int id = int.Parse(Request.Form["userID"]);
            string userLvl = _db.Users.Where(u => u.Id == id).Select(u => u.UserLevel).FirstOrDefault();

            // ============ To activate related pages for example, when add page is true then edit page is also true ============
            permissionX.Index = true; // this is the default page aka homepage
            if (permissionX.RFP_Add || permissionX.RFP_Archive || permissionX.RFP_Print || permissionX.RFP_View)
            {
                permissionX.RFP_Index = true;
                permissionX.RFP_Dash = true;
            } else
            {
                permissionX.RFP_Index = false;
                permissionX.RFP_Dash = false;
            }

            if (permissionX.RFP_Add)
            {
                permissionX.RFP_Edit = true;
                permissionX.RFP_PoP_Edit = true;
                permissionX.RFP_PoP_Delete = true;
            } else
            {
                permissionX.RFP_Edit = false;
                permissionX.RFP_PoP_Edit = false;
                permissionX.RFP_PoP_Delete = false;
            }

            if (permissionX.RFP_Archive)
            {
                permissionX.RFP_Activate = true;
                permissionX.Archive = true;
            } else
            {
                permissionX.RFP_Activate = false;
                permissionX.Archive = false;
            }

            if (permissionX.RFPutil_Payee_Index)
            {
                permissionX.RFPutil_Dash = true;
                permissionX.RFPutil_Payee_Edit = true;
            } else
            {
                if (permissionX.RFPutil_TOC_Index)
                {
                    permissionX.RFPutil_Dash = true;
                }
                permissionX.RFPutil_Payee_Edit = false;
            }

            if (permissionX.RFPutil_TOC_Index)
            {
                permissionX.RFPutil_Dash = true;
                permissionX.RFPutil_TOC_Edit = true;
            } else
            {
                if (permissionX.RFPutil_Payee_Index)
                {
                    permissionX.RFPutil_Dash = true;
                }
                permissionX.RFPutil_TOC_Edit = false;
            }

            if (permissionX.Util_Store_Index)
            {
                permissionX.Util_Store_Edit = true;
            } else
            {
                permissionX.Util_Store_Edit = false;
            }

            if (permissionX.Util_Store_Archive)
            {
                permissionX.Util_Store_Actv = true;
            } else
            {
                permissionX.Util_Store_Actv = false;
            }

            if (permissionX.Util_StoreEnt_Index)
            {
                permissionX.Util_StoreEnt_Edit = true;
            } else
            {
                permissionX.Util_StoreEnt_Edit = false;
            }

            if (permissionX.Util_StoreType_Index)
            {
                permissionX.Util_StoreType_Edit = true;
            } else
            {
                permissionX.Util_StoreType_Edit = false;
            }

            if (permissionX.Util_Permission || permissionX.Util_User_Edit)
            {
                permissionX.Util_User_Index = true;
            }
            else
            {
                permissionX.Util_User_Index = false;
            }

            if (permissionX.Util_Store_Index && permissionX.Util_User_Index)
            {
                permissionX.Util_Dash = true;
            } else
            {
                permissionX.Util_Dash = false;
            }

            if (permissionX.Util_UserDep_Edit)
            {
                permissionX.Util_UserDep_Index = true;
            } else
            {
                permissionX.Util_UserDep_Index = false;
            }

            if (userLvl.Equals("0"))
            {
                permissionX.Util_UserDep_Index = true;
                permissionX.Util_UserDep_Edit = true;
                permissionX.Util_UserDep_Archive = true;
                permissionX.Util_UserDep_Actv = true;
            }                                                                   // Automatically sets the permission for departments when user is admin or 0
            else if (userLvl.Contains("-2"))
            {
                permissionX.Util_StoreEnt_Index = false;
                permissionX.Util_StoreEnt_Edit = false;
                permissionX.Util_StoreType_Index = false;
                permissionX.Util_StoreType_Edit = false;
                permissionX.Util_UserDep_Index = false;
                permissionX.Util_UserDep_Edit = false;
                permissionX.Util_UserDep_Archive = false;
                permissionX.Util_UserDep_Actv = false;
            }

            // For debugging
            Type type = permissionX.GetType();
            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(permissionX);
                _logger.LogInformation("Value of {PropertyName}: {PropertyValue}", property.Name, value);
            }

            // This first check if the user already have page permissions for the system to add new or update existing
            var existingPermission = _db.Permissions
                .Any(p => p.UserId == id);

            string errorMessage = ModelState.Values.FirstOrDefault(v => v.Errors.Any())?.Errors.FirstOrDefault()?.ErrorMessage;

            if (!existingPermission)
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        _logger.LogInformation("The modification will be added.");

                        if (userLvl.Equals("0"))
                        {
                            if (!permissionX.Util_UserDep_Edit)
                            {
                                permissionX.Util_UserDep_Index = false;
                                permissionX.Util_UserDep_Archive = false;
                                permissionX.Util_UserDep_Actv = false;
                            }
                            
                        }

                        permissionX.UserId = userId;
                        permissionX.AddedBy = User.Identity.Name;
                        permissionX.AddedDate = DateTime.Now;
                        await _db.Permissions.AddAsync(permissionX);
                        
                        await _db.SaveChangesAsync();

                        TempData["Success"] = "User page permission added successfully.";

                        return RedirectToPage("/adminSetup/admUser_Index");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogInformation("Error - " + ex);
                }

                TempData["Error"] = errorMessage ?? "Failed to add user page permission.";

                return Page();
            }
            else
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        _logger.LogInformation("The modification will be updated.");
                        // Retrieve the existing user from the database
                        //var updateUser = _db.Permissions.Find(permissionX.UserId);
                        var updateUser = _db.Permissions.FirstOrDefault(p => p.UserId == id);

                        // Add specfic pages to update
                        updateUser.Index = permissionX.Index;
                        updateUser.Archive = permissionX.Archive;
                        updateUser.IFC_Index = permissionX.IFC_Index;

                        
                        updateUser.RFP_Index = permissionX.RFP_Index;
                        updateUser.RFP_Add = permissionX.RFP_Add;
                        updateUser.RFP_Edit = permissionX.RFP_Edit;
                        updateUser.RFP_Archive = permissionX.RFP_Archive;
                        updateUser.RFP_Activate = permissionX.RFP_Activate;
                        updateUser.RFP_PoP_Edit = permissionX.RFP_PoP_Edit;
                        updateUser.RFP_PoP_Delete = permissionX.RFP_PoP_Delete;
                        updateUser.RFP_Print = permissionX.RFP_Print;
                        updateUser.RFP_View = permissionX.RFP_View;

                        
                        updateUser.RFPutil_Payee_Index = permissionX.RFPutil_Payee_Index;
                        updateUser.RFPutil_Payee_Edit = permissionX.RFPutil_Payee_Edit;
                        updateUser.RFPutil_TOC_Index = permissionX.RFPutil_TOC_Index;
                        updateUser.RFPutil_TOC_Edit = permissionX.RFPutil_TOC_Edit;
                        

                        
                        updateUser.Util_Store_Index = permissionX.Util_Store_Index;
                        updateUser.Util_Store_Edit = permissionX.Util_Store_Edit;
                        updateUser.Util_Store_Archive = permissionX.Util_Store_Archive;
                        updateUser.Util_Store_Actv = permissionX.Util_Store_Actv;


                        if(userLvl.Contains("-0") || userLvl.Contains("-1"))
                        {
                            updateUser.RFP_Dash = updateUser.RFP_Dash;
                            updateUser.RFPutil_Dash = updateUser.RFPutil_Dash;
                            updateUser.Util_Dash = updateUser.Util_Dash;
                            updateUser.Util_Permission = updateUser.Util_Permission;

                            updateUser.Util_StoreEnt_Index = updateUser.Util_StoreEnt_Index;
                            updateUser.Util_StoreEnt_Edit = updateUser.Util_StoreEnt_Edit;

                            updateUser.Util_StoreType_Index = updateUser.Util_StoreType_Index;
                            updateUser.Util_StoreType_Edit = updateUser.Util_StoreType_Edit;

                            updateUser.Util_User_Index = updateUser.Util_User_Index;
                            updateUser.Util_User_Edit = updateUser.Util_User_Edit;

                            updateUser.Util_UserDep_Index = updateUser.Util_UserDep_Index;
                            updateUser.Util_UserDep_Edit = updateUser.Util_UserDep_Edit;
                            updateUser.Util_UserDep_Actv = updateUser.Util_UserDep_Actv;
                            updateUser.Util_UserDep_Archive = updateUser.Util_UserDep_Archive;
                        }
                        else
                        {
                            updateUser.RFP_Dash = permissionX.RFP_Dash;
                            updateUser.RFPutil_Dash = permissionX.RFPutil_Dash;
                            updateUser.Util_Dash = permissionX.Util_Dash;
                            updateUser.Util_Permission = permissionX.Util_Permission;

                            updateUser.Util_StoreEnt_Index = permissionX.Util_StoreEnt_Index;
                            updateUser.Util_StoreEnt_Edit = permissionX.Util_StoreEnt_Edit;

                            updateUser.Util_StoreType_Index = permissionX.Util_StoreType_Index;
                            updateUser.Util_StoreType_Edit = permissionX.Util_StoreType_Edit;

                            updateUser.Util_User_Index = permissionX.Util_User_Index;
                            updateUser.Util_User_Edit = permissionX.Util_User_Edit;

                            updateUser.Util_UserDep_Index = permissionX.Util_UserDep_Index;
                            updateUser.Util_UserDep_Edit = permissionX.Util_UserDep_Edit;
                            updateUser.Util_UserDep_Actv = permissionX.Util_UserDep_Actv;
                            updateUser.Util_UserDep_Archive = permissionX.Util_UserDep_Archive;
                        }

                        

                        updateUser.ModifiedBy = User.Identity.Name;
                        updateUser.ModifiedDate = DateTime.Now;

                        // Save the changes

                        await _db.SaveChangesAsync();

                        TempData["Success"] = "User page permission updated successfully.";

                        return RedirectToPage("/adminSetup/admUser_Index");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogInformation("Error - " + ex);
                }

                TempData["Error"] = errorMessage ?? "Failed to update user page permission.";

                return Page();
            }
            
        }
    }
}
