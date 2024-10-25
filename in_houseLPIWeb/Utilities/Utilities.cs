using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using in_houseLPIWeb.Model;

namespace in_houseLPIWeb.Utilities
{
    public class Mathify
    {
        // Method to calculate the Basic Amount
        public static decimal getBasicAmt(decimal gross, int vat)
        {
            decimal vatDecimal = 1 + ((decimal)vat / 100); // To get the decimal form of VAT
            decimal basic = gross / vatDecimal; // To solve the Basic Amount

            // Round the sum to two decimal places
            return (decimal)Math.Round(basic, 2);
        }

        // Method to calculate the VAT Amount
        public static decimal getVATAmt(decimal gross, int vat)
        {
            decimal vatDecimal = 1 + ((decimal)vat / 100); // To get the decimal form of VAT
            decimal basic = gross / vatDecimal; // To solve the Basic Amount

            decimal vatAmt = gross - basic;  // To solve the VAT Amount

            // Round the sum to two decimal places
            return (decimal)Math.Round(vatAmt, 2);
        }

        // Method to calculate the WHT Amount
        public static decimal getWHTAmt(decimal gross, int vat, int wht)
        {
            decimal vatDecimal = 1 + ((decimal)vat / 100); // To get the decimal form of VAT
            decimal whtDecimal = (decimal)wht / 100; // To get the decimal form of WHT
            decimal basic = gross / vatDecimal; // To solve the Basic Amount

            decimal whtAmt = basic * whtDecimal; // To solve the WHT Amount

            // Round the sum to two decimal places
            return (decimal)Math.Round(whtAmt, 2);
        }

        // Method to calculate the WHT Amount
        public static decimal getNETAmt(decimal gross, int vat, int wht)
        {
            decimal vatDecimal = 1 + ((decimal)vat / 100); // To get the decimal form of VAT
            decimal whtDecimal = (decimal)wht / 100; // To get the decimal form of WHT
            decimal basic = gross / vatDecimal; // To solve the Basic Amount
            decimal whtAmt = basic * whtDecimal; // To solve the VAT Amount

            decimal net = gross - whtAmt; // To solve the NET Amount

            // Round the sum to two decimal places
            return (decimal)Math.Round(net, 2);
        }
    }

    public static class Wordify
    {
        // For Math Logic Content
        public static decimal getAmountSum(DbContext context, string poP_ID)
        {
            decimal sum = context.Set<PoPList>()
                .Where(m => m.PoP_Id == poP_ID)
                .Sum(m => m.Amount);

            // Round the sum to two decimal places
            return (decimal)Math.Round(sum, 2);
        }
        private static readonly string[] Units = { "", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine" };
        private static readonly string[] Teens = { "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
        private static readonly string[] Tens = { "", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };
        public static string convertAmtToWord(decimal amtToConvert, string currency)
        {
            string amtNote = "";
            if (currency == "PHP")
            {
                amtNote = "Pesos Only";
            }
            else if (currency == "YEN")
            {
                amtNote = "Japanese yen";
            }
            else if (currency == "USD")
            {
                amtNote = "US dollars";
            }
            if (amtToConvert == 0)
            {
                return ""; // To display blank if Amount is 0
            }

            int integerPart = (int)Math.Floor(amtToConvert);
            int decimalPart = (int)Math.Round((amtToConvert - integerPart) * 100);

            string words = ConvertToWords(integerPart);

            if (decimalPart > 0)
            {
                words += " and " + decimalPart + "/100 " + amtNote;
            }
            else
            {
                words += " and 00/100 " + amtNote;
            }

            return words;
        }
        private static string ConvertToWords(int number)
        {
            string words = "";

            if (number < 0)
            {
                words += "Negative ";
                number = -number;
            }

            if ((number / 1000000) > 0)
            {
                words += ConvertToWords(number / 1000000) + " Million, ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += ConvertToWords(number / 1000) + " Thousand, ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += ConvertToWords(number / 100) + " Hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (number == 10)
                {
                    words += "Ten";
                }
                else if (number < 10)
                {
                    words += Units[number];
                }
                else if (number < 20)
                {
                    words += Teens[number - 11];
                }
                else
                {
                    words += Tens[number / 10];
                    if (number % 10 > 0)
                    {
                        words += "-" + Units[number % 10];
                    }
                }
            }

            return words;
        }
    }

    public class UserChecker
    {
        public static bool IsActive(DbContext context, string name)
        {
            var result = context.Set<User>()
                .Where(u => EF.Functions.Collate(u.Name, "SQL_Latin1_General_CP1_CS_AS") == name && u.isActive == true)
                .Any();
            return result;
        }
    }

    public class PagePermission
    {
        public static bool HasAccess(DbContext db, string name, string page)
        {
            var userId = db.Set<User>()
                .Where(u => u.Name == name)
                .Select(u => u.Id)
                .FirstOrDefault();

            var permissions = db.Set<Permission>()
                .Where(p => p.UserId == userId)
                .ToList();

            var access = permissions.Any(p => p.UserId == userId && (bool)p.GetType().GetProperty(page).GetValue(p));
            // this action  (bool)p.GetType().GetProperty(page).GetValue(p) dynamically access the page in Permission table

            return access;
        }
    }

    public class AuditChanges
    {
        public async Task AddUpdateAuditAsync(DbContext db, int recordId, string tableName, IDictionary<string, (string OldValue, string NewValue)> changes, string user)
        {
            foreach (var change in changes)
            {
                var auditLog = new AuditLog
                {
                    TableName = tableName,
                    RecordId = recordId,
                    ColumnName = change.Key,
                    OldValue = change.Value.OldValue,
                    NewValue = change.Value.NewValue,
                    ModifiedBy = user,
                    ModifiedDate = DateTime.Now,
                    ActionType = "Update",
                };
                db.Set<AuditLog>().Add(auditLog);
            }

            await db.SaveChangesAsync();
        }

        public async Task AddCreateAuditAsync(DbContext db, int recordId, string tableName, string user)
        {
            var auditLog = new AuditLog
            {
                TableName = tableName,
                RecordId = recordId,
                ColumnName = null,
                OldValue = null,
                NewValue = null,
                ModifiedBy = user,
                ModifiedDate = DateTime.Now,
                ActionType = "Create",
            };
            db.Set<AuditLog>().Add(auditLog);

            await db.SaveChangesAsync();
        }

        public void AddUpdateAudit(DbContext db, int recordId, string tableName, IDictionary<string, (string OldValue, string NewValue)> changes, string user)
        {
            foreach (var change in changes)
            {
                var auditLog = new AuditLog
                {
                    TableName = tableName,
                    RecordId = recordId,
                    ColumnName = change.Key,
                    OldValue = change.Value.OldValue,
                    NewValue = change.Value.NewValue,
                    ModifiedBy = user,
                    ModifiedDate = DateTime.Now,
                    ActionType = "Update",
                };
                db.Set<AuditLog>().Add(auditLog);
            }

            db.SaveChanges();
        }

        public void AddCreateAudit(DbContext db, int recordId, string tableName, string user)
        {
            var auditLog = new AuditLog
            {
                TableName = tableName,
                RecordId = recordId,
                ColumnName = null,
                OldValue = null,
                NewValue = null,
                ModifiedBy = user,
                ModifiedDate = DateTime.Now,
                ActionType = "Create",
            };
            db.Set<AuditLog>().Add(auditLog);

            db.SaveChanges();
        }
    }
}

/*
 This code is reserved for AuditChanges to be called in al pages

private readonly AuditChanges _auditChanges;

public YourPageModel(ApplicationDbContext db, AuditChanges auditChanges)
    {
        _db = db;
        _auditChanges = auditChanges;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        _db.Payees.Add(payee);

        var entry = _db.Entry(payee);
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

        await _db.SaveChangesAsync();

        // For new entries, we log the current state as new values
        if (entry.State == EntityState.Added)
        {
            foreach (var prop in entry.Properties)
            {
                changes.Add(prop.Metadata.Name, (null, prop.CurrentValue?.ToString()));
            }
        }

        await _auditChanges.AddAuditAsync(_db, payee.Id, nameof(Payee), changes);

        return RedirectToPage("Index");
    }
 
 */