using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParcelScanTracker.Common.Models
{
    [Table("Device")]
    public class Device
    {
        [Key]
        public long DeviceTransactionId { get; set; }

        public long DeviceId { get; set; }

        //[Required]
        //public long EventId { get; set; } // Foreign key to ParcelScanEvent

        //[Required]
        //[MaxLength(10)]
        //public string WorkerId { get; set; } // Foreign key to WorkerInfo
    }
}
