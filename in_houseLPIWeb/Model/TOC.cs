using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace in_houseLPIWeb.Model
{
    public class TOC
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Possible solution to gaps in Id
        public int Id { get; set; }
        [Required]
        [Display(Name = "Type of Charge")]
        public string TOCName { get; set; }

        [Display(Name = "Type of Charge description")]
        public string TOCdescription { get; set; }
        public bool IsArchived { get; set; } = false;
        [Display(Name = "Department")]
        public string TOCDepartment { get; set; } ////// Changes
    }
}
