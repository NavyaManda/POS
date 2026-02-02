-- Loyalty Service Database Setup
-- Creates POSLoyaltyDb with LoyaltyAccounts and PointsTransactions tables

USE master;
GO

-- Create Loyalty Service Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'POSLoyaltyDb')
BEGIN
    CREATE DATABASE POSLoyaltyDb;
END
GO

USE POSLoyaltyDb;
GO

-- MembershipTiers Table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'MembershipTiers')
BEGIN
    CREATE TABLE [MembershipTiers] (
        [TierId] NVARCHAR(36) PRIMARY KEY,
        [TierName] NVARCHAR(100) NOT NULL UNIQUE,
        [MinimumPoints] DECIMAL(10, 2),
        [PointsMultiplier] DECIMAL(5, 2),
        [DiscountPercentage] DECIMAL(5, 2),
        [CreatedAt] DATETIME2 DEFAULT GETUTCDATE()
    );
END
GO

-- LoyaltyAccounts Table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'LoyaltyAccounts')
BEGIN
    CREATE TABLE [LoyaltyAccounts] (
        [AccountId] NVARCHAR(36) PRIMARY KEY,
        [CustomerId] NVARCHAR(36) NOT NULL UNIQUE,
        [TierId] NVARCHAR(36),
        [TotalPoints] DECIMAL(10, 2) DEFAULT 0,
        [LifetimePoints] DECIMAL(10, 2) DEFAULT 0,
        [IsActive] BIT DEFAULT 1,
        [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
        [LastActivityAt] DATETIME2 NULL,
        [TierUpgradedAt] DATETIME2 NULL,
        FOREIGN KEY ([TierId]) REFERENCES [MembershipTiers]([TierId])
    );
    
    CREATE INDEX IX_LoyaltyAccounts_CustomerId ON [LoyaltyAccounts]([CustomerId]);
    CREATE INDEX IX_LoyaltyAccounts_TierId ON [LoyaltyAccounts]([TierId]);
    CREATE INDEX IX_LoyaltyAccounts_TotalPoints ON [LoyaltyAccounts]([TotalPoints]);
END
GO

-- PointsTransactions Table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'PointsTransactions')
BEGIN
    CREATE TABLE [PointsTransactions] (
        [TransactionId] NVARCHAR(36) PRIMARY KEY,
        [AccountId] NVARCHAR(36) NOT NULL,
        [Points] DECIMAL(10, 2) NOT NULL,
        [Type] NVARCHAR(50) NOT NULL,
        [OrderId] NVARCHAR(36),
        [Description] NVARCHAR(MAX),
        [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
        FOREIGN KEY ([AccountId]) REFERENCES [LoyaltyAccounts]([AccountId]) ON DELETE CASCADE
    );
    
    CREATE INDEX IX_PointsTransactions_AccountId ON [PointsTransactions]([AccountId]);
    CREATE INDEX IX_PointsTransactions_Type ON [PointsTransactions]([Type]);
    CREATE INDEX IX_PointsTransactions_CreatedAt ON [PointsTransactions]([CreatedAt]);
END
GO

-- Rewards Table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Rewards')
BEGIN
    CREATE TABLE [Rewards] (
        [RewardId] NVARCHAR(36) PRIMARY KEY,
        [RewardName] NVARCHAR(255) NOT NULL,
        [Description] NVARCHAR(MAX),
        [PointsCost] DECIMAL(10, 2) NOT NULL,
        [RewardType] NVARCHAR(50),
        [IsActive] BIT DEFAULT 1,
        [ExpiresAt] DATETIME2 NULL,
        [CreatedAt] DATETIME2 DEFAULT GETUTCDATE()
    );
    
    CREATE INDEX IX_Rewards_IsActive ON [Rewards]([IsActive]);
END
GO

-- RewardRedemptions Table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'RewardRedemptions')
BEGIN
    CREATE TABLE [RewardRedemptions] (
        [RedemptionId] NVARCHAR(36) PRIMARY KEY,
        [AccountId] NVARCHAR(36) NOT NULL,
        [RewardId] NVARCHAR(36) NOT NULL,
        [OrderId] NVARCHAR(36),
        [Status] NVARCHAR(50) NOT NULL,
        [RedeemedAt] DATETIME2 DEFAULT GETUTCDATE(),
        [UsedAt] DATETIME2 NULL,
        [ExpiresAt] DATETIME2 NULL,
        FOREIGN KEY ([AccountId]) REFERENCES [LoyaltyAccounts]([AccountId]) ON DELETE CASCADE,
        FOREIGN KEY ([RewardId]) REFERENCES [Rewards]([RewardId])
    );
    
    CREATE INDEX IX_RewardRedemptions_AccountId ON [RewardRedemptions]([AccountId]);
    CREATE INDEX IX_RewardRedemptions_Status ON [RewardRedemptions]([Status]);
END
GO

-- Promotions Table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Promotions')
BEGIN
    CREATE TABLE [Promotions] (
        [PromotionId] NVARCHAR(36) PRIMARY KEY,
        [PromotionName] NVARCHAR(255) NOT NULL,
        [Description] NVARCHAR(MAX),
        [PromotionType] NVARCHAR(50),
        [Value] DECIMAL(10, 2),
        [BonusPoints] DECIMAL(10, 2),
        [StartDate] DATETIME2 NOT NULL,
        [EndDate] DATETIME2 NOT NULL,
        [IsActive] BIT DEFAULT 1,
        [CreatedAt] DATETIME2 DEFAULT GETUTCDATE()
    );
    
    CREATE INDEX IX_Promotions_IsActive ON [Promotions]([IsActive]);
    CREATE INDEX IX_Promotions_StartDate ON [Promotions]([StartDate]);
END
GO

-- Seed default membership tiers
IF NOT EXISTS (SELECT 1 FROM [MembershipTiers] WHERE [TierName] = 'BRONZE')
BEGIN
    INSERT INTO [MembershipTiers] ([TierId], [TierName], [MinimumPoints], [PointsMultiplier], [DiscountPercentage])
    VALUES 
        (NEWID(), 'BRONZE', 0, 1.0, 0),
        (NEWID(), 'SILVER', 500, 1.5, 5),
        (NEWID(), 'GOLD', 1500, 2.0, 10),
        (NEWID(), 'PLATINUM', 3000, 3.0, 15);
END
GO

PRINT 'Loyalty Service database setup completed successfully.'
