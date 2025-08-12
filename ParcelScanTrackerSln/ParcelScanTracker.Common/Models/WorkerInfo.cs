using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParcelScanTracker.Common.Models
{
    /// <summary>
    /// Represents a scan event worker instance.
    /// </summary>
    [Table("WorkerInfo")]
    public class WorkerInfo
    {
        /// <summary>
        /// Unique identifier for the worker (primary key).
        /// </summary>
        [Key]
        [MaxLength(10)]
        public string WorkerId { get; set; }

        /// <summary>
        /// Name or description of the worker.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string WorkerName { get; set; }
    }
}
