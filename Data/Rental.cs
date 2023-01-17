using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleRentSystem.Data
{
    public class Rental
    {
        public int Id { get; set; }

        [Column(TypeName = "Date")]
        public DateTime StartDate { get; set; }

        [Column(TypeName = "Date")]
        public DateTime EndDate { get; set; }

        public DateTime EntryDate { get; set; }

        public int VehicleId { get; set; }

        public bool IsArchived { get; set; }

        public string? UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
