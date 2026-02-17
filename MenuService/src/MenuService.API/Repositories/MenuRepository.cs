using Microsoft.EntityFrameworkCore;
using MenuService.API.Models;
using MenuService.API.Data;

namespace MenuService.API.Repositories;

public class MenuItemRepository : IMenuItemRepository
{
    private readonly MenuContext _context;

    public MenuItemRepository(MenuContext context)
    {
        _context = context;
    }

    public async Task<MenuItemResponse?> GetByIdAsync(int id)
    {
        var item = await _context.MenuItems
            .Include(m => m.Category)
            .FirstOrDefaultAsync(m => m.Id == id);

        return item == null ? null : MapToResponse(item);
    }

    public async Task<List<MenuItemResponse>> GetAllAsync()
    {
        var items = await _context.MenuItems
            .Include(m => m.Category)
            .ToListAsync();

        return items.Select(MapToResponse).ToList();
    }

    public async Task<List<MenuItemResponse>> GetByCategoryAsync(int categoryId)
    {
        var items = await _context.MenuItems
            .Where(m => m.CategoryId == categoryId)
            .Include(m => m.Category)
            .ToListAsync();

        return items.Select(MapToResponse).ToList();
    }

    public async Task<MenuItemResponse> CreateAsync(MenuItemRequest request)
    {
        var menuItem = new MenuItem
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            CategoryId = request.CategoryId,
            IsAvailable = request.IsAvailable,
            ImageUrl = request.ImageUrl,
            Calories = request.Calories,
            IsVegetarian = request.IsVegetarian,
            IsSpicy = request.IsSpicy
        };

        _context.MenuItems.Add(menuItem);
        await _context.SaveChangesAsync();

        await _context.Entry(menuItem).Reference(m => m.Category).LoadAsync();
        return MapToResponse(menuItem);
    }

    public async Task<MenuItemResponse?> UpdateAsync(int id, MenuItemRequest request)
    {
        var menuItem = await _context.MenuItems.FindAsync(id);
        if (menuItem == null) return null;

        menuItem.Name = request.Name;
        menuItem.Description = request.Description;
        menuItem.Price = request.Price;
        menuItem.CategoryId = request.CategoryId;
        menuItem.IsAvailable = request.IsAvailable;
        menuItem.ImageUrl = request.ImageUrl;
        menuItem.Calories = request.Calories;
        menuItem.IsVegetarian = request.IsVegetarian;
        menuItem.IsSpicy = request.IsSpicy;
        menuItem.UpdatedAt = DateTime.UtcNow;

        _context.MenuItems.Update(menuItem);
        await _context.SaveChangesAsync();

        await _context.Entry(menuItem).Reference(m => m.Category).LoadAsync();
        return MapToResponse(menuItem);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var menuItem = await _context.MenuItems.FindAsync(id);
        if (menuItem == null) return false;

        _context.MenuItems.Remove(menuItem);
        await _context.SaveChangesAsync();
        return true;
    }

    private static MenuItemResponse MapToResponse(MenuItem item)
    {
        return new MenuItemResponse
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            Price = item.Price,
            CategoryId = item.CategoryId,
            CategoryName = item.Category?.Name,
            IsAvailable = item.IsAvailable,
            ImageUrl = item.ImageUrl,
            Calories = item.Calories,
            IsVegetarian = item.IsVegetarian,
            IsSpicy = item.IsSpicy,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt
        };
    }
}

public class CategoryRepository : ICategoryRepository
{
    private readonly MenuContext _context;

    public CategoryRepository(MenuContext context)
    {
        _context = context;
    }

    public async Task<CategoryResponse?> GetByIdAsync(int id)
    {
        var category = await _context.Categories
            .Include(c => c.MenuItems)
            .FirstOrDefaultAsync(c => c.Id == id);

        return category == null ? null : MapToResponse(category);
    }

    public async Task<List<CategoryResponse>> GetAllAsync()
    {
        var categories = await _context.Categories
            .Include(c => c.MenuItems)
            .ToListAsync();

        return categories.Select(MapToResponse).ToList();
    }

    public async Task<CategoryResponse> CreateAsync(CategoryRequest request)
    {
        var category = new Category
        {
            Name = request.Name,
            Description = request.Description
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return MapToResponse(category);
    }

    public async Task<CategoryResponse?> UpdateAsync(int id, CategoryRequest request)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return null;

        category.Name = request.Name;
        category.Description = request.Description;

        _context.Categories.Update(category);
        await _context.SaveChangesAsync();

        await _context.Entry(category).Collection(c => c.MenuItems).LoadAsync();
        return MapToResponse(category);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return false;

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return true;
    }

    private static CategoryResponse MapToResponse(Category category)
    {
        return new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ItemCount = category.MenuItems.Count,
            CreatedAt = category.CreatedAt
        };
    }
}
