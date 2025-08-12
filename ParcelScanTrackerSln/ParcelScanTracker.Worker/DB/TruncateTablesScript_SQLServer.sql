-- ===============================================
-- ParcelScanDb SQL Server Truncate Tables Script
-- ===============================================
-- This script removes all data from parcel scan tracking tables.
-- Use to quickly clear table contents without dropping table structures.
-- Tables truncated: RawScanEvent, ParcelScanEvent, ScanEventState
-- Note: TRUNCATE TABLE resets identity columns and cannot be used if tables have foreign key constraints.
-- ===============================================

USE [ParcelScanDb];
GO

TRUNCATE TABLE dbo.RawScanEvent;      -- Remove all rows from RawScanEvent
TRUNCATE TABLE dbo.ParcelScanEvent;   -- Remove all rows from ParcelScanEvent
TRUNCATE TABLE dbo.ScanEventState;    -- Remove all rows from ScanEventState
