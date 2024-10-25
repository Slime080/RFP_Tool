using in_houseLPIWeb.Data;
using in_houseLPIWeb.Model;
using in_houseLPIWeb.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace in_houseLPIWeb.Pages.IFC
{
    [Authorize]
    public class ifcIndexModel : PageModel
    {
        private readonly WebDbContext _db;
        private readonly ILogger<ifcIndexModel> _logger;
        private readonly ifcDataService _dataService;
        private readonly ExcelExportService _excelExportService;

        public IEnumerable<rfpForm> rfpForms { get; set; }
        public IEnumerable<PoPList> PoPLists { get; set; }

        public List<SelectListItem> cbChargeTo { get; set; }
        public List<rfpForm> RFPListQ { get; set; }
        public List<PoPList> PoPListQ { get; set; }
        public List<ifcView> ifcViewData { get; set; }
        public string userRole { get; set; }
        public bool ifcViewAccess { get; set; }
        public ifcIndexModel(WebDbContext db, ILogger<ifcIndexModel> logger, ifcDataService ifcService, ExcelExportService excelExportService)
        {
            _db = db;
            _logger = logger;
            _dataService = ifcService;
            _excelExportService = excelExportService;
        }

        public async Task<IActionResult> OnGet()
        {
            var userInfo = _db.Users.Where(u => u.Name == User.Identity.Name).Select(u => new { Id = u.Id, isActive = u.isActive }).FirstOrDefault();

            var hasPermission = _db.Permissions.Any(p => p.UserId == userInfo.Id && p.IFC_Index == true);
            if (!hasPermission || !userInfo.isActive)
            {
                return RedirectToPage("/userLogin/userDenied");
            }

            // Below are the regular functions required to load the page

            userRole = _db.Users
                        .Where(u => u.Name == User.Identity.Name)
                        .Select(u => u.UserLevel)
                        .FirstOrDefault();
            try
            {
                cbChargeTo = GetChargeTo();

                var result = await _dataService.GetDataSPifcViewAsync(0, "ExpensesX"); // Default value "This Month" == 0
                //_logger.LogInformation("Result: {Result}", result);

                ifcViewData = result;
                _logger.LogInformation("Result:", ifcViewData);

                TempData["PreviousPage"] = "/IFC/ifcIndex";

                _logger.LogInformation("Data fetched successfully."); // Log success information
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching data."); // Log error information
            }

            ifcViewAccess = PagePermission.HasAccess(_db, User.Identity.Name, "IFC_Index");

            return null;
        }

        public async Task<IActionResult> OnGetExportToExcel(int param)
        {
            _logger.LogInformation("Search: " + param);

            //string param = Request.Form
            var data = await _dataService.GetDataSPifcViewAsync(param, "Export");

            var excelData = _excelExportService.ExportDataToExcel(data, "Sheet1");

            string dynamicFilename = $"IFC Expenses_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
            return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", dynamicFilename);
        }

        private List<SelectListItem> GetChargeTo()
        {
            var activeRFP = _db.rfpForms
                .Where(x => x.IsActive == true)
                .Select(x => x.PoPCode)
                .Distinct()
                .ToList();
            var chargeTo = _db.PoP
                .Where(d => d.Entity == "1003" && activeRFP.Contains(d.PoP_Id))
                .OrderBy(d => d.ChargeTo)
                .Select(d => new SelectListItem { Text = d.ChargeTo, Value = d.ChargeTo })
                .Distinct()
                .ToList();

            return chargeTo;
        }

        public async Task<IActionResult> OnGetFetchData(int param)
        {
            var result = await _dataService.GetDataSPifcViewAsync(param, "ExpensesX");

            return new JsonResult(result);
        }
    }
}
