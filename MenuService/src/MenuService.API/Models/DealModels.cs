namespace MenuService.API.Models;

/// <summary>
/// Combo deals for bundled pricing
/// Example: 1 Pizza + 1 Coke + 1 Garlic Bread = Special Price
/// </summary>
public class ComboDeal
{
    public int Id { get; set; }
    
    public int RestaurantConfigId { get; set; }
    public RestaurantConfig? RestaurantConfig { get; set; }
    
    public required string DealName { get; set; }
    public string? Description { get; set; }
    
    public decimal ComboPrice { get; set; }
    public decimal? OriginalPrice { get; set; } // Sum of individual prices
    
    public bool IsAvailable { get; set; } = true;
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }
    
    public int MaxQuantityPerOrder { get; set; } = 5;
    public string? ImageUrl { get; set; }
    
    public List<ComboDealItem> Items { get; set; } = new();
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public int DisplayOrder { get; set; }
}

/// <summary>
/// Items included in a combo deal
/// </summary>
public class ComboDealItem
{
    public int Id { get; set; }
    
    public int ComboDealId { get; set; }
    public ComboDeal? ComboDeal { get; set; }
    
    public int MenuItemId { get; set; }
    public EnhancedMenuItem? MenuItem { get; set; }
    
    /// <summary>
    /// Quantity of this item in the combo
    /// </summary>
    public int Quantity { get; set; } = 1;
    
    /// <summary>
    /// Is this item interchangeable with other similar items?
    /// Example: Choose any 1 beverage
    /// </summary>
    public bool IsInterchangeable { get; set; } = false;
    public string? InterchangeableGroup { get; set; } // "Beverages", "Desserts", etc.
    
    public int DisplayOrder { get; set; }
}

/// <summary>
/// Bundle pricing for quantity discounts
/// Example: Buy 3 Biryani, get 10% off. Buy 5, get 15% off.
/// </summary>
public class BundlePrice
{
    public int Id { get; set; }
    
    public int MenuItemId { get; set; }
    public EnhancedMenuItem? MenuItem { get; set; }
    
    /// <summary>
    /// Minimum quantity to trigger this bundle price
    /// </summary>
    public int MinimumQuantity { get; set; }
    
    /// <summary>
    /// Price per unit at this quantity level
    /// </summary>
    public decimal UnitPrice { get; set; }
    
    /// <summary>
    /// Discount percentage applied
    /// </summary>
    public decimal DiscountPercentage { get; set; }
    
    public bool IsActive { get; set; } = true;
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
