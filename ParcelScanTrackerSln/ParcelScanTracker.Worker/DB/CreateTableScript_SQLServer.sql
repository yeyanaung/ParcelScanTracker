-- ===============================================
-- ParcelScanDb SQL Server Table Creation Script
-- ===============================================
-- This script creates the database and tables for parcel scan event tracking.
-- Tables: ScanEventState, ParcelScanEvent, RawScanEvent
-- Each table is dropped and recreated for a clean setup.
-- ===============================================

-- Create DB if missing
IF DB_ID(N'ParcelScanDb') IS NULL
BEGIN
    CREATE DATABASE [ParcelScanDb];
END;
GO

USE [ParcelScanDb];
GO

/* ===== Reset (drop in dependency order) ===== */
-- Drop tables if they exist to ensure a clean state.
IF OBJECT_ID(N'dbo.RawScanEvent', N'U') IS NOT NULL
    DROP TABLE dbo.RawScanEvent;
IF OBJECT_ID(N'dbo.ParcelScanEvent', N'U') IS NOT NULL
    DROP TABLE dbo.ParcelScanEvent;
IF OBJECT_ID(N'dbo.ScanEventState', N'U') IS NOT NULL
    DROP TABLE dbo.ScanEventState;
GO

/* ===== Recreate tables ===== */

-- 1) ScanEventState: Tracks the last processed event for each worker.
CREATE TABLE dbo.ScanEventState (
    Id            INT IDENTITY(1,1) PRIMARY KEY,         -- Auto-increment primary key
    LastEventId   INT       NOT NULL,                    -- Last processed event ID
    WorkerId      NVARCHAR(30) NOT NULL UNIQUE           -- Unique worker identifier
);

-- 2) ParcelScanEvent: Stores parcel scan events.
CREATE TABLE dbo.ParcelScanEvent (
    Id                   INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ParcelScanEvent PRIMARY KEY, -- Auto-increment PK
    EventId              INT       NOT NULL,                    -- Unique event ID (business key)
    ParcelId             INT       NOT NULL,                    -- Parcel identifier
    [Type]            NVARCHAR(50) NOT NULL,                    -- Event type (e.g., PICKUP, DELIVERY)
    CreatedDateTimeUtc   DATETIME2    NOT NULL CONSTRAINT DF_ParcelScanEvent_Created DEFAULT SYSUTCDATETIME(), -- UTC creation time
    StatusCode           NVARCHAR(50)     NULL,                 -- Optional status code
    RunId                NVARCHAR(50)     NULL,                 -- Optional run identifier
    WorkerId             NVARCHAR(30) NOT NULL,                 -- Worker identifier
    CONSTRAINT UQ_ParcelScanEvent_EventId UNIQUE (EventId)      -- Enforces unique EventId
);

-- Helpful indexes for query performance
CREATE INDEX IX_ParcelScanEvent_ParcelId ON dbo.ParcelScanEvent (ParcelId);           -- Index for parcel lookups
CREATE INDEX IX_ParcelScanEvent_Created  ON dbo.ParcelScanEvent (CreatedDateTimeUtc);  -- Index for date queries
CREATE INDEX IX_ParcelScanEvent_Worker   ON dbo.ParcelScanEvent (WorkerId);            -- Index for worker queries

-- 3) RawScanEvent: Stores raw event payloads for auditing.
CREATE TABLE dbo.RawScanEvent (
    Id             INT IDENTITY(1,1) NOT NULL PRIMARY KEY,      -- Auto-increment PK
    EventId        INT       NOT NULL UNIQUE,                   -- Unique event ID (FK to ParcelScanEvent)
    WorkerId       NVARCHAR(30) NOT NULL,                       -- Worker identifier
    RawJson        NVARCHAR(MAX) NOT NULL,                      -- Raw event JSON payload
    IngestedAtUtc  DATETIME2     NOT NULL CONSTRAINT DF_RawScanEvent_IngestedAtUtc DEFAULT SYSUTCDATETIME(), -- UTC ingest time
);
GO
