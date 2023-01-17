using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using VehicleRentSystem.Areas.Identity.Data;
using VehicleRentSystem.Data;
using VehicleRentSystem.Models;
using VehicleRentSystem.Services;
using VehicleRentSystem.Services.Interfaces;

namespace VehicleRentSystem.Controllers
{
    public class VehicleRentalController : Controller
    {
        private readonly IVehicleRentalService _vehicleRentalService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public VehicleRentalController(UserManager<ApplicationUser> userManager, IVehicleRentalService vehicleRentalService, ApplicationDbContext context)
        {
            _userManager = userManager;
            _vehicleRentalService = vehicleRentalService;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Administrator, User")]
        public async Task<ViewResult> Rent(bool isSuccess = false, int id = 0)
        {
            var model = new RentalModel();

            ViewBag.IsSuccess = isSuccess;
            ViewBag.Id = id;
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator, User")]
        public async Task<IActionResult> Rent(RentalModel rentalModel, int Id)
        {

            rentalModel.VehicleId = Id;
            rentalModel.UserId = _userManager.GetUserId(HttpContext.User);
            rentalModel.FirstName = _userManager.GetUserAsync(HttpContext.User).Result.FirstName;
            rentalModel.LastName = _userManager.GetUserAsync(HttpContext.User).Result.LastName;

            if (!_context.Vehicles.Any(v => v.Id == Id))
            {
                TempData["InvalidId"] = "Vehicle ID is invalid.";
                return View();
            }

            if (rentalModel.StartDate > rentalModel.EndDate)
            {
                TempData["IncorrectDate"] = "Date is incorrect.";
                return View();
            }

            if ((_context.Rentals.Count(x => x.VehicleId == rentalModel.VehicleId) == 0))
            {
                int id = await _vehicleRentalService.AddRental(rentalModel);
                if (id > 0)
                {
                    return RedirectToAction(nameof(Rent), new { isSuccess = true });
                }
            }

            bool dateOverlap = true;

            foreach (var item in _context.Rentals.Where(x => x.VehicleId == Id))
            {

                dateOverlap = (rentalModel.StartDate <= item.EndDate && item.StartDate <= rentalModel.EndDate);

                if (dateOverlap == true)
                {
                    break;
                }
            }

            if (dateOverlap == false)
            {
                int id = await _vehicleRentalService.AddRental(rentalModel);
                if (id > 0)
                {
                    return RedirectToAction(nameof(Rent), new { isSuccess = true, rentalId = id });
                }
            }

            else
            {
                TempData["VehicleUnavailable"] = "Vehicle is unavailable in given interval.";
                return View();
            }

            return View();
        }

        [Authorize(Roles = "Administrator")]
        public async Task<ViewResult> AddRentalManually(bool isSuccess = false, int id = 0)
        {
            ViewData["Archived"] = "False";

            var model = new RentalModel();

            ViewBag.IsSuccess = isSuccess;
            ViewBag.Id = id;
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> AddRentalManually(RentalModel rentalModel)
        {

            ViewData["Archived"] = "False";

            if (!ModelState.IsValid)
            {
                return View(rentalModel);
            }

            if (rentalModel.StartDate > rentalModel.EndDate)
            {
                TempData["IncorrectDate"] = "Date is incorrect.";
                return View();
            }

            if (_context.Rentals.Count(x => x.VehicleId == rentalModel.VehicleId) == 0)
            {
                int id = await _vehicleRentalService.AddRental(rentalModel);
                if (id > 0)
                {
                    return RedirectToAction(nameof(AddRentalManually), new { isSuccess = true, });
                }
            }

            bool dateOverlap = true;

            foreach (var item in _context.Rentals.Where(x => x.VehicleId == rentalModel.VehicleId))
            {
                dateOverlap = (rentalModel.StartDate <= item.EndDate && item.StartDate <= rentalModel.EndDate);

                if (dateOverlap == true)
                {
                    break;
                }
            }

            if (dateOverlap == false)
            {
                int id = await _vehicleRentalService.AddRental(rentalModel);
                if (id > 0)
                {
                    return RedirectToAction(nameof(AddRentalManually), new { isSuccess = true, rentalId = id, });
                }
            }

            else
            {
                TempData["VehicleUnavailable"] = "Vehicle is unavailable in given interval.";
                return View();
            }

            return View();
        }

        [Authorize(Roles = "Administrator")]
        public async Task<ViewResult> RentalList(string currentSort, string sortOrder, string searchString, string archived, int pg = 1)
        {
            ViewData["FirstNameOrder"] = sortOrder == "firstname_asc" ? "firstname_desc" : "firstname_asc";
            ViewData["LastNameOrder"] = sortOrder == "lastname_asc" ? "lastname_desc" : "lastname_asc";
            ViewData["StartDateOrder"] = sortOrder == "startdate_asc" ? "startdate_desc" : "startdate_asc";
            ViewData["EndDateOrder"] = sortOrder == "enddate_asc" ? "enddate_desc" : "enddate_asc";
            ViewData["EntryDateOrder"] = sortOrder == "entrydate_asc" ? "entrydate_desc" : "entrydate_asc";
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentSort"] = sortOrder;
            ViewData["Archived"] = archived;

            var rentals = await _vehicleRentalService.GetAllRentals();
            var qRentals = rentals.AsQueryable().Where(r => r.IsArchived.ToString() == archived);

            if (!String.IsNullOrEmpty(searchString))
            {
                qRentals = qRentals.Where(r => r.FirstName.ToLower().Contains(searchString.ToLower())
                                       || r.LastName.ToLower().Contains(searchString.ToLower())
                                       || r.Id.ToString().Contains(searchString)
                                       || r.StartDate.ToString().Contains(searchString)
                                       || r.EndDate.ToString().Contains(searchString)
                                       || r.EntryDate.ToString().Contains(searchString));
            }

            switch (sortOrder)
            {
                case "firstname_desc":
                    qRentals = qRentals.OrderByDescending(r => r.FirstName);
                    break;
                case "firstname_asc":
                    qRentals = qRentals.OrderBy(r => r.FirstName);
                    break;
                case "lastname_desc":
                    qRentals = qRentals.OrderByDescending(r => r.LastName);
                    break;
                case "lastname_asc":
                    qRentals = qRentals.OrderBy(r => r.LastName);
                    break;
                case "startdate_desc":
                    qRentals = qRentals.OrderByDescending(r => r.StartDate);
                    break;
                case "startdate_asc":
                    qRentals = qRentals.OrderBy(r => r.StartDate);
                    break;
                case "enddate_desc":
                    qRentals = qRentals.OrderByDescending(r => r.EndDate);
                    break;
                case "enddate_asc":
                    qRentals = qRentals.OrderBy(r => r.EndDate);
                    break;
                case "entrydate_desc":
                    qRentals = qRentals.OrderByDescending(r => r.EntryDate);
                    break;
                case "endtrydate_asc":
                    qRentals = qRentals.OrderBy(r => r.EntryDate);
                    break;
                default:
                    qRentals = qRentals.OrderBy(r => r.Id);
                    break;
            }

            const int pageSize = 5;
            if (pg < 1)
            {
                pg = 1;
            }

            var pager = new Pager(qRentals.Count(), pg, pageSize);

            int skip = (pg - 1) * pageSize;

            var data = qRentals.Skip(skip).Take(pager.PageSize).ToList();

            this.ViewBag.Pager = pager;

            return View(data);
        }

        [Authorize(Roles = "Administrator")]
        [Route("rental-details/{id:int:min(1)}")]
        public async Task<ViewResult> RentalDetails(int id, string archived)
        {
            ViewData["Archived"] = archived;

            if (!_context.Rentals.Any(v => v.Id == id))
            {
                TempData["InvalidId"] = "ID is invalid.";
                return View("~/Views/Home/Index.cshtml");
            }

            else
            {
                var data = await _vehicleRentalService.GetRentalById(id);
                return View(data);
            }        
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Calendar()
        {
            return View();
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult GetAllRentalsToJson()
        {
            var rentals = _context.Rentals.Select(r => new
            {
                title = _context.Vehicles.Where(v => v.Id == r.VehicleId).Select(v => v.Manufacturer + " " + v.Model).FirstOrDefault(),
                start = r.StartDate.ToString("yyyy-MM-dd"),
                end = r.EndDate.AddDays(1).ToString("yyyy-MM-dd"), // dodany jeden dzien, bo kalendarz przy wyswietlaniu podaje date z wyłączeniem ostatniego dnia
                url = ($"/rental-details/{r.Id}"),
                color = _context.Categories.Where(c => c.Id == _context.Vehicles.Where(v => v.Id == r.VehicleId).Select(v => v.CategoryId).FirstOrDefault()).Select(c => c.Color).FirstOrDefault()
            }).ToList();

            return new JsonResult(rentals);
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Delete(int id)
        {
            if (_context.Rentals.Any(v => v.Id == id))
            {
                var rental = _context.Rentals.Find(id);
                return PartialView("_DeleteRentalModalPartial", rental);
            }

            else
            {
                TempData["InvalidId"] = "ID is invalid.";
                return View("~/Views/Home/Index.cshtml");
            }

        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public IActionResult Delete(Rental rental, string archived)
        {
            _context.Rentals.Remove(rental);
            _context.SaveChanges();

            ViewData["Archived"] = archived;

            return RedirectToAction("RentalList", routeValues: new { archived = archived });
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Archive(int id)
        {
            if (_context.Rentals.Any(v => v.Id == id))
            {
                var rental = await _vehicleRentalService.GetRentalById(id);
                return PartialView("_ArchiveRentalModalPartial", rental);
            }

            else
            {
                TempData["InvalidId"] = "ID is invalid.";
                return View("~/Views/Home/Index.cshtml");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Archive(RentalModel rentalModel, string archived)
        {
            await _vehicleRentalService.ChangeStatus(rentalModel);

            return RedirectToAction("RentalList", routeValues: new { archived = archived });
        }

        [Authorize(Roles = "Administrator, User")]
        public async Task<ViewResult> GetUserRentals(int pg = 1)
        {
            var rentals = await _vehicleRentalService.GetUserRentals(HttpContext);

            const int pageSize = 5;
            if (pg < 1)
            {
                pg = 1;
            }

            var pager = new Pager(rentals.Count(), pg, pageSize);

            int skip = (pg - 1) * pageSize;

            var data = rentals.Skip(skip).Take(pager.PageSize).ToList();

            this.ViewBag.Pager = pager;

            return View(data);

        }

    }
}
