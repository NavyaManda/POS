-- Notification Service Database Setup
-- Creates POSNotificationDb with NotificationTemplates and NotificationLogs tables

USE master;
GO

-- Create Notification Service Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'POSNotificationDb')
BEGIN
    CREATE DATABASE POSNotificationDb;
END
GO

USE POSNotificationDb;
GO

-- NotificationChannels Table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'NotificationChannels')
BEGIN
    CREATE TABLE [NotificationChannels] (
        [ChannelId] NVARCHAR(36) PRIMARY KEY,
        [ChannelName] NVARCHAR(100) NOT NULL UNIQUE,
        [ChannelType] NVARCHAR(50),
        [IsActive] BIT DEFAULT 1,
        [CreatedAt] DATETIME2 DEFAULT GETUTCDATE()
    );
END
GO

-- NotificationTemplates Table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'NotificationTemplates')
BEGIN
    CREATE TABLE [NotificationTemplates] (
        [TemplateId] NVARCHAR(36) PRIMARY KEY,
        [TemplateName] NVARCHAR(255) NOT NULL UNIQUE,
        [ChannelId] NVARCHAR(36) NOT NULL,
        [Subject] NVARCHAR(255),
        [Body] NVARCHAR(MAX) NOT NULL,
        [Variables] NVARCHAR(MAX),
        [IsActive] BIT DEFAULT 1,
        [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 DEFAULT GETUTCDATE(),
        FOREIGN KEY ([ChannelId]) REFERENCES [NotificationChannels]([ChannelId])
    );
    
    CREATE INDEX IX_NotificationTemplates_ChannelId ON [NotificationTemplates]([ChannelId]);
    CREATE INDEX IX_NotificationTemplates_IsActive ON [NotificationTemplates]([IsActive]);
END
GO

-- NotificationPreferences Table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'NotificationPreferences')
BEGIN
    CREATE TABLE [NotificationPreferences] (
        [PreferenceId] NVARCHAR(36) PRIMARY KEY,
        [UserId] NVARCHAR(36) NOT NULL,
        [ChannelId] NVARCHAR(36) NOT NULL,
        [IsEnabled] BIT DEFAULT 1,
        [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 DEFAULT GETUTCDATE(),
        FOREIGN KEY ([ChannelId]) REFERENCES [NotificationChannels]([ChannelId])
    );
    
    CREATE INDEX IX_NotificationPreferences_UserId ON [NotificationPreferences]([UserId]);
    CREATE INDEX IX_NotificationPreferences_ChannelId ON [NotificationPreferences]([ChannelId]);
END
GO

-- NotificationLogs Table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'NotificationLogs')
BEGIN
    CREATE TABLE [NotificationLogs] (
        [LogId] NVARCHAR(36) PRIMARY KEY,
        [RecipientId] NVARCHAR(36) NOT NULL,
        [TemplateId] NVARCHAR(36) NOT NULL,
        [ChannelId] NVARCHAR(36) NOT NULL,
        [RecipientAddress] NVARCHAR(255),
        [Subject] NVARCHAR(255),
        [Body] NVARCHAR(MAX),
        [Status] NVARCHAR(50) NOT NULL,
        [ErrorMessage] NVARCHAR(MAX),
        [SentAt] DATETIME2 NULL,
        [DeliveredAt] DATETIME2 NULL,
        [ReadAt] DATETIME2 NULL,
        [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
        FOREIGN KEY ([TemplateId]) REFERENCES [NotificationTemplates]([TemplateId]),
        FOREIGN KEY ([ChannelId]) REFERENCES [NotificationChannels]([ChannelId])
    );
    
    CREATE INDEX IX_NotificationLogs_RecipientId ON [NotificationLogs]([RecipientId]);
    CREATE INDEX IX_NotificationLogs_Status ON [NotificationLogs]([Status]);
    CREATE INDEX IX_NotificationLogs_CreatedAt ON [NotificationLogs]([CreatedAt]);
    CREATE INDEX IX_NotificationLogs_SentAt ON [NotificationLogs]([SentAt]);
END
GO

-- NotificationQueue Table (for async processing)
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'NotificationQueue')
BEGIN
    CREATE TABLE [NotificationQueue] (
        [QueueId] NVARCHAR(36) PRIMARY KEY,
        [LogId] NVARCHAR(36) NOT NULL,
        [Status] NVARCHAR(50) NOT NULL,
        [RetryCount] INT DEFAULT 0,
        [MaxRetries] INT DEFAULT 3,
        [NextRetryAt] DATETIME2 NULL,
        [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
        [ProcessedAt] DATETIME2 NULL,
        FOREIGN KEY ([LogId]) REFERENCES [NotificationLogs]([LogId]) ON DELETE CASCADE
    );
    
    CREATE INDEX IX_NotificationQueue_Status ON [NotificationQueue]([Status]);
    CREATE INDEX IX_NotificationQueue_NextRetryAt ON [NotificationQueue]([NextRetryAt]);
END
GO

-- Seed default notification channels
IF NOT EXISTS (SELECT 1 FROM [NotificationChannels] WHERE [ChannelName] = 'EMAIL')
BEGIN
    INSERT INTO [NotificationChannels] ([ChannelId], [ChannelName], [ChannelType], [IsActive])
    VALUES 
        (NEWID(), 'EMAIL', 'EMAIL', 1),
        (NEWID(), 'SMS', 'SMS', 1),
        (NEWID(), 'PUSH', 'PUSH', 1),
        (NEWID(), 'IN_APP', 'IN_APP', 1);
END
GO

PRINT 'Notification Service database setup completed successfully.'
