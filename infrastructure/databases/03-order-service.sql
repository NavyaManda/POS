-- Order Service Database Setup
-- Creates POSOrderDb with Orders and OrderItems tables

USE master;
GO

-- Create Order Service Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'POSOrderDb')
BEGIN
    CREATE DATABASE POSOrderDb;
END
GO

USE POSOrderDb;
GO

-- Orders Table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Orders')
BEGIN
    CREATE TABLE [Orders] (
        [OrderId] NVARCHAR(36) PRIMARY KEY,
        [OrderNumber] NVARCHAR(50) NOT NULL UNIQUE,
        [CustomerId] NVARCHAR(36) NOT NULL,
        [TotalAmount] DECIMAL(10, 2) NOT NULL,
        [Status] NVARCHAR(50) NOT NULL,
        [Notes] NVARCHAR(MAX),
        [PaymentId] NVARCHAR(36),
        [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
        [ConfirmedAt] DATETIME2 NULL,
        [ReadyAt] DATETIME2 NULL,
        [CompletedAt] DATETIME2 NULL,
        [CancelledAt] DATETIME2 NULL
    );
    
    CREATE INDEX IX_Orders_CustomerId ON [Orders]([CustomerId]);
    CREATE INDEX IX_Orders_Status ON [Orders]([Status]);
    CREATE INDEX IX_Orders_CreatedAt ON [Orders]([CreatedAt]);
    CREATE INDEX IX_Orders_OrderNumber ON [Orders]([OrderNumber]);
END
GO

-- OrderItems Table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'OrderItems')
BEGIN
    CREATE TABLE [OrderItems] (
        [ItemId] NVARCHAR(36) PRIMARY KEY,
        [OrderId] NVARCHAR(36) NOT NULL,
        [MenuItemId] NVARCHAR(36) NOT NULL,
        [MenuItemName] NVARCHAR(255) NOT NULL,
        [Quantity] INT NOT NULL,
        [UnitPrice] DECIMAL(10, 2) NOT NULL,
        [SpecialInstructions] NVARCHAR(MAX),
        [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
        FOREIGN KEY ([OrderId]) REFERENCES [Orders]([OrderId]) ON DELETE CASCADE
    );
    
    CREATE INDEX IX_OrderItems_OrderId ON [OrderItems]([OrderId]);
    CREATE INDEX IX_OrderItems_MenuItemId ON [OrderItems]([MenuItemId]);
END
GO

-- OrderStatusHistory Table (for audit trail)
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'OrderStatusHistory')
BEGIN
    CREATE TABLE [OrderStatusHistory] (
        [HistoryId] NVARCHAR(36) PRIMARY KEY,
        [OrderId] NVARCHAR(36) NOT NULL,
        [PreviousStatus] NVARCHAR(50),
        [NewStatus] NVARCHAR(50) NOT NULL,
        [ChangedAt] DATETIME2 DEFAULT GETUTCDATE(),
        [ChangedBy] NVARCHAR(255),
        FOREIGN KEY ([OrderId]) REFERENCES [Orders]([OrderId]) ON DELETE CASCADE
    );
    
    CREATE INDEX IX_OrderStatusHistory_OrderId ON [OrderStatusHistory]([OrderId]);
    CREATE INDEX IX_OrderStatusHistory_ChangedAt ON [OrderStatusHistory]([ChangedAt]);
END
GO

-- OrderModifications Table (for tracking changes)
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'OrderModifications')
BEGIN
    CREATE TABLE [OrderModifications] (
        [ModificationId] NVARCHAR(36) PRIMARY KEY,
        [OrderId] NVARCHAR(36) NOT NULL,
        [ModificationType] NVARCHAR(50) NOT NULL,
        [OldValue] NVARCHAR(MAX),
        [NewValue] NVARCHAR(MAX),
        [ModifiedAt] DATETIME2 DEFAULT GETUTCDATE(),
        [ModifiedBy] NVARCHAR(255),
        FOREIGN KEY ([OrderId]) REFERENCES [Orders]([OrderId]) ON DELETE CASCADE
    );
    
    CREATE INDEX IX_OrderModifications_OrderId ON [OrderModifications]([OrderId]);
END
GO

PRINT 'Order Service database setup completed successfully.'
