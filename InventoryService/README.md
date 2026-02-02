# Inventory Service

Manages restaurant inventory, ingredients, stock levels, suppliers, and reorder points.

## Project Structure

```
InventoryService/
├── src/
│   └── InventoryService.API/
│       ├── Models/            # Ingredient, StockLevel, Supplier models
│       ├── Services/          # InventoryService, StockAlertService
│       ├── Controllers/       # InventoryController
│       ├── Properties/
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       └── InventoryService.API.csproj
├── Dockerfile
└── README.md
```

## Build & Run

### Local Development

```bash
cd InventoryService/src/InventoryService.API
dotnet restore
dotnet build
dotnet watch run
```

Service runs on `http://localhost:5003`

### Docker

```bash
docker build -t inventory-service:1.0 .
docker run -p 5003:80 inventory-service:1.0
```

## API Endpoints

### Get Ingredient Stock
```
GET /api/v1/ingredients/{ingredientId}/stock
```

Response:
```json
{
  "success": true,
  "data": {
    "ingredientId": "uuid",
    "name": "Tomato",
    "currentStock": 50.0,
    "unit": "kg",
    "reorderPoint": 20.0,
    "reorderQuantity": 100.0,
    "status": "healthy"
  }
}
```

### Check Availability
```
POST /api/v1/inventory/check-availability
Content-Type: application/json

{
  "items": [
    { "ingredientId": "uuid", "quantity": 5.0 }
  ]
}
```

Response:
```json
{
  "success": true,
  "data": {
    "allAvailable": true,
    "items": [
      {
        "ingredientId": "uuid",
        "requested": 5.0,
        "available": 50.0,
        "sufficient": true
      }
    ]
  }
}
```

### Deduct Stock
```
POST /api/v1/inventory/deduct-stock
Authorization: Bearer {token}
Content-Type: application/json

{
  "orderId": "uuid",
  "items": [
    { "ingredientId": "uuid", "quantity": 5.0 }
  ]
}
```

### Get Stock History
```
GET /api/v1/ingredients/{ingredientId}/history?days=30
```

### Update Reorder Point (Admin)
```
PATCH /api/v1/ingredients/{ingredientId}/reorder-point
Authorization: Bearer {token}

{
  "reorderPoint": 25.0,
  "reorderQuantity": 120.0
}
```

## Events Published

### inventory.stock.deducted.v1
```json
{
  "eventType": "inventory.stock.deducted",
  "version": 1,
  "timestamp": "2024-01-01T10:00:00Z",
  "orderId": "uuid",
  "items": [
    { "ingredientId": "uuid", "quantity": 5.0 }
  ]
}
```

### inventory.stock.depleted.v1
```json
{
  "eventType": "inventory.stock.depleted",
  "version": 1,
  "timestamp": "2024-01-01T10:00:00Z",
  "ingredientId": "uuid",
  "name": "Tomato",
  "currentStock": 2.0
}
```

### inventory.reorder.required.v1
```json
{
  "eventType": "inventory.reorder.required",
  "version": 1,
  "timestamp": "2024-01-01T10:00:00Z",
  "ingredientId": "uuid",
  "name": "Tomato",
  "currentStock": 15.0,
  "reorderQuantity": 100.0,
  "supplierId": "uuid"
}
```

## Events Consumed

### order.created.v1
- Check ingredient availability

### order.cancelled.v1
- Restore stock for cancelled orders

## Dependencies

- **AuthService**: JWT token validation
- **MenuService**: Menu item to ingredient mapping
- **RabbitMQ**: Event messaging

## Configuration

```json
{
  "Database": {
    "ConnectionString": "Server=...;Database=POSInventoryDb;..."
  }
}
```

## Low Stock Alerts

Service automatically publishes `inventory.stock.depleted` event when:
- Current stock falls below reorder point
- Action: Notify procurement, display unavailable flag

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
  "lowStockAlerts": 3
}
```
