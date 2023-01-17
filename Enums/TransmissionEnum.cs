using System.ComponentModel.DataAnnotations;

namespace VehicleRentSystem.Enums
{
    public enum TransmissionEnum
    {
        [Display(Name = "-")]
        X,

        Manual,

        Automatic,

        [Display(Name = "Semi-automatic")]
        SemiAutomatic,
    }
}
