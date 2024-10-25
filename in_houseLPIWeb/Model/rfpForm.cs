using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace in_houseLPIWeb.Model
{
    public class rfpForm
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // Inform EF Core that the database will generate the key PLEASE CHANGE FROM NONE TO IDENTITY WHEN MIGRATING BUT MAKE IT NONE BEFORE PUBLISHING
        public int RFP_No { get; set; }
        [Required]
        public string Payee { get; set; }
        [Required]
        public string MoP { get; set; }
        [Required]
        public string ToP { get; set; }
        [Required]
        [Column(TypeName = "date")]
        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; }
        [Required]
        public string PoPCode { get; set; }
        public string Remarks { get; set; }
        public string Checked { get; set; }
        public string Noted { get; set; }
        public string Approved { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; } = true;
        public string Ap_Voucher { get; set; }
        public string Cdj_Number { get; set; }
        public string Status { get; set; }
        public DateTime? Ap_Voucher_Posted_Date { get; set; }
        public DateTime? CDJ_Num_Posted_Date { get; set; }
    }
}
