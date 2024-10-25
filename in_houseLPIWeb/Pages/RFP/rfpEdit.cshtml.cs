using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
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
    public class rfpEditModel : PageModel
    {
        // ================================== Initializing code ==================================
        private readonly WebDbContext _db;
        private readonly ILogger<rfpEditModel> _logger;
        private readonly IConfiguration _configuration;
        private readonly AuditChanges _auditChanges;////// Changes
        public rfpForm rfpFormx { get; set; }
        public PoPList PoPListx  { get; set; }
        public IEnumerable<PoPList> PoPLists { get; set; }
        public IEnumerable<SelectListItem> StoreDetails { get; set; }


        public List<SelectListItem> cbEntities { get; set; }
        public List<SelectListItem> cbPayees { get; set; }
        public List<SelectListItem> cbToC { get; set; }
        public List<SelectListItem> cbMoPs { get; set; }
        public List<SelectListItem> cbCurrencies { get; set; }




        public rfpEditModel(WebDbContext db, ILogger<rfpEditModel> logger, IConfiguration configuration, AuditChanges auditChanges)////// Changes
        {
            _db = db;
            _logger = logger;
            _configuration = configuration;
            _auditChanges = auditChanges;////// Changes
            
        }
        public int rfpNo { get; private set; }
        public string CurrencyQ { get; set; }

        // This will initialize the string to get the PoPCode from the RFP
        public string popQ { get; private set; }
    
        public PoPList NewPoPList { get; private set; }
        public string GeneratedAmountWordHtml { get; set; } // Add property to store generated HTML
        public string GeneratedPoPTBHtml { get; set; } // Add property to store generated HTML
        public string GeneratedSign { get; set; } // Add property to store generated HTML
        public string userRole { get; set; }
        public string creatorName { get; set; }


        public IActionResult OnGet(int id)
        {
            
            // Check if the user is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/userLogin/userDenied");
            }

            // Retrieve user info
            var userInfo = _db.Users
                              .Where(u => u.Name == User.Identity.Name)
                              .Select(u => new { Id = u.Id, isActive = u.isActive })
                              .FirstOrDefault();

            // Check if user exists and is active
            if (userInfo == null || !userInfo.isActive)
            {
                return RedirectToPage("/userLogin/userDenied");
            }

            // Check if user has permission to edit
            var hasPermission = _db.Permissions
                                  .Any(p => p.UserId == userInfo.Id && p.RFP_Edit == true);

            if (!hasPermission)
            {
                return RedirectToPage("/userLogin/userDenied");
            }

            // Load the page data
            TempData["PreviousPage"] = $"/RFP/rfpEdit?id={id}";

            rfpFormx = _db.rfpForms.Find(id);

            if (rfpFormx.Status == "Closed")
            {
                TempData["Message"] = "The edit for this RFP number is locked.";
                return RedirectToPage("/RFP/rfpView", new { id = id });
            }
            if (rfpFormx == null)
            {
                return NotFound(); // Return a 404 if the form is not found
            }

            rfpNo = id;

            string popId = rfpFormx.PoPCode; // Get PoPCode directly from rfpFormx

            userRole = _db.Users
                          .Where(u => u.Name == User.Identity.Name)
                          .Select(u => u.UserLevel)
                          .FirstOrDefault() ?? string.Empty; // Handle null

            creatorName = _db.Users
                             .Where(u => u.Name == rfpFormx.CreatedBy)
                             .Select(u => u.Name)
                             .FirstOrDefault() ?? string.Empty; // Handle null

            PoPLists = _db.PoP
                         .Where(p => p.PoP_Id == popId)
                         .ToList();

            popQ = rfpFormx.PoPCode; // Get PoPCode directly from rfpFormx

            CurrencyQ = _db.PoP
                         .Where(c => c.PoP_Id == popQ)
                         .Select(c => c.Currency)
                         .FirstOrDefault() ?? string.Empty; // Handle null

            NewPoPList = new PoPList
            {
                PoP_Id = popQ
            };

            GeneratedPoPTBHtml = GenerateHtmlContent(userRole, popQ);
            GeneratedAmountWordHtml = GenerateHtmlAnotherContent(popQ);

            string deptx = _db.Users
                              .Where(u => u.Name == User.Identity.Name)
                              .Select(u => u.Department)
                              .FirstOrDefault() ?? string.Empty; // Handle null

            GeneratedSign = GenerateHtmlSignatories(deptx, popQ);

            LoadDropDowns();

            return Page(); // Return the page after loading all necessary data
        }


        // ==================================Kurt 9-10-2024 For populating the dropdown  ==================================
        //public JsonResult OnGetStores(string entity)
        //{
        //    var stores = _db.Stores
        //        .Where(s => s.Entity == entity && s.IsOpen)
        //        .Select(s => new
        //        {
        //            Value = s.StoreCode,
        //            Text = s.StoreName
        //        })
        //        .ToList();

        //    return new JsonResult(stores);
        //}
        public async Task<IActionResult> OnGetStoresByEntityAsync(int entityId)
        {
            var stores = await _db.Stores
                .Where(s => s.Id == entityId) // Adjust based on your schema
                .Select(s => new { s.Id, s.StoreName })
                .ToListAsync();

            return new JsonResult(stores);
        }



        // ================================== Main Function Code ==================================

        // For Editing RFP
        [BindProperty(Name = "rfpFormx.PropertyToInclude")]
        public string rfpFormPropertyToInclude { get; set; }

        public async Task<IActionResult> OnPostEditRFPAsync(rfpForm rfpFormx)
        {
            string checkedBy = rfpFormx.Checked.ToUpper();
            string notedBy = rfpFormx.Noted.ToUpper();
            string approvedBy = rfpFormx.Approved.ToUpper();
            string createdBy = Request.Form["createdBy"];
            rfpFormx.ModifiedDate = DateTime.Now;
            rfpFormx.ModifiedBy = User.Identity.Name;

            // Debugging Statements
            foreach (var prop in typeof(rfpForm).GetProperties())
            {
                _logger.LogInformation($"{prop.Name}: {prop.GetValue(rfpFormx)}");
            }


            var requiredFields = new Dictionary<string, string>
            {
                { nameof(rfpFormx.Checked), "Please Enter RFP checker's name." },
                { nameof(rfpFormx.Noted), "Please Enter RFP noter's name." },
                { nameof(rfpFormx.Approved), "Please Enter RFP approver's name." }
            };

            foreach (var field in requiredFields)
            {
                var fieldValue = typeof(rfpForm).GetProperty(field.Key)?.GetValue(rfpFormx, null) as string;
                if (string.IsNullOrEmpty(fieldValue))
                {
                    ModelState.AddModelError($"rfpFormx.{field.Key}", field.Value);
                }
            }

            if (rfpFormx.ModifiedBy != Request.Form["CreatedBy"])
            {
                ModelState.AddModelError("rfpFormx.CreatedBy", "Warning: Editing restricted to form creator.");
            }

            if (!ModelState.IsValid)
            {

                foreach (var key in ModelState.Keys)
                {
                    foreach (var error in ModelState[key].Errors)
                    {
                        //Console.WriteLine($"{key}: {error.ErrorMessage}");
                        _logger.LogInformation($"{key}: {error.ErrorMessage}");
                    }
                }

                string errorMessage = ModelState.Values.FirstOrDefault(v => v.Errors.Any())?.Errors.FirstOrDefault()?.ErrorMessage;
                TempData["Error"] = errorMessage ?? "Failed to edit Request for Payment.";
                // To retain the dropdown selection
                LoadDropDowns();
            }

            if (ModelState.IsValid)
            {
                var existing = _db.rfpForms.Find(rfpFormx.RFP_No);
                existing.Payee = rfpFormx.Payee;
                existing.MoP = rfpFormx.MoP;
                existing.ToP = rfpFormx.ToP;
                existing.DueDate = rfpFormx.DueDate;
                existing.Remarks = rfpFormx.Remarks;
                existing.Checked = checkedBy;
                existing.Noted = notedBy;
                existing.Approved = approvedBy;
                existing.ModifiedBy = rfpFormx.ModifiedBy;
                existing.ModifiedDate = rfpFormx.ModifiedDate;

                #region Changes starts here for OnPost

                var entry = _db.Entry(existing);
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

                await _auditChanges.AddUpdateAuditAsync(_db, existing.RFP_No, nameof(rfpForm), changes, User.Identity.Name);

                #endregion

                TempData.Remove("Error");
                TempData["Success"] = "Request for Payment edited successfully.";

                // To retain the dropdown selection
                LoadDropDowns();
                TempData.Remove("PreviousPage");
                return RedirectToPage("/RFP/rfpView", new { id = rfpFormx.RFP_No });
            }// will be used to validate the input

            return RedirectToPage("/RFP/rfpEdit", new { id = rfpFormx.RFP_No });
        }

        // For Editing Purpose of Payment
        [BindProperty(Name = "PoPListx.Property1")]
        public string PoPListProperty1 { get; set; }

        public async Task<IActionResult> OnPostAddPOPAsync(PoPList PoPListx)
        {
            if (string.IsNullOrEmpty(PoPListx.ToC))
            {
                ModelState.AddModelError("PoPListx.ToC", "Warning: Please don't leave Type of Charge blank.");
            }
            string userRole = _db.Users
                        .Where(u => u.Name == User.Identity.Name)
                        .Select(u => u.UserLevel)
                        .FirstOrDefault();
            string deptName = _db.Users
                        .Where(u => u.Name == User.Identity.Name)
                        .Select(u => u.Department)
                        .FirstOrDefault();

            string sdwan = PoPListx.ChargeTo.ToLower();

            if (!ModelState.IsValid)
            {
                foreach (var key in ModelState.Keys)
                {
                    foreach (var error in ModelState[key].Errors)
                    {
                        _logger.LogInformation($"{key}: {error.ErrorMessage}");
                    }
                }

                // To retain the dropdown selection
                LoadDropDowns();

                return Page();
            }

            if (ModelState.IsValid)
            {
                if (sdwan.Contains("sdwan"))
                {
                    string entity = PoPListx.Entity;
                    DateTime? due = PoPListx.DueDate;
                    DateTime csd = PoPListx.CoverStartDate;
                    DateTime ced = PoPListx.CoverEndDate;
                    int vat = PoPListx.VATPercent;
                    int wht = PoPListx.WHTPercent;

                    string dueX = due.ToString();
                    string startX = csd.ToString();
                    string endX = ced.ToString();
                    string siX = PoPListx.SI_Number;
                    string currX = PoPListx.Currency;
                    string vatX = vat.ToString();
                    string whtX = wht.ToString();
                    string remX = PoPListx.Remarks;
                    string createX = PoPListx.CreatedBy;

                    int VATx = PoPListx.VATPercent;
                    int WHTx = PoPListx.WHTPercent;

                    using (var conn = new SqlConnection(_configuration.GetConnectionString("ProductionConnection")))
                    {
                        conn.Open();

                        using (var coms = new SqlCommand("SP_IFC", conn))
                        {
                            coms.CommandType = System.Data.CommandType.StoredProcedure;
                            coms.CommandText = "SP_IFC";

                            coms.Parameters.AddWithValue("@Action", "SDWAN");
                            coms.Parameters.AddWithValue("@Pop", popQ);
                            coms.Parameters.AddWithValue("@Due", dueX);
                            coms.Parameters.AddWithValue("@Start", startX);
                            coms.Parameters.AddWithValue("@End", endX);
                            coms.Parameters.AddWithValue("@SI", siX);
                            coms.Parameters.AddWithValue("@CURRENCY", currX);
                            coms.Parameters.AddWithValue("@VAT", VATx);
                            coms.Parameters.AddWithValue("@WHT", WHTx);
                            coms.Parameters.AddWithValue("@REM", remX);
                            coms.Parameters.AddWithValue("@CREATE", createX);

                            coms.ExecuteNonQuery();
                        }
                    }

                    _logger.LogInformation("Successfully added the SDWAN!");

                    // LOAD THE TABLE WITH DATA IN IT

                    PoPLists = _db.PoP.ToList();

                    LoadDropDowns();

                    _logger.LogInformation("Role: " + userRole);
                    // Generate the HTML for the table and other elements
                    var htmlContent = GenerateHtmlContent(userRole, PoPListx.PoP_Id);
                    var AdditionalContent = GenerateHtmlAnotherContent(popQ);
                    var signatories = GenerateHtmlSignatories(deptName, PoPListx.PoP_Id);

                    // TempData["Success"] = "Purpose of Payment details added successfully.";
                    CurrencyQ = Request.Form["currency"];

                    // Return JSON data with necessary updates
                    return new JsonResult(new { success = true, html = htmlContent, additionalHtml = AdditionalContent, signatoriesHtml = signatories });
                }
                else
                {
                    // Set the CreatedDate
                    PoPListx.CreateDate = DateTime.Now;

                    await _db.PoP.AddAsync(PoPListx);
                    await _db.SaveChangesAsync();

                    #region Start of Changes for logging newly added data
                    _auditChanges.AddCreateAudit(_db, PoPListx.Id, nameof(PoPList), User.Identity.Name);
                    #endregion

                    _logger.LogInformation("PoP was added successfully.");

                    PoPLists = _db.PoP.ToList();

                    // To retain the dropdown selection
                    LoadDropDowns();

                    _logger.LogInformation("Role: " + userRole);
                    // Generate the HTML for the table and other elements
                    var htmlContent = GenerateHtmlContent(userRole, PoPListx.PoP_Id);
                    var AdditionalContent = GenerateHtmlAnotherContent(popQ);
                    var signatories = GenerateHtmlSignatories(deptName, PoPListx.PoP_Id);

                    TempData["Success"] = "Purpose of Payment details added successfully.";
                    CurrencyQ = Request.Form["currency"];


                    return RedirectToPage("/RFP/rfpEdit", new { id = rfpFormx.RFP_No });
                    // Return JSON data with necessary updates
                    //return new JsonResult(new { success = true, html = htmlContent, additionalHtml = AdditionalContent, signatoriesHtml = signatories });
                }
            }

            string errorMessage = ModelState.Values.FirstOrDefault(v => v.Errors.Any())?.Errors.FirstOrDefault()?.ErrorMessage;
            TempData["Error"] = errorMessage ?? "Failed to add Purpose of Payment details.";

            // To retain the dropdown selection
            LoadDropDowns();

            return RedirectToPage("/RFP/rfpEdit", new { id = rfpFormx.RFP_No });
            //return new JsonResult(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) });
        }

        // ================================== Utility Function Code ==================================

        // For Dropdown Contents
        private void LoadDropDowns()
        {
            cbEntities = GetEntities();
            cbPayees = GetPayees();
            cbToC = GetToCs();
            cbMoPs = GetMoPs();
            cbCurrencies = GetCurrencies();
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
        private List<SelectListItem> GetPayees()
        {
            var udept = _db.Users.Where(u => u.Name == User.Identity.Name).Select(u => u.Department).FirstOrDefault();
            var payees = _db.Payees
                .Where(e => e.IsActive && (e.PayeeDepartment == udept || e.PayeeDepartment == "ALL"))
                .OrderBy(e => e.PayeeName)
                .ToList();

            if (payees != null && payees.Any())
            {
                return payees.Select(d => new SelectListItem
                {
                    Text = d.PayeeName,
                    Value = d.PayeeName.ToString()
                }).ToList();
            }
            return new List<SelectListItem>();
        }
        private List<SelectListItem> GetToCs()
        {
            var udept = _db.Users.Where(u => u.Name == User.Identity.Name).Select(u => u.Department).FirstOrDefault();
            var tocs = _db.TOCs
                .Where(e => !e.IsArchived && (e.TOCDepartment == udept || e.TOCDepartment == "ALL"))
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

        private List<SelectListItem> GetMoPs()
        {
            try
            {
                using (var conn = new SqlConnection(_configuration.GetConnectionString("ProductionConnection")))
                {
                    conn.Open();

                    string query = "SELECT List FROM dbo.DropdownList WHERE Active = 1 AND Description = 'MODE OF PAYMENT'";

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

        // For fetching data

        public JsonResult OnGetStoresForEntity(string ent)
        {
            _logger.LogInformation($"GetStoresForEntity found: {ent}");


            // Handle the change event on the server side
            var stores = _db.Stores
                .Where(p => p.Entity == ent && p.IsOpen == true)
                .Select(p => new { StoreCode = p.StoreCode, StoreName = p.StoreName })
                .OrderBy(p => p.StoreCode)
                .ToList();

            foreach (var store in stores)
            {
                _logger.LogInformation($"Store found: StoreCode = {store.StoreCode}, StoreName = {store.StoreName}");
            }


            if (stores != null)
            {
                return new JsonResult(stores);
            }

            return new JsonResult(new List<object>());
        }

        public JsonResult OnGetChargeToInfo(string popId, string popEntity, int? id)
        {
            _logger.LogInformation($"ChargeToInfo found: {popId}");
            var chargeToList = new List<string>();
            var storeOptions = new List<string>();

            if (id.HasValue)
            {
                // Fetch ChargeTo from rfpForms table
                chargeToList = _db.PoP
                    .Where(p => p.Id == id && p.PoP_Id == popId && p.Entity == popEntity)
                    .Select(p => p.ChargeTo)
                    .Distinct()
                    .ToList();
            }
            else
            {
                // Fetch ChargeTo from rfpForms table
                chargeToList = _db.PoP
                    .Where(p => p.PoP_Id == popId && p.Entity == popEntity)
                    .Select(p => p.ChargeTo)
                    .Distinct()
                    .ToList();
            }

            // Fetch Store options from Store table
            storeOptions = _db.Stores
                .Where(s => s.Entity == popEntity && s.IsOpen == true)
                .Select(s => $"{s.StoreCode} - {s.StoreName}")
                .Distinct()
                .ToList();

            // Remove items from storeOptions that are also in chargeToList
            storeOptions = storeOptions.Except(chargeToList).ToList();

            // Combine ChargeTo and updated Store options
            var combinedOptions = chargeToList.Union(storeOptions).ToList();

            if (combinedOptions.Any())
            {
                return new JsonResult(combinedOptions);
            }

            return new JsonResult(null);
        }

       

        private async Task<IEnumerable<SelectListItem>> GetStoreDetailsAsync()
        {
            var query = "SELECT Entity, CONCAT(StoreCode, '-', StoreName) AS StoreDetails FROM Stores";

            using (var connection = new SqlConnection("ProductionConnection"))
            {
                var storeDetails = await connection.QueryAsync<StoreDetail>(query);

                // Convert to SelectListItem
                return storeDetails.Select(sd => new SelectListItem
                {
                    Value = sd.Entity,
                    Text = sd.StoreDetails
                }).ToList();
            }
        }

        public class StoreDetail
        {
            public string Entity { get; set; }
            public string StoreDetails { get; set; }
        }



        // ================================== Generation of New HTML Layout ==================================


        private string GenerateHtmlContent(string userRole, string popQ)
        {
            StringBuilder htmlBuilder = new StringBuilder();


            htmlBuilder.Append("<table class=\"table table-hover text-center my-2 py-0 mx-auto\" style=\"margin-top: 20px;\">");
            htmlBuilder.Append("<thead style=\"height: 50px;\">");
            htmlBuilder.Append("<tr style=\"height: 40px;\">");

            // Apply sticky headers to all the <th> elements
            htmlBuilder.Append("<th class=\"sticky-top\" style=\"position: sticky; top: 0; z-index: 0;\">Charge To</th>");
            htmlBuilder.Append("<th class=\"sticky-top\" style=\"position: sticky; top: 0; z-index: 0;\">Operation Controls</th>");
            htmlBuilder.Append("<th class=\"sticky-top\" style=\"position: sticky; top: 0; z-index: 0;\">APV</th>");
            htmlBuilder.Append("<th class=\"sticky-top\" style=\"position: sticky; top: 0; z-index: 0;\">APV Posted Date</th>");
            htmlBuilder.Append("<th class=\"sticky-top\" style=\"position: sticky; top: 0; z-index: 0;\">CDJ</th>");

            htmlBuilder.Append("<th class=\"sticky-top\">CDJ Posted Date</th>");
            htmlBuilder.Append("<th class=\"sticky-top\">Due Date</th>");
            htmlBuilder.Append("<th class=\"sticky-top\">Cover Start Date</th>");
            htmlBuilder.Append("<th class=\"sticky-top\">Cover End Date</th>");
            htmlBuilder.Append("<th class=\"sticky-top\">OR #</th>");
            htmlBuilder.Append("<th class=\"sticky-top\">DR #</th>");
            htmlBuilder.Append("<th class=\"sticky-top\">SI # / Service Invoice</th>");
            //htmlBuilder.Append("<th class=\"sticky-top\">Service Invoice #</th>");
            htmlBuilder.Append("<th class=\"sticky-top\">PO #</th>");
            htmlBuilder.Append("<th class=\"sticky-top\">Basic Amt</th>");
            htmlBuilder.Append("<th class=\"sticky-top\">VAT %</th>");
            htmlBuilder.Append("<th class=\"sticky-top\">VAT Amount</th>");
            htmlBuilder.Append("<th class=\"sticky-top\">Gross Amt</th>");
            htmlBuilder.Append("<th class=\"sticky-top\">WHT %</th>");
            htmlBuilder.Append("<th class=\"sticky-top\">WHT Amt</th>");
            htmlBuilder.Append("<th class=\"sticky-top\">Net Amt</th>");
            htmlBuilder.Append("<th class=\"sticky-top\">Remarks</th>");
            htmlBuilder.Append("<th class=\"sticky-top\">Type of Charge</th>");

            htmlBuilder.Append("</tr>");
            htmlBuilder.Append("</thead>");

            htmlBuilder.Append("<tbody>");
            if (_db.PoP != null)
            {
                foreach (var obj in _db.PoP.Where(p => p.PoP_Id == popQ))
                {
                    htmlBuilder.Append($"<td style='display:none;'>{obj.Id}</td>");
                   
                    htmlBuilder.Append($"<td>{obj.ChargeTo}</td>");
                    htmlBuilder.Append("<td>");  // Begin the <td> for actions

                    htmlBuilder.Append("<div style='display: inline-block;'>");  // Wrap buttons in a container

                    htmlBuilder.Append($"<a href=\"/RFP/popEdit?id={obj.Id}\" class=\"btn btn-outline-info\" style=\"margin-right: 10px;\">Edit</a>");

                    if (userRole == "0" || creatorName == User.Identity.Name || new[] { "-0", "-1" }.Any(level => userRole.Contains(level)))
                    {
                        htmlBuilder.Append($"<a href=\"/RFP/popDelete?id={obj.Id}\" class=\"btn btn-outline-danger\">Delete</a>");
                    }
                    else
                    {
                        htmlBuilder.Append($"<a href=\"/RFP/popDelete?id={obj.Id}\" class=\"btn btn-outline-secondary disabled\">Delete</a>");
                    }
                  
                    htmlBuilder.Append("</div>");  
                    htmlBuilder.Append("</td>"); 
                    htmlBuilder.Append($"<td>{obj.Ap_Voucher}</td>");
                    htmlBuilder.Append($"<td>{obj.Ap_Voucher_Posted_Date}</td>");
                    htmlBuilder.Append($"<td>{obj.Cdj_Number}</td>");
                    htmlBuilder.Append($"<td>{obj.CDJ_Num_Posted_Date}</td>");
                    htmlBuilder.Append($"<td>{((obj.DueDate != null) ? obj.DueDate.Value.ToString("MM/dd/yyyy") : "")}</td>");
                    htmlBuilder.Append($"<td>{obj.CoverStartDate.ToString("MM/dd/yyyy")}</td>");
                    htmlBuilder.Append($"<td>{obj.CoverEndDate.ToString("MM/dd/yyyy")}</td>");
                    htmlBuilder.Append($"<td>{obj.OR_Number}</td>");
                    htmlBuilder.Append($"<td>{obj.DR_Number}</td>");
                    //htmlBuilder.Append($"<td>{obj.ServiceInvoice}</td>");
                    htmlBuilder.Append($"<td>{obj.SI_Number}</td>");
                    htmlBuilder.Append($"<td>{obj.PO_Number}</td>");
                    htmlBuilder.Append($"<td>{Mathify.getBasicAmt(obj.Amount, obj.VATPercent).ToString("N")}</td>");
                    htmlBuilder.Append($"<td>{obj.VATPercent}</td>");
                    htmlBuilder.Append($"<td>{Mathify.getVATAmt(obj.Amount, obj.VATPercent).ToString("N")}</td>");
                    htmlBuilder.Append($"<td>{obj.Amount.ToString("N")}</td>");
                    htmlBuilder.Append($"<td>{obj.WHTPercent}</td>");
                    htmlBuilder.Append($"<td>{Mathify.getWHTAmt(obj.Amount, obj.VATPercent, obj.WHTPercent).ToString("N")}</td>");
                    htmlBuilder.Append($"<td>{Mathify.getNETAmt(obj.Amount, obj.VATPercent, obj.WHTPercent).ToString("N")}</td>");
                    htmlBuilder.Append($"<td>{obj.Remarks}</td>");
                    htmlBuilder.Append($"<td>{obj.ToC}</td>");                
                    htmlBuilder.Append("<div class=\"d-grid gap-2 d-md-block text-center\">");
                 
                    htmlBuilder.Append("</div>");
                    htmlBuilder.Append("</td>");
                    htmlBuilder.Append("</tr>");
                }
            }
            else
            {
                htmlBuilder.Append("<tr>");
                htmlBuilder.Append("<td colspan=\"18\">");
                htmlBuilder.Append("<p>No RFP found</p>");
                htmlBuilder.Append("</td>");
                htmlBuilder.Append("</tr>");
            }
      
            htmlBuilder.Append("</tbody>");
            htmlBuilder.Append("</table>");
        

            return htmlBuilder.ToString();
        }
        
        private string GenerateHtmlAnotherContent(string popQ)
        {
            StringBuilder htmlBuilderx = new StringBuilder();

            string Amtx = Wordify.getAmountSum(_db, popQ).ToString();
            decimal toConv = Wordify.getAmountSum(_db, popQ);
            string Wordx = Wordify.convertAmtToWord(toConv, CurrencyQ);

            decimal? AmtDisplay; // Declare the variable outside the if-else block

            if (Amtx == "0.00" || Amtx == "0" || Amtx == null)
            {
                AmtDisplay = null; // Default value when Amtx is "0"
            }
            else
            {
                AmtDisplay = decimal.Parse(Amtx);
            }

            string display = AmtDisplay.HasValue ? AmtDisplay.Value.ToString("N") : "";

            htmlBuilderx.Append("                <div class=\"row align-items-center\">");
            htmlBuilderx.Append("                    <div class=\"col-2 px-0\">");
            htmlBuilderx.Append("                        <p class=\"text-right my-2 pr-1\" style=\"margin-top:8px;\"><b>AMOUNT:</b></p>");
            htmlBuilderx.Append("                    </div>");
            htmlBuilderx.Append("                    <div class=\"col-2 px-0\">");
            htmlBuilderx.Append("                        <div class=\"row pl-3\">");
            htmlBuilderx.Append("                            <div class=\"col-4 px-0 mx-0\">");
            htmlBuilderx.Append($"                                <input class=\"form-control py-0\" id=\"currency\" value=\"{ CurrencyQ }\" disabled />");
            htmlBuilderx.Append("                            </div>");
            htmlBuilderx.Append("                            <div class=\"col-8 px-0 mx-0\">");
            htmlBuilderx.Append($"                                <input class=\"form-control py-0\" id=\"amountInput\" name=\"amountInput\" value=\"{ display }\" readonly />");
            htmlBuilderx.Append("                            </div>");
            htmlBuilderx.Append("                        </div>");
            htmlBuilderx.Append("                    </div>");
            htmlBuilderx.Append("                    <div class=\"col-8 px-0\"> ");
            htmlBuilderx.Append("                        <div class=\"row px-3 py-1 align-items-center\"> ");
            htmlBuilderx.Append("                            <div class=\"col-2 px-0\"> ");
            htmlBuilderx.Append("                                <p class=\"text-right my-2 pr-1\"><b>AMOUNT IN WORDS:</b></p> ");
            htmlBuilderx.Append("                            </div> ");
            htmlBuilderx.Append("                            <div class=\"col-10 px-0\"> ");
            htmlBuilderx.Append($"                                <textarea class=\"form-control py-0\" id=\"amountInWords\" readonly rows=\"2\"> { Wordx } </textarea>");
            htmlBuilderx.Append("                            </div> ");
            htmlBuilderx.Append("                        </div> ");
            htmlBuilderx.Append("                    </div> ");
            htmlBuilderx.Append("                </div>");

            return htmlBuilderx.ToString();
        }

        // ============================================== FOR DISPLAYING SIGNATORIES ==========================================

        private string GenerateHtmlSignatories(string deptName, string popQ)
        {
            StringBuilder htmlBuilderx = new StringBuilder();
            decimal Amtx = Wordify.getAmountSum(_db, popQ);
            string checkedBy = _db.Departments
                        .Where(e => e.DeptName == deptName)
                        .Select(e => e.DeptHead)
                        .FirstOrDefault();

            string notedBy = "Takuya Gotou"; //Fixed name for Noted By
            string approvedBy = "";

            if (Amtx < 50000)
            {
                approvedBy = "Tsukasa Nakanishi";
            }
            else
            {
                approvedBy = "Masaaki Senoo";
            }

            htmlBuilderx.Append("<div class=\"row align-items-center\">");
            htmlBuilderx.Append("<div class=\"col-2 px-0\">");
            htmlBuilderx.Append("<p class=\"text-right my-2 pr-1\" style=\"margin-top:8px;\">CHECKED BY:</p>");
            htmlBuilderx.Append("</div>");
            htmlBuilderx.Append("<div class=\"col-2 px-0\">");
            htmlBuilderx.Append($"<input class=\"form-control py-0\" type=\"text\" id=\"checked\" name=\"checked\" value=\"{ checkedBy }\" />");
            htmlBuilderx.Append("</div>");
            htmlBuilderx.Append("<div class=\"col-2 px-0\">");
            htmlBuilderx.Append("<p class=\"text-right my-2 pr-1\" style=\"margin-top:8px;\">NOTED BY:</p>");
            htmlBuilderx.Append("</div>");
            htmlBuilderx.Append("<div class=\"col-2 px-0\">");
            htmlBuilderx.Append($"<input class=\"form-control py-0\" type=\"text\" id=\"noted\" name=\"noted\" value=\"{ notedBy }\" />");
            htmlBuilderx.Append("</div>");
            htmlBuilderx.Append("<div class=\"col-2 px-0\">");
            htmlBuilderx.Append("<p class=\"text-right my-2 pr-1\" style=\"margin-top:8px;\">APPROVED BY:</p>");
            htmlBuilderx.Append("</div>");
            htmlBuilderx.Append("<div class=\"col-2 px-0\">");
            htmlBuilderx.Append($"<input class=\"form-control py-0\" type=\"text\" id=\"approved\" name=\"approved\" value=\"{ approvedBy }\" />");
            htmlBuilderx.Append("</div>");
            htmlBuilderx.Append("</div>");

            return htmlBuilderx.ToString();
        }
    }
}
