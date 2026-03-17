namespace MenuService.API.Models;

/// <summary>
/// Restaurant configuration to support different cuisines and restaurant types
/// Allows menu customization for Pizza, Biryani, Breakfast, etc.
/// </summary>
public class RestaurantConfig
{
    public int Id { get; set; }
    
    public required string RestaurantId { get; set; }
    public required string RestaurantName { get; set; }
    
    /// <summary>
    /// Restaurant type: Pizza, Biryani, Breakfast, FastFood, Fine Dining, etc.
    /// </summary>
    public required string RestaurantType { get; set; }
    
    /// <summary>
    /// Cuisine type: Italian, Indian, Continental, American, etc.
    /// </summary>
    public required string CuisineType { get; set; }
    
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    
    // Operating hours
    public TimeSpan OperatingHoursStart { get; set; }
    public TimeSpan OperatingHoursEnd { get; set; }
    
    // Customization options
    public bool EnableSpiceLevelCustomization { get; set; } = true;
    public bool EnableAllergenInfo { get; set; } = true;
    public bool EnableNutritionalInfo { get; set; } = true;
    public bool EnablePreparationTime { get; set; } = true;
    
    // Menu structure
    public bool AllowSubcategories { get; set; } = true;
    public bool RequireItemVariants { get; set; } = false;
    public int MaxCustomizationOptionsPerItem { get; set; } = 5;
    
    // Pricing
    public required string CurrencyCode { get; set; } = "USD";
    public bool EnableDynamicPricing { get; set; } = false;
    
    // Features
    public bool EnableComboDeals { get; set; } = true;
    public bool EnableBundlePricing { get; set; } = true;
    public bool EnableSeasonalItems { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    
    // Navigation
    public List<Category> Categories { get; set; } = new();
    public List<EnhancedMenuItem> MenuItems { get; set; } = new();
    public List<ComboDeal> ComboDeals { get; set; } = new();
}

