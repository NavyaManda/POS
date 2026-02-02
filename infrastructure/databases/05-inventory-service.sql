-- Inventory Service Database Setup
-- Creates POSInventoryDb with Ingredients, StockLevels, and StockTransactions tables

USE master;
GO

-- Create Inventory Service Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'POSInventoryDb')
BEGIN
    CREATE DATABASE POSInventoryDb;
END
GO

USE POSInventoryDb;
GO

-- Ingredients Table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Ingredients')
BEGIN
    CREATE TABLE [Ingredients] (
        [IngredientId] NVARCHAR(36) PRIMARY KEY,
        [Name] NVARCHAR(255) NOT NULL UNIQUE,
        [Description] NVARCHAR(MAX),
        [Unit] NVARCHAR(50),
        [IsActive] BIT DEFAULT 1,
        [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 DEFAULT GETUTCDATE()
    );
    
    CREATE INDEX IX_Ingredients_Name ON [Ingredients]([Name]);
END
GO

-- StockLevels Table (current inventory)
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'StockLevels')
BEGIN
    CREATE TABLE [StockLevels] (
        [StockLevelId] NVARCHAR(36) PRIMARY KEY,
        [IngredientId] NVARCHAR(36) NOT NULL UNIQUE,
        [Quantity] DECIMAL(10, 2) NOT NULL,
        [ReservedQuantity] DECIMAL(10, 2) DEFAULT 0,
        [LastUpdated] DATETIME2 DEFAULT GETUTCDATE(),
        FOREIGN KEY ([IngredientId]) REFERENCES [Ingredients]([IngredientId]) ON DELETE CASCADE
    );
    
    CREATE INDEX IX_StockLevels_IngredientId ON [StockLevels]([IngredientId]);
END
GO

-- StockTransactions Table (transaction log)
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'StockTransactions')
BEGIN
    CREATE TABLE [StockTransactions] (
        [TransactionId] NVARCHAR(36) PRIMARY KEY,
        [IngredientId] NVARCHAR(36) NOT NULL,
        [TransactionType] NVARCHAR(50) NOT NULL,
        [Quantity] DECIMAL(10, 2) NOT NULL,
        [Reference] NVARCHAR(MAX),
        [Notes] NVARCHAR(MAX),
        [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
        FOREIGN KEY ([IngredientId]) REFERENCES [Ingredients]([IngredientId]) ON DELETE CASCADE
    );
    
    CREATE INDEX IX_StockTransactions_IngredientId ON [StockTransactions]([IngredientId]);
    CREATE INDEX IX_StockTransactions_CreatedAt ON [StockTransactions]([CreatedAt]);
    CREATE INDEX IX_StockTransactions_TransactionType ON [StockTransactions]([TransactionType]);
END
GO

-- ReorderPoints Table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ReorderPoints')
BEGIN
    CREATE TABLE [ReorderPoints] (
        [ReorderPointId] NVARCHAR(36) PRIMARY KEY,
        [IngredientId] NVARCHAR(36) NOT NULL UNIQUE,
        [MinimumLevel] DECIMAL(10, 2) NOT NULL,
        [ReorderQuantity] DECIMAL(10, 2),
        [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 DEFAULT GETUTCDATE(),
        FOREIGN KEY ([IngredientId]) REFERENCES [Ingredients]([IngredientId]) ON DELETE CASCADE
    );
    
    CREATE INDEX IX_ReorderPoints_IngredientId ON [ReorderPoints]([IngredientId]);
END
GO

-- Suppliers Table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Suppliers')
BEGIN
    CREATE TABLE [Suppliers] (
        [SupplierId] NVARCHAR(36) PRIMARY KEY,
        [SupplierName] NVARCHAR(255) NOT NULL,
        [ContactEmail] NVARCHAR(255),
        [ContactPhone] NVARCHAR(20),
        [Address] NVARCHAR(MAX),
        [IsActive] BIT DEFAULT 1,
        [CreatedAt] DATETIME2 DEFAULT GETUTCDATE()
    );
    
    CREATE INDEX IX_Suppliers_SupplierName ON [Suppliers]([SupplierName]);
END
GO

-- IngredientSuppliers Junction Table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'IngredientSuppliers')
BEGIN
    CREATE TABLE [IngredientSuppliers] (
        [IngredientSupplierId] NVARCHAR(36) PRIMARY KEY,
        [IngredientId] NVARCHAR(36) NOT NULL,
        [SupplierId] NVARCHAR(36) NOT NULL,
        [SupplierPartNumber] NVARCHAR(100),
        [UnitCost] DECIMAL(10, 2),
        [LeadTimeDays] INT,
        [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
        FOREIGN KEY ([IngredientId]) REFERENCES [Ingredients]([IngredientId]) ON DELETE CASCADE,
        FOREIGN KEY ([SupplierId]) REFERENCES [Suppliers]([SupplierId]) ON DELETE CASCADE
    );
    
    CREATE INDEX IX_IngredientSuppliers_IngredientId ON [IngredientSuppliers]([IngredientId]);
    CREATE INDEX IX_IngredientSuppliers_SupplierId ON [IngredientSuppliers]([SupplierId]);
END
GO

PRINT 'Inventory Service database setup completed successfully.'
