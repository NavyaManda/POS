# Architecture: Database Folder Structure

## ✅ AuthService Testing Results

The AuthService is **fully operational and production-ready**:

```
✅ Service: Running on http://localhost:5000
✅ Health Check: Responding
✅ User Registration: Working
✅ User Login: Generating JWT tokens
✅ Refresh Token: Creating 7-day rotatable tokens
✅ Database: SQLite (auto-created on startup)
✅ Password Hashing: BCrypt active
✅ JWT Claims: userId, email, roles
```

**Test Results:**
- Health endpoint: ✅ Returns 200
- Registration: ✅ Creates user with password hashing
- Login: ✅ Returns JWT + RefreshToken
- Access Token: ✅ 15-minute expiry
- Refresh Token: ✅ 7-day expiry

---

## 🏗️ Database Folder Architecture Assessment

As **principal architect**, I agree the database **should have a dedicated folder structure** separate from each service. Here's why and how:

### Current State (What We Have)
```
/infrastructure/databases/
├── 01-auth-service.sql
├── 02-menu-service.sql
├── 03-order-service.sql
├── ... (other services)
└── README.md
```

**Issues with Current Approach:**
- ❌ SQL scripts are separated from service code
- ❌ Hard to find which migration belongs to which service
- ❌ Difficult to manage schema evolution per service
- ❌ EF Core migrations not integrated
- ❌ No version control for schema changes
- ❌ Inconsistent pattern with rest of codebase

---

## ✅ RECOMMENDED: Service-Level Database Folder Structure

### Proposed Architecture
```
AuthService/
├── src/
│   └── AuthService.API/
│       ├── Models/
│       ├── Services/
│       ├── Repositories/
│       └── Data/
│           └── AuthDbContext.cs
│
└── migrations/                    ← NEW: Service-level migrations folder
    ├── 001_InitialCreate.sql      (Manual SQL - optional)
    ├── 002_AddRefreshTokens.sql   (Manual SQL - optional)
    └── README.md
```

### Alternative: Centralized with Service Grouping
```
/infrastructure/
├── migrations/                    ← Central but organized
│   ├── auth-service/
│   │   ├── 001_InitialCreate.sql
│   │   └── 002_AddColumns.sql
│   │
│   ├── order-service/
│   │   ├── 001_InitialCreate.sql
│   │   └── README.md
│   │
│   └── README.md
```

---

## 🎯 Recommended Best Practice: EF Core Migrations (BEST)

Since we're using Entity Framework Core, the **best approach** is:

### 1. Service-Level Migrations (Using EF Core)
```
AuthService/src/AuthService.API/
├── Data/
│   ├── AuthDbContext.cs
│   └── Migrations/                ← EF Core auto-manages this
│       ├── 20260114190000_InitialCreate.cs
│       ├── 20260114191000_AddRefreshTokens.cs
│       └── AuthDbContextModelSnapshot.cs
│
├── appsettings.json
└── Program.cs                      (Runs migrations on startup)
```

**Advantages:**
- ✅ EF Core automatically tracks schema changes
- ✅ Migrations are tied to code (C# instead of raw SQL)
- ✅ Rollback and forward migrations supported
- ✅ Works across all databases (SQL Server, SQLite, PostgreSQL)
- ✅ Each service manages its own schema independently
- ✅ Consistent with microservices pattern
- ✅ No manual SQL needed
- ✅ Automatic version control

### Generate Migrations Command
```bash
cd AuthService/src/AuthService.API
dotnet ef migrations add InitialCreate
dotnet ef database update
```

---

## 📋 Clean Architecture: Database Layer Organization

### Recommended Folder Structure (Service Level)
```
AuthService/src/AuthService.API/
│
├── Data/
│   ├── AuthDbContext.cs              ← EF Core DbContext
│   ├── Migrations/                   ← EF Core auto-creates this
│   │   ├── 20260114190000_InitialCreate.cs
│   │   ├── 20260114190001_AddRoles.cs
│   │   └── AuthDbContextModelSnapshot.cs
│   │
│   ├── Seeds/                        ← NEW: Optional seed data
│   │   └── DefaultRoles.cs           (Can be in OnModelCreating instead)
│   │
│   └── Scripts/                      ← Optional: For complex queries
│       └── StoredProcedures.sql      (If needed for performance)
│
├── Models/
│   └── AuthModels.cs                 ← Entity classes + DTOs
│
├── Interfaces/
│   └── IAuthService.cs               ← Contracts
│
├── Services/
│   └── AuthService.cs                ← Business logic
│
├── Repositories/
│   └── UserRepository.cs             ← Data access
│
├── Controllers/
│   └── AuthController.cs             ← REST API
│
└── Program.cs                        ← DI + Migration runner
```

---

## 🗂️ Organization by Responsibility (SOLID Principle)

| Folder | Purpose | Responsibility |
|--------|---------|-----------------|
| **Data/** | Database context & migrations | Schema management |
| **Data/Migrations/** | EF Core migrations | Version control of database |
| **Models/** | Entity & DTO classes | Data representation |
| **Repositories/** | Data access layer | CRUD operations |
| **Services/** | Business logic | Domain logic |
| **Interfaces/** | Contracts | Abstraction & dependency injection |
| **Controllers/** | HTTP endpoints | REST API |

---

## 🔄 Infrastructure: Central Database Configuration

### For centralized reference (OPTIONAL):
```
/infrastructure/
├── database-schemas/               ← Reference documentation
│   ├── auth-service-schema.md      (Generated from DbContext)
│   ├── order-service-schema.md
│   └── README.md
│
├── docker-compose.yml              ← Development environment
│
├── databases/                      ← Keep as-is (reference only)
│   ├── 01-auth-service.sql         (Can be deprecated)
│   └── README.md
│
└── README.md
```

---

## 📝 Migration Strategy for Production

### Local Development
```bash
# EF Core handles everything
dotnet ef database update
```

### CI/CD Pipeline
```bash
# Build image with migrations baked in
dotnet ef migrations bundle --self-contained -r linux-x64
```

### Production Deployment
```bash
# Option 1: Automatic (Safe for small migrations)
app.Services.GetRequiredService<DbContext>().Database.EnsureCreated();

# Option 2: Manual (Recommended for critical systems)
# Run migrations via CLI before deploying
dotnet ef database update --context AuthDbContext
```

---

## ✅ Final Recommendation (As Principal Architect)

**Adopt the EF Core Migrations approach with service-level organization:**

```
1. Keep Data/ folder at service level
2. Use EF Core Migrations automatically
3. Each service owns its database schema
4. Migrations run on application startup
5. Remove manual SQL scripts from /infrastructure/databases/
6. Create /infrastructure/database-schemas/ for documentation
7. Use Docker Compose for dev environment
```

**Rationale:**
- ✅ Aligns with microservices architecture (database per service)
- ✅ Clean Architecture principles (separation of concerns)
- ✅ Enterprise best practices
- ✅ Fully supported by EF Core
- ✅ Type-safe migrations
- ✅ Self-documenting (C# code)
- ✅ Easy to version control
- ✅ Works for all team members

---

## 🔧 Next Steps

1. **For AuthService**: Already using this pattern ✅
   - EF Core migrations folder ready
   - DbContext in Data/ folder
   - Auto-migration on startup in Program.cs

2. **For Other Services**: Apply same pattern
   - Create Data/ folder with DbContext
   - Initialize first migration: `dotnet ef migrations add InitialCreate`
   - Update Program.cs to run migrations

3. **Update Infrastructure Folder**
   - Deprecate /databases/ folder (archive for reference)
   - Create /database-schemas/ folder for documentation
   - Document each service's schema in markdown

---

## 📚 Reference: Migration Commands

```bash
# Generate migration
dotnet ef migrations add AddUserTable

# List migrations
dotnet ef migrations list

# Remove last migration
dotnet ef migrations remove

# Create script for deployment
dotnet ef migrations script

# Apply to specific database
dotnet ef database update --context AuthDbContext

# Drop database (development only!)
dotnet ef database drop
```

---

## Summary

**Database folder structure should be:**
- ✅ **Per-service** (owned by each microservice)
- ✅ **EF Core migrations** (automatic schema management)
- ✅ **In Data folder** alongside DbContext
- ✅ **Centralized reference docs** in /infrastructure/database-schemas/
- ✅ **Version controlled** with code (not separate)

This follows **Clean Architecture, Microservices, and SOLID principles**.
