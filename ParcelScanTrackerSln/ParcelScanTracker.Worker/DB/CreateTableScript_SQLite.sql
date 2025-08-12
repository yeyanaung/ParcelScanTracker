PRAGMA foreign_keys = ON;

BEGIN TRANSACTION;

-- Table: WorkerInfo
CREATE TABLE IF NOT EXISTS WorkerInfo (
    WorkerId TEXT PRIMARY KEY,
    WorkerName TEXT NOT NULL
);

-- Insert demo records into WorkerInfo
INSERT INTO WorkerInfo (WorkerId, WorkerName) VALUES
    ('W_AKL', 'Auckland Scan Event Worker'),
    ('W_WLG', 'Wellington Scan Event Worker'),
    ('W_CHC', 'Christchurch Scan Event Worker'),
    ('W_HAM', 'Hamilton Scan Event Worker'),
    ('W_ZQN', 'Queenstown Scan Event Worker');

-- Table: ScanEventState
CREATE TABLE IF NOT EXISTS ScanEventState (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    WorkerId TEXT NOT NULL,
    LastFetchedEventId INTEGER NOT NULL,
    UNIQUE(WorkerId),
    FOREIGN KEY (WorkerId) REFERENCES WorkerInfo(WorkerId) ON DELETE CASCADE
);

-- Table: ParcelScanEvent
CREATE TABLE IF NOT EXISTS ParcelScanEvent (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    EventId INTEGER NOT NULL UNIQUE,
    ParcelId INTEGER NOT NULL,
    EventType INTEGER NOT NULL, -- stores ScanEvent enum as int
    CreatedDateTimeUtc DATETIME NOT NULL DEFAULT (datetime('now')),
    StatusCode TEXT,
    WorkerId TEXT NOT NULL,
    FOREIGN KEY (WorkerId) REFERENCES WorkerInfo(WorkerId) ON DELETE CASCADE
);

-- Table: Device
CREATE TABLE IF NOT EXISTS Device (
    DeviceTransactionId INT NOT NULL PRIMARY KEY,
    DeviceId INT NOT NULL,
    EventId INTEGER NOT NULL,
    FOREIGN KEY (EventId) REFERENCES ParcelScanEvent(EventId) ON DELETE CASCADE
);

-- Table: User
CREATE TABLE IF NOT EXISTS User (
    UserId TEXT PRIMARY KEY,
    CarrierId TEXT NOT NULL,
    RunId TEXT NOT NULL,
    EventId INTEGER NOT NULL,
    FOREIGN KEY (EventId) REFERENCES ParcelScanEvent(EventId) ON DELETE CASCADE
);

COMMIT;
