using Microsoft.EntityFrameworkCore;
using ParcelScanTracker.Common.EnumType;
using ParcelScanTracker.Common.Interfaces;
using ParcelScanTracker.Common.Models;

namespace ParcelScanTracker.DAL
{
    /// <summary>
    /// Repository for managing parcel scan events and worker state in the database.
    /// Provides methods to save events, query latest events, and track worker progress.
    /// </summary>
    public class ParcelScanEventRepository : IParcelScanEventRepository
    {
        private readonly ScanEventDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParcelScanEventRepository"/> class.
        /// </summary>
        /// <param name="context">The database context to use.</param>
        public ParcelScanEventRepository(ScanEventDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Saves a new scan event to the database.
        /// </summary>
        /// <param name="scanEvent">The scan event to save.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task SaveScanEventAsync(ParcelScanEvent scanEvent, CancellationToken cancellationToken = default)
        {
            _context.ParcelScanEvents.Add(scanEvent);
            await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Adds a new parcel scan event to the database.
        /// </summary>
        /// <param name="parcelScanEvent">The parcel scan event to add.</param>
        public async Task AddParcelScanEventAsync(ParcelScanEvent parcelScanEvent)
        {
            _context.ParcelScanEvents.Add(parcelScanEvent);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Adds a new raw scan event to the database.
        /// </summary>
        /// <param name="rawScanEvent">The raw scan event to add.</param>
        public async Task AddRawScanEventAsync(RawScanEvent rawScanEvent)
        {
            _context.RawScanEvents.Add(rawScanEvent);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates the scan event state for a worker, or creates it if not found.
        /// </summary>
        /// <param name="state">The scan event state to update or add.</param>
        public async Task UpdateScanEventStateAsync(ScanEventState state)
        {
            var existing = await _context.ScanEventStates
                .FirstOrDefaultAsync(s => s.WorkerId == state.WorkerId);

            if (existing != null)
            {
                existing.LastEventId = state.LastEventId;
            }
            else
            {
                _context.ScanEventStates.Add(state);
            }
            await _context.SaveChangesAsync();
        }


        /// <summary>
        /// Gets the most recent scan event for a given parcel.
        /// </summary>
        /// <param name="parcelId">The parcel ID to query.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The latest scan event or null if not found.</returns>
        public async Task<ParcelScanEvent?> GetLatestEventForParcelAsync(int parcelId, CancellationToken cancellationToken = default)
        {
            return await _context.ParcelScanEvents
                .Where(e => e.ParcelId == parcelId)
                .OrderByDescending(e => e.CreatedDateTimeUtc)
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Gets the earliest pickup time for a given parcel.
        /// </summary>
        /// <param name="parcelId">The parcel ID to query.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The pickup DateTime or null if not found.</returns>
        public async Task<DateTime?> GetPickupTimeAsync(int parcelId, CancellationToken cancellationToken = default)
        {
            return await _context.ParcelScanEvents
                .Where(e => e.ParcelId == parcelId && e.Type == ScanEvent.PICKUP.ToString())
                .OrderBy(e => e.CreatedDateTimeUtc)
                .Select(e => (DateTime?)e.CreatedDateTimeUtc)
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Gets the earliest delivery time for a given parcel.
        /// </summary>
        /// <param name="parcelId">The parcel ID to query.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The delivery DateTime or null if not found.</returns>
        public async Task<DateTime?> GetDeliveryTimeAsync(int parcelId, CancellationToken cancellationToken = default)
        {
            return await _context.ParcelScanEvents
                .Where(e => e.ParcelId == parcelId && e.Type == ScanEvent.DELIVERY.ToString())
                .OrderBy(e => e.CreatedDateTimeUtc)
                .Select(e => (DateTime?)e.CreatedDateTimeUtc)
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// Gets the last fetched event ID for the worker.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The last fetched event ID, or 0 if not found.</returns>
        public async Task<int> GetLastFetchedEventIdAsync(CancellationToken cancellationToken = default)
        {
            var state = await _context.ScanEventStates.FirstOrDefaultAsync(cancellationToken);
            return state?.LastEventId ?? 0;
        }

        /// <summary>
        /// Updates or creates the last fetched event ID for the worker.
        /// </summary>
        /// <param name="eventId">The event ID to set.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task SetLastFetchedEventIdAsync(int eventId, CancellationToken cancellationToken = default)
        {
            var state = await _context.ScanEventStates.FirstOrDefaultAsync(cancellationToken);
            if (state == null)
            {
                state = new ScanEventState { LastEventId = eventId };
                _context.ScanEventStates.Add(state);
            }
            else
            {
                state.LastEventId = eventId;
                _context.ScanEventStates.Update(state);
            }
            await _context.SaveChangesAsync(cancellationToken);
        }

    }
}
