using MenuService.API.Models;
using MenuService.API.Repositories;

namespace MenuService.API.Services;

public class CategoryService : ICategoryService
{
    private readonly IEnhancedMenuRepository _menuRepository;

    public CategoryService(IEnhancedMenuRepository menuRepository)
    {
        _menuRepository = menuRepository;
    }

    public async Task<List<CategoryResponse>> GetRestaurantCategoriesAsync(int restaurantConfigId)
    {
        var categories = await _menuRepository.GetCategoriesByRestaurantAsync(restaurantConfigId);
        return categories.Select(MapToResponse).ToList();
    }

    public async Task<CategoryResponse> GetCategoryByIdAsync(int id)
    {
        var category = await _menuRepository.GetCategoryByIdAsync(id);
        if (category == null)
            throw new InvalidOperationException($"Category {id} not found");
        return MapToResponse(category);
    }

    public async Task<CategoryResponse> CreateCategoryAsync(int restaurantConfigId, CategoryRequest request)
    {
        var category = new Category
        {
            RestaurantConfigId = restaurantConfigId,
            Name = request.Name,
            Description = request.Description,
            ImageUrl = request.ImageUrl,
            DisplayOrder = request.DisplayOrder,
            IsActive = true
        };

        var created = await _menuRepository.CreateCategoryAsync(category);
        return MapToResponse(created);
    }

    public async Task<CategoryResponse> UpdateCategoryAsync(int id, CategoryRequest request)
    {
        var category = await _menuRepository.GetCategoryByIdAsync(id);
        if (category == null)
            throw new InvalidOperationException($"Category {id} not found");

        category.Name = request.Name;
        category.Description = request.Description;
        category.ImageUrl = request.ImageUrl;
        category.DisplayOrder = request.DisplayOrder;

        var updated = await _menuRepository.UpdateCategoryAsync(category);
        return MapToResponse(updated);
    }

    public async Task<List<CategoryResponse>> GetCategoriesByRestaurantAsync(int restaurantId)
    {
        var categories = await _menuRepository.GetCategoriesByRestaurantAsync(restaurantId);
        return categories.Select(MapToResponse).ToList();
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        try
        {
            await _menuRepository.DeleteCategoryAsync(id);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private CategoryResponse MapToResponse(Category category)
    {
        return new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ImageUrl = category.ImageUrl,
            ItemCount = category.EnhancedMenuItems.Count,
            DisplayOrder = category.DisplayOrder,
            IsActive = category.IsActive,
            Subcategories = category.Subcategories.Select(s => new SubcategoryResponse
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                ImageUrl = s.ImageUrl,
                DisplayOrder = s.DisplayOrder,
                IsActive = s.IsActive,
                ItemCount = s.MenuItems.Count
            }).ToList(),
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }
}

public class SubcategoryService : ISubcategoryService
{
    private readonly IEnhancedMenuRepository _menuRepository;

    public SubcategoryService(IEnhancedMenuRepository menuRepository)
    {
        _menuRepository = menuRepository;
    }

    public async Task<List<SubcategoryResponse>> GetSubcategoriesByCategoryAsync(int categoryId)
    {
        var subcategories = await _menuRepository.GetSubcategoriesByCategoryAsync(categoryId);
        return subcategories.Select(MapToResponse).ToList();
    }

    public async Task<SubcategoryResponse> GetSubcategoryByIdAsync(int id)
    {
        var subcategory = await _menuRepository.GetSubcategoryByIdAsync(id);
        if (subcategory == null)
            throw new InvalidOperationException($"Subcategory {id} not found");
        return MapToResponse(subcategory);
    }

    public async Task<SubcategoryResponse> CreateSubcategoryAsync(int categoryId, SubcategoryRequest request)
    {
        var subcategory = new Subcategory
        {
            CategoryId = categoryId,
            Name = request.Name,
            Description = request.Description,
            ImageUrl = request.ImageUrl,
            DisplayOrder = request.DisplayOrder,
            IsActive = true
        };

        var created = await _menuRepository.CreateSubcategoryAsync(subcategory);
        return MapToResponse(created);
    }

    public async Task<SubcategoryResponse> UpdateSubcategoryAsync(int id, SubcategoryRequest request)
    {
        var subcategory = await _menuRepository.GetSubcategoryByIdAsync(id);
        if (subcategory == null)
            throw new InvalidOperationException($"Subcategory {id} not found");

        subcategory.Name = request.Name;
        subcategory.Description = request.Description;
        subcategory.ImageUrl = request.ImageUrl;
        subcategory.DisplayOrder = request.DisplayOrder;

        var updated = await _menuRepository.UpdateSubcategoryAsync(subcategory);
        return MapToResponse(updated);
    }

    public async Task DeleteSubcategoryAsync(int id)
    {
        await _menuRepository.DeleteSubcategoryAsync(id);
    }

    private SubcategoryResponse MapToResponse(Subcategory subcategory)
    {
        return new SubcategoryResponse
        {
            Id = subcategory.Id,
            Name = subcategory.Name,
            Description = subcategory.Description,
            ImageUrl = subcategory.ImageUrl,
            DisplayOrder = subcategory.DisplayOrder,
            IsActive = subcategory.IsActive,
            ItemCount = subcategory.MenuItems.Count,
            CreatedAt = subcategory.CreatedAt
        };
    }
}

public class RestaurantConfigService : IRestaurantConfigService
{
    private readonly IRestaurantConfigRepository _configRepository;

    public RestaurantConfigService(IRestaurantConfigRepository configRepository)
    {
        _configRepository = configRepository;
    }

    public async Task<RestaurantConfigResponse> GetConfigByRestaurantIdAsync(string restaurantId)
    {
        var config = await _configRepository.GetByRestaurantIdAsync(restaurantId);
        if (config == null)
            throw new InvalidOperationException($"Restaurant {restaurantId} not found");
        return MapToResponse(config);
    }

    public async Task<RestaurantConfigResponse> GetConfigByIdAsync(int id)
    {
        var config = await _configRepository.GetByIdAsync(id);
        if (config == null)
            throw new InvalidOperationException($"Restaurant config {id} not found");
        return MapToResponse(config);
    }

    public async Task<RestaurantConfigResponse> CreateConfigAsync(RestaurantConfigRequest request)
    {
        var config = new RestaurantConfig
        {
            RestaurantId = request.RestaurantId,
            RestaurantName = request.RestaurantName,
            RestaurantType = request.RestaurantType,
            CuisineType = request.CuisineType,
            Description = request.Description,
            LogoUrl = request.LogoUrl,
            OperatingHoursStart = request.OperatingHoursStart,
            OperatingHoursEnd = request.OperatingHoursEnd,
            CurrencyCode = request.CurrencyCode,
            IsActive = true
        };

        var created = await _configRepository.CreateAsync(config);
        return MapToResponse(created);
    }

    public async Task<RestaurantConfigResponse> UpdateConfigAsync(int id, RestaurantConfigRequest request)
    {
        var config = await _configRepository.GetByIdAsync(id);
        if (config == null)
            throw new InvalidOperationException($"Restaurant config {id} not found");

        config.RestaurantName = request.RestaurantName;
        config.RestaurantType = request.RestaurantType;
        config.CuisineType = request.CuisineType;
        config.Description = request.Description;
        config.LogoUrl = request.LogoUrl;
        config.OperatingHoursStart = request.OperatingHoursStart;
        config.OperatingHoursEnd = request.OperatingHoursEnd;
        config.CurrencyCode = request.CurrencyCode;

        var updated = await _configRepository.UpdateAsync(config);
        return MapToResponse(updated);
    }

    public async Task<bool> DeleteConfigAsync(int id)
    {
        try
        {
            await _configRepository.DeleteAsync(id);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<RestaurantConfigResponse>> GetRestaurantsByTypeAsync(string restaurantType)
    {
        var configs = await _configRepository.GetAllActiveAsync();
        return configs
            .Where(c => c.RestaurantType == restaurantType)
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<List<RestaurantConfigResponse>> GetRestaurantsByCuisineAsync(string cuisineType)
    {
        var configs = await _configRepository.GetAllActiveAsync();
        return configs
            .Where(c => c.CuisineType == cuisineType)
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<List<RestaurantConfigResponse>> GetAllActiveConfigsAsync()
    {
        var configs = await _configRepository.GetAllActiveAsync();
        return configs.Select(MapToResponse).ToList();
    }

    private RestaurantConfigResponse MapToResponse(RestaurantConfig config)
    {
        return new RestaurantConfigResponse
        {
            Id = config.Id,
            RestaurantId = config.RestaurantId,
            RestaurantName = config.RestaurantName,
            RestaurantType = config.RestaurantType,
            CuisineType = config.CuisineType,
            Description = config.Description,
            LogoUrl = config.LogoUrl,
            OperatingHoursStart = config.OperatingHoursStart,
            OperatingHoursEnd = config.OperatingHoursEnd,
            EnableSpiceLevelCustomization = config.EnableSpiceLevelCustomization,
            EnableAllergenInfo = config.EnableAllergenInfo,
            EnableNutritionalInfo = config.EnableNutritionalInfo,
            EnablePreparationTime = config.EnablePreparationTime,
            AllowSubcategories = config.AllowSubcategories,
            RequireItemVariants = config.RequireItemVariants,
            EnableDynamicPricing = config.EnableDynamicPricing,
            EnableComboDeals = config.EnableComboDeals,
            EnableBundlePricing = config.EnableBundlePricing,
            CurrencyCode = config.CurrencyCode,
            IsActive = config.IsActive,
            CreatedAt = config.CreatedAt,
            UpdatedAt = config.UpdatedAt
        };
    }
}

public class ComboDealService : IComboDealService
{
    private readonly IComboDealRepository _dealRepository;
    private readonly IMenuValidationService _validationService;

    public ComboDealService(IComboDealRepository dealRepository, IMenuValidationService validationService)
    {
        _dealRepository = dealRepository;
        _validationService = validationService;
    }

    public async Task<List<ComboDealResponse>> GetAvailableDealsAsync(int restaurantConfigId)
    {
        var deals = await _dealRepository.GetAvailableDealsAsync(restaurantConfigId);
        return deals.Select(MapToResponse).ToList();
    }

    public async Task<ComboDealResponse> GetDealByIdAsync(int id)
    {
        var deal = await _dealRepository.GetDealByIdAsync(id);
        if (deal == null)
            throw new InvalidOperationException($"Combo deal {id} not found");
        return MapToResponse(deal);
    }

    public async Task<ComboDealResponse> CreateDealAsync(int restaurantConfigId, ComboDealRequest request)
    {
        await _validationService.ValidateComboDealAsync(request);

        var deal = new ComboDeal
        {
            RestaurantConfigId = restaurantConfigId,
            DealName = request.DealName,
            Description = request.Description,
            ComboPrice = request.ComboPrice,
            IsAvailable = request.IsAvailable,
            ValidFrom = request.ValidFrom,
            ValidUntil = request.ValidUntil,
            MaxQuantityPerOrder = request.MaxQuantityPerOrder,
            ImageUrl = request.ImageUrl
        };

        foreach (var item in request.Items)
        {
            deal.Items.Add(new ComboDealItem
            {
                MenuItemId = item.MenuItemId,
                Quantity = item.Quantity,
                IsInterchangeable = item.IsInterchangeable,
                InterchangeableGroup = item.InterchangeableGroup
            });
        }

        var created = await _dealRepository.CreateDealAsync(deal);
        return MapToResponse(created);
    }

    public async Task<ComboDealResponse> UpdateDealAsync(int id, ComboDealRequest request)
    {
        await _validationService.ValidateComboDealAsync(request);

        var deal = await _dealRepository.GetDealByIdAsync(id);
        if (deal == null)
            throw new InvalidOperationException($"Combo deal {id} not found");

        deal.DealName = request.DealName;
        deal.Description = request.Description;
        deal.ComboPrice = request.ComboPrice;
        deal.IsAvailable = request.IsAvailable;
        deal.ValidFrom = request.ValidFrom;
        deal.ValidUntil = request.ValidUntil;
        deal.MaxQuantityPerOrder = request.MaxQuantityPerOrder;
        deal.ImageUrl = request.ImageUrl;

        var updated = await _dealRepository.UpdateDealAsync(deal);
        return MapToResponse(updated);
    }

    public async Task DeleteDealAsync(int id)
    {
        await _dealRepository.DeleteDealAsync(id);
    }

    public async Task<bool> ValidateDealAsync(int dealId)
    {
        var deal = await _dealRepository.GetDealByIdAsync(dealId);
        if (deal == null)
            return false;

        var now = DateTime.UtcNow;
        return deal.IsAvailable &&
               (deal.ValidFrom == null || deal.ValidFrom <= now) &&
               (deal.ValidUntil == null || deal.ValidUntil >= now);
    }

    private ComboDealResponse MapToResponse(ComboDeal deal)
    {
        var response = new ComboDealResponse
        {
            Id = deal.Id,
            DealName = deal.DealName,
            Description = deal.Description,
            ComboPrice = deal.ComboPrice,
            OriginalPrice = deal.OriginalPrice,
            IsAvailable = deal.IsAvailable,
            ValidFrom = deal.ValidFrom,
            ValidUntil = deal.ValidUntil,
            ImageUrl = deal.ImageUrl,
            CreatedAt = deal.CreatedAt,
            Items = deal.Items.Select(i => new ComboDealItemResponse
            {
                Id = i.Id,
                MenuItemId = i.MenuItemId,
                MenuItemName = i.MenuItem?.Name,
                Quantity = i.Quantity,
                IsInterchangeable = i.IsInterchangeable,
                InterchangeableGroup = i.InterchangeableGroup
            }).ToList()
        };

        // Calculate savings
        if (response.OriginalPrice.HasValue)
        {
            response.SavingsAmount = response.OriginalPrice - response.ComboPrice;
            response.SavingsPercentage = (response.SavingsAmount / response.OriginalPrice) * 100;
        }

        return response;
    }
}

public class BundlePricingService : IBundlePricingService
{
    private readonly IBundlePriceRepository _bundleRepository;

    public BundlePricingService(IBundlePriceRepository bundleRepository)
    {
        _bundleRepository = bundleRepository;
    }

    public async Task<BundlePrice?> GetApplicableBundleAsync(int menuItemId, int quantity)
    {
        return await _bundleRepository.GetApplicableBundleAsync(menuItemId, quantity);
    }

    public async Task<decimal> CalculateBundlePriceAsync(int menuItemId, int quantity)
    {
        var bundle = await _bundleRepository.GetApplicableBundleAsync(menuItemId, quantity);
        return bundle != null ? bundle.UnitPrice * quantity : 0;
    }

    public async Task<BundlePrice> CreateBundlePriceAsync(int menuItemId, int minimumQuantity, decimal unitPrice, decimal discountPercentage)
    {
        var bundlePrice = new BundlePrice
        {
            MenuItemId = menuItemId,
            MinimumQuantity = minimumQuantity,
            UnitPrice = unitPrice,
            DiscountPercentage = discountPercentage,
            IsActive = true
        };

        return await _bundleRepository.CreateAsync(bundlePrice);
    }
}
