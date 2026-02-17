using MenuService.API.Models;

namespace MenuService.API.Repositories;

public interface IMenuItemRepository
{
    Task<MenuItemResponse?> GetByIdAsync(int id);
    Task<List<MenuItemResponse>> GetAllAsync();
    Task<List<MenuItemResponse>> GetByCategoryAsync(int categoryId);
    Task<MenuItemResponse> CreateAsync(MenuItemRequest request);
    Task<MenuItemResponse?> UpdateAsync(int id, MenuItemRequest request);
    Task<bool> DeleteAsync(int id);
}

public interface ICategoryRepository
{
    Task<CategoryResponse?> GetByIdAsync(int id);
    Task<List<CategoryResponse>> GetAllAsync();
    Task<CategoryResponse> CreateAsync(CategoryRequest request);
    Task<CategoryResponse?> UpdateAsync(int id, CategoryRequest request);
    Task<bool> DeleteAsync(int id);
}
