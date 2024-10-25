using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace in_houseLPIWeb.Model
{
    public class StoreType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Possible solution to gaps in Id
        public int Id { get; set; }
        [Required(ErrorMessage = "Display Name field is required. Acronym for Store Type Name.")]
        [Display(Name = "Store Type Code")]
        public string TypeCode { get; set; }
        [Required(ErrorMessage = "Store Type Name field is required.")]
        [Display(Name = "Store Type Name")]
        public string TypeName { get; set; }
        [Display(Name = "Store Type Description")]
        public string TypeDescription { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
