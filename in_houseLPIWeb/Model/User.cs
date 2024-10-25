using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace in_houseLPIWeb.Model
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Possible solution to gaps in Id
        public int Id { get; set; }
        [EmailAddress]
        [Required(ErrorMessage = "The Email field is required.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "The Name field is required.")]
        [Column(TypeName = "nvarchar(255) COLLATE Latin1_General_CS_AS")]
        public string Name { get; set; }
        [Required(ErrorMessage = "The Password field is required.")]
        [DataType(DataType.Password)]
        [Column(TypeName = "nvarchar(255) COLLATE Latin1_General_CS_AS")]
        public string Password { get; set; }
        public string Department { get; set; }
        public string UserLevel { get; set; }
        public bool isActive { get; set; }
        public bool IsActive { get; internal set; }
        public DateTime createdDate { get; set; }
    }
}
