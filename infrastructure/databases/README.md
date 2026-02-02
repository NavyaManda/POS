# Database Setup Guide

This directory contains individual SQL scripts for each microservice database in the POS system.

## Database Files Organization

Each service has its own dedicated SQL file for initialization:

- **01-auth-service.sql** - Auth Service (POSAuthDb)
- **02-menu-service.sql** - Menu Service (POSMenuDb)
- **03-order-service.sql** - Order Service (POSOrderDb)
- **04-payment-service.sql** - Payment Service (POSPaymentDb)
- **05-inventory-service.sql** - Inventory Service (POSInventoryDb)
- **06-kds-service.sql** - Kitchen Display Service (POSKDSDb)
- **07-loyalty-service.sql** - Loyalty Service (POSLoyaltyDb)
- **08-notification-service.sql** - Notification Service (POSNotificationDb)

## How to Run

### Option 1: Run All Scripts (Recommended)

Using SQL Server Management Studio (SSMS):

```bash
# Connect to your SQL Server instance
# Open Query Window
# Execute all scripts in order:
```

```sql
:r C:\path\to\01-auth-service.sql
:r C:\path\to\02-menu-service.sql
:r C:\path\to\03-order-service.sql
:r C:\path\to\04-payment-service.sql
:r C:\path\to\05-inventory-service.sql
:r C:\path\to\06-kds-service.sql
:r C:\path\to\07-loyalty-service.sql
:r C:\path\to\08-notification-service.sql
```

### Option 2: Run Individual Scripts

Execute each script separately based on your deployment needs:

```bash
# Using sqlcmd (SQL Server Command Line Tool)
sqlcmd -S localhost\SQLEXPRESS -U sa -P YourPassword -i "01-auth-service.sql"
sqlcmd -S localhost\SQLEXPRESS -U sa -P YourPassword -i "02-menu-service.sql"
# ... continue for other services
```

### Option 3: With Docker Compose

The docker-compose.yml will automatically run the migration scripts during startup:

```bash
docker-compose up -d mssql
# Wait for SQL Server to be healthy
sleep 30
# Run initialization scripts
docker exec -it pos_mssql /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'PosSystem@123' -i /scripts/01-auth-service.sql
```

## Database Structure Overview

### Auth Service (POSAuthDb)
- **Users** - User accounts with authentication credentials
- **Roles** - Available roles (admin, staff, kitchen, customer)
- **UserRoles** - Many-to-many mapping of users to roles
- **RefreshTokens** - JWT refresh token management

### Menu Service (POSMenuDb)
- **Categories** - Menu categories (appetizers, mains, desserts, etc.)
- **MenuItems** - Individual menu items with pricing and availability
- **MenuItemVariants** - Item variants (size, spice level, etc.)
- **MenuItemPrices** - Price history for audit and pricing changes

### Order Service (POSOrderDb)
- **Orders** - Main order records
- **OrderItems** - Line items in each order
- **OrderStatusHistory** - Audit trail of status changes
- **OrderModifications** - Track order changes

### Payment Service (POSPaymentDb)
- **Payments** - Payment transaction records
- **Refunds** - Refund transaction records
- **PaymentMethods** - Supported payment methods
- **IdempotencyKeys** - Prevent duplicate charge attempts

### Inventory Service (POSInventoryDb)
- **Ingredients** - Available ingredients/stock items
- **StockLevels** - Current inventory quantities
- **StockTransactions** - Transaction log (stock-in, stock-out, adjustments)
- **ReorderPoints** - Minimum levels and reorder quantities
- **Suppliers** - Supplier information
- **IngredientSuppliers** - Supplier pricing and lead times

### Kitchen Display Service (POSKDSDb)
- **KDSOrders** - Orders in kitchen display system
- **KDSOrderItems** - Items within KDS orders
- **Stations** - Kitchen stations (grill, fryer, prep, etc.)
- **StationAssignments** - Order assignments to stations
- **KDSMetrics** - Performance metrics (prep time, on-time delivery)

### Loyalty Service (POSLoyaltyDb)
- **MembershipTiers** - Loyalty program tiers (Bronze, Silver, Gold, Platinum)
- **LoyaltyAccounts** - Customer loyalty accounts
- **PointsTransactions** - Points earned/redeemed
- **Rewards** - Available rewards catalog
- **RewardRedemptions** - Customer reward redemptions
- **Promotions** - Promotional campaigns

### Notification Service (POSNotificationDb)
- **NotificationChannels** - Communication channels (email, SMS, push, in-app)
- **NotificationTemplates** - Message templates
- **NotificationPreferences** - User notification preferences
- **NotificationLogs** - Delivery logs
- **NotificationQueue** - Async notification processing queue

## Key Features

✅ **Idempotent Scripts**: All CREATE statements use `IF NOT EXISTS` to prevent errors on re-runs

✅ **Proper Indexes**: Strategic indexes created for performance optimization

✅ **Foreign Key Constraints**: Referential integrity with cascading deletes where appropriate

✅ **Audit Fields**: CreatedAt, UpdatedAt timestamps on most tables

✅ **Seed Data**: Default roles, stations, and membership tiers pre-populated

✅ **Service Isolation**: Each service database is completely independent

## Connection Strings

After running these scripts, use these connection strings in your services:

```
POSAuthDb: Server=localhost;Database=POSAuthDb;User Id=sa;Password=YourPassword;
POSMenuDb: Server=localhost;Database=POSMenuDb;User Id=sa;Password=YourPassword;
POSOrderDb: Server=localhost;Database=POSOrderDb;User Id=sa;Password=YourPassword;
POSPaymentDb: Server=localhost;Database=POSPaymentDb;User Id=sa;Password=YourPassword;
POSInventoryDb: Server=localhost;Database=POSInventoryDb;User Id=sa;Password=YourPassword;
POSKDSDb: Server=localhost;Database=POSKDSDb;User Id=sa;Password=YourPassword;
POSLoyaltyDb: Server=localhost;Database=POSLoyaltyDb;User Id=sa;Password=YourPassword;
POSNotificationDb: Server=localhost;Database=POSNotificationDb;User Id=sa;Password=YourPassword;
```

## Default Seeded Data

### Roles (POSAuthDb)
- **admin** - Administrator with full access
- **staff** - Restaurant staff member
- **kitchen** - Kitchen staff member
- **customer** - Regular customer

### Stations (POSKDSDb)
- GRILL
- FRYER
- PREP
- SALAD
- DRINKS
- PASTRY

### Membership Tiers (POSLoyaltyDb)
- **BRONZE** - 0 points (1x multiplier, 0% discount)
- **SILVER** - 500 points (1.5x multiplier, 5% discount)
- **GOLD** - 1500 points (2x multiplier, 10% discount)
- **PLATINUM** - 3000 points (3x multiplier, 15% discount)

### Notification Channels (POSNotificationDb)
- EMAIL
- SMS
- PUSH
- IN_APP

## Troubleshooting

### SQL Server Connection Issues
```bash
# Test connection
sqlcmd -S localhost -U sa -P 'YourPassword' -Q "SELECT @@VERSION"
```

### Database Already Exists
The scripts use `IF NOT EXISTS` to prevent errors if databases already exist. To reset:
```sql
DROP DATABASE POSAuthDb;
DROP DATABASE POSMenuDb;
-- ... etc for all services
-- Then re-run the scripts
```

### Permission Issues
Ensure the SQL Server user has permissions to:
- Create databases
- Create tables
- Create indexes
- Insert seed data

## Migration Strategy

For future schema changes:
1. Create new numbered migration files (09-*, 10-*, etc.)
2. Use `ALTER TABLE` statements with `IF NOT EXISTS` checks
3. Maintain backward compatibility
4. Document breaking changes

## Maintenance

### Backup
```sql
BACKUP DATABASE POSAuthDb 
TO DISK = 'C:\backup\POSAuthDb.bak'
```

### Statistics Update
```sql
USE POSAuthDb;
UPDATE STATISTICS;
```

### Index Maintenance
```sql
-- Rebuild fragmented indexes
ALTER INDEX ALL ON [Users] REBUILD;
ALTER INDEX ALL ON [Orders] REBUILD;
```
