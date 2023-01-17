using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using VehicleRentSystem.Enums;

namespace VehicleRentSystem.Data
{
    public class Vehicle
    {
        public int Id { get; set; }

        public string Manufacturer { get; set; }

        public string Model { get; set; }

        public string Body { get; set; }

        public int ProductionYear { get; set; }

        public string Color { get; set; }

        public string Engine { get; set; }

        public int RentalPrice { get; set; }

        public string Description { get; set; }

        public string Fuel { get; set; }

        public string Transmission { get; set; }

        public string DrivenAxle { get; set; }

        public string MainImageUrl { get; set; }

        public ICollection<VehicleGallery> vehicleGallery { get; set; }

        public Category Category { get; set; }

        public int CategoryId { get; set; }

        public string Payload { get; set; }
    }
}
