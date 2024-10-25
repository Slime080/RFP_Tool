using in_houseLPIWeb.Data;
using in_houseLPIWeb.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace in_houseLPIWeb.Pages.RFP
{
    public class RFPInventoryModel : PageModel
    {
        private readonly WebDbContext _db;

        public RFPInventoryModel(WebDbContext db)
        {
            _db = db;
        }

        public IEnumerable<RfpCombinedData> RfpDataList { get; set; }

        public async Task OnGetAsync()
        {
            // Execute the second stored procedure for updating the lock status
            await _db.Database.ExecuteSqlRawAsync("EXEC UpdateRfpFormsWithLockStatus");

            // Execute the stored procedure
            RfpDataList = await _db.RfpCombinedData.FromSqlRaw("EXEC GetRfpCDJAPV").ToListAsync();
        }
    }
}
