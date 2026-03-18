using MenuService.API.Models;
using MenuService.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace MenuService.API.Controllers
{
    [ApiController]
    [Route("api/v1/restaurants")]
    public class RestaurantConfigController : ControllerBase
    {
        private readonly IRestaurantConfigService _restaurantConfigService;
        private readonly ILogger<RestaurantConfigController> _logger;

        public RestaurantConfigController(
            IRestaurantConfigService restaurantConfigService,
            ILogger<RestaurantConfigController> logger)
        {
            _restaurantConfigService = restaurantConfigService;
            _logger = logger;
        }

        /// <summary>
        /// Create a new restaurant configuration
        /// </summary>
        [HttpPost]
        [Produces(typeof(RestaurantConfigResponse))]
        public async Task<IActionResult> CreateRestaurant(RestaurantConfigRequest request)
        {
            try
            {
                _logger.LogInformation($"Creating restaurant: {request.RestaurantName}");
                var response = await _restaurantConfigService.CreateConfigAsync(request);
                return CreatedAtAction(nameof(GetRestaurantById), new { id = response.Id }, response);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError($"Error creating restaurant: {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Get restaurant configuration by ID
        /// </summary>
        [HttpGet("{id}")]
        [Produces(typeof(RestaurantConfigResponse))]
        public async Task<IActionResult> GetRestaurantById(int id)
        {
            try
            {
                _logger.LogInformation($"Getting restaurant config: {id}");
                var response = await _restaurantConfigService.GetConfigByIdAsync(id);
                if (response == null)
                    return NotFound(new { error = "Restaurant not found" });

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving restaurant: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Get all restaurant configurations
        /// </summary>
        [HttpGet]
        [Produces(typeof(List<RestaurantConfigResponse>))]
        public async Task<IActionResult> GetAllRestaurants()
        {
            try
            {
                var restaurants = await _restaurantConfigService.GetAllActiveConfigsAsync();
                return Ok(restaurants);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving restaurants: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Update restaurant configuration
        /// </summary>
        [HttpPut("{id}")]
        [Produces(typeof(RestaurantConfigResponse))]
        public async Task<IActionResult> UpdateRestaurant(int id, RestaurantConfigRequest request)
        {
            try
            {
                _logger.LogInformation($"Updating restaurant config: {id}");
                var response = await _restaurantConfigService.UpdateConfigAsync(id, request);
                if (response == null)
                    return NotFound(new { error = "Restaurant not found" });

                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError($"Error updating restaurant: {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Delete restaurant configuration
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRestaurant(int id)
        {
            try
            {
                _logger.LogInformation($"Deleting restaurant config: {id}");
                var success = await _restaurantConfigService.DeleteConfigAsync(id);
                if (!success)
                    return NotFound(new { error = "Restaurant not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting restaurant: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Get restaurants by type (Pizza, Biryani, Breakfast, etc.)
        /// </summary>
        [HttpGet("type/{restaurantType}")]
        [Produces(typeof(List<RestaurantConfigResponse>))]
        public async Task<IActionResult> GetRestaurantsByType(string restaurantType)
        {
            try
            {
                var restaurants = await _restaurantConfigService.GetRestaurantsByTypeAsync(restaurantType);
                return Ok(restaurants);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving restaurants by type: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Get restaurants by cuisine type (Italian, Indian, Continental, etc.)
        /// </summary>
        [HttpGet("cuisine/{cuisineType}")]
        [Produces(typeof(List<RestaurantConfigResponse>))]
        public async Task<IActionResult> GetRestaurantsByCuisine(string cuisineType)
        {
            try
            {
                var restaurants = await _restaurantConfigService.GetRestaurantsByCuisineAsync(cuisineType);
                return Ok(restaurants);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving restaurants by cuisine: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }
    }

    [ApiController]
    [Route("api/v1/restaurants/{restaurantId}/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(
            ICategoryService categoryService,
            ILogger<CategoriesController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        /// <summary>
        /// Create a new category for a restaurant
        /// </summary>
        [HttpPost]
        [Produces(typeof(CategoryResponse))]
        public async Task<IActionResult> CreateCategory(int restaurantId, CategoryRequest request)
        {
            try
            {
                _logger.LogInformation($"Creating category for restaurant {restaurantId}: {request.Name}");
                var response = await _categoryService.CreateCategoryAsync(restaurantId, request);
                return CreatedAtAction(nameof(GetCategoryById), new { restaurantId, id = response.Id }, response);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError($"Error creating category: {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Get category by ID
        /// </summary>
        [HttpGet("{id}")]
        [Produces(typeof(CategoryResponse))]
        public async Task<IActionResult> GetCategoryById(int restaurantId, int id)
        {
            try
            {
                var response = await _categoryService.GetCategoryByIdAsync(id);
                if (response == null)
                    return NotFound(new { error = "Category not found" });

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving category: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Get all categories for a restaurant
        /// </summary>
        [HttpGet]
        [Produces(typeof(List<CategoryResponse>))]
        public async Task<IActionResult> GetCategoriesByRestaurant(int restaurantId)
        {
            try
            {
                var categories = await _categoryService.GetCategoriesByRestaurantAsync(restaurantId);
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving categories: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Update category
        /// </summary>
        [HttpPut("{id}")]
        [Produces(typeof(CategoryResponse))]
        public async Task<IActionResult> UpdateCategory(int restaurantId, int id, CategoryRequest request)
        {
            try
            {
                var response = await _categoryService.UpdateCategoryAsync(id, request);
                if (response == null)
                    return NotFound(new { error = "Category not found" });

                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError($"Error updating category: {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Delete category
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int restaurantId, int id)
        {
            try
            {
                var success = await _categoryService.DeleteCategoryAsync(id);
                if (!success)
                    return NotFound(new { error = "Category not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting category: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }
    }
}
