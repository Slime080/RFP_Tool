using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace in_houseLPIWeb.Model
{
    public class Department
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Possible solution to gaps in Id
        public int Id { get; set; }

        [Required]
        [Display(Name = "Department Name")]
        public string DeptName { get; set; }
        [Required]
        [Display(Name = "Department Head")]
        public string DeptHead { get; set; }

        [Required]
        public bool isActive { get; set; }
    }
}
