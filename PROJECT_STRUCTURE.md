# POS System - Microservices Project Structure

## Overview

This restructured layout provides **independent, standalone projects** for each microservice, making it easier to:
- Develop services independently
- Deploy services separately
- Manage dependencies per service
- Scale services independently

## Current Directory Structure

```
POS/
├── AuthService/                          # Independent Auth project
│   ├── src/AuthService.API/             # .NET project
│   │   ├── Controllers/
│   │   ├── Services/
│   │   ├── Models/
│   │   ├── appsettings.json
│   │   └── AuthService.API.csproj
│   ├── Dockerfile
│   └── README.md
│
├── OrderService/                         # Independent Order project
│   ├── src/OrderService.API/
│   ├── Dockerfile
│   └── README.md
│
├── PaymentService/                       # Independent Payment project
│   ├── src/PaymentService.API/
│   ├── Dockerfile
│   └── README.md
│
├── APIGateway/                           # Independent Gateway project
│   ├── src/APIGateway.API/
│   ├── Dockerfile
│   └── README.md
│
├── MenuService/                          # TBD - Independent Menu project
├── InventoryService/                     # TBD - Independent Inventory project
├── KitchenDisplayService/                # TBD - Independent KDS project
├── LoyaltyService/                       # TBD - Independent Loyalty project
├── NotificationService/                  # TBD - Independent Notification project
│
├── SharedLibrary/                        # Shared code across services
│   ├── Models.cs                        # Common models & DTOs
│   └── Resilience.cs                    # Resilience patterns
│
├── frontend/                             # Angular application
│   ├── src/
│   ├── package.json
│   └── Dockerfile
│
├── infrastructure/                       # Infrastructure & deployment
│   ├── docker-compose.yml               # Local development environment
│   ├── kubernetes/                      # K8s manifests (TBD)
│   └── databases/                       # Database scripts
│       ├── 01-auth-service.sql
│       ├── 02-menu-service.sql
│       ├── 03-order-service.sql
│       ├── 04-payment-service.sql
│       ├── 05-inventory-service.sql
│       ├── 06-kds-service.sql
│       ├── 07-loyalty-service.sql
│       └── 08-notification-service.sql
│
├── ARCHITECTURE.md                       # System design & architecture
├── docker-compose.yml                    # Compose for local dev
└── README.md                             # Main project README
```

## Service Projects (Created)

### ✅ AuthService
- **Location**: `/AuthService`
- **Port**: 5001
- **Database**: POSAuthDb
- **Status**: Complete with controllers, services, models

### ✅ OrderService
- **Location**: `/OrderService`
- **Port**: 5004
- **Database**: POSOrderDb
- **Status**: Directory structure ready

### ✅ PaymentService
- **Location**: `/PaymentService`
- **Port**: 5005
- **Database**: POSPaymentDb
- **Status**: Directory structure ready

### ✅ APIGateway
- **Location**: `/APIGateway`
- **Port**: 5000
- **Database**: None (routing only)
- **Status**: Directory structure ready

## Service Projects (To Create)

### MenuService
- **Location**: `/MenuService`
- **Port**: 5002
- **Database**: POSMenuDb

### InventoryService
- **Location**: `/InventoryService`
- **Port**: 5003
- **Database**: POSInventoryDb

### KitchenDisplayService
- **Location**: `/KitchenDisplayService`
- **Port**: 5006
- **Database**: POSKDSDb

### LoyaltyService
- **Location**: `/LoyaltyService`
- **Port**: 5007
- **Database**: POSLoyaltyDb

### NotificationService
- **Location**: `/NotificationService`
- **Port**: 5008
- **Database**: POSNotificationDb

## Building & Running

### Individual Service (Example: AuthService)

```bash
cd AuthService/src/AuthService.API
dotnet restore
dotnet build
dotnet run
```

### All Services with Docker Compose

```bash
# From POS root directory
docker-compose -f infrastructure/docker-compose.yml up -d
```

### Individual Service as Docker

```bash
cd AuthService
docker build -t auth-service:1.0 .
docker run -p 5001:80 auth-service:1.0
```

## Service Communication

- **REST Calls**: Between services for synchronous operations
- **Event Bus**: RabbitMQ for asynchronous updates
- **API Gateway**: Central entry point for all client requests

## Development Workflow

1. **Clone Repository**
   ```bash
   git clone <repo>
   cd POS
   ```

2. **Start Infrastructure**
   ```bash
   docker-compose -f infrastructure/docker-compose.yml up -d mssql rabbitmq redis
   ```

3. **Initialize Databases**
   ```bash
   # Run database scripts
   sqlcmd -S localhost -i infrastructure/databases/01-auth-service.sql
   sqlcmd -S localhost -i infrastructure/databases/02-menu-service.sql
   # ... etc
   ```

4. **Run Individual Service**
   ```bash
   cd AuthService/src/AuthService.API
   dotnet watch run
   ```

5. **Or Run All with Docker Compose**
   ```bash
   docker-compose up
   ```

## Technology Stack per Service

Each service uses:
- **.NET 8.0** - Framework
- **Entity Framework Core** - ORM
- **SQL Server** - Database
- **NLog/Serilog** - Logging
- **RabbitMQ** - Event bus
- **Redis** - Caching (some services)

## Configuration

Each service has:
- `appsettings.json` - Production config
- `appsettings.Development.json` - Dev config
- `Dockerfile` - Container image definition
- `README.md` - Service-specific documentation

## Shared Components

Location: `/SharedLibrary`

- **Models.cs**: Common DTOs and response models
- **Resilience.cs**: Circuit breaker, retry policies, bulkhead patterns

## API Versioning

All services follow versioning pattern:
- `GET /api/v1/resource` - Version 1 endpoints
- `GET /api/v2/resource` - Version 2 endpoints (when available)

## Deployment

### Local Development
```bash
docker-compose up
```

### Staging
```bash
# Deploy to K8s
kubectl apply -f infrastructure/kubernetes/staging/
```

### Production
```bash
# Deploy to K8s with proper secrets
kubectl apply -f infrastructure/kubernetes/production/
```

## Project Files Template

Each service follows this structure:

```
ServiceName/
├── src/
│   └── ServiceName.API/
│       ├── Controllers/          # REST endpoints
│       ├── Services/            # Business logic
│       ├── Models/              # Data models & DTOs
│       ├── Repositories/        # Data access (EF Core)
│       ├── Properties/          # Project metadata
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       ├── Program.cs           # Startup configuration
│       └── ServiceName.API.csproj
├── Dockerfile
├── .dockerignore
└── README.md                    # Service documentation
```

## Next Steps

1. **Fill AuthService Templates**
   - Add `Program.cs` for DI configuration
   - Implement repository pattern for EF Core
   - Add integration tests

2. **Create Remaining Services**
   - Follow AuthService as template
   - Generate projects for Menu, Order, Payment, etc.

3. **API Gateway**
   - Implement routing logic
   - Add authentication middleware
   - Rate limiting and request logging

4. **Frontend**
   - Angular project setup
   - API service clients
   - State management (NgRx)

5. **Infrastructure**
   - Kubernetes manifests
   - CI/CD pipeline (GitHub Actions)
   - Monitoring & logging stack

## Troubleshooting

**Port conflicts?**
- Change service port in `appsettings.json`
- Update docker-compose.yml

**Database connection fails?**
- Ensure SQL Server container is running: `docker ps`
- Check connection string in appsettings.json
- Run database initialization scripts

**Service can't reach another service?**
- Ensure both are running
- Check docker-compose network configuration
- Verify service endpoints in configuration

## References

- [ARCHITECTURE.md](./ARCHITECTURE.md) - Full system design
- [AuthService README](./AuthService/README.md) - AuthService details
- [Docker Compose](./infrastructure/docker-compose.yml) - Local environment
- [Database Scripts](./infrastructure/databases/) - DB initialization
