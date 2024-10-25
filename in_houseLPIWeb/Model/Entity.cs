using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace in_houseLPIWeb.Model
{
    public class Entity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Possible solution to gaps in Id
        public int Id { get; set; }
        [Required(ErrorMessage = "The Entity Code field is required.")]
        [Display(Name = "Entity Code")]
        public string EntityCode { get; set; }
        [Required(ErrorMessage = "The Entity Name field is required.")]
        [Display(Name = "Entity Name")]
        public string EntityName { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
