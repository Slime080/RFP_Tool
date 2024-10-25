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
    public class admRFPTOC_EditModel : PageModel
    {
        private readonly WebDbContext _db;
        public TOC TOCs { get; set; }
        public admRFPTOC_EditModel(WebDbContext db)
        {
            _db = db;
        }
        
        public IActionResult OnGet(int id)
        {
            var userInfo = _db.Users.Where(u => u.Name == User.Identity.Name).Select(u => new { Id = u.Id, isActive = u.isActive }).FirstOrDefault();

            var hasPermission = _db.Permissions.Any(p => p.UserId == userInfo.Id && p.RFPutil_TOC_Edit == true);
            if (!hasPermission || !userInfo.isActive)
            {
                return RedirectToPage("/userLogin/userDenied");
            }

            // Below are the regular functions required to load the page

            TOCs = _db.TOCs.Find(id);

            return null;
        }
        public async Task<IActionResult> OnPost(TOC TOCs)
        {
            if (!ModelState.IsValid)
            {
                string errorMessage = ModelState.Values.FirstOrDefault(v => v.Errors.Any())?.Errors.FirstOrDefault()?.ErrorMessage;
                TempData["Error"] = errorMessage ?? "Failed to edit type of charge.";
            }

            if (ModelState.IsValid)
            {
                ModelState.Clear();
                var existing = _db.TOCs.Find(TOCs.Id);
                existing.TOCdescription = TOCs.TOCdescription;
                existing.IsArchived = TOCs.IsArchived;

                // Save the changes
                await _db.SaveChangesAsync();
                TempData.Remove("Error");
                TempData["Success"] = "Type of charge edited successfully.";
                return RedirectToPage("/adminSetup/admRFPTOC_Index");
            }// will be used to validate the input
            return Page();
        }
    }
}
