namespace MenuService.API.Models;

#region Request/Response Models

public class MenuItemRequest
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public string? ShortDescription { get; set; }
    public decimal BasePrice { get; set; }
    public decimal? SalePrice { get; set; }
    public int CategoryId { get; set; }
    public int? SubcategoryId { get; set; }
    public bool IsAvailable { get; set; } = true;
    public string? ImageUrl { get; set; }
    public int? Calories { get; set; }
    public decimal? Protein { get; set; }
    public decimal? Carbohydrates { get; set; }
    public decimal? Fat { get; set; }
    public bool IsVegetarian { get; set; }
    public bool IsVegan { get; set; }
    public bool IsGlutenFree { get; set; }
    public string? AllergenInfo { get; set; }
    public bool SupportSpiceLevel { get; set; } = false;
    public SpiceLevel DefaultSpiceLevel { get; set; } = SpiceLevel.Medium;
    public int PreparationTimeMinutes { get; set; } = 15;
    public string? Tags { get; set; }
}

public class MenuItemResponse
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public string? ShortDescription { get; set; }
    public decimal BasePrice { get; set; }
    public decimal? SalePrice { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public int? SubcategoryId { get; set; }
    public string? SubcategoryName { get; set; }
    public bool IsAvailable { get; set; }
    public string? ImageUrl { get; set; }
    public int? Calories { get; set; }
    public decimal? Protein { get; set; }
    public decimal? Carbohydrates { get; set; }
    public decimal? Fat { get; set; }
    public bool IsVegetarian { get; set; }
    public bool IsVegan { get; set; }
    public bool IsGlutenFree { get; set; }
    public string? AllergenInfo { get; set; }
    public bool SupportSpiceLevel { get; set; }
    public SpiceLevel DefaultSpiceLevel { get; set; }
    public int PreparationTimeMinutes { get; set; }
    public bool IsPopularItem { get; set; }
    public string? Tags { get; set; }
    public List<ItemVariantResponse> Variants { get; set; } = new();
    public List<CustomizationGroupResponse> CustomizationGroups { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CategoryRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int DisplayOrder { get; set; }
}

public class CategoryResponse
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int ItemCount { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public List<SubcategoryResponse> Subcategories { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class SubcategoryRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int DisplayOrder { get; set; }
}

public class SubcategoryResponse
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public int ItemCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ItemVariantRequest
{
    public required string VariantType { get; set; }
    public required string VariantName { get; set; }
    public string? Description { get; set; }
    public decimal PriceModifier { get; set; }
    public bool IsDefault { get; set; }
    public int DisplayOrder { get; set; }
}

public class ItemVariantResponse
{
    public int Id { get; set; }
    public required string VariantType { get; set; }
    public required string VariantName { get; set; }
    public string? Description { get; set; }
    public decimal PriceModifier { get; set; }
    public decimal FinalPrice { get; set; }
    public bool IsDefault { get; set; }
    public bool IsAvailable { get; set; }
    public int DisplayOrder { get; set; }
}

public class CustomizationGroupRequest
{
    public required string GroupName { get; set; }
    public string? Description { get; set; }
    public SelectionType SelectionType { get; set; }
    public int MinimumSelections { get; set; }
    public int MaximumSelections { get; set; }
    public bool IsRequired { get; set; }
    public int DisplayOrder { get; set; }
    public List<CustomizationOptionRequest> Options { get; set; } = new();
}

public class CustomizationGroupResponse
{
    public int Id { get; set; }
    public required string GroupName { get; set; }
    public string? Description { get; set; }
    public SelectionType SelectionType { get; set; }
    public int MinimumSelections { get; set; }
    public int MaximumSelections { get; set; }
    public bool IsRequired { get; set; }
    public int DisplayOrder { get; set; }
    public List<CustomizationOptionResponse> Options { get; set; } = new();
}

public class CustomizationOptionRequest
{
    public required string OptionName { get; set; }
    public string? Description { get; set; }
    public decimal AdditionalPrice { get; set; }
    public bool IsDefault { get; set; }
    public int? AdditionalCalories { get; set; }
    public int DisplayOrder { get; set; }
}

public class CustomizationOptionResponse
{
    public int Id { get; set; }
    public required string OptionName { get; set; }
    public string? Description { get; set; }
    public decimal AdditionalPrice { get; set; }
    public bool IsDefault { get; set; }
    public bool IsAvailable { get; set; }
    public int? AdditionalCalories { get; set; }
    public int DisplayOrder { get; set; }
}

public class RestaurantConfigRequest
{
    public required string RestaurantId { get; set; }
    public required string RestaurantName { get; set; }
    public required string RestaurantType { get; set; } // Pizza, Biryani, Breakfast, etc.
    public required string CuisineType { get; set; } // Italian, Indian, American, etc.
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public TimeSpan OperatingHoursStart { get; set; }
    public TimeSpan OperatingHoursEnd { get; set; }
    public string CurrencyCode { get; set; } = "USD";
}

public class RestaurantConfigResponse
{
    public int Id { get; set; }
    public required string RestaurantId { get; set; }
    public required string RestaurantName { get; set; }
    public required string RestaurantType { get; set; }
    public required string CuisineType { get; set; }
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public TimeSpan OperatingHoursStart { get; set; }
    public TimeSpan OperatingHoursEnd { get; set; }
    public bool EnableSpiceLevelCustomization { get; set; }
    public bool EnableAllergenInfo { get; set; }
    public bool EnableNutritionalInfo { get; set; }
    public bool EnablePreparationTime { get; set; }
    public bool AllowSubcategories { get; set; }
    public bool RequireItemVariants { get; set; }
    public bool EnableDynamicPricing { get; set; }
    public bool EnableComboDeals { get; set; }
    public bool EnableBundlePricing { get; set; }
    public string CurrencyCode { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class ComboDealRequest
{
    public required string DealName { get; set; }
    public string? Description { get; set; }
    public decimal ComboPrice { get; set; }
    public bool IsAvailable { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }
    public int MaxQuantityPerOrder { get; set; }
    public string? ImageUrl { get; set; }
    public List<ComboDealItemRequest> Items { get; set; } = new();
}

public class ComboDealResponse
{
    public int Id { get; set; }
    public required string DealName { get; set; }
    public string? Description { get; set; }
    public decimal ComboPrice { get; set; }
    public decimal? OriginalPrice { get; set; }
    public decimal? SavingsAmount { get; set; }
    public decimal? SavingsPercentage { get; set; }
    public bool IsAvailable { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }
    public string? ImageUrl { get; set; }
    public List<ComboDealItemResponse> Items { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class ComboDealItemRequest
{
    public int MenuItemId { get; set; }
    public int Quantity { get; set; } = 1;
    public bool IsInterchangeable { get; set; }
    public string? InterchangeableGroup { get; set; }
}

public class ComboDealItemResponse
{
    public int Id { get; set; }
    public int MenuItemId { get; set; }
    public string? MenuItemName { get; set; }
    public int Quantity { get; set; }
    public bool IsInterchangeable { get; set; }
    public string? InterchangeableGroup { get; set; }
}

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public required string Message { get; set; }
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
}

public class MenuSearchRequest
{
    public string? SearchTerm { get; set; }
    public int? CategoryId { get; set; }
    public int? SubcategoryId { get; set; }
    public bool? IsVegetarian { get; set; }
    public bool? IsVegan { get; set; }
    public bool? IsGlutenFree { get; set; }
    public int? MaxPrice { get; set; }
    public int? MaxCalories { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; } = "name"; // name, price, popularity, newest
}

public class MenuSearchResponse<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

#endregion

