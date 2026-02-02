# PaymentService - Payment Processing

Standalone microservice for payment processing, transactions, and refunds.

## Project Structure

```
PaymentService/
├── src/
│   └── PaymentService.API/
│       ├── Controllers/        # API endpoints
│       ├── Services/          # Business logic
│       ├── Models/            # Data models & DTOs
│       ├── Repositories/      # Data access layer
│       ├── Properties/
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       └── PaymentService.API.csproj
├── Dockerfile
└── README.md
```

## Build & Run

### Local Development

```bash
cd PaymentService/src/PaymentService.API
dotnet restore
dotnet build
dotnet watch run
```

Service runs on `http://localhost:5005`

### Docker

```bash
docker build -t payment-service:1.0 .
docker run -p 5005:80 \
  -e "PaymentGateway:StripeApiKey=sk_test_..." \
  payment-service:1.0
```

## Endpoints

- **POST** `/api/v1/payments/process` - Process payment
- **GET** `/api/v1/payments/{id}` - Get payment details
- **POST** `/api/v1/payments/{id}/refund` - Process refund
- **GET** `/api/v1/payments/health` - Health check

## Configuration

Set payment gateway keys in `appsettings.json`:

```json
{
  "PaymentGateway": {
    "StripeApiKey": "sk_test_...",
    "StripeSecretKey": "rk_test_..."
  }
}
```

## Events Published

- `payment.initiated.v1`
- `payment.completed.v1`
- `payment.failed.v1`
- `payment.refund.v1`

## Events Consumed

- `order.created`
- `order.cancelled`

## PCI Compliance

- No credit card data stored
- Uses payment gateway tokens
- Idempotency keys for duplicate prevention
