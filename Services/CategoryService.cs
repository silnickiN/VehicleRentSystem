using Microsoft.EntityFrameworkCore;
using VehicleRentSystem.Areas.Identity.Data;
using VehicleRentSystem.Data;
using VehicleRentSystem.Models;
using VehicleRentSystem.Services.Interfaces;

namespace VehicleRentSystem.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context = null;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddCategory(CategoryModel categoryModel)
        {
            var newCategory = new Category()
            {
                Name = categoryModel.Name,
                Color = categoryModel.Color
            };

            await _context.Categories.AddAsync(newCategory);
            await _context.SaveChangesAsync();

            return newCategory.Id;

        }

        public async Task<List<CategoryModel>> CategoryList()
        {
            return await _context.Categories.Select(x => new CategoryModel()
            {
                Id = x.Id,
                Name = x.Name,
                Color = x.Color
            }).ToListAsync();
        }
    }
}
