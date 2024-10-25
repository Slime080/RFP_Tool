using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using in_houseLPIWeb.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using in_houseLPIWeb.Data;

namespace in_houseLPIWeb.Pages.userLogin
{
    public class userResetModel : PageModel
    {
        private readonly WebDbContext _db;
        private readonly ILogger<userResetModel> _logger;
        public User Users { get; set; }
        public string GeneratedCode { get; set; }
        public bool ShowResetPWfields { get; set; }
        public bool HideCodefields { get; set; }
        public string userEmail { get; set; }
        public string userName { get; set; }
        public userResetModel(ILogger<userResetModel> logger, WebDbContext db)
        {
            _logger = logger;
            _db = db;
        }
        public void OnGet()
        {
        }

        public void OnPostEmailUser(User Users)
        {
            _logger.LogInformation("Email sending checker");

            Response.Cookies.Delete("LawCookieAuth");

            string emailX = Users.Email;

            if (IsEmailExisting(emailX))
            {
                TempData.Remove("Error");
                GeneratedCode = Function.GenerateRandomString(8);

                _logger.LogInformation("Email: " + emailX);
                _logger.LogInformation("Code: " + GeneratedCode);

                userEmail = emailX;
                userName = GetUsername(emailX);

                ViewData["UserEmail"] = userEmail;
                ViewData["UserName"] = userName;

                //sendCode(emailX, GeneratedCode);

                string code = GeneratedCode.Trim();

                //string messageX = $"Good day sir/ma'am,{Environment.NewLine}{Environment.NewLine}Your confirmation code is {GeneratedCode.Trim()}";

                Function.SendEmail(emailX, code);
            }
            else
            {
                TempData["Error"] = "Email not existing.";
            }
        }

        private bool IsEmailExisting(string email)
        {
            // Check the user's credentials against the database with lettercase sensitivity
            var result = _db.Users
                    .Where(u => u.Email == email)
                    .Any();
            return result;
        }

        private string GetUsername(string email)
        {
            string name = _db.Users.Where(u => u.Email == email).Select(u => u.Name).FirstOrDefault();
            return name;
        }

        public void OnPostConfirmCode()
        {
            _logger.LogInformation("Validating entered code");

            string genCode = Request.Form["generatedC"];
            string enteredCode = Request.Form["enteredCode"];

            string emailQ = Request.Form["userEmailQ"];
            string nameQ = Request.Form["userNameQ"];

            userName = nameQ;
            userEmail = emailQ;

            _logger.LogInformation("Generated code:" + genCode);
            _logger.LogInformation("Entered code:" + enteredCode);

            if (genCode == enteredCode)
            {
                TempData.Remove("Error");
                ShowResetPWfields = (genCode == enteredCode);
                HideCodefields = (genCode == enteredCode);
            }
            else if (genCode == "")
            {
                TempData["Error"] = "Warning: Please send a confirmation code to email first.";
            }
            else
            {
                TempData["Error"] = "Code doesn't match.";
            }

            _logger.LogInformation("Email:" + userEmail);
            _logger.LogInformation("Name:" + userName);
        }

        public async Task<IActionResult> OnPostResetUser(User Users)
        {
            // This will hash the password before saving it to the database to secure the password
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(Users.Password);
            _logger.LogInformation("User Password has been hashed: " + hashedPassword);

            if (!ModelState.IsValid)
            {
                string errorMessage = ModelState.Values.FirstOrDefault(v => v.Errors.Any())?.Errors.FirstOrDefault()?.ErrorMessage;
                TempData["Error"] = errorMessage ?? "Failed to reset password.";
            }

            if (ModelState.IsValid)
            {
                ModelState.Clear();
                var id = _db.Users.Where(u => u.Email == Users.Email).Select(u => u.Id).FirstOrDefault();
                // Retrieve the existing user from the database
                var existingUser = _db.Users.Find(id);

                //Update only the password
                existingUser.Password = hashedPassword;

                // Save the changes
                await _db.SaveChangesAsync();
                TempData.Remove("Error");
                TempData["Success"] = "Successfully reset the password.";
                return RedirectToPage("/userLogin/Login");
            }// will be used to validate the input
            return Page();
        }

    }
}
