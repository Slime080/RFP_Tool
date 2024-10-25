using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using in_houseLPIWeb.Data;
using in_houseLPIWeb.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Text.Encodings.Web;
using in_houseLPIWeb.Utilities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Dapper;

namespace in_houseLPIWeb.Pages.RFP
{
    [Authorize]
    public class rfpAddModel : PageModel
    {
        private readonly WebDbContext _db;
        private readonly ILogger<rfpAddModel> _logger;
        private readonly IConfiguration _configuration;
        private readonly AuditChanges _auditChanges;////// Changes
        public string GeneratedPopCode { get; set; }
        public IEnumerable<PoPList> PoPLists { get; set; }
        public IEnumerable<Entity> Entities { get; set; }
        public List<SelectListItem> cbEntities { get; set; }
        public List<SelectListItem> cbPayees { get; set; }
        public List<SelectListItem> cbToC { get; set; }
        public List<SelectListItem> cbMoPs { get; set; }
        public List<SelectListItem> cbCurrencies { get; set; }

        public string CurrencyQ { get; set; }
        public string userRole { get; set; }
        public bool billing { get; set; }

        public PoPList PoPListx { get; set; }
        public rfpForm rfpFormx { get; set; }
        public string GeneratedHtml { get; set; } // Add property to store generated HTML
        public string GeneratedSign { get; set; } // Add property to store generated HTML

        public IActionResult OnGetFilteredToCOptions(string searchQuery)
        {
            var allOptions = cbToC; // Get the list of all options
            var filteredOptions = allOptions
                .Where(option => option.Text.ToLower().Contains(searchQuery.ToLower()))
                .Select(option => new { option.Value, option.Text })
                .ToList();

            return new JsonResult(filteredOptions);
        }




        public rfpAddModel(WebDbContext db, ILogger<rfpAddModel> logger, IConfiguration configuration, AuditChanges auditChanges)////// Changes
        {
            _db = db;
            _logger = logger;
            _configuration = configuration;
            _auditChanges = auditChanges;////// Changes
        }

        // This function is used for getting necessary data on loading this page
        public IActionResult OnGet()
        {
            var userInfo = _db.Users.Where(u => u.Name == User.Identity.Name).Select(u => new { Id = u.Id, isActive = u.isActive }).FirstOrDefault();

            var hasPermission = _db.Permissions.Any(p => p.UserId == userInfo.Id && p.RFP_Add == true);
            if (!hasPermission || !userInfo.isActive)
            {
                return RedirectToPage("/userLogin/userDenied");
            }

            // Below are the regular functions required to load the page

            cbMoPs = GetMoPs();
            cbCurrencies = GetCurrencies();
            cbEntities = GetEntities();
            cbPayees = GetPayees();
            cbToC = GetToCs();

            var storename = _db.Stores.ToDictionary(p => p.StoreCode, p => p.StoreName);

            ViewData["StoreName"] = storename;

            // Check if PopCode is already in session
            if (HttpContext.Session.TryGetValue("PopCode", out byte[] popCodeBytes))
            {
                GeneratedPopCode = Encoding.UTF8.GetString(popCodeBytes);

                PoPLists = _db.PoP.Where(p => p.PoP_Id == GeneratedPopCode).ToList();
            }
            else
            {
                // Generate a new PopCode
                GeneratedPopCode = GenerateRandomString(16);

                // Store it in session
                HttpContext.Session.Set("PopCode", Encoding.UTF8.GetBytes(GeneratedPopCode));
            }

            CurrencyQ = _db.PoP.Where(c => c.PoP_Id == GeneratedPopCode).Select(c => c.Currency).FirstOrDefault();

            GeneratedHtml = GenerateHtmlAnotehrContent(GeneratedPopCode, CurrencyQ);

            TempData["PreviousPage"] = "/RFP/rfpAdd";

            userRole = _db.Users
                        .Where(u => u.Name == User.Identity.Name)
                        .Select(u => u.UserLevel)
                        .FirstOrDefault();
            string deptx = _db.Users
                        .Where(u => u.Name == User.Identity.Name)
                        .Select(u => u.Department)
                        .FirstOrDefault();

            GeneratedSign = GenerateHtmlSignatories(deptx, GeneratedPopCode);

            return null;
        }

        // This function is used to add the RFP values from form to database
        [BindProperty(Name = "rfpFormx.PropertyToInclude")]
        public string rfpFormPropertyToInclude { get; set; }
        public IActionResult OnPostAddRFPAsync(rfpForm rfpFormx)
        {
            string checkedBy = Request.Form["checked"];
            string notedBy = Request.Form["noted"];
            string approvedBy = Request.Form["approved"];

            rfpFormx.Checked = checkedBy.ToUpper();
            rfpFormx.Noted = notedBy.ToUpper();
            rfpFormx.Approved = approvedBy.ToUpper();
            rfpFormx.CreatedBy = User.Identity.Name;
            rfpFormx.CreatedDate = DateTime.Now;
            rfpFormx.Status = "Open";

            _logger.LogInformation("payee: " + rfpFormx.Payee);
            _logger.LogInformation("mop: " + rfpFormx.MoP);
            _logger.LogInformation("top: " + rfpFormx.ToP);
            _logger.LogInformation("duedate: " + rfpFormx.DueDate);
            _logger.LogInformation("popcode: " + rfpFormx.PoPCode);
            _logger.LogInformation("remarks: " + rfpFormx.Remarks);
            _logger.LogInformation("checked: " + rfpFormx.Checked);
            _logger.LogInformation("noted: " + rfpFormx.Noted);
            _logger.LogInformation("approved: " + rfpFormx.Approved);
            _logger.LogInformation("createdby: " + rfpFormx.CreatedBy);
            _logger.LogInformation("createdate: " + rfpFormx.CreatedDate);
            _logger.LogInformation("status: " + rfpFormx.Status);
            _logger.LogInformation("isactive: " + rfpFormx.IsActive);

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

            if (!ModelState.IsValid)
            {
                _logger.LogInformation("PopCode: " + rfpFormx.PoPCode);
                foreach (var key in ModelState.Keys)
                {
                    foreach (var error in ModelState[key].Errors)
                    {
                        //Console.WriteLine($"{key}: {error.ErrorMessage}");
                        _logger.LogInformation($"{key}: {error.ErrorMessage}");
                    }
                }
                return Page();
            }

            if (ModelState.IsValid)
            {
                _db.rfpForms.Add(rfpFormx);
                _db.SaveChanges();

                #region Start of Changes for logging newly added data
                _auditChanges.AddCreateAudit(_db, rfpFormx.RFP_No, nameof(rfpForm), User.Identity.Name);
                #endregion

                TempData["Success"] = "Request of Payment added successfully.";

                int newRFP = _db.rfpForms
                    .Where(e => e.PoPCode == rfpFormx.PoPCode)
                    .Select(e => e.RFP_No)
                    .FirstOrDefault();

                //int addedRFP_No = rfpFormx.RFP_No;

                // To retain the dropdown selection
                LoadDropDowns();

                HttpContext.Session.Remove("PopCode");
                TempData.Remove("PreviousPage");

                return RedirectToPage("/RFP/rfpView", new { id = newRFP });
            }

            string errorMessage = ModelState.Values.FirstOrDefault(v => v.Errors.Any())?.Errors.FirstOrDefault()?.ErrorMessage;
            TempData["Error"] = errorMessage ?? "Failed to add Request of Payment details.";

            // To retain the dropdown selection
            LoadDropDowns();

            return Page();
        }

        // This function is used to add the PoP values from form to database
        [BindProperty(Name = "PoPListx.Property1")]
        public string PoPListProperty1 { get; set; }
        public async Task<IActionResult> OnPostAddPOPAsync(PoPList PoPListx)
        {
            if (string.IsNullOrEmpty(PoPListx.ToC))
            {
                ModelState.AddModelError("PoPListx.ToC", "Please don't leave blank.");
            }
            string popQ = PoPListx.PoP_Id;
            string userRole = _db.Users
                        .Where(u => u.Name == User.Identity.Name)
                        .Select(u => u.UserLevel)
                        .FirstOrDefault();
            string deptName = _db.Users
                        .Where(u => u.Name == User.Identity.Name)
                        .Select(u => u.Department)
                        .FirstOrDefault();
            string sdwan = "";
            if (PoPListx.ChargeTo != null)
            {
               sdwan = PoPListx.ChargeTo.ToLower();
            }
            
            _logger.LogInformation("ChargeTo: " + sdwan);

            if (sdwan.Contains("sdwan"))
            {
                if (PoPListx.CoverStartDate == DateTime.MinValue)
                {
                    ModelState.AddModelError("PoPListx.CoverStartDate", "Please don't leave blank.");
                }
                if (PoPListx.CoverEndDate == DateTime.MinValue)
                {
                    ModelState.AddModelError("PoPListx.CoverEndDate", "Please don't leave blank.");
                }
            }

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
                    var serviceInvoice = PoPListx.ServiceInvoice;
               
                  
                    try
                    {
                        using (var conn = new SqlConnection(_configuration.GetConnectionString("ProductionConnection")))
                        {
                            conn.Open();

                            using (var coms = new SqlCommand("SP_IFC", conn))
                            {
                                coms.CommandType = System.Data.CommandType.StoredProcedure;

                                // Add parameters to the SqlCommand
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

                                coms.Parameters.AddWithValue("@ServiceInvoice", PoPListx.ServiceInvoice ?? (object)DBNull.Value);                         

                                // Execute the stored procedure
                                coms.ExecuteNonQuery();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log or handle the exception
                        Console.WriteLine("Error executing stored procedure: " + ex.Message);
                    }

                    _logger.LogInformation("Successfull add the SDWAN!");

                    // LOAD THE TABLE WITH DATA IN IT

                    PoPLists = _db.PoP.ToList();

                    LoadDropDowns();

                    _logger.LogInformation("Role: " + userRole);
                    // Generate the HTML for the table and other elements
                    var htmlContent = GenerateHtmlContent(userRole, PoPListx.PoP_Id);
                    var AdditionalContent = GenerateHtmlAnotehrContent(PoPListx.PoP_Id, PoPListx.Currency);
                    var signatories = GenerateHtmlSignatories(deptName, PoPListx.PoP_Id);

                    // TempData["Success"] = "Purpose of Payment details added successfully.";
                    CurrencyQ = Request.Form["currency"];

                    // Return JSON data with necessary updates
                    return new JsonResult(new { success = true, html = htmlContent, additionalHtml = AdditionalContent, signatoriesHtml = signatories });
                }
                else
                {
                    PoPListx.Status = "Unpaid";

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
                    var AdditionalContent = GenerateHtmlAnotehrContent(PoPListx.PoP_Id, PoPListx.Currency);
                    var signatories = GenerateHtmlSignatories(deptName, PoPListx.PoP_Id);

                    // TempData["Success"] = "Purpose of Payment details added successfully.";
                    CurrencyQ = Request.Form["currency"];

                    // Return JSON data with necessary updates
                    return new JsonResult(new { success = true, html = htmlContent, additionalHtml = AdditionalContent, signatoriesHtml = signatories });
                }
            }

            // To retain the dropdown selection
            LoadDropDowns();

            return new JsonResult(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) });
        }

       


        // This function is use to generate random string for the unique PoPCode
        static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder stringBuidler = new StringBuilder();

            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                stringBuidler.Append(chars[random.Next(chars.Length)]);
            }

            return stringBuidler.ToString();
        }

        private void LoadDropDowns()
        {
            cbMoPs = GetMoPs();
            cbCurrencies = GetCurrencies();
            cbEntities = GetEntities();
            cbPayees = GetPayees();
            cbToC = GetToCs();
        }

        // Below are the functions for the dropdown lists from the RFP Add Form
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
        // this is for getting the types of charges drop down
        //private List<SelectListItem> GetToCs()
        //{
        //    var udept = _db.Users.Where(u => u.Name == User.Identity.Name).Select(u => u.Department).FirstOrDefault();
        //    var tocs = _db.TOCs
        //        .Where(e => !e.IsArchived && (e.TOCDepartment == udept || e.TOCDepartment == "ALL"))
        //        .OrderBy(e => e.TOCName)
        //        .ToList();

        //    if (tocs != null && tocs.Any())
        //    {
        //        return tocs.Select(d => new SelectListItem
        //        {
        //            Text = d.TOCName,
        //            Value = d.TOCName
        //        }).ToList();
        //    }
        //    return new List<SelectListItem>();
        //}
        private List<SelectListItem> GetToCs()
        {
            // Populate your list as before
            var udept = _db.Users
                .Where(u => u.Name == User.Identity.Name)
                .Select(u => u.Department)
                .FirstOrDefault();
            var tocs = _db.TOCs
                .Where(e => !e.IsArchived && (e.TOCDepartment == udept || e.TOCDepartment == "ALL"))
                .OrderBy(e => e.TOCName)
                .Select(d => new SelectListItem
                {
                    Text = d.TOCName,
                    Value = d.TOCName
                }).ToList();
            return tocs;
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

        // This function will be use for javascript to access the store list
        public JsonResult OnGetStoresForEntity(string ent)
        {
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

        // ============================================ FOR DISPLAYING DYNAMIC VALUES =========================================

        // This function dynamically adds the PoP table to the RFP Add form view
        private string GenerateHtmlContent(string userRole, string popQ)
        {
            StringBuilder htmlBuilder = new StringBuilder();

            htmlBuilder.Append("<div class=\"table-container\">");
            htmlBuilder.Append("<table class=\"table table-hover text-center my-2 py-0 mx-auto\">");
            htmlBuilder.Append("<thead>");
            htmlBuilder.Append("<tr>");
            htmlBuilder.Append("<th>Charge To</th>");
            htmlBuilder.Append("<th>Due Date</th>");
            htmlBuilder.Append("<th>Cover Start Date</th>");
            htmlBuilder.Append("<th>Cover End Date</th>");
            htmlBuilder.Append("<th>OR #</th>");
            htmlBuilder.Append("<th>DR #</th>");
            htmlBuilder.Append("<th>SI #</th>");
            htmlBuilder.Append("<th>Service Invoice #</th>");
            htmlBuilder.Append("<th>PO #</th>");
            htmlBuilder.Append("<th>Basic Amt</th>");
            htmlBuilder.Append("<th>VAT %</th>");
            htmlBuilder.Append("<th>VAT Amount</th>");
            htmlBuilder.Append("<th>Gross Amt</th>");
            htmlBuilder.Append("<th>WHT %</th>");
            htmlBuilder.Append("<th>WHT Amt</th>");
            htmlBuilder.Append("<th>Net Amt</th>");
            htmlBuilder.Append("<th>Remarks</th>");
            htmlBuilder.Append("<th>TOC</th>");
            htmlBuilder.Append("<th>Operation Controls</th>");
            htmlBuilder.Append("</tr>");
            htmlBuilder.Append("</thead>");
            htmlBuilder.Append("<tbody>");
            if (_db.PoP != null)
            {
                foreach (var obj in _db.PoP.Where(p => p.PoP_Id == popQ))
                {
                    htmlBuilder.Append("<tr>");
                    htmlBuilder.Append($"<td>{obj.ChargeTo}</td>");
                    htmlBuilder.Append($"<td>{((obj.DueDate != null) ? obj.DueDate.Value.ToString("MM/dd/yyyy") : "")}</td>");
                    htmlBuilder.Append($"<td>{obj.CoverStartDate.ToString("MM/dd/yyyy")}</td>");
                    htmlBuilder.Append($"<td>{obj.CoverEndDate.ToString("MM/dd/yyyy")}</td>");  
                    htmlBuilder.Append($"<td>{obj.OR_Number}</td>");
                    htmlBuilder.Append($"<td>{obj.DR_Number}</td>");
                    htmlBuilder.Append($"<td>{obj.SI_Number}</td>");
                    htmlBuilder.Append($"<td>{obj.ServiceInvoice}</td>");
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
                    htmlBuilder.Append("<td>");
                    htmlBuilder.Append("<div class=\"d-grid gap-2 d-md-block text-center\">");
                    htmlBuilder.Append($"<a href=\"/RFP/popEdit?id={obj.Id}\" class=\"redirect-link btn btn-outline-info disabled\" hidden>Edit</a>");
                    if (userRole == "0" || new[] { "-0", "-1" }.Any(level => userRole.Contains(level)))
                    {
                        htmlBuilder.Append($"<a href=\"/RFP/popDelete?id={obj.Id}\" class=\"redirect-link btn btn-outline-danger\" hidden>Delete</a>");
                    }
                    else
                    {
                        htmlBuilder.Append($"<a href=\"/RFP/popDelete?id={obj.Id}\" class=\"redirect-link btn btn-outline-secondary disabled\" hidden>Delete</a>");
                    }
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
            htmlBuilder.Append("</div>");

            return htmlBuilder.ToString();
        }

        // This function dynamically adds the amount fields from the RFP Add form view
        private string GenerateHtmlAnotehrContent(string popQ, string currency)
        {
            StringBuilder htmlBuilderx = new StringBuilder();

            string Amtx = Wordify.getAmountSum(_db, popQ).ToString();
            decimal toConv = Wordify.getAmountSum(_db, popQ);
            string Wordx = Wordify.convertAmtToWord(toConv, currency);
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
            htmlBuilderx.Append("                        <p class=\"text-right my-2 pr-1\" style=\"margin-top:8px;\"><b>AMOUNT</b>:</p>");
            htmlBuilderx.Append("                    </div>");
            htmlBuilderx.Append("                    <div class=\"col-2 px-0\">");
            htmlBuilderx.Append("                        <div class=\"row pl-3\">");
            htmlBuilderx.Append("                            <div class=\"col-4 px-0 mx-0\">");
            htmlBuilderx.Append($"                                <input class=\"form-control py-0\" id=\"currency\" value=\"{currency}\" disabled />");
            htmlBuilderx.Append("                            </div>");
            htmlBuilderx.Append("                            <div class=\"col-8 px-0 mx-0\">");
            htmlBuilderx.Append($"                                <input class=\"form-control py-0\" id=\"amountInput\" name=\"amountInput\" value=\"{ display }\" readonly />");
            htmlBuilderx.Append("                            </div>");
            htmlBuilderx.Append("                        </div>");
            htmlBuilderx.Append("                    </div>");
            htmlBuilderx.Append("                    <div class=\"col-8 px-0\"> ");
            htmlBuilderx.Append("                        <div class=\"row px-3 align-items-center\"> ");
            htmlBuilderx.Append("                            <div class=\"col-2 px-0\"> ");
            htmlBuilderx.Append("                               <p class=\"text-right my-2 pr-1\" style=\"font-size: 1em;\"><b>AMOUNT IN WORDS:</b></p>");   
            htmlBuilderx.Append("                            </div> ");
            htmlBuilderx.Append("                            <div class=\"col-10 px-0\"> ");
            htmlBuilderx.Append($"                              <textarea class=\"form-control py-0 w-50\" id=\"amountInWords\" readonly rows=\"2\">{Wordx}</textarea>");
            htmlBuilderx.Append("                            </div> ");
            htmlBuilderx.Append("                        </div> ");
            htmlBuilderx.Append("                    </div> ");
            htmlBuilderx.Append("                </div>");

            return htmlBuilderx.ToString();
        }

        // ============================================== FOR DISPLAYING SIGNATORIES ==========================================

        // This function dynamically adds the signatory names for the RFP Add form view
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
            htmlBuilderx.Append("<p class=\"text-right my-2 pr-2\" style=\"margin-top:8px; \"><b>CHECKED BY:</b></p>");
            htmlBuilderx.Append("</div>");
            htmlBuilderx.Append("<div class=\"col-2 pl-0\">");
            htmlBuilderx.Append($"<input class=\"form-control py-0 w-100 ml-0\" type=\"text\" id=\"checked\" name=\"checked\" value=\"{checkedBy}\" />");
            htmlBuilderx.Append("</div>");
            htmlBuilderx.Append("<div class=\"col-1.2 pl-4 \">");
            htmlBuilderx.Append("<p class=\"text-right my-2 pl-3 pr-2\" style=\"margin-top:8px;\"><b>NOTED BY:</b></p>");
            htmlBuilderx.Append("</div>");
            htmlBuilderx.Append("<div class=\"col-2 px-0\">");
            htmlBuilderx.Append($"<input  class=\"form-control w-150\" py-0\" type=\"text\" id=\"noted\" name=\"noted\" value=\"{ notedBy }\" />");
            htmlBuilderx.Append("</div>");
            htmlBuilderx.Append("<div class=\"col-2 px-2\">");
            htmlBuilderx.Append("<p class=\"text-right my-2 pr-1\" style=\"margin-top:8px;\"><b>APPROVED BY:</b></p>");
            htmlBuilderx.Append("</div>");
            htmlBuilderx.Append("<div class=\"col-2 px-0\">");
            htmlBuilderx.Append($"<input class=\"form-control py-0\" type=\"text\" id=\"approved\" name=\"approved\" value=\"{ approvedBy }\" />");
            htmlBuilderx.Append("</div>");
            htmlBuilderx.Append("</div>");

            return htmlBuilderx.ToString();
        }
    }
}
