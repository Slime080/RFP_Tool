using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using in_houseLPIWeb.Data;
using in_houseLPIWeb.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace in_houseLPIWeb.Pages.RFP
{
    [Authorize]
    public class popDeleteModel : PageModel
    {
        // ================================== Initializing code ==================================
        private readonly WebDbContext _db;
        private readonly ILogger<popDeleteModel> _logger;

        public PoPList PoPListx { get; set; }
        public IEnumerable<PoPList> PoPLists { get; set; }

        public string previousURL { get; set; }
        public popDeleteModel(WebDbContext db, ILogger<popDeleteModel> logger)
        {
            _db = db;
            _logger = logger;
        }
        // This will initialize the string to get the PoPCode from the RFP
        public string existingPopCode { get; private set; }
        public int existingRFPNo { get; private set; }
        public string userRole { get; set; }
        public bool hasContent { get; set; }
        public bool billingDate { get; set; }

        public IActionResult OnGet(int id)
        {
            var userInfo = _db.Users.Where(u => u.Name == User.Identity.Name).Select(u => new { Id = u.Id, isActive = u.isActive }).FirstOrDefault();

            var hasPermission = _db.Permissions.Any(p => p.UserId == userInfo.Id && p.RFP_PoP_Delete == true);
            if (!hasPermission || !userInfo.isActive)
            {
                return RedirectToPage("/userLogin/userDenied");
            }

            // Below are the regular functions required to load the page

            // Log or print the value of PoP_Id
            _logger.LogInformation("PoP_Id received: " + id);

            PoPListx = _db.PoP.Find(id);

            existingPopCode = _db.PoP.Where(e => e.Id == id).Select(e => e.PoP_Id).FirstOrDefault();

            existingRFPNo = _db.rfpForms
                .Where(e => e.PoPCode == existingPopCode)
                .Select(e => e.RFP_No)
                .FirstOrDefault();
            userRole = _db.Users
                        .Where(u => u.Name == User.Identity.Name)
                        .Select(u => u.UserLevel)
                        .FirstOrDefault();
            hasContent = _db.PoP
               .Where(e => e.Id == id)
               .Any(e => !string.IsNullOrEmpty(e.OR_Number) || !string.IsNullOrEmpty(e.SI_Number) || !string.IsNullOrEmpty(e.DR_Number) || !string.IsNullOrEmpty(e.PO_Number));

            billingDate = _db.PoP
                .Where(e => e.Id == id && e.CoverStartDate == e.CoverEndDate)
                .Any();


            return null;
        }

        public async Task<IActionResult> OnPost(PoPList PoPListx)
        {
            int RFP_No = _db.rfpForms
                    .Where(e => e.PoPCode == PoPListx.PoP_Id)
                    .Select(e => e.RFP_No)
                    .FirstOrDefault();
            string userRole = _db.Users
                        .Where(u => u.Name == User.Identity.Name)
                        .Select(u => u.UserLevel)
                        .FirstOrDefault();
            string creator = PoPListx.CreatedBy;
            if (userRole.Equals("0") || new[] { "-0", "-1" }.Any(level => userRole.Contains(level)) || creator.Equals(User.Identity.Name))
            {
                var popFromDb = _db.PoP.Find(PoPListx.Id);

                if (popFromDb != null)
                {
                    _db.PoP.Remove(popFromDb);
                    await _db.SaveChangesAsync();
                    TempData.Remove("Error");
                    TempData["Success"] = "Purpose of Payment detail deleted successfully.";

                    existingRFPNo = _db.rfpForms
                        .Where(e => e.PoPCode == PoPListx.PoP_Id)
                        .Select(e => e.RFP_No)
                        .FirstOrDefault();

                    return RedirectToPage("/RFP/rfpEdit", new { id = RFP_No });
                }
            }
            else
            {
                ModelState.AddModelError("", "Warning: You don't have access to delete this Form.");
            }

            string errorMessage = ModelState.Values.FirstOrDefault(v => v.Errors.Any())?.Errors.FirstOrDefault()?.ErrorMessage;
            TempData["Error"] = errorMessage ?? "Failed to delete Purpose of Payment details.";

            return RedirectToPage("/RFP/rfpEdit", new { id = RFP_No });
        }

        public IActionResult OnPostCancel(PoPList PoPListx)
        {
            int existingRFPNo = _db.rfpForms
                .Where(e => e.PoPCode == PoPListx.PoP_Id)
                .Select(e => e.RFP_No)
                .FirstOrDefault();

            previousURL = TempData.Peek("PreviousPage")?.ToString();
            //_logger.LogInformation("Previous URL: " + previousURL);

            // Add a condition to check the referring page
            if (!string.IsNullOrEmpty(previousURL) && previousURL.Contains("/RFP/rfpAdd"))
            {
                // Redirect back to the first page
                return RedirectToPage("/RFP/rfpAdd");
            }
            else if (!string.IsNullOrEmpty(previousURL) && previousURL.Contains($"/RFP/rfpEdit?id={ existingRFPNo }"))
            {
                // Redirect back to the second page
                return RedirectToPage("/RFP/rfpEdit", new { id = existingRFPNo });
            }

            TempData["Error"] = "Cannot Find Previous Page.";
            // Return a default action if none of the conditions are met
            return Page();
        }
    }
}
