using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace in_houseLPIWeb.Model
{
    public class PoPList
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int Id { get; set; }

        [Required(ErrorMessage = "PoP ID is required.")]
        public string PoP_Id { get; set; }

        public string Entity { get; set; }

        [Required(ErrorMessage = "Charge To is required.")]
        public string ChargeTo { get; set; }

        [Column(TypeName = "date")]
        [Display(Name = "Due Date")]
        [Required(ErrorMessage = "Due Date is required.")]
        public DateTime? DueDate { get; set; }

        [Column(TypeName = "date")]
        [Display(Name = "Cover Start Date")]
        public DateTime CoverStartDate { get; set; }

        [Column(TypeName = "date")]
        [Display(Name = "Cover End Date")]
        public DateTime CoverEndDate { get; set; }

        [Required(ErrorMessage = "Type of Charge is required.")]
        [Display(Name = "Type of Charge")]
        public string ToC { get; set; }

        public string OR_Number { get; set; }
        public string SI_Number { get; set; }
        public string DR_Number { get; set; }
        public string PO_Number { get; set; }

        public string Currency { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        [Required(ErrorMessage = "Gross Amount is required.")]
        public decimal Amount { get; set; }

        [Display(Name = "VAT Percent")]
        public int VATPercent { get; set; }

        [Display(Name = "WHT Percent")]
        public int WHTPercent { get; set; }

        public string Remarks { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // New Status column Kurt 8/29/2024
        public string ServiceInvoice { get; set; }
 
        public string Status { get; set; }

        public string Ap_Voucher { get; set; }
        public string Cdj_Number { get; set; }
        public DateTime? Ap_Voucher_Posted_Date { get; set; }
        public DateTime? CDJ_Num_Posted_Date { get; set; }
    }
}
