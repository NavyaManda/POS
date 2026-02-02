# Restaurant POS System - Microservices Architecture

## Executive Summary
A highly scalable, resilient microservices architecture for modern restaurant POS operations. Each service owns its data, communicates via REST APIs and async events, with centralized authentication and API Gateway orchestration.

---

## 1. MICROSERVICES RESPONSIBILITIES

### 1.1 API Gateway
**Responsibility**: Single entry point for all client requests; authentication, routing, rate limiting.

- Route requests to appropriate services
- Validate JWT tokens with Auth Service
- Rate limiting and throttling
- Request/Response transformation
- API versioning management
- Circuit breaker integration
- Centralized logging and monitoring

**Port**: 5000

---

### 1.2 Auth Service
**Responsibility**: User authentication, JWT generation, OAuth token validation.

**Core Features**:
- User registration and login
- JWT token generation (access + refresh tokens)
- OAuth 2.0 integration (social login)
- Token validation and refresh
- Role-based access control (RBAC) management
- Session management

**Database**: `POSAuthDb`
- Users table
- Roles table
- UserRoles mapping
- RefreshTokens table
- OAuthProviders table

**Port**: 5001
**Key Endpoints**:
- POST `/api/v1/auth/register`
- POST `/api/v1/auth/login`
- POST `/api/v1/auth/validate-token`
- POST `/api/v1/auth/refresh-token`
- POST `/api/v1/auth/logout`

---

### 1.3 Menu Service
**Responsibility**: Product catalog management, categories, pricing, descriptions.

**Core Features**:
- Create/Read/Update/Delete menu items
- Category management
- Pricing tiers and variants
- Item availability status
- Image management (URLs stored, actual storage external)
- Menu publishing/versioning

**Database**: `POSMenuDb`
- MenuItems table
- Categories table
- MenuItemVariants table
- MenuItemPrices table
- MenuItemImages table

**Events Published**:
- `MenuItemCreated`
- `MenuItemUpdated`
- `MenuItemPriceChanged`
- `MenuItemAvailabilityChanged`

**Port**: 5002
**Key Endpoints**:
- GET `/api/v1/menu/items`
- POST `/api/v1/menu/items`
- PUT `/api/v1/menu/items/{id}`
- DELETE `/api/v1/menu/items/{id}`
- GET `/api/v1/menu/categories`

---

### 1.4 Inventory Service
**Responsibility**: Stock management, ingredient tracking, reorder points.

**Core Features**:
- Real-time inventory tracking
- Stock-in/Stock-out operations
- Low stock alerts
- Supplier management
- Ingredient consumption tracking
- Reorder automation

**Database**: `POSInventoryDb`
- Ingredients table
- StockLevels table
- StockTransactions table
- Suppliers table
- ReorderPoints table

**Events Published**:
- `InventoryLevelChanged`
- `LowStockAlert`
- `IngredientReceivedFromSupplier`
- `InventoryAdjusted`

**Port**: 5003
**Key Endpoints**:
- GET `/api/v1/inventory/items`
- POST `/api/v1/inventory/stock-in`
- POST `/api/v1/inventory/stock-out`
- GET `/api/v1/inventory/levels`
- POST `/api/v1/inventory/adjust`

---

### 1.5 Order Service
**Responsibility**: Order processing, order history, order status tracking.

**Core Features**:
- Order creation and validation
- Order status management
- Order history and search
- Order modifications
- Inventory reservation
- Order timing and preparation tracking

**Database**: `POSOrderDb`
- Orders table
- OrderItems table
- OrderStatus table
- OrderNotes table
- OrderTimestamps table

**Events Published**:
- `OrderCreated`
- `OrderConfirmed`
- `OrderPreparing`
- `OrderReady`
- `OrderPickedUp`
- `OrderCancelled`
- `OrderModified`

**Events Consumed**:
- `PaymentCompleted`
- `PaymentFailed`
- `MenuItemUnavailable`
- `LowInventoryAlert`

**Port**: 5004
**Key Endpoints**:
- POST `/api/v1/orders`
- GET `/api/v1/orders/{id}`
- PUT `/api/v1/orders/{id}`
- DELETE `/api/v1/orders/{id}`
- GET `/api/v1/orders`
- POST `/api/v1/orders/{id}/cancel`

---

### 1.6 Payment Service
**Responsibility**: Payment processing, transaction management, multiple payment methods.

**Core Features**:
- Payment processing (card, cash, digital wallets)
- Payment gateway integration (Stripe, Square, etc.)
- Refund management
- Payment verification
- PCI-DSS compliance
- Transaction logging

**Database**: `POSPaymentDb`
- Payments table
- Transactions table
- PaymentMethods table
- Refunds table
- PaymentGateways table

**Events Published**:
- `PaymentInitiated`
- `PaymentCompleted`
- `PaymentFailed`
- `RefundProcessed`

**Events Consumed**:
- `OrderCreated`
- `OrderCancelled`

**Port**: 5005
**Key Endpoints**:
- POST `/api/v1/payments/process`
- GET `/api/v1/payments/{transactionId}`
- POST `/api/v1/payments/{transactionId}/refund`
- GET `/api/v1/payments/methods`

---

### 1.7 Kitchen Display Service (KDS)
**Responsibility**: Real-time kitchen order display and management.

**Core Features**:
- Real-time order updates via WebSocket
- Order prioritization
- Preparation time tracking
- Station-specific orders (grill, fryer, etc.)
- Order status updates
- Order metrics and analytics

**Database**: `POSKDSDb`
- OrderDisplayStatus table
- StationAssignments table
- PreparationTimes table
- KDSMetrics table

**Events Consumed**:
- `OrderCreated`
- `OrderConfirmed`
- `OrderModified`
- `OrderCancelled`

**Events Published**:
- `OrderPreparing`
- `OrderReady`
- `PreparationTimeExceeded`

**Port**: 5006 (WebSocket on 5006)
**Key Endpoints**:
- WebSocket `/ws/kitchen-orders`
- POST `/api/v1/kds/orders/{id}/status`
- GET `/api/v1/kds/metrics`

---

### 1.8 Loyalty Service
**Responsibility**: Customer loyalty program management, rewards, points.

**Core Features**:
- Customer loyalty account creation
- Points tracking and accumulation
- Rewards redemption
- Membership tiers
- Promotional offers
- Customer segmentation

**Database**: `POSLoyaltyDb`
- LoyaltyAccounts table
- LoyaltyPoints table
- PointsTransactions table
- Rewards table
- MembershipTiers table
- Promotions table

**Events Published**:
- `PointsEarned`
- `PointsRedeemed`
- `TierUpgraded`
- `RewardRedeemed`

**Events Consumed**:
- `PaymentCompleted`
- `OrderCompleted`

**Port**: 5007
**Key Endpoints**:
- POST `/api/v1/loyalty/accounts`
- GET `/api/v1/loyalty/accounts/{customerId}`
- POST `/api/v1/loyalty/points/add`
- POST `/api/v1/loyalty/rewards/redeem`

---

### 1.9 Notification Service
**Responsibility**: Multi-channel notifications (SMS, Email, Push).

**Core Features**:
- Email notifications
- SMS notifications
- Push notifications
- In-app notifications
- Notification scheduling
- Notification templates
- Delivery tracking

**Database**: `POSNotificationDb`
- NotificationTemplates table
- NotificationLogs table
- NotificationPreferences table
- NotificationChannels table

**Events Consumed**:
- `OrderCreated`
- `OrderReady`
- `PaymentCompleted`
- `LoyaltyPointsEarned`
- `InventoryLowStock`

**Port**: 5008
**Key Endpoints**:
- POST `/api/v1/notifications/send`
- GET `/api/v1/notifications/logs`
- PUT `/api/v1/notifications/preferences/{userId}`

---

## 2. SERVICE BOUNDARIES

```
┌─────────────────────────────────────────────────────────────────┐
│                          API GATEWAY                            │
│              (Authentication, Routing, Rate Limiting)           │
└─────────────────────────────────────────────────────────────────┘
           ↓                ↓                ↓                ↓
    ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐
    │  Auth        │  │  Menu        │  │  Inventory   │  │  Order       │
    │  Service     │  │  Service     │  │  Service     │  │  Service     │
    │  Port: 5001  │  │  Port: 5002  │  │  Port: 5003  │  │  Port: 5004  │
    └──────────────┘  └──────────────┘  └──────────────┘  └──────────────┘
           ↓                ↓                ↓                ↓
    ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐
    │ Auth DB      │  │ Menu DB      │  │ Inventory DB │  │  Order DB    │
    │(POSAuthDb)   │  │(POSMenuDb)   │  │(POSInvDb)    │  │(POSOrderDb)  │
    └──────────────┘  └──────────────┘  └──────────────┘  └──────────────┘

    ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐
    │  Payment     │  │  KDS         │  │  Loyalty     │  │ Notification │
    │  Service     │  │  Service     │  │  Service     │  │ Service      │
    │  Port: 5005  │  │  Port: 5006  │  │  Port: 5007  │  │ Port: 5008   │
    └──────────────┘  └──────────────┘  └──────────────┘  └──────────────┘
           ↓                ↓                ↓                ↓
    ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐
    │ Payment DB   │  │  KDS DB      │  │ Loyalty DB   │  │Notification D│
    │(POSPayDb)    │  │(POSKDSDb)    │  │(POSLoyalDb)  │  │(POSNotifyDb) │
    └──────────────┘  └──────────────┘  └──────────────┘  └──────────────┘
```

---

## 3. DATABASE OWNERSHIP & SCHEMA ISOLATION

### Principle
**Each service owns one database. NO cross-service direct database access.**

| Service | Database | Owner | Connection String |
|---------|----------|-------|-------------------|
| Auth | POSAuthDb | Auth Service | Server=localhost;Database=POSAuthDb |
| Menu | POSMenuDb | Menu Service | Server=localhost;Database=POSMenuDb |
| Inventory | POSInventoryDb | Inventory Service | Server=localhost;Database=POSInventoryDb |
| Order | POSOrderDb | Order Service | Server=localhost;Database=POSOrderDb |
| Payment | POSPaymentDb | Payment Service | Server=localhost;Database=POSPaymentDb |
| KDS | POSKDSDb | KDS Service | Server=localhost;Database=POSKDSDb |
| Loyalty | POSLoyaltyDb | Loyalty Service | Server=localhost;Database=POSLoyaltyDb |
| Notification | POSNotificationDb | Notification Service | Server=localhost;Database=POSNotificationDb |

**Data Sharing Rule**: Services share data through REST APIs and async events, never direct DB access.

---

## 4. API COMMUNICATION FLOW

### 4.1 Request/Response Pattern (Synchronous)

```
Angular Client
    │
    ├─ POST /api/v1/orders
    │
API Gateway (Validates JWT, Routes to Order Service)
    │
    ├─ GET /api/v1/menu/items (Check menu)
    │
Menu Service (Returns menu items)
    │
    ├─ POST /api/v1/inventory/stock-out (Reserve items)
    │
Inventory Service (Returns reservation confirmation)
    │
    ├─ POST /api/v1/orders (Create order)
    │
Order Service (Validates, stores, publishes event)
    │
Response: OrderCreatedEvent
```

### 4.2 HTTP Status Codes Convention

```
200 OK               - Successful GET/PUT
201 Created          - Successful POST (resource created)
202 Accepted         - Async operation initiated
204 No Content       - Successful DELETE
400 Bad Request      - Invalid input
401 Unauthorized     - Missing/invalid token
403 Forbidden        - Authorized but not permitted
404 Not Found        - Resource not found
409 Conflict         - Business rule violation
500 Internal Error   - Server error
503 Service Unavailable - Circuit breaker open
```

### 4.3 API Versioning Strategy

**URL Versioning**: `/api/v1`, `/api/v2`
- Maintain backward compatibility
- Deploy new versions alongside old versions
- Client explicitly requests version
- Deprecation warnings in headers

```csharp
// Example versioning header
GET /api/v1/orders
X-API-Version: 1.0
Accept: application/json
```

---

## 5. EVENT-DRIVEN COMMUNICATION FLOW

### 5.1 Message Broker Architecture

**Technology**: RabbitMQ or Azure Service Bus
**Pattern**: Publish-Subscribe with Dead Letter Queues

### 5.2 Event Naming Convention

```
Format: {ServiceName}.{DomainEvent}.{EventVersion}

Examples:
- order.created.v1
- payment.completed.v1
- inventory.levelchanged.v1
- loyalty.pointsearned.v1
```

### 5.3 Event Message Structure

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
    "Items": [
      {
        "MenuItemId": "ITEM-001",
        "Quantity": 2,
        "UnitPrice": 10.99
      }
    ]
  }
}
```

### 5.4 Service Event Subscriptions

| Service | Publishes | Subscribes To |
|---------|-----------|---------------|
| Auth | UserCreated, UserRoleChanged | - |
| Menu | ItemCreated, ItemUpdated, PriceChanged | - |
| Inventory | LevelChanged, LowStockAlert, Received | OrderCreated, OrderCancelled |
| Order | Created, Confirmed, Preparing, Ready, Pickedup, Cancelled, Modified | PaymentCompleted, PaymentFailed, ItemUnavailable |
| Payment | Initiated, Completed, Failed, Refund | OrderCreated, OrderCancelled |
| KDS | Preparing, Ready, TimeExceeded | OrderCreated, OrderConfirmed, OrderModified, OrderCancelled |
| Loyalty | PointsEarned, PointsRedeemed, TierUpgraded | PaymentCompleted, OrderCompleted |
| Notification | Sent, Delivered, Failed | OrderCreated, OrderReady, PaymentCompleted, PointsEarned, LowStock |

### 5.5 Event Flow Example: Order Creation to Completion

```
1. User places order via Angular
   ├─ POST /api/v1/orders (to Order Service)
   
2. Order Service receives request
   ├─ Validates menu items (REST call to Menu Service)
   ├─ Reserves inventory (REST call to Inventory Service)
   ├─ Creates order in POSOrderDb
   ├─ Publishes: order.created.v1 event
   
3. KDS Service subscribes to order.created.v1
   ├─ Updates kitchen display in real-time (WebSocket)
   ├─ Publishes: order.preparing.v1 when chef starts
   
4. Payment Service subscribes to order.created.v1
   ├─ Waits for payment initiation
   
5. Payment Service (after payment)
   ├─ Publishes: payment.completed.v1
   
6. Order Service subscribes to payment.completed.v1
   ├─ Updates order status to "Confirmed"
   ├─ Publishes: order.confirmed.v1
   
7. Notification Service subscribes to order.confirmed.v1
   ├─ Sends email/SMS to customer
   
8. When order is ready, KDS publishes order.ready.v1
   ├─ Order Service updates status
   ├─ Notification Service sends notification
```

---

## 6. SECURITY ARCHITECTURE

### 6.1 Authentication Flow (JWT + OAuth)

```
Login Request
    │
    ├─ POST /api/v1/auth/login
    │
Auth Service
    ├─ Validate credentials
    ├─ Generate JWT token (expires in 15 min)
    ├─ Generate Refresh Token (expires in 7 days)
    │
Response:
{
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "550e8400-e29b-41d4-a716-446655440000",
  "expiresIn": 900,
  "user": {
    "id": "USER-001",
    "email": "user@restaurant.com",
    "roles": ["customer", "loyalty-member"]
  }
}
```

### 6.2 JWT Structure

```
Header:
{
  "alg": "HS256",
  "typ": "JWT"
}

Payload:
{
  "sub": "USER-001",
  "email": "user@restaurant.com",
  "roles": ["customer"],
  "iat": 1705232400,
  "exp": 1705233300,
  "iss": "pos-system",
  "aud": "pos-api"
}

Signature: HMACSHA256(base64(header) + "." + base64(payload), SECRET_KEY)
```

### 6.3 API Gateway Security

```csharp
// Middleware chain in API Gateway
1. Authentication Middleware
   ├─ Extract JWT from Authorization header
   ├─ Validate token signature
   ├─ Check expiration
   ├─ Verify claims (sub, roles, aud)
   
2. Authorization Middleware
   ├─ Check route requires authentication
   ├─ Validate user roles for resource
   
3. Rate Limiting Middleware
   ├─ Per-user: 100 requests/minute
   ├─ Per-IP: 1000 requests/minute
   
4. Request Validation Middleware
   ├─ Validate request headers
   ├─ Validate request body schema
   
5. Correlation ID Middleware
   ├─ Generate X-Correlation-ID header
   ├─ Pass through to all services for tracing
```

### 6.4 Service-to-Service Communication

**Mutual TLS (mTLS) for internal communication**:
- Each service has a client certificate
- Each service validates peer certificates
- Encrypted communication between services
- Certificate rotation policy

**API Key for fallback**:
```
X-API-Key: service-api-key-12345
X-Service-Name: order-service
```

### 6.5 Data Protection

| Layer | Protection |
|-------|-----------|
| Transport | HTTPS/TLS 1.3, mTLS between services |
| Authentication | JWT (15 min expiry), Refresh tokens (7 days) |
| Authorization | Role-based access control (RBAC) |
| Database | Row-level security, encryption at rest |
| Sensitive Data | PCI-DSS compliance for payment data |
| Audit Logging | All sensitive operations logged with user ID |

### 6.6 OAuth 2.0 Social Login

```
1. User clicks "Login with Google/Facebook"
2. Angular redirects to OAuth provider
3. User authenticates with provider
4. Provider redirects with authorization code
5. Auth Service exchanges code for token
   POST /api/v1/auth/oauth/callback
   {
     "provider": "google",
     "code": "4/0AY-E5Z..."
   }
6. Auth Service creates/links user account
7. Returns JWT + Refresh token
```

---

## 7. FAILURE HANDLING & RESILIENCE

### 7.1 Circuit Breaker Pattern

```
States:
├─ CLOSED (Normal operation)
│  ├─ Requests pass through
│  ├─ Track failure count
│  
├─ OPEN (Service failing)
│  ├─ Fail immediately without calling service
│  ├─ Return cached response or error
│  ├─ Wait before attempting recovery
│  
└─ HALF_OPEN (Recovery attempt)
   ├─ Send limited requests to test if service recovered
   ├─ If successful → CLOSED
   ├─ If fails → OPEN (restart wait period)
```

**Configuration**:
- Failure threshold: 5 failed requests
- Timeout threshold: 10 seconds
- Circuit open duration: 30 seconds

### 7.2 Retry Strategy

```
Retry Configuration (Exponential Backoff):
├─ Max retries: 3
├─ Initial delay: 1 second
├─ Max delay: 8 seconds
├─ Jitter: ±10% (prevent thundering herd)
├─ Retry only on: 408, 429, 500, 502, 503, 504

Example: 1s → 2s → 4s (3 attempts total)
```

### 7.3 Timeout Strategy

| Operation | Timeout |
|-----------|---------|
| Synchronous REST call | 10 seconds |
| Database query | 5 seconds |
| External API call | 15 seconds |
| Payment processing | 30 seconds |
| Event publishing | 5 seconds |

### 7.4 Compensation/Saga Pattern

**Choreography-based Saga for Order Placement**:

```
Order Service (Orchestrator)
├─ Create order (PENDING)
├─ Call Payment Service: ProcessPayment()
│  ├─ If SUCCESS → Order status: CONFIRMED
│  ├─ If FAIL → Order status: PAYMENT_FAILED
│        └─ Release inventory reservation
│
├─ Call Inventory Service: ReserveItems()
│  ├─ If SUCCESS → Continue
│  ├─ If FAIL → Compensate
│        ├─ Cancel payment
│        └─ Order status: INVENTORY_UNAVAILABLE
│
├─ Call KDS Service: DisplayOrder()
│
└─ Emit OrderCreatedEvent
```

### 7.5 Dead Letter Queue (DLQ) Handling

```
Event Flow with DLQ:
├─ Message published to order.created.v1
├─ Service A processes successfully
├─ Service B fails to process (3 retries)
│  ├─ Message moved to DLQ
│  ├─ Alert sent to monitoring team
│  ├─ Manual intervention/replay possible
│
DLQ Monitoring:
├─ Alerts if > 10 messages in DLQ
├─ Auto-replay after service recovery
├─ Monthly DLQ audit
```

### 7.6 Fault Tolerance Guidelines

**Idempotency Keys**:
- All POST operations require `Idempotency-Key` header
- Prevent duplicate processing on retries
- Server stores (RequestId, Response) for 24 hours

```
POST /api/v1/orders
{
  "Idempotency-Key": "550e8400-e29b-41d4-a716-446655440000",
  "Items": [...]
}
```

---

## 8. VERSIONING STRATEGY

### 8.1 Semantic Versioning

```
Format: MAJOR.MINOR.PATCH

- MAJOR: Breaking changes (Auth.v2, Order.v2)
- MINOR: New features, backward compatible
- PATCH: Bug fixes, backward compatible

Example: AuthService v1.2.3
```

### 8.2 API Versioning

```
URL-based versioning:
GET /api/v1/orders          → Current stable
GET /api/v2/orders          → New version (when breaking changes)

Deprecation Timeline:
├─ v1 Released: Jan 14, 2026
├─ v2 Released: Jun 14, 2026 (with breaking changes)
├─ v1 Deprecated: Aug 14, 2026 (6-month notice)
├─ v1 Sunset: Jan 14, 2027 (6 months after deprecation)
└─ v2 Becomes standard
```

### 8.3 Database Migration Strategy

```
Approach: Expand-Contract Pattern

Phase 1: EXPAND
├─ Add new column/table
├─ Backfill existing data
├─ Deploy code using new structure

Phase 2: CONTRACT
├─ Remove old column (after validation)
├─ Update schema documentation

Rollback always possible during EXPAND phase.
```

### 8.4 Service Compatibility Matrix

```
Service | Supports | Latest | Deprecated |
---------|----------|--------|-----------|
Auth | v1, v2 | v2 | v1 (sunset: Jan 14, 2027) |
Order | v1 | v1 | - |
Menu | v1 | v1 | - |
Payment | v1 | v1 | - |
Inventory | v1 | v1 | - |
KDS | v1 | v1 | - |
Loyalty | v1 | v1 | - |
Notification | v1 | v1 | - |
```

---

## 9. DEPLOYMENT MODEL

### 9.1 Infrastructure

```
Deployment: Containerized (Docker + Kubernetes)

├─ Kubernetes Cluster (Production)
│  ├─ Namespace: pos-system
│  ├─ Pods per service: 3 (high availability)
│  ├─ Resource limits: 512MB RAM, 250m CPU (per pod)
│
├─ Database Servers
│  ├─ Primary: SQL Server (production data)
│  ├─ Secondary: Read replica (reporting/analytics)
│
├─ Message Broker
│  ├─ RabbitMQ or Azure Service Bus (HA cluster)
│  ├─ 3 nodes minimum
│
├─ Caching Layer
│  ├─ Redis (distributed cache)
│  ├─ Session store, menu cache
│
├─ API Gateway
│  ├─ 3 instances (load balanced)
│  ├─ Nginx/Kong
```

### 9.2 CI/CD Pipeline

```
Developer Push
    │
    ├─ GitHub Actions / Azure DevOps
    │
    ├─ Build Stage
    │  ├─ Build .NET project
    │  ├─ Run unit tests (80% coverage required)
    │  ├─ Run integration tests
    │
    ├─ Quality Gate
    │  ├─ Code coverage > 80%
    │  ├─ SonarQube scan
    │  ├─ No critical security issues
    │
    ├─ Build Docker Image
    │  ├─ Tag: {service}:{version}-{build-number}
    │  ├─ Scan for vulnerabilities
    │
    ├─ Push to Container Registry
    │  ├─ Azure Container Registry / Docker Hub
    │
    ├─ Deploy to Staging
    │  ├─ Kubernetes rolling update
    │  ├─ Smoke tests
    │
    ├─ Deploy to Production
    │  ├─ Blue-green deployment
    │  ├─ Health check validation
    │  ├─ Automatic rollback on failure
```

### 9.3 Service Deployment

```
Service | Container | Resources | Replicas | Port |
---------|-----------|-----------|----------|------|
API Gateway | api-gateway:v1 | 512MB, 250m CPU | 3 | 5000 |
Auth Service | auth-service:v1 | 512MB, 250m CPU | 3 | 5001 |
Menu Service | menu-service:v1 | 512MB, 250m CPU | 2 | 5002 |
Inventory Service | inventory-service:v1 | 512MB, 250m CPU | 2 | 5003 |
Order Service | order-service:v1 | 512MB, 250m CPU | 3 | 5004 |
Payment Service | payment-service:v1 | 1GB, 500m CPU | 3 | 5005 |
KDS Service | kds-service:v1 | 512MB, 250m CPU | 2 | 5006 |
Loyalty Service | loyalty-service:v1 | 512MB, 250m CPU | 2 | 5007 |
Notification | notification-service:v1 | 512MB, 250m CPU | 2 | 5008 |
```

### 9.4 Monitoring & Observability

```
Centralized Logging:
├─ ELK Stack (Elasticsearch, Logstash, Kibana)
├─ All services log to centralized cluster
├─ Log retention: 30 days
├─ Log level: INFO (production), DEBUG (staging)

Metrics:
├─ Prometheus + Grafana
├─ Key metrics:
│  ├─ Request count & latency per endpoint
│  ├─ Error rate per service
│  ├─ Database query performance
│  ├─ Message queue depth
│  ├─ Circuit breaker state

Tracing:
├─ Jaeger/Zipkin distributed tracing
├─ Correlation IDs across services
├─ Trace sampling: 10% (production), 100% (staging)

Alerting:
├─ Alert on: Error rate > 1%, P99 latency > 1s
├─ Alert on: Service unavailable, circuit breaker open
├─ Escalation: On-call engineer, Slack notification
```

### 9.5 Rollback Strategy

```
Automated Rollback Triggers:
├─ Error rate > 5% (compared to baseline)
├─ P99 latency > 2x baseline
├─ Service health check fails > 3 times
├─ Pod crash loop detected

Blue-Green Deployment:
├─ Deploy new version (Green) alongside current (Blue)
├─ Run smoke tests on Green
├─ If tests pass: Switch traffic
├─ If tests fail: Keep Blue, discard Green
├─ Keep Blue for 1 hour as instant rollback
```

---

## 10. SYSTEM ARCHITECTURE DIAGRAM

```
┌──────────────────────────────────────────────────────────────────┐
│                           CLIENT LAYER                           │
│  ┌─────────────────────────────────────────────────────────────┐ │
│  │        Angular Frontend (TypeScript + RxJS)                 │ │
│  │  ├─ Menu browsing                                           │ │
│  │  ├─ Order placement                                         │ │
│  │  ├─ Order tracking                                          │ │
│  │  ├─ Payment page                                            │ │
│  │  └─ Loyalty dashboard                                       │ │
│  └─────────────────────────────────────────────────────────────┘ │
└──────────────────────────────────────────────────────────────────┘
                           │ (HTTPS)
                           ↓
┌──────────────────────────────────────────────────────────────────┐
│                    API GATEWAY (Port 5000)                       │
│  ┌─────────────────────────────────────────────────────────────┐ │
│  │  Request Pipeline:                                          │ │
│  │  1. Authentication (JWT validation)                         │ │
│  │  2. Authorization (Role-based access)                       │ │
│  │  3. Rate Limiting (100 req/min per user)                    │ │
│  │  4. Request Transformation                                  │ │
│  │  5. Route to microservice                                   │ │
│  │  6. Response aggregation                                    │ │
│  └─────────────────────────────────────────────────────────────┘ │
└──────────────────────────────────────────────────────────────────┘
     ↓                ↓                ↓                ↓
  5001            5002             5003             5004
┌──────────┐  ┌──────────┐   ┌──────────────┐  ┌──────────┐
│   AUTH   │  │  MENU    │   │ INVENTORY    │  │  ORDER   │
│ SERVICE  │  │ SERVICE  │   │  SERVICE     │  │ SERVICE  │
│          │  │          │   │              │  │          │
│ ├─User   │  │ ├─Browse │   │ ├─Stock      │  │ ├─Create │
│ │  mgmt  │  │ │  items │   │ │  levels    │  │ │  order │
│ ├─Auth   │  │ ├─Pricing│   │ ├─Alerts     │  │ ├─Track  │
│ ├─JWT    │  │ ├─Images │   │ ├─Suppliers  │  │ ├─Status │
│ └─OAuth  │  │ └─Search │   │ └─Reorder    │  │ └─Cancel │
└──────────┘  └──────────┘   └──────────────┘  └──────────┘
     │              │              │               │
     ↓              ↓              ↓               ↓
┌──────────┐  ┌──────────┐   ┌──────────────┐  ┌──────────┐
│POSAuthDb │  │POSMenuDb │   │POSInventoryDb│  │POSOrderDb│
│          │  │          │   │              │  │          │
│Users     │  │Items     │   │Ingredients   │  │Orders    │
│Roles     │  │Variants  │   │StockLevels   │  │Items     │
│Tokens    │  │Prices    │   │Suppliers     │  │Status    │
└──────────┘  └──────────┘   └──────────────┘  └──────────┘

     5005             5006            5007            5008
┌──────────┐   ┌──────────┐     ┌──────────┐   ┌──────────┐
│ PAYMENT  │   │   KDS    │     │ LOYALTY  │   │  NOTIFY  │
│ SERVICE  │   │ SERVICE  │     │ SERVICE  │   │ SERVICE  │
│          │   │          │     │          │   │          │
│ ├─Process│   │ ├─Display│     │ ├─Points │   │ ├─Email  │
│ │  card  │   │ │  orders│     │ │ accts  │   │ ├─SMS    │
│ ├─Refund │   │ ├─Update │     │ ├─Rewards│   │ ├─Push   │
│ ├─Verify │   │ │ status │     │ ├─Tiers │   │ └─In-app │
│ └─Log    │   │ └─Metrics│     │ └─Promo  │   │          │
└──────────┘   └──────────┘     └──────────┘   └──────────┘
     │              │                 │             │
     ↓              ↓                 ↓             ↓
┌──────────┐   ┌──────────┐     ┌──────────┐   ┌──────────┐
│POSPayDb  │   │POSKDSDb  │     │POSLoyDb  │   │POSNotifyDb
│          │   │          │     │          │   │          │
│Payments  │   │Orders    │     │Accounts  │   │Templates │
│Txns      │   │Status    │     │Points    │   │Logs      │
│Refunds   │   │Stations  │     │Rewards   │   │Prefs     │
└──────────┘   └──────────┘     └──────────┘   └──────────┘

                    ┌──────────────────────┐
                    │  MESSAGE BROKER      │
                    │   (RabbitMQ/ASBS)    │
                    │                      │
                    │ Exchanges/Topics:    │
                    │ ├─ order.events      │
                    │ ├─ payment.events    │
                    │ ├─ inventory.events  │
                    │ ├─ loyalty.events    │
                    │ └─ notification.events
                    └──────────────────────┘
                    ↑                      ↑
                    │ Event Subscription   │
                    │  (async/background)  │
                    │                      │
        ┌───────────┴────────────────────────┐
        │                                    │
        ▼                                    ▼
  [Order Service]                     [KDS Service]
  [Loyalty Service]                   [Notification Service]
  [Inventory Service]
```

---

## 11. ORDER PLACEMENT SEQUENCE DIAGRAM

```
Client          APIGateway      OrderService    MenuService    PaymentService
   │                │               │               │               │
   ├─ POST /orders─→│               │               │               │
   │                │               │               │               │
   │                ├─ Validate JWT─→               │               │
   │                │               │               │               │
   │                ├─ Route to Order Service       │               │
   │                ├──────────────→│               │               │
   │                │               │               │               │
   │                │               ├─ Validate menu items          │
   │                │               ├──────────────→│               │
   │                │               │←──────────────┤ (item details)│
   │                │               │               │               │
   │                │               ├─ Reserve inventory            │
   │                │               │  (via REST)                   │
   │                │               │               │               │
   │                │               ├─ Create order in DB           │
   │                │               │ (status: PENDING)            │
   │                │               │               │               │
   │                │               ├─ Publish: order.created.v1   │
   │                │               │  event (async)                │
   │                │               │               │               │
   │                │←──────────────┤ (201 Created)                │
   │←───────────────┤               │               │               │
   │ { OrderId,    │               │               │               │
   │   OrderNo,    │               │               │               │
   │   Items,      │               │               │               │
   │   Total }     │               │               │               │
   │               │               │               │               │
   │ (async flow - events)         │               │               │
   │               │               │               │               │
   │               │    [Event: order.created.v1 published to queue]
   │               │               │               │               │
   │               │   [KDS subscribes]  [Payment subscribes]      │
   │               │               │               │               │
   │               │               │               ├─ Process payment
   │               │               │               │  (card auth)
   │               │               │               │
   │               │               │  [Event: payment.completed]   │
   │               │               │←──────────────┤               │
   │               │               │               │               │
   │               │               ├─ Update: status=CONFIRMED     │
   │               │               │ ├─ Emit: order.confirmed.v1   │
   │               │               │               │               │
   │               │   [Notification subscribes]   │               │
   │               │               ├─ Send SMS/Email              │
   │               │               │ to customer                   │
   │               │               │               │               │
   │               │  [KDS updates kitchen display]                │
   │               │               │               │               │
   └───────────────────────────────────────────────────────────────┘
```

---

## 12. MICROSERVICES DEPENDENCY MATRIX

```
Dependency Chain:

API Gateway
├─ Depends on: All services (routes requests)
│
├─ Auth Service (no dependencies)
│
├─ Menu Service (no dependencies)
│
├─ Inventory Service
│  └─ Depends on: (async events only)
│
├─ Order Service
│  ├─ Calls (REST): Menu Service (sync)
│  ├─ Calls (REST): Inventory Service (sync)
│  ├─ Publishes: order.* events
│  └─ Subscribes: payment.completed
│
├─ Payment Service
│  ├─ Subscribes: order.created
│  ├─ Publishes: payment.* events
│  └─ Calls: External payment gateways (Stripe, etc.)
│
├─ KDS Service
│  ├─ Subscribes: order.* events
│  └─ Publishes: order.preparing, order.ready
│
├─ Loyalty Service
│  ├─ Subscribes: payment.completed, order.completed
│  └─ Publishes: points.earned, tier.upgraded
│
└─ Notification Service
   └─ Subscribes: All major events
      (order.*, payment.*, loyalty.*, inventory.*)
```

---

## 13. KEY ARCHITECTURAL DECISIONS (ADRs)

### ADR-001: Event-Driven for Cross-Service Updates
**Decision**: Use async events instead of synchronous calls for updates that span multiple services.
**Rationale**: Loose coupling, fault tolerance, better scalability.
**Trade-off**: Eventual consistency vs. immediate consistency.

### ADR-002: REST for Commands, Events for Notifications
**Decision**: REST for synchronous operations (queries, commands that need immediate response), events for asynchronous notifications.
**Rationale**: Clear separation of concerns, predictable latency.

### ADR-003: Database per Service
**Decision**: Each service owns and manages its own database schema.
**Rationale**: Independent scaling, technology flexibility, reduced blast radius.
**Trade-off**: Requires careful API design for data sharing.

### ADR-004: API Gateway as Single Entry Point
**Decision**: All client requests route through API Gateway.
**Rationale**: Centralized security, routing, monitoring.
**Trade-off**: Additional latency, potential single point of failure (mitigated with HA).

### ADR-005: JWT for Stateless Authentication
**Decision**: Use JWT tokens for authentication instead of session-based auth.
**Rationale**: Scalable across services, no shared session store needed.
**Trade-off**: Token revocation is eventual, not immediate.

---

## SUMMARY TABLE

| Aspect | Implementation |
|--------|-----------------|
| **Services** | 9 microservices + API Gateway |
| **Communication** | REST (sync) + Events (async) |
| **Database** | SQL Server (1 per service) |
| **Authentication** | JWT + OAuth 2.0 |
| **Message Broker** | RabbitMQ / Azure Service Bus |
| **Caching** | Redis |
| **Deployment** | Docker + Kubernetes |
| **Monitoring** | ELK + Prometheus + Jaeger |
| **API Versioning** | URL-based (/api/v1, /api/v2) |
| **Resilience** | Circuit breaker, Retries, Timeouts |
| **Saga Pattern** | Choreography-based for distributed transactions |
| **Idempotency** | Idempotency-Key header |

---

**Document Version**: 1.0  
**Last Updated**: January 14, 2026  
**Status**: Architecture Approved
