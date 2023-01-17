
namespace VehicleRentSystem.Data
{
    public class VehicleGallery
    {
        public int Id { get; set; }

        public int VehicleId { get; set; }

        public string Name { get; set; }

        public string URL { get; set; }

        public Vehicle Vehicle { get; set; }

    }
}
