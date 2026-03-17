using MenuService.API.Models;

namespace MenuService.API.Repositories;

public interface IRestaurantConfigRepository
{
    Task<RestaurantConfig?> GetByRestaurantIdAsync(string restaurantId);
    Task<RestaurantConfig?> GetByIdAsync(int id);
    Task<List<RestaurantConfig>> GetAllActiveAsync();
    Task<RestaurantConfig> CreateAsync(RestaurantConfig config);
    Task<RestaurantConfig> UpdateAsync(RestaurantConfig config);
    Task DeleteAsync(int id);
}

public interface IEnhancedMenuRepository
{
    // Menu Items
    Task<EnhancedMenuItem?> GetMenuItemByIdAsync(int id);
    Task<List<EnhancedMenuItem>> GetMenuItemsByCategoryAsync(int categoryId);
    Task<List<EnhancedMenuItem>> GetMenuItemsBySubcategoryAsync(int subcategoryId);
    Task<List<EnhancedMenuItem>> GetMenuItemsByRestaurantAsync(int restaurantConfigId);
    Task<List<EnhancedMenuItem>> SearchMenuItemsAsync(MenuSearchRequest request);
    Task<EnhancedMenuItem> CreateMenuItemAsync(EnhancedMenuItem item);
    Task<EnhancedMenuItem> UpdateMenuItemAsync(EnhancedMenuItem item);
    Task DeleteMenuItemAsync(int id);

    // Variants
    Task<ItemVariant?> GetVariantByIdAsync(int id);
    Task<List<ItemVariant>> GetVariantsByMenuItemAsync(int menuItemId);
    Task<ItemVariant> CreateVariantAsync(ItemVariant variant);
    Task<ItemVariant> UpdateVariantAsync(ItemVariant variant);
    Task DeleteVariantAsync(int id);

    // Customization
    Task<CustomizationGroup?> GetCustomizationGroupByIdAsync(int id);
    Task<List<CustomizationGroup>> GetCustomizationGroupsByMenuItemAsync(int menuItemId);
    Task<CustomizationGroup> CreateCustomizationGroupAsync(CustomizationGroup group);
    Task<CustomizationGroup> UpdateCustomizationGroupAsync(CustomizationGroup group);
    Task DeleteCustomizationGroupAsync(int id);

    Task<CustomizationOption?> GetCustomizationOptionByIdAsync(int id);
    Task<CustomizationOption> CreateCustomizationOptionAsync(CustomizationOption option);
    Task<CustomizationOption> UpdateCustomizationOptionAsync(CustomizationOption option);
    Task DeleteCustomizationOptionAsync(int id);

    // Categories & Subcategories
    Task<Category?> GetCategoryByIdAsync(int id);
    Task<List<Category>> GetCategoriesByRestaurantAsync(int restaurantConfigId);
    Task<Category> CreateCategoryAsync(Category category);
    Task<Category> UpdateCategoryAsync(Category category);
    Task DeleteCategoryAsync(int id);

    Task<Subcategory?> GetSubcategoryByIdAsync(int id);
    Task<List<Subcategory>> GetSubcategoriesByCategoryAsync(int categoryId);
    Task<Subcategory> CreateSubcategoryAsync(Subcategory subcategory);
    Task<Subcategory> UpdateSubcategoryAsync(Subcategory subcategory);
    Task DeleteSubcategoryAsync(int id);
}

public interface IComboDealRepository
{
    Task<ComboDeal?> GetDealByIdAsync(int id);
    Task<List<ComboDeal>> GetAvailableDealsAsync(int restaurantConfigId);
    Task<List<ComboDeal>> GetAllDealsAsync(int restaurantConfigId);
    Task<ComboDeal> CreateDealAsync(ComboDeal deal);
    Task<ComboDeal> UpdateDealAsync(ComboDeal deal);
    Task DeleteDealAsync(int id);

    Task<ComboDealItem?> GetDealItemByIdAsync(int id);
    Task<List<ComboDealItem>> GetDealItemsByDealAsync(int dealId);
    Task<ComboDealItem> CreateDealItemAsync(ComboDealItem item);
    Task<ComboDealItem> UpdateDealItemAsync(ComboDealItem item);
    Task DeleteDealItemAsync(int id);
}

public interface IBundlePriceRepository
{
    Task<BundlePrice?> GetByIdAsync(int id);
    Task<List<BundlePrice>> GetByMenuItemAsync(int menuItemId);
    Task<BundlePrice?> GetApplicableBundleAsync(int menuItemId, int quantity);
    Task<BundlePrice> CreateAsync(BundlePrice bundlePrice);
    Task<BundlePrice> UpdateAsync(BundlePrice bundlePrice);
    Task DeleteAsync(int id);
}
