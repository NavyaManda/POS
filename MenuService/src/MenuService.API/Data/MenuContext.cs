using Microsoft.EntityFrameworkCore;
using MenuService.API.Models;

namespace MenuService.API.Data;

public class MenuContext : DbContext
{
    public MenuContext(DbContextOptions<MenuContext> options) : base(options)
    {
    }

    public DbSet<MenuItem> MenuItems { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.HasMany(e => e.MenuItems).WithOne(m => m.Category).HasForeignKey(m => m.CategoryId);
        });

        modelBuilder.Entity<MenuItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Price).HasPrecision(10, 2);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.HasOne(e => e.Category).WithMany(c => c.MenuItems).HasForeignKey(e => e.CategoryId);
        });

        // Seed default categories
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Appetizers", Description = "Starters and appetizers" },
            new Category { Id = 2, Name = "Main Course", Description = "Main dishes" },
            new Category { Id = 3, Name = "Desserts", Description = "Sweet treats" },
            new Category { Id = 4, Name = "Beverages", Description = "Drinks and beverages" }
        );

        // Seed sample menu items
        modelBuilder.Entity<MenuItem>().HasData(
            new MenuItem 
            { 
                Id = 1, 
                Name = "Caesar Salad", 
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
