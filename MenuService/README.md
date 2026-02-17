# MenuService API

Restaurant menu management microservice for the POS system.

## Features

- Menu item management (CRUD operations)
- Category management
- Menu filtering by category
- Item attributes (vegetarian, spicy, calories)
- SQLite database with seeded data

## API Endpoints

### Menu Items
- `GET /api/v1/menu/items` - Get all menu items
- `GET /api/v1/menu/items/{id}` - Get specific menu item
- `GET /api/v1/menu/category/{categoryId}` - Get items by category
- `POST /api/v1/menu/items` - Create menu item
- `PUT /api/v1/menu/items/{id}` - Update menu item
- `DELETE /api/v1/menu/items/{id}` - Delete menu item

### Categories
- `GET /api/v1/menu/categories` - Get all categories
- `GET /api/v1/menu/categories/{id}` - Get specific category
- `POST /api/v1/menu/categories` - Create category
- `PUT /api/v1/menu/categories/{id}` - Update category
- `DELETE /api/v1/menu/categories/{id}` - Delete category

### Health
- `GET /api/v1/menu/health` - Service health check

## Building & Running

### Local Development
```bash
cd MenuService/src/MenuService.API
dotnet build
dotnet run --urls http://localhost:5002
```

### Docker
```bash
docker build -f MenuService/Dockerfile -t menuservice:latest .
docker run -p 5002:5002 menuservice:latest
```

## Database

SQLite database is automatically created on first run with:
- 4 default categories (Appetizers, Main Course, Desserts, Beverages)
- 4 sample menu items
- Proper indexing and foreign keys

## Default Menu Items

1. **Caesar Salad** - $8.99 (Appetizers, Vegetarian)
2. **Grilled Chicken Breast** - $15.99 (Main Course)
3. **Spicy Thai Curry** - $14.99 (Main Course, Spicy)
4. **Chocolate Cake** - $6.99 (Desserts, Vegetarian)

Manages restaurant menu items, categories, variants, and pricing.

## Project Structure

```
MenuService/
├── src/
│   └── MenuService.API/
│       ├── Models/            # Category, MenuItem, Variant models
│       ├── Services/          # MenuService, CacheService
│       ├── Controllers/       # MenuController
│       ├── Properties/
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       └── MenuService.API.csproj
├── Dockerfile
└── README.md
```

## Build & Run

### Local Development

```bash
cd MenuService/src/MenuService.API
dotnet restore
dotnet build
dotnet watch run
```

Service runs on `http://localhost:5002`

### Docker

```bash
docker build -t menu-service:1.0 .
docker run -p 5002:80 menu-service:1.0
```

## API Endpoints

### Get All Categories
```
GET /api/v1/categories
```

Response:
```json
{
  "success": true,
  "data": [
    {
      "id": "uuid",
      "name": "Appetizers",
      "displayOrder": 1,
      "isActive": true
    }
  ]
}
```

### Get Menu Items
```
GET /api/v1/menu-items?categoryId={categoryId}&isActive=true
```

### Get Item Details
```
GET /api/v1/menu-items/{itemId}
```

Response:
```json
{
  "success": true,
  "data": {
    "id": "uuid",
    "name": "Caesar Salad",
    "description": "Fresh romaine...",
    "categoryId": "uuid",
    "basePrice": 12.99,
    "variants": [
      {
        "id": "uuid",
        "name": "Large",
        "priceModifier": 2.00
      }
    ],
    "available": true,
    "preparationTime": 10
  }
}
```

### Add Menu Item (Admin)
```
POST /api/v1/menu-items
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "Caesar Salad",
  "description": "...",
  "categoryId": "uuid",
  "basePrice": 12.99,
  "preparationTime": 10
}
```

### Update Item Availability
```
PATCH /api/v1/menu-items/{itemId}/availability
Authorization: Bearer {token}

{
  "available": false
}
```

## Events Published

### menu.item.created.v1
```json
{
  "eventType": "menu.item.created",
  "version": 1,
  "timestamp": "2024-01-01T10:00:00Z",
  "itemId": "uuid",
  "name": "Caesar Salad",
  "basePrice": 12.99
}
```

### menu.item.updated.v1
```json
{
  "eventType": "menu.item.updated",
  "version": 1,
  "timestamp": "2024-01-01T10:00:00Z",
  "itemId": "uuid",
  "name": "Caesar Salad",
  "basePrice": 12.99
}
```

### menu.item.availability.changed.v1
```json
{
  "eventType": "menu.item.availability.changed",
  "version": 1,
  "timestamp": "2024-01-01T10:00:00Z",
  "itemId": "uuid",
  "available": false
}
```

## Events Consumed

### inventory.stock.depleted.v1
- When inventory service signals stock is low
- Action: Mark item as unavailable or reduce availability

## Dependencies

- **AuthService**: JWT token validation
- **InventoryService**: Stock availability checks
- **RabbitMQ**: Event messaging

## Caching Strategy

Menu items cached in Redis:
- TTL: 30 minutes
- Invalidated on create/update
- Cache key: `menu:items:{categoryId}`, `menu:item:{itemId}`

## Configuration

```json
{
  "Database": {
    "ConnectionString": "Server=...;Database=POSMenuDb;..."
  },
  "ServiceEndpoints": {
    "AuthService": "http://auth-service:80",
    "InventoryService": "http://inventory-service:80"
  }
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
  "cache": "connected"
}
```
