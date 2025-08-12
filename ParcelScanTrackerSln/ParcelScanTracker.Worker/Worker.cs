using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ParcelScanTracker.Common.EnumType;
using ParcelScanTracker.Common.Models;

namespace ParcelScanTracker.Worker
{
    /// <summary>
    /// Background service that polls the Scan Event API, processes scan events,
    /// and updates the local database. Handles logging and event state tracking.
    /// </summary>
    public class Worker : BackgroundService
    {
        // Dependencies injected via constructor.
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly ScanEventApiOptions _apiOptions;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly LogHelper _logService;
        private bool _isFirstRun = true; // Tracks first execution for startup logic.

        /// <summary>
        /// Initializes the Worker with required services and configuration.
        /// </summary>
        public Worker(
            IConfiguration configuration,
            IServiceProvider serviceProvider,
            IOptions<ScanEventApiOptions> apiOptions,
            IHttpClientFactory httpClientFactory,
            LogHelper logService)
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            _apiOptions = apiOptions.Value;
            _httpClientFactory = httpClientFactory;
            _logService = logService;
        }

        /// <summary>
        /// Main background loop: polls API, processes events, updates state, and logs activity.
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            string workerId = _configuration.GetValue<string>("Config:WorkerId");
            int LastEventId = 1; // Default value
            int limit = _apiOptions.Limit;

            // Get last processed event ID from database.
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ScanEventDbContext>();
                var scanEventState = dbContext.ScanEventStates.FirstOrDefault(s => s.WorkerId == workerId);
                if (scanEventState != null)
                {
                    LastEventId = scanEventState.LastEventId;
                    LastEventId++; // Avoid re-fetching last event
                }
            }

            _logService.WriteToLog(LogType.Info, $"Starting ScanEvent Worker with LastFetchedEventId: {LastEventId}");

            // Show last event info on first run.
            if (_isFirstRun)
            {
                WriteConsoleLastFetchedEventInfo(_serviceProvider);
            }

            // Enforce maximum limit for API calls.
            if (limit > 500)
            {
                _logService.WriteToLog(LogType.Warning, $"Requested count is {limit}. Changing to the maximum allowed (500).");
                limit = 500;
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                // Build API URL and log the call.
                var url = $"{_apiOptions.BaseUrl}?FromEventId={LastEventId}&Limit={limit}";
                _logService.WriteToLog(LogType.Info, $"Calling ScanEvent API: {url}");

                try
                {
                    var client = _httpClientFactory.CreateClient();
                    var httpResponse = await client.GetAsync(url, stoppingToken);

                    if (!httpResponse.IsSuccessStatusCode)
                    {
                        _logService.WriteToLog(
                            LogType.Error,
                            $"API call failed. StatusCode: {(int)httpResponse.StatusCode} {httpResponse.ReasonPhrase}"
                        );
                        continue;
                    }

                    var response = await httpResponse.Content.ReadAsStringAsync(stoppingToken);
                    _logService.WriteToLog(LogType.Debug, $"Raw API response: {response}");

                    ScanEventsEnvelope result = null;
                    try
                    {
                        result = JsonConvert.DeserializeObject<ScanEventsEnvelope>(response);
                    }
                    catch (Exception ex)
                    {
                        _logService.WriteToLog(LogType.Error, $"Deserialization error: {ex.Message}");
                    }

                    var parcelScanEvents = result?.ScanEvents;

                    // Process new scan events and update database.
                    if (parcelScanEvents != null && parcelScanEvents.Count > 0)
                    {
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var dbContext = scope.ServiceProvider.GetRequiredService<ScanEventDbContext>();
                            var firstEventId = parcelScanEvents.First().EventId;
                            if (firstEventId < LastEventId)
                            {
                                _logService.WriteToLog(LogType.Warning, $"RawScanEvent Skip already processed EventId: {firstEventId}");
                                await Task.Delay(_apiOptions.PollingDelayMs, stoppingToken);
                                continue;
                            }

                            // Avoid duplicate raw event entries.
                            if (dbContext.RawScanEvents.Any(r => r.EventId == firstEventId))
                            {
                                _logService.WriteToLog(LogType.Warning, $"RawScanEvent with EventId: {firstEventId} already exists. Skipping insertion.");
                                await Task.Delay(_apiOptions.PollingDelayMs, stoppingToken);
                                continue;
                            }
                            else
                            {
                                var rawScanEvent = new RawScanEvent
                                {
                                    RawJson = response,
                                    WorkerId = workerId,
                                    EventId = firstEventId,
                                };

                                dbContext.RawScanEvents.Add(rawScanEvent);
                                await dbContext.SaveChangesAsync(stoppingToken);
                                _logService.WriteToLog(LogType.Info, $"Inserted RawScanEvent with EventId: {firstEventId}");
                            }

                            // Save each scan event and update state.
                            foreach (var scanEvent in parcelScanEvents)
                            {
                                scanEvent.WorkerId = workerId;

                                if (scanEvent.EventId < LastEventId)
                                {
                                    _logService.WriteToLog(LogType.Warning, $"Skipping already processed EventId: {scanEvent.EventId}");
                                    continue;
                                }

                                dbContext.ParcelScanEvents.Add(scanEvent);
                                _logService.WriteToLog(LogType.Info, $"Saved EventId: {scanEvent.EventId}");
                                await dbContext.SaveChangesAsync(stoppingToken);

                                int lastFetchedEventId = scanEvent.EventId;
                                _logService.WriteToLog(LogType.Info, $"Latest EventId updated to: {lastFetchedEventId}");

                                await UpdateScanEventStateAsync(workerId, lastFetchedEventId, _serviceProvider, stoppingToken);
                            }
                        }
                    }
                    else
                    {
                        _logService.WriteToLog(LogType.Info, "No new scan events found.");
                    }
                }
                catch (Exception ex)
                {
                    _logService.WriteToLog(LogType.Error, $"Error processing scan events: {ex}");
                }

                // Wait before next poll.
                if (_isFirstRun)
                {
                    await Task.Delay(1000, stoppingToken);
                    _isFirstRun = false;
                    continue;
                }
                await Task.Delay(_apiOptions.PollingDelayMs, stoppingToken);
            }
        }

        /// <summary>
        /// Updates the ScanEventState table with the latest fetched event ID for the worker.
        /// </summary>
        private async Task UpdateScanEventStateAsync(string workerId, int lastFetchedEventId, IServiceProvider serviceProvider, CancellationToken stoppingToken)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ScanEventDbContext>();

            var scanEventState = dbContext.ScanEventStates.FirstOrDefault(s => s.WorkerId == workerId);

            if (scanEventState == null)
            {
                scanEventState = new ScanEventState
                {
                    WorkerId = workerId,
                    LastEventId = lastFetchedEventId
                };
                dbContext.ScanEventStates.Add(scanEventState);
                _logService.WriteToLog(LogType.Info, $"Created ScanEventState for WorkerId: {workerId} with LastFetchedEventId: {lastFetchedEventId}");
            }
            else
            {
                scanEventState.LastEventId = lastFetchedEventId;
                dbContext.ScanEventStates.Update(scanEventState);
                _logService.WriteToLog(LogType.Info, $"Updated ScanEventState for WorkerId: {workerId} to LastFetchedEventId: {lastFetchedEventId}");
            }

            await dbContext.SaveChangesAsync(stoppingToken);
        }

        /// <summary>
        /// Writes details about the most recent scan events to the console.
        /// </summary>
        private void WriteConsoleLastFetchedEventInfo(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ScanEventDbContext>();

            if (!dbContext.ParcelScanEvents.Any())
            {
                Console.WriteLine("No events found.");
                return;
            }

            var lastEvent = dbContext.ParcelScanEvents
                .OrderByDescending(e => e.EventId)
                .FirstOrDefault();

            if (lastEvent != null)
            {
                Console.WriteLine("Most Recent Scan Event:");
                Console.WriteLine($"EventId: {lastEvent.EventId}");
                Console.WriteLine($"ParcelId: {lastEvent.ParcelId}");
                Console.WriteLine($"Type: {lastEvent.Type}");
                Console.WriteLine($"CreatedDateTimeUtc: {lastEvent.CreatedDateTimeUtc}");
                Console.WriteLine($"StatusCode: {lastEvent.StatusCode}");
                Console.WriteLine($"RunId: {lastEvent.RunId}");
            }
            else
            {
                Console.WriteLine("No scan events found.");
            }

            var pickupEvent = dbContext.ParcelScanEvents
                .Where(e => e.Type == ScanEvent.PICKUP.ToString())
                .OrderByDescending(e => e.CreatedDateTimeUtc)
                .FirstOrDefault();

            if (pickupEvent != null)
            {
                Console.WriteLine("Most Recent PICKUP Event:");
                Console.WriteLine($"ParcelId: {pickupEvent.ParcelId}");
                Console.WriteLine($"CreatedDateTimeUtc: {pickupEvent.CreatedDateTimeUtc}");
            }
            else
            {
                Console.WriteLine("No PICKUP events found.");
            }

            var deliveryEvent = dbContext.ParcelScanEvents
                .Where(e => e.Type == ScanEvent.DELIVERY.ToString())
                .OrderByDescending(e => e.CreatedDateTimeUtc)
                .FirstOrDefault();

            if (deliveryEvent != null)
            {
                Console.WriteLine("Most Recent DELIVERY Event:");
                Console.WriteLine($"ParcelId: {deliveryEvent.ParcelId}");
                Console.WriteLine($"CreatedDateTimeUtc: {deliveryEvent.CreatedDateTimeUtc}");
            }
            else
            {
                Console.WriteLine("No DELIVERY events found.");
            }
        }
    }
}
