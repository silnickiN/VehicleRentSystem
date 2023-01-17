namespace VehicleRentSystem.Data
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Color { get; set; }

        public ICollection<Vehicle> Vehicles { get; set; }

    }
}
