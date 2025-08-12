using Microsoft.EntityFrameworkCore;

using ParcelScanTracker.Common.Models;

/// <summary>
/// EF Core DbContext for parcel scan tracking.
/// Provides access to scan events, worker states, and raw event data.
/// </summary>
public class ScanEventDbContext : DbContext
{
    /// <summary>
    /// Table of parcel scan events.
    /// </summary>
    public DbSet<ParcelScanEvent> ParcelScanEvents { get; set; }

    /// <summary>
    /// Table of last fetched event states for each worker.
    /// </summary>
    public DbSet<ScanEventState> ScanEventStates { get; set; }

    /// <summary>
    /// Table of raw scan event payloads.
    /// </summary>
    public DbSet<RawScanEvent> RawScanEvents { get; set; }

    public ScanEventDbContext(DbContextOptions<ScanEventDbContext> options)
        : base(options) { }
}
