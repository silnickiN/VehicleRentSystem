using VehicleRentSystem.Services;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using VehicleRentSystem.Models;
using VehicleRentSystem.Services.Interfaces;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using VehicleRentSystem.Data;
using VehicleRentSystem.Areas.Identity.Data;
using static NuGet.Packaging.PackagingConstants;

namespace VehicleRentSystem.Controllers
{
    public class VehicleManagementController : Controller
    {
        private readonly IVehicleManagementService _vehicleManagementService;
        private readonly ICategoryService _categoryService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;

        public VehicleManagementController(IVehicleManagementService vehicleManagementService,
            IWebHostEnvironment webHostEnvironment,
            ICategoryService categoryService,
            ApplicationDbContext context)
        {
            _vehicleManagementService = vehicleManagementService;
            _categoryService = categoryService;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }

        public async Task<ViewResult> VehicleDisplayList(string currentSort, string sortOrder, string searchString, int pg = 1)
        {
            ViewData["ManufacturerOrder"] = sortOrder == "manufacturer_asc" ? "manufacturer_desc" : "manufacturer_asc";
            ViewData["PriceOrder"] = sortOrder == "price_asc" ? "price_desc" : "price_asc";
            ViewData["YearOrder"] = sortOrder == "year_asc" ? "year_desc" : "year_asc";
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentSort"] = sortOrder;

            var vehicles = await _vehicleManagementService.GetAllVehicles();
            var qVehicles = vehicles.AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                qVehicles = qVehicles.Where(v => v.Manufacturer.ToLower().Contains(searchString.ToLower())
                                       || v.Model.ToLower().Contains(searchString.ToLower())
                                       || v.Color.ToLower().Contains(searchString.ToLower())
                                       || v.Body.ToLower().Contains(searchString.ToLower())
                                       || v.ProductionYear.ToString().Contains(searchString));
            }

            switch (sortOrder)
            {
                case "price_desc":
                    qVehicles = qVehicles.OrderByDescending(v => v.RentalPrice);
                    break;
                case "price_asc":
                    qVehicles = qVehicles.OrderBy(v => v.RentalPrice);
                    break;
                case "manufacturer_desc":
                    qVehicles = qVehicles.OrderByDescending(v => v.Manufacturer);
                    break;
                case "manufacturer_asc":
                    qVehicles = qVehicles.OrderBy(v => v.Manufacturer);
                    break;
                case "year_desc":
                    qVehicles = qVehicles.OrderByDescending(v => v.ProductionYear);
                    break;
                case "year_asc":
                    qVehicles = qVehicles.OrderBy(v => v.ProductionYear);
                    break;
                default:
                    qVehicles = qVehicles.OrderBy(v => v.Id);
                    break;
            }

            const int pageSize = 12;
            if (pg < 1)
            {
                pg = 1;
            }

            var pager = new Pager(qVehicles.Count(), pg, pageSize);

            int skip = (pg - 1) * pageSize;

            var data = qVehicles.Skip(skip).Take(pager.PageSize);

            this.ViewBag.Pager = pager;

            return View(data);

        }

        [Route("/VehicleDetails/{id:int:min(1)}")]
        public async Task<ViewResult> VehicleDetails(int id)
        {
            if (!_context.Vehicles.Any(v => v.Id == id))
            {
                TempData["InvalidId"] = "ID is invalid.";
                return View("~/Views/Home/Index.cshtml");
            }

            else
            {
                var data = await _vehicleManagementService.GetVehicleById(id);

                return View(data);
            }

        }

        [Authorize(Roles = "Administrator")]
        public async Task<ViewResult> AddVehicle(bool isSuccess = false, int id = 0)
        {
            var model = new VehicleModel();

            ViewBag.IsSuccess = isSuccess;
            ViewBag.Id = id;
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> AddVehicle(VehicleModel vehicleModel)
        {
            if (vehicleModel.CategoryId == 0)
            {
                TempData["NoCategories"] = "You have to add category firstly.";
                return RedirectToAction("AddVehicle");
            }

            if (!ModelState.IsValid)
            {
                return View(vehicleModel);
            }

            if (vehicleModel.MainImage != null)
            {
                string folder = "vehicles/mainImage/";
                vehicleModel.MainImageUrl = await UploadImage(folder, vehicleModel.MainImage);
            }

            else
            {
                vehicleModel.MainImageUrl = "/vehicles/noImage.jpg";
            }

            if (vehicleModel.GalleryFiles != null)
            {
                string folder = "vehicles/gallery/";

                vehicleModel.Gallery = new List<GalleryModel>();

                foreach (var file in vehicleModel.GalleryFiles)
                {
                    var gallery = new GalleryModel()
                    {
                        Name = file.FileName,
                        URL = await UploadImage(folder, file)
                    };
                    vehicleModel.Gallery.Add(gallery);
                }
            }
            else
            {
                vehicleModel.Gallery = new List<GalleryModel>();
                var gallery = new GalleryModel()
                {
                    Name = "NoImage",
                    URL = "/vehicles/noImage.jpg",
                };
                vehicleModel.Gallery.Add(gallery);
            }

            int id = await _vehicleManagementService.AddVehicle(vehicleModel);

            if (id > 0)
            {
                return RedirectToAction(nameof(AddVehicle), new { isSuccess = true, VehicleId = id });
            }

            return View();
        }

        private async Task<string> UploadImage(string folderPath, IFormFile file)
        {

            folderPath += Guid.NewGuid().ToString() + "_" + file.FileName;

            string serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, folderPath);

            var stream = new FileStream(serverFolder, FileMode.Create);

            await file.CopyToAsync(stream);

            stream.Close();

            return "/" + folderPath;
        }

        [Authorize(Roles = "Administrator")]
        public async Task<ViewResult> VehicleManagementList(string currentSort, string sortOrder, string searchString, int pg = 1)
        {
            ViewData["ManufacturerOrder"] = sortOrder == "manufacturer_asc" ? "manufacturer_desc" : "manufacturer_asc";
            ViewData["PriceOrder"] = sortOrder == "price_asc" ? "price_desc" : "price_asc";
            ViewData["YearOrder"] = sortOrder == "year_asc" ? "year_desc" : "year_asc";
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentSort"] = sortOrder;

            var vehicles = await _vehicleManagementService.GetAllVehicles();
            var qVehicles = vehicles.AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                qVehicles = qVehicles.Where(v => v.Manufacturer.ToLower().Contains(searchString.ToLower())
                                       || v.Model.ToLower().Contains(searchString.ToLower())
                                       || v.Id.ToString().Contains(searchString.ToLower())
                                       || v.Color.ToLower().Contains(searchString.ToLower())
                                       || v.Body.ToLower().Contains(searchString.ToLower())
                                       || v.ProductionYear.ToString().Contains(searchString));
            }

            switch (sortOrder)
            {
                case "price_desc":
                    qVehicles = qVehicles.OrderByDescending(v => v.RentalPrice);
                    break;
                case "price_asc":
                    qVehicles = qVehicles.OrderBy(v => v.RentalPrice);
                    break;
                case "manufacturer_desc":
                    qVehicles = qVehicles.OrderByDescending(v => v.Manufacturer);
                    break;
                case "manufacturer_asc":
                    qVehicles = qVehicles.OrderBy(v => v.Manufacturer);
                    break;
                case "year_desc":
                    qVehicles = qVehicles.OrderByDescending(v => v.ProductionYear);
                    break;
                case "year_asc":
                    qVehicles = qVehicles.OrderBy(v => v.ProductionYear);
                    break;
                default:
                    qVehicles = qVehicles.OrderBy(v => v.Id);
                    break;
            }

            const int pageSize = 5;
            if (pg < 1)
            {
                pg = 1;
            }

            var pager = new Pager(qVehicles.Count(), pg, pageSize);

            int skip = (pg - 1) * pageSize;

            var data = qVehicles.Skip(skip).Take(pager.PageSize).ToList();

            this.ViewBag.Pager = pager;

            return View(data);
        }

        [HttpGet]
        public IActionResult GetImageGallery()
        {
            var result = _context.VehicleGalleries.ToList();

            return Ok(result.Select(t => new { t.Id, t.Name }));
        }

        [Authorize(Roles = "Administrator")]
        public async Task<ViewResult> AddCategory(bool isSuccess = false, int id = 0)
        {
            var model = new CategoryModel();

            ViewBag.IsSuccess = isSuccess;
            ViewBag.Id = id;
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> AddCategory(CategoryModel categoryModel)
        {
            if (!ModelState.IsValid)
            {
                return View(categoryModel);
            }

            int id = await _categoryService.AddCategory(categoryModel);
            if (id > 0)
            {
                return RedirectToAction(nameof(CategoryList), new { isSuccess = true });
            }

            return View();
        }

        [Authorize(Roles = "Administrator")]
        public async Task<ViewResult> CategoryList()
        {
            var data = await _categoryService.CategoryList();

            return View(data);
        }
     
        [Authorize(Roles = "Administrator")]
        public IActionResult DeleteVehicle(int id)
        {
            if (_context.Vehicles.Any(v => v.Id == id))
            {
                var vehicle = _context.Vehicles.Find(id);
                return PartialView("_DeleteVehicleModalPartial", vehicle);
            }

            else
            {
                TempData["InvalidId"] = "ID is invalid.";
                return View("~/Views/Home/Index.cshtml");
            }

        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public IActionResult DeleteVehicle(Vehicle vehicle)
        {
            if (_context.Rentals.Any(r => r.VehicleId == vehicle.Id))
            {
                TempData["ExistingRentals"] = "There are existing rentals connected with this vehicle.";
                return RedirectToAction("VehicleManagementList");
            }
            else
            {
                _context.Vehicles.Remove(vehicle);
                _context.SaveChanges();
                return RedirectToAction("VehicleManagementList");
            }

        }

        [Authorize(Roles = "Administrator")]
        public IActionResult DeleteCategory(int id)
        {
            if (_context.Categories.Any(c => c.Id == id))
            {
                var category = _context.Categories.Find(id);
                return PartialView("_DeleteCategoryModalPartial", category);
            }
            else
            {
                TempData["InvalidId"] = "ID is invalid.";
                return View("~/Views/Home/Index.cshtml");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public IActionResult DeleteCategory(Category category)
        {
            if (_context.Vehicles.Any(v => v.CategoryId == category.Id))
            {
                TempData["ExistingVehicles"] = "There are existing vehicles connected with this category.";
                return RedirectToAction("CategoryList");
            }

            else
            {
                _context.Categories.Remove(category);
                _context.SaveChanges();
                return RedirectToAction("CategoryList");
            }

        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteMainImage(int id)
        {
            if (_context.Vehicles.Any(v => v.Id == id))
            {
                var vehicle = await _vehicleManagementService.GetVehicleById(id);
                return PartialView("_DeleteMainImageModalPartial", vehicle);
            }

            else
            {
                TempData["InvalidId"] = "ID is invalid.";
                return View("~/Views/Home/Index.cshtml");
            }

        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteMainImage(VehicleModel vehicleModel)
        {
            await _vehicleManagementService.DeleteMainImage(vehicleModel);

            return RedirectToAction("EditVehicle", vehicleModel);

        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteGallery(int id)
        {
            if (_context.Vehicles.Any(v => v.Id == id))
            {
                var vehicle = await _vehicleManagementService.GetVehicleById(id);
                return PartialView("_DeleteGalleryModalPartial", vehicle);
            }

            else
            {
                TempData["InvalidId"] = "ID is invalid.";
                return View("~/Views/Home/Index.cshtml");
            }

        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteGallery(VehicleModel vehicleModel)
        {
            await _vehicleManagementService.DeleteGallery(vehicleModel);

            return RedirectToAction("EditVehicle", vehicleModel);

        }

        [Authorize(Roles = "Administrator")]
        public async Task<ViewResult> EditVehicle(int id)
        {
            if (_context.Vehicles.Any(v => v.Id == id))
            {
                var vehicle = await _vehicleManagementService.GetVehicleById(id);
                return View(vehicle);
            }
            else
            {
                TempData["InvalidId"] = "ID is invalid.";
                return View("~/Views/Home/Index.cshtml");
            }

        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> EditVehicle(int id, VehicleModel vehicleModel)
        {
            if (!ModelState.IsValid)
            {
                return View(vehicleModel);
            }

            if (vehicleModel.MainImage != null)
            {
                string folder = "vehicles/mainImage/";
                vehicleModel.MainImageUrl = await UploadImage(folder, vehicleModel.MainImage);
            }
            else
            {
                if (_context.Vehicles.Any(v => v.Id == id))
                {
                    vehicleModel.MainImageUrl = _context.Vehicles.Where(v => v.Id == id).Select(v => v.MainImageUrl).FirstOrDefault();
                }
                else
                {
                    vehicleModel.MainImageUrl = "/vehicles/noImage.jpg";
                }
            }

            if (vehicleModel.GalleryFiles != null)
            {
                string folder = "vehicles/gallery/";

                vehicleModel.Gallery = new List<GalleryModel>();

                foreach (var file in vehicleModel.GalleryFiles)
                {
                    var gallery = new GalleryModel()
                    {
                        Name = file.FileName,
                        URL = await UploadImage(folder, file)
                    };
                    vehicleModel.Gallery.Add(gallery);
                }
            }
            else
            {
                if (_context.VehicleGalleries.Any(v => v.VehicleId == id))
                {
                }
                else
                {
                    vehicleModel.Gallery = new List<GalleryModel>();
                    var gallery = new GalleryModel()
                    {
                        Name = "NoImage",
                        URL = "/vehicles/noImage.jpg",
                    };
                    vehicleModel.Gallery.Add(gallery);
                }
            }

            await _vehicleManagementService.Edit(vehicleModel, id);

            await _context.SaveChangesAsync();

            return RedirectToAction("VehicleManagementList");

        }

        [Authorize(Roles = "Administrator")]
        public ActionResult EditCategory(int id)
        {
            if (_context.Categories.Any(c => c.Id == id))
            {
                var category = _context.Categories.Find(id);
                return View(category);
            }

            else
            {
                TempData["InvalidId"] = "ID is invalid.";
                return View("~/Views/Home/Index.cshtml");
            }

        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<ViewResult> EditCategory(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();

            return View(category);
        }


    }
}


