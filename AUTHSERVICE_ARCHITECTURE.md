# AuthService - Architecture Summary

## Overview

Complete, production-ready authentication microservice using **Clean Architecture** with **Repository Pattern**.

## Folder Structure

```
AuthService/
├── src/
│   └── AuthService.API/
│       ├── Models/                           ← Data Models & DTOs
│       │   ├── AuthModels.cs                 Entity definitions (User, Role, UserRole, RefreshToken)
│       │   └── ApiResponse.cs                Generic response wrappers
│       │
│       ├── Interfaces/                       ← Abstraction/Contract Layer
│       │   ├── IAuthService.cs               Authentication contract
│       │   ├── ITokenService.cs              Token operations contract
│       │   ├── IUserRepository.cs            User data access contract
│       │   └── IRefreshTokenRepository.cs    Token storage contract
│       │
│       ├── Services/                         ← Business Logic Layer
│       │   ├── AuthService.cs                Login, Register, Refresh, Validate operations
│       │   └── TokenService.cs               JWT token generation & validation
│       │
│       ├── Repositories/                     ← Data Access Layer (EF Core)
│       │   ├── UserRepository.cs             User CRUD operations
│       │   └── RefreshTokenRepository.cs     Token management operations
│       │
│       ├── Data/                             ← Database Context
│       │   └── AuthDbContext.cs              EF Core DbContext with mappings & seed data
│       │
│       ├── Controllers/                      ← Presentation/API Layer
│       │   └── AuthController.cs             REST endpoints with error handling
│       │
│       ├── Program.cs                        Dependency Injection & Middleware setup
│       ├── appsettings.json                  Production configuration
│       ├── appsettings.Development.json      Development configuration
│       └── AuthService.API.csproj            Project file
│
├── Dockerfile                                Docker containerization
└── README.md                                 Comprehensive documentation
```

## Clean Architecture Layers

### 1. **Presentation Layer** (Controllers)
- Handles HTTP requests/responses
- Request validation
- Error handling
- Maps DTOs to domain models

**File**: `Controllers/AuthController.cs`

### 2. **Business Logic Layer** (Services)
- Core authentication logic
- Token management
- Password validation
- Registration validation

**Files**: 
- `Services/AuthService.cs`
- `Services/TokenService.cs`

### 3. **Data Access Layer** (Repositories)
- EF Core CRUD operations
- Data retrieval logic
- Database queries
- Encapsulates data access

**Files**:
- `Repositories/UserRepository.cs`
- `Repositories/RefreshTokenRepository.cs`

### 4. **Data Layer** (Models & Context)
- EF Core entities
- Database context
- Entity mappings
- Seed data

**Files**:
- `Data/AuthDbContext.cs`
- `Models/AuthModels.cs`

### 5. **Abstraction Layer** (Interfaces)
- Service contracts
- Repository interfaces
- Enables loose coupling
- Supports dependency injection & testing

**Files**:
- `Interfaces/IAuthService.cs`
- `Interfaces/ITokenService.cs`
- `Interfaces/IUserRepository.cs`
- `Interfaces/IRefreshTokenRepository.cs`

## Data Flow

```
Client Request
    ↓
AuthController (Presentation)
    ↓
IAuthService (Abstract)
    ↓
AuthService (Business Logic)
    ├─→ ITokenService (Abstract)
    │       ↓
    │   TokenService
    │
    └─→ IUserRepository (Abstract)
    │       ↓
    │   UserRepository (Data Access)
    │       ↓
    │   AuthDbContext
    │       ↓
    │   SQL Server Database
    │
    └─→ IRefreshTokenRepository (Abstract)
            ↓
        RefreshTokenRepository (Data Access)
            ↓
        AuthDbContext
            ↓
        SQL Server Database

Response
    ↑
ApiResponse<T> (DTO)
    ↑
AuthController
    ↑
Client
```

## Key Design Principles

### ✅ SOLID Principles
- **S**ingle Responsibility: Each class has one reason to change
- **O**pen/Closed: Open for extension, closed for modification
- **L**iskov Substitution: Implementations can be substituted for interfaces
- **I**nterface Segregation: Specific interfaces instead of broad ones
- **D**ependency Inversion: Depend on abstractions, not concretions

### ✅ Repository Pattern
- Data access abstraction
- Testable services (mock repositories)
- Single source of data logic
- Easy to switch data sources

### ✅ Dependency Injection
- Loose coupling between layers
- Registered in `Program.cs`
- Constructor-based injection
- Scoped lifetime for repositories

### ✅ Async/Await
- Non-blocking operations
- Scalable performance
- All data operations are async

## Database Entities

### User
```
id (PK)
email (unique)
passwordHash
firstName
lastName
isActive
createdAt
lastLoginAt
UserRoles (navigation)
RefreshTokens (navigation)
```

### Role
```
id (PK)
name (unique)
description
createdAt
UserRoles (navigation)
```

### UserRole (Many-to-Many)
```
userId (FK, PK part 1)
roleId (FK, PK part 2)
assignedAt
User (navigation)
Role (navigation)
```

### RefreshToken
```
id (PK)
userId (FK)
token (unique)
expiryDate
isRevoked
createdAt
User (navigation)
```

## API Endpoints

| Method | Path | Handler | Purpose |
|--------|------|---------|---------|
| POST | `/api/v1/auth/login` | `Login()` | User login with email/password |
| POST | `/api/v1/auth/register` | `Register()` | New user registration |
| POST | `/api/v1/auth/refresh-token` | `RefreshToken()` | Get new access token |
| POST | `/api/v1/auth/validate-token` | `ValidateToken()` | Validate JWT token |
| GET | `/api/v1/auth/health` | `Health()` | Service health check |

## Dependencies Registered (Program.cs)

```csharp
// Database
AddDbContext<AuthDbContext>()

// Repositories (Scoped)
AddScoped<IUserRepository, UserRepository>()
AddScoped<IRefreshTokenRepository, RefreshTokenRepository>()

// Services (Scoped)
AddScoped<ITokenService, TokenService>()
AddScoped<IAuthService, AuthService>()

// Authentication
AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
AddJwtBearer()

// Other
AddAuthorization()
AddControllers()
AddCors()
```

## Security Implementation

### Password Security
- **Hashing**: BCrypt with auto-generated salt
- **Validation**: 8+ chars, uppercase, lowercase, number, special char
- **Storage**: Hashed only (never plaintext)

### Token Security
- **Type**: JWT (JSON Web Token)
- **Algorithm**: HMAC SHA256
- **Signing**: Using configured secret key
- **Claims**: userId, email, roles
- **Access Token**: 15-minute expiry
- **Refresh Token**: 7-day expiry
- **Revocation**: Marked as revoked in database

### Database Security
- SQL Server with connection encryption
- Entity Framework Core for SQL injection prevention
- User role-based access control

## Configuration Management

### appsettings.json (Production)
- Database connection string
- JWT configuration
- Service endpoints
- RabbitMQ settings

### appsettings.Development.json
- Local database
- Debug logging
- Development JWT key

### Sensitive Values
- Stored in environment variables
- User secrets (local development)
- Docker environment variables (production)

## Testing Strategy

### Unit Testing (Services)
- Mock repositories
- Test business logic
- Validate error handling

### Integration Testing
- Real database (test DB)
- Full request/response cycle
- Database operations

### Manual Testing
- cURL commands
- Postman collection
- Browser testing

## Performance Considerations

- ✅ Async database operations
- ✅ Token caching (Redis optional)
- ✅ Connection pooling (EF Core)
- ✅ Indexed email column (unique)
- ✅ Indexed token column (unique)

## Scalability

- **Stateless**: No session affinity required
- **Horizontally Scalable**: Multiple instances share database
- **Database**: SQL Server supports clustering
- **Caching**: Can add Redis for token validation
- **Load Balancing**: Supported via API Gateway

## Future Enhancements

### Phase 2
- [ ] Two-Factor Authentication (2FA)
- [ ] Email verification on signup
- [ ] Password reset functionality
- [ ] Login audit logging

### Phase 3
- [ ] OAuth2 integration (Google, Microsoft)
- [ ] Social login
- [ ] Account lockout after failed attempts
- [ ] IP whitelist/blacklist

### Phase 4
- [ ] Single Sign-On (SSO)
- [ ] LDAP integration
- [ ] Multi-tenant support
- [ ] Advanced security policies

## Files Summary

| File | Lines | Purpose |
|------|-------|---------|
| AuthModels.cs | 160 | Entities & DTOs |
| ApiResponse.cs | 35 | Response wrappers |
| IAuthService.cs | 9 | Auth service contract |
| ITokenService.cs | 12 | Token service contract |
| IUserRepository.cs | 13 | User repo contract |
| IRefreshTokenRepository.cs | 14 | Token repo contract |
| AuthService.cs | 130 | Auth business logic |
| TokenService.cs | 80 | Token operations |
| UserRepository.cs | 65 | User data access |
| RefreshTokenRepository.cs | 75 | Token data access |
| AuthDbContext.cs | 100 | EF Core context |
| AuthController.cs | 180 | REST endpoints |
| Program.cs | 65 | DI & setup |

**Total**: ~950 lines of clean, maintainable code

## Deployment

### Docker
```bash
docker build -t auth-service:1.0 .
docker run -p 5001:80 auth-service:1.0
```

### Docker Compose
```yaml
auth-service:
  image: auth-service:1.0
  ports:
    - "5001:80"
  environment:
    - ASPNETCORE_ENVIRONMENT=Production
    - Database__ConnectionString=...
  depends_on:
    - mssql
```

### Kubernetes
- Deployment YAML (pending)
- Service YAML (pending)
- ConfigMap for settings (pending)
- Secrets for sensitive data (pending)
