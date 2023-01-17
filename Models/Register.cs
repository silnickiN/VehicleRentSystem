using System.ComponentModel.DataAnnotations;

namespace VehicleRentSystem.Models
{
    public class Register
    {

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        [EmailAddress]
        public string Email { get; set; }
    }
}
