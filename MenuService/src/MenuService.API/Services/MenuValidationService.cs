using MenuService.API.Models;
using MenuService.API.Repositories;

namespace MenuService.API.Services;

public class MenuValidationService : IMenuValidationService
{
    private readonly IEnhancedMenuRepository _menuRepository;

    public MenuValidationService(IEnhancedMenuRepository menuRepository)
    {
        _menuRepository = menuRepository;
    }

    public async Task ValidateMenuItemAsync(MenuItemRequest request, int restaurantConfigId)
    {
        var errors = new List<string>();

        // Basic validation
        if (string.IsNullOrWhiteSpace(request.Name))
            errors.Add("Menu item name is required");

        if (string.IsNullOrWhiteSpace(request.Description))
            errors.Add("Menu item description is required");

        if (request.BasePrice <= 0)
            errors.Add("Base price must be greater than zero");

        if (request.SalePrice.HasValue && request.SalePrice <= 0)
            errors.Add("Sale price must be greater than zero");

        if (request.SalePrice.HasValue && request.SalePrice > request.BasePrice)
            errors.Add("Sale price cannot be greater than base price");

        if (request.PreparationTimeMinutes < 0)
            errors.Add("Preparation time cannot be negative");

        // Category validation
        if (request.CategoryId <= 0)
            errors.Add("Valid category ID is required");
        else
        {
            var category = await _menuRepository.GetCategoryByIdAsync(request.CategoryId);
            if (category == null || category.RestaurantConfigId != restaurantConfigId)
                errors.Add($"Category {request.CategoryId} not found for this restaurant");
        }

        // Subcategory validation
        if (request.SubcategoryId.HasValue && request.SubcategoryId > 0)
        {
            var subcategory = await _menuRepository.GetSubcategoryByIdAsync(request.SubcategoryId.Value);
            if (subcategory == null)
                errors.Add($"Subcategory {request.SubcategoryId} not found");
        }

        // Nutritional info validation
        if (request.Calories.HasValue && request.Calories < 0)
            errors.Add("Calories cannot be negative");

        if (request.Protein.HasValue && request.Protein < 0)
            errors.Add("Protein cannot be negative");

        if (request.Carbohydrates.HasValue && request.Carbohydrates < 0)
            errors.Add("Carbohydrates cannot be negative");

        if (request.Fat.HasValue && request.Fat < 0)
            errors.Add("Fat cannot be negative");

        if (errors.Any())
            throw new ArgumentException($"Menu item validation failed: {string.Join(", ", errors)}");
    }

    public async Task ValidateCustomizationGroupAsync(CustomizationGroupRequest request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.GroupName))
            errors.Add("Customization group name is required");

        if (request.MinimumSelections < 0)
            errors.Add("Minimum selections cannot be negative");

        if (request.MaximumSelections < request.MinimumSelections)
            errors.Add("Maximum selections cannot be less than minimum selections");

        if (request.Options == null || !request.Options.Any())
            errors.Add("At least one customization option is required");

        // Validate options
        if (request.Options != null)
        {
            var optionNames = new HashSet<string>();
            foreach (var option in request.Options)
            {
                if (string.IsNullOrWhiteSpace(option.OptionName))
                    errors.Add("Customization option name cannot be empty");

                if (option.AdditionalPrice < 0)
                    errors.Add($"Additional price for '{option.OptionName}' cannot be negative");

                if (option.AdditionalCalories.HasValue && option.AdditionalCalories < 0)
                    errors.Add($"Additional calories for '{option.OptionName}' cannot be negative");

                if (!optionNames.Add(option.OptionName))
                    errors.Add($"Duplicate customization option: '{option.OptionName}'");
            }
        }

        if (errors.Any())
            throw new ArgumentException($"Customization group validation failed: {string.Join(", ", errors)}");
    }

    public async Task ValidateComboDealAsync(ComboDealRequest request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.DealName))
            errors.Add("Combo deal name is required");

        if (request.ComboPrice <= 0)
            errors.Add("Combo price must be greater than zero");

        if (request.Items == null || !request.Items.Any())
            errors.Add("At least one item is required in the combo deal");

        if (request.MaxQuantityPerOrder <= 0)
            errors.Add("Max quantity per order must be greater than zero");

        // Validate date range
        if (request.ValidFrom.HasValue && request.ValidUntil.HasValue && request.ValidFrom > request.ValidUntil)
            errors.Add("ValidFrom date cannot be after ValidUntil date");

        // Validate items
        if (request.Items != null)
        {
            foreach (var item in request.Items)
            {
                if (item.MenuItemId <= 0)
                    errors.Add("Valid menu item ID is required for each combo item");

                if (item.Quantity <= 0)
                    errors.Add("Quantity must be at least 1 for each combo item");

                if (item.IsInterchangeable && string.IsNullOrWhiteSpace(item.InterchangeableGroup))
                    errors.Add("Interchangeable group name is required for interchangeable items");
            }
        }

        if (errors.Any())
            throw new ArgumentException($"Combo deal validation failed: {string.Join(", ", errors)}");
    }

    public async Task<bool> IsMenuItemAvailableAsync(int menuItemId)
    {
        var item = await _menuRepository.GetMenuItemByIdAsync(menuItemId);
        if (item == null)
            return false;

        // Check if item is available
        if (!item.IsAvailable)
            return false;

        // Check if seasonal item is in season
        if (item.IsSeasonalItem)
        {
            var now = DateTime.UtcNow;
            if (item.SeasonalStartDate.HasValue && item.SeasonalStartDate > now)
                return false;
            if (item.SeasonalEndDate.HasValue && item.SeasonalEndDate < now)
                return false;
        }

        return true;
    }

    public async Task<bool> AreCustomizationsValidAsync(int menuItemId, List<int> selectedOptionIds)
    {
        if (selectedOptionIds == null || !selectedOptionIds.Any())
            return true; // No customizations required

        var customizationGroups = await _menuRepository.GetCustomizationGroupsByMenuItemAsync(menuItemId);

        if (!customizationGroups.Any())
            return true; // No customization groups for this item

        // Verify each required customization group has valid selections
        foreach (var group in customizationGroups.Where(g => g.IsRequired))
        {
            var selectionsForGroup = selectedOptionIds
                .Where(optionId => group.Options.Any(o => o.Id == optionId))
                .Count();

            if (selectionsForGroup < group.MinimumSelections)
                return false;

            if (selectionsForGroup > group.MaximumSelections)
                return false;
        }

        return true;
    }
}
