namespace ScanEventAPI.Models
{
    public class ScanEvent
    {
        public int EventId { get; set; }
        public int ParcelId { get; set; }
        public string Type { get; set; }
        public string CreatedDateTimeUtc { get; set; }
        public string StatusCode { get; set; }
        public Device Device { get; set; }
        public User User { get; set; }
    }
}
