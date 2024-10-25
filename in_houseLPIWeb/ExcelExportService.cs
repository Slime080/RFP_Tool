using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Table;

namespace in_houseLPIWeb
{
    public class ExcelExportService
    {
        public byte[] ExportDataToExcel<T>(List<T> data, string worksheetName)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add(worksheetName);
                worksheet.Cells.LoadFromCollection(data, true);

                // Check if data is not null before proceeding
                if (data != null && data.Any())
                {

                    // Apply specific formatting to columns if needed
                    worksheet.Column(4).Style.Numberformat.Format = "mm/dd/yyyy";
                    worksheet.Column(5).Style.Numberformat.Format = "mm/dd/yyyy";
                    worksheet.Column(6).Style.Numberformat.Format = "mm/dd/yyyy";
                    worksheet.Column(8).Style.Numberformat.Format = "#,##0.00";
                    worksheet.Column(10).Style.Numberformat.Format = "#,##0.00";
                    worksheet.Column(11).Style.Numberformat.Format = "#,##0.00";
                    worksheet.Column(13).Style.Numberformat.Format = "#,##0.00";
                    worksheet.Column(14).Style.Numberformat.Format = "#,##0.00";
                    worksheet.Column(20).Style.Numberformat.Format = "#,##0.00";
                    worksheet.Column(21).Style.Numberformat.Format = "#,##0.00";
                    worksheet.Column(22).Style.Numberformat.Format = "#,##0.00";
                    worksheet.Column(25).Style.Numberformat.Format = "#,##0.00";
                    worksheet.Column(26).Style.Numberformat.Format = "#,##0.00";
                    worksheet.Column(27).Style.Numberformat.Format = "#,##0.00";


                    // Create a table (ListObject) for the data
                    string tableName = "ifcExpenses";
                    //string tableName = $"ifcExpenses_{DateTime.Now.ToString("yyyyMMddHHmmss")}";
                    var table = worksheet.Tables.Add(new ExcelAddressBase(1, 1, data.Count + 1, typeof(T).GetProperties().Length), tableName);
                    table.TableStyle = TableStyles.Light1;
                    // Table formatting
                    table.ShowFilter = true;

                    // Auto-fit columns for the entire worksheet
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                }

                return package.GetAsByteArray();
            }
        }
    }
}
