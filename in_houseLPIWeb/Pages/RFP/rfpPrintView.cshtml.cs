using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using in_houseLPIWeb.Data;
using in_houseLPIWeb.Model;
using in_houseLPIWeb.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace in_houseLPIWeb.Pages.RFP
{
    [Authorize]
    public class rfpPrintViewModel : PageModel
    {
        private readonly WebDbContext _db;
        private readonly ILogger<rfpPrintViewModel> _logger;
        public int RFP_Nox { get; set; }
        public string popC { get; set; }
        public string Amtx { get; set; }
        public string Wordx { get; set; }
        public string userInfox { get; set; }
        public string CurrencyQ { get; set; }
        public bool rfpPrintAccess { get; set; }

        public rfpForm rfpFormx { get; set; }
        public PoPList PoPListx { get; set; }
        public IEnumerable<PoPList> PoPLists { get; set; }

        public string GeneratedAmountWordHtml { get; set; } // Add property to store generated HTML
        public rfpPrintViewModel(WebDbContext db, ILogger<rfpPrintViewModel> logger)
        {
            _db = db;
            _logger = logger;
        }
        // Property to set the number of rows per page
        public List<PoPList> InitialRows { get; set; } // Rows displayed on the first page
        public List<PoPList> RemainingRows { get; set; } // Rows displayed on subsequent pages
        public int CurrentPage { get; set; } // Current page number
        public int TotalPages { get; set; } // Total number of pages
        public int rowsPerPage { get; set; }

        public string fullName { get; set; }
        public bool withLineBreak { get; set; }

        public IActionResult OnGet(int id)
        {
            var userInfo = _db.Users.Where(u => u.Name == User.Identity.Name).Select(u => new { Id = u.Id, isActive = u.isActive }).FirstOrDefault();

            var hasPermission = _db.Permissions.Any(p => p.UserId == userInfo.Id && p.RFP_Print == true);
            if (!hasPermission || !userInfo.isActive)
            {
                return RedirectToPage("/userLogin/userDenied");
            }

            // Below are the regular functions required to load the page

            RFP_Nox = id;
            rfpFormx = _db.rfpForms.Find(id);

            string userX = User.Identity.Name;

            string uEmail = getUserEmail(userX);

            string newName = ExtractFullNameFromEmail(uEmail).ToUpper();


            fullName = newName;

            // Fetch the PoP Code to display all the Purpose of payment list
            var popX = _db.rfpForms
                .Where(t => t.RFP_No == id)
                .Select(t => t.PoPCode)
                .FirstOrDefault();
            CurrencyQ = _db.PoP
                .Where(c => c.PoP_Id == popX)
                .Select(c => c.Currency)
                .FirstOrDefault();

            popC = popX;
            // End

            PoPLists = _db.PoP.Where(p => p.PoP_Id == popC).ToList();

            decimal gross = Wordify.getAmountSum(_db, popC);
            decimal? grossDisplay; // Declare the variable outside the if-else block

            if (gross == 0)
            {
                grossDisplay = null; // Default value when Amtx is "0"
            }
            else
            {
                grossDisplay = gross;
            }

            string display = grossDisplay.HasValue ? grossDisplay.Value.ToString("N") : "";

            Amtx = display;

            decimal toConv = Wordify.getAmountSum(_db, popC);

            Wordx = Wordify.convertAmtToWord(toConv, CurrencyQ);

            userInfox = _db.Users.Where(p => p.Name == User.Identity.Name).Select(p => p.Department).FirstOrDefault();


            withLineBreak = ContainsLineBreaks(rfpFormx.Remarks);

            bool ContainsLineBreaks(string input)
            {
                if (string.IsNullOrEmpty(input))
                {
                    return false;
                }

                string trimmedInput = input.TrimEnd('\r', '\n');

                // Check for Environment.NewLine and individual newline characters
                return trimmedInput.Contains(Environment.NewLine) || trimmedInput.Contains("\n");
            }

            int totalRows = PoPLists.Count();
            //rowsPerPage = CalculateRowsPerPage();
            if (CheckColumnValueExist(popX))
            {
                rowsPerPage = CalculateRowsPerPage(35);
                _logger.LogInformation("Height: 35");
            }
            else
            {
                rowsPerPage = CalculateRowsPerPage(20);
                _logger.LogInformation("Height: 20");
            }

            TotalPages = (int)Math.Ceiling((double)totalRows / rowsPerPage + 1);

            CurrentPage = 1;

            // Calculate the index range for the current page
            int startIndex = (CurrentPage - 1) * rowsPerPage;

            if (CurrentPage == 1)
            {
                // Populate initial rows with the first 5 rows
                InitialRows = PoPLists.Take(10).ToList();
            }
            else
            {
                // Populate initial rows based on the index range
                InitialRows = PoPLists.Skip(InitialRows.Count()).Take(rowsPerPage).ToList();
            }

            // Populate remaining rows based on the index range
            RemainingRows = PoPLists.Skip(10).ToList();

            _logger.LogInformation("Row Per Page: " + rowsPerPage);
            _logger.LogInformation("Initial Rows: " + InitialRows.Count());
            _logger.LogInformation("Remaining Rows: " + RemainingRows.Count());
            _logger.LogInformation("Current Page: " + CurrentPage);
            _logger.LogInformation("Total Page: " + TotalPages);

            rfpPrintAccess = PagePermission.HasAccess(_db, User.Identity.Name, "RFP_Print");

            return null;
        }

        private static string ExtractFullNameFromEmail(string email)
        {
            int atIndex = email.IndexOf('@');
            if (atIndex != -1)
            {
                string namePart = email.Substring(0, atIndex);

                // Split the name part based on dots and hyphen
                string[] nameParts = namePart.Split('.', '-');

                // Check if there are at least two segments after splitting
                if (nameParts.Length >= 3)
                {
                    // Capitalize each word in the second to last segments
                    for (int i = 1; i < nameParts.Length - 1; i++)
                    {
                        nameParts[i] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(nameParts[i].ToLower());
                    }

                    // Join the segments, removing hyphens, to form the full name
                    string fullName = string.Join(" ", nameParts, 1, nameParts.Length - 2).Replace("-", "");
                    return fullName;
                }
            }
            return "";
        }

        public string userEmail { get; set; }
        private string getUserEmail(string namex)
        {
            return userEmail = _db.Users
                    .Where(e => e.Name == namex)
                    .Select(e => e.Email)
                    .FirstOrDefault();
        }

        private int CalculateRowsPerPage(int height)
        {
            // Adjust this value based on your layout and available space
            int availableSpacePerPage = 875; // Example value in pixels

            // Determine the height of each row in the table (adjust as needed)
            int rowHeight = height; // Example value in pixels

            // Determine the height of other elements on the page (headers, footers, margins, etc.)
            int otherElementsHeight = 120; // Example value in pixels

            // Calculate the available space for the table content on a page
            int availableTableSpacePerPage = availableSpacePerPage - otherElementsHeight;

            // Calculate the number of rows that will fit on a page
            int rowsPerPage = availableTableSpacePerPage / rowHeight;

            return rowsPerPage;
        }

        public bool CheckColumnValueExist(string code)
        {
            var exists = _db.PoP
                        .Any(x => x.PoP_Id == code &&
                        (!string.IsNullOrEmpty(x.OR_Number) || !string.IsNullOrEmpty(x.SI_Number) || !string.IsNullOrEmpty(x.DR_Number) || !string.IsNullOrEmpty(x.PO_Number)));
            return exists;
        }
    }
}
