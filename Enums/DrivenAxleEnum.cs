using System.ComponentModel.DataAnnotations;

namespace VehicleRentSystem.Enums
{
    public enum DrivenAxleEnum
    {
        [Display(Name = "-")]
        X,

        FWD,

        RWD,

        AWD
    }
}
