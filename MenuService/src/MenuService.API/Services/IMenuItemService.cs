using MenuService.API.Models;

namespace MenuService.API.Services;

public interface IMenuItemService
{
    Task<EnhancedMenuItem> GetMenuItemAsync(int id);
    Task<List<EnhancedMenuItem>> GetMenuItemsByCategoryAsync(int categoryId);
    Task<List<EnhancedMenuItem>> GetMenuItemsBySubcategoryAsync(int subcategoryId);
    Task<List<EnhancedMenuItem>> GetRestaurantMenuAsync(int restaurantConfigId);
    Task<MenuSearchResponse<MenuItemResponse>> SearchMenuAsync(MenuSearchRequest request);
    Task<EnhancedMenuItem> CreateMenuItemAsync(int restaurantConfigId, MenuItemRequest request);
    Task<EnhancedMenuItem> UpdateMenuItemAsync(int id, MenuItemRequest request);
    Task DeleteMenuItemAsync(int id);
}

public interface ICustomizationService
{
    // Get customization details for an item
    Task<List<CustomizationGroup>> GetItemCustomizationsAsync(int menuItemId);
    Task<CustomizationGroup> CreateCustomizationGroupAsync(int menuItemId, CustomizationGroupRequest request);
    Task<CustomizationGroup> UpdateCustomizationGroupAsync(int id, CustomizationGroupRequest request);
    Task DeleteCustomizationGroupAsync(int id);

    // Calculate price with customizations
    Task<decimal> CalculateFinalPriceAsync(int menuItemId, List<int> selectedVariantIds, List<int> selectedOptionIds);
}

public interface IVariantService
{
    Task<List<ItemVariant>> GetItemVariantsAsync(int menuItemId);
    Task<ItemVariant> CreateVariantAsync(int menuItemId, ItemVariantRequest request);
    Task<ItemVariant> UpdateVariantAsync(int id, ItemVariantRequest request);
    Task DeleteVariantAsync(int id);
    Task<decimal> GetVariantFinalPriceAsync(int menuItemId, int variantId);
}

public interface IComboDealService
{
    Task<List<ComboDealResponse>> GetAvailableDealsAsync(int restaurantConfigId);
    Task<ComboDealResponse> GetDealByIdAsync(int id);
    Task<ComboDealResponse> CreateDealAsync(int restaurantConfigId, ComboDealRequest request);
    Task<ComboDealResponse> UpdateDealAsync(int id, ComboDealRequest request);
    Task DeleteDealAsync(int id);
    Task<bool> ValidateDealAsync(int dealId);
}

public interface IBundlePricingService
{
    Task<BundlePrice?> GetApplicableBundleAsync(int menuItemId, int quantity);
    Task<decimal> CalculateBundlePriceAsync(int menuItemId, int quantity);
    Task<BundlePrice> CreateBundlePriceAsync(int menuItemId, int minimumQuantity, decimal unitPrice, decimal discountPercentage);
}

public interface ICategoryService
{
    Task<List<CategoryResponse>> GetRestaurantCategoriesAsync(int restaurantConfigId);
    Task<CategoryResponse> GetCategoryByIdAsync(int id);
    Task<CategoryResponse> CreateCategoryAsync(int restaurantConfigId, CategoryRequest request);
    Task<CategoryResponse> UpdateCategoryAsync(int id, CategoryRequest request);
    Task DeleteCategoryAsync(int id);
    Task<CategoryResponse> CreateCategoryAsync(int restaurantId, CategoryRequest request);
    Task<CategoryResponse> GetCategoryByIdAsync(int id, int restaurantId);
    Task<List<CategoryResponse>> GetCategoriesByRestaurantAsync(int restaurantId);
    Task<CategoryResponse> UpdateCategoryAsync(int id, int restaurantId, CategoryRequest request);
    Task<bool> DeleteCategoryAsync(int id, int restaurantId);
}

public interface ISubcategoryService
{
    Task<List<SubcategoryResponse>> GetSubcategoriesByCategoryAsync(int categoryId);
    Task<SubcategoryResponse> GetSubcategoryByIdAsync(int id);
    Task<SubcategoryResponse> CreateSubcategoryAsync(int categoryId, SubcategoryRequest request);
    Task<SubcategoryResponse> UpdateSubcategoryAsync(int id, SubcategoryRequest request);
    Task DeleteSubcategoryAsync(int id);
}

public interface IRestaurantConfigService
{
    Task<RestaurantConfigResponse> GetConfigByRestaurantIdAsync(string restaurantId);
    Task<RestaurantConfigResponse> GetConfigByIdAsync(int id);
    Task<RestaurantConfigResponse> CreateConfigAsync(RestaurantConfigRequest request);
    Task<RestaurantConfigResponse> UpdateConfigAsync(int id, RestaurantConfigRequest request);
    Task DeleteConfigAsync(int id);
    Task<List<RestaurantConfigResponse>> GetAllActiveConfigsAsync();
    Task<RestaurantConfigResponse> CreateRestaurantConfigAsync(RestaurantConfigRequest request);
    Task<RestaurantConfigResponse> GetRestaurantConfigByIdAsync(int id);
    Task<List<RestaurantConfigResponse>> GetAllRestaurantConfigsAsync();
    Task<RestaurantConfigResponse> UpdateRestaurantConfigAsync(int id, RestaurantConfigRequest request);
    Task<bool> DeleteRestaurantConfigAsync(int id);
    Task<List<RestaurantConfigResponse>> GetRestaurantsByTypeAsync(string restaurantType);
    Task<List<RestaurantConfigResponse>> GetRestaurantsByCuisineAsync(string cuisineType);
}

public interface IMenuValidationService
{
    Task ValidateMenuItemAsync(MenuItemRequest request, int restaurantConfigId);
    Task ValidateCustomizationGroupAsync(CustomizationGroupRequest request);
    Task ValidateComboDealAsync(ComboDealRequest request);
    Task<bool> IsMenuItemAvailableAsync(int menuItemId);
    Task<bool> AreCustomizationsValidAsync(int menuItemId, List<int> selectedOptionIds);
}
