using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParcelScanTracker.Common.Models
{
    /// <summary>
    /// Represents a raw scan event, storing the original event JSON and metadata.
    /// Maps to dbo.RawScanEvent.
    /// </summary>
    [Table("RawScanEvent")]
    public class RawScanEvent
    {
        /// <summary>
        /// Auto-generated primary key.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Unique event ID, matches ParcelScanEvent.EventId.
        /// </summary>
        [Required]
        public int EventId { get; set; }

        /// <summary>
        /// Worker identifier.
        /// </summary>
        [Required]
        [MaxLength(30)]
        public string WorkerId { get; set; } = string.Empty;

        /// <summary>
        /// Raw event JSON payload.
        /// </summary>
        [Required]
        public string RawJson { get; set; } = string.Empty;

        /// <summary>
        /// UTC time when event was ingested.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime IngestedAtUtc { get; set; }
    }
}

