using MenuService.API.Models;
using MenuService.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace MenuService.API.Controllers
{
    [ApiController]
    [Route("api/v1/menu-items/{menuItemId}/customizations")]
    public class CustomizationsController : ControllerBase
    {
        private readonly ICustomizationService _customizationService;
        private readonly IMenuValidationService _validationService;
        private readonly ILogger<CustomizationsController> _logger;

        public CustomizationsController(
            ICustomizationService customizationService,
            IMenuValidationService validationService,
            ILogger<CustomizationsController> logger)
        {
            _customizationService = customizationService;
            _validationService = validationService;
            _logger = logger;
        }

        /// <summary>
        /// Get all customization groups for a menu item
        /// </summary>
        [HttpGet]
        [Produces(typeof(List<CustomizationGroupResponse>))]
        public async Task<IActionResult> GetCustomizations(int menuItemId)
        {
            try
            {
                var groups = await _customizationService.GetItemCustomizationsAsync(menuItemId);
                return Ok(groups.Select(MapToResponse).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving customizations: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Create a new customization group
        /// </summary>
        [HttpPost]
        [Produces(typeof(CustomizationGroupResponse))]
        public async Task<IActionResult> CreateCustomizationGroup(int menuItemId, CustomizationGroupRequest request)
        {
            try
            {
                _logger.LogInformation($"Creating customization group for menu item {menuItemId}");
                await _validationService.ValidateCustomizationGroupAsync(request);
                
                var group = await _customizationService.CreateCustomizationGroupAsync(menuItemId, request);
                return CreatedAtAction(nameof(GetCustomizationGroupById), 
                    new { menuItemId, id = group.Id }, MapToResponse(group));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError($"Error creating customization group: {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Get customization group by ID
        /// </summary>
        [HttpGet("{id}")]
        [Produces(typeof(CustomizationGroupResponse))]
        public async Task<IActionResult> GetCustomizationGroupById(int menuItemId, int id)
        {
            try
            {
                var groups = await _customizationService.GetItemCustomizationsAsync(menuItemId);
                var group = groups.FirstOrDefault(g => g.Id == id);
                
                if (group == null)
                    return NotFound(new { error = "Customization group not found" });

                return Ok(MapToResponse(group));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving customization group: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Update customization group
        /// </summary>
        [HttpPut("{id}")]
        [Produces(typeof(CustomizationGroupResponse))]
        public async Task<IActionResult> UpdateCustomizationGroup(int menuItemId, int id, CustomizationGroupRequest request)
        {
            try
            {
                await _validationService.ValidateCustomizationGroupAsync(request);
                
                var group = await _customizationService.UpdateCustomizationGroupAsync(id, request);
                if (group == null)
                    return NotFound(new { error = "Customization group not found" });

                return Ok(MapToResponse(group));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError($"Error updating customization group: {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Delete customization group
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomizationGroup(int menuItemId, int id)
        {
            try
            {
                await _customizationService.DeleteCustomizationGroupAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting customization group: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Calculate final price with selected customizations
        /// </summary>
        [HttpPost("calculate-price")]
        [Produces(typeof(object))]
        public async Task<IActionResult> CalculatePrice(int menuItemId, 
            [FromBody] PriceCalculationRequest request)
        {
            try
            {
                var finalPrice = await _customizationService.CalculateFinalPriceAsync(
                    menuItemId, 
                    request.SelectedVariantIds, 
                    request.SelectedOptionIds);
                
                return Ok(new { finalPrice });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error calculating price: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        private CustomizationGroupResponse MapToResponse(CustomizationGroup group)
        {
            return new CustomizationGroupResponse
            {
                Id = group.Id,
                GroupName = group.GroupName,
                SelectionType = group.SelectionType,
                MinimumSelections = group.MinimumSelections,
                MaximumSelections = group.MaximumSelections,
                IsRequired = group.IsRequired,
                DisplayOrder = group.DisplayOrder,
                Options = group.Options?.Select(o => new CustomizationOptionResponse
                {
                    Id = o.Id,
                    OptionName = o.OptionName,
                    AdditionalPrice = o.AdditionalPrice,
                    AdditionalCalories = o.AdditionalCalories,
                    DisplayOrder = o.DisplayOrder
                }).ToList() ?? new List<CustomizationOptionResponse>()
            };
        }
    }

    [ApiController]
    [Route("api/v1/categories/{categoryId}/subcategories")]
    public class SubcategoriesController : ControllerBase
    {
        private readonly ISubcategoryService _subcategoryService;
        private readonly ILogger<SubcategoriesController> _logger;

        public SubcategoriesController(
            ISubcategoryService subcategoryService,
            ILogger<SubcategoriesController> logger)
        {
            _subcategoryService = subcategoryService;
            _logger = logger;
        }

        /// <summary>
        /// Get all subcategories for a category
        /// </summary>
        [HttpGet]
        [Produces(typeof(List<SubcategoryResponse>))]
        public async Task<IActionResult> GetSubcategories(int categoryId)
        {
            try
            {
                var subcategories = await _subcategoryService.GetSubcategoriesByCategoryAsync(categoryId);
                return Ok(subcategories);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving subcategories: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Create a new subcategory
        /// </summary>
        [HttpPost]
        [Produces(typeof(SubcategoryResponse))]
        public async Task<IActionResult> CreateSubcategory(int categoryId, SubcategoryRequest request)
        {
            try
            {
                _logger.LogInformation($"Creating subcategory for category {categoryId}");
                var response = await _subcategoryService.CreateSubcategoryAsync(categoryId, request);
                return CreatedAtAction(nameof(GetSubcategoryById), new { categoryId, id = response.Id }, response);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError($"Error creating subcategory: {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Get subcategory by ID
        /// </summary>
        [HttpGet("{id}")]
        [Produces(typeof(SubcategoryResponse))]
        public async Task<IActionResult> GetSubcategoryById(int categoryId, int id)
        {
            try
            {
                var response = await _subcategoryService.GetSubcategoryByIdAsync(id);
                if (response == null)
                    return NotFound(new { error = "Subcategory not found" });

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving subcategory: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Update subcategory
        /// </summary>
        [HttpPut("{id}")]
        [Produces(typeof(SubcategoryResponse))]
        public async Task<IActionResult> UpdateSubcategory(int categoryId, int id, SubcategoryRequest request)
        {
            try
            {
                var response = await _subcategoryService.UpdateSubcategoryAsync(id, request);
                if (response == null)
                    return NotFound(new { error = "Subcategory not found" });

                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError($"Error updating subcategory: {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Delete subcategory
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubcategory(int categoryId, int id)
        {
            try
            {
                await _subcategoryService.DeleteSubcategoryAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting subcategory: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }
    }

    [ApiController]
    [Route("api/v1/restaurants/{restaurantId}/combo-deals")]
    public class ComboDealsController : ControllerBase
    {
        private readonly IComboDealService _comboDealService;
        private readonly IMenuValidationService _validationService;
        private readonly ILogger<ComboDealsController> _logger;

        public ComboDealsController(
            IComboDealService comboDealService,
            IMenuValidationService validationService,
            ILogger<ComboDealsController> logger)
        {
            _comboDealService = comboDealService;
            _validationService = validationService;
            _logger = logger;
        }

        /// <summary>
        /// Get all available combo deals
        /// </summary>
        [HttpGet]
        [Produces(typeof(List<ComboDealResponse>))]
        public async Task<IActionResult> GetAvailableDeals(int restaurantId)
        {
            try
            {
                var deals = await _comboDealService.GetAvailableDealsAsync(restaurantId);
                return Ok(deals);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving combo deals: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Get combo deal by ID
        /// </summary>
        [HttpGet("{id}")]
        [Produces(typeof(ComboDealResponse))]
        public async Task<IActionResult> GetDealById(int restaurantId, int id)
        {
            try
            {
                var deal = await _comboDealService.GetDealByIdAsync(id);
                if (deal == null)
                    return NotFound(new { error = "Combo deal not found" });

                return Ok(deal);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving combo deal: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Create a new combo deal
        /// </summary>
        [HttpPost]
        [Produces(typeof(ComboDealResponse))]
        public async Task<IActionResult> CreateDeal(int restaurantId, ComboDealRequest request)
        {
            try
            {
                _logger.LogInformation($"Creating combo deal for restaurant {restaurantId}");
                await _validationService.ValidateComboDealAsync(request);
                
                var response = await _comboDealService.CreateDealAsync(restaurantId, request);
                return CreatedAtAction(nameof(GetDealById), new { restaurantId, id = response.Id }, response);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError($"Error creating combo deal: {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Update combo deal
        /// </summary>
        [HttpPut("{id}")]
        [Produces(typeof(ComboDealResponse))]
        public async Task<IActionResult> UpdateDeal(int restaurantId, int id, ComboDealRequest request)
        {
            try
            {
                await _validationService.ValidateComboDealAsync(request);
                
                var response = await _comboDealService.UpdateDealAsync(id, request);
                if (response == null)
                    return NotFound(new { error = "Combo deal not found" });

                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError($"Error updating combo deal: {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Delete combo deal
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDeal(int restaurantId, int id)
        {
            try
            {
                await _comboDealService.DeleteDealAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting combo deal: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Validate a combo deal can be purchased
        /// </summary>
        [HttpPost("{id}/validate")]
        [Produces(typeof(object))]
        public async Task<IActionResult> ValidateDeal(int restaurantId, int id)
        {
            try
            {
                var isValid = await _comboDealService.ValidateDealAsync(id);
                return Ok(new { isValid });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error validating combo deal: {ex.Message}");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }
    }

    // Helper class for price calculation request
    public class PriceCalculationRequest
    {
        public List<int> SelectedVariantIds { get; set; } = new();
        public List<int> SelectedOptionIds { get; set; } = new();
    }
}
