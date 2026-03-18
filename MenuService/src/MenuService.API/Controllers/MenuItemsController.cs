using MenuService.API.Models;
using MenuService.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace MenuService.API.Controllers
{
    [ApiController]
    [Route("api/v1/restaurants/{restaurantId}/menu-items")]
    public class MenuItemsController : ControllerBase
    {
        private readonly IMenuItemService _menuItemService;
        private readonly IMenuValidationService _validationService;
        private readonly ILogger<MenuItemsController> _logger;

        public MenuItemsController(
            IMenuItemService menuItemService,
            IMenuValidationService validationService,
            ILogger<MenuItemsController> logger)
        {
            _menuItemService = menuItemService;
            _validationService = validationService;
            _logger = logger;
        }

        /// <summary>
        /// Create a new menu item
        /// </summary>
        [HttpPost]
        [Produces(typeof(MenuItemResponse))]
        public async Task<IActionResult> CreateMenuItem(int restaurantId, MenuItemRequest request)
        {
            try
            {
                _logger.LogInformation($"Creating menu item for restaurant {restaurantId}: {request.Name}");
                await _validationService.ValidateMenuItemAsync(request, restaurantId);
                
                var menuItem = await _menuItemService.CreateMenuItemAsync(restaurantId, request);
                return CreatedAtAction(nameof(GetMenuItemById), new { restaurantId, id = menuItem.Id }, 
                    MapToResponse(menuItem));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError($"Error creating menu item: {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Get menu item by ID
        /// </summary>
        [HttpGet("{id}")]
        [Produces(typeof(MenuItemResponse))]
        public async Task<IActionResult> GetMenuItemById(int restaurantId, int id)
        {
            try
            {
                var menuItem = await _menuItemService.GetMenuItemAsync(id);
                if (menuItem == null)
                    return NotFound(new { error = "Menu item not found" });

                return Ok(MapToResponse(menuItem));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving menu item: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Get all menu items for a restaurant
        /// </summary>
        [HttpGet]
        [Produces(typeof(List<MenuItemResponse>))]
        public async Task<IActionResult> GetRestaurantMenu(int restaurantId)
        {
            try
            {
                var menuItems = await _menuItemService.GetRestaurantMenuAsync(restaurantId);
                return Ok(menuItems.Select(MapToResponse).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving restaurant menu: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Get menu items by category
        /// </summary>
        [HttpGet("category/{categoryId}")]
        [Produces(typeof(List<MenuItemResponse>))]
        public async Task<IActionResult> GetMenuItemsByCategory(int restaurantId, int categoryId)
        {
            try
            {
                var menuItems = await _menuItemService.GetMenuItemsByCategoryAsync(categoryId);
                return Ok(menuItems.Select(MapToResponse).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving menu items by category: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Get menu items by subcategory
        /// </summary>
        [HttpGet("subcategory/{subcategoryId}")]
        [Produces(typeof(List<MenuItemResponse>))]
        public async Task<IActionResult> GetMenuItemsBySubcategory(int restaurantId, int subcategoryId)
        {
            try
            {
                var menuItems = await _menuItemService.GetMenuItemsBySubcategoryAsync(subcategoryId);
                return Ok(menuItems.Select(MapToResponse).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving menu items by subcategory: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Search menu items with advanced filters
        /// </summary>
        [HttpPost("search")]
        [Produces(typeof(MenuSearchResponse<MenuItemResponse>))]
        public async Task<IActionResult> SearchMenu(int restaurantId, MenuSearchRequest request)
        {
            try
            {
                var result = await _menuItemService.SearchMenuAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error searching menu: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Update menu item
        /// </summary>
        [HttpPut("{id}")]
        [Produces(typeof(MenuItemResponse))]
        public async Task<IActionResult> UpdateMenuItem(int restaurantId, int id, MenuItemRequest request)
        {
            try
            {
                _logger.LogInformation($"Updating menu item {id}");
                await _validationService.ValidateMenuItemAsync(request, restaurantId);
                
                var menuItem = await _menuItemService.UpdateMenuItemAsync(id, request);
                if (menuItem == null)
                    return NotFound(new { error = "Menu item not found" });

                return Ok(MapToResponse(menuItem));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError($"Error updating menu item: {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Delete menu item
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenuItem(int restaurantId, int id)
        {
            try
            {
                _logger.LogInformation($"Deleting menu item {id}");
                await _menuItemService.DeleteMenuItemAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting menu item: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Check menu item availability
        /// </summary>
        [HttpGet("{id}/availability")]
        [Produces(typeof(object))]
        public async Task<IActionResult> CheckAvailability(int restaurantId, int id)
        {
            try
            {
                var isAvailable = await _validationService.IsMenuItemAvailableAsync(id);
                return Ok(new { isAvailable });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error checking availability: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        private MenuItemResponse MapToResponse(EnhancedMenuItem menuItem)
        {
            return new MenuItemResponse
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Description = menuItem.Description,
                ShortDescription = menuItem.ShortDescription,
                BasePrice = menuItem.BasePrice,
                ImageUrl = menuItem.ImageUrl,
                Calories = menuItem.Calories,
                Protein = menuItem.Protein,
                Carbohydrates = menuItem.Carbohydrates,
                Fat = menuItem.Fat,
                IsVegetarian = menuItem.IsVegetarian,
                IsVegan = menuItem.IsVegan,
                IsGlutenFree = menuItem.IsGlutenFree,
                AllergenInfo = menuItem.AllergenInfo,
                IsAvailable = menuItem.IsAvailable,
                SupportSpiceLevel = menuItem.SupportSpiceLevel,
                DefaultSpiceLevel = menuItem.DefaultSpiceLevel,
                PreparationTimeMinutes = menuItem.PreparationTimeMinutes,
                IsPopularItem = menuItem.RecommendationScore.HasValue && menuItem.RecommendationScore >= 4,
                Tags = menuItem.Tags,
                CategoryId = menuItem.CategoryId ?? 0,
                SubcategoryId = menuItem.SubcategoryId
            };
        }
    }

    [ApiController]
    [Route("api/v1/menu-items/{menuItemId}/variants")]
    public class VariantsController : ControllerBase
    {
        private readonly IVariantService _variantService;
        private readonly ILogger<VariantsController> _logger;

        public VariantsController(
            IVariantService variantService,
            ILogger<VariantsController> logger)
        {
            _variantService = variantService;
            _logger = logger;
        }

        /// <summary>
        /// Get all variants for a menu item
        /// </summary>
        [HttpGet]
        [Produces(typeof(List<ItemVariantResponse>))]
        public async Task<IActionResult> GetVariants(int menuItemId)
        {
            try
            {
                var variants = await _variantService.GetItemVariantsAsync(menuItemId);
                return Ok(variants.Select(MapToResponse).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving variants: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Create a new variant
        /// </summary>
        [HttpPost]
        [Produces(typeof(ItemVariantResponse))]
        public async Task<IActionResult> CreateVariant(int menuItemId, ItemVariantRequest request)
        {
            try
            {
                _logger.LogInformation($"Creating variant for menu item {menuItemId}");
                var variant = await _variantService.CreateVariantAsync(menuItemId, request);
                return CreatedAtAction(nameof(GetVariantById), new { menuItemId, id = variant.Id }, 
                    MapToResponse(variant));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError($"Error creating variant: {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Get variant by ID
        /// </summary>
        [HttpGet("{id}")]
        [Produces(typeof(ItemVariantResponse))]
        public async Task<IActionResult> GetVariantById(int menuItemId, int id)
        {
            try
            {
                var variants = await _variantService.GetItemVariantsAsync(menuItemId);
                var variant = variants.FirstOrDefault(v => v.Id == id);
                
                if (variant == null)
                    return NotFound(new { error = "Variant not found" });

                return Ok(MapToResponse(variant));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving variant: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Update variant
        /// </summary>
        [HttpPut("{id}")]
        [Produces(typeof(ItemVariantResponse))]
        public async Task<IActionResult> UpdateVariant(int menuItemId, int id, ItemVariantRequest request)
        {
            try
            {
                var variant = await _variantService.UpdateVariantAsync(id, request);
                if (variant == null)
                    return NotFound(new { error = "Variant not found" });

                return Ok(MapToResponse(variant));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError($"Error updating variant: {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Delete variant
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVariant(int menuItemId, int id)
        {
            try
            {
                await _variantService.DeleteVariantAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting variant: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        private ItemVariantResponse MapToResponse(ItemVariant variant)
        {
            return new ItemVariantResponse
            {
                Id = variant.Id,
                VariantType = variant.VariantType,
                VariantName = variant.VariantName,
                Description = variant.Description,
                PriceModifier = variant.PriceModifier,
                IsDefault = variant.IsDefault,
                DisplayOrder = variant.DisplayOrder
            };
        }
    }
}
