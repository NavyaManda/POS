using MenuService.API.Models;
using MenuService.API.Repositories;

namespace MenuService.API.Services;

public interface IMenuService
{
    Task<MenuItemResponse?> GetMenuItemByIdAsync(int id);
    Task<List<MenuItemResponse>> GetAllMenuItemsAsync();
    Task<List<MenuItemResponse>> GetMenuItemsByCategoryAsync(int categoryId);
    Task<MenuItemResponse> CreateMenuItemAsync(MenuItemRequest request);
    Task<MenuItemResponse?> UpdateMenuItemAsync(int id, MenuItemRequest request);
    Task<bool> DeleteMenuItemAsync(int id);

    Task<CategoryResponse?> GetCategoryByIdAsync(int id);
    Task<List<CategoryResponse>> GetAllCategoriesAsync();
    Task<CategoryResponse> CreateCategoryAsync(CategoryRequest request);
    Task<CategoryResponse?> UpdateCategoryAsync(int id, CategoryRequest request);
    Task<bool> DeleteCategoryAsync(int id);
}

public class MenuService : IMenuService
{
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly ICategoryRepository _categoryRepository;

    public MenuService(IMenuItemRepository menuItemRepository, ICategoryRepository categoryRepository)
    {
        _menuItemRepository = menuItemRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<MenuItemResponse?> GetMenuItemByIdAsync(int id) => await _menuItemRepository.GetByIdAsync(id);
    public async Task<List<MenuItemResponse>> GetAllMenuItemsAsync() => await _menuItemRepository.GetAllAsync();
    public async Task<List<MenuItemResponse>> GetMenuItemsByCategoryAsync(int categoryId) => 
        await _menuItemRepository.GetByCategoryAsync(categoryId);
    public async Task<MenuItemResponse> CreateMenuItemAsync(MenuItemRequest request) => 
        await _menuItemRepository.CreateAsync(request);
    public async Task<MenuItemResponse?> UpdateMenuItemAsync(int id, MenuItemRequest request) => 
        await _menuItemRepository.UpdateAsync(id, request);
    public async Task<bool> DeleteMenuItemAsync(int id) => await _menuItemRepository.DeleteAsync(id);

    public async Task<CategoryResponse?> GetCategoryByIdAsync(int id) => await _categoryRepository.GetByIdAsync(id);
    public async Task<List<CategoryResponse>> GetAllCategoriesAsync() => await _categoryRepository.GetAllAsync();
    public async Task<CategoryResponse> CreateCategoryAsync(CategoryRequest request) => 
        await _categoryRepository.CreateAsync(request);
    public async Task<CategoryResponse?> UpdateCategoryAsync(int id, CategoryRequest request) => 
        await _categoryRepository.UpdateAsync(id, request);
    public async Task<bool> DeleteCategoryAsync(int id) => await _categoryRepository.DeleteAsync(id);
}
