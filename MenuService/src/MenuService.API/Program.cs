using Microsoft.EntityFrameworkCore;
using MenuService.API.Data;
using MenuService.API.Repositories;
using MenuService.API.Services;
using MenuService.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// Database
builder.Services.AddDbContext<MenuContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<IMenuItemRepository, MenuItemRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IEnhancedMenuRepository, EnhancedMenuRepository>();
builder.Services.AddScoped<IRestaurantConfigRepository, RestaurantConfigRepository>();
builder.Services.AddScoped<IComboDealRepository, ComboDealRepository>();
builder.Services.AddScoped<IBundlePriceRepository, BundlePriceRepository>();

// Services
builder.Services.AddScoped<IMenuService, MenuService.API.Services.MenuService>();
builder.Services.AddScoped<IMenuItemService, MenuItemService>();
builder.Services.AddScoped<ICustomizationService, CustomizationService>();
builder.Services.AddScoped<IVariantService, VariantService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ISubcategoryService, SubcategoryService>();
builder.Services.AddScoped<IRestaurantConfigService, RestaurantConfigService>();
builder.Services.AddScoped<IComboDealService, ComboDealService>();
builder.Services.AddScoped<IBundlePricingService, BundlePricingService>();
builder.Services.AddScoped<IMenuValidationService, MenuValidationService>();

// Logging
builder.Services.AddLogging();

var app = builder.Build();

// Migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MenuContext>();
    db.Database.EnsureCreated();
}

// Exception handling middleware (must be early in pipeline)
app.UseExceptionHandlingMiddleware();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();
