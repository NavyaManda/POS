namespace MenuService.API.Models;

/// <summary>
/// Menu category for organizing items
/// Example: Pizzas, Appetizers, Main Course, Desserts (for different restaurant types)
/// </summary>
public class Category
{
    public int Id { get; set; }
    
    public int RestaurantConfigId { get; set; }
    public RestaurantConfig? RestaurantConfig { get; set; }
    
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    
    /// <summary>
    /// Display order in the menu
    /// </summary>
    public int DisplayOrder { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    // Navigation - for backward compatibility
    public List<MenuItem> MenuItems { get; set; } = new();
    
    // Navigation - for new enhanced structure
    public List<Subcategory> Subcategories { get; set; } = new();
    public List<EnhancedMenuItem> EnhancedMenuItems { get; set; } = new();
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
