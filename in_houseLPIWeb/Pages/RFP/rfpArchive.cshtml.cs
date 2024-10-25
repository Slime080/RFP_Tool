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
    public class rfpArchiveModel : PageModel
    {
        private readonly WebDbContext _db;
        private readonly ILogger<rfpArchiveModel> _logger;

        public rfpForm rfpFormx { get; set; }
        public PoPList PoPListx { get; set; }
        public IEnumerable<PoPList> PoPLists { get; set; }
        public rfpArchiveModel(WebDbContext db, ILogger<rfpArchiveModel> logger)
        {
            _db = db;
            _logger = logger;
        }
        public string CurrencyQ { get; set; }
        public string popQ { get; private set; }
        public PoPList NewPoPList { get; private set; }
        public string GeneratedHtml { get; set; } // Add property to store generated HTML
        
        public IActionResult OnGet(int id)
        {
            var userInfo = _db.Users.Where(u => u.Name == User.Identity.Name).Select(u => new { Id = u.Id, isActive = u.isActive }).FirstOrDefault();

            var hasPermission = _db.Permissions.Any(p => p.UserId == userInfo.Id && p.RFP_Archive == true);
            if (!hasPermission || !userInfo.isActive)
            {
                return RedirectToPage("/userLogin/userDenied");
            }

            // Below are the regular functions required to load the page

            rfpFormx = _db.rfpForms.Find(id);

            string popId = _db.rfpForms.Where(e => e.RFP_No == id).Select(e => e.PoPCode).FirstOrDefault();

            //string PopIdFromRFP = rfpFormx.PoPCode;
            PoPLists = _db.PoP.Where(p => p.PoP_Id == popId).ToList(); // To display related Purpose of Payment in specific RFP_No

            popQ = _db.rfpForms
                .Where(e => e.RFP_No == id)
                .Select(e => e.PoPCode)
                .FirstOrDefault();
            CurrencyQ = _db.PoP
                .Where(c => c.PoP_Id == popQ)
                .Select(c => c.Currency)
                .FirstOrDefault();

            NewPoPList = new PoPList
            {
                PoP_Id = popQ
            };

            GeneratedHtml = GenerateHtmlAnotherContent(popQ);

            return null;
        }

        public async Task<IActionResult> OnPost(rfpForm rfpFormx)
        {
            var rfpFromDb = _db.rfpForms.Find(rfpFormx.RFP_No);

            if (rfpFromDb == null)
            {
                TempData["Error"] = "Invalid Request for Payment selected.";
                return RedirectToPage("/RFP/rfpIndex");
            }

            if (rfpFromDb.IsActive == true)
            {
                rfpFromDb.IsActive = false;
                rfpFromDb.ModifiedBy = User.Identity.Name;
                rfpFromDb.ModifiedDate = DateTime.Now;
                rfpFromDb.Status = "Closed";
                await _db.SaveChangesAsync();
                TempData["Success"] = "Request for Payment archived successfully.";
            }
            else
            {
                TempData["Error"] = "Failed to archive Request for Payment.";
                return RedirectToPage("/RFP/rfpIndex");
            }

            return RedirectToPage("/RFP/rfpIndex");
        }

        // ================================== Generation of New HTML Layout ==================================

        private string GenerateHtmlAnotherContent(string popQ)
        {
            StringBuilder htmlBuilderx = new StringBuilder();

            string Amtx = Wordify.getAmountSum(_db, popQ).ToString();

            decimal toConv = Wordify.getAmountSum(_db, popQ);

            string Wordx = Wordify.convertAmtToWord(toConv, CurrencyQ);

            htmlBuilderx.Append("                <div class=\"row align-items-center\">");
            htmlBuilderx.Append("                    <div class=\"col-2 px-0\">");
            htmlBuilderx.Append("                        <p class=\"text-right my-2 pr-1\" style=\"margin-top:8px;\">AMOUNT:</p>");
            htmlBuilderx.Append("                    </div>");
            htmlBuilderx.Append("                    <div class=\"col-2 px-0\">");
            htmlBuilderx.Append("                        <div class=\"row pl-3\">");
            htmlBuilderx.Append("                            <div class=\"col-4 px-0 mx-0\">");
            htmlBuilderx.Append($"                                <input class=\"form-control py-0\" id=\"currency\" value=\"{ CurrencyQ }\" disabled />");
            htmlBuilderx.Append("                            </div>");
            htmlBuilderx.Append("                            <div class=\"col-8 px-0 mx-0\">");
            htmlBuilderx.Append($"                                <input class=\"form-control py-0\" id=\"amountInput\" name=\"amountInput\" value=\"{ decimal.Parse(Amtx).ToString("N") }\" readonly />");
            htmlBuilderx.Append("                            </div>");
            htmlBuilderx.Append("                        </div>");
            htmlBuilderx.Append("                    </div>");
            htmlBuilderx.Append("                    <div class=\"col-8 px-0\"> ");
            htmlBuilderx.Append("                        <div class=\"row p-3 align-items-center\"> ");
            htmlBuilderx.Append("                            <div class=\"col-2 px-0\"> ");
            htmlBuilderx.Append("                                <p class=\"text-right my-2 pr-1\">AMOUNT IN WORDS:</p> ");
            htmlBuilderx.Append("                            </div> ");
            htmlBuilderx.Append("                            <div class=\"col-10 px-0\"> ");
            htmlBuilderx.Append($"                                <textarea class=\"form-control py-0\" id=\"amountInWords\" readonly rows=\"3\"> { Wordx } </textarea>");
            htmlBuilderx.Append("                            </div> ");
            htmlBuilderx.Append("                        </div> ");
            htmlBuilderx.Append("                    </div> ");
            htmlBuilderx.Append("                </div>");

            return htmlBuilderx.ToString();
        }

    }
}
