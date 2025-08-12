-- ===============================================
-- ParcelScanDb SQL Server Drop Tables Script
-- ===============================================
-- This script drops all tables used for parcel scan event tracking.
-- Run this to clean up the database before recreating tables or resetting state.
-- Tables dropped: RawScanEvent, ParcelScanEvent, ScanEventState
-- ===============================================

-- Drop RawScanEvent table if it exists (depends on ParcelScanEvent)
IF OBJECT_ID(N'dbo.RawScanEvent', N'U') IS NOT NULL
    DROP TABLE dbo.RawScanEvent;

-- Drop ParcelScanEvent table if it exists (depends on ScanEventState)
IF OBJECT_ID(N'dbo.ParcelScanEvent', N'U') IS NOT NULL
    DROP TABLE dbo.ParcelScanEvent;

-- Drop ScanEventState table if it exists (no dependencies)
IF OBJECT_ID(N'dbo.ScanEventState', N'U') IS NOT NULL
    DROP TABLE dbo.ScanEventState;

GO
