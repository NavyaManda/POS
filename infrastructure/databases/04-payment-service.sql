-- Payment Service Database Setup
-- Creates POSPaymentDb with Payments and Refunds tables

USE master;
GO

-- Create Payment Service Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'POSPaymentDb')
BEGIN
    CREATE DATABASE POSPaymentDb;
END
GO

USE POSPaymentDb;
GO

-- Payments Table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Payments')
BEGIN
    CREATE TABLE [Payments] (
        [PaymentId] NVARCHAR(36) PRIMARY KEY,
        [TransactionId] NVARCHAR(36) NOT NULL UNIQUE,
        [OrderId] NVARCHAR(36) NOT NULL,
        [Amount] DECIMAL(10, 2) NOT NULL,
        [Method] NVARCHAR(50) NOT NULL,
        [Status] NVARCHAR(50) NOT NULL,
        [GatewayReference] NVARCHAR(255),
        [GatewayMessage] NVARCHAR(MAX),
        [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
        [CompletedAt] DATETIME2 NULL
    );
    
    CREATE INDEX IX_Payments_OrderId ON [Payments]([OrderId]);
    CREATE INDEX IX_Payments_TransactionId ON [Payments]([TransactionId]);
    CREATE INDEX IX_Payments_Status ON [Payments]([Status]);
    CREATE INDEX IX_Payments_CreatedAt ON [Payments]([CreatedAt]);
END
GO

-- Refunds Table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Refunds')
BEGIN
    CREATE TABLE [Refunds] (
        [RefundId] NVARCHAR(36) PRIMARY KEY,
        [PaymentId] NVARCHAR(36) NOT NULL,
        [Amount] DECIMAL(10, 2) NOT NULL,
        [Reason] NVARCHAR(255),
        [Status] NVARCHAR(50) NOT NULL,
        [GatewayReference] NVARCHAR(255),
        [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
        [CompletedAt] DATETIME2 NULL,
        FOREIGN KEY ([PaymentId]) REFERENCES [Payments]([PaymentId]) ON DELETE CASCADE
    );
    
    CREATE INDEX IX_Refunds_PaymentId ON [Refunds]([PaymentId]);
    CREATE INDEX IX_Refunds_Status ON [Refunds]([Status]);
END
GO

-- PaymentMethods Table (for storing payment method details)
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'PaymentMethods')
BEGIN
    CREATE TABLE [PaymentMethods] (
        [MethodId] NVARCHAR(36) PRIMARY KEY,
        [MethodType] NVARCHAR(50) NOT NULL,
        [MethodName] NVARCHAR(255),
        [IsActive] BIT DEFAULT 1,
        [CreatedAt] DATETIME2 DEFAULT GETUTCDATE()
    );
END
GO

-- IdempotencyKeys Table (for preventing duplicate charges)
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'IdempotencyKeys')
BEGIN
    CREATE TABLE [IdempotencyKeys] (
        [KeyId] NVARCHAR(36) PRIMARY KEY,
        [IdempotencyKey] NVARCHAR(255) NOT NULL UNIQUE,
        [PaymentId] NVARCHAR(36),
        [Response] NVARCHAR(MAX),
        [ExpiresAt] DATETIME2,
        [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
        FOREIGN KEY ([PaymentId]) REFERENCES [Payments]([PaymentId])
    );
    
    CREATE INDEX IX_IdempotencyKeys_Key ON [IdempotencyKeys]([IdempotencyKey]);
    CREATE INDEX IX_IdempotencyKeys_ExpiresAt ON [IdempotencyKeys]([ExpiresAt]);
END
GO

PRINT 'Payment Service database setup completed successfully.'
