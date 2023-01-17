using VehicleRentSystem.Models;

namespace VehicleRentSystem.Services.Interfaces
{
    public interface IVehicleRentalService
    {
        Task<int> AddRental(RentalModel rental);

        Task<List<RentalModel>> GetAllRentals();

        Task<List<RentalModel>> GetUserRentals(HttpContext httpContext);

        Task<RentalModel> GetRentalById(int id);

        Task<int> ChangeStatus(RentalModel rentalModel);
    }
}
