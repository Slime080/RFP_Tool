using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace in_houseLPIWeb.Model
{
    [Keyless]
    public class ifcView
    {
        [NotMapped]
        public string RFP_No { get; set; }
        [NotMapped]
        public string ChargeTo { get; set; }
        [NotMapped]
        public string StoreType { get; set; }
        [NotMapped]
        public DateTime OpenDate { get; set; }
        [NotMapped]
        public DateTime CoverStartDate { get; set; }
        [NotMapped]
        public DateTime CoverEndDate { get; set; }
        [NotMapped]
        public string ToC { get; set; }
        [NotMapped]
        public decimal GrossAmt { get; set; }
        [NotMapped]
        public string VATPerc { get; set; }
        [NotMapped]
        public decimal VATAmt { get; set; }
        [NotMapped]
        public decimal BasicAmt { get; set; }
        [NotMapped]
        public string WHTPerc { get; set; }
        [NotMapped]
        public decimal WHTAmt { get; set; }
        [NotMapped]
        public decimal NETAmt { get; set; }
        [NotMapped]
        public string ProrateResult { get; set; }
        [NotMapped]
        public int ProrateDays { get; set; }
        [NotMapped]
        public string ProrateDivident { get; set; }
        [NotMapped]
        public string IFCProratePerc { get; set; }
        [NotMapped]
        public string LPIProratePerc { get; set; }
        [NotMapped]
        public decimal MatrixAmtResult { get; set; }
        [NotMapped]
        public decimal IFCPartAmt { get; set; }
        [NotMapped]
        public decimal LPIPartAmt { get; set; }
        [NotMapped]
        public int MatrixVATResult { get; set; }
        [NotMapped]
        public int IFCMatrixVat { get; set; }
        [NotMapped]
        public decimal LPIMatrixVat { get; set; }
        [NotMapped]
        public decimal TotalIFCPart { get; set; }
        [NotMapped]
        public decimal TotalLPIPart { get; set; }
    }
}
