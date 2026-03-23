using Xunit;
using Moq;
using MenuService.API.Models;
using MenuService.API.Repositories;
using MenuService.API.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MenuService.Tests.Services;

public class CustomizationServiceTests
{
    private readonly Mock<IEnhancedMenuRepository> _mockRepository;
    private readonly CustomizationService _service;

    public CustomizationServiceTests()
    {
        _mockRepository = new Mock<IEnhancedMenuRepository>();
        _service = new CustomizationService(_mockRepository.Object);
    }

    [Fact]
    public async Task CreateCustomizationGroupAsync_ValidGroup_ReturnsGroup()
    {
        // Arrange
        var request = new CustomizationGroupRequest
        {
            Name = "Size",
            SelectionType = SelectionType.SingleSelect,
            RestaurantConfigId = 1,
            IsRequired = true
        };

        var expectedGroup = new CustomizationGroup
        {
            Id = 1,
            Name = request.Name,
            SelectionType = request.SelectionType,
            RestaurantConfigId = request.RestaurantConfigId,
            IsRequired = request.IsRequired
        };

        _mockRepository.Setup(r => r.CreateCustomizationGroupAsync(It.IsAny<CustomizationGroup>()))
            .ReturnsAsync(expectedGroup);

        // Act
        var result = await _service.CreateCustomizationGroupAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Size", result.Name);
        Assert.Equal(SelectionType.SingleSelect, result.SelectionType);
    }

    [Fact]
    public async Task CreateCustomizationOptionAsync_ValidOption_ReturnsOption()
    {
        // Arrange
        var request = new CustomizationOptionRequest
        {
            Name = "Large",
            Price = 50,
            CustomizationGroupId = 1
        };

        var expectedOption = new CustomizationOption
        {
            Id = 1,
            Name = request.Name,
            Price = request.Price,
            CustomizationGroupId = request.CustomizationGroupId
        };

        _mockRepository.Setup(r => r.CreateCustomizationOptionAsync(It.IsAny<CustomizationOption>()))
            .ReturnsAsync(expectedOption);

        // Act
        var result = await _service.CreateCustomizationOptionAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Large", result.Name);
        Assert.Equal(50, result.Price);
    }

    [Fact]
    public async Task GetCustomizationGroupAsync_ValidId_ReturnsGroup()
    {
        // Arrange
        var groupId = 1;
        var expectedGroup = new CustomizationGroup
        {
            Id = groupId,
            Name = "Size",
            SelectionType = SelectionType.SingleSelect
        };

        _mockRepository.Setup(r => r.GetCustomizationGroupAsync(groupId))
            .ReturnsAsync(expectedGroup);

        // Act
        var result = await _service.GetCustomizationGroupAsync(groupId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(groupId, result.Id);
        Assert.Equal("Size", result.Name);
    }

    [Fact]
    public async Task GetCustomizationGroupsAsync_MultipleGroups_ReturnsAll()
    {
        // Arrange
        var groups = new List<CustomizationGroup>
        {
            new CustomizationGroup { Id = 1, Name = "Size", SelectionType = SelectionType.SingleSelect },
            new CustomizationGroup { Id = 2, Name = "Toppings", SelectionType = SelectionType.MultiSelect },
            new CustomizationGroup { Id = 3, Name = "Sauce", SelectionType = SelectionType.SingleSelect }
        };

        _mockRepository.Setup(r => r.GetAllCustomizationGroupsAsync())
            .ReturnsAsync(groups);

        // Act
        var result = await _service.GetAllCustomizationGroupsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task DeleteCustomizationGroupAsync_ValidId_ReturnsTrue()
    {
        // Arrange
        var groupId = 1;
        _mockRepository.Setup(r => r.DeleteCustomizationGroupAsync(groupId))
            .ReturnsAsync(true);

        // Act
        var result = await _service.DeleteCustomizationGroupAsync(groupId);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.DeleteCustomizationGroupAsync(groupId), Times.Once);
    }

    [Fact]
    public async Task CalculateCustomizationPriceAsync_MultipleOptions_ReturnsCorrectTotal()
    {
        // Arrange
        var optionIds = new List<int> { 1, 2, 3 };
        var options = new List<CustomizationOption>
        {
            new CustomizationOption { Id = 1, Name = "Large", Price = 50 },
            new CustomizationOption { Id = 2, Name = "Extra Cheese", Price = 30 },
            new CustomizationOption { Id = 3, Name = "Extra Sauce", Price = 20 }
        };

        _mockRepository.Setup(r => r.GetCustomizationOptionsAsync(optionIds))
            .ReturnsAsync(options);

        // Act
        var result = await _service.CalculateCustomizationPriceAsync(optionIds);

        // Assert
        // 50 + 30 + 20 = 100
        Assert.Equal(100m, result);
    }

    [Fact]
    public async Task ValidateCustomizationSelectionAsync_MultiSelectWithMultipleSelections_ReturnsTrue()
    {
        // Arrange
        var groupId = 1;
        var group = new CustomizationGroup
        {
            Id = groupId,
            SelectionType = SelectionType.MultiSelect
        };
        var selectedOptionCount = 3;

        _mockRepository.Setup(r => r.GetCustomizationGroupAsync(groupId))
            .ReturnsAsync(group);

        // Act
        var result = await _service.ValidateCustomizationSelectionAsync(groupId, selectedOptionCount);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ValidateCustomizationSelectionAsync_SingleSelectWithMultipleSelections_ReturnsFalse()
    {
        // Arrange
        var groupId = 1;
        var group = new CustomizationGroup
        {
            Id = groupId,
            SelectionType = SelectionType.SingleSelect
        };
        var selectedOptionCount = 2; // Should only allow 1

        _mockRepository.Setup(r => r.GetCustomizationGroupAsync(groupId))
            .ReturnsAsync(group);

        // Act
        var result = await _service.ValidateCustomizationSelectionAsync(groupId, selectedOptionCount);

        // Assert
        Assert.False(result);
    }
}
