using VehicleRentSystem.Areas.Identity.Data;
using VehicleRentSystem.Models;
using VehicleRentSystem.Data;
using VehicleRentSystem.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace VehicleRentSystem.Services
{
    public class VehicleRentalService : IVehicleRentalService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public VehicleRentalService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<int> AddRental(RentalModel rentalModel)
        {
            var newRental = new Rental()
            {
                StartDate = rentalModel.StartDate,
                EndDate = rentalModel.EndDate,
                VehicleId = rentalModel.VehicleId,
                UserId = rentalModel.UserId,
                FirstName = rentalModel.FirstName,
                LastName = rentalModel.LastName,
                EntryDate = rentalModel.EntryDate = DateTime.Now,
                IsArchived = rentalModel.IsArchived = false,
            };

            await _context.Rentals.AddAsync(newRental);
            await _context.SaveChangesAsync();

            return newRental.Id;
        }

        public async Task<List<RentalModel>> GetAllRentals()
        {
            return await _context.Rentals
                  .Select(rental => new RentalModel()
                  {
                      Id = rental.Id,
                      UserId = rental.UserId,
                      FirstName = rental.FirstName,
                      LastName = rental.LastName,
                      StartDate = rental.StartDate,
                      EndDate = rental.EndDate,
                      EntryDate = rental.EntryDate,
                      VehicleId = rental.VehicleId,
                      Vehicle = _context.Vehicles.Where(v => v.Id == rental.VehicleId).Select(v => v.Manufacturer + " " + v.Model).FirstOrDefault(),
                      IsArchived = rental.IsArchived
                  }).ToListAsync();
        }

        public async Task<List<RentalModel>> GetUserRentals(HttpContext httpContext)
        {
            var user = await _userManager.GetUserAsync(httpContext.User);

            return await _context.Rentals
                  .Where(r => r.UserId == user.Id)
                  .Select(rental => new RentalModel()
                  {
                      Id = rental.Id,
                      FirstName = rental.FirstName,
                      LastName = rental.LastName,
                      StartDate = rental.StartDate,
                      EndDate = rental.EndDate,
                      EntryDate = rental.EntryDate,
                      VehicleId = rental.VehicleId,
                      Vehicle = _context.Vehicles.Where(v => v.Id == rental.VehicleId).Select(v => v.Manufacturer + " " + v.Model).FirstOrDefault()                 
                  }).ToListAsync();
        }

        public async Task<RentalModel> GetRentalById(int id)
        {
            return await _context.Rentals.Where(x => x.Id == id)
                 .Select(rental => new RentalModel()
                 {
                     Id = rental.Id,
                     UserId = rental.UserId,
                     FirstName = rental.FirstName,
                     LastName = rental.LastName,
                     EntryDate = rental.EntryDate,
                     StartDate = rental.StartDate,
                     EndDate = rental.EndDate,
                     PhoneNumber = _context.Users.Where(u => u.Id == rental.UserId).Select(u => u.PhoneNumber).FirstOrDefault(),
                     Vehicle = _context.Vehicles.Where(v => v.Id == rental.VehicleId).Select(v => v.Manufacturer + " " + v.Model).FirstOrDefault()
                 }).FirstOrDefaultAsync();
        }

        public async Task<int> ChangeStatus(RentalModel rentalModel)
        {
            var d = _context.Rentals.Where(r => r.Id == rentalModel.Id).Select(r => r.Id).Single();

            var record = _context.Rentals.FirstOrDefault(r => r.Id == d);

            record.IsArchived = true;

            await _context.SaveChangesAsync();

            return record.Id;

        }

    }
}
