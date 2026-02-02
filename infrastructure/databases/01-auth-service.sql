-- Auth Service Database Setup
-- Creates POSAuthDb with Users, Roles, UserRoles, and RefreshTokens tables

USE master;
GO

-- Create Auth Service Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'POSAuthDb')
BEGIN
    CREATE DATABASE POSAuthDb;
END
GO

USE POSAuthDb;
GO

-- Users Table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Users')
BEGIN
    CREATE TABLE [Users] (
        [UserId] NVARCHAR(36) PRIMARY KEY,
        [Email] NVARCHAR(255) NOT NULL UNIQUE,
        [PasswordHash] NVARCHAR(MAX) NOT NULL,
        [FirstName] NVARCHAR(100),
        [LastName] NVARCHAR(100),
        [IsActive] BIT DEFAULT 1,
        [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
        [LastLoginAt] DATETIME2 NULL
    );
    
    CREATE INDEX IX_Users_Email ON [Users]([Email]);
END
GO

-- Roles Table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Roles')
BEGIN
    CREATE TABLE [Roles] (
        [RoleId] NVARCHAR(36) PRIMARY KEY,
        [RoleName] NVARCHAR(100) NOT NULL UNIQUE,
        [Description] NVARCHAR(MAX),
        [CreatedAt] DATETIME2 DEFAULT GETUTCDATE()
    );
END
GO

-- UserRoles Junction Table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'UserRoles')
BEGIN
    CREATE TABLE [UserRoles] (
        [UserId] NVARCHAR(36) NOT NULL,
        [RoleId] NVARCHAR(36) NOT NULL,
        PRIMARY KEY ([UserId], [RoleId]),
        FOREIGN KEY ([UserId]) REFERENCES [Users]([UserId]) ON DELETE CASCADE,
        FOREIGN KEY ([RoleId]) REFERENCES [Roles]([RoleId]) ON DELETE CASCADE
    );
END
GO

-- RefreshTokens Table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'RefreshTokens')
BEGIN
    CREATE TABLE [RefreshTokens] (
        [TokenId] NVARCHAR(36) PRIMARY KEY,
        [UserId] NVARCHAR(36) NOT NULL,
        [Token] NVARCHAR(MAX) NOT NULL,
        [ExpiresAt] DATETIME2 NOT NULL,
        [IsRevoked] BIT DEFAULT 0,
        [CreatedAt] DATETIME2 DEFAULT GETUTCDATE(),
        FOREIGN KEY ([UserId]) REFERENCES [Users]([UserId]) ON DELETE CASCADE
    );
    
    CREATE INDEX IX_RefreshTokens_UserId ON [RefreshTokens]([UserId]);
    CREATE INDEX IX_RefreshTokens_Token ON [RefreshTokens]([Token]);
    CREATE INDEX IX_RefreshTokens_ExpiresAt ON [RefreshTokens]([ExpiresAt]);
END
GO

-- Seed default roles
IF NOT EXISTS (SELECT 1 FROM [Roles] WHERE [RoleName] = 'admin')
BEGIN
    INSERT INTO [Roles] ([RoleId], [RoleName], [Description])
    VALUES 
        (NEWID(), 'admin', 'Administrator with full access'),
        (NEWID(), 'staff', 'Restaurant staff member'),
        (NEWID(), 'kitchen', 'Kitchen staff member'),
        (NEWID(), 'customer', 'Regular customer');
END
GO

PRINT 'Auth Service database setup completed successfully.'
