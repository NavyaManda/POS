namespace MenuService.API.Models;

/// <summary>
/// Subcategory for better menu organization
/// Example: Under "Pizzas" - Vegetarian Pizzas, Meat Pizzas, etc.
/// Under "Biryani" - Chicken Biryani, Lamb Biryani, Vegetable Biryani
/// </summary>
public class Subcategory
{
    public int Id { get; set; }
    
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
    
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public List<EnhancedMenuItem> MenuItems { get; set; } = new();
}

/// <summary>
/// Item variants for size, portion, or preparation options
/// Example for Pizza: Small, Medium, Large
/// Example for Biryani: Half, Full, Family Pack
/// Example for Breakfast: Continental, Indian, Mixed
/// </summary>
public class ItemVariant
{
    public int Id { get; set; }
    
    public int MenuItemId { get; set; }
    public EnhancedMenuItem? MenuItem { get; set; }
    
    /// <summary>
    /// Variant type: Size, Portion, Preparation, Base, etc.
    /// </summary>
    public required string VariantType { get; set; }
    
    /// <summary>
    /// Variant name: Small, Medium, Large / Half, Full, etc.
    /// </summary>
    public required string VariantName { get; set; }
    
    public string? Description { get; set; }
    
    /// <summary>
    /// Price modifier: base price + this = variant price
    /// For example, if base pizza is $10 and Large adds $3, Large = $13
    /// </summary>
    public decimal PriceModifier { get; set; } = 0;
    
    public bool IsDefault { get; set; } = false;
    public int DisplayOrder { get; set; }
    public bool IsAvailable { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Customization groups for add-ons and modifications
/// Example: Toppings, Sauces, Cheese Options, etc.
/// </summary>
public class CustomizationGroup
{
    public int Id { get; set; }
    
    public int MenuItemId { get; set; }
    public EnhancedMenuItem? MenuItem { get; set; }
    
    /// <summary>
    /// Group name: Toppings, Sauces, Extra Cheese, Spice Level, etc.
    /// </summary>
    public required string GroupName { get; set; }
    
    public string? Description { get; set; }
    
    /// <summary>
    /// Selection type: SingleSelect, MultiSelect (checkbox)
    /// </summary>
    public SelectionType SelectionType { get; set; } = SelectionType.MultiSelect;
    
    /// <summary>
    /// Minimum and maximum selections allowed
    /// Example: Select at least 1 topping, max 5
    /// </summary>
    public int MinimumSelections { get; set; } = 0;
    public int MaximumSelections { get; set; } = 10;
    
    public bool IsRequired { get; set; } = false;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    
    public List<CustomizationOption> Options { get; set; } = new();
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum SelectionType
{
    SingleSelect = 1,
    MultiSelect = 2
}

/// <summary>
/// Individual customization options within a group
/// Example in Toppings group: Pepperoni, Mushrooms, Onions, etc.
/// </summary>
public class CustomizationOption
{
    public int Id { get; set; }
    
    public int CustomizationGroupId { get; set; }
    public CustomizationGroup? CustomizationGroup { get; set; }
    
    public required string OptionName { get; set; }
    public string? Description { get; set; }
    
    /// <summary>
    /// Price to add if this option is selected
    /// </summary>
    public decimal AdditionalPrice { get; set; } = 0;
    
    public bool IsDefault { get; set; } = false;
    public int DisplayOrder { get; set; }
    public bool IsAvailable { get; set; } = true;
    
    /// <summary>
    /// Calories if this option is added
    /// </summary>
    public int? AdditionalCalories { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
