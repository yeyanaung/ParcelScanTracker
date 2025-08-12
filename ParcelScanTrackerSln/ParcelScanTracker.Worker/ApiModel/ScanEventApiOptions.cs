/// <summary>
/// Configuration options for the Scan Event API.
/// </summary>
public class ScanEventApiOptions
{
    /// <summary>
    /// The base URL of the Scan Event API.
    /// </summary>
    public string BaseUrl { get; set; }

    /// <summary>
    /// The maximum number of events to fetch in a single API call.
    /// </summary>
    public int Limit { get; set; }

    /// <summary>
    /// The delay (in milliseconds) between consecutive API polling attempts.
    /// </summary>
    public int PollingDelayMs { get; set; }
}