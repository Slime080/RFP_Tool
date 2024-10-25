using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using in_houseLPIWeb.Data;
using in_houseLPIWeb.Model;
using in_houseLPIWeb.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace in_houseLPIWeb.Pages.RFP
{
    [Authorize]
    public class popEditModel : PageModel
    {
        // ================================== Initializing code ==================================
        private readonly WebDbContext _db;
        private readonly ILogger<popEditModel> _logger;
        private readonly IConfiguration _configuration;
        private readonly AuditChanges _auditChanges;////// Changes
        public string userRole { get; set; }
        public string previousURL { get; set; }

        public rfpForm rfpFormx { get; set; }
        public PoPList PoPListx { get; set; }
        public IEnumerable<PoPList> PoPLists { get; set; }
        public List<SelectListItem> cbToC { get; set; }
        public List<SelectListItem> cbEntities { get; set; }
        public List<SelectListItem> cbCurrencies { get; set; }
        public popEditModel(WebDbContext db, ILogger<popEditModel> logger, IConfiguration configuration, AuditChanges auditChanges)////// Changes
        {
            _db = db;
            _logger = logger;
            _configuration = configuration;
            _auditChanges = auditChanges;////// Changes
        }
        // This will initialize the string to get the PoPCode from the RFP
        public string existingPopCode { get; private set; }
        public int existingRFPNo { get; private set; }
        public bool hasContent { get; set; }

        public IActionResult OnGet(int id)
        {
            var userInfo = _db.Users.Where(u => u.Name == User.Identity.Name).Select(u => new { Id = u.Id, isActive = u.isActive }).FirstOrDefault();

            var hasPermission = _db.Permissions.Any(p => p.UserId == userInfo.Id && p.RFP_PoP_Edit == true);
            if (!hasPermission || !userInfo.isActive)
            {
                return RedirectToPage("/userLogin/userDenied");
            }

            // Below are the regular functions required to load the page

            // Log or print the value of PoP_Id
            _logger.LogInformation("PoP_Id received: " + id);

            PoPListx = _db.PoP.Find(id);
            rfpFormx = _db.rfpForms.Find(id);

            existingPopCode = _db.PoP.Where(e => e.Id == id).Select(e => e.PoP_Id).FirstOrDefault();

            existingRFPNo = _db.rfpForms
                .Where(e => e.PoPCode == existingPopCode)
                .Select(e => e.RFP_No)
                .FirstOrDefault();
            _logger.LogInformation("existingRFPNo: " + existingRFPNo);
            cbToC = GetToCs();
            cbEntities = GetEntities();
            cbCurrencies = GetCurrencies();

            // to activate the container for transaction number
            hasContent = _db.PoP
                .Where(e => e.Id == id)
                .Any(e => !string.IsNullOrEmpty(e.OR_Number) || !string.IsNullOrEmpty(e.SI_Number) || !string.IsNullOrEmpty(e.DR_Number) || !string.IsNullOrEmpty(e.PO_Number));

            return null;
        }

        public async Task<IActionResult> OnPost(PoPList PoPListx)
        {
            string userx = User.Identity.Name;
            string userRole = _db.Users
                    .Where(p => p.Name == userx)
                    .Select(p => p.UserLevel)
                    .FirstOrDefault();
            string creatorName = _db.rfpForms
                    .Where(p => p.PoPCode == PoPListx.PoP_Id)
                    .Select(p => p.CreatedBy)
                    .FirstOrDefault();
            string creatorRole = _db.Users
                    .Where(p => p.Name == creatorName)
                    .Select(p => p.UserLevel)
                    .FirstOrDefault();
            int rfpNumExist = _db.rfpForms
                    .Where(e => e.PoPCode == PoPListx.PoP_Id)
                    .Select(e => e.RFP_No)
                    .FirstOrDefault();
            int getSiteID = _db.Departments.Where(e => e.DeptName.ToLower() == "site development").Select(e => e.Id).FirstOrDefault();
            int getIFCID = _db.Departments.Where(e => e.DeptName.ToLower() == "ifc").Select(e => e.Id).FirstOrDefault();

            if (creatorName == userx || userRole.Equals("0") || userRole == creatorRole || userRole.Contains($"{getSiteID}-") || (PoPListx.Entity == "1003" && userRole.Contains($"{getIFCID}-")))
            {
                PoPListx.CreatedBy = Request.Form["createdBy"];
                PoPListx.ModifiedDate = DateTime.Now;
                PoPListx.ModifiedBy = User.Identity.Name;

                _logger.LogInformation("User Name: " + PoPListx.ModifiedBy);
                _logger.LogInformation("userRole: " + userRole);
                _logger.LogInformation("creatorRole: " + PoPListx.CreatedBy);
                _logger.LogInformation("creatorRole: " + creatorRole);

                // Debugging Statements
                foreach (var prop in typeof(PoPList).GetProperties())
                {
                    _logger.LogInformation($"{prop.Name}: {prop.GetValue(PoPListx)}");
                }

                if (ModelState.IsValid)
                {
                    #region Changes starts here for OnPost

                    var entry = _db.Entry(PoPListx);
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

                    _db.PoP.Update(PoPListx);
                    await _db.SaveChangesAsync();

                    // For new entries, we log the current state as new values
                    if (entry.State == EntityState.Added)
                    {
                        foreach (var prop in entry.Properties)
                        {
                            changes.Add(prop.Metadata.Name, (null, prop.CurrentValue?.ToString()));
                        }
                    }

                    await _auditChanges.AddUpdateAuditAsync(_db, PoPListx.Id, nameof(PoPList), changes, User.Identity.Name);

                    #endregion
                    TempData.Remove("Error");
                    TempData["Success"] = "Purpose of Payment detail edited successfully.";

                    cbToC = GetToCs();
                    cbEntities = GetEntities();
                    cbCurrencies = GetCurrencies();
                    int RFP_No = existingRFPNo;

                    previousURL = TempData.Peek("PreviousPage")?.ToString();
                    //_logger.LogInformation("Previous URL: " + previousURL);

                    // Add a condition to check the referring page
                    if (!string.IsNullOrEmpty(previousURL) && previousURL.Contains("/RFP/rfpAdd"))
                    {
                        // Redirect back to the first page
                        return RedirectToPage("/RFP/rfpAdd");
                    }
                    else if (!string.IsNullOrEmpty(previousURL) && previousURL.Contains($"/RFP/rfpEdit?id={RFP_No}"))
                    {
                        // Redirect back to the second page
                        return RedirectToPage("/RFP/rfpEdit", new { id = rfpNumExist });
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Warning: You don't have access to edit this Form.");
            }

            if (!ModelState.IsValid)
            {
                string errorMessage = ModelState.Values.FirstOrDefault(v => v.Errors.Any())?.Errors.FirstOrDefault()?.ErrorMessage;
                TempData["Error"] = errorMessage ?? "Failed to edit Purpose of Payment details.";
            }

            cbToC = GetToCs();
            cbEntities = GetEntities();
            cbCurrencies = GetCurrencies();

            return RedirectToPage("/RFP/rfpEdit", new { id = rfpNumExist });
        }

        public IActionResult OnPostCancel(PoPList PoPListx)
        {
            existingRFPNo = _db.rfpForms
                    .Where(e => e.PoPCode == PoPListx.PoP_Id)
                    .Select(e => e.RFP_No)
                    .FirstOrDefault();

            int RFP_No = existingRFPNo;

            previousURL = TempData.Peek("PreviousPage")?.ToString();
            //_logger.LogInformation("Previous URL: " + previousURL);

            // Add a condition to check the referring page
            if (!string.IsNullOrEmpty(previousURL) && previousURL.Contains("/RFP/rfpAdd"))
            {
                // Redirect back to the first page
                return RedirectToPage("/RFP/rfpAdd");
            }
            else if (!string.IsNullOrEmpty(previousURL) && previousURL.Contains($"/RFP/rfpEdit?id={RFP_No}"))
            {
                // Redirect back to the second page
                return RedirectToPage("/RFP/rfpEdit", new { id = RFP_No });
            }
            else if (!string.IsNullOrEmpty(previousURL) && previousURL.Contains("/IFC/ifcIndex"))
            {
                TempData.Remove("PreviousPage");
                // Redirect back to the second page
                return RedirectToPage("/IFC/ifcIndex");
            }

            TempData["Error"] = "Cannot Find Previous Page.";
            // Return a default action if none of the conditions are met
            return Page();
        }

        private List<SelectListItem> GetToCs()
        {
            var tocs = _db.TOCs
                .Where(e => !e.IsArchived)
                .OrderBy(e => e.TOCName)
                .ToList();

            if (tocs != null && tocs.Any())
            {
                return tocs.Select(d => new SelectListItem
                {
                    Text = d.TOCName,
                    Value = d.TOCName
                }).ToList();
            }
            return new List<SelectListItem>();
        }

        private List<SelectListItem> GetEntities()
        {
            var entities = _db.Entities
                .Where(e => e.IsActive)
                .OrderBy(e => e.EntityCode)
                .ToList();

            if (entities != null && entities.Any())
            {
                return entities.Select(d => new SelectListItem
                {
                    Text = d.EntityCode,
                    Value = d.EntityCode.ToString()
                }).ToList();
            }

            // Handle the case where there are no active entities
            return new List<SelectListItem>();
        }

        private List<SelectListItem> GetCurrencies()
        {
            try
            {
                using (var conn = new SqlConnection(_configuration.GetConnectionString("ProductionConnection")))
                {
                    conn.Open();

                    string query = "SELECT List FROM dbo.DropdownList WHERE Active = 1 AND Description = 'CURRENCY'";

                    var modesOfPayment = conn.Query<string>(query);

                    return modesOfPayment.Select(d => new SelectListItem
                    {
                        Text = d,
                        Value = d
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                Console.WriteLine("Error executing query: " + ex.Message);
                return new List<SelectListItem>(); // Return an empty list in case of an error
            }
        }

        // ================================ TO CALCULATE THE COVER DATES AND STORE TYPE =====================================

        public JsonResult OnGetStoreTypeInfo(string ent, string store, string CoverStartDate, string CoverEndDate)
        {
            if (ent == "1003")
            {
                _logger.LogInformation($"Is everything have value? : Entity: {ent}, Store: {store}, CSD: {CoverStartDate}, CED: {CoverEndDate}");

                if (string.IsNullOrEmpty(store))
                {
                    return new JsonResult(new { Error = "Invalid Store Selected." });
                }

                string[] chargeToParts = store.Split('-');

                if (chargeToParts.Length >= 2)
                {
                    string storeCode = chargeToParts[0].Trim();

                    var storeData = _db.Stores
                        .Where(s => s.StoreCode == storeCode && s.IsOpen == true)
                        .Select(s => new { s.OpenDate, s.StoreType })
                        .FirstOrDefault();

                    if (storeData != null)
                    {
                        DateTime openDate = storeData.OpenDate;
                        string sType = storeData.StoreType;

                        DateTime coverStartDate = DateTime.Parse(CoverStartDate);
                        DateTime coverEndDate = DateTime.Parse(CoverEndDate);

                        double totalDays = (coverEndDate - openDate).TotalDays;
                        int averageDaysInYear = 365 + (DateTime.IsLeapYear(openDate.Year) ? 1 : 0);
                        int yearsDifference = (int)(totalDays / averageDaysInYear);

                        if (yearsDifference < 1)
                        {
                            // If less than a year, return true and get store type
                            return new JsonResult(new { LessThanAYear = true, StoreType = sType });
                        }
                        else
                        {
                            // If greater than a year, return false and get store type
                            return new JsonResult(new { LessThanAYear = false, StoreType = sType });
                        }
                    }
                    else
                    {
                        return new JsonResult(new { Error = "Store not found in the database." });
                    }
                }
                else
                {
                    return new JsonResult(new { Error = "Invalid Store Selected." });
                }
            }
            else
            {
                return new JsonResult(new { Error = "Store not found in the database." }) { StatusCode = 400 };
            }

        }
    }
}
