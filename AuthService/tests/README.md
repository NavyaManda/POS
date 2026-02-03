# AuthService Unit Tests

Comprehensive unit and integration tests for the Authentication Service using xUnit, Moq, and FluentAssertions.

## Project Structure

```
tests/
├── Services/
│   ├── TokenServiceTests.cs      # JWT token generation and validation tests
│   └── AuthServiceTests.cs       # Authentication logic tests
├── Controllers/
│   └── AuthControllerTests.cs    # API endpoint tests
├── Repositories/
│   └── UserRepositoryTests.cs    # Data access layer tests
├── AuthService.Tests.csproj      # Test project configuration
└── README.md                     # This file
```

## Test Coverage

### TokenServiceTests (26 test cases)
- **GenerateAccessToken**: Token creation with valid parameters, claim inclusion, expiry validation
- **GenerateRefreshToken**: Refresh token generation and uniqueness
- **ValidateToken**: Token validation with valid/invalid/tampered/expired tokens

### AuthServiceTests (15 test cases)
- **LoginAsync**: Valid credentials, invalid email/password, inactive users, last login updates
- **RegisterAsync**: User creation, duplicate email handling, password hashing
- **RefreshTokenAsync**: Valid/invalid/expired/revoked refresh tokens
- **ValidateTokenAsync**: Token validation response

### AuthControllerTests (16 test cases)
- **Login**: Valid login, invalid credentials, service exceptions, model validation
- **Register**: User registration, existing email, weak password, service errors
- **RefreshToken**: Valid/invalid/expired refresh tokens
- **ValidateToken**: Token validation endpoint
- **Logout**: Session termination

### UserRepositoryTests (12 test cases)
- **GetByIdAsync**: Existing and non-existing users
- **GetByEmailAsync**: Email lookup with case-insensitive search
- **CreateAsync**: User creation and duplicate email handling
- **UpdateAsync**: User updates and non-existing user handling
- **DeleteAsync**: User deletion and non-existing user handling

## Running Tests

### Prerequisites
```bash
cd AuthService/tests
```

### Run All Tests
```bash
dotnet test
```

### Run Specific Test File
```bash
dotnet test --filter "ClassName=AuthService.Tests.Services.TokenServiceTests"
```

### Run with Coverage Report
```bash
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover
```

### Run with Verbose Output
```bash
dotnet test --logger "console;verbosity=detailed"
```

## Testing Tools Used

- **xUnit**: Modern unit testing framework for .NET
- **Moq**: Mocking library for creating test doubles
- **FluentAssertions**: More readable assertion library
- **EntityFrameworkCore InMemory**: Database testing without SQL Server

## Test Patterns

### Mock Setup Pattern
```csharp
_mockUserRepository.Setup(x => x.GetByEmailAsync(email))
    .ReturnsAsync(user);
```

### In-Memory Database Pattern
```csharp
var options = new DbContextOptionsBuilder<AuthDbContext>()
    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
    .Options;
```

### Assertion Pattern
```csharp
result.Should().NotBeNull();
result!.AccessToken.Should().Be("expected-token");
result.Success.Should().BeTrue();
```

## Key Test Scenarios

### Authentication Flow
1. ✅ User registers with valid credentials
2. ✅ Password is properly hashed and never stored in plaintext
3. ✅ User logs in with correct password
4. ✅ Login fails with incorrect password
5. ✅ Login fails with inactive account
6. ✅ Last login timestamp is updated

### Token Management
1. ✅ Access tokens are generated with correct claims
2. ✅ Access tokens expire after configured duration
3. ✅ Refresh tokens are generated as valid base64 strings
4. ✅ Expired tokens are rejected
5. ✅ Tampered tokens are rejected
6. ✅ Revoked tokens cannot be used

### API Validation
1. ✅ Invalid email format is rejected
2. ✅ Weak passwords are rejected
3. ✅ Duplicate emails cannot be registered
4. ✅ Missing required fields return 400 Bad Request
5. ✅ Service errors return 500 Internal Server Error

## Continuous Integration

These tests are designed to run in CI/CD pipelines:

```bash
# In your CI pipeline
dotnet test --logger "xunit;LogFileName=test-results.xml"
```

## Future Test Enhancements

- [ ] Add OAuth integration tests
- [ ] Add RBAC permission tests
- [ ] Add token revocation list tests
- [ ] Add concurrent login handling tests
- [ ] Add performance/load tests
- [ ] Add security vulnerability tests (e.g., SQL injection)
