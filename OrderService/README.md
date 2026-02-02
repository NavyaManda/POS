# OrderService - Order Processing & Management

Standalone microservice for order creation, management, and tracking.

## Project Structure

```
OrderService/
├── src/
│   └── OrderService.API/
│       ├── Controllers/        # API endpoints
│       ├── Services/          # Business logic
│       ├── Models/            # Data models & DTOs
│       ├── Repositories/      # Data access layer
│       ├── Properties/
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       └── OrderService.API.csproj
├── Dockerfile
└── README.md
```

## Build & Run

### Local Development

```bash
cd OrderService/src/OrderService.API
dotnet restore
dotnet build
dotnet watch run
```

Service runs on `http://localhost:5004`

### Docker

```bash
docker build -t order-service:1.0 .
docker run -p 5004:80 \
  -e "Database:ConnectionString=Server=host.docker.internal;Database=POSOrderDb;..." \
  order-service:1.0
```

## Endpoints

- **POST** `/api/v1/orders` - Create order
- **GET** `/api/v1/orders/{id}` - Get order details
- **GET** `/api/v1/orders` - List customer orders
- **PUT** `/api/v1/orders/{id}` - Update order
- **DELETE** `/api/v1/orders/{id}` - Cancel order
- **GET** `/api/v1/orders/health` - Health check

## Dependencies

- Menu Service (http://localhost:5002)
- Inventory Service (http://localhost:5003)
- RabbitMQ for event publishing

## Events Published

- `order.created.v1`
- `order.confirmed.v1`
- `order.preparing.v1`
- `order.ready.v1`
- `order.cancelled.v1`

## Events Consumed

- `payment.completed`
- `payment.failed`
