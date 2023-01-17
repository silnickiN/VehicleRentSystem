using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleRentSystem.Models
{
    public class RentalModel
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Start date")]
        [Column(TypeName = "Date")]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        [Column(TypeName = "Date")]
        public DateTime EndDate { get; set; }

        public DateTime EntryDate { get; set; }

        [Display(Name = "Vehicle")]
        public int VehicleId { get; set; }

        public string? UserId { get; set; }

        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        public string LastName { get; set; }

        public string? Vehicle { get; set; }

        public string? PhoneNumber { get; set; }

        public bool IsArchived { get; set; }
    }
}