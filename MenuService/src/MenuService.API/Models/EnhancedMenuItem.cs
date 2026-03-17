namespace MenuService.API.Models;

/// <summary>
/// Enhanced MenuItem with customization support for different restaurant types
/// Supports variants, modifiers, and restaurant-specific attributes
/// </summary>
public class EnhancedMenuItem
{
    public int Id { get; set; }
    
    public int RestaurantConfigId { get; set; }
    public RestaurantConfig? RestaurantConfig { get; set; }
    
    public required string Name { get; set; }
    public required string Description { get; set; }
    public string? ShortDescription { get; set; } // Brief description for displays
    
    // Primary price
    public decimal BasePrice { get; set; }
    public decimal? SalePrice { get; set; }
    
    // Categorization
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }
    
    public int? SubcategoryId { get; set; }
    public Subcategory? Subcategory { get; set; }
    
    // Availability
    public bool IsAvailable { get; set; } = true;
    public bool IsSeasonalItem { get; set; } = false;
    public DateTime? SeasonalStartDate { get; set; }
    public DateTime? SeasonalEndDate { get; set; }
    
    // Media
    public string? ImageUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    
    // Nutritional Information
    public int? Calories { get; set; }
    public decimal? Protein { get; set; } // in grams
    public decimal? Carbohydrates { get; set; } // in grams
    public decimal? Fat { get; set; } // in grams
    
    // Dietary & Allergen Info
    public bool IsVegetarian { get; set; }
    public bool IsVegan { get; set; }
    public bool IsGlutenFree { get; set; }
    public string? AllergenInfo { get; set; } // Comma-separated: nuts, dairy, etc.
    
    // Spice Level (for Indian cuisines)
    public bool SupportSpiceLevel { get; set; } = false;
    public SpiceLevel DefaultSpiceLevel { get; set; } = SpiceLevel.Medium;
    
    // Preparation
    public int PreparationTimeMinutes { get; set; } = 15;
    public bool IsPopularItem { get; set; } = false;
    public int? RecommendationScore { get; set; } // 1-5 rating
    
    // Variants & Modifiers
    public List<ItemVariant> Variants { get; set; } = new();
    public List<CustomizationGroup> CustomizationGroups { get; set; } = new();
    
    // Metadata
    public string? Tags { get; set; } // Comma-separated for search: "hot", "bestseller", "new"
    public string? Notes { get; set; } // Internal notes
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public int DisplayOrder { get; set; }
}

public enum SpiceLevel
{
    Mild = 1,
    Medium = 2,
    Hot = 3,
    VeryHot = 4,
    Extreme = 5
}
