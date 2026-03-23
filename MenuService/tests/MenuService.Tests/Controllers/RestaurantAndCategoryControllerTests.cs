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

public class RestaurantConfigControllerTests
{
    private readonly Mock<IRestaurantConfigService> _mockRestaurantService;
    private readonly Mock<ILogger<RestaurantConfigController>> _mockLogger;
    private readonly RestaurantConfigController _controller;

    public RestaurantConfigControllerTests()
    {
        _mockRestaurantService = new Mock<IRestaurantConfigService>();
        _mockLogger = new Mock<ILogger<RestaurantConfigController>>();
        _controller = new RestaurantConfigController(
            _mockRestaurantService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task CreateRestaurant_ValidRequest_ReturnsCreatedAtAction()
    {
        // Arrange
        var request = new RestaurantConfigRequest
        {
            Name = "Pizza Palace",
            RestaurantType = "Pizza",
            CuisineType = "Italian"
        };

        var expectedResponse = new RestaurantConfigResponse
        {
            Id = 1,
            Name = request.Name,
            RestaurantType = request.RestaurantType
        };

        _mockRestaurantService.Setup(s => s.CreateRestaurantConfigAsync(request))
            .ReturnsAsync(new RestaurantConfig
            {
                Id = 1,
                Name = request.Name,
                RestaurantType = request.RestaurantType,
                CuisineType = request.CuisineType
            });

        // Act
        var result = await _controller.CreateRestaurant(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(_controller.GetRestaurant), createdResult.ActionName);
        _mockRestaurantService.Verify(s => s.CreateRestaurantConfigAsync(request), Times.Once);
    }

    [Fact]
    public async Task GetRestaurant_ExistingId_ReturnsOkWithData()
    {
        // Arrange
        var restaurantId = 1;
        var expectedConfig = new RestaurantConfig
        {
            Id = restaurantId,
            Name = "Pizza Palace",
            RestaurantType = "Pizza"
        };

        _mockRestaurantService.Setup(s => s.GetRestaurantConfigAsync(restaurantId))
            .ReturnsAsync(expectedConfig);

        // Act
        var result = await _controller.GetRestaurant(restaurantId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task GetRestaurant_NonExistentId_ReturnsNotFound()
    {
        // Arrange
        var restaurantId = 999;
        _mockRestaurantService.Setup(s => s.GetRestaurantConfigAsync(restaurantId))
            .ReturnsAsync((RestaurantConfig)null);

        // Act
        var result = await _controller.GetRestaurant(restaurantId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetAllRestaurants_ReturnsOkWithList()
    {
        // Arrange
        var configs = new List<RestaurantConfig>
        {
            new RestaurantConfig { Id = 1, Name = "Pizza Palace", RestaurantType = "Pizza" },
            new RestaurantConfig { Id = 2, Name = "Biryani House", RestaurantType = "Biryani" }
        };

        _mockRestaurantService.Setup(s => s.GetAllRestaurantConfigsAsync())
            .ReturnsAsync(configs);

        // Act
        var result = await _controller.GetAllRestaurants();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedList = Assert.IsType<List<RestaurantConfigResponse>>(okResult.Value);
        Assert.Equal(2, returnedList.Count);
    }

    [Fact]
    public async Task UpdateRestaurant_ValidRequest_ReturnsOkWithData()
    {
        // Arrange
        var restaurantId = 1;
        var request = new RestaurantConfigRequest
        {
            Name = "Pizza Palace Updated",
            RestaurantType = "Pizza"
        };

        var updatedConfig = new RestaurantConfig
        {
            Id = restaurantId,
            Name = request.Name,
            RestaurantType = request.RestaurantType
        };

        _mockRestaurantService.Setup(s => s.UpdateRestaurantConfigAsync(restaurantId, request))
            .ReturnsAsync(updatedConfig);

        // Act
        var result = await _controller.UpdateRestaurant(restaurantId, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task DeleteRestaurant_ValidId_ReturnsNoContent()
    {
        // Arrange
        var restaurantId = 1;
        _mockRestaurantService.Setup(s => s.DeleteRestaurantConfigAsync(restaurantId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteRestaurant(restaurantId);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockRestaurantService.Verify(s => s.DeleteRestaurantConfigAsync(restaurantId), Times.Once);
    }

}
