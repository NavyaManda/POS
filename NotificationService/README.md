# Notification Service

Handles multi-channel notifications: email, SMS, push notifications, and in-app notifications.

## Project Structure

```
NotificationService/
├── src/
│   └── NotificationService.API/
│       ├── Models/            # Notification, Template, Channel models
│       ├── Services/          # EmailService, SMSService, NotificationService
│       ├── Controllers/       # NotificationController
│       ├── Properties/
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       └── NotificationService.API.csproj
├── Dockerfile
└── README.md
```

## Build & Run

### Local Development

```bash
cd NotificationService/src/NotificationService.API
dotnet restore
dotnet build
dotnet watch run
```

Service runs on `http://localhost:5008`

### Docker

```bash
docker build -t notification-service:1.0 .
docker run -p 5008:80 notification-service:1.0
```

## API Endpoints

### Send Notification (Internal)
```
POST /api/v1/notifications/send
X-Internal-Service: true
Content-Type: application/json

{
  "customerId": "uuid",
  "channels": ["email", "sms"],
  "templateId": "order-ready",
  "variables": {
    "orderNumber": "1001",
    "readyTime": "2024-01-01T10:15:00Z"
  }
}
```

Response:
```json
{
  "success": true,
  "message": "Notification queued",
  "data": {
    "notificationId": "uuid",
    "channels": ["email", "sms"],
    "status": "queued"
  }
}
```

### Get Notification Templates
```
GET /api/v1/notifications/templates
```

Response:
```json
{
  "success": true,
  "data": [
    {
      "templateId": "order-ready",
      "name": "Order Ready",
      "subject": "Your order {{orderNumber}} is ready!",
      "emailBody": "Your order {{orderNumber}} is ready for pickup...",
      "smsBody": "Your order {{orderNumber}} is ready! Pickup time: {{readyTime}}",
      "variables": ["orderNumber", "readyTime"]
    }
  ]
}
```

### Get Notification History
```
GET /api/v1/notifications/history/{customerId}?limit=50
```

### Update Notification Preferences
```
PATCH /api/v1/notifications/preferences/{customerId}
Authorization: Bearer {token}

{
  "emailNotifications": true,
  "smsNotifications": false,
  "pushNotifications": true,
  "unsubscribeFromMarketing": false
}
```

## Notification Types

### Order Notifications
- **order.created** - Confirmation of order placement
- **order.confirmed** - Restaurant confirmed order
- **order.preparing** - Order is being prepared
- **order.ready** - Order ready for pickup/delivery
- **order.cancelled** - Order was cancelled

### Payment Notifications
- **payment.received** - Payment confirmation
- **payment.failed** - Payment processing failed
- **refund.issued** - Refund confirmation

### Loyalty Notifications
- **loyalty.points.earned** - Points earned for order
- **loyalty.tier.upgraded** - Customer reached new tier
- **loyalty.reward.available** - New reward unlocked

### System Notifications
- **system.alert** - System alerts and issues
- **maintenance.scheduled** - Scheduled maintenance notice

## Event Consumption

Service subscribes to and processes:
- `order.*` events (created, confirmed, ready, cancelled)
- `payment.*` events (completed, failed, refunded)
- `loyalty.*` events (points earned, tier upgraded)
- `inventory.*` events (stock depleted, reorder needed)

## Channels

### Email
- Provider: SendGrid
- Features: HTML templates, attachments, tracking
- Rate limit: 100 emails/minute

### SMS
- Provider: Twilio
- Features: Personalized messages, opt-in tracking
- Rate limit: 50 SMS/minute

### Push Notifications
- Via Firebase Cloud Messaging
- Sent to mobile apps
- Real-time order updates

### In-App
- Stored in database
- Real-time via WebSocket
- User-dismissed notifications

## Configuration

```json
{
  "SendGrid": {
    "ApiKey": "SG.xxx",
    "FromEmail": "noreply@pos.local"
  },
  "Twilio": {
    "AccountSid": "AC...",
    "AuthToken": "...",
    "FromNumber": "+1234567890"
  }
}
```

## Message Queue

- Failed notifications queued for retry
- Retry logic: 3 attempts, exponential backoff
- Max queue size: 10,000 messages

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
  "sendGrid": "connected",
  "twilio": "connected",
  "pendingNotifications": 23
}
```

## Rate Limiting

- Per customer: 10 notifications per hour
- Exception: Critical alerts (order ready, payment failure)
- Batch similar notifications within 5-minute window
