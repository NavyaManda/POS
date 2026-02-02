-- Menu Service Database Setup
-- Creates POSMenuDb with MenuItems, Categories, and MenuItemVariants tables

USE master;
GO

-- Create Menu Service Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'POSMenuDb')
BEGIN
    CREATE DATABASE POSMenuDb;
END
GO

USE POSMenuDb;
GO

-- Categories Table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Categories')
BEGIN
    CREATE TABLE [Categories] (
        [CategoryId] NVARCHAR(36) PRIMARY KEY,
        [CategoryName] NVARCHAR(255) NOT NULL UNIQUE,
        [Description] NVARCHAR(MAX),
        [DisplayOrder] INT DEFAULT 0,
        [IsActive] BIT DEFAULT 1,
        [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 DEFAULT GETUTCDATE()
    );
END
GO

-- MenuItems Table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'MenuItems')
BEGIN
    CREATE TABLE [MenuItems] (
        [ItemId] NVARCHAR(36) PRIMARY KEY,
        [Name] NVARCHAR(255) NOT NULL,
        [Description] NVARCHAR(MAX),
        [Price] DECIMAL(10, 2) NOT NULL,
        [CategoryId] NVARCHAR(36) NOT NULL,
        [IsAvailable] BIT DEFAULT 1,
        [PrepTimeMinutes] INT,
        [ImageUrl] NVARCHAR(MAX),
        [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 DEFAULT GETUTCDATE(),
        FOREIGN KEY ([CategoryId]) REFERENCES [Categories]([CategoryId]) ON DELETE CASCADE
    );
    
    CREATE INDEX IX_MenuItems_CategoryId ON [MenuItems]([CategoryId]);
    CREATE INDEX IX_MenuItems_IsAvailable ON [MenuItems]([IsAvailable]);
    CREATE INDEX IX_MenuItems_Name ON [MenuItems]([Name]);
END
GO

-- MenuItemVariants Table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'MenuItemVariants')
BEGIN
    CREATE TABLE [MenuItemVariants] (
        [VariantId] NVARCHAR(36) PRIMARY KEY,
        [ItemId] NVARCHAR(36) NOT NULL,
        [VariantName] NVARCHAR(100) NOT NULL,
        [VariantDescription] NVARCHAR(MAX),
        [PriceAdjustment] DECIMAL(10, 2) DEFAULT 0,
        [IsAvailable] BIT DEFAULT 1,
        [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
        FOREIGN KEY ([ItemId]) REFERENCES [MenuItems]([ItemId]) ON DELETE CASCADE
    );
    
    CREATE INDEX IX_MenuItemVariants_ItemId ON [MenuItemVariants]([ItemId]);
END
GO

-- MenuItemPrices Table (for price history)
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'MenuItemPrices')
BEGIN
    CREATE TABLE [MenuItemPrices] (
        [PriceId] NVARCHAR(36) PRIMARY KEY,
        [ItemId] NVARCHAR(36) NOT NULL,
        [Price] DECIMAL(10, 2) NOT NULL,
        [EffectiveFrom] DATETIME2 NOT NULL,
        [EffectiveTo] DATETIME2 NULL,
        [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
        FOREIGN KEY ([ItemId]) REFERENCES [MenuItems]([ItemId]) ON DELETE CASCADE
    );
    
    CREATE INDEX IX_MenuItemPrices_ItemId ON [MenuItemPrices]([ItemId]);
    CREATE INDEX IX_MenuItemPrices_EffectiveFrom ON [MenuItemPrices]([EffectiveFrom]);
END
GO

PRINT 'Menu Service database setup completed successfully.'
