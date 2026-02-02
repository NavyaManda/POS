# AuthService - Complete Authentication Service

Comprehensive authentication microservice with JWT tokens, user management, and role-based access control.

## Architecture

```
┌─────────────┐
│  Client     │ (Web, Mobile, etc.)
└──────┬──────┘
       │ HTTP/HTTPS
       ▼
┌──────────────────────────┐
│  AuthController          │ ← REST Endpoints
│  /api/v1/auth/*          │
└──────────────┬───────────┘
               │
               ▼
┌──────────────────────────┐
│  IAuthService            │ ← Business Logic
│  - LoginAsync            │
│  - RegisterAsync         │
│  - RefreshTokenAsync     │
│  - ValidateTokenAsync    │
└───────────┬──────────────┘
            │
      ┌─────┴──────────┐
      ▼                ▼
┌──────────────┐  ┌─────────────────────┐
│ITokenService │  │IUserRepository      │ ← Data Access
│              │  │IRefreshTokenRepo    │
└──────────────┘  └────────┬────────────┘
                           │
                           ▼
                  ┌─────────────────┐
                  │ AuthDbContext   │ ← EF Core ORM
                  └────────┬────────┘
                           │
                           ▼
                      ┌─────────────┐
                      │ POSAuthDb   │ ← SQL Server
                      │ (Users,     │
                      │  Roles,     │
                      │  Tokens)    │
                      └─────────────┘
```

## Project Structure (Clean Architecture)

```
AuthService/
├── src/
│   └── AuthService.API/
│       ├── Models/                    (Data Models & DTOs)
│       │   ├── AuthModels.cs         ├─ User, Role, UserRole, RefreshToken
│       │   └── ApiResponse.cs        ├─ LoginRequest, RegisterRequest, etc.
│       │
│       ├── Interfaces/               (Contracts - Abstraction Layer)
│       │   ├── IAuthService.cs       ├─ Authentication operations
│       │   ├── ITokenService.cs      ├─ Token generation & validation
│       │   ├── IUserRepository.cs    ├─ User data access contract
│       │   └── IRefreshTokenRepository.cs
│       │
│       ├── Services/                 (Business Logic Layer)
│       │   ├── AuthService.cs        ├─ Login, Register, Refresh, Validate
│       │   └── TokenService.cs       ├─ JWT token handling
│       │
│       ├── Repositories/             (Data Access Layer - EF Core)
│       │   ├── UserRepository.cs     ├─ User CRUD operations
│       │   └── RefreshTokenRepository.cs ├─ Token management
│       │
│       ├── Data/                     (Database Context)
│       │   └── AuthDbContext.cs      ├─ EF Core DbContext
│       │                             ├─ Entity mappings
│       │                             ├─ Seed data
│       │
│       ├── Controllers/              (Presentation Layer)
│       │   └── AuthController.cs     ├─ REST API endpoints
│       │                             ├─ Request/Response handling
│       │
│       ├── Program.cs                (DI & Middleware)
│       ├── appsettings.json          (Configuration)
│       ├── appsettings.Development.json
│       └── AuthService.API.csproj
│
├── Dockerfile
└── README.md
```

## API Endpoints

### 1. Login
```
POST /api/v1/auth/login
Content-Type: application/json

Request:
{
  "email": "user@example.com",
  "password": "MyPassword123!"
}

Response (200):
{
  "success": true,
  "message": "Login successful",
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIs...",
    "refreshToken": "aB3dEf1234567890...",
    "expiresIn": 900,
    "user": {
      "userId": "550e8400-e29b-41d4-a716-446655440000",
      "email": "user@example.com",
      "firstName": "John",
      "lastName": "Doe",
      "roles": ["customer"]
    }
  }
}

Error (401):
{
  "success": false,
  "message": "Invalid email or password"
}
```

### 2. Register
```
POST /api/v1/auth/register
Content-Type: application/json

Request:
{
  "email": "newuser@example.com",
  "password": "SecurePass123!",
  "firstName": "Jane",
  "lastName": "Smith"
}

Response (201):
{
  "success": true,
  "message": "User registered successfully. Please log in."
}

Error (400):
{
  "success": false,
  "message": "User with this email already exists"
}
```

### 3. Refresh Token
```
POST /api/v1/auth/refresh-token
Content-Type: application/json

Request:
{
  "refreshToken": "aB3dEf1234567890..."
}

Response (200):
{
  "success": true,
  "message": "Token refreshed successfully",
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIs...",
    "refreshToken": "zZ9gHi1234567890...",
    "expiresIn": 900,
    "user": { ... }
  }
}
```

### 4. Validate Token
```
POST /api/v1/auth/validate-token
Content-Type: application/json

Request:
{
  "token": "eyJhbGciOiJIUzI1NiIs..."
}

Response (200):
{
  "success": true,
  "message": "Token is valid",
  "data": {
    "isValid": true,
    "userId": "550e8400-e29b-41d4-a716-446655440000",
    "roles": ["customer"]
  }
}
```

### 5. Health Check
```
GET /api/v1/auth/health

Response (200):
{
  "status": "healthy",
  "timestamp": "2024-01-14T10:30:00Z",
  "service": "AuthService"
}
```

## Build & Run

### Local Development

```bash
cd AuthService/src/AuthService.API
dotnet restore
dotnet build
dotnet watch run
```

Service runs on `http://localhost:5001`

### Docker

```bash
docker build -t auth-service:1.0 .
docker run -p 5001:80 auth-service:1.0
```

## Security Features

- ✅ **Password Hashing**: BCrypt with salt
- ✅ **JWT Tokens**: HMAC SHA256
- ✅ **Token Rotation**: Refresh token mechanism
- ✅ **Token Revocation**: Logout support
- ✅ **Password Validation**: Strength requirements
- ✅ **Email Validation**: Format verification
- ✅ **CORS Support**: Configurable origins
- ✅ **Database**: SQL Server with encryption

## Token Details

- **Access Token Expiry**: 15 minutes
- **Refresh Token Expiry**: 7 days
- **Algorithm**: HMAC SHA256
- **Claims**: userId, email, roles, iat, exp

## Configuration

Update `appsettings.json`:

```json
{
  "Database": {
    "ConnectionString": "Server=localhost;Database=POSAuthDb;User Id=sa;Password=YourPassword;Encrypt=true;TrustServerCertificate=true;"
  },
  "Jwt": {
    "Secret": "your-secret-key-must-be-at-least-32-characters-long-for-security",
    "Issuer": "https://pos-system.local",
    "Audience": "pos-api",
    "ExpiryMinutes": 15
  },
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest"
  }
}
```

## Dependencies

- **Microsoft.EntityFrameworkCore**: ORM for database access
- **Microsoft.EntityFrameworkCore.SqlServer**: SQL Server provider
- **Microsoft.AspNetCore.Authentication.JwtBearer**: JWT authentication
- **System.IdentityModel.Tokens.Jwt**: JWT token handling
- **BCrypt.Net-Next**: Password hashing
- **Serilog**: Logging

## Testing

### Test Login
```bash
curl -X POST http://localhost:5001/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@pos.local",
    "password": "Password123!"
  }'
```

### Test Register
```bash
curl -X POST http://localhost:5001/api/v1/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "newuser@pos.local",
    "password": "SecurePass123!",
    "firstName": "John",
    "lastName": "Doe"
  }'
```

## Error Codes

| Code | Message | Cause |
|------|---------|-------|
| 400 | Invalid request | Validation failed |
| 401 | Invalid credentials | Wrong email/password |
| 409 | User already exists | Duplicate email |
| 500 | Internal error | Server error |

## Design Patterns

- ✅ **Repository Pattern**: Data abstraction
- ✅ **Service Pattern**: Business logic encapsulation
- ✅ **Dependency Injection**: Loose coupling
- ✅ **Interface Segregation**: Clean contracts
- ✅ **Async/Await**: Non-blocking operations

## Database Schema

- **Users**: User accounts and profiles
- **Roles**: Role definitions (admin, staff, customer)
- **UserRoles**: Many-to-many user-role mapping
- **RefreshTokens**: Token storage and revocation

## Future Enhancements

- [ ] OAuth2 (Google, Microsoft, GitHub)
- [ ] Two-Factor Authentication (2FA)
- [ ] Email Verification
- [ ] Password Reset
- [ ] Login Audit Log
- [ ] IP Whitelist
- [ ] Account Lockout
- [ ] Social Login Integration
