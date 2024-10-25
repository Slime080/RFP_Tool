using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace in_houseLPIWeb.Model
{
    public class Store
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Possible solution to gaps in Id
        public int Id { get; set; }
        [Required]
        [Display(Name = "Store Code")]
        public string StoreCode { get; set; }
        [Required]
        public string Entity { get; set; }
        [Required]
        [Display(Name = "Store Name")]
        public string StoreName { get; set; }
        [Display(Name = "Store Type")]
        public string StoreType { get; set; }
        [Display(Name = "Contract Code")]
        public string ContractCode { get; set; }
        [Display(Name = "Business Name")]
        public string BusinessName { get; set; }
        [Display(Name = "Contract Name")]
        public string ContractName { get; set; }
        public bool isSDWAN { get; set; }
        [Display(Name = "Monthly Recurring Charge")]
        [Column(TypeName = "decimal(12,2)")]
        public decimal MRC { get; set; }
        [Column(TypeName = "date")]
        [Display(Name = "Open Date")]
        public DateTime OpenDate { get; set; }
        [Column(TypeName = "date")]
        [Display(Name = "Close Date")]
        public DateTime? CloseDate { get; set; }
        public bool IsOpen { get; set; } = true;
    }
}
