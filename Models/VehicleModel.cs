using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using VehicleRentSystem.Enums;

namespace VehicleRentSystem.Models
{
    public class VehicleModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Manufacturer { get; set; }

        [Required]
        public string Model { get; set; }

        [Required]
        public string Color { get; set; }

        [Required]
        public string Engine { get; set; }

        [Required]
        [Display(Name = "Daily price of rental")]
        public int RentalPrice { get; set; }

        public string Description { get; set; }

        public string? Fuel { get; set; }
        [Display(Name = "Fuel")]
        public FuelEnum FuelEnum { get; set; }

        public string? Body { get; set; }
        [Display(Name = "Body")]
        public BodyEnum BodyEnum { get; set; }

        public string? DrivenAxle { get; set; }
        [Display(Name = "Driven axle")]
        public DrivenAxleEnum DrivenAxleEnum { get; set; }

        [Display(Name = "Year of production")]
        public int ProductionYear { get; set; }

        public string? Transmission { get; set; }
        [Display(Name = "Transmission")]
        public TransmissionEnum TransmissionEnum { get; set; }

        [Display(Name = "Choose the main photo of vehicle")]
        public IFormFile? MainImage { get; set; }
        public string? MainImageUrl { get; set; }

        [Display(Name = "Choose the gallery images of vehicle")]
        public IFormFileCollection? GalleryFiles { get; set; }

        public List<GalleryModel>? Gallery { get; set; }

        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        public string? Category { get; set; }

        public string? Payload { get; set; }


    }
}
