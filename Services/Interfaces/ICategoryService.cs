using VehicleRentSystem.Models;

namespace VehicleRentSystem.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<int> AddCategory(CategoryModel category);

        Task<List<CategoryModel>> CategoryList();
    }
}
