using System.ComponentModel.DataAnnotations;

namespace VehicleRentSystem.Enums
{
    public enum FuelEnum
    {
        [Display(Name = "-")]
        X,

        Gasoline,

        [Display(Name = "Gasoline + CNG")]
        CNGGasoline,

        [Display(Name = "Gasoline + LPG")]
        LPGGasoline,

        Diesel,

        Electricity,

        Ethanol,

        Hybrid,

        Hydrogen
    }
}
