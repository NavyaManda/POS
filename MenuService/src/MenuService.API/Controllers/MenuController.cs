using Microsoft.AspNetCore.Mvc;
using MenuService.API.Models;
using MenuService.API.Services;

namespace MenuService.API.Controllers;

[ApiController]
[Route("api/v1/menu")]
public class MenuController : ControllerBase
{
    private readonly IMenuService _menuService;
    private readonly ILogger<MenuController> _logger;

    public MenuController(IMenuService menuService, ILogger<MenuController> logger)
    {
        _menuService = menuService;
        _logger = logger;
    }

    // Menu Items
    [HttpGet("items")]
    public async Task<ActionResult<ApiResponse<List<MenuItemResponse>>>> GetAllMenuItems()
    {
        try
        {
            var items = await _menuService.GetAllMenuItemsAsync();
            return Ok(new ApiResponse<List<MenuItemResponse>>
            {
                Success = true,
                Message = "Menu items retrieved successfully",
                Data = items
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving menu items");
            return StatusCode(500, new ApiResponse<List<MenuItemResponse>>
            {
                Success = false,
                Message = "Error retrieving menu items",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpGet("items/{id}")]
    public async Task<ActionResult<ApiResponse<MenuItemResponse>>> GetMenuItemById(int id)
    {
        try
        {
            var item = await _menuService.GetMenuItemByIdAsync(id);
            if (item == null)
                return NotFound(new ApiResponse<MenuItemResponse>
                {
                    Success = false,
                    Message = "Menu item not found"
                });

            return Ok(new ApiResponse<MenuItemResponse>
            {
                Success = true,
                Message = "Menu item retrieved successfully",
                Data = item
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving menu item");
            return StatusCode(500, new ApiResponse<MenuItemResponse>
            {
                Success = false,
                Message = "Error retrieving menu item",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<ApiResponse<List<MenuItemResponse>>>> GetMenuItemsByCategory(int categoryId)
    {
        try
        {
            var items = await _menuService.GetMenuItemsByCategoryAsync(categoryId);
            return Ok(new ApiResponse<List<MenuItemResponse>>
            {
                Success = true,
                Message = "Menu items retrieved successfully",
                Data = items
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving menu items by category");
            return StatusCode(500, new ApiResponse<List<MenuItemResponse>>
            {
                Success = false,
                Message = "Error retrieving menu items",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPost("items")]
    public async Task<ActionResult<ApiResponse<MenuItemResponse>>> CreateMenuItem([FromBody] MenuItemRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<MenuItemResponse>
                {
                    Success = false,
                    Message = "Invalid request",
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                });

            var item = await _menuService.CreateMenuItemAsync(request);
            return CreatedAtAction(nameof(GetMenuItemById), new { id = item.Id }, 
                new ApiResponse<MenuItemResponse>
                {
                    Success = true,
                    Message = "Menu item created successfully",
                    Data = item
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating menu item");
            return StatusCode(500, new ApiResponse<MenuItemResponse>
            {
                Success = false,
                Message = "Error creating menu item",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPut("items/{id}")]
    public async Task<ActionResult<ApiResponse<MenuItemResponse>>> UpdateMenuItem(int id, [FromBody] MenuItemRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<MenuItemResponse>
                {
                    Success = false,
                    Message = "Invalid request",
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                });

            var item = await _menuService.UpdateMenuItemAsync(id, request);
            if (item == null)
                return NotFound(new ApiResponse<MenuItemResponse>
                {
                    Success = false,
                    Message = "Menu item not found"
                });

            return Ok(new ApiResponse<MenuItemResponse>
            {
                Success = true,
                Message = "Menu item updated successfully",
                Data = item
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating menu item");
            return StatusCode(500, new ApiResponse<MenuItemResponse>
            {
                Success = false,
                Message = "Error updating menu item",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpDelete("items/{id}")]
    public async Task<ActionResult<ApiResponse<string>>> DeleteMenuItem(int id)
    {
        try
        {
            var result = await _menuService.DeleteMenuItemAsync(id);
            if (!result)
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Menu item not found"
                });

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Menu item deleted successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting menu item");
            return StatusCode(500, new ApiResponse<string>
            {
                Success = false,
                Message = "Error deleting menu item",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    // Categories
    [HttpGet("categories")]
    public async Task<ActionResult<ApiResponse<List<CategoryResponse>>>> GetAllCategories()
    {
        try
        {
            var categories = await _menuService.GetAllCategoriesAsync();
            return Ok(new ApiResponse<List<CategoryResponse>>
            {
                Success = true,
                Message = "Categories retrieved successfully",
                Data = categories
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving categories");
            return StatusCode(500, new ApiResponse<List<CategoryResponse>>
            {
                Success = false,
                Message = "Error retrieving categories",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpGet("categories/{id}")]
    public async Task<ActionResult<ApiResponse<CategoryResponse>>> GetCategoryById(int id)
    {
        try
        {
            var category = await _menuService.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound(new ApiResponse<CategoryResponse>
                {
                    Success = false,
                    Message = "Category not found"
                });

            return Ok(new ApiResponse<CategoryResponse>
            {
                Success = true,
                Message = "Category retrieved successfully",
                Data = category
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving category");
            return StatusCode(500, new ApiResponse<CategoryResponse>
            {
                Success = false,
                Message = "Error retrieving category",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPost("categories")]
    public async Task<ActionResult<ApiResponse<CategoryResponse>>> CreateCategory([FromBody] CategoryRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<CategoryResponse>
                {
                    Success = false,
                    Message = "Invalid request",
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                });

            var category = await _menuService.CreateCategoryAsync(request);
            return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, 
                new ApiResponse<CategoryResponse>
                {
                    Success = true,
                    Message = "Category created successfully",
                    Data = category
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");
            return StatusCode(500, new ApiResponse<CategoryResponse>
            {
                Success = false,
                Message = "Error creating category",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPut("categories/{id}")]
    public async Task<ActionResult<ApiResponse<CategoryResponse>>> UpdateCategory(int id, [FromBody] CategoryRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<CategoryResponse>
                {
                    Success = false,
                    Message = "Invalid request",
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                });

            var category = await _menuService.UpdateCategoryAsync(id, request);
            if (category == null)
                return NotFound(new ApiResponse<CategoryResponse>
                {
                    Success = false,
                    Message = "Category not found"
                });

            return Ok(new ApiResponse<CategoryResponse>
            {
                Success = true,
                Message = "Category updated successfully",
                Data = category
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category");
            return StatusCode(500, new ApiResponse<CategoryResponse>
            {
                Success = false,
                Message = "Error updating category",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpDelete("categories/{id}")]
    public async Task<ActionResult<ApiResponse<string>>> DeleteCategory(int id)
    {
        try
        {
            var result = await _menuService.DeleteCategoryAsync(id);
            if (!result)
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Category not found"
                });

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Category deleted successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category");
            return StatusCode(500, new ApiResponse<string>
            {
                Success = false,
                Message = "Error deleting category",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new
        {
            status = "healthy",
            timestamp = DateTime.UtcNow,
            service = "MenuService"
        });
    }
}
