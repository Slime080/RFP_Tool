using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    [BindProperties]
    public class rfpViewModel : PageModel
    {
        private readonly WebDbContext _db;
        private readonly ILogger<rfpViewModel> _logger;

        public int RFP_Nox { get; set; }
        public string popC { get; set; }

        public rfpForm rfpFormx { get; set; }
        public PoPList PoPListx { get; set; }
        public IEnumerable<PoPList> PoPLists { get; set; }

        public rfpViewModel(WebDbContext db, ILogger<rfpViewModel> logger)
        {
            _db = db;
            _logger = logger;
        }

        // This will initialize the string to get the PoPCode from the RFP
        public string GeneratedHtml { get; set; } // Add property to store generated HTML
        public string userInfox { get; set; }
        public string CurrencyQ { get; set; }
        public string userRole { get; set; }
        public bool rfpViewAccess { get; set; }
        public bool rfpEditAccess { get; set; }
        public bool rfpPrintAccess { get; set; }
        public bool rfpArchiveAccess { get; set; }
        public bool rfpActvtAccess { get; set; }
        
        public IActionResult OnGet(int id)
        {
            var userInfo = _db.Users.Where(u => u.Name == User.Identity.Name).Select(u => new { Id = u.Id, isActive = u.isActive }).FirstOrDefault();

            var hasPermission = _db.Permissions.Any(p => p.UserId == userInfo.Id && p.RFP_View == true);
            if (!hasPermission || !userInfo.isActive)
            {
                return RedirectToPage("/userLogin/userDenied");
            }

            // Below are the regular functions required to load the page

            RFP_Nox = id;
            rfpFormx = _db.rfpForms.Find(id);
            // Fetch the PoP Code to display all the Purpose of payment list
            var popX = _db.rfpForms
                .Where(t => t.RFP_No == id)
                .Select(t => t.PoPCode)
                .FirstOrDefault();
            CurrencyQ = _db.PoP
                .Where(c => c.PoP_Id == popX)
                .Select(c => c.Currency)
                .FirstOrDefault();
            userRole = _db.Users
                .Where(u => u.Name == User.Identity.Name)
                .Select(u => u.UserLevel)
                .FirstOrDefault();

            popC = popX;
            // End

            PoPLists = _db.PoP.Where(p => p.PoP_Id == popC).ToList();

            userInfox = _db.Users.Where(p => p.Name == User.Identity.Name).Select(p => p.Department).FirstOrDefault();

            GeneratedHtml = GenerateHtmlAnotherContent(popC);

            // for button permission
            rfpViewAccess = PagePermission.HasAccess(_db, User.Identity.Name, "RFP_View");
            rfpEditAccess = PagePermission.HasAccess(_db, User.Identity.Name, "RFP_Edit");
            rfpPrintAccess = PagePermission.HasAccess(_db, User.Identity.Name, "RFP_Print");
            rfpArchiveAccess = PagePermission.HasAccess(_db, User.Identity.Name, "RFP_Archive");
            rfpActvtAccess = PagePermission.HasAccess(_db, User.Identity.Name, "RFP_Activate");

            return null;
        }

        // ================================== Generation of New HTML Layout ==================================

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
            htmlBuilderx.Append($"                                <input class=\"form-control py-0\" id=\"currency\" value=\"{CurrencyQ}\" disabled />");
            htmlBuilderx.Append("                            </div>");
            htmlBuilderx.Append("                            <div class=\"col-8 px-0 mx-0\">");
            htmlBuilderx.Append($"                                <input class=\"form-control py-0\" id=\"amountInput\" name=\"amountInput\" value=\"{ display }\" readonly />");
            htmlBuilderx.Append("                            </div>");
            htmlBuilderx.Append("                        </div>");
            htmlBuilderx.Append("                    </div>");
            htmlBuilderx.Append("                    <div class=\"col-8 px-0\"> ");
            htmlBuilderx.Append("                        <div class=\"row px-3 py-1 align-items-center\"> ");
            htmlBuilderx.Append("                            <div class=\"col-3 px-0\"> ");
            htmlBuilderx.Append("                                <p class=\"text-right my-2 pr-1\"><b>AMOUNT IN WORDS:</b></p> ");
            htmlBuilderx.Append("                            </div> ");
            htmlBuilderx.Append("                            <div class=\"col-9 px-0\"> ");
            htmlBuilderx.Append($"                                <textarea class=\"form-control py-0 w-75 m-2\" id=\"amountInWords\" readonly rows=\"2\"> { Wordx } </textarea>");
            htmlBuilderx.Append("                            </div> ");
            htmlBuilderx.Append("                        </div> ");
            htmlBuilderx.Append("                    </div> ");
            htmlBuilderx.Append("                </div>");

            return htmlBuilderx.ToString();
        }

    }
}
