using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace in_houseLPIWeb.Model
{
    public class Payee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Possible solution to gaps in Id
        public int Id { get; set; }
        [Required(ErrorMessage = "The Payee Name field is required.")]
        [Display(Name = "Payee Name")]
        public string PayeeName { get; set; }
        
        [Display(Name = "Payee Address")]
        public string PayeeAddress { get; set; }
        [Required]
        public bool IsActive { get; set; } = true;
        [Display(Name = "Department")]
        public string PayeeDepartment { get; set; } ////// Changes
    }
}
