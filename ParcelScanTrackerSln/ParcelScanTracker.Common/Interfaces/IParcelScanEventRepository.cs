using ParcelScanTracker.Common.Models;

namespace ParcelScanTracker.Common.Interfaces
{
    public interface IParcelScanEventRepository
    {
        Task SaveScanEventAsync(ParcelScanEvent scanEvent, CancellationToken cancellationToken = default);
        Task<ParcelScanEvent?> GetLatestEventForParcelAsync(int parcelId, CancellationToken cancellationToken = default);
        Task<DateTime?> GetPickupTimeAsync(int parcelId, CancellationToken cancellationToken = default);
        Task<DateTime?> GetDeliveryTimeAsync(int parcelId, CancellationToken cancellationToken = default);
        Task<int> GetLastFetchedEventIdAsync(CancellationToken cancellationToken = default);
        Task SetLastFetchedEventIdAsync(int eventId, CancellationToken cancellationToken = default);
    }
}
