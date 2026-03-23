# MenuService Unit Tests

Comprehensive unit test suite for the MenuService API using xUnit and Moq.

## Test Setup Instructions

Before running tests, the test models need to match the actual API models. Follow these steps to generate proper tests:

### 1. Check Actual Models
```bash
cd MenuService/src/MenuService.API/Models
ls -la
```

### 2. Generate Tests from Models
Review the following actual model files:
- `MenuStructure.cs` - Contains entity models
- `MenuModels.cs` - Contains request/response DTOs
- Repository interfaces in `Repositories/`
- Service interfaces in `Services/`

### 3. Test Coverage Areas

#### Service Layer Tests
- **RestaurantConfigService** - CRUD for restaurant configurations
- **CategoryService** - Category management per restaurant
- **MenuItemService** - Menu item operations and search
- **CustomizationService** - Customization groups and options
- **ComboDealService** - Combo deal management
- **VariantService** - Menu item variants
- **SubcategoryService** - Menu item subcategories

#### Controller Tests
- **RestaurantConfigController** - Restaurant CRUD endpoints
- **CategoriesController** - Category management endpoints  
- **MenuItemsController** - Menu item endpoints
- **VariantsController** - Variant management endpoints
- **CustomizationsController** - Customization endpoints
- **SubcategoriesController** - Subcategory endpoints
- **ComboDealsController** - Combo deal endpoints

### 4. Test Running

```bash
# Run all tests
dotnet test

# Run specific test class
dotnet test --filter "ClassName=RestaurantConfigServiceTests"

# Run with verbose output
dotnet test --verbosity=detailed

# Run with code coverage
dotnet test /p:CollectCoverage=true
```

## Test File Structure

Place test files in the appropriate directories:
- **Services/** - Service layer unit tests
- **Controllers/** - Controller/endpoint tests
- **Repositories/** - Repository pattern tests (optional)

## Mocking Strategy

- Use `Moq` library for mocking dependencies
- Mock all external dependencies (repositories, logger, services)
- Use `Arrange-Act-Assert` pattern for test structure
- Setup returns using `.ReturnsAsync()` for async operations

## Creating Tests

### Example Service Test Template

```csharp
using Xunit;
using Moq;

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
    public async Task Method_Scenario_Expected_Result()
    {
        // Arrange
        var input = new MockInput();
        _mockRepository.Setup(r => r.SomeMethodAsync())
            .ReturnsAsync(expected Result);

        // Act
        var result = await _service.MethodUnderTest(input);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedValue, result.PropertyName);
    }
}
```

### Example Controller Test Template

```csharp
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;

namespace MenuService.Tests.Controllers;

public class RestaurantConfigControllerTests
{
    private readonly Mock<IRestaurantConfigService> _mockService;
    private readonly RestaurantConfigController _controller;

    public RestaurantConfigControllerTests()
    {
        _mockService = new Mock<IRestaurantConfigService>();
        _controller = new RestaurantConfigController(_mockService.Object);
    }

    [Fact]
    public async Task Endpoint_Scenario_ReturnsCorrectStatusCode()
    {
        // Arrange
        var request = new RestaurantConfigRequest { /* properties */ };
        _mockService.Setup(s => s.CreateRestaurantConfigAsync(request))
            .ReturnsAsync(new RestaurantConfig { /* properties */ });

        // Act
        var result = await _controller.CreateRestaurant(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
    }
}
```

## Important Notes

1. **Test Data** - Keep test data realistic and meaningful
2. **Naming** - Use descriptive test names following `Method_Scenario_Expected` pattern
3. **Independence** - Each test should be independent and not rely on test order
4. **Cleanup** - Setup proper mocks to avoid side effects
5. **Coverage** - Aim for >80% code coverage on service layer

## Next Steps

1. Install NUnit or xUnit test runners in VS Code if needed
2. Create test files following the templates above
3. Run tests frequently during development
4. Update tests when changing API contracts
5. Consider adding integration tests with InMemory database

## CI/CD Integration

Add to your CI/CD pipeline:
```bash
dotnet test --logger "trx;LogFileName=test-results.trx" /p:CollectCoverage=true /p:CoverageFormat=opencover
```

This will generate test results and coverage reports for your pipeline.
