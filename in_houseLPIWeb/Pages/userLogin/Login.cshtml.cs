using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using in_houseLPIWeb.Data;
using in_houseLPIWeb.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Net;
using System.Net.Mail;
using in_houseLPIWeb.Utilities;

namespace in_houseLPIWeb.Pages.userLogin
{
    public class LoginModel : PageModel
    {
        private readonly WebDbContext _db;
        private readonly UserService _userService;
        [BindProperty]
        public Login Logins { get; set; }
        public User Users { get; set; }
        public List<SelectListItem> Departments { get; set; }
        private readonly ILogger<LoginModel> _logger;
        public LoginModel(WebDbContext db, UserService userService, ILogger<LoginModel> logger)
        {
            _db = db;
            _userService = userService;
            _logger = logger;
        }
        public void OnGet()
        {
            Departments = GetDepartments();
        }
        ///                         LOGIN                         ///
        public async Task<IActionResult> OnPostLoginUserAsync()
        {
            _logger.LogInformation("Login User");

            _logger.LogInformation(Logins.Name + " - "+ Logins.Password);

            if (Logins.Name == "" || Logins.Name == null)
            {
                ModelState.AddModelError("Name", "This field is required.");
                return Page();
            }
            if (Logins.Password == "" || Logins.Password == null)
            {
                ModelState.AddModelError("Password", "This field is required.");
                return Page();
            }

            // Retrieve user from the database based on the provided username
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Name == Logins.Name);

            if (user != null)
            {
                if(!UserChecker.IsActive(_db, user.Name))
                {
                    TempData["Error"] = "The user is inactive.";
                    return Page();
                }

                // Verify the entered password agaisnt the hashed password from the database
                if (BCrypt.Net.BCrypt.Verify(Logins.Password, user.Password))
                {
                    var userLevel = await _userService.GetUserLevelAsync(Logins.Name);

                    if (!string.IsNullOrEmpty(userLevel))
                    {
                        var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim("UserLevel", userLevel)
                };

                        var identity = new ClaimsIdentity(claims, "LawCookieAuth");
                        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = Logins.RememberMe
                        };

                        await HttpContext.SignInAsync("LawCookieAuth", claimsPrincipal, authProperties);
                        return RedirectToPage("/Index");
                    }
                }
                else
                {
                    TempData["Error"] = "Incorrect Password.";
                    return Page();
                }
            }
            else
            {
                TempData["Error"] = "The user does not exist.";
                return Page();
            }

            return Page();
        }
        
        ///                         REGISTER                         ///
        public async Task<IActionResult> OnPostRegUserAsync(User Users)
        {

            // This will hash the password before saving it to the database to secure the password
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(Users.Password);
            _logger.LogInformation("User Password has been hashed: " + hashedPassword);

            if (Users.Department == "Guest")
            {
                Users.UserLevel = "999";
                Users.Department = "Guest";
            }
            // This sets the default UserLevel for each department
            if (Users.Department == "IT")
            {
                Users.UserLevel = "0-1"; // This prevents to automatically gain administrator role
            }
            else
            {
                Department department = _db.Departments.FirstOrDefault(d => d.DeptName == Users.Department);
                if (department != null)
                {
                    Users.UserLevel = $"{department.Id}-2";
                }
            }


            Users.createdDate = DateTime.Now;

            _logger.LogInformation("Email: " + Users.Email);
            _logger.LogInformation("Password: " + Users.Password);
            _logger.LogInformation("Name: " + Users.Name);
            _logger.LogInformation("Department: " + Users.Department);
            _logger.LogInformation("UserLevel: " + Users.UserLevel);

            if (!IsEmailUsingAllowedDomain(Users.Email, "lawson-philippines.com"))
            {
                ModelState.AddModelError("Users.Email", "Email must use the domain @lawson-philippines.com.");
            }
            if (Users.Password != Request.Form["confirmPassword"])
            {
                ModelState.AddModelError("Users.ConfirmPassword", "The password and confirmation password do not match.");
            }
            if (!IsEmailUnique(Users.Email))
            {
                ModelState.AddModelError("Users.Email", "The email is already existing.");
            }
            if (!IsUserUnique(Users.Name))
            {
                ModelState.AddModelError("Users.Name", "Username is already taken. Please choose a different one.");
            }
            if (string.IsNullOrEmpty(Users.Department))
            {
                ModelState.AddModelError("Users.Department", "Please select department.");
            }

            ModelState.Remove("Name");
            ModelState.Remove("Password");

            if (ModelState.IsValid)
            {
                Users.Password = hashedPassword;
                Users.isActive = true;
                await _db.Users.AddAsync(Users);
                await _db.SaveChangesAsync();

                await AddPermissionRecordAsync(Users.Id);

                _logger.LogInformation("User was saved!");

                TempData["Success"] = Users.Name + " was registered successfully.";
                return RedirectToPage("/userLogin/Login");
            }
            string errorMessage = ModelState.Values.FirstOrDefault(v => v.Errors.Any())?.Errors.FirstOrDefault()?.ErrorMessage;
            TempData["Error"] = errorMessage ?? "Registration failed.";

            // this updates the dropdown list
            Departments = GetDepartments();

            return Page();
        }

        // ============= Calling this function will either add or create new record in permissions table
        private async Task AddPermissionRecordAsync(int userID)
        {
            var Users = await _db.Users.FirstOrDefaultAsync(u => u.Id == userID);
            int ifcDept = GetDepartmentId("ifc");
            bool ifcIndex = Users.UserLevel.Contains($"{ifcDept}-");

            Permission permission;

            permission = new Permission
            {
                UserId = Users.Id,
                Index = true,
                Archive = false,
                IFC_Index = ifcIndex,
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
                AddedBy = Users.Name,
                AddedDate = DateTime.Now,
            };

            // Add the new permission record to the database
            _db.Permissions.Add(permission);

            // Save changes to persist the updates or new record
            await _db.SaveChangesAsync();
        }
        private int GetDepartmentId(string departmentName)
        {
            return _db.Departments
                .Where(d => d.DeptName.ToLower() == departmentName)
                .Select(d => d.Id)
                .FirstOrDefault();
        }

        private bool IsEmailUsingAllowedDomain(string email, string allowedDomain)
        {
            if (email.EndsWith($"@{allowedDomain}", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }
        private bool IsUserUnique(string username)
        {
            return !_db.Users.Any(u => u.Name == username);
        }
        private bool IsEmailUnique(string email)
        {
            return !_db.Users.Any(u => u.Email == email);
        }
        private List<SelectListItem> GetDepartments()
        {
            var departments = _db.Departments
                .Where(e => e.isActive == true)
                .OrderBy(e => e.DeptName)
                .ToList();

            return departments.Select(d => new SelectListItem { Text = d.DeptName }).ToList();
        }

        public async Task<IActionResult> OnPostHashed()
        {
            // HTML BUTTON TO USE THIS METHOD
            ////// <!-- NOTE: Please remove this last button after hashing all the existing password before publishing -->
            ////// <button type="submit" asp-page-handler="Hashed" class="btn btn-outline-secondary mx-2">Hashed Existing Passwords</button>

            var users = await _db.Users.ToListAsync();

            foreach (var user in users)
            {
                // Check if the password is already hashed and in the correct bcrypt format
                if (user.Password.StartsWith("$2a$"))
                {
                    // Password is already hashed and in the correct format, no need to rehash
                    continue;
                }
                else
                {
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);
                    _logger.LogInformation("Password has been hashed. " + hashedPassword + " for " + user.Name);
                    user.Password = hashedPassword;
                    await _db.SaveChangesAsync();
                }
            }

            
            TempData["Success"] = "Successfully hashed the passwords.";
            return Page();
        }

    }
}
