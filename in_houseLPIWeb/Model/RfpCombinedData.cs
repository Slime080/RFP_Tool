using System;

namespace in_houseLPIWeb.Model
{
    public class RfpCombinedData
    {
        public string RFP_No { get; set; }
        public string PoPCode { get; set; }
        public string Entity { get; set; }
        public string ChargeTo { get; set; }
        public string ToC { get; set; }
        public DateTime CoverStartDate { get; set; }
        public DateTime CoverEndDate { get; set; }
        public string OR_Number { get; set; }
        public string DR_Number { get; set; }
        public string SI_Number { get; set; }
        public string PO_Number { get; set; }
        public decimal Amount { get; set; } // Adjusted type to decimal for financial values
        public int VATPercent { get; set; } // Adjusted type to decimal for percentage values
        public int WHTPercent { get; set; }
        public bool IsActive { get; set; }
        public string Voucher { get; set; }
        public string ApprovedDate { get; set; }
        public string LastSettleVoucher { get; set; }
        public string Closed { get; set; }
        public string TOCNAME { get; set; }
        public string Status{ get; set; }

        public string Lock_Status { get; set; }
    }
}
