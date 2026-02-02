# API Gateway - Central Entry Point

Standalone microservice acting as the single entry point for all client requests.

## Project Structure

```
APIGateway/
├── src/
│   └── APIGateway.API/
│       ├── Middleware/        # Request/response pipeline
│       ├── Routing/           # Service routing logic
│       ├── Models/            # Response models
│       ├── Properties/
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       └── APIGateway.API.csproj
├── Dockerfile
└── README.md
```

## Build & Run

### Local Development

```bash
cd APIGateway/src/APIGateway.API
dotnet restore
dotnet build
dotnet watch run
```

Gateway runs on `http://localhost:5000`

### Docker

```bash
docker build -t api-gateway:1.0 .
docker run -p 5000:80 api-gateway:1.0
```

## Features

- **Authentication**: JWT token validation
- **Authorization**: Role-based access control
- **Routing**: Route requests to appropriate services
- **Rate Limiting**: 100 requests/minute per user
- **Request Logging**: Centralized correlation IDs
- **Error Handling**: Standardized error responses

## Service Routing

Maps paths to backend services:

- `/api/v1/auth/*` → Auth Service (5001)
- `/api/v1/menu/*` → Menu Service (5002)
- `/api/v1/inventory/*` → Inventory Service (5003)
- `/api/v1/orders/*` → Order Service (5004)
- `/api/v1/payments/*` → Payment Service (5005)
- `/api/v1/kds/*` → KDS Service (5006)
- `/api/v1/loyalty/*` → Loyalty Service (5007)
- `/api/v1/notifications/*` → Notification Service (5008)

## Configuration

Update `appsettings.json` to point to your services:

```json
{
  "ServiceEndpoints": {
    "AuthService": "http://auth-service:80",
    "MenuService": "http://menu-service:80",
    ...
  }
}
```

## Middleware Chain

1. **Correlation ID** - Generate X-Correlation-ID header
2. **Logging** - Log incoming requests
3. **Rate Limiting** - Check request quota
4. **Authentication** - Validate JWT token
5. **Authorization** - Check user roles
6. **Request Routing** - Forward to backend service
7. **Response** - Return to client

## Headers

Request headers passed through:
- `Authorization: Bearer {token}`
- `X-Correlation-ID: {id}`
- `X-API-Version: 1.0`

## Error Responses

```json
{
  "success": false,
  "message": "Error message",
  "errors": {
    "field": ["error details"]
  },
  "traceId": "..."
}
```

## Rate Limiting

- **Default**: 100 requests/minute per user
- **Burst**: Up to 150 requests
- **Response**: 429 Too Many Requests with `Retry-After` header
