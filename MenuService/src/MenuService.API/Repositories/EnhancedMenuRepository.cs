using Microsoft.EntityFrameworkCore;
using MenuService.API.Data;
using MenuService.API.Models;

namespace MenuService.API.Repositories;

public class EnhancedMenuRepository : IEnhancedMenuRepository
{
    private readonly MenuContext _context;

    public EnhancedMenuRepository(MenuContext context)
    {
        _context = context;
    }

    #region Menu Items

    public async Task<EnhancedMenuItem?> GetMenuItemByIdAsync(int id)
    {
        return await _context.EnhancedMenuItems
            .Include(m => m.Category)
            .Include(m => m.Subcategory)
            .Include(m => m.Variants)
            .Include(m => m.CustomizationGroups)
                .ThenInclude(g => g.Options)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<List<EnhancedMenuItem>> GetMenuItemsByCategoryAsync(int categoryId)
    {
        return await _context.EnhancedMenuItems
            .Where(m => m.CategoryId == categoryId && m.IsAvailable)
            .Include(m => m.Variants)
            .Include(m => m.CustomizationGroups)
                .ThenInclude(g => g.Options)
            .OrderBy(m => m.DisplayOrder)
            .ToListAsync();
    }

    public async Task<List<EnhancedMenuItem>> GetMenuItemsBySubcategoryAsync(int subcategoryId)
    {
        return await _context.EnhancedMenuItems
            .Where(m => m.SubcategoryId == subcategoryId && m.IsAvailable)
            .Include(m => m.Variants)
            .Include(m => m.CustomizationGroups)
                .ThenInclude(g => g.Options)
            .OrderBy(m => m.DisplayOrder)
            .ToListAsync();
    }

    public async Task<List<EnhancedMenuItem>> GetMenuItemsByRestaurantAsync(int restaurantConfigId)
    {
        return await _context.EnhancedMenuItems
            .Where(m => m.RestaurantConfigId == restaurantConfigId && m.IsAvailable)
            .Include(m => m.Category)
            .Include(m => m.Subcategory)
            .Include(m => m.Variants)
            .Include(m => m.CustomizationGroups)
                .ThenInclude(g => g.Options)
            .OrderBy(m => m.Category!.DisplayOrder)
            .ThenBy(m => m.DisplayOrder)
            .ToListAsync();
    }

    public async Task<List<EnhancedMenuItem>> SearchMenuItemsAsync(MenuSearchRequest request)
    {
        var query = _context.EnhancedMenuItems.AsQueryable();

        // Search term
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(m => 
                m.Name.ToLower().Contains(searchTerm) || 
                m.Description.ToLower().Contains(searchTerm) ||
                m.Tags!.ToLower().Contains(searchTerm));
        }

        // Category filter
        if (request.CategoryId.HasValue)
            query = query.Where(m => m.CategoryId == request.CategoryId);

        // Subcategory filter
        if (request.SubcategoryId.HasValue)
            query = query.Where(m => m.SubcategoryId == request.SubcategoryId);

        // Dietary filters
        if (request.IsVegetarian.HasValue)
            query = query.Where(m => m.IsVegetarian == request.IsVegetarian);

        if (request.IsVegan.HasValue)
            query = query.Where(m => m.IsVegan == request.IsVegan);

        if (request.IsGlutenFree.HasValue)
            query = query.Where(m => m.IsGlutenFree == request.IsGlutenFree);

        // Price filter
        if (request.MaxPrice.HasValue)
            query = query.Where(m => m.BasePrice <= request.MaxPrice);

        // Calories filter
        if (request.MaxCalories.HasValue)
            query = query.Where(m => m.Calories.HasValue && m.Calories <= request.MaxCalories);

        // Only available items
        query = query.Where(m => m.IsAvailable);

        // Sorting
        query = request.SortBy?.ToLower() switch
        {
            "price" => query.OrderBy(m => m.BasePrice),
            "popularity" => query.OrderByDescending(m => m.RecommendationScore),
            "newest" => query.OrderByDescending(m => m.CreatedAt),
            _ => query.OrderBy(m => m.Name)
        };

        // Pagination
        var skip = (request.PageNumber - 1) * request.PageSize;
        return await query
            .Include(m => m.Variants)
            .Include(m => m.CustomizationGroups)
                .ThenInclude(g => g.Options)
            .Skip(skip)
            .Take(request.PageSize)
            .ToListAsync();
    }

    public async Task<EnhancedMenuItem> CreateMenuItemAsync(EnhancedMenuItem item)
    {
        _context.EnhancedMenuItems.Add(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task<EnhancedMenuItem> UpdateMenuItemAsync(EnhancedMenuItem item)
    {
        item.UpdatedAt = DateTime.UtcNow;
        _context.EnhancedMenuItems.Update(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task DeleteMenuItemAsync(int id)
    {
        var item = await _context.EnhancedMenuItems.FindAsync(id);
        if (item != null)
        {
            _context.EnhancedMenuItems.Remove(item);
            await _context.SaveChangesAsync();
        }
    }

    #endregion

    #region Variants

    public async Task<ItemVariant?> GetVariantByIdAsync(int id)
    {
        return await _context.ItemVariants.FindAsync(id);
    }

    public async Task<List<ItemVariant>> GetVariantsByMenuItemAsync(int menuItemId)
    {
        return await _context.ItemVariants
            .Where(v => v.MenuItemId == menuItemId && v.IsAvailable)
            .OrderBy(v => v.DisplayOrder)
            .ToListAsync();
    }

    public async Task<ItemVariant> CreateVariantAsync(ItemVariant variant)
    {
        _context.ItemVariants.Add(variant);
        await _context.SaveChangesAsync();
        return variant;
    }

    public async Task<ItemVariant> UpdateVariantAsync(ItemVariant variant)
    {
        _context.ItemVariants.Update(variant);
        await _context.SaveChangesAsync();
        return variant;
    }

    public async Task DeleteVariantAsync(int id)
    {
        var variant = await _context.ItemVariants.FindAsync(id);
        if (variant != null)
        {
            _context.ItemVariants.Remove(variant);
            await _context.SaveChangesAsync();
        }
    }

    #endregion

    #region Customization

    public async Task<CustomizationGroup?> GetCustomizationGroupByIdAsync(int id)
    {
        return await _context.CustomizationGroups
            .Include(g => g.Options)
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<List<CustomizationGroup>> GetCustomizationGroupsByMenuItemAsync(int menuItemId)
    {
        return await _context.CustomizationGroups
            .Where(g => g.MenuItemId == menuItemId && g.IsActive)
            .Include(g => g.Options)
            .OrderBy(g => g.DisplayOrder)
            .ToListAsync();
    }

    public async Task<CustomizationGroup> CreateCustomizationGroupAsync(CustomizationGroup group)
    {
        _context.CustomizationGroups.Add(group);
        await _context.SaveChangesAsync();
        return group;
    }

    public async Task<CustomizationGroup> UpdateCustomizationGroupAsync(CustomizationGroup group)
    {
        _context.CustomizationGroups.Update(group);
        await _context.SaveChangesAsync();
        return group;
    }

    public async Task DeleteCustomizationGroupAsync(int id)
    {
        var group = await _context.CustomizationGroups.FindAsync(id);
        if (group != null)
        {
            _context.CustomizationGroups.Remove(group);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<CustomizationOption?> GetCustomizationOptionByIdAsync(int id)
    {
        return await _context.CustomizationOptions.FindAsync(id);
    }

    public async Task<CustomizationOption> CreateCustomizationOptionAsync(CustomizationOption option)
    {
        _context.CustomizationOptions.Add(option);
        await _context.SaveChangesAsync();
        return option;
    }

    public async Task<CustomizationOption> UpdateCustomizationOptionAsync(CustomizationOption option)
    {
        _context.CustomizationOptions.Update(option);
        await _context.SaveChangesAsync();
        return option;
    }

    public async Task DeleteCustomizationOptionAsync(int id)
    {
        var option = await _context.CustomizationOptions.FindAsync(id);
        if (option != null)
        {
            _context.CustomizationOptions.Remove(option);
            await _context.SaveChangesAsync();
        }
    }

    #endregion

    #region Categories & Subcategories

    public async Task<Category?> GetCategoryByIdAsync(int id)
    {
        return await _context.Categories
            .Include(c => c.Subcategories)
            .Include(c => c.EnhancedMenuItems)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<Category>> GetCategoriesByRestaurantAsync(int restaurantConfigId)
    {
        return await _context.Categories
            .Where(c => c.RestaurantConfigId == restaurantConfigId && c.IsActive)
            .Include(c => c.Subcategories)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();
    }

    public async Task<Category> CreateCategoryAsync(Category category)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<Category> UpdateCategoryAsync(Category category)
    {
        category.UpdatedAt = DateTime.UtcNow;
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task DeleteCategoryAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category != null)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Subcategory?> GetSubcategoryByIdAsync(int id)
    {
        return await _context.Subcategories.FindAsync(id);
    }

    public async Task<List<Subcategory>> GetSubcategoriesByCategoryAsync(int categoryId)
    {
        return await _context.Subcategories
            .Where(s => s.CategoryId == categoryId && s.IsActive)
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync();
    }

    public async Task<Subcategory> CreateSubcategoryAsync(Subcategory subcategory)
    {
        _context.Subcategories.Add(subcategory);
        await _context.SaveChangesAsync();
        return subcategory;
    }

    public async Task<Subcategory> UpdateSubcategoryAsync(Subcategory subcategory)
    {
        _context.Subcategories.Update(subcategory);
        await _context.SaveChangesAsync();
        return subcategory;
    }

    public async Task DeleteSubcategoryAsync(int id)
    {
        var subcategory = await _context.Subcategories.FindAsync(id);
        if (subcategory != null)
        {
            _context.Subcategories.Remove(subcategory);
            await _context.SaveChangesAsync();
        }
    }

    #endregion
}

public class RestaurantConfigRepository : IRestaurantConfigRepository
{
    private readonly MenuContext _context;

    public RestaurantConfigRepository(MenuContext context)
    {
        _context = context;
    }

    public async Task<RestaurantConfig?> GetByRestaurantIdAsync(string restaurantId)
    {
        return await _context.RestaurantConfigs
            .Include(r => r.Categories)
            .FirstOrDefaultAsync(r => r.RestaurantId == restaurantId);
    }

    public async Task<RestaurantConfig?> GetByIdAsync(int id)
    {
        return await _context.RestaurantConfigs
            .Include(r => r.Categories)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<List<RestaurantConfig>> GetAllActiveAsync()
    {
        return await _context.RestaurantConfigs
            .Where(r => r.IsActive)
            .ToListAsync();
    }

    public async Task<RestaurantConfig> CreateAsync(RestaurantConfig config)
    {
        _context.RestaurantConfigs.Add(config);
        await _context.SaveChangesAsync();
        return config;
    }

    public async Task<RestaurantConfig> UpdateAsync(RestaurantConfig config)
    {
        config.UpdatedAt = DateTime.UtcNow;
        _context.RestaurantConfigs.Update(config);
        await _context.SaveChangesAsync();
        return config;
    }

    public async Task DeleteAsync(int id)
    {
        var config = await _context.RestaurantConfigs.FindAsync(id);
        if (config != null)
        {
            _context.RestaurantConfigs.Remove(config);
            await _context.SaveChangesAsync();
        }
    }
}

public class ComboDealRepository : IComboDealRepository
{
    private readonly MenuContext _context;

    public ComboDealRepository(MenuContext context)
    {
        _context = context;
    }

    public async Task<ComboDeal?> GetDealByIdAsync(int id)
    {
        return await _context.ComboDeals
            .Include(d => d.Items)
                .ThenInclude(i => i.MenuItem)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<List<ComboDeal>> GetAvailableDealsAsync(int restaurantConfigId)
    {
        var now = DateTime.UtcNow;
        return await _context.ComboDeals
            .Where(d => d.RestaurantConfigId == restaurantConfigId &&
                       d.IsAvailable &&
                       (d.ValidFrom == null || d.ValidFrom <= now) &&
                       (d.ValidUntil == null || d.ValidUntil >= now))
            .Include(d => d.Items)
                .ThenInclude(i => i.MenuItem)
            .OrderBy(d => d.DisplayOrder)
            .ToListAsync();
    }

    public async Task<List<ComboDeal>> GetAllDealsAsync(int restaurantConfigId)
    {
        return await _context.ComboDeals
            .Where(d => d.RestaurantConfigId == restaurantConfigId)
            .Include(d => d.Items)
                .ThenInclude(i => i.MenuItem)
            .ToListAsync();
    }

    public async Task<ComboDeal> CreateDealAsync(ComboDeal deal)
    {
        _context.ComboDeals.Add(deal);
        await _context.SaveChangesAsync();
        return deal;
    }

    public async Task<ComboDeal> UpdateDealAsync(ComboDeal deal)
    {
        deal.UpdatedAt = DateTime.UtcNow;
        _context.ComboDeals.Update(deal);
        await _context.SaveChangesAsync();
        return deal;
    }

    public async Task DeleteDealAsync(int id)
    {
        var deal = await _context.ComboDeals.FindAsync(id);
        if (deal != null)
        {
            _context.ComboDeals.Remove(deal);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<ComboDealItem?> GetDealItemByIdAsync(int id)
    {
        return await _context.ComboDealItems.FindAsync(id);
    }

    public async Task<List<ComboDealItem>> GetDealItemsByDealAsync(int dealId)
    {
        return await _context.ComboDealItems
            .Where(i => i.ComboDealId == dealId)
            .Include(i => i.MenuItem)
            .ToListAsync();
    }

    public async Task<ComboDealItem> CreateDealItemAsync(ComboDealItem item)
    {
        _context.ComboDealItems.Add(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task<ComboDealItem> UpdateDealItemAsync(ComboDealItem item)
    {
        _context.ComboDealItems.Update(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task DeleteDealItemAsync(int id)
    {
        var item = await _context.ComboDealItems.FindAsync(id);
        if (item != null)
        {
            _context.ComboDealItems.Remove(item);
            await _context.SaveChangesAsync();
        }
    }
}

public class BundlePriceRepository : IBundlePriceRepository
{
    private readonly MenuContext _context;

    public BundlePriceRepository(MenuContext context)
    {
        _context = context;
    }

    public async Task<BundlePrice?> GetByIdAsync(int id)
    {
        return await _context.BundlePrices.FindAsync(id);
    }

    public async Task<List<BundlePrice>> GetByMenuItemAsync(int menuItemId)
    {
        return await _context.BundlePrices
            .Where(b => b.MenuItemId == menuItemId && b.IsActive)
            .OrderByDescending(b => b.MinimumQuantity)
            .ToListAsync();
    }

    public async Task<BundlePrice?> GetApplicableBundleAsync(int menuItemId, int quantity)
    {
        return await _context.BundlePrices
            .Where(b => b.MenuItemId == menuItemId &&
                       b.IsActive &&
                       b.MinimumQuantity <= quantity)
            .OrderByDescending(b => b.MinimumQuantity)
            .FirstOrDefaultAsync();
    }

    public async Task<BundlePrice> CreateAsync(BundlePrice bundlePrice)
    {
        _context.BundlePrices.Add(bundlePrice);
        await _context.SaveChangesAsync();
        return bundlePrice;
    }

    public async Task<BundlePrice> UpdateAsync(BundlePrice bundlePrice)
    {
        _context.BundlePrices.Update(bundlePrice);
        await _context.SaveChangesAsync();
        return bundlePrice;
    }

    public async Task DeleteAsync(int id)
    {
        var bundlePrice = await _context.BundlePrices.FindAsync(id);
        if (bundlePrice != null)
        {
            _context.BundlePrices.Remove(bundlePrice);
            await _context.SaveChangesAsync();
        }
    }
}
