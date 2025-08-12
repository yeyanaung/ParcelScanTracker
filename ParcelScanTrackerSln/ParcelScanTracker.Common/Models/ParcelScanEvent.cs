using ParcelScanTracker.Common.EnumType;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParcelScanTracker.Common.Models
{
    /// <summary>
    /// Represents a scan event for a parcel, storing essential event details.
    /// </summary>
    [Table("ParcelScanEvent")]
    public class ParcelScanEvent
    {
        /// <summary>
        /// Auto-generated primary key.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Unique event ID from the source system.
        /// </summary>
        [Required]
        public int EventId { get; set; }

        /// <summary>
        /// Parcel identifier.
        /// </summary>
        [Required]
        public int ParcelId { get; set; }

        /// <summary>
        /// Scan Event type
        /// </summary>
        [MaxLength(50)]
        public string Type { get; set; } =  string.Empty;

        /// <summary>
        /// Event creation time (set by database).
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDateTimeUtc { get; set; }

        /// <summary>
        /// Status code for the event.
        /// </summary>
        [MaxLength(100)]
        public string StatusCode { get; set; } = string.Empty;

        /// <summary>
        /// Run identifier.
        /// </summary>
        [MaxLength(50)]
        public string RunId { get; set; } = string.Empty;

        /// <summary>
        /// Worker identifier (foreign key).
        /// </summary>
        [Required]
      //  [ForeignKey("WorkerInfo")]
        [MaxLength(10)]
        public string WorkerId { get; set; }
    }
}
