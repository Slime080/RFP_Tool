using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace in_houseLPIWeb.Model
{
    [Keyless]
    public class Login
    {
        [NotMapped]
        public int Id { get; set; }
        [NotMapped]
        [Required]
        public string Name { get; set; }
        [NotMapped]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [NotMapped]
        public string UserLevel { get; set; }
        [NotMapped]
        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
