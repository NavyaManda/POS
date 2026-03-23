using Xunit;
using Moq;
using MenuService.API.Models;
using MenuService.API.Repositories;
using MenuService.API.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MenuService.Tests.Services;

public class CategoryServiceTests
{
    private readonly Mock<IEnhancedMenuRepository> _mockRepository;
    private readonly CategoryService _service;

    public CategoryServiceTests()
    {
        _mockRepository = new Mock<IEnhancedMenuRepository>();
        _service = new CategoryService(_mockRepository.Object);
    }

    [Fact]
    public async Task CreateCategoryAsync_ValidCategory_ReturnsCategory()
    {
        // Arrange
        var request = new CategoryRequest
        {
            Name = "Pizzas",
            Description = "All types of pizzas",
            RestaurantConfigId = 1,
            DisplayOrder = 1
        };

        var expectedCategory = new Category
        {
            Id = 1,
            Name = request.Name,
            Description = request.Description,
            RestaurantConfigId = request.RestaurantConfigId,
            DisplayOrder = request.DisplayOrder
        };

        _mockRepository.Setup(r => r.CreateCategoryAsync(It.IsAny<Category>()))
            .ReturnsAsync(expectedCategory);

        // Act
        var result = await _service.CreateCategoryAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Pizzas", result.Name);
        Assert.Equal(1, result.RestaurantConfigId);
    }

    [Fact]
    public async Task GetCategoryAsync_ExistingCategory_ReturnsCategory()
    {
        // Arrange
        var categoryId = 1;
        var expectedCategory = new Category
        {
            Id = categoryId,
            Name = "Pizzas",
            RestaurantConfigId = 1
        };

        _mockRepository.Setup(r => r.GetCategoryAsync(categoryId))
            .ReturnsAsync(expectedCategory);

        // Act
        var result = await _service.GetCategoryAsync(categoryId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(categoryId, result.Id);
        Assert.Equal("Pizzas", result.Name);
    }

    [Fact]
    public async Task GetAllCategoriesAsync_ReturnsAllCategories()
    {
        // Arrange
        var categories = new List<Category>
        {
            new Category { Id = 1, Name = "Pizzas", RestaurantConfigId = 1 },
            new Category { Id = 2, Name = "Pastas", RestaurantConfigId = 1 },
            new Category { Id = 3, Name = "Desserts", RestaurantConfigId = 1 }
        };

        _mockRepository.Setup(r => r.GetAllCategoriesAsync())
            .ReturnsAsync(categories);

        // Act
        var result = await _service.GetAllCategoriesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task UpdateCategoryAsync_ValidCategory_ReturnsUpdatedCategory()
    {
        // Arrange
        var categoryId = 1;
        var request = new CategoryRequest
        {
            Name = "Pizzas Updated",
            Description = "Updated description",
            RestaurantConfigId = 1
        };

        var updatedCategory = new Category
        {
            Id = categoryId,
            Name = request.Name,
            Description = request.Description,
            RestaurantConfigId = request.RestaurantConfigId
        };

        _mockRepository.Setup(r => r.UpdateCategoryAsync(categoryId, It.IsAny<Category>()))
            .ReturnsAsync(updatedCategory);

        // Act
        var result = await _service.UpdateCategoryAsync(categoryId, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Pizzas Updated", result.Name);
    }

    [Fact]
    public async Task DeleteCategoryAsync_ValidId_ReturnsTrue()
    {
        // Arrange
        var categoryId = 1;
        _mockRepository.Setup(r => r.DeleteCategoryAsync(categoryId))
            .ReturnsAsync(true);

        // Act
        var result = await _service.DeleteCategoryAsync(categoryId);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.DeleteCategoryAsync(categoryId), Times.Once);
    }

    [Fact]
    public async Task GetCategoriesByRestaurantAsync_ValidRestaurantId_ReturnsRestaurantCategories()
    {
        // Arrange
        var restaurantId = 1;
        var categories = new List<Category>
        {
            new Category { Id = 1, Name = "Pizzas", RestaurantConfigId = restaurantId },
            new Category { Id = 2, Name = "Pastas", RestaurantConfigId = restaurantId },
            new Category { Id = 3, Name = "Desserts", RestaurantConfigId = restaurantId }
        };

        _mockRepository.Setup(r => r.GetCategoriesByRestaurantAsync(restaurantId))
            .ReturnsAsync(categories);

        // Act
        var result = await _service.GetCategoriesByRestaurantAsync(restaurantId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.All(result, cat => Assert.Equal(restaurantId, cat.RestaurantConfigId));
    }

    [Fact]
    public async Task GetCategoriesByRestaurantAsync_NoCategories_ReturnsEmptyList()
    {
        // Arrange
        var restaurantId = 999;
        _mockRepository.Setup(r => r.GetCategoriesByRestaurantAsync(restaurantId))
            .ReturnsAsync(new List<Category>());

        // Act
        var result = await _service.GetCategoriesByRestaurantAsync(restaurantId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
