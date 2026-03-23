using Xunit;
using Moq;
using MenuService.API.Models;
using MenuService.API.Repositories;
using MenuService.API.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MenuService.Tests.Services;

public class ComboDealServiceTests
{
    private readonly Mock<IComboDealRepository> _mockRepository;
    private readonly ComboDealService _service;

    public ComboDealServiceTests()
    {
        _mockRepository = new Mock<IComboDealRepository>();
        _service = new ComboDealService(_mockRepository.Object);
    }

    [Fact]
    public async Task CreateComboDealAsync_ValidDeal_ReturnsDeal()
    {
        // Arrange
        var request = new ComboDealRequest
        {
            Name = "Combo A",
            Description = "2 Pizzas + 1 Coke + Garlic Bread",
            BasePrice = 599,
            RestaurantConfigId = 1
        };

        var expectedDeal = new ComboDeal
        {
            Id = 1,
            Name = request.Name,
            Description = request.Description,
            BasePrice = request.BasePrice,
            RestaurantConfigId = request.RestaurantConfigId
        };

        _mockRepository.Setup(r => r.CreateDealAsync(It.IsAny<ComboDeal>()))
            .ReturnsAsync(expectedDeal);

        // Act
        var result = await _service.CreateComboDealAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Combo A", result.Name);
        Assert.Equal(599, result.BasePrice);
    }

    [Fact]
    public async Task GetComboDealAsync_ExistingDeal_ReturnsDeal()
    {
        // Arrange
        var dealId = 1;
        var expectedDeal = new ComboDeal
        {
            Id = dealId,
            Name = "Combo A",
            Description = "2 Pizzas + 1 Coke + Garlic Bread",
            BasePrice = 599,
            RestaurantConfigId = 1
        };

        _mockRepository.Setup(r => r.GetDealAsync(dealId))
            .ReturnsAsync(expectedDeal);

        // Act
        var result = await _service.GetComboDealAsync(dealId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dealId, result.Id);
        Assert.Equal("Combo A", result.Name);
    }

    [Fact]
    public async Task GetAllComboDealsAsync_MultipleDeals_ReturnsAll()
    {
        // Arrange
        var deals = new List<ComboDeal>
        {
            new ComboDeal { Id = 1, Name = "Combo A", BasePrice = 599, RestaurantConfigId = 1 },
            new ComboDeal { Id = 2, Name = "Combo B", BasePrice = 799, RestaurantConfigId = 1 },
            new ComboDeal { Id = 3, Name = "Combo C", BasePrice = 999, RestaurantConfigId = 1 }
        };

        _mockRepository.Setup(r => r.GetAllDealsAsync())
            .ReturnsAsync(deals);

        // Act
        var result = await _service.GetAllComboDealsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task UpdateComboDealAsync_ValidDeal_ReturnsUpdatedDeal()
    {
        // Arrange
        var dealId = 1;
        var request = new ComboDealRequest
        {
            Name = "Combo A Updated",
            Description = "3 Pizzas + 1 Coke + Garlic Bread",
            BasePrice = 699,
            RestaurantConfigId = 1
        };

        var updatedDeal = new ComboDeal
        {
            Id = dealId,
            Name = request.Name,
            Description = request.Description,
            BasePrice = request.BasePrice,
            RestaurantConfigId = request.RestaurantConfigId
        };

        _mockRepository.Setup(r => r.UpdateDealAsync(dealId, It.IsAny<ComboDeal>()))
            .ReturnsAsync(updatedDeal);

        // Act
        var result = await _service.UpdateComboDealAsync(dealId, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Combo A Updated", result.Name);
        Assert.Equal(699, result.BasePrice);
    }

    [Fact]
    public async Task DeleteComboDealAsync_ValidId_ReturnsTrue()
    {
        // Arrange
        var dealId = 1;
        _mockRepository.Setup(r => r.DeleteDealAsync(dealId))
            .ReturnsAsync(true);

        // Act
        var result = await _service.DeleteComboDealAsync(dealId);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.DeleteDealAsync(dealId), Times.Once);
    }

    [Fact]
    public async Task ValidateComboDealAsync_ValidDeal_ReturnsTrue()
    {
        // Arrange
        var deal = new ComboDeal
        {
            Id = 1,
            Name = "Combo A",
            BasePrice = 599,
            RestaurantConfigId = 1
        };

        _mockRepository.Setup(r => r.ValidateDealAsync(deal))
            .ReturnsAsync(true);

        // Act
        var result = await _service.ValidateComboDealAsync(deal);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task GetComboDealsByRestaurantAsync_ValidRestaurantId_ReturnsRestaurantDeals()
    {
        // Arrange
        var restaurantId = 1;
        var deals = new List<ComboDeal>
        {
            new ComboDeal { Id = 1, Name = "Combo A", RestaurantConfigId = restaurantId },
            new ComboDeal { Id = 2, Name = "Combo B", RestaurantConfigId = restaurantId }
        };

        _mockRepository.Setup(r => r.GetDealsByRestaurantAsync(restaurantId))
            .ReturnsAsync(deals);

        // Act
        var result = await _service.GetComboDealsByRestaurantAsync(restaurantId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, deal => Assert.Equal(restaurantId, deal.RestaurantConfigId));
    }
}
