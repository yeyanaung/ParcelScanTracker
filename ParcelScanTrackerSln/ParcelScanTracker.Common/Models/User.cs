using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParcelScanTracker.Common.Models
{
    [Table("User")]
    public class User
    {
        [Key]
        public string UserId { get; set; }

        [Required]
        public string CarrierId { get; set; }

        [Required]
        public string RunId { get; set; }

        [Required]
        public long EventId { get; set; } // Foreign key to ParcelScanEvent

        [Required]
        [MaxLength(10)]
        public string WorkerId { get; set; } // Foreign key to WorkerInfo
    }
}
