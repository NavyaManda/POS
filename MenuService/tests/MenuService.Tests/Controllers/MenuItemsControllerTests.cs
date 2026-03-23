using Xunit;
using Moq;
using MenuService.API.Controllers;
using MenuService.API.Models;
using MenuService.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MenuService.Tests.Controllers;

public class MenuItemsControllerTests
{
    private readonly Mock<IMenuItemService> _mockMenuItemService;
    private readonly Mock<IMenuValidationService> _mockValidationService;
    private readonly Mock<ILogger<MenuItemsController>> _mockLogger;
    private readonly MenuItemsController _controller;

    public MenuItemsControllerTests()
    {
        _mockMenuItemService = new Mock<IMenuItemService>();
        _mockValidationService = new Mock<IMenuValidationService>();
        _mockLogger = new Mock<ILogger<MenuItemsController>>();
        _controller = new MenuItemsController(
            _mockMenuItemService.Object,
            _mockValidationService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task CreateMenuItem_ValidRequest_ReturnsCreatedAtAction()
    {
        // Arrange
        var request = new MenuItemRequest
        {
            Name = "Margherita Pizza",
            Description = "Classic pizza",
            BasePrice = 299,
            RestaurantConfigId = 1
        };

        var expectedItem = new EnhancedMenuItem
        {
            Id = 1,
            Name = request.Name,
            BasePrice = request.BasePrice,
            RestaurantConfigId = request.RestaurantConfigId
        };

        _mockValidationService.Setup(v => v.ValidateMenuItemAsync(request))
            .ReturnsAsync(true);
        _mockMenuItemService.Setup(s => s.CreateMenuItemAsync(request))
            .ReturnsAsync(expectedItem);

        // Act
        var result = await _controller.CreateMenuItem(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(_controller.GetMenuItem), createdResult.ActionName);
    }

    [Fact]
    public async Task GetMenuItem_ExistingId_ReturnsOkWithData()
    {
        // Arrange
        var itemId = 1;
        var expectedItem = new EnhancedMenuItem
        {
            Id = itemId,
            Name = "Margherita Pizza",
            BasePrice = 299,
            RestaurantConfigId = 1
        };

        _mockMenuItemService.Setup(s => s.GetMenuItemAsync(itemId))
            .ReturnsAsync(expectedItem);

        // Act
        var result = await _controller.GetMenuItem(itemId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task GetMenuItem_NonExistentId_ReturnsNotFound()
    {
        // Arrange
        var itemId = 999;
        _mockMenuItemService.Setup(s => s.GetMenuItemAsync(itemId))
            .ReturnsAsync((EnhancedMenuItem)null);

        // Act
        var result = await _controller.GetMenuItem(itemId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetMenuItemsByRestaurant_ValidRestaurantId_ReturnsOkWithList()
    {
        // Arrange
        var restaurantId = 1;
        var items = new List<EnhancedMenuItem>
        {
            new EnhancedMenuItem { Id = 1, Name = "Margherita Pizza", RestaurantConfigId = restaurantId },
            new EnhancedMenuItem { Id = 2, Name = "Pepperoni Pizza", RestaurantConfigId = restaurantId }
        };

        _mockMenuItemService.Setup(s => s.GetMenuItemsByRestaurantAsync(restaurantId))
            .ReturnsAsync(items);

        // Act
        var result = await _controller.GetMenuItemsByRestaurant(restaurantId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedList = Assert.IsType<List<MenuItemResponse>>(okResult.Value);
        Assert.Equal(2, returnedList.Count);
    }

    [Fact]
    public async Task UpdateMenuItem_ValidRequest_ReturnsOkWithData()
    {
        // Arrange
        var itemId = 1;
        var request = new MenuItemRequest
        {
            Name = "Margherita Pizza Updated",
            BasePrice = 349,
            RestaurantConfigId = 1
        };

        var updatedItem = new EnhancedMenuItem
        {
            Id = itemId,
            Name = request.Name,
            BasePrice = request.BasePrice
        };

        _mockValidationService.Setup(v => v.ValidateMenuItemAsync(request))
            .ReturnsAsync(true);
        _mockMenuItemService.Setup(s => s.UpdateMenuItemAsync(itemId, request))
            .ReturnsAsync(updatedItem);

        // Act
        var result = await _controller.UpdateMenuItem(itemId, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task DeleteMenuItem_ValidId_ReturnsNoContent()
    {
        // Arrange
        var itemId = 1;
        _mockMenuItemService.Setup(s => s.DeleteMenuItemAsync(itemId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteMenuItem(itemId);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockMenuItemService.Verify(s => s.DeleteMenuItemAsync(itemId), Times.Once);
    }

    [Fact]
    public async Task SearchMenuItems_WithValidFilters_ReturnsOkWithFilteredList()
    {
        // Arrange
        var searchParams = new MenuItemSearchParams
        {
            SearchTerm = "Pizza",
            RestaurantConfigId = 1,
            PageNumber = 1,
            PageSize = 10
        };

        var expectedItems = new List<EnhancedMenuItem>
        {
            new EnhancedMenuItem { Id = 1, Name = "Margherita Pizza" },
            new EnhancedMenuItem { Id = 2, Name = "Pepperoni Pizza" }
        };

        _mockMenuItemService.Setup(s => s.SearchMenuItemsAsync(searchParams))
            .ReturnsAsync(expectedItems);

        // Act
        var result = await _controller.SearchMenuItems(searchParams);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedList = Assert.IsType<List<MenuItemResponse>>(okResult.Value);
        Assert.Equal(2, returnedList.Count);
    }

    [Fact]
    public async Task CheckAvailabilityAsync_AvailableItem_ReturnsTrue()
    {
        // Arrange
        var itemId = 1;
        _mockMenuItemService.Setup(s => s.IsItemAvailableAsync(itemId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.CheckAvailabilityAsync(itemId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<AvailabilityResponse>(okResult.Value);
        Assert.True(response.IsAvailable);
    }
}
