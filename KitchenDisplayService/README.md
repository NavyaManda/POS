# Kitchen Display System (KDS)

Real-time order management and display for kitchen staff. Handles order routing to cooking stations and progress tracking.

## Project Structure

```
KitchenDisplayService/
├── src/
│   └── KitchenDisplayService.API/
│       ├── Models/            # KDSOrder, Station, Metrics models
│       ├── Services/          # KDSService, StationService, WebSocket handler
│       ├── Controllers/       # KDSController
│       ├── Properties/
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       └── KitchenDisplayService.API.csproj
├── Dockerfile
└── README.md
```

## Build & Run

### Local Development

```bash
cd KitchenDisplayService/src/KitchenDisplayService.API
dotnet restore
dotnet build
dotnet watch run
```

Service runs on `http://localhost:5006`

### Docker

```bash
docker build -t kds-service:1.0 .
docker run -p 5006:80 kds-service:1.0
```

## API Endpoints

### Get Active Orders
```
GET /api/v1/orders?status=pending&station={stationId}
```

Response:
```json
{
  "success": true,
  "data": [
    {
      "orderId": "uuid",
      "orderNumber": 1001,
      "assignedStation": "Grill",
      "items": [
        {
          "itemId": "uuid",
          "name": "Grilled Steak",
          "quantity": 2,
          "specialInstructions": "Medium rare"
        }
      ],
      "status": "in_preparation",
      "receivedTime": "2024-01-01T10:00:00Z",
      "estimatedCompletionTime": "2024-01-01T10:15:00Z"
    }
  ]
}
```

### Mark Order as Complete
```
PATCH /api/v1/orders/{orderId}/complete
Authorization: Bearer {token}
```

### Get Station Performance
```
GET /api/v1/stations/{stationId}/metrics?timeRange=shift
```

Response:
```json
{
  "success": true,
  "data": {
    "stationId": "uuid",
    "stationName": "Grill",
    "ordersCompleted": 45,
    "averagePreparationTime": 12.5,
    "peakLoadTime": "2024-01-01T12:30:00Z",
    "efficiency": 94.5
  }
}
```

### Get all Orders (Active + Completed)
```
GET /api/v1/orders/history?limit=50&offset=0
```

## WebSocket Connection

Kitchen displays connect via WebSocket for real-time updates:

```
ws://localhost:5006/ws/kitchen?stationId={stationId}&token={token}
```

**Events Sent:**
- `order.new` - New order assigned to station
- `order.updated` - Order status changed
- `order.completed` - Order marked ready for service
- `order.cancelled` - Order was cancelled
- `alert.urgent` - High priority order alert

## Events Published

### kds.order.started.v1
```json
{
  "eventType": "kds.order.started",
  "version": 1,
  "timestamp": "2024-01-01T10:00:00Z",
  "orderId": "uuid",
  "stationId": "uuid"
}
```

### kds.order.completed.v1
```json
{
  "eventType": "kds.order.completed",
  "version": 1,
  "timestamp": "2024-01-01T10:15:00Z",
  "orderId": "uuid",
  "stationId": "uuid",
  "preparationTime": 15
}
```

## Events Consumed

### order.created.v1
- Route order items to appropriate stations
- Update display in real-time

### order.cancelled.v1
- Remove order from display
- Update station metrics

### order.modified.v1
- Update order items on display

## Station Types

- **Grill**: Steaks, burgers, hot items
- **Prep**: Salads, cold items, appetizers
- **Fryer**: Fried items
- **Bakery**: Baked goods
- **Expediting**: Final plating and QC

## Performance Metrics

Tracks per station:
- Orders prepared
- Average preparation time
- Peak load times
- Efficiency percentage
- Alerts and exceptions

## Configuration

```json
{
  "Database": {
    "ConnectionString": "..."
  },
  "Stations": [
    { "id": "grill", "name": "Grill", "capacity": 20 },
    { "id": "prep", "name": "Prep", "capacity": 15 }
  ]
}
```

## Health Check

```
GET /health
```

Response:
```json
{
  "status": "healthy",
  "timestamp": "2024-01-01T10:00:00Z",
  "database": "connected",
  "activeConnections": 5,
  "ordersInProgress": 23
}
```
