# Loyalty Service

Manages customer loyalty accounts, points, rewards, and promotions.

## Project Structure

```
LoyaltyService/
├── src/
│   └── LoyaltyService.API/
│       ├── Models/            # LoyaltyAccount, Reward, Promotion models
│       ├── Services/          # LoyaltyService, RewardService
│       ├── Controllers/       # LoyaltyController
│       ├── Properties/
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       └── LoyaltyService.API.csproj
├── Dockerfile
└── README.md
```

## Build & Run

### Local Development

```bash
cd LoyaltyService/src/LoyaltyService.API
dotnet restore
dotnet build
dotnet watch run
```

Service runs on `http://localhost:5007`

### Docker

```bash
docker build -t loyalty-service:1.0 .
docker run -p 5007:80 loyalty-service:1.0
```

## API Endpoints

### Get Loyalty Account
```
GET /api/v1/loyalty/account/{customerId}
Authorization: Bearer {token}
```

Response:
```json
{
  "success": true,
  "data": {
    "accountId": "uuid",
    "customerId": "uuid",
    "tier": "Gold",
    "points": 2500,
    "lifetimeSpend": 5000.00,
    "enrollmentDate": "2023-01-01T00:00:00Z",
    "nextTierThreshold": 3000,
    "rewards": [
      {
        "rewardId": "uuid",
        "name": "$20 Off",
        "pointsRequired": 2000,
        "available": true
      }
    ]
  }
}
```

### Apply Reward
```
POST /api/v1/loyalty/rewards/{rewardId}/apply
Authorization: Bearer {token}

{
  "orderId": "uuid"
}
```

Response:
```json
{
  "success": true,
  "message": "Reward applied successfully",
  "data": {
    "discountAmount": 20.00,
    "pointsRemaining": 500,
    "orderId": "uuid"
  }
}
```

### Get Points History
```
GET /api/v1/loyalty/account/{customerId}/points-history?limit=50
```

### Earn Points (Internal)
```
POST /api/v1/loyalty/points/earn
X-Internal-Service: true

{
  "customerId": "uuid",
  "orderId": "uuid",
  "amount": 25.00,
  "pointsEarned": 25
}
```

### Get Promotions
```
GET /api/v1/loyalty/promotions?active=true
```

Response:
```json
{
  "success": true,
  "data": [
    {
      "promotionId": "uuid",
      "name": "Double Points Weekend",
      "description": "Earn 2x points on all purchases",
      "validFrom": "2024-01-01T00:00:00Z",
      "validUntil": "2024-01-08T23:59:59Z",
      "multiplier": 2.0,
      "active": true
    }
  ]
}
```

## Tier System

| Tier | Lifetime Spend | Benefits | Next Tier |
|------|---|---|---|
| Bronze | $0 - $999 | 1 point per $1 | $1000 |
| Silver | $1000 - $2999 | 1.25 points per $1 | $3000 |
| Gold | $3000 - $9999 | 1.5 points per $1 | $10000 |
| Platinum | $10000+ | 2 points per $1, Birthday bonus | Highest |

## Events Published

### loyalty.points.earned.v1
```json
{
  "eventType": "loyalty.points.earned",
  "version": 1,
  "timestamp": "2024-01-01T10:00:00Z",
  "customerId": "uuid",
  "points": 25,
  "orderId": "uuid"
}
```

### loyalty.reward.redeemed.v1
```json
{
  "eventType": "loyalty.reward.redeemed",
  "version": 1,
  "timestamp": "2024-01-01T10:00:00Z",
  "customerId": "uuid",
  "rewardId": "uuid",
  "discount": 20.00,
  "orderId": "uuid"
}
```

### loyalty.tier.upgraded.v1
```json
{
  "eventType": "loyalty.tier.upgraded",
  "version": 1,
  "timestamp": "2024-01-01T10:00:00Z",
  "customerId": "uuid",
  "newTier": "Gold",
  "previousTier": "Silver"
}
```

## Events Consumed

### order.completed.v1
- Calculate points earned based on total amount
- Apply active promotions (2x points, etc.)
- Check tier upgrade eligibility

### payment.completed.v1
- Confirm order completion and point earning

## Configuration

```json
{
  "Database": {
    "ConnectionString": "..."
  },
  "PointsRules": {
    "basePointsPerDollar": 1.0,
    "tierMultipliers": {
      "bronze": 1.0,
      "silver": 1.25,
      "gold": 1.5,
      "platinum": 2.0
    }
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
  "totalMembers": 5240
}
```
