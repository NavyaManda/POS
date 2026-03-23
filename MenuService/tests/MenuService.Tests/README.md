# MenuService Unit Tests

Comprehensive unit test suite for the MenuService API using xUnit and Moq.

## Test Structure

### Service Tests
- **MenuItemServiceTests.cs** - Tests for menu item CRUD, search, and price calculation
- **RestaurantConfigServiceTests.cs** - Tests for restaurant configuration management
- **CategoryServiceTests.cs** - Tests for category operations
- **CustomizationServiceTests.cs** - Tests for customization groups and options
- **ComboDealServiceTests.cs** - Tests for combo deal management

### Controller Tests
- **RestaurantAndCategoryControllerTests.cs** - Tests for restaurant and category endpoints
- **MenuItemsControllerTests.cs** - Tests for menu item endpoints

## Running Tests

### Run All Tests
```bash
dotnet test
```

### Run Specific Test Class
```bash
dotnet test --filter "ClassName=MenuItemServiceTests"
```

### Run With Coverage
```bash
dotnet test /p:CollectCoverage=true /p:CoverageFormat=cobertura
```

### Run Tests in Watch Mode
```bash
dotnet watch test
```

## Test Coverage

### Service Layer Tests (65+ test cases)
- **Create operations** - Valid requests return correctly created entities
- **Retrieve operations** - Existing and non-existent entity handling
- **Update operations** - Valid updates return modified entities
- **Delete operations** - Valid deletions return success indication
- **Search/Filter operations** - Advanced filtering with multiple parameters
- **Price calculations** - Complex pricing with variants and customizations
- **Validation** - Business logic validation (e.g., SingleSelect vs MultiSelect)

### Controller Layer Tests (25+ test cases)
- **HTTP Status codes** - Correct status returned for each scenario
- **CreatedAtAction** - New resources return 201 with location header
- **OkObjectResult** - Successful retrievals return 200 with data
- **NotFoundResult** - Missing resources return 404
- **NoContentResult** - Deletions return 204
- **Request validation** - Invalid requests handled appropriately

## Key Test Patterns

### Mock Repository Setup
```csharp
_mockRepository.Setup(r => r.GetMenuItemAsync(1))
    .ReturnsAsync(expectedItem);
```

### Service Method Testing
```csharp
var result = await _service.CreateMenuItemAsync(request);
Assert.NotNull(result);
Assert.Equal("Margherita Pizza", result.Name);
```

### Controller Endpoint Testing
```csharp
var result = await _controller.GetMenuItem(1);
var okResult = Assert.IsType<OkObjectResult>(result);
```

## Test Data

All tests use in-memory mocked data with realistic values:
- Restaurant: Pizza Palace (ID: 1, Type: Pizza, Cuisine: Italian)
- Categories: Pizzas, Pastas, Desserts
- Menu Items: Margherita Pizza (299₹), Pepperoni Pizza (349₹)
- Customizations: Size (Single/Multi-select), Toppings (Multi-select)
- Prices: Base + Variants + Customizations + Bundles

## Dependencies

- **xUnit** - Test framework
- **Moq** - Mocking framework
- **Microsoft.EntityFrameworkCore** - EF Core packages for entity handling

## Continuous Integration

All tests should pass before commits:
```bash
# Pre-commit hook
dotnet build && dotnet test
```

## Next Steps

1. Add integration tests with InMemory database
2. Add Swagger/API documentation tests
3. Add performance/load tests
4. Add E2E tests with real database
5. Setup code coverage reporting

## Test Maintenance

- Update tests when adding new service methods
- Maintain mock data consistency across test suites
- Keep test names descriptive (e.g., `CreateMenuItem_ValidRequest_ReturnsCreatedAtAction`)
- Arrange-Act-Assert pattern for all tests
