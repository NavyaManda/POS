# Restaurant POS Microservices System

Enterprise-grade Point of Sale system built with microservices architecture using .NET 8 and Angular.

## 📋 Architecture Overview

```
Client Layer
    ↓
API Gateway (Port 5000) - Authentication & Routing
    ↓
Microservices Cluster
├── Auth Service (5001)
├── Menu Service (5002)
├── Inventory Service (5003)
├── Order Service (5004)
├── Payment Service (5005)
├── Kitchen Display Service (5006)
├── Loyalty Service (5007)
└── Notification Service (5008)
```

## 🏗️ Project Structure

```
POS/
├── ARCHITECTURE.md                    # Complete architecture documentation
├── services/
│   ├── api-gateway/                  # API Gateway
│   │   ├── Routing.cs               # Service discovery & routing
│   │   ├── Middleware.cs            # Auth, rate limiting, logging
│   │   └── project.json
│   ├── auth-service/                # Authentication Service
│   │   ├── Models.cs                # User, token models
│   │   ├── AuthService.cs           # JWT generation, token validation
│   │   └── project.json
│   ├── menu-service/                # Menu Management
│   ├── order-service/               # Order Processing
│   │   ├── Models.cs
│   │   └── OrderService.cs
│   ├── inventory-service/           # Inventory Management
│   ├── payment-service/             # Payment Processing
│   │   ├── Models.cs
│   │   └── PaymentService.cs
│   ├── kitchen-display-service/     # KDS (Real-time)
│   │   ├── Models.cs
│   │   └── KDSService.cs
│   ├── loyalty-service/             # Loyalty Program
│   └── notification-service/        # Notifications
├── shared/
│   ├── Models.cs                    # Shared DTOs, responses
│   └── Resilience.cs                # Circuit breaker, retry policies
├── frontend/                         # Angular application
├── infrastructure/
│   ├── docker-compose.yml           # Service orchestration
│   └── db-init.sql                  # Database initialization
└── docs/
    └── API_SPECIFICATIONS.md         # REST API documentation
```

## 🚀 Quick Start

### Prerequisites
- Docker & Docker Compose
- .NET 8 SDK (for local development)
- Node.js 18+ (for Angular)
- SQL Server (via Docker)

### Running with Docker Compose

```bash
cd infrastructure
docker-compose up -d
```

This will start:
- SQL Server (Port 1433) - All 8 databases
- RabbitMQ (Port 5672, UI: 15672)
- Redis (Port 6379)
- All 9 microservices
- Angular Frontend (Port 4200)

### Database Setup

```bash
# Using SQL Server Management Studio or sqlcmd:
sqlcmd -S localhost,1433 -U sa -P PosSystem@123 -i infrastructure/db-init.sql
```

### Access Points

| Service | URL | Purpose |
|---------|-----|---------|
| API Gateway | http://localhost:5000 | Main API entry point |
| Angular App | http://localhost:4200 | Frontend application |
| RabbitMQ UI | http://localhost:15672 | Message broker management |
| SQL Server | localhost:1433 | Database server |

## 📡 API Communication Patterns

### 1. Synchronous REST (Commands/Queries)

```bash
# Create order
POST /api/v1/orders
{
  "customerId": "CUST-001",
  "items": [
    {"menuItemId": "ITEM-001", "quantity": 2}
  ]
}

# Response: 201 Created
{
  "orderId": "ORD-001",
  "orderNumber": "ORD-234567",
  "totalAmount": 45.99
}
```

### 2. Asynchronous Events (Notifications)

```
Order Service publishes:
  → order.created.v1
    ├─ KDS Service subscribes (displays order)
    ├─ Payment Service subscribes (awaits payment)
    └─ Notification Service subscribes (sends confirmation)

When payment completes:
  → payment.completed.v1
    └─ Order Service subscribes (confirms order)
```

## 🔐 Authentication Flow

### Login

```bash
POST /api/v1/auth/login
{
  "email": "user@restaurant.com",
  "password": "password123"
}

Response:
{
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "550e8400-e29b-41d4-a716...",
  "expiresIn": 900,
  "user": {
    "userId": "USER-001",
    "email": "user@restaurant.com",
    "roles": ["customer", "loyalty-member"]
  }
}
```

### Using the Token

```bash
# All subsequent requests:
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

### Token Refresh

```bash
POST /api/v1/auth/refresh-token
{
  "refreshToken": "550e8400-e29b-41d4-a716..."
}
```

## 📊 Database Ownership

Each microservice owns its database:

| Service | Database | Shared Data? |
|---------|----------|--------------|
| Auth | POSAuthDb | NO - Central auth only |
| Menu | POSMenuDb | NO - Menu catalog |
| Inventory | POSInventoryDb | NO - Stock levels |
| Order | POSOrderDb | NO - Order data |
| Payment | POSPaymentDb | NO - Transactions |
| KDS | POSKDSDb | NO - Display orders |
| Loyalty | POSLoyaltyDb | NO - Loyalty data |
| Notification | POSNotificationDb | NO - Logs |

**Data sharing via**: REST APIs (sync) + Events (async)

## 🔄 Order Processing Flow

```
1. User places order
   POST /api/v1/orders

2. Order Service:
   - Validates items (calls Menu Service)
   - Reserves inventory (calls Inventory Service)
   - Creates order (PENDING)
   - Publishes: order.created.v1

3. KDS Service (async):
   - Subscribes to order.created.v1
   - Displays order in kitchen
   - Chef starts preparation

4. Payment Service (async):
   - Subscribes to order.created.v1
   - Awaits payment processing
   - Publishes: payment.completed.v1

5. Order Service (async):
   - Subscribes to payment.completed.v1
   - Updates order to CONFIRMED
   - Publishes: order.confirmed.v1

6. KDS Service:
   - Updates to PREPARING
   - Publishes: order.ready.v1

7. Notification Service (async):
   - Subscribes to all events
   - Sends SMS/Email to customer
```

## 💾 Event Schema

All events published to RabbitMQ follow this structure:

```json
{
  "EventId": "550e8400-e29b-41d4-a716-446655440000",
  "EventType": "order.created.v1",
  "Timestamp": "2026-01-14T10:30:00Z",
  "CorrelationId": "550e8400-e29b-41d4-a716-446655440001",
  "Source": "OrderService",
  "Payload": {
    "OrderId": "ORD-001",
    "CustomerId": "CUST-001",
    "TotalAmount": 45.99,
    "Items": [...]
  }
}
```

## 🛡️ Security

### API Gateway Middleware
1. **Authentication**: JWT validation against Auth Service
2. **Authorization**: Role-based access control (RBAC)
3. **Rate Limiting**: 100 requests/minute per user
4. **Correlation ID**: Distributed tracing
5. **Request Logging**: All requests logged

### Service-to-Service Communication
- **mTLS**: Mutual TLS certificates for internal communication
- **API Keys**: X-API-Key header for fallback authentication

### Sensitive Data
- Password hashing with BCrypt
- PCI-DSS compliance for payment data
- Encrypted database connections
- Row-level security in SQL Server

## 🚨 Resilience Patterns

### Circuit Breaker
```
CLOSED (normal) → OPEN (failing) → HALF_OPEN (recovery) → CLOSED
Failure threshold: 5
Recovery timeout: 30 seconds
```

### Retry Policy
```
Max retries: 3
Initial delay: 1 second
Backoff multiplier: 2x
Max delay: 8 seconds
Jitter: ±10% (prevents thundering herd)
```

### Timeout Strategy
- Sync REST: 10 seconds
- Database: 5 seconds
- Payment: 30 seconds
- Event publishing: 5 seconds

### Idempotency
All POST operations require `Idempotency-Key` header:
```bash
POST /api/v1/orders
Idempotency-Key: 550e8400-e29b-41d4-a716-446655440000
```

## 📈 Monitoring

### Logs
- **ELK Stack**: Elasticsearch, Logstash, Kibana
- **Retention**: 30 days
- **Level**: INFO (production), DEBUG (staging)
- **Correlation IDs**: Trace requests across services

### Metrics
- **Prometheus + Grafana**:
  - Request count/latency per endpoint
  - Error rate per service
  - Database query performance
  - Message queue depth
  - Circuit breaker state

### Distributed Tracing
- **Jaeger/Zipkin**: End-to-end request tracing
- **Sampling**: 10% (production), 100% (staging)
- **Correlation IDs**: Cross-service tracking

### Alerts
- Error rate > 1%
- P99 latency > 1 second
- Service unavailable
- Circuit breaker OPEN
- DLQ messages > 10

## 📝 API Versioning

### Strategy: URL-based versioning

```
/api/v1/orders      # Current stable version
/api/v2/orders      # New version (if breaking changes)
```

### Deprecation Timeline
- V1 Released: Jan 14, 2026
- V2 Released: Jun 14, 2026 (with breaking changes)
- V1 Deprecated: Aug 14, 2026 (6-month notice)
- V1 Sunset: Jan 14, 2027 (6 months after deprecation)

## 🧪 Testing

### Unit Tests
Each service includes:
```bash
dotnet test --configuration Release --logger "console;verbosity=normal"
```

### Integration Tests
- Database: Use test containers
- Message broker: Use embedded RabbitMQ
- HTTP: Use TestServer from WebApplicationFactory

### E2E Tests
```bash
# Angular integration tests
npm run e2e
```

## 📦 Deployment

### Local Docker
```bash
docker-compose up -d
```

### Production (Kubernetes)
```bash
kubectl apply -f k8s/
```

See `infrastructure/k8s/` for Kubernetes manifests.

## 🔧 Configuration

### Environment Variables

**API Gateway**:
```
JwtSecret=your-secret-key
```

**All Services**:
```
ASPNETCORE_ENVIRONMENT=Production
ConnectionString=Server=mssql;Database=POSServiceDb;User Id=sa;Password=***
RabbitMqHost=rabbitmq
RedisConnection=redis:6379
```

**Payment Service**:
```
StripeApiKey=sk_live_***
```

**Notification Service**:
```
SendGridApiKey=SG.***
TwilioAccountSid=AC***
TwilioAuthToken=***
```

## 📚 Documentation

- **[ARCHITECTURE.md](./ARCHITECTURE.md)** - Complete system design
- **API Specifications** - [docs/API_SPECIFICATIONS.md](./docs/API_SPECIFICATIONS.md)
- **Database Schema** - [infrastructure/db-init.sql](./infrastructure/db-init.sql)
- **Docker Setup** - [infrastructure/docker-compose.yml](./infrastructure/docker-compose.yml)

## 🤝 Contributing

1. Fork the repository
2. Create feature branch (`git checkout -b feature/name`)
3. Commit changes (`git commit -am 'Add feature'`)
4. Push to branch (`git push origin feature/name`)
5. Create Pull Request

## 📄 License

Proprietary - Restaurant POS System

## 👥 Support

For issues, questions, or suggestions:
- Create an issue on GitHub
- Email: team@posystem.local
- Slack: #pos-development

---

**Last Updated**: January 14, 2026  
**Version**: 1.0.0  
**Status**: Production Ready
