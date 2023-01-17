using VehicleRentSystem.Services.Interfaces;
using VehicleRentSystem.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;
using VehicleRentSystem.Areas.Identity.Data;
using VehicleRentSystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using VehicleRentSystem.EnumExtensions;

namespace VehicleRentSystem.Services
{
    public class VehicleManagementService : IVehicleManagementService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public VehicleManagementService(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<int> AddVehicle(VehicleModel vehicleModel)
        {
            var newVehicle = new Vehicle()
            {
                Manufacturer = vehicleModel.Manufacturer,
                Model = vehicleModel.Model,
                ProductionYear = vehicleModel.ProductionYear,
                Color = vehicleModel.Color,
                RentalPrice = vehicleModel.RentalPrice,
                Transmission = vehicleModel.TransmissionEnum.GetDisplayName(),
                Body = vehicleModel.BodyEnum.GetDisplayName(),
                Engine = vehicleModel.Engine,
                Description = vehicleModel.Description,
                Fuel = vehicleModel.FuelEnum.GetDisplayName(),
                DrivenAxle = vehicleModel.DrivenAxleEnum.GetDisplayName(),
                CategoryId = vehicleModel.CategoryId,
                MainImageUrl = vehicleModel.MainImageUrl,
                Payload = vehicleModel.Payload
            };

            newVehicle.vehicleGallery = new List<VehicleGallery>();

            foreach (var file in vehicleModel.Gallery)
            {
                newVehicle.vehicleGallery.Add(new VehicleGallery()
                {
                    Name = file.Name,
                    URL = file.URL
                });
            }

            await _context.Vehicles.AddAsync(newVehicle);
            await _context.SaveChangesAsync();

            return newVehicle.Id;

        }

        public async Task<List<VehicleModel>> GetAllVehicles()
        {
            return await _context.Vehicles
                  .Select(vehicle => new VehicleModel()
                  {
                      Id = vehicle.Id,
                      Manufacturer = vehicle.Manufacturer,
                      Model = vehicle.Model,
                      Color = vehicle.Color,
                      Body = vehicle.Body,
                      ProductionYear = vehicle.ProductionYear,
                      RentalPrice = vehicle.RentalPrice,
                      DrivenAxle = vehicle.DrivenAxle,
                      MainImageUrl = vehicle.MainImageUrl
                  }).ToListAsync();
        }

        public async Task<VehicleModel> GetVehicleById(int id)
        {
            return await _context.Vehicles.Where(x => x.Id == id)
                 .Select(vehicle => new VehicleModel()
                 {
                     Id = vehicle.Id,
                     Manufacturer = vehicle.Manufacturer,
                     Model = vehicle.Model,
                     ProductionYear = vehicle.ProductionYear,
                     CategoryId = vehicle.CategoryId,
                     Color = vehicle.Color,
                     Fuel = vehicle.Fuel,
                     Engine = vehicle.Engine,
                     Transmission = vehicle.Transmission,
                     RentalPrice = vehicle.RentalPrice,
                     Body = vehicle.Body,
                     Description = vehicle.Description,
                     Category = vehicle.Category.Name,
                     DrivenAxle = vehicle.DrivenAxle,
                     MainImageUrl = vehicle.MainImageUrl,
                     Payload = vehicle.Payload,
                     Gallery = vehicle.vehicleGallery.Select(g => new GalleryModel()
                     {
                         Id = g.Id,
                         Name = g.Name,
                         URL = g.URL
                     }).ToList(),
                 }).FirstOrDefaultAsync();
        }

        public async Task<int> Edit(VehicleModel vehicleModel, int id)
        {
            var newVehicle = new Vehicle()
            {
                Id = id,
                Manufacturer = vehicleModel.Manufacturer,
                Model = vehicleModel.Model,
                ProductionYear = vehicleModel.ProductionYear,
                Color = vehicleModel.Color,
                RentalPrice = vehicleModel.RentalPrice,
                Transmission = vehicleModel.Transmission,
                Body = vehicleModel.Body,
                Engine = vehicleModel.Engine,
                Description = vehicleModel.Description,
                Fuel = vehicleModel.Fuel,
                DrivenAxle = vehicleModel.DrivenAxle,
                CategoryId = vehicleModel.CategoryId,
                MainImageUrl = vehicleModel.MainImageUrl,
                Payload = vehicleModel.Payload
            };

            newVehicle.vehicleGallery = new List<VehicleGallery>();

            if (vehicleModel.Gallery != null)
            {
                if (_context.VehicleGalleries.Any(v => v.VehicleId == id && v.Name == "Noimage"))
                {
                    _context.VehicleGalleries.RemoveRange(_context.VehicleGalleries.Where(x => x.VehicleId == id && x.Name == "Noimage"));
                }

                foreach (var file in vehicleModel.Gallery)
                {
                    newVehicle.vehicleGallery.Add(new VehicleGallery()
                    {
                        Name = file.Name,
                        URL = file.URL
                    });
                }
            }
            _context.Vehicles.Update(newVehicle);
            await _context.SaveChangesAsync();

            return newVehicle.Id;

        }

        public async Task<int> DeleteMainImage(VehicleModel vehicleModel)
        {
            var d = _context.Vehicles.Where(v => v.Id == vehicleModel.Id).Select(v => v.Id).Single();

            var record = _context.Vehicles.FirstOrDefault(v => v.Id == d);

            File.Delete($"{_webHostEnvironment.WebRootPath}/{record.MainImageUrl}");

            record.MainImageUrl = "/vehicles/noImage.jpg";

            await _context.SaveChangesAsync();

            return record.Id;

        }

        public async Task<int> DeleteGallery(VehicleModel vehicleModel)
        {
            var d = _context.Vehicles.Where(v => v.Id == vehicleModel.Id).Select(v => v.Id).Single();

            var record = _context.Vehicles.FirstOrDefault(v => v.Id == d);

            foreach (var item in _context.VehicleGalleries.Where(v => v.VehicleId == vehicleModel.Id).Select(v => v.URL).ToList())
            {
                File.Delete($"{_webHostEnvironment.WebRootPath}/{item}");
            }

            _context.VehicleGalleries.RemoveRange(_context.VehicleGalleries.Where(x => x.VehicleId == record.Id));

            var newGallery = new VehicleGallery()
            {
                VehicleId = vehicleModel.Id,
                Name = "NoImage",
                URL = "/vehicles/noImage.jpg",
            };

            await _context.VehicleGalleries.AddAsync(newGallery);

            await _context.SaveChangesAsync();

            return newGallery.Id;

        }


    }
}
