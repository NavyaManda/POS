using MenuService.API.Models;
using MenuService.API.Repositories;

namespace MenuService.API.Services;

public class MenuItemService : IMenuItemService
{
    private readonly IEnhancedMenuRepository _menuRepository;
    private readonly IMenuValidationService _validationService;

    public MenuItemService(IEnhancedMenuRepository menuRepository, IMenuValidationService validationService)
    {
        _menuRepository = menuRepository;
        _validationService = validationService;
    }

    public async Task<EnhancedMenuItem> GetMenuItemAsync(int id)
    {
        var item = await _menuRepository.GetMenuItemByIdAsync(id);
        if (item == null)
            throw new InvalidOperationException($"Menu item {id} not found");
        return item;
    }

    public async Task<List<EnhancedMenuItem>> GetMenuItemsByCategoryAsync(int categoryId)
    {
        return await _menuRepository.GetMenuItemsByCategoryAsync(categoryId);
    }

    public async Task<List<EnhancedMenuItem>> GetMenuItemsBySubcategoryAsync(int subcategoryId)
    {
        return await _menuRepository.GetMenuItemsBySubcategoryAsync(subcategoryId);
    }

    public async Task<List<EnhancedMenuItem>> GetRestaurantMenuAsync(int restaurantConfigId)
    {
        return await _menuRepository.GetMenuItemsByRestaurantAsync(restaurantConfigId);
    }

    public async Task<MenuSearchResponse<MenuItemResponse>> SearchMenuAsync(MenuSearchRequest request)
    {
        var items = await _menuRepository.SearchMenuItemsAsync(request);
        
        // Get total count (without pagination)
        var allItems = await _menuRepository.SearchMenuItemsAsync(new MenuSearchRequest
        {
            SearchTerm = request.SearchTerm,
            CategoryId = request.CategoryId,
            SubcategoryId = request.SubcategoryId,
            IsVegetarian = request.IsVegetarian,
            IsVegan = request.IsVegan,
            IsGlutenFree = request.IsGlutenFree,
            MaxPrice = request.MaxPrice,
            MaxCalories = request.MaxCalories,
            SortBy = request.SortBy,
            PageNumber = 1,
            PageSize = int.MaxValue
        });

        var responses = items.Select(MapToResponse).ToList();
        
        return new MenuSearchResponse<MenuItemResponse>
        {
            Items = responses,
            TotalCount = allItems.Count,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)allItems.Count / request.PageSize)
        };
    }

    public async Task<EnhancedMenuItem> CreateMenuItemAsync(int restaurantConfigId, MenuItemRequest request)
    {
        await _validationService.ValidateMenuItemAsync(request, restaurantConfigId);

        var menuItem = new EnhancedMenuItem
        {
            RestaurantConfigId = restaurantConfigId,
            Name = request.Name,
            Description = request.Description,
            ShortDescription = request.ShortDescription,
            BasePrice = request.BasePrice,
            SalePrice = request.SalePrice,
            CategoryId = request.CategoryId,
            SubcategoryId = request.SubcategoryId,
            IsAvailable = request.IsAvailable,
            ImageUrl = request.ImageUrl,
            Calories = request.Calories,
            Protein = request.Protein,
            Carbohydrates = request.Carbohydrates,
            Fat = request.Fat,
            IsVegetarian = request.IsVegetarian,
            IsVegan = request.IsVegan,
            IsGlutenFree = request.IsGlutenFree,
            AllergenInfo = request.AllergenInfo,
            SupportSpiceLevel = request.SupportSpiceLevel,
            DefaultSpiceLevel = request.DefaultSpiceLevel,
            PreparationTimeMinutes = request.PreparationTimeMinutes,
            Tags = request.Tags
        };

        return await _menuRepository.CreateMenuItemAsync(menuItem);
    }

    public async Task<EnhancedMenuItem> UpdateMenuItemAsync(int id, MenuItemRequest request)
    {
        var menuItem = await _menuRepository.GetMenuItemByIdAsync(id);
        if (menuItem == null)
            throw new InvalidOperationException($"Menu item {id} not found");

        await _validationService.ValidateMenuItemAsync(request, menuItem.RestaurantConfigId);

        menuItem.Name = request.Name;
        menuItem.Description = request.Description;
        menuItem.ShortDescription = request.ShortDescription;
        menuItem.BasePrice = request.BasePrice;
        menuItem.SalePrice = request.SalePrice;
        menuItem.CategoryId = request.CategoryId;
        menuItem.SubcategoryId = request.SubcategoryId;
        menuItem.IsAvailable = request.IsAvailable;
        menuItem.ImageUrl = request.ImageUrl;
        menuItem.Calories = request.Calories;
        menuItem.Protein = request.Protein;
        menuItem.Carbohydrates = request.Carbohydrates;
        menuItem.Fat = request.Fat;
        menuItem.IsVegetarian = request.IsVegetarian;
        menuItem.IsVegan = request.IsVegan;
        menuItem.IsGlutenFree = request.IsGlutenFree;
        menuItem.AllergenInfo = request.AllergenInfo;
        menuItem.SupportSpiceLevel = request.SupportSpiceLevel;
        menuItem.DefaultSpiceLevel = request.DefaultSpiceLevel;
        menuItem.PreparationTimeMinutes = request.PreparationTimeMinutes;
        menuItem.Tags = request.Tags;

        return await _menuRepository.UpdateMenuItemAsync(menuItem);
    }

    public async Task DeleteMenuItemAsync(int id)
    {
        await _menuRepository.DeleteMenuItemAsync(id);
    }

    private MenuItemResponse MapToResponse(EnhancedMenuItem item)
    {
        var discountPercentage = item.SalePrice.HasValue && item.SalePrice < item.BasePrice
            ? ((item.BasePrice - item.SalePrice) / item.BasePrice) * 100
            : null;

        return new MenuItemResponse
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            ShortDescription = item.ShortDescription,
            BasePrice = item.BasePrice,
            SalePrice = item.SalePrice,
            DiscountPercentage = discountPercentage,
            CategoryId = item.CategoryId ?? 0,
            CategoryName = item.Category?.Name,
            SubcategoryId = item.SubcategoryId,
            SubcategoryName = item.Subcategory?.Name,
            IsAvailable = item.IsAvailable,
            ImageUrl = item.ImageUrl,
            Calories = item.Calories,
            Protein = item.Protein,
            Carbohydrates = item.Carbohydrates,
            Fat = item.Fat,
            IsVegetarian = item.IsVegetarian,
            IsVegan = item.IsVegan,
            IsGlutenFree = item.IsGlutenFree,
            AllergenInfo = item.AllergenInfo,
            SupportSpiceLevel = item.SupportSpiceLevel,
            DefaultSpiceLevel = item.DefaultSpiceLevel,
            PreparationTimeMinutes = item.PreparationTimeMinutes,
            IsPopularItem = item.IsPopularItem,
            Tags = item.Tags,
            Variants = item.Variants.Select(v => MapVariantToResponse(item, v)).ToList(),
            CustomizationGroups = item.CustomizationGroups.Select(MapCustomizationGroupToResponse).ToList(),
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt
        };
    }

    private ItemVariantResponse MapVariantToResponse(EnhancedMenuItem item, ItemVariant variant)
    {
        return new ItemVariantResponse
        {
            Id = variant.Id,
            VariantType = variant.VariantType,
            VariantName = variant.VariantName,
            Description = variant.Description,
            PriceModifier = variant.PriceModifier,
            FinalPrice = item.BasePrice + variant.PriceModifier,
            IsDefault = variant.IsDefault,
            IsAvailable = variant.IsAvailable,
            DisplayOrder = variant.DisplayOrder
        };
    }

    private CustomizationGroupResponse MapCustomizationGroupToResponse(CustomizationGroup group)
    {
        return new CustomizationGroupResponse
        {
            Id = group.Id,
            GroupName = group.GroupName,
            Description = group.Description,
            SelectionType = group.SelectionType,
            MinimumSelections = group.MinimumSelections,
            MaximumSelections = group.MaximumSelections,
            IsRequired = group.IsRequired,
            DisplayOrder = group.DisplayOrder,
            Options = group.Options.Select(MapCustomizationOptionToResponse).ToList()
        };
    }

    private CustomizationOptionResponse MapCustomizationOptionToResponse(CustomizationOption option)
    {
        return new CustomizationOptionResponse
        {
            Id = option.Id,
            OptionName = option.OptionName,
            Description = option.Description,
            AdditionalPrice = option.AdditionalPrice,
            IsDefault = option.IsDefault,
            IsAvailable = option.IsAvailable,
            AdditionalCalories = option.AdditionalCalories,
            DisplayOrder = option.DisplayOrder
        };
    }
}

public class CustomizationService : ICustomizationService
{
    private readonly IEnhancedMenuRepository _menuRepository;
    private readonly IMenuValidationService _validationService;

    public CustomizationService(IEnhancedMenuRepository menuRepository, IMenuValidationService validationService)
    {
        _menuRepository = menuRepository;
        _validationService = validationService;
    }

    public async Task<List<CustomizationGroup>> GetItemCustomizationsAsync(int menuItemId)
    {
        return await _menuRepository.GetCustomizationGroupsByMenuItemAsync(menuItemId);
    }

    public async Task<CustomizationGroup> CreateCustomizationGroupAsync(int menuItemId, CustomizationGroupRequest request)
    {
        await _validationService.ValidateCustomizationGroupAsync(request);

        var group = new CustomizationGroup
        {
            MenuItemId = menuItemId,
            GroupName = request.GroupName,
            Description = request.Description,
            SelectionType = request.SelectionType,
            MinimumSelections = request.MinimumSelections,
            MaximumSelections = request.MaximumSelections,
            IsRequired = request.IsRequired,
            DisplayOrder = request.DisplayOrder
        };

        // Add options
        foreach (var optionRequest in request.Options)
        {
            group.Options.Add(new CustomizationOption
            {
                OptionName = optionRequest.OptionName,
                Description = optionRequest.Description,
                AdditionalPrice = optionRequest.AdditionalPrice,
                IsDefault = optionRequest.IsDefault,
                AdditionalCalories = optionRequest.AdditionalCalories,
                DisplayOrder = optionRequest.DisplayOrder
            });
        }

        return await _menuRepository.CreateCustomizationGroupAsync(group);
    }

    public async Task<CustomizationGroup> UpdateCustomizationGroupAsync(int id, CustomizationGroupRequest request)
    {
        await _validationService.ValidateCustomizationGroupAsync(request);

        var group = await _menuRepository.GetCustomizationGroupByIdAsync(id);
        if (group == null)
            throw new InvalidOperationException($"Customization group {id} not found");

        group.GroupName = request.GroupName;
        group.Description = request.Description;
        group.SelectionType = request.SelectionType;
        group.MinimumSelections = request.MinimumSelections;
        group.MaximumSelections = request.MaximumSelections;
        group.IsRequired = request.IsRequired;
        group.DisplayOrder = request.DisplayOrder;

        return await _menuRepository.UpdateCustomizationGroupAsync(group);
    }

    public async Task DeleteCustomizationGroupAsync(int id)
    {
        await _menuRepository.DeleteCustomizationGroupAsync(id);
    }

    public async Task<decimal> CalculateFinalPriceAsync(int menuItemId, List<int> selectedVariantIds, List<int> selectedOptionIds)
    {
        var menuItem = await _menuRepository.GetMenuItemByIdAsync(menuItemId);
        if (menuItem == null)
            throw new InvalidOperationException($"Menu item {menuItemId} not found");

        decimal price = menuItem.BasePrice;

        // Add variant prices
        foreach (var variantId in selectedVariantIds)
        {
            var variant = await _menuRepository.GetVariantByIdAsync(variantId);
            if (variant?.MenuItemId == menuItemId)
                price += variant.PriceModifier;
        }

        // Add customization option prices
        foreach (var optionId in selectedOptionIds)
        {
            var option = await _menuRepository.GetCustomizationOptionByIdAsync(optionId);
            if (option != null)
                price += option.AdditionalPrice;
        }

        return price;
    }
}

public class VariantService : IVariantService
{
    private readonly IEnhancedMenuRepository _menuRepository;

    public VariantService(IEnhancedMenuRepository menuRepository)
    {
        _menuRepository = menuRepository;
    }

    public async Task<List<ItemVariant>> GetItemVariantsAsync(int menuItemId)
    {
        return await _menuRepository.GetVariantsByMenuItemAsync(menuItemId);
    }

    public async Task<ItemVariant> CreateVariantAsync(int menuItemId, ItemVariantRequest request)
    {
        var variant = new ItemVariant
        {
            MenuItemId = menuItemId,
            VariantType = request.VariantType,
            VariantName = request.VariantName,
            Description = request.Description,
            PriceModifier = request.PriceModifier,
            IsDefault = request.IsDefault,
            DisplayOrder = request.DisplayOrder
        };

        return await _menuRepository.CreateVariantAsync(variant);
    }

    public async Task<ItemVariant> UpdateVariantAsync(int id, ItemVariantRequest request)
    {
        var variant = await _menuRepository.GetVariantByIdAsync(id);
        if (variant == null)
            throw new InvalidOperationException($"Variant {id} not found");

        variant.VariantType = request.VariantType;
        variant.VariantName = request.VariantName;
        variant.Description = request.Description;
        variant.PriceModifier = request.PriceModifier;
        variant.IsDefault = request.IsDefault;
        variant.DisplayOrder = request.DisplayOrder;

        return await _menuRepository.UpdateVariantAsync(variant);
    }

    public async Task DeleteVariantAsync(int id)
    {
        await _menuRepository.DeleteVariantAsync(id);
    }

    public async Task<decimal> GetVariantFinalPriceAsync(int menuItemId, int variantId)
    {
        var menuItem = await _menuRepository.GetMenuItemByIdAsync(menuItemId);
        if (menuItem == null)
            throw new InvalidOperationException($"Menu item {menuItemId} not found");

        var variant = await _menuRepository.GetVariantByIdAsync(variantId);
        if (variant == null || variant.MenuItemId != menuItemId)
            throw new InvalidOperationException($"Variant {variantId} not found for menu item {menuItemId}");

        return menuItem.BasePrice + variant.PriceModifier;
    }
}
