using System.ComponentModel.DataAnnotations;

namespace VehicleRentSystem.Enums
{
    public enum BodyEnum
    {
        [Display(Name = "-")]
        X,

        Sedan,

        Wagon,

        Coupe,

        Convertible,

        Hatchback,

        Suv,

        Pickup,

        Van,
    }
}
