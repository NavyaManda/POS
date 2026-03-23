using Xunit;
using Moq;
using MenuService.API.Models;
using MenuService.API.Repositories;
using MenuService.API.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MenuService.Tests.Services;

public class MenuItemServiceTests
{
    private readonly Mock<IEnhancedMenuRepository> _mockRepository;
    private readonly Mock<IMenuValidationService> _mockValidationService;
    private readonly MenuItemService _service;

    public MenuItemServiceTests()
    {
        _mockRepository = new Mock<IEnhancedMenuRepository>();
        _mockValidationService = new Mock<IMenuValidationService>();
        _service = new MenuItemService(_mockRepository.Object, _mockValidationService.Object);
    }

    [Fact]
    public async Task CreateMenuItemAsync_ValidItem_ReturnsItem()
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
            Description = request.Description,
            BasePrice = request.BasePrice,
            RestaurantConfigId = request.RestaurantConfigId
        };

        _mockValidationService.Setup(v => v.ValidateMenuItemAsync(It.IsAny<MenuItemRequest>()))
            .ReturnsAsync(true);
        _mockRepository.Setup(r => r.CreateMenuItemAsync(It.IsAny<EnhancedMenuItem>()))
            .ReturnsAsync(expectedItem);

        // Act
        var result = await _service.CreateMenuItemAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Margherita Pizza", result.Name);
        Assert.Equal(299, result.BasePrice);
        _mockRepository.Verify(r => r.CreateMenuItemAsync(It.IsAny<EnhancedMenuItem>()), Times.Once);
    }

    [Fact]
    public async Task GetMenuItemAsync_ExistingItem_ReturnsItem()
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

        _mockRepository.Setup(r => r.GetMenuItemAsync(itemId))
            .ReturnsAsync(expectedItem);

        // Act
        var result = await _service.GetMenuItemAsync(itemId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(itemId, result.Id);
        Assert.Equal("Margherita Pizza", result.Name);
    }

    [Fact]
    public async Task GetMenuItemAsync_NonExistentItem_ReturnsNull()
    {
        // Arrange
        var itemId = 999;
        _mockRepository.Setup(r => r.GetMenuItemAsync(itemId))
            .ReturnsAsync((EnhancedMenuItem)null);

        // Act
        var result = await _service.GetMenuItemAsync(itemId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateMenuItemAsync_ValidItem_ReturnsUpdatedItem()
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
            BasePrice = request.BasePrice,
            RestaurantConfigId = request.RestaurantConfigId
        };

        _mockValidationService.Setup(v => v.ValidateMenuItemAsync(request))
            .ReturnsAsync(true);
        _mockRepository.Setup(r => r.UpdateMenuItemAsync(itemId, It.IsAny<EnhancedMenuItem>()))
            .ReturnsAsync(updatedItem);

        // Act
        var result = await _service.UpdateMenuItemAsync(itemId, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Margherita Pizza Updated", result.Name);
        Assert.Equal(349, result.BasePrice);
    }

    [Fact]
    public async Task DeleteMenuItemAsync_ValidId_ReturnsTrue()
    {
        // Arrange
        var itemId = 1;
        _mockRepository.Setup(r => r.DeleteMenuItemAsync(itemId))
            .ReturnsAsync(true);

        // Act
        var result = await _service.DeleteMenuItemAsync(itemId);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.DeleteMenuItemAsync(itemId), Times.Once);
    }

    [Fact]
    public async Task SearchMenuItemsAsync_WithFilters_ReturnsFilteredItems()
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
            new EnhancedMenuItem { Id = 1, Name = "Margherita Pizza", BasePrice = 299 },
            new EnhancedMenuItem { Id = 2, Name = "Pepperoni Pizza", BasePrice = 349 }
        };

        _mockRepository.Setup(r => r.SearchMenuItemsAsync(
            searchParams.SearchTerm,
            searchParams.RestaurantConfigId,
            searchParams.CategoryId,
            searchParams.SubcategoryId,
            searchParams.IsVegetarian,
            searchParams.MinPrice,
            searchParams.MaxPrice,
            searchParams.PageNumber,
            searchParams.PageSize))
            .ReturnsAsync(expectedItems);

        // Act
        var result = await _service.SearchMenuItemsAsync(searchParams);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, item => Assert.Contains("Pizza", item.Name));
    }

    [Fact]
    public async Task CalculateItemPriceAsync_WithVariantsAndCustomizations_ReturnsCorrectPrice()
    {
        // Arrange
        var itemId = 1;
        var variantIds = new List<int> { 1, 2 };
        var customizationIds = new List<int> { 5, 6 };

        var menuItem = new EnhancedMenuItem
        {
            Id = itemId,
            BasePrice = 299,
            RestaurantConfigId = 1
        };

        var variants = new List<ItemVariant>
        {
            new ItemVariant { Id = 1, PriceModifier = 50 },
            new ItemVariant { Id = 2, PriceModifier = 30 }
        };

        var customizations = new List<CustomizationOption>
        {
            new CustomizationOption { Id = 5, Price = 20 },
            new CustomizationOption { Id = 6, Price = 15 }
        };

        _mockRepository.Setup(r => r.GetMenuItemAsync(itemId))
            .ReturnsAsync(menuItem);
        _mockRepository.Setup(r => r.GetItemVariantsAsync(itemId))
            .ReturnsAsync(variants);
        _mockRepository.Setup(r => r.GetCustomizationOptionsAsync(It.IsAny<List<int>>()))
            .ReturnsAsync(customizations);

        // Act
        var result = await _service.CalculateItemPriceAsync(itemId, variantIds, customizationIds);

        // Assert
        // Base price (299) + variants (50 + 30) + customizations (20 + 15) = 414
        Assert.Equal(414m, result);
    }
}
