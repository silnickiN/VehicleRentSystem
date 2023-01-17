using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using VehicleRentSystem.Models;

namespace VehicleRentSystem.Services.Interfaces
{
    public interface IVehicleManagementService
    {
        Task<List<VehicleModel>> GetAllVehicles();

        Task<VehicleModel> GetVehicleById(int id);

        Task<int> AddVehicle(VehicleModel vehicle);

        Task<int> Edit(VehicleModel newVehicle, int id);

        Task<int> DeleteMainImage(VehicleModel newVehicle);

        Task<int> DeleteGallery(VehicleModel newVehicle);
    }
}
