public class JsonParcelScanEvent
{
    public int EventId { get; set; }
    public int ParcelId { get; set; }
    public string EventType { get; set; } // Use string to match JSON
    public DateTime CreatedDateTimeUtc { get; set; }
    public string StatusCode { get; set; }
    public string RunId { get; set; }
    public JsonDevice Device { get; set; }
    public JsonUser User { get; set; }
}

