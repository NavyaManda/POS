using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MenuService.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialMenuServiceMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RestaurantConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RestaurantId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    RestaurantName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    RestaurantType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CuisineType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    LogoUrl = table.Column<string>(type: "TEXT", nullable: true),
                    OperatingHoursStart = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    OperatingHoursEnd = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    EnableSpiceLevelCustomization = table.Column<bool>(type: "INTEGER", nullable: false),
                    EnableAllergenInfo = table.Column<bool>(type: "INTEGER", nullable: false),
                    EnableNutritionalInfo = table.Column<bool>(type: "INTEGER", nullable: false),
                    EnablePreparationTime = table.Column<bool>(type: "INTEGER", nullable: false),
                    AllowSubcategories = table.Column<bool>(type: "INTEGER", nullable: false),
                    RequireItemVariants = table.Column<bool>(type: "INTEGER", nullable: false),
                    MaxCustomizationOptionsPerItem = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrencyCode = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    EnableDynamicPricing = table.Column<bool>(type: "INTEGER", nullable: false),
                    EnableComboDeals = table.Column<bool>(type: "INTEGER", nullable: false),
                    EnableBundlePricing = table.Column<bool>(type: "INTEGER", nullable: false),
                    EnableSeasonalItems = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestaurantConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RestaurantConfigId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ImageUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_RestaurantConfigs_RestaurantConfigId",
                        column: x => x.RestaurantConfigId,
                        principalTable: "RestaurantConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComboDeals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RestaurantConfigId = table.Column<int>(type: "INTEGER", nullable: false),
                    DealName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ComboPrice = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    OriginalPrice = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: true),
                    IsAvailable = table.Column<bool>(type: "INTEGER", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ValidUntil = table.Column<DateTime>(type: "TEXT", nullable: true),
                    MaxQuantityPerOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComboDeals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComboDeals_RestaurantConfigs_RestaurantConfigId",
                        column: x => x.RestaurantConfigId,
                        principalTable: "RestaurantConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MenuItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsAvailable = table.Column<bool>(type: "INTEGER", nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Calories = table.Column<int>(type: "INTEGER", nullable: false),
                    IsVegetarian = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsSpicy = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuItems_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Subcategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: true),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subcategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subcategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EnhancedMenuItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RestaurantConfigId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    ShortDescription = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    BasePrice = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    SalePrice = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: true),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: true),
                    SubcategoryId = table.Column<int>(type: "INTEGER", nullable: true),
                    IsAvailable = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsSeasonalItem = table.Column<bool>(type: "INTEGER", nullable: false),
                    SeasonalStartDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SeasonalEndDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ImageUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ThumbnailUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Calories = table.Column<int>(type: "INTEGER", nullable: true),
                    Protein = table.Column<decimal>(type: "TEXT", precision: 5, scale: 2, nullable: true),
                    Carbohydrates = table.Column<decimal>(type: "TEXT", precision: 5, scale: 2, nullable: true),
                    Fat = table.Column<decimal>(type: "TEXT", precision: 5, scale: 2, nullable: true),
                    IsVegetarian = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsVegan = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsGlutenFree = table.Column<bool>(type: "INTEGER", nullable: false),
                    AllergenInfo = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    SupportSpiceLevel = table.Column<bool>(type: "INTEGER", nullable: false),
                    DefaultSpiceLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    PreparationTimeMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    IsPopularItem = table.Column<bool>(type: "INTEGER", nullable: false),
                    RecommendationScore = table.Column<int>(type: "INTEGER", nullable: true),
                    Tags = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnhancedMenuItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnhancedMenuItems_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_EnhancedMenuItems_RestaurantConfigs_RestaurantConfigId",
                        column: x => x.RestaurantConfigId,
                        principalTable: "RestaurantConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EnhancedMenuItems_Subcategories_SubcategoryId",
                        column: x => x.SubcategoryId,
                        principalTable: "Subcategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "BundlePrices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MenuItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    MinimumQuantity = table.Column<int>(type: "INTEGER", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "TEXT", precision: 5, scale: 2, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ValidUntil = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BundlePrices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BundlePrices_EnhancedMenuItems_MenuItemId",
                        column: x => x.MenuItemId,
                        principalTable: "EnhancedMenuItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComboDealItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ComboDealId = table.Column<int>(type: "INTEGER", nullable: false),
                    MenuItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    IsInterchangeable = table.Column<bool>(type: "INTEGER", nullable: false),
                    InterchangeableGroup = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComboDealItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComboDealItems_ComboDeals_ComboDealId",
                        column: x => x.ComboDealId,
                        principalTable: "ComboDeals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComboDealItems_EnhancedMenuItems_MenuItemId",
                        column: x => x.MenuItemId,
                        principalTable: "EnhancedMenuItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomizationGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MenuItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    GroupName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    SelectionType = table.Column<int>(type: "INTEGER", nullable: false),
                    MinimumSelections = table.Column<int>(type: "INTEGER", nullable: false),
                    MaximumSelections = table.Column<int>(type: "INTEGER", nullable: false),
                    IsRequired = table.Column<bool>(type: "INTEGER", nullable: false),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomizationGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomizationGroups_EnhancedMenuItems_MenuItemId",
                        column: x => x.MenuItemId,
                        principalTable: "EnhancedMenuItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemVariants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MenuItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    VariantType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    VariantName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    PriceModifier = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsAvailable = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemVariants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemVariants_EnhancedMenuItems_MenuItemId",
                        column: x => x.MenuItemId,
                        principalTable: "EnhancedMenuItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomizationOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomizationGroupId = table.Column<int>(type: "INTEGER", nullable: false),
                    OptionName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    AdditionalPrice = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsAvailable = table.Column<bool>(type: "INTEGER", nullable: false),
                    AdditionalCalories = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomizationOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomizationOptions_CustomizationGroups_CustomizationGroupId",
                        column: x => x.CustomizationGroupId,
                        principalTable: "CustomizationGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "RestaurantConfigs",
                columns: new[] { "Id", "AllowSubcategories", "CreatedAt", "CuisineType", "CurrencyCode", "Description", "EnableAllergenInfo", "EnableBundlePricing", "EnableComboDeals", "EnableDynamicPricing", "EnableNutritionalInfo", "EnablePreparationTime", "EnableSeasonalItems", "EnableSpiceLevelCustomization", "IsActive", "LogoUrl", "MaxCustomizationOptionsPerItem", "OperatingHoursEnd", "OperatingHoursStart", "RequireItemVariants", "RestaurantId", "RestaurantName", "RestaurantType", "UpdatedAt" },
                values: new object[] { 1, true, new DateTime(2026, 3, 22, 21, 1, 15, 904, DateTimeKind.Utc).AddTicks(7570), "Italian", "USD", "Premium Italian Pizza Restaurant", true, true, true, false, true, true, true, true, true, null, 5, new TimeSpan(0, 23, 0, 0, 0), new TimeSpan(0, 11, 0, 0, 0), false, "pizza_place_001", "Pizza Palace", "Pizza", new DateTime(2026, 3, 22, 21, 1, 15, 904, DateTimeKind.Utc).AddTicks(7570) });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Description", "DisplayOrder", "ImageUrl", "IsActive", "Name", "RestaurantConfigId", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 3, 22, 21, 1, 15, 905, DateTimeKind.Utc).AddTicks(1730), "All types of pizzas", 1, null, true, "Pizzas", 1, new DateTime(2026, 3, 22, 21, 1, 15, 905, DateTimeKind.Utc).AddTicks(1740) },
                    { 2, new DateTime(2026, 3, 22, 21, 1, 15, 905, DateTimeKind.Utc).AddTicks(2320), "Starters and appetizers", 2, null, true, "Appetizers", 1, new DateTime(2026, 3, 22, 21, 1, 15, 905, DateTimeKind.Utc).AddTicks(2320) },
                    { 3, new DateTime(2026, 3, 22, 21, 1, 15, 905, DateTimeKind.Utc).AddTicks(2320), "Sweet treats", 3, null, true, "Desserts", 1, new DateTime(2026, 3, 22, 21, 1, 15, 905, DateTimeKind.Utc).AddTicks(2320) },
                    { 4, new DateTime(2026, 3, 22, 21, 1, 15, 905, DateTimeKind.Utc).AddTicks(2320), "Drinks and beverages", 4, null, true, "Beverages", 1, new DateTime(2026, 3, 22, 21, 1, 15, 905, DateTimeKind.Utc).AddTicks(2320) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BundlePrices_MenuItemId",
                table: "BundlePrices",
                column: "MenuItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_RestaurantConfigId",
                table: "Categories",
                column: "RestaurantConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_ComboDealItems_ComboDealId",
                table: "ComboDealItems",
                column: "ComboDealId");

            migrationBuilder.CreateIndex(
                name: "IX_ComboDealItems_MenuItemId",
                table: "ComboDealItems",
                column: "MenuItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ComboDeals_RestaurantConfigId",
                table: "ComboDeals",
                column: "RestaurantConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomizationGroups_MenuItemId",
                table: "CustomizationGroups",
                column: "MenuItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomizationOptions_CustomizationGroupId",
                table: "CustomizationOptions",
                column: "CustomizationGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_EnhancedMenuItems_CategoryId",
                table: "EnhancedMenuItems",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_EnhancedMenuItems_RestaurantConfigId",
                table: "EnhancedMenuItems",
                column: "RestaurantConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_EnhancedMenuItems_SubcategoryId",
                table: "EnhancedMenuItems",
                column: "SubcategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemVariants_MenuItemId",
                table: "ItemVariants",
                column: "MenuItemId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_CategoryId",
                table: "MenuItems",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantConfigs_RestaurantId",
                table: "RestaurantConfigs",
                column: "RestaurantId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subcategories_CategoryId",
                table: "Subcategories",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BundlePrices");

            migrationBuilder.DropTable(
                name: "ComboDealItems");

            migrationBuilder.DropTable(
                name: "CustomizationOptions");

            migrationBuilder.DropTable(
                name: "ItemVariants");

            migrationBuilder.DropTable(
                name: "MenuItems");

            migrationBuilder.DropTable(
                name: "ComboDeals");

            migrationBuilder.DropTable(
                name: "CustomizationGroups");

            migrationBuilder.DropTable(
                name: "EnhancedMenuItems");

            migrationBuilder.DropTable(
                name: "Subcategories");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "RestaurantConfigs");
        }
    }
}
