using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParcelScanTracker.Common.Models
{
    /// <summary>
    /// Tracks the last fetched event ID for a specific worker.
    /// </summary>
    [Table("ScanEventState")]
    public class ScanEventState
    {
        /// <summary>
        /// Auto-generated primary key.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Worker identifier (foreign key).
        /// </summary>
        [Required]
        [MaxLength(10)]
        public string WorkerId { get; set; } = string.Empty;

        /// <summary>
        /// The last fetched event ID for this worker.
        /// </summary>
        [Required]
        public int LastEventId { get; set; }

    }
}
