-- Kitchen Display Service Database Setup
-- Creates POSKDSDb with KDSOrders and related tables

USE master;
GO

-- Create Kitchen Display Service Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'POSKDSDb')
BEGIN
    CREATE DATABASE POSKDSDb;
END
GO

USE POSKDSDb;
GO

-- KDSOrders Table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'KDSOrders')
BEGIN
    CREATE TABLE [KDSOrders] (
        [KDSOrderId] NVARCHAR(36) PRIMARY KEY,
        [OrderId] NVARCHAR(36) NOT NULL UNIQUE,
        [OrderNumber] NVARCHAR(50),
        [AssignedStation] NVARCHAR(50),
        [Status] NVARCHAR(50) NOT NULL,
        [Priority] INT DEFAULT 0,
        [EstimatedMinutes] INT,
        [ReceivedAt] DATETIME2 DEFAULT GETUTCDATE(),
        [StartedAt] DATETIME2 NULL,
        [ReadyAt] DATETIME2 NULL,
        [ServedAt] DATETIME2 NULL
    );
    
    CREATE INDEX IX_KDSOrders_Status ON [KDSOrders]([Status]);
    CREATE INDEX IX_KDSOrders_Station ON [KDSOrders]([AssignedStation]);
    CREATE INDEX IX_KDSOrders_Priority ON [KDSOrders]([Priority]);
    CREATE INDEX IX_KDSOrders_ReceivedAt ON [KDSOrders]([ReceivedAt]);
END
GO

-- KDSOrderItems Table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'KDSOrderItems')
BEGIN
    CREATE TABLE [KDSOrderItems] (
        [KDSItemId] NVARCHAR(36) PRIMARY KEY,
        [KDSOrderId] NVARCHAR(36) NOT NULL,
        [MenuItemId] NVARCHAR(36) NOT NULL,
        [MenuItemName] NVARCHAR(255) NOT NULL,
        [Quantity] INT NOT NULL,
        [SpecialInstructions] NVARCHAR(MAX),
        [IsCompleted] BIT DEFAULT 0,
        [CompletedAt] DATETIME2 NULL,
        [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
        FOREIGN KEY ([KDSOrderId]) REFERENCES [KDSOrders]([KDSOrderId]) ON DELETE CASCADE
    );
    
    CREATE INDEX IX_KDSOrderItems_KDSOrderId ON [KDSOrderItems]([KDSOrderId]);
    CREATE INDEX IX_KDSOrderItems_IsCompleted ON [KDSOrderItems]([IsCompleted]);
END
GO

-- Stations Table (kitchen stations)
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Stations')
BEGIN
    CREATE TABLE [Stations] (
        [StationId] NVARCHAR(36) PRIMARY KEY,
        [StationName] NVARCHAR(100) NOT NULL UNIQUE,
        [StationType] NVARCHAR(50),
        [DisplayOrder] INT,
        [IsActive] BIT DEFAULT 1,
        [CreatedAt] DATETIME2 DEFAULT GETUTCDATE()
    );
END
GO

-- KDSMetrics Table (for performance tracking)
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'KDSMetrics')
BEGIN
    CREATE TABLE [KDSMetrics] (
        [MetricId] NVARCHAR(36) PRIMARY KEY,
        [OrderId] NVARCHAR(36) NOT NULL,
        [StationId] NVARCHAR(36),
        [PrepTimeMinutes] INT,
        [EstimatedTimeMinutes] INT,
        [WasOnTime] BIT,
        [RecordedAt] DATETIME2 DEFAULT GETUTCDATE(),
        FOREIGN KEY ([OrderId]) REFERENCES [KDSOrders]([OrderId]) ON DELETE CASCADE
    );
    
    CREATE INDEX IX_KDSMetrics_OrderId ON [KDSMetrics]([OrderId]);
    CREATE INDEX IX_KDSMetrics_RecordedAt ON [KDSMetrics]([RecordedAt]);
END
GO

-- StationAssignments Table (for tracking which station should handle what)
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'StationAssignments')
BEGIN
    CREATE TABLE [StationAssignments] (
        [AssignmentId] NVARCHAR(36) PRIMARY KEY,
        [KDSOrderId] NVARCHAR(36) NOT NULL,
        [StationId] NVARCHAR(36) NOT NULL,
        [AssignedAt] DATETIME2 DEFAULT GETUTCDATE(),
        [StartedAt] DATETIME2 NULL,
        [CompletedAt] DATETIME2 NULL,
        FOREIGN KEY ([KDSOrderId]) REFERENCES [KDSOrders]([KDSOrderId]) ON DELETE CASCADE,
        FOREIGN KEY ([StationId]) REFERENCES [Stations]([StationId])
    );
    
    CREATE INDEX IX_StationAssignments_KDSOrderId ON [StationAssignments]([KDSOrderId]);
    CREATE INDEX IX_StationAssignments_StationId ON [StationAssignments]([StationId]);
END
GO

-- Seed default stations
IF NOT EXISTS (SELECT 1 FROM [Stations] WHERE [StationName] = 'GRILL')
BEGIN
    INSERT INTO [Stations] ([StationId], [StationName], [StationType], [DisplayOrder], [IsActive])
    VALUES 
        (NEWID(), 'GRILL', 'HOT', 1, 1),
        (NEWID(), 'FRYER', 'HOT', 2, 1),
        (NEWID(), 'PREP', 'COLD', 3, 1),
        (NEWID(), 'SALAD', 'COLD', 4, 1),
        (NEWID(), 'DRINKS', 'BEVERAGE', 5, 1),
        (NEWID(), 'PASTRY', 'BAKE', 6, 1);
END
GO

PRINT 'Kitchen Display Service database setup completed successfully.'
