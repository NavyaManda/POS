using Xunit;
using Moq;
using MenuService.API.Models;
using MenuService.API.Repositories;
using MenuService.API.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MenuService.Tests.Services;

public class RestaurantConfigServiceTests
{
    private readonly Mock<IRestaurantConfigRepository> _mockRepository;
    private readonly RestaurantConfigService _service;

    public RestaurantConfigServiceTests()
    {
        _mockRepository = new Mock<IRestaurantConfigRepository>();
        _service = new RestaurantConfigService(_mockRepository.Object);
    }

    [Fact]
    public async Task CreateRestaurantConfigAsync_ValidConfig_ReturnsConfig()
    {
        // Arrange
        var request = new RestaurantConfigRequest
        {
            Name = "Pizza Palace",
            RestaurantType = "Pizza",
            CuisineType = "Italian",
            IsActive = true
        };

        var expectedConfig = new RestaurantConfig
        {
            Id = 1,
            Name = request.Name,
            RestaurantType = request.RestaurantType,
            CuisineType = request.CuisineType,
            IsActive = request.IsActive
        };

        _mockRepository.Setup(r => r.CreateConfigAsync(It.IsAny<RestaurantConfig>()))
            .ReturnsAsync(expectedConfig);

        // Act
        var result = await _service.CreateRestaurantConfigAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Pizza Palace", result.Name);
        Assert.Equal("Pizza", result.RestaurantType);
        _mockRepository.Verify(r => r.CreateConfigAsync(It.IsAny<RestaurantConfig>()), Times.Once);
    }

    [Fact]
    public async Task GetRestaurantConfigAsync_ExistingConfig_ReturnsConfig()
    {
        // Arrange
        var configId = 1;
        var expectedConfig = new RestaurantConfig
        {
            Id = configId,
            Name = "Pizza Palace",
            RestaurantType = "Pizza",
            IsActive = true
        };

        _mockRepository.Setup(r => r.GetConfigAsync(configId))
            .ReturnsAsync(expectedConfig);

        // Act
        var result = await _service.GetRestaurantConfigAsync(configId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(configId, result.Id);
        Assert.Equal("Pizza Palace", result.Name);
    }

    [Fact]
    public async Task GetAllRestaurantConfigsAsync_MultipleConfigs_ReturnsAll()
    {
        // Arrange
        var configs = new List<RestaurantConfig>
        {
            new RestaurantConfig { Id = 1, Name = "Pizza Palace", RestaurantType = "Pizza", IsActive = true },
            new RestaurantConfig { Id = 2, Name = "Biryani House", RestaurantType = "Biryani", IsActive = true },
            new RestaurantConfig { Id = 3, Name = "Breakfast Haven", RestaurantType = "Breakfast", IsActive = true }
        };

        _mockRepository.Setup(r => r.GetAllActiveAsync())
            .ReturnsAsync(configs);

        // Act
        var result = await _service.GetAllRestaurantConfigsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task UpdateRestaurantConfigAsync_ValidConfig_ReturnsUpdatedConfig()
    {
        // Arrange
        var configId = 1;
        var request = new RestaurantConfigRequest
        {
            Name = "Pizza Palace Updated",
            RestaurantType = "Pizza",
            CuisineType = "Italian"
        };

        var updatedConfig = new RestaurantConfig
        {
            Id = configId,
            Name = request.Name,
            RestaurantType = request.RestaurantType,
            CuisineType = request.CuisineType,
            IsActive = true
        };

        _mockRepository.Setup(r => r.UpdateConfigAsync(configId, It.IsAny<RestaurantConfig>()))
            .ReturnsAsync(updatedConfig);

        // Act
        var result = await _service.UpdateRestaurantConfigAsync(configId, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Pizza Palace Updated", result.Name);
    }

    [Fact]
    public async Task DeleteRestaurantConfigAsync_ValidId_ReturnsTrue()
    {
        // Arrange
        var configId = 1;
        _mockRepository.Setup(r => r.DeleteConfigAsync(configId))
            .ReturnsAsync(true);

        // Act
        var result = await _service.DeleteRestaurantConfigAsync(configId);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.DeleteConfigAsync(configId), Times.Once);
    }

    [Fact]
    public async Task GetRestaurantsByTypeAsync_PizzaType_ReturnsPizzaRestaurants()
    {
        // Arrange
        var pizzaConfigs = new List<RestaurantConfig>
        {
            new RestaurantConfig { Id = 1, Name = "Pizza Palace", RestaurantType = "Pizza", IsActive = true },
            new RestaurantConfig { Id = 2, Name = "Pizza Hut", RestaurantType = "Pizza", IsActive = true }
        };

        _mockRepository.Setup(r => r.GetAllActiveAsync())
            .ReturnsAsync(pizzaConfigs);

        // Act
        var result = await _service.GetRestaurantsByTypeAsync("Pizza");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, config => Assert.Equal("Pizza", config.RestaurantType));
    }

    [Fact]
    public async Task GetRestaurantsByCuisineAsync_ItalianCuisine_ReturnsItalianRestaurants()
    {
        // Arrange
        var italianConfigs = new List<RestaurantConfig>
        {
            new RestaurantConfig { Id = 1, Name = "Pizza Palace", CuisineType = "Italian", IsActive = true },
            new RestaurantConfig { Id = 2, Name = "Pasta Express", CuisineType = "Italian", IsActive = true }
        };

        _mockRepository.Setup(r => r.GetAllActiveAsync())
            .ReturnsAsync(italianConfigs);

        // Act
        var result = await _service.GetRestaurantsByCuisineAsync("Italian");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, config => Assert.Equal("Italian", config.CuisineType));
    }

    [Fact]
    public async Task GetRestaurantsByTypeAsync_NoMatches_ReturnsEmptyList()
    {
        // Arrange
        var allConfigs = new List<RestaurantConfig>
        {
            new RestaurantConfig { Id = 1, Name = "Pizza Palace", RestaurantType = "Pizza", IsActive = true }
        };

        _mockRepository.Setup(r => r.GetAllActiveAsync())
            .ReturnsAsync(allConfigs);

        // Act
        var result = await _service.GetRestaurantsByTypeAsync("NonExistent");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
