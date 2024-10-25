using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using in_houseLPIWeb.Data;
using in_houseLPIWeb.Model;
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace in_houseLPIWeb
{
    public class ifcDataService
    {
        private readonly IDbConnection _db;

        public ifcDataService(IDbConnection db)
        {
            _db = db;
        }

        public async Task<List<ifcView>> GetDataSPifcViewAsync(int param, string action)
        {
            var parameters = new { Action = action, Param = param };

            var result = await _db.QueryAsync<ifcView>("SP_IFC", parameters, commandType: CommandType.StoredProcedure);
            return result.ToList();
        }
    }
}
