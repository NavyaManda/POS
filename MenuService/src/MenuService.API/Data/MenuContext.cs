using Microsoft.EntityFrameworkCore;
using MenuService.API.Models;

namespace MenuService.API.Data;

public class MenuContext : DbContext
{
    public MenuContext(DbContextOptions<MenuContext> options) : base(options)
    {
    }

    // Backward compatibility
    public DbSet<MenuItem> MenuItems { get; set; }
    public DbSet<Category> Categories { get; set; }
    
    // New enhanced models
    public DbSet<RestaurantConfig> RestaurantConfigs { get; set; }
    public DbSet<EnhancedMenuItem> EnhancedMenuItems { get; set; }
    public DbSet<Subcategory> Subcategories { get; set; }
    public DbSet<ItemVariant> ItemVariants { get; set; }
    public DbSet<CustomizationGroup> CustomizationGroups { get; set; }
    public DbSet<CustomizationOption> CustomizationOptions { get; set; }
    public DbSet<ComboDeal> ComboDeals { get; set; }
    public DbSet<ComboDealItem> ComboDealItems { get; set; }
    public DbSet<BundlePrice> BundlePrices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure RestaurantConfig
        modelBuilder.Entity<RestaurantConfig>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RestaurantId).IsRequired().HasMaxLength(50);
            entity.Property(e => e.RestaurantName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.RestaurantType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CuisineType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CurrencyCode).IsRequired().HasMaxLength(3);
            entity.HasIndex(e => e.RestaurantId).IsUnique();
            entity.HasMany(e => e.Categories).WithOne(c => c.RestaurantConfig).HasForeignKey(c => c.RestaurantConfigId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.MenuItems).WithOne(m => m.RestaurantConfig).HasForeignKey(m => m.RestaurantConfigId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.ComboDeals).WithOne(c => c.RestaurantConfig).HasForeignKey(c => c.RestaurantConfigId).OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Category with Restaurant
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.HasMany(e => e.MenuItems).WithOne(m => m.Category).HasForeignKey(m => m.CategoryId).OnDelete(DeleteBehavior.SetNull);
            entity.HasMany(e => e.Subcategories).WithOne(s => s.Category).HasForeignKey(s => s.CategoryId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.EnhancedMenuItems).WithOne(m => m.Category).HasForeignKey(m => m.CategoryId).OnDelete(DeleteBehavior.SetNull);
        });

        // Configure Subcategory
        modelBuilder.Entity<Subcategory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.HasMany(e => e.MenuItems).WithOne(m => m.Subcategory).HasForeignKey(m => m.SubcategoryId).OnDelete(DeleteBehavior.SetNull);
        });

        // Configure EnhancedMenuItem
        modelBuilder.Entity<EnhancedMenuItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.ShortDescription).HasMaxLength(200);
            entity.Property(e => e.BasePrice).HasPrecision(10, 2);
            entity.Property(e => e.SalePrice).HasPrecision(10, 2);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.ThumbnailUrl).HasMaxLength(500);
            entity.Property(e => e.Protein).HasPrecision(5, 2);
            entity.Property(e => e.Carbohydrates).HasPrecision(5, 2);
            entity.Property(e => e.Fat).HasPrecision(5, 2);
            entity.Property(e => e.AllergenInfo).HasMaxLength(500);
            entity.Property(e => e.Tags).HasMaxLength(500);
            entity.HasMany(e => e.Variants).WithOne(v => v.MenuItem).HasForeignKey(v => v.MenuItemId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.CustomizationGroups).WithOne(c => c.MenuItem).HasForeignKey(c => c.MenuItemId).OnDelete(DeleteBehavior.Cascade);
        });

        // Configure ItemVariant
        modelBuilder.Entity<ItemVariant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.VariantType).IsRequired().HasMaxLength(50);
            entity.Property(e => e.VariantName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(300);
            entity.Property(e => e.PriceModifier).HasPrecision(10, 2);
        });

        // Configure CustomizationGroup
        modelBuilder.Entity<CustomizationGroup>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.GroupName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(300);
            entity.HasMany(e => e.Options).WithOne(o => o.CustomizationGroup).HasForeignKey(o => o.CustomizationGroupId).OnDelete(DeleteBehavior.Cascade);
        });

        // Configure CustomizationOption
        modelBuilder.Entity<CustomizationOption>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OptionName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(300);
            entity.Property(e => e.AdditionalPrice).HasPrecision(10, 2);
        });

        // Configure ComboDeal
        modelBuilder.Entity<ComboDeal>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DealName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ComboPrice).HasPrecision(10, 2);
            entity.Property(e => e.OriginalPrice).HasPrecision(10, 2);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.HasMany(e => e.Items).WithOne(i => i.ComboDeal).HasForeignKey(i => i.ComboDealId).OnDelete(DeleteBehavior.Cascade);
        });

        // Configure ComboDealItem
        modelBuilder.Entity<ComboDealItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.InterchangeableGroup).HasMaxLength(100);
        });

        // Configure BundlePrice
        modelBuilder.Entity<BundlePrice>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UnitPrice).HasPrecision(10, 2);
            entity.Property(e => e.DiscountPercentage).HasPrecision(5, 2);
        });

        // Legacy MenuItem configuration (keep for backward compatibility)
        modelBuilder.Entity<MenuItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Price).HasPrecision(10, 2);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.HasOne(e => e.Category).WithMany(c => c.MenuItems).HasForeignKey(e => e.CategoryId).OnDelete(DeleteBehavior.SetNull);
        });

        // Seed default RestaurantConfig for Pizza
        modelBuilder.Entity<RestaurantConfig>().HasData(
            new RestaurantConfig
            {
                Id = 1,
                RestaurantId = "pizza_place_001",
                RestaurantName = "Pizza Palace",
                RestaurantType = "Pizza",
                CuisineType = "Italian",
                Description = "Premium Italian Pizza Restaurant",
                OperatingHoursStart = new TimeSpan(11, 0, 0),
                OperatingHoursEnd = new TimeSpan(23, 0, 0),
                CurrencyCode = "USD",
                EnableSpiceLevelCustomization = true,
                AllowSubcategories = true,
                EnableComboDeals = true,
                EnableBundlePricing = true,
                IsActive = true
            }
        );

        // Seed default categories for Pizza restaurant
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, RestaurantConfigId = 1, Name = "Pizzas", Description = "All types of pizzas", DisplayOrder = 1, IsActive = true },
            new Category { Id = 2, RestaurantConfigId = 1, Name = "Appetizers", Description = "Starters and appetizers", DisplayOrder = 2, IsActive = true },
            new Category { Id = 3, RestaurantConfigId = 1, Name = "Desserts", Description = "Sweet treats", DisplayOrder = 3, IsActive = true },
            new Category { Id = 4, RestaurantConfigId = 1, Name = "Beverages", Description = "Drinks and beverages", DisplayOrder = 4, IsActive = true }
        );
    }
} 
                Description = "Fresh romaine lettuce with Caesar dressing",
                Price = 8.99m,
                CategoryId = 1,
                IsAvailable = true,
                IsVegetarian = true,
                IsSpicy = false,
                Calories = 250
            },
            new MenuItem 
            { 
                Id = 2, 
                Name = "Grilled Chicken Breast", 
                Description = "Tender grilled chicken with seasonal vegetables",
                Price = 15.99m,
                CategoryId = 2,
                IsAvailable = true,
                IsVegetarian = false,
                IsSpicy = false,
                Calories = 350
            },
            new MenuItem 
            { 
                Id = 3, 
                Name = "Spicy Thai Curry", 
                Description = "Traditional Thai curry with coconut milk and spices",
                Price = 14.99m,
                CategoryId = 2,
                IsAvailable = true,
                IsVegetarian = false,
                IsSpicy = true,
                Calories = 450
            },
            new MenuItem 
            { 
                Id = 4, 
                Name = "Chocolate Cake", 
                Description = "Decadent chocolate layer cake",
                Price = 6.99m,
                CategoryId = 3,
                IsAvailable = true,
                IsVegetarian = true,
                IsSpicy = false,
                Calories = 450
            }
        );
    }
}
